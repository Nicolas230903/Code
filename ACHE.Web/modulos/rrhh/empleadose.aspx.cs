using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;


public partial class modulos_rrhh_empleadose : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            litPath.Text = "Alta";
            Idusuario.Value = CurrentUser.IDUsuario.ToString();
            txtFechaAlta.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
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
            var entity = dbContext.Empleados.Where(x => x.IDEmpleados== id && x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                txtNombre.Text = entity.Nombre;
                txtApellido.Text = entity.Apellido;
                txtCUIT.Text = entity.CUIT;
                txtTelefono.Text = entity.Telefono;
                txtCelular.Text = entity.Celular;
                txtNroLegajo.Text = entity.NroLegajo.ToString();
                txtContactoEmergencia.Text = entity.ContactoEmergencia;
                hdnProvincia.Value = entity.IDProvincia.ToString();
                hdnCiudad.Value= entity.IDCiudad.ToString();
                txtDomicilio.Text = entity.Domicilio;
                txtPisoDepto.Text = entity.PisoDepto;
                txtObraSocial.Text = entity.ObraSocial;
                txtSueldo.Text = entity.Sueldo;
                txtEmail.Text = entity.Email;
                txtFechaAlta.Text = entity.FechaAlta.ToString("dd/MM/yyyy");
                txtFechaBaja.Text = (entity.FechaBaja == null) ? "" : Convert.ToDateTime(entity.FechaBaja).ToString("dd/MM/yyyy");

                if (!string.IsNullOrWhiteSpace(entity.Foto))
                {
                    imgFoto.Src = "/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/Empleados/" + entity.Foto;
                    hdnTieneFoto.Value = "1";
                }
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, string Nombre, string Apellido, string CUIT, string Telefono, string Celular, string NroLegajo, string ContactoEmergencia,
        int idProvincia, int idCiudad, string Domicilio, string PisoDepto, string ObraSocial, string Sueldo, string Email, string fechaAlta, string fechaBaja)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {

                if (dbContext.Empleados.Any(x => x.IDUsuario == usu.IDUsuario && x.IDEmpleados != id && x.CUIT == CUIT))
                    throw new Exception("El nro de CUIT ingresado ya se encuentra registrado.");

                Empleados entity;
                if (id > 0)
                    entity = dbContext.Empleados.Where(x => x.IDEmpleados == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new Empleados();
                    entity.IDUsuario = usu.IDUsuario;
                }

                entity.FechaAlta = Convert.ToDateTime(fechaAlta);

                if (!string.IsNullOrWhiteSpace(fechaBaja))
                    entity.FechaBaja = Convert.ToDateTime(fechaBaja);
                else
                    entity.FechaBaja = null;

                entity.Nombre = Nombre;
                entity.Apellido = Apellido;
                entity.CUIT = CUIT;
                entity.Telefono = Telefono;
                entity.Celular = Celular;

                entity.ContactoEmergencia = ContactoEmergencia;
                entity.IDProvincia= idProvincia;
                entity.IDCiudad= idCiudad;
                entity.Domicilio = Domicilio;
                entity.PisoDepto = PisoDepto;
                entity.ObraSocial = ObraSocial;
                entity.Sueldo = Sueldo;
                entity.Email = Email;

                if (NroLegajo != "")
                    entity.NroLegajo = Convert.ToInt32(NroLegajo);

                if (id == 0)
                    dbContext.Empleados.Add(entity);

                dbContext.SaveChanges();

                return entity.IDEmpleados;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarFoto(int idEmpleado)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Empleados.Where(x => x.IDEmpleados == idEmpleado && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Empleados/" + entity.Foto);

                if (File.Exists(Serverpath))
                {
                    File.Delete(Serverpath);
                    entity.Foto = "";
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new Exception("El cheque no tiene una imagen guardada");
                }
            }
        }
    }
}