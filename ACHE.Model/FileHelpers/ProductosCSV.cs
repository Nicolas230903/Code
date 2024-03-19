using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model
{
    [DelimitedRecord(";")]
    [IgnoreFirst(1)]
    public class ProductosCSV //: INotifyRead<prodcutosCSV>
    {
        public string nombre;
        public string codigo;
        public string descripcion;
        public string stock;
        public string StockMinimo;
        public string precioUnitario;
        public string observaciones;
        public string iva;
        public string CostoInterno;
        public string CodigoProveedor;
        

        /*public void BeforeRead(BeforeReadEventArgs<prodcutosCSV> e)
        {
        }

        public void AfterRead(AfterReadEventArgs<prodcutosCSV> e)
        {
            if (e.Record.nombre.Length > 20)
                throw new Exception("Line " + e.LineNumber + ": First name is too long");

        }*/
        /*public void AfterRead(EngineBase engine, string line)
        {
            if (nombre.Length>50)
                throw new Exception("Line " + line + ": Surname name is too long");

        }*/

    }

    //internal class TwoDecimalConverter : ConverterBase
    //{
    //    public override object StringToField(string from)
    //    {
    //        decimal res = Convert.ToDecimal(from);
    //        return res / 100;
    //    }

    //    public override string FieldToString(object from)
    //    {
    //        decimal d = (decimal)from;
    //        return Math.Round(d * 100).ToString();
    //    }

    //}


    public class ProductosCSVTmp : ProductosCSV
    {
        public string tipo { get; set; }
        public string resultados { get; set; }
        public DateTime fechaAlta { get; set; }
        public string Estado { get; set; }
        public int IDUsuario { get; set; }
        public int IDPersona { get; set; }
    }
}
