using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ComprobantesDetalleViewModel
    /// </summary>
    public class ComprobantesDetalleViewModel
    {
        public int ID { get; set; }
        public string Concepto { get; set; }
        public decimal Cantidad { get; set; }
        public int? IDConcepto { get; set; }
        public int? IDPlanesPagos { get; set; }
        public int? IDAbonos { get; set; }
        public int? IDPlanDeCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Iva { get; set; }
        public int IdTipoIva { get; set; }
        public decimal Bonificacion { get; set; }
        public string Codigo { get; set; }
        public string CodigoPlanCta { get; set; }
        public bool Ajuste { get; set; }
        public decimal SubTotalAjustado { get; set; }
        public decimal PrecioUnitarioSinIVA
        {
            get
            {
                decimal subTotal = PrecioUnitario;

                if (Bonificacion > 0)
                    subTotal = subTotal - ((subTotal * Bonificacion) / 100);

                //return Math.Round(subTotal, 2);
                return subTotal;
            }
        }

        public decimal PrecioUnitarioConIva
        {
            get
            {
                decimal subTotal = PrecioUnitario;

                if (Bonificacion > 0)
                    subTotal = subTotal - ((subTotal * Bonificacion) / 100);

                if (Iva > 0)
                    subTotal = ((subTotal) + ((subTotal * Iva) / 100));

                //return Math.Round(subTotal, 2);
                return subTotal;
            }
        }

        public decimal TotalSinIva
        {
            get
            {
                //decimal subTotal = Cantidad * PrecioUnitario;

                //if (Bonificacion > 0)
                //    subTotal = subTotal - ((subTotal * Bonificacion) / 100);               

                decimal subTotal = Cantidad * PrecioUnitarioSinIVA;
                
                return Math.Round(subTotal, 2);
            }
        }

        public decimal TotalConIva
        {
            get
            {
               //decimal subTotal = Cantidad * PrecioUnitario;
                
                decimal subTotal = Cantidad * PrecioUnitarioConIva;

                //if (Bonificacion > 0)
                //    subTotal = subTotal - ((subTotal * Bonificacion) / 100);
                /*
                if (Iva > 0)
                    subTotal = ((subTotal) + ((subTotal * Iva) / 100));*/

                return Math.Round(subTotal, 2);
            }
        }

    }
}