using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptCcViewModel
    /// </summary>
    public class RptCcViewModel
    {
        public int IDPersona { get; set; }
        public string FechaUltPago { get; set; }
        public string Estado { get; set; }
        public string RazonSocial { get; set; }
        public string Importe { get; set; }
        public string Abonado { get; set; }
        public string Saldo { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptIvaViewModel
    /// </summary>
    public class ResultadosRptCcViewModel
    {
        public IList<RptCcViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}