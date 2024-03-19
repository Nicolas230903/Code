<%@ WebHandler Language="C#" Class="mercadopago" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACHE.Model;
using ACHE.Extensions;
using System.Web.Security;
using System.Collections.Specialized;
using ACHE.MercadoPago;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using ACHE.Negocio.Common;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using System.Configuration;
public class mercadopago : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.QueryString["id"] != "null")
        {
            try
            {
                string idMP = context.Request.QueryString["id"];
                API.ClientIDMercadoPago = System.Configuration.ConfigurationManager.AppSettings["MP.ClientID"];
                API.PasswordMercadoPago = System.Configuration.ConfigurationManager.AppSettings["MP.ClientSecret"];

                string infoPago = API.GetPago(idMP);

                var dynamicObject = System.Web.Helpers.Json.Decode(infoPago);
                var external_reference = dynamicObject.collection.external_reference;
                //Obtengo la info del pago desde MP
                var MetodoDePago = dynamicObject.collection.payment_type;
                var EstadoMP = dynamicObject.collection.status;


                var aux = external_reference.ToString();
                var ListaReferencias = aux.Split('-');
                int idPlan = int.Parse(ListaReferencias[0].Replace("P", ""));
                int IdUsuario = int.Parse(ListaReferencias[1].Replace("U", ""));
                bool pagoAnual = ListaReferencias[2].Replace("A", "") == "1" ? true : false;


                //IdUsuario = 2127; // TEST
                //EstadoMP = "approved";// TEST
                if (EstadoMP == "rejected" || EstadoMP == "refunded" || EstadoMP == "cancelled" || EstadoMP == "pending")
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        agregarPago(external_reference, EstadoMP, idPlan, IdUsuario, pagoAnual, dbContext);
                        dbContext.SaveChanges();
                    }
                }
                else if (EstadoMP == "approved")
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        var planesPagos = dbContext.PlanesPagos.Where(x => x.IDUsuario == IdUsuario && x.IDPlan == idPlan && x.PagoAnual == pagoAnual && x.Estado == "Pendiente")
                                                     .OrderByDescending(x => x.IDPlanesPagos)
                                                     .FirstOrDefault();

                        if (planesPagos == null)
                            planesPagos = agregarPago(external_reference, EstadoMP, idPlan, IdUsuario, pagoAnual, dbContext);
                        else
                            planesPagos.Estado = "Aceptado";
                        
                        var usuario = dbContext.Usuarios.Where(x => x.IDUsuario == IdUsuario).FirstOrDefault();

                        if (idPlan == 5)
                            usuario.UsaPlanCorporativo = true;
                        else
                            usuario.UsaPlanCorporativo = false;
                        
                        enviarEmail(planesPagos, usuario);

                        var pathBase = HttpContext.Current.Server.MapPath("");
                        var comprobanteViewModel = ComprobantesCommon.CrearDatosParaContabilium(idMP, IdUsuario, dbContext, planesPagos, pathBase);
                        planesPagos.IDComprobante = comprobanteViewModel.comprobante.IDComprobante;
                        dbContext.SaveChanges();

                        PersonasCommon.CrearDatosParaElCliente(idMP, IdUsuario, dbContext, planesPagos, comprobanteViewModel.comprobante, comprobanteViewModel.nroComprobanteElectronico);
                        EnviarComprobantePorEmail(comprobanteViewModel);
                    }
                }
                context.Response.StatusCode = 200;
            }
            catch (Exception e)
            {
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "Error procesar el pago ", msg);
            }
        }
    }
  
    private static PlanesPagos agregarPago(dynamic external_reference, dynamic EstadoMP, int idPlan, int IdUsuario, bool pagoAnual, ACHEEntities dbContext)
    {
        var plan = dbContext.Planes.Where(x => x.IDPlan == idPlan).FirstOrDefault();
        var usu = dbContext.Usuarios.Where(x => x.IDUsuario == IdUsuario).FirstOrDefault();
        var ultimoPago = dbContext.PlanesPagos.Where(x => x.IDUsuario == IdUsuario && x.Estado == "Aceptado").OrderByDescending(x => x.IDPlanesPagos).FirstOrDefault();

        PlanesPagos p = new PlanesPagos();
        p.IDUsuario = IdUsuario;
        p.IDPlan = idPlan;
        p.FechaDeAlta = DateTime.Now.Date;

        p.PagoAnual = pagoAnual;
        p.FormaDePago = "Mercado Pago";
        p.NroReferencia = external_reference;
        p.FechaDePago = DateTime.Now.Date;

        if (ultimoPago == null)
            p.FechaInicioPlan = p.FechaDePago;
        else
            p.FechaInicioPlan = ultimoPago.FechaFinPlan;

        p.FechaFinPlan = (p.PagoAnual) ? Convert.ToDateTime(p.FechaInicioPlan).AddYears(1) : Convert.ToDateTime(p.FechaInicioPlan).AddMonths(1);

        if (EstadoMP == "pending")
            p.Estado = "Pendiente";
        else if (EstadoMP == "approved")
            p.Estado = "Aceptado";
        else
            p.Estado = EstadoMP;

        decimal importeTotal = 0;

        // hardcodeado por estos 6 meses para los usuarios que ya fueron creados antes de 31/07/2015
        var fecha = Convert.ToDateTime("31/07/2015");
        if (usu.FechaAlta <= fecha && p.IDPlan == 2)
            importeTotal = (pagoAnual) ? 149 * 12 : 149;
        else// FIN hardcodeado 
            importeTotal = (pagoAnual) ? plan.Precio * 12 : plan.Precio;
        
        
        importeTotal = importeTotal + ((importeTotal * 21) / 100);
        p.ImportePagado = importeTotal;
        dbContext.PlanesPagos.Add(p);

        return p;
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

    private static void enviarEmail(PlanesPagos planesPago, Usuarios usu)
    {
        ListDictionary replacements = new ListDictionary();
        replacements.Add("<USUARIO-MAIL>", usu.RazonSocial + " -CUIT: " + usu.CUIT);
        replacements.Add("<USUARIO>", usu.RazonSocial);
        replacements.Add("<ID>", usu.IDUsuario);
        replacements.Add("<EMAIL>", usu.Email);
        replacements.Add("<FORMAPAGO>", planesPago.FormaDePago);
        replacements.Add("<IMPORTE>", planesPago.ImportePagado);
        replacements.Add("<NOTIFICACION>", "Hemos recibido los datos de su pago y los mismos han sido verificados. Su plan ya se encuentra renovado.");

        bool send = EmailHelper.SendMessage(EmailTemplate.PagoPlanes, replacements, System.Configuration.ConfigurationManager.AppSettings["Email.Administracion"], "axanweb: Nuevo pago recibido");
        bool send2 = EmailHelper.SendMessage(EmailTemplate.Notificacion, replacements, usu.Email, "axanweb: Su pago ha sido acreditado.");
        if (!send || !send2)
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "Error al enviar mails del IDPlanesPagos: " + planesPago.IDPlanesPagos + " UDUID:" + usu.IDUsuario, "");
    }

    private static void EnviarComprobantePorEmail(ComprobanteNroViewModel comprobante)
    {
        System.Net.Mail.MailAddressCollection listTo = new System.Net.Mail.MailAddressCollection();

        listTo.Add(new System.Net.Mail.MailAddress(comprobante.comprobante.Personas.Email));

        ListDictionary replacements = new ListDictionary();
        replacements.Add("<NOTIFICACION>", "Comprobante de contratación de plan");
        replacements.Add("<USUARIO>", comprobante.comprobante.Personas.RazonSocial);


        var nroComprobane = comprobante.comprobante.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + comprobante.comprobante.Tipo + "-" + comprobante.nroComprobanteElectronico + ".pdf";
        List<string> attachments = new List<string>();
        attachments.Add(HttpContext.Current.Server.MapPath("~/files/explorer/" + comprobante.comprobante.Usuarios.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/" + nroComprobane));
        bool send = EmailHelper.SendMessage(EmailTemplate.EnvioComprobante, replacements, listTo, ConfigurationManager.AppSettings["Email.Notifications"], comprobante.comprobante.Personas.Email, "Comprobante electrónico", attachments);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
