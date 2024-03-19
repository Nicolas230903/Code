using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Model;
using ACHE.Extensions;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using ACHE.FacturaElectronica;

namespace ACHE.WebAPI.Controllers
{
    public class RequestController : ApiController
    {

        public HttpResponseMessage getFactura(string token, int idComprobante, int idPersona)
        {
            HttpResponseMessage resul = new HttpResponseMessage();

            try
            {
                //var nroComprobanteCae = generarComprobante("asd", idComprobante, idPersona);

                if (string.IsNullOrEmpty(token))
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Debe pasar su token como parámetro");
                else if (idComprobante == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Debe pasar su idComprobante como parámetro");
                else if (idPersona == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Debe pasar su idPersona como parámetro");
                else
                {
                    //var msg = "Entro al metodo";
                    //BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApiError"]), msg, null);

                    using (var dbContext = new ACHEEntities())
                    {
                        var auth = dbContext.AuthenticationTokenClientes.Where(x => x.Token == token).FirstOrDefault();
                        if (auth != null)
                        {
                            var fecha = DateTime.Now;
                            if (auth.FechaExpiracion > fecha)
                            {
                                var comp = dbContext.Comprobantes.Include("Personas").Where(x => x.IDComprobante == idComprobante).FirstOrDefault();
                                if (comp != null)
                                {
                                    if (comp.Personas.IDPersona == idPersona)
                                    {
                                        var fileName = comp.Tipo + "-" + comp.PuntosDeVenta.Punto.ToString("#0000") + "-" + comp.Numero.ToString("#00000000") + ".pdf";
                                        var PathBase = ConfigurationManager.AppSettings["PathBaseWeb"];
                                        string localFilePath = PathBase + "//files//explorer//" + comp.IDUsuario.ToString() + "//comprobantes//" + comp.FechaComprobante.Year.ToString() + "//";

                                        string url = localFilePath + fileName;
                                        if (!System.IO.File.Exists(url))
                                        {
                                            fileName = comp.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + fileName;
                                            url = localFilePath + fileName;
                                        }
                                        
                                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                                        response.Content = new StreamContent(new FileStream(url, FileMode.Open, FileAccess.Read));
                                        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                                        response.Content.Headers.ContentDisposition.FileName = fileName;

                                        return response;

                                    }
                                    else
                                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "El comprobante no corresponde a la persona indicada con el parametro persona");
                                }
                                else
                                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se ha encontrado ningún comprobante con el ID ingresado");
                            }
                            else
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Su token ya venció, por favor solicite uno nuevo");
                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Su tokenes inválido, por favor solicite uno nuevo");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage RegistrarUsuario(string cuit, string empresa, string email, string pwd, string contacto)
        {
            cuit = cuit.Replace("-", "").Trim();
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    //var auth = dbContext.AuthenticationToken.Where(x => x.Token == token).FirstOrDefault();
                    //if (auth == null)
                    //    throw new Exception(("Token invalido");
                    //if (auth.FechaExpiracion > DateTime.Now)
                    //    throw new Exception(("Su token ya venció, por favor solicite uno nuevo");

                    if (string.IsNullOrEmpty(pwd))
                        throw new Exception("El password no puede esta vacío");
                    if (!cuit.IsValidCUIT())
                        throw new Exception("El CUIT ingresado no es valido");

                    guardar(cuit, email, pwd, empresa, contacto);

                  //  return Ok("Usuario registrado correctamente");
                    return Request.CreateResponse(HttpStatusCode.OK, "Usuario registrado correctamente");
                }
            }
            catch (Exception ex)
            {
                //return Ok(ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        private static void guardar(string cuit, string email, string pwd, string empresa, string contacto)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.Usuarios.Any(x => x.Email == email))
                    throw new Exception("El E-mail ingresado ya se encuentra registrado.");
                else if (dbContext.Usuarios.Any(x => x.CUIT == cuit))
                    throw new Exception("El CUIT ingresado ya se encuentra registrado.");
                else
                {
                    Usuarios entity = new Usuarios();

                    entity.RazonSocial = empresa; //razonSocial;
                    entity.FechaInicioActividades = null;
                    entity.CondicionIva = string.Empty; //condicionIva;
                    entity.CUIT = cuit;
                    entity.IIBB = string.Empty;
                    entity.Personeria = string.Empty; //personeria;
                    entity.Email = email;
                    entity.EmailAlertas = email;
                    entity.Pwd = pwd;
                    entity.Telefono = string.Empty;
                    entity.Celular = string.Empty;
                    entity.Contacto = contacto;

                    //Domicilio
                    entity.Pais = "Argentina";
                    entity.IDProvincia = 2;//Ciudad de Buenos Aires
                    entity.IDCiudad = 5003;//SIN IDENTIFICAR
                    entity.Domicilio = string.Empty;
                    entity.PisoDepto = string.Empty;
                    entity.CodigoPostal = string.Empty;

                    entity.Theme = "default";
                    entity.TemplateFc = "default";
                    entity.FechaAlta = DateTime.Now;
                    entity.FechaUltLogin = DateTime.Now;
                    entity.TieneFacturaElectronica = false;
                    entity.Logo = null;
                    entity.Activo = true;
                    entity.IDPlan = 4;
                    entity.FechaFinPlan = DateTime.Now.AddDays(30);
                    entity.UsaFechaFinPlan = true;
                    entity.SetupRealizado = false;
                    entity.UsaProd = false;
                    entity.IDUsuarioPadre = null;
                    entity.CodigoPromo = string.Empty;

                    entity.EsAgentePercepcionIVA = false;
                    entity.EsAgentePercepcionIIBB = false;
                    entity.EsAgenteRetencion = false;


                    try
                    {
                        PuntosDeVenta punto = new PuntosDeVenta();
                        punto.FechaAlta = DateTime.Now;
                        punto.Punto = 1;
                        punto.PorDefecto = true;
                        entity.PuntosDeVenta.Add(punto);

                        if (cuit.StartsWith("30"))
                        {
                            punto = new PuntosDeVenta();
                            punto.FechaAlta = DateTime.Now;
                            punto.Punto = 2;
                            punto.PorDefecto = false;
                            entity.PuntosDeVenta.Add(punto);
                        }

                        Categorias cat = new Categorias();
                        cat.Nombre = "General";
                        entity.Categorias.Add(cat);

                        Categorias cat2 = new Categorias();
                        cat2.Nombre = "Honorarios";
                        entity.Categorias.Add(cat2);

                        Categorias cat3 = new Categorias();
                        cat3.Nombre = "Gastos varios";
                        entity.Categorias.Add(cat3);

                        Categorias cat4 = new Categorias();
                        cat4.Nombre = "Nafta";
                        entity.Categorias.Add(cat4);

                        Categorias cat5 = new Categorias();
                        cat5.Nombre = "Equipamiento";
                        entity.Categorias.Add(cat5);

                        Bancos banco = new Bancos();
                        banco.Moneda = "Pesos Argentinos";
                        banco.SaldoInicial = 0;
                        banco.NroCuenta = "";
                        banco.IDBancoBase = dbContext.BancosBase.Where(x => x.Nombre == "Default").FirstOrDefault().IDBancoBase;
                        banco.FechaAlta = DateTime.Now;
                        banco.Activo = true;
                        entity.Bancos.Add(banco);

                        dbContext.Usuarios.Add(entity);

                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

        }
    }
}