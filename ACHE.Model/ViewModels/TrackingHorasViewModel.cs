using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for BancosViewModel
    /// </summary>
    public class TrackingHorasViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Fecha { get; set; }
        public string Horas { get; set; }
        public string Tarea { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public string NombreUsuario { get; set; }

        public int CantHorasFacturables { get; set; }
        public int  CantHorasNOFacturables { get; set; }

        public int  SubTotal
        {
            get
            {
                return CantHorasFacturables + CantHorasNOFacturables;
            }
        }
    }

    public class ResultadosTrackingHorasViewModel
    {
        public IList<TrackingHorasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }

        public int TotalHorasFacturables { get { return (Items.Sum(x => x.CantHorasFacturables)); } }
        public int TotalHorasNoFacturables { get { return (Items.Sum(x => x.CantHorasNOFacturables)); } }

        public int Total { get { return (Items.Sum(x => x.SubTotal)); } }
        
    }
}