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

public partial class modulos_reportes_compras_por_categoria : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoFinancierosComprasPorCategoria)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<Chart> obtenerCompras(string desde, string hasta)
    {
        List<Chart> list = null;
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fechaDesde = DateTime.Now.GetFirstDayOfMonth().ToString(formato);
            string fechaHasta = DateTime.Now.GetLastDayOfMonth().ToString(formato + " 12:59:59");

            if (desde != string.Empty)
                fechaDesde = DateTime.Parse(desde).ToString(formato);
            if (hasta != string.Empty)
                fechaHasta = DateTime.Parse(hasta).ToString(formato + " 12:59:59");

            using (var dbContext = new ACHEEntities())
            {
                list = dbContext.Database.SqlQuery<Chart>("exec Dashboard_ComprasPorCategoria '" + fechaDesde + "','" + fechaHasta + "', " + usu.IDUsuario, new object[] { }).ToList();
            }
        }

        return list;
    }

    [WebMethod(true)]
    public static string getDetail(string fechaDesde, string fechaHasta, string Etiqueta)
    {
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var html = "";
            var compras = ObtenerComprasPorFecha(Convert.ToDateTime(fechaDesde), Convert.ToDateTime(fechaHasta), Etiqueta);

            if (compras.Any())
            {
                decimal total = 0;
                foreach (var detalle in compras)
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

    private static List<RptIngresoEgresoViewModel> ObtenerComprasPorFecha(DateTime fechaDesde, DateTime fechaHasta, string Etiqueta)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        List<RptIngresoEgresoViewModel> IngresosEgresos = new List<RptIngresoEgresoViewModel>();
        RptIngresoEgresoViewModel det;
        using (var dbContext = new ACHEEntities())
        {

            var listEgr = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo != "COT" && x.Fecha >= fechaDesde && x.Fecha <= fechaHasta && x.Categorias.Nombre.Contains(Etiqueta)).OrderBy(x=>x.Fecha).ToList();
            foreach (var item in listEgr)
            {
                det = new RptIngresoEgresoViewModel();
                det.RazonSocial = item.Personas.RazonSocial;
                det.Comprobante = item.Tipo + " " + item.NroFactura;
                det.Fecha = item.Fecha.ToString("dd/MM/yyyy");
                det.Total = Convert.ToDecimal(item.Total).ToString("N2");
                IngresosEgresos.Add(det);
            }

        }
        return IngresosEgresos;
    }

    [WebMethod(true)]
    public static string exportDetalle(string fechaDesde, string fechaHasta, string Etiqueta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string fileName = "Detalle" + Etiqueta;
            string path = "~/tmp/";
            try
            {
                var resultados = ObtenerComprasPorFecha(Convert.ToDateTime(fechaDesde), Convert.ToDateTime(fechaHasta), Etiqueta);
                DataTable dt = new DataTable();
                dt = resultados.ToDataTable();

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return  (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~","");
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