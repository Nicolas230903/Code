using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class AsientosManualesViewModel
    {
        public int IDPlanDeCuenta { get; set; }
        public string Codigo { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public string NombreCuenta { get; set; }
    }

    public class ResultadoAsientosManualesViewModel
    {
        public List<AsientosManualesViewModel> items { get; set; }
        public string Fecha { get; set; }
        public string Leyenda { get; set; }
    }
}
