using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Facturacion;

public partial class compras : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");

            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.SuministroComprobanteDeCompra)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                var TieneDatos = dbContext.Compras.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
                ComprasCommon.EliminarCompra(id, usu);
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
    public static ResultadosComprasViewModel getResults(int idPersona, string condicion, string periodo,
        string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprasCommon.ObtenerCompras(condicion, periodo, fechaDesde, fechaHasta, page, pageSize, usu);
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
    public static string export(int idPersona, string condicion, string periodo, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Compras";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Compras.Include("Personas").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    Int32 numero = 0;
                    if (Int32.TryParse(condicion, out numero))
                    {
                        results = results.Where(x => x.NroFactura.Contains(condicion));
                    }
                    else if (condicion != string.Empty)
                    {
                        results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion));
                    }

                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                    }

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
                        RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                        Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                        Tipo = x.Tipo,
                        NroFactura = x.NroFactura,
                        Importe = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? (x.Total.Value * -1) : x.Total.Value,
                        Iva = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? x.Iva * -1 : x.Iva,
                        NoGravado = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? (-1 * (x.NoGravado + x.Exento)) : (x.NoGravado + x.Exento),
                        Retenciones = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? -1 * x.TotalImpuestos.Value : x.TotalImpuestos.Value,
                        Total = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? (-1 * (x.TotalImpuestos.Value + x.Total.Value + x.Iva)) : (x.TotalImpuestos.Value + x.Total.Value + x.Iva),
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