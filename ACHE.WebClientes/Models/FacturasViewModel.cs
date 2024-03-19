using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.WebClientes.Models
{
    public class FacturasViewModel
    {
        public List<FacturaViewModel> listaFacturas { get; set; }
    }

    public class FacturaViewModel 
    {
        public int IDUsuario { get; set; }
        public int IDFactura { get; set; }
        public string Descargar { get; set; }
        public string Emisor { get; set; }
        public string TipoComprobante { get; set; }
        public string NroComprobante { get; set; }
        public string CAE { get; set; }
        public DateTime FechaFacturacion { get; set; }
        public decimal Importe{ get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public decimal Saldo { get; set; }
        public string Modo { get; set; }
        public HtmlString BtnPago { get; set; }

        public string ResultadoOperacion { get; set; }
        public string MensajeResultadoOperacion { get; set; }
    }
}