using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.ComunicacionesAfip
{
    public class ComunicacionesAfipSinAdjunto
    {
        public long IdComunicacion { get; set; }
        public string CuitDestinatario { get; set; }
        public string FechaPublicacion { get; set; }
        public string fechaVencimiento { get; set; }
        public Nullable<long> SistemaPublicador { get; set; }
        public string sistemaPublicadorDesc { get; set; }
        public Nullable<int> Estado { get; set; }
        public string EstadoDesc { get; set; }
        public string Asunto { get; set; }
        public Nullable<int> Prioridad { get; set; }
        public string PrioridadDesc { get; set; }
        public Nullable<int> TieneAdjunto { get; set; }
        public Nullable<int> Visto { get; set; }
        public int IdUsuario { get; set; }
        public System.DateTime FechaRegistro { get; set; } 
    }
}
