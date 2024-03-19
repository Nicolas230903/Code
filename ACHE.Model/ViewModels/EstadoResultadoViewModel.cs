using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class EstadoResultadoViewModel
    {
        public string TipoDeAsiento { get; set; }
        public decimal? importe { get; set; }
        public string fecha { get; set; }
        public string NombreMeses { get; set; }
        public string Cuenta { get; set; }
    }

    public class ResultadosEstadoResultadoViewModel 
    {
        public List<EstadoResultadoViewModel> Items { get; set; }
        public List<MesesEstadoResultado> Listathead { get; set; }
        public List<CuentasEstadoResultado> ListaTbody { get; set; }

        public int TotalItems { get; set; }
        public int TotalPage { get; set; }
    }


    public class MesesEstadoResultado
    {
        public string Nombre { get; set; }
    }

    public class CuentasEstadoResultado
    {
        public string Nombre { get; set; }
        public List<ImportesEstadoResultado> ListaImportes { get; set; }
    }

    public class ImportesEstadoResultado
    {
        public decimal Importe { get; set; }
    }
}
