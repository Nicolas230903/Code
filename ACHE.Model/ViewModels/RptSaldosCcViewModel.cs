using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptSaldosCcViewModel
    /// </summary>
    public class RptSaldosCcViewModel
    {
        public int IDPersona { get; set; }
        public string RazonSocial { get; set; }
        public string SaldoCliente { get; set; }
        public string SaldoProveedor { get; set; }
        public string SaldoConsolidado { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptSaldosCcViewModelViewModel
    /// </summary>
    public class ResultadosRptSaldosCcViewModelViewModel
    {
        public IList<RptSaldosCcViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}