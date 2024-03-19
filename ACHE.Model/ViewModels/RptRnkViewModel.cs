using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptRnkViewModel
    /// </summary>
    public class RptRnkViewModel
    {
        public string Valor1 { get; set; }
        public string Valor2 { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
        public string Total { get; set; }
        public string CostoInterno { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptRnkViewModel
    /// </summary>
    public class ResultadosRptRnkViewModel
    {
        public IList<RptRnkViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}