using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Reportes;

public partial class modulos_reportes_Percepciones : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ",";//"."
    public const string SeparadorDeDecimales = ".";//","

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoImpositivosPercepciones)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosPercepcionesViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, string tipo, string impuesto, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                if (tipo == "Emitidas")
                    return ReportesCommon.ObtenerPercepcionesEmitidas(idPersona, fechaDesde, fechaHasta, page, pageSize, usu, impuesto);
                else
                {
                    if (impuesto == "IIBB")
                        return ReportesCommon.ObtenerPercepcionesSufridas(idPersona, fechaDesde, fechaHasta, page, pageSize, usu);
                    else
                        return ReportesCommon.ObtenerPercepcionesSufridasIVA(idPersona, fechaDesde, fechaHasta, page, pageSize, usu);
                }
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
    public static string export(int idPersona, string fechaDesde, string fechaHasta, string tipo, string impuesto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Percepciones";
            string path = "~/tmp/";
            try
            {
                var resultado = new ResultadosPercepcionesViewModel();

                if (tipo == "Emitidas")
                    resultado = ReportesCommon.ObtenerPercepcionesEmitidas(idPersona, fechaDesde, fechaHasta, 1, 1000000, usu, impuesto);
                else
                {
                    if (impuesto == "IIBB")
                        resultado = ReportesCommon.ObtenerPercepcionesSufridas(idPersona, fechaDesde, fechaHasta, 1, 1000000, usu);
                    else
                        resultado = ReportesCommon.ObtenerPercepcionesSufridasIVA(idPersona, fechaDesde, fechaHasta, 1, 1000000, usu);
                }

                DataTable dt = new DataTable();

                switch (impuesto)
                {
                    case "IIBB":
                        dt = resultado.Items.ToList().Select(x => new
                        {
                            Fecha = x.Fecha,
                            RazonSocial = x.RazonSocial,
                            Cuit = x.Cuit,
                            CondicionIVA = x.CondicionIVA,
                            NroComprobante = x.NroComprobante,
                            Jurisdiccion = x.Jurisdiccion,
                            Importe = Convert.ToDecimal(x.Importe.Replace(SeparadorDeMiles, SeparadorDeDecimales))
                        }).ToList().ToDataTable();
                        break;
                    case "IVA":
                        dt = resultado.Items.ToList().Select(x => new
                        {
                            Fecha = x.Fecha,
                            RazonSocial = x.RazonSocial,
                            Cuit = x.Cuit,
                            CondicionIVA = x.CondicionIVA,
                            NroComprobante = x.NroComprobante,
                            Importe = Convert.ToDecimal(x.Importe.Replace(SeparadorDeMiles, SeparadorDeDecimales))
                        }).ToList().ToDataTable();
                        break;
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