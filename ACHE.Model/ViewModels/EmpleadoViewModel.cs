using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for BancosViewModel
    /// </summary>
    public class EmpleadoViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CUIT { get; set; }
        public string NroLegajo { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Domicilio { get; set; }
    }

    public class ResultadosEmpleadoViewModel
    {
        public IList<EmpleadoViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}
