using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Model;

public partial class Front : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var plan = PermisosModulos.ObtenerPlanActual(dbContext, usu.IDUsuario);
                if (plan != null && plan.IDPlan > 1)
                {
                    if (HttpContext.Current.Request.IsLocal.Equals(false) && HttpContext.Current.Request.Url.ToString().ToLower().Substring(0, 18).Contains("app"))
                    {
                        liZopin.Text = "";
                    }
                }
                else { liZopin.Text = ""; }
            }
        }
    }
}