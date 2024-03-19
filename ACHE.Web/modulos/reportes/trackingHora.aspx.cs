using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Data;
using System.IO;
using ACHE.Extensions;

public partial class modulos_reportes_trackingHora : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionTrackingHoras)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    public static void delete(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.TrackingHoras.Where(x => x.IDTrackingHoras == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.TrackingHoras.Remove(entity);
                        dbContext.SaveChanges();
                    }
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

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosTrackingHorasViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.TrackingHoras.Include("Personas").Include("UsuariosAdicionales").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();


                    if (idPersona != 0)
                        results = results.Where(x => x.IDPersona == idPersona);

                    DateTime dtDesde = new DateTime();
                    DateTime dtHasta = new DateTime();

                    if (fechaDesde != "")
                    {
                        dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.Fecha >= dtDesde).OrderBy(x => x.Personas.RazonSocial);
                    }
                    if (fechaHasta != "")
                    {
                        dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Fecha <= dtHasta);
                    }

                    page--;
                    ResultadosTrackingHorasViewModel resultado = new ResultadosTrackingHorasViewModel();

                    var list = results.OrderBy(x => x.Personas.RazonSocial).Skip(page * pageSize).Take(pageSize).ToList().GroupBy(x => new { x.IDUsuarioAdicional, x.IDPersona })
                     .Select(x => new TrackingHorasViewModel()
                     {
                         RazonSocial = x.FirstOrDefault().Personas.RazonSocial,
                         NombreUsuario = (string.IsNullOrWhiteSpace(x.FirstOrDefault().IDUsuarioAdicional.ToString())) ? x.FirstOrDefault().Usuarios.RazonSocial : x.FirstOrDefault().UsuariosAdicionales.Email,
                         CantHorasFacturables = x.Where(z => z.Estado == "Facturable").Sum(y => y.Horas),
                         CantHorasNOFacturables = x.Where(z => z.Estado == "No Facturable").Sum(y => y.Horas)
                     });


                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();
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

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Tesoreria";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.TrackingHoras.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (idPersona != 0)
                        results = results.Where(x => x.IDPersona == idPersona);

                    DateTime dtDesde = new DateTime();
                    DateTime dtHasta = new DateTime();

                    if (fechaDesde != string.Empty)
                    {
                        dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.Fecha >= dtDesde).OrderBy(x => x.Personas.RazonSocial);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Fecha <= dtHasta);
                    }

                    dt = results.OrderBy(x => x.Personas.RazonSocial).ToList().GroupBy(x => x.IDUsuarioAdicional).Select(x => new
                    {
                        NombreCliente = x.FirstOrDefault().Personas.RazonSocial,
                        Usuario = (string.IsNullOrWhiteSpace(x.FirstOrDefault().IDUsuarioAdicional.ToString())) ? x.FirstOrDefault().Usuarios.RazonSocial : x.FirstOrDefault().UsuariosAdicionales.Email,
                        CantHorasFacturables = x.Where(z => z.Estado == "Facturable").Sum(y => y.Horas),
                        CantHorasNOFacturables = x.Where(z => z.Estado == "NoFacturable").Sum(y => y.Horas),
                    }).ToList().ToDataTable();

                }

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

    //////////


}