
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
    using System.Collections.Generic;
    
public partial class Comprobantes
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Comprobantes()
    {

        this.Asientos = new HashSet<Asientos>();

        this.CobranzasDetalle = new HashSet<CobranzasDetalle>();

        this.CobranzasFormasDePago = new HashSet<CobranzasFormasDePago>();

        this.ComprobantesEnviados = new HashSet<ComprobantesEnviados>();

        this.PlanesPagos = new HashSet<PlanesPagos>();

        this.ComprobantesDetalle = new HashSet<ComprobantesDetalle>();

    }


    public int IDComprobante { get; set; }

    public int IDUsuario { get; set; }

    public int IDPuntoVenta { get; set; }

    public int IDPersona { get; set; }

    public string TipoDestinatario { get; set; }

    public string TipoDocumento { get; set; }

    public string NroDocumento { get; set; }

    public string Modo { get; set; }

    public string Tipo { get; set; }

    public System.DateTime FechaComprobante { get; set; }

    public System.DateTime FechaVencimiento { get; set; }

    public decimal ImporteTotalNeto { get; set; }

    public decimal ImporteTotalBruto { get; set; }

    public string CondicionVenta { get; set; }

    public int Numero { get; set; }

    public int TipoConcepto { get; set; }

    public string CAE { get; set; }

    public Nullable<System.DateTime> FechaCAE { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public string Error { get; set; }

    public Nullable<System.DateTime> FechaProceso { get; set; }

    public Nullable<System.DateTime> FechaError { get; set; }

    public bool Enviada { get; set; }

    public Nullable<System.DateTime> FechaEnvio { get; set; }

    public string EnvioError { get; set; }

    public Nullable<System.DateTime> FechaRecibida { get; set; }

    public string Observaciones { get; set; }

    public decimal ImporteNoGravado { get; set; }

    public decimal PercepcionIVA { get; set; }

    public decimal PercepcionIIBB { get; set; }

    public decimal Saldo { get; set; }

    public Nullable<int> IDJurisdiccion { get; set; }

    public byte[] Adjunto { get; set; }

    public string Nombre { get; set; }

    public Nullable<int> IdComprobanteVinculado { get; set; }

    public string Vendedor { get; set; }

    public Nullable<int> IdPresupuestoVinculado { get; set; }

    public string Envio { get; set; }

    public Nullable<System.DateTime> FechaEntrega { get; set; }

    public Nullable<long> ProcesoCompraAutomatica { get; set; }

    public Nullable<int> IdComprobanteAsociado { get; set; }

    public Nullable<long> ProcesoFacturaAutomatica { get; set; }

    public Nullable<int> IdDomicilio { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }

    public string Estado { get; set; }

    public Nullable<int> IdTransporte { get; set; }

    public Nullable<int> IdTransportePersona { get; set; }

    public Nullable<int> IdUsuarioAdicional { get; set; }

    public Nullable<decimal> ImporteComisionVendedor { get; set; }

    public decimal ImporteExento { get; set; }

    public Nullable<int> IdCompraVinculada { get; set; }

    public decimal Descuento { get; set; }

    public Nullable<int> NumeroOriginal { get; set; }

    public int IdActividad { get; set; }

    public string ModalidadPagoAFIP { get; set; }

    public string CBU { get; set; }



    public virtual Actividad Actividad { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Asientos> Asientos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CobranzasDetalle> CobranzasDetalle { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CobranzasFormasDePago> CobranzasFormasDePago { get; set; }

    public virtual Personas Personas { get; set; }

    public virtual Provincias Provincias { get; set; }

    public virtual PuntosDeVenta PuntosDeVenta { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComprobantesEnviados> ComprobantesEnviados { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PlanesPagos> PlanesPagos { get; set; }

    public virtual PersonaDomicilio PersonaDomicilio { get; set; }

    public virtual Transporte Transporte { get; set; }

    public virtual TransportePersona TransportePersona { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComprobantesDetalle> ComprobantesDetalle { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}