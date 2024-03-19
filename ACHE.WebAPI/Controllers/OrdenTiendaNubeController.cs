using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Negocio.Common;
using ACHE.Negocio.Productos;

namespace ACHE.WebAPI.Controllers
{
    public class OrdenTiendaNubeController : ApiController
    {

        [HttpPut]
        public HttpResponseMessage modificarEstado(string token, int idOrden, string estado, string motivo)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (idOrden <= 0)
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo idOrden debe ser mayor a cero");

                    if (!estado.Trim().ToUpper().Equals("CERRADO") && !estado.Trim().ToUpper().Equals("CANCELADO"))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo estado contiene un valor invalido.");
                    }
                    else
                    {
                        switch (estado)
                        {
                            case "CERRADO":
                                estado = "close";
                                break;
                            case "CANCELADO":
                                estado = "cancel";
                                break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(motivo))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo motivo obligatorio.");
                    }

                    if (estado.Equals("cancel"))
                    {
                        if (!motivo.Trim().ToUpper().Equals("CLIENTE") && !motivo.Trim().ToUpper().Equals("STOCK") &&
                                                            !motivo.Trim().ToUpper().Equals("FRAUDE") && !motivo.Trim().ToUpper().Equals("OTRO"))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo motivo contiene un valor invalido.");
                        }
                        else
                        {
                            switch (motivo)
                            {
                                case "CLIENTE":
                                    motivo = "customer";
                                    break;
                                case "STOCK":
                                    motivo = "inventory";
                                    break;
                                case "FRAUDE":
                                    motivo = "fraud";
                                    break;
                                case "OTRO":
                                    motivo = "other";
                                    break;
                            }
                        }
                    }
                    if (estado.Equals("close"))
                    {
                        if (!motivo.Trim().ToUpper().Equals("COMPLETADO"))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo motivo contiene un valor invalido.");
                        else                        
                            motivo = "completed";                                               
                    }


                    string userAgent = usu.RazonSocial.ToUpper() + " (" + usu.Email.Trim() + ")";

                    if (TiendaNubeCommon.ModificarEstadoOrden(usu.TiendaNubeIdTienda, usu.TiendaNubeToken, userAgent, idOrden, estado, motivo, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "La orden no existe");
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
        public HttpResponseMessage obtenerOrdenes(string token, int desdeIdOrden, int hastaIdOrden, string estado)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (desdeIdOrden < 0)
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo desdeIdOrden debe ser 0 o mayor.");

                    if(hastaIdOrden != -1)
                    {
                        if (desdeIdOrden > hastaIdOrden)
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo hastaIdOrden debe ser mayor o igual a desdeIdOrden.");
                    }

                    if (!estado.Trim().ToUpper().Equals("ABIERTO") && !estado.Trim().ToUpper().Equals("CERRADO") && 
                        !estado.Trim().ToUpper().Equals("CANCELADO") && !estado.Trim().ToUpper().Equals("TODOS"))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo estado contiene un valor invalido.");
                    }                        
                    else
                    {
                        switch (estado)
                        {
                            case "ABIERTO":
                                estado = "open";
                                break;
                            case "CERRADO":
                                estado = "closed";
                                break;
                            case "CANCELADO":
                                estado = "cancelled";
                                break;
                            case "TODOS":
                                estado = "any";
                                break;
                        }
                    }                    

                    string userAgent = usu.RazonSocial.ToUpper() + " (" + usu.Email.Trim() + ")";

                    var resultado = TiendaNubeCommon.ObtenerOrdenes(usu.TiendaNubeIdTienda, usu.TiendaNubeToken, userAgent, desdeIdOrden, hastaIdOrden, estado, usu);
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
    }
}