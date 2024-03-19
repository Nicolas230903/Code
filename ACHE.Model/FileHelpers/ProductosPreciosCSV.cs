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
    public class ProductosPreciosCSV
    {
        public string Codigo;
        public string Costo;
        public string Precio;
        public string Stock;
    }

    public class ProductosPreciosCSVTmp : ProductosPreciosCSV
    {
        public int IDLista { get; set; }
        public string resultados { get; set; }
        public string Estado { get; set; }
        public int IDUsuario { get;set; }
    }
}
