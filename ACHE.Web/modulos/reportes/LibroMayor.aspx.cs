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


public partial class modulos_reportes_LibroMayor : BasePage
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
    public static ResultadosLibroMayorViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
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
                    ResultadosLibroMayorViewModel resultado = new ResultadosLibroMayorViewModel();
                    resultado.TotalPage = ((results.GroupBy(x => x.Codigo).Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.GroupBy(x => x.Codigo).Count();

                    resultado.Asientos = results.GroupBy(x => x.Codigo).OrderBy(x => x.FirstOrDefault().Codigo).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new CuentasViewModel()
                        {
                            nroCuenta = x.FirstOrDefault().Codigo,
                            NombreCuenta = x.FirstOrDefault().Nombre,
                            Items = DetalleComprobantesMayor(x.FirstOrDefault().Codigo, results.ToList()),
                            TotalDebe = (x.Sum(y => y.Debe)).ToString("N2"),
                            TotalHaber = (x.Sum(y => y.Haber)).ToString("N2"),
                            Saldo = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2")
                        }).ToList();

                    resultado.TotalDebe = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalDebe)).ToString("N2");
                    resultado.TotalHaber = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalHaber)).ToString("N2");
                    resultado.TotalSaldo = resultado.Asientos.Sum(x => Convert.ToDecimal(x.Saldo)).ToString("N2");
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

    private static List<DetalleComprobantesMayorViewModel> DetalleComprobantesMayor(string Codigo, List<rptImpositivoLibroDiario> results)
    {
        var lista = results.Where(x => x.Codigo == Codigo).OrderBy(x => x.Fecha).Select(x => new DetalleComprobantesMayorViewModel()
        {
            Fecha = x.Fecha.ToString("dd/MM/yyyy"),
            Leyenda = x.Leyenda,
            Debe = x.Debe.ToString("N2"),
            Haber = x.Haber.ToString("N2")
        }).ToList();

        decimal total = 0;
        for (int i = 0; i < lista.Count; i++)
        {
            total += Convert.ToDecimal(lista[i].Debe) - Convert.ToDecimal(lista[i].Haber);
            lista[i].Saldo = total;
        }
        return lista;
    }

    [WebMethod(true)]
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "LibroMayor";
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

                    //var listaAux = results.OrderBy(x => x.Codigo).ToList().Select(x => new LibroMayorExport()
                    //{
                    //    Codigo = x.Codigo,
                    //    Nombre = x.Nombre,
                    //    Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                    //    Leyenda = x.Leyenda,
                    //    Debe = x.Debe,
                    //    Haber = x.Haber,
                    //}).ToList();

                    //decimal total = 0;
                    //for (int i = 0; i < listaAux.Count; i++)
                    //{
                    //    total += Convert.ToDecimal(listaAux[i].Debe) - Convert.ToDecimal(listaAux[i].Haber);
                    //    listaAux[i].Saldo = total;
                    //}


                    var listaCuentas = results.GroupBy(x => x.Codigo).OrderBy(x => x.FirstOrDefault().Codigo).ToList()
                       .Select(x => new CuentasViewModel()
                       {
                           nroCuenta = x.FirstOrDefault().Codigo,
                           NombreCuenta = x.FirstOrDefault().Nombre,
                           Items = DetalleComprobantesMayor(x.FirstOrDefault().Codigo, results.ToList()),
                           TotalDebe = (x.Sum(y => y.Debe)).ToString("N2"),
                           TotalHaber = (x.Sum(y => y.Haber)).ToString("N2"),
                           Saldo = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2")
                       }).ToList();


                    List<LibroMayorExport> listaFinal = new List<LibroMayorExport>();
                    LibroMayorExport asiento;
                    foreach (var item in listaCuentas)
                    {
                        asiento = new LibroMayorExport();
                        asiento.Codigo = item.nroCuenta;
                        asiento.Nombre = item.NombreCuenta;
                        listaFinal.Add(asiento);
                        foreach (var itemC in item.Items)
                        {
                            asiento = new LibroMayorExport();
                            asiento.Fecha = itemC.Fecha;
                            asiento.Leyenda = itemC.Leyenda;
                            asiento.Debe = Convert.ToDecimal(itemC.Debe);
                            asiento.Haber = Convert.ToDecimal(itemC.Haber);
                            asiento.Saldo = itemC.Saldo;
                            listaFinal.Add(asiento);
                        }
                    }

                    dt = listaFinal.ToList().Select(x => new LibroMayorExport()
                    {
                        Codigo = x.Codigo,
                        Nombre = x.Nombre,
                        Fecha = x.Fecha,
                        Leyenda = x.Leyenda,
                        Debe = x.Debe,
                        Haber = x.Haber,
                        Saldo = x.Saldo
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