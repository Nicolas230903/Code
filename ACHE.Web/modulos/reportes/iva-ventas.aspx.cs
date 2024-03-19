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

public partial class modulos_reportes_iva_ventas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoImpositivosIVAVentas)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptIvaVentasViewModel getResults(int idPersona, string fechaDesde, 
        string fechaHasta, int page, int pageSize, int idPuntoVenta, int idActividad, string condicionIVA)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];


                using (var dbContext = new ACHEEntities())
                {
                    string[] listaTipo = { "FCA", "FCB", "FCC", "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
                    string[] tipoComprobanteNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };

                    var results = dbContext.RptIvaVentas.Where(x => x.IDUsuario == usu.IDUsuario && listaTipo.Contains(x.Tipo)).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (idPuntoVenta > 0)
                        results = results.Where(x => x.IDPuntoVenta == idPuntoVenta);
                    if (idActividad > 0)
                        results = results.Where(x => x.IdActividad == idActividad);
                    if (condicionIVA != string.Empty)
                        results = results.Where(x => x.CondicionIva.Equals(condicionIVA));
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
                    ResultadosRptIvaVentasViewModel resultado = new ResultadosRptIvaVentasViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptIvaVentasViewModel()
                        {
                            Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                            RazonSocial = x.RazonSocial,
                            Cuit = x.CUIT,
                            NroFactura = x.Tipo + " " + x.PuntoVenta.ToString("#0000") + "-" + x.Factura.ToString("#00000000"),
                            CondicionIVA = x.CondicionIva,
                            Importe = (!tipoComprobanteNotas.Contains(x.Tipo)) ? x.ImporteBruto.ToString("N2") : "-" + x.ImporteBruto.ToString("N2"),
                            IVA2 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA2).ToString("N2") : "-" + Convert.ToDecimal(x.IVA2).ToString("N2"),
                            IVA10 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA10).ToString("N2") : "-" + Convert.ToDecimal(x.IVA10).ToString("N2"),
                            IVA5 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA5).ToString("N2") : "-" + Convert.ToDecimal(x.IVA5).ToString("N2"),
                            IVA21 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA21).ToString("N2") : "-" + Convert.ToDecimal(x.IVA21).ToString("N2"),
                            IVA27 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA27).ToString("N2") : "-" + Convert.ToDecimal(x.IVA27).ToString("N2"),
                            Iva = (!tipoComprobanteNotas.Contains(x.Tipo)) ? x.Iva.ToString("N2") : "-" + x.Iva.ToString("N2"),
                            Total = (!tipoComprobanteNotas.Contains(x.Tipo)) ? x.ImporteNeto.ToString("N2") : "-" + x.ImporteNeto.ToString("N2")
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
    public static string export(int idPersona, string fechaDesde, string fechaHasta
        , int idPuntoVenta, int idActividad, string condicionIVA)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "IvaVentas";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    string[] listaTipo = { "FCA", "FCB", "FCC", "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
                    string[] tipoComprobanteNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };

                    var results = dbContext.RptIvaVentas.Where(x => x.IDUsuario == usu.IDUsuario && listaTipo.Contains(x.Tipo)).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (idPuntoVenta > 0)
                        results = results.Where(x => x.IDPuntoVenta == idPuntoVenta);
                    if (idActividad > 0)
                        results = results.Where(x => x.IdActividad == idActividad);
                    if (condicionIVA != string.Empty)
                        results = results.Where(x => x.CondicionIva.Equals(condicionIVA));
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

                    dt = results.OrderBy(x => x.Fecha).ToList().Select(x => new
                    {
                        Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                        RazonSocial = x.RazonSocial,
                        Cuit = x.CUIT,
                        CondicionIVA = x.CondicionIva,
                        PuntoDeVenta = x.PuntoVenta,
                        Actividad = x.Actividad,
                        NroFactura = x.Tipo + " " + x.PuntoVenta.ToString("#0000") + "-" + x.Factura.ToString("#00000000"),
                        ImporteNetoGravado = (!tipoComprobanteNotas.Contains(x.Tipo)) ? x.ImporteBruto : (x.ImporteBruto * -1),
                        IVA2 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA2) : -Convert.ToDecimal(x.IVA2),
                        IVA10 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA10) : -Convert.ToDecimal(x.IVA10),
                        IVA5 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA5) : -Convert.ToDecimal(x.IVA5),
                        IVA21 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA21) : -Convert.ToDecimal(x.IVA21),
                        IVA27 = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.IVA27) : -Convert.ToDecimal(x.IVA27),
                        TotalIVA = (!tipoComprobanteNotas.Contains(x.Tipo)) ? Convert.ToDecimal(x.Iva) : -Convert.ToDecimal(x.Iva),
                        TotalFacturado = (!tipoComprobanteNotas.Contains(x.Tipo)) ? x.ImporteNeto : -x.ImporteNeto
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