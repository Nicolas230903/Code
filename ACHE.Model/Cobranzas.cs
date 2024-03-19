
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
    
public partial class Cobranzas
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Cobranzas()
    {

        this.AlertasGeneradas = new HashSet<AlertasGeneradas>();

        this.Asientos = new HashSet<Asientos>();

        this.CobranzasDetalle = new HashSet<CobranzasDetalle>();

        this.CobranzasFormasDePago = new HashSet<CobranzasFormasDePago>();

        this.CobranzasRetenciones = new HashSet<CobranzasRetenciones>();

        this.ComprobantesEnviados = new HashSet<ComprobantesEnviados>();

    }


    public int IDCobranza { get; set; }

    public int IDUsuario { get; set; }

    public int IDPuntoVenta { get; set; }

    public int IDPersona { get; set; }

    public string TipoDestinatario { get; set; }

    public string TipoDocumento { get; set; }

    public string NroDocumento { get; set; }

    public string Modo { get; set; }

    public string Tipo { get; set; }

    public System.DateTime FechaCobranza { get; set; }

    public decimal ImporteTotal { get; set; }

    public int Numero { get; set; }

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

    public string EstadoCaja { get; set; }

    public System.DateTime EstadoCajaFecha { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AlertasGeneradas> AlertasGeneradas { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Asientos> Asientos { get; set; }

    public virtual PuntosDeVenta PuntosDeVenta { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CobranzasDetalle> CobranzasDetalle { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CobranzasFormasDePago> CobranzasFormasDePago { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CobranzasRetenciones> CobranzasRetenciones { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComprobantesEnviados> ComprobantesEnviados { get; set; }

    public virtual Personas Personas { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}
