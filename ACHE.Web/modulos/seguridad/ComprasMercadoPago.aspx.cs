using ACHE.MercadoPago;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using ACHE.Negocio.Contabilidad;


public partial class modulos_ventas_ComprasMercadoPago : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var resultado = string.Empty;
            var MensajeResultado = string.Empty;
            var tipo = Request.QueryString["tipo"];
            var external_reference = Request.QueryString["external_reference"];

            switch (tipo)
            {
                case "1":
                    resultado = "Exitoso";
                    MensajeResultado = "El pago fue realizado correctamente, en menos de 24 horas se actualizara su cuenta";
                    verificarPago();
                    break;

                case "2":
                    resultado = "Pendiente";
                    MensajeResultado = "El pago se encuentra Pendiente";
                    break;
                case "5":
                    resultado = "Fallido";
                    MensajeResultado = "El pago no pudo ser realizado correctamente";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(external_reference))
            {
                var ListaReferencias = Regex.Split(external_reference, "-").ToList();
                var idPlan = Convert.ToInt32(ListaReferencias.Where(x => x.Contains("P")).FirstOrDefault().Replace("P", ""));


                using (var dbContext = new ACHEEntities())
                {
                    var plan = dbContext.Planes.Where(x => x.IDPlan == idPlan).FirstOrDefault();

                    liPlan.Text = plan.Nombre;

                    var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];// hardcodeado por estos 6 meses para los usuarios que ya fueron creados antes de 31/07/2015
                    var fecha = Convert.ToDateTime("31/07/2015");
                    if (usu.FechaAlta <= fecha && idPlan == 2)
                        liImporte.Text = "149";
                    else // FIN hardcodeado 
                        liImporte.Text = plan.Precio.ToString("N2");

                    liResultadoOperacion.Text = resultado;
                    liMensajeResultadoOperacion.Text = MensajeResultado;
                }
            }
        }
    }

    private void verificarPago()
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var planes = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
            var idPlanActual = PermisosModulos.ObtenerTodosLosFormularios(dbContext, usu.IDUsuario);
            var PlanVigente = PermisosModulos.PlanVigente(dbContext, usu.IDUsuario);
            //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(dbContext, usu.IDUsuario, usu.CondicionIVA);
            HttpContext.Current.Session["CurrentUser"] = new WebUser(
            usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, usu.CondicionIVA,
            usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia,
            usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, usu.IIBB, usu.FechaInicio,
            usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa, usu.ModoQA, idPlanActual,
            usu.EmailAlerta, usu.Provincia, usu.Ciudad, usu.AgentePercepcionIVA, usu.AgentePercepcionIIBB, usu.AgenteRetencionGanancia, usu.AgenteRetencion,
            PlanVigente, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA, usu.FechaAlta,
            usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo, 
            usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, 
            usu.PorcentajeCompra, usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
            usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);
        }
    }
}