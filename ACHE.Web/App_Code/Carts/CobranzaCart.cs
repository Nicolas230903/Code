using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    public class CobranzaCart
    {
        #region Properties

        public int IDPersona { get; set; }
        public int IDPuntoVenta { get; set; }
        public string Modo { get; set; }
        public string TipoComprobante { get; set; }
        public DateTime FechaComprobante { get; set; }
        public string Numero { get; set; }
        public string Observaciones { get; set; }
        public List<CobranzasDetalleViewModel> Items { get; set; }
        public List<CobranzasFormasDePagoViewModel> FormasDePago { get; set; }
        public List<CobranzasRetencionesViewModel> Retenciones { get; set; }

        #endregion

        public static CobranzaCart Instance;

        public static CobranzaCart Retrieve()
        {
            if (HttpContext.Current.Session["ASPNETCobranzaCart"] == null)
            {
                Instance = new CobranzaCart();
                Instance.Items = new List<CobranzasDetalleViewModel>();
                Instance.FormasDePago = new List<CobranzasFormasDePagoViewModel>();
                Instance.Retenciones = new List<CobranzasRetencionesViewModel>();
                HttpContext.Current.Session["ASPNETCobranzaCart"] = Instance;
            }
            else
            {
                Instance = (CobranzaCart)HttpContext.Current.Session["ASPNETCobranzaCart"];
            }

            return Instance;
        }

        // A protected constructor ensures that an object can't be created from outside  
        protected CobranzaCart() { }

        #region Reporting Methods

        public decimal GetTotal()
        {
            var total = CobranzaCart.Retrieve().Items.Sum(x => x.Total);// +CobranzaCart.Retrieve().Retenciones.Sum(x => x.Importe);
            return total;
        }

        #endregion


    }
}