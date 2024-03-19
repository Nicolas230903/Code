using System;
namespace ACHE.FacturaElectronica
{
    public class FERegistroIVA
    {
        private double _baseImp;

        public FETipoIva TipoIva { get; set; }

        /// <summary>
        /// Base imponible para la determinación de la alícuota. 
        /// </summary>
        public double BaseImp
        {
            get
            {
                return _baseImp;
            }
            set
            {
                _baseImp = Math.Round(value, 2);
            }
        }

        public double Importe
        {
            get
            {
                switch (TipoIva)
                {
                    case FETipoIva.Iva10_5:
                        return Math.Round(BaseImp * 10.5 / 100, 2);

                    case FETipoIva.Iva21:
                        return Math.Round(BaseImp * 21 / 100, 2);

                    case FETipoIva.Iva27:
                        return Math.Round(BaseImp * 27 / 100, 2);

                    case FETipoIva.Iva5:
                        return Math.Round(BaseImp * 5 / 100, 2);

                    case FETipoIva.Iva2_5:
                        return Math.Round(BaseImp * 2.5 / 100, 2);

                    case FETipoIva.Iva0:
                    default:
                        return 0;
                }
            }
        }
    }
}
