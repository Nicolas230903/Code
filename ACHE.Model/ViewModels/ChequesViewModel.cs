using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for BancosViewModel
    /// </summary>
    public class ChequesViewModel
    {
        public int ID { get; set; }
        public int IDCheque { get; set; }
        public string Banco { get; set; }
        public string Numero { get; set; }
        public string Importe { get; set; }
        public string FechaEmision { get; set; }
        public string Estado { get; set; }
        public string Emisor { get; set; }
        public string Cliente { get; set; }
        public string CantDiasVencimientos { get; set; }
        public string Accion { get; set; }
        public string FechaCobro { get; set; }
        public string FechaVencimiento { get; set; }
    }

    public class ResultadosChequesViewModel
    {
        public IList<ChequesViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}