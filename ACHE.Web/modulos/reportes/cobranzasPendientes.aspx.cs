using ACHE.Model;
using System;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;
using ACHE.Model.ViewModels;
using ACHE.Extensions;
using Aspose.Pdf.Operators;
using System.Security.Cryptography;

public partial class modulos_reportes_cobranzasPendientes : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionCobranzaPendientes)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            //txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            //txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptCobranzasPendientesViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var tipoComprobantesNoValidos = new[] { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "PDC", "EDA", "DDC" };

                    var results = dbContext.RptCobranzasPendientes.Where(x => x.IDUsuario == usu.IDUsuario && !tipoComprobantesNoValidos.Contains(x.tipo)).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaComprobante <= dtHasta);
                    }

                    page--;
                    ResultadosRptCobranzasPendientesViewModel resultado = new ResultadosRptCobranzasPendientesViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.FechaComprobante).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptCobranzasPendientesViewModel()
                        {
                            Fecha = x.FechaComprobante.ToString("dd/MM/yyyy"),
                            RazonSocial = x.RazonSocial,
                            NroDocumento = (x.NroDocumento == "") ? "-" : x.TipoDocumento + " " + x.NroDocumento,
                            CondicionIVA = x.CondicionIVA,
                            NroFactura = x.tipo + " " + x.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            Importe = x.Importe.ToString("N2"),
                            Iva = x.Iva.ToString("N2"),
                            importeTotal = x.ImporteTotal.ToString("N2"),
                            Saldo = x.Saldo.ToString("N2")
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
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
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
                    var tipoComprobantesNoValidos = new[] { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "PDC", "EDA", "DDC" };

                    var results = dbContext.RptCobranzasPendientes.Where(x => x.IDUsuario == usu.IDUsuario && !tipoComprobantesNoValidos.Contains(x.tipo)).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaComprobante <= dtHasta);
                    }

                    dt = results.OrderBy(x => x.FechaComprobante).ToList().Select(x => new
                    {
                        Fecha = x.FechaComprobante.ToString("dd/MM/yyyy"),
                        RazonSocial = x.RazonSocial,
                        TipoDocumento = x.TipoDocumento,
                        NroDocumento = x.NroDocumento,
                        CondicionIVA = x.CondicionIVA,
                        NroFactura = x.tipo + " " + x.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                        Importe = x.Importe,
                        Iva = x.Iva,
                        importeTotal = x.ImporteTotal,
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