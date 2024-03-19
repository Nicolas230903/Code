using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio
{
    public class PersonaDomicilioConGeo
    {
        public string Domicilio { get; set; }
        public string PisoDepto { get; set; }
        public string CodigoPostal { get; set; }
        public int IDProvincia { get; set; }
        public int IDCiudad { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }

    }
}
