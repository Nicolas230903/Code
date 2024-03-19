using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio
{
    public class PagosCartDto
    {
        public string Token { get; set; }
        public int IDPago { get; set; }
        public int IDPersona { get; set; }
        public string Observaciones { get; set; }
        public string FechaPago { get; set; }
        public List<PagosDetalleViewModel> Items { get; set; }
        public List<PagosFormasDePagoViewModel> FormasDePago { get; set; }
        public List<PagosRetencionesViewModel> Retenciones { get; set; }
    }
}