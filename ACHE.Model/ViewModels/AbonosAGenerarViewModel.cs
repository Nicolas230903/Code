using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for AbonosViewModel
    /// </summary>
    public class AbonosAGenerarViewModel
    {
        public int IDPersona { get; set; }
        public string RazonSocial { get; set; }
        public string ClienteEmail { get; set; }
        public string Cuit { get; set; }
        public string CondicionIva { get; set; }
        public decimal Importe { get; set; }
        public decimal Iva { get; set; }
        public string ivaCalculado { get { return ((((Importe * Iva) / 100))).ToString("N2"); } }
        public string nroRegistro { get; set; }
        public string nroComprobante { get; set; }
        public string Estado { get; set; }
        public string FEGenerada { get; set; }
        public string URL { get; set; }
        public string EnvioFE { get; set; }
        public decimal Total { get { return ((Importe + ((Importe * Iva) / 100)) * Cantidad); } }
        public string FrecuenciaAbono { get; set; }

        public string ImportaPantalla { get { return Importe.ToString("N2"); } }
        public string TotalPantalla { get { return Total.ToString("N2"); } }

        public decimal Cantidad { get; set; }
    }

    public class AbonosAGenerarGrupoViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
        public int tipoConcepto { get; set; }
        public int Cantidad { get { return (Items.Count); } }
        public int? IDPlanDeCuenta { get; set; }
        public string TotalCant { get { return (Items.Sum(x => x.Total)).ToString("N2"); } }
        public IList<AbonosAGenerarViewModel> Items;
    }

    public class ResultadosAbonosAGenerarViewModel
    {
        public IList<AbonosAGenerarGrupoViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }

        public string TotalIva { get { return (Items.Sum(x => x.Items.Sum(y => Convert.ToDecimal(y.ivaCalculado) * y.Cantidad))).ToString("N2"); } }
        public string Total { get { return (Items.Sum(x => x.Items.Sum(y => y.Total))).ToString("N2"); } }
        public string totalImporte { get { return (Items.Sum(x => x.Items.Sum(y => y.Importe * y.Cantidad))).ToString("N2"); } }
    }
}