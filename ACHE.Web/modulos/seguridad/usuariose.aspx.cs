using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class usuariose : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (CurrentUser.TipoUsuario != "A")
                Response.Redirect("/home.aspx");

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";
                    txtPwd.CssClass = "form-control";
                    litPwd.Text = "<label class='control-label'> Contraseña</label>";
                }
            }
            else
            {
                litPath.Text = "Alta";
                txtPwd.CssClass = "form-control required";
                litPwd.Text = "<label class='control-label'><span class='asterisk'>*</span> Contraseña</label>";
                chkActivo.Checked = true;                
            }
            if (PermisosModulos.tienePlan("Empresas"))
                accesoEmpresas(int.Parse(hdnID.Value), litPath.Text == "Alta");

            armarChkModulos(int.Parse(hdnID.Value), litPath.Text == "Alta");
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.UsuariosAdicionales.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDUsuarioAdicional == id).FirstOrDefault();
            if (entity != null)
            {
                txtEmail.Text= entity.Email.ToUpper();
                ddlTipo.Text = entity.Tipo;
                chkActivo.Checked = entity.Activo;
                chkEsVendedor.Checked = entity.EsVendedor;
                txtPorcentajeComision.Text = entity.PorcentajeComision.ToString();

                var accesos = dbContext.AccesoFormularioUsuario.Where(x => x.IdUsuario == CurrentUser.IDUsuario && x.IdUsuarioAdicional == id).FirstOrDefault();

                if (accesos != null)
                {
                    chkHomeValores.Checked = accesos.HomeValores;
                    chkComercialPedidoDeVenta.Checked = accesos.ComercialPedidoDeVenta;
                    chkComercialFacturaDeVenta.Checked = accesos.ComercialFacturaDeVenta;
                    chkComercialCobranzas.Checked = accesos.ComercialCobranzas;
                    chkComercialPresupuestos.Checked = accesos.ComercialPresupuestos;
                    chkComercialEntregas.Checked = accesos.ComercialEntregas;
                    chkComercialAbonos.Checked = accesos.ComercialAbonos;
                    chkComercialProductosYServicios.Checked = accesos.ComercialProductosYServicios;
                    chkComercialClientes.Checked = accesos.ComercialClientes;
                    chkSuministroPedidoDeCompra.Checked = accesos.SuministroPedidoDeCompra;
                    chkSuministroComprobanteDeCompra.Checked = accesos.SuministroComprobanteDeCompra;
                    chkSuministroPagos.Checked = accesos.SuministroPagos;
                    chkSuministroProveedores.Checked = accesos.SuministroProveedores;
                    chkAdministracionBancos.Checked = accesos.AdministracionBancos;
                    chkAdministracionInstituciones.Checked = accesos.AdministracionInstituciones;
                    chkAdministracionGastos.Checked = accesos.AdministracionGastos;
                    chkAdministracionMovimientos.Checked = accesos.AdministracionMovimientos;
                    chkAdministracionDetalleBancario.Checked = accesos.AdministracionDetalleBancario;
                    chkAdministracionCheques.Checked = accesos.AdministracionCheques;
                    chkAdministracionCaja.Checked = accesos.AdministracionCaja;
                    chkAdministracionCuentasCorrientes.Checked = accesos.AdministracionCuentasCorrientes;
                    chkProduccionMateriales.Checked = accesos.ProduccionMateriales;
                    chkProduccionAlmacenes.Checked = accesos.ProduccionAlmacenes;
                    chkProduccionCostos.Checked = accesos.ProduccionCostos;
                    chkProduccionRecursos.Checked = accesos.ProduccionRecursos;
                    chkProduccionPlanificacion.Checked = accesos.ProduccionPlanificacion;
                    chkPlaneamientoObjetivos.Checked = accesos.PlaneamientoObjetivos;
                    chkPlaneamientoProgramas.Checked = accesos.PlaneamientoProgramas;
                    chkPlaneamientoPresupuestos.Checked = accesos.PlaneamientoPresupuestos;
                    chkInfoFinancierosVentasVsCompras.Checked = accesos.InfoFinancierosVentasVsCompras;
                    chkInfoFinancierosComprasPorCategoria.Checked = accesos.InfoFinancierosComprasPorCategoria;
                    chkInfoGananciasVsPerdidasCobradoVsPagado.Checked = accesos.InfoGananciasVsPerdidasCobradoVsPagado;
                    chkInfoImpositivosIVAVentas.Checked = accesos.InfoImpositivosIVAVentas;
                    chkInfoImpositivosIVACompras.Checked = accesos.InfoImpositivosIVACompras;
                    chkInfoImpositivosIVASaldo.Checked = accesos.InfoImpositivosIVASaldo;
                    chkInfoImpositivosRetenciones.Checked = accesos.InfoImpositivosRetenciones;
                    chkInfoImpositivosPercepciones.Checked = accesos.InfoImpositivosPercepciones;
                    chkInfoImpositivosCITIComprasYVentas.Checked = accesos.InfoImpositivosCITIComprasYVentas;
                    chkInfoGestionCuentaCorriente.Checked = accesos.InfoGestionCuentaCorriente;
                    chkInfoGestionCobranzaPendientes.Checked = accesos.InfoGestionCobranzaPendientes;
                    chkInfoGestionPagoAProveedores.Checked = accesos.InfoGestionPagoAProveedores;
                    chkInfoGestionStockProductos.Checked = accesos.InfoGestionStockProductos;
                    chkInfoGestionStockProductosDetalle.Checked = accesos.InfoGestionStockDetalle;
                    chkInfoGestionCuentasAPagar.Checked = accesos.InfoGestionCuentasAPagar;
                    chkInfoGestionRankingPorCliente.Checked = accesos.InfoGestionRankingPorCliente;
                    chkInfoGestionRankingPorProductoServicio.Checked = accesos.InfoGestionRankingPorProductoServicio;
                    chkInfoGestionTrackingHoras.Checked = accesos.InfoGestionTrackingHoras;
                    chkInfoGestionListaFacturas.Checked = accesos.InfoGestionListaFacturas;
                    chkInfoGestionComisiones.Checked = accesos.InfoGestionComisiones;
                    chkHerramientasExploradorDeArchivos.Checked = accesos.HerramientasExploradorDeArchivos;
                    chkHerramientasImportacionMasiva.Checked = accesos.HerramientasImportacionMasiva;
                    chkHerramientasTrackingDeHoras.Checked = accesos.HerramientasTrackingDeHoras;
                    chkHerramientasConfigurarAlertas.Checked = accesos.HerramientasConfigurarAlertas;
                    chkHerramientasGeneracionCompraAutomatica.Checked = accesos.HerramientasGeneracionCompraAutomatica;
                    chkHerramientasGeneracionLiquidoProducto.Checked = accesos.HerramientasGeneracionLiquidoProducto;
                    chkHerramientasGeneracionFacturaAutomatica.Checked = accesos.HerramientasGeneracionFacturaAutomatica;
                    chkHerramientasAuditoria.Checked = accesos.HerramientasAuditoria;
                    chkHabilitarCambioIvaEnArticulosDesdeComprobante.Checked = accesos.HabilitarCambioIvaEnArticulosDesdeComprobante;
                }
                
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, string email, string tipo, string pwd, bool activo, string idEmpresas,
                                bool HomeValores, bool ComercialPedidoDeVenta,
                                bool ComercialFacturaDeVenta, bool ComercialCobranzas,
                                bool ComercialPresupuestos, bool ComercialEntregas,
                                bool ComercialAbonos,
                                bool ComercialProductosYServicios, bool ComercialClientes,
                                bool SuministroPedidoDeCompra, bool SuministroComprobanteDeCompra,
                                bool SuministroPagos, bool SuministroProveedores,
                                bool AdministracionBancos, bool AdministracionInstituciones,
                                bool AdministracionGastos, bool AdministracionMovimientos,
                                bool AdministracionDetalleBancario, bool AdministracionCheques,
                                bool AdministracionCaja, bool AdministracionCuentasCorrientes,
                                bool ProduccionMateriales, bool ProduccionAlmacenes,
                                bool ProduccionCostos, bool ProduccionRecursos,
                                bool ProduccionPlanificacion, bool PlaneamientoObjetivos,
                                bool PlaneamientoProgramas, bool PlaneamientoPresupuestos,
                                bool InfoFinancierosVentasVsCompras, bool InfoFinancierosComprasPorCategoria,
                                bool InfoGananciasVsPerdidasCobradoVsPagado, bool InfoImpositivosIVAVentas,
                                bool InfoImpositivosIVACompras, bool InfoImpositivosIVASaldo,
                                bool InfoImpositivosRetenciones, bool InfoImpositivosPercepciones,
                                bool InfoImpositivosCITIComprasYVentas, bool InfoGestionCuentaCorriente,
                                bool InfoGestionCobranzaPendientes, bool InfoGestionPagoAProveedores,
                                bool InfoGestionStockProductos, bool InfoGestionStockProductosDetalle,
                                bool InfoGestionCuentasAPagar,
                                bool InfoGestionRankingPorCliente, bool InfoGestionRankingPorProductoServicio,
                                bool InfoGestionTrackingHoras, bool InfoGestionListaFacturas,
                                bool InfoGestionComisiones,
                                bool HerramientasExploradorDeArchivos,
                                bool HerramientasImportacionMasiva, bool HerramientasTrackingDeHoras,
                                bool HerramientasConfigurarAlertas, bool HerramientasGeneracionCompraAutomatica,
                                bool HerramientasGeneracionLiquidoProducto, bool HerramientasGeneracionFacturaAutomatica,
                                bool HerramientasAuditoria,
                                bool HabilitarCambioIvaEnArticulosDesdeComprobante, bool EsVendedor,
                                string PorcentajeComision)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];            

            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.UsuariosAdicionales.Any(x => x.Email == email && x.IDUsuarioAdicional != id))
                    throw new Exception("El Email ingresado ya se encuentra registrado.");
                if (dbContext.Usuarios.Any(x => x.Email == email && x.IDUsuarioPadre == null))
                    throw new Exception("El E-mail ingresado ya se encuentra registrado.");

                UsuariosAdicionales entity;
                if (id > 0)
                {
                    if(pwd != "")
                        if (!Common.validarPassword(pwd))
                            throw new Exception("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");

                    entity = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                }                    
                else
                {
                    if (!Common.validarPassword(pwd))
                        throw new Exception("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");

                    entity = new UsuariosAdicionales();
                    entity.FechaAlta = DateTime.Now;
                    entity.IDUsuario = usu.IDUsuario;
                }

                entity.Email = email.ToUpper();
                entity.Tipo = tipo;
                if (!string.IsNullOrWhiteSpace(pwd))
                    entity.Pwd = Common.MD5Hash(pwd);

                entity.EsVendedor = EsVendedor;
                if (entity.EsVendedor)
                    entity.PorcentajeComision = Convert.ToDecimal(PorcentajeComision);

                entity.Activo = activo;

                if (id > 0)
                {
                    dbContext.SaveChanges();
                }                    
                else
                {
                    dbContext.UsuariosAdicionales.Add(entity);
                    dbContext.SaveChanges();
                }

                AccesoFormularioUsuario ac = new AccesoFormularioUsuario();
                ac.IdUsuario = usu.IDUsuario;
                ac.IdUsuarioAdicional = entity.IDUsuarioAdicional;
                ac.HomeValores = HomeValores;
                ac.ComercialPedidoDeVenta = ComercialPedidoDeVenta;
                ac.ComercialFacturaDeVenta = ComercialFacturaDeVenta;
                ac.ComercialCobranzas = ComercialCobranzas;
                ac.ComercialPresupuestos = ComercialPresupuestos;
                ac.ComercialEntregas = ComercialEntregas;
                ac.ComercialAbonos = ComercialAbonos;
                ac.ComercialProductosYServicios = ComercialProductosYServicios;
                ac.ComercialClientes = ComercialClientes;
                ac.SuministroPedidoDeCompra = SuministroPedidoDeCompra;
                ac.SuministroComprobanteDeCompra = SuministroComprobanteDeCompra;
                ac.SuministroPagos = SuministroPagos;
                ac.SuministroProveedores = SuministroProveedores;
                ac.AdministracionBancos = AdministracionBancos;
                ac.AdministracionInstituciones = AdministracionInstituciones;
                ac.AdministracionGastos = AdministracionGastos;
                ac.AdministracionMovimientos = AdministracionMovimientos;
                ac.AdministracionDetalleBancario = AdministracionDetalleBancario;
                ac.AdministracionCheques = AdministracionCheques;
                ac.AdministracionCaja = AdministracionCaja;
                ac.AdministracionCuentasCorrientes = AdministracionCuentasCorrientes;
                ac.ProduccionMateriales = ProduccionMateriales;
                ac.ProduccionAlmacenes = ProduccionAlmacenes;
                ac.ProduccionCostos = ProduccionCostos;
                ac.ProduccionRecursos = ProduccionRecursos;
                ac.ProduccionPlanificacion = ProduccionPlanificacion;
                ac.PlaneamientoObjetivos = PlaneamientoObjetivos;
                ac.PlaneamientoProgramas = PlaneamientoProgramas;
                ac.PlaneamientoPresupuestos = PlaneamientoPresupuestos;
                ac.InfoFinancierosVentasVsCompras = InfoFinancierosVentasVsCompras;
                ac.InfoFinancierosComprasPorCategoria = InfoFinancierosComprasPorCategoria;
                ac.InfoGananciasVsPerdidasCobradoVsPagado = InfoGananciasVsPerdidasCobradoVsPagado;
                ac.InfoImpositivosIVAVentas = InfoImpositivosIVAVentas;
                ac.InfoImpositivosIVACompras = InfoImpositivosIVACompras;
                ac.InfoImpositivosIVASaldo = InfoImpositivosIVASaldo;
                ac.InfoImpositivosRetenciones = InfoImpositivosRetenciones;
                ac.InfoImpositivosPercepciones = InfoImpositivosPercepciones;
                ac.InfoImpositivosCITIComprasYVentas = InfoImpositivosCITIComprasYVentas;
                ac.InfoGestionCuentaCorriente = InfoGestionCuentaCorriente;
                ac.InfoGestionCobranzaPendientes = InfoGestionCobranzaPendientes;
                ac.InfoGestionPagoAProveedores = InfoGestionPagoAProveedores;
                ac.InfoGestionStockProductos = InfoGestionStockProductos;
                ac.InfoGestionStockDetalle = InfoGestionStockProductosDetalle;
                ac.InfoGestionCuentasAPagar = InfoGestionCuentasAPagar;
                ac.InfoGestionRankingPorCliente = InfoGestionRankingPorCliente;
                ac.InfoGestionRankingPorProductoServicio = InfoGestionRankingPorProductoServicio;
                ac.InfoGestionTrackingHoras = InfoGestionTrackingHoras;
                ac.InfoGestionListaFacturas = InfoGestionListaFacturas;
                ac.InfoGestionComisiones = InfoGestionComisiones;
                ac.HerramientasExploradorDeArchivos = HerramientasExploradorDeArchivos;
                ac.HerramientasImportacionMasiva = HerramientasImportacionMasiva;
                ac.HerramientasTrackingDeHoras = HerramientasTrackingDeHoras;
                ac.HerramientasConfigurarAlertas = HerramientasConfigurarAlertas;
                ac.HerramientasGeneracionCompraAutomatica = HerramientasGeneracionCompraAutomatica;
                ac.HerramientasGeneracionLiquidoProducto = HerramientasGeneracionLiquidoProducto;
                ac.HerramientasGeneracionFacturaAutomatica = HerramientasGeneracionFacturaAutomatica;
                ac.HerramientasAuditoria = HerramientasAuditoria;
                ac.HabilitarCambioIvaEnArticulosDesdeComprobante = HabilitarCambioIvaEnArticulosDesdeComprobante;

                GuardarAccesos(ac);                

                vincularEmpresaUsuario(idEmpresas, entity);
                vincularRolesUsuariosAdicionales(idEmpresas, entity);
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public static void GuardarAccesos(AccesoFormularioUsuario ac)
    {
        using (var dbContext = new ACHEEntities())
        {
            var usuAdc = dbContext.AccesoFormularioUsuario.Where(x => x.IdUsuarioAdicional == ac.IdUsuarioAdicional).FirstOrDefault();

            if (usuAdc != null)
            {
                usuAdc.HomeValores = ac.HomeValores;
                usuAdc.ComercialPedidoDeVenta = ac.ComercialPedidoDeVenta;
                usuAdc.ComercialFacturaDeVenta = ac.ComercialFacturaDeVenta;
                usuAdc.ComercialCobranzas = ac.ComercialCobranzas;
                usuAdc.ComercialPresupuestos = ac.ComercialPresupuestos;
                usuAdc.ComercialEntregas = ac.ComercialEntregas;
                usuAdc.ComercialAbonos = ac.ComercialAbonos;
                usuAdc.ComercialProductosYServicios = ac.ComercialProductosYServicios;
                usuAdc.ComercialClientes = ac.ComercialClientes;
                usuAdc.SuministroPedidoDeCompra = ac.SuministroPedidoDeCompra;
                usuAdc.SuministroComprobanteDeCompra = ac.SuministroComprobanteDeCompra;
                usuAdc.SuministroPagos = ac.SuministroPagos;
                usuAdc.SuministroProveedores = ac.SuministroProveedores;
                usuAdc.AdministracionBancos = ac.AdministracionBancos;
                usuAdc.AdministracionInstituciones = ac.AdministracionInstituciones;
                usuAdc.AdministracionGastos = ac.AdministracionGastos;
                usuAdc.AdministracionMovimientos = ac.AdministracionMovimientos;
                usuAdc.AdministracionDetalleBancario = ac.AdministracionDetalleBancario;
                usuAdc.AdministracionCheques = ac.AdministracionCheques;
                usuAdc.AdministracionCaja = ac.AdministracionCaja;
                usuAdc.AdministracionCuentasCorrientes = ac.AdministracionCuentasCorrientes;
                usuAdc.ProduccionMateriales = ac.ProduccionMateriales;
                usuAdc.ProduccionAlmacenes = ac.ProduccionAlmacenes;
                usuAdc.ProduccionCostos = ac.ProduccionCostos;
                usuAdc.ProduccionRecursos = ac.ProduccionRecursos;
                usuAdc.ProduccionPlanificacion = ac.ProduccionPlanificacion;
                usuAdc.PlaneamientoObjetivos = ac.PlaneamientoObjetivos;
                usuAdc.PlaneamientoProgramas = ac.PlaneamientoProgramas;
                usuAdc.PlaneamientoPresupuestos = ac.PlaneamientoPresupuestos;
                usuAdc.InfoFinancierosVentasVsCompras = ac.InfoFinancierosVentasVsCompras;
                usuAdc.InfoFinancierosComprasPorCategoria = ac.InfoFinancierosComprasPorCategoria;
                usuAdc.InfoGananciasVsPerdidasCobradoVsPagado = ac.InfoGananciasVsPerdidasCobradoVsPagado;
                usuAdc.InfoImpositivosIVAVentas = ac.InfoImpositivosIVAVentas;
                usuAdc.InfoImpositivosIVACompras = ac.InfoImpositivosIVACompras;
                usuAdc.InfoImpositivosIVASaldo = ac.InfoImpositivosIVASaldo;
                usuAdc.InfoImpositivosRetenciones = ac.InfoImpositivosRetenciones;
                usuAdc.InfoImpositivosPercepciones = ac.InfoImpositivosPercepciones;
                usuAdc.InfoImpositivosCITIComprasYVentas = ac.InfoImpositivosCITIComprasYVentas;
                usuAdc.InfoGestionCuentaCorriente = ac.InfoGestionCuentaCorriente;
                usuAdc.InfoGestionCobranzaPendientes = ac.InfoGestionCobranzaPendientes;
                usuAdc.InfoGestionPagoAProveedores = ac.InfoGestionPagoAProveedores;
                usuAdc.InfoGestionStockProductos = ac.InfoGestionStockProductos;
                usuAdc.InfoGestionStockDetalle = ac.InfoGestionStockDetalle;
                usuAdc.InfoGestionCuentasAPagar = ac.InfoGestionCuentasAPagar;
                usuAdc.InfoGestionRankingPorCliente = ac.InfoGestionRankingPorCliente;
                usuAdc.InfoGestionRankingPorProductoServicio = ac.InfoGestionRankingPorProductoServicio;
                usuAdc.InfoGestionTrackingHoras = ac.InfoGestionTrackingHoras;
                usuAdc.InfoGestionListaFacturas = ac.InfoGestionListaFacturas;
                usuAdc.InfoGestionComisiones = ac.InfoGestionComisiones;
                usuAdc.HerramientasExploradorDeArchivos = ac.HerramientasExploradorDeArchivos;
                usuAdc.HerramientasImportacionMasiva = ac.HerramientasImportacionMasiva;
                usuAdc.HerramientasTrackingDeHoras = ac.HerramientasTrackingDeHoras;
                usuAdc.HerramientasConfigurarAlertas = ac.HerramientasConfigurarAlertas;
                usuAdc.HerramientasGeneracionCompraAutomatica = ac.HerramientasGeneracionCompraAutomatica;
                usuAdc.HerramientasGeneracionLiquidoProducto = ac.HerramientasGeneracionLiquidoProducto;
                usuAdc.HerramientasGeneracionFacturaAutomatica = ac.HerramientasGeneracionFacturaAutomatica;
                usuAdc.HerramientasAuditoria = ac.HerramientasAuditoria;
                usuAdc.HabilitarCambioIvaEnArticulosDesdeComprobante = ac.HabilitarCambioIvaEnArticulosDesdeComprobante;
                dbContext.SaveChanges();
            }
            else
            {
                dbContext.AccesoFormularioUsuario.Add(ac);
                dbContext.SaveChanges();
            }
        }
            
    }

    public static void vincularEmpresaUsuario(string idEmpresas, UsuariosAdicionales usuAd)
    {
        using (var dbContext = new ACHEEntities())
        {
            var usuariosEmpresas = dbContext.UsuariosEmpresa.Where(x => x.IDUsuarioAdicional == usuAd.IDUsuarioAdicional).ToList();
            foreach (var item in usuariosEmpresas)
            {
                dbContext.UsuariosEmpresa.Remove(item);
            }

            var lista = Regex.Split(idEmpresas, "#-#").ToList();
            lista.RemoveAt(0);
            for (int i = 0; i < lista.Count; i++)
            {
                var flagChk = Regex.Split(lista[i], "_")[0];
                if (flagChk == "chkEmpresa")
                {
                    var idEmpresa = Convert.ToInt32(Regex.Split(lista[i], "_")[1]);
                    UsuariosEmpresa entity = new UsuariosEmpresa();

                    entity.IDUsuario = idEmpresa;
                    entity.IDUsuarioAdicional = usuAd.IDUsuarioAdicional;

                    dbContext.UsuariosEmpresa.Add(entity);
                }
            }
            dbContext.SaveChanges();
        }
    }

    public static void vincularRolesUsuariosAdicionales(string idRoles, UsuariosAdicionales usuAd)
    {
        using (var dbContext = new ACHEEntities())
        {
            //var RolesUsuariosAD = dbContext.RolesUsuariosAdicionales.Where(x => x.IDUsuarioAdicional == usuAd.IDUsuarioAdicional).ToList();
            //foreach (var item in RolesUsuariosAD)
            //    dbContext.RolesUsuariosAdicionales.Remove(item);

            //if (usuAd.Tipo == "B")
            //{
            //    var lista = Regex.Split(idRoles, "#-#").ToList();
            //    lista.RemoveAt(0);
            //    for (int i = 0; i < lista.Count; i++)
            //    {
            //        var flagChk = Regex.Split(lista[i], "_")[0];
            //        if (flagChk == "chkModulos")
            //        {
            //            var idRol = Convert.ToInt32(Regex.Split(lista[i], "_")[1]);
            //            var entity = new RolesUsuariosAdicionales();

            //            entity.IDRol = idRol;
            //            entity.IDUsuarioAdicional = usuAd.IDUsuarioAdicional;

            //            dbContext.RolesUsuariosAdicionales.Add(entity);
            //        }
            //    }
            //}
            dbContext.SaveChanges();
        }
    }

    private void accesoEmpresas(int IDUsuarioAd, bool esAlta)
    {
        try
        {
            CheckBox chkList1;

            using (var dbContext = new ACHEEntities())
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                var listEntity = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == usu.IDUsuario).ToList();

                if (listEntity.Any())
                {
                    lblNombre.Text = "<label class='control-label'>Acceso a otras empresas </label><br />";

                    foreach (var item in listEntity)
                    {
                        chkList1 = new CheckBox();
                        chkList1.Text = "&nbsp;" + item.RazonSocial;
                        chkList1.ID = "chkEmpresa_" + item.IDUsuario.ToString();
                        chkList1.Font.Name = "Verdana";
                        chkList1.Font.Size = 9;

                        if (!esAlta)
                        {
                            if (dbContext.UsuariosEmpresa.Any(x => x.IDUsuario == item.IDUsuario && x.IDUsuarioAdicional == IDUsuarioAd))
                            {
                                chkList1.Checked = true;
                            }
                        }

                        panelChk.Controls.Add(chkList1);
                        panelChk.Controls.Add(new LiteralControl("<br>"));
                    }

                    var listEmpresas = dbContext.UsuariosEmpresa.Where(x => x.IDUsuarioAdicional == usu.IDUsuario).ToList();
                }
            }
        }
        catch (Exception exp)
        {
            throw new Exception(exp.Message);
        }
    }

    private void armarChkModulos(int IDUsuarioAd, bool esAlta)
    {
        try
        {
            CheckBox chkList1;

            using (var dbContext = new ACHEEntities())
            {
                var listEntity = dbContext.Roles.ToList();

                if (listEntity.Any())
                {
                    foreach (var item in listEntity)
                    {
                        chkList1 = new CheckBox();
                        chkList1.Text = "&nbsp;" + item.Nombre;
                        chkList1.ID = "chkModulos_" + item.IDRol.ToString();
                        chkList1.Font.Name = "Verdana";
                        chkList1.Font.Size = 9;

                        if (!esAlta)
                        {
                            //if (dbContext.RolesUsuariosAdicionales.Any(x => x.IDUsuarioAdicional == IDUsuarioAd && x.IDRol == item.IDRol))
                            //{
                            //    chkList1.Checked = true;
                            //}
                        }

                        pnlChkAccesoModulos.Controls.Add(chkList1);
                        pnlChkAccesoModulos.Controls.Add(new LiteralControl("<br>"));
                    }
                }
            }
        }
        catch (Exception exp)
        {
            throw new Exception(exp.Message);
        }
    }
}