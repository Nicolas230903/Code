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

public partial class modulos_reportes_rnk_clientes : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionRankingPorCliente)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptRnkViewModel getResults(string provincia, string ciudad, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptRankingClientes.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (provincia != "")
                    {
                        var idProvincia = Convert.ToInt32(provincia);
                        results = results.Where(x => x.IDProvincia == idProvincia);
                    }
                    if (ciudad != "")
                    {
                        var idCiudad = Convert.ToInt32(ciudad);
                        results = results.Where(x => x.IDCiudad == idCiudad);
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

                    page--;
                    ResultadosRptRnkViewModel resultado = new ResultadosRptRnkViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    IQueryable<RptRankingClientes> listaNC = results.Where(x => x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC");
                    var listaFinal = RestarCantidadNC(listaNC, results.Except(listaNC)).ToList();

                    var aux = listaFinal.GroupBy(x => new { x.IDPersona, x.RazonSocial, x.CUIT, })
                     .Select(g => new                      {
                         RazonSocial = g.Key.RazonSocial.ToUpper(),
                         CUIT = g.Key.CUIT,
                         Cantidad = g.Sum(x => x.Cantidad),
                         Importe = g.Sum(x => x.Importe)
                     }).ToList();


                    var list = aux.OrderByDescending(x => x.Importe).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptRnkViewModel()
                        {
                            Valor1 = x.RazonSocial,
                            Valor2 = x.CUIT,
                            Cantidad = x.Cantidad.ToString(),
                            Total = x.Importe.ToString("N2")
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
    public static string export(string provincia, string ciudad, string fechaDesde, string fechaHasta)
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
                    var results = dbContext.RptRankingClientes.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (provincia != string.Empty)
                        results = results.Where(x => x.Provincia == provincia);
                    if (ciudad != string.Empty)
                        results = results.Where(x => x.Ciudad.ToLower().Contains(ciudad.ToLower()));
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

                    IQueryable<RptRankingClientes> listaNC = results.Where(x => x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC");
                    var listaFinal = RestarCantidadNC(listaNC, results.Except(listaNC)).ToList();

                    var aux = listaFinal.GroupBy(x => new { x.IDPersona, x.RazonSocial, x.CUIT, })
                     .Select(g => new
                     {
                         RazonSocial = g.Key.RazonSocial.ToUpper(),
                         CUIT = g.Key.CUIT,
                         Cantidad = g.Sum(x => x.Cantidad),
                         Importe = g.Sum(x => x.Importe)
                     }).ToList();


                    dt = aux.OrderByDescending(x => x.Importe).ToList().Select(x => new 
                    {
                        RazonSocial = x.RazonSocial,
                        CUIT = x.CUIT,
                        Cantidad = x.Cantidad.ToString(),
                        Total = x.Importe
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

    private static IQueryable<RptRankingClientes> RestarCantidadNC(IQueryable<RptRankingClientes> listaNC, IQueryable<RptRankingClientes> resultados)
    {
        foreach (var nc in listaNC)
        {
            foreach (var item in resultados)
            {
                if (item.IDPersona == nc.IDPersona)
                {
                    item.Cantidad -= nc.Cantidad;
                    item.Importe -= nc.Importe;
                    break;
                }
            }
        }
        return resultados;
    }
}