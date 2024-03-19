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
using Ionic.Zip;

public partial class lista_facturas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionListaFacturas)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptRnkViewModel getResults(string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo.Contains("F")).AsQueryable();
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaComprobante <= dtHasta);
                    }

                    page--;
                    ResultadosRptRnkViewModel resultado = new ResultadosRptRnkViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();


                    var list = results.OrderBy(x => x.FechaComprobante).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptRnkViewModel()
                        {
                            Valor1 = x.FechaComprobante.ToString(),
                            Valor2 = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            Total = x.ImporteTotalNeto.ToString()
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
    public static string export(string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string pathFacturas = "~/files/explorer/" + usu.IDUsuario.ToString() + "/Comprobantes/";
            string path = "~/tmp/";
            string pathZip = path + usu.IDUsuario.ToString() + "_" + DateTime.Now.ToString("yyyMMddhhmmss") + ".zip";
            List<string> files = new List<string>();

            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo.Contains("F")).AsQueryable();
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaComprobante <= dtHasta);
                    }                   

                    foreach(Comprobantes r in results)
                    {
                        var persona = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == r.IDPersona).FirstOrDefault();

                        string archivoNombre = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + r.Tipo + "-" + r.PuntosDeVenta.Punto.ToString("0000") + "-" + r.Numero.ToString("#00000000") + ".pdf";

                        if (File.Exists(HttpContext.Current.Server.MapPath(pathFacturas + r.FechaComprobante.Year.ToString() + "/" + archivoNombre)))
                            files.Add(pathFacturas + r.FechaComprobante.Year.ToString() + "/" + archivoNombre);
                    }                    

                }

                if (files.Count > 0)
                {
                    using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                    {
                        foreach (string f in files)
                        {
                               zip.AddFile(HttpContext.Current.Server.MapPath(f),"");//Zip file inside filename  
                        }

                        zip.Save((HttpContext.Current.Server.MapPath(pathZip)).Replace("~", ""));//location and name for creating zip file  

                    }
                }
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return  (pathZip).Replace("~","");
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