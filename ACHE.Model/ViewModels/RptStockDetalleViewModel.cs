using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for RptRnkViewModel
    /// </summary>
    public class RptStockDetalleViewModel
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Accion { get; set; }
        public string Cantidad { get; set; }
        public string StockAnterior { get; set; }
        public string StockActual { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Fecha { get; set; }
        public string Usuario { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosRptRnkViewModel
    /// </summary>
    public class ResultadosRptStockDetalleViewModel
    {
        public IList<RptStockDetalleViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}