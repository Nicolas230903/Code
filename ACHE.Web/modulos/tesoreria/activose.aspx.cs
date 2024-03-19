using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class modulos_tesoreria_activose : BasePage
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
        }
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> cargarCompras(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                return dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDCompra,
                        Nombre = x.Tipo + " - " + x.NroFactura
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {

            var entity = dbContext.Activos.Where(x => x.IDActivos == id).FirstOrDefault();
            if (entity != null)
            {
                hdnIDPersona.Value = entity.IDPersona.ToString();
                ddlCompra.SelectedValue = entity.IDPersona.ToString();
                if (entity.FechaInicioDeUso != null)
                    txtFechaInicio.Text = Convert.ToDateTime(entity.FechaInicioDeUso).ToString("dd/MM/yyyy");
                txtFechaCompra.Text = Convert.ToDateTime(entity.FechaCompra).ToString("dd/MM/yyyy");

                txtGarantia.Text = entity.Garantia;
                txtVidaUtil.Text = entity.VidaUtil;
                txtMarca.Text = entity.Marca;
                txtNroDeSerie.Text = entity.NumeroDeSerie;

                txtDescripcion.Text = entity.Descripcion;
                txtResponsable.Text = entity.Responsable;
                txtUbicacion.Text = entity.Ubicacion;
                txtObservaciones.Text = entity.Observaciones;

                hdnCompra.Value = entity.IDCompra.ToString();
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, int idPersona, int idCompras, string fechaInicio, string fechaCompra,
        string garantia, string vidaUtil, string marca, string nroDeSerie,
        string descripcion, string responsable, string ubicacion, string observaciones)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {

                Activos entity;
                if (id > 0)
                    entity = dbContext.Activos.Where(x => x.IDActivos == id).FirstOrDefault();
                else
                {
                    entity = new Activos();
                }

                entity.IDPersona = idPersona;
                entity.IDCompra = idCompras;
                if (fechaInicio != "")
                    entity.FechaInicioDeUso = Convert.ToDateTime(fechaInicio);

                entity.FechaCompra = Convert.ToDateTime(fechaCompra);

                entity.Garantia = garantia;
                entity.VidaUtil = vidaUtil;
                entity.Marca = marca;
                entity.NumeroDeSerie = nroDeSerie;

                entity.Descripcion = descripcion;
                entity.Responsable = responsable;
                entity.Ubicacion = ubicacion;
                entity.Observaciones = observaciones;

                if (id > 0)
                {
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.Activos.Add(entity);
                    dbContext.SaveChanges();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string cargarFechaCompra(int idCompra)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var fecha = "";
                if (idCompra == 0)
                    fecha = "";
                else
                    fecha =dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.IDCompra == idCompra).FirstOrDefault().Fecha.ToString("dd/MM/yyyy");

                return fecha;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}