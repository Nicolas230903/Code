using ACHE.FacturaElectronica.VEConsumerService;
using ACHE.FacturaElectronica.WSFacturaElectronica;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Web;

namespace ACHE.FacturaElectronica
{
    public class AFIPVEConsumerService
    {
        public bool GetEstadoServicio(string modo)
        {
            try
            {
                var remoteAddress = new System.ServiceModel.EndpointAddress(modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);

                BasicHttpBinding b = new BasicHttpBinding();
                b.Security.Mode = BasicHttpSecurityMode.Transport;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                b.MessageEncoding = WSMessageEncoding.Mtom;


                using (var v = new VEConsumerClient(b, remoteAddress))
                {
                    //set timeout
                    v.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, 20);
                    

                    //call web service method
                    DummyResult d = v.dummy();
                    if (d.appserver != "OK" || d.authserver != "OK" || d.dbserver != "OK")
                        return false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public static bool OnValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        //{
        //    //Simplemente devolvemos un true indicando que el certificado es válido --> es lo mismo que pulsar el botón continuar.
        //    return true;
        //}

        public List<RespuestaPaginada> GetComunicaciones(long cuitPersona, long cuitAfip, string modo, long ultimaComunicacion, bool forzarTicketNuevo)
        {
            try
            {
                List<RespuestaPaginada> lr = new List<RespuestaPaginada>();

                var remoteAddress = new System.ServiceModel.EndpointAddress(modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);

                BasicHttpBinding b = new BasicHttpBinding();
                b.Security.Mode = BasicHttpSecurityMode.Transport;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                b.MessageEncoding = WSMessageEncoding.Mtom;


                using (var v = new VEConsumerClient(b, remoteAddress))
                {
                    //set timeout
                    v.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, 20);

                    Service objWSFEV1 = new Service();
                    FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                    FETicket ticket = FEAutenticacion.GetTicket(0, cuitAfip, "veconsumerws", modo, forzarTicketNuevo);

                    //call web service method
                    AuthRequest ar = new AuthRequest();
                    ar.cuitRepresentada = cuitPersona;
                    ar.sign = ticket.Sign;
                    ar.token = ticket.Token;                    

                    Filter f = new Filter();
                    DateTime fechaDesde = DateTime.Now.AddDays(-360);
                    f.fechaDesde = fechaDesde.ToString("yyyy-MM-dd");
                    f.comunicacionIdDesde = ultimaComunicacion;
                    

                    RespuestaPaginada r = v.consultarComunicaciones(ar,f);
                    
                    if (r != null)
                    {
                        lr.Add(r);
                        for (int i = 2; i <= r.totalPaginas; i++)
                        {
                            f.pagina = i;
                            RespuestaPaginada rPag = v.consultarComunicaciones(ar, f);
                            if (rPag != null)
                            {
                                lr.Add(rPag);
                            }
                        }
                    }

                    return lr;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<RespuestaPaginada> GetComunicaciones(long cuitPersona, long cuitAfip, string modo, long desdeIdComunicacion, long hastaIdComunicacion, string desdeFecha, string hastaFecha, bool forzarTicketNuevo)
        {
            try
            {
                List<RespuestaPaginada> lr = new List<RespuestaPaginada>();

                var remoteAddress = new System.ServiceModel.EndpointAddress(modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);

                BasicHttpBinding b = new BasicHttpBinding();
                b.Security.Mode = BasicHttpSecurityMode.Transport;
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                b.MessageEncoding = WSMessageEncoding.Mtom;


                using (var v = new VEConsumerClient(b, remoteAddress))
                {
                    //set timeout
                    v.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, 30);

                    Service objWSFEV1 = new Service();
                    FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                    FETicket ticket = FEAutenticacion.GetTicket(0, cuitAfip, "veconsumerws", modo, forzarTicketNuevo);

                    //call web service method
                    AuthRequest ar = new AuthRequest();
                    ar.cuitRepresentada = cuitPersona;
                    ar.sign = ticket.Sign;
                    ar.token = ticket.Token;

                    Filter f = new Filter();
                    if (hastaIdComunicacion != 0)
                    {
                        f.comunicacionIdDesde = desdeIdComunicacion;
                        f.comunicacionIdDesdeSpecified = true;
                    }

                    if (hastaIdComunicacion != -1)
                    {
                        f.comunicacionIdHasta = hastaIdComunicacion;
                        f.comunicacionIdHastaSpecified = true;
                    }                        

                    if (!desdeFecha.Equals("19000101"))
                    {
                        f.fechaDesde = desdeFecha.Substring(0, 4) + "-" + desdeFecha.Substring(4, 2) + "-" + desdeFecha.Substring(6, 2);
                    }
                    else
                    {
                        DateTime fechaDesde = DateTime.Now.AddDays(-360);
                        f.fechaDesde = fechaDesde.ToString("yyyy-MM-dd");
                    }                    

                    if (!hastaFecha.Equals("19000101"))
                    {
                        f.fechaHasta = hastaFecha.Substring(0, 4) + "-" + hastaFecha.Substring(4, 2) + "-" + hastaFecha.Substring(6, 2);
                    }

                    RespuestaPaginada r = v.consultarComunicaciones(ar, f);
                    
                    if (r != null)
                    {
                        lr.Add(r);
                        for (int i = 2; i <= r.totalPaginas; i++)
                        {
                            f.pagina = i;
                            RespuestaPaginada rPag = v.consultarComunicaciones(ar, f);
                            if (rPag != null)
                            {
                                lr.Add(rPag);
                            }
                        }
                    }

                    return lr;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Comunicacion GetAdjuntos(long cuitPersona, long cuitAfip, long idComunicacion, string modo, bool forzarTicketNuevo)
        {
            try
            {
                List<RespuestaPaginada> lr = new List<RespuestaPaginada>();

                var remoteAddress = new System.ServiceModel.EndpointAddress(modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);

                BasicHttpBinding b = new BasicHttpBinding();
                b.Security.Mode = BasicHttpSecurityMode.Transport;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                b.MessageEncoding = WSMessageEncoding.Mtom;
                b.MaxReceivedMessageSize = 64000000;

                using (var v = new VEConsumerClient(b, remoteAddress))
                {
                    //set timeout
                    v.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 0, 20);

                    Service objWSFEV1 = new Service();
                    FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                    FETicket ticket = FEAutenticacion.GetTicket(0, cuitAfip, "veconsumerws", modo, forzarTicketNuevo);

                    //call web service method
                    AuthRequest ar = new AuthRequest();
                    ar.cuitRepresentada = cuitPersona;
                    ar.sign = ticket.Sign;
                    ar.token = ticket.Token;

                    Comunicacion c = v.consumirComunicacion(ar,idComunicacion,true);                  

                    return c;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
