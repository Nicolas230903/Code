using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class RptCobranzasPendientesViewModel
    {
        public string Fecha { get; set; }
        public string RazonSocial { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string NroFactura { get; set; }
        public string CondicionIVA { get; set; }
        public string Iva { get; set; }
        public string importeTotal { get; set; }
        public string Importe { get; set; }
        public string Saldo { get; set; }
    }

    public class ResultadosRptCobranzasPendientesViewModel
    {
        public IList<RptCobranzasPendientesViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}