
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Data;
using System.IO;
using ACHE.Negocio.Facturacion;
using ACHE.Model;
using System.Web.Services;
using ACHE.Extensions;

public partial class cobranzas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.ComercialCobranzas)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");


                var TieneDatos = dbContext.Cobranzas.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
                CobranzasCommon.EliminarCobranza(id, usu);
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
    public static ResultadosComprobantesViewModel getResults(int idPersona, string condicion, string periodo,
        string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                return CobranzasCommon.ObtenerCobranzas(condicion, periodo, fechaDesde, fechaHasta, page, pageSize, usu);
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
    public static ResultadosComprobantesViewModel getResultsCobranzasVinculadas(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                return CobranzasCommon.ObtenerCobranzasVinculadas(id, usu);
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
    public static ResultadosComprobantesViewModel getResultsChequesVinculados(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                return CobranzasCommon.ObtenerChequesVinculados(id, usu);
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
    public static string export(int idPersona, string condicion, string periodo, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Cobranzas";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Cobranzas.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    Int32 numero = 0;
                    if (Int32.TryParse(condicion, out numero))
                    {
                        results = results.Where(x => x.Numero == numero);
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
                        results = results.Where(x => x.FechaCobranza >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaCobranza <= dtHasta);
                    }


                    var listaCobranzas = (from cob in results
                                            join cobDet in dbContext.CobranzasDetalle on cob.IDCobranza equals cobDet.IDCobranza
                                            join com in dbContext.Comprobantes on cobDet.IDComprobante equals com.IDComprobante
                                            //join cobFor in dbContext.CobranzasFormasDePago on cob.IDCobranza equals cobFor.IDCobranza
                                            select new
                                            {
                                                cob.Personas.NombreFantansia,
                                                cob.Personas.RazonSocial,
                                                cob.TipoDocumento,
                                                cob.NroDocumento,
                                                cob.FechaCobranza,
                                                cob.Tipo,              
                                                DetalleCodigoComprobante = com.IDComprobante,
                                                DetalleNombreComprobante = com.Nombre,
                                                DetalleImporteComprobante = cobDet.Importe,
                                                cob.PuntosDeVenta.Punto,
                                                cob.Numero,                                             
                                                cob.ImporteTotal,
                                                cob.Observaciones
                                            }).ToList();



                    dt = listaCobranzas.OrderBy(x => x.FechaCobranza).ToList().Select(x => new
                    {
                        RazonSocial = (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper()),
                        Documento = x.TipoDocumento + " " + x.NroDocumento,
                        Fecha = x.FechaCobranza.ToString("dd/MM/yyyy"),
                        Tipo = x.Tipo == "SIN" ? "" : "RC",
                        //Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                        //Numero = x.Tipo == "SIN" ? "" : x.Numero.ToString("#00000000"),
                        Numero = x.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),                        
                        ImporteTotalNeto = x.ImporteTotal,
                        Observaciones = x.Observaciones,
                        x.DetalleCodigoComprobante,
                        x.DetalleNombreComprobante,
                        x.DetalleImporteComprobante                        
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