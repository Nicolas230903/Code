using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for PersonasViewModel
    /// </summary>
    public class CuentaCorrienteViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public string CondicionIva { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string Saldo { get; set; }
    }

    public class ResultadosCuentaCorrienteViewModel
    {
        public IList<CuentaCorrienteViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}
