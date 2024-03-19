using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptCcDetalleViewModel
    /// </summary>
    public class RptCcDetalleViewModel
    {
        public string PDC { get; set; }
        public string Raiz { get; set; }      
        public string RazonSocial { get; set; }
        public string Comprobante { get; set; }
        public string CAE { get; set; }
        public string Fecha { get; set; }
        public string ComprobanteAplicado { get; set; }
        public string FechaCobro { get; set; }
        public string Importe { get; set; }
        public string IVA { get; set; }
        public string VaADeuda { get; set; }
        public string Cobrado { get; set; }
        public string Total { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptCcDetalleViewModel
    /// </summary>
    public class ResultadosRptCcDetalleViewModel
    {
        public IList<RptCcDetalleViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}