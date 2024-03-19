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

public partial class modulos_reportes_iva_compras : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoImpositivosIVACompras)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptIvaComprasViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];


                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptIvaCompras.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
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
                    ResultadosRptIvaComprasViewModel resultado = new ResultadosRptIvaComprasViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptIvaComprasViewModel()
                        {
                            Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                            Tipo = x.Tipo,
                            NroFactura = x.Factura,
                            RazonSocial = x.RazonSocial,
                            Cuit = x.CUIT,

                            MontoGravadoIva2 = x.Importe2.ToString("N2"),
                            MontoGravadoIva5 = x.Importe5.ToString("N2"),
                            MontoGravadoIva10 = x.Importe10.ToString("N2"),
                            MontoGravadoIva21 = x.Importe21.ToString("N2"),
                            MontoGravadoIva27 = x.Importe27.ToString("N2"),
                            MontoNoGravadoYExentos = x.CondicionIVA == "EX" ? (x.TotalImporte.Value).ToString("N2") : x.NoGravado.ToString("N2"),
                            MontoGravadoMonotributistas = x.ImporteMon.ToString("N2"),
                            IvaFacturado = (x.Iva).ToString("N2"),
                            //IvaPercepcion = x.Percepciones.ToString("N2"),
                            
                            ImpInterno = x.ImpInterno.ToString("N2"),
                            ImpMunicipal = x.ImpMunicipal.ToString("N2"),
                            ImpNacional = x.ImpNacional.ToString("N2"),
                            Otros = x.Otros.ToString("N2"),
                            PercepcionIVA = x.PercepcionIVA.ToString("N2"),
                            IIBB = x.IIBB.ToString("N2"),
                            TotalFacturado = (x.Iva + x.TotalImporte.Value + x.Percepciones).ToString("N2"),
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

            string fileName = "IvaCompras";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptIvaCompras.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
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
                        Tipo = x.Tipo,
                        NroFactura = x.Factura,
                        RazonSocial = x.RazonSocial,
                        Cuit = x.CUIT,
                        //CondicionIVA = x.CondicionIVA,
                        //Importe = x.Importe.ToString("N2"),
                        MontoGravadoIva2 = x.Importe2,
                        MontoGravadoIva5 = x.Importe5,
                        MontoGravadoIva10 = x.Importe10,
                        MontoGravadoIva21 = x.Importe21,
                        MontoGravadoIva27 = x.Importe27,
                        MontoNoGravadoYExentos = x.CondicionIVA == "EX" ? (x.TotalImporte.Value) : x.NoGravado + x.Exento,
                        MontoGravadoMonotributistas = x.ImporteMon,
                        IvaFacturado = x.Iva,
                        //IvaPercepcion = +x.Percepciones,
                        ImpInterno = x.ImpInterno,
                        ImpMunicipal = x.ImpMunicipal,
                        ImpNacional = x.ImpNacional,
                        Otros = x.Otros,
                        PercepcionIVA = x.PercepcionIVA,
                        IIBB = x.IIBB,
                        TotalFacturado = (x.Iva + x.TotalImporte.Value + x.Percepciones),
                        Rubro = x.Rubro != null ? x.Rubro : "",
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