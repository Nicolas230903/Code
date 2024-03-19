using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;

public partial class modulos_reportes_cuentasPagar : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionCuentasAPagar)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptCuentasPagarViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize, string tipoVencimiento)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.Saldo != 0).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaEmision >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaEmision <= dtHasta);
                    }

                    var fecha = DateTime.Now.Date;
                    switch (tipoVencimiento)
                    {
                        case "Vencidas":
                            results = results.Where(x => x.FechaPrimerVencimiento <= fecha || x.FechaSegundoVencimiento <= fecha);
                            break;   
                        case "Vencidas al 1° vencimiento":
                            results = results.Where(x => x.FechaPrimerVencimiento <= fecha);
                            break;
                        case "Vencidas al 2° vencimiento":
                            results = results.Where(x => x.FechaSegundoVencimiento <= fecha);
                            break;
                        case "Proximas a vencer":
                            results = results.Where(x => x.FechaPrimerVencimiento >= fecha || x.FechaSegundoVencimiento >= fecha);
                            break;
                        case "Proximas a vencer al 1° vencimiento":
                            results = results.Where(x => x.FechaPrimerVencimiento >= fecha);
                            break;
                        case "Proximas a vencer al 2° vencimiento":
                            results = results.Where(x => x.FechaSegundoVencimiento >= fecha);
                            break;
                    }

                    page--;
                    ResultadosRptCuentasPagarViewModel resultado = new ResultadosRptCuentasPagarViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.FechaEmision).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptCuentasPagarViewModel()
                        {
                            Fecha = x.FechaEmision.ToString("dd/MM/yyyy"),
                            Proveedor = x.Personas.RazonSocial,
                            TipoDocumento = x.Personas.TipoDocumento,
                            NroDocumento = x.Personas.NroDocumento,
                            CondicionIVA = x.Personas.CondicionIva,
                            NroFactura = x.NroFactura,
                            Saldo = x.Saldo.ToString("N2"),
                            FechaPrimerVencimiento = (x.FechaPrimerVencimiento != null) ? Convert.ToDateTime(x.FechaPrimerVencimiento).ToString("dd/MM/yyyy") : "",
                            FechaSegundoVencimiento = (x.FechaSegundoVencimiento != null) ? Convert.ToDateTime(x.FechaSegundoVencimiento).ToString("dd/MM/yyyy") : ""
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
    public static string export(int idPersona, string fechaDesde, string fechaHasta, string tipoVencimiento)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Pago_a_proveedores";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.Saldo != 0).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaAlta >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaAlta <= dtHasta);
                    }

                    dt = results.OrderBy(x => x.FechaAlta).ToList().Select(x => new
                    {
                        Fecha = x.FechaAlta.ToString("dd/MM/yyyy"),
                        Proveedor = x.Personas.RazonSocial,
                        TipoDocumento = x.Personas.TipoDocumento,
                        NroDocumento = x.Personas.NroDocumento,
                        CondicionIVA = x.Personas.CondicionIva,
                        NroFactura = x.NroFactura,
                        Saldo = x.Saldo
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