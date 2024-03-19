using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using ACHE.Model;
using System.Web.Services;
using ACHE.Negocio.Facturacion;
using ACHE.Extensions;

public partial class pagos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {                
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.SuministroPagos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                var TieneDatos = dbContext.Pagos.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
    public static void delete(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                PagosCommon.EliminarPago(id, usu);
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
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosPagosViewModel getResults(string condicion, string periodo, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return PagosCommon.ObtenerPagos(condicion, periodo, fechaDesde, fechaHasta, page, pageSize, usu);
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
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(string condicion, string periodo, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Pagos";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Pagos.Include("Personas").Include("PagosDetalle").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    Int32 numero = 0;
                    if (Int32.TryParse(condicion, out numero))
                    {
                        results = results.Where(x => x.PagosDetalle.Any(y => y.Compras.NroFactura.Contains(condicion)));
                    }
                    else if (condicion != string.Empty)
                    {
                        results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion));
                    }

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
                        results = results.Where(x => x.FechaPago >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaPago <= dtHasta);
                    }


                    var listaPagos = (from pag in results
                                          join pagDet in dbContext.PagosDetalle on pag.IDPago equals pagDet.IDPago
                                          join com in dbContext.Compras on pagDet.IDCompra equals com.IDCompra
                                          select new
                                          {
                                              pag.Personas.NombreFantansia,
                                              pag.Personas.RazonSocial,
                                              pag.FechaPago,
                                              DetalleCodigoCompras = com.IDCompra,
                                              DetalleImporteCompras = pagDet.Importe,
                                              ImporteNeto = pag.ImporteTotal,
                                              Iva = pag.PagosDetalle.Sum(y => y.Compras.Iva),//.ToString("N2"),
                                              NoGravado = (pag.PagosDetalle.Sum(y => y.Compras.NoGravado) + pag.PagosDetalle.Sum(y => y.Compras.Exento)),//.ToString("N2"),
                                              Retenciones = pag.PagosDetalle.Sum(y => y.Compras.TotalImpuestos.Value),
                                              Total = pag.ImporteTotal
                                          }).ToList();           

                    dt = listaPagos.OrderBy(x => x.FechaPago).ToList().Select(x => new
                    {
                        // = x.IDPago,
                        RazonSocial = (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper()),
                        Fecha = x.FechaPago.ToString("dd/MM/yyyy"),                       
                        x.ImporteNeto,
                        x.Iva,
                        x.NoGravado,
                        x.Retenciones,
                        x.Total,
                        x.DetalleCodigoCompras,
                        x.DetalleImporteCompras
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