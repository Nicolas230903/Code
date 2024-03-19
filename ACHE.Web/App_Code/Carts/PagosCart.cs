using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    public class PagosCart
    {
        #region Properties

        public int IDPersona { get; set; }
        public int IDPuntoVenta { get; set; }
        public string Modo { get; set; }
        public string TipoComprobante { get; set; }
        public DateTime FechaComprobante { get; set; }
        public string Numero { get; set; }
        public string Observaciones { get; set; }
        public List<PagosDetalleViewModel> Items { get; set; }
        public List<PagosFormasDePagoViewModel> FormasDePago { get; set; }
        public List<PagosRetencionesViewModel> Retenciones { get; set; }

        #endregion

        public static PagosCart Instance;

        public static PagosCart Retrieve()
        {
            if (HttpContext.Current.Session["ASPNETPagosCart"] == null)
            {
                Instance = new PagosCart();
                Instance.Items = new List<PagosDetalleViewModel>();
                Instance.FormasDePago = new List<PagosFormasDePagoViewModel>();
                Instance.Retenciones = new List<PagosRetencionesViewModel>();
                HttpContext.Current.Session["ASPNETPagosCart"] = Instance;
            }
            else
            {
                Instance = (PagosCart)HttpContext.Current.Session["ASPNETPagosCart"];
            }

            return Instance;
        }

        // A protected constructor ensures that an object can't be created from outside  
        protected PagosCart() { }

        #region Reporting Methods

        public decimal GetTotal()
        {
            var total = PagosCart.Retrieve().Items.Sum(x => x.Total);// +PagosCart.Retrieve().Retenciones.Sum(x => x.Importe);
            return total;
        }

        #endregion


    }
}