using ACHE.Negocio.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ACHE.WebAPI.Controllers
{
    public class ConfiguracionController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage obtener(string token)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    var resultado = UsuarioCommon.ObtenerConfiguracion(usu);
                    return Request.CreateResponse(HttpStatusCode.OK, resultado);
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token inválido");
            }
            catch (CustomException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, ex.Message);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage procesar(string token, string razonSocial, string condicionIva, string cuit, string iibb, string fechaInicio,
        string personeria, string emailAlertas, string telefono, string celular, string contacto,
        string idProvincia, string idCiudad, string domicilio, string pisoDepto, string cp, bool esAgentePersepcionIVA,
        bool esAgentePersepcionIIBB, bool esAgenteRetencionGanancia, bool esAgenteRetencion, 
        bool exentoIIBB, string fechaCierreContable, string idJurisdiccion, string cbu, string textoFinalFactura)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    UsuarioCommon.GuardarConfiguracion(razonSocial, condicionIva, cuit, iibb, fechaInicio, personeria, emailAlertas,
                    telefono, celular, contacto, idProvincia, idCiudad, domicilio, pisoDepto, cp, esAgentePersepcionIVA, esAgentePersepcionIIBB,
                    esAgenteRetencionGanancia, esAgenteRetencion, exentoIIBB, 
                    fechaCierreContable, idJurisdiccion, cbu, textoFinalFactura, usu);
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token inválido");
            }
            catch (CustomException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, ex.Message);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage cambiarPassword(string token, string passwordActual, string PasswordNuevo, string Passwordverificado)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    UsuarioCommon.CambiarPassword(passwordActual, PasswordNuevo, Passwordverificado, usu);
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");

                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token inválido");
            }
            catch (CustomException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, ex.Message);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}
