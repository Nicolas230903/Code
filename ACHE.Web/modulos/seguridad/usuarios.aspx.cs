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

public partial class usuarios : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (CurrentUser.TipoUsuario != "A")
                Response.Redirect("/home.aspx");
            
            using (var dbContext = new ACHEEntities())
            {
                var TieneDatos = dbContext.UsuariosAdicionales.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
                    var entity = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity.Email.ToUpper() == usu.Email.ToUpper())
                        throw new Exception("No se puede eliminar el usuario administrador de la cuenta.");
                    if (entity != null)
                    {
                        dbContext.UsuariosAdicionales.Remove(entity);
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
    public static ResultadosUsuariosViewModel getResults(string email, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.UsuariosAdicionales.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (email != string.Empty)
                        results = results.Where(x => x.Email.Contains(email));

                    var list = results.Select(x => new UsuariosViewModel()
                    {
                        ID = x.IDUsuarioAdicional,
                        Tipo = x.Tipo == "A" ? "Administrador" : "Backoffice",
                        Email = x.Email.ToLower(),
                        Activo = x.Activo ? "Si" : "No"
                    });

                    page--;

                    ResultadosUsuariosViewModel resultado = new ResultadosUsuariosViewModel();
                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();
                    resultado.Items = list.OrderBy(x => x.Email).Skip(page * pageSize).Take(pageSize).ToList();

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

}