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
using ACHE.Model.ViewModels;
using ACHE.Negocio.Contabilidad;

public partial class modulos_reportes_LibroDiario : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosLibroDiarioViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.rptImpositivoLibroDiario.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
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
                    ResultadosLibroDiarioViewModel resultado = new ResultadosLibroDiarioViewModel();
                    resultado.TotalPage = ((results.GroupBy(x => x.IDAsiento).Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.GroupBy(x => x.IDAsiento).Count();


                    var CantAsientos = 1;
                    resultado.Asientos = results.GroupBy(x => x.IDAsiento).OrderBy(x => x.FirstOrDefault().IDAsiento).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new AsientoViewModel()
                        {
                            IDAsiento = x.FirstOrDefault().IDAsiento,
                            NroAsiento = NroAsiento(ref CantAsientos).ToString(),
                            Fecha = x.FirstOrDefault().Fecha.ToString("dd/MM/yyyy"),
                            Leyenda = x.FirstOrDefault().Leyenda,
                            Items = AsientosLibroDiario(x.FirstOrDefault().IDAsiento, results.ToList()),
                            TotalDebe = x.Sum(y => y.Debe),
                            TotalHaber = x.Sum(y => y.Haber)
                        }).ToList();

                    resultado.TotalDebe = resultado.Asientos.Sum(x => x.TotalDebe).ToString("N2");
                    resultado.TotalHaber = resultado.Asientos.Sum(x => x.TotalHaber).ToString("N2");
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

    private static List<LibroDiarioViewModel> AsientosLibroDiario(int IDAsiento, List<rptImpositivoLibroDiario> results)
    {
        return results.Where(x => x.IDAsiento == IDAsiento).Select(x => new LibroDiarioViewModel()
                                {
                                    nroCuenta = x.Codigo,
                                    NombreCuenta = x.Nombre,
                                    Debe = x.Debe.ToString("N2"),
                                    Haber = x.Haber.ToString("N2"),
                                }).ToList();
    }

    private static int NroAsiento(ref int cantidad)
    {
        return cantidad++;
    }

    [WebMethod(true)]
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "ImpositivoLibroDiario";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.rptImpositivoLibroDiario.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
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

                    var CantAsientos = 1;
                    var listaCuentas = results.GroupBy(x => x.IDAsiento).OrderBy(x => x.FirstOrDefault().IDAsiento).ToList()
                    .Select(x => new AsientoViewModel()
                    {
                        IDAsiento = x.FirstOrDefault().IDAsiento,
                        NroAsiento = NroAsiento(ref CantAsientos).ToString(),
                        Fecha = x.FirstOrDefault().Fecha.ToString("dd/MM/yyyy"),
                        Leyenda = x.FirstOrDefault().Leyenda,
                        Items = AsientosLibroDiario(x.FirstOrDefault().IDAsiento, results.ToList()),
                        TotalDebe = x.Sum(y => y.Debe),
                        TotalHaber = x.Sum(y => y.Haber)
                    }).ToList();

                    List<AsientoExportacionViewModel> listaFinal = new List<AsientoExportacionViewModel>();
                    AsientoExportacionViewModel asiento;
                    foreach (var item in listaCuentas)
                    {
                        asiento = new AsientoExportacionViewModel();
                        asiento.NroAsiento = item.NroAsiento;
                        asiento.Fecha = item.Fecha;
                        asiento.Leyenda = item.Leyenda;
                        listaFinal.Add(asiento);
                        foreach (var itemC in item.Items)
                        {
                            asiento = new AsientoExportacionViewModel();
                            asiento.codigo = itemC.nroCuenta;
                            asiento.NombreCuenta = itemC.NombreCuenta;
                            asiento.Debe = Convert.ToDecimal(itemC.Debe);
                            asiento.Haber = Convert.ToDecimal(itemC.Haber);
                            listaFinal.Add(asiento);
                        }
                    }

                    dt = listaFinal.ToList().Select(x => new AsientoExportacionViewModel()
                    {
                        NroAsiento = x.NroAsiento,
                        Fecha = x.Fecha,
                        Leyenda = x.Leyenda,
                        codigo = x.NombreCuenta,
                        NombreCuenta = x.NombreCuenta,
                        Debe = x.Debe,
                        Haber = x.Haber
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
                    var entity = dbContext.Asientos.Where(x => x.IDAsiento == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        if (entity.IDCobranza != null || entity.IDCompra != null || entity.IDComprobante != null || entity.IDPago != null)
                            throw new Exception("Solo los asientos manuales se pueden eliminar.");
                        else if (!entity.EsAsientoInicio && !entity.EsAsientoCierre && ContabilidadCommon.ValidarCierreContable(usu, entity.Fecha))
                            throw new Exception("El comprobante no puede eliminarse ya que el año contable ya fue cerrado.");
                        dbContext.Asientos.Remove(entity);
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
}