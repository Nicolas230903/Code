using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACHE.WebClientes.Models
{
    public class PrimerLoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de Contraseña")]
        //[System.Web.Mvc.CompareAttribute("Password")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password")]
        public string Password2 { get; set; }
    }
}