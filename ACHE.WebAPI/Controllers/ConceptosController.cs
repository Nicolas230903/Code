using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Negocio.Common;
using ACHE.Negocio.Productos;

namespace ACHE.WebAPI.Controllers
{
    public class ConceptosController : ApiController
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
                    var resultado = ConceptosCommon.ObtenerConceptos(filtro, page, pageSize, usu);
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
        public HttpResponseMessage procesar(string token, int id, string nombre, string codigo, string tipo, string descripcion, string estado, string precio, string iva, string stock, string obs, string constoInterno, string stockMinimo, int idPersona)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(nombre))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la nombre como parámetro");
                    else if (string.IsNullOrWhiteSpace(codigo))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la codigo como parámetro");
                    else if (string.IsNullOrWhiteSpace(precio))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la precio como parámetro");
                    else if (tipo.ToUpper() == "P" && stock == "0")
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la stock como parámetro");

                    int idConcepto = ConceptosCommon.GuardarConcepto(id, nombre, codigo, tipo, descripcion, estado, precio, iva, stock, obs, constoInterno, stockMinimo, idPersona, usu.IDUsuario);
                    return Request.CreateResponse(HttpStatusCode.OK, idConcepto.ToString());
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
        public HttpResponseMessage eliminar(string token, int id)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    if (ConceptosCommon.EliminarConcepto(id, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El concepto no existe");
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
