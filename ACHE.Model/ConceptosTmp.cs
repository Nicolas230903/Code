
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
    
public partial class ConceptosTmp
{

    public string Codigo { get; set; }

    public decimal Precio { get; set; }

    public int IDUsuario { get; set; }

    public Nullable<decimal> Stock { get; set; }

    public decimal Costo { get; set; }

    public int UnidadDeMedida { get; set; }

    public int Moneda { get; set; }

    public string AtributoDeSeguridad { get; set; }



    public virtual Usuarios Usuarios { get; set; }

}

}