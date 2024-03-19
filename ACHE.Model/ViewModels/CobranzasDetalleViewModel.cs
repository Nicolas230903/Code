using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for CobranzasDetalleViewModel
    /// </summary>
    public class CobranzasDetalleViewModel
    {
        public int ID { get; set; }
        public int IDComprobante { get; set; }
        public string Comprobante { get; set; }
        public decimal Importe { get; set; }
        public string Fecha { get; set; }

        //public decimal RetGanancias { get; set; }
        //public decimal IIBB { get; set; }
        //public decimal SUSS { get; set; }
        //public decimal Otros { get; set; }

        public decimal Total
        {
            get
            {
                decimal subTotal = Importe;// +RetGanancias + IIBB + SUSS + Otros;

                return Math.Round(subTotal, 2);
            }
        }
    }

    /// <summary>
    /// Summary description for CobranzasEditViewModel
    /// </summary>
    public class CobranzasEditViewModel
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