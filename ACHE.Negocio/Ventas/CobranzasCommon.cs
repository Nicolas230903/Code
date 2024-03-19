using System;
using System.Linq;
using ACHE.Model;
using ACHE.Model.Negocio;
using ACHE.Negocio.Contabilidad;
using System.IO;
using ACHE.Extensions;
using System.Configuration;
using System.Collections.Generic;
using ACHE.Negocio.Banco;

namespace ACHE.Negocio.Facturacion
{
    public static class CobranzasCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static Cobranzas Guardar(ACHEEntities dbContext, CobranzaCartDto cobranza, WebUser usu)
        {
            if (ContabilidadCommon.ValidarCierreContable(usu, Convert.ToDateTime(cobranza.Fecha)))
                throw new CustomException("No puede agregar ni modificar una cobranza que se encuentre en un periodo cerrado.");

            Cobranzas entity;
            if (cobranza.IDCobranza > 0)
                entity = dbContext.Cobranzas
                    .Include("CobranzasDetalle").Include("CobranzasFormasDePago").Include("CobranzasRetenciones").Include("CobranzasFormasDePago.Comprobantes")
                    .Where(x => x.IDCobranza == cobranza.IDCobranza && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            else
            {
                entity = new Cobranzas();
                entity.FechaAlta = DateTime.Now;
                entity.IDUsuario = usu.IDUsuario;
            }

            Personas persona = dbContext.Personas.Where(x => x.IDPersona == cobranza.IDPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (persona == null)
                throw new CustomException("El cliente/proveedor es inexistente");

            entity.IDPersona = cobranza.IDPersona;
            entity.Tipo = cobranza.Tipo;
            entity.Modo = "T";
            entity.FechaCobranza = DateTime.Parse(cobranza.Fecha);
            entity.TipoDestinatario = persona.Tipo.ToUpper();
            entity.NroDocumento = persona.NroDocumento;
            entity.TipoDocumento = persona.TipoDocumento;
            entity.EstadoCaja = "Cargado";
            entity.EstadoCajaFecha = DateTime.Now;

            entity.IDPuntoVenta = cobranza.IDPuntoVenta;
            if (cobranza.NumeroCobranza != "")
            {
                entity.Numero = int.Parse(cobranza.NumeroCobranza);
                entity.FechaProceso = DateTime.Now;
            }
            entity.Observaciones = cobranza.Observaciones;

            #region Detalle, Formas y Retenciones

            if (entity.CobranzasDetalle.Any())
                dbContext.CobranzasDetalle.RemoveRange(entity.CobranzasDetalle);

            decimal total = 0;
            foreach (var det in cobranza.Items)
            {
                //CobranzasDetalle compDet = new CobranzasDetalle();
                //compDet.IDComprobante = det.IDComprobante;
                //compDet.IDCobranza = entity.IDCobranza;
                //compDet.Importe = det.Importe;

                total += det.Total;
                //entity.CobranzasDetalle.Add(compDet);
            }

            entity.ImporteTotal = Math.Round(total, 2);

            if (entity.CobranzasFormasDePago.Any())
            {
                var nota = entity.CobranzasFormasDePago.Where(x => x.IDNotaCredito != null && x.IDCobranza == cobranza.IDCobranza).FirstOrDefault();
                if (nota != null)
                    nota.Comprobantes.Saldo = nota.Importe;

                dbContext.CobranzasFormasDePago.RemoveRange(entity.CobranzasFormasDePago);
            }
            var tieneNC = false;
            decimal totalFormasDePago = 0;
            foreach (var det in cobranza.FormasDePago)
            {
                int IdBanco = 0;

                if (det.IDCheque != null)
                {
                    IdBanco = (from cheq in dbContext.Cheques
                                   join banco in dbContext.Bancos on cheq.IDBanco equals banco.IDBancoBase
                                   where cheq.IDCheque == det.IDCheque
                                   select banco.IDBanco).FirstOrDefault();

                    if (IdBanco == 0)
                    {
                        var cheque = dbContext.Cheques.Where(x => x.IDCheque == det.IDCheque).FirstOrDefault();
                        IdBanco = BancosCommon.GuardarBanco(0, (int)cheque.IDBanco, "", "Pesos Argentinos", 1, "", "", "", "", "", "", usu);
                    }
                }

                CobranzasFormasDePago compForm = new CobranzasFormasDePago();
                compForm.IDCobranza = entity.IDCobranza;
                compForm.Importe = det.Importe;
                compForm.FormaDePago = det.FormaDePago;
                compForm.NroReferencia = det.NroReferencia;
                compForm.IDCheque = det.IDCheque;
                if (IdBanco != 0)
                    compForm.IDBanco = IdBanco;
                compForm.IDNotaCredito = det.IDNotaCredito;
                totalFormasDePago += det.Importe;
                entity.CobranzasFormasDePago.Add(compForm);

                var cheques = dbContext.Cheques.Where(x => x.IDCheque == det.IDCheque).FirstOrDefault();
                if (cheques != null)
                    cheques.Estado = "Libre";

                if (compForm.IDNotaCredito != null)
                    tieneNC = true;
            }

            decimal restoTotalFormasDePago = totalFormasDePago;
            foreach (var det in cobranza.Items.OrderByDescending(o => o.ID))
            {
                CobranzasDetalle compDet = new CobranzasDetalle();
                compDet.IDComprobante = det.IDComprobante;
                compDet.IDCobranza = entity.IDCobranza;
                if (restoTotalFormasDePago >= det.Importe)
                {
                    compDet.Importe = det.Importe;
                    restoTotalFormasDePago = restoTotalFormasDePago - det.Importe;
                }
                else
                {
                    compDet.Importe = restoTotalFormasDePago;
                    restoTotalFormasDePago = 0;
                }                             

                entity.CobranzasDetalle.Add(compDet);
            }

            if (entity.CobranzasRetenciones.Any())
                dbContext.CobranzasRetenciones.RemoveRange(entity.CobranzasRetenciones);

            foreach (var det in cobranza.Retenciones)
            {
                CobranzasRetenciones compRet = new CobranzasRetenciones();
                compRet.IDCobranza = entity.IDCobranza;
                compRet.Importe = det.Importe;
                compRet.Tipo = det.Tipo;
                compRet.NroReferencia = det.NroReferencia;

                entity.CobranzasRetenciones.Add(compRet);
            }

            #endregion

            if (total == 0 && !tieneNC)
                throw new CustomException("No puede generar un recibo con Importe 0");

            //if (totalFormasDePago != total)
            //    throw new CustomException("Las formas de cobro deben coincidir");

            if (cobranza.IDCobranza > 0)
            {
                dbContext.SaveChanges();
            }
            else
            {
                dbContext.Cobranzas.Add(entity);
                dbContext.SaveChanges();
            }

            dbContext.ActualizarSaldosPorCobranza(entity.IDCobranza);

            ContabilidadCommon.AgregarAsientoDeCobranza(usu, entity.IDCobranza);
            return entity;
        }

        public static Cobranzas Guardar(CobranzaCartDto cobrCartdto, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                return Guardar(dbContext, cobrCartdto, usu);
            }
        }

