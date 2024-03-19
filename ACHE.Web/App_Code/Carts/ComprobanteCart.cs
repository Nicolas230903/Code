using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    public class ComprobanteCart
    {
        #region Properties

        public int IDPersona { get; set; }
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
        public decimal Descuento { get; set; }

        public decimal PercepcionIVA { get; set; }
        public decimal PercepcionIIBB { get; set; }
        public int IDJuresdiccion { get; set; }

        #endregion

        public static ComprobanteCart Instance;

        public static ComprobanteCart Retrieve()
        {
            if (HttpContext.Current.Session["ASPNETComprobanteCart"] == null)
            {
                Instance = new ComprobanteCart();
                Instance.Items = new List<ComprobantesDetalleViewModel>();
                HttpContext.Current.Session["ASPNETComprobanteCart"] = Instance;
            }
            else
            {
                Instance = (ComprobanteCart)HttpContext.Current.Session["ASPNETComprobanteCart"];
            }

            return Instance;
        }

        // A protected constructor ensures that an object can't be created from outside  
        protected ComprobanteCart() { }

        #region Reporting Methods

        public decimal GetDescuento()
        {
            //return ComprobanteCart.Retrieve().Items.Sum(x => x.TotalSinIva);
            return ComprobanteCart.Retrieve().Descuento;
        }



        public decimal GetSubTotal()
        {
            decimal subTotal = 0;

            subTotal += ComprobanteCart.Retrieve().Items.Where(x => x.IdTipoIva > 2 && x.SubTotalAjustado > 0).Sum(x => (x.SubTotalAjustado - ((x.SubTotalAjustado * x.Iva) / 100)));
            subTotal += ComprobanteCart.Retrieve().Items.Where(x => x.IdTipoIva > 2 && x.SubTotalAjustado == 0).Sum(x => x.TotalSinIva);

            //return ComprobanteCart.Retrieve().Items.Sum(x => x.TotalSinIva);
            return subTotal;
        }

        public decimal GetIva()
        {
            decimal iva = 0;

            iva += ComprobanteCart.Retrieve().Items.Where(x => x.Iva > 0 && x.SubTotalAjustado > 0).Sum(x => (x.SubTotalAjustado - (x.SubTotalAjustado - ((x.SubTotalAjustado * x.Iva) / 100))));
            iva += ComprobanteCart.Retrieve().Items.Where(x => x.Iva > 0 && x.SubTotalAjustado == 0).Sum(x => (x.TotalConIva - x.TotalSinIva));

            //return ComprobanteCart.Retrieve().Items.Where(x => x.Iva > 0).Sum(x => (x.TotalSinIva * x.Iva / 100));
            return iva;
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            total = GetIva() + GetSubTotal() + GetImporteNoGravado() + GetImporteExento() + GetPercepcionIVA() + GetPercepcionIIBB();
            decimal tempDescuento = 0;
            tempDescuento = (GetDescuento() / 100) * total;            
            return (total - tempDescuento);
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
            return ComprobanteCart.Retrieve().Items.Where(x => x.IdTipoIva == 1).Sum(x => x.TotalSinIva);
        }
        public decimal GetImporteExento()
        {
            return ComprobanteCart.Retrieve().Items.Where(x => x.IdTipoIva == 2).Sum(x => x.TotalSinIva);
        }

        #endregion
    }
}