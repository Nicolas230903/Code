using ACHE.FacturaElectronica.WSFacturaElectronica;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ACHE.FacturaElectronica
{
    public class FEComprobante
    {
        private List<FERegistroIVA> _detalleIva = new List<FERegistroIVA>();
        private List<FERegistroTributo> _tributos = new List<FERegistroTributo>();
        private List<FEItemDetalle> _itemsDetalle = new List<FEItemDetalle>();
        private List<FEItemFormasDePago> _itemsFormasDePago = new List<FEItemFormasDePago>();
        private List<FEComprobanteAsociado> _ComprobantesAsociados = new List<FEComprobanteAsociado>();
        private List<FEActividad> _actividades = new List<FEActividad>();
        private List<FEOpcional> _opcionales = new List<FEOpcional>();
        public string IDComprobante { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteDomicilio { get; set; }
        public string ClienteLocalidad { get; set; }
        public string ClienteContacto { get; set; }
        public string ClienteCondiionIva { get; set; }
        public string CondicionVenta { get; set; }
        public string Observaciones { get; set; }
        public string RazonSocial { get; set; }
        public string Domicilio { get; set; }
        public string CiudadProvincia { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string CondicionIva { get; set; }
        public string IIBB { get; set; }
        public string FechaInicioActividades { get; set; }
        public bool Original { get; set; }
        public string Vendedor { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Tipo { get; set; }
        public string NroPedidoDeVenta { get; set; }
        public string NroPresupuesto { get; set; }
        public bool NotaDeCreditoPorServicio { get; set; }
        public string TextoFinalFactura { get; set; }

        public string ConceptoRecibo { get; set; }

        /// <summary>
        /// Es asignado por la AFIP
        /// </summary>
        public int NumeroComprobante { get; set; }

        /// <summary>
        /// Tipo de comprobante a generar
        /// </summary>
        public FETipoComprobante TipoComprobante { get; set; }

        public long Cuit { get; set; }

        /// <summary>
        /// Punto de Venta
        /// </summary>
        public int PtoVta { get; set; }

        /// <summary>
        /// Código de documento identificatorio del comprador
        /// 80	CUIT
        /// 86	CUIL
        /// 87	CDI
        /// 89	LE
        /// 90	LC
        /// 91	CI extranjera
        /// 92	en trámite
        /// 93	Acta nacimiento
        /// 94	Pasaporte
        /// 95	CI Bs. As. RNP
        /// 96	DNI
        /// 99	Sin identificar/venta global diaria
        /// 30	Certificado de Migración
        /// 88	Usado por Anses para Padrón
        /// </summary>
        public int DocTipo { get; set; }

        /// <summary>
        /// Documento identificatorio del comprador
        /// </summary>
        public long DocNro { get; set; }

        public FEConcepto Concepto { get; set; }

        /// <summary>
        /// Fecha del comprobante, para concepto igual a 1, la fecha de emisión del comprobante puede ser hasta 5 días anteriores o posteriores respecto de la fecha de generación; si se indica Concepto igual a 2 ó 3 puede ser hasta 10 días anteriores o posteriores a la fecha de generación. Si no se envía la fecha del comprobante se asignará la fecha de proceso
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Importe total del comprobante, Debe ser igual a Importe neto no gravado + Importe exento + Importe neto gravado + todos los campos de IVA al XX% + Importe de tributos.
        /// </summary>
        public double ImpTotal
        {
            get
            {
                if (TipoComprobante == FETipoComprobante.RECIBO_C || TipoComprobante == FETipoComprobante.FACTURAS_C || TipoComprobante == FETipoComprobante.NOTAS_CREDITO_C || TipoComprobante == FETipoComprobante.NOTAS_DEBITO_C)
                    return ImpNeto;
                else
                    //return ImpNeto + ImpTotConc + TotalIva;
                    return ImpNeto + ImpTotConc + TotalIva + TotalTributos + ImpOpEx;
            }
        }

        /// <summary>
        /// Importe neto no gravado. Debe ser menor o igual a Importe total y no puede ser menor a cero. No puede ser mayor al Importe total de la operación ni menor a cero (0). Para comprobantes tipo C debe ser igual a cero (0). Para comprobantes tipo Bienes Usados – Emisor Monotributista este campo corresponde al importe subtotal.
        /// </summary>
        public double ImpTotConc { get; set; }

        /// <summary>
        /// Importe neto gravado. Debe ser menor o igual a Importe total y no puede ser menor a cero. Para comprobantes tipo C este campo corresponde al Importe del Sub Total. Para comprobantes tipo Bienes Usados – Emisor Monotributista no debe informarse o debe ser igual a cero (0).
        /// </summary>
        public double ImpNeto
        {
            get
            {
                /*if (TipoComprobante == FETipoComprobante.FACTURAS_C)
                    return 0;
                else*/
                //return _itemsDetalle.Sum(i => Math.Round(i.Total, 2));
                
                /* return _itemsDetalle.Sum(i => i.Total);*/

                if (TipoComprobante == FETipoComprobante.COBRANZA)
                    return _itemsDetalle.Sum(i => Math.Round(i.Total, 2)); 
                else
                    //return ImpNeto + ImpTotConc + TotalIva;
                    return _itemsDetalle.Where(w => w.IdTipoIVA > 2).Sum(i => Math.Round(i.Total, 2));
            }
        }

        /// <summary>
        /// Importe exento. Debe ser menor o igual a Importe total y no puede ser menor a cero. Para comprobantes tipo C debe ser igual a cero (0). Para comprobantes tipo Bienes Usados – Emisor Monotributista no debe informarse o debe ser igual a cero (0).
        /// </summary>
        public double ImpOpEx { get; set; }

        /// <summary>
        /// Fecha de inicio del abono para el servicio a facturar. Dato obligatorio para concepto 2 o 3 (Servicios / Productos y Servicios).
        /// </summary>
        public DateTime? FchServDesde { get; set; }

        /// <summary>
        /// Fecha de fin del abono para el servicio a facturar. Dato obligatorio para concepto 2 o 3 (Servicios / Productos y Servicios). FchServHasta no puede ser menor a FchServDesde
        /// </summary>
        public DateTime? FchServHasta { get; set; }

        /// <summary>
        /// Fecha de vencimiento del pago servicio a facturar. Dato obligatorio para concepto 2 o 3 (Servicios / Productos y Servicios). Debe ser igual o posterior a la fecha del comprobante.
        /// </summary>
        public DateTime? FchVtoPago { get; set; }

        public string CodigoMoneda { get; set; }
        public double CotizacionMoneda { get; set; }

        /// <summary>
        /// Es asignado por la AFIP
        /// </summary>
        public string CAE { get; set; }

        /// <summary>
        /// Es asignado por la AFIP
        /// </summary>
        public DateTime FechaVencCAE { get; set; }

        public double TotalIva
        {
            get { return DetalleIva.Sum(i => i.Importe); }
        }

        public double TotalTributos
        {
            get { return Tributos.Sum(i => i.Importe); }
        }

        public List<FERegistroIVA> DetalleIva
        {
            get { return _detalleIva; }
            set { _detalleIva = value; }
        }
        public List<FERegistroTributo> Tributos
        {
            get { return _tributos; }
            set { _tributos = value; }
        }

        public List<FEItemDetalle> ItemsDetalle
        {
            get { return _itemsDetalle; }
            set { _itemsDetalle = value; }
        }

        public List<FEComprobanteAsociado> ComprobantesAsociados
        {
            get { return _ComprobantesAsociados; }
            set { _ComprobantesAsociados = value; }
        }

        public List<FEItemFormasDePago> ItemsFormasDePago
        {
            get { return _itemsFormasDePago; }
            set { _itemsFormasDePago = value; }
        }

        public List<FEActividad> Actividades
        {
            get { return _actividades; }
            set { _actividades = value; }
        }

        public List<FEOpcional> Opcionales
        {
            get { return _opcionales; }
            set { _opcionales = value; }
        }

        public double? DescuentoPorcentaje { get; set; }
        public double? DescuentoImporte { get; set; }

        public Stream ArchivoFactura { get; set; }
    }
}
