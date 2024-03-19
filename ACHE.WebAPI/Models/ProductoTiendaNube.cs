﻿using ACHE.Model.Negocio.TiendaNube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.WebAPI.Models
{
    public class ProductoTiendaNube
    {
        public string token { get; set; }
        public string identificadorUrl { get; set; }
        public string nombre { get; set; }
        public string categoria { get; set; }
        public string subCategoria { get; set; }
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
        public string mostrarEnTienda { get; set; }
        public string enviaSinCargo { get; set; }
        public string descripcion { get; set; }
        public string tags { get; set; }
        public string tituloParaSEO { get; set; }
        public string descripcionParaSEO { get; set; }
        public string esVariante { get; set; }
        public string marca { get; set; }
        public string imagen { get; set; }
        public string IdNubePadre { get; set; }
        
    }
}