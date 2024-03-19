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
    public class PlanDeCuentasCSV
    {
        public string Nombre;
        public string Codigo;
        public string CodigoPadre;
        public string AdmiteAsientoManual;
        public string TipoDeCuenta;
    }

    public class PlanDeCuentasCSVTmp : PlanDeCuentasCSV
    {
        public string resultados { get; set; }
        public string Estado { get; set; }
        public int IDUsuario { get; set; }
        public int IDPadre { get; set; }
    }
}
