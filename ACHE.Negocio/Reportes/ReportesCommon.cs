using ACHE.Model;
using ACHE.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ACHE.Negocio.Reportes
{
    public class ReportesCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","


        #region PERCEPCIONES
        public static ResultadosPercepcionesViewModel ObtenerPercepcionesEmitidas(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu, string impuesto)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.PercepcionIIBB > 0).AsQueryable();
                if (idPersona > 0)
                    results = results.Where(x => x.IDPersona == idPersona);
                if (fechaDesde != string.Empty)
                {
                    DateTime dtDesde = DateTime.Parse(fechaDesde);
                    results = results.Where(x => x.FechaComprobante >= dtDesde);
                }
                if (fechaHasta != string.Empty)
                {
                    DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                    results = results.Where(x => x.FechaComprobante <= dtHasta);
                }

                page--;
                ResultadosPercepcionesViewModel resultado = new ResultadosPercepcionesViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                List<PercepcionesViewModel> list = new List<PercepcionesViewModel>();


                switch (impuesto)
                {
                    case "IIBB":
                        list = results.OrderBy(x => x.FechaComprobante).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new PercepcionesViewModel()
                        {
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            RazonSocial = x.Personas.RazonSocial,
                            Cuit = x.Personas.NroDocumento,
                            CondicionIVA = x.Personas.CondicionIva,
                            NroComprobante = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            Jurisdiccion = (x.Provincias == null) ? "" : x.Provincias.Nombre,
                            Importe = Math.Round(((x.ImporteTotalBruto * x.PercepcionIIBB) / 100), 2).ToString("N2")
                        }).ToList();
                        break;
                    case "IVA":
                        list = results.OrderBy(x => x.FechaComprobante).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new PercepcionesViewModel()
                        {
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            RazonSocial = x.Personas.RazonSocial,
                            Cuit = x.Personas.NroDocumento,
                            CondicionIVA = x.Personas.CondicionIva,
                            NroComprobante = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            Importe = Math.Round(((x.ImporteTotalBruto * x.PercepcionIVA) / 100), 2).ToString("N2")
                        }).ToList();
                        break;
                }
                resultado.Items = list;
                return resultado;
            }
        }
        public static ResultadosPercepcionesViewModel ObtenerPercepcionesSufridas(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Jurisdicciones.Where(x => x.Compras.IDUsuario == usu.IDUsuario).AsQueryable();
                if (idPersona > 0)
                    results = results.Where(x => x.Compras.IDPersona == idPersona);
                if (fechaDesde != string.Empty)
                {
                    DateTime dtDesde = DateTime.Parse(fechaDesde);
                    results = results.Where(x => x.Compras.FechaEmision >= dtDesde);
                }
                if (fechaHasta != string.Empty)
                {
                    DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                    results = results.Where(x => x.Compras.FechaEmision <= dtHasta);
                }

                page--;
                ResultadosPercepcionesViewModel resultado = new ResultadosPercepcionesViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var list = results.OrderBy(x => x.Compras.FechaEmision).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new PercepcionesViewModel()
                    {
                        Fecha = x.Compras.FechaEmision.ToString(formatoFecha),
                        RazonSocial = x.Compras.Personas.RazonSocial,
                        Cuit = x.Compras.Personas.NroDocumento,
                        CondicionIVA = x.Compras.Personas.CondicionIva,
                        NroComprobante = x.Compras.Tipo + " " + x.Compras.NroFactura,
                        Jurisdiccion = (x.Provincias == null) ? "" : x.Provincias.Nombre,
                        Importe = x.Importe.ToString("N2")
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }
        public static ResultadosPercepcionesViewModel ObtenerPercepcionesSufridasIVA(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.PercepcionIVA > 0).AsQueryable();
                if (idPersona > 0)
                    results = results.Where(x => x.IDPersona == idPersona);
                if (fechaDesde != string.Empty)
                {
                    DateTime dtDesde = DateTime.Parse(fechaDesde);
                    results = results.Where(x => x.FechaEmision >= dtDesde);
                }
                if (fechaHasta != string.Empty)
                {
                    DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                    results = results.Where(x => x.FechaEmision <= dtHasta);
                }

                page--;
                ResultadosPercepcionesViewModel resultado = new ResultadosPercepcionesViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var list = results.OrderBy(x => x.FechaEmision).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new PercepcionesViewModel()
                    {
                        Fecha = x.FechaEmision.ToString(formatoFecha),
                        RazonSocial = x.Personas.RazonSocial,
                        Cuit = x.Personas.NroDocumento,
                        CondicionIVA = x.Personas.CondicionIva,
                        NroComprobante = x.Tipo + " " + x.NroFactura,
                        Importe = x.PercepcionIVA.ToString("N2")
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }
        #endregion

        #region DETALLE BANCARIO
        public static ResultadosDetalleBancarioViewModel ObtenerDetalleBancario(int idBanco, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                string sql = "select * from RptBancarioView where IDUsuario =" + usu.IDUsuario;
                if (idBanco > 0)
                    sql += " and IDBanco =" + idBanco;
                if (!string.IsNullOrWhiteSpace(fechaDesde))
                    sql += " and fecha >='" + DateTime.Parse(fechaDesde).Date.ToString(formatoFecha) + "'";
                if (!string.IsNullOrWhiteSpace(fechaHasta))
                    sql += " and fecha <='" + DateTime.Parse(fechaHasta).Date.ToString(formatoFecha) + "'";

                var results = dbContext.Database.SqlQuery<RptBancarioView>(sql, new object[] { }).ToList();

                page--;
                ResultadosDetalleBancarioViewModel resultado = new ResultadosDetalleBancarioViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var list = results.OrderBy(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new DetalleBancarioViewModel()
                    {
                        Fecha = x.Fecha.ToString(formatoFecha),
                        Nombre = x.Nombre,
                        Tipo = x.Tipo,
                        FormaDePago = x.FormaDePago,
                        NroReferencia = x.NroReferencia,
                        TipoMovimiento = x.TipoMovimiento,
                        Comprobante = (x.Tipo == "COBRANZA" || x.Tipo == "PAGOS") ? x.TipoComprobante + " " + x.NumeroComprobante : "",
                        Importe = (x.TipoMovimiento == "Ingreso") ? Convert.ToDecimal(x.Importe).ToString() : (Convert.ToDecimal(x.Importe) * -1).ToString("N2")
                    });
                resultado.Items = list.ToList();
                return resultado;
            }
        }
        #endregion
    }
}
