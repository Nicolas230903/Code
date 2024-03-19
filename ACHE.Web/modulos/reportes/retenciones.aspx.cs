using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;

public partial class modulos_reportes_retenciones : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoImpositivosRetenciones)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptRetencionesSufridasViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, string tipo, string impuesto, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptRetenciones.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipo && x.Impuesto == impuesto).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.Fecha >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Fecha <= dtHasta);
                    }

                    page--;
                    ResultadosRptRetencionesSufridasViewModel resultado = new ResultadosRptRetencionesSufridasViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptRetencionesSufridasViewModel()
                        {
                            Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                            RazonSocial = x.RazonSocial,
                            Cuit = x.CUIT,
                            CondicionIVA = x.CondicionIVA,
                            NroReferencia = x.NroReferencia,
                            Importe = x.Importe.ToString("N2"),
                            IDRetencion = x.IDRetencion
                        });
                    resultado.Items = list.ToList();

                    return resultado;
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

            string fileName = "Retenciones";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptRetenciones.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipo && x.Impuesto == impuesto).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.Fecha >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Fecha <= dtHasta);
                    }

                    dt = results.OrderBy(x => x.Fecha).ToList().Select(x => new
                    {
                        Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                        RazonSocial = x.RazonSocial,
                        Cuit = x.CUIT,
                        CondicionIVA = x.CondicionIVA,
                        NroReferencia = x.NroReferencia,
                        Importe = x.Importe
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string imprimirRetencionGananciaPorId(int idPagoRetencion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string numero = "0";
            Common.CrearRetencionGananciaPorIdRetencion(usu, idPagoRetencion, ref numero, Common.ComprobanteModo.Generar);
            return usu.IDUsuario.ToString() + "/" + numero + ".pdf";

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}