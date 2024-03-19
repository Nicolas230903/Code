using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class RegInfoAlicuotasViewModel
    {
        public string TipoComprobante { get; set; }
        public string Puntoventa { get; set; }
        public string NroComprobante { get; set; }
        public string alicuotaIVA { get; set; }
        public string ImporteNetoGravado { get; set; }
        public string ImpuestoLiquidado { get; set; }
        public int CodigoDocVendedor { get; set; }
        public string DocNroVendedor { get; set; }
    }
}
