
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
    
public partial class Transporte
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Transporte()
    {

        this.Comprobantes = new HashSet<Comprobantes>();

    }


    public int IdTransporte { get; set; }

    public int IdUsuario { get; set; }

    public int IDProvincia { get; set; }

    public int IDCiudad { get; set; }

    public string RazonSocial { get; set; }

    public string Domicilio { get; set; }

    public string PisoDepto { get; set; }

    public string CodigoPostal { get; set; }

    public string Provincia { get; set; }

    public string Ciudad { get; set; }

    public string Contacto { get; set; }

    public string Telefono { get; set; }

    public Nullable<System.DateTime> FechaAlta { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Comprobantes> Comprobantes { get; set; }

    public virtual Usuarios Usuarios { get; set; }

}

}