using ACHE.FacturaElectronica.WSFacturaElectronica;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ACHE.FacturaElectronica
{
    public class FEPuntoDeVenta
    {
        public FEPtoVentaResponse GetPuntoDeVenta(long cuitPersona, long cuitAfip, string modo, bool forzarNuevoTicket)
        {
            try
            {
                Service objWSFEV1 = new Service();
                FEAuthRequest objFEAuthRequest = new FEAuthRequest();
                FETicket ticket = FEAutenticacion.GetTicket(0, cuitAfip, "wsfe", modo, forzarNuevoTicket);

                objFEAuthRequest.Token = ticket.Token;
                objFEAuthRequest.Sign = ticket.Sign;
                objFEAuthRequest.Cuit = cuitPersona;

                long cuitRep = 0;
                if (modo.Equals("QA"))
                    cuitRep = Convert.ToInt64(ConfigurationManager.AppSettings["FE.QA.CUIL"]);
                else
                    cuitRep = Convert.ToInt64(cuitAfip);

                objWSFEV1.Url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);
                FEPtoVentaResponse r = objWSFEV1.FEParamGetPtosVenta(objFEAuthRequest);

                return r;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
