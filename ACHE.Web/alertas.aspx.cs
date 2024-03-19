using ACHE.Model;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Data;
using System.Web.Services;
using ACHE.Extensions;

public partial class alertas : BasePage
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
                    if (!afu.HerramientasConfigurarAlertas)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                var TieneDatos = dbContext.Alertas.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
                    var entity = dbContext.Alertas.Where(x => x.IDAlerta == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.Alertas.Remove(entity);
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
    public static ResultadosAlertasViewModel getResults(string avisoAlertas, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Alertas.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (avisoAlertas != string.Empty)
                        results = results.Where(x => x.AvisoAlerta == avisoAlertas);


                    page--;
                    ResultadosAlertasViewModel resultado = new ResultadosAlertasViewModel();

                    var list = results.OrderByDescending(x => x.IDAlerta).Skip(page * pageSize).Take(pageSize).ToList()
                     .Select(x => new AlertasViewModel()
                     {
                         ID = x.IDAlerta,
                         AvisoAlerta = x.AvisoAlerta,
                         Condicion = x.Condicion,
                         Importe = x.Importe.ToString("N2")
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
    public static AlertasViewModel cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            AlertasViewModel AlertasViewModel = new AlertasViewModel();
            var entity = dbContext.Alertas.Where(x => x.IDAlerta == id).FirstOrDefault();
            if (entity != null)
            {
                AlertasViewModel.ID = entity.IDAlerta;
                AlertasViewModel.Condicion = entity.Condicion;
                AlertasViewModel.AvisoAlerta = entity.AvisoAlerta;
                AlertasViewModel.Importe = entity.Importe.ToString().Replace(",", ".");
            }
            return AlertasViewModel;
        }
    }

    [WebMethod(true)]
    public static void guardarAlertas(int id, string importe, string avisos, string condiciones)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var existe = dbContext.Alertas.Any(x => x.Condicion == condiciones && x.AvisoAlerta == avisos && x.IDUsuario == usu.IDUsuario && x.IDAlerta != id);
                if (existe)
                    throw new Exception("Ya existe una alerta con estas caracteristicas.");

                Alertas entity;
                if (id > 0)
                {
                    entity = dbContext.Alertas.Where(x => x.IDAlerta == id & x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                }
                else
                {
                    entity = new Alertas();
                }

                entity.Importe = (importe != string.Empty) ? Convert.ToDecimal(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.AvisoAlerta = avisos;
                entity.Condicion = condiciones;
                entity.IDUsuario = usu.IDUsuario;


                if (id == 0)
                    dbContext.Alertas.Add(entity);

                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void esconderAlertaGenerada(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var alerta = dbContext.AlertasGeneradas.Where(x => x.IDAlertasGeneradas == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                alerta.Visible = false;
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}