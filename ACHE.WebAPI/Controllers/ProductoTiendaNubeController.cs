using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Negocio.Common;
using ACHE.Negocio.Productos;

namespace ACHE.WebAPI.Controllers
{
    public class ProductoTiendaNubeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage consultarProducto(string token, string idProducto, string idVariante)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(idProducto))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo idProducto obligatorio");
                    else if (string.IsNullOrWhiteSpace(idVariante))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo idVariante obligatorio");

                    string userAgent = usu.RazonSocial.ToUpper() + " (" + usu.Email.Trim() + ")";

                    var resultado = TiendaNubeCommon.ConsultarProducto(usu.TiendaNubeIdTienda, usu.TiendaNubeToken, userAgent, idProducto, idVariante, usu);

                    if (resultado != null)
                        return Request.CreateResponse(HttpStatusCode.OK, resultado);
                    else
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Producto no encontrado."); 
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
        public HttpResponseMessage crearProducto(Models.ProductoTiendaNube prod)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(prod.token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(prod.identificadorUrl))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo identificadorUrl obligatorio");
                    else if (string.IsNullOrWhiteSpace(prod.nombre))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo nombre obligatorio");
                    else if (string.IsNullOrWhiteSpace(prod.precio))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo precio obligatorio");
                    else if (prod.stock < 0)
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo stock obligatorio");
                    else if (string.IsNullOrWhiteSpace(prod.sku))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo sku obligatorio");
                    else if (string.IsNullOrWhiteSpace(prod.mostrarEnTienda))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo mostrarEnTienda obligatorio");
                    else if (!prod.mostrarEnTienda.Equals("SI") && !prod.mostrarEnTienda.Equals("NO"))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo mostrarEnTienda admite el valor SI o NO");
                    else if (string.IsNullOrWhiteSpace(prod.enviaSinCargo))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo enviaSinCargo obligatorio");
                    else if (!prod.enviaSinCargo.Equals("SI") && !prod.enviaSinCargo.Equals("NO"))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo enviaSinCargo admite el valor SI o NO");
                    else if (string.IsNullOrWhiteSpace(prod.esVariante))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo esVariante obligatorio");
                    else if (!prod.esVariante.Equals("SI") && !prod.esVariante.Equals("NO"))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo esVariante admite el valor SI o NO");
                    else if (prod.esVariante.Equals("SI") && prod.IdNubePadre.Equals(""))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo IdNubePadre es obligatorio si esVariante tiene el valor SI");
                    else if (string.IsNullOrWhiteSpace(prod.categoria) && !string.IsNullOrWhiteSpace(prod.subCategoria))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo categoria olbigatorio si el campo subcategoria tiene datos");
                    else if (string.IsNullOrWhiteSpace(usu.TiendaNubeIdTienda))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El usuario no tiene una tienda de tiendaNube registrada en ELUM");
                    else if (string.IsNullOrWhiteSpace(usu.TiendaNubeToken))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El usuario no tiene token generado en tiendaNube");

                    if (prod.esVariante.Equals("SI"))
                    {
                        if (string.IsNullOrWhiteSpace(prod.nombrePropiedad1) || string.IsNullOrWhiteSpace(prod.valorPropiedad1))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo nombrePropiedad1 y valorPropiedad1 obligatorios si es una variante.");
                    }

                    if (!string.IsNullOrWhiteSpace(prod.nombrePropiedad1) || !string.IsNullOrWhiteSpace(prod.valorPropiedad1)) //Quiero agregar un atributo.                    
                        if (string.IsNullOrWhiteSpace(prod.nombrePropiedad1) || string.IsNullOrWhiteSpace(prod.valorPropiedad1))                        
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo nombrePropiedad1 y valorPropiedad1 obligatorios.");

                    if (!string.IsNullOrWhiteSpace(prod.nombrePropiedad2) || !string.IsNullOrWhiteSpace(prod.valorPropiedad2)) //Quiero agregar un atributo.                    
                        if (string.IsNullOrWhiteSpace(prod.nombrePropiedad2) || string.IsNullOrWhiteSpace(prod.valorPropiedad2) ||
                            string.IsNullOrWhiteSpace(prod.nombrePropiedad1) || string.IsNullOrWhiteSpace(prod.valorPropiedad1))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo nombrePropiedad2 y valorPropiedad2 obligatorios.");
                        else if (prod.nombrePropiedad1 == prod.nombrePropiedad2)
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El nombre de las propiedades no pueden repetirse.");

                    if (!string.IsNullOrWhiteSpace(prod.nombrePropiedad3) || !string.IsNullOrWhiteSpace(prod.valorPropiedad3)) //Quiero agregar un atributo.                    
                        if (string.IsNullOrWhiteSpace(prod.nombrePropiedad3) || string.IsNullOrWhiteSpace(prod.valorPropiedad3) ||
                            string.IsNullOrWhiteSpace(prod.nombrePropiedad2) || string.IsNullOrWhiteSpace(prod.valorPropiedad2) ||
                            string.IsNullOrWhiteSpace(prod.nombrePropiedad1) || string.IsNullOrWhiteSpace(prod.valorPropiedad1))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo nombrePropiedad3 y valorPropiedad3 obligatorios.");
                        else if ((prod.nombrePropiedad1 == prod.nombrePropiedad2) || (prod.nombrePropiedad1 == prod.nombrePropiedad3) ||
                                 (prod.nombrePropiedad2 == prod.nombrePropiedad3))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El nombre de las propiedades no pueden repetirse.");

                    string userAgent = usu.RazonSocial.ToUpper() + " (" + usu.Email.Trim() + ")";

                    int idProducto = TiendaNubeCommon.CrearProducto(usu.TiendaNubeIdTienda, usu.TiendaNubeToken, userAgent, prod.identificadorUrl, prod.nombre, prod.categoria, prod.subCategoria,
                                                                    prod.nombrePropiedad1, prod.valorPropiedad1, prod.nombrePropiedad2, prod.valorPropiedad2, prod.nombrePropiedad3, 
                                                                    prod.valorPropiedad3, prod.precio, prod.precioPromocional, prod.peso, prod.stock, prod.sku, prod.codigoDeBarras,
                                                                    prod.mostrarEnTienda, prod.enviaSinCargo, prod.descripcion, prod.tags, prod.tituloParaSEO, prod.descripcionParaSEO, 
                                                                    prod.marca, prod.esVariante, prod.imagen, prod.IdNubePadre, usu);

                    return Request.CreateResponse(HttpStatusCode.OK, idProducto.ToString());
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


        [HttpDelete]
        public HttpResponseMessage eliminarProducto(string token, string idProducto, string idVariante)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(idProducto))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo idProducto obligatorio");
                    else if (string.IsNullOrWhiteSpace(idVariante))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo idVariante obligatorio");

                    string userAgent = usu.RazonSocial.ToUpper() + " (" + usu.Email.Trim() + ")";

                    if (TiendaNubeCommon.EliminarProducto(usu.TiendaNubeIdTienda, usu.TiendaNubeToken, userAgent, idProducto, idVariante, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El producto o variante no existe");
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


        [HttpPut]
        public HttpResponseMessage modificarProducto(Models.ProductoTiendaNube prod)
        {
            try
            {
                var idUsuario = TokenCommon.validarToken(prod.token);
                if (idUsuario > 0)
                {
                    var usu = TokenCommon.ObtenerWebUser(idUsuario);

                    if (string.IsNullOrWhiteSpace(prod.IdNubePadre))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo IdNubePadre obligatorio");
                    else if (string.IsNullOrWhiteSpace(prod.sku))
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo sku obligatorio");
                    if (!string.IsNullOrWhiteSpace(prod.mostrarEnTienda))
                    {
                        if (!prod.mostrarEnTienda.Equals("SI") && !prod.mostrarEnTienda.Equals("NO"))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo mostrarEnTienda admite el valor SI o NO");
                    }
                    if (!string.IsNullOrWhiteSpace(prod.enviaSinCargo))
                    {
                        if (!prod.enviaSinCargo.Equals("SI") && !prod.enviaSinCargo.Equals("NO"))
                            return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Campo enviaSinCargo admite el valor SI o NO");
                    }
             
                    string userAgent = usu.RazonSocial.ToUpper() + " (" + usu.Email.Trim() + ")";

                    if (TiendaNubeCommon.ModificarProducto(usu.TiendaNubeIdTienda, usu.TiendaNubeToken, userAgent, prod.IdNubePadre, prod.sku, prod.precio, prod.precioPromocional,
                                                           prod.stock, prod.mostrarEnTienda, prod.enviaSinCargo, prod.imagen, usu))
                        return Request.CreateResponse(HttpStatusCode.OK, "OK");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El producto no existe");
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