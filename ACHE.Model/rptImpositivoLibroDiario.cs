
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
    
public partial class rptImpositivoLibroDiario
{

    public int IDAsiento { get; set; }

    public int IDUsuario { get; set; }

    public System.DateTime Fecha { get; set; }

    public string Leyenda { get; set; }

    public string Codigo { get; set; }

    public int IDPlanDeCuenta { get; set; }

    public string Nombre { get; set; }

    public string tipodeasiento { get; set; }

    public string TipoDeCuenta { get; set; }

    public decimal Debe { get; set; }

    public decimal Haber { get; set; }

}

}