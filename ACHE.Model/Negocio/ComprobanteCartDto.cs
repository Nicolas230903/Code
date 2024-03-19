using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model.Negocio
{
    public class ComprobanteCartDto
    {
        #region Properties
        public string Token { get; set; }
        public int IDComprobante { get; set; }
        public int IDPersona { get; set; }
        public int IDUsuario { get; set; }
        public int IDPuntoVenta { get; set; }
        public string Modo { get; set; }
        public string TipoComprobante { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string CondicionVenta { get; set; }
        public string TipoConcepto { get; set; }
        public string Numero { get; set; }
        public string Observaciones { get; set; }
        public List<ComprobantesDetalleViewModel> Items { get; set; }

        public decimal PercepcionIVA { get; set; }
        public decimal PercepcionIIBB { get; set; }
        public decimal Descuento { get; set; }
        public decimal ImporteNoGravado { get; set; }
        public decimal ImporteExento { get; set; }
        public int IDJuresdiccion { get; set; }
        public decimal Tributos { get; set; }
        public string Adjunto { get; set; }
        public string Nombre { get; set; }
        public string Vendedor { get; set; }
        public string Envio { get; set; }
        public DateTime FechaEntrega { get; set; }
        public long? ProcesoCompraAutomatica { get; set; }
        public int? IDComprobanteAsociado { get; set; }
        public int? IDDomicilio { get; set; }
        public int? IDTransporte { get; set; }
        public int? IDTransportePersona { get; set; }
        public int? IDUsuarioAdicional { get; set; }
        public int? IDCompraVinculada { get; set; }
        public string Estado { get; set; }
        public int IDActividad { get; set; }
        public string ModalidadPagoAfip { get; set; }
        public string CBU { get; set; }

        #endregion

        // A protected constructor ensures that an object can't be created from outside  
        //protected ComprobanteCartDto() { }

        #region Reporting Methods

        public decimal GetSubTotal()
        {
            return Items.Sum(x => x.TotalSinIva);
        }

        public decimal GetIva()
        {
            return Items.Where(x => x.Iva > 0).Sum(x => (x.TotalConIva - x.TotalSinIva));
        }

        public decimal GetTotal()
        {
            return (GetIva() + GetSubTotal() + GetImporteNoGravado() + GetPercepcionIVA() + GetPercepcionIIBB());
        }
        #endregion

        #region PERCEPCIONES
        public decimal GetPercepcionIVA()
        {
            return ((PercepcionIVA > 0) ? ((GetSubTotal() * PercepcionIVA) / 100) : 0);
        }
        public decimal GetPercepcionIIBB()
        {
            return ((PercepcionIIBB > 0) ? ((GetSubTotal() * PercepcionIIBB) / 100) : 0);
        }
        public decimal GetImporteNoGravado()
        {
            return ImporteNoGravado;
        }

        #endregion
    }
}
