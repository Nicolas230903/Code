
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
    
public partial class PagosRetenciones
{

    public int IDPagoRetenciones { get; set; }

    public int IDPago { get; set; }

    public string Tipo { get; set; }

    public decimal Importe { get; set; }

    public string NroReferencia { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }



    public virtual Pagos Pagos { get; set; }

}

}