using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class LibroDiarioViewModel
    {
        public string nroCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string Debe { get; set; }
        public string Haber { get; set; }
        public bool EsAsientoManual { get; set; }
    }

    public class AsientoViewModel
    {
        public int IDAsiento { get; set; }
        public string Fecha { get; set; }
        public string NroAsiento { get; set; }
        public string Leyenda { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public IList<LibroDiarioViewModel> Items;
    }

    public class ResultadosLibroDiarioViewModel
    {
        public IList<AsientoViewModel> Asientos;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
        public string TotalDebe { get; set; }
        public string TotalHaber { get; set; }
    }

    public class AsientoExportacionViewModel
    {
        public string NroAsiento { get; set; }
        public string Fecha { get; set; }
        public string Leyenda { get; set; }
        public string codigo { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
    }
}
