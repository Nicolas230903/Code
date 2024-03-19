using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.WebAPI.Models
{
    public class Error
    {
        public string codigo { get; set; }
        public string mensaje { get; set; }
        public string descripcion { get; set; }
    }
}