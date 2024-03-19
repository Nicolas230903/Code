using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptRetencionesSufridasViewModel
    /// </summary>
    public class RptRetencionesSufridasViewModel
    {
        public string Fecha { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string CondicionIVA { get; set; }
        public string Tipo { get; set; }
        public string Importe { get; set; }
        public string NroReferencia { get; set; }
        public int IDRetencion { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptRetencionesSufridasViewModel
    /// </summary>
    public class ResultadosRptRetencionesSufridasViewModel
    {
        public IList<RptRetencionesSufridasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}