using System;
using System.Collections.Generic;
using System.Linq;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Productos;
using ACHE.Model.Negocio;
using ACHE.FacturaElectronica;
using System.Configuration;
using System.IO;
using ACHE.Negocio.Common;
using ACHE.Negocio.Contabilidad;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;
using ACHE.Negocio.Helper;
using System.Reflection;

namespace ACHE.Negocio.Facturacion
{
    public enum ComprobanteModo
    {
        Previsualizar = 1,
        Generar = 2,
        GenerarRemito = 3
    }

    public static class ComprobantesCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static FETipoComprobante ObtenerTipoComprobante(string tipo)
        {
            FETipoComprobante tipoComprobante = new FETipoComprobante();
            switch (tipo)
            {
                case "FCA":
                    tipoComprobante = FETipoComprobante.FACTURAS_A;
                    break;
                case "FCB":
                    tipoComprobante = FETipoComprobante.FACTURAS_B;
                    break;
                case "FCC":
                    tipoComprobante = FETipoComprobante.FACTURAS_C;
                    break;
                case "NCA":
                    tipoComprobante = FETipoComprobante.NOTAS_CREDITO_A;
                    break;
                case "NCB":
                    tipoComprobante = FETipoComprobante.NOTAS_CREDITO_B;
                    break;
                case "NCC":
                    tipoComprobante = FETipoComprobante.NOTAS_CREDITO_C;
                    break;
                case "NDA":
                    tipoComprobante = FETipoComprobante.NOTAS_DEBITO_A;
                    break;
                case "NDB":
                    tipoComprobante = FETipoComprobante.NOTAS_DEBITO_B;
                    break;
                case "NDC":
                    tipoComprobante = FETipoComprobante.NOTAS_DEBITO_C;
                    break;
                case "RCA":
                    tipoComprobante = FETipoComprobante.RECIBO_A;
                    break;
                case "RCB":
                    tipoComprobante = FETipoComprobante.RECIBO_B;
                    break;
                case "RCC":
                    tipoComprobante = FETipoComprobante.RECIBO_C;
                    break;
                case "FCAMP":
                    tipoComprobante = FETipoComprobante.FACTURAS_A_MiPyMEs;
                    break;
                case "FCBMP":
                    tipoComprobante = FETipoComprobante.FACTURAS_B_MiPyMEs;
                    break;
                case "FCCMP":
                    tipoComprobante = FETipoComprobante.FACTURAS_C_MiPyMEs;
                    break;
                case "NCAMP":
                    tipoComprobante = FETipoComprobante.NOTA_CREDITO_A_MiPyMEs;
                    break;
                case "NCBMP":
                    tipoComprobante = FETipoComprobante.NOTA_CREDITO_B_MiPyMEs;
                    break;
                case "NCCMP":
                    tipoComprobante = FETipoComprobante.NOTA_CREDITO_C_MiPyMEs;
                    break;
                case "NDAMP":
                    tipoComprobante = FETipoComprobante.NOTA_DEBITO_A_MiPyMEs;
                    break;
                case "NDBMP":
                    tipoComprobante = FETipoComprobante.NOTA_DEBITO_B_MiPyMEs;
                    break;
                case "NDCMP":
                    tipoComprobante = FETipoComprobante.NOTA_DEBITO_C_MiPyMEs;
                    break;
                case "SIN":
                case "COT":
                    tipoComprobante = FETipoComprobante.SIN_DEFINIR;
                    break;
                case "PRE":
                    tipoComprobante = FETipoComprobante.PRESUPUESTO;
                    break;
            }
            return tipoComprobante;
        }

