using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio
{
    public class CobranzaCartDto
    {
        public string Token { get; set; }
        public int IDCobranza { get; set; }
        public int IDPersona { get; set; }
        public string Tipo { get; set; }
        public string Fecha { get; set; }
        public int IDPuntoVenta { get; set; }
        public string NumeroCobranza { get; set; }
        public string Observaciones { get; set; }

        public List<CobranzasDetalleViewModel> Items { get; set; }
        public List<CobranzasFormasDePagoViewModel> FormasDePago { get; set; }
        public List<CobranzasRetencionesViewModel> Retenciones { get; set; }
    }
}