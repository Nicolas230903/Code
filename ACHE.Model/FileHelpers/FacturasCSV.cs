using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model
{
    [DelimitedRecord(";")]
    [IgnoreFirst(1)]
    public class FacturasCSV
    {
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public string Email { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string CondicionIva { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }
        public string Web { get; set; }

        public string Fecha { get; set; }
        public string TipoComprobante { get; set; }
        public string PuntoDeVenta { get; set; }
        public string NroComprobante { get; set; }
        public string Modo { get; set; }        
        public string CAE { get; set; }
        public string ImporteNeto { get; set; }
        public string ImporteNoGravado { get; set; }
        public string IVA2700 { get; set; }
        public string IVA2100 { get; set; }
        public string IVA1005 { get; set; }
        public string IVA0500 { get; set; }
        public string IVA0205 { get; set; }
        public string IVA0000 { get; set; }
        public string PercepcionesIVA { get; set; }
        public string PercepcionesIIBB { get; set; }
        public string Total { get; set; }
        public string MontoPagado { get; set; }
        public string FechaDePago { get; set; }
        public string CodigoCuentaContable { get; set; }
        public string Observaciones { get; set; }
        public string CodigoConcepto { get; set; }
    }

    public class FacturasCSVTmp : FacturasCSV
    {
        public string Tipo { get; set; }
        public int IDUsuario { get; set; }
        public int IDPersona { get; set; }
        public int IDProvincia { get; set; }
        public int? IDCiudad { get; set; }
        public string Personeria { get; set; }
        public string resultados { get; set; }
        public DateTime fechaAlta { get; set; }
        public string FechaCAE { get; set; }
        public string Estado { get; set; }
        public int idPlanDeCuenta { get; set; }
        public int IDPuntoDeVenta { get; set; }
        public List<ComprobantesDetalleViewModel> Items { get; set; }
    }
}
