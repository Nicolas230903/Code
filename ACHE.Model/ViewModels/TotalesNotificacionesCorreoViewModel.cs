using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class TotalesNotificacionesCorreoViewModel
    {
        public int TotalCorreos { get; set; }
        public int TotalCorreosEnviados { get; set; }
        public string Usuarios { get; set; }
        public string TipoNotificacion { get; set; }
    }
}
