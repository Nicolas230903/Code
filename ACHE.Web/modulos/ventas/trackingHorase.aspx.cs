using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class modulos_ventas_trackingHorase : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.HerramientasTrackingDeHoras)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            litTitulo.Text = "<i class='fa fa-university'></i> Tracking Horas";
            litPathPadre.Text = "<a href='/Modulos/ventas/trackingHorase.aspx'>Tracking Horas</a>";

            litPath.Text = "Alta";

            using (var dbContext = new ACHEEntities())
            {

                var listaUsuariosAdicionales = dbContext.UsuariosAdicionales.Where(x => x.IDUsuario == CurrentUser.IDUsuario);

                ddlUuarios.Items.Add(new ListItem(CurrentUser.RazonSocial,""));
                foreach (var item in listaUsuariosAdicionales)
                    ddlUuarios.Items.Add(new ListItem(item.Email, item.IDUsuarioAdicional.ToString()));
                
              
            }

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";
                }
            }

        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.TrackingHoras.Where(x=> x.IDTrackingHoras== id && x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                hdnIDPersona.Value = entity.IDPersona.ToString();
                txtFecha.Text = entity.Fecha.ToString("dd/MM/yyyy");
                //ddlTarea.SelectedValue = entity.Tarea;
                ddlTarea.Text = entity.Tarea;
                txtCantHoras.Text = entity.Horas.ToString();
                ddlEstado.SelectedValue = entity.Estado;
                txtObservaciones.Text = entity.Observaciones;
                ddlUuarios.SelectedValue = (string.IsNullOrWhiteSpace(entity.IDUsuarioAdicional.ToString())) ? "" : entity.IDUsuarioAdicional.ToString();
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, int IDPersona, string Fecha, string Horas, string Tarea, string Observaciones,string estado, string idUsuarioAdicional)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {

                TrackingHoras entity;
                if (id > 0)
                    entity = dbContext.TrackingHoras.Where(x => x.IDTrackingHoras == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new TrackingHoras();
                }

                entity.IDUsuario = usu.IDUsuario;
                entity.IDPersona = IDPersona;
                entity.Fecha = Convert.ToDateTime(Fecha);
                entity.Horas = Convert.ToInt32(Horas);
                entity.Tarea = Tarea;
                entity.Observaciones = Observaciones;
                entity.Estado = estado;
                if(!string.IsNullOrWhiteSpace(idUsuarioAdicional))
                {
                    entity.IDUsuarioAdicional = Convert.ToInt32(idUsuarioAdicional);
                }

                if (id == 0)
                    dbContext.TrackingHoras.Add(entity);
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}