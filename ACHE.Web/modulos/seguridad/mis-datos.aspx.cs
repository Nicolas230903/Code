using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Facturacion;
using System.Data.SqlClient;
using System.Data;
using System.ServiceModel.Security.Tokens;

public partial class mis_datos : BasePage
{
    private const string LOGO_PATH = "/files/usuarios/";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            hdnAccion.Value = Request.QueryString["accion"];

            IDUsuarioAdicional.Value = CurrentUser.IDUsuarioAdicional.ToString();


            liEmpresas.Visible = CurrentUser.TieneMultiEmpresa;
            liUrlMercadoPago.Text = "https://clientes.axanweb.com/mercadopago.ashx?token=" + CurrentUser.ApiKey;
            Empresas.Visible = CurrentUser.TieneMultiEmpresa;

            if (CurrentUser.IDUsuarioPadre != null && CurrentUser.IDUsuarioPadre > 0)
                btnNuevo.Visible = false;

            if (CurrentUser.IDUsuarioPadre > 0 || CurrentUser.IDUsuarioAdicional > 0)
            {
                liplanPagos.Visible = false;
                planPagos.Visible = false;
            }

            using (var dbContext = new ACHEEntities())
            {
                var usu = dbContext.Usuarios.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
                IDusuario.Value = usu.IDUsuario.ToString();

                OcultarDatosUsuarioEmpresas(dbContext);
                OcultarDatosUsuarioTemplates();

                if (usu != null)
                {
                    txtRazonSocial.Text = usu.RazonSocial;
                    ddlCondicionIva.SelectedValue = usu.CondicionIva;
                    txtCuit.Text = usu.CUIT;
                    txtIIBB.Text = usu.IIBB;
                    if (usu.FechaInicioActividades.HasValue)
                        txtFechaInicioAct.Text = usu.FechaInicioActividades.Value.ToString("dd/MM/yyyy");
                    ddlPersoneria.SelectedValue = usu.Personeria;
                    txtEmail.Text = usu.Email;
                    txtEmailAlertas.Text = usu.EmailAlertas;
                    txtContacto.Text = usu.Contacto;
                    txtCelular.Text = usu.Celular;
                    chkExento.Checked = Convert.ToBoolean(usu.ExentoIIBB);
                    //Domicilio
                    txtDomicilio.Text = usu.Domicilio;
                    hdnProvincia.Value = usu.IDProvincia.ToString();
                    hdnCiudad.Value = usu.IDCiudad.ToString();

                    txtPisoDepto.Text = usu.PisoDepto;
                    txtCp.Text = usu.CodigoPostal;
                    txtTelefono.Text = usu.Telefono;

                    //Portal Cliente
                    ChkCorreoPortal.Checked = Convert.ToBoolean(usu.CorreoPortal);
                    chkPortalClientes.Checked = Convert.ToBoolean(usu.PortalClientes);
                    txtClientId.Text = usu.MercadoPagoClientID;
                    txtClientSecret.Text = usu.MercadoPagoClientSecret;

                    //Datos Fiscales
                    esAgenteRetencionGanancia.Checked = Convert.ToBoolean(usu.EsAgenteRetencionGanancia);
                    esAgentePersepcionIVA.Checked = Convert.ToBoolean(usu.EsAgentePercepcionIVA);
                    esAgentePersepcionIIBB.Checked = Convert.ToBoolean(usu.EsAgentePercepcionIIBB);
                    esAgenteRetencion.Checked = Convert.ToBoolean(usu.EsAgenteRetencion);                   

                    if (usu.IDJurisdiccion != null)
                        hdnJuresdiccion.Value = usu.IDJurisdiccion.Trim();

                    if (usu.FechaCierreContable.HasValue)
                        txtFechaCierreContable.Text = Convert.ToDateTime(usu.FechaCierreContable).ToString("dd/MM/yyyy");
                    //Configuracion
                    //chkPrecioUnitarioConIVA.Checked = usu.UsaPrecioFinalConIVA;
                    if (usu.UsaPrecioFinalConIVA)
                        rPrecioUnitarioConIVA.Checked = true;
                    else
                        rPrecioUnitarioSinIVA.Checked = true;

                    if (usu.PedidoDeVenta)
                        rPedidoDeVentaDefectoSi.Checked = true;
                    else
                        rPedidoDeVentaDefectoNo.Checked = true;

                    if (usu.ParaPDVSolicitarCompletarContacto)
                        rParaPDVSolicitarCompletarContactoSi.Checked = true;
                    else
                        rParaPDVSolicitarCompletarContactoNo.Checked = true;

                    if (usu.EsVendedor)
                        chkEsVendedor.Checked = true;
                    else
                        chkEsVendedor.Checked = false;

                    txtPorcentajeComision.Text = usu.PorcentajeComision.ToString();

                    if (usu.FacturaSoloContraEntrega)
                        chkFacturaSoloContraEntrega.Checked = true;
                    else
                        chkFacturaSoloContraEntrega.Checked = false;

                    if (usu.UsaCantidadConDecimales)
                        chkUsaCantidadConDecimales.Checked = true;
                    else
                        chkUsaCantidadConDecimales.Checked = false;

                    if (string.IsNullOrWhiteSpace(usu.Logo))
                    {
                        imgLogo.Src = "/files/usuarios/no-photo.png";
                        hdnTieneFoto.Value = "0";
                    }
                    else
                    {
                        imgLogo.Src = "/files/usuarios/" + usu.Logo;
                        hdnTieneFoto.Value = "1";
                    }

                    if (CurrentUser.UsaPlanCorporativo)//Plan Corporativo
                        cargarSelectConfiguracionPlanDeCuenta();
                    else
                        liPlandeCuentas.Visible = false;

                    if (usu.TemplateFc == "default")
                        default1.Checked = true;
                    else if (usu.TemplateFc == "amarillo")
                        default2.Checked = true;
                    else if (usu.TemplateFc == "celeste")
                        default3.Checked = true;
                    else if (usu.TemplateFc == "negro")
                        default4.Checked = true;
                    else if (usu.TemplateFc == "rojo")
                        default5.Checked = true;
                    else
                        default6.Checked = true;

                    //Si viene de los msj para configurar puntos de venta
                    var pdv = Request.QueryString["pdv"];
                    if (pdv != null)                    
                        if (pdv.Equals("active"))
                        {
                            this.liDatosPrincipales.Attributes.Remove("class");
                            this.info.Attributes.Remove("class");
                            this.info.Attributes.Add("class", "tab-pane");
                            this.datosFiscales.Attributes.Remove("class");
                            this.datosFiscales.Attributes.Add("class", "tab-pane active");
                            this.liDatosFiscales.Attributes.Add("class", "active");
                        }

                    txtCBU.Text = usu.CBU;
                    txtTextoFinalFactura.Text = usu.TextoFinalFactura;
                    
                }
                else
                    throw new Exception("El usuario no existe");
            }
        }
    }

    private void cargarSelectConfiguracionPlanDeCuenta()
    {
        using (var dbContext = new ACHEEntities())
        {
            var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
            var lista = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == CurrentUser.IDUsuario);
            foreach (var item in lista)
            {
                //Comprobantes
                ddlCtaProveedoresComprobante.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlCtaIIBBComprobante.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlPercepcionIVAComprobante.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlIVACreditoFiscalComprobante.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlNoGravadoCompras.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                //Pagos
                ddlValoresADepositar.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlCaja.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlBanco.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlRetIIBB.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlRetIVA.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlRetGanancias.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));

                //Cobranzas
                ddlRetSUSS.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));

                //ventas
                ddlIVADebitoFiscal.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlDeudoresPorVenta.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlNoGravadoVentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                // Filtro en compras y ventas
                ddlCuentasCompras.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                ddlCuentasVentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));

            }

            if (config != null)
            {
                ddlCtaProveedoresComprobante.SelectedValue = config.IDCtaProveedores.ToString();
                ddlCtaIIBBComprobante.SelectedValue = config.IDCtaIIBB.ToString();
                ddlPercepcionIVAComprobante.SelectedValue = config.IDCtaPercepcionIVA.ToString();
                ddlIVACreditoFiscalComprobante.SelectedValue = config.IDCtaIVACreditoFiscal.ToString();
                ddlNoGravadoCompras.SelectedValue = config.IDCtaConceptosNoGravadosxCompras.ToString();

                ddlValoresADepositar.SelectedValue = config.IDCtaValoresADepositar.ToString();
                ddlCaja.SelectedValue = config.IDCtaCaja.ToString();
                ddlBanco.SelectedValue = config.IDCtaBancos.ToString();
                ddlRetIIBB.SelectedValue = config.IDctaRetIIBB.ToString();
                ddlRetIVA.SelectedValue = config.IDctaRetIVA.ToString();
                ddlRetGanancias.SelectedValue = config.IDctaRetGanancias.ToString();
                ddlRetSUSS.SelectedValue = config.IDCtaRetSUSS.ToString();

                hdnCuentasCompras.Value = config.CtasFiltroCompras;
                hdnCuentasVentas.Value = config.CtasFiltroVentas;
                ddlNoGravadoVentas.SelectedValue = config.IDCtaConceptosNoGravadosxVentas.ToString();

                ddlIVADebitoFiscal.SelectedValue = config.IDCtaIVADebitoFiscal.ToString();
                ddlDeudoresPorVenta.SelectedValue = config.IDCtaDeudoresPorVentas.ToString();
            }
        }
    }

    private void OcultarDatosUsuarioEmpresas(ACHEEntities dbContext)
    {
        var usuAdic = new UsuariosAdicionales();
        if (!PermisosModulos.tienePlan("template"))
            liTemplate.Visible = false;
        if (!PermisosModulos.tienePlan("accecoCliente"))
            liportalClientes.Visible = false;
        else if (CurrentUser.TipoUsuario != "A")
        {
            if (CurrentUser.IDUsuarioAdicional > 0)
            {
                usuAdic = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();
                if (usuAdic.Usuarios.IDUsuarioPadre > 0)
                    Response.Redirect("/home.aspx");
            }

            liPerfilUsuario.Visible = false;
            PerfilUsuario.Visible = false;
            liPerfilUsuario.Attributes.Remove("class");
            PerfilUsuario.Attributes.Remove("class");

            liEmpresas.Attributes.Add("class", "active");
            Empresas.Attributes.Add("class", "tab-pane active");
        }

        if (CurrentUser.IDUsuarioPadre > 0 && CurrentUser.IDUsuarioAdicional > 1)
        {
            usuAdic = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();
            if (usuAdic.IDUsuario != CurrentUser.IDUsuarioPadre)
            {
                liEmpresas.Visible = false;
                Empresas.Visible = false;
            }
        }
    }

    private void OcultarDatosUsuarioTemplates()
    {
        if (!PermisosModulos.tienePlan("Empresas"))
        {
            liEmpresas.Visible = false;
            Empresas.Visible = false;
            //liAlertasyAvisos.Visible = false;
            //AlertasyAvisos.Visible = false;
        }
    }

    private static bool SuperaLimiteDeEmpresas(ACHEEntities dbContext)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        if (usu.IDUsuarioPadre == null || usu.IDUsuarioPadre == 0)
        {
            var cantEmpresas = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == usu.IDUsuario).ToList();
            var cantEmpresasHabilitadas = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault().CantidadEmpresas;

            if (cantEmpresas.Count() >= cantEmpresasHabilitadas)
                return true;
        }
        return false;
    }

    [WebMethod(true)]
    public static void guardar(string razonSocial, string condicionIva, string cuit, string iibb, string fechaInicio,
        string personeria, string email, string emailAlertas, string telefono, string celular, string contacto,
        string idProvincia, string idCiudad, string domicilio, string pisoDepto, string cp, bool esAgentePersepcionIVA,
        bool esAgentePersepcionIIBB, bool esAgenteRetencionGanancia, bool esAgenteRetencion, 
        bool exentoIIBB, string fechaCierreContable, string idJurisdiccion, string cbu, string textoFinalFactura)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                UsuarioCommon.GuardarConfiguracion(razonSocial, condicionIva, cuit, iibb, 
                    fechaInicio, personeria, emailAlertas, telefono, celular, contacto, 
                    idProvincia, idCiudad, domicilio, pisoDepto, cp, esAgentePersepcionIVA, 
                    esAgentePersepcionIIBB, esAgenteRetencionGanancia, esAgenteRetencion, 
                    exentoIIBB, fechaCierreContable, idJurisdiccion, cbu, textoFinalFactura,
                    usu);
                ActualizarDatosSesion(esAgentePersepcionIVA, esAgentePersepcionIIBB, esAgenteRetencionGanancia, esAgenteRetencion, (bool)exentoIIBB, iibb, condicionIva, idJurisdiccion);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #region Punto de venta
    [WebMethod(true)]
    public static void agregarPunto(int punto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            PuntoDeVentaCommon.GuardarPuntoDeVenta(punto, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarPunto(int punto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            PuntoDeVentaCommon.EliminarPuntoDeVenta(punto, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static void eliminarUsuario(int idUsuario)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

                SqlCommand comando = new SqlCommand("PA_EliminarUsuario", cn);
                comando.CommandType = CommandType.StoredProcedure;

                //Parametros
                SqlParameter parIdUsuario = new SqlParameter("@idUsuario", SqlDbType.Int);
                parIdUsuario.Direction = ParameterDirection.Input;
                parIdUsuario.Value = Convert.ToInt32(idUsuario);
                comando.Parameters.Add(parIdUsuario);

                comando.Connection.Open();
                comando.ExecuteNonQuery();
                comando.Connection.Close();
                comando.Connection.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar eliminar el usuario: " + ex.Message);
            }               

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerPuntos()
    {
        //common.consultarPuntosDeVentaAfip();

        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == usu.IDUsuario).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var punto in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td>" + punto.Punto.ToString("#0000") + "</td>";
                        html += "<td>" + punto.FechaAlta.ToString("dd/MM/yyyy") + "</td>";
                        if (punto.FechaBaja.HasValue)
                            html += "<td>" + punto.FechaBaja.Value.ToString("dd/MM/yyyy") + "</td>";
                        else
                            html += "<td></td>";
                        html += "<td>" + ((punto.PorDefecto) ? "SI" : "NO") + "</td>";

                        if (!punto.PorDefecto && !punto.FechaBaja.HasValue)
                            html += "<td><a href='#' title='Dar de baja' style='font-size: 16px;' onclick='MisDatos.eliminarPunto(" + punto.IDPuntoVenta + ");'><i class='fa fa-times'></i></a> <a href='#' title='Poner por defecto' style='font-size: 16px;' onclick='MisDatos.ponerPorDefecto(" + punto.IDPuntoVenta + ");'><i class='fa fa-check' style='color: green;'></i></a></td>";
                        else if (punto.FechaBaja.HasValue)
                        {
                            html += "<td><a href='#' title='Habilitar punto de venta' style='font-size: 16px;' onclick='MisDatos.habilitarPuntoVenta(" + punto.IDPuntoVenta + ");'><i class='fa fa-hand-o-up' style='color: green;'></i></a></td>";
                        }
                        else
                            html += "<td><a href='#' title='Dar de baja' style='font-size: 16px;' onclick='MisDatos.eliminarPunto(" + punto.IDPuntoVenta + ");'><i class='fa fa-times'></i></a>";
                        index++;
                        html += "</tr>";
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='5' style='text-align:center'>No tienes puntos de venta registrados</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static void HabilitarPuntoVenta(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                PuntosDeVenta entity = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                    entity.FechaBaja = null;
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string GuardarPorDefecto(int idPunto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var ListaPuntos = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == usu.IDUsuario);

                foreach (var item in ListaPuntos)
                {
                    if (item.IDPuntoVenta == idPunto)
                        item.PorDefecto = true;
                    else
                        item.PorDefecto = false;
                }
                dbContext.SaveChanges();
            }

            return obtenerPuntos();
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion


    #region Actividades
    [WebMethod(true)]
    public static void agregarActividad(string codigo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            ActividadCommon.GuardarActividad(codigo, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarActividad(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            ActividadCommon.EliminarActividad(id, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerActividades()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = dbContext.Actividad.Where(x => x.IdUsuario == usu.IDUsuario).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var punto in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td>" + punto.Codigo + "</td>";
                        html += "<td>" + punto.FechaAlta.ToString("dd/MM/yyyy") + "</td>";
                        if (punto.FechaBaja.HasValue)
                            html += "<td>" + punto.FechaBaja.Value.ToString("dd/MM/yyyy") + "</td>";
                        else
                            html += "<td></td>";
                        html += "<td>" + ((punto.PorDefecto) ? "SI" : "NO") + "</td>";

                        if (!punto.PorDefecto && !punto.FechaBaja.HasValue)
                            html += "<td><a href='#' title='Dar de baja' style='font-size: 16px;' onclick='MisDatos.eliminarActividad(" + punto.IdActividad + ");'><i class='fa fa-times'></i></a> <a href='#' title='Poner por defecto' style='font-size: 16px;' onclick='MisDatos.ponerActividadPorDefecto(" + punto.IdActividad + ");'><i class='fa fa-check' style='color: green;'></i></a></td>";
                        else if (punto.FechaBaja.HasValue)
                        {
                            html += "<td><a href='#' title='Habilitar actividad' style='font-size: 16px;' onclick='MisDatos.habilitarActividad(" + punto.IdActividad + ");'><i class='fa fa-hand-o-up' style='color: green;'></i></a></td>";
                        }
                        else
                            html += "<td><a href='#' title='Dar de baja' style='font-size: 16px;' onclick='MisDatos.eliminarActividad(" + punto.IdActividad + ");'><i class='fa fa-times'></i></a>";
                        index++;
                        html += "</tr>";
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='5' style='text-align:center'>No tienes actividades registradas</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static void HabilitarActividad(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                Actividad entity = dbContext.Actividad.Where(x => x.IdActividad == id && x.IdUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                    entity.FechaBaja = null;
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string GuardarActividadPorDefecto(int idActividad)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var ListaActividades = dbContext.Actividad.Where(x => x.IdUsuario == usu.IDUsuario);

                foreach (var item in ListaActividades)
                {
                    if (item.IdActividad == idActividad)
                        item.PorDefecto = true;
                    else
                        item.PorDefecto = false;
                }
                dbContext.SaveChanges();
            }

            return obtenerActividades();
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion



    [WebMethod(true)]
    public static void portalClientes(bool ChkCorreoPortal, bool chkPortalClientes, string clientId, string clientSecret)
    {

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Usuarios entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                entity.CorreoPortal = ChkCorreoPortal;
                entity.PortalClientes = chkPortalClientes;
                entity.MercadoPagoClientID = clientId.Trim();
                entity.MercadoPagoClientSecret = clientSecret.Trim();
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void ActualizarTemplate(string ddlTemplate)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Usuarios entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                entity.TemplateFc = ddlTemplate;
                dbContext.SaveChanges();
                usu.TemplateFc = ddlTemplate;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static void eliminarFoto()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                Usuarios entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/usuarios/" + entity.Logo);
                entity.Logo = "";
                if (File.Exists(Serverpath))
                {
                    File.Delete(Serverpath);
                    dbContext.SaveChanges();
                }
                dbContext.SaveChanges();
                HttpContext.Current.Session["CurrentUser"] = TokenCommon.ObtenerWebUser(usu.IDUsuario);
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void guardarConfiguracionPlanDeCuenta(int IDCtaProveedores, int IDCtaIIBB, int IDCtaIVACreditoFiscal, int IDCtaPercepcionIVA, int IDCtaBancos,
    int IDCtaCaja, int IDCtaValoresADepositar, int IDctaRetIIBB, int IDctaRetIVA, int IDctaRetGanancias,
    int IDCtaIVADebitoFiscal, int IDCtaDeudoresPorVentas, int IDctaRetSUSS, string ctasFiltrosCompras, string ctasFiltrosVentas, int IDCtaConceptosNoGravadosxCompras, int IDCtaConceptosNoGravadosxVentas)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                ConfiguracionPlanDeCuenta entity = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                if (entity == null)
                {
                    entity = new ConfiguracionPlanDeCuenta();
                    entity.IDUsuario = usu.IDUsuario;
                }
                //COMPROBANTES
                entity.IDCtaProveedores = IDCtaProveedores;
                entity.IDCtaIIBB = IDCtaIIBB;
                entity.IDCtaIVACreditoFiscal = IDCtaIVACreditoFiscal;
                entity.IDCtaPercepcionIVA = IDCtaPercepcionIVA;
                entity.IDCtaConceptosNoGravadosxCompras = IDCtaConceptosNoGravadosxCompras;
                entity.IDCtaConceptosNoGravadosxVentas = IDCtaConceptosNoGravadosxVentas;
                //PAGOS
                entity.IDCtaBancos = IDCtaBancos;
                entity.IDCtaCaja = IDCtaCaja;
                entity.IDCtaValoresADepositar = IDCtaValoresADepositar;
                entity.IDctaRetIIBB = IDctaRetIIBB;
                entity.IDctaRetIVA = IDctaRetIVA;
                entity.IDctaRetGanancias = IDctaRetGanancias;
                entity.IDCtaIVADebitoFiscal = IDCtaIVADebitoFiscal;
                entity.IDCtaDeudoresPorVentas = IDCtaDeudoresPorVentas;
                entity.IDCtaRetSUSS = IDctaRetSUSS;
                entity.CtasFiltroCompras = ctasFiltrosCompras;
                entity.CtasFiltroVentas = ctasFiltrosVentas;

                if (entity.IDConfiguracionPlanDeCuenta == 0)
                    dbContext.ConfiguracionPlanDeCuenta.Add(entity);

                if (usu.UsaPlanCorporativo)//Plan Corporativo
                    dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void guardarConfiguracion(bool usaPrecioUnitarioConIva, bool usaPedidoDeVenta,
                                            bool paraPDVSolicitarCompletarContacto, bool esVendedor,
                                            string porcentajeComision, bool facturaSoloContraEntrega, bool usaCantidadConDecimales)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            if (!usaPrecioUnitarioConIva)
            {
                if (usu.CondicionIVA != "RI")
                {
                    throw new Exception("Solo los responsables Inscriptos pueden ingresar el precio final sin IVA");
                }
                                  
            }           

            using (var dbContext = new ACHEEntities())
            {
                Usuarios entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                entity.UsaPrecioFinalConIVA = usaPrecioUnitarioConIva;
                entity.PedidoDeVenta = usaPedidoDeVenta;
                entity.ParaPDVSolicitarCompletarContacto = paraPDVSolicitarCompletarContacto;
                entity.EsVendedor = esVendedor;
                entity.FacturaSoloContraEntrega = facturaSoloContraEntrega;
                entity.UsaCantidadConDecimales = usaCantidadConDecimales;
                if (entity.EsVendedor)
                    entity.PorcentajeComision = Convert.ToDecimal(porcentajeComision);
                dbContext.SaveChanges();

                ActualizarDatosSesion(entity.UsaPrecioFinalConIVA, entity.PedidoDeVenta, entity.FechaAlta, 
                    entity.EnvioAutomaticoComprobante, entity.EnvioAutomaticoRecibo, entity.ParaPDVSolicitarCompletarContacto, entity.EsVendedor, entity.PorcentajeComision,
                    entity.FacturaSoloContraEntrega, entity.UsaCantidadConDecimales);
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }   

    private static void InsertarPlanDeCuentas(ACHEEntities dbContext)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        var planesCuentas = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
        if (planesCuentas == null)
            dbContext.InsertarPadresPlanesDeCuenta(usu.IDUsuario);
    }

    #region ActualizarDatosSesion
    private static void ActualizarDatosSesion(string TemplateFc, bool AgentePercepcionIVA, bool AgentePercepcionIIBB, bool AgenteRetencionGanancia, bool AgenteRetencion,
        bool exentoIIBB, string nroIIBB, string logo, string condicionIVA, string IDJurisdiccion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(usu.IDUsuario, usu.CondicionIVA);
            HttpContext.Current.Session["CurrentUser"] = new WebUser(
               usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, condicionIVA,
               usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia, usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, nroIIBB, usu.FechaInicio,
               logo, TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa,
               usu.ModoQA, usu.IDPlan, usu.EmailAlerta, usu.Provincia, usu.Ciudad, AgentePercepcionIVA, AgentePercepcionIIBB,
               AgenteRetencionGanancia, AgenteRetencion, true, usu.UsaFechaFinPlan, usu.ApiKey, exentoIIBB, usu.UsaPrecioFinalConIVA,
               usu.FechaAlta, usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, IDJurisdiccion, usu.UsaPlanCorporativo, 
               usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, usu.PorcentajeCompra, 
               usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
               usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    private static void ActualizarDatosSesion(bool AgentePercepcionIVA, bool AgentePercepcionIIBB, bool AgenteRetencionGanancia, bool AgenteRetencion,
       bool exentoIIBB, string nroIIBB, string condicionIVA, string IDJurisdiccion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(usu.IDUsuario, usu.CondicionIVA);
            HttpContext.Current.Session["CurrentUser"] = new WebUser(
               usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, condicionIVA,
               usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia, usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, nroIIBB, usu.FechaInicio,
               usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa,
               usu.ModoQA, usu.IDPlan, usu.EmailAlerta, usu.Provincia, usu.Ciudad, AgentePercepcionIVA, AgentePercepcionIIBB,
               AgenteRetencionGanancia, AgenteRetencion, true, usu.UsaFechaFinPlan, usu.ApiKey, exentoIIBB, usu.UsaPrecioFinalConIVA,
               usu.FechaAlta, usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, IDJurisdiccion, usu.UsaPlanCorporativo, 
               usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, usu.PorcentajeCompra, usu.PorcentajeRentabilidad,
               usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
               usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    private static void ActualizarDatosSesion(bool EnvioFE, bool EnvioCR)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(usu.IDUsuario, usu.CondicionIVA);
            HttpContext.Current.Session["CurrentUser"] = new WebUser(
         usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, usu.CondicionIVA,
         usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia, usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, usu.IIBB, usu.FechaInicio,
         usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa,
         usu.ModoQA, usu.IDPlan, usu.EmailAlerta, usu.Provincia, usu.Ciudad, usu.AgentePercepcionIVA, usu.AgentePercepcionIIBB,
         usu.AgenteRetencionGanancia, usu.AgenteRetencion, true, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA,
         usu.FechaAlta, EnvioFE, EnvioCR, usu.IDJurisdiccion, usu.UsaPlanCorporativo, usu.PedidoDeVenta,
         usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, usu.PorcentajeCompra, usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto,
         usu.EsVendedor, usu.PorcentajeComision, usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    private static void ActualizarDatosSesion(bool UsaPrecioFinalConIVA, bool PedidoDeVenta, DateTime FechaAlta, bool EnvioAutomaticoComprobante, 
        bool EnvioAutomaticoRecibo, bool ParaPDVSolicitarCompletarContacto, bool EsVendedor, decimal PorcentajeComision,
        bool FacturaSoloContraEntrega, bool UsaCantidadConDecimales)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(usu.IDUsuario, usu.CondicionIVA);
            HttpContext.Current.Session["CurrentUser"] = new WebUser(
            usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, usu.RazonSocial, usu.CUIT, usu.CondicionIVA,
            usu.Email, "", usu.Domicilio, usu.Pais, usu.IDProvincia, usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFE, usu.IIBB, usu.FechaInicio,
            usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupFinalizado, usu.TieneMultiEmpresa,
            usu.ModoQA, usu.IDPlan, usu.EmailAlerta, usu.Provincia, usu.Ciudad, usu.AgentePercepcionIVA, usu.AgentePercepcionIIBB,
            usu.AgenteRetencionGanancia, usu.AgenteRetencion, true, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, UsaPrecioFinalConIVA,
            FechaAlta, EnvioAutomaticoComprobante, EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo, 
            PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, usu.PorcentajeCompra, usu.PorcentajeRentabilidad, ParaPDVSolicitarCompletarContacto, 
            EsVendedor, PorcentajeComision, FacturaSoloContraEntrega, UsaCantidadConDecimales);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion

    [WebMethod(true)]
    public static void GuardarAlertasyAvisos(List<AvisosVencimientoViewModel> listaAvisos)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var plandeCuenta = PermisosModulosCommon.ObtenerPlanActual(dbContext, usu.IDUsuario);
                if (plandeCuenta.IDPlan >= 3)//Plan Pyme
                {
                    var avisos = dbContext.AvisosVencimiento.Where(x => x.IDUsuario == usu.IDUsuario).ToList();
                    foreach (var item in avisos)
                        dbContext.AvisosVencimiento.Remove(item);

                    foreach (var item in listaAvisos)
                    {
                        if ((item.TipoAlerta == "Primer aviso" || item.TipoAlerta == "Segundo aviso" || item.TipoAlerta == "Tercer aviso") && plandeCuenta.IDPlan >= 4)
                        {
                            var entity = new AvisosVencimiento();
                            entity.IDUsuario = usu.IDUsuario;
                            entity.Activa = item.Activa;
                            entity.Asunto = item.Asunto;
                            entity.CantDias = item.CantDias;
                            entity.Mensaje = item.Mensaje;
                            entity.ModoDeEnvio = item.ModoDeEnvio;
                            entity.TipoAlerta = item.TipoAlerta;
                            dbContext.AvisosVencimiento.Add(entity);
                        }
                        else if ((item.TipoAlerta == "Envio FE" || item.TipoAlerta == "Envio CR" || item.TipoAlerta == "Stock") && plandeCuenta.IDPlan >= 3)
                        {
                            var entity = new AvisosVencimiento();
                            entity.IDUsuario = usu.IDUsuario;
                            entity.Activa = item.Activa;
                            entity.Asunto = item.Asunto;
                            entity.CantDias = item.CantDias;
                            entity.Mensaje = item.Mensaje;
                            entity.ModoDeEnvio = item.ModoDeEnvio;
                            entity.TipoAlerta = item.TipoAlerta;
                            dbContext.AvisosVencimiento.Add(entity);
                        }
                    }
                    dbContext.SaveChanges();
                    ActualizarDatosSecionAvisosYAlertas(dbContext, listaAvisos);
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static void ActualizarDatosSecionAvisosYAlertas(ACHEEntities dbContext, List<AvisosVencimientoViewModel> listaAvisos)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        var EnvioFE = false;
        var EnvioCR = false;
        foreach (var item in listaAvisos)
        {
            if (listaAvisos.Any(x => x.TipoAlerta == "Envio FE" && x.Activa))
                EnvioFE = true;
            if (listaAvisos.Any(x => x.TipoAlerta == "Envio CR" && x.Activa))
                EnvioCR = true;
        }

        var entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
        entity.EnvioAutomaticoComprobante = EnvioFE;
        entity.EnvioAutomaticoRecibo = EnvioCR;
        dbContext.SaveChanges();
        ActualizarDatosSesion(EnvioFE, EnvioCR);
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static List<AvisosVencimientoViewModel> ObtenerAvisosVencimientos()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.AvisosVencimiento.Where(x => x.IDUsuario == usu.IDUsuario).ToList().Select(x => new AvisosVencimientoViewModel()
                {
                    //IDAvisosVencimientos = x.IDAvisosVencimientos,
                    // IDUsuario = x.IDUsuario,
                    Activa = x.Activa,
                    TipoAlerta = x.TipoAlerta,
                    ModoDeEnvio = x.ModoDeEnvio,
                    CantDias = x.CantDias,
                    Asunto = x.Asunto,
                    Mensaje = x.Mensaje,
                }).ToList();

                return lista;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public class AvisosVencimientoViewModel
    {
        public int IDAvisosVencimientos { get; set; }
        public int IDUsuario { get; set; }
        public bool Activa { get; set; }
        public string TipoAlerta { get; set; }
        public string ModoDeEnvio { get; set; }
        public int CantDias { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
    }

    //PLANES Y PAGOS
    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static ResultadosPlanDePagosViewModel ObtenerHistorialPagos()
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.PlanesPagos.Where(x => x.IDUsuario == usu.IDUsuario).ToList();
                    var planActual = results.Where(x => x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();
                    ResultadosPlanDePagosViewModel resultado = new ResultadosPlanDePagosViewModel();

                    if (planActual != null)
                    {
                        resultado.IDPlanActual = planActual.IDPlan;
                        resultado.NombrePlanActual = planActual.Planes.Nombre;
                        resultado.FechaVencimiento = Convert.ToDateTime(planActual.FechaFinPlan).ToString("dd/MM/yyyy");
                    }
                    var list = results.OrderByDescending(x => x.IDPlanesPagos).ToList()
                        .Select(x => new PlanDePagosViewModel()
                        {
                            TipoDePlan = x.Planes.Nombre,
                            FechaDePago = x.FechaDePago.ToString("dd/MM/yyyy"),
                            ImportePagado = x.ImportePagado.ToString("N2"),
                            FomaDePago = x.FormaDePago,
                            NroReferencia = x.NroReferencia,
                            Estado = x.Estado,
                            FechaInicio = Convert.ToDateTime(x.FechaInicioPlan).ToString("dd/MM/yyyy"),
                            FechaVencimiento = Convert.ToDateTime(x.FechaFinPlan).ToString("dd/MM/yyyy"),
                        });
                    resultado.Items = list.ToList();
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

    // EMPRESA
    [WebMethod(true)]
    public static void CrearEmpresa(string razonSocial, string condicionIva, string cuit, string personeria, string email, string pwd, int idProvincia, int idCiudad, string domicilio, string pisoDepto)
    {

        if (!Common.validarPassword(pwd))
            throw new Exception("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                if (!cuit.IsValidCUIT())
                    throw new Exception("El CUIT ingresado es incorrecto");
                else
                {
                    if (dbContext.Usuarios.Any(x => x.Email == email))
                        throw new Exception("El E-mail ingresado ya se encuentra registrado.");
                    else if (dbContext.Usuarios.Any(x => x.CUIT == cuit))
                        throw new Exception("El CUIT ingresado ya se encuentra registrado.");
                    if (dbContext.UsuariosAdicionales.Any(x => x.Email == email))
                        throw new Exception("El E-mail ingresado ya se encuentra registrado.");
                    else if (SuperaLimiteDeEmpresas(dbContext))
                        throw new Exception("Superó el máximo de empresas permitidas (5). Si desea obtener más por favor envíe por correo a axan.sistemas@gmail.com.");
                    else
                    {
                        var currentUsuario = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        Usuarios entity = new Usuarios();
                        entity.FechaAlta = DateTime.Now;
                        entity.IDUsuarioPadre = usu.IDUsuario;

                        entity.Theme = "default";
                        entity.Pais = "Argentina";
                        entity.TemplateFc = "default";
                        entity.Pwd = "";
                        entity.TieneFacturaElectronica = false;
                        entity.Logo = null;
                        entity.Activo = true;
                        entity.IDPlan = 1;
                        entity.FechaUltLogin = DateTime.Now;
                        entity.SetupRealizado = true;
                        entity.CorreoPortal = currentUsuario.CorreoPortal;
                        entity.PortalClientes = currentUsuario.PortalClientes;
                        entity.FechaFinPlan = DateTime.Now.AddDays(30);
                        entity.UsaProd = currentUsuario.UsaProd;
                        entity.UsaFechaFinPlan = false;

                        entity.RazonSocial = razonSocial;

                        entity.CondicionIva = condicionIva;
                        entity.CUIT = cuit;
                        entity.IIBB = string.Empty;
                        entity.Personeria = personeria;
                        entity.Email = email;
                        entity.EmailAlertas = email;
                        entity.Telefono = string.Empty;
                        entity.Celular = string.Empty;
                        entity.Contacto = string.Empty;
                        //Domicilio
                        entity.IDProvincia = idProvincia;
                        entity.IDCiudad = idCiudad;
                        entity.Domicilio = domicilio;
                        entity.PisoDepto = pisoDepto;
                        entity.CodigoPostal = string.Empty;

                        //if (entity.CondicionIva == "MO")
                        //    entity.UsaPrecioFinalConIVA = true;
                        //else
                        //    entity.UsaPrecioFinalConIVA = false;

                        entity.EsAgentePercepcionIVA = false;
                        entity.EsAgentePercepcionIIBB = false;
                        entity.IDJurisdiccion = entity.IDProvincia.ToString();
                        entity.EsAgenteRetencion = false;
                        entity.CantidadEmpresas = 0;
                        entity.ExentoIIBB = false;
                        entity.UsaPrecioFinalConIVA = true;
                        entity.EnvioAutomaticoComprobante = false;
                        entity.EnvioAutomaticoRecibo = false;
                        entity.EsContador = false;
                        entity.UsaPlanCorporativo = false;

                        PuntosDeVenta punto = new PuntosDeVenta();
                        punto.FechaAlta = DateTime.Now;
                        punto.Punto = 1;
                        punto.PorDefecto = true;
                        entity.PuntosDeVenta.Add(punto);
                        entity.ApiKey = Guid.NewGuid().ToString().Replace("-", "");

                        if (cuit.StartsWith("30"))
                        {
                            punto = new PuntosDeVenta();
                            punto.FechaAlta = DateTime.Now;
                            punto.Punto = 2;
                            punto.PorDefecto = false;
                            entity.PuntosDeVenta.Add(punto);
                        }

                        Categorias cat = new Categorias();
                        cat.Nombre = "General";
                        entity.Categorias.Add(cat);

                        Categorias cat2 = new Categorias();
                        cat2.Nombre = "Honorarios";
                        entity.Categorias.Add(cat2);

                        Categorias cat3 = new Categorias();
                        cat3.Nombre = "Gastos varios";
                        entity.Categorias.Add(cat3);

                        Categorias cat4 = new Categorias();
                        cat4.Nombre = "Nafta";
                        entity.Categorias.Add(cat4);

                        Categorias cat5 = new Categorias();
                        cat5.Nombre = "Equipamiento";
                        entity.Categorias.Add(cat5);

                        Bancos banco = new Bancos();
                        banco.Moneda = "Pesos Argentinos";
                        banco.SaldoInicial = 0;
                        banco.NroCuenta = "";
                        banco.IDBancoBase = dbContext.BancosBase.Where(x => x.Nombre == "Default").FirstOrDefault().IDBancoBase;
                        banco.FechaAlta = DateTime.Now;
                        banco.Activo = true;
                        entity.Bancos.Add(banco);

                        UsuariosAdicionales usuariosAdicionales = new UsuariosAdicionales();
                        usuariosAdicionales.Email = email;
                        usuariosAdicionales.Activo = true;
                        usuariosAdicionales.FechaAlta = DateTime.Now.Date;
                        usuariosAdicionales.Pwd = Common.MD5Hash(pwd);
                        usuariosAdicionales.Tipo = "A";
                        entity.UsuariosAdicionales.Add(usuariosAdicionales);

                        dbContext.Usuarios.Add(entity);
                        dbContext.SaveChanges();

                        if (usu.UsaPlanCorporativo)//Plan Corporativo
                        {
                            dbContext.ConfigurarPlanCorporativo(entity.IDUsuario);
                            var listaBancos = dbContext.Bancos.Where(x => x.IDUsuario == entity.IDUsuario).ToList();
                            var usuEmpresa = TokenCommon.ObtenerWebUser(entity.IDUsuario);
                            foreach (var item in listaBancos)
                                ContabilidadCommon.CrearCuentaBancos(item.IDBanco, usuEmpresa);
                        }
                    }
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static ResultadosEmpresasViewModel getResults()
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                //int idusuarioLogiado = 0;
                using (var dbContext = new ACHEEntities())
                {
                    List<EmpresasViewModel> list = UsuarioCommon.ListaEmpresasDisponibles(usu, dbContext);

                    ResultadosEmpresasViewModel resultado = new ResultadosEmpresasViewModel();
                    resultado.SuperoLimite = SuperaLimiteDeEmpresas(dbContext);
                    resultado.TotalPage = 1;
                    resultado.UsuLogiado = usu.IDUsuario.ToString();
                    resultado.TotalItems = list.Count();
                    resultado.Items = list.OrderBy(x => x.RazonSocial).ToList();

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
}