
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
    
public partial class Caja
{

    public int IDCaja { get; set; }

    public int IDUsuario { get; set; }

    public string TipoMovimiento { get; set; }

    public string Concepto { get; set; }

    public decimal Importe { get; set; }

    public string Observaciones { get; set; }

    public Nullable<System.DateTime> Fecha { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public string Estado { get; set; }

    public Nullable<System.DateTime> FechaAnulacion { get; set; }

    public string MedioDePago { get; set; }

    public string Ticket { get; set; }

    public Nullable<int> IDConceptosCaja { get; set; }

    public string Foto { get; set; }

    public Nullable<int> IDPlanDeCuenta { get; set; }

    public System.DateTime EstadoFecha { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }



    public virtual ConceptosCaja ConceptosCaja { get; set; }

    public virtual PlanDeCuentas PlanDeCuentas { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}
