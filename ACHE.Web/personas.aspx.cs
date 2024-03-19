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
using ACHE.Negocio.Common;
using ACHE.Extensions;

public partial class personas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var tipo = Request.QueryString["tipo"];
            hdnTipo.Value = tipo;

            if (tipo == "c")
                litTipo.Text = "cliente";
            else if (tipo == "p")
                litTipo.Text = "proveedor";

            if (CurrentUser.TipoUsuario == "B")
            {
                if (!PermisosModulos.mostrarPersonaSegunPermiso(tipo))
                    Response.Redirect("home.aspx");
            }
            if (tipo == "c")
            {
                using (var dbContext = new ACHEEntities())
                {
                    AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                    if (afu != null)
                        if (!afu.ComercialClientes)
                            Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                }

                litTitulo.Text = "<i class='fa fa-suitcase'></i> Clientes";
                litPath.Text = "Clientes";
            }
            else if (tipo == "p")
            {
                using (var dbContext = new ACHEEntities())
                {
                    AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                    if (afu != null)
                        if (!afu.SuministroProveedores)
                            Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                }

                litTitulo.Text = "<i class='fa fa-users'></i> Proveedores";
                litPath.Text = "Proveedores";
            }
            else
                Response.Redirect("home.aspx");

            using (var dbContext = new ACHEEntities())
            {
                var TieneDatos = dbContext.Personas.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo == tipo);
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
                PersonasCommon.EliminarPersona(id, usu);
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
    public static ResultadosPersonasViewModel getResults(string condicion, string tipo, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return PersonasCommon.ObtenerPersonas(condicion, tipo, page, pageSize, usu);
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
    public static string export(string condicion, string tipo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "ClientesProv";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipo.ToUpper()).AsQueryable();

                    if (condicion != string.Empty)
                        results = results.Where(x => x.RazonSocial.Contains(condicion) || x.NombreFantansia.Contains(condicion) || x.NroDocumento.Contains(condicion));


                    dt = results.OrderBy(x => x.RazonSocial).ToList().Select(x => new
                    {
                        IdClienteProveedor = x.IDPersona,
                        Personeria = x.Personeria == "F" ? "Fisica" : "Juridica",
                        RazonSocial = x.RazonSocial.ToUpper(),
                        NombreFantasia = (x.CondicionIva == "RI" || x.CondicionIva == "EX") ? x.NombreFantansia.ToUpper() : "",
                        Documento = x.NroDocumento,
                        CodigoInterno = x.Codigo,
                        CategoriaImpositiva = x.CondicionIva,
                        Telefono = x.Telefono,
                        Celular = x.Celular,
                        Email = x.Email.ToLower(),
                        Web = x.Web,
                        Observaciones = x.Observaciones,
                        Provincia = x.Provincias.Nombre,
                        Ciudad = x.Ciudades.Nombre,
                        Domicilio = x.Domicilio,
                        PisoDepto = x.PisoDepto,
                        CodigoPostal = x.CodigoPostal,
                        EmailsEnvioFc = x.EmailsEnvioFc

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