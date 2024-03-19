using ACHE.Model;
using ACHE.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Common;
using ACHE.Model.Negocio;
using ACHE.FacturaElectronica.VEConsumerService;
using System.Data.Entity;

public partial class generarAbonos : BasePage
{
    private static ResultadosAbonosAGenerarViewModel resultadosAbonos;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                ddlPuntoVenta.DataSource = UsuarioCommon.ObtenerPuntosDeVenta(usu.IDUsuario);
                ddlPuntoVenta.DataTextField = "Nombre";
                ddlPuntoVenta.DataValueField = "ID";
                ddlPuntoVenta.DataBind();
            }

            CondicionIva.Value = usu.CondicionIVA;

            hdnEnvioFE.Value = CurrentUser.EnvioAutomaticoComprobante ? "1" : "0";

            ddlModo.Items.Add(new ListItem("Talonario preimpreso", "T"));
            ddlModo.Items.Add(new ListItem("Cotización", "COT"));
            if (usu.TieneFE)
                ddlModo.Items.Add(new ListItem("Facturación electrónica", "E"));
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosAbonosAGenerarViewModel getResults(string fecha)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Abonos.Where(x => x.IDUsuario == usu.IDUsuario && x.Estado == "A").OrderBy(x => x.Nombre).Select(x => new AbonosAGenerarGrupoViewModel()
                    {
                        ID = x.IDAbono,
                        Nombre = x.Nombre,
                        tipoConcepto = x.Tipo,
                        Fecha = x.FechaInicio,
                        FechaFin = x.FechaFin,
                        IDPlanDeCuenta = x.IDPlanDeCuenta
                    }).ToList();

                    if (fecha != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fecha);
                        results = results.Where(x => x.Fecha <= dtDesde && (!x.FechaFin.HasValue || (x.FechaFin.HasValue && x.FechaFin >= dtDesde))).ToList();
                    }

                    foreach (var abono in results)
                    {
                        abono.Items = dbContext.AbonosPersona.Include("Personas").Where(x => x.IDAbono == abono.ID).ToList().Select(x => new AbonosAGenerarViewModel()
                        {
                            IDPersona = x.IDPersona,
                            RazonSocial = x.Personas.RazonSocial,
                            Cuit = x.Personas.NroDocumento,
                            CondicionIva = x.Personas.CondicionIva,
                            Importe = x.Abonos.PrecioUnitario,
                            Iva = x.Abonos.Iva,
                            nroRegistro = x.IDAbono.ToString() + x.IDPersona.ToString(),
                            FEGenerada = "Pendiente de generación.",
                            ClienteEmail = x.Personas.Email,
                            FrecuenciaAbono = x.Abonos.Frecuencia,
                            Cantidad = x.Cantidad
                        }).ToList();
                        foreach (var item in abono.Items)
                        {
                            var fechaFrecuenciaDesde = new DateTime();
                            var fechaFrecuenciaHasta = DateTime.Now.Date;
                            switch (item.FrecuenciaAbono)
                            {
                                case "S":
                                    fechaFrecuenciaDesde = DateTime.Now.AddMonths(-6).Date;
                                    break;
                                case "T":
                                    fechaFrecuenciaDesde = DateTime.Now.AddMonths(-3).Date;
                                    break;
                                case "M":
                                    fechaFrecuenciaDesde = DateTime.Now.AddMonths(-1).Date;
                                    break;
                                case "Q":
                                    fechaFrecuenciaDesde = DateTime.Now.AddDays(-15).Date;
                                    break;
                            }
                            var ComprobantePago = dbContext.ComprobantesDetalle.Include("Comprobantes").Any(x =>
                                                                                                            x.Comprobantes.IDPersona == item.IDPersona &&
                                                                                                            x.IDAbono == abono.ID &&
                                                                                                            x.Comprobantes.FechaComprobante >= fechaFrecuenciaDesde &&
                                                                                                            x.Comprobantes.FechaComprobante <= fechaFrecuenciaHasta);

                            if (ComprobantePago)
                                abono.Items.Where(x => x.IDPersona == item.IDPersona).FirstOrDefault().FEGenerada = "Comprobante ya emitido. ";
                        }
                    }

                    ResultadosAbonosAGenerarViewModel resultado = new ResultadosAbonosAGenerarViewModel();
                    resultado.Items = results.Where(x => x.Cantidad > 0).ToList();

                    if (usu.CondicionIVA == "RI")
                    {
                        var puedeDarFacturaA = results.Any(x => x.Items.Count(y => y.CondicionIva == "RI") > 0);
                    }

                    resultadosAbonos = new ResultadosAbonosAGenerarViewModel();
                    resultadosAbonos = resultado;
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

    private static string obtenerTipoDeFacturaAFacturar(string CondicionIva)
    {
        string tipoDeFactura = string.Empty;
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        if (usu.CondicionIVA == "MO")
        {
            tipoDeFactura = "FCC";
        }
        else
        {
            switch (CondicionIva)
            {
                case "RI":
                    tipoDeFactura = "FCA";
                    break;
                default:
                    tipoDeFactura = "FCB";
                    break;
            }
        }
        return tipoDeFactura;
    }

    [WebMethod(true)]
    public static ResultadosAbonosAGenerarViewModel generarComprobanteAbono(string idAbonos, string modo, int idPuntoVenta, string numeroFacturaA, string numeroFacturaB, string numeroFacturaC, string fechaComprobante)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            int nroFacturaA = (numeroFacturaA == "") ? 0 : Convert.ToInt32(numeroFacturaA);
            int nroFacturaB = (numeroFacturaB == "") ? 0 : Convert.ToInt32(numeroFacturaB);
            int nroFacturaC = (numeroFacturaC == "") ? 0 : Convert.ToInt32(numeroFacturaC);

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            var listaIdPersonas = Regex.Split(idAbonos, "chkAbono_").ToList();
            listaIdPersonas.RemoveAt(0);

            int idPersona;// id cliente Proveedor 
            string fecha = fechaComprobante;
            string condicionVenta = "Efectivo";
            string fechaVencimiento = DateTime.Now.AddDays(31).ToShortDateString();
            string nroComprobante = string.Empty;
            string obs = string.Empty;
            string tipodeFactura = string.Empty;
            int idComprobante = 0;
            int idActividad = 0;

            ResultadosAbonosAGenerarViewModel listaAbonosFacturados = new ResultadosAbonosAGenerarViewModel();

            foreach (var abonos in resultadosAbonos.Items)
            {
                foreach (var clienteAbono in abonos.Items)
                {
                    clienteAbono.Estado = "Comprobante no seleccionado.";
                    clienteAbono.FEGenerada = "";
                    clienteAbono.nroComprobante = "";

                    for (int i = 0; i < listaIdPersonas.Count; i++)
                    {
                        if (listaIdPersonas[i] == clienteAbono.nroRegistro)
                        {
                            listaIdPersonas.RemoveAt(0);
                            try
                            {
                                idPersona = clienteAbono.IDPersona;
                                tipodeFactura = obtenerDatosFactura(ref nroFacturaA, ref nroFacturaB, ref nroFacturaC, ref nroComprobante, clienteAbono.CondicionIva, modo, idPuntoVenta);
                                idComprobante = generarComprobante(0, idPersona, tipodeFactura, modo, fecha, condicionVenta, fechaVencimiento, idPuntoVenta, nroComprobante, obs, clienteAbono, abonos);
                                idActividad = UsuarioCommon.ObtenerActividades(usu.IDUsuario).Select(s => s.ID).FirstOrDefault();


                                var puntoDeVenta = ObtenerPuntodeVenta(idPuntoVenta);
                                clienteAbono.nroComprobante = tipodeFactura + "-" + puntoDeVenta + "-" + nroComprobante;
                                clienteAbono.Estado = "comprobante generado. ";

                                if (modo == "E")
                                {
                                    Common.CrearComprobante(usu, idComprobante, idPersona, tipodeFactura, 
                                        modo, fecha, condicionVenta, abonos.tipoConcepto, fechaVencimiento, idPuntoVenta, 
                                        ref nroComprobante, obs, 0,"","",false, idActividad, "", Common.ComprobanteModo.Generar);
                                    clienteAbono.nroComprobante = nroComprobante;

                                    var razonSocial = clienteAbono.RazonSocial.RemoverCaracteresParaPDF();
                                    clienteAbono.URL = usu.IDUsuario + "/" + razonSocial + "_" + tipodeFactura + "-" + nroComprobante + ".pdf";
                                    clienteAbono.FEGenerada = "Más opciones";

                                    clienteAbono.EnvioFE = EnviarComprobanteAutomaticamente(idComprobante);
                                }
                                if (tipodeFactura != "COT")
                                    GenerarAsientosContables(idComprobante);
                            }
                            catch (Exception ex)
                            {
                                clienteAbono.FEGenerada = "";
                                clienteAbono.Estado += "ERROR: " + ex.Message;
                            }
                        }
                    }
                }
            }

            return resultadosAbonos;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static string ObtenerPuntodeVenta(int idPunto)
    {
        using (var dbContext = new ACHEEntities())
        {
            var punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == idPunto).FirstOrDefault();
            if (punto != null)
                return punto.Punto.ToString("#0000");
            else
                return "0000";
        }
    }
    private static void GenerarAsientosContables(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                ContabilidadCommon.AgregarAsientoDeVentas(usu, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    private static int generarComprobante(int id, int idPersona, string tipodeFactura, string modo, string fecha, string condicionVenta, string fechaVencimiento, int idPuntoVenta, string nroComprobante, string obs, AbonosAGenerarViewModel clienteAbono, AbonosAGenerarGrupoViewModel abono)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            ComprobanteCart.Retrieve().Items.Clear();
            ComprobanteCart.Retrieve().Items.Add(new ComprobantesDetalleViewModel()
            {
                PrecioUnitario = clienteAbono.Importe,
                Iva = clienteAbono.Iva,
                Concepto = abono.Nombre,
                Cantidad = clienteAbono.Cantidad,
                Bonificacion = 0,
                IDAbonos = abono.ID,
                IDPlanDeCuenta = (abono.IDPlanDeCuenta == null) ? 0 : abono.IDPlanDeCuenta,
            });

            ComprobanteCartDto compCart = new ComprobanteCartDto();
            compCart.IDComprobante = id;
            compCart.IDPersona = idPersona;
            compCart.TipoComprobante = tipodeFactura;
            compCart.TipoConcepto = abono.tipoConcepto.ToString();
            compCart.IDUsuario = usu.IDUsuario;
            compCart.Modo = modo;
            compCart.FechaComprobante = Convert.ToDateTime(fecha);
            compCart.FechaVencimiento = Convert.ToDateTime(fechaVencimiento);
            compCart.IDPuntoVenta = idPuntoVenta;
            compCart.Numero = nroComprobante;
            compCart.Observaciones = obs;
            compCart.CondicionVenta = condicionVenta;
            //compCart.IDJuresdiccion = idJurisdiccion;

            compCart.Items = new List<ComprobantesDetalleViewModel>();
            compCart.ImporteNoGravado = ComprobanteCart.Retrieve().GetImporteNoGravado();
            compCart.ImporteExento = ComprobanteCart.Retrieve().GetImporteExento();
            compCart.PercepcionIVA = ComprobanteCart.Retrieve().PercepcionIVA;
            compCart.PercepcionIIBB = ComprobanteCart.Retrieve().PercepcionIIBB;

            compCart.Items = ComprobanteCart.Retrieve().Items;

            var entity = ComprobantesCommon.GuardarComprobante(compCart);
            return entity.IDComprobante;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string EnviarComprobanteAutomaticamente(int idComprobante)
    {
        var resultado = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            if (usu.EnvioAutomaticoComprobante == true)
            {
                using (var dbContext = new ACHEEntities())
                {
                    try
                    {
                        var plandeCuenta = PermisosModulosCommon.ObtenerPlanActual(dbContext, usu.IDUsuario);
                        if (plandeCuenta.IDPlan >= 3)//Plan Pyme
                        {
                            bool CompEnviado = dbContext.ComprobantesEnviados.Any(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == idComprobante && x.Resultado == true);
                            if (!CompEnviado)
                            {
                                var avisos = dbContext.AvisosVencimiento.Where(x => x.IDUsuario == usu.IDUsuario && x.TipoAlerta == "Envio FE").FirstOrDefault();
                                if (avisos != null)
                                {
                                    var c = dbContext.Comprobantes.Include("PuntosDeVenta").Where(x => x.IDComprobante == idComprobante && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                                    if (c.Personas.Email != null && c.Personas.Email != "")
                                    {
                                        var mensaje = avisos.Mensaje.ReplaceAll("\n", "<br/>");
                                        var file = c.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + c.Tipo + "-" + c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000") + ".pdf";
                                        Common.EnviarComprobantePorMail(c.Personas.RazonSocial, c.Personas.Email, avisos.Asunto, mensaje, file);
                                        ComprobantesCommon.GuardarComprobantesEnviados(dbContext, idComprobante, null, "Abono enviado correctamente", true, usu);
                                    }
                                    else
                                        return "El cliente no tiene configurado el correo electrónico";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ComprobantesCommon.GuardarComprobantesEnviados(dbContext, idComprobante, null, "Abonos: " + ex.Message, false, usu);
                        resultado = "No se pudo enviar automáticamente la factura electrónonica";
                    }
                }
            }
            return resultado;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static string obtenerDatosFactura(ref int numeroFacturaA, ref int numeroFacturaB, ref int numeroFacturaC, ref string nroComprobante, string CondicionIva, string modo, int idPuntoDeVenta)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        var tipodeFactura = string.Empty;
        var _nroComprobante = 0;
        if (modo == "COT")
        {
            tipodeFactura = "COT";
            nroComprobante = ComprobantesCommon.ObtenerProxNroComprobante(tipodeFactura, usu.IDUsuario, idPuntoDeVenta);
        }
        else
        {
            tipodeFactura = obtenerTipoDeFacturaAFacturar(CondicionIva);
            switch (tipodeFactura)
            {
                case "FCA":
                    _nroComprobante = numeroFacturaA;
                    numeroFacturaA++;
                    break;
                case "FCB":
                    _nroComprobante = numeroFacturaB;
                    numeroFacturaB++;
                    break;
                case "FCC":
                    _nroComprobante = numeroFacturaC;
                    numeroFacturaC++;
                    break;
            }
            nroComprobante = _nroComprobante.ToString("#00000000");
        }
        return tipodeFactura;
    }
}