using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class PuntoDeVentaViewModel
    {
        public int IDPuntoDeVenta { get; set; }
        public int PuntoDeVenta { get; set; }
        public DateTime FechaDeAlta { get; set; }
        public DateTime? FechaDeBaja { get; set; }
        public bool PorDefecto { get; set; }
    }
}
