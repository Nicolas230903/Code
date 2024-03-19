using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ActivosViewModel
    /// </summary>
    public class ActivosViewModel
    {
        public int ID { get; set; }
        public string FechaCompra { get; set; }
        public string NumeroDeSerie { get; set; }
        public string fechaCompra { get; set; }
        public string vidaUtil { get; set; }

        public string razonSocial { get; set; }
    }

    public class ResultadosActivosViewModel
    {
        public IList<ActivosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}