using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for AbonosViewModel
    /// </summary>
    public class PlanDePagosViewModel
    {
        public int ID { get; set; }
        public string TipoDePlan { get; set; }
        public string FechaDePago { get; set; }
        public string FechaVencimiento { get; set; }
        public string ImportePagado { get; set; }
        public string FomaDePago { get; set; }
        public string NroReferencia { get; set; }
        public string Estado { get; set; }
        public string FechaInicio { get; set; }
    }

    public class ResultadosPlanDePagosViewModel
    {
        public IList<PlanDePagosViewModel> Items;
        public int IDPlanActual { get; set; }
        public string NombrePlanActual { get; set; }
        public string FechaVencimiento { get; set; }
    }

    public class PlanesViewModel
    {
        public string Plan { get; set; }
        public string Estado { get; set; }
    }
}