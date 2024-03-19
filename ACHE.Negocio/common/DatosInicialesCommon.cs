using ACHE.Model;
using ACHE.Model.Negocio;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Abono;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Presupuesto;
using ACHE.Negocio.Productos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ACHE.Negocio.Common
{
    public class DatosInicialesCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static int CrearDatosClientes(WebUser usu)
        {
            return PersonasCommon.GuardarPersonas(0, "Cliente Demo", "", "CF", "F", "", "", "4251108", "correo@correo.com", "C", 2, 5003,"CATAMARCA", "CHAVARRIA", "algun lugar", "7", "1414", "alguna observación", 0, "101", 0, 0,"",0, usu);
        }
        public static int CrearDatosClientesGenerico(WebUser usu)
        {
            return PersonasCommon.GuardarPersonas(0, "Consumidor final", "", "CF", "F", "CUIT", "20111111112", "", "consumidorFinal@correo.com", "C", 2, 5003, "CATAMARCA", "CHAVARRIA", "algun lugar", "7", "1414", "cliente generico", 0, "998", 0, 0, "",0, usu);
        }
        public static int CrearDatosProveedores(WebUser usu)
        {
            return PersonasCommon.GuardarPersonas(0, "Proveedor Demo", "Nombre de fantasia del Proveedor", "MO", "F", "DNI", "12345678", "4251108", "correo@correo.com", "P", 2, 5003, "CATAMARCA", "CHAVARRIA", "algun lugar", "7", "1414", "alguna observación", 0, "1001", 0, 0, "",0, usu);
        }
        public static int CrearDatosProveedoresGenerico(WebUser usu)
        {
            return PersonasCommon.GuardarPersonas(0, "Proveedor Genérico", "Proveedor Genérico", "MO", "F", "DNI", "99999999", "4251108", "proveedorgenerico@correo.com", "P", 2, 5003, "CATAMARCA", "CHAVARRIA", "algun lugar", "7", "1414", "alguna observación", 0, "9999", 0, 0, "",0, usu);
        }
        public static void CrearDatosConceptos(WebUser usu)
        {
            var idConcepto = ConceptosCommon.GuardarConcepto(0, "Producto DEMO", "1", "P", "Alguna descripción", "A", "100", "5", "100", "Alguna observación del producto", "10", "10", 0, usu.IDUsuario);
        }
        public static void CrearDatosPresupuestos(WebUser usu, int idPersona)
        {
            PresupuestoCartDto comprobanteCart = new PresupuestoCartDto();
            comprobanteCart.Items = new List<ComprobantesDetalleViewModel>();
            comprobanteCart.Items.Add(new ComprobantesDetalleViewModel()
            {
                ID = 1,
                Cantidad = 1,
                Concepto = "Concepto DEMO ",
                PrecioUnitario = 100,
                Bonificacion = 0,
                Iva = 21
            });

            comprobanteCart.IDPresupuesto = 0;
            comprobanteCart.IDPersona = idPersona;
            comprobanteCart.Fecha = DateTime.Now.Date.ToString(formatoFecha);
            comprobanteCart.Nombre = "presupuesto demo";
            comprobanteCart.Numero = 2;
            comprobanteCart.CondicionesPago = "A convenir";
            comprobanteCart.Observaciones = "Alguna observacion";
            comprobanteCart.Estado = "B";

            var idPresupuesto = PresupuestosCommon.GuardarPresupuesto(comprobanteCart, usu);
        }
        public static Compras CrearDatosCompras(WebUser usu, int idPersona)
        {
            var idCuenta = 0;
            if (usu.UsaPlanCorporativo)
            {
                using (var dbContext = new ACHEEntities())
                    idCuenta = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario && x.Codigo == "1.4.1").FirstOrDefault().IDPlanDeCuenta;
            }

            List<JurisdiccionesViewModel> Jurisdicciones = new List<JurisdiccionesViewModel>();
            return ComprasCommon.Guardar(0, idPersona, DateTime.Now.Date.ToString(formatoFecha), "0001-99999999", "0", "0", "0", "0", "0", "0", "0", "100", "0", "0", "0", "0", "0",
                                        "Alguna observación de la compra", "FCC", "", "", "0", DateTime.Now.Date.ToString(formatoFecha), idCuenta, usu.IDUsuario, Jurisdicciones,
                                        DateTime.Now.AddDays(15).Date.ToString(formatoFecha), DateTime.Now.AddDays(30).Date.ToString(formatoFecha),null);

        }
        public static int CrearDatosPagos(WebUser usu, Compras compra)
        {
            PagosCartDto PagosCart = new PagosCartDto();
            PagosCart.IDPago = 0;
            PagosCart.IDPersona = compra.IDPersona;
            PagosCart.Observaciones = "Alguna observación del pago";
            PagosCart.FechaPago = DateTime.Now.Date.ToString(formatoFecha);

            PagosCart.Items = new List<PagosDetalleViewModel>();
            PagosCart.FormasDePago = new List<PagosFormasDePagoViewModel>();
            PagosCart.Retenciones = new List<PagosRetencionesViewModel>();

            PagosCart.Items.Add(new PagosDetalleViewModel()
            {
                ID = 1,
                IDCompra = compra.IDCompra,
                Importe = Convert.ToDecimal(compra.Total),
                nroFactura = compra.NroFactura
            });

            PagosCart.FormasDePago.Add(new PagosFormasDePagoViewModel()
            {
                ID = 1,
                Importe = Convert.ToDecimal(compra.Total),
                NroReferencia = "",
                FormaDePago = "Efectivo"
            });
            var pago = PagosCommon.Guardar(PagosCart, usu);
            return pago.IDPago;
        }
        public static Comprobantes CrearDatosVentas(WebUser usu, int idPersona)
        {
            var idCuenta = 0;
            if (usu.UsaPlanCorporativo)
            {
                using (var dbContext = new ACHEEntities())
                    idCuenta = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario && x.Codigo == "4.1.1").FirstOrDefault().IDPlanDeCuenta;
            }

            ComprobanteCartDto compCart = new ComprobanteCartDto();
            compCart.Items = new List<ComprobantesDetalleViewModel>();
            compCart.Items.Add(new ComprobantesDetalleViewModel()
            {
                ID = 0,
                IDConcepto = null,
                Codigo = "",
                Concepto = "Servicios de venta DEMO",
                Bonificacion = 0,
                Cantidad = 1,
                Iva = (usu.CondicionIVA == "RI") ? 21 : 0,
                PrecioUnitario = 100,
                IDPlanDeCuenta = idCuenta
            });

            compCart.IDComprobante = 0;
            compCart.TipoComprobante = (usu.CondicionIVA == "RI") ? "FCB" : "FCC";
            compCart.Numero = "00000001";
            compCart.Modo = "T";
            compCart.FechaComprobante = DateTime.Now.Date;
            compCart.CondicionVenta = "Efectivo";
            compCart.TipoConcepto = "2";
            compCart.FechaVencimiento = DateTime.Now.AddDays(30).Date;
            compCart.IDPuntoVenta = UsuarioCommon.ObtenerPuntosDeVenta(usu.IDUsuario).FirstOrDefault().ID;
            compCart.IDActividad = UsuarioCommon.ObtenerActividades(usu.IDUsuario).FirstOrDefault().ID;
            compCart.Observaciones = "Alguna observación de la factura";

            compCart.IDPersona = idPersona;
            compCart.IDUsuario = usu.IDUsuario;

            return ComprobantesCommon.GuardarComprobante(compCart);
        }       


        public static int CrearDatosCobranzas(WebUser usu, Comprobantes comprobante)
        {
            CobranzaCartDto cobrCartdto = new CobranzaCartDto();
            cobrCartdto.IDCobranza = 0;
            cobrCartdto.IDPersona = comprobante.IDPersona;
            cobrCartdto.Tipo = "RC";
            cobrCartdto.Fecha = DateTime.Now.Date.ToString(formatoFecha);
            cobrCartdto.IDPuntoVenta = comprobante.IDPuntoVenta;
            cobrCartdto.NumeroCobranza = "00000001";
            cobrCartdto.Observaciones = "Alguna observación de la cobranza";

            cobrCartdto.Items = new List<CobranzasDetalleViewModel>();
            cobrCartdto.Retenciones = new List<CobranzasRetencionesViewModel>();
            cobrCartdto.FormasDePago = new List<CobranzasFormasDePagoViewModel>();

            cobrCartdto.Items.Add(new CobranzasDetalleViewModel()
            {
                ID = 1,
                Comprobante = comprobante.Tipo + " 0001-" + comprobante.Numero.ToString("#00000000"),
                Importe = comprobante.ImporteTotalNeto,
                IDComprobante = comprobante.IDComprobante,
            });
            cobrCartdto.FormasDePago.Add(new CobranzasFormasDePagoViewModel()
            {
                ID = 1,
                Importe = comprobante.ImporteTotalNeto,
                FormaDePago = "Efectivo",
                NroReferencia = "",
            });

            var cobranza = CobranzasCommon.Guardar(cobrCartdto, usu);
            return cobranza.IDCobranza;
        }
        public static void CrearDatosAbonos(WebUser usu, List<AbonosPersonasViewModel> personas)
        {
            AbonosCommon.GuardarAbono(0, "Abono por servicios prestados DEMO", "M", DateTime.Now.Date.ToString(formatoFecha), DateTime.Now.AddDays(30).Date.ToString(formatoFecha),
                                     "A", "100", "21,00", "Alguna observación del abono", personas, 1, usu, 0);
        }
    }
}