using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACHE.WebClientes.Models
{
    public class PasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string PasswordOld { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Nueva")]
        public string PasswordNew { get; set; }

        [Required]
        //[System.Web.Mvc.Compare("PasswordNew")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("PasswordNew")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación Contraseña nueva")]
        public string PasswordNew2 { get; set; }
    }
}