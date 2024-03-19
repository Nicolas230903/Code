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

public partial class modulos_reportes_stock_detalle : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionStockDetalle)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptStockDetalleViewModel getResults(int idConcepto, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.StockAuditoria.Where(x => x.IdConcepto == idConcepto).AsQueryable();

                    page--;
                    ResultadosRptStockDetalleViewModel resultado = new ResultadosRptStockDetalleViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var aux = results
                     .Select(g => new
                     {
                         Nombre = g.Conceptos.Nombre,
                         Codigo = g.Conceptos.Codigo,
                         Accion = g.Accion,
                         StockAnterior = g.StockAnterior,
                         Cantidad = g.Cantidad,
                         StockNuevo = g.StockNuevo,
                         FechaAlta = g.FechaAlta,
                         Fecha = g.FechaAlta,
                         Usuario = g.Usuarios.RazonSocial
                     }).ToList();


                    var list = aux.OrderByDescending(x => x.FechaAlta).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptStockDetalleViewModel()
                        {
                            Nombre = x.Nombre,
                            Codigo = x.Codigo,
                            Accion = x.Accion,
                            Cantidad = x.Cantidad.ToString("N2"),
                            StockAnterior = x.StockAnterior.ToString("N2"),
                            StockActual = x.StockNuevo.ToString("N2"),
                            Fecha = x.Fecha.ToString(),
                            Usuario = x.Usuario
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
    public static string export(int idConcepto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "RankingClientes";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.StockAuditoria.Where(x => x.IdConcepto == idConcepto).AsQueryable();

                    var aux = results
                     .Select(g => new
                     {
                         Nombre = g.Conceptos.Nombre,
                         Codigo = g.Conceptos.Codigo,
                         Accion = g.Accion,
                         StockAnterior = g.StockAnterior,
                         Cantidad = g.Cantidad,
                         StockNuevo = g.StockNuevo,
                         FechaAlta = g.FechaAlta,
                         Fecha = g.FechaAlta,
                         Usuario = g.Usuarios.RazonSocial
                     }).ToList();

                    dt = aux.OrderBy(x => x.FechaAlta).ToList().Select(x => new
                    {
                        Nombre = x.Nombre,
                        Codigo = x.Codigo,
                        Accion = x.Accion,
                        Cantidad = x.Cantidad.ToString("N2"),
                        StockAnterior = x.StockAnterior.ToString("N2"),
                        StockActual = x.StockNuevo.ToString("N2"),
                        Fecha = x.Fecha.ToString(),
                        Usuario = x.Usuario
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