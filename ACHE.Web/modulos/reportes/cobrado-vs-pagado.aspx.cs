﻿using ACHE.Extensions;
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

public partial class modulos_reportes_cobrado_vs_pagado : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (var dbContext = new ACHEEntities())
        {
            AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

            if (afu != null)
                if (!afu.InfoGananciasVsPerdidasCobradoVsPagado)
                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

        }
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public static List<Chart> obtenerCobrado()
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
                var mes12 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "1", data = (mes12.Any() ? mes12[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-10).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-10).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes11 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "2", data = (mes11.Any() ? mes11[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-9).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-9).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes10 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "3", data = (mes10.Any() ? mes10[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-8).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-8).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes9 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "4", data = (mes9.Any() ? mes9[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-7).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-7).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes8 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "5", data = (mes8.Any() ? mes8[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-6).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-6).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes7 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "6", data = (mes7.Any() ? mes7[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-5).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-5).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes6 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "7", data = (mes6.Any() ? mes6[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-4).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-4).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes5 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "8", data = (mes5.Any() ? mes5[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-3).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-3).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes4 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "9", data = (mes4.Any() ? mes4[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-2).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-2).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes3 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "10", data = (mes3.Any() ? mes3[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-1).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-1).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes2 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "11", data = (mes2.Any() ? mes2[0].data : 0) });

                fechaDesde = DateTime.Now.GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes1 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Cobrado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "12", data = (mes1.Any() ? mes1[0].data : 0) });

            }
        }

        return list;
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public static List<Chart> obtenerPagado()
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
                var mes12 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "1", data = (mes12.Any() ? mes12[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-10).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-10).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes11 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "2", data = (mes11.Any() ? mes11[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-9).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-9).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes10 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "3", data = (mes10.Any() ? mes10[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-8).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-8).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes9 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "4", data = (mes9.Any() ? mes9[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-7).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-7).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes8 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "5", data = (mes8.Any() ? mes8[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-6).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-6).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes7 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "6", data = (mes7.Any() ? mes7[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-5).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-5).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes6 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "7", data = (mes6.Any() ? mes6[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-4).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-4).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes5 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "8", data = (mes5.Any() ? mes5[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-3).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-3).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes4 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "9", data = (mes4.Any() ? mes4[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-2).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-2).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes3 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "10", data = (mes3.Any() ? mes3[0].data : 0) });

                fechaDesde = DateTime.Now.AddMonths(-1).GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.AddMonths(-1).GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes2 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
                list.Add(new Chart() { label = "11", data = (mes2.Any() ? mes2[0].data : 0) });

                fechaDesde = DateTime.Now.GetFirstDayOfMonth().ToString(formato);
                fechaHasta = DateTime.Now.GetLastDayOfMonth().ToString(formato + " 12:59:59");
                var mes1 = dbContext.Database.SqlQuery<Chart>("exec Dashboard_Pagado '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
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
                foreach (var detalle in IngresosEgresos)
                {
                    total += Convert.ToDecimal(detalle.Total);

                    html += "<tr>";
                    html += "<td class='bgRow'>" + detalle.RazonSocial + "</td>";
                    html += "<td class='bgRow'>" + detalle.Comprobante + "</td>";
                    html += "<td class='bgRow'>" + detalle.Fecha + "</td>";
                    html += "<td class='bgRow'>" + Convert.ToDecimal(detalle.Total).ToString("N2") + "</td>";
                    html += "</tr>";
                }

                html += "<tr>";
                html += "<td class='bgTotal' colspan='2'></td>";
                html += "<td class='bgTotal text-danger'>Total</td>";
                html += "<td class='bgTotal text-danger'>" + total.ToString("N2") + "</td>";
                html += "</tr>";
            }
            else
                html += "<tr><td colspan='4'>No hay un detalle disponible</td></tr>";


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

            if (Etiqueta == "Cobrado")
            {
                var listIng = dbContext.Cobranzas.Where(x => x.IDUsuario == usu.IDUsuario && x.FechaCobranza >= fechaDesde && x.FechaCobranza <= fechaHasta).OrderBy(x => x.FechaCobranza).ToList();

                foreach (var item in listIng)
                {
                    if (item.CobranzasFormasDePago.Any(x => x.IDNotaCredito == null))
                    {
                        det = new RptIngresoEgresoViewModel();
                        det.RazonSocial = item.Personas.RazonSocial;
                        //det.Comprobante = item.Tipo + " " + item.PuntosDeVenta.Punto.ToString("#0000") + "-" + item.Numero.ToString("#00000000");
                        det.Comprobante = item.Tipo + " " + item.Numero.ToString("#00000000");
                        det.Fecha = item.FechaCobranza.ToString("dd/MM/yyyy");
                        det.Total = Convert.ToInt32(item.ImporteTotal).ToString("N2");
                        IngresosEgresos.Add(det);
                    }
                }
            }
            else
            {
                var listEgr = dbContext.Pagos.Include("PagosDetalle").Where(x => x.IDUsuario == usu.IDUsuario && x.FechaPago >= fechaDesde && x.FechaPago <= fechaHasta).OrderBy(x => x.FechaPago).ToList();
                foreach (var item in listEgr)
                {
                    det = new RptIngresoEgresoViewModel();
                    det.RazonSocial = item.Personas.RazonSocial;
                    det.Comprobante = item.PagosDetalle.Count() > 1 ? "Varios" : item.PagosDetalle.First().Compras.Tipo + " " + item.PagosDetalle.First().Compras.NroFactura;
                    det.Fecha = item.FechaPago.ToString("dd/MM/yyyy");
                    det.Total = Convert.ToDecimal(item.ImporteTotal).ToString("N2");
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
                dt = resultados.ToDataTable();

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