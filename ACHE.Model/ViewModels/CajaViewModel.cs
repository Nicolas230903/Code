using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for CajaViewModel
    /// </summary>
    public class CajaViewModel
    {
        public int ID { get; set; }
        public string tipoMovimiento { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public string Observaciones { get; set; }
        public string Fecha { get; set; }
        public string FechaAnulacion { get; set; }
        public string Estado { get; set; }
        public string EstadoFecha { get; set; }
        public string MedioDePago { get; set; }
        public string Ticket { get; set; }

        public string Ingreso { get; set; }
        public string Egreso { get; set; }
        public string Saldo { get; set; }

        public string TieneFoto { get; set; }
        public string FileName { get; set; }
        public int IDPlanDeCuenta { get; set; }
    }

    public class ConceptosMeses
    {
        public IList<CajaViewModel> Conceptos { get; set; }
        public string concepto {get;set;}
    }
    public class ResultadosCajaViewModel
    {
        //public IList<CajaViewModel> Items;
        public IList<ConceptosMeses> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
        public string TotalSinConsolidar { get; set; }
    }
}