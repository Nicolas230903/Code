using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio
{
    public class DatosAfipPersonasConGeo
    {
        public string CategoriaImpositiva { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Personeria { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaInicioActividades { get; set; }
        public int IdUsuario { get; set; }
        public string DomicilioFiscalCP { get; set; }
        public int? DomicilioFiscalIdProvincia { get; set; }
        public string DomicilioFiscalProvincia { get; set; }
        public string DomicilioFiscalCiudad { get; set; }
        public string DomicilioFiscalDomicilio { get; set; }
        public string CategoriaMonotributoDescripcionCategoria { get; set; }
        public string CategoriaMonotributoIdCategoria { get; set; }
        public string CategoriaMonotributoIdImpuesto { get; set; }
        public string CategoriaMonotributoPeriodo { get; set; }
        public int? IdProvincia { get; set; }
        public int? IdCiudad { get; set; }
        public string Mensaje { get; set; }
    }
}
