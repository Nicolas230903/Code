using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ProductosViewModel
    /// </summary>
    public class ConceptosViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Precio { get; set; }
        public string Stock { get; set; }
        public string Provincia { get; set; }
        public string Estado { get; set; }
        public string Iva { get; set; }
        public string TipoIva { get; set; }
        public string Tipo { get; set; }
        public string CodigoProveedor { get; set; }
        public string CostoInterno { get; set; }
    }

    public class ResultadosProductosViewModel
    {
        public IList<ConceptosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}