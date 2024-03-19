using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Services;
using ACHE.Model;
using ACHE.Negocio.Common;
using ACHE.Negocio.Contabilidad;
using System.Collections.Specialized;
using ACHE.FacturaElectronica;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using ACHE.FacturaElectronica.WSFacturaElectronica;
using ACHE.Negocio.Facturacion;
using ACHE.Extensions;

public partial class finRegistro : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        txtTelefono.Text = CurrentUser.Telefono;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            this.hdnCUIT.Value = usu.CUIT.ToString();
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");       
    }

    [WebMethod(true)]
    public static void guardar(string razonSocial, string condicionIva, string personeria, string idProvincia, string idCiudad, string domicilio, string pisoDepto, string cp, bool esContador, bool usaPlanCorporativo, string fechaInicioActividades)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                int usuariosPermitidos = Convert.ToInt32(ConfigurationManager.AppSettings["UsuariosPermitidos"]);
                if (usuariosPermitidos == 1)
                {
                    var usrPer = dbContext.UsuariosPermitidos.Where(x => x.CUIT == usu.CUIT).FirstOrDefault();
                    if (usrPer != null)
                        entity.Activo = true;
                    else
                        entity.Activo = false;
                }

                entity.RazonSocial = razonSocial;
                entity.CondicionIva = condicionIva;
                entity.Personeria = personeria;
                entity.IDProvincia = Convert.ToInt32(idProvincia);
                entity.IDJurisdiccion = idProvincia;
                entity.IDCiudad = Convert.ToInt32(idCiudad);
                entity.Domicilio = domicilio;
                entity.PisoDepto = pisoDepto;
                entity.CodigoPostal = cp;
                entity.SetupRealizado = true;
                entity.EsContador = esContador;
                entity.PedidoDeVenta = false;
                entity.UsaPlanCorporativo = (entity.CondicionIva == "RI") ? usaPlanCorporativo : false;                
                if (!fechaInicioActividades.Equals(""))
                    entity.FechaInicioActividades = Convert.ToDateTime(fechaInicioActividades);

                dbContext.SaveChanges();

                //if (entity.CondicionIva == "RI")
                //    agregarPunto(dbContext, 2, false);

                ActualizarDatosSesion(entity);
                if (entity.UsaPlanCorporativo && entity.CondicionIva == "RI")
                {
                    dbContext.ConfigurarPlanCorporativo(entity.IDUsuario);
                    var listaBancos = dbContext.Bancos.Where(x => x.IDUsuario == entity.IDUsuario).ToList();
                    foreach (var item in listaBancos)
                        ContabilidadCommon.CrearCuentaBancos(item.IDBanco, usu);
                }

                ListDictionary replacements = new ListDictionary();
                bool send = EmailHelper.SendMessage(EmailTemplate.Bienvenido, replacements, entity.Email, "axanweb: Bienvenido");

                GenerarPerfilUsuario(entity);

                //CrearDatosPrincipales();
                //common.consultarPuntosDeVentaAfip();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static void GenerarPerfilUsuario(Usuarios entity)
    {
        using (var dbContext = new ACHEEntities())
        {
            AccesoFormularioUsuario ac = new AccesoFormularioUsuario
            {
                IdUsuario = entity.IDUsuario,
                IdUsuarioAdicional = 0,
                HomeValores = true,
                ComercialPedidoDeVenta = true,
                ComercialFacturaDeVenta = true,
                ComercialCobranzas = true,
                ComercialPresupuestos = true,
                ComercialAbonos = true,
                ComercialProductosYServicios = true,
                ComercialClientes = true,
                SuministroPedidoDeCompra = true,
                SuministroComprobanteDeCompra = true,
                SuministroPagos = true,
                SuministroProveedores = true,
                AdministracionBancos = true,
                AdministracionInstituciones = true,
                AdministracionGastos = true,
                AdministracionMovimientos = true,
                AdministracionDetalleBancario = true,
                AdministracionCheques = true,
                AdministracionCaja = true,
                AdministracionCuentasCorrientes = true,
                ProduccionMateriales = true,
                ProduccionAlmacenes = true,
                ProduccionCostos = true,
                ProduccionRecursos = true,
                ProduccionPlanificacion = true,
                PlaneamientoObjetivos = true,
                PlaneamientoProgramas = true,
                PlaneamientoPresupuestos = true,
                InfoFinancierosVentasVsCompras = true,
                InfoFinancierosComprasPorCategoria = true,
                InfoGananciasVsPerdidasCobradoVsPagado = true,
                InfoImpositivosIVAVentas = true,
                InfoImpositivosIVACompras = true,
                InfoImpositivosIVASaldo = true,
                InfoImpositivosRetenciones = true,
                InfoImpositivosPercepciones = true,
                InfoImpositivosCITIComprasYVentas = true,
                InfoGestionCuentaCorriente = true,
                InfoGestionCobranzaPendientes = true,
                InfoGestionPagoAProveedores = true,
                InfoGestionStockProductos = true,
                InfoGestionCuentasAPagar = true,
                InfoGestionRankingPorCliente = true,
                InfoGestionRankingPorProductoServicio = true,
                InfoGestionTrackingHoras = true,
                HerramientasExploradorDeArchivos = true,
                HerramientasImportacionMasiva = true,
                HerramientasTrackingDeHoras = true,
                HerramientasConfigurarAlertas = true,
                HerramientasGeneracionCompraAutomatica = true,
                HerramientasGeneracionLiquidoProducto = true,
                HerramientasGeneracionFacturaAutomatica = true
            };
            dbContext.AccesoFormularioUsuario.Add(ac);
            dbContext.SaveChanges();
        }

    }


    private static void ActualizarDatosSesion(Usuarios entity)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        HttpContext.Current.Session["CurrentUser"] = TokenCommon.ObtenerWebUser(entity.IDUsuario);
        ////bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(usu.IDUsuario, usu.CondicionIVA);
        //HttpContext.Current.Session["CurrentUser"] = new WebUser(
        //           usu.IDUsuario, usu.IDUsuarioAdicional, usu.TipoUsuario, entity.RazonSocial, usu.CUIT, entity.CondicionIva,
        //           usu.Email, "", entity.Domicilio + " " + entity.PisoDepto, usu.Pais, entity.IDProvincia, entity.IDCiudad, usu.Telefono, usu.TieneFE, usu.IIBB, usu.FechaInicio,
        //           usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, entity.SetupRealizado, usu.TieneMultiEmpresa,
        //           usu.ModoQA, usu.IDPlan, usu.EmailAlerta, usu.Provincia, usu.Ciudad, usu.AgentePercepcionIVA, usu.AgentePercepcionIIBB,
        //           usu.AgenteRetencion, true, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA, usu.FechaAlta,
        //           usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo);
    }

    private static void CrearDatosPrincipales()
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                List<AbonosPersonasViewModel> personas = new List<AbonosPersonasViewModel>();
                var idClientes = DatosInicialesCommon.CrearDatosClientes(usu);
                var idProveedor = DatosInicialesCommon.CrearDatosProveedores(usu);
                var comprobante = DatosInicialesCommon.CrearDatosVentas(usu, idClientes);
                var compra = DatosInicialesCommon.CrearDatosCompras(usu, idProveedor);

                personas.Add(new AbonosPersonasViewModel() { IDPersona = idClientes, Cantidad = "1" });
                personas.Add(new AbonosPersonasViewModel() { IDPersona = idProveedor, Cantidad = "1" });

                DatosInicialesCommon.CrearDatosConceptos(usu);
                var idPago = DatosInicialesCommon.CrearDatosPagos(usu, compra);
                var idCobranza = DatosInicialesCommon.CrearDatosCobranzas(usu, comprobante);
                DatosInicialesCommon.CrearDatosPresupuestos(usu, idClientes);
                DatosInicialesCommon.CrearDatosAbonos(usu, personas);
                if (usu.UsaPlanCorporativo && usu.CondicionIVA == "RI")
                {
                    ContabilidadCommon.AgregarAsientoDeCompra(compra.IDCompra, usu);
                    ContabilidadCommon.AgregarAsientoDeVentas(usu, comprobante.IDComprobante);
                    ContabilidadCommon.AgregarAsientoDePago(usu, idPago);
                    ContabilidadCommon.AgregarAsientoDeCobranza(usu, idCobranza);
                }
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }    

}