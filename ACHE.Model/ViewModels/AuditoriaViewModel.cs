using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ComprobantesViewModel
    /// </summary>
    public class AuditoriaViewModel
    {
        public int ID { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNuevo { get; set; }
        public string Tabla { get; set; }
        public string Columna { get; set; }
        public string Identificador { get; set; }
        public string Fecha { get; set; }
        public string Usuario { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosComprobantesViewModel
    /// </summary>
    public class ResultadosAuditoriaViewModel
    {
        public IList<AuditoriaViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }    
}