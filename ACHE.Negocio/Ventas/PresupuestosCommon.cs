using System;
using System.Linq;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.Negocio;
using System.IO;
using System.Configuration;

namespace ACHE.Negocio.Presupuesto
{
    public class PresupuestosCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","


        public static int GuardarPresupuesto(PresupuestoCartDto comprobanteCart, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (comprobanteCart.IDPresupuesto != 0)
                        if (dbContext.Presupuestos.Any(x => x.IDUsuario == usu.IDUsuario && x.Numero == comprobanteCart.Numero && x.IDPresupuesto != comprobanteCart.IDPresupuesto))
                            throw new CustomException("El Numero ingresado ya se encuentra registrado.");                                     

                    Personas persona = dbContext.Personas.Where(x => x.IDPersona == comprobanteCart.IDPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (persona == null)
                        throw new CustomException("El cliente/proveedor es inexistente");

                    Presupuestos entity;
                    if (comprobanteCart.IDPresupuesto > 0)
                        entity = dbContext.Presupuestos.Where(x => x.IDPresupuesto == comprobanteCart.IDPresupuesto && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new Presupuestos();
                        entity.FechaAlta = DateTime.Now.Date;
                        entity.IDUsuario = usu.IDUsuario;
                        entity.Estado = "B";

                        var pre = dbContext.Presupuestos.Where(x => x.IDUsuario == usu.IDUsuario && x.Numero == comprobanteCart.Numero).FirstOrDefault();
                        if (pre != null)
                        {
                            var presupuesto = dbContext.Presupuestos.Where(x => x.IDUsuario == usu.IDUsuario);
                            if (presupuesto.Count() > 0)
                            {
                                var numero = presupuesto.Max(x => x.Numero);
                                numero++;
                                entity.Numero = numero;
                            }
                            else
                                entity.Numero = 1;
                        }
                        else
                        {
                            entity.Numero = comprobanteCart.Numero;
                        }

                    }

                    entity.IDPersona = comprobanteCart.IDPersona;
                    entity.FechaValidez = DateTime.Parse(comprobanteCart.Fecha);
                    entity.Nombre = comprobanteCart.Nombre.ToUpper();
                    entity.Estado = comprobanteCart.Estado;
                    entity.Descripcion = "";
                    
                    entity.FormaDePago = comprobanteCart.CondicionesPago;
                    entity.Observaciones = comprobanteCart.Observaciones;
                    entity.Vendedor = comprobanteCart.Vendedor;

                    if (entity.PresupuestoDetalle.Any())
                        dbContext.PresupuestoDetalle.RemoveRange(entity.PresupuestoDetalle);

                    entity.ImporteTotalBruto = 0;
                    entity.ImporteTotalNeto = 0;

                    if (comprobanteCart.Items.Count == 0)
                        throw new CustomException("Ingrese al menos un producto o servicio, para realizar el presupuesto.");
                    foreach (var det in comprobanteCart.Items)
                    {
                        PresupuestoDetalle presupuestoDet = new PresupuestoDetalle();
                        presupuestoDet.IDPresupuesto = entity.IDPresupuesto;
                        presupuestoDet.PrecioUnitario = det.PrecioUnitario;
                        presupuestoDet.Iva = det.Iva;
                        presupuestoDet.IdTipoIVA = det.IdTipoIva;
                        presupuestoDet.Concepto = det.Concepto;
                        presupuestoDet.Cantidad = det.Cantidad;
                        presupuestoDet.Bonificacion = det.Bonificacion;
                        presupuestoDet.IDConcepto = det.IDConcepto;

                        entity.ImporteTotalNeto += det.TotalConIva;
                        entity.ImporteTotalBruto += det.TotalSinIva;

                        entity.PresupuestoDetalle.Add(presupuestoDet);
                    }

                    if (comprobanteCart.IDPresupuesto > 0)
                        dbContext.SaveChanges();
                    else
                    {
                        dbContext.Presupuestos.Add(entity);
                        dbContext.SaveChanges();
                    }

                    return entity.IDPresupuesto;
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

        public static bool EliminarPresupuesto(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Presupuestos.Where(x => x.IDPresupuesto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        var archivo = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_R-" + entity.Numero.ToString("#00000000") + ".pdf";
                        dbContext.Presupuestos.Remove(entity);
                        dbContext.SaveChanges();

                        var path = Path.Combine(ConfigurationManager.AppSettings["PathBaseWeb"] + "~/files/Presupuestos/" + usu.IDUsuario.ToString() + "/" + archivo);
                        if (File.Exists(path))
                            File.Delete(path);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static ResultadosPresupuestosViewModel ObtenerPresupuesto(string condicion, string periodo, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Presupuestos.Include("Personas").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    Int32 numero = 0;
                    if (Int32.TryParse(condicion, out numero))
                        results = results.Where(x => x.Numero == numero);
                    else if (!string.IsNullOrWhiteSpace(condicion))
                        results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion));

                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(fechaDesde))
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaAlta >= dtDesde);
                    }
                    if (!string.IsNullOrWhiteSpace(fechaHasta))
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaAlta <= dtHasta);
                    }

                    page--;
                    ResultadosPresupuestosViewModel resultado = new ResultadosPresupuestosViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderByDescending(x => x.FechaAlta).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new PresupuestosViewModel()
                        {
                            ID = x.IDPresupuesto,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            Fecha = x.FechaAlta.ToString(formatoFecha),
                            Estado = x.Estado == "A" ? "Aprobado" : (x.Estado == "E" ? "Enviado" : "Borrador"),
                            Nombre = x.Nombre,
                            Numero = x.Numero.ToString("#00000000"),
                            Total = x.ImporteTotalNeto.ToString("N2"),
                            FechaValidez = x.FechaValidez.ToString(formatoFecha)
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}