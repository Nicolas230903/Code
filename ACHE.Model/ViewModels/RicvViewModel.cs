using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class RicvViewModel
    {
        public string NombreArchivo { get; set; }
        public string URL { get; set; }
        public string Errores { get; set; }
    }

    public class ResultadosRicvViewModel
    {
        public IList<RicvViewModel> Items;
    }
}
