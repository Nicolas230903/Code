using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    public class DashboardViewModel
    {
        public IList<ChartYXZ> Items;
        public decimal TotalCobranzas { get; set; }
        public decimal TotalPagos { get; set; }
    }

    public class Chart
    {
        public string label { get; set; }
        public int data { get; set; }
    }

    public class ChartDate
    {
        public DateTime fecha { get; set; }
        public int importe { get; set; }
    }

    public class ChartString
    {
        public string label { get; set; }
        public string data { get; set; }
    }

    public class ChartDecimal
    {
        public string label { get; set; }
        public decimal data { get; set; }
    }

    public class ChartDouble
    {
        public string label { get; set; }
        public double data { get; set; }
    }

    public class TableHtml
    {
        public string uno { get; set; }
        public string dos { get; set; }
        public string tres { get; set; }
        public decimal cuatro { get; set; }
    }

    public class ChartDecimalInt
    {
        public int fecha { get; set; }
        public decimal data { get; set; }
    }

    public class ChartYXZ
    {
        public string Fecha { get; set; }
        public decimal Uno { get; set; }
        public decimal Dos { get; set; }
    }

    public class ChartXYZ
    {
        public int Fecha { get; set; }
        public int Basico { get; set; }
        public int Profesional { get; set; }
        public int Pyme { get; set; }
        public int Empresa { get; set; }
        public int Prueba { get; set; }
    }

    public class CharPlanes
    {
        public string Fecha { get; set; }
        public int Basico { get; set; }
        public int Profesional { get; set; }
        public int Pyme { get; set; }
        public int Empresa { get; set; }
        public int Prueba { get; set; }
    }

    public class CharFormas
    {
        public string Fecha { get; set; }
        public int MercadoPago { get; set; }
        public int Transferencia { get; set; }
    }

    public class CharFacturacion
    {
        public string Fecha { get; set; }
        public decimal ImporteTotal { get; set; }
    }

    public class PlanViewModel
    {
        public int? IDPlan { get; set; }
        public string Nombre { get; set; }
        public int TotalInactivos { get; set; }
        public int TotalActivos { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalPendienteDePago { get; set; }
        public string ClassMaxCantUsuarios { get; set; }

        public string ToolTipsInactivos { get; set; }
        public string ToolTipsActivos { get; set; }
        public string ToolTipsTotalUsuarios { get; set; }
        public string ToolTipsPendienteDePago { get; set; }   
    }

    public class DashboardAdminViewModel
    {
        public List<PlanViewModel> Listaplanes { get; set; }
        public List<Chart> listaRegistrados { get; set; }
    }

    public class ErrorViewModel
    {
        public bool TieneError { get; set; }
        public string Mensaje { get; set; }
        public object Datos { get; set; }
    }
}