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
using System.Globalization;

public partial class modulos_reportes_evolucion_ventas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ChartDecimalInt> ObtenerProductos(int idPersona, string desde, string hasta)
    {
        List<ChartDecimalInt> listapersonas = new List<ChartDecimalInt>();
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            DateTime dtDesde = new DateTime();
            DateTime dtHasta = new DateTime();

            if (desde != string.Empty)
            {
                dtDesde = DateTime.Parse(desde);
            }
            if (hasta != string.Empty)
            {
                dtHasta = DateTime.Parse(hasta + " 12:59:59 pm");
            }
            //Cobr
            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && x.Tipo != "COT" && x.FechaComprobante >= dtDesde && x.FechaComprobante <= dtHasta).ToList();
                var listafechas = lista.GroupBy(x => new { x.FechaComprobante.Month, x.FechaComprobante.Year }).Select(x => new
                {
                    importe = x.Sum(y => y.ImporteTotalNeto),
                    fecha = x.Select(y => y.FechaComprobante.Year.ToString() + y.FechaComprobante.Month.ToString()).FirstOrDefault()
                }).ToList();
                int contador = 0;
                foreach (var item in listafechas)
                {
                    ChartDecimalInt cd = new ChartDecimalInt();
                    cd.fecha = contador;
                    cd.data = item.importe;
                    listapersonas.Add(cd);
                    contador++;
                }
            }
        }
        return listapersonas;
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ChartDecimal> ObtenerTicks(int idPersona, string desde, string hasta)
    {
        List<ChartDecimal> listaTicks = new List<ChartDecimal>();
        string formato = ConfigurationManager.AppSettings["FormatoFechasSQL"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            DateTime dtDesde = new DateTime();
            DateTime dtHasta = new DateTime();

            if (desde != string.Empty)
                dtDesde = DateTime.Parse(desde);

            if (hasta != string.Empty)
                dtHasta = DateTime.Parse(hasta + " 12:59:59 pm");

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && x.Tipo != "COT" && x.FechaComprobante >= dtDesde && x.FechaComprobante <= dtHasta).ToList();
                var listafechas = lista.GroupBy(x => new { x.FechaComprobante.Month, x.FechaComprobante.Year }).Select(x => new
                {
                    label = x.Select(y => MonthName(y.FechaComprobante.Month) + " - " + y.FechaComprobante.Year.ToString()).FirstOrDefault(),
                    fecha = x.Select(y => y.FechaComprobante.Year.ToString() + y.FechaComprobante.Month.ToString()).FirstOrDefault()
                }).ToList();

                int contador = 0;
                foreach (var item in listafechas)
                {
                    ChartDecimal cd = new ChartDecimal();
                    cd.data = contador;
                    cd.label = item.label;
                    listaTicks.Add(cd);
                    contador++;
                }
            }
        }

        return listaTicks;
    }

    private static string MonthName(int month)
    {
        DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
        return dtinfo.GetMonthName(month);
    }

}