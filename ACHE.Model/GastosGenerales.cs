
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
    
public partial class GastosGenerales
{

    public int IdGastosGenerales { get; set; }

    public int IdUsuario { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public System.DateTime Periodo { get; set; }

    public decimal Sueldos { get; set; }

    public decimal SeguridadEHigiene { get; set; }

    public decimal Municipales { get; set; }

    public decimal Monotributos { get; set; }

    public decimal AportesYContribuciones { get; set; }

    public decimal Ganancias12 { get; set; }

    public decimal CreditoBancario { get; set; }

    public decimal RetencionesDeIIBB { get; set; }

    public decimal PlanesAFIP { get; set; }

    public decimal Gastos1 { get; set; }

    public decimal Gastos2 { get; set; }

    public decimal Gastos3 { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }

    public decimal F1 { get; set; }

    public decimal F2 { get; set; }



    public virtual Usuarios Usuarios { get; set; }

}

}
