using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.WebAPI.Models
{
    public class VarianteTiendaNube
    {
        public string token { get; set; }
        public string nombrePropiedad1 { get; set; }
        public string valorPropiedad1 { get; set; }
        public string nombrePropiedad2 { get; set; }
        public string valorPropiedad2 { get; set; }
        public string nombrePropiedad3 { get; set; }
        public string valorPropiedad3 { get; set; }
        public string precio { get; set; }
        public string precioPromocional { get; set; }
        public string peso { get; set; }
        public int stock { get; set; }
        public string sku { get; set; }
        public string codigoDeBarras { get; set; }
        public string imagen { get; set; }
        public string IdNubePadre { get; set; }
        public string IdNubeVariante { get; set; }
    }
}