using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class listaPreciosViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public string Activa { get; set; }
    }

    public class listaPreciosAPIViewModel
    {
        public string Token { get; set; }
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public string Activa { get; set; }
        public List<PreciosConceptos> Items { get; set; }
    }

    public class listaPreciosConceptosViewModel
    {
        public int ID { get; set; }
        public int IDConcepto { get; set; }
        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Precio { get; set; }
        public string PrecioLista { get; set; }
    }

    public class ResultadoslistaPreciosViewModel
    {
        public IList<listaPreciosViewModel> Items;
        public IList<listaPreciosConceptosViewModel> Conceptos;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}
