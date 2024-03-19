using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for ComprobantesViewModel
    /// </summary>
    public class ComprobantesViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Modo { get; set; }
        public string CAE { get; set; }
        public string ImporteTotalNeto { get; set; }
        public string ImporteTotalBruto { get; set; }
        public string PuedeAdm { get; set; }
        public string Resultado { get; set; }
        public string MensajeResultado { get; set; }
        public string ComprobanteOrigen { get; set; }
        public string IVA { get; set; }
    }

    /// <summary>
    /// Summary description for ResultadosComprobantesViewModel
    /// </summary>
    public class ResultadosComprobantesViewModel
    {
        public IList<ComprobantesViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }

    /// <summary>
    /// Summary description for ComprobantesEditViewModel
    /// </summary>
    public class ComprobantesEditViewModel
    {
        public int ID { get; set; }
        public int IDPersona { get; set; }
        public string Fecha { get; set; }
        public string FechaVencimiento { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public int TipoConcepto { get; set; }
        public string Modo { get; set; }
        public string CondicionVenta { get; set; }
        public int IDPuntoVenta { get; set; }
        public string Observaciones { get; set; }
        public string Nombre { get; set; }
        public string Vendedor { get; set; }
        public string Envio { get; set; }
        public string FechaEntrega { get; set; }
        public string FechaAlta { get; set; }
        public int IDDomicilio { get; set; }
        public int IDTransporte { get; set; }
        public int IDTransportePersona { get; set; }
        public int IDUsuarioAdicional { get; set; }
        public string Estado { get; set; }
        public int IDComprobanteAsociado { get; set; }
        public string NumeroComprobanteAsociado { get; set; }
        public int IDComprobanteVinculado { get; set; }
        public string NumeroComprobanteVinculado { get; set; }
        public int IDCompraVinculada { get; set; }
        public string Descuento { get; set; }
        public int IDActividad { get; set; }
        public string ModalidadPagoAFIP { get; set; }
        public PersonasEditViewModel Personas { get; set; }
    }

    public class PersonasEditViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public string CondicionIva { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool? esAgenteRetencion { get; set; }

        public string PercepcionIIBB { get; set; }
        public int IDJuresdiccion { get; set; }
        public string PercepcionIVA { get; set; }
        public string ImporteNoGravado { get; set; }
    }
}