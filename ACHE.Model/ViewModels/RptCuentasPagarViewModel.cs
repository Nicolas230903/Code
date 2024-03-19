using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptPagoProvViewModel
    /// </summary>
    public class RptCuentasPagarViewModel
    {
        public string Fecha { get; set; }
        public string Proveedor { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string NroFactura { get; set; }
        public string CondicionIVA { get; set; }
        public string Saldo { get; set; }
        public string FechaPrimerVencimiento { get; set; }
        public string FechaSegundoVencimiento { get; set; }
    }
    /// <summary>
    /// Summary description for ResultadosRptPagoProvViewModel
    /// </summary>
    public class ResultadosRptCuentasPagarViewModel
    {
        public IList<RptCuentasPagarViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}