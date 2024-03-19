using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for AlertasViewModel
    /// </summary>
    public class AlertasViewModel
    {
        public int ID { get; set; }
        public string AvisoAlerta { get; set; }
        public string Condicion { get; set; }
        public string Importe { get; set; }
    }

    public class ResultadosAlertasViewModel
    {
        public IList<AlertasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}