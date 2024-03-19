using ACHE.Model;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using ACHE.Negocio.Productos;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Common;
using System.Configuration;

public partial class conceptose : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ".";
    public const string SeparadorDeDecimales = ",";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.ComercialProductosYServicios)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            litPath.Text = "Alta";
            hdnIdUsuario.Value = CurrentUser.IDUsuario.ToString();
            hdnUsaPrecioFinalConIVA.Value = (CurrentUser.UsaPrecioFinalConIVA) ? "1" : "0";

            if (CurrentUser.CondicionIVA == "MO")
            {
                ddlIva.Enabled = false;
                ddlIva.SelectedValue = "3";
            }
            else if (CurrentUser.CondicionIVA == "RI")
                ddlIva.SelectedValue = "5";

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
                    {
                        litPath.Text = "Edición";
                        //txtStock.Enabled = false;
                    }
                }
            }

            if (CurrentUser.UsaPrecioFinalConIVA)
            {
                liPrecioUnitario.Text = " Precio unit. con IVA";
                liPrecioTotal.Text = " PRECIO SIN IVA";
            }
            else
            {
                liPrecioUnitario.Text = " Precio unit. sin IVA";
                liPrecioTotal.Text = " PRECIO CON IVA";
            }
        }
    }

    private void cargarEntidad(int id, string duplicado)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Conceptos.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDConcepto == id).FirstOrDefault();
            if (entity != null)
            {
                ddlTipo.SelectedValue = entity.Tipo;
                txtNombre.Text = entity.Nombre.ToUpper();
                txtCodigo.Text = (duplicado == "0") ? entity.Codigo.ToUpper() : "";
                ddlEstado.SelectedValue = entity.Estado;
                txtDescripcion.Text = entity.Descripcion;
                txtStock.Text = entity.Stock.ToString();
                txtPrecio.Text = entity.PrecioUnitario.ToString("").Replace(",", ".");
                ddlIva.SelectedValue = entity.IdTipoIVA.ToString();
                txtObservaciones.Text = entity.Observaciones;
                txtCostoInterno.Text = entity.CostoInterno.ToString();
                hdnIDPersona.Value = Convert.ToInt32(entity.IDPersona).ToString();
                if (CurrentUser.UsaPrecioFinalConIVA)
                    litTotal.Text = Math.Round(ConceptosCommon.ObtenerPrecioFinal(entity.PrecioUnitario, entity.Iva.ToString()), 2).ToString();
                else
                    litTotal.Text = (entity.PrecioUnitario + ((entity.PrecioUnitario * entity.Iva) / 100)).ToString("N2");

                txtStockMinimo.Text = entity.StockMinimo.ToString();
                txtStockFisico.Text = entity.StockFisico.ToString();
                if (!string.IsNullOrWhiteSpace(entity.Foto))
                {
                    imgFoto.Src = "/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/Productos-Servicios/" + entity.Foto;
                    hdnTieneFoto.Value = "1";
                }
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, string nombre, string codigo, string tipo, string descripcion, string estado, string precio, string iva, string stock, string obs, string constoInterno, string stockMinimo, int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            int IdConcepto = ConceptosCommon.GuardarConcepto(id, nombre, codigo, tipo, descripcion, estado, precio, iva, stock, obs, constoInterno, stockMinimo, idPersona, usu.IDUsuario);                            

            return IdConcepto;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



    [WebMethod(true)]
    public static ConceptosViewModel obtenerDatos(int id, int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Conceptos entity = dbContext.Conceptos.Where(x => x.IDConcepto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    ConceptosViewModel result = new ConceptosViewModel();
                    result.ID = id;
                    //result.Nombre = (entity.Descripcion != "") ? entity.Descripcion : entity.Nombre;
                    result.Nombre = entity.Nombre;
                    result.Iva = entity.Iva.ToString("");
                    result.TipoIva = entity.IdTipoIVA.ToString();
                    result.Codigo = entity.Codigo;
                    if (idPersona > 0)
                    {
                        var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                        if (persona.ListaPrecios != null && persona.ListaPrecios.PreciosConceptos.Where(x => x.IDConceptos == id && x.Precio > 0).Any())
                            result.Precio = persona.ListaPrecios.PreciosConceptos.Where(x => x.IDConceptos == id).FirstOrDefault().Precio.ToString().Replace(",", ".");
                        else
                            result.Precio = entity.PrecioUnitario.ToString("").Replace(",", ".");
                    }
                    else
                        result.Precio = entity.PrecioUnitario.ToString("").Replace(",", ".");

                    result.Precio = ObtenerPrecioFinal(Convert.ToDecimal(result.Precio.Replace(SeparadorDeMiles, SeparadorDeDecimales)), result.Iva, idPersona).ToString("").Replace(",", ".");
                    return result;
                }
                else
                    throw new Exception("Error al obtener los datos");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static decimal ObtenerPrecioFinal(decimal PrecioUnitario, string iva, int idPersona)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        decimal resultado = Math.Round(PrecioUnitario,2);

        /*using (var dbContext = new ACHEEntities())
        {
            //var UsaPrecioConIva = dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && (x.CondicionIva == "MO" || x.CondicionIva == "CF"));
            //if (!usu.UsaPrecioFinalConIVA && UsaPrecioConIva)
            if (usu.UsaPrecioFinalConIVA)
                resultado = PrecioUnitario - ((PrecioUnitario * Convert.ToDecimal(iva)) / 100);

            return Math.Round(resultado, 2);
        }*/
        return resultado;
    }

    [WebMethod(true)]
    public static void eliminarFoto(int idConcepto)
    {

        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Conceptos.Where(x => x.IDConcepto == idConcepto && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Productos-Servicios/" + entity.Foto);

                if (File.Exists(Serverpath))
                {
                    File.Delete(Serverpath);

                    entity.Foto = "";
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new Exception("El producto no tiene una imagen guardada");
                }
            }
        }
    }
}