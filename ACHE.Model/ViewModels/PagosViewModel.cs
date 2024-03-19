using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for PagosViewModel
    /// </summary>
    public class PagosViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Fecha { get; set; }
        public string ImporteNeto { get; set; }
        public string Iva { get; set; }
        public string Retenciones { get; set; }
        public string NoGravado { get; set; }
        public string Total { get; set; }
        public string Tipo { get; set; }
        public string NroFactura { get; set; }
        public string NroComprobante { get; set; }
     
    }

    /// <summary>
    /// Summary description for ResultadosPagosViewModel
    /// </summary>
    public class ResultadosPagosViewModel
    {
        public IList<PagosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}