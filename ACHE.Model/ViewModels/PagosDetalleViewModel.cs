using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for PagosDetalleViewModel
    /// </summary>
    public class PagosDetalleViewModel
    {
        public int ID { get; set; }
        public int IDCompra { get; set; }
        public string nroFactura { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteNeto { get; set; }

        public decimal Total
        {
            get
            {
                decimal subTotal = Importe;

                return Math.Round(subTotal, 2);
            }
        }
    }

    /// <summary>
    /// Summary description for PagosEditViewModel
    /// </summary>
    public class PagosEditViewModel
    {
        public int ID { get; set; }
        public int IDPersona { get; set; }
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public string Modo { get; set; }
        public int IDPuntoVenta { get; set; }
        public string Observaciones { get; set; }
        public PersonasEditViewModel Personas { get; set; }
    }
}