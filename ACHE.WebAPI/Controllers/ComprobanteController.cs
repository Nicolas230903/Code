using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Model;
using ACHE.Extensions;
using ACHE.Negocio.Common;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;

namespace ACHE.WebAPI.Controllers
{
    public class ComprobanteController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage obtener(string token, string condicion, string periodo, string fechaDesde, string fechaHasta, int page, int pageSize)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    using (var dbContext = new ACHEEntities())
                    {
                        var resultado = ComprobantesCommon.ObtenerComprobantes(condicion, periodo, fechaDesde, fechaHasta, "", page, pageSize, usu, "",false,false,"", false, false);
                        return Request.CreateResponse(HttpStatusCode.OK, resultado);
                    }
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
        public HttpResponseMessage procesar(ComprobanteCartDto comprobante)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(comprobante.Token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    comprobante.IDUsuario = idUsuario;
                    ComprobantesCommon.GuardarComprobante(comprobante);
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
        public HttpResponseMessage eliminar(string token, int id)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    if (ComprobantesCommon.EliminarComprobante(id, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El Comprobante no existe");
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