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
using System.Globalization;

public partial class modulos_reportes_EstadoResultado : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        txtFechaDesde.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
        txtFechaHasta.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosEstadoResultadoViewModel getResults(string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ObtenerEstadoDeResultado(fechaDesde, fechaHasta, page, pageSize, usu.IDUsuario);
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

    #region ObtenerEstadoDeResultado
    private static ResultadosEstadoResultadoViewModel ObtenerEstadoDeResultado(string fechaDesde, string fechaHasta, int page, int pageSize, int idUsuario)
    {
        try
        {
            ResultadosEstadoResultadoViewModel resultado = new ResultadosEstadoResultadoViewModel();
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.ConfiguracionPlanDeCuenta.Any(x => x.IDUsuario == idUsuario))
                {
                    var cuentas = ObtenerCuentasFiltros(dbContext, idUsuario);
                    var results = dbContext.RptEstadoResultadoView.Where(x => x.IDUsuario == idUsuario && cuentas.Contains(x.IDPlanDeCuenta)).AsQueryable();
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

                    var ListaCuentas = results.ToList();
                    page--;

                    var listaAux = ListaCuentas.GroupBy(x => new { x.Fecha.Month, x.Fecha.Year, x.Cuenta }).Select(x => new EstadoResultadoViewModel()
                    {
                        Cuenta = x.FirstOrDefault().Cuenta,
                        importe = Convert.ToDecimal(x.Where(y => y.TipoDeAsiento == "D").Sum(y => y.Importe) - x.Where(y => y.TipoDeAsiento == "H").Sum(y => y.Importe)),
                        fecha = x.Select(y => y.Fecha.Year.ToString() + y.Fecha.Month.ToString()).FirstOrDefault(),
                        NombreMeses = MonthName(x.FirstOrDefault().Fecha.Month, x.FirstOrDefault().Fecha.Year)
                    }).ToList();

                    var Listathead = listaAux.GroupBy(x => x.fecha).Select(x => new MesesEstadoResultado()
                    {
                        Nombre = x.FirstOrDefault().NombreMeses
                    }).ToList();

                    var ListaTbody = listaAux.GroupBy(x => x.Cuenta).Select(x => new CuentasEstadoResultado()
                    {
                        Nombre = x.FirstOrDefault().Cuenta,
                        ListaImportes = InsertarImportesCuentas(listaAux, x.FirstOrDefault().Cuenta, Listathead)
                    }).ToList();

                    resultado.TotalPage = ((ListaTbody.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = ListaTbody.Count();

                    resultado.Listathead = Listathead;
                    resultado.ListaTbody = ListaTbody;
                }
                return resultado;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private static List<Int32> ObtenerCuentasFiltros(ACHEEntities dbContext, int idUsuario)
    {
        List<Int32> cuentas = new List<Int32>();
        var idctasVentas = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == idUsuario).FirstOrDefault().CtasFiltroVentas.Split(',');
        for (int i = 0; i < idctasVentas.Length; i++)
            cuentas.Add(Convert.ToInt32(idctasVentas[i]));
        var idctasCompras = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == idUsuario).FirstOrDefault().CtasFiltroCompras.Split(',');
        for (int i = 0; i < idctasCompras.Length; i++)
            cuentas.Add(Convert.ToInt32(idctasCompras[i]));

        return cuentas;
    }
    private static List<ImportesEstadoResultado> InsertarImportesCuentas(List<EstadoResultadoViewModel> listaAux, string cuenta, List<MesesEstadoResultado> Listathead)
    {
        var lista = new List<ImportesEstadoResultado>();

        foreach (var item in Listathead)
        {
            var resultado = listaAux.Where(x => x.NombreMeses == item.Nombre && x.Cuenta == cuenta).FirstOrDefault();
            if (resultado != null)
                lista.Add(new ImportesEstadoResultado { Importe = Convert.ToDecimal(resultado.importe) });
            else
                lista.Add(new ImportesEstadoResultado { Importe = 0 });
        }

        return lista;
    }
    private static string MonthName(int month, int año)
    {
        DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
        var mes = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtinfo.GetMonthName(month));
        return mes + " " + año.ToString();
    }
    #endregion

    [WebMethod(true)]
    public static string export(string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "EstadoResultado";
            string path = "~/tmp/";
            try
            {
                var listaEstadoR = ObtenerEstadoDeResultado(fechaDesde, fechaHasta, 1, 100000, usu.IDUsuario);

                DataTable dt = new DataTable("Customers");
                DataColumn workCol = new DataColumn();
                workCol.AllowDBNull = false;

                dt.Columns.Add("Cuentas", typeof(String));
                foreach (var item in listaEstadoR.Listathead)
                    dt.Columns.Add(item.Nombre + "-", typeof(String));

                foreach (var item in listaEstadoR.ListaTbody)
                {
                    object[] arrAux = new object[item.ListaImportes.Count + 1];
                    arrAux[0] = item.Nombre;

                    for (int i = 0; i < item.ListaImportes.Count; i++)
                        arrAux[i + 1] = (item.ListaImportes[i].Importe);

                    dt.Rows.Add(arrAux);
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