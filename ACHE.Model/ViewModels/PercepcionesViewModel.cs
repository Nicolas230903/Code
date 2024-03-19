using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class PercepcionesViewModel
    {
        public string Fecha { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string CondicionIVA { get; set; }
        public string Importe { get; set; }
        public string NroComprobante { get; set; }
        public string Jurisdiccion { get; set; }
    }

    public class ResultadosPercepcionesViewModel
    {
        public IList<PercepcionesViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}
