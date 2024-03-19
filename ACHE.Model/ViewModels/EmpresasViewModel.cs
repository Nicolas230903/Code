using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for UsuariosViewModel
    /// </summary>
    public class EmpresasViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string CondicionIva { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public bool? CorreoPortal { get; set; }
        public bool? PortalClientes { get; set; }

        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public string TieneFacturaElectronica { get; set; }
        public string UsaProd { get; set; }
    }

    public class ResultadosEmpresasViewModel
    {
        public IList<EmpresasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
        public string UsuLogiado { get; set; }
        public bool SuperoLimite { get; set; }
    }
}