﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace ACHE.Model
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using System.Data.Entity.Core.Objects;
using System.Linq;


public partial class ACHEEntities : DbContext
{
    public ACHEEntities()
        : base("name=ACHEEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<Abonos> Abonos { get; set; }

    public virtual DbSet<AbonosPersona> AbonosPersona { get; set; }

    public virtual DbSet<Activos> Activos { get; set; }

    public virtual DbSet<Alertas> Alertas { get; set; }

    public virtual DbSet<AlertasGeneradas> AlertasGeneradas { get; set; }

    public virtual DbSet<AsientoDetalle> AsientoDetalle { get; set; }

    public virtual DbSet<Asientos> Asientos { get; set; }

    public virtual DbSet<AuthenticationToken> AuthenticationToken { get; set; }

    public virtual DbSet<AuthenticationTokenClientes> AuthenticationTokenClientes { get; set; }

    public virtual DbSet<AvisosVencimiento> AvisosVencimiento { get; set; }

    public virtual DbSet<Bancos> Bancos { get; set; }

    public virtual DbSet<BancosBase> BancosBase { get; set; }

    public virtual DbSet<BancosPlanDeCuenta> BancosPlanDeCuenta { get; set; }

    public virtual DbSet<Caja> Caja { get; set; }

    public virtual DbSet<Categorias> Categorias { get; set; }

    public virtual DbSet<ChequeAccion> ChequeAccion { get; set; }

    public virtual DbSet<Ciudades> Ciudades { get; set; }

    public virtual DbSet<Cobranzas> Cobranzas { get; set; }

    public virtual DbSet<CobranzasDetalle> CobranzasDetalle { get; set; }

    public virtual DbSet<CobranzasFormasDePago> CobranzasFormasDePago { get; set; }

    public virtual DbSet<CobranzasRetenciones> CobranzasRetenciones { get; set; }

    public virtual DbSet<Compras> Compras { get; set; }

    public virtual DbSet<ComprobantesEnviados> ComprobantesEnviados { get; set; }

    public virtual DbSet<ComunicacionesAFIP> ComunicacionesAFIP { get; set; }

    public virtual DbSet<ComunicacionesAFIPAdjuntos> ComunicacionesAFIPAdjuntos { get; set; }

    public virtual DbSet<ConceptosCaja> ConceptosCaja { get; set; }

    public virtual DbSet<ConfiguracionPlanDeCuenta> ConfiguracionPlanDeCuenta { get; set; }

    public virtual DbSet<DatosAFIPPersonas> DatosAFIPPersonas { get; set; }

    public virtual DbSet<Empleados> Empleados { get; set; }

    public virtual DbSet<Estudios> Estudios { get; set; }

    public virtual DbSet<Formularios> Formularios { get; set; }

    public virtual DbSet<GastosBancarios> GastosBancarios { get; set; }

    public virtual DbSet<Jurisdicciones> Jurisdicciones { get; set; }

    public virtual DbSet<LicenciaTemp> LicenciaTemp { get; set; }

    public virtual DbSet<ListaPrecios> ListaPrecios { get; set; }

    public virtual DbSet<LogServicios> LogServicios { get; set; }

    public virtual DbSet<Message> Message { get; set; }

    public virtual DbSet<MovimientoDeFondos> MovimientoDeFondos { get; set; }

    public virtual DbSet<ObservacionesUsuario> ObservacionesUsuario { get; set; }

    public virtual DbSet<PagosFormasDePago> PagosFormasDePago { get; set; }

    public virtual DbSet<PagosRetenciones> PagosRetenciones { get; set; }

    public virtual DbSet<PersonaDomicilio> PersonaDomicilio { get; set; }

    public virtual DbSet<PersonasPwd> PersonasPwd { get; set; }

    public virtual DbSet<PersonasTemp> PersonasTemp { get; set; }

    public virtual DbSet<PlanDeCuentaBase> PlanDeCuentaBase { get; set; }

    public virtual DbSet<PlanDeCuentas> PlanDeCuentas { get; set; }

    public virtual DbSet<Planes> Planes { get; set; }

    public virtual DbSet<PlanesPagos> PlanesPagos { get; set; }

    public virtual DbSet<PreciosConceptos> PreciosConceptos { get; set; }

    public virtual DbSet<Presupuestos> Presupuestos { get; set; }

    public virtual DbSet<Provincias> Provincias { get; set; }

    public virtual DbSet<PuntosDeVenta> PuntosDeVenta { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<UsuariosPermitidos> UsuariosPermitidos { get; set; }

    public virtual DbSet<CiudadesPrueba> CiudadesPrueba { get; set; }

    public virtual DbSet<ConceptosTmp> ConceptosTmp { get; set; }

    public virtual DbSet<TicketsAfip> TicketsAfip { get; set; }

    public virtual DbSet<AvisosVencimientosView> AvisosVencimientosView { get; set; }

    public virtual DbSet<CajaView> CajaView { get; set; }

    public virtual DbSet<ObtenerCuentasDeCompras> ObtenerCuentasDeCompras { get; set; }

    public virtual DbSet<RptBancarioView> RptBancarioView { get; set; }

    public virtual DbSet<RptCobranzasPendientes> RptCobranzasPendientes { get; set; }

    public virtual DbSet<RptEstadoResultadoView> RptEstadoResultadoView { get; set; }

    public virtual DbSet<rptImpositivoLibroDiario> rptImpositivoLibroDiario { get; set; }

    public virtual DbSet<RptIvaCompras> RptIvaCompras { get; set; }

    public virtual DbSet<RptPagoProveedores> RptPagoProveedores { get; set; }

    public virtual DbSet<RptRankingClientes> RptRankingClientes { get; set; }

    public virtual DbSet<RptRetenciones> RptRetenciones { get; set; }

    public virtual DbSet<RptSaldosCc> RptSaldosCc { get; set; }

    public virtual DbSet<UsuariosPlanesView> UsuariosPlanesView { get; set; }

    public virtual DbSet<vCiudades> vCiudades { get; set; }

    public virtual DbSet<vLogServicios> vLogServicios { get; set; }

    public virtual DbSet<vMovimientoDeFondos> vMovimientoDeFondos { get; set; }

    public virtual DbSet<vPdvPendientesDeProcesar> vPdvPendientesDeProcesar { get; set; }

    public virtual DbSet<vPdvPendientesDeProcesarDetalle> vPdvPendientesDeProcesarDetalle { get; set; }

    public virtual DbSet<vPdvProcesados> vPdvProcesados { get; set; }

    public virtual DbSet<RptStock> RptStock { get; set; }

    public virtual DbSet<Pagos> Pagos { get; set; }

    public virtual DbSet<PagosDetalle> PagosDetalle { get; set; }

    public virtual DbSet<RptRankingConceptos> RptRankingConceptos { get; set; }

    public virtual DbSet<Transporte> Transporte { get; set; }

    public virtual DbSet<StockAuditoria> StockAuditoria { get; set; }

    public virtual DbSet<AuditoriaDeCambio> AuditoriaDeCambio { get; set; }

    public virtual DbSet<LiquidoProductoMargenes> LiquidoProductoMargenes { get; set; }

    public virtual DbSet<TransportePersona> TransportePersona { get; set; }

    public virtual DbSet<LoginUsuarios> LoginUsuarios { get; set; }

    public virtual DbSet<TrackingHoras> TrackingHoras { get; set; }

    public virtual DbSet<UsuariosAdicionales> UsuariosAdicionales { get; set; }

    public virtual DbSet<UsuariosEmpresa> UsuariosEmpresa { get; set; }

    public virtual DbSet<AccesoFormularioUsuario> AccesoFormularioUsuario { get; set; }

    public virtual DbSet<Personas> Personas { get; set; }

    public virtual DbSet<TipoIVA> TipoIVA { get; set; }

    public virtual DbSet<Conceptos> Conceptos { get; set; }

    public virtual DbSet<PresupuestoDetalle> PresupuestoDetalle { get; set; }

    public virtual DbSet<GastosGenerales> GastosGenerales { get; set; }

    public virtual DbSet<RemitoComprobanteMargenes> RemitoComprobanteMargenes { get; set; }

    public virtual DbSet<ComprobantesDetalle> ComprobantesDetalle { get; set; }

    public virtual DbSet<vCuadroResumen> vCuadroResumen { get; set; }

    public virtual DbSet<vCuadroResumenOriginal> vCuadroResumenOriginal { get; set; }

    public virtual DbSet<Actividad> Actividad { get; set; }

    public virtual DbSet<RptIvaVentas> RptIvaVentas { get; set; }

    public virtual DbSet<UsuariosView> UsuariosView { get; set; }

    public virtual DbSet<Cheques> Cheques { get; set; }

    public virtual DbSet<RptChequesAcciones> RptChequesAcciones { get; set; }

    public virtual DbSet<Comprobantes> Comprobantes { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Conceptos_Lote> Conceptos_Lote { get; set; }

    public virtual DbSet<ConceptosBackUp> ConceptosBackUp { get; set; }


    [DbFunction("ACHEEntities", "fn_Split")]
    public virtual IQueryable<fn_Split_Result> fn_Split(string @string, string delimiter)
    {

        var stringParameter = @string != null ?
            new ObjectParameter("String", @string) :
            new ObjectParameter("String", typeof(string));


        var delimiterParameter = delimiter != null ?
            new ObjectParameter("Delimiter", delimiter) :
            new ObjectParameter("Delimiter", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_Split_Result>("[ACHEEntities].[fn_Split](@String, @Delimiter)", stringParameter, delimiterParameter);
    }


    public virtual int ActualizarSaldosPorCobranza(Nullable<int> iDCobranza)
    {

        var iDCobranzaParameter = iDCobranza.HasValue ?
            new ObjectParameter("IDCobranza", iDCobranza) :
            new ObjectParameter("IDCobranza", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ActualizarSaldosPorCobranza", iDCobranzaParameter);
    }


    public virtual int ActualizarSaldosPorCompra(Nullable<int> iDCompra)
    {

        var iDCompraParameter = iDCompra.HasValue ?
            new ObjectParameter("IDCompra", iDCompra) :
            new ObjectParameter("IDCompra", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ActualizarSaldosPorCompra", iDCompraParameter);
    }


    public virtual int ActualizarSaldosPorComprobante(Nullable<int> iDComprobante)
    {

        var iDComprobanteParameter = iDComprobante.HasValue ?
            new ObjectParameter("IDComprobante", iDComprobante) :
            new ObjectParameter("IDComprobante", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ActualizarSaldosPorComprobante", iDComprobanteParameter);
    }


    public virtual int ActualizarSaldosPorPersona(Nullable<int> iDPersona)
    {

        var iDPersonaParameter = iDPersona.HasValue ?
            new ObjectParameter("IDPersona", iDPersona) :
            new ObjectParameter("IDPersona", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ActualizarSaldosPorPersona", iDPersonaParameter);
    }


    public virtual int BajaFisicaUsuarios(Nullable<int> iDUsuario)
    {

        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BajaFisicaUsuarios", iDUsuarioParameter);
    }


    public virtual int ConfigurarPlanCorporativo(Nullable<int> iDUsuario)
    {

        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ConfigurarPlanCorporativo", iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_Cobrado_Result> Dashboard_Cobrado(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_Cobrado_Result>("Dashboard_Cobrado", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_Compras_Result> Dashboard_Compras(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_Compras_Result>("Dashboard_Compras", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_ComprasPorCategoria_Result> Dashboard_ComprasPorCategoria(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_ComprasPorCategoria_Result>("Dashboard_ComprasPorCategoria", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_IvaCompras_Result> Dashboard_IvaCompras(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_IvaCompras_Result>("Dashboard_IvaCompras", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_IvaVentas_Result> Dashboard_IvaVentas(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_IvaVentas_Result>("Dashboard_IvaVentas", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_Pagado_Result> Dashboard_Pagado(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_Pagado_Result>("Dashboard_Pagado", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual ObjectResult<Dashboard_Ventas_Result> Dashboard_Ventas(Nullable<System.DateTime> fechaDesde, Nullable<System.DateTime> fechaHasta, Nullable<int> iDUsuario)
    {

        var fechaDesdeParameter = fechaDesde.HasValue ?
            new ObjectParameter("FechaDesde", fechaDesde) :
            new ObjectParameter("FechaDesde", typeof(System.DateTime));


        var fechaHastaParameter = fechaHasta.HasValue ?
            new ObjectParameter("FechaHasta", fechaHasta) :
            new ObjectParameter("FechaHasta", typeof(System.DateTime));


        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Dashboard_Ventas_Result>("Dashboard_Ventas", fechaDesdeParameter, fechaHastaParameter, iDUsuarioParameter);
    }


    public virtual int deleteConceptosTmp()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("deleteConceptosTmp");
    }


    public virtual int InsertarConfPlanesDeCuenta(Nullable<int> iDUsuario)
    {

        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertarConfPlanesDeCuenta", iDUsuarioParameter);
    }


    public virtual int InsertarHijosPlanesDeCuenta(Nullable<int> iDUsuario, string codigo)
    {

        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        var codigoParameter = codigo != null ?
            new ObjectParameter("codigo", codigo) :
            new ObjectParameter("codigo", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertarHijosPlanesDeCuenta", iDUsuarioParameter, codigoParameter);
    }


    public virtual int InsertarPadresPlanesDeCuenta(Nullable<int> iDUsuario)
    {

        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertarPadresPlanesDeCuenta", iDUsuarioParameter);
    }


    public virtual int InsertarPersonasFacturacion(Nullable<int> iDUsuario, string tipo, string razonSocial, string nombreFantasia, string email, string tipoDocumento, string nroDocumento, string condicionIva, Nullable<int> provincia, Nullable<int> ciudad, string domicilio, string web, string personeria)
    {

        var iDUsuarioParameter = iDUsuario.HasValue ?
            new ObjectParameter("IDUsuario", iDUsuario) :
            new ObjectParameter("IDUsuario", typeof(int));


        var tipoParameter = tipo != null ?
            new ObjectParameter("Tipo", tipo) :
            new ObjectParameter("Tipo", typeof(string));


        var razonSocialParameter = razonSocial != null ?
            new ObjectParameter("RazonSocial", razonSocial) :
            new ObjectParameter("RazonSocial", typeof(string));


        var nombreFantasiaParameter = nombreFantasia != null ?
            new ObjectParameter("NombreFantasia", nombreFantasia) :
            new ObjectParameter("NombreFantasia", typeof(string));


        var emailParameter = email != null ?
            new ObjectParameter("Email", email) :
            new ObjectParameter("Email", typeof(string));


        var tipoDocumentoParameter = tipoDocumento != null ?
            new ObjectParameter("TipoDocumento", tipoDocumento) :
            new ObjectParameter("TipoDocumento", typeof(string));


        var nroDocumentoParameter = nroDocumento != null ?
            new ObjectParameter("NroDocumento", nroDocumento) :
            new ObjectParameter("NroDocumento", typeof(string));


        var condicionIvaParameter = condicionIva != null ?
            new ObjectParameter("CondicionIva", condicionIva) :
            new ObjectParameter("CondicionIva", typeof(string));


        var provinciaParameter = provincia.HasValue ?
            new ObjectParameter("Provincia", provincia) :
            new ObjectParameter("Provincia", typeof(int));


        var ciudadParameter = ciudad.HasValue ?
            new ObjectParameter("Ciudad", ciudad) :
            new ObjectParameter("Ciudad", typeof(int));


        var domicilioParameter = domicilio != null ?
            new ObjectParameter("Domicilio", domicilio) :
            new ObjectParameter("Domicilio", typeof(string));


        var webParameter = web != null ?
            new ObjectParameter("Web", web) :
            new ObjectParameter("Web", typeof(string));


        var personeriaParameter = personeria != null ?
            new ObjectParameter("Personeria", personeria) :
            new ObjectParameter("Personeria", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertarPersonasFacturacion", iDUsuarioParameter, tipoParameter, razonSocialParameter, nombreFantasiaParameter, emailParameter, tipoDocumentoParameter, nroDocumentoParameter, condicionIvaParameter, provinciaParameter, ciudadParameter, domicilioParameter, webParameter, personeriaParameter);
    }


    public virtual int ObtenerDatosPersonasTmp()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ObtenerDatosPersonasTmp");
    }


    public virtual ObjectResult<PA_ConsultarTicketAfip_Result> PA_ConsultarTicketAfip(Nullable<decimal> cuit, string servicio, string modo)
    {

        var cuitParameter = cuit.HasValue ?
            new ObjectParameter("cuit", cuit) :
            new ObjectParameter("cuit", typeof(decimal));


        var servicioParameter = servicio != null ?
            new ObjectParameter("servicio", servicio) :
            new ObjectParameter("servicio", typeof(string));


        var modoParameter = modo != null ?
            new ObjectParameter("modo", modo) :
            new ObjectParameter("modo", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PA_ConsultarTicketAfip_Result>("PA_ConsultarTicketAfip", cuitParameter, servicioParameter, modoParameter);
    }


    public virtual int PA_EliminarTicketAfip(Nullable<decimal> cuit, string servicio, string modo)
    {

        var cuitParameter = cuit.HasValue ?
            new ObjectParameter("cuit", cuit) :
            new ObjectParameter("cuit", typeof(decimal));


        var servicioParameter = servicio != null ?
            new ObjectParameter("servicio", servicio) :
            new ObjectParameter("servicio", typeof(string));


        var modoParameter = modo != null ?
            new ObjectParameter("modo", modo) :
            new ObjectParameter("modo", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PA_EliminarTicketAfip", cuitParameter, servicioParameter, modoParameter);
    }


    public virtual int PA_EliminarUsuario(Nullable<int> idUsuario)
    {

        var idUsuarioParameter = idUsuario.HasValue ?
            new ObjectParameter("idUsuario", idUsuario) :
            new ObjectParameter("idUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PA_EliminarUsuario", idUsuarioParameter);
    }


    public virtual int PA_InsertarTicketAfip(Nullable<decimal> cuit, Nullable<System.DateTime> creado, string servicio, string firma, string token, string uniqueID, Nullable<System.DateTime> vencimiento, string modo)
    {

        var cuitParameter = cuit.HasValue ?
            new ObjectParameter("cuit", cuit) :
            new ObjectParameter("cuit", typeof(decimal));


        var creadoParameter = creado.HasValue ?
            new ObjectParameter("creado", creado) :
            new ObjectParameter("creado", typeof(System.DateTime));


        var servicioParameter = servicio != null ?
            new ObjectParameter("servicio", servicio) :
            new ObjectParameter("servicio", typeof(string));


        var firmaParameter = firma != null ?
            new ObjectParameter("firma", firma) :
            new ObjectParameter("firma", typeof(string));


        var tokenParameter = token != null ?
            new ObjectParameter("token", token) :
            new ObjectParameter("token", typeof(string));


        var uniqueIDParameter = uniqueID != null ?
            new ObjectParameter("uniqueID", uniqueID) :
            new ObjectParameter("uniqueID", typeof(string));


        var vencimientoParameter = vencimiento.HasValue ?
            new ObjectParameter("vencimiento", vencimiento) :
            new ObjectParameter("vencimiento", typeof(System.DateTime));


        var modoParameter = modo != null ?
            new ObjectParameter("modo", modo) :
            new ObjectParameter("modo", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PA_InsertarTicketAfip", cuitParameter, creadoParameter, servicioParameter, firmaParameter, tokenParameter, uniqueIDParameter, vencimientoParameter, modoParameter);
    }


    public virtual int ProcesarConceptos()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ProcesarConceptos");
    }


    public virtual int InsertarConceptos_BackUp_Vacio(Nullable<int> idUsuario)
    {

        var idUsuarioParameter = idUsuario.HasValue ?
            new ObjectParameter("IdUsuario", idUsuario) :
            new ObjectParameter("IdUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertarConceptos_BackUp_Vacio", idUsuarioParameter);
    }


    public virtual ObjectResult<Obtener_Registros_lotes_Result> Obtener_Registros_lotes(Nullable<int> idUsuario)
    {

        var idUsuarioParameter = idUsuario.HasValue ?
            new ObjectParameter("IdUsuario", idUsuario) :
            new ObjectParameter("IdUsuario", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Obtener_Registros_lotes_Result>("Obtener_Registros_lotes", idUsuarioParameter);
    }

}

}

