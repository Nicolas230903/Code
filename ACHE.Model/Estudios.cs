
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
    
public partial class Estudios
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Estudios()
    {

        this.Usuarios = new HashSet<Usuarios>();

    }


    public int IDEstudio { get; set; }

    public string Usuario { get; set; }

    public string Pwd { get; set; }

    public string Email { get; set; }

    public System.DateTime FechaAlta { get; set; }

    public bool Activo { get; set; }

    public System.DateTime FechaUltLogin { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Usuarios> Usuarios { get; set; }

}

}