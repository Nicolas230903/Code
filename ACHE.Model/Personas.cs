
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
    
public partial class Personas
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Personas()
    {

        this.AbonosPersona = new HashSet<AbonosPersona>();

        this.Activos = new HashSet<Activos>();

        this.AlertasGeneradas = new HashSet<AlertasGeneradas>();

        this.AuthenticationTokenClientes = new HashSet<AuthenticationTokenClientes>();

        this.Cobranzas = new HashSet<Cobranzas>();

        this.Compras = new HashSet<Compras>();

        this.Pagos = new HashSet<Pagos>();

        this.PersonaDomicilio = new HashSet<PersonaDomicilio>();

        this.Presupuestos = new HashSet<Presupuestos>();

        this.TrackingHoras = new HashSet<TrackingHoras>();

        this.TransportePersona = new HashSet<TransportePersona>();

        this.Conceptos = new HashSet<Conceptos>();

        this.Cheques = new HashSet<Cheques>();

        this.Comprobantes = new HashSet<Comprobantes>();

    }


    public int IDPersona { get; set; }

    public string Tipo { get; set; }

    public int IDUsuario { get; set; }

    public string RazonSocial { get; set; }

    public string TipoDocumento { get; set; }

    public string NroDocumento { get; set; }

    public string CondicionIva { get; set; }

    public string Telefono { get; set; }

    public string Celular { get; set; }

    public string Web { get; set; }

    public string Email { get; set; }

    public string Observaciones { get; set; }

    public int IDProvincia { get; set; }

    public int IDCiudad { get; set; }

    public string ProvinciaDesc { get; set; }

    public string CiudadDesc { get; set; }

    public string Domicilio { get; set; }

    public string PisoDepto { get; set; }

    public string CodigoPostal { get; set; }

    public string EmailsEnvioFc { get; set; }

    public string Personeria { get; set; }

    public string AlicuotaIvaDefecto { get; set; }

    public string TipoComprobanteDefecto { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public string CBU { get; set; }

    public string Banco { get; set; }

    public string NombreFantansia { get; set; }

    public Nullable<int> IDListaPrecio { get; set; }

    public string Contacto { get; set; }

    public string Foto { get; set; }

    public string Codigo { get; set; }

    public Nullable<decimal> SaldoInicial { get; set; }

    public decimal PorcentajeDescuento { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }

    public string Avisos { get; set; }

    public int IdRango { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AbonosPersona> AbonosPersona { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Activos> Activos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AlertasGeneradas> AlertasGeneradas { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AuthenticationTokenClientes> AuthenticationTokenClientes { get; set; }

    public virtual Ciudades Ciudades { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Cobranzas> Cobranzas { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Compras> Compras { get; set; }

    public virtual ListaPrecios ListaPrecios { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Pagos> Pagos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PersonaDomicilio> PersonaDomicilio { get; set; }

    public virtual Provincias Provincias { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Presupuestos> Presupuestos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TrackingHoras> TrackingHoras { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<TransportePersona> TransportePersona { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Conceptos> Conceptos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Cheques> Cheques { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Comprobantes> Comprobantes { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}
