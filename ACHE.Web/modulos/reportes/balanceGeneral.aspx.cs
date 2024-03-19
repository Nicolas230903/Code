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
using ACHE.Model.ViewModels;
using ACHE.Negocio.Contabilidad;

public partial class modulos_reportes_balanceGeneral : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosLibroMayorViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ContabilidadCommon.ObtenerBalanceDeResultados(usu, fechaDesde, fechaHasta, page, pageSize);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "BalanceGeneral";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.rptImpositivoLibroDiario.Where(x => x.IDUsuario == usu.IDUsuario).GroupBy(x => x.Codigo).AsQueryable();
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FirstOrDefault().Fecha >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FirstOrDefault().Fecha <= dtHasta);
                    }


                    var listaBalance = results.OrderBy(x => x.FirstOrDefault().Codigo).ToList();


                    dt = results.OrderBy(x => x.FirstOrDefault().Codigo).ToList().Select(x => new
                    {
                        Codigo = x.FirstOrDefault().Codigo,
                        NombreCuenta = x.FirstOrDefault().Nombre,
                        TotalDebe = x.Sum(y => y.Debe),
                        TotalHaber = x.Sum(y => y.Haber),

                        TotalDeudor = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) > 0) ? (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)): 0,
                        TotalAcreedor = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) < 0) ? Math.Abs(x.Sum(y => y.Debe) - x.Sum(y => y.Haber)) : 0,

                        TotalActivo = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "ACTIVO") ? (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)) : 0,
                        TotalPasivo = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "PASIVO") ? Math.Abs(x.Sum(y => y.Debe) - x.Sum(y => y.Haber)) : 0,

                        TotalPerdidas = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "RESULTADO" && (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) > 0)) ? (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)) : 0,
                        TotalGanancias = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "RESULTADO" && (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) < 0)) ? Math.Abs(x.Sum(y => y.Debe) - x.Sum(y => y.Haber)) : 0,
                    }).ToList().ToDataTable();
                }

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