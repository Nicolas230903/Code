using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.Licencia
{
    public class PostResponseLicencia
    {
        public string cliente { get; set; }
        public string plan { get; set; }
        public string vigencia { get; set; }
        public string serial { get; set; }
        public string clave { get; set; }
        public List<Modulos> modulos { get; set; }

    }
    public class Modulos
    {
        public string nombre { get; set; }
        public string version { get; set; }
        public string urlInstalador32 { get; set; }
        public string urlInstalador64 { get; set; }
    }
}
