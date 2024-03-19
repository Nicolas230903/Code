using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Negocio.Common;

namespace ACHE.WebAPI.Controllers
{
    public class ComunicacionesAfipController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ObtenerComunicacionesAfip(string token, string cuit, 
                                                             long desdeIdComunicacion, long hastaIdComunicacion, 
                                                             string desdeFecha, string hastaFecha, 
                                                             string adjunto)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrEmpty(cuit))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo CUIT obligatorio.");
                    else
                        if (!IsValidCUIT(cuit))
                           return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El CUIT es inválido.");

                    if (desdeIdComunicacion < 0)
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo desdeIdComunicacion debe ser 0 o mayor.");

                    if (hastaIdComunicacion != -1)
                    {
                        if (hastaIdComunicacion == 0)                        
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo hastaIdComunicacion no puede ser 0.");                        
                        else                        
                            if (hastaIdComunicacion < desdeIdComunicacion)                            
                                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo hastaIdComunicacion debe ser mayor o igual a al campo desdeIdComunicacion."); 
                    }  

                    if (string.IsNullOrEmpty(desdeFecha))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo desdeFecha obligatorio."); 
                    else
                        if (!ValidarFecha(desdeFecha))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El campo desdeFecha es invalido.");

                    if (string.IsNullOrEmpty(hastaFecha))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo hastaFecha obligatorio.");
                    else
                        if (!ValidarFecha(hastaFecha))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El campo hastaFecha es invalido.");

                    DateTime dfechaDesde = new DateTime(Convert.ToInt16(desdeFecha.Substring(0, 4)), Convert.ToInt16(desdeFecha.Substring(4, 2)), Convert.ToInt16(desdeFecha.Substring(6, 2)));
                    DateTime dfechaHasta = new DateTime(Convert.ToInt16(hastaFecha.Substring(0, 4)), Convert.ToInt16(hastaFecha.Substring(4, 2)), Convert.ToInt16(hastaFecha.Substring(6, 2)));

                    if(dfechaDesde > dfechaHasta)
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El campo hastaFecha debe ser mayor o igual a fechaDesde.");


                    bool descargarAdjuntos = false;
                    if (string.IsNullOrEmpty(adjunto))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo adjunto obligatorio.");
                    }
                    else
                    {
                        if (!adjunto.Equals("SI") && !adjunto.Equals("NO"))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El campo adjunto solo admite los parametros SI y NO.");
                        else
                        {
                            switch (adjunto)
                            {
                                case "SI":
                                    descargarAdjuntos = true;
                                    break;
                                case "NO":
                                    descargarAdjuntos = false;
                                    break;
                            }
                        }
                    }

                    if (descargarAdjuntos)
                    {
                        var resultado = ComunicacionesAfipCommon.ObtenerComunicacionesAfipConAdjunto(cuit, desdeIdComunicacion, hastaIdComunicacion, desdeFecha, hastaFecha, usu);
                        return Request.CreateResponse(HttpStatusCode.OK, resultado);
                    }
                    else
                    {
                        var resultado = ComunicacionesAfipCommon.ObtenerComunicacionesAfipSinAdjunto(cuit, desdeIdComunicacion, hastaIdComunicacion, desdeFecha, hastaFecha, usu);
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

        public static bool IsValidCUIT(string cuit)
        {
            if (cuit == null)
            {
                return false;
            }

            cuit = cuit.Replace("-", string.Empty);
            if (cuit.Length != 11)
            {
                return false;
            }
            else
            {
                int calculado = CalcularDigitoCuil(cuit);
                int digito = int.Parse(cuit.Substring(10));
                return calculado == digito;
            }
        }

        private static int CalcularDigitoCuil(string cuil)
        {
            int[] mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            char[] nums = cuil.ToCharArray();
            int total = 0;
            for (int i = 0; i < mult.Length; i++)
            {
                total += int.Parse(nums[i].ToString()) * mult[i];
            }
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

        private static bool ValidarFecha(string fecha)
        {
            DateTime result;
            if (!DateTime.TryParseExact(
                    fecha,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out result))
            {
                return false;
            };
            return true;
        }

    }
}