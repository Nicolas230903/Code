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
using System.IO;
using ACHE.Negocio.Tesoreria;

public partial class modulos_tesoreria_MovimientoDeFondose : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionMovimientos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            txtFechaMovimiento.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            litPath.Text = "Alta";
            CargarCuentas();
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

    private void CargarCuentas()
    {
        using (var dbContext = new ACHEEntities())
        {
            var bancos = dbContext.Bancos.Where(x => x.IDUsuario == CurrentUser.IDUsuario).ToList();
            ddlPlanDeCuentaOrigen.Items.Add(new ListItem("", ""));
            ddlPlanDeCuentaDestino.Items.Add(new ListItem("", ""));

            foreach (var item in bancos)
            {
                ddlPlanDeCuentaOrigen.Items.Add(new ListItem(item.BancosBase.Nombre, "BANCO_" + item.IDBanco.ToString()));
                ddlPlanDeCuentaDestino.Items.Add(new ListItem(item.BancosBase.Nombre, "BANCO_" + item.IDBanco.ToString()));
            }
            var idMov = Request.QueryString["ID"];
            if (idMov != null && idMov != "0")
            {
                var id = Convert.ToInt32(idMov);
                var cajas = dbContext.MovimientoDeFondos.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDMovimientoDeFondo == id).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(cajas.CajaOrigen))
                    ddlPlanDeCuentaOrigen.Items.Add(new ListItem("CAJA", "CAJA_" + cajas.CajaOrigen));
                else
                    ddlPlanDeCuentaOrigen.Items.Add(new ListItem("CAJA", "CAJA_0"));

                if (!string.IsNullOrWhiteSpace(cajas.CajaDestino))
                    ddlPlanDeCuentaDestino.Items.Add(new ListItem("CAJA", "CAJA_" + cajas.CajaDestino));
                else
                    ddlPlanDeCuentaDestino.Items.Add(new ListItem("CAJA", "CAJA_0"));
            }
            else
            {
                ddlPlanDeCuentaOrigen.Items.Add(new ListItem("CAJA", "CAJA_0"));
                ddlPlanDeCuentaDestino.Items.Add(new ListItem("CAJA", "CAJA_0"));
            }

        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.MovimientoDeFondos.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDMovimientoDeFondo == id).FirstOrDefault();
            if (entity != null)
            {
                ddlPlanDeCuentaOrigen.SelectedValue = (entity.Origen == "BANCO") ? "BANCO_" + entity.IDBancoOrigen.ToString() : "CAJA_" + entity.CajaOrigen;
                ddlPlanDeCuentaDestino.SelectedValue = (entity.Destino == "BANCO") ? "BANCO_" + entity.IDBancoDestino.ToString() : "CAJA_" + entity.CajaDestino;

                txtImporte.Text = entity.Importe.ToString().Replace(".", ",");
                txtFechaMovimiento.Text = entity.FechaMovimiento.ToString("dd/MM/yyyy");
                txtObservaciones.Text = entity.Observaciones;
                if (!string.IsNullOrWhiteSpace(entity.Foto))
                {
                    hdnFileName.Value = entity.Foto;
                    hdnTieneFoto.Value = (!string.IsNullOrWhiteSpace(entity.Foto)) ? "1" : "0";
                    lnkComprobante.HRef = "/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/MovimientosDeFondos/" + entity.Foto;
                }
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, string idCuentaOrigen, string idCuentaDestino, string importe, string fechaMovimiento, string observaciones)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return MovimientoDeFondosCommon.GuardarMovimientoDeFondos(id, idCuentaOrigen, idCuentaDestino, importe, fechaMovimiento, observaciones, usu);
            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
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

    [WebMethod(true)]
    public static void eliminarFoto(int idMovimiento)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.MovimientoDeFondos.Where(x => x.IDMovimientoDeFondo == idMovimiento && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/MovimientosDeFondos/" + entity.Foto);

                if (File.Exists(Serverpath))
                {
                    File.Delete(Serverpath);
                    entity.Foto = "";
                    dbContext.SaveChanges();
                }
                else
                    throw new Exception("El movimiento no tiene una imagen guardada");
            }
        }
    }
}