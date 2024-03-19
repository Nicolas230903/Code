using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.ViewModels;
using System.IO;
using System.Configuration;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Common;
using System.Text.RegularExpressions;

namespace ACHE.Negocio.Productos
{
    public class ConceptosCommon
    {
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","
        #region ABM
        public static int GuardarConcepto(int id, string nombre, string codigo, string tipo, string descripcion, string estado, string precio, string iva, string stock, string obs, string constoInterno, string stockMinimo,int idPersona, int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.Conceptos.Any(x => x.IDUsuario == idUsuario && x.Codigo == codigo && x.IDConcepto != id && x.Codigo != ""))
                        throw new CustomException("El Código ingresado ya se encuentra registrado.");

                    Conceptos entity;
                    if (id > 0)
                        entity = dbContext.Conceptos.Where(x => x.IDConcepto == id && x.IDUsuario == idUsuario).FirstOrDefault();
                    else
                    {
                        entity = new Conceptos();
                        entity.FechaAlta = DateTime.Now;
                        entity.IDUsuario = idUsuario;
                    }
         
                    entity.Tipo = tipo;

                    if(codigo == "")
                        entity.Codigo = sugerirProximoCodigoConcepto(idUsuario);
                    else
                        entity.Codigo = codigo.ToUpper();
                    
