using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Extensions;
using ACHE.Admin.Models;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using ACHE.Negocio.Common;
using ACHE.Negocio.Contabilidad;
using ACHE.Model.ViewModels;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json;

namespace ACHE.Admin.Controllers
{
    public class UsuarioController : BaseController
    {
        public ActionResult Index(int idPlan = 0, string estado = "")
        {
            if (idPlan > 0 && !string.IsNullOrWhiteSpace(estado))
            {
                var model = new PlanesViewModel();
                model.Estado = estado;
                switch (idPlan)
                {
                    case 1:
                        model.Plan = "Basico";
                        break;
                    case 2:
                        model.Plan = "Profesional";
                        break;
                    case 3:
                        model.Plan = "Pyme";
                        break;
                    case 4:
                        model.Plan = "Empresa";
                        break;
                    case 5:
                        model.Plan = "Corporativo";
                        break;
                    case 6:
                        model.Plan = "Prueba";
                        break;
                }
                return View(model);
            }
            else
                return View();
        }



        public ActionResult Edicion(int id)
        {
            var model = new UsuarioViewModel();
            using (var dbContext = new ACHEEntities())
            {
                model = dbContext.Usuarios.Include("PlanesPagos").Where(x => x.IDUsuario == id).Select(x => new UsuarioViewModel()
                {
                    ID = x.IDUsuario,
                    RazonSocial = x.RazonSocial,
                    CondicionIva = x.CondicionIva,
                    CUIT = x.CUIT,
                    Telefono = x.Telefono,
                    Email = x.Email,
                    EsAgentePercepcionIIBB = (x.EsAgentePercepcionIIBB == null) ? false : (bool)x.EsAgentePercepcionIIBB,
                    EsAgentePercepcionIVA = (x.EsAgentePercepcionIVA == null) ? false : (bool)x.EsAgentePercepcionIVA,
                    EsAgenteRetencion = (x.EsAgenteRetencion == null) ? false : (bool)x.EsAgenteRetencion,
                    MercadoPagoClientSecret = x.MercadoPagoClientSecret,
                    MercadoPagoClientID = x.MercadoPagoClientID,
                    SetupRealizado = x.SetupRealizado ? "SI" : "NO",
                    ApiKey = x.ApiKey,
                    TemplateFc = x.TemplateFc,
                    Celular = x.Celular,
                    IIBB = x.IIBB,
                    Activo = x.Activo,
                    EmailAlertas = x.EmailAlertas,
                    TieneFacturaElectronica = x.TieneFacturaElectronica,
                    FechaAlta = x.FechaAlta,
                    FechaInicioActividades = (x.FechaInicioActividades == null) ? DateTime.Now : (DateTime)x.FechaInicioActividades,
                    CodigoPostal = x.CodigoPostal,
                    CodigoPromo = x.CodigoPromo,
                    Domicilio = x.Domicilio,
                    Contacto = x.Contacto,
                    CorreoPortal = x.CorreoPortal,
                    FechaFinPlan = x.FechaFinPlan,
                    IDCiudad = x.IDCiudad,
                    Personeria = x.Personeria,
                    PisoDepto = x.PisoDepto,
                    Logo = x.Logo,
                    //FechaUltLogin = x.FechaUltLogin.ToString("dd/MM/yyyy"),
                    UsaFechaFinPlan = x.UsaFechaFinPlan,
                    IDProvincia = x.IDProvincia,
                    IDUsuarioPadre = (x.IDUsuarioPadre == null) ? 0 : (Int32)x.IDUsuarioPadre,
                    PortalClientes = x.PortalClientes,
                    UsaProd = x.UsaProd,
                    listaPuntos = x.PuntosDeVenta.ToList(),
                    Observaciones = x.Observaciones,
                    ProvinciaNombre = x.Provincias.Nombre,
                    CiudadNombre = (x.Ciudades == null) ? "" : x.Ciudades.Nombre,
                    CantEmpresasHabilitadas = x.CantidadEmpresas,
                    ExentoIIBB = (x.ExentoIIBB != null) ? false : x.ExentoIIBB,
                    Pwd = x.Pwd,
                    UsaPrecioFinalConIVA = x.UsaPrecioFinalConIVA,
                    FechaBaja = x.FechaBaja,
                    MotivoBaja = (x.MotivoBaja == null) ? "" : x.MotivoBaja,
                    EstaBloqueado = x.EstaBloqueado,
                    EstaBloqueadoAd = x.UsuariosAdicionales.Any(y => y.EstaBloqueado),
                    EsContador = x.EsContador,
                    UsaPlanCorporativo = x.UsaPlanCorporativo,
                    CUITAfip = x.CUITAfip,
                    EsVendedor = x.EsVendedor,
                    PorcentajeComision = x.PorcentajeComision
                }).FirstOrDefault();

                string rutaCertificadoPfxAfip = ConfigurationManager.AppSettings["PathBaseCertificadosAfip"] + model.CUITAfip + ".pfx";
                if (System.IO.File.Exists(rutaCertificadoPfxAfip))
                {
                    var file = new FileInfo(rutaCertificadoPfxAfip);
                    model.CertificadoPfxActual = "Nombre: " + file.Name + " - Fecha: " + file.LastWriteTime.ToShortDateString();
                }

                var planActual = dbContext.PlanesPagos.Include("Planes").Where(x => x.IDUsuario == id && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();

                model.PlanActual = (planActual == null) ? "SIN Plan Actual" : planActual.Planes.Nombre;
                model.PlanEstado = (planActual == null) ? "SIN Plan Actual" : planActual.Estado;
                model.FechaFinPlanActual = (planActual == null) ? "SIN Plan Actual" : Convert.ToDateTime(planActual.FechaFinPlan).ToString("dd/MM/yyyy");
                model.TienePlanDeCuentas = dbContext.PlanDeCuentas.Any(x => x.IDUsuario == id);
                model.ListaUsuariosAd = dbContext.UsuariosAdicionales.Where(x => x.IDUsuario == id).Select(x => new UsuariosAdViewModel()
                {
                    Activo = (x.Activo) ? "SI" : "NO",
                    Correo = x.Email,
                    NivelSeguridad = (x.Tipo == "A") ? "Administrador" : "Backoffice"
                }).ToList();


                model.ListaEmpresas = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == id).Select(x => new EmpresasViewModel()
                {
                    ID = x.IDUsuario,
                    CondicionIva = x.CondicionIva,
                    CUIT = x.CUIT,
                    RazonSocial = x.RazonSocial,
                    Domicilio = x.Domicilio + ", " + x.PisoDepto,
                    TieneFacturaElectronica = (x.TieneFacturaElectronica) ? "SI" : "NO",
                    UsaProd = (x.UsaProd) ? "SI" : "NO"
                }).ToList();

                model.listaPlanesPagos = dbContext.PlanesPagos
                                                  .Include("Planes")
                                                  .Include("Usuarios")
                                                  .Include("Comprobantes")
                                                  .Include("Comprobantes.PuntosDeVenta")
                                                  .Where(x => x.IDUsuario == id)
                                                  .OrderBy(x => x.FechaInicioPlan).ToList();



                model.listaLoginUsuarios = dbContext.LoginUsuarios.Include("Usuarios")
                                                                  .Include("UsuariosAdicionales")
                                                                  .Where(x => x.IDUsuario == id && x.IDUsuarioAdicional == null).OrderByDescending(x => x.IDLogin).Take(10)
                                                                  .Select(x => new LoginUsuarioViewModel()
                                                                  {
                                                                      IDLoginUsuario = x.IDLogin,
                                                                      Email = x.EmailLogin,
                                                                      Fecha = x.FechaLogin,
                                                                      Observaciones = x.Observacion
                                                                  }).ToList();

                model.listaLoginUsuariosAdicionales = dbContext.LoginUsuarios.Include("Usuarios")
                                                                  .Include("UsuariosAdicionales")
                                                                  .Where(x => x.IDUsuario == id && x.IDUsuarioAdicional != null).OrderByDescending(x => x.IDLogin).Take(10)
                                                                  .Select(x => new LoginUsuarioViewModel()
                                                                  {
                                                                      IDLoginUsuario = x.IDLogin,
                                                                      RazonSocial = x.Usuarios.RazonSocial,
                                                                      Email = x.EmailLogin,
                                                                      Fecha = x.FechaLogin,
                                                                      Observaciones = x.Observacion
                                                                  }).ToList();

                var fecha = DateTime.Now.Date.GetFirstDayOfMonth();
                model.Estadisticas = new EstadisticasViewModel();

                model.Estadisticas.CantClientes = dbContext.Personas.Count(x => x.IDUsuario == id && x.Tipo == "C");
                model.Estadisticas.CantProveedores = dbContext.Personas.Count(x => x.IDUsuario == id && x.Tipo == "P");

                model.Estadisticas.CantVentasTotal = dbContext.Comprobantes.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantVentasMes = dbContext.Comprobantes.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantAbonosTotal = dbContext.Abonos.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantAbonosMes = dbContext.Abonos.Count(x => x.IDUsuario == id && x.FechaFin >= fecha);

                model.Estadisticas.CantProductosTotal = dbContext.Conceptos.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantProductosMes = dbContext.Conceptos.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantComprasTotal = dbContext.Compras.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantComprasMes = dbContext.Compras.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantPagosTotal = dbContext.Pagos.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantPagosMes = dbContext.Pagos.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantCobranzasTotal = dbContext.Cobranzas.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantCobranzasMes = dbContext.Cobranzas.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantPresupuestosTotal = dbContext.Presupuestos.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantPresupuestosMes = dbContext.Presupuestos.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantMovcajaTotal = dbContext.Caja.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantMovcajaMes = dbContext.Caja.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.Estadisticas.CantChequesTotal = dbContext.Cheques.Count(x => x.IDUsuario == id);
                model.Estadisticas.CantChequesMes = dbContext.Cheques.Count(x => x.IDUsuario == id && x.FechaAlta >= fecha);

                model.acceso = dbContext.AccesoFormularioUsuario.Where(x => x.IdUsuario == id).FirstOrDefault();

                long cuit = Convert.ToInt64(model.CUIT);
                model.accesoGestor = dbContext.LicenciaTemp.Where(w => w.CUIT == cuit).Any();

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult FijarNuevaClave(int id, string nueva)
        {
            bool resultado = true;
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == id).FirstOrDefault();

                    if (resultado)
                    {
                        usu.EstaBloqueado = false;
                        usu.CantIntentos = 0;
                        usu.Pwd = MD5Hash(nueva);

                        dbContext.SaveChanges();
                    }
                }
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ResetearPwd(int id)
        {
            bool send = false;
            try
            {
                using (var dbContext = new ACHEEntities())
                {

                    var usuView = dbContext.UsuariosView.Where(x => x.IDUsuario == id && x.Activo).FirstOrDefault();
                    if (usuView != null)
                    {
                        string newPwd = string.Empty;
                        //newPwd = newPwd.GenerateRandom(10);
                        newPwd = newPwd.GenerateRandomPassword();

                        ListDictionary replacements = new ListDictionary();
                        replacements.Add("<PASSWORD>", newPwd);
                        if (usuView.IDUsuarioAdicional == 0)
                            replacements.Add("<USUARIO>", usuView.RazonSocial);
                        else
                            replacements.Add("<USUARIO>", "usuario");

                        send = EmailHelper.SendMessage(EmailTemplate.RecuperoPwd, replacements, usuView.Email, "axanweb: Recuperación de contraseña");
                        if (!send)
                            throw new Exception("El email con la nueva contraseña no pudo ser enviado.");
                        else
                        {
                            if (usuView.IDUsuarioAdicional == 0)
                            {
                                var usu = dbContext.Usuarios.Where(x => x.IDUsuario == usuView.IDUsuario).FirstOrDefault();
                                usu.CantIntentos = 0;
                                usu.EstaBloqueado = false;
                                usu.Pwd = MD5Hash(newPwd);
                            }
                            else
                            {
                                var usu = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == usuView.IDUsuarioAdicional).FirstOrDefault();
                                usu.Pwd = MD5Hash(newPwd);
                                usu.CantIntentos = 0;
                                usu.EstaBloqueado = false;
                            }
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(send, JsonRequestBehavior.AllowGet);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        [HttpPost]
        public ActionResult Guardar(int id, string condicionIva, string celular, string email, string observaciones,
            bool factElectronica, bool usaProd, int cantEmpresasHabilitadas, bool activo, string motivoBaja, 
            bool accesoGestor, bool estaBloqueado, string CUITAfip, 
            string flpNombreArchivoCertificado, string flpArchivoCertificado/*, bool estaBloqueadoAd*/,
            bool esVendedor, string porcentajeComision)
        {
            bool resultado = true;
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == id).FirstOrDefault();
                    //if (!email.IsValidEmailAddress())
                    //    resultado = false;
                    if (dbContext.Usuarios.Any(x => x.IDUsuario != id && x.Email == email))
                        resultado = false;

                    if (resultado)
                    {
                        usu.CondicionIva = condicionIva;
                        usu.Celular = celular;
                        usu.Email = email;
                        usu.Observaciones = observaciones;
                        usu.TieneFacturaElectronica = factElectronica;
                        usu.UsaProd = usaProd;
                        usu.CantidadEmpresas = cantEmpresasHabilitadas;
                        usu.MotivoBaja = motivoBaja;
                        usu.Activo = activo;
                        usu.CUITAfip = CUITAfip;
                        if (flpArchivoCertificado != null && flpArchivoCertificado.Length > 0 && flpArchivoCertificado != "undefined")
                        {
                            string base64 = flpArchivoCertificado.Replace("data:application/x-pkcs12;base64,", "");
                            // Saving Base64 string as file.
                            System.IO.File.WriteAllBytes(ConfigurationManager.AppSettings["PathBaseCertificadosAfip"] + flpNombreArchivoCertificado, Convert.FromBase64String(base64));
                        }                      
                        usu.EstaBloqueado = estaBloqueado;
                        usu.CantIntentos = (estaBloqueado) ? 3 : 0;
                        usu.EsVendedor = esVendedor;

                        if (usu.EsVendedor)
                            usu.PorcentajeComision = Convert.ToDecimal(porcentajeComision);

                        //foreach (var item in usu.UsuariosAdicionales)
                        //{
                        //    item.EstaBloqueado = estaBloqueadoAd;
                        //    item.CantIntentos = (estaBloqueadoAd) ? 3 : 0;
                        //}

                        if (activo)
                            usu.FechaBaja = null;
                        else
                            usu.FechaBaja = DateTime.Now;

                        dbContext.SaveChanges();

                        if (accesoGestor) //Tengo que darle acceso al gestor insertando un registro en LicenciaTemp
                        {
                            long cuit = Convert.ToInt64(usu.CUIT);
                            LicenciaTemp lt = dbContext.LicenciaTemp.Where(w => w.CUIT == cuit).FirstOrDefault();
                            if(lt == null)
                            {
                                LicenciaTemp ltTemplate = dbContext.LicenciaTemp.FirstOrDefault();
                                if (ltTemplate  != null)
                                {
                                    LicenciaTemp ltNew = new LicenciaTemp();
                                    ltNew.CUIT = cuit;
                                    ltNew.Email = usu.Email;
                                    ltNew.Vigencia = ltTemplate.Vigencia;
                                    ltNew.Clave = ltTemplate.Clave;
                                    ltNew.Modulo1_Nombre = ltTemplate.Modulo1_Nombre;
                                    ltNew.Modulo1_Version = ltTemplate.Modulo1_Version;
                                    ltNew.Modulo1_UrlInstalador32 = ltTemplate.Modulo1_UrlInstalador32;
                                    ltNew.Modulo1_UrlInstalador64 = ltTemplate.Modulo1_UrlInstalador64;
                                    ltNew.Modulo2_Nombre = ltTemplate.Modulo2_Nombre;
                                    ltNew.Modulo2_Version = ltTemplate.Modulo2_Version;
                                    ltNew.Modulo2_UrlInstalador32 = ltTemplate.Modulo2_UrlInstalador32;
                                    ltNew.Modulo2_UrlInstalador64 = ltTemplate.Modulo2_UrlInstalador64;
                                    ltNew.Modulo3_Nombre = ltTemplate.Modulo3_Nombre;
                                    ltNew.Modulo3_Version = ltTemplate.Modulo3_Version;
                                    ltNew.Modulo3_UrlInstalador32 = ltTemplate.Modulo3_UrlInstalador32;
                                    ltNew.Modulo3_UrlInstalador64 = ltTemplate.Modulo3_UrlInstalador64;
                                    ltNew.Modulo4_Nombre = ltTemplate.Modulo4_Nombre;
                                    ltNew.Modulo4_Version = ltTemplate.Modulo4_Version;
                                    ltNew.Modulo4_UrlInstalador32 = ltTemplate.Modulo4_UrlInstalador32;
                                    ltNew.Modulo4_UrlInstalador64 = ltTemplate.Modulo4_UrlInstalador64;
                                    ltNew.Modulo5_Nombre = ltTemplate.Modulo5_Nombre;
                                    ltNew.Modulo5_Version = ltTemplate.Modulo5_Version;
                                    ltNew.Modulo5_UrlInstalador32 = ltTemplate.Modulo5_UrlInstalador32;
                                    ltNew.Modulo5_UrlInstalador64 = ltTemplate.Modulo5_UrlInstalador64;
                                    dbContext.LicenciaTemp.Add(ltNew);
                                    dbContext.SaveChanges();
                                }
                            }
                        } 
                        else //Me fijo si existe y lo elimino de la tabla licenciaTemp
                        {
                            long cuit = Convert.ToInt64(usu.CUIT);
                            LicenciaTemp lt = dbContext.LicenciaTemp.Where(w => w.CUIT == cuit).FirstOrDefault();
                            if (lt != null)
                            {
                                dbContext.LicenciaTemp.Remove(lt);
                                dbContext.SaveChanges();
                            }
                        }

                    }
                }
            }
            catch 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GuardarAccesos(int id, 
                                          bool HomeValores, 
                                          bool ComercialPedidoDeVenta, 
                                          bool ComercialFacturaDeVenta,  
                                          bool ComercialCobranzas, 
                                          bool ComercialPresupuestos,
                                          bool ComercialEntregas,
                                          bool ComercialAbonos, 
                                          bool ComercialProductosYServicios,  
                                          bool ComercialClientes, 
                                          bool SuministroPedidoDeCompra,  
                                          bool SuministroComprobanteDeCompra, 
                                          bool SuministroPagos,  
                                          bool SuministroProveedores, 
                                          bool AdministracionBancos,  
                                          bool AdministracionInstituciones, 
                                          bool AdministracionGastos,  
                                          bool AdministracionMovimientos, 
                                          bool AdministracionDetalleBancario, 
                                          bool AdministracionCheques, 
                                          bool AdministracionCaja,  
                                          bool AdministracionCuentasCorrientes, 
                                          bool ProduccionMateriales,  
                                          bool ProduccionAlmacenes, 
                                          bool ProduccionCostos,  
                                          bool ProduccionRecursos, 
                                          bool ProduccionPlanificacion,  
                                          bool PlaneamientoObjetivos, 
                                          bool PlaneamientoProgramas,  
                                          bool PlaneamientoPresupuestos, 
                                          bool InfoFinancierosVentasVsCompras,  
                                          bool InfoFinancierosComprasPorCategoria, 
                                          bool InfoGananciasVsPerdidasCobradoVsPagado,  
                                          bool InfoImpositivosIVAVentas, 
                                          bool InfoImpositivosIVACompras,  
                                          bool InfoImpositivosIVASaldo, 
                                          bool InfoImpositivosRetenciones,  
                                          bool InfoImpositivosPercepciones, 
                                          bool InfoImpositivosCITIComprasYVentas, 
                                          bool InfoGestionCuentaCorriente, 
                                          bool InfoGestionCobranzaPendientes,  
                                          bool InfoGestionPagoAProveedores, 
                                          bool InfoGestionStockProductos, 
                                          bool InfoGestionStockProductosDetalle, 
                                          bool InfoGestionCuentasAPagar, 
                                          bool InfoGestionRankingPorCliente, 
                                          bool InfoGestionRankingPorProductoServicio, 
                                          bool InfoGestionTrackingHoras, 
                                          bool InfoGestionListaFacturas, 
                                          bool InfoGestionComisiones, 
                                          bool HerramientasExploradorDeArchivos, 
                                          bool HerramientasImportacionMasiva, 
                                          bool HerramientasTrackingDeHoras, 
                                          bool HerramientasConfigurarAlertas, 
                                          bool HerramientasGeneracionCompraAutomatica, 
                                          bool HerramientasGeneracionLiquidoProducto, 
                                          bool HerramientasGeneracionFacturaAutomatica, 
                                          bool HerramientasAuditoria, 
                                          bool HabilitarCambioIvaEnArticulosDesdeComprobante)
        {
            bool resultado = true;
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = dbContext.AccesoFormularioUsuario.Where(x => x.IdUsuario == id).FirstOrDefault();

                    if (usu != null)
                    {
                        usu.HomeValores = HomeValores;
                        usu.ComercialPedidoDeVenta = ComercialPedidoDeVenta;
                        usu.ComercialFacturaDeVenta = ComercialFacturaDeVenta;
                        usu.ComercialCobranzas = ComercialCobranzas;
                        usu.ComercialPresupuestos = ComercialPresupuestos;
                        usu.ComercialEntregas = ComercialEntregas;
                        usu.ComercialAbonos = ComercialAbonos;
                        usu.ComercialProductosYServicios = ComercialProductosYServicios;
                        usu.ComercialClientes = ComercialClientes;
                        usu.SuministroPedidoDeCompra = SuministroPedidoDeCompra;
                        usu.SuministroComprobanteDeCompra = SuministroComprobanteDeCompra;
                        usu.SuministroPagos = SuministroPagos;
                        usu.SuministroProveedores = SuministroProveedores;
                        usu.AdministracionBancos = AdministracionBancos;
                        usu.AdministracionInstituciones = AdministracionInstituciones;
                        usu.AdministracionGastos = AdministracionGastos;
                        usu.AdministracionMovimientos = AdministracionMovimientos;
                        usu.AdministracionDetalleBancario = AdministracionDetalleBancario;
                        usu.AdministracionCheques = AdministracionCheques;
                        usu.AdministracionCaja = AdministracionCaja;
                        usu.AdministracionCuentasCorrientes = AdministracionCuentasCorrientes;
                        usu.ProduccionMateriales = ProduccionMateriales;
                        usu.ProduccionAlmacenes = ProduccionAlmacenes;
                        usu.ProduccionCostos = ProduccionCostos;
                        usu.ProduccionRecursos = ProduccionRecursos;
                        usu.ProduccionPlanificacion = ProduccionPlanificacion;
                        usu.PlaneamientoObjetivos = PlaneamientoObjetivos;
                        usu.PlaneamientoProgramas = PlaneamientoProgramas;
                        usu.PlaneamientoPresupuestos = PlaneamientoPresupuestos;
                        usu.InfoFinancierosVentasVsCompras = InfoFinancierosVentasVsCompras;
                        usu.InfoFinancierosComprasPorCategoria = InfoFinancierosComprasPorCategoria;
                        usu.InfoGananciasVsPerdidasCobradoVsPagado = InfoGananciasVsPerdidasCobradoVsPagado;
                        usu.InfoImpositivosIVAVentas = InfoImpositivosIVAVentas;
                        usu.InfoImpositivosIVACompras = InfoImpositivosIVACompras;
                        usu.InfoImpositivosIVASaldo = InfoImpositivosIVASaldo;
                        usu.InfoImpositivosRetenciones = InfoImpositivosRetenciones;
                        usu.InfoImpositivosPercepciones = InfoImpositivosPercepciones;
                        usu.InfoImpositivosCITIComprasYVentas = InfoImpositivosCITIComprasYVentas;
                        usu.InfoGestionCuentaCorriente = InfoGestionCuentaCorriente;
                        usu.InfoGestionCobranzaPendientes = InfoGestionCobranzaPendientes;
                        usu.InfoGestionPagoAProveedores = InfoGestionPagoAProveedores;
                        usu.InfoGestionStockProductos = InfoGestionStockProductos;
                        usu.InfoGestionStockDetalle = InfoGestionStockProductosDetalle;
                        usu.InfoGestionCuentasAPagar = InfoGestionCuentasAPagar;
                        usu.InfoGestionRankingPorCliente = InfoGestionRankingPorCliente;
                        usu.InfoGestionRankingPorProductoServicio = InfoGestionRankingPorProductoServicio;
                        usu.InfoGestionTrackingHoras = InfoGestionTrackingHoras;
                        usu.InfoGestionListaFacturas = InfoGestionListaFacturas;
                        usu.InfoGestionComisiones = InfoGestionComisiones;
                        usu.HerramientasExploradorDeArchivos = HerramientasExploradorDeArchivos;
                        usu.HerramientasImportacionMasiva = HerramientasImportacionMasiva;
                        usu.HerramientasTrackingDeHoras = HerramientasTrackingDeHoras;
                        usu.HerramientasConfigurarAlertas = HerramientasConfigurarAlertas;
                        usu.HerramientasGeneracionCompraAutomatica = HerramientasGeneracionCompraAutomatica;
                        usu.HerramientasGeneracionLiquidoProducto = HerramientasGeneracionLiquidoProducto;
                        usu.HerramientasGeneracionFacturaAutomatica = HerramientasGeneracionFacturaAutomatica;
                        usu.HerramientasAuditoria = HerramientasAuditoria;
                        usu.HabilitarCambioIvaEnArticulosDesdeComprobante = HabilitarCambioIvaEnArticulosDesdeComprobante;

                        dbContext.SaveChanges();
                    }
                }
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ObtenerUsuarios(string condicion, string periodo,
        string fechaDesde, string fechaHasta, string tipoPlan, string EstadoUsuario, int page, int pageSize)
        {
            ResultadosUsuarioViewModel resultado = new ResultadosUsuarioViewModel();
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario != 2).OrderByDescending(x => x.FechaAlta).ToList();
                    var fecha = DateTime.Now.Date;

                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                    }

                    switch (EstadoUsuario)
                    {
                        case "Setup OK":
                        case "Total Activos":
                            results = results.Where(x => x.Activo == true && x.SetupRealizado == true && x.Estado == "Aceptado" && x.FechaFinPlan >= fecha).ToList();
                            break;
                        case "Total Pendientes de pago":
                            results = results.Where(x => x.Activo == true && x.Estado == "Pendiente" && x.FechaFinPlan > fecha).ToList();
                            break;
                        case "Total Inactivos":
                            results = results.Where(x => x.Activo == true && (x.SetupRealizado == false || x.FechaFinPlan < fecha || (x.Estado == "Aceptado" && x.FechaFinPlan < fecha))).ToList();
                            break;
                        case "Dados de baja":
                            results = results.Where(x => x.Activo == false).ToList();
                            break;
                        case "Total Usuarios":
                        default:
                            results = results.Where(x => x.Activo == true).ToList();
                            break;
                    }

                    switch (tipoPlan)
                    {
                        case "Basico":
                            results = results.Where(x => x.PlanActual == tipoPlan).ToList();
                            break;
                        case "Profesional":
                            results = results.Where(x => x.PlanActual == tipoPlan).ToList();
                            break;
                        case "Pyme":
                            results = results.Where(x => x.PlanActual == tipoPlan).ToList();
                            break;
                        case "Empresa":
                            results = results.Where(x => x.PlanActual == tipoPlan).ToList();
                            break;
                        case "Corporativo":
                            results = results.Where(x => x.PlanActual == tipoPlan).ToList();
                            break;
                        case "Prueba":
                            results = results.Where(x => x.PlanActual == tipoPlan).ToList();
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(tipoPlan))
                        results = results.Where(x => x.PlanActual.Contains(tipoPlan)).ToList();

                    if (condicion != "")
                        results = results.Where(x => x.RazonSocial.ToUpper().Contains(condicion.ToUpper()) || x.CUIT.Contains(condicion) || x.Email.ToUpper().Contains(condicion.ToUpper())).ToList();

                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaAlta >= dtDesde).ToList();
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaAlta <= dtHasta).ToList();
                    }

