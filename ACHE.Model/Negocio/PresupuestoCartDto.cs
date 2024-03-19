using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model.Negocio
{
    public class PresupuestoCartDto
    {
        public string Token { get; set; }
        public int IDPresupuesto { get; set; }
        public int IDPersona { get; set; }
        public string Fecha { get; set; }
        public string Nombre { get; set; }
        public int Numero { get; set; }
        public string CondicionesPago { get; set; }
        public string Observaciones { get; set; }
        public string Vendedor { get; set; }
        public string Estado { get; set; }
        public List<ComprobantesDetalleViewModel> Items { get; set; }


        #region Reporting Methods

        public decimal GetSubTotal()
        {
            return Items.Sum(x => x.TotalSinIva);
        }

        public decimal GetIva()
        {
            return Items.Where(x => x.Iva > 0).Sum(x => (x.TotalConIva - x.TotalSinIva));
        }

        public decimal GetTotal()
        {
            return (GetIva() + GetSubTotal());
        }
        #endregion

    }
}
