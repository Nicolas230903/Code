using ACHE.Extensions;
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
using System.Web.Services;
public partial class modulos_tesoreria_activos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
            using (var dbContext = new ACHEEntities())
            {
                var TieneDatos = dbContext.Activos.Any(x => x.Compras.IDUsuario == CurrentUser.IDUsuario);
                if (TieneDatos)
                {
                    divConDatos.Visible = true;
                    divSinDatos.Visible = false;
                }
                else
                {
                    divConDatos.Visible = false;
                    divSinDatos.Visible = true;
                }
            }
        }
    }

    [WebMethod(true)]
    public static void delete(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Activos.Where(x => x.IDActivos == id).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.Activos.Remove(entity);
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosActivosViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Activos.AsQueryable();

                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    
                    DateTime dtDesde = new DateTime();
                    DateTime dtHasta = new DateTime();

                    if (fechaDesde != string.Empty)
                    {
                        dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaCompra >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaCompra <= dtHasta);
                    }


                    page--;

                    ResultadosActivosViewModel resultado = new ResultadosActivosViewModel();

                    var list = results.OrderByDescending(x => x.FechaCompra).Skip(page * pageSize).Take(pageSize).ToList()
                      .Select(x => new ActivosViewModel()
                      {
                          ID = x.IDActivos,
                          FechaCompra = Convert.ToDateTime(x.FechaCompra).ToString("dd/MM/yyyy"),
                          razonSocial = x.Personas.RazonSocial,
                          fechaCompra = Convert.ToDateTime(x.FechaCompra).ToString("dd/MM/yyyy"),
                          vidaUtil = x.VidaUtil
                      });

                    resultado.Items = list.ToList();
                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();

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
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "tesoreria";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Activos.Where(x => x.IDPersona == usu.IDUsuario).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);

                    DateTime dtDesde = new DateTime();
                    DateTime dtHasta = new DateTime();

                    if (fechaDesde != string.Empty)
                    {
                        dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaCompra >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaCompra <= dtHasta);
                    }

                    dt = results.OrderByDescending(x => x.FechaCompra).ToList().Select(x => new
                    {
                        FechaCompra = Convert.ToDateTime(x.FechaCompra).ToString("dd/MM/yyyy"),
                        razonSocial = x.Personas.RazonSocial,
                        fechaCompra = Convert.ToDateTime(x.FechaCompra).ToString("dd/MM/yyyy"),
                        vidaUtil = x.VidaUtil

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
}