
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
    
public partial class BancosPlanDeCuenta
{

    public int IDBancoPlanDeCuenta { get; set; }

    public int IDBanco { get; set; }

    public int IDPlanDeCuenta { get; set; }

    public int IDUsuario { get; set; }



    public virtual Bancos Bancos { get; set; }

    public virtual PlanDeCuentas PlanDeCuentas { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}