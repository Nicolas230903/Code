using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Model;
using ACHE.Extensions;
using ACHE.Negocio.Common;

namespace ACHE.WebAPI.Controllers
{
    public class ProveedoresController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage obtener(string token, string filtro, int page, int pageSize)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    using (var dbContext = new ACHEEntities())
                    {
                        var resultado = PersonasCommon.ObtenerPersonas(filtro, "P", page, pageSize, usu);
                        return Request.CreateResponse(HttpStatusCode.OK, resultado);
                    }
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token inválido");
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage procesar(string token, int id, string razonSocial, string nombreFantasia, string condicionIva, string personeria, string tipoDoc, string nroDoc,
        string telefono, string email, int idProvincia, int idCiudad, string provinciaDesc, string ciudadDesc, string domicilio, string pisoDepto, string cp, string obs, int listaPrecio, string codigo, decimal saldoInicial)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(razonSocial))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la razonSocial como parámetro");
                    if (string.IsNullOrWhiteSpace(condicionIva))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la condicionIva como parámetro");

                    PersonasCommon.GuardarPersonas(id, razonSocial, nombreFantasia, condicionIva, personeria, 
                        tipoDoc, nroDoc, telefono, email, "P", idProvincia, idCiudad, provinciaDesc, 
                        ciudadDesc, domicilio, pisoDepto, cp, obs, listaPrecio, codigo, saldoInicial,0,"",0, usu);
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token inválido");
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage eliminar(string token, int id)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    if (PersonasCommon.EliminarPersona(id, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El proveedor no existe");
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
