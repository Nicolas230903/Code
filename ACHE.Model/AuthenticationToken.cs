
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
    
public partial class AuthenticationToken
{

    public int IDToken { get; set; }

    public string Token { get; set; }

    public int IDUsuario { get; set; }

    public System.DateTime FechaExpiracion { get; set; }



    public virtual Usuarios Usuarios { get; set; }

}

}
