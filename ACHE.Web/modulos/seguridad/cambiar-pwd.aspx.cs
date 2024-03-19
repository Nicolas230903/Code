using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class cambiar_pwd : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(true)]
    public static void guardar(string actual, string nueva)
    {
        if (!Common.validarPassword(nueva))
            throw new Exception("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");

        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var verificada = nueva;
                UsuarioCommon.CambiarPassword(Common.MD5Hash(actual), Common.MD5Hash(nueva), Common.MD5Hash(verificada), usu);
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