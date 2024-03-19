using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.ViewModels;
using System.IO;
using System.Configuration;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using ACHE.Model.Negocio.TiendaNube;
using System.Dynamic;
using ACHE.Negocio.Helper;
using ACHE.Model.Negocio.Licencia;

namespace ACHE.Negocio.Common
{
    public class LicenciaCommon
    {
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static PostResponseLicencia registrarEquipo(PostRequestLicencia lic, WebUser usu)
        {
            try
            {

                //if (idProducto.Equals(idVariante))
                //{
                //    //Elimino el producto por el id
                //    var clientProductsDelete = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idProducto.Trim());
                //    var requestProductsDelete = new RestRequest(Method.DELETE);
                //    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //    requestProductsDelete.AddHeader("Content-Type", "application/json");
                //    requestProductsDelete.AddHeader("User-Agent", tiendaUserAgent);
                //    requestProductsDelete.AddHeader("Authentication", tiendaToken);
                //    IRestResponse responseProductsDelete = clientProductsDelete.Execute(requestProductsDelete);
                //    if (responseProductsDelete.StatusCode != HttpStatusCode.OK)
                //    {
                //        if (responseProductsDelete.StatusCode == HttpStatusCode.NotFound)
                //            return false;
                //        else
                //            throw new Exception(responseProductsDelete.StatusCode.ToString());
                //    }
                //    else
                //    {
                //        //Se elimino correctamente el producto en tienda nube
                //        return true;
                //    }
                //}
                //else
                //{
                //    //Elimino la variante por el id
                //    var clientVariantsDelete = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idProducto.Trim() + "/variants/" + idVariante.Trim());
                //    var requestVariantsDelete = new RestRequest(Method.DELETE);
                //    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //    requestVariantsDelete.AddHeader("Content-Type", "application/json");
                //    requestVariantsDelete.AddHeader("User-Agent", tiendaUserAgent);
                //    requestVariantsDelete.AddHeader("Authentication", tiendaToken);
                //    IRestResponse responseVariantsDelete = clientVariantsDelete.Execute(requestVariantsDelete);
                //    if (responseVariantsDelete.StatusCode != HttpStatusCode.OK)
                //    {
                //        if (responseVariantsDelete.StatusCode == HttpStatusCode.NotFound)
                //            return false;
                //        else
                //            throw new Exception(responseVariantsDelete.StatusCode.ToString());
                //    }
                //    else
                //    {
                //        //Se elimino correctamente el producto en tienda nube
                //        return true;
                //    }
                //}

                PostResponseLicencia p = new PostResponseLicencia();

                using (var dbContext = new ACHEEntities())
                {
                    long UsuarioCuit = Convert.ToInt64(usu.CUIT);
                    LicenciaTemp lt = dbContext.LicenciaTemp.Where(x => x.CUIT == UsuarioCuit).FirstOrDefault();

                    if (lt != null)
                    {                        
                        p.cliente = usu.IDUsuario.ToString();
                        p.plan = usu.IDPlan.ToString();
                        p.vigencia = lt.Vigencia.Substring(0,4) + "-" + lt.Vigencia.Substring(4, 2) + "-" + lt.Vigencia.Substring(6, 2);
                        p.serial = lic.serial;
                        p.clave = lt.Clave;
                        List<Modulos> lm = new List<Modulos>();
                        if(!string.IsNullOrWhiteSpace(lt.Modulo1_Nombre) &&
                           !string.IsNullOrWhiteSpace(lt.Modulo1_Version) &&
                           !string.IsNullOrWhiteSpace(lt.Modulo1_UrlInstalador32) &&
                           !string.IsNullOrWhiteSpace(lt.Modulo1_UrlInstalador64))
                        {
                            Modulos m = new Modulos();
                            m.nombre = lt.Modulo1_Nombre.ToUpper().Trim();
                            m.version = lt.Modulo1_Version.Substring(0,3) + "." + lt.Modulo1_Version.Substring(3, 3) + "." + lt.Modulo1_Version.Substring(6, 3);
                            m.urlInstalador32 = lt.Modulo1_UrlInstalador32.Trim();
                            m.urlInstalador64 = lt.Modulo1_UrlInstalador64.Trim();
                            lm.Add(m);
                        }

                        if (!string.IsNullOrWhiteSpace(lt.Modulo2_Nombre) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo2_Version) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo2_UrlInstalador32) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo2_UrlInstalador64))
                        {
                            Modulos m = new Modulos();
                            m.nombre = lt.Modulo2_Nombre.ToUpper().Trim();
                            m.version = lt.Modulo2_Version.Substring(0, 3) + "." + lt.Modulo2_Version.Substring(3, 3) + "." + lt.Modulo2_Version.Substring(6, 3);
                            m.urlInstalador32 = lt.Modulo2_UrlInstalador32.Trim();
                            m.urlInstalador64 = lt.Modulo2_UrlInstalador64.Trim();
                            lm.Add(m);
                        }

                        if (!string.IsNullOrWhiteSpace(lt.Modulo3_Nombre) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo3_Version) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo3_UrlInstalador32) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo3_UrlInstalador64))
                        {
                            Modulos m = new Modulos();
                            m.nombre = lt.Modulo3_Nombre.ToUpper().Trim();
                            m.version = lt.Modulo3_Version.Substring(0, 3) + "." + lt.Modulo3_Version.Substring(3, 3) + "." + lt.Modulo3_Version.Substring(6, 3);
                            m.urlInstalador32 = lt.Modulo3_UrlInstalador32.Trim();
                            m.urlInstalador64 = lt.Modulo3_UrlInstalador64.Trim();
                            lm.Add(m);
                        }

                        if (!string.IsNullOrWhiteSpace(lt.Modulo4_Nombre) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo4_Version) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo4_UrlInstalador32) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo4_UrlInstalador64))
                        {
                            Modulos m = new Modulos();
                            m.nombre = lt.Modulo4_Nombre.ToUpper().Trim();
                            m.version = lt.Modulo4_Version.Substring(0, 3) + "." + lt.Modulo4_Version.Substring(3, 3) + "." + lt.Modulo4_Version.Substring(6, 3);
                            m.urlInstalador32 = lt.Modulo4_UrlInstalador32.Trim();
                            m.urlInstalador64 = lt.Modulo4_UrlInstalador64.Trim();
                            lm.Add(m);
                        }


                        if (!string.IsNullOrWhiteSpace(lt.Modulo5_Nombre) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo5_Version) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo5_UrlInstalador32) &&
                            !string.IsNullOrWhiteSpace(lt.Modulo5_UrlInstalador64))
                        {
                            Modulos m = new Modulos();
                            m.nombre = lt.Modulo5_Nombre.ToUpper().Trim();
                            m.version = lt.Modulo5_Version.Substring(0, 3) + "." + lt.Modulo5_Version.Substring(3, 3) + "." + lt.Modulo5_Version.Substring(6, 3);
                            m.urlInstalador32 = lt.Modulo5_UrlInstalador32.Trim();
                            m.urlInstalador64 = lt.Modulo5_UrlInstalador64.Trim();
                            lm.Add(m);
                        }

                        if(lm.Count > 0)
                            p.modulos = lm;

                        return p;

                    }
                    else
                    {
                        return null;
                    }

                }
                

            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



    }
}
