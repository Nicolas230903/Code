
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
    
public partial class ComprobantesDetalle
{

    public int IDDetalle { get; set; }

    public int IDComprobante { get; set; }

    public decimal Cantidad { get; set; }

    public string Concepto { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal PrecioUnitarioIVA { get; set; }

    public decimal PrecioTotalIVA { get; set; }

    public decimal Iva { get; set; }

    public decimal Bonificacion { get; set; }

    public Nullable<int> IDConcepto { get; set; }

    public Nullable<int> IDAbono { get; set; }

    public Nullable<int> IDPlanDeCuenta { get; set; }

    public int UnidadDeMedida { get; set; }

    public int Moneda { get; set; }

    public Nullable<int> IdTipoIVA { get; set; }

    public decimal SubTotalAjustado { get; set; }



    public virtual Abonos Abonos { get; set; }

    public virtual Conceptos Conceptos { get; set; }

    public virtual PlanDeCuentas PlanDeCuentas { get; set; }

    public virtual TipoIVA TipoIVA { get; set; }

    public virtual Comprobantes Comprobantes { get; set; }

}

}
