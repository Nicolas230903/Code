using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace ACHE.FacturaElectronica
{
    public static class FEAutenticacion
    {
        private static readonly Hashtable tickets = new Hashtable();
        private static object bloqueo = new object();

        public static FETicket GetTicket(long cuit, long cuitAfip, string servicio, /*string urlWsaaWsdl, string certificadoAfip,*/ string modo, bool forzarNuevoTicket)
        {

            if (modo.Equals("QA"))
                cuit = Convert.ToInt64(ConfigurationManager.AppSettings["FE.QA.CUIL"]);
            else
                cuit = cuitAfip;

            //Si forzarNuevoTicket esta en true, solicito nuevo ticket sin importar si tengo un ticket valido en la base de datos. Se utiliza para solucionar problemas de nuevas habilitaciones de afip.
            if (forzarNuevoTicket)
                return GenerarTicket(cuit, servicio, /*urlWsaaWsdl, certificadoAfip,*/ modo);

            //Consulto si ya tengo generado un ticket en la BD.
            FETicket ticket = ConsultarTicketAfip(cuit, servicio, modo);
            if (ticket.Token == null)
            {
                return GenerarTicket(cuit, servicio, /*urlWsaaWsdl, certificadoAfip,*/ modo);
                //lock (bloqueo)
                //{
                //    if (tickets.ContainsKey(cuit + "|" + servicio))
                //    {
                //        if (((FETicket)tickets[cuit + "|" + servicio]).Vencimiento <= DateTime.Now.AddMinutes(20))
                //        {
                //            tickets.Remove(cuit + "|" + servicio);
                //            return GenerarTicket(cuit, servicio, /*urlWsaaWsdl, certificadoAfip,*/ modo);
                //        }
                //        else
                //            return (FETicket)tickets[cuit + "|" + servicio];
                //    }
                //    else
                //        return GenerarTicket(cuit, servicio, /*urlWsaaWsdl, certificadoAfip,*/ modo);
                //}
            }
            else //Todavia no vencio el ticket, lo puedo utilizar
            {
                return ticket;
            }
            
        }

        private static FETicket GenerarTicket(long cuit, string servicio, /*string urlWsaaWsdl, string certificadoAfip,*/ string modo)
        {
            FETicket ticket = new FETicket();
            LoginTicket objTicketRespuesta;
            string strTicketRespuesta;


            //string strUrlWsaaWsdl = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsaa"] : ConfigurationManager.AppSettings["FE.PROD.wsaa"]);
            //string strRutaCertSigner = (modo == "QA" ?  HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.QA.CertificadoAFIP"]) :  HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.PROD.CertificadoAFIP"]));
            string strUrlWsaaWsdl = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsaa"] : ConfigurationManager.AppSettings["FE.PROD.wsaa"]);
            string strRutaCertSigner = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.CertificadoAFIP"] : ConfigurationManager.AppSettings["FE.PROD.CertificadoAFIP"] + cuit + ".pfx");
            //string strRutaCertSigner = (modo == "QA" ? HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.QA.CertificadoAFIP"]) : HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.PROD.CertificadoAFIP"]) + cuit + ".pfx");

            try
            {
                if (!File.Exists(strRutaCertSigner))
                    throw new Exception("El certificado no existe en " + strRutaCertSigner);

                objTicketRespuesta = new LoginTicket();
                strTicketRespuesta = objTicketRespuesta.ObtenerLoginTicketResponse(servicio, strUrlWsaaWsdl, strRutaCertSigner, false);

                ticket.Cuit = cuit;
                ticket.Creado = objTicketRespuesta.GenerationTime;
                ticket.Servicio = objTicketRespuesta.Service;
                ticket.Sign = objTicketRespuesta.Sign;
                ticket.Token = objTicketRespuesta.Token;
                ticket.UniqueID = objTicketRespuesta.UniqueId;
                ticket.Vencimiento = objTicketRespuesta.ExpirationTime;

                InsertarTicketAfip(ticket, modo);

                //lock (bloqueo)
                //{
                //    tickets.Add(cuit + "|" + servicio, ticket);
                //}
            }
            catch (Exception excepcionAlObtenerTicket)
            {
                throw excepcionAlObtenerTicket;
            }

            return ticket;
        }

        private static FETicket ConsultarTicketAfip(long cuit, string servicio, string modo)
        {
            FETicket ticket = new FETicket();

            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

                SqlCommand comando = new SqlCommand("PA_ConsultarTicketAfip", cn);
                comando.CommandType = CommandType.StoredProcedure;

                //Parametros
                SqlParameter parCuit = new SqlParameter("@cuit", SqlDbType.Decimal);
                parCuit.Direction = ParameterDirection.Input;
                parCuit.Value = cuit;
                comando.Parameters.Add(parCuit);

                SqlParameter parServicio = new SqlParameter("@servicio", SqlDbType.VarChar);
                parServicio.Direction = ParameterDirection.Input;
                parServicio.Value = servicio;
                comando.Parameters.Add(parServicio);

                SqlParameter parModo = new SqlParameter("@modo", SqlDbType.VarChar);
                parModo.Direction = ParameterDirection.Input;
                parModo.Value = modo;
                comando.Parameters.Add(parModo);

                comando.Connection.Open();
                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    ticket.Cuit = cuit;
                    ticket.Creado = Convert.ToDateTime(lector["Creado"].ToString());
                    ticket.Servicio = servicio;
                    ticket.Sign = lector["Firma"].ToString().Trim();
                    ticket.Token = lector["Token"].ToString().Trim();
                    ticket.UniqueID = Convert.ToUInt32(lector["UniqueID"].ToString());
                    ticket.Vencimiento = Convert.ToDateTime(lector["Vencimiento"].ToString());
                }

                comando.Connection.Close();
                comando.Connection.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ticket;
        }

        //private static void EliminarTicketAfip(long cuit, string servicio, string modo)
        //{
        //    try
        //    {
        //        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

        //        SqlCommand comando = new SqlCommand("PA_EliminarTicketAfip", cn);
        //        comando.CommandType = CommandType.StoredProcedure;

        //        //Parametros
        //        SqlParameter parCuit = new SqlParameter("@cuit", SqlDbType.Decimal);
        //        parCuit.Direction = ParameterDirection.Input;
        //        parCuit.Value = cuit;
        //        comando.Parameters.Add(parCuit);

        //        SqlParameter parServicio = new SqlParameter("@servicio", SqlDbType.VarChar);
        //        parServicio.Direction = ParameterDirection.Input;
        //        parServicio.Value = servicio;
        //        comando.Parameters.Add(parServicio);

        //        SqlParameter parModo = new SqlParameter("@modo", SqlDbType.VarChar);
        //        parModo.Direction = ParameterDirection.Input;
        //        parModo.Value = modo;
        //        comando.Parameters.Add(parModo);

        //        comando.Connection.Open();
        //        comando.ExecuteNonQuery();
        //        comando.Connection.Close();
        //        comando.Connection.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private static void InsertarTicketAfip(FETicket ticket, String modo)
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

                SqlCommand comando = new SqlCommand("PA_InsertarTicketAfip", cn);
                comando.CommandType = CommandType.StoredProcedure;

                //Parametros
                SqlParameter parCuit = new SqlParameter("@cuit", SqlDbType.Decimal);
                parCuit.Direction = ParameterDirection.Input;
                parCuit.Value = ticket.Cuit;
                comando.Parameters.Add(parCuit);

                SqlParameter parCreado = new SqlParameter("@creado", SqlDbType.DateTime);
                parCreado.Direction = ParameterDirection.Input;
                parCreado.Value = ticket.Creado;
                comando.Parameters.Add(parCreado);

                SqlParameter parServicio = new SqlParameter("@servicio", SqlDbType.VarChar);
                parServicio.Direction = ParameterDirection.Input;
                parServicio.Value = ticket.Servicio;
                comando.Parameters.Add(parServicio);

                SqlParameter parFirma = new SqlParameter("@firma", SqlDbType.VarChar);
                parFirma.Direction = ParameterDirection.Input;
                parFirma.Value = ticket.Sign;
                comando.Parameters.Add(parFirma);

                SqlParameter parToken = new SqlParameter("@token", SqlDbType.VarChar);
                parToken.Direction = ParameterDirection.Input;
                parToken.Value = ticket.Token;
                comando.Parameters.Add(parToken);

                SqlParameter parUniqueID = new SqlParameter("@uniqueID", SqlDbType.VarChar);
                parUniqueID.Direction = ParameterDirection.Input;
                parUniqueID.Value = ticket.UniqueID.ToString();
                comando.Parameters.Add(parUniqueID);

                SqlParameter parVencimiento = new SqlParameter("@vencimiento", SqlDbType.DateTime);
                parVencimiento.Direction = ParameterDirection.Input;
                parVencimiento.Value = ticket.Vencimiento;
                comando.Parameters.Add(parVencimiento);

                SqlParameter parModo = new SqlParameter("@modo", SqlDbType.VarChar);
                parModo.Direction = ParameterDirection.Input;
                parModo.Value = modo;
                comando.Parameters.Add(parModo);

                comando.Connection.Open();
                comando.ExecuteNonQuery();
                comando.Connection.Close();
                comando.Connection.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}