using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for GastosGeneralesViewModel
    /// </summary>
    public class GastosGeneralesViewModel
    {
        public int ID { get; set; }
        public string Periodo { get; set; }
        public string Fecha { get; set; }
        public string Total { get; set; }

    }

    /// <summary>
    /// Summary description for ResultadosPagosViewModel
    /// </summary>
    public class ResultadosGastosGeneralesViewModel
    {
        public IList<GastosGeneralesViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}