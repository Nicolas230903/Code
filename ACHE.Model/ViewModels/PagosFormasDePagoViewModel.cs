using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for PagosFormasDePagoViewModel
    /// </summary>
    public class PagosFormasDePagoViewModel
    {
        public int ID { get; set; }
        public string FormaDePago { get; set; }
        public string NroReferencia { get; set; }
        public decimal Importe { get; set; }
        public int? IDCheque { get; set; }

        public int? IDBanco { get; set; }
    }
}