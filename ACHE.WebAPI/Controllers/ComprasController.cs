using ACHE.Model.Negocio;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Common;
using ACHE.Negocio.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ACHE.WebAPI.Controllers
{
    public class ComprasController : ApiController
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

                    var resultado = ComprasCommon.ObtenerCompras(condicion, periodo, fechaDesde, fechaHasta, page, pageSize, usu);
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
        public HttpResponseMessage procesar(ComprasDto compra)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(compra.Token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
                    ComprasCommon.Guardar(compra.IDCompra, compra.IDPersona, compra.Fecha, compra.NroFactura, compra.Iva, compra.Importe2, compra.Importe5,
                    compra.Importe10, compra.Importe21, compra.Importe27, compra.NoGrav, compra.ImporteMon, compra.ImpNacional, compra.ImpMunicipal,
                    compra.ImpInterno, compra.PercepcionIva, compra.Otros, compra.Obs, compra.Tipo, compra.IdCategoria, compra.Rubro, compra.Exento,
                    compra.FechaEmision, compra.IdPlanDeCuenta, usu.IDUsuario, compra.Jurisdicciones, compra.FechaPrimerVencimiento, compra.FechaSegundoVencimiento, compra.Adjunto);
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
                    if (ComprasCommon.EliminarCompra(id, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "La Compra no existe");
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