                    page--;

                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.GroupBy(x => x.IDUsuario).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new UsuarioViewModel()
                        {
                            ID = x.FirstOrDefault().IDUsuario,
                            RazonSocial = x.FirstOrDefault().RazonSocial,
                            CUIT = x.FirstOrDefault().CUIT,
                            Telefono = x.FirstOrDefault().Telefono,
                            Email = x.FirstOrDefault().Email,
                            SetupRealizado = (x.FirstOrDefault().SetupRealizado) ? "SI" : "NO",
                            PlanActual = x.FirstOrDefault().PlanActual,
                            AntiguedadMeses = Convert.ToInt32(x.FirstOrDefault().AntiguedadMeses),
                            CondicionIva = x.FirstOrDefault().CondicionIva,
                            FechaUltLogin = x.FirstOrDefault().FechaUltLogin.ToString("dd/MM/yyyy"),
                            Baja = x.FirstOrDefault().Activo ? "NO" : "SI",
                            FechaAltaDesc = x.FirstOrDefault().FechaAlta.ToString("dd/MM/yyyy")
                        });
                    resultado.Items = list.ToList();
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult export(string condicion, string periodo, string fechaDesde, string tipoPlan, string fechaHasta, string EstadoUsuario)
        {
            string fileName = "Usuarios";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == null).AsQueryable();
                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                    }


                    if (EstadoUsuario != "")
                    {
                        var estadoUsuario = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario != 2).OrderBy(x => x.FechaAlta).ToList();
                        var fecha = DateTime.Now.Date;
                        switch (EstadoUsuario)
                        {
                            case "Setup OK":
                            case "Total Activos":
                                estadoUsuario = estadoUsuario.Where(x => x.Activo == true && x.SetupRealizado == true && x.Estado == "Aceptado" && x.FechaFinPlan >= fecha).ToList();
                                break;
                            case "Total Pendientes de pago":
                                estadoUsuario = estadoUsuario.Where(x => x.Activo == true && x.Estado == "Pendiente" && x.FechaFinPlan > fecha).ToList();
                                break;
                            case "Total Inactivos":
                                estadoUsuario = estadoUsuario.Where(x => x.Activo == true && (x.SetupRealizado == false || x.FechaFinPlan < fecha || (x.Estado == "Aceptado" && x.FechaFinPlan < fecha))).ToList();
                                break;
                            case "Dados de baja":
                                estadoUsuario = estadoUsuario.Where(x => x.Activo == false).ToList();
                                break;
                            case "Total Usuarios":
                            default:
                                estadoUsuario = estadoUsuario.Where(x => x.Activo == true).ToList();
                                break;
                        }

                        switch (tipoPlan)
                        {
                            case "Basico":
                                estadoUsuario = estadoUsuario.Where(x => x.PlanActual == tipoPlan).ToList();
                                break;
                            case "Profesional":
                                estadoUsuario = estadoUsuario.Where(x => x.PlanActual == tipoPlan).ToList();
                                break;
                            case "Pyme":
                                estadoUsuario = estadoUsuario.Where(x => x.PlanActual == tipoPlan).ToList();
                                break;
                            case "Empresa":
                                estadoUsuario = estadoUsuario.Where(x => x.PlanActual == tipoPlan).ToList();
                                break;
                            case "Corporativo":
                                estadoUsuario = estadoUsuario.Where(x => x.PlanActual == tipoPlan).ToList();
                                break;
                            case "Prueba":
                                estadoUsuario = estadoUsuario.Where(x => x.PlanActual == tipoPlan).ToList();
                                break;
                        }

                        var idUsuarios = estadoUsuario.Select(x => x.IDUsuario).ToList();
                        if (idUsuarios.Count() > 0)
                            results = results.Where(x => idUsuarios.Contains(x.IDUsuario));
                    }


                    if (!string.IsNullOrWhiteSpace(tipoPlan))
                    {
                        var listaIdUsuarios = dbContext.UsuariosPlanesView.Where(x => x.PlanActual.Contains(tipoPlan)).ToList().Select(x => x.IDUsuario).ToList();
                        results = results.Where(x => listaIdUsuarios.Contains(x.IDUsuario));
                    }

                    if (condicion != "")
                        results = results.Where(x => x.RazonSocial.Contains(condicion) || x.CUIT.Contains(condicion) || x.Email.Contains(condicion));

                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaAlta >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaAlta <= dtHasta);
                    }

                    dt = results.ToList()
                        .Select(x => new
                        {
                            ID = x.IDUsuario,
                            RazonSocial = x.RazonSocial,
                            CondicionIva = x.CondicionIva,
                            CUIT = x.CUIT,
                            Telefono = x.Telefono,
                            Email = x.Email,
                            EsAgentePercepcionIIBB = (x.EsAgentePercepcionIIBB == null) ? false : (bool)x.EsAgentePercepcionIIBB,
                            EsAgentePercepcionIVA = (x.EsAgentePercepcionIVA == null) ? false : (bool)x.EsAgentePercepcionIVA,
                            EsAgenteRetencion = (x.EsAgenteRetencion == null) ? false : (bool)x.EsAgenteRetencion,
                            MercadoPagoClientSecret = (x.MercadoPagoClientSecret == null) ? "" : x.MercadoPagoClientSecret,
                            MercadoPagoClientID = (x.MercadoPagoClientID == null) ? "" : x.MercadoPagoClientID,
                            SetupRealizado = x.SetupRealizado ? "SI" : "NO",
                            ApiKey = x.ApiKey,
                            TemplateFc = x.TemplateFc,
                            Celular = x.Celular,
                            IIBB = x.IIBB,
                            Activo = x.Activo,
                            EmailAlertas = x.EmailAlertas,
                            TieneFacturaElectronica = x.TieneFacturaElectronica,
                            FechaAlta = x.FechaAlta,
                            FechaInicioActividades = (x.FechaInicioActividades == null) ? DateTime.Now : (DateTime)x.FechaInicioActividades,
                            CodigoPostal = x.CodigoPostal,
                            Domicilio = x.Domicilio,
                            Contacto = x.Contacto,
                            CorreoPortal = x.CorreoPortal,
                            FechaFinPlan = x.FechaFinPlan,
                            Personeria = x.Personeria,
                            PisoDepto = x.PisoDepto,
                            Logo = (x.Logo == null) ? "" : x.Logo,
                            FechaUltLogin = x.FechaUltLogin,
                            PortalClientes = x.PortalClientes,
                            UsaProd = x.UsaProd,
                            Observaciones = (x.Observaciones == null) ? "" : x.Observaciones,
                            ProvinciaNombre = (x.Provincias == null) ? "" : x.Provincias.Nombre,
                            CiudadNombre = (x.Ciudades == null) ? "" : x.Ciudades.Nombre,
                            ExentoIIBB = (x.ExentoIIBB == null) ? false : (bool)x.ExentoIIBB,
                            CodigoPromo = (string.IsNullOrWhiteSpace(x.CodigoPromo)) ? "" : x.CodigoPromo,
                            MotivoBaja = (x.MotivoBaja == null) ? "" : x.MotivoBaja
                        }).ToList().ToDataTable();
                }

                if (dt.Rows.Count > 0)
                {
                    CommonModel.GenerarArchivo(dt, Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                }
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                var archivo = (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");
                return Json(archivo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult HabilitarFCEmpresas(int id, bool tipo)
        {
            bool resultado = true;
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == id).FirstOrDefault();

                    if (usu != null)
                    {
                        usu.TieneFacturaElectronica = tipo;
                        usu.UsaProd = tipo;
                        dbContext.SaveChanges();
                    }
                }
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult HabilitarPuntoVenta(int id)
        {
            bool resultado = true;
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == id).FirstOrDefault();
                    if (punto != null)
                    {
                        punto.FechaBaja = null;
                        dbContext.SaveChanges();
                    }
                }

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult eliminarUsuario(int id)
        {
            bool resultado = true;
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

                SqlCommand comando = new SqlCommand("PA_EliminarUsuario", cn);
                comando.CommandType = CommandType.StoredProcedure;

                //Parametros
                SqlParameter parIdUsuario = new SqlParameter("@idUsuario", SqlDbType.Int);
                parIdUsuario.Direction = ParameterDirection.Input;
                parIdUsuario.Value = Convert.ToInt32(id);
                comando.Parameters.Add(parIdUsuario);

                comando.Connection.Open();
                comando.ExecuteNonQuery();
                comando.Connection.Close();
                comando.Connection.Dispose();

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult AceptarPago(int id, int idUsuario)
        {
            var dto = new ErrorViewModel();

            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var path = HttpContext.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["Admin.BasicLogError"]);
                    BasicLog.AppendToFile(path, "AceptarPago ", "Empiza la funcion AceptarPago");

                    var planesPagos = dbContext.PlanesPagos.Where(x => x.IDPlanesPagos == id && x.IDUsuario == idUsuario).FirstOrDefault();
                    var nr = planesPagos.NroReferencia;
                    var pathBase = ConfigurationManager.AppSettings["PathBaseWeb"];

                    var comprobanteViewModel = ComprobantesCommon.CrearDatosParaContabilium(nr, idUsuario, dbContext, planesPagos, pathBase);
                    planesPagos.Estado = "Aceptado";
                    planesPagos.IDComprobante = comprobanteViewModel.comprobante.IDComprobante;

                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();

                    if (planesPagos.IDPlan == 5)// Plan Corporativo
                        usu.UsaPlanCorporativo = true;
                    else
                        usu.UsaPlanCorporativo = false;

                    dbContext.SaveChanges();
                    BasicLog.AppendToFile(path, "AceptarPago ", "termina de generar los datos para contabilium");
                    BasicLog.AppendToFile(path, "AceptarPago ", "Manda el mail");
                    var send = EnviarComprobantePorEmail(comprobanteViewModel, pathBase);
                    if (!send)
                        BasicLog.AppendToFile(path, "AceptarPago ", "la factura electronica no fue enviada");
                    PersonasCommon.CrearDatosParaElCliente(nr, idUsuario, dbContext, planesPagos, comprobanteViewModel.comprobante, comprobanteViewModel.nroComprobanteElectronico);
                    BasicLog.AppendToFile(path, "AceptarPago ", "termina de dar crear los datos para el cliente");
                }
                dto.TieneError = false;
                return Json(dto, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                dto.TieneError = true;
                dto.Mensaje = ex.Message;
                var path = HttpContext.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["Admin.BasicLogError"]);
                BasicLog.AppendToFile(path, "AceptarPago Error:", ex.Message);
                return Json(dto, JsonRequestBehavior.AllowGet);
            }

        }

        private static bool EnviarComprobantePorEmail(ComprobanteNroViewModel comprobante, string pathBase)
        {

            System.Net.Mail.MailAddressCollection listTo = new System.Net.Mail.MailAddressCollection();
            listTo.Add(new System.Net.Mail.MailAddress(comprobante.comprobante.Personas.Email));

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<NOTIFICACION>", "Comprobante de contratación de plan");
            replacements.Add("<USUARIO>", comprobante.comprobante.Personas.RazonSocial);


            var nroComprobane = comprobante.comprobante.Personas.RazonSocial.Replace(" ", "_").Replace(",", "") + "_" + comprobante.comprobante.Tipo + "-" + comprobante.nroComprobanteElectronico + ".pdf";
            List<string> attachments = new List<string>();
            attachments.Add(Path.Combine(pathBase + "\\files\\explorer\\" + comprobante.comprobante.Usuarios.IDUsuario + "\\comprobantes\\" + DateTime.Now.Year.ToString() + "\\" + nroComprobane));
            bool send = EmailHelper.SendMessage(EmailTemplate.EnvioComprobante, replacements, listTo, ConfigurationManager.AppSettings["Email.Notifications"], comprobante.comprobante.Personas.Email, "Comprobante electrónico", attachments);

            return send;
        }

        [HttpPost]
        public ActionResult ConfigurarPlanCorporativo(int idUsuario)
        {
            var dto = new ErrorViewModel();
            Usuarios usuario;
            dto.TieneError = false;
            try
            {
                List<Bancos> listaBancos = new List<Bancos>();
                using (var dbContext = new ACHEEntities())
                {
                    usuario = dbContext.Usuarios.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
                    if (usuario.UsaPlanCorporativo && usuario.CondicionIva == "RI") //Plan Corporativo
                    {
                        if (usuario != null)
                        {
                            var planDeCta = dbContext.PlanDeCuentas.Any(x => x.IDUsuario == idUsuario);
                            if (!planDeCta)
                            {
                                dbContext.ConfigurarPlanCorporativo(idUsuario);
                                listaBancos = dbContext.Bancos.Where(x => x.IDUsuario == idUsuario).ToList();
                            }
                            else
                            {
                                dto.TieneError = true;
                                dto.Mensaje = "El usuario seleccionado ya tiene un plan de cuentas cargado.";
                            }
                        }
                        else
                        {
                            dto.TieneError = true;
                            dto.Mensaje = "IDUsuario incorrecto o invalido.";
                        }
                    }
                    else
                    {
                        dto.TieneError = true;
                        dto.Mensaje = "El usuario NO tiene el plan Corporativo activo.";
                    }
                }

                if (!dto.TieneError)
                {
                    ConfigurarBancos(idUsuario, listaBancos, usuario.CondicionIva);
                }
                return Json(dto, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dto.TieneError = true;
                dto.Mensaje = ex.Message;
                return Json(dto, JsonRequestBehavior.AllowGet);
            }
        }

        private void ConfigurarBancos(int idUsuario, List<Bancos> listaBancos, string condicionIVA)
        {
            var usu = TokenCommon.ObtenerWebUser(idUsuario);
            foreach (var item in listaBancos)
                ContabilidadCommon.CrearCuentaBancos(item.IDBanco, usu);
        }
    }
}