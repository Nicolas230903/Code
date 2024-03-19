using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;

public partial class modulos_ventas_aumentoMasivoPrecios : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            CargarPersonas();
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static void Guardar(int idListaPrecios, decimal porcentaje, string actualizarTodos, int idPersona)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    if (actualizarTodos == "1")
                    {
                        List<Conceptos> productos;
                        if (idPersona > 0)
                            productos = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona).ToList();
                        else
                            productos = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario).ToList();

                        foreach (var item in productos)
                            item.PrecioUnitario = item.PrecioUnitario + ((item.PrecioUnitario * porcentaje) / 100);
                    }
                    else
                    {
                        List<PreciosConceptos> productos;
                        if (idPersona > 0)
                            productos = dbContext.PreciosConceptos.Where(x => x.IDListaPrecios == idListaPrecios && x.Conceptos.IDPersona == idPersona && x.Conceptos.IDUsuario == usu.IDUsuario && x.Precio > 0).ToList();
                        else
                            productos = dbContext.PreciosConceptos.Where(x => x.IDListaPrecios == idListaPrecios && x.Conceptos.IDUsuario == usu.IDUsuario && x.Precio > 0).ToList();

                        foreach (var item in productos)
                            if (item.Precio > 0)
                                item.Precio = item.Precio + ((item.Precio * porcentaje) / 100);
                    }

                    dbContext.SaveChanges();
                }
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

    private void CargarPersonas()
    {
        using (var dbContext = new ACHEEntities())
        {
            var listaPersonas = dbContext.Conceptos.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPersona != null);
            ddlPersonas.Items.Add(new ListItem("", ""));
            var nombre="";
            foreach (var item in listaPersonas)
            {
                nombre = item.Personas.NombreFantansia == "" ? item.Personas.RazonSocial.ToUpper() : item.Personas.NombreFantansia.ToUpper();
                ddlPersonas.Items.Add(new ListItem(nombre, item.IDPersona.ToString()));
            }
        }
    }
}