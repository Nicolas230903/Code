using ACHE.FacturaElectronica.WSFacturaElectronica;
using ACHE.FacturaElectronica.WSPersonaServiceA5v34;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace ACHE.FacturaElectronica
{
    public class AFIPPersonaServiceA5v34
    {

        public bool GetEstadoServicio(string modo)
        {
            try
            {
                PersonaServiceA5 a = new PersonaServiceA5();

                a.Url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_sr_padron_a5"] : ConfigurationManager.AppSettings["FE.PROD.ws_sr_padron_a5"]);
                dummyReturn d = a.dummy();
                if (d.appserver != "OK" || d.authserver != "OK" || d.dbserver != "OK")
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public personaReturn GetPersona(long cuitPersona, long cuitAfip, string modo)
        {
            try
            {
                PersonaServiceA5 a = new PersonaServiceA5();

                Service objWSFEV1 = new Service();
                FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                FETicket ticket = FEAutenticacion.GetTicket(0, cuitAfip, "ws_sr_padron_a5", modo, false);

                long cuitRep = 0;
                if (modo.Equals("QA"))
                    cuitRep = Convert.ToInt64(ConfigurationManager.AppSettings["FE.QA.CUIL"]);
                else
                    cuitRep = Convert.ToInt64(cuitAfip);

                a.Url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_sr_padron_a5"] : ConfigurationManager.AppSettings["FE.PROD.ws_sr_padron_a5"]);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                personaReturn p = a.getPersona(ticket.Token, ticket.Sign, cuitRep, cuitPersona);

                return p;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool GetEstadoServicio_v2(string modo)
        {
            try
            {
                PersonaServiceA5 a = new PersonaServiceA5();

                a.Url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_sr_padron_a5"] : ConfigurationManager.AppSettings["FE.PROD.ws_sr_padron_a5"]);
                dummyReturn d = a.dummy();
                if (d.appserver != "OK" || d.authserver != "OK" || d.dbserver != "OK")
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public personaReturn GetPersona_v2(long cuitPersona, long cuitAfip, string modo)
        {
            try
            {
                PersonaServiceA5 a = new PersonaServiceA5();

                Service objWSFEV1 = new Service();
                FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                FETicket ticket = FEAutenticacion.GetTicket(0, cuitAfip, "ws_sr_constancia_inscripcion", modo, false);

                long cuitRep = 0;
                if (modo.Equals("QA"))
                    cuitRep = Convert.ToInt64(ConfigurationManager.AppSettings["FE.QA.CUIL"]);
                else
                    cuitRep = Convert.ToInt64(cuitAfip);  

                a.Url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_sr_padron_a5"] : ConfigurationManager.AppSettings["FE.PROD.ws_sr_padron_a5"]);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                personaReturn p = a.getPersona_v2(ticket.Token, ticket.Sign, cuitRep, cuitPersona);

                return p;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
