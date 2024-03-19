using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for UsuariosViewModel
    /// </summary>
    public class UsuariosViewModel
    {
        public int ID { get; set; }
        public string Tipo { get; set; }
        public string Activo { get; set; }
        public string Email { get; set; }
    }

    public class ResultadosUsuariosViewModel
    {
        public IList<UsuariosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
    public class UsuariosAPIViewModel
    {
        public string RazonSocial { get; set; }
        public string CondicionIva { get; set; }
        public string CUIT { get; set; }
        public string Email { get; set; }
        public string EmailAlertas { get; set; }
        public DateTime? FechaInicioActividades { get; set; }
        public string Personeria { get; set; }
        public bool? ExentoIIBB { get; set; }
        public string IIBB { get; set; }
        public string Contacto { get; set; }
        public string Celular { get; set; }
        public int IDProvincia { get; set; }
        public int IDCiudad { get; set; }
        public string Domicilio { get; set; }
        public string PisoDepto { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public bool? EsAgentePercepcionIVA { get; set; }
        public string IDJurisdiccion { get; set; }
        public bool? EsAgentePercepcionIIBB { get; set; }
        public bool? EsAgenteRetencion { get; set; }
    }
}