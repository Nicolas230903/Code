using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using System.Configuration;
using System.IO;
using System.Web.Script.Services;

public partial class modulos_reportes_iva_saldo : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (var dbContext = new ACHEEntities())
        {
            AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

            if (afu != null)
                if (!afu.InfoImpositivosIVASaldo)
                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

        }
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public static List<Chart> obtenerIvaVentas()
    {
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        List<Chart> list = new List<Chart>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fechaDesde = "";
            string fechaHasta = "";

            using (var dbContext = new ACHEEntities())
            {
                fechaDesde = DateTime.Now.AddMonths(-11).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-11).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes12 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "1", data = (mes12.Any() ? mes12[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-10).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-10).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes11 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "2", data = (mes11.Any() ? mes11[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-9).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-9).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes10 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "3", data = (mes10.Any() ? mes10[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-8).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-8).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes9 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "4", data = (mes9.Any() ? mes9[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-7).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-7).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes8 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "5", data = (mes8.Any() ? mes8[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-6).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-6).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes7 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "6", data = (mes7.Any() ? mes7[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-5).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-5).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes6 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "7", data = (mes6.Any() ? mes6[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-4).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-4).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes5 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "8", data = (mes5.Any() ? mes5[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-3).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-3).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes4 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "9", data = (mes4.Any() ? mes4[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-2).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-2).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes3 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "10", data = (mes3.Any() ? mes3[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-1).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-1).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes2 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "11", data = (mes2.Any() ? mes2[0].data : 0) });

                fechaDesde = DateTime.Now.GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes1 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaVentas '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "12", data = (mes1.Any() ? mes1[0].data : 0) });

            }
        }

        return list;
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,UseHttpGet = true)]
    public static List<Chart> obtenerIvaCompras()
    {
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        List<Chart> list = new List<Chart>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fechaDesde = "";
            string fechaHasta = "";

            using (var dbContext = new ACHEEntities())
            {
                fechaDesde = DateTime.Now.AddMonths(-11).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-11).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes12 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "1", data = (mes12.Any() ? mes12[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-10).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-10).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes11 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "2", data = (mes11.Any() ? mes11[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-9).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-9).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes10 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "3", data = (mes10.Any() ? mes10[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-8).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-8).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes9 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "4", data = (mes9.Any() ? mes9[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-7).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-7).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes8 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "5", data = (mes8.Any() ? mes8[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-6).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-6).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes7 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "6", data = (mes7.Any() ? mes7[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-5).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-5).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes6 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "7", data = (mes6.Any() ? mes6[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-4).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-4).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes5 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "8", data = (mes5.Any() ? mes5[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-3).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-3).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes4 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "9", data = (mes4.Any() ? mes4[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-2).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-2).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes3 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "10", data = (mes3.Any() ? mes3[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-1).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-1).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes2 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "11", data = (mes2.Any() ? mes2[0].data : 0) });

                fechaDesde = DateTime.Now.GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes1 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_IvaCompras '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "12", data = (mes1.Any() ? mes1[0].data : 0) });

            }
        }

        return list;
    }


    [WebMethod(true)]
    public static string getDetail(int Periodo, string Etiqueta)
    {
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var html = "";
            var IngresosEgresos = ObtenerDetalle(Periodo, Etiqueta);

            if (IngresosEgresos.Any())
            {
                decimal total = 0;
                decimal totalIVA = 0;
                foreach (var detalle in IngresosEgresos)
                {
                    total += Convert.ToDecimal(detalle.Total);
                    totalIVA += Convert.ToDecimal(detalle.TotalIVA);


                    html += "<tr>";
                    html += "<td class='bgRow'>" + detalle.RazonSocial + "</td>";
                    html += "<td class='bgRow'>" + detalle.Comprobante + "</td>";
                    html += "<td class='bgRow'>" + detalle.Fecha + "</td>";
                    html += "<td class='bgRow'>" + detalle.Total + "</td>";
                    html += "<td class='bgRow'>" + detalle.TotalIVA + "</td>";
                    html += "</tr>";
                }

                html += "<tr>";
                html += "<td class='bgTotal' colspan='2'></td>";
                html += "<td class='bgTotal text-danger'>Total</td>";
                html += "<td class='bgTotal text-danger'>" + total + "</td>";

                html += "<td class='bgTotal text-danger'>" + totalIVA + "</td>";

                html += "</tr>";

            }
            else
                html += "<tr><td colspan='5'>No hay un detalle disponible</td></tr>";


            return html;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static List<RptIngresoEgresoViewModel> ObtenerDetalle(int Periodo, string Etiqueta)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        List<RptIngresoEgresoViewModel> IngresosEgresos = new List<RptIngresoEgresoViewModel>();
        RptIngresoEgresoViewModel det;
        using (var dbContext = new ACHEEntities())
        {

            var fechaDesde = DateTime.Now.AddMonths(Periodo - 11).GetFirstDayOfMonth();
            var fechaHasta = DateTime.Now.AddMonths(Periodo - 11).GetLastDayOfMonth();

            if (Etiqueta == "Venta")
            {
                var listIng = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo != "COT" && x.FechaComprobante >= fechaDesde && x.FechaComprobante <= fechaHasta).OrderBy(x => x.FechaComprobante).ToList();

                foreach (var item in listIng)
                {
                    det = new RptIngresoEgresoViewModel();
                    det.RazonSocial = item.Personas.RazonSocial;
                    det.Comprobante = item.Tipo + " " + item.PuntosDeVenta.Punto.ToString("#0000") + "-" + item.Numero.ToString("#00000000");
                    det.TipoComprobante = item.Tipo;
                    det.Fecha = item.FechaComprobante.ToString("dd/MM/yyyy");

                    if (det.TipoComprobante == "NCA" || det.TipoComprobante == "NCB" || det.TipoComprobante == "NCC")
                    {
                        det.Total = Convert.ToInt32(-item.ImporteTotalNeto).ToString("N2");
                        det.TotalIVA = Convert.ToInt32(item.ImporteTotalNeto - item.ImporteTotalBruto - item.ImporteNoGravado).ToString("N2");
                        det.TotalIVA = "-" + det.TotalIVA;
                    }
                    else
                    {
                        det.Total = Convert.ToInt32(item.ImporteTotalNeto).ToString("N2");
                        det.TotalIVA = Convert.ToInt32(item.ImporteTotalNeto - item.ImporteTotalBruto - item.ImporteNoGravado).ToString("N2");
                    }

                    IngresosEgresos.Add(det);
                }
            }
            else
            {
                var listEgr = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo != "COT" && x.Fecha >= fechaDesde && x.Fecha <= fechaHasta).OrderBy(x => x.Fecha).ToList();
                foreach (var item in listEgr)
                {
                    det = new RptIngresoEgresoViewModel();
                    det.RazonSocial = item.Personas.RazonSocial;
                    det.Comprobante = item.Tipo + " " + item.NroFactura;
                    det.Fecha = item.Fecha.ToString("dd/MM/yyyy");
                    det.TipoComprobante = item.Tipo;


                    if (det.TipoComprobante == "NCA" || det.TipoComprobante == "NCB" || det.TipoComprobante == "NCC")
                    {
                        det.Total = Convert.ToDecimal(-item.Total - item.Iva - item.TotalImpuestos).ToString("N2");
                        det.TotalIVA = Convert.ToInt32(item.Iva).ToString("N2");
                        det.TotalIVA = "-" + det.TotalIVA;
                    }
                    else
                    {
                        det.Total = Convert.ToDecimal(item.Total + item.Iva + item.TotalImpuestos).ToString("N2");
                        det.TotalIVA = Convert.ToInt32(item.Iva).ToString("N2");
                    }

                    IngresosEgresos.Add(det);
                }
            }
        }
        return IngresosEgresos;
    }

    [WebMethod(true)]
    public static string exportDetalle(int Periodo, string Etiqueta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string fileName = "Detalle" + Etiqueta;
            string path = "~/tmp/";
            try
            {
                var resultados = ObtenerDetalle(Periodo, Etiqueta);
                DataTable dt = new DataTable();

                dt = resultados.Select(x => new
                {
                    Comprobante = x.Comprobante,
                    Fecha = x.Fecha,
                    RazonSocial =x.RazonSocial,
                    Total = Convert.ToDecimal(x.Total),
                    TotalIVA = Convert.ToDecimal(x.TotalIVA),

                }).ToList().ToDataTable();

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");
            }
            catch (Exception e)
            {
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
                throw e;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}