using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class JurisdiccionesViewModel
    {
        public int IDJurisdicion { get; set; }
        public int IDCompra { get; set; }
        public decimal Importe { get; set; }
        public string NombreJurisdiccion { get; set; }
    }
}
