using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptPagoProvViewModel
    /// </summary>
    public class RptComisionesViewModel
    {
        public string Vendedor { get; set; }
        public string Fecha { get; set; }
        public string Cliente { get; set; }
        public string NroComprobante { get; set; }
        public string Importe { get; set; }
        public string Comision { get; set; }
        public decimal ComisionDecimal { get; set; }
    }
    /// <summary>
    /// Summary description for ResultadosRptPagoProvViewModel
    /// </summary>
    public class ResultadosRptComisionesViewModel
    {
        public IList<RptComisionesViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}