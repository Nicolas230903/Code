using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Banco;
using ACHE.Negocio.Contabilidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class modulos_Tesoreria_bancose : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionBancos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            litPath.Text = "Alta";
            cargarBancos();
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

    private void cargarBancos()
    {
        using (var dbContext = new ACHEEntities())
        {
            var bancosBase = dbContext.BancosBase.ToList();
            ddlBanco.Items.Add(new ListItem("", ""));
            foreach (var item in bancosBase)
                ddlBanco.Items.Add(new ListItem(item.Nombre, item.IDBancoBase.ToString()));
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Bancos.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDBanco == id).FirstOrDefault();
            if (entity != null)
            {
                ddlBanco.SelectedValue = entity.IDBancoBase.ToString();
                txtNroCuenta.Text = entity.NroCuenta.ToString();
                ddlMoneda.SelectedValue = entity.Moneda;
                ddlActivo.SelectedValue = (entity.Activo) ? "1" : "0";
                txtsaldoInicial.Text = entity.SaldoInicial.ToString();
                txtEjecutivo.Text = entity.Ejecutivo;
                txtDireccion.Text = entity.Direccion;
                txtTelefono.Text = entity.Telefono;
                txtEmail.Text = entity.Email;
                txtObservacion.Text = entity.Observaciones;
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, int idBancoBase, string nroCuenta, string moneda, int activo, string saldoInicial, string ejecutivo, string direccion, string telefono, string email, string observacion)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var IDBanco = 0;
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                IDBanco = BancosCommon.GuardarBanco(id, idBancoBase, nroCuenta, moneda, activo, saldoInicial, ejecutivo, direccion, telefono, email, observacion, usu);
                ContabilidadCommon.CrearCuentaBancos(IDBanco, usu);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}