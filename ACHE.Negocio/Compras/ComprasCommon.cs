using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Contabilidad;
using Org.BouncyCastle.Utilities.Encoders;

namespace ACHE.Negocio.Facturacion
{
    public static class ComprasCommon 
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static Compras Guardar(int id, int idPersona, string fecha, string nroFactura,
        string iva, string importe2, string importe5, string importe10, string importe21, string importe27, string noGrav, string importeMon,
        string impNacional, string impMunicipal, string impInterno, string percepcionIva, string otros,
        string obs, string tipo, string idCategoria, string rubro, string exento, string FechaEmision, int idPlanDeCuenta, int idUsuario, List<JurisdiccionesViewModel> Jurisdicciones, string fechaPrimerVencimiento, string fechaSegundoVencimiento, string adjunto)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();

                    bool CompraExistente = false;
                    Compras entity;
                    if (id > 0)
                    {
                        entity = dbContext.Compras.Where(x => x.IDCompra == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        if (entity.Tipo.Equals("CDC"))
                        {
                            if (entity.IdComprobante != null)
                            {
                                throw new CustomException("Ya se generó un pedido de compra vinculado. No se puede modificar el comprobante de compra.");
                            }
                        }

                    }
                    else
                    {
                        CompraExistente = dbContext.Compras.Any(x => x.IDPersona == idPersona && x.Tipo == tipo && x.NroFactura == nroFactura && x.IDUsuario == usu.IDUsuario);
                        entity = new Compras();
                        entity.FechaAlta = DateTime.Now.Date;
                        entity.IDUsuario = usu.IDUsuario;
                    }

                    if (CompraExistente)
                        throw new Exception("Ya existe el tipo de comprobante y nro comprobante para para el proveedor/Cliente seleccionado");


                    Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (persona == null)
                        throw new Exception("El cliente/proveedor es inexistente");

                    entity.IDPersona = idPersona;
                    entity.Tipo = tipo;
                    entity.Fecha = DateTime.Parse(fecha);
                    entity.FechaEmision = DateTime.Parse(FechaEmision);
                    entity.FechaPrimerVencimiento = DateTime.Parse(fechaPrimerVencimiento);
                    if (!string.IsNullOrWhiteSpace(fechaSegundoVencimiento))
                        entity.FechaSegundoVencimiento = DateTime.Parse(fechaSegundoVencimiento);
                    else
                        entity.FechaSegundoVencimiento = null;

                    entity.NroFactura = nroFactura;
                    entity.Iva = iva != string.Empty ? decimal.Parse(iva.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Importe2 = importe2 != string.Empty ? decimal.Parse(importe2.Replace(".", "").Replace(".",",")) : 0;
                    entity.Importe5 = importe5 != string.Empty ? decimal.Parse(importe5.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Importe5 = importe5 != string.Empty ? decimal.Parse(importe5.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Importe10 = importe10 != string.Empty ? decimal.Parse(importe10.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Importe21 = importe21 != string.Empty ? decimal.Parse(importe21.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Importe27 = importe27 != string.Empty ? decimal.Parse(importe27.Replace(".", "").Replace(".", ",")) : 0;
                    entity.NoGravado = noGrav != string.Empty ? decimal.Parse(noGrav.Replace(".", "").Replace(".", ",")) : 0;
                    entity.ImporteMon = importeMon != string.Empty ? decimal.Parse(importeMon.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Exento = (exento != string.Empty) ? decimal.Parse(exento.Replace(".", "").Replace(".", ",")) : 0;

                    //IMPUESTOS
                    entity.ImpNacional = impNacional != string.Empty ? decimal.Parse(impNacional.Replace(".", "").Replace(".", ",")) : 0;
                    entity.ImpMunicipal = impMunicipal != string.Empty ? decimal.Parse(impMunicipal.Replace(".", "").Replace(".", ",")) : 0;
                    entity.ImpInterno = impInterno != string.Empty ? decimal.Parse(impInterno.Replace(".", "").Replace(".", ",")) : 0;
                    entity.PercepcionIVA = percepcionIva != string.Empty ? decimal.Parse(percepcionIva.Replace(".", "").Replace(".", ",")) : 0;
                    entity.Otros = otros != string.Empty ? decimal.Parse(otros.Replace(".", "").Replace(".", ",")) : 0;

                    if (Jurisdicciones != null)
                    {
                        if (id > 0)
                        {
                            var jurisdiccion = dbContext.Jurisdicciones.Where(x => x.IDCompra == id).ToList();
                            foreach (var item in jurisdiccion)
                                dbContext.Jurisdicciones.Remove(item);
                        }
                        foreach (var item in Jurisdicciones)
                        {
                            entity.Jurisdicciones.Add(new Jurisdicciones()
                            {
                                Compras = entity,
                                IDProvincia = item.IDJurisdicion,
                                Importe = item.Importe
                            });
                        }
                    }

                    entity.IIBB = (entity.Jurisdicciones.Count > 0) ? entity.Jurisdicciones.Sum(x => x.Importe) : 0;

                    // cta contables
                    if (idPlanDeCuenta > 0)
                        entity.IDPlanDeCuenta = idPlanDeCuenta;

                    entity.Observaciones = obs;

                    if (idCategoria != "" && idCategoria != "0")
                        entity.IDCategoria = int.Parse(idCategoria);
                    else
                        entity.IDCategoria = null;
                    entity.Rubro = rubro;

                    if (adjunto != null)
                        entity.Adjunto = Base64.Decode(adjunto);

                    if (id > 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.Compras.Add(entity);
                        dbContext.SaveChanges();
                    }

                    dbContext.ActualizarSaldosPorCompra(Convert.ToInt32(entity.IDCompra));
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool EliminarCompra(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.PagosDetalle.Any(x => x.IDCompra == id))
                        throw new CustomException("No se puede eliminar por tener pagos asociados");
                    else if (dbContext.Activos.Any(x => x.IDCompra == id))
                        throw new CustomException("No se puede eliminar por tener Activos asociados");

                    var entity = dbContext.Compras.Where(x => x.IDCompra == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        if (ContabilidadCommon.ValidarCierreContable(usu, entity.Fecha))
                            throw new CustomException("El comprobante no puede eliminarse ya que el año contable ya fue cerrado.");

                        var jurisdicciones = dbContext.Jurisdicciones.Where(x => x.IDCompra == id).ToList();
                        foreach (var item in jurisdicciones)
                            dbContext.Jurisdicciones.Remove(item);

                        dbContext.Compras.Remove(entity);
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                        return false;
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

        public static ResultadosComprasViewModel ObtenerCompras(string condicion, string periodo, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Compras.Include("Personas").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    Int32 numero = 0;
                    if (Int32.TryParse(condicion, out numero))
                        results = results.Where(x => x.NroFactura.Contains(condicion));
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

                    if (!periodo.Equals("-2"))
                    {
                        if (!string.IsNullOrWhiteSpace(fechaDesde))
                        {
                            DateTime dtDesde = DateTime.Parse(fechaDesde);
                            results = results.Where(x => x.Fecha >= dtDesde);
                        }
                        if (!string.IsNullOrWhiteSpace(fechaHasta))
                        {
                            DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                            results = results.Where(x => x.Fecha <= dtHasta);
                        }
                    }


                    page--;
                    ResultadosComprasViewModel resultado = new ResultadosComprasViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderByDescending(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new ComprasViewModel()
                        {
                            ID = x.IDCompra,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            Fecha = x.Fecha.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            NroFactura = x.NroFactura,

                            Iva = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? (x.Iva * -1).ToString("N2") : x.Iva.ToString("N2"),
                            NoGravado = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? (-1 * (x.NoGravado + x.Exento)).ToString("N2") : (x.NoGravado + x.Exento).ToString("N2"),
                            Retenciones = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? (-1 * x.TotalImpuestos.Value).ToString("N2") : x.TotalImpuestos.Value.ToString("N2"),
                            Importe = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? ("-" + x.Total.Value.ToString("N2")) : x.Total.Value.ToString("N2"),
                            Total = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? ("-" + Convert.ToDecimal(x.TotalImpuestos + x.Total + x.Iva).ToString("N2")) : Convert.ToDecimal(x.TotalImpuestos + x.Total + x.Iva).ToString("N2"),
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
