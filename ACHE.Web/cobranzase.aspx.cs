using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.FacturaElectronica;
using System.Configuration;
using System.Collections.Specialized;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using ACHE.Negocio.Tesoreria;

public partial class cobranzase : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ".";
    public const string SeparadorDeDecimales = ",";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CobranzaCart.Retrieve().Items.Clear();
            CobranzaCart.Retrieve().FormasDePago.Clear();
            CobranzaCart.Retrieve().Retenciones.Clear();

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            hdnTieneFE.Value = "0";
            litPath.Text = "Alta";
            hdnEnvioCR.Value = (CurrentUser.EnvioAutomaticoRecibo == true) ? "1" : "0";

            hdnIDPersona.Value = Request.QueryString["IDPersona"];
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.ComercialCobranzas)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                if (!string.IsNullOrWhiteSpace(Request.QueryString["IDComprobante"]))
                {
                    var idComprobante = Convert.ToInt32(Request.QueryString["IDComprobante"]);
                    hdntieneComprobante.Value = idComprobante.ToString();
                    var comprobante = dbContext.Comprobantes.Where(x => x.IDComprobante == idComprobante).FirstOrDefault();
                    hdnIDPersona.Value = comprobante.IDPersona.ToString();
                }

                //valido si se puede modificar
                if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    hdnID.Value = Request.QueryString["ID"];
                    if (hdnID.Value != "0")
                    {
                        ddlPuntoVenta.Disabled = true;

                        int id = int.Parse(hdnID.Value);
                        lnkPrint2.Attributes.Add("onclick", "Common.imprimirArchivoDesdeIframe('');");

                        litPath.Text = "Edición";

                        bool CompEnviado = dbContext.ComprobantesEnviados.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDCobranza == id && x.Resultado == true);
                        if (!CompEnviado && CurrentUser.EnvioAutomaticoRecibo)
                            hdnEnvioCR.Value = "1";
                        else
                            hdnEnvioCR.Value = "0";

                        var cobranza = dbContext.Cobranzas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDCobranza == id).FirstOrDefault();
                        txtEnvioPara.Value = cobranza.Personas.Email.ToString();
                        hdnRazonSocial.Value = cobranza.Personas.RazonSocial;

                        var punto = cobranza.PuntosDeVenta;
                        ddlPuntoVenta.Items.Add(new ListItem(punto.Punto.ToString("#0000"), punto.IDPuntoVenta.ToString()));
                    }
                }
            }
        }
    }

    #region Items

    [WebMethod(true)]
    public static void agregarItem(int id, string idComprobante, string comprobante, string importe)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = CobranzaCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
                CobranzaCart.Retrieve().Items.Remove(aux);
            }

            var tra = new CobranzasDetalleViewModel();
            tra.ID = CobranzaCart.Retrieve().Items.Count() + 1;
            tra.Comprobante = comprobante;
            tra.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales));

            tra.IDComprobante = int.Parse(idComprobante);

            CobranzaCart.Retrieve().Items.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarItem(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = CobranzaCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
                CobranzaCart.Retrieve().Items.Remove(aux);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static TotalesViewModel obtenerTotales()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            TotalesViewModel totales = new TotalesViewModel();
            totales.Total = CobranzaCart.Retrieve().GetTotal().ToString("N2");

            return totales;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerItems()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = CobranzaCart.Retrieve().Items.OrderBy(x => x.Comprobante).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td style='text-align:left'>" + detalle.Comprobante + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Importe.ToString("N2") + " </td>";
                        //html += "<td style='text-align:right'>" + detalle.RetGanancias.ToString("N2") + "</td>";
                        //html += "<td style='text-align:right'>" + detalle.IIBB.ToString("N2") + "</td>";
                        //html += "<td style='text-align:right'>" + detalle.SUSS.ToString("N2") + "</td>";
                        //html += "<td style='text-align:right'>" + detalle.Otros.ToString("N2") + "</td>";
                        //html += "<td style='text-align:right'>" + detalle.Total.ToString("N2") + "</td>";

                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:eliminarItem(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarItem(" + detalle.ID + ", '" + detalle.IDComprobante + "' ,'";
                        html += detalle.Importe.ToString("").Replace(".", ",") + "','" + detalle.Comprobante + "')\""; //,'" + detalle.RetGanancias.ToString("").Replace(",", ".") + "','";
                        //html += detalle.IIBB.ToString("").Replace(",", ".") + "','" + detalle.SUSS.ToString("").Replace(",", ".") + "','" + detalle.Otros.ToString("").Replace(",", ".") + "')\"";
                        html += "><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    #endregion

    #region Formas de Pago

    [WebMethod(true)]
    public static void agregarForma(int id, string forma, string nroRef, string importe, string idcheque, string idBanco, string idNotaCredito)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            int codigoCheque = -1;

            if (id != 0)
            {
                var aux = CobranzaCart.Retrieve().FormasDePago.Where(x => x.ID == id).FirstOrDefault();
                CobranzaCart.Retrieve().FormasDePago.Remove(aux);
            }
            else
            {
                
                if (!string.IsNullOrWhiteSpace(idcheque))
                    codigoCheque = int.Parse(idcheque);

                if (CobranzaCart.Retrieve().FormasDePago.Where(w => w.IDCheque == codigoCheque).Any())
                {
                    throw new Exception("El cheque ya esta agregado a la lista");
                }
            }

            var tra = new CobranzasFormasDePagoViewModel();
            tra.ID = CobranzaCart.Retrieve().FormasDePago.Count() + 1;
            tra.FormaDePago = forma;
            tra.NroReferencia = nroRef;
            tra.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales));

            if (!string.IsNullOrWhiteSpace(idcheque))
                tra.IDCheque = int.Parse(idcheque);

            if (!string.IsNullOrWhiteSpace(idBanco))
                tra.IDBanco = int.Parse(idBanco);

            if (!string.IsNullOrWhiteSpace(idNotaCredito))
                tra.IDNotaCredito = int.Parse(idNotaCredito);

            CobranzaCart.Retrieve().FormasDePago.Add(tra);

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarForma(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = CobranzaCart.Retrieve().FormasDePago.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
                CobranzaCart.Retrieve().FormasDePago.Remove(aux);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerFormas()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = CobranzaCart.Retrieve().FormasDePago.OrderBy(x => x.FormaDePago).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td style='text-align:left'>" + detalle.FormaDePago + "</td>";
                        html += "<td style='text-align:left'>" + detalle.NroReferencia + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Importe.ToString("N2") + "</td>";
                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:eliminarForma(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarForma(" + detalle.ID + ", '" + detalle.FormaDePago + "', '" + detalle.NroReferencia + "','" + detalle.Importe.ToString("").Replace(".", ",") + "','" + detalle.IDBanco + "','" + detalle.IDCheque + "','" + detalle.IDNotaCredito + "');\"><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='5' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static string obtenerFormasTotal()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var importeFormas = CobranzaCart.Retrieve().FormasDePago.ToList().Sum(x => x.Importe);
            var importeComprobantes = CobranzaCart.Retrieve().Items.ToList().Sum(x => x.Importe);

            return Math.Round((importeComprobantes - importeFormas), 2).ToString("N2");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    #endregion

    #region Retenciones

    [WebMethod(true)]
    public static void agregarRetencion(int id, string tipo, string nroRef, string importe)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = CobranzaCart.Retrieve().Retenciones.Where(x => x.ID == id).FirstOrDefault();
                CobranzaCart.Retrieve().Retenciones.Remove(aux);
            }

            var tra = new CobranzasRetencionesViewModel();
            tra.ID = CobranzaCart.Retrieve().Retenciones.Count() + 1;
            tra.Tipo = tipo;
            tra.NroReferencia = nroRef;
            tra.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales));

            CobranzaCart.Retrieve().Retenciones.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarRetencion(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = CobranzaCart.Retrieve().Retenciones.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
                CobranzaCart.Retrieve().Retenciones.Remove(aux);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerRetenciones()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = CobranzaCart.Retrieve().Retenciones.OrderBy(x => x.Tipo).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td style='text-align:left'>" + detalle.Tipo + "</td>";
                        html += "<td style='text-align:left'>" + detalle.NroReferencia + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Importe.ToString("N2") + "</td>";
                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:CobRetenciones.eliminarRet(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:CobRetenciones.modificarRet(" + detalle.ID + ", '" + detalle.Tipo + "', '" + detalle.NroReferencia + "','" + detalle.Importe.ToString("").Replace(".", ",") + "');\"><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='5' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    #endregion

    [WebMethod(true)]
    public static int guardar(int id, int idPersona, string tipo, string fecha, int idPuntoVenta, string nroComprobante, string obs)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            CobranzaCartDto cobranza = new CobranzaCartDto();
            cobranza.IDCobranza = id;
            cobranza.IDPersona = idPersona;
            cobranza.Tipo = tipo;
            cobranza.Fecha = fecha;
            cobranza.IDPuntoVenta = idPuntoVenta;
            cobranza.NumeroCobranza = nroComprobante;
            cobranza.Observaciones = obs;;

            cobranza.Items = CobranzaCart.Retrieve().Items;
            cobranza.FormasDePago = CobranzaCart.Retrieve().FormasDePago;
            cobranza.Retenciones = CobranzaCart.Retrieve().Retenciones;

            var entity = CobranzasCommon.Guardar(cobranza, usu);

            if (PermisosModulos.tienePlan("alertas.aspx"))
                generarAlertas(entity);

            return entity.IDCobranza;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string previsualizar(int id, int idPersona, string tipo, /*string modo,*/ string fecha, int idPuntoVenta, string nroComprobante, string obs)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {

            if (CobranzaCart.Retrieve().Items.Count == 0)
                throw new Exception("Ingrese un comprobante");

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            Common.CrearCobranza(usu, id, idPersona, tipo, fecha, ref nroComprobante, obs, Common.ComprobanteModo.Previsualizar);
            return usu.IDUsuario.ToString() + "_prev.pdf";
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string generarPDF(int id, int idPersona, string tipo, string fecha, int idPuntoVenta, string nroComprobante, string obs)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            Common.CrearCobranza(usu, id, idPersona, tipo, fecha, ref nroComprobante, obs, Common.ComprobanteModo.Generar);
            var razonSocial = string.Empty;
            using (var dbContext = new ACHEEntities())
                razonSocial = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault().RazonSocial;

            return razonSocial.RemoverCaracteresParaPDF() + "_" + nroComprobante + ".pdf";
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void EnviarCobranzaAutomaticamente(int idCobranza, string nombre)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            if (usu.EnvioAutomaticoRecibo == true)
            {
                using (var dbContext = new ACHEEntities())
                {
                    try
                    {
                        var plandeCuenta = PermisosModulosCommon.ObtenerPlanActual(dbContext, usu.IDUsuario);
                        if (plandeCuenta.IDPlan >= 3)//Plan Pyme
                        {
                            //TODO: PASAR A NEGOCIO
                            bool CompEnviado = dbContext.ComprobantesEnviados.Any(x => x.IDUsuario == usu.IDUsuario && x.IDCobranza == idCobranza && x.Resultado == true);
                            if (!CompEnviado)
                            {
                                var avisos = dbContext.AvisosVencimiento.Where(x => x.IDUsuario == usu.IDUsuario && x.TipoAlerta == "Envio CR").FirstOrDefault();
                                if (avisos != null)
                                {
                                    var c = dbContext.Cobranzas.Where(x => x.IDCobranza == idCobranza && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                                    if (c.Personas.Email != null && c.Personas.Email != "")
                                    {
                                        var mensaje = avisos.Mensaje.ReplaceAll("\n", "<br/>");
                                        Common.EnviarComprobantePorMail(c.Personas.RazonSocial, c.Personas.Email, avisos.Asunto, mensaje, nombre);
                                        ComprobantesCommon.GuardarComprobantesEnviados(dbContext, null, idCobranza, "Cobranza enviada correctamente", true, usu);
                                    }
                                    else
                                        throw new Exception("El cliente no tiene configurado el correo electrónico");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ComprobantesCommon.GuardarComprobantesEnviados(dbContext, null, idCobranza, "Cobranza: " + ex.Message, false, usu);
                        throw new Exception("No se pudo enviar automáticamente recibo de cobranza.");
                    }
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static CobranzasEditViewModel obtenerDatos(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Cobranzas entity = dbContext.Cobranzas
                    .Include("CobranzasDetalle").Include("CobranzasFormasDePago").Include("CobranzasRetenciones")
                    .Where(x => x.IDCobranza == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                var cheques = dbContext.Cheques.Where(w => w.IDUsuario == usu.IDUsuario).ToList();

                if (entity != null)
                {
                    CobranzasEditViewModel result = new CobranzasEditViewModel();
                    result.ID = id;
                    result.IDPersona = entity.IDPersona;
                    result.Fecha = entity.FechaCobranza.ToString("dd/MM/yyyy");
                    result.Modo = entity.Modo;
                    result.Tipo = entity.Tipo;
                    result.Numero = entity.Numero.ToString("#00000000");
                    result.IDPuntoVenta = entity.IDPuntoVenta;
                    result.Observaciones = entity.Observaciones;


                    var personas = dbContext.Personas.Where(x => x.IDPersona == entity.IDPersona).FirstOrDefault();
                    PersonasEditViewModel PersonasEdit = new PersonasEditViewModel();
                    PersonasEdit.ID = id;
                    PersonasEdit.RazonSocial = personas.RazonSocial.ToUpper();
                    PersonasEdit.Email = personas.Email.ToLower();
                    PersonasEdit.CondicionIva = personas.CondicionIva;
                    PersonasEdit.Domicilio = personas.Domicilio.ToUpper() + " " + personas.PisoDepto;
                    PersonasEdit.Ciudad = personas.Ciudades.Nombre.ToUpper();
                    PersonasEdit.Provincia = personas.Provincias.Nombre;
                    PersonasEdit.TipoDoc = personas.TipoDocumento;
                    PersonasEdit.NroDoc = personas.NroDocumento;
                    result.Personas = PersonasEdit;

                    foreach (var det in entity.CobranzasDetalle)
                    {
                        var tra = new CobranzasDetalleViewModel();
                        tra.ID = CobranzaCart.Retrieve().Items.Count() + 1;
                        tra.Comprobante = det.Comprobantes.Tipo + " " + det.Comprobantes.PuntosDeVenta.Punto.ToString("#0000") + "-" + det.Comprobantes.Numero.ToString("#00000000");
                        tra.Importe = det.Importe;
                        tra.Fecha = det.Comprobantes.FechaComprobante.ToString("dd/MM/yyyy");

                        tra.IDComprobante = det.IDComprobante;

                        CobranzaCart.Retrieve().Items.Add(tra);
                    }

                    foreach (var det in entity.CobranzasFormasDePago)
                    {
                        var tra = new CobranzasFormasDePagoViewModel();
                        tra.ID = CobranzaCart.Retrieve().FormasDePago.Count() + 1;
                        tra.FormaDePago = det.FormaDePago;
                        tra.NroReferencia = det.NroReferencia;
                        tra.Importe = det.Importe;
                        tra.IDCheque = det.IDCheque;
                        tra.IDBanco = det.IDBanco;
                        tra.IDNotaCredito = det.IDNotaCredito;
                        if (det.IDCheque != null)
                            tra.Fecha = cheques.Where(w => w.IDCheque == det.IDCheque).Select(s => s.FechaEmision).FirstOrDefault().ToString("dd/MM/yyyy");
                        else
                            tra.Fecha = det.Cobranzas.FechaAlta.ToString("dd/MM/yyyy");

                        CobranzaCart.Retrieve().FormasDePago.Add(tra);
                    }

                    foreach (var det in entity.CobranzasRetenciones)
                    {
                        var tra = new CobranzasRetencionesViewModel();
                        tra.ID = CobranzaCart.Retrieve().Retenciones.Count() + 1;
                        tra.Tipo = det.Tipo;
                        tra.NroReferencia = det.NroReferencia;
                        tra.Importe = det.Importe;
                        tra.Fecha = det.Cobranzas.FechaAlta.ToString("dd/MM/yyyy");

                        CobranzaCart.Retrieve().Retenciones.Add(tra);
                    }

                    return result;
                }
                else
                    throw new Exception("Error al obtener los datos");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerComprobantesPendientes(int id, int idCobranza)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            var list = CobranzaCart.Retrieve().Items.Select(s => s.IDComprobante).ToList();

            using (var dbContext = new ACHEEntities())
            {
                var tiposComprobantesExcluidos = new[] { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NDP", "PDC", "EDA" };

                List<Comprobantes> lista = new List<Comprobantes>();

                if (idCobranza == 0)
                {
                    return dbContext.Comprobantes
                         .Where(x => x.IDUsuario == usu.IDUsuario 
                                && x.IDPersona == id && x.Saldo > 0 
                                && !tiposComprobantesExcluidos.Contains(x.Tipo)
                                && !list.Contains(x.IDComprobante)).ToList()
                         .Select(x => new Combo2ViewModel()
                         {
                             ID = x.IDComprobante,
                             Nombre = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000") + " (Saldo: $ " + x.Saldo.ToString("N2") + ")"
                         }).OrderBy(x => x.Nombre).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    return dbContext.CobranzasDetalle.Where(x => x.Comprobantes.IDUsuario == usu.IDUsuario 
                                                            && x.Comprobantes.IDPersona == id 
                                                            && x.IDCobranza == idCobranza 
                                                            && !tiposComprobantesExcluidos.Contains(x.Comprobantes.Tipo)
                                                            && !list.Contains(x.IDComprobante)).ToList()
                     .Select(x => new Combo2ViewModel()
                     {
                         ID = x.Comprobantes.IDComprobante,
                         Nombre = x.Comprobantes.Tipo + " " + x.Comprobantes.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Comprobantes.Numero.ToString("#00000000") + " (Saldo: $ " + x.Comprobantes.Saldo.ToString("N2") + ")"
                     }).ToList();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerFormasDePagoCobranzas(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string[] tipoComprobantesExcluidos = { "COT", "FCA", "FCB", "FCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "NDAMP", "NDBMP", "NDCMP" };

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario 
                    && x.IDPersona == idPersona && x.Saldo > 0 
                    && !tipoComprobantesExcluidos.Contains(x.Tipo)).ToList()
                         .Select(x => new Combo2ViewModel()
                         {
                             ID = x.IDComprobante,
                             Nombre = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000") + " (Saldo: $ " + x.Saldo.ToString("N2") + ")"
                         }).OrderBy(x => x.Nombre).OrderByDescending(x => x.ID).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    //TODO: PASAR A NEGOCIO
    private static void generarAlertas(Cobranzas cobranzas)
    {
        using (var dbContext = new ACHEEntities())
        {
            var listaAlertas = dbContext.Alertas.Where(x => x.IDUsuario == cobranzas.IDUsuario).ToList();

            foreach (var alertas in listaAlertas)
            {
                if (alertas.AvisoAlerta == "El cobro a un cliente es")
                {
                    switch (alertas.Condicion)
                    {
                        case "Mayor o igual que":
                            if (cobranzas.ImporteTotal >= alertas.Importe)
                            {
                                insertarAlerta(dbContext, cobranzas, alertas);
                            }
                            break;
                        case "Menor o igual que":
                            if (cobranzas.ImporteTotal <= alertas.Importe)
                            {
                                insertarAlerta(dbContext, cobranzas, alertas);
                            }
                            break;
                    }
                }
            }
        }
    }

    //TODO: PASAR A NEGOCIO
    private static void insertarAlerta(ACHEEntities dbContext, Cobranzas cobranzas, Alertas alertas)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        AlertasGeneradas entity = new AlertasGeneradas();

        entity.IDAlerta = alertas.IDAlerta;
        entity.IDUsuario = cobranzas.IDUsuario;
        entity.IDPersona = cobranzas.IDPersona;

        entity.ImportePagado = cobranzas.ImporteTotal;
        entity.Visible = true;
        entity.Fecha = DateTime.Now.Date;
        entity.IDCobranzas = cobranzas.IDCobranza;

        entity.NroComprobante = "Se cobraron los Comprobante/s: ";
        foreach (var item in cobranzas.CobranzasDetalle)
        {
            var comprobante = dbContext.Comprobantes.Where(x => x.IDComprobante == item.IDComprobante).FirstOrDefault();
            entity.NroComprobante += comprobante.Tipo + " " + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranzas.Numero.ToString("#00000000") + " ; ";
        }

        entity.NroComprobante = entity.NroComprobante.Substring(0, entity.NroComprobante.Length - 3);
        entity.NroComprobante += ".";

        dbContext.AlertasGeneradas.Add(entity);
        dbContext.SaveChanges();

        var alerta = alertas.AvisoAlerta + " - " + alertas.Condicion + " - $" + alertas.Importe;
        var descripcion = entity.NroComprobante;
        enviarEmailAlerta(alerta, descripcion);
    }

    //TODO: PASAR A NEGOCIO
    public static void enviarEmailAlerta(string alerta, string descripcion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            var email = (usu.EmailAlerta == "") ? usu.Email : usu.EmailAlerta;

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<ALERTA>", alerta);
            replacements.Add("<USUARIO>", usu.RazonSocial);
            replacements.Add("<DESCRIPCION>", descripcion);
            replacements.Add("<EMAIL>", email);

            bool send = EmailHelper.SendMessage(EmailTemplate.Alertas, replacements, usu.Email, "");

            if (!send)
                throw new Exception("Comprobante El mensaje no pudo ser enviado. Por favor, escribenos a <a href='mailto:ayuda@axanweb.com'>ayuda@axanweb.com</a>");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}