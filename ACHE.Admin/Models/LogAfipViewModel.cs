using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Admin.Models
{
    public class LogAfipViewModel
    {
        public int ID { get; set; }
        public string Entidad { get; set; }
        public string Url { get; set; }
        public string Nombre { get; set; }
        public string Mensaje { get; set; }
        public string FechaEmision { get; set; }
        public string UsuarioCUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Envio { get; set; }
        public string Respuesta { get; set; }
        public string RespuestaExitosa { get; set; }
        public string FechaRespuesta { get; set; }        
    }

    public class ResultadosLogAfipViewModel
    {
        public IList<LogAfipViewModel> Items { get; set; }
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }

}