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
using ACHE.Extensions;

public partial class modulos_rrhh_empleados : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                var TieneDatos = dbContext.Empleados.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
                    var entity = dbContext.Empleados.Where(x => x.IDEmpleados == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.FechaBaja = DateTime.Now.Date;
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
    public static ResultadosEmpleadoViewModel getResults(string nombre, string apellido, string cuit, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Empleados.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(nombre.ToString()))
                        results = results.Where(x => x.Nombre == nombre);

                    if (apellido != string.Empty)
                        results = results.Where(x => x.Apellido.Contains(apellido));

                    if (cuit != string.Empty)
                        results = results.Where(x => x.CUIT.Contains(cuit));

                    page--;
                    ResultadosEmpleadoViewModel resultado = new ResultadosEmpleadoViewModel();

                    var list = results.OrderBy(x => x.Apellido).Skip(page * pageSize).Take(pageSize).ToList()
                     .Select(x => new EmpleadoViewModel()
                     {
                         ID = x.IDEmpleados,
                         Nombre = x.Nombre,
                         Apellido = x.Apellido,
                         CUIT = x.CUIT,
                         Email = x.Email,
                         Domicilio = x.Domicilio,
                         NroLegajo = x.NroLegajo.ToString(),
                         Telefono = x.Telefono
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(string nombre, string apellido, string cuit)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "rrhh";
            string path = "/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Empleados.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(nombre.ToString()))
                        results = results.Where(x => x.Nombre == nombre);

                    if (apellido != string.Empty)
                        results = results.Where(x => x.Apellido.Contains(apellido));

                    if (cuit != string.Empty)
                        results = results.Where(x => x.CUIT.Contains(cuit));


                    dt = results.OrderBy(x => x.Apellido).ToList().Select(x => new
                    {
                        Nombre = x.Nombre,
                        Apellido = x.Apellido,
                        CUIT = x.CUIT,
                        Email = x.Email,
                        Domicilio = x.Domicilio,
                        NroLegajo = x.NroLegajo.ToString(),
                        Telefono = x.Telefono

                    }).ToList().ToDataTable();

                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx";
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