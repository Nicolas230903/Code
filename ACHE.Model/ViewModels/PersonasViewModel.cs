using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for PersonasViewModel
    /// </summary>
    public class PersonasViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public string CondicionIva { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }
        public string ProvinciaDesc { get; set; }
        public string CiudadDesc { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public string TieneFoto { get; set; }
        public string Codigo { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public string Tipo { get; set; }
        public string Avisos { get; set; }
    }

    public class ResultadosPersonasViewModel
    {
        public IList<PersonasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}