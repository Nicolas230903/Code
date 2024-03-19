using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class ActividadViewModel
    {
        public int IDActividad { get; set; }
        public string Codigo { get; set; }
        public DateTime FechaDeAlta { get; set; }
        public DateTime? FechaDeBaja { get; set; }
        public bool PorDefecto { get; set; }
    }
}
