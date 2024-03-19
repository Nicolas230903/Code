using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ComprasViewModel
    /// </summary>
    public class ComprasViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Fecha { get; set; }
        public string Importe { get; set; }
        public string Iva { get; set; }
        public string Retenciones { get; set; }
        public string NoGravado { get; set; }
        public string Total { get; set; }
        public string NroFactura { get; set; }
        public string Rubro { get; set; }

        public string Tipo { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosPagosViewModel
    /// </summary>
    public class ResultadosComprasViewModel
    {
        public IList<ComprasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}