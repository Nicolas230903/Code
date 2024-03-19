using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de PaginaBase
/// </summary>
public class BasePage : System.Web.UI.Page
{
    public WebUser CurrentUser
    {
        get { return (Session["CurrentUser"] != null) ? (WebUser)Session["CurrentUser"] : null; }
        set { Session["CurrentUser"] = value; }
    }

    private void ValidateUser()
    {
        string pageName = Request.FilePath.Substring(Request.FilePath.LastIndexOf(@"/") + 1).ToLower();
        if (pageName != "login.aspx")
        {
            if (CurrentUser == null)
            {
                Response.Redirect("/login.aspx");
            }
            else
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                if (!usu.SetupFinalizado && pageName != "finregistro.aspx" && pageName != "faq.aspx" 
                    && pageName != "personase.aspx" && pageName != "mis-datos.aspx" && pageName != "importar.aspx" && pageName != "elegir-plan.aspx" 
                    && pageName != "pagodeplanes.aspx" && pageName != "comprasMercadopago.aspx"
                    && pageName != "cambiar-pwd.aspx" && pageName != "usuarios.aspx" && pageName != "usuariose.aspx")
                {
                    Response.Redirect("/finRegistro.aspx");
                }

                //if (!PermisosModulos.tienePlan(pageName))
                //    Response.Redirect("/modulos/seguridad/elegir-plan.aspx?upgrade=4&tipo=" + pageName.Replace(".aspx", ""));

                if (CurrentUser.TipoUsuario == "B" && (pageName != "home.aspx"))
                {
                    if (!PermisosModulos.tieneAccesoAlModulo(pageName))
                        Response.Redirect("/abono.aspx");
                }

                if (pageName != "home.aspx")
                {
                    if (!PermisosModulos.tieneAccesoAlFormulario(pageName))
                    {
                        Response.Redirect("/modulos/seguridad/elegir-plan.aspx?upgrade=" + usu.IDPlan.ToString() + "&tipo =" + pageName.Replace(".aspx", ""));
                    }
                    
                }

                if (!CurrentUser.PlanVigente && pageName != "elegir-plan.aspx" && pageName != "pagodeplanes.aspx")
                {
                    Response.Redirect("/modulos/seguridad/elegir-plan.aspx");
                }
            }
        }
    }

    protected override void OnInit(EventArgs e)
    {
        if (CurrentUser == null)//Esto es para evitar CSRF Attacks
        {
            ViewStateUserKey = Session.SessionID;
        }
        base.OnInit(e);
    }

    public string GetUserIP()
    {
        string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (!string.IsNullOrEmpty(ipList))
        {
            return ipList.Split(',')[0];
        }

        return Request.ServerVariables["REMOTE_ADDR"];
    }

    protected override void OnPreInit(EventArgs e)
    {
        ValidateUser();
    }
}