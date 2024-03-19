using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for BancosViewModel
    /// </summary>
    public class GastosBancariosViewModel
    {
        public int ID { get; set; }
        public string Fecha { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public decimal IVA { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public decimal IIBB { get; set; }
        public decimal Importe21 { get; set; }
        public decimal CreditoComputable { get; set; }
        public decimal Otros { get; set; }
        public decimal PercepcionIVA { get; set; }
        public decimal SIRCREB { get; set; }
        public decimal Importe105 { get; set; }
      
        public string NombreBanco { get; set; }

        public decimal Total
        {
            get
            {
                return Importe + IVA + Debito + Credito + IIBB + Importe21 + CreditoComputable + Otros + PercepcionIVA + SIRCREB + Importe105;
            }
        }
    }

    public class ResultadosGastosBancariosViewModel
    {
        public IList<GastosBancariosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}