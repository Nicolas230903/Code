
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
    
public partial class RptCobranzasPendientes
{

    public System.DateTime FechaComprobante { get; set; }

    public int IDUsuario { get; set; }

    public string tipo { get; set; }

    public int Punto { get; set; }

    public int Numero { get; set; }

    public int IDPersona { get; set; }

    public string TipoDocumento { get; set; }

    public string NroDocumento { get; set; }

    public string RazonSocial { get; set; }

    public string CondicionIVA { get; set; }

    public decimal Importe { get; set; }

    public decimal Iva { get; set; }

    public decimal ImporteTotal { get; set; }

    public decimal Saldo { get; set; }

}

}