        public static string obtenerProxNroCobranza(string tipo, int idUsuario)
        {
            try
            {
                var nro = "";
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.Cobranzas.Any(x => x.IDUsuario == idUsuario && x.Tipo == tipo))
                    {
                        var aux = dbContext.Cobranzas.Where(x => x.IDUsuario == idUsuario && x.Tipo == tipo).ToList();
                        if (aux.Count() > 0)
                            nro = (aux.Max(x => x.Numero) + 1).ToString("#00000000");
                        else
                            nro = "00000001";
                    }
                    else
                        nro = "00000001";
                }

                return nro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool EliminarCobranza(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Cobranzas.Where(x => x.IDCobranza == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        if (entity.FechaCAE.HasValue)
                            throw new CustomException("No se puede eliminar por estar informado a la AFIP");
                        else if (ContabilidadCommon.ValidarCierreContable(usu, entity.FechaCobranza))
                            throw new CustomException("El comprobante no puede eliminarse ya que el año contable ya fue cerrado.");

                        int idPersona = entity.IDPersona;
                        var archivo = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_RC-" + entity.Numero.ToString("#00000000") + ".pdf";
                        var fecha = entity.FechaAlta.Year.ToString();
                        dbContext.Database.ExecuteSqlCommand("DELETE Asientos WHERE IDCobranza=" + id);

                        dbContext.Cobranzas.Remove(entity);
                        dbContext.SaveChanges();

                        dbContext.ActualizarSaldosPorPersona(idPersona);
                        var path = Path.Combine(ConfigurationManager.AppSettings["PathBaseWeb"] + "/files/explorer/" + usu.IDUsuario.ToString() + "/Comprobantes/" + fecha + "/" + archivo);
                        if (File.Exists(path))
                            File.Delete(path);

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

        public static ResultadosComprobantesViewModel ObtenerCobranzas(string condicion, string periodo, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Cobranzas.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                Int32 numero = 0;
                if (Int32.TryParse(condicion, out numero))
                    results = results.Where(x => x.Numero == numero);
                else if (!string.IsNullOrWhiteSpace(condicion))
                    results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion));

                switch (periodo)
                {
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
                    default:
                    case "30":
                        fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                        break;
                }

                if (!periodo.Equals("-2"))
                {
                    if (!string.IsNullOrWhiteSpace(fechaDesde))
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaCobranza >= dtDesde);
                    }
                    if (!string.IsNullOrWhiteSpace(fechaHasta))
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaCobranza <= dtHasta);
                    }
                }

                page--;
                ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var list = results.OrderByDescending(x => x.FechaCobranza).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new ComprobantesViewModel()
                    {
                        ID = x.IDCobranza,
                        RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                        Fecha = x.FechaCobranza.ToString(formatoFecha),
                        Tipo = x.Tipo == "SIN" ? "-" : "RC",
                        Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                        ImporteTotalNeto = x.ImporteTotal.ToString("N2"),
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }


        public static ResultadosComprobantesViewModel ObtenerCobranzasVinculadas(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                List<int?> listaEda = dbContext.Comprobantes.Where(x => x.IdComprobanteVinculado == id && x.Tipo.Equals("EDA"))
                    .Select(a => (int?)a.IDComprobante)
                    .ToList();

                var comp = dbContext.Comprobantes.Where(x => x.IDComprobante == id || x.IdComprobanteVinculado == id || listaEda.Contains(x.IdComprobanteVinculado)).AsQueryable();

                var results = from c in dbContext.Cobranzas
                              join cd in dbContext.CobranzasDetalle on c.IDCobranza equals cd.IDCobranza
                              join p in dbContext.Personas on c.IDPersona equals p.IDPersona
                              join pdv in dbContext.PuntosDeVenta on c.IDPuntoVenta equals pdv.IDPuntoVenta
                              join com in comp on cd.IDComprobante equals com.IDComprobante
                              select new {
                                  c.IDCobranza,
                                  p.NombreFantansia,
                                  comOrigenPunto = com.PuntosDeVenta.Punto,
                                  comOrigenNumero = com.Numero,
                                  p.RazonSocial,
                                  c.FechaCobranza,
                                  c.Tipo,
                                  pdv.Punto,
                                  c.Numero,
                                  c.ImporteTotal
                              };

                var listaMontos = dbContext.CobranzasFormasDePago.GroupBy(c => c.IDCobranza).
                  Select(g => new
                  {
                      g.Key,
                      SUM = g.Sum(s => s.Importe)
                  });

                ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                var list = results.OrderBy(x => x.FechaCobranza).ToList()
                    .Select(x => new ComprobantesViewModel()
                    {
                        ID = x.IDCobranza,
                        RazonSocial = (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper()),
                        ComprobanteOrigen = x.comOrigenPunto.ToString("#0000") + "-" + x.comOrigenNumero.ToString("#00000000"),
                        Fecha = x.FechaCobranza.ToString(formatoFecha),
                        Tipo = x.Tipo == "SIN" ? "-" : "RC",
                        Numero = x.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                        ImporteTotalNeto = listaMontos.Where(w => w.Key == x.IDCobranza).Select(s => s.SUM).DefaultIfEmpty(0).FirstOrDefault().ToString("N2")
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }

        public static ResultadosComprobantesViewModel ObtenerChequesVinculados(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var comp = dbContext.Comprobantes.Where(x => x.IDComprobante == id || x.IdComprobanteVinculado == id).AsQueryable();

                var results = from c in dbContext.Cobranzas
                              join cd in dbContext.CobranzasDetalle on c.IDCobranza equals cd.IDCobranza
                              join cfdp in dbContext.CobranzasFormasDePago on c.IDCobranza equals cfdp.IDCobranza
                              join p in dbContext.Personas on c.IDPersona equals p.IDPersona
                              join pdv in dbContext.PuntosDeVenta on c.IDPuntoVenta equals pdv.IDPuntoVenta
                              join com in comp on cd.IDComprobante equals com.IDComprobante
                              join cheq in dbContext.Cheques on cfdp.IDCheque equals cheq.IDCheque
                              select new
                              {
                                  c.IDCobranza,
                                  p.NombreFantansia,
                                  comOrigenPunto = com.PuntosDeVenta.Punto,
                                  comOrigenNumero = com.Numero,
                                  p.RazonSocial,
                                  c.FechaCobranza,
                                  c.Tipo,
                                  pdv.Punto,
                                  c.Numero,
                                  c.ImporteTotal,
                                  cheq
                              };

                ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                var list = results.OrderBy(x => x.FechaCobranza).ToList()
                    .Select(x => new ComprobantesViewModel()
                    {
                        ID = x.IDCobranza,
                        ComprobanteOrigen = x.comOrigenPunto.ToString("#0000") + "-" + x.comOrigenNumero.ToString("#00000000"),
                        Fecha = x.cheq.FechaEmision.ToString(formatoFecha),
                        Numero = x.cheq.Numero,
                        ImporteTotalNeto = x.cheq.Importe.ToString("N2")
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }

    }
}