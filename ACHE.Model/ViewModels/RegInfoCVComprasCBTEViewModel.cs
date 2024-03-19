using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.FacturaElectronica;

namespace ACHE.Model.ViewModels
{
    public class RegInfoCVComprasCBTEViewModel
    {
        public string FechaComprobante { get; set; }
        public string TipoComprobante { get; set; }
        public string PuntoVenta { get; set; }
        public string NroComprobante { get; set; }
        public string DespachoImportacion { get; set; }
        public int CodigoDocVendedor { get; set; }
        public string DocNroVendedor { get; set; }
        public string NombreVendedor { get; set; }

        public string ImporteTotal { get; set; }
        public string ImporteNoGravado { get; set; }
        public string ImporteOperacionesExentas { get; set; }
        public string ImportePercepcionImpuestoValorAgregado { get; set; }
        public string ImportePercepcionesNacionales { get; set; }
        public string ImportePercepcionesIngresoBruto { get; set; }
        public string ImportePercepcionesMunicipales { get; set; }
        public string ImportePercepcionesInternos { get; set; }

        public string CodigoMoneda { get; set; }
        public string tipoCambio { get; set; }

        private List<FERegistroIVA> _detalleIva = new List<FERegistroIVA>();
        public List<FERegistroIVA> DetalleIva
        {
            get { return _detalleIva; }
            set { _detalleIva = value; }
        }

        public int CantAlicuotaIVA { get; set; }
        public string CodigoOperacion { get; set; }
        public string CreditoFiscalComputable { get; set; }
        public string OtrosTributos { get; set; }

        public string CUITEmisorCorredor { get; set; }
        public string DenominacionEmisorCorredor { get; set; }
        public string IVAComision { get; set; }
    }
}
