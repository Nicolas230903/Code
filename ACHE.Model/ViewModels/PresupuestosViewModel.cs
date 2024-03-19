using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for PresupuestosViewModel
    /// </summary>
    public class PresupuestosViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Nombre { get; set; }
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Estado { get; set; }
        public string FechaValidez { get; set; }
        public string Total { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosPresupuestosViewModel
    /// </summary>
    public class ResultadosPresupuestosViewModel
    {
        public IList<PresupuestosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}