        public static Comprobantes Guardar(ACHEEntities dbContext, ComprobanteCartDto compCart)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(compCart.TipoComprobante))
                    throw new CustomException("El tipo de comprobante es obligatorio");
                else if (string.IsNullOrWhiteSpace(compCart.Modo))
                    throw new CustomException("El modo es obligatorio");
                else if (string.IsNullOrWhiteSpace(compCart.CondicionVenta))
                    throw new CustomException("La condición de venta es obligatoria");
                else if (string.IsNullOrWhiteSpace(compCart.TipoConcepto))
                    throw new CustomException("El tipo de concepto es obligatorio");

                string[] listaTipoComprobanteFactura = { "FCA", "FCB", "FCC", "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };

                int nro = 0;
                var usu = dbContext.Usuarios.Where(x => x.IDUsuario == compCart.IDUsuario).FirstOrDefault();
                Personas persona = dbContext.Personas.Where(x => x.IDPersona == compCart.IDPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                if (persona == null)
                    throw new CustomException("El cliente/proveedor es inexistente");

                bool ComprobanteExistente = false;
                Comprobantes entity;
                Comprobantes entityOld = new Comprobantes();
                List<ComprobantesDetalle> entityCd = null;
                List<ComprobantesDetalle> entityCdOld = new List<ComprobantesDetalle>();
                if (compCart.IDComprobante > 0)
                {
                    entity = dbContext.Comprobantes.Include("ComprobantesDetalle").Where(x => x.IDComprobante == compCart.IDComprobante && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    entityCd = dbContext.ComprobantesDetalle.Where(w => w.IDComprobante == entity.IDComprobante).ToList();
                    //entityOld = entity.DeepCopy();

                    //Agregar en la clase
                    //public Comprobantes DeepCopy()
                    //{
                    //    Comprobantes temp = (Comprobantes)this.MemberwiseClone();
                    //    return temp;
                    //}

                    //foreach (var v in entityCd)
                    //{
                    //    ComprobantesDetalle d = v.DeepCopy();
                    //    entityCdOld.Add(d);
                    //}

                    //Agregar en la clase
                    //public ComprobantesDetalle DeepCopy()
                    //{
                    //    ComprobantesDetalle temp = (ComprobantesDetalle)this.MemberwiseClone();
                    //    return temp;
                    //}

                    nro = Convert.ToInt32(compCart.Numero);

                    if (compCart.TipoComprobante.Equals("PDV"))
                    {
                        if (entity.IdComprobanteVinculado != null)
                        {
                            throw new CustomException("Ya se generó un pedido de compra vinculado. No se puede modificar el pedido de venta.");
                        }
                    }

                    if (compCart.TipoComprobante.Equals("PDC"))
                    {
                        var entityVinc = dbContext.Comprobantes.Where(x => x.IdComprobanteVinculado == entity.IDComprobante && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (entityVinc != null)
                        {
                            throw new CustomException("El comprobante de compra ya esta vinculado a un comprobante de compra, no se puede modificar.");
                        }
                    }
                 
                }                                
                else
                {                    
                    if (compCart.TipoComprobante.Equals("PDV") || compCart.TipoComprobante.Equals("EDA"))                    
                        nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante(compCart.TipoComprobante, usu.IDUsuario, Convert.ToInt32(compCart.IDPuntoVenta)));                    
                    else
                        nro = Convert.ToInt32(compCart.Numero);


                    if (!listaTipoComprobanteFactura.Contains(compCart.TipoComprobante))
                    {
                        ComprobanteExistente = dbContext.Comprobantes
                            .Any(x => x.Tipo == compCart.TipoComprobante
                            && x.Numero == nro && x.IDPuntoVenta == compCart.IDPuntoVenta
                            && x.IDUsuario == usu.IDUsuario);
                    }                 

                    entity = new Comprobantes();
                    entity.FechaAlta = DateTime.Now;
                    entity.IDUsuario = usu.IDUsuario;

                }

                if (ComprobanteExistente)
                    throw new Exception("Ya existe el tipo de comprobante y nro comprobante para el proveedor/Cliente seleccionado");

                entity.IDPersona = compCart.IDPersona;
                entity.Tipo = compCart.TipoComprobante;
                entity.Modo = (compCart.TipoComprobante == "COT") ? "O" : compCart.Modo;
                entity.FechaComprobante = compCart.FechaComprobante;
                entity.TipoDestinatario = persona.Tipo.ToUpper();
                entity.CondicionVenta = compCart.CondicionVenta;
                entity.NroDocumento = persona.NroDocumento;
                entity.TipoDocumento = persona.TipoDocumento;
                entity.FechaVencimiento = compCart.FechaVencimiento;
                entity.IDPuntoVenta = compCart.IDPuntoVenta;
                entity.Nombre = compCart.Nombre;
                entity.Vendedor = compCart.Vendedor;
                entity.Envio = compCart.Envio;
                if(compCart.FechaEntrega.Year != 1)
                    entity.FechaEntrega = compCart.FechaEntrega;

                if (compCart.ProcesoCompraAutomatica != null)
                    entity.ProcesoCompraAutomatica = compCart.ProcesoCompraAutomatica;

                entity.PercepcionIVA = compCart.PercepcionIVA;
                entity.PercepcionIIBB = compCart.PercepcionIIBB;
                entity.ImporteNoGravado = compCart.ImporteNoGravado;
                entity.ImporteExento = compCart.ImporteExento;
                entity.Descuento = compCart.Descuento;

                if (compCart.IDJuresdiccion > 0)
                    entity.IDJurisdiccion = compCart.IDJuresdiccion;

                if (compCart.Modo != "E")
                {
                    entity.Numero = nro;
                    entity.FechaProceso = DateTime.Now;
                }
                entity.TipoConcepto = int.Parse(compCart.TipoConcepto);
                entity.Observaciones = compCart.Observaciones;
                entity.Estado = compCart.Estado;

                //if (entity.ComprobantesDetalle.Any())
                //    ConceptosCommon.SumarStock(dbContext, entity.ComprobantesDetalle.ToList());

                if (entity.ComprobantesDetalle.Any())
                    dbContext.ComprobantesDetalle.RemoveRange(entity.ComprobantesDetalle);

                if (compCart.Adjunto != null)                   
                    entity.Adjunto = Base64.Decode(compCart.Adjunto);


                if (entity.Tipo == "FCAMP" || entity.Tipo == "FCBMP" || entity.Tipo == "FCCMP")
                {
                    if (!compCart.ModalidadPagoAfip.Equals(""))
                        entity.ModalidadPagoAFIP = compCart.ModalidadPagoAfip;
                    else
                        throw new CustomException("Debe seleccionar una modalidad de pago AFIP.");

                    if (usu.CBU != null)
                        entity.CBU = usu.CBU;
                    else
                        throw new CustomException("El usuario no tiene informado un CBU, cargarlo en 'Mis Datos'.");

                }


                if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC"
                    || entity.Tipo == "NCAMP" || entity.Tipo == "NCBMP" || entity.Tipo == "NCCMP" || entity.Tipo == "NDAMP" || entity.Tipo == "NDBMP" || entity.Tipo == "NDCMP")
                {
                    if (compCart.IDComprobanteAsociado != 0)
                        entity.IdComprobanteAsociado = compCart.IDComprobanteAsociado;
                    else
                        throw new CustomException("Debe seleccionar una comprobante asociado cuando el comprobante es una Nota de credito/debito.");
                }

                if (entity.Tipo == "EDA")
                {
                    if (compCart.IDDomicilio != 0)
                        entity.IdDomicilio = compCart.IDDomicilio;
                    else
                        throw new CustomException("Debe seleccionar un domicilio de entrega.");

                    if (compCart.IDTransporte != 0)
                        entity.IdTransporte = compCart.IDTransporte;

                }

                if (entity.Tipo == "DDC")
                {
                    if (compCart.IDCompraVinculada != 0)
                        entity.IdCompraVinculada = compCart.IDCompraVinculada;
                    else
                        throw new CustomException("Error al obtener el codigo de comprobante de compra.");

                }

                if (compCart.IDTransportePersona != 0)
                    entity.IdTransportePersona = compCart.IDTransportePersona;

                if (compCart.IDActividad != 0)
                    entity.IdActividad = compCart.IDActividad;
                else
                    entity.IdActividad = UsuarioCommon.ObtenerActividades(usu.IDUsuario).FirstOrDefault().ID;

                decimal totalNeto = 0;
                decimal totalBruto = 0;

                List<StockAuditoria> lsa = new List<StockAuditoria>();


                foreach (var det in compCart.Items)
                {
                    ComprobantesDetalle compDet = new ComprobantesDetalle();
                    compDet.IDComprobante = entity.IDComprobante;
                    compDet.PrecioUnitario = det.PrecioUnitario;
                    compDet.Iva = det.Iva;
                    compDet.IdTipoIVA = det.IdTipoIva;
                    compDet.PrecioUnitarioIVA = det.PrecioUnitarioConIva - det.PrecioUnitarioSinIVA;
                    compDet.PrecioTotalIVA = det.TotalConIva - det.TotalSinIva;
                    compDet.Concepto = det.Concepto;
                    compDet.Cantidad = det.Cantidad;
                    compDet.Bonificacion = det.Bonificacion;
                    compDet.SubTotalAjustado = det.SubTotalAjustado;
                    if (det.IDPlanDeCuenta > 0)
                        compDet.IDPlanDeCuenta = det.IDPlanDeCuenta;

                    if (det.IDConcepto != null && det.IDConcepto != 0)                    
                        compDet.IDConcepto = det.IDConcepto;                    
                    else {
                        string tipo = "";
                        if (compCart.TipoConcepto.Equals("1") || compCart.TipoConcepto.Equals("3"))
                            tipo = "P";
                        else
                            tipo = "S";               
                        compDet.IDConcepto = ConceptosCommon.GuardarConcepto(0, det.Concepto, "", tipo, "", "A",
                                                                         det.PrecioUnitario.ToString(), det.IdTipoIva.ToString(), "0", "", det.PrecioUnitario.ToString(),
                                                                         "0", 0, usu.IDUsuario);                                                   
                        if (det.IDAbonos != null && det.IDAbonos != 0)
                            compDet.IDAbono = det.IDAbonos;
                    }

                    PuntosDeVenta pdVenta = dbContext.PuntosDeVenta.Where(w => w.IDPuntoVenta == compCart.IDPuntoVenta).FirstOrDefault();
                    if(pdVenta != null)
                    {
                        Conceptos c = dbContext.Conceptos.Where(w => w.IDConcepto == compDet.IDConcepto).FirstOrDefault();
                        if (c != null)
                        {
                            StockAuditoria sa = new StockAuditoria();
                            sa.IdConcepto = (int)compDet.IDConcepto;
                            sa.Accion = (entity.Tipo.Equals("PDV") ? "Venta - ": "Compra - ") + pdVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000");
                            sa.FechaAlta = DateTime.Now;
                            sa.IdUsuario = usu.IDUsuario;
                            sa.Cantidad = compDet.Cantidad;
                            sa.StockAnterior = (decimal)c.Stock;
                            if (entity.Tipo.Equals("PDV"))
                            {
                                sa.StockNuevo = (decimal)c.Stock - det.Cantidad;
                            }
                            else if (entity.Tipo.Equals("PDC"))
                            {
                                sa.StockNuevo = (decimal)c.Stock + det.Cantidad;
                            }
                            lsa.Add(sa);
                        }
                    }

                    if (det.Ajuste)
                    {
                        totalNeto += det.SubTotalAjustado;
                        totalBruto += det.SubTotalAjustado;
                    }
                    else
                    {
                        totalNeto += det.TotalConIva;
                        totalBruto += det.TotalSinIva;
                    }
                    
                    entity.ComprobantesDetalle.Add(compDet);


                    if (entity.Tipo == "PDC")
                        ConceptosCommon.AplicarRentabilidad((int)compDet.IDConcepto, det.PrecioUnitario, usu.IDUsuario);

                }

                var Tributos = compCart.GetPercepcionIIBB() + compCart.GetPercepcionIVA();
                if (usu.CUIT.Equals("30716909839"))
                {
                    entity.ImporteTotalBruto = totalBruto / 1000;
                    entity.ImporteTotalNeto = Math.Round((totalNeto + Tributos) / 1000, 2);
                }
                else
                {
                    entity.ImporteTotalBruto = totalBruto;
                    entity.ImporteTotalNeto = Math.Round(totalNeto + Tributos, 2);
                }

                entity.ImporteTotalBruto = entity.ImporteTotalBruto - ((entity.ImporteTotalBruto * entity.Descuento) / 100);
                entity.ImporteTotalNeto = entity.ImporteTotalNeto - ((entity.ImporteTotalNeto * entity.Descuento) / 100);

                if (compCart.IDComprobante > 0)
                {
                    if (entity.IdComprobanteVinculado != null && !entity.Tipo.Equals("EDA"))
                    {
                        //busco todos los pdv 
                        List<Comprobantes> entityComprobanteVinculado = dbContext.Comprobantes.Where(x => x.IDComprobante == entity.IdComprobanteVinculado && x.IDUsuario == usu.IDUsuario).ToList();

                        foreach(Comprobantes pdv in entityComprobanteVinculado)
                        {
                            // busco la factura
                            Comprobantes FC = dbContext.Comprobantes.Where(w => w.IdComprobanteVinculado == pdv.IDComprobante).FirstOrDefault();
                            if (FC != null)
                            {
                                // busco si tiene una nota de credito
                                Comprobantes NC = dbContext.Comprobantes.Where(w => w.IdComprobanteAsociado == FC.IDComprobante).FirstOrDefault();
                                if (NC == null)
                                {
                                    if (pdv.ImporteTotalNeto < entity.ImporteTotalNeto)
                                    {
                                        throw new CustomException("Monto Neto Factura $" + entity.ImporteTotalNeto.ToString("N2") + " - Importe Neto Pedido $ " + pdv.ImporteTotalNeto.ToString("N2") + ". Una Factura vinculada no puede ser superior al pedido.");
                                    }
                                    else
                                    {
                                        var cd = dbContext.CobranzasDetalle.Where(w => w.IDComprobante == pdv.IDComprobante).ToList();
                                        if (cd.Count == 0)
                                        {
                                            pdv.Saldo = pdv.ImporteTotalNeto - entity.ImporteTotalNeto;
                                            dbContext.SaveChanges();
                                        }
                                            
                                    }
                                }
                            }
                        }

                    }
                }

                //Comisiones
                if (compCart.IDUsuarioAdicional != 0)
                {
                    UsuariosAdicionales usrAd = dbContext.UsuariosAdicionales.Where(w => w.IDUsuario == usu.IDUsuario &&  w.IDUsuarioAdicional == compCart.IDUsuarioAdicional).FirstOrDefault();
                    if (usrAd != null)
                    {
                        entity.ImporteComisionVendedor = (usrAd.PorcentajeComision / 100) * entity.ImporteTotalNeto;
                        entity.IdUsuarioAdicional = usrAd.IDUsuarioAdicional;
                    }
                    else
                    {
                        entity.ImporteComisionVendedor = (usu.PorcentajeComision / 100) * entity.ImporteTotalNeto;
                        entity.IdUsuarioAdicional = usu.IDUsuario;
                    }
                }
                //

                if (entity.Tipo != "EDA")
                {
                    if (totalNeto == 0)
                        throw new CustomException("No puede generar una factura con Importe 0");
                }              
                               
                if (compCart.IDComprobante > 0)
                {             
                    if (entity.Tipo != "EDA")
                    {
                        if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC"
                                || entity.Tipo == "NCAMP" || entity.Tipo == "NCBMP" || entity.Tipo == "NCCMP" || entity.Tipo == "NDAMP" 
                                || entity.Tipo == "NDBMP" || entity.Tipo == "NDCMP" || entity.Tipo == "PDC")
                            ConceptosCommon.SumarStockActualizado(dbContext, entity.ComprobantesDetalle.ToList(), compCart.IDComprobante);
                        else
                            ConceptosCommon.RestarStockActualizado(dbContext, entity.ComprobantesDetalle.ToList(), compCart.IDComprobante);

                        dbContext.SaveChanges();
                        dbContext.ActualizarSaldosPorComprobante(compCart.IDComprobante);
                    }
                    else
                    {
                        ConceptosCommon.RestarStockFisicoActualizado(dbContext, entity.ComprobantesDetalle.ToList(), compCart.IDComprobante);
                        dbContext.SaveChanges();
                    }

                    //bool audit;
                    //audit = entity.IDPersona != entityOld.IDPersona ? Auditoria.Insert(dbContext, entityOld.IDPersona.ToString(), entity.IDPersona.ToString(), "Comprobantes", "IDPersona", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Tipo != entityOld.Tipo ? Auditoria.Insert(dbContext, entityOld.Tipo.ToString(), entity.Tipo.ToString(), "Comprobantes", "Tipo", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Modo != entityOld.Modo ? Auditoria.Insert(dbContext, entityOld.Modo.ToString(), entity.Modo.ToString(), "Comprobantes", "Modo", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.FechaComprobante != entityOld.FechaComprobante ? Auditoria.Insert(dbContext, entityOld.FechaComprobante.ToString(), entity.FechaComprobante.ToString(), "Comprobantes", "FechaComprobante", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.TipoDestinatario != entityOld.TipoDestinatario ? Auditoria.Insert(dbContext, entityOld.TipoDestinatario.ToString(), entity.TipoDestinatario.ToString(), "Comprobantes", "TipoDestinatario", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.CondicionVenta != entityOld.CondicionVenta ? Auditoria.Insert(dbContext, entityOld.CondicionVenta.ToString(), entity.CondicionVenta.ToString(), "Comprobantes", "CondicionVenta", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.NroDocumento != entityOld.NroDocumento ? Auditoria.Insert(dbContext, entityOld.NroDocumento.ToString(), entity.NroDocumento.ToString(), "Comprobantes", "NroDocumento", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.TipoDocumento != entityOld.TipoDocumento ? Auditoria.Insert(dbContext, entityOld.TipoDocumento.ToString(), entity.TipoDocumento.ToString(), "Comprobantes", "TipoDocumento", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.FechaVencimiento != entityOld.FechaVencimiento ? Auditoria.Insert(dbContext, entityOld.FechaVencimiento.ToString(), entity.FechaVencimiento.ToString(), "Comprobantes", "FechaVencimiento", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.IDPuntoVenta != entityOld.IDPuntoVenta ? Auditoria.Insert(dbContext, entityOld.IDPuntoVenta.ToString(), entity.IDPuntoVenta.ToString(), "Comprobantes", "IDPuntoVenta", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Nombre != entityOld.Nombre ? Auditoria.Insert(dbContext, entityOld.Nombre.ToString(), entity.Nombre.ToString(), "Comprobantes", "Nombre", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Vendedor != entityOld.Vendedor ? Auditoria.Insert(dbContext, entityOld.Vendedor.ToString(), entity.Vendedor.ToString(), "Comprobantes", "Vendedor", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Envio != entityOld.Envio ? Auditoria.Insert(dbContext, entityOld.Envio.ToString(), entity.Envio.ToString(), "Comprobantes", "Envio", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.FechaEntrega != entityOld.FechaEntrega ? Auditoria.Insert(dbContext, entityOld.FechaEntrega.ToString(), entity.FechaEntrega.ToString(), "Comprobantes", "FechaEntrega", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.ProcesoCompraAutomatica != entityOld.ProcesoCompraAutomatica ? Auditoria.Insert(dbContext, entityOld.ProcesoCompraAutomatica.ToString(), entity.ProcesoCompraAutomatica.ToString(), "Comprobantes", "ProcesoCompraAutomatica", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.PercepcionIVA != entityOld.PercepcionIVA ? Auditoria.Insert(dbContext, entityOld.PercepcionIVA.ToString(), entity.PercepcionIVA.ToString(), "Comprobantes", "PercepcionIVA", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.PercepcionIIBB != entityOld.PercepcionIIBB ? Auditoria.Insert(dbContext, entityOld.PercepcionIIBB.ToString(), entity.PercepcionIIBB.ToString(), "Comprobantes", "PercepcionIIBB", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.ImporteNoGravado != entityOld.ImporteNoGravado ? Auditoria.Insert(dbContext, entityOld.ImporteNoGravado.ToString(), entity.ImporteNoGravado.ToString(), "Comprobantes", "ImporteNoGravado", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.IDJurisdiccion != entityOld.IDJurisdiccion ? Auditoria.Insert(dbContext, entityOld.IDJurisdiccion.ToString(), entity.IDJurisdiccion.ToString(), "Comprobantes", "IDJurisdiccion", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Numero != entityOld.Numero ? Auditoria.Insert(dbContext, entityOld.Numero.ToString(), entity.Numero.ToString(), "Comprobantes", "Numero", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.FechaProceso != entityOld.FechaProceso ? Auditoria.Insert(dbContext, entityOld.FechaProceso.ToString(), entity.FechaProceso.ToString(), "Comprobantes", "FechaProceso", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.TipoConcepto != entityOld.TipoConcepto ? Auditoria.Insert(dbContext, entityOld.TipoConcepto.ToString(), entity.TipoConcepto.ToString(), "Comprobantes", "TipoConcepto", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Observaciones != entityOld.Observaciones ? Auditoria.Insert(dbContext, entityOld.Observaciones.ToString(), entity.Observaciones.ToString(), "Comprobantes", "Observaciones", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Estado != entityOld.Estado ? Auditoria.Insert(dbContext, entityOld.Estado.ToString(), entity.Estado.ToString(), "Comprobantes", "Estado", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.Adjunto != entityOld.Adjunto ? Auditoria.Insert(dbContext, entityOld.Adjunto.ToString(), entity.Adjunto.ToString(), "Comprobantes", "Adjunto", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.IdComprobanteAsociado != entityOld.IdComprobanteAsociado ? Auditoria.Insert(dbContext, entityOld.IdComprobanteAsociado.ToString(), entity.IdComprobanteAsociado.ToString(), "Comprobantes", "IdComprobanteAsociado", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.IdDomicilio != entityOld.IdDomicilio ? Auditoria.Insert(dbContext, entityOld.IdDomicilio.ToString(), entity.IdDomicilio.ToString(), "Comprobantes", "IdDomicilio", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.IdTransporte != entityOld.IdTransporte ? Auditoria.Insert(dbContext, entityOld.IdTransporte.ToString(), entity.IdTransporte.ToString(), "Comprobantes", "IdTransporte", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //foreach(var d in entity.ComprobantesDetalle)
                    //{
                    //    foreach (var e in entityCdOld)
                    //    {
                    //        if (d.IDConcepto == e.IDConcepto)
                    //        {
                    //            audit = d.PrecioUnitario != e.PrecioUnitario ? Auditoria.Insert(dbContext, e.PrecioUnitario.ToString(), d.PrecioUnitario.ToString(), "ComprobantesDetalle", "PrecioUnitario", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.Iva != e.Iva ? Auditoria.Insert(dbContext, e.Iva.ToString(), d.Iva.ToString(), "ComprobantesDetalle", "Iva", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.PrecioUnitarioIVA != e.PrecioUnitarioIVA ? Auditoria.Insert(dbContext, e.PrecioUnitarioIVA.ToString(), d.PrecioUnitarioIVA.ToString(), "ComprobantesDetalle", "PrecioUnitarioIVA", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.PrecioTotalIVA != e.PrecioTotalIVA ? Auditoria.Insert(dbContext, e.PrecioTotalIVA.ToString(), d.PrecioTotalIVA.ToString(), "ComprobantesDetalle", "PrecioTotalIVA", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.Concepto != e.Concepto ? Auditoria.Insert(dbContext, e.Concepto.ToString(), d.Concepto.ToString(), "ComprobantesDetalle", "Concepto", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.Cantidad != e.Cantidad ? Auditoria.Insert(dbContext, e.Cantidad.ToString(), d.Cantidad.ToString(), "ComprobantesDetalle", "Cantidad", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.Bonificacion != e.Bonificacion ? Auditoria.Insert(dbContext, e.Bonificacion.ToString(), d.Bonificacion.ToString(), "ComprobantesDetalle", "Bonificacion", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.IDPlanDeCuenta != e.IDPlanDeCuenta ? Auditoria.Insert(dbContext, e.IDPlanDeCuenta.ToString(), d.IDPlanDeCuenta.ToString(), "ComprobantesDetalle", "IDPlanDeCuenta", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //            audit = d.IDAbono != e.IDAbono ? Auditoria.Insert(dbContext, e.IDAbono.ToString(), d.IDAbono.ToString(), "ComprobantesDetalle", "IDAbono", null, e.IDDetalle.ToString(), usu.IDUsuario) : false;
                    //        }
                    //    }
                    //}
                    //audit = entity.ImporteTotalBruto != entityOld.ImporteTotalBruto ? Auditoria.Insert(dbContext, entityOld.ImporteTotalBruto.ToString(), entity.ImporteTotalBruto.ToString(), "Comprobantes", "ImporteTotalBruto", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                    //audit = entity.ImporteTotalNeto != entityOld.ImporteTotalNeto ? Auditoria.Insert(dbContext, entityOld.ImporteTotalNeto.ToString(), entity.ImporteTotalNeto.ToString(), "Comprobantes", "ImporteTotalNeto", null, entityOld.IDComprobante.ToString(), usu.IDUsuario) : false;
                                                                                          
                }
                else
                {
                    if (entity.Tipo != "EDA")
                    {
                        if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC"
                                || entity.Tipo == "NCAMP" || entity.Tipo == "NCBMP" || entity.Tipo == "NCCMP" || entity.Tipo == "NDAMP"
                                || entity.Tipo == "NDBMP" || entity.Tipo == "NDCMP" || entity.Tipo == "PDC")
                            ConceptosCommon.SumarStock(dbContext, entity.ComprobantesDetalle.ToList());
                        else
                            ConceptosCommon.RestarStock(dbContext, entity.ComprobantesDetalle.ToList());

                        if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC"
                                || entity.Tipo == "NCAMP" || entity.Tipo == "NCBMP" || entity.Tipo == "NCCMP" || entity.Tipo == "NDAMP"
                                || entity.Tipo == "NDBMP" || entity.Tipo == "NDCMP")
                        {
                            Comprobantes fc = dbContext.Comprobantes.Where(w => w.IDComprobante == compCart.IDComprobanteAsociado).FirstOrDefault();
                            if (fc != null)
                            {
                                Comprobantes pdv = dbContext.Comprobantes.Where(w => w.IDComprobante == fc.IdComprobanteAsociado).FirstOrDefault();
                                if (pdv != null)
                                {
                                    pdv.Saldo = pdv.Saldo + entity.ImporteTotalNeto;
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC"
                                || entity.Tipo == "NCAMP" || entity.Tipo == "NCBMP" || entity.Tipo == "NCCMP" || entity.Tipo == "NDAMP"
                                || entity.Tipo == "NDBMP" || entity.Tipo == "NDCMP")
                        {
                            Comprobantes fc = dbContext.Comprobantes.Where(w => w.IDComprobante == compCart.IDComprobanteAsociado).FirstOrDefault();
                            if (fc != null)
                            {
                                Comprobantes pdv = dbContext.Comprobantes.Where(w => w.IDComprobante == fc.IdComprobanteAsociado).FirstOrDefault();
                                if (pdv != null)
                                {
                                    pdv.Saldo = pdv.Saldo - entity.ImporteTotalNeto;
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        entity.Saldo = entity.ImporteTotalNeto;

                    }
                    else
                    {
                        ConceptosCommon.RestarStockFisico(dbContext, entity.ComprobantesDetalle.ToList());
                    }

                    dbContext.Comprobantes.Add(entity);
                    dbContext.SaveChanges();

                }

                List<StockAuditoria> lsad = new List<StockAuditoria>();
                foreach (StockAuditoria s in lsa)
                {
                    StockAuditoria sa = new StockAuditoria();
                    sa.IdConcepto = s.IdConcepto;
                    sa.Accion = s.Accion;
                    sa.idComprobante = entity.IDComprobante;
                    sa.FechaAlta = s.FechaAlta;
                    sa.IdUsuario = s.IdUsuario;
                    sa.Cantidad = s.Cantidad;
                    sa.StockAnterior = s.StockAnterior;
                    sa.StockNuevo = s.StockNuevo;
                    lsad.Add(sa);
                }
                dbContext.StockAuditoria.AddRange(lsad);
                dbContext.SaveChanges();

                return entity;
            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public static Comprobantes GuardarAltaMasiva(ACHEEntities dbContext, ComprobanteCartDto compCart,
            Usuarios usu, List<Personas> personas, List<PuntosDeVenta> puntosDeVenta, List<Conceptos> conceptos,
            List<UsuariosAdicionales> usuariosAdicionales)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(compCart.TipoComprobante))
                    throw new CustomException("El tipo de comprobante es obligatorio");
                else if (string.IsNullOrWhiteSpace(compCart.Modo))
                    throw new CustomException("El modo es obligatorio");
                else if (string.IsNullOrWhiteSpace(compCart.CondicionVenta))
                    throw new CustomException("La condición de venta es obligatoria");
                else if (string.IsNullOrWhiteSpace(compCart.TipoConcepto))
                    throw new CustomException("El tipo de concepto es obligatorio");

                string[] listaTipoComprobanteFactura = { "FCA", "FCB", "FCC", "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "RCA", "RCB", "RCC", "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };

                int nro = 0;
                //var usu = dbContext.Usuarios.Where(x => x.IDUsuario == compCart.IDUsuario).FirstOrDefault();
                Personas persona = personas.Where(x => x.IDPersona == compCart.IDPersona).FirstOrDefault();

                if (persona == null)
                    throw new CustomException("El cliente/proveedor es inexistente");

                bool ComprobanteExistente = false;
                Comprobantes entity;
                Comprobantes entityOld = new Comprobantes();
                List<ComprobantesDetalle> entityCdOld = new List<ComprobantesDetalle>();
                if (compCart.IDComprobante > 0)
                {
                    //Solo alta masiva para facturas sin CAE
                    throw new CustomException("Solo para alta de facturas masivas sin CAE");

                }
                else
                {
                    if (compCart.TipoComprobante.Equals("PDV") || compCart.TipoComprobante.Equals("EDA"))
                        nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante(compCart.TipoComprobante, usu.IDUsuario, Convert.ToInt32(compCart.IDPuntoVenta)));
                    else
                        nro = Convert.ToInt32(compCart.Numero);


                    if (!listaTipoComprobanteFactura.Contains(compCart.TipoComprobante))
                    {
                        ComprobanteExistente = dbContext.Comprobantes
                            .Any(x => x.Tipo == compCart.TipoComprobante
                            && x.Numero == nro && x.IDPuntoVenta == compCart.IDPuntoVenta
                            && x.IDUsuario == usu.IDUsuario);
                    }

                    entity = new Comprobantes();
                    entity.FechaAlta = DateTime.Now;
                    entity.IDUsuario = usu.IDUsuario;

                }

                if (ComprobanteExistente)
                    throw new Exception("Ya existe el tipo de comprobante y nro comprobante para el proveedor/Cliente seleccionado");

                entity.IDPersona = compCart.IDPersona;
                entity.Tipo = compCart.TipoComprobante;
                entity.Modo = (compCart.TipoComprobante == "COT") ? "O" : compCart.Modo;
                entity.FechaComprobante = compCart.FechaComprobante;
                entity.TipoDestinatario = persona.Tipo.ToUpper();
                entity.CondicionVenta = compCart.CondicionVenta;
                entity.NroDocumento = persona.NroDocumento;
                entity.TipoDocumento = persona.TipoDocumento;
                entity.FechaVencimiento = compCart.FechaVencimiento;
                entity.IDPuntoVenta = compCart.IDPuntoVenta;
                entity.Nombre = compCart.Nombre;
                entity.Vendedor = compCart.Vendedor;
                entity.Envio = compCart.Envio;
                if (compCart.FechaEntrega.Year != 1)
                    entity.FechaEntrega = compCart.FechaEntrega;

                if (compCart.ProcesoCompraAutomatica != null)
                    entity.ProcesoCompraAutomatica = compCart.ProcesoCompraAutomatica;

                entity.PercepcionIVA = compCart.PercepcionIVA;
                entity.PercepcionIIBB = compCart.PercepcionIIBB;
                entity.ImporteNoGravado = compCart.ImporteNoGravado;
                entity.ImporteExento = compCart.ImporteExento;
                entity.Descuento = compCart.Descuento;

                if (compCart.IDJuresdiccion > 0)
                    entity.IDJurisdiccion = compCart.IDJuresdiccion;

                if (compCart.Modo != "E")
                {
                    entity.Numero = nro;
                    entity.FechaProceso = DateTime.Now;
                }
                entity.TipoConcepto = int.Parse(compCart.TipoConcepto);
                entity.Observaciones = compCart.Observaciones;
                entity.Estado = compCart.Estado;

                //if (entity.ComprobantesDetalle.Any())
                //    ConceptosCommon.SumarStock(dbContext, entity.ComprobantesDetalle.ToList());

                if (entity.ComprobantesDetalle.Any())
                    dbContext.ComprobantesDetalle.RemoveRange(entity.ComprobantesDetalle);

                if (compCart.Adjunto != null)
                    entity.Adjunto = Base64.Decode(compCart.Adjunto);


                if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC")
                {
                    if (compCart.IDComprobanteAsociado != 0)
                        entity.IdComprobanteAsociado = compCart.IDComprobanteAsociado;
                    else
                        throw new CustomException("Debe seleccionar una comprobante asociado cuando el comprobante es una Nota de credito/debito.");
                }

                if (entity.Tipo == "EDA")
                {
                    if (compCart.IDDomicilio != 0)
                        entity.IdDomicilio = compCart.IDDomicilio;
                    else
                        throw new CustomException("Debe seleccionar un domicilio de entrega.");

                    if (compCart.IDTransporte != 0)
                        entity.IdTransporte = compCart.IDTransporte;

                }

                if (entity.Tipo == "DDC")
                {
                    if (compCart.IDCompraVinculada != 0)
                        entity.IdCompraVinculada = compCart.IDCompraVinculada;
                    else
                        throw new CustomException("Error al obtener el codigo de comprobante de compra.");

                }

                if (compCart.IDTransportePersona != 0)
                    entity.IdTransportePersona = compCart.IDTransportePersona;

                if (compCart.IDActividad != 0)
                    entity.IdActividad = compCart.IDActividad;
                else
                    entity.IdActividad = UsuarioCommon.ObtenerActividades(usu.IDUsuario).FirstOrDefault().ID;

                decimal totalNeto = 0;
                decimal totalBruto = 0;

                List<StockAuditoria> lsa = new List<StockAuditoria>();


                foreach (var det in compCart.Items)
                {
                    ComprobantesDetalle compDet = new ComprobantesDetalle();
                    compDet.IDComprobante = entity.IDComprobante;
                    compDet.PrecioUnitario = det.PrecioUnitario;
                    compDet.Iva = det.Iva;
                    compDet.IdTipoIVA = det.IdTipoIva;
                    compDet.PrecioUnitarioIVA = det.PrecioUnitarioConIva - det.PrecioUnitarioSinIVA;
                    compDet.PrecioTotalIVA = det.TotalConIva - det.TotalSinIva;
                    compDet.Concepto = det.Concepto;
                    compDet.Cantidad = det.Cantidad;
                    compDet.Bonificacion = det.Bonificacion;
                    compDet.SubTotalAjustado = det.SubTotalAjustado;
                    if (det.IDPlanDeCuenta > 0)
                        compDet.IDPlanDeCuenta = det.IDPlanDeCuenta;

                    if (det.IDConcepto != null && det.IDConcepto != 0)
                        compDet.IDConcepto = det.IDConcepto;
                    else
                    {
                        string tipo = "";
                        if (compCart.TipoConcepto.Equals("1") || compCart.TipoConcepto.Equals("3"))
                            tipo = "P";
                        else
                            tipo = "S";
                        compDet.IDConcepto = ConceptosCommon.GuardarConcepto(0, det.Concepto, "", tipo, "", "A",
                                                                         det.PrecioUnitario.ToString(), det.IdTipoIva.ToString(), "0", "", det.PrecioUnitario.ToString(),
                                                                         "0", 0, usu.IDUsuario);
                        if (det.IDAbonos != null && det.IDAbonos != 0)
                            compDet.IDAbono = det.IDAbonos;
                    }

                    PuntosDeVenta pdVenta = puntosDeVenta.Where(w => w.IDPuntoVenta == compCart.IDPuntoVenta).FirstOrDefault();
                    if (pdVenta != null)
                    {
                        Conceptos c = conceptos.Where(w => w.IDConcepto == compDet.IDConcepto).FirstOrDefault();
                        if (c != null)
                        {
                            StockAuditoria sa = new StockAuditoria();
                            sa.IdConcepto = (int)compDet.IDConcepto;
                            sa.Accion = (entity.Tipo.Equals("PDV") ? "Venta - " : "Compra - ") + pdVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000");
                            sa.FechaAlta = DateTime.Now;
                            sa.IdUsuario = usu.IDUsuario;
                            sa.Cantidad = compDet.Cantidad;
                            sa.StockAnterior = (decimal)c.Stock;
                            if (entity.Tipo.Equals("PDV"))
                            {
                                sa.StockNuevo = (decimal)c.Stock - det.Cantidad;
                            }
                            else if (entity.Tipo.Equals("PDC"))
                            {
                                sa.StockNuevo = (decimal)c.Stock + det.Cantidad;
                            }
                            lsa.Add(sa);
                        }
                    }

                    if (det.Ajuste)
                    {
                        totalNeto += det.SubTotalAjustado;
                        totalBruto += det.SubTotalAjustado;
                    }
                    else
                    {
                        totalNeto += det.TotalConIva;
                        totalBruto += det.TotalSinIva;
                    }

                    entity.ComprobantesDetalle.Add(compDet);


                    if (entity.Tipo == "PDC")
                        ConceptosCommon.AplicarRentabilidad((int)compDet.IDConcepto, det.PrecioUnitario, usu.IDUsuario);

                }

                var Tributos = compCart.GetPercepcionIIBB() + compCart.GetPercepcionIVA();
                if (usu.CUIT.Equals("30716909839"))
                {
                    entity.ImporteTotalBruto = totalBruto / 1000;
                    entity.ImporteTotalNeto = Math.Round((totalNeto + Tributos) / 1000, 2);
                }
                else
                {
                    entity.ImporteTotalBruto = totalBruto;
                    entity.ImporteTotalNeto = Math.Round(totalNeto + Tributos, 2);
                }

                entity.ImporteTotalBruto = entity.ImporteTotalBruto - ((entity.ImporteTotalBruto * entity.Descuento) / 100);
                entity.ImporteTotalNeto = entity.ImporteTotalNeto - ((entity.ImporteTotalNeto * entity.Descuento) / 100);

                if (compCart.IDComprobante > 0)
                {
                    //Solo alta masiva para facturas sin CAE
                    throw new CustomException("Solo para alta de facturas masivas sin CAE");
                }

                //Comisiones
                if (compCart.IDUsuarioAdicional != 0)
                {
                    UsuariosAdicionales usrAd = usuariosAdicionales.Where(w => w.IDUsuarioAdicional == compCart.IDUsuarioAdicional).FirstOrDefault();
                    if (usrAd != null)
                    {
                        entity.ImporteComisionVendedor = (usrAd.PorcentajeComision / 100) * entity.ImporteTotalNeto;
                        entity.IdUsuarioAdicional = usrAd.IDUsuarioAdicional;
                    }
                    else
                    {
                        entity.ImporteComisionVendedor = (usu.PorcentajeComision / 100) * entity.ImporteTotalNeto;
                        entity.IdUsuarioAdicional = usu.IDUsuario;
                    }
                }
                //

                if (entity.Tipo != "EDA")
                {
                    if (totalNeto == 0)
                        throw new CustomException("No puede generar una factura con Importe 0");
                }

                if (compCart.IDComprobante > 0)
                {
                    //Solo alta masiva para facturas sin CAE
                    throw new CustomException("Solo para alta de facturas masivas sin CAE");
                }
                else
                {
                    if (entity.Tipo != "EDA")
                    {
                        if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC" || entity.Tipo == "PDC")
                            ConceptosCommon.SumarStock(dbContext, entity.ComprobantesDetalle.ToList());
                        else
                            ConceptosCommon.RestarStock(dbContext, entity.ComprobantesDetalle.ToList());

                        if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC")
                        {
                            Comprobantes fc = dbContext.Comprobantes.Where(w => w.IDComprobante == compCart.IDComprobanteAsociado).FirstOrDefault();
                            if (fc != null)
                            {
                                Comprobantes pdv = dbContext.Comprobantes.Where(w => w.IDComprobante == fc.IdComprobanteAsociado).FirstOrDefault();
                                if (pdv != null)
                                {
                                    pdv.Saldo = pdv.Saldo + entity.ImporteTotalNeto;
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        if (entity.Tipo == "NDA" || entity.Tipo == "NDB" || entity.Tipo == "NDC")
                        {
                            Comprobantes fc = dbContext.Comprobantes.Where(w => w.IDComprobante == compCart.IDComprobanteAsociado).FirstOrDefault();
                            if (fc != null)
                            {
                                Comprobantes pdv = dbContext.Comprobantes.Where(w => w.IDComprobante == fc.IdComprobanteAsociado).FirstOrDefault();
                                if (pdv != null)
                                {
                                    pdv.Saldo = pdv.Saldo - entity.ImporteTotalNeto;
                                    dbContext.SaveChanges();
                                }
                            }
                        }

                        entity.Saldo = entity.ImporteTotalNeto;

                    }
                    else
                    {
                        ConceptosCommon.RestarStockFisico(dbContext, entity.ComprobantesDetalle.ToList());
                    }

                    dbContext.Comprobantes.Add(entity);
                    dbContext.SaveChanges();

                }
               
                dbContext.SaveChanges();

                return entity;
            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static Comprobantes GuardarComprobante(ComprobanteCartDto compCart)
        {
            using (var dbContext = new ACHEEntities())
            {
                return Guardar(dbContext, compCart);
            }
        }

        public static string ObtenerTipoDeFacturaAFacturar(string SUCondicionIva, string MiCondicionIva)
        {
            string tipoDeFactura = string.Empty;
            if (MiCondicionIva == "MO")
            {
                tipoDeFactura = "FCC";
            }
            else
            {
                switch (SUCondicionIva)
                {
                    case "RI":
                        tipoDeFactura = "FCA";
                        break;
                    default:
                        tipoDeFactura = "FCB";
                        break;
                }
            }
            return tipoDeFactura;
        }
        public static string ObtenerProxNroComprobante(string tipo, int idUsuario, int idPuntoDeVenta)
        {
            try
            {
                long numero = 0;
                var nro = "";
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.Comprobantes.Any(x => x.IDUsuario == idUsuario && x.Tipo == tipo))
                    {
                        var aux = dbContext.Comprobantes.Where(x => x.IDUsuario == idUsuario && x.Tipo == tipo && x.IDPuntoVenta == idPuntoDeVenta).ToList();
                        if (aux != null && aux.Count() > 0)
                        {
                            numero = aux.Max(x => x.Numero) + 1;
                            while (dbContext.Comprobantes.Where(x => x.Numero == numero && x.IDUsuario == idUsuario && x.Tipo == tipo && x.IDPuntoVenta == idPuntoDeVenta).Any())
                            {
                                numero++;
                            }

                            nro = numero.ToString("#00000000");
                        }
                            
                        else
                            nro = "00000001";
                    }
                    else
                        nro = "00000001";
                }

                return nro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void CrearComprobanteElectro(WebUser usu, int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
        int tipoConcepto, string fechaVencimiento, int idPuntoVenta, ref string nroComprobante, string obs, ComprobanteModo accion, 
        ComprobanteCartDto comprCart, string pathBase, string domicilioTransporteCliente)
        {
            var pathPdf = "";
            //var pathCertificado = "";

            if (accion == ComprobanteModo.Previsualizar)
            {
                Path.Combine(pathBase + "/files/comprobantes/" + usu.IDUsuario + "_prev.pdf");
                pathPdf = Path.Combine(pathBase + "/files/comprobantes/" + usu.IDUsuario + "_prev.pdf"); //HttpContext.Current.Server.MapPath("~/files/comprobantes/" + usu.IDUsuario + "_prev.pdf");
                if (System.IO.File.Exists(pathPdf))
                    System.IO.File.Delete(pathPdf);
            }
            else
                pathPdf = Path.Combine(pathBase + "/files/explorer/" + usu.IDUsuario + "/Comprobantes/" + DateTime.Now.Year.ToString() + "/"); //HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Comprobantes/" + DateTime.Now.Year.ToString() + "/");



            using (var dbContext = new ACHEEntities())
            {
                Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (persona == null)
                    throw new Exception("El cliente/proveedor es inexistente");

                if (ComprobanteModo.Generar == accion)
                {
                    pathPdf = pathPdf + persona.RazonSocial.RemoverCaracteresParaPDF() + "_";
                }

                int puntoVenta = 0;
                PuntosDeVenta punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == idPuntoVenta && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (punto != null)
                    puntoVenta = punto.Punto;

                if (puntoVenta == 0)
                    throw new Exception("Punto de venta invalido");

                FEFacturaElectronica fe = new FEFacturaElectronica();
                FEComprobante comprobante = new FEComprobante();

                #region Datos del emisor

                comprobante.TipoComprobante = ComprobantesCommon.ObtenerTipoComprobante(tipo);
               
                //var feMode = ConfigurationManager.AppSettings["FE.Modo"];
                if (usu.ModoQA)
                {
                    comprobante.Cuit = 30714075159;//long.Parse(usu.CUIT);
                    comprobante.PtoVta = 5;// puntoVenta;
                    //pathCertificado = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.CertificadoAFIP.QA"]);
                }
                else
                {
                    comprobante.Cuit = long.Parse(usu.CUIT);
                    comprobante.PtoVta = puntoVenta;
                    //pathCertificado = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.CertificadoAFIP.PROD"]);
                }

                comprobante.Concepto = (FEConcepto)tipoConcepto;
                comprobante.Fecha = DateTime.Parse(fecha);


                //si el tipo de factura es C solo se tiene que setear las fechas si el tipo de concepto es 2 o 3 (productos o productos y servicios)
                if (tipoConcepto != 1)
                {
                    comprobante.FchServDesde = DateTime.Parse(fecha);
                    comprobante.FchServHasta = DateTime.Parse(fechaVencimiento);
                    comprobante.FchVtoPago = DateTime.Parse(fechaVencimiento);
                }

                comprobante.CodigoMoneda = "PES";
                comprobante.CotizacionMoneda = 1;
                if (nroComprobante != string.Empty)
                    comprobante.NumeroComprobante = int.Parse(nroComprobante);

                comprobante.CondicionVenta = usu.CondicionIVA;
                comprobante.Domicilio = usu.Domicilio;
                comprobante.CiudadProvincia = usu.Ciudad + ", " + usu.Provincia;
                comprobante.RazonSocial = usu.RazonSocial;
                comprobante.Telefono = usu.Telefono;
                comprobante.Celular = usu.Celular;
                if (usu.ExentoIIBB != null)
                    comprobante.IIBB = ((bool)usu.ExentoIIBB) ? "Exento" : usu.IIBB;
                comprobante.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usu.CondicionIVA);
                comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString(formatoFecha) : "";

                #endregion

                #region Datos del cliente
                if (persona.Tipo == "CF")
                    comprobante.DocTipo = 99; // Consumidor final
                else
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;

                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);

                comprobante.CondicionVenta = condicionVenta;
                comprobante.ClienteNombre = persona.RazonSocial;
                comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);
                comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
                comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
                comprobante.Observaciones = obs;
                comprobante.Original = (accion == ComprobanteModo.Previsualizar ? false : true);
                if (tipo == "PRE")
                    comprobante.Original = true;

                #endregion


                #region Seteo los datos de la factura

                //comprobante.ImpTotConc = 0;
                //comprobante.ImpOpEx = 0;
                comprobante.DetalleIva = new List<FERegistroIVA>();
                comprobante.Tributos = new List<FERegistroTributo>();


                var list = comprCart.Items.OrderBy(x => x.Concepto).ToList();


                if (list.Any())
                {
                    foreach (var detalle in list)
                    {
                        if (usu.CondicionIVA == "RI" && persona.CondicionIva == "RI")
                        {
                            comprobante.ItemsDetalle.Add(new FEItemDetalle()
                            {
                                Cantidad = Convert.ToDouble(detalle.Cantidad),
                                Descripcion = detalle.Concepto,
                                //Precio = Math.Round(double.Parse(detalle.PrecioUnitario.ToString()), 2), 
                                Precio = Math.Round(double.Parse(detalle.PrecioUnitarioSinIVA.ToString()), 2),
                                Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                                Bonificacion = detalle.Bonificacion
                            });

                            //totalizar importe e IVA
                            switch (detalle.Iva.ToString("N2"))
                            {
                                case "21,00":
                                    AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                                    break;
                                case "27,00":
                                    AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                                    break;
                                case "10,50":
                                    AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                                    break;
                                case "5,00":
                                    AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                                    break;
                                case "2,50":
                                    AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                                    break;
                                case "0,00":
                                    AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                                    break;
                            }
                        }
                        else
                        {
                            comprobante.ItemsDetalle.Add(new FEItemDetalle()
                            {
                                Cantidad = Convert.ToDouble(detalle.Cantidad),
                                Descripcion = detalle.Concepto,
                                Precio = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2),
                                Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                                Bonificacion = detalle.Bonificacion
                            });

                            if (comprobante.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                            {
                                comprobante.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2);
                            }
                            else
                            {
                                if (tipo != "FCC" && tipo != "NDC" && tipo != "RCC")
                                    comprobante.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2), TipoIva = FETipoIva.Iva0 });
                            }
                        }
                    }
                }

                //Remplazar el IDConcepto por codigo 
                foreach (var item in comprobante.ItemsDetalle)
                {
                    if (item.Codigo != "")
                    {
                        int idConceto = Convert.ToInt32(item.Codigo);
                        item.Codigo = dbContext.Conceptos.Where(x => x.IDConcepto == idConceto).FirstOrDefault().Codigo.ToString();
                    }
                }

                #endregion

                comprobante.ImpTotConc = (double)comprCart.GetImporteNoGravado();
                if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIIBB)
                {
                    if ((double)comprCart.GetPercepcionIIBB() > 0)
                    {
                        comprobante.Tributos.Add(new FERegistroTributo()
                        {
                            BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                            Alicuota = (double)comprCart.PercepcionIIBB,
                            Importe = Math.Round((double)comprCart.GetPercepcionIIBB(), 2),
                            Decripcion = "Percepción IIBB",
                            Tipo = FETipoTributo.ImpuestosNacionales
                        });
                    }
                }

                if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIVA)
                {
                    if ((double)comprCart.GetPercepcionIVA() > 0)
                    {
                        comprobante.Tributos.Add(new FERegistroTributo()
                        {
                            BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                            Alicuota = (double)comprCart.PercepcionIVA,
                            Importe = Math.Round((double)comprCart.GetPercepcionIVA(), 2),
                            Decripcion = "Percepción IVA",
                            Tipo = FETipoTributo.ImpuestosNacionales
                        });
                    }
                }

                var pathTemplateFc = Path.Combine(pathBase + ConfigurationManager.AppSettings["FE.Template"].Replace("~", "") + usu.TemplateFc + ".pdf"); //HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"] + usu.TemplateFc + ".pdf");
                var imgLogo = "logo-fc-" + usu.TemplateFc + ".png";
                if (!string.IsNullOrEmpty(usu.Logo))
                {
                    if (File.Exists(Path.Combine(pathBase + "/files/usuarios/" + usu.Logo))) //HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                        imgLogo = usu.Logo;
                }

                var pathLogo = Path.Combine(pathBase + "/files/usuarios/" + imgLogo);//HttpContext.Current.Server.MapPath("~/files/usuarios/" + imgLogo);
                if (accion == ComprobanteModo.Previsualizar)
                    fe.GrabarEnDisco(comprobante, pathPdf, pathTemplateFc, pathLogo);
                else//SI no se previsualiza se genera electronicamente el comprobante
                {
                    Comprobantes cmp = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();
                    if (cmp != null)
                    {
                        cmp.FechaProceso = DateTime.Now;
                        try
                        {
                            //fe.GenerarComprobante(comprobante, ConfigurationManager.AppSettings["FE.PROD.wsaa"], pathCertificado, pathTemplateFc, pathLogo);
                            try
                            {
                                fe.GenerarComprobante(comprobante, Convert.ToInt64(usu.CUITAfip), pathTemplateFc, pathLogo, (usu.ModoQA ? "QA" : "PROD"),false);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("ValidacionDeToken: No aparecio CUIT en lista de relaciones"))
                                    fe.GenerarComprobante(comprobante, Convert.ToInt64(usu.CUITAfip), pathTemplateFc, pathLogo, (usu.ModoQA ? "QA" : "PROD"),true);
                                else
                                    throw new Exception(ex.Message);
                            }                            
                            string numeroComprobante = comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0');
                            pathPdf = pathPdf + tipo + "-" + numeroComprobante + ".pdf";

                            cmp.FechaCAE = comprobante.FechaVencCAE;
                            cmp.CAE = comprobante.CAE;
                            cmp.Numero = comprobante.NumeroComprobante;
                            cmp.Error = null;
                            cmp.FechaError = null;

                            nroComprobante = numeroComprobante;
                            dbContext.SaveChanges();

                            fe.GrabarEnDisco(comprobante.ArchivoFactura, pathPdf, pathTemplateFc, pathLogo);
                        }
                        catch (Exception ex)
                        {
                            cmp.Error = ex.Message;
                            cmp.FechaError = DateTime.Now;
                            dbContext.SaveChanges();

                            throw new CustomException(cmp.Error, ex.InnerException);
                        }
                    }
                }
            }
        }
        public static void AgregarDetalleIvaTotalizado(ref FEComprobante comprobante, ComprobantesDetalleViewModel detalle, FETipoIva tipoiva)
        {
            if (comprobante.DetalleIva.Any(x => x.TipoIva == tipoiva))
                comprobante.DetalleIva.Where(x => x.TipoIva == tipoiva).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalSinIva.ToString()), 2);
            else
                comprobante.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(detalle.TotalSinIva.ToString()), 2), TipoIva = tipoiva });
        }
        public static FEComprobante ObtenerComprobanteElectronica(long cuit, long cuitAfip, long CbtNro, int PtoVta, string tipoComprobante)
        {
            FEFacturaElectronica fe = new FEFacturaElectronica();
            var tipo = ObtenerTipoComprobante(tipoComprobante);
            FEComprobante comprobante = null;
            try
            {
                comprobante = fe.GetComprobante(cuit, cuitAfip, CbtNro, PtoVta, tipo, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("600"))                
                    comprobante = fe.GetComprobante(cuit, cuitAfip, CbtNro, PtoVta, tipo, true);      
                else
                    throw new Exception(ex.Message);
            }            
            return comprobante;
        }
        public static bool ExisteComprobante(long cuit, long nroComprobante, int PtoVta, string tipoComprobante)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    Usuarios usuario = UsuarioCommon.ObtenerUsuarioPorNroDoc(cuit.ToString());
                    if (usuario == null)
                        throw new Exception("No se encontro el usuario con el cuit seleccionado");

                    var existe = dbContext.Comprobantes.Any(x => x.IDUsuario == usuario.IDUsuario && x.Numero == nroComprobante && x.PuntosDeVenta.Punto == PtoVta && x.Tipo == tipoComprobante);
                    return existe;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void InsertarComprobanteRecuperado(FEComprobante comprobante, 
            string tipoComprobante, string cuit)
        {
            try
            {
                Usuarios usuario = UsuarioCommon.ObtenerUsuarioPorNroDoc(cuit);
                Personas Cliente = null;
                if (usuario != null)
                    Cliente = PersonasCommon.ObtenerPersonaPorNroDoc(comprobante.DocNro.ToString(), usuario.IDUsuario);
                else
                    throw new Exception("No se encontro el usuario con el ciut seleccionado");
                if (Cliente == null)
                    throw new Exception("No se encontro el cliente del usuario con el nro de documento seleccionado");

                using (var dbContext = new ACHEEntities())
                {
                    PuntosDeVenta punto = dbContext.PuntosDeVenta.Where(x => x.Punto == comprobante.PtoVta && x.IDUsuario == usuario.IDUsuario).FirstOrDefault();
                    ComprobanteCartDto compCart = new ComprobanteCartDto();
                    ComprobantesDetalleViewModel comp = new ComprobantesDetalleViewModel();

                    compCart.IDComprobante = 0;
                    compCart.TipoConcepto = tipoComprobante;
                    compCart.Numero = comprobante.NumeroComprobante.ToString();
                    compCart.Modo = "E";
                    compCart.FechaComprobante = comprobante.Fecha;
                    compCart.CondicionVenta = "Efectivo";
                    compCart.TipoConcepto = ((int)comprobante.Concepto).ToString();
                    compCart.FechaVencimiento = comprobante.Fecha.AddDays(30);
                    compCart.IDPuntoVenta = punto.IDPuntoVenta;
                    compCart.TipoComprobante = tipoComprobante;
                    compCart.Observaciones = comprobante.Observaciones;
                    compCart.IDPersona = Cliente.IDPersona;
                    compCart.IDUsuario = usuario.IDUsuario;
                    compCart.Items = new List<ComprobantesDetalleViewModel>();
                    compCart.IDActividad = 0;

                    int contador = 0;
                    foreach (var item in comprobante.DetalleIva)
                    {
                        comp.ID = contador;
                        comp.IDConcepto = null;
                        comp.IDPlanesPagos = null;

                        comp.Codigo = "";
                        comp.Cantidad = 1;
                        switch (item.TipoIva)
                        {
                            case FETipoIva.Iva0:
                                comp.Iva = decimal.Parse("0,00");
                                break;
                            case FETipoIva.Iva10_5:
                                comp.Iva = decimal.Parse("10,50");
                                break;
                            case FETipoIva.Iva21:
                                comp.Iva = decimal.Parse("21,00");
                                break;
                            case FETipoIva.Iva27:
                                comp.Iva = decimal.Parse("27,00");
                                break;
                            case FETipoIva.Iva5:
                                comp.Iva = decimal.Parse("5,00");
                                break;
                            case FETipoIva.Iva2_5:
                                comp.Iva = decimal.Parse("2,50");
                                break;
                        }
                        comp.Concepto = "Total Item IVA " + comp.Iva;

                        comp.PrecioUnitario = Convert.ToDecimal(item.BaseImp);
                        compCart.Items.Add(comp);
                        contador++;
                    }

                    var cmp = Guardar(dbContext, compCart);

                    cmp.FechaCAE = comprobante.FechaVencCAE;
                    cmp.CAE = comprobante.CAE;
                    cmp.Numero = comprobante.NumeroComprobante;
                    cmp.Error = null;
                    cmp.FechaError = null;
                    cmp.FechaProceso = DateTime.Now;
                    dbContext.SaveChanges();
                    //// para generarPDF
                    generarPDF(comprobante, usuario, Cliente, compCart.TipoComprobante);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private static void generarPDF(FEComprobante comprobante, Usuarios usuario, Personas Cliente, string tipo)
        {
            comprobante.Cuit = Convert.ToInt64(usuario.CUIT);
            comprobante.CiudadProvincia = (usuario.Ciudades == null) ? "" : usuario.Ciudades.Nombre + ", " + usuario.Provincias.Nombre;
            comprobante.Domicilio = usuario.Domicilio + " " + usuario.PisoDepto;
            comprobante.Telefono = usuario.Telefono;
            comprobante.Celular = usuario.Celular;
            comprobante.ClienteCondiionIva = usuario.CondicionIva;
            comprobante.IIBB = usuario.IIBB;
            comprobante.FechaInicioActividades = Convert.ToDateTime(usuario.FechaInicioActividades).ToString(formatoFecha);
            comprobante.CondicionVenta = "Efectivo";

            comprobante.ClienteCondiionIva = Cliente.CondicionIva;
            comprobante.ClienteDomicilio = Cliente.Domicilio + " " + Cliente.PisoDepto;
            comprobante.ClienteLocalidad = (Cliente.Provincias == null) ? "" : Cliente.Provincias.Nombre;
            comprobante.ClienteNombre = Cliente.RazonSocial;


            foreach (var item in comprobante.DetalleIva)
            {

                comprobante.ItemsDetalle.Add(new FEItemDetalle()
                {

                    Cantidad = 1,
                    Descripcion = "Item recuperado",
                    Precio = item.BaseImp,
                    Codigo = "",
                    Bonificacion = 0,
                });
            }

            var PathBaseWeb = ConfigurationManager.AppSettings["PathBaseWeb"];
            var imgLogo = "logo-fc-" + usuario.TemplateFc + ".png";
            if (!string.IsNullOrEmpty(usuario.Logo))
            {
                if (File.Exists(Path.Combine(PathBaseWeb + "/files/usuarios/" + usuario.Logo))) //HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                    imgLogo = usuario.Logo;
            }

            var pathTemplateFc = Path.Combine(PathBaseWeb + ConfigurationManager.AppSettings["FE.Template"].Replace("~", "") + usuario.TemplateFc + ".pdf");
            var pathLogo = Path.Combine(PathBaseWeb + "/files/usuarios/" + imgLogo);
            var pathPdf = Path.Combine(PathBaseWeb + "/files/explorer/" + usuario.IDUsuario + "/Comprobantes/" + DateTime.Now.Year.ToString() + "/");

            string numeroComprobante = comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0');
            pathPdf = pathPdf + Cliente.RazonSocial.RemoverCaracteresParaPDF() + "_";
            pathPdf = pathPdf + tipo + "-" + numeroComprobante + ".pdf";

            FEFacturaElectronica fe = new FEFacturaElectronica();
            comprobante.ArchivoFactura = fe.GetStreamPDF(comprobante, pathTemplateFc, pathLogo, "");
            fe.GrabarEnDisco(comprobante.ArchivoFactura, pathPdf, pathTemplateFc, pathLogo);
        }
        public static void GuardarComprobantesEnviados(ACHEEntities dbContext, int? idcomprobante, int? idCobranza, string mensaje, bool resultado, WebUser usu)
        {
            try
            {
                var entity = new ComprobantesEnviados();
                entity.IDUsuario = usu.IDUsuario;
                entity.Mensaje = mensaje;
                entity.Resultado = resultado;
                if (idcomprobante != null)
                    entity.IDComprobante = idcomprobante;
                if (idCobranza != null)
                    entity.IDCobranza = idCobranza;
                dbContext.ComprobantesEnviados.Add(entity);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #region crear comprobante para contabilium
        /// <summary>
        /// Crea una fc electronico y la cobranza asociada del plan a un cliente determinado, si el cliente no existe lo crea.
        /// al cliente lo asocia a CONTABILIUM
        /// </summary>
        /// <param name="nroReferencia"> numero de trasferencia bancario o de idmp</param>
        /// <param name="IdUsuario">usuario al que se ve a imputar el pago</param>
        /// <param name="dbContext"></param>
        /// <param name="planesPagos">plan a facturar</param>
        /// <param name="pathBase">Ruta de donde se BASE de donde se va a guardar la factura electronica</param>
        /// <returns></returns>
        public static ComprobanteNroViewModel CrearDatosParaContabilium(string nroReferencia, int IdUsuario, ACHEEntities dbContext, PlanesPagos planesPagos, string pathBase)
        {
            var comprobanteViewModel = new ComprobanteNroViewModel();
            // ** CREACION DE USUARIO FACTURA Y COBRANZA ** //
            var idContabilium = Convert.ToInt32(ConfigurationManager.AppSettings["Usu.idContabilium"]);
            var contabilium = dbContext.Usuarios.Where(x => x.IDUsuario == idContabilium).FirstOrDefault();
            if (contabilium == null)
                throw new Exception("No se encontro el usuario de Contabilium");
            // ** CREACION DE USUARIO ** //
            var Cliente = PersonasCommon.Guardar(dbContext, contabilium.IDUsuario, IdUsuario, "C");
            // ** CREACION DE FACTURA ** //
            var comprobante = CrearComprobante(dbContext, planesPagos, Cliente, contabilium);
            // ** CREACION FACTURA ELECTRONICA ** //
            var nroComprobante = CrearComprobanteElectronico(dbContext, comprobante, planesPagos, contabilium, pathBase);
            // ** CREACION COBRANZA **//
            CrearCobranza(dbContext, comprobante, planesPagos, nroReferencia, contabilium);

            comprobanteViewModel.comprobante = comprobante;
            comprobanteViewModel.nroComprobanteElectronico = nroComprobante;
            return comprobanteViewModel;
        }
        private static Comprobantes CrearComprobante(ACHEEntities dbContext, PlanesPagos planesPagos, Personas Cliente, Usuarios usuario)
        {
            ComprobanteCartDto compCart = new ComprobanteCartDto();
            ComprobantesDetalleViewModel comp = new ComprobantesDetalleViewModel();

            compCart.IDComprobante = 0;
            compCart.TipoComprobante = ObtenerTipoDeFacturaAFacturar(Cliente.CondicionIva, usuario.CondicionIva);
            compCart.IDPuntoVenta = Convert.ToInt32(ConfigurationManager.AppSettings["Usu.idPuntoDeVenta"]);
            compCart.Numero = ObtenerProxNroComprobante(compCart.TipoComprobante, usuario.IDUsuario, compCart.IDPuntoVenta);
            compCart.Modo = "E";
            compCart.FechaComprobante = DateTime.Now.Date;
            compCart.CondicionVenta = "Tarjeta de credito";
            compCart.TipoConcepto = "2";
            compCart.FechaVencimiento = DateTime.Now.Date;
            compCart.Observaciones = "";
            compCart.IDActividad = 0;

            compCart.IDPersona = Cliente.IDPersona;
            compCart.IDUsuario = usuario.IDUsuario;

            compCart.Items = new List<ComprobantesDetalleViewModel>();

            comp.ID = 0;
            comp.IDConcepto = null;
            comp.IDPlanesPagos = planesPagos.IDPlanesPagos;

            comp.Codigo = "";
            comp.Concepto = "Plan " + planesPagos.Planes.Nombre;
            if (planesPagos.PagoAnual)
            {
                comp.Bonificacion = 10;
                comp.Cantidad = 12;
            }
            else
                comp.Cantidad = 1;

            comp.Iva = 21;

            comp.PrecioUnitario = ConceptosCommon.ObtenerPrecioFinal(planesPagos.ImportePagado, "21,00");

            compCart.Items.Add(comp);
            var comprobante = Guardar(dbContext, compCart);
            return comprobante;
        }
        /// <summary>
        /// Esta funcion solo se usa cuando se crean las fc de contabilium
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="comprobante"></param>
        /// <param name="planesPagos"></param>
        /// <param name="Contabilium"></param>
        /// <param name="pathBase"></param>
        /// <returns></returns>
        private static string CrearComprobanteElectronico(ACHEEntities dbContext, Comprobantes comprobante, PlanesPagos planesPagos, Usuarios Contabilium, string pathBase)
        {
            var usu = dbContext.UsuariosView.Where(x => x.IDUsuario == Contabilium.IDUsuario).FirstOrDefault();

            //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(dbContext, usu.IDUsuario, usu.CondicionIva);
            var webUser = new WebUser(
                       usu.IDUsuario, usu.IDUsuarioAdicional, usu.Tipo, usu.RazonSocial, usu.CUIT, usu.CondicionIva,
                       usu.Email, "", usu.Domicilio + " " + usu.PisoDepto, usu.Pais, usu.IDProvincia,
                       usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFacturaElectronica, usu.IIBB, usu.FechaInicioActividades,
                       usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupRealizado, true /*tieneMultiempresa*/, !usu.UsaProd,
                       6/*idPlan*/, usu.EmailAlertas, usu.Provincia, usu.Ciudad, usu.EsAgentePercepcionIVA, usu.EsAgentePercepcionIIBB,
                       usu.EsAgenteRetencionGanancia, usu.EsAgenteRetencion, true /*PlanVigente*/, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB,
                       usu.UsaPrecioFinalConIVA, usu.FechaAlta, usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo, 
                       usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, 
                       usu.PorcentajeCompra, usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
                       usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);

            int id = comprobante.IDComprobante;
            int idPersona = comprobante.IDPersona;
            string tipo = comprobante.Tipo;
            string modo = comprobante.Modo;
            string fecha = comprobante.FechaAlta.ToString(formatoFecha);
            string condicionVenta = comprobante.CondicionVenta;
            int tipoConcepto = comprobante.TipoConcepto;
            string fechaVencimiento = comprobante.FechaVencimiento.ToString(formatoFecha);
            int idPuntoVenta = comprobante.IDPuntoVenta;
            string nroComprobante = comprobante.Numero.ToString("#00000000");
            string obs = comprobante.Observaciones;

            ComprobanteCartDto compCart = new ComprobanteCartDto();
            compCart.Items = new List<ComprobantesDetalleViewModel>();
            ComprobantesDetalleViewModel comp = new ComprobantesDetalleViewModel();
            comp.ID = 0;
            comp.IDConcepto = null;
            comp.IDPlanesPagos = planesPagos.IDPlanesPagos;

            comp.Codigo = "";
            comp.Concepto = "Plan: " + planesPagos.Planes.Nombre;
            if (planesPagos.PagoAnual)
            {
                comp.Bonificacion = 10;
                comp.Cantidad = 12;
            }
            else
                comp.Cantidad = 1;

            comp.Iva = decimal.Parse("21,00");

            comp.PrecioUnitario = comprobante.ImporteTotalBruto;

            compCart.Items.Add(comp);
            var testIVA = compCart.GetIva();

            string domicilioTransporteCliente = "";

            CrearComprobanteElectro(webUser, id, idPersona, tipo, modo, fecha, condicionVenta,
            tipoConcepto, fechaVencimiento, idPuntoVenta, ref nroComprobante, obs, ComprobanteModo.Generar, compCart, pathBase, domicilioTransporteCliente);
            return nroComprobante;
        }
        /// <summary>
        /// Esta funcion solo se usa cuando se crean las fc de contabilium
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="comprobante"></param>
        /// <param name="planesPagos"></param>
        /// <param name="nroReferencia"></param>
        /// <param name="contabilium"></param>
        private static void CrearCobranza(ACHEEntities dbContext, Comprobantes comprobante, PlanesPagos planesPagos, string nroReferencia, Usuarios contabilium)
        {
            var punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == comprobante.IDPuntoVenta).FirstOrDefault();

            CobranzaCartDto cobrCartdto = new CobranzaCartDto();
            cobrCartdto.IDCobranza = 0;
            cobrCartdto.IDPersona = comprobante.IDPersona;
            cobrCartdto.Tipo = "RC";
            cobrCartdto.Fecha = DateTime.Now.Date.ToString(formatoFecha);
            cobrCartdto.IDPuntoVenta = comprobante.IDPuntoVenta;
            cobrCartdto.NumeroCobranza = CobranzasCommon.obtenerProxNroCobranza(cobrCartdto.Tipo, contabilium.IDUsuario);
            cobrCartdto.Observaciones = "Cobranza generada automaticamente por mercado pago";

            CobranzasDetalleViewModel item = new CobranzasDetalleViewModel();
            cobrCartdto.Items = new List<CobranzasDetalleViewModel>();
            item.ID = 1;
            item.Comprobante = comprobante.Tipo + " " + punto.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
            item.Importe = comprobante.Saldo;
            item.IDComprobante = comprobante.IDComprobante;
            cobrCartdto.Items.Add(item);

            CobranzasFormasDePagoViewModel formasDePago = new CobranzasFormasDePagoViewModel();
            cobrCartdto.FormasDePago = new List<CobranzasFormasDePagoViewModel>();
            formasDePago.ID = 1;
            formasDePago.Importe = planesPagos.ImportePagado;
            formasDePago.FormaDePago = planesPagos.FormaDePago;
            formasDePago.NroReferencia = nroReferencia;
            cobrCartdto.FormasDePago.Add(formasDePago);

            cobrCartdto.Retenciones = new List<CobranzasRetencionesViewModel>();

            var usu = TokenCommon.ObtenerWebUser(contabilium.IDUsuario);
            CobranzasCommon.Guardar(dbContext, cobrCartdto, usu);
        }
        #endregion

        private class ComprobanteBusqueda
        {
            public int IdComprobante { get; set; }
            public int? IdComprobanteVinculado { get; set; }
            public decimal ImporteTotalNeto { get; set; }
            public ICollection<ComprobantesDetalle> compDet { get; set; }
        }

        public static ResultadosComprobantesViewModel ObtenerComprobantes(string condicion, string periodo, 
            string fechaDesde, string fechaHasta, string fechaUltimoLiquidoProducto, 
            int page, int pageSize, WebUser usu, string tipo,
            bool entregaPendiente, bool entregaEstado, string estado,
            bool cobranzaPendiente, bool facturaPendiente)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    List<Comprobantes> lc = new List<Comprobantes>();

                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                        case "-2":
                            fechaDesde = DateTime.Now.AddYears(-20).ToShortDateString();
                            break;
                    }

                    DateTime dtDesdeInicial = DateTime.Parse(fechaDesde);

                    var comprobantesTotales = dbContext.Comprobantes
                        .Include("Personas").Include("PuntosDeVenta")
                        .Where(x => x.IDUsuario == usu.IDUsuario && !x.Personas.NroDocumento.Equals("99999999") && x.FechaComprobante >= dtDesdeInicial).AsQueryable();

                    var tipoComprobantesIncluidos = new[] { "" };

                    if (!string.IsNullOrWhiteSpace(tipo))
                    {
                        switch (tipo)
                        {
                            case "EDA":
                                tipoComprobantesIncluidos = new[] { "EDA" };
                                break;
                            case "PDV":
                                tipoComprobantesIncluidos = new[] { "PDV"};
                                break;
                            case "PDC":
                                tipoComprobantesIncluidos = new[] { "PDC" };
                                break;
                            case "DDC":
                                tipoComprobantesIncluidos = new[] { "DDC" };
                                break;
                            case "FAC":
                                tipoComprobantesIncluidos = new[] { "FCA", "FCB", "FCC" , "RCA", "RCB", "RCC", "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
                                break;
                            default:
                                tipoComprobantesIncluidos = new[] { "COT", "EDA", "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NDP", "PDC", "PDV", "RCB", "RCC", "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
                                break;
                        }
                        //results = results.Where(x => x.Tipo.Equals(tipo));
                    }

                    condicion = (!string.IsNullOrWhiteSpace(condicion)) ? condicion : "";

                    var resultadoInicial = comprobantesTotales                                          
                                            .Where(x => x.IDUsuario == usu.IDUsuario && !x.Personas.NroDocumento.Equals("99999999") && tipoComprobantesIncluidos.Contains(x.Tipo)).AsQueryable();
                    Int32 numero = 0;

                    //if (Int32.TryParse(condicion, out numero))
                    //    results = results.Where(x => x.Numero == Math.Abs(numero));
                    //else if (condicion.Contains("-"))
                    //{
                    //    var punto = (string.IsNullOrWhiteSpace(condicion.Split("-")[0])) ? "0" : condicion.Split("-")[0];
                    //    var nro = (string.IsNullOrWhiteSpace(condicion.Split("-")[1])) ? "0" : condicion.Split("-")[1];
                    //    if (Int32.TryParse(punto, out numero) && Int32.TryParse(nro, out numero))
                    //    {
                    //        if (Int32.TryParse(punto, out numero))
                    //            results = results.Where(x => x.PuntosDeVenta.Punto == numero);

                    //        if (Int32.TryParse(nro, out numero))
                    //            results = results.Where(x => x.Numero == numero);
                    //    }
                    //    else
                    //        results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion) || x.Tipo.Contains(condicion) || x.Nombre.Contains(condicion));
                    //}
                    //else if (condicion != "")
                    //    results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion) || x.Tipo.Contains(condicion) || x.Nombre.Contains(condicion));
                                                                          
                    if (Int32.TryParse(condicion, out numero))
                    {
                        var lNumero = resultadoInicial.Where(x => x.Numero == Math.Abs(numero));
                        foreach (Comprobantes c in lNumero)
                            lc.Add(c);
                    }

                    if (condicion.Contains("-"))
                    {                        
                        var punto = (string.IsNullOrWhiteSpace(condicion.Split("-")[0])) ? "0" : condicion.Split("-")[0];
                        var nro = (string.IsNullOrWhiteSpace(condicion.Split("-")[1])) ? "0" : condicion.Split("-")[1];
                        if (Int32.TryParse(punto, out numero) && Int32.TryParse(nro, out numero))
                        {
                            Int32 nroPunto = Convert.ToInt32(punto);
                            Int32 nroNumero = Convert.ToInt32(nro);
                            var lFactura = resultadoInicial.Where(x => x.PuntosDeVenta.Punto == nroPunto && x.Numero == nroNumero);

                            foreach (Comprobantes c in lFactura)
                                lc.Add(c);
                        } 
                    }

                    if (condicion != "" && !condicion.Contains("-"))
                    {
                        var lNombres = resultadoInicial.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion) || x.Tipo.Contains(condicion) || x.Nombre.Contains(condicion));
                        foreach (Comprobantes c in lNombres)
                            lc.Add(c);
                    }
                    else
                    {
                        foreach (Comprobantes c in resultadoInicial)
                            lc.Add(c);
                    }


                    //if (condicion != "")
                    //{
                    //    var lcDetalle = from c in dbContext.Comprobantes
                    //                    join cd in dbContext.ComprobantesDetalle on c.IDComprobante equals cd.IDComprobante
                    //                    join con in dbContext.Conceptos on cd.IDConcepto equals con.IDConcepto
                    //                    join per in dbContext.Personas on c.IDPersona equals per.IDPersona
                    //                    where c.IDUsuario == usu.IDUsuario 
                    //                          && !per.NroDocumento.Equals("99999999")
                    //                          && con.Nombre.Contains(condicion)
                    //                    select c;
                    //    foreach (Comprobantes c in lcDetalle)
                    //        lc.Add(c);
                    //}
                    //else
                    //{
                    //    var lcInicial = from c in dbContext.Comprobantes
                    //                    join cd in dbContext.ComprobantesDetalle on c.IDComprobante equals cd.IDComprobante
                    //                    join con in dbContext.Conceptos on cd.IDConcepto equals con.IDConcepto
                    //                    join per in dbContext.Personas on c.IDPersona equals per.IDPersona
                    //                    where c.IDUsuario == usu.IDUsuario
                    //                          && !per.NroDocumento.Equals("99999999")                                            
                    //                    select c;
                    //    foreach (Comprobantes c in lcInicial)
                    //        lc.Add(c);
                    //}

                    List<int?> listaComprobantes = new List<int?>();

                    if (lc != null)
                    {
                        foreach (Comprobantes item in lc.Distinct())
                        {
                            int idComprobante = Convert.ToInt32(item.IDComprobante.ToString());
                            listaComprobantes.Add(idComprobante);
                        }
                    }

                    var results = comprobantesTotales.Where(x => listaComprobantes.Contains(x.IDComprobante)).AsQueryable();


                    if (!string.IsNullOrWhiteSpace(fechaUltimoLiquidoProducto))
                    {
                        DateTime dtFechaUltimoLiquidoProducto = DateTime.Parse(fechaUltimoLiquidoProducto + " 12:59:59 pm");
                        results = results.Where(x => x.FechaEntrega <= dtFechaUltimoLiquidoProducto);
                    }

                    if (!periodo.Equals("-2"))
                    {
                        if (!string.IsNullOrWhiteSpace(fechaDesde))
                        {
                            DateTime dtDesde = DateTime.Parse(fechaDesde);
                            results = results.Where(x => x.FechaComprobante >= dtDesde);
                        }
                        if (!string.IsNullOrWhiteSpace(fechaHasta))
                        {
                            DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                            results = results.Where(x => x.FechaComprobante <= dtHasta);
                        }
                    }

                    if (entregaPendiente)
                    {
                        //List<int?> lcEdaPendiente = new List<int?>();
                        //var tipoFacturas = new[] { "PDV" };

                        //var listaPdv = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && tipoFacturas.Contains(w.Tipo)).ToList();
                        //var listaPdvSinEntregas = listaPdv.ToList();

                        //var listaEda = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Equals("EDA")).ToList();

                        //foreach (Comprobantes c in listaPdv)
                        //{
                        //    var busqueda = listaEda.Where(w => w.IdComprobanteVinculado == c.IDComprobante).ToList();
                        //    foreach (Comprobantes cs in busqueda)
                        //    {
                        //        foreach (ComprobantesDetalle cd in c.ComprobantesDetalle)
                        //        {
                        //            cd.Cantidad = cd.Cantidad - cs.ComprobantesDetalle.Where(w => w.IDConcepto == cd.IDConcepto).Select(s => s.Cantidad).FirstOrDefault();
                        //        }
                        //    }

                        //    if (c.ComprobantesDetalle.Sum(s => s.Cantidad) == 0)
                        //        listaPdvSinEntregas.Remove(c);
                        //}

                        //foreach (Comprobantes c in listaPdvSinEntregas)
                        //    lcEdaPendiente.Add(c.IDComprobante);

                        //results = results.Where(x => lcEdaPendiente.Contains(x.IDComprobante));

                        List<ComprobanteBusqueda> listaPdv = new List<ComprobanteBusqueda>();
                        List<ComprobanteBusqueda> listaEda = new List<ComprobanteBusqueda>();

                        List<int?> lcEdaPendiente = new List<int?>();
                        var tipoFacturas = new[] { "PDV" };

                        listaPdv = comprobantesTotales
                                        .Where(w => w.IDUsuario == usu.IDUsuario && tipoFacturas.Contains(w.Tipo))
                                        .Select(x => new ComprobanteBusqueda()
                                        {
                                            IdComprobante = x.IDComprobante,
                                            IdComprobanteVinculado = x.IdComprobanteVinculado,
                                            compDet = x.ComprobantesDetalle
                                        })
                                        .ToList();
                        var listaPdvSinEntregas = listaPdv.ToList();

                        listaEda = comprobantesTotales
                                        .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Equals("EDA"))
                                         .Select(x => new ComprobanteBusqueda()
                                         {
                                             IdComprobante = x.IDComprobante,
                                             IdComprobanteVinculado = x.IdComprobanteVinculado,
                                             compDet = x.ComprobantesDetalle
                                         })
                                        .ToList();

                        foreach (ComprobanteBusqueda c in listaPdv)
                        {

                            var busqueda = listaEda.Where(w => w.IdComprobanteVinculado == c.IdComprobante).ToList();
                            foreach (ComprobanteBusqueda cs in busqueda)
                            {
                                foreach (ComprobantesDetalle cd in c.compDet)
                                {
                                    if (cs.compDet != null)
                                    {
                                        foreach (ComprobantesDetalle i in cs.compDet)
                                        {
                                            if(i.IDConcepto == cd.IDConcepto)
                                                cd.Cantidad = cd.Cantidad - i.Cantidad;
                                        }

                                        //cd.Cantidad = cd.Cantidad - cs.compDet.Where(w => w.IDConcepto == cd.IDConcepto).Select(s => s.Cantidad).FirstOrDefault();
                                    }                                    
                                }
                            }

                            if (c.compDet.Sum(s => s.Cantidad) == 0)
                                listaPdvSinEntregas.Remove(c);
                        }

                        foreach (ComprobanteBusqueda c in listaPdvSinEntregas)
                            lcEdaPendiente.Add(c.IdComprobante);

                        results = results.Where(x => lcEdaPendiente.Contains(x.IDComprobante));

                    }

                    if (entregaEstado)
                    {
                        results = results.Where(x => x.Estado.Equals(estado));
                    }

                    if (cobranzaPendiente)
                    {
                        //List<int?> lcPdvConSaldoCero = new List<int?>();
                        //List<int?> lcFacConSaldoPendiente = new List<int?>();
                        //List<int?> lcPDVCobradas = new List<int?>();
                        //var tipoFacturas = new[] { "PDV" };

                        //var listaPdvConSaldoCero = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && tipoFacturas.Contains(w.Tipo) && w.Saldo == 0).ToList();

                        //foreach (Comprobantes c in listaPdvConSaldoCero)
                        //    lcPdvConSaldoCero.Add(c.IDComprobante);

                        ////Verifico que los saldos cero no sea por hacer copia a factura
                        //var listaFacDesdeCopia = dbContext.Comprobantes
                        //                            .Where(w => lcPdvConSaldoCero.Contains(w.IdComprobanteVinculado) && w.Tipo.Contains("F") && w.Saldo != 0).ToList();

                        //foreach (Comprobantes c in listaFacDesdeCopia)
                        //    lcFacConSaldoPendiente.Add(c.IDComprobante);

                        //// Listado definitivo
                        //var listaPDVCobradas = listaPdvConSaldoCero.Where(w => !lcFacConSaldoPendiente.Contains(w.IdComprobanteVinculado)).ToList();

                        //foreach (Comprobantes c in listaPDVCobradas)
                        //    lcPDVCobradas.Add(c.IDComprobante);

                        //results = results.Where(x => !lcPDVCobradas.Contains(x.IDComprobante));

                        //List<ComprobanteBusqueda> listaPdvConSaldoCero = new List<ComprobanteBusqueda>();
                        //List<ComprobanteBusqueda> listaPdvTotal = new List<ComprobanteBusqueda>();
                        //List<ComprobanteBusqueda> listaEdaConSaldoCero = new List<ComprobanteBusqueda>();
                        //List<ComprobanteBusqueda> listaEdaTotal = new List<ComprobanteBusqueda>();
                        //List<ComprobanteBusqueda> listaFacDesdeCopia = new List<ComprobanteBusqueda>();
                        //List<ComprobanteBusqueda> listaFacTotal = new List<ComprobanteBusqueda>();


                        List<int?> lcPdvConSaldoCero = new List<int?>();
                        List<int> lcPdvTotal = new List<int>();
                        List<int?> lcFacConSaldoPendiente = new List<int?>();
                        List<int?> lcPDVCobradas = new List<int?>();
                        var tipoFacturas = new[] { "PDV" };

                        lcPdvTotal = comprobantesTotales.Where(w => tipoFacturas.Contains(w.Tipo)).Select(s => s.IDComprobante).ToList();

                        foreach (int d in lcPdvTotal)
                        {
                            if(ComprobantesCommon.CalcularPendienteCobranza(d,usu) == 0)
                                lcPDVCobradas.Add(d);
                        }

                        //listaPdvConSaldoCero = comprobantesTotales
                        //                            .Where(w => tipoFacturas.Contains(w.Tipo) && w.Saldo == 0)
                        //                            .Select(x => new ComprobanteBusqueda()
                        //                            {
                        //                                IdComprobante = x.IDComprobante,
                        //                                IdComprobanteVinculado = x.IdComprobanteVinculado
                        //                            })
                        //                            .ToList();

                        //foreach (ComprobanteBusqueda c in listaPdvConSaldoCero)
                        //    lcPdvConSaldoCero.Add(c.IdComprobante);

                        //listaPdvTotal = comprobantesTotales
                        //    .Where(w => tipoFacturas.Contains(w.Tipo) && w.Saldo != 0)
                        //    .Select(x => new ComprobanteBusqueda()
                        //    {
                        //        IdComprobante = x.IDComprobante,
                        //        IdComprobanteVinculado = x.IdComprobanteVinculado
                        //    })
                        //    .ToList();

                        //listaPdvTotal = comprobantesTotales
                        //    .Where(w => tipoFacturas.Contains(w.Tipo))
                        //    .Select(x => new ComprobanteBusqueda()
                        //    {
                        //        IdComprobante = x.IDComprobante,
                        //        IdComprobanteVinculado = x.IdComprobanteVinculado,
                        //        ImporteTotalNeto = x.Saldo
                        //    })
                        //    .ToList();

                        //foreach (ComprobanteBusqueda c in listaPdvTotal)
                        //    lcPdvTotal.Add(c.IdComprobante);

                        //listaEdaTotal = comprobantesTotales
                        //    .Where(w => lcPdvTotal.Contains(w.IdComprobanteVinculado) && w.Tipo.Equals("EDA"))
                        //    .Select(x => new ComprobanteBusqueda()
                        //    {
                        //        IdComprobante = x.IDComprobante,
                        //        IdComprobanteVinculado = x.IdComprobanteVinculado,
                        //        ImporteTotalNeto = x.Saldo
                        //    })
                        //    .ToList();                

                        //foreach (ComprobanteBusqueda c in listaEdaTotal)
                        //    lcPdvTotal.Add(c.IdComprobante);

                        ////Verifico que los saldos cero no sea por hacer copia a factura
                        //listaFacTotal = comprobantesTotales
                        //                            .Where(w => lcPdvTotal.Contains(w.IdComprobanteVinculado) && w.Tipo.Contains("F"))
                        //                            .Select(x => new ComprobanteBusqueda()
                        //                            {
                        //                                IdComprobante = x.IDComprobante,
                        //                                IdComprobanteVinculado = x.IdComprobanteVinculado,
                        //                                ImporteTotalNeto = x.Saldo
                        //                            })
                        //                            .ToList();

                        //foreach (ComprobanteBusqueda c in listaFacTotal)
                        //    lcPdvTotal.Add(c.IdComprobante);


                        //foreach (int? d in lcPdvTotal)
                        //{
                            
                        //}

                        //// Listado definitivo
                        //var listaPDVCobradas = listaPdvConSaldoCero.Where(w => !lcFacConSaldoPendiente.Contains(w.IdComprobanteVinculado)).ToList();



                        //foreach (ComprobanteBusqueda c in listaPDVCobradas)
                        //    lcPDVCobradas.Add(c.IdComprobante);

                        //var listaEDACobradas = listaEdaConSaldoCero.Where(w => !lcFacConSaldoPendiente.Contains(w.IdComprobanteVinculado)).ToList();

                        //foreach (ComprobanteBusqueda c in listaEDACobradas)
                        //{
                        //    lcPDVCobradas.Add(c.IdComprobanteVinculado);
                        //}

                        results = results.Where(x => !lcPDVCobradas.Contains(x.IDComprobante));

                    }

                    if (facturaPendiente)
                    {
                        //List<int?> lcPdv = new List<int?>();
                        //List<int?> lcPDVFacturados = new List<int?>();
                        //var tipoFacturas = new[] { "PDV" };

                        //var listaPdv = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && tipoFacturas.Contains(w.Tipo)).ToList();

                        //foreach (Comprobantes c in listaPdv)
                        //    lcPdv.Add(c.IDComprobante);

                        ////Verifico que las facturas vinculadas sumen el total pdv
                        //foreach (Comprobantes c in listaPdv)
                        //{
                        //    decimal total = c.ImporteTotalNeto;
                        //    decimal sumaFacturas = 0;

                        //    var listaFacDesdePdv = dbContext.Comprobantes
                        //                       .Where(w => lcPdv.Contains(w.IdComprobanteVinculado) && w.Tipo.Contains("F") && w.CAE != null).ToList();

                        //    foreach (Comprobantes f in listaFacDesdePdv)
                        //    {
                        //        sumaFacturas += f.ImporteTotalNeto;
                        //    }

                        //    if (total == sumaFacturas)
                        //        lcPDVFacturados.Add(c.IDComprobante);

                        //}

                        //results = results.Where(x => !lcPDVFacturados.Contains(x.IDComprobante));

                        List<ComprobanteBusqueda> listaPdv = new List<ComprobanteBusqueda>();
                        List<ComprobanteBusqueda> listaFacDesdePdv = new List<ComprobanteBusqueda>();

                        List<int?> lcPdv = new List<int?>();
                        List<int?> lcPDVFacturados = new List<int?>();
                        var tipoFacturas = new[] { "PDV" };

                        listaPdv = results
                                        .Where(w => w.IDUsuario == usu.IDUsuario && tipoFacturas.Contains(w.Tipo))
                                        .Select(x => new ComprobanteBusqueda()
                                        {
                                            IdComprobante = x.IDComprobante,
                                            IdComprobanteVinculado = x.IdComprobanteVinculado,
                                            ImporteTotalNeto = x.ImporteTotalNeto
                                        })
                                        .ToList();

                        foreach (ComprobanteBusqueda c in listaPdv)
                            lcPdv.Add(c.IdComprobante);

                        listaFacDesdePdv = comprobantesTotales
                            .Where(w => lcPdv.Contains(w.IdComprobanteVinculado) && w.Tipo.Contains("F") && w.CAE != null)
                            .Select(x => new ComprobanteBusqueda()
                            {
                                IdComprobante = x.IDComprobante,
                                IdComprobanteVinculado = x.IdComprobanteVinculado,
                                ImporteTotalNeto = x.ImporteTotalNeto
                            })
                            .ToList();


                        //Verifico que las facturas vinculadas sumen el total pdv
                        foreach (ComprobanteBusqueda c in listaPdv)
                        {
                            decimal total = c.ImporteTotalNeto;
                            decimal sumaFacturas = 0;

                            var listaFacDesdeUnPdv = listaFacDesdePdv
                                        .Where(w => w.IdComprobanteVinculado == c.IdComprobante)
                                        .ToList();

                            foreach (ComprobanteBusqueda f in listaFacDesdeUnPdv)
                            {
                                sumaFacturas += f.ImporteTotalNeto; 
                            }

                            if (total == sumaFacturas)
                                lcPDVFacturados.Add(c.IdComprobante);                            

                        }

                        results = results.Where(x => !lcPDVFacturados.Contains(x.IDComprobante));

                    }


                    page--;
                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),                            
                            Tipo = x.Tipo,
                            Nombre = x.Nombre,
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T"
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ResultadosComprobantesViewModel ObtenerComprobante(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).AsQueryable();

                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                    var list = results.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T",
                            IVA = (x.ImporteTotalNeto - x.ImporteTotalBruto).ToString("N2")
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static decimal CalcularPendienteCAE(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    //List<Comprobantes> lcomp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F")).ToList();

                    //decimal totalFacturado = dbContext.Comprobantes
                    //            .Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id && x.Tipo.Contains("F"))
                    //            .Select(x => x.ImporteTotalNeto)
                    //            .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturado = dbContext.Comprobantes
                    //                                .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F"))
                    //                                .Select(x => x.ImporteTotalNeto)
                    //                                .DefaultIfEmpty(0).Sum();

                    List<int?> listaEda = dbContext.Comprobantes.Where(x => x.IdComprobanteVinculado == id && x.Tipo.Equals("EDA"))
                                .Select(a => (int?)a.IDComprobante)
                                .ToList();

                    listaEda.Add(id);

                    decimal totalFacturado = dbContext.Comprobantes
                                                    .Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F"))
                                                    .Select(x => x.ImporteTotalBruto)
                                                    .DefaultIfEmpty(0).Sum();


                    decimal totalFacturadoCAE = dbContext.Comprobantes
                                                    .Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F") && x.CAE != null)
                                                    .Select(x => x.ImporteTotalBruto)
                                                    .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturado = dbContext.Comprobantes
                    //            .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F"))
                    //            .Select(x => x.ImporteTotalNeto)
                    //            .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturadoCAE = dbContext.Comprobantes
                    //                                .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F") && x.CAE != null)
                    //                                .Select(x => x.ImporteTotalNeto)
                    //                                .DefaultIfEmpty(0).Sum();

                    //return totalFacturado - totalFacturadoCAE;

                    return totalFacturado - totalFacturadoCAE;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static decimal CalcularPendienteFacturar(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    //List<Comprobantes> lcomp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("PDV")).ToList();

                    decimal totalPedidoDeVenta = dbContext.Comprobantes
                                .Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id)
                                .Select(x => x.ImporteTotalBruto)
                                .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturado = dbContext.Comprobantes
                    //                                .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F"))
                    //                                .Select(x => x.ImporteTotalNeto)
                    //                                .DefaultIfEmpty(0).Sum();

                    List<int?> listaEda = dbContext.Comprobantes.Where(x => x.IdComprobanteVinculado == id && x.Tipo.Equals("EDA"))
                                .Select(a => (int?)a.IDComprobante)
                                .ToList();

                    listaEda.Add(id);


                    decimal totalFacturado = dbContext.Comprobantes
                                                    .Where(x => x.IDUsuario == usu.IDUsuario && listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Contains("F"))
                                                    .Select(x => x.ImporteTotalBruto)
                                                    .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturado = dbContext.Comprobantes
                    //            .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F"))
                    //            .Select(x => x.ImporteTotalNeto)
                    //            .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturadoCAE = dbContext.Comprobantes
                    //                                .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F") && x.CAE != null)
                    //                                .Select(x => x.ImporteTotalNeto)
                    //                                .DefaultIfEmpty(0).Sum();

                    //return totalFacturado - totalFacturadoCAE;

                    return totalPedidoDeVenta - totalFacturado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static decimal CalcularPendienteEntrega(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    List<int> lcomp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id && x.Tipo.Contains("PDV")).Select(s => s.IDComprobante).ToList();


                    decimal totalPedidoDeVenta = dbContext.ComprobantesDetalle
                                .Where(x => lcomp.Contains(x.IDComprobante))
                                .Select(s => s.Cantidad)
                                .DefaultIfEmpty(0).Sum();


                    List<int> lcompEntrega = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && lcomp.Contains((int)x.IdComprobanteVinculado) && x.Tipo.Contains("EDA"))
                                .Select(s => s.IDComprobante).ToList();


                    decimal totalEntregado = dbContext.ComprobantesDetalle
                                .Where(x => lcompEntrega.Contains(x.IDComprobante))
                                .Select(s => s.Cantidad)
                                .DefaultIfEmpty(0).Sum();


                    //decimal totalFacturado = dbContext.Comprobantes
                    //            .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F"))
                    //            .Select(x => x.ImporteTotalNeto)
                    //            .DefaultIfEmpty(0).Sum();

                    //decimal totalFacturadoCAE = dbContext.Comprobantes
                    //                                .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Contains("F") && x.CAE != null)
                    //                                .Select(x => x.ImporteTotalNeto)
                    //                                .DefaultIfEmpty(0).Sum();

                    //return totalFacturado - totalFacturadoCAE;

                    return (100 - ((100 / totalPedidoDeVenta) * totalEntregado));
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static decimal CalcularPendienteCobranza(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    decimal totalCobranza = 0;

                    decimal totalVenta = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).Select(x => x.ImporteTotalBruto).FirstOrDefault();

                    //List<Comprobantes> lcompOriginal = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).ToList();                    

                    //foreach (Comprobantes c in lcompOriginal)
                    //{
                    //    List<CobranzasDetalle> lcd = dbContext.CobranzasDetalle.Where(w => w.IDComprobante == c.IDComprobante).ToList();

                    //    foreach (CobranzasDetalle cd in lcd)
                    //    {
                    //        totalCobranza = totalCobranza + dbContext.CobranzasFormasDePago.Where(x => x.IDCobranza == cd.IDCobranza).Select(x => x.Importe).DefaultIfEmpty(0).Sum();
                    //    }
                    //}

                    List<int?> listaEda = dbContext.Comprobantes.Where(x => x.IdComprobanteVinculado == id && x.Tipo.Equals("EDA"))
                        .Select(a => (int?)a.IDComprobante)
                        .ToList();
                   
                    List<Comprobantes> lcomp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id || listaEda.Contains(x.IdComprobanteVinculado)).ToList();

                    foreach (Comprobantes c in lcomp)
                    {
                        List<CobranzasDetalle> lcd = dbContext.CobranzasDetalle.Where(w => w.IDComprobante == c.IDComprobante).ToList();

                        foreach(CobranzasDetalle cd in lcd)
                        {
                            totalCobranza = totalCobranza + dbContext.CobranzasFormasDePago.Where(x => x.IDCobranza == cd.IDCobranza).Select(x => x.Importe).DefaultIfEmpty(0).Sum();
                        }                        
                    }

                    //totalCobranza = totalCobranza + dbContext.CobranzasDetalle.Where(x => x.IDComprobante == id).Select(x => x.Importe).DefaultIfEmpty(0).Sum();

                    return (totalVenta - totalCobranza) < 0 ? 0 : totalVenta - totalCobranza;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static ResultadosComprobantesViewModel ObtenerComprobanteRaiz(string tipo, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == null && x.Tipo == tipo).AsQueryable();

                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                    var list = results.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T"
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ResultadosComprobantesViewModel ObtenerComprobantesVinculados(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listaComprobantes = dbContext.Comprobantes
                                    .Include("Personas").Include("PuntosDeVenta")
                                    .Where(x => x.IDUsuario == usu.IDUsuario).ToList();


                    List<int?> listaEda = listaComprobantes.Where(x => x.IdComprobanteVinculado == id && x.Tipo.Equals("EDA"))
                                .Select(a => (int?)a.IDComprobante)
                                .ToList();

                    listaEda.Add(id);

                    var comprobantesVinculados = listaComprobantes
                                                    .Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F"))
                                                    .AsQueryable();

                    //var results = dbContext.Comprobantes
                    //                .Include("Personas").Include("PuntosDeVenta")
                    //                .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && !x.Tipo.Equals("EDA")).AsQueryable();

                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                    var list = comprobantesVinculados.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            CAE = (x.CAE != null) ? x.CAE : "Borrador",
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T",
                            IVA = (x.ImporteTotalNeto - x.ImporteTotalBruto).ToString("N2")
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ResultadosComprobantesViewModel ObtenerEntregasVinculadas(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes
                                        .Include("Personas").Include("PuntosDeVenta")
                                        .Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Equals("EDA")).AsQueryable();

                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                    var list = results.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T"
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ResultadosComprobantesViewModel ObtenerComprobantesVinculadosAUnPedidoDeCompra(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id && x.Tipo.Equals("PDV")).AsQueryable();

                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                    var list = results.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T"
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static ResultadosComprobantesViewModel ObtenerComprobantesParaJuntar(int idPersona, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo.Equals("PDV") && x.IDPersona == idPersona).AsQueryable();

                    ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();

                    var list = results.OrderByDescending(x => x.FechaComprobante).ThenByDescending(x => x.IDComprobante).ToList()
                        .Select(x => new ComprobantesViewModel()
                        {
                            ID = x.IDComprobante,
                            RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                            //RazonSocial = (string.IsNullOrEmpty(x.Nombre) ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : x.Nombre.ToUpper()),
                            Fecha = x.FechaComprobante.ToString(formatoFecha),
                            Tipo = x.Tipo,
                            Nombre = x.Nombre,
                            Modo = x.Modo == "E" ? "Electrónica" : (x.Modo == "T" ? "Talonario" : "Otro"),
                            Numero = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                            ImporteTotalNeto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalNeto.ToString("N2") : x.ImporteTotalNeto.ToString("N2"),
                            ImporteTotalBruto = (x.Tipo == "NCA" || x.Tipo == "NCB" || x.Tipo == "NCC") ? "-" + x.ImporteTotalBruto.ToString("N2") : x.ImporteTotalBruto.ToString("N2"),
                            PuedeAdm = x.FechaCAE.HasValue ? "F" : "T"
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static bool EliminarComprobante(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.FechaCAE.HasValue)
                        throw new CustomException("No se puede eliminar por estar informado a la AFIP");
                    else if (dbContext.CobranzasDetalle.Any(x => x.IDComprobante == id))
                    {
                        var cobranzaDetalle = dbContext.CobranzasDetalle.Where(x => x.IDComprobante == id).ToList();

                        foreach(var cob in cobranzaDetalle)
                        {
                            var cobranzaDetalleCompleto = dbContext.CobranzasDetalle.Where(x => x.IDCobranza == cob.IDCobranza).ToList();
                            dbContext.CobranzasDetalle.RemoveRange(cobranzaDetalleCompleto);
                            dbContext.SaveChanges();

                            var cobranza = dbContext.Cobranzas.Where(x => x.IDCobranza == cob.IDCobranza).FirstOrDefault();

                            dbContext.Cobranzas.Remove(cobranza);
                            dbContext.SaveChanges();
                        }

                        //throw new CustomException("No se puede eliminar por tener cobranzas asociadas");
                    }

                    else if (ContabilidadCommon.ValidarCierreContable(usu, entity.FechaComprobante))
                        throw new CustomException("El comprobante no puede eliminarse ya que el año contable ya fue cerrado.");                                                         
                    else if (dbContext.Comprobantes.Any(x => x.IdComprobanteVinculado == id))
                    {

                        var comprobantesVinculados = dbContext.Comprobantes.Where(x => x.IdComprobanteVinculado == id).ToList();

                        foreach(var com in comprobantesVinculados)
                        {
                            var cobranzaDetalle = dbContext.CobranzasDetalle.Where(x => x.IDComprobante == com.IDComprobante).ToList();

                            foreach (var cob in cobranzaDetalle)
                            {
                                var cobranzaDetalleCompleto = dbContext.CobranzasDetalle.Where(x => x.IDCobranza == cob.IDCobranza).ToList();
                                dbContext.CobranzasDetalle.RemoveRange(cobranzaDetalleCompleto);
                                dbContext.SaveChanges();

                                var cobranza = dbContext.Cobranzas.Where(x => x.IDCobranza == cob.IDCobranza).FirstOrDefault();

                                dbContext.Cobranzas.Remove(cobranza);
                                dbContext.SaveChanges();

                            }

                            dbContext.Comprobantes.Remove(com);
                            dbContext.SaveChanges();
                        }                       

                        //throw new CustomException("El comprobante no puede eliminarse ya que está vinculado a otro comprobante.");
                    }


                    if (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC")
                        ConceptosCommon.RestarStock(dbContext, entity.ComprobantesDetalle.ToList());
                    else
                        ConceptosCommon.SumarStock(dbContext, entity.ComprobantesDetalle.ToList());

                    dbContext.Comprobantes.Remove(entity);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
        }

    }
}