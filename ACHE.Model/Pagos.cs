
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
    
public partial class Pagos
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Pagos()
    {

        this.AlertasGeneradas = new HashSet<AlertasGeneradas>();

        this.Asientos = new HashSet<Asientos>();

        this.PagosFormasDePago = new HashSet<PagosFormasDePago>();

        this.PagosRetenciones = new HashSet<PagosRetenciones>();

        this.PagosDetalle = new HashSet<PagosDetalle>();

    }


    public int IDPago { get; set; }

    public int IDUsuario { get; set; }

    public int IDPersona { get; set; }

    public System.DateTime FechaPago { get; set; }

    public string Observaciones { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public string NroReferencia { get; set; }

    public decimal ImporteTotal { get; set; }

    public decimal ImporteNeto { get; set; }

    public string EstadoCaja { get; set; }

    public System.DateTime EstadoCajaFecha { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AlertasGeneradas> AlertasGeneradas { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Asientos> Asientos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PagosFormasDePago> PagosFormasDePago { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PagosRetenciones> PagosRetenciones { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PagosDetalle> PagosDetalle { get; set; }

    public virtual Personas Personas { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}
