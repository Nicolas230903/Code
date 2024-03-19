using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for CuadroResumenViewModel
    /// </summary>
    public class CuadroResumenViewModel
    {
        public string IDUsuario { get; set; }
        public string IDPersona { get; set; }
        public string RazonSocial { get; set; }
        public string Periodo { get; set; }
        public string Total { get; set; }
        public string Ventas { get; set; }
        public string ConEntrega { get; set; }
        public string SinEntrega { get; set; }
        public string Factura { get; set; }
        public string ConCAE { get; set; }
        public string SinCAE { get; set; }
        public string IVA { get; set; }
        public string Cobros { get; set; }
        public string Saldo { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosCuadroResumenViewModel
    /// </summary>
    public class ResultadosCuadroResumenViewModel
    {
        public IList<CuadroResumenViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }

}