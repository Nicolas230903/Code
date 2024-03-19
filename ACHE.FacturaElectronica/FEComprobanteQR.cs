using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ACHE.FacturaElectronica
{
    public class FEComprobanteQR
    {
        public int ver { get; set; }
        public string fecha { get; set; }
        public long cuit { get; set; }
        public long ptoVta { get; set; }
        public long tipoCmp { get; set; }
        public long nroCmp { get; set; }
        public double importe { get; set; }
        public string moneda { get; set; }
        public double ctz { get; set; }
        //public long tipoDocRec { get; set; }
        //public long nroDocRec { get; set; }
        public string tipoCodAut { get; set; }
        public long codAut { get; set; }
       
    }
}
