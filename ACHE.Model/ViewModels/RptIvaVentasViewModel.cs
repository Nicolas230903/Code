using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptIvaVentasViewModel
    /// </summary>
    public class RptIvaVentasViewModel
    {
        public string Fecha { get; set; }
        public string NroFactura { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string CondicionIVA { get; set; }
        public string Iva { get; set; }
        public string Importe { get; set; }
        public string Total { get; set; }

        public string IVA27 { get; set; }
        public string IVA21 { get; set; }
        public string IVA10 { get; set; }
        public string IVA5 { get; set; }
        public string IVA2 { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptIvaVentasViewModel
    /// </summary>
    public class ResultadosRptIvaVentasViewModel
    {
        public IList<RptIvaVentasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}