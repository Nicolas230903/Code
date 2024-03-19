using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptIvaComprasViewModel
    /// </summary>
    public class RptIvaComprasViewModel
    {
        public string Fecha { get; set; }
        public string Tipo { get; set; }
        public string NroFactura { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string CondicionIVA { get; set; }
        public string MontoGravadoIva2 { get; set; }
        public string MontoGravadoIva5 { get; set; }
        public string MontoGravadoIva10 { get; set; }
        public string MontoGravadoIva21 { get; set; }
        public string MontoGravadoIva27 { get; set; }
        public string MontoNoGravadoYExentos { get; set; }
        public string MontoGravadoMonotributistas { get; set; }
        public string IvaFacturado { get; set; }
        public string IvaPercepcion { get; set; }
        public string TotalFacturado { get; set; }
        public string Rubro { get; set; }

        public string ImpInterno { get; set; }
        public string ImpMunicipal { get; set; }
        public string ImpNacional { get; set; }
        public string Otros { get; set; }
        public string PercepcionIVA { get; set; }
        public string IIBB { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptIvaComprasViewModel
    /// </summary>
    public class ResultadosRptIvaComprasViewModel
    {
        public IList<RptIvaComprasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}