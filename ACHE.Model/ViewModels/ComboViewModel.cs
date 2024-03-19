using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{

    /// <summary>
    /// Summary description for ComboViewModel
    /// </summary>
    public class ComboViewModel
    {
        public string ID { get; set; }
        public string Nombre { get; set; }
    }

    public class Combo2ViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
    }

    public class Combo3ViewModel
    {
        public long ID { get; set; }
        public string Nombre { get; set; }
    }

    public class ResultadosCombo2ViewModel
    {
        public IList<Combo2ViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}