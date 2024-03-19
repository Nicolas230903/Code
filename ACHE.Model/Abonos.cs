
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
    
public partial class Abonos
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Abonos()
    {

        this.AbonosPersona = new HashSet<AbonosPersona>();

        this.ComprobantesDetalle = new HashSet<ComprobantesDetalle>();

    }


    public int IDAbono { get; set; }

    public int IDUsuario { get; set; }

    public string Nombre { get; set; }

    public string Frecuencia { get; set; }

    public System.DateTime FechaInicio { get; set; }

    public Nullable<System.DateTime> FechaFin { get; set; }

    public string Estado { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Iva { get; set; }

    public string Observaciones { get; set; }

    public int Tipo { get; set; }

    public Nullable<int> IDPlanDeCuenta { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }



    public virtual PlanDeCuentas PlanDeCuentas { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AbonosPersona> AbonosPersona { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComprobantesDetalle> ComprobantesDetalle { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}