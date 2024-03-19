using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for CobranzasRetencionesViewModel
    /// </summary>
    public class CobranzasRetencionesViewModel
    {
        public int ID { get; set; }
        public string Tipo { get; set; }
        public string NroReferencia { get; set; }
        public decimal Importe { get; set; }
        public string Fecha { get; set; }

    }
}