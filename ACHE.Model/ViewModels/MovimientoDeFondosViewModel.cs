using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class MovimientoDeFondosViewModel
    {
        public int ID { get; set; }
        public string CuentaOrigen { get; set; }
        public string CuentaDestino { get; set; }
        public string FechaMovimiento { get; set; }
        public string Importe { get; set; }
        public string Observaciones { get; set; }

        public int IDBancoOrigen { get; set; }
        public int IDBancoDestino { get; set; }
    }

    public class ResultadosMovimientoDeFondosViewModel
    {
        public IList<MovimientoDeFondosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}
