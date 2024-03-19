using ACHE.Model;
using ACHE.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio
{
    public class ComprasDto
    {
        public string Token { get; set; }
        public int IDCompra { get; set; }
        public int IDPersona { get; set; }
        public string Fecha { get; set; }
        public string NroFactura { get; set; }
        public string Iva { get; set; }
        public string Importe2 { get; set; }
        public string Importe5 { get; set; }
        public string Importe10 { get; set; }
        public string Importe21 { get; set; }
        public string Importe27 { get; set; }
        public string NoGrav { get; set; }
        public string ImporteMon { get; set; }
        public string ImpNacional { get; set; }
        public string ImpMunicipal { get; set; }
        public string ImpInterno { get; set; }
        public string PercepcionIva { get; set; }
        public string Otros { get; set; }
        public string Obs { get; set; }
        public string Tipo { get; set; }
        public string IdCategoria { get; set; }
        public string Rubro { get; set; }
        public string Exento { get; set; }
        public string FechaEmision { get; set; }
        public int IdPlanDeCuenta { get; set; }
        public List<JurisdiccionesViewModel> Jurisdicciones { get; set; }
        public string FechaPrimerVencimiento { get; set; }
        public string FechaSegundoVencimiento { get; set; }
        public string Adjunto { get; set; }
    }
}