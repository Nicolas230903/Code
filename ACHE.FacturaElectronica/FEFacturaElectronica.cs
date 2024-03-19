using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ACHE.FacturaElectronica.Lib;
using ACHE.FacturaElectronica.WSFacturaElectronica;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using iTextSharp.text;
using Newtonsoft.Json;
using System.Net;
using System.Drawing.Drawing2D;
using QRCoder;
using System.Collections;
using System.Runtime.ConstrainedExecution;

namespace ACHE.FacturaElectronica
{
    public class FEFacturaElectronica
    {
        private static object bloqueo = new object();

        /*public void GenerarComprobante(FEComprobante comprobante, string template)
        {
            GenerarComprobante(comprobante, ConfigurationManager.AppSettings["FE.wsaa"], ConfigurationManager.AppSettings["FE.CertificadoAFIP"], template);//ConfigurationManager.AppSettings["FE.Template"]);
        }*/

        static object lockLogXML = new object();
        //private int size;

        public void GetArchivoXML(string xmlCAERequest, string xmlAuthRequest, string xmlCAEResponse)
        {
            lock (lockLogXML)
            {
                string pathXML = ConfigurationManager.AppSettings["FE.PatLoghXML"];
                XmlDocument doc = new XmlDocument();
                XmlElement root;
                XmlNode xmlLOGS;
                string entorno = ConfigurationManager.AppSettings["FE.wsfev1"].Contains("homo.afip.gov.ar") ? "TESTING" : "PRODUCCION";

                if (!File.Exists(pathXML))
                {
                    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-16", null);
                    root = doc.DocumentElement;
                    doc.InsertBefore(xmlDeclaration, root);

                    xmlLOGS = doc.CreateElement("LOGS");
                    doc.AppendChild(xmlLOGS);
                }
                else
                {
                    doc.Load(pathXML);
                    //root = doc.DocumentElement;
                    xmlLOGS = doc.GetElementsByTagName("LOGS")[0];
                }

                XmlElement xmlFECAERequest = doc.CreateElement("LogFECAERequest");
                xmlFECAERequest.SetAttribute("Entorno", entorno);
                xmlFECAERequest.InnerText = xmlCAERequest;
                xmlLOGS.AppendChild(xmlFECAERequest);

                XmlElement FEAuthRequest = doc.CreateElement("LogFEAuthRequest");
                FEAuthRequest.SetAttribute("Entorno", entorno);
                FEAuthRequest.InnerText = xmlAuthRequest;
                xmlLOGS.AppendChild(FEAuthRequest);

                XmlElement FECAEResponse = doc.CreateElement("LogFECAEResponse");
                FECAEResponse.SetAttribute("Entorno", entorno);
                FECAEResponse.InnerText = xmlCAEResponse;
                xmlLOGS.AppendChild(FECAEResponse);

                doc.Save(pathXML);
            }
        }

        private bool ValidarComprobante(FEComprobante comprobante, out string error)
        {
            List<FETipoComprobante> tiposCUITObligatorio = new List<FETipoComprobante>() { FETipoComprobante.FACTURAS_A, FETipoComprobante.NOTAS_CREDITO_A, FETipoComprobante.NOTAS_DEBITO_A };
            error = "";

            if (comprobante.Cuit == comprobante.DocNro)
                error = "El CUIT del emisor no puede ser el mismo cuit o documento de quien recibe la factura";
            else if (comprobante.TipoComprobante == FETipoComprobante.FACTURAS_C && comprobante.TotalIva != 0)
                error = "El Importe de IVA para comprobantes tipo C debe ser igual a cero (0).";
            //else if (comprobante.TipoComprobante != FETipoComprobante.FACTURAS_C && comprobante.ImpTotal != comprobante.ImpNeto + comprobante.TotalIva)
            else if (comprobante.TipoComprobante != FETipoComprobante.FACTURAS_C && comprobante.ImpTotal != comprobante.ImpNeto + comprobante.TotalIva + comprobante.ImpOpEx + comprobante.ImpTotConc + comprobante.Tributos.Sum(x => x.Importe))
                error = "El importe total no coincide con el neto + iva.";
            //else if (comprobante.TipoComprobante != FETipoComprobante.FACTURAS_C && comprobante.ImpNeto != 0 && comprobante.DetalleIva.Count == 0)
            //    error = "Si ImpNeto es mayor a 0 el IVA es obligatorio.";
            else if (tiposCUITObligatorio.Contains(comprobante.TipoComprobante) && comprobante.DocTipo != 80)
                error = "Para comprobantes clase A, se debe informar el CUIT";// el campo DocTipo debe ser igual a 80 (CUIT)
            else if (comprobante.Concepto == FEConcepto.Producto && comprobante.Fecha <= DateTime.Now.AddDays(-5))
                error = "La fecha de emisión del comprobante no puede ser anterior al " + DateTime.Now.AddDays(-5).ToString("dd/MM/yyyy");
            else if (comprobante.Concepto == FEConcepto.Producto && comprobante.Fecha >= DateTime.Now.AddDays(5))
                error = "La fecha de emisión del comprobante no puede ser supeior al " + DateTime.Now.AddDays(5).ToString("dd/MM/yyyy");
            else if (comprobante.Concepto != FEConcepto.Producto && comprobante.Fecha <= DateTime.Now.AddDays(-11))
                error = "La fecha de emisión del comprobante no puede ser anterior al " + DateTime.Now.AddDays(-11).ToString("dd/MM/yyyy");
            else if (comprobante.Concepto != FEConcepto.Producto && comprobante.Fecha >= DateTime.Now.AddDays(10))
                error = "La fecha de emisión del comprobante no puede ser supeior al " + DateTime.Now.AddDays(10).ToString("dd/MM/yyyy");
            else if (comprobante.FchVtoPago.HasValue && comprobante.FchVtoPago.Value < comprobante.Fecha)
                error = "La fecha de vencimiento no puede ser menor a la fecha de emisión del comprobante";

            /* else if (comprobante.TipoComprobante != FETipoComprobante.FACTURAS_C && comprobante.DetalleIva.Sum(d => d.BaseImp) != comprobante.ImpNeto)
                 error = "La suma de los campos BaseImp en AlicIva debe ser igual al valor ingresado en ImpNeto.";
             */
            return error == "";
        }

