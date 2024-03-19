using ACHE.Extensions;
using ACHE.MercadoPago;
using ACHE.Model;
using ACHE.Negocio.Contabilidad;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class modulos_ventas_pagoDePlanes : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ",";//"."
    public const string SeparadorDeDecimales = ".";//","

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFechaDePago.Text = DateTime.Now.ToString("dd/MM/yyyy");
            var plan = Request.QueryString["plan"];
            var modo = Request.QueryString["modo"];
            if (!string.IsNullOrWhiteSpace(plan))
            {
                using (var dbContext = new ACHEEntities())
                {
                    var planSeleccionado = dbContext.Planes.Where(x => x.Nombre.ToUpper().Contains(plan.ToUpper())).FirstOrDefault();
                    if (planSeleccionado != null)
                    {
                        hdnIdPlan.Value = planSeleccionado.IDPlan.ToString();

                        if (hdnIdPlan.Value == "6")
                            Response.Redirect("/modulos/seguridad/elegir-plan.aspx?upgrade=0");

                        var NombrePlan = planSeleccionado.Nombre;
                        decimal importeTotal = 0;


                        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];// hardcodeado por estos 6 meses para los usuarios que ya fueron creados antes de 31/07/2015
                        var fecha = Convert.ToDateTime("31/07/2015");
                        if (usu.FechaAlta <= fecha && hdnIdPlan.Value == "2")
                            importeTotal = (Convert.ToBoolean(modo)) ? 149 * 12 : 149;
                        else// FIN hardcodeado 
                            importeTotal = (Convert.ToBoolean(modo)) ? planSeleccionado.Precio * 12 : planSeleccionado.Precio;

                        if (Convert.ToBoolean(modo))
                        {
                            decimal descuento = importeTotal * Convert.ToDecimal("0,10");
                            importeTotal = importeTotal - descuento;
                        }
                        importeTotal = importeTotal + ((importeTotal * 21) / 100);

                        hdnImporteTotal.Value = importeTotal.ToString();
                        hdnNombrePlan.Value = NombrePlan;
                        hdnModo.Value = modo;
                        btnMercadoPago.Text = integracionMercadoPago(planSeleccionado.IDPlan, Convert.ToBoolean(modo));
                    }
                }
            }
        }
    }
    [WebMethod(true)]
    public static int GuardarPago(int id, int idPlan, string formaDePago, string importePagado, bool pagoAnual, string NroReferencia, string fechaDePago)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    PlanesPagos p;
                    if (id > 0)
                    {
                        p = dbContext.PlanesPagos.Where(x => x.IDPlanesPagos == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    }
                    else
                    {
                        p = new PlanesPagos();
                        p.FechaDeAlta = DateTime.Now.Date;
                        p.IDUsuario = usu.IDUsuario;

                        var ultimoPago = dbContext.PlanesPagos.Where(x => x.IDUsuario == usu.IDUsuario && x.Estado == "Aceptado").OrderByDescending(x => x.IDPlanesPagos).FirstOrDefault();

                        p.IDPlan = idPlan;
                        p.ImportePagado = decimal.Parse(importePagado.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                        p.PagoAnual = pagoAnual;
                        p.FormaDePago = formaDePago;
                        p.NroReferencia = NroReferencia;
                        p.FechaDePago = Convert.ToDateTime(fechaDePago);
                        p.Estado = "Pendiente";

                        if (ultimoPago == null)
                            p.FechaInicioPlan = p.FechaDePago;
                        else
                            p.FechaInicioPlan = ultimoPago.FechaFinPlan;

                        p.FechaFinPlan = (p.PagoAnual) ? Convert.ToDateTime(p.FechaInicioPlan).AddDays(365) : Convert.ToDateTime(p.FechaInicioPlan).AddDays(30);

                        dbContext.PlanesPagos.Add(p);
                        dbContext.SaveChanges();

                        var idPlanActual = PermisosModulos.ObtenerTodosLosFormularios(dbContext, usu.IDUsuario);
                        HttpContext.Current.Session["CurrentUser"] = new WebUser(
                        usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, usu.CondicionIVA,
                        usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia,
                        usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, usu.IIBB, usu.FechaInicio,
                        usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa, usu.ModoQA, idPlanActual,
                        usu.EmailAlerta, usu.Provincia, usu.Ciudad, usu.AgentePercepcionIVA, usu.AgentePercepcionIIBB, usu.AgenteRetencionGanancia, usu.AgenteRetencion,
                        true, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA, usu.FechaAlta,
                        usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, false, usu.PedidoDeVenta,
                        usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, usu.PorcentajeCompra, 
                        usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
                        usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);
                        enviarEmail(p);
                    }
                    return p.IDPlanesPagos;
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

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static void GuardarPlanBasico()
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var ultimoPago = dbContext.PlanesPagos.Where(x => x.IDUsuario == usu.IDUsuario && x.Estado == "Aceptado").OrderByDescending(x => x.IDPlanesPagos).FirstOrDefault();

                    var idPlan = 1;
                    PlanesPagos p = new PlanesPagos();
                    p.IDUsuario = usu.IDUsuario;
                    p.IDPlan = idPlan;
                    p.FechaDeAlta = DateTime.Now.Date;
                    p.ImportePagado = 0;
                    p.PagoAnual = false;
                    p.FormaDePago = "-";
                    p.NroReferencia = "";
                    p.FechaDePago = DateTime.Now.Date;
                    p.Estado = "Aceptado";

                    if (ultimoPago == null)
                        p.FechaInicioPlan = p.FechaDePago;
                    else
                        p.FechaInicioPlan = ultimoPago.FechaFinPlan;

                    p.FechaFinPlan = Convert.ToDateTime(p.FechaInicioPlan).AddDays(30);

                    var usuario = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    usuario.UsaPlanCorporativo = false;

                    dbContext.PlanesPagos.Add(p);
                    dbContext.SaveChanges();

                    var idPlanActual = PermisosModulos.ObtenerTodosLosFormularios(dbContext, usu.IDUsuario);
                    //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(dbContext, usu.IDUsuario, usu.CondicionIVA);
                    HttpContext.Current.Session["CurrentUser"] = new WebUser(
                    usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, usu.CondicionIVA,
                    usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia,
                    usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, usu.IIBB, usu.FechaInicio,
                    usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa, usu.ModoQA, idPlanActual, usu.EmailAlerta,
                    usu.Provincia, usu.Ciudad, usu.AgentePercepcionIVA, usu.AgentePercepcionIIBB, usu.AgenteRetencionGanancia, usu.AgenteRetencion,
                    true, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA, usu.FechaAlta,
                    usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usuario.UsaPlanCorporativo, 
                    usuario.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, 
                    usu.PorcentajeCompra, usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
                    usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);
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

    private string integracionMercadoPago(int idPlan, bool pagoAnual)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var plan = dbContext.Planes.Where(x => x.IDPlan == idPlan).FirstOrDefault();
                var btnMP = string.Empty;
                if (plan != null)
                {
                    API.ClientIDMercadoPago = ConfigurationManager.AppSettings["MP.ClientID"];
                    API.PasswordMercadoPago = ConfigurationManager.AppSettings["MP.ClientSecret"];

                    if (!string.IsNullOrEmpty(API.ClientIDMercadoPago) && !string.IsNullOrEmpty(API.PasswordMercadoPago))
                    {
                        string NombrePlan = string.Empty;
                        decimal importeTotal = 0;
                        decimal descuento = 0;



                        var fecha = Convert.ToDateTime("31/07/2015");
                        if (usu.FechaAlta <= fecha && hdnIdPlan.Value == "2") // hardcodeado por estos 6 meses para los usuarios que ya fueron creados antes de 31/07/2015
                        {
                            NombrePlan = plan.Nombre + " - " + plan.Precio + " + IVA";
                            importeTotal = (pagoAnual) ? 149 * 12 : 149;
                            if (pagoAnual)
                            {
                                descuento = importeTotal * Convert.ToDecimal("0,10");
                                importeTotal = importeTotal - descuento;
                            }
                        }
                        else// FIN hardcodeado 
                        {
                            NombrePlan = plan.Nombre + " - " + plan.Precio + "+ IVA";
                            importeTotal = (pagoAnual) ? plan.Precio * 12 : plan.Precio;

                            if (pagoAnual)
                            {
                                descuento = importeTotal * Convert.ToDecimal("0,10");
                                importeTotal = importeTotal - descuento;
                            }
                        }



                        importeTotal = importeTotal + ((importeTotal * 21) / 100);
                        var referencia = "P" + plan.IDPlan.ToString() + "-U" + usu.IDUsuario.ToString() + "-A" + Convert.ToByte(pagoAnual).ToString();

                        var mpRef = API.AddPreference(referencia, "Pago del plan: " + NombrePlan, string.Empty, 1, importeTotal, API.Mondeda.ARS, string.Empty);
                        btnMP = "<a class='btn btn-success' id='lnkComprar' href='" + mpRef
                                + "&payer.name=" + usu.RazonSocial
                                + "&payer.surname=" + usu.RazonSocial
                                + "&payer.email=" + ConfigurationManager.AppSettings["Email.ReplyTo"]
                                + "' name='MP-Checkout' mp-mode='modal' onreturn='pagoCompleto'>pagar</a>";
                    }
                    else
                    {
                        throw new Exception("Mercado Pago no se encuentra configurado");
                    }
                }
                return btnMP;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static void enviarEmail(PlanesPagos planesPago)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<USUARIO>", usu.RazonSocial);
            replacements.Add("<ID>", usu.IDUsuario);
            replacements.Add("<EMAIL>", usu.Email);
            replacements.Add("<FORMAPAGO>", planesPago.FormaDePago);
            replacements.Add("<IMPORTE>", planesPago.ImportePagado);
            replacements.Add("<NOTIFICACION>", "Hemos recibido los datos de pago y los estaremos corroborando.");

            EmailHelper.SendMessage(EmailTemplate.PagoPlanes, replacements, ConfigurationManager.AppSettings["Email.Administracion"], "axanweb: Pago de plan");
            EmailHelper.SendMessage(EmailTemplate.Notificacion, replacements, usu.Email, "axanweb: Hemos recibido los datos de pago y los estaremos corroborando.");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}