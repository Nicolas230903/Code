using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for AbonosViewModel
    /// </summary>
    public class AbonosViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Precio { get; set; }
        public string Iva { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string Estado { get; set; }
        public string CantClientes { get; set; }
    }

    public class ResultadosAbonosViewModel
    {
        public IList<AbonosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }

    public class AbonosPersonasViewModel
    {
        public int IDAbono { get; set; }
        public int IDPersona { get; set; }
        public string Cantidad { get; set; }
        public string RazonSocial { get; set; }
        public string Total { get; set; }
    }
}