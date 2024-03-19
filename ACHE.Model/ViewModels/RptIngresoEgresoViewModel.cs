using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptCcDetalleViewModel
    /// </summary>
    public class RptIngresoEgresoViewModel
    {
        public string RazonSocial { get; set; }
        public string Comprobante { get; set; }
        public string Fecha { get; set; }
        public string Total { get; set; }
        public string TipoComprobante { get; set; }

        public string TotalIVA { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptCcDetalleViewModel
    /// </summary>
    public class ResultadosRptIngresoEgresoViewModel
    {
        public IList<RptIngresoEgresoViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}