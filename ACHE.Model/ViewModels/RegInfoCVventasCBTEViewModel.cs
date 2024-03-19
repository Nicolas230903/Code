using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.FacturaElectronica;

namespace ACHE.Model.ViewModels
{
    public class RegInfoCVventasCBTEViewModel
    {
        public string PuntoVenta { get; set; }
        public string FechaComprobante { get; set; }
        public string TipoComprobante { get; set; }
        public string NroComprobante { get; set; }
        public string NroComprobanteHasta { get; set; }
        public int ClienteDocTipo { get; set; }
        public string ClienteDocNro { get; set; }
        public string ClienteNombre { get; set; }

        public string ImporteTotal { get; set; }
        public string ImporteNoGravado { get; set; }
        public string ImportePercepcionNoCategorizada { get; set; }
        public string ImporteOperacionesExentas { get; set; }
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
        public string OtrosTributos { get; set; }

        public string FechaVencimientoPago { get; set; }
    }
}
