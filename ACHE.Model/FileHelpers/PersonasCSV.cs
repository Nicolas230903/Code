using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model
{
    [DelimitedRecord(";")]
    [IgnoreFirst(1)]
    public class PersonasCSV 
    {
        public string Codigo { get;set; }
        public string RazonSocial { get; set; }
        public string NombreFantansia { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string CondicionIva { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }
        public string Observaciones { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }
        public string PisoDepto { get; set; }
        public string CodigoPostal { get; set; }
        public string EmailsEnvioFc { get; set; }
        public string Personeria { get; set; }
        public string AlicuotaIvaDefecto { get; set; }
        public string TipoComprobanteDefecto { get; set; }
        public string CBU { get; set; }
        public string Banco { get; set; }
       
        public string Contacto { get; set; }

    }

    public class PersonasCSVTmp : PersonasCSV
    {
        public string Tipo { get; set; }
        public string resultados { get; set; }
        public DateTime fechaAlta { get; set; }
        public string Estado{ get; set; }
        public int IDUsuario { get; set; }
    }
}