        public FEComprobante GetComprobante(long cuit, long cuitAfip, long CbtNro, int PtoVta, FETipoComprobante tipoComprobante, bool forzarNuevoTicket)
        {
            try
            {
                Service objWSFEV1 = new Service();
                FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                FETicket ticket = FEAutenticacion.GetTicket(cuit, cuitAfip, "wsfe", /*urlWsaaWsdl, certificadoAfip,*/ "PROD", forzarNuevoTicket);
                FEComprobante comprobante = new FEComprobante();
                FECompConsultaReq filtros = new FECompConsultaReq();

                objWSFEV1.Url = ConfigurationManager.AppSettings["FE.PROD.wsfev1"];

                objFEAuthRequest.Token = ticket.Token;
                objFEAuthRequest.Sign = ticket.Sign;
                objFEAuthRequest.Cuit = ticket.Cuit;

                filtros.CbteTipo = (int)tipoComprobante;
                filtros.CbteNro = CbtNro;
                filtros.PtoVta = PtoVta;
                FECompConsultaResponse respuesta = objWSFEV1.FECompConsultar(objFEAuthRequest, filtros);

                if (respuesta.Errors != null && respuesta.Errors.Length > 0)
                    throw new Exception(respuesta.Errors[0].Msg + " (Cod " + respuesta.Errors[0].Code.ToString() + ")");
                else
                {
                    comprobante.CAE = respuesta.ResultGet.CodAutorizacion;
                    comprobante.PtoVta = respuesta.ResultGet.PtoVta;
                    comprobante.TipoComprobante = (FETipoComprobante)respuesta.ResultGet.CbteTipo;
                    comprobante.NumeroComprobante = (int)respuesta.ResultGet.CbteDesde;
                    comprobante.Concepto = (FEConcepto)respuesta.ResultGet.Concepto;
                    comprobante.DocTipo = respuesta.ResultGet.DocTipo;
                    comprobante.DocNro = respuesta.ResultGet.DocNro;
                    comprobante.Fecha = DateTime.ParseExact(respuesta.ResultGet.CbteFch, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    //comprobante.ImpTotal = respuesta.ResultGet.ImpTotal;
                    comprobante.ImpTotConc = respuesta.ResultGet.ImpTotConc;
                    //comprobante.ImpNeto = respuesta.ResultGet.ImpNeto;
                    comprobante.ImpOpEx = respuesta.ResultGet.ImpOpEx;
                    comprobante.FchServDesde = respuesta.ResultGet.FchServDesde == string.Empty ? (DateTime?)null : DateTime.ParseExact(respuesta.ResultGet.FchServDesde, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    comprobante.FchServHasta = respuesta.ResultGet.FchServHasta == string.Empty ? (DateTime?)null : DateTime.ParseExact(respuesta.ResultGet.FchServHasta, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    comprobante.FchVtoPago = respuesta.ResultGet.FchVtoPago == string.Empty ? (DateTime?)null : DateTime.ParseExact(respuesta.ResultGet.FchVtoPago, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    comprobante.CodigoMoneda = respuesta.ResultGet.MonId;
                    comprobante.CotizacionMoneda = respuesta.ResultGet.MonCotiz;
                    comprobante.DocTipo = respuesta.ResultGet.DocTipo;
                    comprobante.DocNro = respuesta.ResultGet.DocNro;

                    comprobante.FechaVencCAE = DateTime.ParseExact(respuesta.ResultGet.FchVto, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                    if (respuesta.ResultGet.Observaciones != null && respuesta.ResultGet.Observaciones.Length > 0)
                        comprobante.Observaciones = respuesta.ResultGet.Observaciones[0].Msg;
                    else
                        comprobante.Observaciones = string.Empty;

                    var lregIva = new List<FERegistroIVA>();
                    foreach (AlicIva alicIva in respuesta.ResultGet.Iva)
                    {
                        var regIva = new FERegistroIVA();
                        regIva.BaseImp = alicIva.BaseImp;
                        regIva.TipoIva = (FETipoIva)alicIva.Id;
                        lregIva.Add(regIva);
                    }
                    comprobante.DetalleIva = lregIva;


                    var lregTrib = new List<FERegistroTributo>();
                    if (respuesta.ResultGet.Tributos != null)
                    {
                        foreach (Tributo tributo in respuesta.ResultGet.Tributos)
                        {
                            var regTrib = new FERegistroTributo();
                            regTrib.BaseImp = tributo.BaseImp;
                            regTrib.Importe = tributo.Importe;
                            regTrib.Alicuota = tributo.Alic;
                            regTrib.Decripcion = tributo.Desc;
                            lregTrib.Add(regTrib);
                        }
                    }
                    comprobante.Tributos = lregTrib;

                }

                return comprobante;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        public FECompConsultaResponse GetComprobanteResponse(long cuit, long cuitAfip, long CbtNro, int PtoVta, FETipoComprobante tipoComprobante, bool forzarNuevoTicket)
        {
            try
            {
                Service objWSFEV1 = new Service();
                FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                FETicket ticket = FEAutenticacion.GetTicket(cuit, cuitAfip, "wsfe", /*urlWsaaWsdl, certificadoAfip,*/ "PROD", forzarNuevoTicket);
                FEComprobante comprobante = new FEComprobante();
                FECompConsultaReq filtros = new FECompConsultaReq();

                objWSFEV1.Url = ConfigurationManager.AppSettings["FE.PROD.wsfev1"];

                objFEAuthRequest.Token = ticket.Token;
                objFEAuthRequest.Sign = ticket.Sign;
                objFEAuthRequest.Cuit = ticket.Cuit;

                filtros.CbteTipo = (int)tipoComprobante;
                filtros.CbteNro = CbtNro;
                filtros.PtoVta = PtoVta;
                FECompConsultaResponse respuesta = objWSFEV1.FECompConsultar(objFEAuthRequest, filtros);

                if (respuesta.Errors != null && respuesta.Errors.Length > 0)
                    throw new Exception(respuesta.Errors[0].Msg + " (Cod " + respuesta.Errors[0].Code.ToString() + ")");

                return respuesta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void GenerarComprobante(FEComprobante comprobante, long cuitAfip, /*string urlWsaaWsdl, string certificadoAfip,*/ string templateFc, string modo)
        {
            try
            {
                GenerarComprobante(comprobante, cuitAfip, /*urlWsaaWsdl, certificadoAfip,*/ templateFc, null, modo, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ValidacionDeToken: No aparecio CUIT en lista de relaciones"))
                    GenerarComprobante(comprobante, cuitAfip, /*urlWsaaWsdl, certificadoAfip,*/ templateFc, null, modo, true);
                else
                    throw new Exception(ex.Message);
            }            
        }

        public List<FEComprobante> ConsultarUltimosComprobantesAfip(string condicionIVA, long cuit,
            long cuitAfip, string modo, bool forzarNuevoTicket, string url)
        {
            try
            {
                List<FEComprobante> lc = new List<FEComprobante>();
                List<FETipoComprobante> ltc = new List<FETipoComprobante>();
                if (condicionIVA.Equals("RI"))
                {
                    ltc.Add(FETipoComprobante.FACTURAS_A);
                    ltc.Add(FETipoComprobante.FACTURAS_B);
                    ltc.Add(FETipoComprobante.NOTAS_CREDITO_A);
                    ltc.Add(FETipoComprobante.NOTAS_CREDITO_B);
                    ltc.Add(FETipoComprobante.NOTAS_DEBITO_A);
                    ltc.Add(FETipoComprobante.NOTAS_DEBITO_B);
                    ltc.Add(FETipoComprobante.RECIBO_A);
                    ltc.Add(FETipoComprobante.RECIBO_B);
                    ltc.Add(FETipoComprobante.FACTURAS_A_MiPyMEs);
                    ltc.Add(FETipoComprobante.FACTURAS_B_MiPyMEs);
                    ltc.Add(FETipoComprobante.NOTA_CREDITO_A_MiPyMEs);
                    ltc.Add(FETipoComprobante.NOTA_CREDITO_B_MiPyMEs);
                    ltc.Add(FETipoComprobante.NOTA_DEBITO_A_MiPyMEs);
                    ltc.Add(FETipoComprobante.NOTA_DEBITO_B_MiPyMEs);
                }
                else
                {
                    ltc.Add(FETipoComprobante.FACTURAS_C);
                    ltc.Add(FETipoComprobante.NOTAS_CREDITO_C);
                    ltc.Add(FETipoComprobante.NOTAS_DEBITO_C);
                    ltc.Add(FETipoComprobante.RECIBO_C);
                    ltc.Add(FETipoComprobante.FACTURAS_C_MiPyMEs);
                    ltc.Add(FETipoComprobante.NOTA_CREDITO_C_MiPyMEs);
                    ltc.Add(FETipoComprobante.NOTA_DEBITO_C_MiPyMEs);
                }

                List<FEComprobante> fEComprobantes = new List<FEComprobante>();
                Service objWSFEV1 = new Service();
                FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                FETicket ticket = FEAutenticacion.GetTicket(cuit, cuitAfip, "wsfe", /*urlWsaaWsdl, certificadoAfip,*/ modo, forzarNuevoTicket);

                objWSFEV1.Url = url;

                objFEAuthRequest.Token = ticket.Token;
                objFEAuthRequest.Sign = ticket.Sign;
                objFEAuthRequest.Cuit = cuitAfip;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                FEPtoVentaResponse r = objWSFEV1.FEParamGetPtosVenta(objFEAuthRequest);

                foreach (PtoVenta v in r.ResultGet)
                {
                    foreach (FETipoComprobante tc in ltc)
                    {
                        FEComprobante c = new FEComprobante();
                        c.PtoVta = v.Nro;
                        c.TipoComprobante = tc;
                        lc.Add(c);
                    }
                }

                foreach (FEComprobante c in lc)
                { 
                    c.NumeroComprobante = GetUltimoComprobante(cuitAfip, c.PtoVta, c.TipoComprobante, objFEAuthRequest, objWSFEV1.Url);
                }

                return lc;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void GenerarComprobante(FEComprobante comprobante, long cuitAfip, /*string urlWsaaWsdl, string certificadoAfip,*/ 
            string templateFc, string pathLogo, string modo, bool forzarNuevoTicket)
        {
            lock (bloqueo)
            {
                string error;

                if (!ValidarComprobante(comprobante, out error))
                    throw new Exception(error);
                else
                {
                    Service objWSFEV1 = new Service();
                    FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                    FETicket ticket = FEAutenticacion.GetTicket(comprobante.Cuit, cuitAfip, "wsfe", /*urlWsaaWsdl, certificadoAfip,*/ modo, forzarNuevoTicket);

                    objWSFEV1.Url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);

                    objFEAuthRequest.Token = ticket.Token;
                    objFEAuthRequest.Sign = ticket.Sign;
                    objFEAuthRequest.Cuit = comprobante.Cuit;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    FECAECabRequest objFECAECabRequest = new FECAECabRequest();
                    FECAERequest objFECAERequest = new FECAERequest();
                    FECAEResponse objFECAEResponse = new FECAEResponse();

                    objFECAECabRequest.CantReg = 1;
                    objFECAECabRequest.PtoVta = comprobante.PtoVta;
                    objFECAECabRequest.CbteTipo = (int)comprobante.TipoComprobante;

                    FECAEDetRequest[] objFECAEDetRequest = new FECAEDetRequest[1];
                    comprobante.NumeroComprobante = GetUltimoComprobante(comprobante.Cuit, comprobante.PtoVta, comprobante.TipoComprobante, objFEAuthRequest, objWSFEV1.Url) + 1;

                    objFECAEDetRequest[0] = new FECAEDetRequest();
                    objFECAEDetRequest[0].Concepto = (int)comprobante.Concepto;
                    objFECAEDetRequest[0].DocTipo = comprobante.DocTipo;
                    objFECAEDetRequest[0].DocNro = comprobante.DocNro;
                    objFECAEDetRequest[0].CbteDesde = comprobante.NumeroComprobante;
                    objFECAEDetRequest[0].CbteHasta = comprobante.NumeroComprobante;
                    objFECAEDetRequest[0].CbteFch = comprobante.Fecha.ToString("yyyyMMdd");
                    objFECAEDetRequest[0].ImpTotal = Math.Round(comprobante.ImpTotal, 2);
                    objFECAEDetRequest[0].ImpTotConc = Math.Round(comprobante.ImpTotConc, 2);
                    objFECAEDetRequest[0].ImpNeto = Math.Round(comprobante.ImpNeto, 2);
                    objFECAEDetRequest[0].ImpOpEx = Math.Round(comprobante.ImpOpEx, 2);
                    objFECAEDetRequest[0].ImpTrib = Math.Round(comprobante.Tributos.Sum(t => t.Importe), 2);
                    objFECAEDetRequest[0].ImpIVA = Math.Round(comprobante.DetalleIva.Sum(i => i.Importe), 2);
                    objFECAEDetRequest[0].FchServDesde = comprobante.FchServDesde == null ? "" : comprobante.FchServDesde.Value.ToString("yyyyMMdd");
                    objFECAEDetRequest[0].FchServHasta = comprobante.FchServHasta == null ? "" : comprobante.FchServHasta.Value.ToString("yyyyMMdd");

                    if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_A_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_B_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_C_MiPyMEs
                        ) //Si el tipo de comprobante que está autorizando es MiPyMEs(FCE), el campo “fecha de vencimiento para el pago” < FchVtoPago > no debe informarse si NO es Factura de Crédito.En el caso de ser Débito o Crédito, solo puede informarse si es de Anulación.
                    {

                        objFECAEDetRequest[0].FchVtoPago = "";
                    }
                    else
                    {
                        objFECAEDetRequest[0].FchVtoPago = comprobante.FchVtoPago == null ? "" : comprobante.FchVtoPago.Value.ToString("yyyyMMdd");
                    }


                    objFECAEDetRequest[0].MonId = comprobante.CodigoMoneda;
                    objFECAEDetRequest[0].MonCotiz = comprobante.CotizacionMoneda;

                    if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_CREDITO_A ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_CREDITO_B ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_CREDITO_C ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_DEBITO_A ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_DEBITO_B ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTAS_DEBITO_C )
                    {
                        if (comprobante.NotaDeCreditoPorServicio)//Voy a enviar la nota de credito como SERVICIO para extender la fecha de vencimiento
                        {
                            objFECAEDetRequest[0].Concepto = 2;
                            //objFECAEDetRequest[0].FchServDesde = "";
                            //objFECAEDetRequest[0].FchServHasta = "";
                            //objFECAEDetRequest[0].FchVtoPago = "";
                        }

                        if (comprobante.ComprobantesAsociados.Count > 0)
                        {
                            objFECAEDetRequest[0].CbtesAsoc = new CbteAsoc[comprobante.ComprobantesAsociados.Count];

                            int regNo = 0;
                            foreach (FEComprobanteAsociado comAsoc in comprobante.ComprobantesAsociados)
                                objFECAEDetRequest[0].CbtesAsoc[regNo++] = new CbteAsoc()
                                {
                                    Tipo = comAsoc.TipoField,
                                    PtoVta = comAsoc.PtoVtaField,
                                    Nro = comAsoc.NroField
                                    //Cuit = comAsoc.CuitField,
                                    //CbteFch = comAsoc.CbteFchField
                                };
                        }
                    }

                    if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_A_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_B_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_C_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_A_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_B_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_C_MiPyMEs)
                    {
                        if(comprobante.NotaDeCreditoPorServicio)//Voy a enviar la nota de credito como SERVICIO para extender la fecha de vencimiento
                        {
                            objFECAEDetRequest[0].Concepto = 2;
                            //objFECAEDetRequest[0].FchServDesde = "";
                            //objFECAEDetRequest[0].FchServHasta = "";
                            //objFECAEDetRequest[0].FchVtoPago = "";
                        }

                        if (comprobante.ComprobantesAsociados.Count > 0)
                        {
                            objFECAEDetRequest[0].CbtesAsoc = new CbteAsoc[comprobante.ComprobantesAsociados.Count];

                            int regNo = 0;
                            foreach (FEComprobanteAsociado comAsoc in comprobante.ComprobantesAsociados)
                                objFECAEDetRequest[0].CbtesAsoc[regNo++] = new CbteAsoc()
                                {
                                    Tipo = comAsoc.TipoField,
                                    PtoVta = comAsoc.PtoVtaField,
                                    Nro = comAsoc.NroField,
                                    Cuit = comprobante.Cuit.ToString(),
                                    CbteFch = comAsoc.CbteFchField
                                };
                        }
                    }


                    if (comprobante.Actividades.Count > 0)
                    {
                        objFECAEDetRequest[0].Actividades = new Actividad[comprobante.Actividades.Count];

                        int regNo = 0;
                        foreach (FEActividad regActividad in comprobante.Actividades)
                            objFECAEDetRequest[0].Actividades[regNo++] = new Actividad() { Id = regActividad.Id };

                    }

                    if ((int)comprobante.TipoComprobante == (int)FETipoComprobante.FACTURAS_A_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.FACTURAS_B_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.FACTURAS_C_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_A_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_B_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_CREDITO_C_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_A_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_B_MiPyMEs ||
                        (int)comprobante.TipoComprobante == (int)FETipoComprobante.NOTA_DEBITO_C_MiPyMEs)
                    {
                        if (comprobante.Opcionales.Count > 0)
                        {
                            objFECAEDetRequest[0].Opcionales = new Opcional[comprobante.Opcionales.Count];

                            int regNo = 0;
                            foreach (FEOpcional regOp in comprobante.Opcionales)
                                objFECAEDetRequest[0].Opcionales[regNo++] = new Opcional()
                                {
                                    Id = regOp.Id,
                                    Valor = regOp.Valor
                                };
                        }
                    }

                    if (comprobante.DetalleIva.Count > 0)
                    {
                        objFECAEDetRequest[0].Iva = new AlicIva[comprobante.DetalleIva.Count];

                        int regNo = 0;
                        foreach (FERegistroIVA regIva in comprobante.DetalleIva)
                            objFECAEDetRequest[0].Iva[regNo++] = new AlicIva() { BaseImp = regIva.BaseImp, Importe = regIva.Importe, Id = (int)regIva.TipoIva };
                    }

                    if (comprobante.Tributos.Count > 0)
                    {
                        objFECAEDetRequest[0].Tributos = new Tributo[comprobante.Tributos.Count];

                        int regNo = 0;
                        foreach (FERegistroTributo regTributo in comprobante.Tributos)
                            objFECAEDetRequest[0].Tributos[regNo++] = new Tributo() { BaseImp = regTributo.BaseImp, Importe = regTributo.Importe, Id = (short)regTributo.Tipo };
                    }
                    
                    objFECAERequest.FeCabReq = objFECAECabRequest;
                    objFECAERequest.FeDetReq = objFECAEDetRequest;

                    //Invoco al método FECAEARegInformativo
                    try
                    {
                        objFECAEResponse = objWSFEV1.FECAESolicitar(objFEAuthRequest, objFECAERequest);

                        if (bool.Parse(ConfigurationManager.AppSettings["FE.LoguearWS"]))
                        {
                            string xmlCAERequest, xmlAuthRequest, xmlCAEResponse;
                            StringWriter textWriter = new StringWriter();
                            XmlSerializer xml;

                            xml = new XmlSerializer(objFECAERequest.GetType());
                            xml.Serialize(textWriter, objFECAERequest);
                            xmlCAERequest = textWriter.ToString();

                            xml = new XmlSerializer(objFEAuthRequest.GetType());
                            xml.Serialize(textWriter, objFEAuthRequest);
                            xmlAuthRequest = textWriter.ToString();

                            xml = new XmlSerializer(objFECAEResponse.GetType());
                            xml.Serialize(textWriter, objFECAEResponse);
                            xmlCAEResponse = textWriter.ToString();

                            GetArchivoXML(xmlCAERequest, xmlAuthRequest, xmlCAEResponse);
                        }

                        if (objFECAEResponse != null && objFECAEResponse.FeDetResp != null && !string.IsNullOrEmpty(objFECAEResponse.FeDetResp[0].CAE))
                        {
                            comprobante.CAE = objFECAEResponse.FeDetResp[0].CAE;
                            comprobante.FechaVencCAE = DateTime.ParseExact(objFECAEResponse.FeDetResp[0].CAEFchVto, "yyyyMMdd", null);
                            comprobante.ArchivoFactura = GetStreamPDF(comprobante, templateFc, pathLogo, "");
                        }

                        if (objFECAEResponse.Errors != null)
                        {
                            var auxMsg = objFECAEResponse.Errors[0].Msg.ToString();
                            auxMsg = auxMsg.Replace("Ãº", "ú").Replace("Ã³", "ó").Replace("Ã©", "é");
                            var CodigoError = "Error FE: " + objFECAEResponse.Errors[0].Code;

                            if (objFECAEResponse.Errors[0].Code == 500 || objFECAEResponse.Errors[0].Code == 501 || objFECAEResponse.Errors[0].Code == 502 || objFECAEResponse.Errors[0].Code == 600 || objFECAEResponse.Errors[0].Code == 601 || objFECAEResponse.Errors[0].Code == 602)
                                throw new Exception("###Error### " + objFECAEResponse.Errors[0].Code + " - " + auxMsg);
                            else
                                throw new Exception("Error: " + objFECAEResponse.Errors[0].Code + " - " + auxMsg);

                            //BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), CodigoError, auxMsg);
                        }
                        else if (objFECAEResponse != null && objFECAEResponse.FeDetResp != null && string.IsNullOrEmpty(objFECAEResponse.FeDetResp[0].CAE))
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (Obs obs in objFECAEResponse.FeDetResp[0].Observaciones)
                                sb.AppendLine(obs.Msg);

                            var auxMsg = sb.ToString();
                            auxMsg = auxMsg.Replace("Ãº", "ú").Replace("Ã³", "ó").Replace("Ã©", "é");

                            throw new Exception("No se pudo obtener el CAE, revise los datos cargados.\n" + auxMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        private int GetUltimoComprobante(long cuit, int ptoVta, FETipoComprobante tipoComprobante, FEAuthRequest ticket, string url)
        {
            Service objWSFE = new Service();

            objWSFE.Url = url;// ConfigurationManager.AppSettings["FE.wsfev1"];
            FERecuperaLastCbteResponse ultimo = objWSFE.FECompUltimoAutorizado(ticket, ptoVta, (int)tipoComprobante);
            if (ultimo.Errors != null && ultimo.Errors.Any())
                throw new Exception(ultimo.Errors[0].Msg);
            return ultimo.CbteNro;
        }

        public void GrabarEnDisco(FEComprobante comprobante, string archivoDestino, string templateFc, string domicilioTransporteCliente)
        {
            GrabarEnDisco(comprobante, archivoDestino, templateFc, null, domicilioTransporteCliente);
        }

        public void GrabarEnDisco(FEComprobante comprobante, string archivoDestino, string templateFc, string pathLogo, string domicilioTransporteCliente)
        {
            Stream streamPDF = GetStreamPDF(comprobante, templateFc, pathLogo, domicilioTransporteCliente);

            using (Stream destination = File.Create(archivoDestino))
            {
                for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                    destination.WriteByte((byte)a);
            }
        }

        public void GrabarEnDiscoSinLogo(FEComprobante comprobante, string archivoDestino, string templateFc, string pathLogo, int margenHorizontal, int margenVertical)
        {
            Stream streamPDF = GetStreamPDFSinLogo(comprobante, templateFc, pathLogo, margenHorizontal, margenVertical);

            using (Stream destination = File.Create(archivoDestino))
            {
                for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                    destination.WriteByte((byte)a);
            }
        }

        public void GrabarEnDiscoTalonario(FEComprobante comprobante, string archivoDestino, string templateFc, 
                                string pathLogo, int margenHorizontal, int margenVertical, bool verTotal, string domicilioTransporteCliente)
        {
            Stream streamPDF = GetStreamPDFTalonario(comprobante, templateFc, pathLogo, margenHorizontal, margenVertical, verTotal, domicilioTransporteCliente);

            using (Stream destination = File.Create(archivoDestino))
            {
                for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                    destination.WriteByte((byte)a);
            }
        }

        public void GrabarEnDisco(Stream streamPDF, string archivoDestino, string templateFc, string pathLogo)
        {
            //Stream streamPDF = GetStreamPDF(comprobante, templateFc, pathLogo);

            using (Stream destination = File.Create(archivoDestino))
            {
                for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                    destination.WriteByte((byte)a);
            }
        }


        public void GrabarEnDiscoTicket(FEComprobante comprobante, string archivoDestino, string templateFc)
        {
            GrabarEnDiscoTicket(comprobante, archivoDestino, templateFc, null);
        }
        public void GrabarEnDiscoTicket(FEComprobante comprobante, string archivoDestino, string templateFc, string pathLogo)
        {
            Stream streamPDF = GetStreamPDFTicket(comprobante, templateFc, pathLogo);

            using (Stream destination = File.Create(archivoDestino))
            {
                for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                    destination.WriteByte((byte)a);
            }
        }
        public void GrabarEnDiscoTicket(Stream streamPDF, string archivoDestino, string templateFc, string pathLogo)
        {
            //Stream streamPDF = GetStreamPDF(comprobante, templateFc, pathLogo);

            using (Stream destination = File.Create(archivoDestino))
            {
                for (int a = streamPDF.ReadByte(); a != -1; a = streamPDF.ReadByte())
                    destination.WriteByte((byte)a);
            }
        }

        public void GrabarEnDiscoLiquidoProducto(List<FEComprobante> comprobante, string pathArchivoDestino, 
                                                string nombreArchivoDestino, string tipoImpresion,
                                                int xInicial, int yInicial)
        {
            GrabarEnDiscoLiquidoProducto(comprobante, pathArchivoDestino, nombreArchivoDestino, 
                                            tipoImpresion, null, xInicial, yInicial);
        }
        public void GrabarEnDiscoLiquidoProducto(List<FEComprobante> comprobante, string pathArchivoDestino, 
                                            string nombreArchivoDestino, string tipoImpresion, string pathLogo,
                                            int xInicial, int yInicial)
        {
            Stream streamPDF = GetStreamPDFLiquidoProducto(comprobante, tipoImpresion, pathLogo, xInicial, yInicial);

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

        public void ImprimirFacturaElectronicaToResponse(FEComprobante comprobante, string templateFc, string pathLogo)
        {
            FEFacturaElectronica fe = new FEFacturaElectronica();
            Stream stream = GetStreamPDF(comprobante, templateFc, pathLogo, "");
            HttpResponse Response = HttpContext.Current.Response;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
            }

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=Comprobante.pdf");
            Response.BinaryWrite(buffer);
            // myMemoryStream.WriteTo(Response.OutputStream); //works too
            Response.Flush();
            Response.Close();
            Response.End();
        }

        public Stream GetStreamPDF(FEComprobante comprobante, string templateFc, string pathLogo, string domicilioTransporteCliente)
        {
            string[] tipoComprobantesTodasMiPymes = { "FCAMP", "FCBMP", "FCCMP", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
            string[] tipoComprobantesFacturasMiPymes = { "FCAMP", "FCBMP", "FCCMP", };
            string[] tipoComprobantesNotasMiPymes = { "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };

            string letra = "";
            string comp = "";
            string codigo = "";
            bool esRecibo = comprobante.TipoComprobante == FETipoComprobante.RECIBO_A || comprobante.TipoComprobante == FETipoComprobante.RECIBO_B 
                                    || comprobante.TipoComprobante == FETipoComprobante.RECIBO_C 
                                    || comprobante.TipoComprobante == FETipoComprobante.SIN_DEFINIR_RECIBO
                                    || comprobante.TipoComprobante == FETipoComprobante.COBRANZA;
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
                case FETipoComprobante.FACTURAS_A_MiPyMEs:
                    letra = "A";
                    comp = "FACTURA MiPyMEs";
                    break;
                case FETipoComprobante.FACTURAS_B:
                    letra = "B";
                    comp = "FACTURA";
                    break;
                case FETipoComprobante.FACTURAS_B_MiPyMEs:
                    letra = "B";
                    comp = "FACTURA MiPyMEs";
                    break;
                case FETipoComprobante.FACTURAS_C:
                    letra = "C";
                    comp = "FACTURA";
                    break;
                case FETipoComprobante.FACTURAS_C_MiPyMEs:
                    letra = "C";
                    comp = "FACTURA MiPyMEs";
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
                case FETipoComprobante.NOTA_DEBITO_A_MiPyMEs:
                    letra = "A";
                    comp = "NOTA DE DÉBITO MiPyMEs";
                    break;
                case FETipoComprobante.NOTAS_DEBITO_B:
                    letra = "B";
                    comp = "NOTA DE DÉBITO";
                    break;
                case FETipoComprobante.NOTA_DEBITO_B_MiPyMEs:
                    letra = "B";
                    comp = "NOTA DE DÉBITO MiPyMEs";
                    break;
                case FETipoComprobante.NOTAS_DEBITO_C:
                    letra = "C";
                    comp = "NOTA DE DÉBITO";
                    break;
                case FETipoComprobante.NOTA_DEBITO_C_MiPyMEs:
                    letra = "C";
                    comp = "NOTA DE DÉBITO MiPyMEs";
                    break;
                case FETipoComprobante.NOTAS_DEBITO_E:
                    letra = "E";
                    comp = "NOTA DE DÉBITO";
                    break;

                case FETipoComprobante.NOTAS_CREDITO_A:
                    letra = "A";
                    comp = "NOTA DE CRÉDITO";
                    break;
                case FETipoComprobante.NOTA_CREDITO_A_MiPyMEs:
                    letra = "A";
                    comp = "NOTA DE CRÉDITO MiPyMEs";
                    break;
                case FETipoComprobante.NOTAS_CREDITO_B:
                    letra = "B";
                    comp = "NOTA DE CRÉDITO";
                    break;
                case FETipoComprobante.NOTA_CREDITO_B_MiPyMEs:
                    letra = "B";
                    comp = "NOTA DE CRÉDITO MiPyMEs";
                    break;
                case FETipoComprobante.NOTAS_CREDITO_C:
                    letra = "C";
                    comp = "NOTA DE CRÉDITO";
                    break;
                case FETipoComprobante.NOTA_CREDITO_C_MiPyMEs:
                    letra = "C";
                    comp = "NOTA DE CRÉDITO MiPyMEs";
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
                case FETipoComprobante.COBRANZA:
                    letra = "X";
                    comp = "Cobranza";
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
            if(comprobante.Tipo != null)
            {
                if (comprobante.Tipo.Equals("EDA") || comprobante.Tipo.Equals("PDV"))
                {

                    pdf.EscribirXY("Fecha Entrega: " + comprobante.FechaEntrega.ToString("dd/MM/yyyy"), 570, 105, 12, Alineado.Derecha);

                    if(comprobante.NroPedidoDeVenta != "" && comprobante.NroPresupuesto != "")
                        pdf.EscribirXY("PDV: " + comprobante.NroPedidoDeVenta + " - PRE: " + comprobante.NroPresupuesto, 570, 125, 10, Alineado.Derecha);
                    if (comprobante.NroPedidoDeVenta != "" && comprobante.NroPresupuesto == "")
                        pdf.EscribirXY("PDV: " + comprobante.NroPedidoDeVenta, 570, 125, 10, Alineado.Derecha);
                    if (comprobante.NroPedidoDeVenta == "" && comprobante.NroPresupuesto != "")
                        pdf.EscribirXY("PRE: " + comprobante.NroPresupuesto, 570, 125, 10, Alineado.Derecha);


                    //pdf.EscribirBoxXY(comprobante.Observaciones, 27, 715, 10, 400);
                }
                else
                    pdf.EscribirXY("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy"), 570, 105, 12, Alineado.Derecha);

                if (comprobante.Tipo == "NCA" || comprobante.Tipo == "NCB" || comprobante.Tipo == "NCC" 
                    || comprobante.Tipo == "NDA" || comprobante.Tipo == "NDB" || comprobante.Tipo == "NDC"
                    || comprobante.Tipo == "NCAMP" || comprobante.Tipo == "NCBMP" || comprobante.Tipo == "NCCMP"
                    || comprobante.Tipo == "NDAMP" || comprobante.Tipo == "NDBMP" || comprobante.Tipo == "NDCMP")
                {
                    if(!esRemito)
                        pdf.EscribirXY("Factura de origen: " + "Nº: " + comprobante.ComprobantesAsociados[0].PtoVtaField.ToString().PadLeft(4, '0') + "-" + comprobante.ComprobantesAsociados[0].NroField.ToString().PadLeft(8, '0'), 570, 125, 10, Alineado.Derecha);
                }

                if (tipoComprobantesFacturasMiPymes.Contains(comprobante.Tipo))
                {
                    pdf.EscribirXY("CBU del emisor: " + comprobante.Opcionales[0].Valor, 570, 125, 10, Alineado.Derecha);
                }

            }
            else
            {
                pdf.EscribirXY("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy"), 570, 105, 12, Alineado.Derecha);
            }
           

#region Datos de la empresa facturante

            if (!string.IsNullOrEmpty(pathLogo))
            {
                pdf.InsertarImagenXYConTransparencia(pathLogo, 27, 750, 80);
                pdf.EscribirXY(comprobante.RazonSocial, 27, 110, 10, Alineado.Izquierda);
            }
            else
                pdf.EscribirXY(comprobante.RazonSocial, 27, 70, 22, Alineado.Izquierda);

            pdf.EscribirXY(comprobante.Domicilio, 27, 125, 10, Alineado.Izquierda);
            pdf.EscribirXY(comprobante.CiudadProvincia, 27, 140, 10, Alineado.Izquierda);
            pdf.EscribirXY("Tel.: " + comprobante.Telefono + (!string.IsNullOrEmpty(comprobante.Celular) ? " - Cel.: " + comprobante.Celular : ""), 27, 155, 10, Alineado.Izquierda);
            pdf.EscribirXY(comprobante.CondicionIva, 27, 170, 10, Alineado.Izquierda);

            pdf.EscribirXY("CUIT: " + comprobante.Cuit, 570, 140, 10, Alineado.Derecha);
            pdf.EscribirXY("Ingresos brutos: " + comprobante.IIBB, 570, 155, 10, Alineado.Derecha);
            pdf.EscribirXY("Inicio de actividades: " + comprobante.FechaInicioActividades, 570, 170, 10, Alineado.Derecha);

            #endregion

            #region Vendedor
            
            if (comprobante.Vendedor != null)
                if (!comprobante.Vendedor.Equals(""))
                    pdf.EscribirXY("Vendedor: " + comprobante.Vendedor, 27, 190, 10, Alineado.Izquierda);

            #endregion

            #region Datos del cliente

            var tipoDoc = (comprobante.DocTipo == 96) ? "DNI" : "CUIT";
            var NroDeDoc = (comprobante.DocNro != 0) ? comprobante.DocNro.ToString() : "";
            pdf.EscribirXY("Señor/es: " + comprobante.ClienteNombre, 27, 205, 10, Alineado.Izquierda);
            pdf.EscribirXY("Domicilio: " + comprobante.ClienteDomicilio, 27, 220, 10, Alineado.Izquierda);
            pdf.EscribirXY("Localidad: " + comprobante.ClienteLocalidad, 27, 235, 10, Alineado.Izquierda);
            pdf.EscribirXY(comprobante.ClienteContacto, 331, 250, 10, Alineado.Izquierda);
            if (comprobante.DocTipo != 99)
                pdf.EscribirXY(tipoDoc + ": " + NroDeDoc, 331, 220, 10, Alineado.Izquierda);

            pdf.EscribirXY("Condición de IVA: " + comprobante.ClienteCondiionIva, 331, 235, 10, Alineado.Izquierda);

            if (tipoComprobantesFacturasMiPymes.Contains(comprobante.Tipo))
            {
                pdf.EscribirXY("Forma de pago: " + comprobante.Opcionales[1].Valor, 27, 250, 10, Alineado.Izquierda);
            }
            else
            {
                if (!esRecibo)
                    pdf.EscribirXY("Condición de venta: " + comprobante.CondicionVenta, 27, 250, 10, Alineado.Izquierda);
            }


            #endregion

            if (!string.IsNullOrEmpty(comprobante.TextoFinalFactura))
            {
                pdf.EscribirBoxXY(comprobante.TextoFinalFactura, 27, 650, 10, 400);
            }

            #region Datos generales
            if (!esRemito)
            {
                pdf.EscribirBoxXY("Son Pesos " + NumeroALetrasMoneda(comprobante.ImpTotal) + ".", 27, 680, 10, 400);
                pdf.EscribirBoxXY(comprobante.Observaciones, 140, 715, 10, 400);

                if(letra == "A" && comp == "FACTURA" && comprobante.ClienteCondiionIva.ToUpper() == "MONOTRIBUTO")
                {
                    pdf.EscribirBoxXY("'El crédito fiscal discriminado en el presente comprobante, sólo  ", 140, 730, 10, 400);
                    pdf.EscribirBoxXY(" podrá ser computado a efectos del Régimen de Sostenimiento e ", 140, 743, 10, 400);
                    pdf.EscribirBoxXY(" Inclusión Fiscal para  Pequeños Contribuyentes de la Ley Nº 27.618.'", 140, 757, 10, 400);
                }

            }
            else
            {
                if (domicilioTransporteCliente != "")
                {
                    int yInicial = 680;
                    pdf.EscribirXY("Transporte:", 27, yInicial, 10, Alineado.Izquierda);
                    string[] transporteCliente = domicilioTransporteCliente.Split('|');
                    for (int i = 0; i < transporteCliente.Length; i++)
                    {
                        yInicial += 12;
                        pdf.EscribirXY(transporteCliente[i], 27, yInicial, 10, Alineado.Izquierda);
                    }
                }
            }
            int totalesY = 680;

            if (letra == "A" && !esRecibo && !esRemito)
            {
                pdf.EscribirXY("Gravado: $" + Math.Abs(comprobante.ImpNeto).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
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
                if (letra == "B" && !esRecibo)
                {
                    pdf.EscribirXY("Subtotal: $" + Math.Abs(comprobante.ImpNeto).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                    totalesY += 15;
                }

                foreach (var item in comprobante.Tributos)
                {
                    if (item.Importe != 0)
                    {
                        pdf.EscribirXY(item.Decripcion + " " + item.Alicuota + "%: $" + Math.Abs(item.Importe).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                        totalesY += 15;
                    }
                }

                if (comprobante.ImpOpEx > 0)
                {
                    pdf.EscribirXY("Exento: $" + Math.Abs(comprobante.ImpOpEx).ToString("N2"), 570, totalesY, 10, Alineado.Derecha);
                    totalesY += 10;
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

                    pdf.EscribirXY("CAE: " + comprobante.CAE, 140, 775, 10, Alineado.Izquierda);
                    pdf.EscribirXY("Vencimiento CAE: " + comprobante.FechaVencCAE.ToString("dd/MM/yyyy"), 140, 790, 10, Alineado.Izquierda);
                    pdf.InsertarImagenXY(ImagenQR(comprobante), 27, 40, 40);
                }
                
            }
            /*int i = 0;
            double itemPrecio;*/
            int fontDetalleFE = 10;

            if (ConfigurationManager.AppSettings["FE.FontDetalle"] != null) fontDetalleFE = int.Parse(ConfigurationManager.AppSettings["FE.FontDetalle"]);

            List<FETipoComprobante> tipoRemitos = new List<FETipoComprobante>() { FETipoComprobante.RECIBO_A, FETipoComprobante.RECIBO_B, FETipoComprobante.RECIBO_C, FETipoComprobante.SIN_DEFINIR_RECIBO, FETipoComprobante.COBRANZA };

            //if (tipoRemitos.Contains(comprobante.TipoComprobante))
            //{
            //    if(comprobante.ConceptoRecibo != null)
            //    {
            //        //pdf.EscribirBoxXY(comprobante.ConceptoRecibo, 27, 280, 10, 540);
            //        totalesY = 705;
            //        int i = 0;
            //        foreach (var concepto in comprobante.ConceptoRecibo.Split('|'))
            //        {
            //            pdf.EscribirBoxXY(concepto, 28, totalesY, 10, 540);
            //            totalesY += (i == 0 ? 15 : 7);
            //            i++;
            //        }
            //    }
            //}            

            if (esRemito)
                pdf.InsertarTablaDetalleRemito(comprobante.ItemsDetalle, 0, 0);
            else
            {
                if(comprobante.TipoComprobante == FETipoComprobante.COBRANZA)
                {
                    int y = 280;
                    pdf.EscribirXY("Fecha", 40, y, 10, Alineado.Izquierda);
                    pdf.EscribirXY("Detalle", 120, y, 10, Alineado.Izquierda);
                    pdf.EscribirXY("Importe", 500, y, 10, Alineado.Izquierda);      
                    pdf.InsertarTablaDetalleCobranza(comprobante, 0, -5);
                    y += 100;
                    pdf.EscribirXY("------------------------------------------------------------------------------------------------------------------------------------------------------------------", 27, y, 10, Alineado.Izquierda);
                    y += 10;
                    pdf.EscribirXY("Forma de Pago: ", 40, y, 10, Alineado.Izquierda);
                    y -= 500;
                    pdf.InsertarTablaDetalleCobranzaFormasDePago(comprobante, 0, y);
                }
                else
                {
                    pdf.InsertarTablaDetalle(comprobante, 0, 0);
                }                
            }
                


            pdf.EscribirXY("ID: " + comprobante.IDComprobante, 500, 830, 10, Alineado.Izquierda);
            #endregion

            return pdf.GenerarPDFStream();
        }

        public Stream GetStreamPDFSinLogo(FEComprobante comprobante, string templateFc, string pathLogom, int MargenHorizonal, int MargenVertical)
        {

            int xInicial = MargenHorizonal;
            int yInicial = MargenVertical;

            string letra = "";
            string comp = "";
            string codigo = "";
            bool esRecibo = comprobante.TipoComprobante == FETipoComprobante.RECIBO_A || comprobante.TipoComprobante == FETipoComprobante.RECIBO_B || comprobante.TipoComprobante == FETipoComprobante.RECIBO_C || comprobante.TipoComprobante == FETipoComprobante.SIN_DEFINIR_RECIBO;
            bool esRemito = comprobante.TipoComprobante == FETipoComprobante.REMITO;

            var template = (templateFc == string.Empty ? ConfigurationManager.AppSettings["FE.Template"] : templateFc);
            NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, null);

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

            xInicial += 302;
            yInicial += 50;
            pdf.EscribirXY(letra, xInicial, yInicial, 22, Alineado.Centro, NFPDFColor.WHITE);


            if (codigo != "")
            {
                pdf.EscribirXY("Cod. " + codigo, xInicial, yInicial + 15, 10, Alineado.Centro, NFPDFColor.WHITE);
            }

            xInicial += 268;
            yInicial -= 5; 
            pdf.EscribirXY(comp, xInicial, yInicial, 12, Alineado.Derecha);
            //if (!esRecibo) 
            //{
            //    pdf.EscribirXY(comprobante.Original ? "Original" : "Duplicado", 570, 60, 10, Alineado.Derecha);

            //}
            //else
            //{
            //    pdf.EscribirXY("Nº: " + comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), 570, 90, 15, Alineado.Derecha);

            //}

            yInicial += 15;
            pdf.EscribirXY(comprobante.Original ? "Original" : "Duplicado", xInicial, yInicial, 10, Alineado.Derecha);


            if (letra == "X")
            {
                pdf.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), xInicial, yInicial + 30, 15, Alineado.Derecha);
            }
            else
            {
                pdf.EscribirXY("Nº: " + comprobante.PtoVta.ToString().PadLeft(4, '0') + "-" + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), xInicial, yInicial + 30, 15, Alineado.Derecha);
            }

            //pdf.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), 570, 90, 15, Alineado.Derecha);
            if (comprobante.Tipo != null)
            {
                if (comprobante.Tipo.Equals("EDA") || comprobante.Tipo.Equals("PDV"))
                {

                    pdf.EscribirXY("Fecha Entrega: " + comprobante.FechaEntrega.ToString("dd/MM/yyyy"), xInicial, yInicial + 45, 12, Alineado.Derecha);

                    
                    if (comprobante.NroPedidoDeVenta != "" && comprobante.NroPresupuesto != "")
                        pdf.EscribirXY("PDV: " + comprobante.NroPedidoDeVenta + " - PRE: " + comprobante.NroPresupuesto, xInicial, yInicial + 75, 10, Alineado.Derecha);
                    if (comprobante.NroPedidoDeVenta != "" && comprobante.NroPresupuesto == "")
                        pdf.EscribirXY("PDV: " + comprobante.NroPedidoDeVenta, xInicial, yInicial + 75, 10, Alineado.Derecha);
                    if (comprobante.NroPedidoDeVenta == "" && comprobante.NroPresupuesto != "")
                        pdf.EscribirXY("PRE: " + comprobante.NroPresupuesto, xInicial, yInicial + 75, 10, Alineado.Derecha);


                    //pdf.EscribirBoxXY(comprobante.Observaciones, 27, 715, 10, 400);
                }
                else
                    pdf.EscribirXY("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy"), xInicial, yInicial + 45, 12, Alineado.Derecha);

                
                if (comprobante.Tipo == "NCA" || comprobante.Tipo == "NCB" || comprobante.Tipo == "NCC" ||
                    comprobante.Tipo == "NDA" || comprobante.Tipo == "NDB" || comprobante.Tipo == "NDC")
                {
                    pdf.EscribirXY("Factura de origen: " + "Nº: " + comprobante.ComprobantesAsociados[0].PtoVtaField.ToString().PadLeft(4, '0') + "-" + comprobante.ComprobantesAsociados[0].NroField.ToString().PadLeft(8, '0'), xInicial, yInicial + 105, 10, Alineado.Derecha);
                }

            }
            else
            {
                pdf.EscribirXY("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy"), xInicial, yInicial + 45, 12, Alineado.Derecha);
            }


            #region Datos de la empresa facturante

            xInicial -= 543;
            yInicial += 65;
            pdf.EscribirXY(comprobante.Domicilio, xInicial, yInicial, 10, Alineado.Izquierda);

            yInicial += 15;
            pdf.EscribirXY(comprobante.CiudadProvincia, xInicial, yInicial, 10, Alineado.Izquierda);

            yInicial += 15; 
            pdf.EscribirXY("Tel.: " + comprobante.Telefono + (!string.IsNullOrEmpty(comprobante.Celular) ? " - Cel.: " + comprobante.Celular : ""), xInicial, yInicial, 10, Alineado.Izquierda);

            yInicial += 15;
            pdf.EscribirXY(comprobante.CondicionIva, xInicial, yInicial, 10, Alineado.Izquierda);


            xInicial += 543;
            yInicial -= 30;
            pdf.EscribirXY("CUIT: " + comprobante.Cuit, xInicial, yInicial, 10, Alineado.Derecha);
            yInicial += 15;
            pdf.EscribirXY("Ingresos brutos: " + comprobante.IIBB, xInicial, yInicial, 10, Alineado.Derecha);
            yInicial += 15;
            pdf.EscribirXY("Inicio de actividades: " + comprobante.FechaInicioActividades, xInicial, yInicial, 10, Alineado.Derecha);

            #endregion

            #region Vendedor            
           
            if (comprobante.Vendedor != null)
                if (!comprobante.Vendedor.Equals(""))
                    pdf.EscribirXY("Vendedor: " + comprobante.Vendedor, xInicial - 543, yInicial + 20, 10, Alineado.Izquierda);

            #endregion

            #region Datos del cliente

            
            var tipoDoc = (comprobante.DocTipo == 96) ? "DNI" : "CUIT";
            var NroDeDoc = (comprobante.DocNro != 0) ? comprobante.DocNro.ToString() : "";

            xInicial -= 543;
            yInicial += 35;
            pdf.EscribirXY("Señor/es: " + comprobante.ClienteNombre, xInicial, yInicial, 10, Alineado.Izquierda);
            yInicial += 15;
            pdf.EscribirXY("Domicilio: " + comprobante.ClienteDomicilio, xInicial, yInicial, 10, Alineado.Izquierda);
            yInicial += 15;
            pdf.EscribirXY("Localidad: " + comprobante.ClienteLocalidad, xInicial, yInicial, 10, Alineado.Izquierda);

            xInicial += 304;
            yInicial += 15;
            pdf.EscribirXY(comprobante.ClienteContacto, xInicial, yInicial, 10, Alineado.Izquierda);

            if (comprobante.DocTipo != 99)
                pdf.EscribirXY(tipoDoc + ": " + NroDeDoc, xInicial, yInicial - 30, 10, Alineado.Izquierda);

            yInicial -= 15;
            pdf.EscribirXY("Condición de IVA: " + comprobante.ClienteCondiionIva, xInicial, yInicial, 10, Alineado.Izquierda);

            if (!esRecibo)
                pdf.EscribirXY("Condición de venta: " + comprobante.CondicionVenta, xInicial - 304, yInicial + 15, 10, Alineado.Izquierda);


            #endregion

            #region Datos generales
            //if (!esRemito)
            //{
            //    pdf.EscribirBoxXY("Son Pesos " + NumeroALetrasMoneda(comprobante.ImpTotal) + ".", xInicial - 304, yInicial + 445, 10, 400);

            //    pdf.EscribirBoxXY(comprobante.Observaciones, xInicial - 191, yInicial + 480, 10, 400);


            //    if (letra == "A" && comp == "FACTURA" && comprobante.ClienteCondiionIva.ToUpper() == "MONOTRIBUTO")
            //    {
            //        pdf.EscribirBoxXY("'El crédito fiscal discriminado en el presente comprobante, sólo  ", xInicial - 191, yInicial + 495, 10, 400);
            //        pdf.EscribirBoxXY(" podrá ser computado a efectos del Régimen de Sostenimiento e ", xInicial - 191, yInicial + 508, 10, 400);
            //        pdf.EscribirBoxXY(" Inclusión Fiscal para  Pequeños Contribuyentes de la Ley Nº 27.618.'", xInicial - 191, yInicial + 522, 10, 400);
            //    }

            //}

            int totalesY = yInicial + 445;

            //pdf.EscribirXY("Subtotal: $" + Math.Abs(comprobante.ImpNeto).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //totalesY += 15;

            //if (comprobante.DescuentoPorcentaje != null && comprobante.DescuentoImporte != null)
            //{
            //    pdf.EscribirXY("Descuento " + comprobante.DescuentoPorcentaje.Value.ToString("N2") + "%: $" + Math.Abs(comprobante.DescuentoImporte.Value).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //    totalesY += 15;
            //}

            //foreach (FERegistroIVA rIVA in comprobante.DetalleIva)
            //{
            //    if (rIVA.Importe != 0)
            //    {
            //        switch (rIVA.TipoIva)
            //        {
            //            case FETipoIva.Iva0:
            //                pdf.EscribirXY("IVA 0%: $" + Math.Abs(rIVA.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //                totalesY += 15;
            //                break;

            //            case FETipoIva.Iva10_5:
            //                pdf.EscribirXY("IVA 10.5%: $" + Math.Abs(rIVA.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //                totalesY += 15;
            //                break;

            //            case FETipoIva.Iva21:
            //                pdf.EscribirXY("IVA 21%: $" + Math.Abs(rIVA.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //                totalesY += 15;
            //                break;

            //            case FETipoIva.Iva27:
            //                pdf.EscribirXY("IVA 27%: $" + Math.Abs(rIVA.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //                totalesY += 15;
            //                break;

            //            case FETipoIva.Iva5:
            //                pdf.EscribirXY("IVA 5%: $" + Math.Abs(rIVA.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //                totalesY += 15;
            //                break;
            //            case FETipoIva.Iva2_5:
            //                pdf.EscribirXY("IVA 2.5%: $" + Math.Abs(rIVA.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //                totalesY += 15;
            //                break;
            //        }
            //    }
            //}
            

            //if (!esRemito)
            //{
            //    foreach (var item in comprobante.Tributos)
            //    {
            //        if (item.Importe != 0)
            //        {
            //            pdf.EscribirXY(item.Decripcion + " " + item.Alicuota + "%: $" + Math.Abs(item.Importe).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
            //            totalesY += 15;
            //        }
            //    }

                //if (comprobante.ImpOpEx > 0)
                //{
                //    pdf.EscribirXY("Exento: $" + Math.Abs(comprobante.ImpOpEx).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
                //    totalesY += 10;
                //}

                //if (comprobante.ImpTotConc > 0)
                //{
                //    pdf.EscribirXY("No Gravado: $" + Math.Abs(comprobante.ImpTotConc).ToString("N2"), xInicial + 239, totalesY, 10, Alineado.Derecha);
                //    totalesY += 15;
                //}

                if(comprobante.CAE != null)
                    pdf.EscribirXY("Total: $" + Math.Abs(comprobante.ImpTotal).ToString("N2"), xInicial + 239, totalesY, 15, Alineado.Derecha);
                else
                    pdf.EscribirXY("Total: $" + Math.Abs(comprobante.ImpNeto).ToString("N2"), xInicial + 239, totalesY, 15, Alineado.Derecha);

            //}


            //if (!esRemito)
            //{
            //    if (comprobante.TipoComprobante != FETipoComprobante.SIN_DEFINIR &&
            //        comprobante.TipoComprobante != FETipoComprobante.SIN_DEFINIR_RECIBO &&
            //        !string.IsNullOrEmpty(comprobante.CAE))
            //    {
            //        //pdf.EscribirXY("CAE: " + comprobante.CAE, 27, 770, 8, Alineado.Izquierda);
            //        //pdf.EscribirXY("Vencimiento CAE: " + comprobante.FechaVencCAE.ToString("dd/MM/yyyy"), 150, 770, 8, Alineado.Izquierda);
            //        //pdf.InsertarImagenXY(ImagenCodigoBarras(comprobante), 27, 45);
            //        //pdf.EscribirXY(CodigoDeBarras(comprobante), 27, 808, 10, Alineado.Izquierda);

            //        pdf.EscribirXY("CAE: " + comprobante.CAE, xInicial - 191, yInicial + 540, 10, Alineado.Izquierda);
            //        pdf.EscribirXY("Vencimiento CAE: " + comprobante.FechaVencCAE.ToString("dd/MM/yyyy"), xInicial - 191, yInicial + 555, 10, Alineado.Izquierda);
            //        pdf.InsertarImagenXY(ImagenQR(comprobante), xInicial - 304, yInicial - 195, 40);
            //    }

            //}
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

            //if (esRemito)
            //    pdf.InsertarTablaDetalleRemito(comprobante.ItemsDetalle, MargenHorizonal, MargenVertical);
            //else
                pdf.InsertarTablaDetalle(comprobante, MargenHorizonal, MargenVertical);

            #endregion

            pdf.EscribirXY("ID: " + comprobante.IDComprobante, 500, 830, 10, Alineado.Izquierda);

            return pdf.GenerarPDFStream();
        }

        public Stream GetStreamPDFTalonario(FEComprobante comprobante, string templateFc, string pathLogom, int MargenHorizonal, int MargenVertical, bool verTotal, string domicilioTransporteCliente)
        {

            int xInicial = MargenHorizonal;
            int yInicial = MargenVertical;

            NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, null);

            xInicial += 550;
            yInicial += 70;
            pdf.EscribirXY(comprobante.Fecha.ToString("dd/MM/yyyy"), xInicial, yInicial, 12, Alineado.Derecha);


            xInicial -= 450;
            yInicial += 40;
            pdf.EscribirXY(comprobante.ClienteNombre, xInicial, yInicial, 10, Alineado.Izquierda);

            yInicial += 15;
            pdf.EscribirXY(comprobante.ClienteDomicilio + ", " + comprobante.ClienteLocalidad, xInicial, yInicial, 10, Alineado.Izquierda);

            xInicial += 450;
            yInicial += 15;
            var NroDeDoc = (comprobante.DocNro != 0) ? comprobante.DocNro.ToString() : "";
            pdf.EscribirXY(NroDeDoc, xInicial, yInicial, 10, Alineado.Derecha);

            xInicial -= 400;
            yInicial += 20;
            pdf.EscribirXY(comprobante.ClienteCondiionIva, xInicial, yInicial, 10, Alineado.Izquierda);

            //detalle
            xInicial -= 100;
            yInicial += 10;
            foreach (var item in comprobante.ItemsDetalle)
            {
                yInicial += 15;
                pdf.EscribirXY(item.Cantidad.ToString(), xInicial, yInicial, 9, Alineado.Centro);
                pdf.EscribirXY(item.Codigo, xInicial + 35, yInicial, 9, Alineado.Izquierda);
                pdf.EscribirXY(item.Descripcion, xInicial + 100, yInicial, 9, Alineado.Izquierda);
            }

            //pdf.InsertarTablaDetalleRemitoTalonario(comprobante, MargenHorizonal, MargenVertical);
                        
            yInicial += 240;
            if (domicilioTransporteCliente != "")
            {
                pdf.EscribirXY("Transporte:", xInicial - 25, yInicial, 10, Alineado.Izquierda);                
                string[] transporteCliente = domicilioTransporteCliente.Split('|');
                for (int i = 0; i < transporteCliente.Length; i++)
                {
                    yInicial += 12;
                    pdf.EscribirXY(transporteCliente[i], xInicial - 25, yInicial, 10, Alineado.Izquierda);
                }
            }           

            if (verTotal)
            {
                xInicial += 500;
                pdf.EscribirXY(Math.Abs(comprobante.ImpTotal).ToString("N2"), xInicial, yInicial, 10, Alineado.Derecha);
            }

            pdf.EscribirXY("ID: " + comprobante.IDComprobante, 500, 830, 10, Alineado.Izquierda);

            return pdf.GenerarPDFStream();
        }

        public Stream GetStreamPDFTicket(FEComprobante comprobante, string templateFc, string pathLogo)
        {

            int x = 10;
            int y = 0;
            string[] pdfs = new string[3];



            //################################################################################################################
            //################################################################################################################
            //Cliente Cotizacion            
            NFPDFWriter pdf1 = new NFPDFWriter(MedidasDocumento.Ticket);       

            y = y + 40;
            pdf1.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 15;
            pdf1.EscribirXY(comprobante.ClienteNombre.ToString(), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 20;
            pdf1.EscribirXY("Cotización", x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);
      
            y = y + 8;
            pdf1.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 10;
            pdf1.EscribirXY("Detalle", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 8;
            pdf1.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            foreach (FEItemDetalle item in comprobante.ItemsDetalle)
            {
                y = y + 10;
                pdf1.EscribirXY(item.Cantidad.ToString() + " x " + item.Precio.ToString("N2"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                y = y + 8;
                //pdf1.EscribirXY(item.Codigo, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                //pdf1.EscribirXY(item.Descripcion, x + 30, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                if (item.Descripcion.Length > 20)
                    pdf1.EscribirXY(item.Descripcion.Substring(0, 20), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                else
                    pdf1.EscribirXY(item.Descripcion, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                pdf1.EscribirXY((item.Precio * item.Cantidad).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            }

            y = y + 8;
            pdf1.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            y = y + 8;
            pdf1.EscribirXY("Total: $" + Math.Abs(comprobante.ImpTotal).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            y = y + 8;
            pdf1.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            pdf1.FileName = "P1_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
            pdf1.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/comprobantes"));
            pdfs[0] = HttpContext.Current.Server.MapPath("~/files/comprobantes/") + pdf1.FileName;


            //################################################################################################################
            //################################################################################################################
            // Cliente vale retiro
            x = 10;
            y = 0;
            NFPDFWriter pdf2 = new NFPDFWriter(MedidasDocumento.Ticket);

            y = y + 40;
            pdf2.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 15;
            pdf2.EscribirXY(comprobante.ClienteNombre.ToString(), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 20;
            pdf2.EscribirXY("Vale de Retiro", x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 8;
            pdf2.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 10;
            pdf2.EscribirXY("Detalle", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 8;
            pdf2.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            foreach (FEItemDetalle item in comprobante.ItemsDetalle)
            {
                y = y + 10;
                pdf2.EscribirXY(item.Cantidad.ToString() + " x " + item.Precio.ToString("N2"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                y = y + 8;
                //pdf2.EscribirXY(item.Codigo, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                //pdf2.EscribirXY(item.Descripcion, x + 30, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                if (item.Descripcion.Length > 20)
                    pdf2.EscribirXY(item.Descripcion.Substring(0, 20), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                else
                    pdf2.EscribirXY(item.Descripcion, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                pdf2.EscribirXY((item.Precio * item.Cantidad).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            }

            y = y + 8;
            pdf2.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);


            pdf2.FileName = "P2_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
            pdf2.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/comprobantes"));
            pdfs[1] = HttpContext.Current.Server.MapPath("~/files/comprobantes/") + pdf2.FileName;


            //################################################################################################################
            //################################################################################################################
            //Puesto vale entrega
            x = 10;
            y = 0;
            NFPDFWriter pdf3 = new NFPDFWriter(MedidasDocumento.Ticket);

            y = y + 40;
            pdf3.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 15;
            pdf3.EscribirXY(comprobante.ClienteNombre.ToString(), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 20;
            pdf3.EscribirXY("Vale de Entrega", x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 8;
            pdf3.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 10;
            pdf3.EscribirXY("Detalle", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            y = y + 8;
            pdf3.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            foreach (FEItemDetalle item in comprobante.ItemsDetalle)
            {
                y = y + 10;
                pdf3.EscribirXY(item.Cantidad.ToString() + " x " + item.Precio.ToString("N2"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                y = y + 8;
                //pdf3.EscribirXY(item.Codigo, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                //pdf3.EscribirXY(item.Descripcion, x + 30, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                if (item.Descripcion.Length > 20)
                    pdf3.EscribirXY(item.Descripcion.Substring(0, 20), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                else
                    pdf3.EscribirXY(item.Descripcion, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                pdf3.EscribirXY((item.Precio * item.Cantidad).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            }

            y = y + 8;
            pdf3.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            y = y + 8;
            pdf3.EscribirXY("Total: $" + Math.Abs(comprobante.ImpTotal).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            y = y + 8;
            pdf3.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            pdf3.FileName = "P3_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
            pdf3.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/comprobantes"));
            pdfs[2] = HttpContext.Current.Server.MapPath("~/files/comprobantes/") + pdf3.FileName;


            //################################################################################################################
            //################################################################################################################
            //################################################################################################################
            //################################################################################################################

            string destino = HttpContext.Current.Server.MapPath("~/files/comprobantes/") + (new Random()).Next(0, Int32.MaxValue).ToString() + "_Document.pdf";

            Merge(destino, pdfs);

            for(int i = 0; i < 3; i++)
            {
                if (File.Exists(pdfs[i]))
                    File.Delete(pdfs[i]);
            }

            var pdfContent = new MemoryStream(System.IO.File.ReadAllBytes(destino));
            pdfContent.Position = 0;
            if (File.Exists(destino))
                File.Delete(destino);
            return pdfContent;


            //NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.Ticket);

            //y = y + 40;
            //pdf.EscribirXY("Nº: " + comprobante.NumeroComprobante.ToString().PadLeft(8, '0'), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            //y = y + 20;
            //pdf.EscribirXY(comprobante.ClienteNombre.ToString(), x, y, 12, Alineado.Izquierda, NFPDFColor.BLACK);

            //y = y + 20;
            //pdf.EscribirXY("Cotización", x, y, 10, Alineado.Izquierda, NFPDFColor.BLACK);


            //#region Datos de la empresa facturante

            ////y = y + 40;
            ////pdf.EscribirXY(comprobante.RazonSocial.ToUpper(), x, y, 32, Alineado.Izquierda, NFPDFColor.BLACK);

            ////y = y + 15;
            ////pdf.EscribirXY(comprobante.Domicilio, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY(comprobante.CiudadProvincia, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY("Tel.: " + comprobante.Telefono, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY(comprobante.CondicionIva, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            ////y = y + 12;
            ////pdf.EscribirXY("CUIT: " + comprobante.Cuit, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY("Ingresos brutos: " + comprobante.IIBB, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY("Inicio de actividades: " + comprobante.FechaInicioActividades, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            //#endregion


            ////if ((int)comprobante.TipoComprobante < 0)
            ////    codigo = "";
            ////else if ((int)comprobante.TipoComprobante == 999)
            ////    codigo = "P";
            ////else
            ////    codigo = ((int)comprobante.TipoComprobante).ToString().PadLeft(2, '0');

            ////y = y + 12;
            ////pdf.EscribirXY("Fecha: " + comprobante.Fecha.ToString("dd/MM/yyyy"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);    


            //#region Datos del cliente


            ////var tipoDoc = (comprobante.DocTipo == 96) ? "DNI" : "CUIT";
            ////var NroDeDoc = (comprobante.DocNro != 0) ? comprobante.DocNro.ToString() : "";
            ////y = y + 12;
            ////pdf.EscribirXY("Señor/es: " + comprobante.ClienteNombre, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY("Domicilio: " + comprobante.ClienteDomicilio, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 8;
            ////pdf.EscribirXY("Localidad: " + comprobante.ClienteLocalidad, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            ////if (comprobante.DocTipo != 99)
            ////{
            ////    y = y + 8;
            ////    pdf.EscribirXY(tipoDoc + ": " + NroDeDoc, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////}
            ////y = y + 8;
            ////pdf.EscribirXY("Condición de IVA: " + comprobante.ClienteCondiionIva, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////if (!esRecibo)
            ////{
            ////    y = y + 8;
            ////    pdf.EscribirXY("Condición de venta: " + comprobante.CondicionVenta, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////}


            //#endregion

            //y = y + 8;
            //pdf.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            //foreach (FEItemDetalle item in comprobante.ItemsDetalle)
            //{
            //    y = y + 10;
            //    pdf.EscribirXY(item.Cantidad.ToString() + " x " + item.Precio.ToString("N2"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            //    y = y + 8;
            //    pdf.EscribirXY(item.Codigo, x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            //    pdf.EscribirXY(item.Descripcion, x + 30, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            //    pdf.EscribirXY((item.Precio * item.Cantidad).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            //}

            //y = y + 8;
            //pdf.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            //y = y + 8;
            //pdf.EscribirXY("Total: $" + Math.Abs(comprobante.ImpTotal).ToString("N2"), x + 135, y, 7, Alineado.Derecha, NFPDFColor.BLACK);
            //y = y + 8;
            //pdf.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
            ////y = y + 30;
            ////pdf.EscribirXY("-----------------------------------------------------------", x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);

            //y = y + 20;
            //pdf.EscribirXY("Vale Retiro", x, y, 10, Alineado.Izquierda, NFPDFColor.BLACK);


        }

        public Stream GetStreamPDFLiquidoProducto(List<FEComprobante> comprobante, string tipoImpresion, string pathLogo, int xInicial, int yInicial)
        {

            //var template = (templateFc == string.Empty ? ConfigurationManager.AppSettings["FE.Template"] : templateFc);

            int cantidadHojas = 0;
            foreach (FEComprobante f in comprobante)
            {
                cantidadHojas+= (int)Math.Ceiling(((decimal)f.ItemsDetalle.Count()) / 18);
            }


            //int cantidadHojas = comprobante.Count();
            string[] pdfs = new string[cantidadHojas];
            int contadorHojas = 0;
            string pathTemplateLiquidoProducto = null;


            foreach (FEComprobante f in comprobante)
            {
                int resultadoHojas = (int)Math.Ceiling(((decimal)f.ItemsDetalle.Count()) / 18);      

                for (int i = 1; i <= resultadoHojas; i++)
                {

                    if (tipoImpresion.Equals("PreLiquidoProducto"))
                    {
                        if (f.ClienteCondiionIva.Equals("Monotributo"))
                            pathTemplateLiquidoProducto = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateLiquidoProductoB"]);
                        else
                            pathTemplateLiquidoProducto = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.TemplateLiquidoProductoA"]);

                        NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, pathTemplateLiquidoProducto);

                        pdf.EscribirXY(f.Fecha.ToString("dd"), 440, 124, 12, Alineado.Derecha);
                        pdf.EscribirXY(f.Fecha.ToString("MM"), 464, 124, 12, Alineado.Derecha);
                        pdf.EscribirXY(f.Fecha.ToString("yy"), 488, 124, 12, Alineado.Derecha);

                        pdf.EscribirXY(f.ClienteNombre, 90, 193, 12, Alineado.Izquierda);

                        pdf.EscribirXY(f.ClienteDomicilio, 100, 213, 10, Alineado.Izquierda);
                        pdf.EscribirXY(f.ClienteLocalidad, 285, 213, 10, Alineado.Izquierda);

                        var tipoDoc = (f.DocTipo == 96) ? "DNI" : "CUIT";
                        var NroDeDoc = (f.DocNro != 0) ? f.DocNro.ToString() : "";
                        if (f.DocTipo != 99)
                            pdf.EscribirXY(NroDeDoc, 420, 235, 10, Alineado.Izquierda);

                        //pdf.EscribirXY(f.CondicionIva, 27, 170, 10, Alineado.Izquierda);

                        int y = 275;
                        double ItemIVA = 0;
                        double ComisionIVA = 0;
                        double ComisionSubtotal = 0;
                        double ComisionTotal = 0;
                        double TotalImporte = 0;

                        List<FEItemDetalle> li = f.ItemsDetalle.OrderBy(o => o.Descripcion).Take(18).ToList();

                        foreach (FEItemDetalle item in li)
                        {
                            double Precio = item.Precio;
                            if (f.Cuit.Equals(30716909839))
                            {
                                Precio = Precio / 1000;
                            }

                            if (!item.Descripcion.ToUpper().Equals("COMISIÓN"))
                            {
                                y = y + 20;
                                //pdf.EscribirXY(item.Cantidad.ToString() + " x " + item.Precio.ToString("N2"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                                //y = y + 8;

                                //if (f.CondicionIva.Equals("Responsable Inscripto"))                        
                                //    ItemIVA = Precio * 0.21;

                                Precio = Precio + ItemIVA;
                                pdf.EscribirXY(item.Descripcion, 40, y, 11, Alineado.Izquierda, NFPDFColor.BLACK);
                                pdf.EscribirXY((Precio * item.Cantidad).ToString("N2"), 540, y, 11, Alineado.Derecha, NFPDFColor.BLACK);
                                TotalImporte += Precio * item.Cantidad;
                            }
                            else
                            {
                                if (f.CondicionIva.Equals("Responsable Inscripto"))
                                    ComisionIVA = Precio * 0.21;

                                ComisionSubtotal = Precio;
                                ComisionTotal = Precio + ComisionIVA;


                                pdf.EscribirXY("$" + Math.Abs(ComisionSubtotal).ToString("N2"), 363, 704, 11, Alineado.Derecha);
                                pdf.EscribirXY("$" + Math.Abs(ComisionIVA).ToString("N2"), 363, 719, 11, Alineado.Derecha);
                                pdf.EscribirXY("$" + Math.Abs(ComisionTotal).ToString("N2"), 363, 734, 11, Alineado.Derecha);
                            }
                            f.ItemsDetalle.Remove(item);
                        }


                        double TotalIVA;
                        double TotalSubtotal;
                        double Total;
                        double TotalNeto;
                        if (f.CondicionIva.Equals("Responsable Inscripto"))
                            TotalIVA = TotalImporte * 0.21;
                        else
                            TotalIVA = 0;
                        TotalSubtotal = TotalImporte;
                        Total = TotalIVA + TotalImporte;
                        TotalNeto = ComisionTotal + Total;
                        pdf.EscribirXY("$" + Math.Abs(TotalSubtotal).ToString("N2"), 543, 704, 11, Alineado.Derecha);
                        pdf.EscribirXY("$" + Math.Abs(TotalIVA).ToString("N2"), 543, 719, 11, Alineado.Derecha);
                        pdf.EscribirXY("$" + Math.Abs(Total).ToString("N2"), 543, 734, 11, Alineado.Derecha);
                        pdf.EscribirXY("$" + Math.Abs(TotalNeto).ToString("N2"), 543, 749, 11, Alineado.Derecha);

                        pdf.EscribirXY("PDC Nº: " + f.PtoVta.ToString().PadLeft(4, '0') + "-" + f.NumeroComprobante.ToString().PadLeft(8, '0'), 543, 830, 12, Alineado.Derecha);

                        pdf.FileName = "P" + contadorHojas.ToString() + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
                        pdf.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/liquidoProducto"));                        
                        
                        pdfs[contadorHojas] = HttpContext.Current.Server.MapPath("~/files/liquidoProducto/") + pdf.FileName;
                        contadorHojas++;

                    }
                    else
                    {
                        int x = 440 + xInicial;
                        int y = 139 + yInicial;

                        NFPDFWriter pdf = new NFPDFWriter(MedidasDocumento.A4, pathTemplateLiquidoProducto);

                        pdf.EscribirXY(f.Fecha.ToString("dd"), x, y, 12, Alineado.Derecha);
                        x += 24;
                        pdf.EscribirXY(f.Fecha.ToString("MM"), x, y, 12, Alineado.Derecha);
                        x += 24;
                        pdf.EscribirXY(f.Fecha.ToString("yy"), x, y, 12, Alineado.Derecha);

                        x -= 398;
                        y += 54; 
                        pdf.EscribirXY(f.ClienteNombre, x, y, 12, Alineado.Izquierda);

                        x += 10;
                        y += 35;
                        pdf.EscribirXY(f.ClienteDomicilio, x, y, 10, Alineado.Izquierda);
                        x += 185;
                        pdf.EscribirXY(f.ClienteLocalidad, x, y, 10, Alineado.Izquierda);

                        x += 135;
                        y += 22;
                        var tipoDoc = (f.DocTipo == 96) ? "DNI" : "CUIT";
                        var NroDeDoc = (f.DocNro != 0) ? f.DocNro.ToString() : "";
                        if (f.DocTipo != 99)
                            pdf.EscribirXY(NroDeDoc, x, y, 10, Alineado.Izquierda);

                        //pdf.EscribirXY(f.CondicionIva, 27, 170, 10, Alineado.Izquierda);

                        y = 285 + yInicial;
                        double ItemIVA = 0;
                        double ComisionIVA = 0;
                        double ComisionSubtotal = 0;
                        double ComisionTotal = 0;
                        double TotalImporte = 0;

                        List<FEItemDetalle> li = f.ItemsDetalle.OrderBy(o => o.Descripcion).Take(18).ToList();

                        foreach (FEItemDetalle item in li)
                        {
                            double Precio = item.Precio;
                            if (f.Cuit.Equals(30716909839))
                            {
                                Precio = Precio / 1000;
                            }

                            if (!item.Descripcion.ToUpper().Equals("COMISIÓN"))
                            {
                                y = y + 20;
                                //pdf.EscribirXY(item.Cantidad.ToString() + " x " + item.Precio.ToString("N2"), x, y, 7, Alineado.Izquierda, NFPDFColor.BLACK);
                                //y = y + 8;

                                //if (f.CondicionIva.Equals("Responsable Inscripto"))                        
                                //    ItemIVA = Precio * 0.21;

                                Precio = Precio + ItemIVA;
                                pdf.EscribirXY(item.Descripcion, 43 + xInicial, y, 11, Alineado.Izquierda, NFPDFColor.BLACK);
                                pdf.EscribirXY((Precio * item.Cantidad).ToString("N2"), 543 + xInicial, y, 11, Alineado.Derecha, NFPDFColor.BLACK);
                                TotalImporte += Precio * item.Cantidad;
                            }
                            else
                            {
                                if (f.CondicionIva.Equals("Responsable Inscripto"))
                                    ComisionIVA = Precio * 0.21;

                                ComisionSubtotal = Precio;
                                ComisionTotal = Precio + ComisionIVA;
                                pdf.EscribirXY("$" + Math.Abs(ComisionSubtotal).ToString("N2"), 363 + xInicial, 685 + yInicial, 11, Alineado.Derecha);
                                pdf.EscribirXY("$" + Math.Abs(ComisionIVA).ToString("N2"), 363 + xInicial, 700 + yInicial, 11, Alineado.Derecha);
                                pdf.EscribirXY("$" + Math.Abs(ComisionTotal).ToString("N2"), 363 + xInicial, 715 + yInicial, 11, Alineado.Derecha);
                            }
                            f.ItemsDetalle.Remove(item);
                        }


                        double TotalIVA;
                        double TotalSubtotal;
                        double Total;
                        double TotalNeto;
                        if (f.CondicionIva.Equals("Responsable Inscripto"))
                            TotalIVA = TotalImporte * 0.21;
                        else
                            TotalIVA = 0;
                        TotalSubtotal = TotalImporte;
                        Total = TotalIVA + TotalImporte;
                        TotalNeto = ComisionTotal + Total;

                        x += 123;
                        y = 685 + yInicial;
                        pdf.EscribirXY("$" + Math.Abs(TotalSubtotal).ToString("N2"), x, y, 11, Alineado.Derecha);
                        y += 15;
                        pdf.EscribirXY("$" + Math.Abs(TotalIVA).ToString("N2"), x, y, 11, Alineado.Derecha);
                        y += 15;
                        pdf.EscribirXY("$" + Math.Abs(Total).ToString("N2"), x, y, 11, Alineado.Derecha);
                        y += 15;
                        pdf.EscribirXY("$" + Math.Abs(TotalNeto).ToString("N2"), x, y, 11, Alineado.Derecha);

                        y += 95;
                        pdf.EscribirXY("PDC Nº: " + f.PtoVta.ToString().PadLeft(4, '0') + "-" + f.NumeroComprobante.ToString().PadLeft(8, '0'), x, y, 12, Alineado.Derecha);

                        pdf.FileName = "P" + contadorHojas.ToString() + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";
                        pdf.GenerarPDFEnDisco(HttpContext.Current.Server.MapPath("~/files/liquidoProducto"));
                        
                        pdfs[contadorHojas] = HttpContext.Current.Server.MapPath("~/files/liquidoProducto/") + pdf.FileName;
                        contadorHojas++;
                    }

                    
                }
                
            }

            string destino = HttpContext.Current.Server.MapPath("~/files/liquidoProducto/") + (new Random()).Next(0, Int32.MaxValue).ToString() + "_Document.pdf";

            Merge(destino, pdfs);

            for (int i = 0; i < cantidadHojas; i++)
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

        public string CodigoDeBarras(FEComprobante comprobante)
        {
            /*
            El código de barras deberá contener los siguientes datos con su correspondiente orden:
            *Clave Unica de Identificación Tributaria (C.U.I.T.) del emisor de la factura (11 caracteres
            *Código de tipo de comprobante (2 caracteres)
            *Punto de venta (4 caracteres)
            *Código de Autorización de Impresión (C.A.I.) (14 caracteres)
            *Fecha de vencimiento (8 caracteres)
            *Dígito verificador (1 carácter)
            */

            string codigo = comprobante.Cuit.ToString();
            codigo += ((int)comprobante.TipoComprobante).ToString().PadLeft(2, '0');
            codigo += comprobante.PtoVta.ToString().PadLeft(4, '0');
            codigo += comprobante.CAE;
            codigo += comprobante.FechaVencCAE.ToString("yyyyMMdd");
            codigo += DigitoVerificador(codigo);
            codigo = codigo.Replace("-", "");

            return codigo;

        }

        private System.Drawing.Image ImagenCodigoBarras(FEComprobante comprobante)
        {
            BarcodeInter25 bar = new BarcodeInter25();

            bar.ChecksumText = true;
            bar.GenerateChecksum = false;
            bar.Code = CodigoDeBarras(comprobante);

            return bar.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
        }

        private System.Drawing.Image ImagenQR(FEComprobante comprobante)
        {

            BarcodeInter25 QR = new BarcodeInter25();
            FEComprobanteQR comQR = new FEComprobanteQR();
            string urlAfipQR = ConfigurationManager.AppSettings["FE.UrlQR"];

            comQR.ver = 1;
            comQR.fecha = comprobante.Fecha.ToString("yyyy-MM-dd");
            comQR.cuit = comprobante.Cuit;
            comQR.ptoVta = comprobante.PtoVta;
            comQR.tipoCmp = (int)comprobante.TipoComprobante;
            comQR.nroCmp = comprobante.NumeroComprobante;
            comQR.importe = comprobante.ImpTotal;
            comQR.moneda = comprobante.CodigoMoneda;
            comQR.ctz = comprobante.CotizacionMoneda;
            comQR.tipoCodAut = "E";
            comQR.codAut = Convert.ToInt64(comprobante.CAE);

            var json = JsonConvert.SerializeObject(comQR);
            string base64Temp = System.Web.HttpServerUtility.UrlTokenEncode(Encoding.ASCII.GetBytes(json));
            string base64 = base64Temp.Substring(0, base64Temp.Length - 1);
            string codigo = urlAfipQR + base64;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(codigo, QRCodeGenerator.ECCLevel.L);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = new Bitmap(qrCode.GetGraphic(4));

            return qrCodeImage;            
        }

        private static int DigitoVerificador(string codigoBarras)
        {
            int digito;
            int pares = 0;
            int impares = 0;

            var aux = codigoBarras.Replace("-", "");

            for (int i = 0; i < aux.Length; i++)
            {
                if (i % 2 == 0)
                    pares = pares + int.Parse(aux.Substring(i, 1));
                else
                    impares = impares + int.Parse(aux.Substring(i, 1));
            }

            digito = 10 - ((pares + (3 * impares)) % 10);

            if (digito == 10)
                digito = 0;

            return digito;
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
    }
}
