
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
    
public partial class StockAuditoria
{

    public int IdStockAuditoria { get; set; }

    public int IdUsuario { get; set; }

    public int IdConcepto { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public decimal Cantidad { get; set; }

    public decimal StockAnterior { get; set; }

    public decimal StockNuevo { get; set; }

    public Nullable<long> idComprobante { get; set; }

    public string Accion { get; set; }



    public virtual Conceptos Conceptos { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}