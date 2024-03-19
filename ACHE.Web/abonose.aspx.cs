using ACHE.Model;
using ACHE.Negocio.Abono;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ACHE.Negocio.Contabilidad;

public partial class abonose : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.ComercialAbonos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            litPath.Text = "Alta";

            if (CurrentUser.CondicionIVA == "MO")
            {
                ddlIva.Enabled = false;
                ddlIva.SelectedValue = "0,00";
            }
            else if (CurrentUser.CondicionIVA == "RI")
                ddlIva.SelectedValue = "21,00";

            CargarCuentasActivo();

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    var duplicado = (!String.IsNullOrEmpty(Request.QueryString["Duplicar"])) ? Request.QueryString["Duplicar"] : "0";
                    cargarEntidad(int.Parse(hdnID.Value), duplicado);
                    if (duplicado == "1")
                    {
                        litPath.Text = "Alta";
                        hdnID.Value = "0";
                    }
                    else
                        litPath.Text = "Edición";
                }
            }
        }
    }

    private void CargarCuentasActivo()
    {
        using (var dbContext = new ACHEEntities())
        {
            if (CurrentUser.UsaPlanCorporativo) //Plan Corporativo
            {
                if (dbContext.ConfiguracionPlanDeCuenta.Any(x => x.IDUsuario == CurrentUser.IDUsuario))
                {
                    var idctas = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault().CtasFiltroVentas.Split(',');
                    List<Int32> cuentas = new List<Int32>();
                    for (int i = 0; i < idctas.Length; i++)
                    {
                        if (idctas[i] != string.Empty)
                            cuentas.Add(Convert.ToInt32(idctas[i]));
                    }

                    var listaAux = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && cuentas.Contains(x.IDPlanDeCuenta)).ToList();
                    ddlPlanDeCuentas.Items.Add(new ListItem("", ""));

                    foreach (var item in listaAux)
                        ddlPlanDeCuentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                }
                hdnUsaPlanCorporativo.Value = "1";
            }
        }
    }

    private void cargarEntidad(int id,string duplicado)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Abonos.Include("AbonosPersona").Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDAbono == id).FirstOrDefault();
            if (entity != null)
            {
                ddlFrecuencia.SelectedValue = entity.Frecuencia;
                txtNombre.Text = entity.Nombre.ToUpper();
                txtFechaInicio.Text = entity.FechaInicio.ToString("dd/MM/yyyy");
                if (entity.FechaFin.HasValue)
                    txtFechaFin.Text = entity.FechaFin.Value.ToString("dd/MM/yyyy");
                ddlEstado.SelectedValue = entity.Estado;
                txtPrecio.Text = entity.PrecioUnitario.ToString("").Replace(",", ".");
                ddlIva.SelectedValue = entity.Iva.ToString();
                txtObservaciones.Text = entity.Observaciones;
                

                ddlProducto.SelectedValue = entity.Tipo.ToString();
                if (entity.IDPlanDeCuenta != null)
                    ddlPlanDeCuentas.SelectedValue = entity.IDPlanDeCuenta.ToString();

                foreach (var per in entity.AbonosPersona)
                    hdnPersonasID.Value += per.IDPersona + ",";

                if (hdnPersonasID.Value.Length > 0)
                    hdnPersonasID.Value = hdnPersonasID.Value.Substring(0, hdnPersonasID.Value.Length - 1);

                litTotal.Text = (entity.PrecioUnitario + ((entity.PrecioUnitario * entity.Iva) / 100)).ToString("N2");
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, string nombre, string frecuencia, string fechaInicio, string fechaFin, string estado, string precio, string iva, string obs, List<AbonosPersonasViewModel> personas, int tipo, int idPlanDeCuenta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            AbonosCommon.GuardarAbono(id, nombre, frecuencia, fechaInicio, fechaFin, estado, precio, iva, obs, personas, tipo, usu, idPlanDeCuenta);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    [WebMethod(true)]
    public static List<AbonosPersonasViewModel> ObtenerClientes(int id)
    {
        List<AbonosPersonasViewModel> lista = new List<AbonosPersonasViewModel>();

        using (var dbContext = new ACHEEntities())
        {
            lista = dbContext.AbonosPersona.Where(x => x.IDAbono == id).ToList()
                                           .Select(x => new AbonosPersonasViewModel()
                                           {
                                               IDAbono = x.IDAbono,
                                               Cantidad = x.Cantidad.ToString(),
                                               IDPersona = x.IDPersona,
                                               RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                                               Total = (x.Abonos.Iva == 0) ? x.Abonos.PrecioUnitario.ToString("N2") : (x.Cantidad * (x.Abonos.PrecioUnitario + ((x.Abonos.Iva * x.Abonos.PrecioUnitario) / 100))).ToString("N2")
                                           }).ToList();
        }
        return lista;
    }


}