using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class empresas : BasePage
{
    private const string LOGO_PATH = "/files/usuarios/";

    protected void Page_Load(object sender, EventArgs e)
    {
        IDUsuarioAdicional.Value = CurrentUser.IDUsuarioAdicional.ToString();
        if (!CurrentUser.TieneMultiEmpresa)
        {
            Response.Redirect("~/home.aspx");
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
                    if (dbContext.UsuariosEmpresa.Any(x => x.IDUsuario == id))
                        throw new Exception("No se puede eliminar por tener usuarios adicionales asociados");
                    if (dbContext.Comprobantes.Any(x => x.IDUsuario == id))
                        throw new Exception("No se puede eliminar por tener Comprobantes adicionales asociados");
                    if (dbContext.Personas.Any(x => x.IDUsuario == id))
                        throw new Exception("No se puede eliminar por tener Comprobantes adicionales asociados");
                    else
                    {
                        var entity = dbContext.Usuarios.Where(x => x.IDUsuario == id && x.IDUsuarioPadre == usu.IDUsuario).FirstOrDefault();
                        if (entity != null)
                        {
                            dbContext.Usuarios.Remove(entity);
                            dbContext.SaveChanges();
                        }
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
    public static ResultadosEmpresasViewModel getResults()//string razonSocial, string cuit, int page, int pageSize
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                //int idusuarioLogiado = 0;
                using (var dbContext = new ACHEEntities())
                {
                    List<EmpresasViewModel> list = new List<EmpresasViewModel>();

                    if (usu.IDUsuarioAdicional != 0)
                    {
                        //Carga los datos si es una usuario Adicional
                        var results = dbContext.UsuariosEmpresa.Include("Usuarios").Where(x => x.IDUsuarioAdicional == usu.IDUsuarioAdicional).ToList();
                        Usuarios usuarioPadre;
                        if (string.IsNullOrWhiteSpace(usu.IDUsuarioPadre.ToString()))
                        {
                            usuarioPadre = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario && x.IDUsuarioPadre == null).FirstOrDefault();
                        }
                        else
                        {
                            usuarioPadre = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuarioPadre).FirstOrDefault();
                        }
                        list = esUsuAdicional(list, results, usuarioPadre);
                    }
                    else
                    {
                        IList<Usuarios> results = null;
                        if (string.IsNullOrWhiteSpace(usu.IDUsuarioPadre.ToString()))
                        {
                            //Soy un usuario Padre y busco todas mis empresas
                            results = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == usu.IDUsuario).ToList();
                            list = esUsuarioPadre(list, results, usu);
                        }
                        else
                        {
                            //Soy un usuario Padre pero estoy logiado con otra de mis  empresas y busco todas mis empresas
                            results = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == usu.IDUsuarioPadre).ToList();
                            var usuarioPadre = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuarioPadre).FirstOrDefault();
                            list = esCambioSesion(list, results, usuarioPadre);
                        }
                    }

                    ResultadosEmpresasViewModel resultado = new ResultadosEmpresasViewModel();
                    resultado.TotalPage = 1;
                    resultado.UsuLogiado = usu.IDUsuario.ToString();
                    resultado.TotalItems = list.Count();
                    resultado.Items = list.OrderBy(x => x.RazonSocial).ToList();

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

    private static List<EmpresasViewModel> esCambioSesion(List<EmpresasViewModel> list, IList<Usuarios> results, Usuarios usuarioPadre)
    {
        list = results.Select(x => new EmpresasViewModel()
        {
            ID = x.IDUsuario,
            RazonSocial = x.RazonSocial.ToUpper(),
            CUIT = x.CUIT,
            CondicionIva = UsuarioCommon.GetCondicionIvaDesc(x.CondicionIva),
            Email = x.Email.ToLower(),
            Domicilio = x.Domicilio,
            Ciudad = x.Ciudades.Nombre,
            Provincia = x.Provincias.Nombre,
            Logo = (string.IsNullOrEmpty(x.Logo)) ? LOGO_PATH + "no-photo.png" : LOGO_PATH + usuarioPadre.Logo
        }).ToList();

        var empresa = new EmpresasViewModel();
        empresa.ID = usuarioPadre.IDUsuario;
        empresa.RazonSocial = usuarioPadre.RazonSocial.ToUpper();
        empresa.CUIT = usuarioPadre.CUIT;
        empresa.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usuarioPadre.CondicionIva);
        empresa.Email = usuarioPadre.Email.ToLower();
        empresa.Domicilio = usuarioPadre.Domicilio;
        empresa.Ciudad = usuarioPadre.Ciudades.Nombre;
        empresa.Provincia = usuarioPadre.Provincias.Nombre;
        empresa.Logo = (string.IsNullOrEmpty(usuarioPadre.Logo)) ? LOGO_PATH + "no-photo.png" : LOGO_PATH + usuarioPadre.Logo;
        list.Add(empresa);

        return list;
    }

    private static List<EmpresasViewModel> esUsuarioPadre(List<EmpresasViewModel> list, IList<Usuarios> results, WebUser usu)
    {
        list = results.Select(x => new EmpresasViewModel()
        {
            ID = x.IDUsuario,
            RazonSocial = x.RazonSocial.ToUpper(),
            CUIT = x.CUIT,
            CondicionIva = UsuarioCommon.GetCondicionIvaDesc(x.CondicionIva),
            Email = x.Email.ToLower(),
            Domicilio = x.Domicilio,
            Ciudad = x.Ciudades.Nombre,
            Provincia = x.Provincias.Nombre,
            Logo = (string.IsNullOrEmpty(x.Logo)) ? LOGO_PATH + "no-photo.png" : LOGO_PATH + x.Logo
        }).ToList();


        //Agrego la empresa actual
        var empresa = new EmpresasViewModel();
        empresa.ID = usu.IDUsuario;
        empresa.RazonSocial = usu.RazonSocial.ToUpper();
        empresa.CUIT = usu.CUIT;
        empresa.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usu.CondicionIVA);
        empresa.Email = usu.Email.ToLower();
        empresa.Domicilio = usu.Domicilio.ToLower();
        empresa.Ciudad = usu.Ciudad.ToLower();
        empresa.Provincia = usu.Provincia.ToLower();
        empresa.Logo = (string.IsNullOrEmpty(usu.Logo)) ? LOGO_PATH + "no-photo.png" : LOGO_PATH + usu.Logo;
        list.Add(empresa);

        return list;
    }

    private static List<EmpresasViewModel> esUsuAdicional(List<EmpresasViewModel> list, List<UsuariosEmpresa> results, Usuarios usuarioPadre)
    {
        list = results.Select(x => new EmpresasViewModel()
        {
            ID = x.IDUsuario,
            RazonSocial = x.Usuarios.RazonSocial.ToUpper(),
            CUIT = x.Usuarios.CUIT,
            CondicionIva = UsuarioCommon.GetCondicionIvaDesc(x.Usuarios.CondicionIva),
            Email = x.Usuarios.Email.ToLower(),
            Domicilio = x.Usuarios.Domicilio,
            Ciudad = x.Usuarios.Ciudades.Nombre,
            Provincia = x.Usuarios.Provincias.Nombre,
            Logo = (string.IsNullOrWhiteSpace(x.Usuarios.Logo)) ? LOGO_PATH + "no-photo.png" : LOGO_PATH + usuarioPadre.Logo
        }).ToList();

        if (usuarioPadre != null)
        {
            var empresa = new EmpresasViewModel();
            empresa.ID = usuarioPadre.IDUsuario;
            empresa.RazonSocial = usuarioPadre.RazonSocial.ToUpper();
            empresa.CUIT = usuarioPadre.CUIT;
            empresa.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usuarioPadre.CondicionIva);
            empresa.Email = usuarioPadre.Email.ToLower();
            empresa.Domicilio = usuarioPadre.Domicilio.ToLower();
            empresa.Ciudad = usuarioPadre.Ciudades.Nombre.ToLower();
            empresa.Provincia = usuarioPadre.Provincias.Nombre.ToLower();
            empresa.Logo = (string.IsNullOrWhiteSpace(usuarioPadre.Logo)) ? LOGO_PATH + "no-photo.png" : LOGO_PATH + usuarioPadre.Logo;
            list.Add(empresa);
        }

        return list;
    }
}