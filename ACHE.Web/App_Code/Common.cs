using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using ACHE.FacturaElectronica;
using System.Configuration;
using System.Security.Cryptography;
using ACHE.Negocio.Productos;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Common;
using ACHE.Extensions;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;
using ACHE.FacturaElectronica.Lib;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ACHE.FacturaElectronica.WSFacturaElectronica;

/// <summary>
/// Descripción breve de Common
/// </summary>
public static class Common
{

    public enum ComprobanteModo
    {
        Previsualizar = 1,
        Generar = 2,
        GenerarRemito = 3,
        GenerarPDF = 4
    }

    public static string MD5Hash(string input)
    {
        StringBuilder hash = new StringBuilder();
        MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

        for (int i = 0; i < bytes.Length; i++)
        {
            hash.Append(bytes[i].ToString("x2"));
        }
        return hash.ToString();
    }
    public static bool validarPassword(string password)
    {
        bool valido = false;
        if (password != null)
        {
            if (!password.Equals(""))
            {
                if (password.Length >= 6 && password.Length <= 20)
                {
                    var r = new Regex(@"^(?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])\S{8,12}$");
                    if (r.Match(password).Success)
                    {
                        valido = true;
                    }
                }
            }
        }
        return valido;
    }

    public static void CrearComprobanteDesdeIdComprobante(int IdComprobante)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        var dbContext = new ACHEEntities();

        Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == IdComprobante).FirstOrDefault();

        if(c != null)
        {
            string nroComprobante = "";

            Common.CrearComprobante(usu, IdComprobante, c.IDPersona, c.Tipo, c.Modo, c.FechaComprobante.ToString(), c.CondicionVenta,
               c.TipoConcepto, c.FechaVencimiento.ToString(), c.IDPuntoVenta, ref nroComprobante, c.Observaciones,
               (int)c.IdComprobanteAsociado, "", c.FechaEntrega.ToString(), false, c.IdActividad, c.ModalidadPagoAFIP, Common.ComprobanteModo.Generar);

        }
        else
            throw new Exception("El Comprobante no existe");

    }

    public static void CrearComprobante(WebUser usu, int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
        int tipoConcepto, string fechaVencimiento, int idPuntoVenta, ref string nroComprobante, string obs, int idComprobanteAsociado, 
        string vendedor, string fechaEntrega, bool notaDeCreditoPorServicio, int idActividad, string modalidadPagoAfip, ComprobanteModo accion)
    {
        var pathPdf = "";
        //var pathCertificado = "";

        if (accion == ComprobanteModo.Previsualizar)
        {
            pathPdf = HttpContext.Current.Server.MapPath("~/files/comprobantes/" + usu.IDUsuario + "_prev.pdf");
            if (System.IO.File.Exists(pathPdf))
                System.IO.File.Delete(pathPdf);
        }
        else
            pathPdf = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Comprobantes/" + DateTime.Now.Year.ToString() + "/");

        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (persona == null)
                throw new Exception("El cliente/proveedor es inexistente");

            Usuarios usuario = dbContext.Usuarios.Where(w => w.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (usuario == null)
                throw new Exception("El usuario es inexistente");

            if (ComprobanteModo.Generar == accion || ComprobanteModo.GenerarPDF == accion)
            {
                if (usu.CUIT.Equals("20147963271") && id != 0 && !tipo.Contains("N"))
                {
                    Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();

                    if(c != null)
                    {
                        Comprobantes cpdv = dbContext.Comprobantes.Where(x => x.IDComprobante == c.IdComprobanteVinculado && x.Tipo.Equals("PDV")).FirstOrDefault();

                        if (cpdv == null)
                            throw new Exception("No es posible facturar fuera de un pedido de venta vinculado.");
                    }                    
                }                   

                pathPdf = pathPdf + persona.RazonSocial.RemoverCaracteresParaPDF() + "_";

                if (tipo.Contains("F"))
                {
                    Comprobantes compFAC = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();
                    if (compFAC != null)
                    {
                        Comprobantes compPDV = dbContext.Comprobantes.Where(x => x.IDComprobante == compFAC.IdComprobanteVinculado && x.Tipo.Equals("PDV")).FirstOrDefault();
                        if (compPDV != null)
                        {
                            if (compFAC.ImporteTotalNeto > compPDV.ImporteTotalNeto)
                            {
                                throw new Exception("El importe de la factura no debe superar al PDV vinculado.");
                            }
                        }
                    }

                }

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

            comprobante.IDComprobante = id.ToString();
            comprobante.TipoComprobante = ComprobantesCommon.ObtenerTipoComprobante(tipo);
            comprobante.Tipo = tipo;
            comprobante.NotaDeCreditoPorServicio = notaDeCreditoPorServicio;

            //var feMode = ConfigurationManager.AppSettings["FE.Modo"];
            //if (usu.ModoQA)
            //{
            //    //Cambio de CUIT hardcoded
            //    comprobante.Cuit = 23185938999;//long.Parse(usu.CUIT);
            //    comprobante.PtoVta = 5;// puntoVenta;
            //    //pathCertificado = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.CertificadoAFIP.QA"]);
            //}
            //else
            //{
            //    comprobante.Cuit = long.Parse(usu.CUIT);
            //    comprobante.PtoVta = puntoVenta;
            //    //pathCertificado = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.CertificadoAFIP.PROD"]);
            //}

            comprobante.NroPedidoDeVenta = "";
            comprobante.NroPresupuesto = "";

            Comprobantes comp = dbContext.Comprobantes.Where(w => w.IDComprobante == id).FirstOrDefault();
            if(comp != null)
            {
                if(comp.Tipo == "EDA")
                {
                    Comprobantes compPdv = dbContext.Comprobantes.Where(w => w.IDComprobante == comp.IdComprobanteVinculado).FirstOrDefault();
                    if (compPdv != null)
                    {
                        comprobante.NroPedidoDeVenta = compPdv.PuntosDeVenta.Punto.ToString("#0000") + "-" + compPdv.Numero.ToString().PadLeft(8, '0');

                        Presupuestos compPre = dbContext.Presupuestos.Where(w => w.IDPresupuesto == compPdv.IdPresupuestoVinculado).FirstOrDefault();
                        if (compPre != null)
                            comprobante.NroPresupuesto = compPre.Numero.ToString().PadLeft(8, '0');
                    }
                }

                if (comp.Tipo == "PDV")
                {
                    Presupuestos compPre = dbContext.Presupuestos.Where(w => w.IDPresupuesto == comp.IdPresupuestoVinculado).FirstOrDefault();
                    if (compPre != null)
                        comprobante.NroPresupuesto = compPre.Numero.ToString().PadLeft(8, '0');
                }

            }


            comprobante.Cuit = long.Parse(usu.CUIT);
            comprobante.PtoVta = puntoVenta;
            comprobante.Concepto = (FEConcepto)tipoConcepto;
            comprobante.Fecha = DateTime.Parse(fecha);
            if(fechaEntrega.Equals(""))
                comprobante.FechaEntrega = comprobante.Fecha;
            else
                comprobante.FechaEntrega = DateTime.Parse(fechaEntrega);

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
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";           

            #endregion

            #region Datos del cliente

            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento == string.Empty && persona.TipoDocumento == "DNI")
                    comprobante.DocNro = 0;

                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }              

            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }

            comprobante.CondicionVenta = condicionVenta;
            comprobante.ClienteNombre = persona.RazonSocial;
            comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);
            comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
            comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
            comprobante.Observaciones = obs;
            comprobante.Vendedor = vendedor;
            comprobante.Original = (accion == ComprobanteModo.Previsualizar ? false : true);
            if (tipo == "PRE")
                comprobante.Original = true;

            #endregion

            #region Seteo los datos de la factura


            if (accion == ComprobanteModo.GenerarPDF)
            {
                var c = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (c != null && !string.IsNullOrWhiteSpace(c.CAE))
                {
                    comprobante.CAE = c.CAE;
                    comprobante.FechaVencCAE = Convert.ToDateTime(c.FechaCAE);
                }
            }
            //comprobante.ImpTotConc = 0;
            //comprobante.ImpOpEx = 0;
            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();


            var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();

            //DESCUENTO
            //list.Add(new ComprobantesDetalleViewModel()
            //{
            //    Codigo = "1111",
            //    Cantidad = 1,
            //    Bonificacion = 0,
            //    Concepto = "Descuento global",
            //    PrecioUnitario = -100,
            //    Iva = 0
            //});

            if (list.Any())
            {
                foreach (var detalle in list)
                {
                    if (usu.CUIT.Equals("30716909839"))
                        detalle.PrecioUnitario = detalle.PrecioUnitario / 1000;
                                       
                    if ((usu.CondicionIVA == "RI" && persona.CondicionIva == "RI") 
                        || (usu.CondicionIVA == "RI" && persona.CondicionIva == "MO")
                        || (usu.CondicionIVA == "RI" && persona.CondicionIva == "CF")
                        || (usu.CondicionIVA == "RI" && persona.CondicionIva == "EX"))
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = Convert.ToDouble(detalle.Cantidad),
                            Descripcion = detalle.Concepto,
                            Precio = Math.Round(double.Parse(detalle.PrecioUnitarioSinIVA.ToString()), 2),
                            PrecioConIVA = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        ////totalizar importe e IVA
                        //switch (detalle.Iva.ToString("N2"))
                        //{
                        //    case "21,00":
                        //        ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                        //        break;
                        //    case "27,00":
                        //        ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                        //        break;
                        //    case "10,50":
                        //        ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                        //        break;
                        //    case "5,00":
                        //        ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                        //        break;
                        //    case "2,50":
                        //        ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                        //        break;
                        //    case "0,00":
                        //        ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                        //        break;
                        //}

                        //totalizar importe e IVA
                        switch (detalle.IdTipoIva)
                        {
                            case (int)FETipoIva.Iva21:
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                                break;
                            case (int)FETipoIva.Iva27:
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                                break;
                            case (int)FETipoIva.Iva10_5:
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                                break;
                            case (int)FETipoIva.Iva5:
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                                break;
                            case (int)FETipoIva.Iva2_5:
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                                break;
                            case (int)FETipoIva.Iva0:
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                                break;
                            //case (int)FETipoIva.Exento:
                            //    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Exento);
                            //    break;
                            //case (int)FETipoIva.NoGravado:
                            //    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.NoGravado);
                            //    break;
                        }

                    }
                    else
                    {
                        FEItemDetalle feid = new FEItemDetalle();
                        feid.Cantidad = Convert.ToDouble(detalle.Cantidad);
                        feid.Descripcion = detalle.Concepto;
                        if (usu.CUIT.Equals("30716909839"))
                            feid.Decimal = true;
                        else
                            feid.Decimal = false;
                        feid.Precio = double.Parse(detalle.PrecioUnitarioConIva.ToString());
                        //feid.Precio = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2);
                        feid.PrecioConIVA = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2);
                        feid.Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "";
                        feid.Bonificacion = detalle.Bonificacion;
                        feid.IdTipoIVA = detalle.IdTipoIva;
                        comprobante.ItemsDetalle.Add(feid);
                      
                        if (comprobante.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                        {
                            comprobante.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2);
                        }
                        else
                        {
                            if (tipo != "FCC" && tipo != "NDC" && tipo != "NCC" && tipo != "RCC" && tipo != "FCCMP" && tipo != "NDCMP" && tipo != "NCCMP")
                                comprobante.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2), TipoIva = FETipoIva.Iva0 });
                            
                        }
                    }

                    if (usu.CUIT.Equals("30716909839"))
                        detalle.PrecioUnitario = detalle.PrecioUnitario * 1000;

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

            comprobante.ImpTotConc = (double)ComprobanteCart.Retrieve().GetImporteNoGravado();
            comprobante.ImpOpEx = (double)ComprobanteCart.Retrieve().GetImporteExento();

            if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIIBB)
            {
                if ((double)ComprobanteCart.Retrieve().GetPercepcionIIBB() > 0)
                {
                    comprobante.Tributos.Add(new FERegistroTributo()
                    {
                        BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                        Alicuota = (double)ComprobanteCart.Retrieve().PercepcionIIBB,
                        Importe = Math.Round((double)ComprobanteCart.Retrieve().GetPercepcionIIBB(), 2),
                        Decripcion = "Percepción IIBB",
                        Tipo = FETipoTributo.ImpuestosNacionales
                    });
                }
            }

            if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIVA)
            {
                if ((double)ComprobanteCart.Retrieve().GetPercepcionIVA() > 0)
                {
                    comprobante.Tributos.Add(new FERegistroTributo()
                    {
                        BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                        Alicuota = (double)ComprobanteCart.Retrieve().PercepcionIVA,
                        Importe = Math.Round((double)ComprobanteCart.Retrieve().GetPercepcionIVA(), 2),
                        Decripcion = "Percepción IVA",
                        Tipo = FETipoTributo.ImpuestosNacionales
                    });
                }
            }

            if(idActividad != 0)
            {
                ACHE.Model.Actividad act = dbContext.Actividad.Where(w => w.IdActividad == idActividad).FirstOrDefault();

                if (act != null)
                {
                    if(Convert.ToInt64(act.Codigo) != 0)
                    {
                        comprobante.Actividades.Add(new FEActividad()
                        {
                            Id = long.Parse(act.Codigo)
                        });
                    }             
                }
                else
                    throw new CustomException("El usuario no tiene cargada una actividad, cargarlo en 'Mis Datos'.");
            }
            else
                throw new Exception("Debe seleccionar una actividad.");


            if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.FACTURAS_A_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.FACTURAS_B_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.FACTURAS_C_MiPyMEs)
            {
                if (!modalidadPagoAfip.Equals(""))
                {
                    if (usuario.CBU != null)
                    {
                        comprobante.Opcionales.Add(new FEOpcional()
                        {
                            Id = "2101",
                            Valor = usuario.CBU
                        });
                        comprobante.Opcionales.Add(new FEOpcional()
                        {
                            Id = "27",
                            Valor = modalidadPagoAfip
                        });
                    }
                    else
                        throw new CustomException("El usuario no tiene informado un CBU, cargarlo en 'Mis Datos'.");
                }
                else
                    throw new Exception("Debe seleccionar una modalidad de pago AFIP.");
            }

            if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_A_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_B_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_C_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_A_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_B_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_C_MiPyMEs)
            {
                comprobante.Opcionales.Add(new FEOpcional()
                {
                    Id = "22",
                    Valor = "N"
                });
            }

            if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_CREDITO_A ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_CREDITO_B ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_CREDITO_C ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_DEBITO_A ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_DEBITO_B ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_DEBITO_C ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_A_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_B_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_C_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_A_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_B_MiPyMEs ||
                (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_C_MiPyMEs)
            {
                if (idComprobanteAsociado != 0)
                {
                    Comprobantes ca = dbContext.Comprobantes.Where(w => w.IDComprobante == idComprobanteAsociado).FirstOrDefault();
                    if (ca != null)
                    {
                        int puntoVentaAsociado = 0;
                        PuntosDeVenta puntoAsociado = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == ca.IDPuntoVenta && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (puntoAsociado != null)
                            puntoVentaAsociado = puntoAsociado.Punto;

                        if (puntoVentaAsociado == 0)
                            throw new Exception("Punto de venta del comprobante asociado invalido");

                        FEComprobanteAsociado feComAsoc = new FEComprobanteAsociado();
                        feComAsoc.TipoField = (int)ComprobantesCommon.ObtenerTipoComprobante(ca.Tipo); 
                        feComAsoc.PtoVtaField = puntoVentaAsociado;
                        feComAsoc.NroField = ca.Numero;
                        feComAsoc.CuitField = ca.NroDocumento;
                        feComAsoc.CbteFchField = ca.FechaComprobante.ToString("yyyyMMdd");
                        comprobante.ComprobantesAsociados.Add(feComAsoc);
                    }
                    else
                        throw new Exception("No existe el comprobante asociado.");   
                }
                else
                    throw new Exception("Debe seleccionar un comprobante asociado para las notas de credito.");
            }

            if (usuario.TextoFinalFactura != null)            
                comprobante.TextoFinalFactura = usuario.TextoFinalFactura;
            

            var pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"] + usu.TemplateFc + ".pdf");
            var imgLogo = "logo-fc-" + usu.TemplateFc + ".png";
            if (!string.IsNullOrEmpty(usu.Logo))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                    imgLogo = usu.Logo;
            }

            var pathLogo = HttpContext.Current.Server.MapPath("~/files/usuarios/" + imgLogo);
            if (accion == ComprobanteModo.Previsualizar)
                fe.GrabarEnDisco(comprobante, pathPdf, pathTemplateFc, pathLogo);
            else if (accion == ComprobanteModo.GenerarPDF)
            {
                var numero = comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0');
                pathPdf = pathPdf + tipo + "-" + numero + ".pdf";
                fe.GrabarEnDisco(comprobante, pathPdf, pathTemplateFc, pathLogo);
            }
            else//SI no se previsualiza se genera electronicamente el comprobante
            {
                Comprobantes cmp = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();
                if (cmp != null)
                {
                    cmp.FechaProceso = DateTime.Now;
                    int CodigoLogServicio = -1;
                    try
                    {
                        //fe.GenerarComprobante(comprobante, ConfigurationManager.AppSettings["FE.PROD.wsaa"], pathCertificado, pathTemplateFc, pathLogo);     

                        try
                        {
                            //Registro el envio a AFIP para generar CAE.
                            var url = (usu.ModoQA ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);
                            XmlSerializer xsSubmitFeComprobante = new XmlSerializer(typeof(FEComprobante));
                            var subReqFeComprobante = new FEComprobante();
                            var xmlFeComprobante = "";
                            using (var swwFeComprobante = new StringWriter())
                            {
                                using (XmlWriter writerFeComprobante = XmlWriter.Create(swwFeComprobante))
                                {
                                    xsSubmitFeComprobante.Serialize(writerFeComprobante, comprobante);
                                    xmlFeComprobante = swwFeComprobante.ToString(); // Your XML
                                }
                            }
                            CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "wsfev1", xmlFeComprobante, usu.IDUsuario);
                        }
                        catch (Exception ex)
                        {
                            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                            throw new CustomException("Error al acceder a la base de datos, comuniquese con el administrador del sitio.");
                        }

                        try
                        {
                            fe.GenerarComprobante(comprobante, Convert.ToInt64(usu.CUITAfip), pathTemplateFc, pathLogo, (usu.ModoQA ? "QA" : "PROD"), false);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("ValidacionDeToken: No aparecio CUIT en lista de relaciones"))
                            {
                                try
                                {
                                    fe.GenerarComprobante(comprobante, Convert.ToInt64(usu.CUITAfip), pathTemplateFc, pathLogo, (usu.ModoQA ? "QA" : "PROD"), true);
                                }
                                catch (Exception exForce)
                                {
                                    try
                                    {
                                        //Registro la respuesta de AFIP con error.
                                        Common.RegistrarRespuestaLog(CodigoLogServicio, 1, exForce.Message, false);
                                    }
                                    catch (Exception exDb)
                                    {
                                        BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), exDb.Message, " # common.cs # - " + DateTime.Now.ToString() + " - " + exDb.InnerException);
                                    }

                                    throw new Exception(exForce.Message);
                                }
                            }
                            else
                            {
                                try
                                {
                                    //Registro la respuesta de AFIP con error.
                                    Common.RegistrarRespuestaLog(CodigoLogServicio, 1, ex.Message, false);
                                }
                                catch (Exception exDb)
                                {
                                    BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), exDb.Message, " # common.cs # - " + DateTime.Now.ToString() + " - " + exDb.InnerException);
                                }                                

                                throw new Exception(ex.Message);
                            }
                                
                        }

                        try
                        {
                            //Registro la respuesta de AFIP para generar CAE.
                            Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Comprobante generado correctamente - CAE: " + comprobante.CAE + " - FechaCAE: " + comprobante.FechaVencCAE.ToString(), true);
                        }
                        catch (Exception ex)
                        {
                            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                            throw new CustomException("Error al acceder a la base de datos, comuniquese con el administrador del sitio.");
                        }

                        string numeroComprobante = comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0');
                        pathPdf = pathPdf + tipo + "-" + numeroComprobante + ".pdf";

                        cmp.FechaCAE = comprobante.FechaVencCAE;
                        cmp.CAE = comprobante.CAE;
                        cmp.Numero = comprobante.NumeroComprobante;
                        cmp.Error = null;
                        cmp.FechaError = null;

                        //nroComprobante = tipo + "-" + numeroComprobante;
                        nroComprobante = numeroComprobante;
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        cmp.Error = ex.Message;
                        cmp.FechaError = DateTime.Now;
                        dbContext.SaveChanges();

                        if (ex.Message.Contains("###Error###"))
                            throw new CustomException("El servicio de la AFIP se encuentra inaccesible momentáneamente.");
                        else
                            throw new CustomException(cmp.Error, ex.InnerException);
                    }

                    try
                    {
                        fe.GrabarEnDisco(comprobante.ArchivoFactura, pathPdf, pathTemplateFc, pathLogo);
                    }
                    catch (Exception)
                    {
                        throw new CustomException("El comprobante fue emitido correctamente. Pero no pudo ser generado el archivo PDF contáctese con el administrador para obtener más información.");
                    }
                }
            }
        }
    }

    public static void CrearComprobanteTicket(WebUser usu, int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
        int tipoConcepto, string fechaVencimiento, int idPuntoVenta, ref string nroComprobante, string obs, ComprobanteModo accion)
    {
        var pathPdf = "";
        //var pathCertificado = "";

        if (accion == ComprobanteModo.Previsualizar)
        {
            pathPdf = HttpContext.Current.Server.MapPath("~/files/comprobantes/" + usu.IDUsuario + "_prevTicket.pdf");
            if (System.IO.File.Exists(pathPdf))
                System.IO.File.Delete(pathPdf);
        }
        else
            pathPdf = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Comprobantes/" + DateTime.Now.Year.ToString() + "/");

        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (persona == null)
                throw new Exception("El cliente/proveedor es inexistente");

            if (ComprobanteModo.Generar == accion || ComprobanteModo.GenerarPDF == accion)
                pathPdf = pathPdf + persona.RazonSocial.RemoverCaracteresParaPDF() + "_";

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

            comprobante.Cuit = long.Parse(usu.CUIT);
            comprobante.PtoVta = puntoVenta;
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
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";

            #endregion

            #region Datos del cliente

            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }

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


            if (accion == ComprobanteModo.GenerarPDF)
            {
                var c = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (c != null && !string.IsNullOrWhiteSpace(c.CAE))
                {
                    comprobante.CAE = c.CAE;
                    comprobante.FechaVencCAE = Convert.ToDateTime(c.FechaCAE);
                }
            }
            //comprobante.ImpTotConc = 0;
            //comprobante.ImpOpEx = 0;
            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();


            var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();

            //DESCUENTO
            //list.Add(new ComprobantesDetalleViewModel()
            //{
            //    Codigo = "1111",
            //    Cantidad = 1,
            //    Bonificacion = 0,
            //    Concepto = "Descuento global",
            //    PrecioUnitario = -100,
            //    Iva = 0
            //});

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
                            Precio = Math.Round(double.Parse(detalle.PrecioUnitarioSinIVA.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        //totalizar importe e IVA
                        switch (detalle.Iva.ToString("N2"))
                        {
                            case "21,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                                break;
                            case "27,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                                break;
                            case "10,50":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                                break;
                            case "5,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                                break;
                            case "2,50":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                                break;
                            case "0,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                                break;
                        }
                    }
                    else
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = Convert.ToDouble(detalle.Cantidad),
                            Descripcion = detalle.Concepto,
                            Precio = double.Parse(detalle.PrecioUnitarioConIva.ToString()),
                            //Precio = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        if (comprobante.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                        {
                            comprobante.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2);
                        }
                        else
                        {
                            if (tipo != "FCC" && tipo != "NDC" && tipo != "NCC" && tipo != "RCC")
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

            comprobante.ImpTotConc = (double)ComprobanteCart.Retrieve().GetImporteNoGravado();
            if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIIBB)
            {
                if ((double)ComprobanteCart.Retrieve().GetPercepcionIIBB() > 0)
                {
                    comprobante.Tributos.Add(new FERegistroTributo()
                    {
                        BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                        Alicuota = (double)ComprobanteCart.Retrieve().PercepcionIIBB,
                        Importe = Math.Round((double)ComprobanteCart.Retrieve().GetPercepcionIIBB(), 2),
                        Decripcion = "Percepción IIBB",
                        Tipo = FETipoTributo.ImpuestosNacionales
                    });
                }
            }

            if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIVA)
            {
                if ((double)ComprobanteCart.Retrieve().GetPercepcionIVA() > 0)
                {
                    comprobante.Tributos.Add(new FERegistroTributo()
                    {
                        BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                        Alicuota = (double)ComprobanteCart.Retrieve().PercepcionIVA,
                        Importe = Math.Round((double)ComprobanteCart.Retrieve().GetPercepcionIVA(), 2),
                        Decripcion = "Percepción IVA",
                        Tipo = FETipoTributo.ImpuestosNacionales
                    });
                }
            }
            var pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"] + usu.TemplateFc + ".pdf");
            var imgLogo = "logo-fc-" + usu.TemplateFc + ".png";
            if (!string.IsNullOrEmpty(usu.Logo))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                    imgLogo = usu.Logo;
            }

            var pathLogo = HttpContext.Current.Server.MapPath("~/files/usuarios/" + imgLogo);
            if (accion == ComprobanteModo.Previsualizar)
                fe.GrabarEnDiscoTicket(comprobante, pathPdf, pathTemplateFc, pathLogo);
            else if (accion == ComprobanteModo.GenerarPDF)
            {
                var numero = comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0');
                pathPdf = pathPdf + tipo + "-" + numero + "_Ticket.pdf";
                fe.GrabarEnDiscoTicket(comprobante, pathPdf, pathTemplateFc, pathLogo);
            }
            else//SI no se previsualiza se genera electronicamente el comprobante
            {
                Comprobantes cmp = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();
                if (cmp != null)
                {
                    cmp.FechaProceso = DateTime.Now;
                    int CodigoLogServicio = -1;
                    try
                    {
                        //fe.GenerarComprobante(comprobante, ConfigurationManager.AppSettings["FE.PROD.wsaa"], pathCertificado, pathTemplateFc, pathLogo);     

                        try
                        {
                            //Registro el envio a AFIP para generar CAE.
                            var url = (usu.ModoQA ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);
                            XmlSerializer xsSubmitFeComprobante = new XmlSerializer(typeof(FEComprobante));
                            var subReqFeComprobante = new FEComprobante();
                            var xmlFeComprobante = "";
                            using (var swwFeComprobante = new StringWriter())
                            {
                                using (XmlWriter writerFeComprobante = XmlWriter.Create(swwFeComprobante))
                                {
                                    xsSubmitFeComprobante.Serialize(writerFeComprobante, comprobante);
                                    xmlFeComprobante = swwFeComprobante.ToString(); // Your XML
                                }
                            }
                            CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "wsfev1", xmlFeComprobante, usu.IDUsuario);
                        }
                        catch (Exception ex)
                        {
                            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                            throw new CustomException("Error al acceder a la base de datos, comuniquese con el administrador del sitio.");
                        }

                        try
                        {
                            fe.GenerarComprobante(comprobante, Convert.ToInt64(usu.CUITAfip), pathTemplateFc, pathLogo, (usu.ModoQA ? "QA" : "PROD"), false);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("ValidacionDeToken: No aparecio CUIT en lista de relaciones"))
                                fe.GenerarComprobante(comprobante, Convert.ToInt64(usu.CUITAfip), pathTemplateFc, pathLogo, (usu.ModoQA ? "QA" : "PROD"), true);
                            else
                                throw new Exception(ex.Message);
                        }

                        try
                        {
                            //Registro la respuesta de AFIP para generar CAE.
                            Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Comprobante generado correctamente - CAE: " + comprobante.CAE + " - FechaCAE: " + comprobante.FechaVencCAE.ToString(), true);
                        }
                        catch (Exception ex)
                        {
                            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                            throw new CustomException("Error al acceder a la base de datos, comuniquese con el administrador del sitio.");
                        }

                        string numeroComprobante = comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0');
                        pathPdf = pathPdf + tipo + "-" + numeroComprobante + ".pdf";

                        cmp.FechaCAE = comprobante.FechaVencCAE;
                        cmp.CAE = comprobante.CAE;
                        cmp.Numero = comprobante.NumeroComprobante;
                        cmp.Error = null;
                        cmp.FechaError = null;

                        //nroComprobante = tipo + "-" + numeroComprobante;
                        nroComprobante = numeroComprobante;
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        cmp.Error = ex.Message;
                        cmp.FechaError = DateTime.Now;
                        dbContext.SaveChanges();

                        if (ex.Message.Contains("###Error###"))
                            throw new CustomException("El servicio de la AFIP se encuentra inaccesible momentáneamente.");
                        else
                            throw new CustomException(cmp.Error, ex.InnerException);
                    }

                    try
                    {
                        fe.GrabarEnDiscoTicket(comprobante.ArchivoFactura, pathPdf, pathTemplateFc, pathLogo);
                    }
                    catch (Exception)
                    {
                        throw new CustomException("El comprobante fue emitido correctamente. Pero no pudo ser generado el archivo PDF contáctese con el administrador para obtener más información.");
                    }
                }
            }
        }
    }    

    public static void GenerarRemito(WebUser usu, int id, int idPersona, 
        string fecha, int idPunto, string numero, string condicionVenta, 
        string obs, string vendedor, int idDomicilio, string tipo, 
        string fechaEntrega, FETipoComprobante tipoComprobante, string random,
        string nroPedidoDeVenta, string nroPresupuesto, bool conLogo, string domicilioTransporteCliente)
    {
        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            FEFacturaElectronica fe = new FEFacturaElectronica();
            FEComprobante comprobante = new FEComprobante();
            string numeroComprobanteRemito = string.Empty;

            #region Datos del emisor

            int puntoVenta = 0;
            if (idPunto != 0)
            {
                PuntosDeVenta punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == idPunto && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                puntoVenta = punto.Punto;
                comprobante.PtoVta = puntoVenta;
            }

            var pathLogo = string.Empty;
            if (!string.IsNullOrEmpty(usu.Logo) && conLogo)
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                    pathLogo = HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo);
            }
            else            
                pathLogo = HttpContext.Current.Server.MapPath("~/files/usuarios/logo-fc-default.png");
            

            var pathRemito = string.Empty;
            var pathTemplateFc = string.Empty;
            switch (tipoComprobante)
            {
                case FETipoComprobante.PRESUPUESTO:
                    comprobante.TipoComprobante = tipoComprobante;
                    comprobante.TipoComprobante = tipoComprobante;
                    pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"] + usu.TemplateFc + ".pdf");
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario + "/");
                    break;
                case FETipoComprobante.REMITO:
                    comprobante.TipoComprobante = tipoComprobante;
                    pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateRemito"]);
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/remitos/" + usu.IDUsuario + "/");
                    break;
            }

            comprobante.IDComprobante = id.ToString();
            comprobante.Cuit = long.Parse(usu.CUIT);
            comprobante.Fecha = DateTime.Parse(fecha);

            if (fechaEntrega.Equals(""))
                comprobante.FechaEntrega = comprobante.Fecha;
            else
                comprobante.FechaEntrega = DateTime.Parse(fechaEntrega);

            comprobante.Tipo = tipo;
            comprobante.CodigoMoneda = "PES";
            comprobante.CotizacionMoneda = 1;
            if (numero != string.Empty)
                comprobante.NumeroComprobante = int.Parse(numero);

            comprobante.CondicionVenta = usu.CondicionIVA;
            comprobante.Domicilio = usu.Domicilio;
            comprobante.CiudadProvincia = usu.Ciudad + ", " + usu.Provincia;
            comprobante.RazonSocial = usu.RazonSocial;
            comprobante.Telefono = usu.Telefono;
            comprobante.Celular = usu.Celular;
            if (usu.ExentoIIBB != null)
                comprobante.IIBB = ((bool)usu.ExentoIIBB) ? "Exento" : usu.IIBB;
            comprobante.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usu.CondicionIVA);
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";


            #endregion

            #region Datos del cliente

            /*Int32 nroDNI = 0;
            
            if (Int32.TryParse(persona.NroDocumento, out nroDNI))
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
            else
            {*/
            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }
            //}


            //if (persona.CondicionIva == "CF")
            //    comprobante.DocTipo = 99; // Consumidor final
            //else
            //    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
            //comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);

            comprobante.CondicionVenta = condicionVenta;
            comprobante.ClienteNombre = persona.RazonSocial;
            comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);


            //Me fijo si es en comprobante EDA y chequeo el domicilio de entrega
            if (idDomicilio != 0)
            {
                PersonaDomicilio pd = dbContext.PersonaDomicilio.Where(w => w.IdPersonaDomicilio == idDomicilio).FirstOrDefault();
                comprobante.ClienteDomicilio = pd.Domicilio + " " + pd.PisoDepto;
                comprobante.ClienteLocalidad = pd.Provincia + ", " + pd.Ciudad;
                comprobante.ClienteContacto = "Contacto: " + pd.Contacto + " , Tel: " + pd.Telefono;
            }
            else
            {
                comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
                comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
                comprobante.ClienteContacto = "";
            }
            comprobante.Observaciones = obs;
            comprobante.Vendedor = vendedor;
            comprobante.Original = true;
            #endregion

            #region Seteo los datos de la factura

            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();


            var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();


            if (list.Any())
            {
                foreach (var detalle in list)
                {
                    comprobante.ItemsDetalle.Add(new FEItemDetalle()
                    {
                        Cantidad = Convert.ToDouble(detalle.Cantidad),
                        Descripcion = detalle.Concepto,
                        Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",

                        Precio = (tipoComprobante == FETipoComprobante.PRESUPUESTO) ? Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2) : 0,
                        Bonificacion = (tipoComprobante == FETipoComprobante.PRESUPUESTO) ? detalle.Bonificacion : 0,
                        IdTipoIVA = detalle.IdTipoIva
                        
                    });
                }
            }

            foreach (var item in comprobante.ItemsDetalle)
            {
                if (item.Codigo != "")
                {
                    int idConceto = Convert.ToInt32(item.Codigo);
                    item.Codigo = dbContext.Conceptos.Where(x => x.IDConcepto == idConceto).FirstOrDefault().Codigo.ToString();
                }
            }            

            comprobante.NroPedidoDeVenta = nroPedidoDeVenta;
            comprobante.NroPresupuesto = nroPresupuesto;

            #endregion


            //if (!string.IsNullOrEmpty(usu.Logo) && conLogo)
            //{
            //    if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
            //        imgLogo = usu.Logo;
            //}

            if (puntoVenta != 0)
                numeroComprobanteRemito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + puntoVenta.ToString("#0000") + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0') + "-" + random;
            else
                numeroComprobanteRemito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0') + "-" + random;

            pathRemito = pathRemito + numeroComprobanteRemito + ".pdf";

            fe.GrabarEnDisco(comprobante, pathRemito, pathTemplateFc, pathLogo, domicilioTransporteCliente);
        }
    }

    public static void GenerarRemitoSinLogo(WebUser usu, int id, int idPersona,
        string fecha, int idPunto, string numero, string condicionVenta,
        string obs, string vendedor, int idDomicilio, string tipo,
        string fechaEntrega, FETipoComprobante tipoComprobante, string random,
        string nroPedidoDeVenta, string nroPresupuesto)
    {
        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            FEFacturaElectronica fe = new FEFacturaElectronica();
            FEComprobante comprobante = new FEComprobante();
            string numeroComprobanteRemito = string.Empty;

            #region Datos del emisor

            int puntoVenta = 0;
            if (idPunto != 0)
            {
                PuntosDeVenta punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == idPunto && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                puntoVenta = punto.Punto;
                comprobante.PtoVta = puntoVenta;
            }

            var pathLogo = string.Empty;

            var pathRemito = string.Empty;
            var pathTemplateFc = string.Empty;
            switch (tipoComprobante)
            {
                case FETipoComprobante.PRESUPUESTO:
                    comprobante.TipoComprobante = tipoComprobante;
                    comprobante.TipoComprobante = tipoComprobante;
                    pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"] + usu.TemplateFc + ".pdf");
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario + "/");
                    break;
                case FETipoComprobante.REMITO:
                    comprobante.TipoComprobante = tipoComprobante;
                    pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateRemito"]);
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/remitos/" + usu.IDUsuario + "/");
                    break;
            }

            int xInicial = 0;
            int yInicial = 0;
            RemitoComprobanteMargenes l = dbContext.RemitoComprobanteMargenes.Where(w => w.IdUsuario == usu.IDUsuario).FirstOrDefault();
            if (l != null)
            {
                xInicial = l.Horizontal;
                yInicial = l.Vertical;
            }
            else
            {
                RemitoComprobanteMargenes nlpm = new RemitoComprobanteMargenes();
                nlpm.IdUsuario = usu.IDUsuario;
                nlpm.Horizontal = 0;
                nlpm.Vertical = 0;
                nlpm.Fecha = DateTime.Now;
                dbContext.RemitoComprobanteMargenes.Add(nlpm);
                dbContext.SaveChanges();
            }

            comprobante.IDComprobante = id.ToString();
            comprobante.Cuit = long.Parse(usu.CUIT);
            comprobante.Fecha = DateTime.Parse(fecha);

            if (fechaEntrega.Equals(""))
                comprobante.FechaEntrega = comprobante.Fecha;
            else
                comprobante.FechaEntrega = DateTime.Parse(fechaEntrega);

            comprobante.Tipo = tipo;
            comprobante.CodigoMoneda = "PES";
            comprobante.CotizacionMoneda = 1;
            if (numero != string.Empty)
                comprobante.NumeroComprobante = int.Parse(numero);

            comprobante.CondicionVenta = usu.CondicionIVA;
            comprobante.Domicilio = usu.Domicilio;
            comprobante.CiudadProvincia = usu.Ciudad + ", " + usu.Provincia;
            comprobante.RazonSocial = usu.RazonSocial;
            comprobante.Telefono = usu.Telefono;
            comprobante.Celular = usu.Celular;
            if (usu.ExentoIIBB != null)
                comprobante.IIBB = ((bool)usu.ExentoIIBB) ? "Exento" : usu.IIBB;
            comprobante.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usu.CondicionIVA);
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";


            #endregion

            #region Datos del cliente

            /*Int32 nroDNI = 0;
            
            if (Int32.TryParse(persona.NroDocumento, out nroDNI))
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
            else
            {*/
            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }
            //}


            //if (persona.CondicionIva == "CF")
            //    comprobante.DocTipo = 99; // Consumidor final
            //else
            //    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
            //comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);

            comprobante.CondicionVenta = condicionVenta;
            comprobante.ClienteNombre = persona.RazonSocial;
            comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);


            //Me fijo si es en comprobante EDA y chequeo el domicilio de entrega
            if (idDomicilio != 0)
            {
                PersonaDomicilio pd = dbContext.PersonaDomicilio.Where(w => w.IdPersonaDomicilio == idDomicilio).FirstOrDefault();
                comprobante.ClienteDomicilio = pd.Domicilio + " " + pd.PisoDepto;
                comprobante.ClienteLocalidad = pd.Provincia + ", " + pd.Ciudad;
                comprobante.ClienteContacto = "Contacto: " + pd.Contacto + " , Tel: " + pd.Telefono;
            }
            else
            {
                comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
                comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
                comprobante.ClienteContacto = "";
            }
            comprobante.Observaciones = obs;
            comprobante.Vendedor = vendedor;
            comprobante.Original = true;
            #endregion

            #region Seteo los datos de la factura

            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();


            var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();


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
                            Precio = Math.Round(double.Parse(detalle.PrecioUnitarioSinIVA.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        //totalizar importe e IVA
                        switch (detalle.Iva.ToString("N2"))
                        {
                            case "21,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                                break;
                            case "27,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                                break;
                            case "10,50":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                                break;
                            case "5,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                                break;
                            case "2,50":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                                break;
                            case "0,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                                break;
                        }
                    }
                    else
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = Convert.ToDouble(detalle.Cantidad),
                            Descripcion = detalle.Concepto,
                            Precio = double.Parse(detalle.PrecioUnitarioSinIVA.ToString()),
                            //Precio = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        if (comprobante.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                        {
                            comprobante.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalSinIva.ToString()), 2);
                        }
                        else
                        {
                            if (tipo != "FCC" && tipo != "NDC" && tipo != "NCC" && tipo != "RCC")
                                comprobante.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(detalle.TotalSinIva.ToString()), 2), TipoIva = FETipoIva.Iva0 });
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

            comprobante.NroPedidoDeVenta = nroPedidoDeVenta;
            comprobante.NroPresupuesto = nroPresupuesto;

            #endregion


            //if (!string.IsNullOrEmpty(usu.Logo) && conLogo)
            //{
            //    if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
            //        imgLogo = usu.Logo;
            //}

            if (puntoVenta != 0)
                numeroComprobanteRemito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + puntoVenta.ToString("#0000") + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0') + "-" + random;
            else
                numeroComprobanteRemito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0') + "-" + random;

            pathRemito = pathRemito + numeroComprobanteRemito + ".pdf";

            fe.GrabarEnDiscoSinLogo(comprobante, pathRemito, pathTemplateFc, pathLogo, xInicial, yInicial);
        }
    }

    public static void GenerarRemitoTalonario(WebUser usu, int id, int idPersona,
        string fecha, int idPunto, string numero, string condicionVenta,
        string obs, string vendedor, int idDomicilio, string tipo,
        string fechaEntrega, FETipoComprobante tipoComprobante, string random,
        string nroPedidoDeVenta, string nroPresupuesto, bool verTotal, string domicilioTransporteCliente)
    {
        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            FEFacturaElectronica fe = new FEFacturaElectronica();
            FEComprobante comprobante = new FEComprobante();
            string numeroComprobanteRemito = string.Empty;

            #region Datos del emisor

            int puntoVenta = 0;
            if (idPunto != 0)
            {
                PuntosDeVenta punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == idPunto && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                puntoVenta = punto.Punto;
                comprobante.PtoVta = puntoVenta;
            }

            var pathLogo = string.Empty;

            var pathRemito = string.Empty;
            var pathTemplateFc = string.Empty;
            switch (tipoComprobante)
            {
                case FETipoComprobante.PRESUPUESTO:
                    comprobante.TipoComprobante = tipoComprobante;
                    comprobante.TipoComprobante = tipoComprobante;
                    pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"] + usu.TemplateFc + ".pdf");
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario + "/");
                    break;
                case FETipoComprobante.REMITO:
                    comprobante.TipoComprobante = tipoComprobante;
                    pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateRemito"]);
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/remitos/" + usu.IDUsuario + "/");
                    break;
            }

            int xInicial = 0;
            int yInicial = 0;
            RemitoComprobanteMargenes l = dbContext.RemitoComprobanteMargenes.Where(w => w.IdUsuario == usu.IDUsuario).FirstOrDefault();
            if (l != null)
            {
                xInicial = l.Horizontal;
                yInicial = l.Vertical;
            }
            else
            {
                RemitoComprobanteMargenes nlpm = new RemitoComprobanteMargenes();
                nlpm.IdUsuario = usu.IDUsuario;
                nlpm.Horizontal = 0;
                nlpm.Vertical = 0;
                nlpm.Fecha = DateTime.Now;
                dbContext.RemitoComprobanteMargenes.Add(nlpm);
                dbContext.SaveChanges();
            }

            comprobante.IDComprobante = id.ToString();
            comprobante.Cuit = long.Parse(usu.CUIT);
            comprobante.Fecha = DateTime.Parse(fecha);

            if (fechaEntrega.Equals(""))
                comprobante.FechaEntrega = comprobante.Fecha;
            else
                comprobante.FechaEntrega = DateTime.Parse(fechaEntrega);

            comprobante.Tipo = tipo;
            comprobante.CodigoMoneda = "PES";
            comprobante.CotizacionMoneda = 1;
            if (numero != string.Empty)
                comprobante.NumeroComprobante = int.Parse(numero);

            comprobante.CondicionVenta = usu.CondicionIVA;
            comprobante.Domicilio = usu.Domicilio;
            comprobante.CiudadProvincia = usu.Ciudad + ", " + usu.Provincia;
            comprobante.RazonSocial = usu.RazonSocial;
            comprobante.Telefono = usu.Telefono;
            comprobante.Celular = usu.Celular;
            if (usu.ExentoIIBB != null)
                comprobante.IIBB = ((bool)usu.ExentoIIBB) ? "Exento" : usu.IIBB;
            comprobante.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usu.CondicionIVA);
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";


            #endregion

            #region Datos del cliente

            /*Int32 nroDNI = 0;
            
            if (Int32.TryParse(persona.NroDocumento, out nroDNI))
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
            else
            {*/
            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }
            //}


            //if (persona.CondicionIva == "CF")
            //    comprobante.DocTipo = 99; // Consumidor final
            //else
            //    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
            //comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);

            comprobante.CondicionVenta = condicionVenta;
            comprobante.ClienteNombre = persona.RazonSocial;
            comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);


            //Me fijo si es en comprobante EDA y chequeo el domicilio de entrega
            if (idDomicilio != 0)
            {
                PersonaDomicilio pd = dbContext.PersonaDomicilio.Where(w => w.IdPersonaDomicilio == idDomicilio).FirstOrDefault();
                comprobante.ClienteDomicilio = pd.Domicilio + " " + pd.PisoDepto;
                comprobante.ClienteLocalidad = pd.Provincia + ", " + pd.Ciudad;
                comprobante.ClienteContacto = "Contacto: " + pd.Contacto + " , Tel: " + pd.Telefono;
            }
            else
            {
                comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
                comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
                comprobante.ClienteContacto = "";
            }
            comprobante.Observaciones = obs;
            comprobante.Vendedor = vendedor;
            comprobante.Original = true;
            #endregion

            #region Seteo los datos de la factura

            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();


            var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();


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
                            Precio = Math.Round(double.Parse(detalle.PrecioUnitarioSinIVA.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        //totalizar importe e IVA
                        switch (detalle.Iva.ToString("N2"))
                        {
                            case "21,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                                break;
                            case "27,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                                break;
                            case "10,50":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                                break;
                            case "5,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                                break;
                            case "2,50":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                                break;
                            case "0,00":
                                ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                                break;
                        }
                    }
                    else
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = Convert.ToDouble(detalle.Cantidad),
                            Descripcion = detalle.Concepto,
                            Precio = double.Parse(detalle.PrecioUnitarioConIva.ToString()),
                            //Precio = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2),
                            Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                            Bonificacion = detalle.Bonificacion,
                            IdTipoIVA = detalle.IdTipoIva
                        });

                        if (comprobante.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                        {
                            comprobante.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2);
                        }
                        else
                        {
                            if (tipo != "FCC" && tipo != "NDC" && tipo != "NCC" && tipo != "RCC")
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

            comprobante.NroPedidoDeVenta = nroPedidoDeVenta;
            comprobante.NroPresupuesto = nroPresupuesto;

            #endregion


            //if (!string.IsNullOrEmpty(usu.Logo) && conLogo)
            //{
            //    if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
            //        imgLogo = usu.Logo;
            //}

            if (puntoVenta != 0)
                numeroComprobanteRemito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + puntoVenta.ToString("#0000") + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0') + "-" + random;
            else
                numeroComprobanteRemito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0') + "-" + random;

            pathRemito = pathRemito + numeroComprobanteRemito + ".pdf";

            fe.GrabarEnDiscoTalonario(comprobante, pathRemito, pathTemplateFc, pathLogo, xInicial, yInicial, verTotal, domicilioTransporteCliente);
        }
    }


    public static void GenerarLiquidoProducto(WebUser usu, List<Comprobantes> comp, string nombreArchivo, string tipoImpresion)
    {
        using (var dbContext = new ACHEEntities())
        {
            int xInicial = 0;
            int yInicial = 0;
            LiquidoProductoMargenes l = dbContext.LiquidoProductoMargenes.Where(w => w.IdUsuario == usu.IDUsuario).FirstOrDefault();
            if(l != null)
            {
                xInicial = l.Horizontal;
                yInicial = l.Vertical;
            }
            else
            {
                LiquidoProductoMargenes nlpm = new LiquidoProductoMargenes();
                nlpm.IdUsuario = usu.IDUsuario;
                nlpm.Horizontal = 0;
                nlpm.Vertical = 0;
                nlpm.Fecha = DateTime.Now;
                dbContext.LiquidoProductoMargenes.Add(nlpm);
                dbContext.SaveChanges();
            }

            List<FEComprobante> listaComprobantes = new List<FEComprobante>();            
            string pathLiquidoProducto = HttpContext.Current.Server.MapPath("~/files/liquidoProducto/" + usu.IDUsuario);            

            foreach (Comprobantes c in comp)
            {
                Personas persona = dbContext.Personas.Where(x => x.IDPersona == c.IDPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                FEComprobante comprobante = new FEComprobante();

                string numeroComprobanteLiquidoProducto = string.Empty;

                #region Datos del emisor

                int puntoVenta = 0;
                if (c.IDPuntoVenta != 0)
                {
                    PuntosDeVenta punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == c.IDPuntoVenta && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    puntoVenta = punto.Punto;
                    comprobante.PtoVta = puntoVenta;
                }           
   
                comprobante.Cuit = long.Parse(usu.CUIT);
                comprobante.Fecha = (DateTime)c.FechaEntrega;

                comprobante.CodigoMoneda = "PES";
                comprobante.CotizacionMoneda = 1;
                comprobante.NumeroComprobante = c.Numero;

                comprobante.CondicionVenta = usu.CondicionIVA;
                comprobante.Domicilio = usu.Domicilio;
                comprobante.CiudadProvincia = usu.Ciudad + ", " + usu.Provincia;
                comprobante.RazonSocial = usu.RazonSocial;
                comprobante.Telefono = usu.Telefono;
                comprobante.Celular = usu.Celular;
                if (usu.ExentoIIBB != null)
                    comprobante.IIBB = ((bool)usu.ExentoIIBB) ? "Exento" : usu.IIBB;
                comprobante.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);
                comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";

                #endregion

                #region Datos del cliente

                /*Int32 nroDNI = 0;

                if (Int32.TryParse(persona.NroDocumento, out nroDNI))
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                else
                {*/
                if (persona.CondicionIva == "CF")
                {
                    comprobante.DocTipo = 99; // Consumidor final
                    if (persona.NroDocumento != string.Empty)
                    {
                        comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                        comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                    }
                }
                else
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
                //}


                //if (persona.CondicionIva == "CF")
                //    comprobante.DocTipo = 99; // Consumidor final
                //else
                //    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                //comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);

                comprobante.CondicionVenta = c.CondicionVenta;
                comprobante.ClienteNombre = persona.RazonSocial;
                comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);
                comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
                comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
                comprobante.Observaciones = c.Observaciones;
                comprobante.Original = true;
                #endregion

                #region Seteo los datos de la factura

                comprobante.DetalleIva = new List<FERegistroIVA>();
                comprobante.Tributos = new List<FERegistroTributo>();

                ComprobanteCart.Retrieve().Items.Clear();

                var listDet = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == c.IDComprobante).OrderBy(x => x.Concepto).ToList();

                foreach(ComprobantesDetalle cd in listDet)
                {
                    var tra = new ComprobantesDetalleViewModel();
                    tra.ID = cd.IDDetalle;
                    tra.Concepto = cd.Concepto;
                    tra.Codigo = cd.Concepto;
                    tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                    tra.Iva = cd.Iva;
                    tra.Bonificacion = cd.Bonificacion;
                    tra.PrecioUnitario = cd.PrecioUnitario;
                    tra.Cantidad = cd.Cantidad;
                    tra.IDConcepto = cd.IDConcepto;
                    tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;

                    //tra.PrecioUnitario = ObtenerPrecioFinal(tra.PrecioUnitario, cd.Iva.ToString(), c.IDPersona);
                    tra.PrecioUnitario = tra.PrecioUnitario;

                    ComprobanteCart.Retrieve().Items.Add(tra);

                }

                var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();

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
                                Precio = Math.Round(double.Parse(detalle.PrecioUnitarioSinIVA.ToString()), 2),
                                Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                                Bonificacion = detalle.Bonificacion,
                                IdTipoIVA = detalle.IdTipoIva
                            });

                            //totalizar importe e IVA
                            switch (detalle.Iva.ToString("N2"))
                            {
                                case "21,00":
                                    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva21);
                                    break;
                                case "27,00":
                                    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva27);
                                    break;
                                case "10,50":
                                    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva10_5);
                                    break;
                                case "5,00":
                                    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva5);
                                    break;
                                case "2,50":
                                    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva2_5);
                                    break;
                                case "0,00":
                                    ComprobantesCommon.AgregarDetalleIvaTotalizado(ref comprobante, detalle, FETipoIva.Iva0);
                                    break;
                            }
                        }
                        else
                        {
                            comprobante.ItemsDetalle.Add(new FEItemDetalle()
                            {
                                Cantidad = Convert.ToDouble(detalle.Cantidad),
                                Descripcion = detalle.Concepto,
                                Precio = double.Parse(detalle.PrecioUnitarioConIva.ToString()),
                                //Precio = Math.Round(double.Parse(detalle.PrecioUnitarioConIva.ToString()), 2),
                                Codigo = detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "",
                                Bonificacion = detalle.Bonificacion,
                                IdTipoIVA = detalle.IdTipoIva
                            });

                            if (comprobante.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                            {
                                comprobante.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.TotalConIva.ToString()), 2);
                            }
                            else
                            {
                                if (c.Tipo != "FCC" && c.Tipo != "NDC" && c.Tipo != "NCC" && c.Tipo != "RCC")
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

                comprobante.ImpTotConc = (double)ComprobanteCart.Retrieve().GetImporteNoGravado();
                if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIIBB)
                {
                    if ((double)ComprobanteCart.Retrieve().GetPercepcionIIBB() > 0)
                    {
                        comprobante.Tributos.Add(new FERegistroTributo()
                        {
                            BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                            Alicuota = (double)ComprobanteCart.Retrieve().PercepcionIIBB,
                            Importe = Math.Round((double)ComprobanteCart.Retrieve().GetPercepcionIIBB(), 2),
                            Decripcion = "Percepción IIBB",
                            Tipo = FETipoTributo.ImpuestosNacionales
                        });
                    }
                }

                if (usu.CondicionIVA == "RI" && (bool)usu.AgentePercepcionIVA)
                {
                    if ((double)ComprobanteCart.Retrieve().GetPercepcionIVA() > 0)
                    {
                        comprobante.Tributos.Add(new FERegistroTributo()
                        {
                            BaseImp = comprobante.ItemsDetalle.Sum(x => x.Total),
                            Alicuota = (double)ComprobanteCart.Retrieve().PercepcionIVA,
                            Importe = Math.Round((double)ComprobanteCart.Retrieve().GetPercepcionIVA(), 2),
                            Decripcion = "Percepción IVA",
                            Tipo = FETipoTributo.ImpuestosNacionales
                        });
                    }
                }   

                listaComprobantes.Add(comprobante);               
            }            

            FEFacturaElectronica fe = new FEFacturaElectronica();
            fe.GrabarEnDiscoLiquidoProducto(listaComprobantes, pathLiquidoProducto, nombreArchivo, tipoImpresion, xInicial, yInicial);
        }
    }

    public static void GenerarDetalleCuentaCorriente(WebUser usu, IList<RptCcDetalleViewModel> comp, string razonSocial, int verComo, string nombreArchivo)
    {
        using (var dbContext = new ACHEEntities())
        {
            string pathDetalleCuentaCorriente = HttpContext.Current.Server.MapPath("~/files/detalleCuentaCorriente/" + usu.IDUsuario);           

            FEFacturaElectronica fe = new FEFacturaElectronica();
            GrabarEnDiscoDetalleCuentaCorriente(comp, razonSocial,verComo, pathDetalleCuentaCorriente, nombreArchivo);
        }
    }
    public static void GrabarEnDiscoDetalleCuentaCorriente(IList<RptCcDetalleViewModel> detalle, string razonSocial, int verComo, string pathArchivoDestino, string nombreArchivoDestino)
    {
        GrabarEnDiscoDetalleCuentaCorriente(detalle, razonSocial, verComo, pathArchivoDestino, nombreArchivoDestino, null);
    }
    public static void GrabarEnDiscoDetalleCuentaCorriente(IList<RptCcDetalleViewModel> detalle, string razonSocial, int verComo, string pathArchivoDestino, string nombreArchivoDestino, string pathLogo)
    {
        Stream streamPDF = GetStreamPDFDetalleCuentaCorriente(detalle, razonSocial,verComo, pathLogo);

        if (!Directory.Exists(pathArchivoDestino))
        {
            Directory.CreateDirectory(pathArchivoDestino);
        }

        using (Stream destination = File.Create(pathArchivoDestino + @"\" + nombreArchivoDestino))
        {
            for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                destination.WriteByte((byte)a);
        }
    }


    public static void GenerarCaja(WebUser usu, ResultadosCajaViewModel dt, string nombreArchivo)
    {
        using (var dbContext = new ACHEEntities())
        {
            string pathCaja = HttpContext.Current.Server.MapPath("~/files/caja/" + usu.IDUsuario);

            FEFacturaElectronica fe = new FEFacturaElectronica();
            GrabarEnDiscoCaja(dt, pathCaja, nombreArchivo);
        }
    }
    public static void GrabarEnDiscoCaja(ResultadosCajaViewModel dt, string pathArchivoDestino, string nombreArchivoDestino)
    {
        GrabarEnDiscoCaja(dt, pathArchivoDestino, nombreArchivoDestino, null);
    }
    public static void GrabarEnDiscoCaja(ResultadosCajaViewModel dt, string pathArchivoDestino, string nombreArchivoDestino, string pathLogo)
    {
        Stream streamPDF = GetStreamPDFCaja(dt, pathLogo);

        if (!Directory.Exists(pathArchivoDestino))
        {
            Directory.CreateDirectory(pathArchivoDestino);
        }

        using (Stream destination = File.Create(pathArchivoDestino + @"\" + nombreArchivoDestino))
        {
            for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                destination.WriteByte((byte)a);
        }
    }

    public static Stream GetStreamPDFCaja(ResultadosCajaViewModel dt, string pathLogo)
    {

        //var template = (templateFc == string.Empty ? ConfigurationManager.AppSettings["FE.Template"] : templateFc);        
             

        List<CajaViewModel> lc = new List<CajaViewModel>();

        foreach (ConceptosMeses m in dt.Items)
        {
            lc.AddRange(m.Conceptos);
        }

        int resultadoHojas = (int)Math.Ceiling(((decimal)lc.Count) / 30);

        string[] pdfs = new string[resultadoHojas];
        int contadorHojas = 0;
        string pathTemplateCaja = null;

        for (int i = 1; i <= resultadoHojas; i++)
        {
            int y = 50;
            NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, pathTemplateCaja);

            pdf.EscribirXY("Detalle Caja", 30, y, 20, Alineado.Izquierda);
            y += 30;

            List<CajaViewModel> li = lc.Take(30).ToList();

            pdf.EscribirXY("Fecha", 30, y, 7, Alineado.Izquierda);
            pdf.EscribirXY("Concepto", 90, y, 7, Alineado.Izquierda);
            pdf.EscribirXY("Medio de Pago", 320, y, 7, Alineado.Izquierda);
            pdf.EscribirXY("Estado", 390, y, 7, Alineado.Izquierda);
            //pdf.EscribirXY("Observaciones", 380, y, 7, Alineado.Izquierda);
            pdf.EscribirXY("Debe", 440, y, 7, Alineado.Izquierda);
            pdf.EscribirXY("Haber", 480, y, 7, Alineado.Izquierda);
            pdf.EscribirXY("Saldo", 530, y, 7, Alineado.Izquierda);

            y += 10;
            pdf.EscribirXY("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------", 30, y, 9, Alineado.Izquierda);

            y += 20;
            foreach (CajaViewModel item in li)
            {
                pdf.EscribirXY(item.Fecha, 30, y, 7, Alineado.Izquierda);
                pdf.EscribirXY(item.Concepto, 90, y, 7, Alineado.Izquierda);
                pdf.EscribirXY(item.MedioDePago, 320, y, 7, Alineado.Izquierda);
                pdf.EscribirXY(item.Estado, 390, y, 7, Alineado.Izquierda);
                //pdf.EscribirXY(item.Observaciones, 380, y, 7, Alineado.Izquierda);
                pdf.EscribirXY(item.Ingreso, 440, y, 7, Alineado.Izquierda);
                pdf.EscribirXY(item.Egreso, 480, y, 7, Alineado.Izquierda);
                pdf.EscribirXY(item.Saldo, 530, y, 7, Alineado.Izquierda);
                lc.Remove(item);
                y += 20;
            }
            
            pdf.FileName = "P" + contadorHojas.ToString() + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
            pdf.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/caja"));
            pdfs[contadorHojas] = HttpContext.Current.Server.MapPath("~/files/caja/") + pdf.FileName;
            contadorHojas++;
        }

        string destino = HttpContext.Current.Server.MapPath("~/files/caja/") + (new Random()).Next(0, Int32.MaxValue).ToString() + "_Document.pdf";

        Merge(destino, pdfs);

        for (int i = 0; i < resultadoHojas; i++)
        {
            if (File.Exists(pdfs[i]))
                File.Delete(pdfs[i]);
        }

        var pdfContent = new MemoryStream(System.IO.File.ReadAllBytes(destino));
        pdfContent.Position = 0;
        if (File.Exists(destino))
            File.Delete(destino);

        return pdfContent;

    }

    public static Stream GetStreamPDFDetalleCuentaCorriente(IList<RptCcDetalleViewModel> detalle, string razonSocial, int verComo, string pathLogo)
    {

        //var template = (templateFc == string.Empty ? ConfigurationManager.AppSettings["FE.Template"] : templateFc);        

        int resultadoHojas = (int)Math.Ceiling(((decimal)detalle.Count()) / 30);

        string[] pdfs = new string[resultadoHojas];
        int contadorHojas = 0;
        string pathTemplateDetalleCuentaCorriente = null;

        for (int i = 1; i <= resultadoHojas; i++)
        {
            int y = 50;
            NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, pathTemplateDetalleCuentaCorriente);

            pdf.EscribirXY("Detalle Cuenta Corriente", 30, y, 20, Alineado.Izquierda);
            y += 30;
            pdf.EscribirXY("Razón Social: " + razonSocial, 30, y, 18, Alineado.Izquierda);
            y += 30;
            if (verComo == 1)
                pdf.EscribirXY("Como Cliente", 30, y, 18, Alineado.Izquierda);
            else
                pdf.EscribirXY("Como Proveedor", 30, y, 18, Alineado.Izquierda);

            List<RptCcDetalleViewModel> li = detalle.Take(30).ToList();

            y += 30;
            pdf.EscribirXY("PDC", 30, y, 10, Alineado.Izquierda);
            pdf.EscribirXY("Fecha", 100, y, 10, Alineado.Izquierda);
            pdf.EscribirXY("Comprobante Aplicado", 170, y, 10, Alineado.Izquierda);
            pdf.EscribirXY("Fecha Cobro", 290, y, 10, Alineado.Izquierda);
            pdf.EscribirXY("Importe", 360, y, 10, Alineado.Izquierda);
            pdf.EscribirXY("Cobrado", 430, y, 10, Alineado.Izquierda);
            pdf.EscribirXY("Total", 490, y, 10, Alineado.Izquierda);

            y += 10;
            pdf.EscribirXY("------------------------------------------------------------------------------------------------------------------------------------------------------------", 30, y, 10, Alineado.Izquierda);

            y += 20;
            foreach (RptCcDetalleViewModel item in li)
            {
                pdf.EscribirXY(item.PDC, 30, y, 10, Alineado.Izquierda);
                pdf.EscribirXY(item.Fecha, 100, y, 10, Alineado.Izquierda);
                pdf.EscribirXY(item.ComprobanteAplicado, 170, y, 10, Alineado.Izquierda);
                pdf.EscribirXY(item.FechaCobro, 290, y, 10, Alineado.Izquierda);
                pdf.EscribirXY(item.Importe, 360, y, 10, Alineado.Izquierda);
                pdf.EscribirXY(item.Cobrado, 430, y, 10, Alineado.Izquierda);
                pdf.EscribirXY(item.Total, 490, y, 10, Alineado.Izquierda);
                detalle.Remove(item);
                y += 20;
            }

            pdf.FileName = "P" + contadorHojas.ToString() + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
            pdf.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/detalleCuentaCorriente"));
            pdfs[contadorHojas] = HttpContext.Current.Server.MapPath("~/files/detalleCuentaCorriente/") + pdf.FileName;
            contadorHojas++;
        }        

        string destino = HttpContext.Current.Server.MapPath("~/files/detalleCuentaCorriente/") + (new Random()).Next(0, Int32.MaxValue).ToString() + "_Document.pdf";

        Merge(destino, pdfs);

        for (int i = 0; i < resultadoHojas; i++)
        {
            if (File.Exists(pdfs[i]))
                File.Delete(pdfs[i]);
        }

        var pdfContent = new MemoryStream(System.IO.File.ReadAllBytes(destino));
        pdfContent.Position = 0;
        if (File.Exists(destino))
            File.Delete(destino);

        return pdfContent;

    }

    internal static bool Merge(string strFileTarget, string[] arrStrFilesSource)
    {
        bool blnMerged = false;

        // Crea el PDF de salida
        try
        {
            using (System.IO.FileStream stmFile = new System.IO.FileStream(strFileTarget, System.IO.FileMode.Create))
            {
                Document objDocument = null;
                PdfWriter objWriter = null;

                // Recorre los archivos
                for (int intIndexFile = 0; intIndexFile < arrStrFilesSource.Length; intIndexFile++)
                {
                    PdfReader objReader = new PdfReader(arrStrFilesSource[intIndexFile]);
                    int intNumberOfPages = objReader.NumberOfPages;

                    // La primera vez, inicializa el documento y el escritor
                    if (intIndexFile == 0)
                    { // Asigna el documento y el generador
                        objDocument = new Document(objReader.GetPageSizeWithRotation(1));
                        objWriter = PdfWriter.GetInstance(objDocument, stmFile);
                        // Abre el documento
                        objDocument.Open();
                    }
                    // Añade las páginas
                    for (int intPage = 0; intPage < intNumberOfPages; intPage++)
                    {
                        int intRotation = objReader.GetPageRotation(intPage + 1);
                        PdfImportedPage objPage = objWriter.GetImportedPage(objReader, intPage + 1);

                        // Asigna el tamaño de la página
                        objDocument.SetPageSize(objReader.GetPageSizeWithRotation(intPage + 1));
                        // Crea una nueva página
                        objDocument.NewPage();
                        // Añade la página leída
                        if (intRotation == 90 || intRotation == 270)
                            objWriter.DirectContent.AddTemplate(objPage, 0, -1f, 1f, 0, 0,
                                                                objReader.GetPageSizeWithRotation(intPage + 1).Height);
                        else
                            objWriter.DirectContent.AddTemplate(objPage, 1f, 0, 0, 1f, 0, 0);
                    }
                }
                // Cierra el documento
                if (objDocument != null)
                    objDocument.Close();
                // Cierra el stream del archivo
                stmFile.Close();
            }
            // Indica que se ha creado el documento
            blnMerged = true;
        }
        catch (Exception objException)
        {
            System.Diagnostics.Debug.WriteLine(objException.Message);
        }
        // Devuelve el valor que indica si se han mezclado los archivos
        return blnMerged;
    }

    private static decimal ObtenerPrecioFinal(decimal PrecioUnitario, string iva, int idPersona)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            //var UsaPrecioConIva = dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && (x.CondicionIva == "MO" || x.CondicionIva == "CF"));
            //if (usu.UsaPrecioFinalConIVA || UsaPrecioConIva)
            if (usu.UsaPrecioFinalConIVA)
                return ConceptosCommon.ObtenerPrecioFinal(PrecioUnitario, iva);
            else
                return PrecioUnitario;
        }
    }

    /// <summary>
    /// Solo se usa para recibos de cobranza X
    /// </summary>
    /// <param name="usu"></param>
    /// <param name="id"></param>
    /// <param name="idPersona"></param>
    /// <param name="tipo"></param>
    /// <param name="modo"></param>
    /// <param name="fecha"></param>
    /// <param name="idPuntoVenta"></param>
    /// <param name="nroComprobante"></param>
    /// <param name="obs"></param>
    /// <param name="accion"></param>
    public static void CrearCobranza(WebUser usu, int id, int idPersona, string tipo, string fecha, ref string nroComprobante, string obs, ComprobanteModo accion)
    {
        var pathPdf = "";

        if (accion == ComprobanteModo.Previsualizar)
        {
            pathPdf = HttpContext.Current.Server.MapPath("~/files/comprobantes/" + usu.IDUsuario + "_prev.pdf");
            if (System.IO.File.Exists(pathPdf))
                System.IO.File.Delete(pathPdf);
        }
        else
            pathPdf = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/");

        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (persona == null)
                throw new Exception("El cliente/proveedor es inexistente");

            if (ComprobanteModo.Generar == accion)
                pathPdf = pathPdf + persona.RazonSocial.RemoverCaracteresParaPDF() + "_";

            FEFacturaElectronica fe = new FEFacturaElectronica();
            FEComprobante comprobante = new FEComprobante();

            #region Datos del emisor
            comprobante.TipoComprobante = FETipoComprobante.COBRANZA;

            var feMode = ConfigurationManager.AppSettings["FE.Modo"];

            if (feMode == "QA")
            {
                //Cambio de CUIT hardcoded
                comprobante.Cuit = 23185938999;//long.Parse(usu.CUIT);
                comprobante.PtoVta = 5;// puntoVenta;
            }
            else
            {
                comprobante.Cuit = long.Parse(usu.CUIT);
                comprobante.PtoVta = 1;//puntoVenta;
            }
            comprobante.Concepto = FEConcepto.ProductoYServicio;
            comprobante.Fecha = DateTime.Parse(fecha);

            comprobante.FchServDesde = null;
            comprobante.FchServHasta = null;
            comprobante.CodigoMoneda = "PES";
            comprobante.CotizacionMoneda = 1;
            if (nroComprobante != string.Empty && nroComprobante != "-")
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
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";

            #endregion

            #region Datos del cliente

            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }

            comprobante.CondicionVenta = "";
            comprobante.ClienteNombre = persona.RazonSocial;
            comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);
            comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
            comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
            comprobante.Observaciones = obs;
            comprobante.Original = true;//accion == ComprobanteModo.Previsualizar ? false : 

            #endregion

            #region Seteo los datos de la factura

            comprobante.ImpTotConc = 0;
            comprobante.ImpOpEx = 0;
            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();

            var list = CobranzaCart.Retrieve().Items.OrderBy(x => x.Comprobante).ToList();
            var retenciones = CobranzaCart.Retrieve().Retenciones.OrderBy(x => x.Tipo).ToList();
            var formasCobro = CobranzaCart.Retrieve().FormasDePago.OrderBy(x => x.ID).ToList();
            if (list.Any() || retenciones.Any())
            {
                if(comprobante.TipoComprobante == FETipoComprobante.COBRANZA)
                {
                    comprobante.ConceptoRecibo = "Items: |";

                    foreach (var detalle in list)
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = 1,
                            Descripcion = detalle.Comprobante,
                            Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2),
                            Fecha = detalle.Fecha
                        });
                        comprobante.ConceptoRecibo += detalle.Comprobante + ": $" + detalle.Importe.ToString("N2");
                        comprobante.ConceptoRecibo += "|";
                    }

                    if (retenciones.Any())
                        comprobante.ConceptoRecibo += "|Retenciones: ||";
                    foreach (var detalle in retenciones)
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = 1,
                            Descripcion = "Retención " + detalle.Tipo + " Nro. " + detalle.NroReferencia,
                            Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2),
                            Fecha = detalle.Fecha
                        });
                        comprobante.ConceptoRecibo += detalle.Tipo + " Nro. " + detalle.NroReferencia + ": $" + detalle.Importe.ToString("N2");
                        comprobante.ConceptoRecibo += "|";
                    }

                    if (formasCobro.Any())
                        comprobante.ConceptoRecibo += "|Formas de cobro: ||";
                    foreach (var detalle in formasCobro)
                    {
                        comprobante.ItemsFormasDePago.Add(new FEItemFormasDePago()
                        {
                            Cantidad = 1,
                            Descripcion = detalle.FormaDePago + (detalle.NroReferencia != "" ? " / Nro: " + detalle.NroReferencia : ""),
                            Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2),
                            Fecha = detalle.Fecha
                        });
                        comprobante.ConceptoRecibo += detalle.FormaDePago + " Nro. " + detalle.NroReferencia + ": $" + detalle.Importe.ToString("N2");
                        comprobante.ConceptoRecibo += "|";
                    }
                }
                else
                {
                    comprobante.ConceptoRecibo = "El recibo contempla los siguientes items: |";

                    foreach (var detalle in list)
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = 1,
                            Descripcion = detalle.Comprobante,
                            Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2)
                        });
                        comprobante.ConceptoRecibo += detalle.Comprobante + ": $" + detalle.Importe.ToString("N2");
                        comprobante.ConceptoRecibo += "|";
                    }

                    if (retenciones.Any())
                        comprobante.ConceptoRecibo += "|El recibo contempla las siguientes retenciones: ||";
                    foreach (var detalle in retenciones)
                    {
                        comprobante.ItemsDetalle.Add(new FEItemDetalle()
                        {
                            Cantidad = 1,
                            Descripcion = "Retención " + detalle.Tipo + " Nro. " + detalle.NroReferencia,
                            Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2)
                        });
                        comprobante.ConceptoRecibo += "Retención " + detalle.Tipo + " Nro. " + detalle.NroReferencia + ": $" + detalle.Importe.ToString("N2");
                        comprobante.ConceptoRecibo += "|";
                    }
                    if (formasCobro.Any())
                        comprobante.ConceptoRecibo += "|El recibo contempla las siguientes cobranzas: ||";
                    foreach (var detalle in formasCobro)
                    {
                        comprobante.ConceptoRecibo += "Forma de cobro " + detalle.FormaDePago + " Nro. " + detalle.NroReferencia + ": $" + detalle.Importe.ToString("N2");
                        comprobante.ConceptoRecibo += "|";
                    }
                }
            }

            #endregion

            var pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateRecibo"]);
            var imgLogo = "logo-fc-default.png";
            if (!string.IsNullOrEmpty(usu.Logo))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                    imgLogo = usu.Logo;
            }
            var pathLogo = HttpContext.Current.Server.MapPath("~/files/usuarios/" + imgLogo);
            string numeroComprobante = nroComprobante.ToString().PadLeft(8, '0');

            if (accion != ComprobanteModo.Previsualizar)
                pathPdf = pathPdf + tipo + "-" + numeroComprobante + ".pdf";

            fe.GrabarEnDisco(comprobante, pathPdf, pathTemplateFc, pathLogo);
            nroComprobante = tipo + "-" + numeroComprobante;
        }
    }

    /// <summary>
    /// Solo se usa para recibos de pagos X
    /// </summary>
    /// <param name="usu"></param>
    /// <param name="id"></param>
    /// <param name="idPersona"></param>
    /// <param name="tipo"></param>
    /// <param name="modo"></param>
    /// <param name="fecha"></param>
    /// <param name="idPuntoVenta"></param>
    /// <param name="nroComprobante"></param>
    /// <param name="obs"></param>
    /// <param name="accion"></param>
    public static void CrearPago(WebUser usu, int id, int idPersona, string tipo, ref string nroComprobante, string fecha, string obs, ComprobanteModo accion)
    {
        var pathPdf = HttpContext.Current.Server.MapPath("~/files/pagos/" + usu.IDUsuario + "/");
        if (!Directory.Exists(pathPdf))
            Directory.CreateDirectory(pathPdf);

        if (accion == ComprobanteModo.Previsualizar)
        {
            pathPdf = pathPdf + nroComprobante + "_prev.pdf";
            if (System.IO.File.Exists(pathPdf))
                System.IO.File.Delete(pathPdf);
        }            

        using (var dbContext = new ACHEEntities())
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (persona == null)
                throw new Exception("El cliente/proveedor es inexistente");

            FEFacturaElectronica fe = new FEFacturaElectronica();
            FEComprobante comprobante = new FEComprobante();

            #region Datos del emisor
            comprobante.TipoComprobante = FETipoComprobante.SIN_DEFINIR_RECIBO;

            var feMode = ConfigurationManager.AppSettings["FE.Modo"];

            if (feMode == "QA")
            {
                //Cambio de CUIT hardcoded
                comprobante.Cuit = 23185938999;//long.Parse(usu.CUIT);
                comprobante.PtoVta = 5;// puntoVenta;
            }
            else
            {
                comprobante.Cuit = long.Parse(usu.CUIT);
                comprobante.PtoVta = 1;//puntoVenta;
            }
            comprobante.Concepto = FEConcepto.ProductoYServicio;
            comprobante.Fecha = DateTime.Parse(fecha);

            comprobante.FchServDesde = null;
            comprobante.FchServHasta = null;
            comprobante.CodigoMoneda = "PES";
            comprobante.CotizacionMoneda = 1;
            if (nroComprobante != string.Empty && nroComprobante != "-")
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
            comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";

            #endregion

            #region Datos del cliente

            if (persona.CondicionIva == "CF")
            {
                comprobante.DocTipo = 99; // Consumidor final
                if (persona.NroDocumento != string.Empty)
                {
                    comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
                }
            }
            else
            {
                comprobante.DocTipo = persona.TipoDocumento == "DNI" ? 96 : 80;
                comprobante.DocNro = (persona.NroDocumento == "") ? 0 : long.Parse(persona.NroDocumento);
            }

            comprobante.CondicionVenta = "";
            comprobante.ClienteNombre = persona.RazonSocial;
            comprobante.ClienteCondiionIva = UsuarioCommon.GetCondicionIvaDesc(persona.CondicionIva);
            comprobante.ClienteDomicilio = persona.Domicilio + " " + persona.PisoDepto;
            comprobante.ClienteLocalidad = persona.Ciudades.Nombre + ", " + persona.Provincias.Nombre;
            comprobante.Observaciones = obs;
            comprobante.Original = true;//accion == ComprobanteModo.Previsualizar ? false : 

            #endregion

            #region Seteo los datos de la factura

            comprobante.ImpTotConc = 0;
            comprobante.ImpOpEx = 0;
            comprobante.DetalleIva = new List<FERegistroIVA>();
            comprobante.Tributos = new List<FERegistroTributo>();

            var list = PagosCart.Retrieve().Items.ToList();
            var retenciones = PagosCart.Retrieve().Retenciones.OrderBy(x => x.Tipo).ToList();
            var formasCobro = PagosCart.Retrieve().FormasDePago.OrderBy(x => x.ID).ToList();
            if (list.Any() || retenciones.Any())
            {
                comprobante.ConceptoRecibo = "El recibo contempla los siguientes items: |";

                foreach (var detalle in list)
                {
                    comprobante.ItemsDetalle.Add(new FEItemDetalle()
                    {
                        Cantidad = 1,
                        Descripcion = detalle.nroFactura,
                        Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2)
                    });
                    comprobante.ConceptoRecibo += detalle.nroFactura + ": $" + detalle.Importe.ToString("N2");
                    comprobante.ConceptoRecibo += "|";
                }

                if (retenciones.Any())
                    comprobante.ConceptoRecibo += "|El recibo contempla las siguientes retenciones: ||";
                foreach (var detalle in retenciones)
                {
                    comprobante.ItemsDetalle.Add(new FEItemDetalle()
                    {
                        Cantidad = 1,
                        Descripcion = "Retención " + detalle.Tipo + " Nro. " + detalle.NroReferencia,
                        Precio = Math.Round(double.Parse(detalle.Importe.ToString()), 2)
                    });
                    comprobante.ConceptoRecibo += "Retención " + detalle.Tipo + " Nro. " + detalle.NroReferencia + ": $" + detalle.Importe.ToString("N2");
                    comprobante.ConceptoRecibo += "|";
                }
                if (formasCobro.Any())
                    comprobante.ConceptoRecibo += "|El recibo contempla las siguientes cobranzas: ||";
                foreach (var detalle in formasCobro)
                {
                    comprobante.ConceptoRecibo += "Forma de cobro " + detalle.FormaDePago + " Nro. " + detalle.NroReferencia + ": $" + detalle.Importe.ToString("N2");
                    comprobante.ConceptoRecibo += "|";
                }
            }

            #endregion

            var pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateRecibo"]);
            var imgLogo = "logo-fc-default.png";
            if (!string.IsNullOrEmpty(usu.Logo))
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/files/usuarios/" + usu.Logo)))
                    imgLogo = usu.Logo;
            }
            var pathLogo = HttpContext.Current.Server.MapPath("~/files/usuarios/" + imgLogo);
            string numero = nroComprobante.ToString().PadLeft(8, '0');

            if (accion != ComprobanteModo.Previsualizar)
                pathPdf = pathPdf + nroComprobante + ".pdf";

            GrabarEnDiscoPago(comprobante, pathPdf, pathTemplateFc, pathLogo);
        }
    }

    public static void GrabarEnDiscoPago(FEComprobante comprobante, string archivoDestino, string templateFc)
    {
        GrabarEnDiscoPago(comprobante, archivoDestino, templateFc, null);
    }

    public static void GrabarEnDiscoPago(FEComprobante comprobante, string archivoDestino, string templateFc, string pathLogo)
    {
        Stream streamPDF = GetStreamPDFPago(comprobante, templateFc, pathLogo);

        using (Stream destination = File.Create(archivoDestino))
        {
            for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                destination.WriteByte((byte)a);
        }
    }

    public static Stream GetStreamPDFPago(FEComprobante comprobante, string templateFc, string pathLogo)
    {
        string letra = "";
        string comp = "";
        string codigo = "";
        bool esRecibo = comprobante.TipoComprobante == FETipoComprobante.RECIBO_A || comprobante.TipoComprobante == FETipoComprobante.RECIBO_B || comprobante.TipoComprobante == FETipoComprobante.RECIBO_C || comprobante.TipoComprobante == FETipoComprobante.SIN_DEFINIR_RECIBO;
        bool esRemito = comprobante.TipoComprobante == FETipoComprobante.REMITO;

        var template = (templateFc == string.Empty ? ConfigurationManager.AppSettings["FE.Template"] : templateFc);
        NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, template);

        if ((int)comprobante.TipoComprobante < 0)
            codigo = "";
        else if ((int)comprobante.TipoComprobante == 999)
            codigo = "P";
        else
            codigo = ((int)comprobante.TipoComprobante).ToString().PadLeft(2, '0');

        switch (comprobante.TipoComprobante)
        {
            case FETipoComprobante.FACTURAS_A:
                letra = "A";
                comp = "FACTURA";
                break;
            case FETipoComprobante.FACTURAS_B:
                letra = "B";
                comp = "FACTURA";
                break;
            case FETipoComprobante.FACTURAS_C:
                letra = "C";
                comp = "FACTURA";
                break;
            case FETipoComprobante.FACTURAS_E:
                letra = "E";
                comp = "FACTURA";
                break;

            case FETipoComprobante.RECIBO_A:
                letra = "A";
                comp = "RECIBO";
                break;
            case FETipoComprobante.RECIBO_B:
                letra = "B";
                comp = "RECIBO";
                break;
            case FETipoComprobante.RECIBO_C:
                letra = "C";
                comp = "RECIBO";
                break;

            case FETipoComprobante.NOTAS_DEBITO_A:
                letra = "A";
                comp = "NOTA DE DÉBITO";
                break;
            case FETipoComprobante.NOTAS_DEBITO_B:
                letra = "B";
                comp = "NOTA DE DÉBITO";
                break;
            case FETipoComprobante.NOTAS_DEBITO_C:
                letra = "C";
                comp = "NOTA DE DÉBITO";
                break;
            case FETipoComprobante.NOTAS_DEBITO_E:
                letra = "E";
                comp = "NOTA DE DÉBITO";
                break;

            case FETipoComprobante.NOTAS_CREDITO_A:
                letra = "A";
                comp = "NOTA DE CRÉDITO";
                break;
            case FETipoComprobante.NOTAS_CREDITO_B:
                letra = "B";
                comp = "NOTA DE CRÉDITO";
                break;
            case FETipoComprobante.NOTAS_CREDITO_C:
                letra = "C";
                comp = "NOTA DE CRÉDITO";
                break;
            case FETipoComprobante.NOTAS_CREDITO_E:
                letra = "E";
                comp = "NOTA DE CRÉDITO";
                break;
            case FETipoComprobante.SIN_DEFINIR_RECIBO:
                letra = "X";
                comp = "Recibo";
                break;
            case FETipoComprobante.SIN_DEFINIR:
                letra = "X";
                comp = "Documento no válido como factura";
                break;
            case FETipoComprobante.PRESUPUESTO:
                letra = "X";
                comp = "Presupuesto";
                break;
            case FETipoComprobante.REMITO:
                letra = "R";
                comp = "Remito";
                break;
        }

        pdf.EscribirXY(letra, 302, 50, 22, Alineado.Centro, NFPDFColor.WHITE);
        if (codigo != "")
            pdf.EscribirXY("Cod. " + codigo, 302, 65, 10, Alineado.Centro, NFPDFColor.WHITE);
        pdf.EscribirXY(comp, 570, 45, 12, Alineado.Derecha);
        //if (!esRecibo) 
        //{
        //    pdf.EscribirXY(comprobante.Original ? "Original" : "Duplicado", 570, 60, 10, Alineado.Derecha);

        //}
        //else
        //{
        //    pdf.EscribirXY("Nº: " + comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), 570, 90, 15, Alineado.Derecha);

        //}

        pdf.EscribirXY(comprobante.Original ? "Original" : "Duplicado", 570, 60, 10, Alineado.Derecha);

        if (letra == "X")
        {
            pdf.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), 570, 90, 15, Alineado.Derecha);
        }
        else
        {
            pdf.EscribirXY("Nº: " + comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), 570, 90, 15, Alineado.Derecha);
        }


        //pdf.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), 570, 90, 15, Alineado.Derecha);
        pdf.EscribirXY("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy"), 570, 105, 12, Alineado.Derecha);

        #region Datos del cliente

        var tipoDoc = (comprobante.DocTipo == 96) ? "DNI" : "CUIT";
        var NroDeDoc = (comprobante.DocNro != 0) ? comprobante.DocNro.ToString() : "";
        pdf.EscribirXY("Señor/es: " + comprobante.ClienteNombre, 27, 125, 10, Alineado.Izquierda);
        pdf.EscribirXY("Domicilio: " + comprobante.ClienteDomicilio, 27, 140, 10, Alineado.Izquierda);
        pdf.EscribirXY("Localidad: " + comprobante.ClienteLocalidad, 27, 155, 10, Alineado.Izquierda);
        if (comprobante.DocTipo != 99)
            pdf.EscribirXY(tipoDoc + ": " + NroDeDoc, 331, 125, 10, Alineado.Izquierda);
        pdf.EscribirXY("Condición de IVA: " + comprobante.ClienteCondiionIva, 331, 140, 10, Alineado.Izquierda);
        if (!esRecibo)
            pdf.EscribirXY("Condición de venta: " + comprobante.CondicionVenta, 27, 155, 10, Alineado.Izquierda);

        #endregion

        #region Datos de la empresa facturante

        pdf.EscribirXY(comprobante.RazonSocial, 27, 205, 10, Alineado.Izquierda);

        pdf.EscribirXY(comprobante.Domicilio, 27, 220, 10, Alineado.Izquierda);
        pdf.EscribirXY(comprobante.CiudadProvincia, 27, 235, 10, Alineado.Izquierda);
        pdf.EscribirXY("Tel.: " + comprobante.Telefono + (!string.IsNullOrEmpty(comprobante.Celular) ? " - Cel.: " + comprobante.Celular : ""), 27, 250, 10, Alineado.Izquierda);

        pdf.EscribirXY("CUIT: " + comprobante.Cuit, 570, 205, 10, Alineado.Derecha);
        pdf.EscribirXY("Ingresos brutos: " + comprobante.IIBB, 570, 220, 10, Alineado.Derecha);
        pdf.EscribirXY("Inicio de actividades: " + comprobante.FechaInicioActividades, 570, 235, 10, Alineado.Derecha);
        pdf.EscribirXY(comprobante.CondicionIva, 570, 250, 10, Alineado.Derecha);

        #endregion

        #region Vendedor

        if (comprobante.Vendedor != null)
            if (!comprobante.Vendedor.Equals(""))
                pdf.EscribirXY("Vendedor: " + comprobante.Vendedor, 27, 190, 10, Alineado.Izquierda);

        #endregion



        #region Datos generales
        if (!esRemito)
        {
            pdf.EscribirBoxXY("Son Pesos " + NumeroALetrasMoneda(comprobante.ImpTotal) + ".", 27, 680, 10, 400);
            pdf.EscribirBoxXY(comprobante.Observaciones, 27, 715, 10, 400);
        }
        int totalesY = 680;

        if (letra == "A" && !esRecibo && !esRemito)
        {
            pdf.EscribirXY("Subtotal: $" + Math.Abs(comprobante.ImpNeto).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
            totalesY += 15;

            if (comprobante.DescuentoPorcentaje != null && comprobante.DescuentoImporte != null)
            {
                pdf.EscribirXY("Descuento " + comprobante.DescuentoPorcentaje.Value.ToString("N2") + "%: $" + Math.Abs(comprobante.DescuentoImporte.Value).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                totalesY += 15;
            }

            foreach (FERegistroIVA rIVA in comprobante.DetalleIva)
            {
                if (rIVA.Importe != 0)
                {
                    switch (rIVA.TipoIva)
                    {
                        case FETipoIva.Iva0:
                            pdf.EscribirXY("IVA 0%: $" + Math.Abs(rIVA.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                            totalesY += 15;
                            break;

                        case FETipoIva.Iva10_5:
                            pdf.EscribirXY("IVA 10.5%: $" + Math.Abs(rIVA.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                            totalesY += 15;
                            break;

                        case FETipoIva.Iva21:
                            pdf.EscribirXY("IVA 21%: $" + Math.Abs(rIVA.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                            totalesY += 15;
                            break;

                        case FETipoIva.Iva27:
                            pdf.EscribirXY("IVA 27%: $" + Math.Abs(rIVA.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                            totalesY += 15;
                            break;

                        case FETipoIva.Iva5:
                            pdf.EscribirXY("IVA 5%: $" + Math.Abs(rIVA.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                            totalesY += 15;
                            break;
                        case FETipoIva.Iva2_5:
                            pdf.EscribirXY("IVA 2.5%: $" + Math.Abs(rIVA.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                            totalesY += 15;
                            break;
                    }
                }
            }
        }

        if (!esRemito)
        {
            foreach (var item in comprobante.Tributos)
            {
                if (item.Importe != 0)
                {
                    pdf.EscribirXY(item.Decripcion + " " + item.Alicuota + "%: $" + Math.Abs(item.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                    totalesY += 15;
                }
            }

            if (comprobante.ImpTotConc > 0)
            {
                pdf.EscribirXY("No Gravado: $" + Math.Abs(comprobante.ImpTotConc).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                totalesY += 15;
            }

            pdf.EscribirXY("Total: $" + Math.Abs(comprobante.ImpTotal).ToString("N2"), 570, 800, 15, Alineado.Derecha);
        }
        if (!esRemito)
        {
            if (comprobante.TipoComprobante != FETipoComprobante.SIN_DEFINIR &&
                comprobante.TipoComprobante != FETipoComprobante.SIN_DEFINIR_RECIBO &&
                !string.IsNullOrEmpty(comprobante.CAE))
            {
                //pdf.EscribirXY("CAE: " + comprobante.CAE, 27, 770, 8, Alineado.Izquierda);
                //pdf.EscribirXY("Vencimiento CAE: " + comprobante.FechaVencCAE.ToString("dd/MM/yyyy"), 150, 770, 8, Alineado.Izquierda);
                //pdf.InsertarImagenXY(ImagenCodigoBarras(comprobante), 27, 45);
                //pdf.EscribirXY(CodigoDeBarras(comprobante), 27, 808, 10, Alineado.Izquierda);

                pdf.EscribirXY("CAE: " + comprobante.CAE, 140, 770, 10, Alineado.Izquierda);
                pdf.EscribirXY("Vencimiento CAE: " + comprobante.FechaVencCAE.ToString("dd/MM/yyyy"), 140, 790, 10, Alineado.Izquierda);
            }

        }
        /*int i = 0;
        double itemPrecio;*/
        int fontDetalleFE = 10;

        if (ConfigurationManager.AppSettings["FE.FontDetalle"] != null) fontDetalleFE = int.Parse(ConfigurationManager.AppSettings["FE.FontDetalle"]);

        List<FETipoComprobante> tipoRemitos = new List<FETipoComprobante>() { FETipoComprobante.RECIBO_A, FETipoComprobante.RECIBO_B, FETipoComprobante.RECIBO_C, FETipoComprobante.SIN_DEFINIR_RECIBO };

        //if (tipoRemitos.Contains(comprobante.TipoComprobante))
        //{
        //    //pdf.EscribirBoxXY(comprobante.ConceptoRecibo, 27, 280, 10, 540);
        //    totalesY = 280;
        //    int i = 0;
        //    foreach (var concepto in comprobante.ConceptoRecibo.Split('|'))
        //    {
        //        pdf.EscribirBoxXY(concepto, 27, totalesY, 10, 540);
        //        totalesY += (i == 0 ? 30 : 15);
        //        i++;
        //    }
        //}
        //else 

        if (esRemito)
            pdf.InsertarTablaDetalleRemito(comprobante.ItemsDetalle, 0, 0);
        else
            pdf.InsertarTablaDetalle(comprobante, 0, 0);

        #endregion

        return pdf.GenerarPDFStream();
    }


    /// <summary>
    /// Solo se usa para recibos de pagos X
    /// </summary>
    /// <param name="usu"></param>
    /// <param name="id"></param>
    /// <param name="idPersona"></param>
    /// <param name="tipo"></param>
    /// <param name="modo"></param>
    /// <param name="fecha"></param>
    /// <param name="idPuntoVenta"></param>
    /// <param name="nroComprobante"></param>
    /// <param name="obs"></param>
    /// <param name="accion"></param>
    public static void CrearRetencionGanancia(WebUser usu, int id, int idPersona, string tipo, ref string nroComprobante, string fecha, string obs, ComprobanteModo accion)
    {
        var pathPdf = HttpContext.Current.Server.MapPath("~/files/retencionGanancia/" + usu.IDUsuario + "/");
        if (!Directory.Exists(pathPdf))
            Directory.CreateDirectory(pathPdf);

        using (var dbContext = new ACHEEntities())
        {
            Personas personaProveedor = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (personaProveedor == null)
                throw new Exception("El cliente/proveedor es inexistente");

            Usuarios personaUsuario = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (personaUsuario == null)
                throw new Exception("El usuario es inexistente");

            Pagos pago = dbContext.Pagos.Where(x => x.IDPago == id).FirstOrDefault();
            if (pago == null)
                throw new Exception("El pago es inexistente");

            PagosRetenciones pagoRetenciones = dbContext.PagosRetenciones.Where(x => x.IDPago == id).FirstOrDefault();
            if (pagoRetenciones == null)
                throw new Exception("El pagoRetenciones es inexistente");

            PagosDetalle pagoDetalles = dbContext.PagosDetalle.Where(x => x.IDPago == id).FirstOrDefault();
            if (pagoDetalles == null)
                throw new Exception("El pagoDetalles es inexistente");

            Compras compra = dbContext.Compras.Where(w => w.IDCompra == pagoDetalles.IDCompra).FirstOrDefault();
            if (compra == null)
                throw new Exception("La compra es inexistente");

            nroComprobante = "0000 - " + pago.FechaPago.Year.ToString() + " - " + pagoRetenciones.NroReferencia.ToString().PadLeft(8, '0');

            pathPdf = pathPdf + nroComprobante + ".pdf";

            GrabarEnDiscoRetencionGanancia(personaProveedor, personaUsuario, pago, pagoRetenciones, compra.Rubro, pathPdf);
        }
    }

    public static void CrearRetencionGananciaPorIdRetencion(WebUser usu, int idPagoRetencion, ref string nroComprobante, ComprobanteModo accion)
    {
        var pathPdf = HttpContext.Current.Server.MapPath("~/files/retencionGanancia/" + usu.IDUsuario + "/");
        if (!Directory.Exists(pathPdf))
            Directory.CreateDirectory(pathPdf);

        using (var dbContext = new ACHEEntities())
        {
            PagosRetenciones pagoRetenciones = dbContext.PagosRetenciones.Where(x => x.IDPagoRetenciones == idPagoRetencion).FirstOrDefault();
            if (pagoRetenciones == null)
                throw new Exception("El pagoRetenciones es inexistente");

            Pagos pago = dbContext.Pagos.Where(x => x.IDPago == pagoRetenciones.IDPago).FirstOrDefault();
            if (pago == null)
                throw new Exception("El pago es inexistente");

            Personas personaProveedor = dbContext.Personas.Where(x => x.IDPersona == pago.IDPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (personaProveedor == null)
                throw new Exception("El cliente/proveedor es inexistente");

            Usuarios personaUsuario = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (personaUsuario == null)
                throw new Exception("El usuario es inexistente");

            PagosDetalle pagoDetalles = dbContext.PagosDetalle.Where(x => x.IDPago == pagoRetenciones.IDPago).FirstOrDefault();
            if (pagoDetalles == null)
                throw new Exception("El pagoDetalles es inexistente");

            Compras compra = dbContext.Compras.Where(w => w.IDCompra == pagoDetalles.IDCompra).FirstOrDefault();
            if (compra == null)
                throw new Exception("La compra es inexistente");

            nroComprobante = "0000 - " + pago.FechaPago.Year.ToString() + " - " + pagoRetenciones.NroReferencia.ToString().PadLeft(8, '0');

            pathPdf = pathPdf + nroComprobante + ".pdf";

            GrabarEnDiscoRetencionGanancia(personaProveedor, personaUsuario, pago, pagoRetenciones, compra.Rubro, pathPdf);
        }
    }

    public static void GrabarEnDiscoRetencionGanancia(Personas personaProveedor, Usuarios personaUsuario, Pagos pago,
                                                        PagosRetenciones pagoRetenciones, string rubro, string archivoDestino)
    {
        Stream streamPDF = GetStreamPDFRetencionGanancia(personaProveedor, personaUsuario, pago, pagoRetenciones, rubro);

        using (Stream destination = File.Create(archivoDestino))
        {
            for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                destination.WriteByte((byte)a);
        }
    }

    public static Stream GetStreamPDFRetencionGanancia(Personas personaProveedor, Usuarios personaUsuario, Pagos pago,
                                                        PagosRetenciones pagoRetenciones, string rubro)
    {        
        int y = 50;

        var dbContext = new ACHEEntities();

        NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4);

        pdf.EscribirXY("SI.CO.RE. - Sistema de Control de Retenciones", 302, y, 22, Alineado.Centro, NFPDFColor.BLACK);

        y += 20;
        pdf.EscribirXY("Certificado N°: 0000-" + pago.FechaPago.Year.ToString() + "-" + pagoRetenciones.NroReferencia.ToString().PadLeft(8, '0'), 130, y, 16, Alineado.Izquierda, NFPDFColor.BLACK);

        y += 20;
        pdf.EscribirXY("Fecha: " + pago.FechaPago.ToShortDateString(), 130, y, 16, Alineado.Izquierda, NFPDFColor.BLACK);



        y += 50;
        pdf.EscribirXY("A - Datos del Agente de Retención", 27, y, 14, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Razón Social: " + personaUsuario.RazonSocial.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Nro de CUIT:  " + personaUsuario.CUIT.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Domicilio:    " + personaUsuario.Domicilio.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Localidad:    " + dbContext.Ciudades.Where(w => w.IDCiudad == personaUsuario.IDCiudad).Select(s => s.Nombre).FirstOrDefault().ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("C.P.:         " + personaUsuario.CodigoPostal.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Provincia:    " + dbContext.Provincias.Where(w => w.IDProvincia == personaUsuario.IDProvincia).Select(s => s.Nombre).FirstOrDefault().ToUpper(), 27, y, 11, Alineado.Izquierda);


        y += 50;
        pdf.EscribirXY("B - Datos del Sujeto Retenido", 27, y, 14, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Razón Social: " + personaProveedor.RazonSocial.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Nro de CUIT:  " + personaProveedor.NroDocumento.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Domicilio:    " + personaProveedor.Domicilio.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Localidad:    " + dbContext.Ciudades.Where(w => w.IDCiudad == personaProveedor.IDCiudad).Select(s => s.Nombre).FirstOrDefault().ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("C.P.:         " + personaProveedor.CodigoPostal.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Provincia:    " + dbContext.Provincias.Where(w => w.IDProvincia == personaProveedor.IDProvincia).Select(s => s.Nombre).FirstOrDefault().ToUpper(), 27, y, 11, Alineado.Izquierda);


        y += 50;
        pdf.EscribirXY("C - Datos de la Retención Practicada", 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Impuesto:                                       RET. GANANCIA", 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Régimen:                                        " + rubro.ToUpper(), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Monto del Comprobante que origina la Retención: " + pago.ImporteTotal.ToString("N2"), 27, y, 11, Alineado.Izquierda);
        y += 20;
        pdf.EscribirXY("Monto de la Retención:                          " + pagoRetenciones.Importe.ToString("N2"), 27, y, 11, Alineado.Izquierda);

        y += 50;
        pdf.EscribirXY("Firma del Agente de Retención: ", 27, y, 11, Alineado.Izquierda);
        y += 30;
        pdf.EscribirXY("Aclaración: ", 27, y, 11, Alineado.Izquierda);
        y += 30;
        pdf.EscribirXY("Cargo: ", 27, y, 11, Alineado.Izquierda);
        y += 40;
        pdf.EscribirXY("Declaro que los datos consignados en este Formulario son correctos y completos, siendo fiel expresión de la verdad.", 27, y, 10, Alineado.Izquierda);


        return pdf.GenerarPDFStream();
    }

    private static string NumeroALetrasMoneda(double value)
    {
        string[] total = value.ToString().Replace(',', '.').Split('.');
        double total_entero = double.Parse(total[0]);
        double total_decimales = 0;

        if (total.Length > 1)
        {
            total[1] = (total[1] + "0").Substring(0, 2);
            total_decimales = double.Parse(total[1]);
        }

        return NumeroALetras(total_entero) + " con " + NumeroALetras(total_decimales);
    }

    private static string NumeroALetras(double value)
    {
        string Num2Text = "";

        value = Math.Abs(Math.Truncate(value));

        if (value == 0) Num2Text = "CERO";
        else if (value == 1) Num2Text = "UNO";
        else if (value == 2) Num2Text = "DOS";
        else if (value == 3) Num2Text = "TRES";
        else if (value == 4) Num2Text = "CUATRO";
        else if (value == 5) Num2Text = "CINCO";
        else if (value == 6) Num2Text = "SEIS";
        else if (value == 7) Num2Text = "SIETE";
        else if (value == 8) Num2Text = "OCHO";
        else if (value == 9) Num2Text = "NUEVE";
        else if (value == 10) Num2Text = "DIEZ";
        else if (value == 11) Num2Text = "ONCE";
        else if (value == 12) Num2Text = "DOCE";
        else if (value == 13) Num2Text = "TRECE";
        else if (value == 14) Num2Text = "CATORCE";
        else if (value == 15) Num2Text = "QUINCE";
        else if (value < 20) Num2Text = "DIECI" + NumeroALetras(value - 10);
        else if (value == 20) Num2Text = "VEINTE";
        else if (value < 30) Num2Text = "VEINTI" + NumeroALetras(value - 20);
        else if (value == 30) Num2Text = "TREINTA";
        else if (value == 40) Num2Text = "CUARENTA";
        else if (value == 50) Num2Text = "CINCUENTA";
        else if (value == 60) Num2Text = "SESENTA";
        else if (value == 70) Num2Text = "SETENTA";
        else if (value == 80) Num2Text = "OCHENTA";
        else if (value == 90) Num2Text = "NOVENTA";


        else if (value < 100) Num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
        else if (value == 100) Num2Text = "CIEN";
        else if (value < 200) Num2Text = "CIENTO " + NumeroALetras(value - 100);
        else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800))
            Num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";
        else if (value == 500) Num2Text = "QUINIENTOS";
        else if (value == 700) Num2Text = "SETECIENTOS";
        else if (value == 900) Num2Text = "NOVECIENTOS";
        else if (value < 1000)
            Num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
        else if (value == 1000) Num2Text = "MIL";
        else if (value < 2000) Num2Text = "MIL " + NumeroALetras(value % 1000);
        else if (value < 1000000)
        {
            Num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
            if ((value % 1000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value % 1000);
        }

        else if (value == 1000000) Num2Text = "UN MILLON";
        else if (value < 2000000) Num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
        else if (value < 1000000000000)
        {
            Num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";

            if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                Num2Text = Num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
        }

        else if (value == 1000000000000) Num2Text = "UN BILLON";
        else if (value < 2000000000000)
            Num2Text = "UN BILLON " +
                       NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
        else
        {
            Num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
            if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                Num2Text = Num2Text + " " +
                           NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
        }

        return Num2Text;
    }

    public static void GrabarEnDiscoPago(Stream streamPDF, string archivoDestino, string templateFc, string pathLogo)
    {
        //Stream streamPDF = GetStreamPDF(comprobante, templateFc, pathLogo);

        using (Stream destination = File.Create(archivoDestino))
        {
            for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                destination.WriteByte((byte)a);
        }
    }


    public static bool AlertaPlan(ACHEEntities dbContext, int idUsuario, ref string nombre, ref int cant)
    {
        var plan = PermisosModulos.ObtenerPlanActual(dbContext, idUsuario);

        if (plan == null)
            return false;

        var planes = dbContext.PlanesPagos.Where(x => x.IDUsuario == plan.IDUsuario && x.FechaInicioPlan >= plan.FechaFinPlan).ToList();
        if (planes.Count() > 0)
            return false;

        if (plan.IDPlan == 1) //Plan Basico NO VENCE
        {
            return false;
        }

        var diferencia = (Convert.ToDateTime(plan.FechaFinPlan) - DateTime.Now.Date).Days;
        if (diferencia <= 5)
        {
            nombre = plan.Planes.Nombre;
            cant = diferencia;
            return true;
        }
        else
            return false;
    }

    public static void EnviarComprobantePorMail(string nombre, string para, string asunto, string mensaje, string file)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        MailAddressCollection listTo = new MailAddressCollection();
        foreach (var mail in para.Split(','))
        {
            if (mail != string.Empty)
                listTo.Add(new MailAddress(mail));
        }

        ListDictionary replacements = new ListDictionary();
        replacements.Add("<NOTIFICACION>", mensaje);
        if (nombre.Trim() != string.Empty)
            replacements.Add("<USUARIO>", nombre);
        else
            replacements.Add("<USUARIO>", "usuario");

        List<string> attachments = new List<string>();
        if (File.Exists(HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/" + file))){
            attachments.Add(HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/" + file));
        }

        bool send = EmailHelper.SendMessage(EmailTemplate.EnvioComprobante, replacements, listTo, ConfigurationManager.AppSettings["Email.Notifications"], usu.Email, asunto, attachments);
        if (!send)
            throw new Exception("El mensaje no pudo ser enviado. Por favor, intente nuevamente. En caso de continuar el error, escribenos a <a href='" + ConfigurationManager.AppSettings["Email.Ayuda"] + "'>" + ConfigurationManager.AppSettings["Email.Ayuda"] + "</a>");
    }

    public static int RegistrarEnvioLog(string entidad, string url, string nombre, string mensaje, int idUsuario)
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                DateTime dateTime = DateTime.Now.AddMonths(-1);
                List<LogServicios> logServicios = dbContext.LogServicios.Where(x => x.FechaEmision < dateTime).ToList();
                if (logServicios != null)
                {
                    dbContext.LogServicios.RemoveRange(logServicios);
                    dbContext.SaveChanges();
                }
                List<DatosAFIPPersonas> logDatosAFIPPersonas = dbContext.DatosAFIPPersonas.Where(x => x.Fecha < dateTime).ToList();
                if (logDatosAFIPPersonas != null)
                {
                    dbContext.DatosAFIPPersonas.RemoveRange(logDatosAFIPPersonas);
                    dbContext.SaveChanges();
                }

                LogServicios l = new LogServicios();
                l.Entidad = entidad;
                l.Url = url;
                l.Nombre = nombre;
                l.Mensaje = mensaje;
                l.FechaEmision = DateTime.Now;
                l.IdUsuario = idUsuario;
                l.Envio = 0;
                l.Respuesta = null;
                l.RespuestaExitosa = false;
                l.FechaRespuesta = null;
                dbContext.LogServicios.Add(l);
                dbContext.SaveChanges();
                return l.IDLogServicio;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static void RegistrarRespuestaLog(int idLogServicio, int envio, string respuesta, bool respuestaExitosa)
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                LogServicios logServicio = dbContext.LogServicios.Where(x => x.IDLogServicio == idLogServicio).FirstOrDefault();
                if (logServicio != null)
                { 
                    logServicio.Envio = envio;
                    logServicio.Respuesta = respuesta;
                    logServicio.RespuestaExitosa = respuestaExitosa;
                    logServicio.FechaRespuesta = DateTime.Now;
                    dbContext.SaveChanges();
                }                
            }

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static Boolean IsNumeric(string valor)
    {
        try
        {
            Int64 resultado = Convert.ToInt64(valor);
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public static Boolean IsDate(string valor)
    {
        try
        {
            DateTime resultado = Convert.ToDateTime(valor);
            if (resultado < DateTime.Now.AddYears(-100))
                return false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void VaciarCarpeta(string carpeta)
    {
        DateTime fecha = DateTime.Now.AddDays(-1);

        string ruta = System.AppDomain.CurrentDomain.BaseDirectory + carpeta;

        String[] files = Directory.GetFiles(@ruta).Where(x => new FileInfo(x).Extension != ".tex").ToArray();
        foreach (string file in files)
        {
            File.Delete(file);
        }
        foreach (string item in Directory.GetDirectories(ruta).Where(x => new FileInfo(x).LastWriteTime.Date <= fecha))
            Directory.Delete(item, true);

    }

}