using ACHE.FacturaElectronica;
using ACHE.FacturaElectronica.VEConsumerService;
using ACHE.Model;
using ACHE.Negocio.Common;
using ACHE.Negocio.Contabilidad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class controls_header : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["CurrentUser"] != null)
        {
            var user = (WebUser)Session["CurrentUser"];

            int cantAvisos = 0;

            this.imgLogoMenu.Src = "/images/logo-inside.png";
            this.imgLogoMenu.Alt = "logoelumweb";   


            //Valido qué avisos se muestran
            //if (!user.TieneFE && user.CondicionIVA == "RI" && user.SetupFinalizado)
            if (!user.TieneFE && user.SetupFinalizado)
            {
                cantAvisos++;
                liFE.Visible = true;
            }
            else
                liFE.Visible = false;

            //liIva1.Visible = liIva2.Visible = liIva3.Visible = (user.CondicionIVA == "RI");

            if (user.TipoUsuario == "B")
                mostrarHeaderSegunPermiso();

            using (var dbContext = new ACHEEntities())
            {
                bool puntos = dbContext.PuntosDeVenta.Any(x => x.IDUsuario == user.IDUsuario);
                if (!puntos)
                {
                    cantAvisos++;
                    liPuntosDeVenta.Visible = true;
                }
                else
                    liPuntosDeVenta.Visible = false;

                cantAvisos += verificarALertas(dbContext);

                //oculta header
                setupFinalizado.Visible = user.SetupFinalizado;


                if (user.UsaPlanCorporativo) //Plan Corporativo
                {
                    //liContabilidad.Visible = true;
                    liImportarPlanDeCuentas.Visible = true;
                }
                else
                {
                    //liContabilidad.Visible = false;
                    liImportarPlanDeCuentas.Visible = false;
                }

                var nombrePlan = string.Empty;
                var cantDias = 0;

                if (Common.AlertaPlan(dbContext, user.IDUsuario, ref nombrePlan, ref cantDias))
                {
                    var txtDias = string.Empty;
                    if (cantDias == 1)
                        txtDias = cantDias.ToString() + " día.";
                    else
                        txtDias = cantDias.ToString() + " días.";
                    
                    cantAvisos++;
                    liPagarPlan.Text = "<li><a href='#' data-toggle='modal' onclick=Common.pagarPlanActual('" + nombrePlan.ToString() + "'); ><span class='desc' style='margin-left: 0px'><span class='name'>Pagar/Renovar plan " + nombrePlan + " </span><span class='msg'>Su plan vence en " + txtDias + "</span></span></a></li>";
                }

                //Consulto comunicaciones de AFIP en la db y si tiene no leidas las agrego como aviso
                //liComunicacionesAFIP.Visible = true;
                List<ComunicacionesAFIP> lc = dbContext.ComunicacionesAFIP.Where(x => x.IdUsuario == user.IDUsuario).ToList();
                if (lc.Count > 0)
                {
                    litCantMsjCmAfipLeidas.Text = lc.Count.ToString();
                    int cantidadNoLeidos = 0;

                    foreach (ComunicacionesAFIP c in lc)
                    {
                        if (c.Visto == 0)
                            cantidadNoLeidos++;
                    }

                    if (cantidadNoLeidos > 0)
                        cantAvisos++;

                    litCantMsjCmAfip.Text = cantidadNoLeidos.ToString();                    
                }
                else
                {
                    litCantMsjCmAfip.Text = "0";
                    litCantMsjCmAfipLeidas.Text = "0";
                }

                divDatos.InnerHtml = "ID: " + user.IDUsuario.ToString() + " - Razón Social: " + user.RazonSocial + " - Email: " + user.Email;

            }

            btnMensaje.Visible = user.SetupFinalizado;

            if (user.SetupFinalizado)
                LitCant.Text = (cantAvisos > 0) ? "<strong><span id='spamMensajes'> Mensajes (" + cantAvisos.ToString() + ")</span></strong>" : "<span id='spamMensajes'> Mensajes</span>";


            //if (user.SetupFinalizado)
            //    LitCant.Text = (cantAvisos > 0) ? "<i class='glyphicon glyphicon-envelope'></i><strong><span id='spamMensajes'> Mensajes (" + cantAvisos.ToString() + ")</span></strong>" : "<i class='glyphicon glyphicon-envelope'></i><span id='spamMensajes'> Mensajes</span>";

            //Muestro avisos  
            litMsjCant.Text = "<span id='spanMsgcantidad'>" + cantAvisos.ToString() + "</span >";
            if (cantAvisos == 0)
                litMsjTitulo.Text = "No tienes avisos";
            else
                litMsjTitulo.Text = "<span id='spanMsgTitulo'>Tienes " + cantAvisos + " aviso/s" + "</span>";

            if (PermisosModulos.tienePlan("Empresas"))
                mostrarMultiEmpresas();
            else
                divUsuariosMultiempresas.Visible = false;

        }
    }

    public String recuperarNombreDePagina()
    {
        return HttpContext.Current.Request.Url.LocalPath;
    }

    private void mostrarMultiEmpresas()
    {
        var user = (WebUser)Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            if (OcultarDatosUsuarioEmpresas(dbContext))
            {
                List<EmpresasViewModel> list = new List<EmpresasViewModel>();
                list = UsuarioCommon.ListaEmpresasDisponibles(user, dbContext);

                if (list.Count <= 1)
                    divUsuariosMultiempresas.Visible = false;
                else
                {
                    foreach (var item in list.OrderBy(x=>x.RazonSocial))
                    {
                        var direccion = item.Domicilio + ", " + item.Ciudad + ", " + item.Provincia;
                        var datosInpositivos = item.CUIT + ", " + item.CondicionIva;

                        if (item.ID != user.IDUsuario)
                            liEmpresasHeader.Text += "<li style='padding: 5px 5px;'><a href='#' onclick=\"Common.cambiarSesion(" + item.ID + ",'" + item.RazonSocial + "');\"><span class='desc' style='margin-left: 0px'><span class='name'>" + item.RazonSocial + " - " + item.CUIT + "</span></a></li>";
                        else
                            litEmpresaActual.Text = item.RazonSocial;
                    }
                }
            }
        }
    }

    private bool OcultarDatosUsuarioEmpresas(ACHEEntities dbContext)
    {
        var CurrentUser = (WebUser)Session["CurrentUser"];
        var tieneAcceso = true;
        if (CurrentUser.IDUsuarioPadre > 0 && CurrentUser.IDUsuarioAdicional > 1)
        {
           var usuAdic = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();
            if (usuAdic.IDUsuario != CurrentUser.IDUsuarioPadre)
            {
                divUsuariosMultiempresas.Visible = false;
                litSinMultiEmpresa.Text = "Empresa Actual: " + CurrentUser.RazonSocial;
                tieneAcceso = false;
            }
        }
        return tieneAcceso;
    }

    private void mostrarHeaderSegunPermiso()
    {
        //Ventas
        if (PermisosModulos.ocultarHeader(2))
            liVentas.Visible = true;
        else
            liVentas.Visible = false;

        //Compras
        if (PermisosModulos.ocultarHeader(1))
            liCompras.Visible = true;
        else
            liCompras.Visible = false;

        //Tesoreria
        if (PermisosModulos.ocultarHeader(3))
            liTesoreria.Visible = true;
        else
            liTesoreria.Visible = false;

        //Reportes
        if (PermisosModulos.ocultarHeader(4))
            liReportes.Visible = true;
        else
            liReportes.Visible = false;

        //Herramientas
        if (PermisosModulos.ocultarHeader(5))
            liHerramientas.Visible = true;
        else
            liHerramientas.Visible = false;

        //Recursos Humanos
        //if (PermisosModulos.ocultarHeader(7))
        //    liRRHH.Visible = true;
        //else
        //    liRRHH.Visible = false;

    }

    private int verificarALertas(ACHEEntities dbContext)
    {
        var user = (WebUser)Session["CurrentUser"];
        var listaAlertas = dbContext.AlertasGeneradas.Include("Personas").Where(x => x.IDUsuario == user.IDUsuario && x.Visible);

        foreach (var alertas in listaAlertas)
        {

            var tipo = (alertas.Alertas.AvisoAlerta == "El pago a un proveedor es") ? "1" : "0";
            var id = (tipo == "1") ? alertas.IDPagos : alertas.IDCobranzas;
            var persona = (tipo == "1") ? " Al Proveedor: " : " Al Cliente: ";

            var mensaje = alertas.NroComprobante + persona + alertas.Personas.RazonSocial;
            litAlertas.Text += "<li class='new' id='alertaGenerada_" + alertas.IDAlertasGeneradas + "'><a href='#'><span class='desc' style='margin-left:0px'><span class='name' onclick='alertas.abrirComprobante(" + id + "," + tipo + ");'>" + mensaje + "</span><span class='msg' id='msgAlertasPagos' onclick='alertas.esconderAlertaGenerada(" + alertas.IDAlertasGeneradas + ");'>Haga click aqui para ocultar las alertas.</span></span></a></li>";
        }
        return listaAlertas.Count();
    }
}
