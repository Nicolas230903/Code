using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class DetalleComprobantesMayorViewModel
    {
        public string Fecha { get; set; }
        public string Leyenda { get; set; }
        public string Debe { get; set; }
        public string Haber { get; set; }
        public decimal Saldo { get; set; }
    }

    public class CuentasViewModel
    {
        public int IDPlanDeCuenta { get; set; }
        public string nroCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string TotalDebe { get; set; }
        public string TotalHaber { get; set; }
        public string TotalDeudor { get; set; }
        public string TotalAcreedor { get; set; }
        public string TotalActivo { get; set; }
        public string TotalPasivo { get; set; }
        public string TotalPerdidas { get; set; }
        public string TotalGanancias { get; set; }

        public string Saldo { get; set; }
        public string TipoDeCuenta { get; set; }

        
        public IList<DetalleComprobantesMayorViewModel> Items;
    }

    public class ResultadosLibroMayorViewModel
    {
        public IList<CuentasViewModel> Asientos;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
        public string TotalDebe { get; set; }
        public string TotalHaber { get; set; }
        public string TotalDeudor { get; set; }
        public string TotalAcreedor { get; set; }
        public string TotalActivo { get; set; }
        public string TotalPasivo { get; set; }
        public string TotalPerdidas { get; set; }
        public string TotalGanancias { get; set; }
        public string TotalSaldo { get; set; }
    }

    public class LibroMayorExport
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Fecha { get; set; }
        public string Leyenda { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Saldo { get; set; }
    }
}