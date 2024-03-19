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

public partial class modulos_reportes_rnk_conceptos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionRankingPorProductoServicio)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptRnkViewModel getResults(string tipo, string estado, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptRankingConceptos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (tipo != string.Empty)
                        results = results.Where(x => x.Tipo == tipo);
                    if (estado != string.Empty)
                        results = results.Where(x => x.Estado == estado);
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

                    page--;
                    ResultadosRptRnkViewModel resultado = new ResultadosRptRnkViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    IQueryable<RptRankingConceptos> listaNC = results.Where(x => x.tipoFC == "NCA" || x.tipoFC == "NCB" || x.tipoFC == "NCC");
                    var listaFinal= RestarCantidadNC(listaNC, results.Except(listaNC)).ToList();

                    var aux = listaFinal.GroupBy(x => new { x.Codigo, x.Descripcion })
                     .Select(g => new                      {
                         Codigo = g.Key.Codigo.ToUpper(),
                         Descripcion = g.Key.Descripcion,
                         Cantidad = g.Sum(x => x.Cantidad),
                         Importe = g.Sum(x => x.Importe)
                     }).ToList();


                    var list = aux.OrderByDescending(x => x.Importe).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptRnkViewModel()
                        {
                            Valor1 = x.Descripcion,
                            Valor2 = x.Codigo,
                            Cantidad = x.Cantidad.ToString(),
                            Total = x.Importe.ToString()
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

    private static IQueryable<RptRankingConceptos> RestarCantidadNC(IQueryable<RptRankingConceptos> listaNC, IQueryable<RptRankingConceptos> resultados)
    {
        foreach (var nc in listaNC)
        {
            foreach (var item in resultados)
            {
                if(item.Codigo == nc.Codigo)
                {
                    item.Cantidad -= nc.Cantidad;
                    item.Importe -= nc.Importe;
                    break;
                }
            }
        }
        return resultados;
    }

    [WebMethod(true)]
    public static string export(string tipo, string estado, string fechaDesde, string fechaHasta)
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
                    var results = dbContext.RptRankingConceptos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (tipo != string.Empty)
                        results = results.Where(x => x.Tipo == tipo);
                    if (estado != string.Empty)
                        results = results.Where(x => x.Estado == estado);
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

                    var aux = results.GroupBy(x => new { x.Codigo, x.Descripcion })
                     .Select(g => new
                     {
                         Codigo = g.Key.Codigo.ToUpper(),
                         Descripcion = g.Key.Descripcion,
                         Cantidad = g.Sum(x => x.Cantidad),
                         Importe = g.Sum(x => x.Importe)
                     }).ToList();

                    dt = aux.OrderByDescending(x => x.Importe).ToList().Select(x => new
                    {
                        Concepto = x.Descripcion,
                        Codigo = x.Codigo,
                        Cantidad = x.Cantidad.ToString(),
                        Total = x.Importe.ToString()
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