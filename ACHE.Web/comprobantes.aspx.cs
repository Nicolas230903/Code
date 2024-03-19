using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Data;
using System.IO;
using ACHE.Negocio.Contabilidad;
using System.Web.Services;
using ACHE.Negocio.Facturacion;
using System.Linq.Dynamic;
using ACHE.FacturaElectronica.WSFacturaElectronica;
using System.Collections.Generic;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using DocumentFormat.OpenXml.Office2013.Excel;
using ACHE.FacturaElectronica.WSPersonaServiceA5v34;

public partial class comprobantes : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            //txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            //txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");

            //ddlModo.Items.Add(new ListItem("Otro", "O"));
            //ddlModo.Items.Add(new ListItem("Talonario preimpreso", "T"));
            //if (CurrentUser.TieneFE)
            //    ddlModo.Items.Add(new ListItem("Electrónica", "E"));
            // ddlModo.Items.Insert(0, new ListItem("Todos", ""));

            var user = (WebUser)Session["CurrentUser"];
            string tipo = Request.QueryString["tipo"];
            hdnTipo.Value = tipo;

            long CantidadRegistrosMaximaParaMostrarTodosLosRegistros = Convert.ToInt64(ConfigurationManager.AppSettings["CantidadRegistrosMaximaParaMostrarTodosLosRegistros"]);

            bool TieneDatos = false;
            long CantidadRegistros = 0;

            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == user.IDUsuario && w.IdUsuarioAdicional == user.IDUsuarioAdicional).FirstOrDefault();

                switch (tipo)
                {
                    case "EDA":
                        if (afu != null)
                            if (!afu.ComercialEntregas)
                                Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                        //TieneDatos = dbContext.Comprobantes.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("EDA"));

                        CantidadRegistros = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("EDA")).Count();
                        if (CantidadRegistros > 0)
                        {
                            TieneDatos = true;
                            if (CantidadRegistros > CantidadRegistrosMaximaParaMostrarTodosLosRegistros)
                                hdnFiltroPorCantidad.Value = "1";
                            else
                                hdnFiltroPorCantidad.Value = "0";
                        }

                        break;
                    case "PDV":
                        if (afu != null)                        
                            if (!afu.ComercialPedidoDeVenta)
                                Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                        //TieneDatos = dbContext.Comprobantes.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("PDV"));

                        CantidadRegistros = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("PDV")).Count();
                        if (CantidadRegistros > 0)
                        {
                            TieneDatos = true;
                            if (CantidadRegistros > CantidadRegistrosMaximaParaMostrarTodosLosRegistros)
                                hdnFiltroPorCantidad.Value = "1";
                            else
                                hdnFiltroPorCantidad.Value = "0";
                        }

                        break;
                    case "PDC":
                        if (afu != null)
                            if (!afu.SuministroComprobanteDeCompra)
                                Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                        //TieneDatos = dbContext.Comprobantes.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("PDC"));

                        CantidadRegistros = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("PDC")).Count();
                        if (CantidadRegistros > 0)
                        {
                            TieneDatos = true;
                            if (CantidadRegistros > CantidadRegistrosMaximaParaMostrarTodosLosRegistros)
                                hdnFiltroPorCantidad.Value = "1";
                            else
                                hdnFiltroPorCantidad.Value = "0";
                        }

                        break;
                    case "DDC":
                        if (afu != null)
                            if (!afu.SuministroComprobanteDeCompra)
                                Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                        //TieneDatos = dbContext.Comprobantes.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("PDC"));

                        CantidadRegistros = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.Tipo.Equals("DDC")).Count();
                        if (CantidadRegistros > 0)
                        {
                            TieneDatos = true;
                            if (CantidadRegistros > CantidadRegistrosMaximaParaMostrarTodosLosRegistros)
                                hdnFiltroPorCantidad.Value = "1";
                            else
                                hdnFiltroPorCantidad.Value = "0";
                        }

                        break;
                    case "FAC":
                        if (afu != null)
                            if (!afu.ComercialFacturaDeVenta)
                                Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                        if (usu.CUIT.Equals("30716909839"))
                            btnAcciones.Visible = false;
                        else
                            btnAcciones.Visible = true;

                        //TieneDatos = dbContext.Comprobantes.Any(x => x.IDUsuario == CurrentUser.IDUsuario && !x.Tipo.Equals("PDV") && !x.Tipo.Equals("PDC") && !x.Tipo.Equals("NDP") && !x.Tipo.Equals("EDA"));

                        CantidadRegistros = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && !x.Tipo.Equals("PDV") && !x.Tipo.Equals("PDC") && !x.Tipo.Equals("NDP") && !x.Tipo.Equals("EDA")).Count();
                        if (CantidadRegistros > 0)
                        {
                            TieneDatos = true;
                            if (CantidadRegistros > CantidadRegistrosMaximaParaMostrarTodosLosRegistros)
                                hdnFiltroPorCantidad.Value = "1";
                            else
                                hdnFiltroPorCantidad.Value = "0";
                        }                        break;
                    default:

                        //TieneDatos = dbContext.Comprobantes.Any(x => x.IDUsuario == CurrentUser.IDUsuario);

                        CantidadRegistros = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario).Count();
                        if (CantidadRegistros > 0)
                        {
                            TieneDatos = true;
                            if (CantidadRegistros > CantidadRegistrosMaximaParaMostrarTodosLosRegistros)
                                hdnFiltroPorCantidad.Value = "1";
                            else
                                hdnFiltroPorCantidad.Value = "0";
                        }

                        break;
                }

                hdnIDUsuario.Value = CurrentUser.IDUsuario.ToString();

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
                ComprobantesCommon.EliminarComprobante(id, usu);
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
    public static ResultadosComprobantesViewModel getResults(string condicion, string periodo, 
                    string fechaDesde, string fechaHasta, int page, 
                    int pageSize, string tipo,
                    bool entregaPendiente,
                    bool entregaEstado,
                    string estado,
                    bool cobranzaPendiente,
                    bool facturaPendiente)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobantes(condicion, periodo, 
                    fechaDesde, fechaHasta, "", page, pageSize, usu, tipo,
                    entregaPendiente, entregaEstado, estado, cobranzaPendiente, facturaPendiente);
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
    public static int getResultsDocumentoRaizDeComprobante(int id)
    {
        string msj = "El comprobante no tiene comprobante vinculado";
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                    Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == id).FirstOrDefault();

                    if (c != null)
                    {
                        if (c.Tipo.Substring(0, 1).Equals("F"))
                        {
                            if (c.IdComprobanteVinculado != null)
                            {
                                Comprobantes c2 = dbContext.Comprobantes.Where(w => w.IDComprobante == c.IdComprobanteVinculado).FirstOrDefault();

                                if (c2 != null)
                                {
                                    if (c2.Tipo.Equals("EDA"))
                                    {
                                        if (c2.IdComprobanteVinculado != null)
                                        {
                                            Comprobantes c3 = dbContext.Comprobantes.Where(w => w.IDComprobante == c2.IdComprobanteVinculado).FirstOrDefault();

                                            if (c3 != null)
                                                return c3.IDComprobante;
                                            else
                                                throw new Exception(msj);
                                        }
                                        else
                                            throw new Exception(msj);
                                    }
                                    else
                                    {
                                        return (int)c2.IdComprobanteVinculado;
                                    }

                                }
                                else
                                    throw new Exception(msj);
                            }
                            else
                                throw new Exception(msj);
                        }


                        if (c.Tipo.Equals("EDA"))
                        {
                            if (c.IdComprobanteVinculado != null)
                            {
                                Comprobantes c2 = dbContext.Comprobantes.Where(w => w.IDComprobante == c.IdComprobanteVinculado).FirstOrDefault();

                                if (c2 != null)
                                {
                                    if (c2.Tipo.Equals("EDA"))
                                    {
                                        if (c2.IdComprobanteVinculado != null)
                                        {
                                            Comprobantes c3 = dbContext.Comprobantes.Where(w => w.IDComprobante == c2.IdComprobanteVinculado).FirstOrDefault();

                                            if (c3 != null)
                                                return c3.IDComprobante;
                                            else
                                                throw new Exception(msj);
                                        }
                                        else
                                            throw new Exception(msj);
                                    }
                                    else
                                    {
                                        return (int)c2.IdComprobanteVinculado;
                                    }
                                }
                                else
                                    throw new Exception(msj);
                            }
                            else
                                throw new Exception(msj);
                        }

                        throw new Exception(msj);

                    }
                    else
                        throw new Exception(msj);
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
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosComprobantesViewModel getResultsDocumentoRaiz(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobante(id, usu);
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
    public static ResultadosComprobantesViewModel getResultsDocumentosRaiz(string tipo)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobanteRaiz(tipo, usu);
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
    public static ResultadosComprobantesViewModel getResultsComprobantesVinculados(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobantesVinculados(id, usu);
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
    public static ResultadosComprobantesViewModel getResultsEntregasVinculadas(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerEntregasVinculadas(id, usu);
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
    public static decimal calcularPendienteCAE(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.CalcularPendienteCAE(id, usu);
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
    public static decimal calcularPendienteFacturar(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.CalcularPendienteFacturar(id, usu);
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
    public static decimal calcularPendienteCobranza(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.CalcularPendienteCobranza(id, usu);
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
    public static decimal calcularPendienteEntrega(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.CalcularPendienteEntrega(id, usu);
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
    public static string export(string condicion, string periodo, string fechaDesde, string fechaHasta, string tipo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Comprobantes";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.ComprobantesDetalle.Include("Comprobantes").Where(x => x.Comprobantes.IDUsuario == usu.IDUsuario).AsQueryable();

                    Int32 numero = 0;
                    if (Int32.TryParse(condicion, out numero))
                        results = results.Where(x => x.Comprobantes.Numero.ToString().Contains(numero.ToString()));
                    else if (condicion.Contains("-"))
                    {
                        var punto = (string.IsNullOrWhiteSpace(condicion.Split("-")[0])) ? 0 : Convert.ToInt32(condicion.Split("-")[0]);
                        var nro = (string.IsNullOrWhiteSpace(condicion.Split("-")[1])) ? 0 : Convert.ToInt32(condicion.Split("-")[1]);
                        results = results.Where(x => x.Comprobantes.Numero == nro && x.Comprobantes.PuntosDeVenta.Punto == punto);
                    }
                    else if (condicion != "")
                        results = results.Where(x => x.Comprobantes.Personas.RazonSocial.Contains(condicion) || x.Comprobantes.Personas.NombreFantansia.Contains(condicion));


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
                        results = results.Where(x => x.Comprobantes.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Comprobantes.FechaComprobante <= dtHasta);
                    }

                    //if (!string.IsNullOrWhiteSpace(tipo))
                    //{
                    //    results = results.Where(x => x.Tipo.Equals(tipo));
                    //}

                    if (!string.IsNullOrWhiteSpace(tipo))
                    {
                        switch (tipo)
                        {
                            case "EDA":
                                results = results.Where(x => x.Comprobantes.Tipo.Equals(tipo));
                                break;
                            case "PDV":
                                results = results.Where(x => x.Comprobantes.Tipo.Equals(tipo));
                                break;
                            case "PDC":
                                results = results.Where(x => x.Comprobantes.Tipo.Equals(tipo));
                                break;
                            case "DDC":
                                results = results.Where(x => x.Comprobantes.Tipo.Equals(tipo));
                                break;
                            case "FAC":
                                results = results.Where(x => !x.Comprobantes.Tipo.Equals("PDV") 
                                        && !x.Comprobantes.Tipo.Equals("PDC") && !x.Comprobantes.Tipo.Equals("NDP") 
                                        && !x.Comprobantes.Tipo.Equals("EDA") && !x.Comprobantes.Tipo.Equals("DDC"));
                                break;
                            default:
                                results = results.Where(x => x.Comprobantes.Tipo.Equals(tipo));
                                break;
                        }

                        //results = results.Where(x => x.Tipo.Equals(tipo));
                    }

                    var listaProveedores = (from comDet in results
                                            join con in dbContext.Conceptos on comDet.IDConcepto equals con.IDConcepto
                                            join per in dbContext.Personas on con.IDPersona equals per.IDPersona
                                            join prov in dbContext.Provincias on per.IDProvincia equals prov.IDProvincia
                                            join ciud in dbContext.Ciudades on per.IDCiudad equals ciud.IDCiudad
                                            select new
                                            {
                                                per,
                                                Provincia = prov.Nombre,
                                                Ciudad = ciud.Nombre,
                                                con.IDConcepto
                                            }).ToList();

                     var datosComprobanteDeCompra = (from comDet in results
                                                    join com in dbContext.Compras on comDet.IDComprobante equals com.IdComprobante
                                                    select com).ToList();


                    dt = results.OrderBy(x => x.Comprobantes.FechaComprobante).ToList().Select(x => new
                    {
                        IdComprobante = x.IDComprobante,
                        IdUsuario = usu.IDUsuario,
                        RazonSocial = (x.Comprobantes.Personas.NombreFantansia == "" ? x.Comprobantes.Personas.RazonSocial.ToUpper() : x.Comprobantes.Personas.NombreFantansia.ToUpper()),
                        TipoDoc = x.Comprobantes.TipoDocumento,
                        Documento = x.Comprobantes.NroDocumento,
                        IdCliente = dbContext.Comprobantes.Where(w => w.IDComprobante == x.IDComprobante).Select(s => s.IDPersona).FirstOrDefault(),
                        Fecha = x.Comprobantes.FechaComprobante.ToString("dd/MM/yyyy"),
                        Tipo = x.Comprobantes.Tipo,
                        Modo = x.Comprobantes.Modo == "E" ? "Electrónica" : (x.Comprobantes.Modo == "T" ? "Talonario" : "Otro"),
                        Numero = x.Comprobantes.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Comprobantes.Numero.ToString("#00000000"),
                        CDCFecha = datosComprobanteDeCompra.Where(p => p.IdComprobante == x.Comprobantes.IDComprobante).Select(s => s.Fecha).FirstOrDefault(),
                        CDCTipo = datosComprobanteDeCompra.Where(p => p.IdComprobante == x.Comprobantes.IDComprobante).Select(s => s.Tipo).FirstOrDefault(),
                        CDCNroFactura = datosComprobanteDeCompra.Where(p => p.IdComprobante == x.Comprobantes.IDComprobante).Select(s => s.NroFactura).FirstOrDefault(),
                        PendienteDePago = datosComprobanteDeCompra.Where(p => p.IdComprobante == x.Comprobantes.IDComprobante).Select(s => s.Saldo).FirstOrDefault(),
                        ImporteNetoGrav = (x.Comprobantes.Tipo == "NCA" || x.Comprobantes.Tipo == "NCB" || x.Comprobantes.Tipo == "NCC") ? -x.Comprobantes.ImporteTotalBruto : x.Comprobantes.ImporteTotalBruto,
                        TotalFact = (x.Comprobantes.Tipo == "NCA" || x.Comprobantes.Tipo == "NCB" || x.Comprobantes.Tipo == "NCC") ? -x.Comprobantes.ImporteTotalNeto : x.Comprobantes.ImporteTotalNeto,
                        Saldo = (x.Comprobantes.Tipo == "NCA" || x.Comprobantes.Tipo == "NCB" || x.Comprobantes.Tipo == "NCC") ? -x.Comprobantes.Saldo : x.Comprobantes.Saldo,
                        CondicionVenta = x.Comprobantes.CondicionVenta,
                        Observaciones = x.Comprobantes.Observaciones,
                        FechaVencimiento = x.Comprobantes.FechaComprobante.ToString("dd/MM/yyyy"),
                        Nombre = x.Comprobantes.Nombre,
                        Envio = x.Comprobantes.Envio,
                        Vendedor = x.Comprobantes.Vendedor,
                        FechaDeEntrega = x.Comprobantes.FechaEntrega != null ? ((DateTime)x.Comprobantes.FechaEntrega).ToString("dd/MM/yyyy") : "",
                        DetalleIdCodigoConcepto = x.IDConcepto != null ? (int)x.IDConcepto : 0,
                        DetalleConcepto = x.Concepto,
                        DetalleCantidad = x.Cantidad,
                        DetalleCodigo = dbContext.Conceptos.Where(w => w.IDConcepto == x.IDConcepto).Select(s => s.Codigo).FirstOrDefault(),
                        DetallePrecioUnitario = usu.CUIT.Equals("30716909839") ? x.PrecioUnitario / 1000 : x.PrecioUnitario,
                        DetalleIVA = x.Iva,
                        DetalleSubtotalSinIVA = usu.CUIT.Equals("30716909839") ? (x.PrecioUnitario * x.Cantidad) / 1000 : (x.PrecioUnitario * x.Cantidad),
                        DetalleSubtotalConIVA = usu.CUIT.Equals("30716909839") ? ((x.PrecioUnitario * x.Cantidad) + (((x.PrecioUnitario * x.Cantidad) * x.Iva) / 100)) / 1000 : ((x.PrecioUnitario * x.Cantidad) + (((x.PrecioUnitario * x.Cantidad) * x.Iva) / 100)),
                        DetalleBonificacion = x.Bonificacion,
                        DetalleConceptoProveedorRazonSocial = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.RazonSocial).FirstOrDefault(),
                        DetalleConceptoProveedorTipoDocumento = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.TipoDocumento).FirstOrDefault(),
                        DetalleConceptoProveedorNroDocumento = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.NroDocumento).FirstOrDefault(),
                        DetalleConceptoProveedorCondicionIva = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.CondicionIva).FirstOrDefault(),
                        DetalleConceptoProveedorTelefono = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Telefono).FirstOrDefault(),
                        DetalleConceptoProveedorCelular = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Celular).FirstOrDefault(),
                        DetalleConceptoProveedorWeb = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Web).FirstOrDefault(),
                        DetalleConceptoProveedorEmail = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Email).FirstOrDefault(),
                        DetalleConceptoProveedorObservaciones = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Observaciones).FirstOrDefault(),
                        DetalleConceptoProveedorProvincia = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.Provincia).FirstOrDefault(),
                        DetalleConceptoProveedorCiudad = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.Ciudad).FirstOrDefault(),
                        DetalleConceptoProveedorDomicilio = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Domicilio).FirstOrDefault(),
                        DetalleConceptoProveedorPisoDepto = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.PisoDepto).FirstOrDefault(),
                        DetalleConceptoProveedorCodigoPostal = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.CodigoPostal).FirstOrDefault(),
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
    public static string exportDocumentosRaiz(string tipo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Comprobantes";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.ComprobantesDetalle.Include("Comprobantes").Where(x => x.Comprobantes.IDUsuario == usu.IDUsuario && x.Comprobantes.Tipo == tipo && x.Comprobantes.IdComprobanteVinculado == null).AsQueryable();

                    var cd = dbContext.ComprobantesDetalle.Where(x => x.Comprobantes.IDUsuario == usu.IDUsuario && x.Comprobantes.Tipo == tipo && x.Comprobantes.IdComprobanteVinculado == null).ToList();

                    var listaProveedores = (from comDet in cd
                                           join con in dbContext.Conceptos on comDet.IDConcepto equals con.IDConcepto
                                           join per in dbContext.Personas on con.IDPersona equals per.IDPersona
                                           join prov in dbContext.Provincias on per.IDProvincia equals prov.IDProvincia
                                           join ciud in dbContext.Ciudades on per.IDCiudad equals ciud.IDCiudad
                                            select new
                                            {
                                                per,
                                                Provincia = prov.Nombre,
                                                Ciudad = ciud.Nombre,
                                                con.IDConcepto
                                            }).ToList();
                    

                    dt = results.OrderBy(x => x.Comprobantes.FechaComprobante).ToList().Select(x => new
                    {
                        RazonSocial = (x.Comprobantes.Personas.NombreFantansia == "" ? x.Comprobantes.Personas.RazonSocial.ToUpper() : x.Comprobantes.Personas.NombreFantansia.ToUpper()),
                        TipoDoc = x.Comprobantes.TipoDocumento,
                        Documento = x.Comprobantes.NroDocumento,
                        Fecha = x.Comprobantes.FechaComprobante.ToString("dd/MM/yyyy"),
                        Tipo = x.Comprobantes.Tipo,
                        Modo = x.Comprobantes.Modo == "E" ? "Electrónica" : (x.Comprobantes.Modo == "T" ? "Talonario" : "Otro"),
                        Numero = x.Comprobantes.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Comprobantes.Numero.ToString("#00000000"),
                        ImporteNetoGrav = (x.Comprobantes.Tipo == "NCA" || x.Comprobantes.Tipo == "NCB" || x.Comprobantes.Tipo == "NCC") ? -x.Comprobantes.ImporteTotalBruto : x.Comprobantes.ImporteTotalBruto,
                        TotalFact = (x.Comprobantes.Tipo == "NCA" || x.Comprobantes.Tipo == "NCB" || x.Comprobantes.Tipo == "NCC") ? -x.Comprobantes.ImporteTotalNeto : x.Comprobantes.ImporteTotalNeto,
                        Saldo = (x.Comprobantes.Tipo == "NCA" || x.Comprobantes.Tipo == "NCB" || x.Comprobantes.Tipo == "NCC") ? -x.Comprobantes.Saldo : x.Comprobantes.Saldo,
                        CondicionVenta = x.Comprobantes.CondicionVenta,
                        Observaciones = x.Comprobantes.Observaciones,
                        FechaVencimiento = x.Comprobantes.FechaComprobante.ToString("dd/MM/yyyy"),
                        Nombre = x.Comprobantes.Nombre,
                        Envio = x.Comprobantes.Envio,
                        Vendedor = x.Comprobantes.Vendedor,
                        FechaDeEntrega = x.Comprobantes.FechaEntrega != null ? ((DateTime)x.Comprobantes.FechaEntrega).ToString("dd/MM/yyyy") : "",
                        DetalleConcepto = x.Concepto,
                        DetalleCantidad = x.Cantidad,
                        DetallePrecioUnitario = x.PrecioUnitario,
                        DetalleIva = x.Iva,
                        DetalleBonificacion = x.Bonificacion,
                        DetalleConceptoProveedorRazonSocial = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.RazonSocial).FirstOrDefault(),
                        DetalleConceptoProveedorTipoDocumento = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.TipoDocumento).FirstOrDefault(),
                        DetalleConceptoProveedorNroDocumento = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.NroDocumento).FirstOrDefault(),
                        DetalleConceptoProveedorCondicionIva = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.CondicionIva).FirstOrDefault(),
                        DetalleConceptoProveedorTelefono = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Telefono).FirstOrDefault(),
                        DetalleConceptoProveedorCelular = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Celular).FirstOrDefault(),
                        DetalleConceptoProveedorWeb = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Web).FirstOrDefault(),
                        DetalleConceptoProveedorEmail = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Email).FirstOrDefault(),
                        DetalleConceptoProveedorObservaciones = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Observaciones).FirstOrDefault(),
                        DetalleConceptoProveedorProvincia = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.Provincia).FirstOrDefault(),
                        DetalleConceptoProveedorCiudad = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.Ciudad).FirstOrDefault(),
                        DetalleConceptoProveedorDomicilio = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.Domicilio).FirstOrDefault(),
                        DetalleConceptoProveedorPisoDepto = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.PisoDepto).FirstOrDefault(),
                        DetalleConceptoProveedorCodigoPostal = listaProveedores.Where(p => p.IDConcepto == x.IDConcepto).Select(s => s.per.CodigoPostal).FirstOrDefault(),
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

    private static int obtenerAnteultimoJulioDesdeUnPeriodo(string periodo)
    {

        // Extraer los componentes de año y mes de la cadena YYMM.
        int year = int.Parse(periodo.Substring(0, 2)) + 2000;
        int month = int.Parse(periodo.Substring(2, 2));

        // Crear una fecha con el año y mes.
        DateTime inputDate = new DateTime(year, month, 1);

        // Encontrar el último julio antes de la fecha de entrada.
        DateTime lastJuly = inputDate.AddMonths(-month + 7);

        // Restar un año al último julio para obtener el antepenúltimo julio.
        DateTime antepenultimateJuly = lastJuly.AddYears(-1);

        return int.Parse(antepenultimateJuly.ToString("yyMM"));        
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string obtenerCuadroResumen(string periodo)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var html = string.Empty;

                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    if (periodo.Equals(""))
                        periodo = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("00");

                    int periodoSeleccionado = int.Parse(periodo);

                    int periodoAnteultimoJulio = obtenerAnteultimoJulioDesdeUnPeriodo(periodoSeleccionado.ToString());

                    var resultsComprobantes = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario && x.periodo == periodoSeleccionado).AsQueryable();

                    ResultadosCuadroResumenViewModel resultado = new ResultadosCuadroResumenViewModel();

                    var list = resultsComprobantes.OrderBy(x => x.RazonSocial).ToList();

                    html += "<tr>";
                    html += "<td hidden='hidden'></td>";
                    html += "<td hidden='hidden'></td>";
                    html += "<td>TOTAL</td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.Total)).ToString("N2") + "</td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.Ventas)).ToString("N2") + "</td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.ConEntrega)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.SinEntrega)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.Factura)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.ConCAE)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.SinCAE)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.IVA)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.Cobranza)).ToString("N2") + " </td>";
                    html += "<td>" + ((decimal)list.Sum(item => item.Saldo)).ToString("N2") + " </td>";
                    html += "<td>--</td>";
                    html += "</tr>";

                    foreach (var x in list)
                    {
                        html += "<tr>";
                        html += "<td hidden='hidden'>" + x.IDUsuario.ToString() + "</td>";
                        html += "<td hidden='hidden'>" + x.IDPersona.ToString() + "</td>";
                        html += "<td>" + x.RazonSocial.ToUpper() + "</td>";
                        html += "<td>" + ((decimal)x.Total).ToString("N2") + "</td>";
                        html += "<td>" + ((decimal)x.Ventas).ToString("N2") + "</td>";
                        html += "<td>" + ((decimal)x.ConEntrega).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.SinEntrega).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Factura).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.ConCAE).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.SinCAE).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.IVA).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Cobranza).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Saldo).ToString("N2") + " </td>";
                        html += "<td>--</td>";
                        html += "</tr>";
                    }

                    var resultsComprobantesConSaldo = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario
                                                                            && x.periodo < periodoSeleccionado && x.Saldo > 0).AsQueryable();

                    //ResultadosCuadroResumenViewModel resultado = new ResultadosCuadroResumenViewModel();

                    var comprobantesVinculadosConSaldo = resultsComprobantesConSaldo
                                .GroupBy(a => a.RazonSocial)
                                .Select(a => new {
                                    a.OrderBy(x => x.IDUsuario).FirstOrDefault().IDUsuario,
                                    a.OrderBy(x => x.IDPersona).FirstOrDefault().IDPersona,
                                    a.OrderBy(x => x.RazonSocial).FirstOrDefault().RazonSocial,
                                    Total = a.Sum(b => b.Total),
                                    Ventas = a.Sum(b => b.Ventas),
                                    ConEntrega = a.Sum(b => b.ConEntrega),
                                    SinEntrega = a.Sum(b => b.SinEntrega),
                                    Factura = a.Sum(b => b.Factura),
                                    ConCAE = a.Sum(b => b.ConCAE),
                                    SinCAE = a.Sum(b => b.SinCAE),
                                    IVA = a.Sum(b => b.IVA),
                                    Cobranza = a.Sum(b => b.Cobranza),
                                    Saldo = a.Sum(b => b.Saldo)
                                })
                                .ToList();

                    var listConSaldo = comprobantesVinculadosConSaldo.OrderBy(x => x.RazonSocial).ToList();

                    if(listConSaldo.Count > 0)
                    {
                        html += "<tr>";
                        html += "<td hidden='hidden'>--</td>";
                        html += "<td hidden='hidden'>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "</tr>";
                    }

                    foreach (var x in listConSaldo)
                    {
                        html += "<tr>";
                        html += "<td hidden='hidden'>" + x.IDUsuario.ToString() + "</td>";
                        html += "<td hidden='hidden'>" + x.IDPersona.ToString() + "</td>";
                        html += "<td>" + x.RazonSocial.ToUpper() + "</td>";
                        html += "<td>" + ((decimal)x.Total).ToString("N2") + "</td>";
                        html += "<td>" + ((decimal)x.Ventas).ToString("N2") + "</td>";
                        html += "<td>" + ((decimal)x.ConEntrega).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.SinEntrega).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Factura).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.ConCAE).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.SinCAE).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.IVA).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Cobranza).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Saldo).ToString("N2") + " </td>";
                        html += "<td>--</td>";
                        html += "</tr>";
                    }

                    var resultsComprobantesSinCC = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario
                                                                            && x.periodo < periodoSeleccionado && x.periodo >= periodoAnteultimoJulio
                                                                            && x.Saldo == 0).AsQueryable();

                    //ResultadosCuadroResumenViewModel resultado = new ResultadosCuadroResumenViewModel();

                    var comprobantesVinculadosSinCC = resultsComprobantesSinCC
                                .GroupBy(a => a.RazonSocial)
                                .Select(a => new {
                                    a.OrderBy(x => x.IDUsuario).FirstOrDefault().IDUsuario,
                                    a.OrderBy(x => x.IDPersona).FirstOrDefault().IDPersona,
                                    a.OrderBy(x => x.RazonSocial).FirstOrDefault().RazonSocial,
                                    Total = a.Sum(b => b.Total),
                                    Ventas = a.Sum(b => b.Ventas),
                                    ConEntrega = a.Sum(b => b.ConEntrega),
                                    SinEntrega = a.Sum(b => b.SinEntrega),
                                    Factura = a.Sum(b => b.Factura),
                                    ConCAE = a.Sum(b => b.ConCAE),
                                    SinCAE = a.Sum(b => b.SinCAE),
                                    IVA = a.Sum(b => b.IVA),
                                    Cobranza = a.Sum(b => b.Cobranza),
                                    Saldo = a.Sum(b => b.Saldo)
                                })
                                .ToList();

                    var listSinCC = comprobantesVinculadosSinCC.OrderBy(x => x.RazonSocial).ToList();

                    if (listSinCC.Count > 0)
                    {
                        html += "<tr>";
                        html += "<td hidden='hidden'>--</td>";
                        html += "<td hidden='hidden'>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "<td>--</td>";
                        html += "</tr>";
                    }

                    foreach (var x in listSinCC)
                    {
                        html += "<tr>";
                        html += "<td hidden='hidden'>" + x.IDUsuario.ToString() + "</td>";
                        html += "<td hidden='hidden'>" + x.IDPersona.ToString() + "</td>";
                        html += "<td>" + x.RazonSocial.ToUpper() + "</td>";
                        html += "<td>" + ((decimal)x.Total).ToString("N2") + "</td>";
                        html += "<td>" + ((decimal)x.Ventas).ToString("N2") + "</td>";
                        html += "<td>" + ((decimal)x.ConEntrega).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.SinEntrega).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Factura).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.ConCAE).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.SinCAE).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.IVA).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Cobranza).ToString("N2") + " </td>";
                        html += "<td>" + ((decimal)x.Saldo).ToString("N2") + " </td>";
                        html += "<td>--</td>";
                        html += "</tr>";
                    }

                }
                if (html == "")
                    html = "<tr><td colspan='4' style='text-align:center'>Sin Datos.</td></tr>";


                return html;

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
    public static string exportarCuadroResumen(string periodo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "CuadroResumen";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    if (periodo.Equals(""))
                        periodo = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("00");

                    int periodoSeleccionado = int.Parse(periodo);

                    int periodoAnteultimoJulio = obtenerAnteultimoJulioDesdeUnPeriodo(periodoSeleccionado.ToString());

                    var resultsComprobantes = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario 
                                                && x.periodo == periodoSeleccionado).AsQueryable();

                    CuadroResumenViewModel totales = new CuadroResumenViewModel
                    {
                        IDUsuario = "",
                        IDPersona = "",
                        RazonSocial = "TOTAL",
                        Periodo = "",
                        Total = ((decimal)resultsComprobantes.Sum(item => item.Total)).ToString("N2"),
                        Ventas = ((decimal)resultsComprobantes.Sum(item => item.Ventas)).ToString("N2"),
                        ConEntrega = ((decimal)resultsComprobantes.Sum(item => item.ConEntrega)).ToString("N2"),
                        SinEntrega = ((decimal)resultsComprobantes.Sum(item => item.SinEntrega)).ToString("N2"),
                        Factura = ((decimal)resultsComprobantes.Sum(item => item.Factura)).ToString("N2"),
                        ConCAE = ((decimal)resultsComprobantes.Sum(item => item.ConCAE)).ToString("N2"),
                        SinCAE = ((decimal)resultsComprobantes.Sum(item => item.SinCAE)).ToString("N2"),
                        IVA = ((decimal)resultsComprobantes.Sum(item => item.IVA)).ToString("N2"),
                        Cobros = ((decimal)resultsComprobantes.Sum(item => item.Cobranza)).ToString("N2"),
                        Saldo = ((decimal)resultsComprobantes.Sum(item => item.Saldo)).ToString("N2"),
                    };

                    List<CuadroResumenViewModel> resultado = new List<CuadroResumenViewModel>();

                    resultado = resultsComprobantes.OrderBy(x => x.RazonSocial).ToList().Select(x => new CuadroResumenViewModel
                    {
                        IDUsuario = x.IDUsuario.ToString(),
                        IDPersona = x.IDPersona.ToString(),
                        RazonSocial = x.RazonSocial.ToUpper(),
                        Periodo = periodo,
                        Total = ((decimal)x.Total).ToString("N2"),
                        Ventas = ((decimal)x.Ventas).ToString("N2"),
                        ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                        SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                        Factura = ((decimal)x.Factura).ToString("N2"),
                        ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                        SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                        IVA = ((decimal)x.IVA).ToString("N2"),
                        Cobros = ((decimal)x.Cobranza).ToString("N2"),
                        Saldo = ((decimal)x.Saldo).ToString("N2"),
                    }).ToList();

                    var resultsComprobantesConSaldo = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario
                                                                            && x.periodo < periodoSeleccionado && x.Saldo > 0).AsQueryable();

                    List<CuadroResumenViewModel> resultadoSaldo = new List<CuadroResumenViewModel>();

                    var comprobantesVinculadosConSaldo = resultsComprobantesConSaldo
                                .GroupBy(a => a.RazonSocial)
                                .Select(a => new {
                                    a.OrderBy(x => x.IDUsuario).FirstOrDefault().IDUsuario,
                                    a.OrderBy(x => x.IDPersona).FirstOrDefault().IDPersona,
                                    a.OrderBy(x => x.RazonSocial).FirstOrDefault().RazonSocial,
                                    Total = a.Sum(b => b.Total),
                                    Ventas = a.Sum(b => b.Ventas),
                                    ConEntrega = a.Sum(b => b.ConEntrega),
                                    SinEntrega = a.Sum(b => b.SinEntrega),
                                    Factura = a.Sum(b => b.Factura),
                                    ConCAE = a.Sum(b => b.ConCAE),
                                    SinCAE = a.Sum(b => b.SinCAE),
                                    IVA = a.Sum(b => b.IVA),
                                    Cobranza = a.Sum(b => b.Cobranza),
                                    Saldo = a.Sum(b => b.Saldo)
                                })
                                .ToList();

                    var listConSaldo = comprobantesVinculadosConSaldo.OrderBy(x => x.RazonSocial).ToList();

                    foreach (var x in listConSaldo)
                    {
                        resultadoSaldo.Add(new CuadroResumenViewModel
                        {
                            IDUsuario = x.IDUsuario.ToString(),
                            IDPersona = x.IDPersona.ToString(),
                            Periodo = "",
                            RazonSocial = x.RazonSocial.ToUpper(),
                            Total = ((decimal)x.Total).ToString("N2"),
                            Ventas = ((decimal)x.Ventas).ToString("N2"),
                            ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                            SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                            Factura = ((decimal)x.Factura).ToString("N2"),
                            ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                            SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                            IVA = ((decimal)x.IVA).ToString("N2"),
                            Cobros = ((decimal)x.Cobranza).ToString("N2"),
                            Saldo = ((decimal)x.Saldo).ToString("N2"),
                        });
                    }


                    var resultsComprobantesSinCC = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario
                                                                            && x.periodo < periodoSeleccionado && x.periodo >= periodoAnteultimoJulio
                                                                            && x.Saldo == 0).AsQueryable();

                    List<CuadroResumenViewModel> resultadoComprobantesSinCC = new List<CuadroResumenViewModel>();

                    var comprobantesSinCC = resultsComprobantesSinCC
                                .GroupBy(a => a.RazonSocial)
                                .Select(a => new {
                                    a.OrderBy(x => x.IDUsuario).FirstOrDefault().IDUsuario,
                                    a.OrderBy(x => x.IDPersona).FirstOrDefault().IDPersona,
                                    a.OrderBy(x => x.RazonSocial).FirstOrDefault().RazonSocial,
                                    Total = a.Sum(b => b.Total),
                                    Ventas = a.Sum(b => b.Ventas),
                                    ConEntrega = a.Sum(b => b.ConEntrega),
                                    SinEntrega = a.Sum(b => b.SinEntrega),
                                    Factura = a.Sum(b => b.Factura),
                                    ConCAE = a.Sum(b => b.ConCAE),
                                    SinCAE = a.Sum(b => b.SinCAE),
                                    IVA = a.Sum(b => b.IVA),
                                    Cobranza = a.Sum(b => b.Cobranza),
                                    Saldo = a.Sum(b => b.Saldo)
                                })
                                .ToList();

                    var listSinCC = comprobantesSinCC.OrderBy(x => x.RazonSocial).ToList();

                    foreach (var x in listSinCC)
                    {
                        resultadoComprobantesSinCC.Add(new CuadroResumenViewModel
                        {
                            IDUsuario = x.IDUsuario.ToString(),
                            IDPersona = x.IDPersona.ToString(),
                            Periodo = "",
                            RazonSocial = x.RazonSocial.ToUpper(),
                            Total = ((decimal)x.Total).ToString("N2"),
                            Ventas = ((decimal)x.Ventas).ToString("N2"),
                            ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                            SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                            Factura = ((decimal)x.Factura).ToString("N2"),
                            ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                            SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                            IVA = ((decimal)x.IVA).ToString("N2"),
                            Cobros = ((decimal)x.Cobranza).ToString("N2"),
                            Saldo = ((decimal)x.Saldo).ToString("N2"),
                        });
                    }


                    resultadoSaldo.AddRange(resultadoComprobantesSinCC);
                    resultadoSaldo.AddRange(resultado);
                    resultadoSaldo.Add(totales);

                    dt = resultadoSaldo.ToList().ToDataTable();

                    //dt = resultsComprobantes.OrderBy(x => x.razonSocial).ToList().Select(x => new CuadroResumenViewModel
                    //{
                    //    IDUsuario = x.IDUsuario.ToString(),
                    //    IDPersona = x.IDPersona.ToString(),
                    //    RazonSocial = x.razonSocial.ToUpper(),
                    //    Total = ((decimal)x.Total).ToString("N2"),
                    //    Ventas = ((decimal)x.Ventas).ToString("N2"),
                    //    ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                    //    SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                    //    Factura = ((decimal)x.Factura).ToString("N2"),
                    //    ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                    //    SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                    //    IVA = ((decimal)x.IVA).ToString("N2"),
                    //    Cobros = ((decimal)x.Cobranza).ToString("N2"),
                    //    Saldo = ((decimal)x.Saldo).ToString("N2"),
                    //}).ToList().ToDataTable();
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
    public static string exportarCuadroResumenOriginal(string periodo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "CuadroResumen";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    if (periodo.Equals(""))
                        periodo = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("00");

                    int periodoSeleccionado = int.Parse(periodo);

                    int periodoAnteultimoJulio = obtenerAnteultimoJulioDesdeUnPeriodo(periodoSeleccionado.ToString());

                    var resultsComprobantes = dbContext.vCuadroResumenOriginal.Where(x => x.IDUsuario == usu.IDUsuario
                                                && x.periodo == periodo).AsQueryable();

                    CuadroResumenViewModel totales = new CuadroResumenViewModel
                    {
                        IDUsuario = "",
                        IDPersona = "",
                        RazonSocial = "TOTAL",
                        Periodo = "",
                        Total = ((decimal)resultsComprobantes.Sum(item => item.Total)).ToString("N2"),
                        Ventas = ((decimal)resultsComprobantes.Sum(item => item.Ventas)).ToString("N2"),
                        ConEntrega = ((decimal)resultsComprobantes.Sum(item => item.ConEntrega)).ToString("N2"),
                        SinEntrega = ((decimal)resultsComprobantes.Sum(item => item.SinEntrega)).ToString("N2"),
                        Factura = ((decimal)resultsComprobantes.Sum(item => item.Factura)).ToString("N2"),
                        ConCAE = ((decimal)resultsComprobantes.Sum(item => item.ConCAE)).ToString("N2"),
                        SinCAE = ((decimal)resultsComprobantes.Sum(item => item.SinCAE)).ToString("N2"),
                        IVA = ((decimal)resultsComprobantes.Sum(item => item.IVA)).ToString("N2"),
                        Cobros = ((decimal)resultsComprobantes.Sum(item => item.Cobranza)).ToString("N2"),
                        Saldo = ((decimal)resultsComprobantes.Sum(item => item.Saldo)).ToString("N2"),
                    };

                    List<CuadroResumenViewModel> resultado = new List<CuadroResumenViewModel>();

                    resultado = resultsComprobantes.OrderBy(x => x.RazonSocial).ToList().Select(x => new CuadroResumenViewModel
                    {
                        IDUsuario = x.IDUsuario.ToString(),
                        IDPersona = x.IDPersona.ToString(),
                        RazonSocial = x.RazonSocial.ToUpper(),
                        Periodo = periodo,
                        Total = ((decimal)x.Total).ToString("N2"),
                        Ventas = ((decimal)x.Ventas).ToString("N2"),
                        ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                        SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                        Factura = ((decimal)x.Factura).ToString("N2"),
                        ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                        SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                        IVA = ((decimal)x.IVA).ToString("N2"),
                        Cobros = ((decimal)x.Cobranza).ToString("N2"),
                        Saldo = ((decimal)x.Saldo).ToString("N2"),
                    }).ToList();

                    //var resultsComprobantesConSaldo = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario
                    //                                                        && x.periodo < periodoSeleccionado && x.Saldo > 0).AsQueryable();

                    //List<CuadroResumenViewModel> resultadoSaldo = new List<CuadroResumenViewModel>();

                    //var comprobantesVinculadosConSaldo = resultsComprobantesConSaldo
                    //            .GroupBy(a => a.RazonSocial)
                    //            .Select(a => new {
                    //                a.OrderBy(x => x.IDUsuario).FirstOrDefault().IDUsuario,
                    //                a.OrderBy(x => x.IDPersona).FirstOrDefault().IDPersona,
                    //                a.OrderBy(x => x.RazonSocial).FirstOrDefault().RazonSocial,
                    //                Total = a.Sum(b => b.Total),
                    //                Ventas = a.Sum(b => b.Ventas),
                    //                ConEntrega = a.Sum(b => b.ConEntrega),
                    //                SinEntrega = a.Sum(b => b.SinEntrega),
                    //                Factura = a.Sum(b => b.Factura),
                    //                ConCAE = a.Sum(b => b.ConCAE),
                    //                SinCAE = a.Sum(b => b.SinCAE),
                    //                IVA = a.Sum(b => b.IVA),
                    //                Cobranza = a.Sum(b => b.Cobranza),
                    //                Saldo = a.Sum(b => b.Saldo)
                    //            })
                    //            .ToList();

                    //var listConSaldo = comprobantesVinculadosConSaldo.OrderBy(x => x.RazonSocial).ToList();

                    //foreach (var x in listConSaldo)
                    //{
                    //    resultadoSaldo.Add(new CuadroResumenViewModel
                    //    {
                    //        IDUsuario = x.IDUsuario.ToString(),
                    //        IDPersona = x.IDPersona.ToString(),
                    //        Periodo = "",
                    //        RazonSocial = x.RazonSocial.ToUpper(),
                    //        Total = ((decimal)x.Total).ToString("N2"),
                    //        Ventas = ((decimal)x.Ventas).ToString("N2"),
                    //        ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                    //        SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                    //        Factura = ((decimal)x.Factura).ToString("N2"),
                    //        ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                    //        SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                    //        IVA = ((decimal)x.IVA).ToString("N2"),
                    //        Cobros = ((decimal)x.Cobranza).ToString("N2"),
                    //        Saldo = ((decimal)x.Saldo).ToString("N2"),
                    //    });
                    //}


                    //var resultsComprobantesSinCC = dbContext.vCuadroResumen.Where(x => x.IDUsuario == usu.IDUsuario
                    //                                                        && x.periodo < periodoSeleccionado && x.periodo >= periodoAnteultimoJulio
                    //                                                        && x.Saldo == 0).AsQueryable();

                    //List<CuadroResumenViewModel> resultadoComprobantesSinCC = new List<CuadroResumenViewModel>();

                    //var comprobantesSinCC = resultsComprobantesSinCC
                    //            .GroupBy(a => a.RazonSocial)
                    //            .Select(a => new {
                    //                a.OrderBy(x => x.IDUsuario).FirstOrDefault().IDUsuario,
                    //                a.OrderBy(x => x.IDPersona).FirstOrDefault().IDPersona,
                    //                a.OrderBy(x => x.RazonSocial).FirstOrDefault().RazonSocial,
                    //                Total = a.Sum(b => b.Total),
                    //                Ventas = a.Sum(b => b.Ventas),
                    //                ConEntrega = a.Sum(b => b.ConEntrega),
                    //                SinEntrega = a.Sum(b => b.SinEntrega),
                    //                Factura = a.Sum(b => b.Factura),
                    //                ConCAE = a.Sum(b => b.ConCAE),
                    //                SinCAE = a.Sum(b => b.SinCAE),
                    //                IVA = a.Sum(b => b.IVA),
                    //                Cobranza = a.Sum(b => b.Cobranza),
                    //                Saldo = a.Sum(b => b.Saldo)
                    //            })
                    //            .ToList();

                    //var listSinCC = comprobantesSinCC.OrderBy(x => x.RazonSocial).ToList();

                    //foreach (var x in listSinCC)
                    //{
                    //    resultadoComprobantesSinCC.Add(new CuadroResumenViewModel
                    //    {
                    //        IDUsuario = x.IDUsuario.ToString(),
                    //        IDPersona = x.IDPersona.ToString(),
                    //        Periodo = "",
                    //        RazonSocial = x.RazonSocial.ToUpper(),
                    //        Total = ((decimal)x.Total).ToString("N2"),
                    //        Ventas = ((decimal)x.Ventas).ToString("N2"),
                    //        ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                    //        SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                    //        Factura = ((decimal)x.Factura).ToString("N2"),
                    //        ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                    //        SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                    //        IVA = ((decimal)x.IVA).ToString("N2"),
                    //        Cobros = ((decimal)x.Cobranza).ToString("N2"),
                    //        Saldo = ((decimal)x.Saldo).ToString("N2"),
                    //    });
                    //}

                    //resultadoSaldo.AddRange(resultadoComprobantesSinCC);
                    //
                    resultado.Add(totales);

                    dt = resultado.ToList().ToDataTable();

                    //dt = resultsComprobantes.OrderBy(x => x.razonSocial).ToList().Select(x => new CuadroResumenViewModel
                    //{
                    //    IDUsuario = x.IDUsuario.ToString(),
                    //    IDPersona = x.IDPersona.ToString(),
                    //    RazonSocial = x.razonSocial.ToUpper(),
                    //    Total = ((decimal)x.Total).ToString("N2"),
                    //    Ventas = ((decimal)x.Ventas).ToString("N2"),
                    //    ConEntrega = ((decimal)x.ConEntrega).ToString("N2"),
                    //    SinEntrega = ((decimal)x.SinEntrega).ToString("N2"),
                    //    Factura = ((decimal)x.Factura).ToString("N2"),
                    //    ConCAE = ((decimal)x.ConCAE).ToString("N2"),
                    //    SinCAE = ((decimal)x.SinCAE).ToString("N2"),
                    //    IVA = ((decimal)x.IVA).ToString("N2"),
                    //    Cobros = ((decimal)x.Cobranza).ToString("N2"),
                    //    Saldo = ((decimal)x.Saldo).ToString("N2"),
                    //}).ToList().ToDataTable();
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