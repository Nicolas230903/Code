using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Negocio.Common;

namespace ACHE.WebAPI.Controllers
{
    public class DatosPersonaAfipController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage DatosPersonaAfip(string token, string cuit)
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

                    var resultado = DatosPersonaAfipCommon.ObtenerDatosPersona(Convert.ToInt64(cuit), usu);
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