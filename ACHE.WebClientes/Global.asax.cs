using ACHE.WebClientes.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ACHE.WebClientes
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);

                newUser.IDPersonaPwd = serializeModel.IDPersonaPwd;
                newUser.RazonSocial = serializeModel.RazonSocial;
                newUser.Documento = serializeModel.Documento;
                newUser.TipoDocumento = serializeModel.TipoDocumento;
                newUser.Email = serializeModel.Email;
                newUser.Token = serializeModel.Token;
                HttpContext.Current.User = newUser;
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //Para que esto funcione tiene que estar el archivo PrecompiledApp.config en el server
            if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http://") && HttpContext.Current.Request.IsLocal.Equals(false))
            {
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", "https://clientes.contabilium.com");
            }
        }
    }
}
