using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class shopping : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(true)]
    public static void PaquetesShopping(string mensaje, string tipoPaquete)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<MENSAJE>", mensaje);
            replacements.Add("<USUARIO>", usu.RazonSocial);
            replacements.Add("<ID>", usu.IDUsuario);
            replacements.Add("<EMAIL>", usu.Email);
            replacements.Add("<PAQUETE>", tipoPaquete);
            replacements.Add("<NOTIFICACION>", "Hemos recibido su mensaje. Le responderemos a la brevedad");


            bool send = EmailHelper.SendMessage(EmailTemplate.Compras, replacements, ConfigurationManager.AppSettings["Email.Compras"], "axanweb: Solicitud de ayuda");
            if (!send)
            {
                throw new Exception("El mensaje no pudo ser enviado. Por favor, escribenos a <a href='mailto:ayuda@axanweb.com'>ayuda@axanweb.com</a>");
            }

            send = EmailHelper.SendMessage(EmailTemplate.Notificacion, replacements, usu.Email, "axanweb: Hemos recibido su consulta.");
        }
        else
        {
            throw new Exception("Por favor, vuelva a iniciar sesión");
        }
    }
}