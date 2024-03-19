using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACHE.WebClientes.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }

        [Required]
        [Display(Name = "Nro de Documento")]
        public string Documento { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}

/*
[Required]
[StringLength(10, ErrorMessage = "La contraseña no es mayor a 10 caracteres")]
[DataType(DataType.Password)]
[Display(Name = "Contraseña actual")]
public string PasswordOld { get; set; }

[Required]
[StringLength(10, ErrorMessage = "La contraseña no puede ser mayor a 10 caracteres")]
[DataType(DataType.Password)]
[Display(Name = "Contraseña nueva")]
public string PasswordNew { get; set; }

[Required]
[StringLength(10, ErrorMessage = "La contraseña no puede ser mayor a 10 caracteres")]
//[Compare("PasswordNew")]
[DataType(DataType.Password)]
[Display(Name = "Confirmación contraseña nueva")]
public string PasswordNew2 { get; set; }
*/