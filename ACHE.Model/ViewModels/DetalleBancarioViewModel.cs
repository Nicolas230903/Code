using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class DetalleBancarioViewModel
    {
        public int IDUsuario { get; set; }
        public string TipoMovimiento { get; set; }
        public int IDBanco { get; set; }
        public string Importe { get; set; }
        public string Fecha { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string TipoComprobante { get; set; }
        public string Punto { get; set; }
        public string NumeroComprobante { get; set; }
        public string FormaDePago { get; set; }
        public string NroReferencia { get; set; }
        public string Comprobante { get; set; }
    }

    public class ResultadosDetalleBancarioViewModel
    {
        public IList<DetalleBancarioViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}
