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

public partial class modulos_reportes_stock : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionStockProductos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptRnkViewModel getResults(string estado, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptStock.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (estado != string.Empty)
                        results = results.Where(x => x.Estado == estado);

                    page--;
                    ResultadosRptRnkViewModel resultado = new ResultadosRptRnkViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var aux = results.GroupBy(x => new { x.Codigo, x.Nombre, x.PrecioUnitario })
                     .Select(g => new
                     {
                         Codigo = g.Key.Codigo.ToUpper(),
                         Nombre = g.Key.Nombre,
                         Stock = g.Sum(x => x.Stock),
                         CostoInterno = g.Sum(x=>x.CostoInterno),
                         PrecioUnitario = g.Key.PrecioUnitario,
                         Valor = g.Sum(x => x.PrecioUnitario * x.Stock)
                     }).ToList();


                    var list = aux.OrderByDescending(x => x.Valor).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptRnkViewModel()
                        {
                            Valor1 = x.Nombre,
                            Valor2 = x.Codigo,
                            Cantidad = x.Stock.ToString(),
                            CostoInterno = Convert.ToInt32(x.CostoInterno).ToString("N2"),
                            Precio = x.PrecioUnitario.ToString("N2"),
                            Total = x.Valor.ToString("N2")
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
    public static string export(string estado)
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
                    var results = dbContext.RptStock.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (estado != string.Empty)
                        results = results.Where(x => x.Estado == estado);

                    var aux = results.GroupBy(x => new { x.Codigo, x.Nombre, x.PrecioUnitario })
                     .Select(g => new
                     {
                         Codigo = g.Key.Codigo.ToUpper(),
                         Nombre = g.Key.Nombre,
                         Stock = g.Sum(x => x.Stock),
                         CostoInterno = g.Sum(x => x.CostoInterno),
                         PrecioUnitario = g.Key.PrecioUnitario,
                         Valor = g.Sum(x => x.PrecioUnitario * x.Stock)
                     }).ToList();

                    dt = aux.OrderByDescending(x => x.Valor).ToList().Select(x => new
                    {
                        Nombre = x.Nombre,
                        Codigo = x.Codigo,
                        Stock = x.Stock.ToString(),
                        CostoInterno = Convert.ToInt32(x.CostoInterno),
                        PrecioUnitario = x.PrecioUnitario,
                        Valor = x.Valor
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