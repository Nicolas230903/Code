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

public partial class gastosGenerales : BasePage
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
                    if (!afu.AdministracionGastosGenerales)
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
                GastosGeneralesCommon.EliminarGastoGeneral(id, usu);
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
    public static ResultadosGastosGeneralesViewModel getResults(string periodo,
        string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return GastosGeneralesCommon.ObtenerGastosGenerales(periodo, fechaDesde, fechaHasta, page, pageSize, usu);
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
    public static string export(string periodo, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "GastosGenerales";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.GastosGenerales.Where(x => x.IdUsuario == usu.IDUsuario).AsQueryable();

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

                    if (!periodo.Equals("-2"))
                    {
                        if (!string.IsNullOrWhiteSpace(fechaDesde))
                        {
                            DateTime dtDesde = DateTime.Parse(fechaDesde);
                            results = results.Where(x => x.Periodo >= dtDesde);
                        }
                        if (!string.IsNullOrWhiteSpace(fechaHasta))
                        {
                            DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                            results = results.Where(x => x.Periodo <= dtHasta);
                        }
                    }

                    dt = results.OrderByDescending(x => x.Periodo).ToList()
                        .Select(x => new GastosGeneralesViewModel()
                        {
                            ID = x.IdGastosGenerales,
                            Periodo = x.Periodo.Year.ToString() + x.Periodo.Month.ToString("D2"),
                            Total = (x.Sueldos + x.SeguridadEHigiene + x.Municipales + x.Monotributos + x.AportesYContribuciones + x.Ganancias12 + x.CreditoBancario + x.RetencionesDeIIBB + x.PlanesAFIP + x.Gastos1 + x.Gastos2 + x.Gastos3).ToString("N2"),
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