using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Model.Negocio.Licencia;
using ACHE.Negocio.Common;
using ACHE.Negocio.Productos;
using ACHE.WebAPI.Models;
using Newtonsoft.Json;

namespace ACHE.WebAPI.Controllers
{
    public class LicenciaController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage registrarEquipo(PostRequestLicencia lic)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(lic.token))
                {
                    Error e = new Error();
                    e.codigo = "412";
                    e.mensaje = "Entidad no procesable";
                    e.descripcion = "Campo token obligatorio";
                    return Request.CreateResponse(HttpStatusCode.PreconditionFailed, e);
                }
                if (string.IsNullOrWhiteSpace(lic.serial))
                {
                    Error e = new Error();
                    e.codigo = "412";
                    e.mensaje = "Entidad no procesable";
                    e.descripcion = "Campo serial obligatorio";
                    return Request.CreateResponse(HttpStatusCode.PreconditionFailed, e);
                }

                var idUsuario = TokenCommon.validarToken(lic.token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);
              
                    PostResponseLicencia resultado = LicenciaCommon.registrarEquipo(lic, usu);

                    if(resultado != null)
                        return Request.CreateResponse(HttpStatusCode.OK, resultado);
                    else
                    {
                        Error e = new Error();
                        e.codigo = "404";
                        e.mensaje = "Not found";
                        e.descripcion = "Cliente no encontrado";
                        return Request.CreateResponse(HttpStatusCode.NotFound, e);
                    }
                }
                else
                {
                    Error e = new Error();
                    e.codigo = "403";
                    e.mensaje = "Autorización";
                    e.descripcion = "Token inválido";
                    return Request.CreateResponse(HttpStatusCode.Forbidden, e);
                }
                    
            }
            catch (CustomException ex)
            {
                Error e = new Error();
                e.codigo = "412";
                e.mensaje = ex.Message;
                e.descripcion = ex.InnerException.InnerException.ToString();
                return Request.CreateResponse(HttpStatusCode.PreconditionFailed, e);
            }
            catch (Exception ex)
            {
                Error e = new Error();
                e.codigo = "500";
                e.mensaje = ex.Message;
                e.descripcion = ex.InnerException.InnerException.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
