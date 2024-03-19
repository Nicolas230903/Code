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
    public class CommonController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ObtenerProvincias(string token)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    using (var dbContext = new ACHEEntities())
                    {
                        var resultado = CommonModel.ObtenerProvincias();
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
        [HttpGet]
        public HttpResponseMessage obtenerCiudades(string token, int idProvincia)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    using (var dbContext = new ACHEEntities())
                    {
                        var resultado = CommonModel.obtenerCiudades(idProvincia);
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

        [HttpGet]
        public HttpResponseMessage obtenerCiudadesPaginadas(string token, int page, int pageSize)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    using (var dbContext = new ACHEEntities())
                    {
                        var resultado = CommonModel.obtenerCiudadesPaginadas(page, pageSize);
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
	}
}