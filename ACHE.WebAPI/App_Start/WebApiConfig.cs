using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ACHE.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Authentication",
                routeTemplate: "{controller}/{apiKey}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    name: "ObtenerToken",
            //    routeTemplate: "Authentication/ObtenerToken/{apiKey}/{email}",
            //    defaults: new { controller = "Authentication", action = "ObtenerToken" }
            //);

            config.Routes.MapHttpRoute(
                name: "ObtenerToken",
                routeTemplate: "Authentication/ObtenerToken/{email}/{pwd}",
                defaults: new { controller = "Authentication", action = "ObtenerToken" }
            );

            #region BANCOS
            config.Routes.MapHttpRoute(
                name: "BancosObtener",
                routeTemplate: "Bancos/obtener/{token}/{filtro}/{page}/{pageSize}",
                defaults: new { controller = "Bancos", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "BancosProcesar",
                routeTemplate: "Bancos/procesar/{token}/{id}/{idBancoBase}/{nroCuenta}/{moneda}/{activo}/{saldoInicial}/{ejecutivo}/{direccion}/{telefono}/{email}/{observacion}",
                defaults: new { controller = "Bancos", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "BancosEliminar",
                routeTemplate: "Bancos/eliminar/{token}/{id}",
                defaults: new { controller = "Bancos", action = "eliminar" }
            );
            #endregion
            #region CONCEPTOS
            config.Routes.MapHttpRoute(
                name: "ConceptosObtener",
                routeTemplate: "Conceptos/obtener/{token}/{filtro}/{page}/{pageSize}",
                defaults: new { controller = "Conceptos", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ConceptosProcesar",
                routeTemplate: "Conceptos/procesar/{token}/{id}/{nombre}/{codigo}/{tipo}/{descripcion}/{estado}/{precio}/{iva}/{stock}/{obs}/{constoInterno}/{stockMinimo}/{idPersona}",
                defaults: new { controller = "Conceptos", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ConceptosEliminar",
                routeTemplate: "Conceptos/eliminar/{token}/{id}",
                defaults: new { controller = "Conceptos", action = "eliminar" }
            );
            #endregion
            #region PRODUCTOS_TIENDANUBE
            //config.Routes.MapHttpRoute(
            //    name: "ProductosObtener",
            //    routeTemplate: "ProductoTiendaNube/obtener/{token}/{filtro}/{page}/{pageSize}",
            //    defaults: new { controller = "ProductoTiendaNube", action = "obtener" }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "ProductosProcesar",
            //    routeTemplate: "ProductoTiendaNube/procesar/{productoTiendaNube}",
            //    defaults: new { controller = "ProductoTiendaNube", action = "procesar" }
            //);

            config.Routes.MapHttpRoute(
                name: "CrearProducto",
                routeTemplate: "ProductoTiendaNube/CrearProducto/{productoTiendaNube}",
                defaults: new { controller = "ProductoTiendaNube", action = "CrearProducto" }
            );
            config.Routes.MapHttpRoute(
                name: "EliminarProducto",
                routeTemplate: "ProductoTiendaNube/EliminarProducto/{token}/{idProducto}/{idVariante}",
                defaults: new { controller = "ProductoTiendaNube", action = "EliminarProducto" }
            );
            config.Routes.MapHttpRoute(
                name: "ModificarProducto",
                routeTemplate: "ProductoTiendaNube/ModificarProducto/{productoTiendaNube}",
                defaults: new { controller = "ProductoTiendaNube", action = "ModificarProducto" }
            );
            config.Routes.MapHttpRoute(
                name: "ConsultarProducto",
                routeTemplate: "ProductoTiendaNube/ConsultarProducto/{token}/{idProducto}/{idVariante}",
                defaults: new { controller = "ProductoTiendaNube", action = "ConsultarProducto" }
            );

            #endregion

            #region ORDENES_TIENDANUBE
            config.Routes.MapHttpRoute(
                name: "ObtenerOrdenes",
                routeTemplate: "OrdenTiendaNube/ObtenerOrdenes/{token}/{desdeIdOrden}/{hastaIdOrden}/{estado}",
                defaults: new { controller = "OrdenTiendaNube", action = "ObtenerOrdenes" }
            );
            config.Routes.MapHttpRoute(
                name: "ModificarEstadoOrden",
                routeTemplate: "OrdenTiendaNube/ModificarEstado/{token}/{idOrden}/{estado}/{motivo}",
                defaults: new { controller = "OrdenTiendaNube", action = "ModificarEstado" }
            );
            #endregion

            #region LICENCIA
            config.Routes.MapHttpRoute(
                name: "RegistrarEquipo",
                routeTemplate: "Licencia/RegistrarEquipo/{PostRequestLicencia}",
                defaults: new { controller = "Licencia", action = "RegistrarEquipo" }
            );
            #endregion

            #region AFIP_COMUNICACIONES
            config.Routes.MapHttpRoute(
                name: "ObtenerComunicacionesAfip",
                routeTemplate: "ComunicacionesAfip/ObtenerComunicacionesAfip/{token}/{cuit}/{desdeIdComunicacion}/{hastaIdComunicacion}/{desdeFecha}/{hastaFecha}/{adjunto}",
                defaults: new { controller = "ComunicacionesAfip", action = "ObtenerComunicacionesAfip" }
            );
            #endregion

            #region AFIP_DATOS_PERSONAS
            config.Routes.MapHttpRoute(
                name: "DatosPersonaAfip",
                routeTemplate: "DatosPersonaAfip/DatosPersonaAfip/{token}/{cuit}",
                defaults: new { controller = "DatosPersonaAfip", action = "DatosPersonaAfip" }
            );
            #endregion

            #region CLIENTES
            config.Routes.MapHttpRoute(
                name: "ClientesObtener",
                routeTemplate: "Clientes/obtener/{token}/{filtro}/{page}/{pageSize}",
                defaults: new { controller = "Clientes", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ClientesProcesar",
                routeTemplate: "Clientes/procesar/{token}/{id}/{razonSocial}/{nombreFantasia}/{condicionIva}/{personeria}/{tipoDoc}/{nroDoc}/{telefono}/{email}/{idProvincia}/{idCiudad}/{domicilio}/{pisoDepto}/{cp}/{obs}/{listaPrecio}/{codigo}/{saldoInicial}",
                defaults: new { controller = "Clientes", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ClientesEliminar",
                routeTemplate: "Clientes/eliminar/{token}/{id}",
                defaults: new { controller = "Clientes", action = "eliminar" }
            );
            #endregion
            #region PROVEEDORES
            config.Routes.MapHttpRoute(
              name: "ProveedoresObtener",
              routeTemplate: "Proveedores/obtener/{token}/{filtro}/{page}/{pageSize}",
              defaults: new { controller = "Proveedores", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ProveedoresProcesar",
                routeTemplate: "Proveedores/procesar/{token}/{id}/{razonSocial}/{nombreFantasia}/{condicionIva}/{personeria}/{tipoDoc}/{nroDoc}/{telefono}/{email}/{idProvincia}/{idCiudad}/{domicilio}/{pisoDepto}/{cp}/{obs}/{listaPrecio}/{codigo}/{saldoInicial}",
                defaults: new { controller = "Proveedores", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ProveedoresEliminar",
                routeTemplate: "Proveedores/eliminar/{token}/{id}",
                defaults: new { controller = "Proveedores", action = "eliminar" }
            );
            #endregion
            #region LISTA DE PRECIOS
            config.Routes.MapHttpRoute(
              name: "ListaDePreciosObtener",
              routeTemplate: "ListaDePrecios/obtener/{token}/{nombre}/{page}/{pageSize}",
              defaults: new { controller = "ListaDePrecios", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ListaDePreciosProcesar",
                routeTemplate: "ListaDePrecios/procesar/{listaDePrecios}",
                defaults: new { controller = "ListaDePrecios", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ListaDePreciosEliminar",
                routeTemplate: "ListaDePrecios/eliminar/{token}/{id}",
                defaults: new { controller = "ListaDePrecios", action = "eliminar" }
            );
            // config.Routes.MapHttpRoute(
            //    name: "ListaDePrecioObtenerItemsListaDePrecio",
            //    routeTemplate: "ListaDePrecios/obtenerItemsListaDePrecio/{token}/{id}",
            //    defaults: new { controller = "ListaDePrecios", action = "obtenerItemsListaDePrecio" }
            //);
            #endregion
            #region COMPROBANTES
            config.Routes.MapHttpRoute(
                name: "ComprobanteObtener",
                routeTemplate: "Comprobante/obtener/{token}/{condicion}/{periodo}/{fechaDesde}/{fechaHasta}/{page}/{pageSize}",
                defaults: new { controller = "Comprobante", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ComprobanteProcesar",
                routeTemplate: "Comprobante/procesar/{comprobante}",
                defaults: new { controller = "Comprobante", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ComprobanteEliminar",
                routeTemplate: "Comprobante/eliminar/{token}/{id}",
                defaults: new { controller = "Comprobante", action = "eliminar" }
            );
            #endregion
            #region COBRANZAS
            config.Routes.MapHttpRoute(
                name: "CobranzasObtener",
                routeTemplate: "Cobranzas/obtener/{token}/{condicion}/{periodo}/{fechaDesde}/{fechaHasta}/{page}/{pageSize}",
                defaults: new { controller = "Cobranzas", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "CobranzasProcesar",
                routeTemplate: "Cobranzas/procesar/{cobranza}",
                defaults: new { controller = "Cobranzas", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "CobranzasEliminar",
                routeTemplate: "Cobranzas/eliminar/{token}/{id}",
                defaults: new { controller = "Cobranzas", action = "eliminar" }
            );
            #endregion
            #region COMPRAS
            config.Routes.MapHttpRoute(
                name: "ComprasObtener",
                routeTemplate: "Compras/obtener/{token}/{condicion}/{periodo}/{fechaDesde}/{fechaHasta}/{page}/{pageSize}",
                defaults: new { controller = "Compras", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ComprasProcesar",
                routeTemplate: "Compras/procesar/{compra}",
                defaults: new { controller = "Compras", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ComprasEliminar",
                routeTemplate: "Compras/eliminar/{token}/{id}",
                defaults: new { controller = "Compras", action = "eliminar" }
            );
            #endregion
            #region PAGOS
            config.Routes.MapHttpRoute(
                name: "PagosObtener",
                routeTemplate: "Pagos/obtener/{token}/{condicion}/{periodo}/{fechaDesde}/{fechaHasta}/{page}/{pageSize}",
                defaults: new { controller = "Pagos", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "PagosProcesar",
                routeTemplate: "Pagos/procesar/{Pagos}",
                defaults: new { controller = "Pagos", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "PagosEliminar",
                routeTemplate: "Pagos/eliminar/{token}/{id}",
                defaults: new { controller = "Pagos", action = "eliminar" }
            );
            #endregion
            #region PRESUPUESTO
            config.Routes.MapHttpRoute(
                name: "PresupuestoObtener",
                routeTemplate: "Presupuesto/obtener/{token}/{condicion}/{periodo}/{fechaDesde}/{fechaHasta}/{page}/{pageSize}",
                defaults: new { controller = "Presupuesto", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "PresupuestoProcesar",
                routeTemplate: "Presupuesto/procesar/{Presupuesto}",
                defaults: new { controller = "Presupuesto", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "PresupuestoEliminar",
                routeTemplate: "Presupuesto/eliminar/{token}/{id}",
                defaults: new { controller = "Presupuesto", action = "eliminar" }
            );
            #endregion
            #region ConfiguracionUsuario
            config.Routes.MapHttpRoute(
                name: "ConfiguracionObtener",
                routeTemplate: "Configuracion/obtener/{token}",
                defaults: new { controller = "Configuracion", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "ConfiguracionProcesar",
                routeTemplate: "Configuracion/procesar/{token}/{razonSocial}/{condicionIva}/{cuit}/{iibb}/{fechaInicio}/{personeria}/{emailAlertas}/{telefono}/{celular}/{contacto}/{idProvincia}/{idCiudad}/{domicilio}/{pisoDepto}/{cp}/{esAgentePersepcionIVA}/{esAgentePersepcionIIBB}/{esAgenteRetencion}/{exentoIIBB}/{fechaCierreContable}/{idJurisdiccion}",
                defaults: new { controller = "Configuracion", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "ConfiguracioncambiarPassword",
                routeTemplate: "Configuracion/cambiarPassword/{token}/{passwordActual}/{PasswordNuevo}/{Passwordverificado}",
                defaults: new { controller = "Configuracion", action = "cambiarPassword" }
            );
            #endregion
            #region PUNTODEVENTA
            config.Routes.MapHttpRoute(
                name: "PuntoDeVentaObtener",
                routeTemplate: "PuntoDeVenta/obtener/{token}",
                defaults: new { controller = "PuntoDeVenta", action = "obtener" }
            );

            config.Routes.MapHttpRoute(
                name: "PuntoDeVentaProcesar",
                routeTemplate: "PuntoDeVenta/procesar/{token}/{punto}",
                defaults: new { controller = "PuntoDeVenta", action = "procesar" }
            );

            config.Routes.MapHttpRoute(
                name: "PuntoDeVentaEliminar",
                routeTemplate: "PuntoDeVenta/eliminar/{token}/{idPuntoDeVenta}",
                defaults: new { controller = "PuntoDeVenta", action = "eliminar" }
            );
            #endregion
            #region COMMON
            config.Routes.MapHttpRoute(
                name: "CommonObtenerProvincias",
                routeTemplate: "Common/ObtenerProvincias/{token}",
                defaults: new { controller = "Common", action = "ObtenerProvincias" }
            );
            config.Routes.MapHttpRoute(
                name: "CommonobtenerCiudades",
                routeTemplate: "Common/obtenerCiudades/{token}/{idProvincia}",
                defaults: new { controller = "Common", action = "obtenerCiudades" }
            );
            config.Routes.MapHttpRoute(
                name: "CommonobtenerCiudadesPaginadas",
                routeTemplate: "Common/obtenerCiudadesPaginadas/{token}/{page}/{pageSize}",
                defaults: new { controller = "Common", action = "obtenerCiudadesPaginadas" }
            );
            #endregion

            config.Routes.MapHttpRoute(
                name: "Request",
                routeTemplate: "{controller}/{action}/{token}/{idComprobante}/{idPersona}",
                defaults: new { token = RouteParameter.Optional, idComprobante = RouteParameter.Optional, idPersona = RouteParameter.Optional }
           );




        }
    }
}