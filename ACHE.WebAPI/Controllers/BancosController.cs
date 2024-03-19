using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Model;
using ACHE.Negocio.Common;
using ACHE.Negocio.Banco;
namespace ACHE.WebAPI.Controllers
{
    public class BancosController : ApiController
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
                        var resultado = BancosCommon.ObtenerBancos(filtro, page, pageSize, usu);
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
        public HttpResponseMessage procesar(string token, int id, int idBancoBase, string nroCuenta, string moneda, int activo, string saldoInicial, string ejecutivo, string direccion, string telefono, string email, string observacion)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(nroCuenta))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la nroCuenta como parámetro");
                    else if (string.IsNullOrWhiteSpace(moneda))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la moneda como parámetro");

                    BancosCommon.GuardarBanco(id, idBancoBase, nroCuenta, moneda, activo, saldoInicial, ejecutivo, direccion, telefono, email, observacion, usu);
                    return Request.CreateResponse(HttpStatusCode.OK,"OK");
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
                    if (BancosCommon.EliminarBancos(id, usu))
                        return Request.CreateResponse(HttpStatusCode.OK,"OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El Banco no existe");
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