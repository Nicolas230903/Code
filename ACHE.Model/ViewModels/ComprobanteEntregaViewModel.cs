using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ComprobantesViewModel
    /// </summary>
    public class ComprobanteEntregaViewModel
    {
        public int IdComprobanteDetalle { get; set; }
        public string Codigo { get; set; }
        public string Concepto { get; set; }
        public int CantidadOriginal { get; set; }
        public int CantidadPendiente { get; set; }
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosComprobantesViewModel
    /// </summary>
    public class ResultadosComprobanteEntregaViewModel
    {
        public IList<ComprobanteEntregaViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }

}