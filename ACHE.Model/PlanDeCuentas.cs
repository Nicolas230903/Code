
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
    
public partial class PlanDeCuentas
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public PlanDeCuentas()
    {

        this.Abonos = new HashSet<Abonos>();

        this.AsientoDetalle = new HashSet<AsientoDetalle>();

        this.BancosPlanDeCuenta = new HashSet<BancosPlanDeCuenta>();

        this.Caja = new HashSet<Caja>();

        this.PlanDeCuentas1 = new HashSet<PlanDeCuentas>();

        this.ComprobantesDetalle = new HashSet<ComprobantesDetalle>();

    }


    public int IDPlanDeCuenta { get; set; }

    public int IDUsuario { get; set; }

    public string Nombre { get; set; }

    public string Codigo { get; set; }

    public Nullable<int> IDPadre { get; set; }

    public bool AdminiteAsientoManual { get; set; }

    public string TipoDeCuenta { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Abonos> Abonos { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AsientoDetalle> AsientoDetalle { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<BancosPlanDeCuenta> BancosPlanDeCuenta { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Caja> Caja { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PlanDeCuentas> PlanDeCuentas1 { get; set; }

    public virtual PlanDeCuentas PlanDeCuentas2 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComprobantesDetalle> ComprobantesDetalle { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}