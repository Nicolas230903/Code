<%@ WebHandler Language="C#" Class="mercadopago" %>

using ACHE.WebClientes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACHE.Model;
using ACHE.Extensions;
using System.Web.Security;
using System.Collections.Specialized;
using ACHE.MercadoPago;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;

public class mercadopago : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.QueryString["id"] != "null" && context.Request.QueryString["token"] != "null")
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    string idMP = context.Request.QueryString["id"];
                    string token = context.Request.QueryString["token"];
                    string log = "idMP : " + idMP + "token: " + token;
                    log = (log == null) ? "" : log;
                    BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "parametros de entrada ", log);

                    var usu = dbContext.Usuarios.Where(x => x.ApiKey == token).FirstOrDefault();
                    if (usu != null)
                    {
                        API.ClientIDMercadoPago = usu.MercadoPagoClientID;
                        API.PasswordMercadoPago = usu.MercadoPagoClientSecret;

                        if (API.ClientIDMercadoPago != null || API.PasswordMercadoPago != null)
                        {
                            string infoPago = API.GetPago(idMP);
                            var dynamicObject = System.Web.Helpers.Json.Decode(infoPago);

                            var referencia = dynamicObject.collection.external_reference.ToString();
                            int idComprobante = int.Parse(referencia);
                            if (idComprobante > 0)
                            {
                                var comprobante = dbContext.Comprobantes.Where(x => x.IDComprobante == idComprobante).FirstOrDefault();
                                if (comprobante != null)
                                {
                                    var MetodoDePago = dynamicObject.collection.payment_type;
                                    var EstadoMP = dynamicObject.collection.status;

                                    if (EstadoMP == "approved")
                                    {
                                        var IDCobranza = guardarCobranza(comprobante, dbContext, idMP, MetodoDePago, EstadoMP);
                                        if (TienePlan(dbContext, usu.IDUsuario))
                                            EnviarEmail(comprobante);
                                    }
                                }
                            }
                        }
                        else
                        {
                            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "no se encotro clientID o ClientSecret ", "no se encotro clientID o ClientSecret ");
                        }
                    }
                    else
                    {
                        BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "Token Incorrecto", "Token Incorrecto");
                    }
                }
                context.Response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "Error procesar el pago ", msg);
            }
        }
    }

    public static bool TienePlan(ACHEEntities dbContext, int idUsuario)
    {
        var plan = new PlanesPagos();
        var usu = dbContext.UsuariosView.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
        var Empresa = dbContext.Usuarios.Where(x => x.IDUsuarioPadre != null && x.IDUsuario == idUsuario).FirstOrDefault();

        if (usu.IDUsuarioAdicional > 0)
            plan = dbContext.PlanesPagos.Where(x => x.IDUsuario == usu.IDUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now && x.Estado == "Aceptado").FirstOrDefault();
        else if (Empresa != null)
            plan = dbContext.PlanesPagos.Where(x => x.IDUsuario == Empresa.IDUsuarioPadre && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now && x.Estado == "Aceptado").FirstOrDefault();
        else
            plan = dbContext.PlanesPagos.Where(x => x.IDUsuario == idUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now && x.Estado == "Aceptado").FirstOrDefault();

        if (plan.IDPlan >= 3)//Plan PYME
            return true;
        else
            return false;
    }

    private static int guardarCobranza(Comprobantes comprobante, ACHEEntities dbContext, string idMP, string MetodoDePago, string EstadoMP)
    {
        var punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == comprobante.IDPuntoVenta).FirstOrDefault();
        WebUser usu = ACHE.Negocio.Common.TokenCommon.ObtenerWebUser(comprobante.IDUsuario);

        CobranzaCartDto cobrCartdto = new CobranzaCartDto();
        cobrCartdto.IDCobranza = 0;
        cobrCartdto.IDPersona = comprobante.IDPersona;
        cobrCartdto.Tipo = "RC";
        cobrCartdto.Fecha = DateTime.Now.Date.ToString("dd/MM/yyyy");
        cobrCartdto.IDPuntoVenta = comprobante.IDPuntoVenta;
        cobrCartdto.NumeroCobranza = CobranzasCommon.obtenerProxNroCobranza(cobrCartdto.Tipo, comprobante.IDUsuario);
        cobrCartdto.Observaciones = "Comprobante generado automaticamente desde cliente, por una cobranza de Mercado Pago: Numero de referencia IDMP: " + idMP;


        CobranzasDetalleViewModel item = new CobranzasDetalleViewModel();
        cobrCartdto.Items = new List<CobranzasDetalleViewModel>();
        item.ID = 1;
        item.Comprobante = comprobante.Tipo + " " + punto.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
        item.Importe = comprobante.Saldo;
        item.IDComprobante = comprobante.IDComprobante;
        cobrCartdto.Items.Add(item);

        CobranzasFormasDePagoViewModel formasDePago = new CobranzasFormasDePagoViewModel();
        cobrCartdto.FormasDePago = new List<CobranzasFormasDePagoViewModel>();
        formasDePago.ID = 1;
        formasDePago.Importe = comprobante.Saldo;
        formasDePago.FormaDePago = MetodoDePago;
        formasDePago.NroReferencia = "idMp: " + idMP;
        cobrCartdto.FormasDePago.Add(formasDePago);
        cobrCartdto.Retenciones = new List<CobranzasRetencionesViewModel>();

        var cobranza = CobranzasCommon.Guardar(dbContext, cobrCartdto, usu);
        generarAlertas(dbContext, cobranza);

        return cobranza.IDCobranza;
    }
    private static void generarAlertas(ACHEEntities dbContext, Cobranzas cobranzas)
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
    private static void insertarAlerta(ACHEEntities dbContext, Cobranzas cobranzas, Alertas alertas)
    {

        AlertasGeneradas entity = new AlertasGeneradas();

        entity.IDAlerta = alertas.IDAlerta;
        entity.IDUsuario = cobranzas.IDUsuario;
        entity.IDPersona = cobranzas.IDPersona;

        entity.ImportePagado = cobranzas.ImporteTotal;
        entity.Visible = true;
        entity.Fecha = DateTime.Now.Date;
        entity.IDCobranzas = cobranzas.IDCobranza;

        entity.NroComprobante = "Mercado pago: Se cobro el Comprobante: ";
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
    }

    private static void EnviarEmail(Comprobantes comprobante)
    {
        ListDictionary replacements = new ListDictionary();
        var factura = comprobante.Tipo + "-" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
        bool send = EmailHelper.SendMessage(EmailTemplate.Notificacion, replacements, comprobante.Usuarios.Email, "AXAN: Hemos recibido la cobranza de la factura: " + factura);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
