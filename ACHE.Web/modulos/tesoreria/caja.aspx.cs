using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Data;
using System.IO;
using System.Web.Services;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Tesoreria;
using ACHE.Extensions;

public partial class modulos_Tesoreria_caja : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionCaja)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                hdnIDUsuario.Value = usu.IDUsuario.ToString();
                var TieneDatos = dbContext.CajaView.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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

    [WebMethod(true)]
    public static void delete(int id, string motivo)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                CajaCommon.EliminarCaja(id, motivo, usu);
            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }


    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosCajaViewModel getResults(string tipoMovimiento, string fechaDesde, string fechaHasta, string periodo, string medioDePago, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return CajaCommon.ObtenerCaja(tipoMovimiento, fechaDesde, fechaHasta, periodo, medioDePago, page, pageSize, usu);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(string tipoMovimiento, string fechaDesde, string fechaHasta, string periodo, string medioDePago)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Tesoreria";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Caja.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (tipoMovimiento != string.Empty)
                        results = results.Where(x => x.TipoMovimiento == tipoMovimiento);

                    if (!medioDePago.Equals("Todos"))
                        results = results.Where(x => x.MedioDePago == medioDePago);

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



                    var lista = results.OrderByDescending(x => x.IDCaja).ToList().Select(x => new CajaViewModel()
                    {
                        ID = x.IDCaja,
                        tipoMovimiento = x.TipoMovimiento,
                        Fecha = Convert.ToDateTime(x.Fecha).ToString("dd/MM/yyyy"),
                        Concepto = (x.ConceptosCaja == null) ? "" : x.ConceptosCaja.Nombre,
                        Estado = x.Estado,
                        FechaAnulacion = (!string.IsNullOrWhiteSpace(x.FechaAnulacion.ToString())) ? Convert.ToDateTime(x.FechaAnulacion).ToString("dd/MM/yyyy") : "",
                        Ingreso = (x.TipoMovimiento == "Ingreso") ? Math.Abs(x.Importe).ToString("N2") : "0",
                        Egreso = (x.TipoMovimiento == "Egreso") ? (-1 * Math.Abs(x.Importe)).ToString("N2") : "0",
                        MedioDePago = x.MedioDePago,
                        Ticket = x.Ticket,
                        Observaciones = x.Observaciones
                    }).ToList();

                    decimal saldo = 0;
                    for (int i = lista.Count - 1; i >= 0; i--)
                    {
                        var ingreso = Convert.ToDecimal((lista[i].Ingreso == "") ? "0" : lista[i].Ingreso);
                        var egreso = Convert.ToDecimal((lista[i].Egreso == "") ? "0" : lista[i].Egreso);
                        saldo = saldo + (Math.Abs(ingreso) - Math.Abs(egreso));
                        lista[i].Saldo = saldo.ToString("N2");
                    }

                    dt = lista.OrderByDescending(x => x.ID).ToList().Select(x => new
                    {
                        tipoMovimiento = x.tipoMovimiento,
                        Fecha = Convert.ToDateTime(x.Fecha).ToString("dd/MM/yyyy"),
                        Concepto = x.Concepto,
                        Estado = x.Estado,
                        FechaAnulacion = x.FechaAnulacion,
                        MedioDePago = x.MedioDePago,
                        Ticket = x.Ticket,
                        Observaciones = x.Observaciones,
                        Debe = Convert.ToDecimal(x.Ingreso),
                        Haber = Convert.ToDecimal(x.Egreso),
                        Saldo = Convert.ToDecimal(x.Saldo)
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
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string imprimir(string tipoMovimiento, string fechaDesde, string fechaHasta, string periodo, string medioDePago, int page, int pageSize)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            var fileNameCaja = "Caja_" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss") + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";

            try
            {
                ResultadosCajaViewModel r = new ResultadosCajaViewModel();
                r = CajaCommon.ObtenerCaja(tipoMovimiento, fechaDesde, fechaHasta, periodo, medioDePago, page, pageSize, usu);

                if (r.TotalItems > 0)
                {
                    var pathFileNameCaja = HttpContext.Current.Server.MapPath("~/files/caja/" + usu.IDUsuario.ToString() + "/" + fileNameCaja);
                    if (!System.IO.File.Exists(pathFileNameCaja))
                    {
                        Common.GenerarCaja(usu, r, fileNameCaja);
                    }
                    return fileNameCaja;
                }                    
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");               

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
    [ScriptMethod(UseHttpGet = true)]
    public static void cerrarCajas()
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    dbContext.Database.ExecuteSqlCommand("update Caja set Estado ='Conciliado', EstadoFecha = GETDATE() where idUsuario=" + usu.IDUsuario);
                    dbContext.Database.ExecuteSqlCommand("update Cobranzas set EstadoCaja ='Conciliado', EstadoCajaFecha = GETDATE() where idUsuario=" + usu.IDUsuario);
                    dbContext.Database.ExecuteSqlCommand("update Pagos set EstadoCaja ='Conciliado', EstadoCajaFecha = GETDATE() where idUsuario=" + usu.IDUsuario);
                    dbContext.Database.ExecuteSqlCommand("update MovimientoDeFondos set EstadoCaja ='Conciliado', EstadoCajaFecha = GETDATE() where idUsuario=" + usu.IDUsuario);
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
    public static string ObtenerTotalSinConsolidar()
    {
        var resultado = "0";
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listaConsolidados = dbContext.CajaView.Where(x => x.Estado == "Cargado" && x.IDUsuario == usu.IDUsuario).ToList();

                    if (listaConsolidados.Count() > 0)
                    {
                        var Ingreso = listaConsolidados.Where(x => x.TipoMovimiento == "Ingreso").Sum(x => Math.Abs(x.Importe));
                        var Egreso = listaConsolidados.Where(x => x.TipoMovimiento == "Egreso").Sum(x => Math.Abs(x.Importe));
                        resultado = (Ingreso - Egreso).ToString("N2");
                    }
                }
            }
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
        return resultado;
    }
}