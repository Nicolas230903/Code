namespace ACHE.FacturaElectronica
{
    public class FERegistroTributo
    {
        public FETipoTributo Tipo { get; set; }
        public string Decripcion { get; set; }
        public double BaseImp { get; set; }
        public double Alicuota { get; set; }
        public double Importe { get; set; }
    }
}