                    entity.Nombre = nombre.ToUpper();
                    entity.Estado = estado.Trim();
                    entity.Descripcion = (string.IsNullOrWhiteSpace(descripcion)) ? "" : descripcion;
                    decimal stockAnterior = entity.Stock;
                    entity.Stock = decimal.Parse(stock.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                    entity.StockFisico = decimal.Parse(stock.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                    entity.PrecioUnitario = decimal.Parse(precio.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                    entity.IdTipoIVA = int.Parse(iva);
                    entity.Iva = dbContext.TipoIVA.Where(x => x.idTipoIVA == entity.IdTipoIVA).Select(s => s.ValorIVA).FirstOrDefault();
                    entity.CostoInterno = (!string.IsNullOrEmpty(constoInterno)) ? decimal.Parse(constoInterno.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                    entity.Observaciones = (string.IsNullOrWhiteSpace(obs)) ? "" : obs;
         
                    if (idPersona > 0)
                        entity.IDPersona = idPersona;
                    else
                        entity.IDPersona = null;

                    if (stockMinimo != string.Empty)
                        entity.StockMinimo = decimal.Parse(stockMinimo.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                    else
                        entity.StockMinimo = null;

                    if (id > 0)
                        dbContext.SaveChanges();
                    else
                    {
                        dbContext.Conceptos.Add(entity);
                        dbContext.SaveChanges();
                    }


                    StockAuditoria sa = new StockAuditoria();
                    sa.IdConcepto = entity.IDConcepto;
                    sa.idComprobante = 1;
                    sa.Accion = "Modificación en la sección de conceptos.";
                    sa.FechaAlta = DateTime.Now;
                    sa.IdUsuario = idUsuario;
                    if (decimal.Parse(stock.Replace(SeparadorDeMiles, SeparadorDeDecimales)) <= stockAnterior)
                        sa.Cantidad = stockAnterior - decimal.Parse(stock.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                    else
                        sa.Cantidad = decimal.Parse(stock.Replace(SeparadorDeMiles, SeparadorDeDecimales)) - stockAnterior;
                    sa.StockAnterior = stockAnterior;
                    sa.StockNuevo = decimal.Parse(stock.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                    dbContext.StockAuditoria.Add(sa);
                    dbContext.SaveChanges();

                    return entity.IDConcepto;
                }
            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string sugerirProximoCodigoConcepto(int idUsuario)
        {
            string nro = null;
            long cod, codigo = 1000;

            using (var dbContext = new ACHEEntities())
            {
                IEnumerable<Conceptos> lp = dbContext
                    .Conceptos
                    .Where(x => x.IDUsuario == idUsuario)
                    .Where(x => x.Codigo != "")
                    .ToList()
                    .AsEnumerable()
                    .Where(m => Regex.IsMatch(m.Codigo, @"^\d+$")); ;
                     

                nro = (from c in lp                       
                        orderby long.Parse(c.Codigo) descending
                        select c.Codigo).FirstOrDefault();

                if (nro != null)
                {
                    cod = Convert.ToInt64(nro);
                    if (cod < codigo)
                    {
                        cod = codigo;
                    }
                    else
                    {
                        cod = cod + 100;
                        cod = cod - (cod % 100);
                    }                        
                }
                else
                    cod = codigo;

                nro = cod.ToString();
            }

            return nro;
        }

        public static ResultadosProductosViewModel ObtenerConceptos(string condicion, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                if (!string.IsNullOrEmpty(condicion))
                    results = results.Where(x => x.Codigo.ToLower().Contains(condicion.ToLower()) || x.Nombre.ToLower().Contains(condicion.ToLower()));

                page--;
                ResultadosProductosViewModel resultado = new ResultadosProductosViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var listTipoIVA = dbContext.TipoIVA.ToList();

                var list = results.OrderByDescending(x => x.IDConcepto).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new ConceptosViewModel()
                    {
                        ID = x.IDConcepto,
                        Tipo = x.Tipo == "S" ? "Servicio" : "Producto",
                        Nombre = x.Nombre.ToUpper(),
                        Codigo = x.Codigo.ToUpper(),
                        Descripcion = x.Descripcion,
                        CodigoProveedor = x.IDPersona.ToString(),
                        Estado = x.Estado == "A" ? "Activo" : "Inactivo",
                        Precio = x.PrecioUnitario.ToString("N2"),
                        CostoInterno = ((decimal)x.CostoInterno).ToString("N2"),
                        Iva = listTipoIVA.Where(w => w.idTipoIVA == x.IdTipoIVA).Select(s => s.TipoIVA1).FirstOrDefault(),
                        Stock = x.Tipo == "S" ? "" : x.Stock.ToString()
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }
        public static bool EliminarConcepto(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.ComprobantesDetalle.Any(x => x.IDConcepto == id))
                    throw new CustomException("No se puede eliminar por tener comprobantes asociados");
                else if (dbContext.PreciosConceptos.Any(x => x.IDConceptos == id))
                    throw new CustomException("No se puede eliminar por estar en una lista de precio");
                else
                {
                    var entity = dbContext.Conceptos.Where(x => x.IDConcepto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        var entityStockAuditoria = dbContext.StockAuditoria.Where(x => x.IdConcepto == id && x.IdUsuario == usu.IDUsuario).ToList();
                        if (entityStockAuditoria != null)
                        {
                            dbContext.StockAuditoria.RemoveRange(entityStockAuditoria);
                            dbContext.SaveChanges();
                        }

                        var archivo = entity.Foto;
                        dbContext.Conceptos.Remove(entity);
                        dbContext.SaveChanges();
                        var path = Path.Combine(ConfigurationManager.AppSettings["PathBaseWeb"] + "/files/explorer/" + usu.IDUsuario.ToString() + "/Productos-Servicios/" + archivo);
                        if (File.Exists(path))
                            File.Delete(path);
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public static void AplicarRentabilidad(int id, decimal importeCompra, int idUsuario)
        {
            using (var dbContext = new ACHEEntities())
            {
                var usu = dbContext.Usuarios.Where(w => w.IDUsuario == idUsuario).FirstOrDefault();
                if (usu != null)
                {
                    var entity = dbContext.Conceptos.Where(x => x.IDConcepto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        if (entity.CostoInterno < importeCompra)
                        {
                            entity.CostoInterno = importeCompra;

                            if (entity.PrecioUnitario < entity.CostoInterno)
                                entity.PrecioUnitario = (decimal)entity.CostoInterno;

                            if (entity.PorcentajeRentabilidad != 0)
                            {
                                decimal porcentaje = (decimal)entity.CostoInterno * entity.PorcentajeRentabilidad;
                                decimal totalAumento = porcentaje / 100;
                                entity.PrecioUnitario = (decimal)entity.CostoInterno + totalAumento;
                            }
                            else
                            {
                                if (usu.PorcentajeRentabilidad != 0)
                                {
                                    decimal porcentaje = (decimal)entity.CostoInterno * usu.PorcentajeRentabilidad;
                                    decimal totalAumento = porcentaje / 100;
                                    entity.PrecioUnitario = (decimal)entity.CostoInterno + totalAumento;
                                }
                            }
                        }

                        dbContext.SaveChanges();
                    }
                }             
            }
        }

        #endregion
        public static void RestarStock(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalle)
        {
            foreach (var item in comprobanteDetalle)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                    c.Stock = (c.Stock - item.Cantidad);
            }
            
        }

        public static void RestarStockFisico(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalle)
        {
            foreach (var item in comprobanteDetalle)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                    c.StockFisico = (c.StockFisico - item.Cantidad);
            }

        }

        public static void SumarStock(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalle)
        {
            foreach (var item in comprobanteDetalle)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                {
                    c.Stock = (c.Stock + item.Cantidad);
                    c.StockFisico = (c.StockFisico + item.Cantidad);
                }
                    
            }
        }
        public static void RestarStockActualizado(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalleNuevo, int idComprobante)
        {
            var lista = (from com in dbContext.Comprobantes
                         join comDet in dbContext.ComprobantesDetalle on com.IDComprobante equals comDet.IDComprobante
                         join con in dbContext.Conceptos on comDet.IDConcepto equals con.IDConcepto
                         where com.IDComprobante == idComprobante
                         select new
                         {
                             con.IDConcepto,
                             comDet.Cantidad
                         }).ToList();


            decimal diferencia;
            foreach (var item in lista)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                {
                    ComprobantesDetalle cd = comprobanteDetalleNuevo.Where(w => w.IDConcepto == item.IDConcepto).FirstOrDefault();
                    if(cd != null)
                    {
                        if (item.Cantidad != cd.Cantidad)
                        {
                            if (item.Cantidad > cd.Cantidad)
                            {
                                diferencia = item.Cantidad - cd.Cantidad;
                                c.Stock = (c.Stock + diferencia);
                            }
                            else
                            {
                                diferencia = cd.Cantidad - item.Cantidad;
                                c.Stock = (c.Stock - diferencia);
                            }
                        }
                    }
                }                    
            }
        }

        public static void RestarStockFisicoActualizado(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalleNuevo, int idComprobante)
        {
            var lista = (from com in dbContext.Comprobantes
                         join comDet in dbContext.ComprobantesDetalle on com.IDComprobante equals comDet.IDComprobante
                         join con in dbContext.Conceptos on comDet.IDConcepto equals con.IDConcepto
                         where com.IDComprobante == idComprobante
                         select new
                         {
                             con.IDConcepto,
                             comDet.Cantidad
                         }).ToList();


            decimal diferencia;
            foreach (var item in lista)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                {
                    ComprobantesDetalle cd = comprobanteDetalleNuevo.Where(w => w.IDConcepto == item.IDConcepto).FirstOrDefault();
                    if (cd != null)
                    {
                        if (item.Cantidad != cd.Cantidad)
                        {
                            if (item.Cantidad > cd.Cantidad)
                            {
                                diferencia = item.Cantidad - cd.Cantidad;
                                c.StockFisico = (c.StockFisico + diferencia);
                            }
                            else
                            {
                                diferencia = cd.Cantidad - item.Cantidad;
                                c.StockFisico = (c.StockFisico - diferencia);
                            }
                        }
                    }
                }
            }
        }
        public static void SumarStockActualizado(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalleNuevo, int idComprobante)
        {

            var lista = (from com in dbContext.Comprobantes
                        join comDet in dbContext.ComprobantesDetalle on com.IDComprobante equals comDet.IDComprobante
                        join con in dbContext.Conceptos on comDet.IDConcepto equals con.IDConcepto
                         where com.IDComprobante == idComprobante
                         select new
                        {
                            con.IDConcepto,
                            comDet.Cantidad
                        }).ToList();


            decimal diferencia;
            foreach (var item in lista)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                {
                    ComprobantesDetalle cd = comprobanteDetalleNuevo.Where(w => w.IDConcepto == item.IDConcepto).FirstOrDefault();
                    if (cd != null)
                    {
                        if (item.Cantidad != cd.Cantidad)
                        {
                            if (item.Cantidad > cd.Cantidad)
                            {
                                diferencia = item.Cantidad - cd.Cantidad;
                                c.Stock = (c.Stock - diferencia);
                                c.StockFisico = (c.StockFisico - diferencia);
                            }
                            else
                            {
                                diferencia = cd.Cantidad - item.Cantidad;
                                c.Stock = (c.Stock + diferencia);
                                c.StockFisico = (c.StockFisico + diferencia);
                            }
                        }
                    }
                }
            }
        }
        public static decimal ObtenerPrecioFinal(decimal precioUnitario, string iva)
        {
            decimal precioFinal = precioUnitario;
            var auxIVA = "1";

            switch (iva)
            {
                case "0,00":
                    auxIVA = "1";
                    break;
                case "2,50":
                    auxIVA = "1,025";
                    break;
                case "5,00":
                    auxIVA = "1,050";
                    break;
                case "10,50":
                    auxIVA = "1,105";
                    break;
                case "21,00":
                    auxIVA = "1,210";
                    break;
                case "27,00":
                    auxIVA = "1,270";
                    break;
            }

            decimal IVA = decimal.Parse(auxIVA);
            precioFinal = precioUnitario / IVA;

            return Math.Round(precioFinal, 2);
        }
    }
}