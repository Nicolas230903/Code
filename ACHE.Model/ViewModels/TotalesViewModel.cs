using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for TotalesViewModel
    /// </summary>
    public class TotalesViewModel
    {
        public string Descuento { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }

        //*** PERCEPCIONES***//
        public string PercepcionIVA { get; set; }
        public string PercepcionIIBB { get; set; }
        public string ImporteNoGravado { get; set; }
        public string ImporteExento { get; set; }
        //*** ***//
        public string Total { get; set; }
    }
}