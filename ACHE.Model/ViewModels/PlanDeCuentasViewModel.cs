using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for AbonosViewModel
    /// </summary>
    public class PlanDeCuentasViewModel
    {
        public int id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int IDPadre { get; set; }
        public string AdminiteAsientoManual { get; set; }
        public List<PlanDeCuentasViewModel> children { get; set; }
        public bool TienePadre { get; set; }
        public string icon { get; set; }
        public string text { get; set; }

        public string TipoDeCuenta { get; set; }
    }
}