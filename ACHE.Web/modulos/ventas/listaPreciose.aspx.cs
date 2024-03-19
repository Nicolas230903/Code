using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using FileHelpers;
using ACHE.Negocio.Productos;

public partial class modulos_ventas_listaPreciose : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            litPath.Text = "Alta";

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";
                }
            }

            if (CurrentUser.UsaPrecioFinalConIVA)
            {
                liPrecioUnitario.Text = "Precio unit. con IVA";
                liPrecioLista.Text = "Precio lista con IVA";
            }
            else
            {
                liPrecioUnitario.Text = "Precio unit. sin IVA";
                liPrecioLista.Text = "Precio lista sin IVA";
            }
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.ListaPrecios.Where(x => x.IDListaPrecio == id && x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                txtNombre.Text = entity.Nombre.ToUpper();
                txtObservaciones.Text = entity.Observaciones.ToString();
                ddlActivo.SelectedValue = (entity.Activa) ? "1" : "0";
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, string nombre, string Observaciones,List<PreciosConceptos> listaDePrecios , int activo )
    {
        //string listaDePrecios;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            ListaPreciosCommon.GuardarListaDePrecio(id, nombre, Observaciones, activo, listaDePrecios, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadoslistaPreciosViewModel ObtenerListaPrecios(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ListaPreciosCommon.ListaDePrecios(id, usu);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }
}