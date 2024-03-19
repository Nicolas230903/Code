namespace ACHE.FacturaElectronica.Lib
{
    public enum MedidasDocumento
    {
        A4 = 1,
        Oficio = 2,
        Carta = 3,
        RemitoPolydem = 4,
        Etiquetas108mmX3 = 5,
        Ticket = 6,
        Ticket80 = 7
    }

    public enum Orientacion
    {
        Horizontal,
        Vertical
    }

    public enum TipoReporte
    {
        Tabla,
        Formulario
    }


    public enum Alineado
    {
        Centro = 1,
        Justificado = 2,
        Izquierda = 3,
        Derecha = 4
    }


    public class Columna
    {
        public Alineado AlineacionDetalle;
        public string DataField;
        public string TituloColumna;
        public float Width;
        public bool Visible = true;
        public bool OmitirFormateadoNumerico;

        /// <summary>
        /// Si el valor es cero muestra este valor, si no se carga en esta propiedad un valor muestra 0
        /// </summary>
        public string MostarSiCero;

        /// <summary>
        /// Especifica si se totalizará por esa columna o no
        /// </summary>
        public bool Totalizar;

        /// <summary>
        /// Especifica si se totalizará por esa columna o no
        /// </summary>
        public bool TotalizarFila;

        /// <summary>
        /// Aqui guardo los totales si la columna es a totalizar
        /// </summary>
        public decimal Total;

        /// <summary>
        /// Si detecta un length mayor a 10 trimea devolviendo dd/MM/yyyy
        /// </summary>
        public bool FechaDDMMYYYY;


        public Columna()
        {
            AlineacionDetalle = Alineado.Centro;
            DataField = "";
            TituloColumna = "";
            Width = 0;
        }

        public Columna(Alineado alineacionDetalle, string dataField, string tituloColumna, float width, bool totalizar)
        {
            AlineacionDetalle = alineacionDetalle;
            DataField = dataField;
            TituloColumna = tituloColumna;
            Width = width;

            Totalizar = totalizar;
        }

        public Columna(Alineado alineacionDetalle, string dataField, string tituloColumna, float width, bool totalizar, string mostrarSiCero)
        {
            AlineacionDetalle = alineacionDetalle;
            DataField = dataField;
            TituloColumna = tituloColumna;
            Width = width;
            Totalizar = totalizar;
            MostarSiCero = mostrarSiCero;
        }

        public Columna(Alineado alineacionDetalle, string tituloColumna, float width, bool totalizar, bool totalizarFila, string mostrarSiCero)
        {
            AlineacionDetalle = alineacionDetalle;
            DataField = "";
            TituloColumna = tituloColumna;
            Width = width;
            Totalizar = totalizar;
            TotalizarFila = totalizarFila;
            MostarSiCero = mostrarSiCero;
        }
    }
}