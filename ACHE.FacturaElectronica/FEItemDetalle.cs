using System.Web;

namespace ACHE.FacturaElectronica
{
    public class FEItemDetalle
    {
        public double Cantidad { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public double PrecioConIVA { get; set; }
        public decimal Bonificacion { get; set; }
        public bool Decimal { get; set; } = false;
        public int IdTipoIVA { get; set; }
        public string Fecha { get; set; }
        public double Total
        {
            get
            {
                if (Decimal)
                    return ((Cantidad * Precio) / 100);
                else
                    return Cantidad * Precio;
            }
        }
        public double TotalConIVA
        {
            get
            {
                if (Decimal)
                    return ((Cantidad * PrecioConIVA) / 100);
                else
                    return Cantidad * PrecioConIVA;
            }
        }
    }
}
