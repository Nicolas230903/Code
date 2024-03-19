using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptPagoProvViewModel
    /// </summary>
    public class RptPagoProvViewModel
    {
        public string Fecha { get; set; }
        public string Proveedor { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string NroFactura { get; set; }
        public string CondicionIVA { get; set; }
        public string Iva { get; set; }
        public string Importe { get; set; }
        public string Total { get; set; }
        public string TotalAbonado { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptPagoProvViewModel
    /// </summary>
    public class ResultadosRptPagoProvViewModel
    {
        public IList<RptPagoProvViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}