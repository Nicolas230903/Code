using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Model;
using System.IO;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using ACHE.Negocio.Contabilidad;
using System.Web.Script.Services;
using ACHE.Model.ViewModels;
using System.Configuration;
using ACHE.Extensions;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using System.Data.Entity;
using System.IdentityModel.Metadata;

public partial class comprase : BasePage
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
                //var plan = PermisosModulos.ObtenerPlanActual(dbContext, CurrentUser.IDUsuario);
                //if (PermisosModulosCommon.VerificarCantComprobantes(plan, CurrentUser))
                //    Response.Redirect("~/modulos/seguridad/elegir-plan.aspx?upgrade=2");
                
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.SuministroComprobanteDeCompra)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                if (CurrentUser.UsaPlanCorporativo) //Plan Corporativo
                CargarPlanDeCuentas();

                Usuarios usu = dbContext.Usuarios.Where(w => w.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
                if (usu != null)
                    hdnEsAgenteRetencionGanancia.Value = (bool)usu.EsAgenteRetencionGanancia ? "1" : "0";

            }

            txtFecha.Text = txtFechaEmision.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtFechaPrimerVencimiento.Text = DateTime.Now.ToString("dd/MM/yyyy");
            cargarCategorias();

            litTotal.Text = "0";
            liTotalImpuestos.Text = "0";
            liTotalImportes.Text = "0";
            litPath.Text = "Alta";
            hdnIdusuario.Value = CurrentUser.IDUsuario.ToString();
            hdnTieneSaldoAPagar.Value = "1";
            hdnIDPersona.Value = Request.QueryString["IDPersona"];


            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";

                    int idCompra = Convert.ToInt32(hdnID.Value);
                    var dbContext = new ACHEEntities();
                    Compras c = dbContext.Compras.Where(x => x.IDCompra == idCompra && x.Adjunto != null).FirstOrDefault();
                    if (c == null)
                        this.lnkDescargarAdjunto.Style.Add("Display", "none");

                }
            }
        }
    }
    private void CargarPlanDeCuentas()
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.ConfiguracionPlanDeCuenta.Any(x => x.IDUsuario == CurrentUser.IDUsuario))
                {
                    var idctas = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault().CtasFiltroCompras.Split(',');
                    List<Int32> cuentas = new List<Int32>();
                    for (int i = 0; i < idctas.Length; i++)
                        cuentas.Add(Convert.ToInt32(idctas[i]));

                    var listaAux = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && cuentas.Contains(x.IDPlanDeCuenta)).ToList();
                    ddlPlanDeCuentas.Items.Add(new ListItem("", ""));
                    foreach (var item in listaAux)
                        ddlPlanDeCuentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                }
            }
            hdnUsaPlanCorporativo.Value = "1";
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void cargarCategorias()
    {
        using (var dbContext = new ACHEEntities())
        {
            var categorias = dbContext.Categorias.Where(x => x.IDUsuario == CurrentUser.IDUsuario).ToList();
            ddlCategoria.DataTextField = "Nombre";
            ddlCategoria.DataValueField = "IDCategoria";
            ddlCategoria.DataSource = categorias;
            ddlCategoria.DataBind();

            ddlCategoria.Items.Insert(0, new ListItem("", ""));
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Compras.Include("Personas").Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDCompra == id).FirstOrDefault();
            if (entity != null)
            {
                hdnIDPersona.Value = entity.IDPersona.ToString();
                hdnTipoComprobante.Value = entity.Tipo;

                txtFecha.Text = entity.Fecha.ToString("dd/MM/yyyy");
                txtFechaEmision.Text = entity.FechaEmision.ToString("dd/MM/yyyy");

                txtFechaPrimerVencimiento.Text = entity.FechaPrimerVencimiento.ToString("dd/MM/yyyy");
                if (entity.FechaSegundoVencimiento != null)
                    txtFechaSegundoVencimiento.Text = Convert.ToDateTime(entity.FechaSegundoVencimiento).ToString("dd/MM/yyyy");

                txtNroFactura.Text = entity.NroFactura;
                txtImporte2.Text = entity.Importe2.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImporte5.Text = entity.Importe5.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImporte10.Text = entity.Importe10.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImporte21.Text = entity.Importe21.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImporte27.Text = entity.Importe27.ToString("N2").Replace(".", "").Replace(",", ".");
                txtIva.Text = entity.Iva.ToString("N2").Replace(".", "").Replace(",", ".");
                txtNoGravado.Text = entity.NoGravado.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImporteMon.Text = entity.ImporteMon.ToString("N2").Replace(".", "").Replace(",", ".");
                txtExento.Text = entity.Exento.ToString("N2").Replace(".", "").Replace(",", ".");
                // IMPUESTOS
                txtImpNacionales.Text = entity.ImpNacional.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImpMunicipales.Text = entity.ImpMunicipal.ToString("N2").Replace(".", "").Replace(",", ".");
                txtImpInternos.Text = entity.ImpInterno.ToString("N2").Replace(".", "").Replace(",", ".");
                //txtIIBB.Text = entity.IIBB.ToString("").Replace(",", "");
                var jurisdicciones = dbContext.Jurisdicciones.Where(x => x.IDCompra == entity.IDCompra).ToList();
                if (jurisdicciones != null)
                    txtIIBB.Text = jurisdicciones.Sum(x => x.Importe).ToString("N2").Replace(".", "").Replace(",", ".");

                txtPercepcionIVA.Text = entity.PercepcionIVA.ToString("N2").Replace(".", "").Replace(",", ".");
                txtOtros.Text = entity.Otros.ToString("N2").Replace(".", "").Replace(",", ".");

                //txtRedondeo.Text = entity.Redondeo.ToString("").Replace(",", ".");
                txtObservaciones.Text = entity.Observaciones;

                litTotal.Text = Convert.ToDecimal(entity.Iva + entity.Total.Value + entity.TotalImpuestos + entity.Redondeo).ToString("N2");
                liTotalImpuestos.Text = Convert.ToDecimal(entity.TotalImpuestos).ToString("N2");
                liTotalImportes.Text = Convert.ToDecimal(entity.Total).ToString("N2");

                //litTotal.Text = Convert.ToDecimal(entity.Iva + entity.Total.Value + entity.TotalImpuestos + entity.Redondeo).ToString("N2").Replace(".", "").Replace(",", ".");
                //liTotalImpuestos.Text = Convert.ToDecimal(entity.TotalImpuestos).ToString("N2").Replace(".", "").Replace(",", ".");
                //liTotalImportes.Text = Convert.ToDecimal(entity.Total).ToString("N2").Replace(".", "").Replace(",", ".");

                if (entity.IDCategoria.HasValue)
                    ddlCategoria.SelectedValue = entity.IDCategoria.Value.ToString();
                if (entity.Rubro != null)
                    ddlRubro.SelectedValue = entity.Rubro;

                hdnTieneSaldoAPagar.Value = entity.Saldo > 0 ? "1" : "0";

                if (!string.IsNullOrWhiteSpace(entity.Foto))
                {
                    hdnFileName.Value = entity.Foto;
                    hdnTieneFoto.Value = "1";

                    lnkComprobante.NavigateUrl = "/pdfGenerator.ashx?file=" + entity.Foto + "&tipoDeArchivo=compras";
                }

                ddlPlanDeCuentas.SelectedValue = entity.IDPlanDeCuenta.ToString();
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, int idPersona, string fecha, string nroFactura,
        string iva, string importe2, string importe5, string importe10, string importe21, string importe27, string noGrav, string importeMon,
        string impNacional, string impMunicipal, string impInterno, string percepcionIva, string otros,
        string obs, string tipo, string idCategoria, string rubro, string exento, string FechaEmision, int idPlanDeCuenta, List<JurisdiccionesViewModel> Jurisdicciones, string fechaPrimerVencimiento, string fechaSegundoVencimiento)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            int idCompra = 0;
            using (var dbContext = new ACHEEntities())
            {
                var compras = ComprasCommon.Guardar(id, idPersona, fecha, nroFactura, iva, importe2, importe5, importe10, importe21, importe27, noGrav, importeMon,
                    impNacional, impMunicipal, impInterno, percepcionIva, otros, obs, tipo, idCategoria, rubro, exento, FechaEmision, idPlanDeCuenta, usu.IDUsuario, Jurisdicciones, fechaPrimerVencimiento, fechaSegundoVencimiento, null);

                idCompra = compras.IDCompra;

            }
            GenerarAsientosContables(idCompra);
            return idCompra;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    #region CATEGORIAS
    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string getCategories()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var html = "";
            using (var dbContext = new ACHEEntities())
            {
                var list = dbContext.Categorias.Where(x => x.IDUsuario == usu.IDUsuario)
                    .OrderBy(x => x.Nombre).ToList();
                if (list.Any())
                {
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        //html += "<td>" + detalle.IDCategoria + "</td>";
                        html += "<td>" + detalle.Nombre + "</td>";
                        html += "<td><a href='#' title='Editar' style='font-size: 16px;' onclick=\"Compras.editarCategoria(" + detalle.IDCategoria + ",'" + detalle.Nombre.Trim() + "');\"><i class='fa fa-pencil'></i></a>&nbsp;<a href='#' title='Eliminar' style='font-size: 16px;' onclick='Compras.eliminarCategoria(" + detalle.IDCategoria + ");'><i class='fa fa-times'></i></a></td>";
                        html += "</tr>";
                    }
                }
                else
                    html += "<tr><td colspan='2'>No hay un detalle disponible</td></tr>";
            }

            return html;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void guardarCategoria(int id, string nombre)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (id == 0 && dbContext.Categorias.Any(x => x.Nombre.ToLower() == nombre.ToLower() && x.IDUsuario == usu.IDUsuario))
                    throw new Exception("El nombre ingresado ya se encuentra creado");

                Categorias entity;
                if (id > 0)
                    entity = dbContext.Categorias.Where(x => x.IDCategoria == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new Categorias();
                    entity.IDUsuario = usu.IDUsuario;
                }
                entity.Nombre = nombre;

                if (id > 0)
                {
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.Categorias.Add(entity);
                    dbContext.SaveChanges();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarCategoria(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (!dbContext.Compras.Any(x => x.IDCategoria == id && x.IDUsuario == usu.IDUsuario))
                {
                    Categorias entity = dbContext.Categorias.Where(x => x.IDCategoria == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.Categorias.Remove(entity);
                        dbContext.SaveChanges();
                    }
                }
                else
                    throw new Exception("La categoría se encuentra asociada a 1 o más pagos registrados.");

            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion
    #region JURISDICCIONES
    [WebMethod(true)]
    public static List<JurisdiccionesViewModel> getJurisdicciones(int idCompra)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Jurisdicciones.Where(x => x.IDCompra == idCompra).ToList().Select(x => new JurisdiccionesViewModel()
                {
                    IDJurisdicion = x.IDProvincia,
                    IDCompra = x.IDCompra,
                    Importe = x.Importe,
                    NombreJurisdiccion = x.Provincias.Nombre
                }).ToList();

                return lista;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> ObtenerJurisdiccionUsuario()
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        List<Combo2ViewModel> select = new List<Combo2ViewModel>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (usu.IDJurisdiccion != null && usu.IDJurisdiccion != "")
                {
                    var provincias = usu.IDJurisdiccion.Split(',');
                    List<Int32> idjurisdicciones = new List<Int32>();

                    foreach (var item in provincias)
                        idjurisdicciones.Add(Convert.ToInt32(item));

                    select = dbContext.Provincias.Where(x => idjurisdicciones.Contains(x.IDProvincia)).Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDProvincia,
                        Nombre = x.Nombre
                    }).ToList();
                }
            }
            return select;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion

    [WebMethod(true)]
    public static string getTotal(int idCompra)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];           

            using (var dbContext = new ACHEEntities())
            {

                var entity = dbContext.Compras.Where(x => x.IDCompra == idCompra).FirstOrDefault();
                if (entity != null)
                {
                    return (entity.Tipo == "NCA" || entity.Tipo == "NCB" || entity.Tipo == "NCC") ? ("-" + Convert.ToDecimal(entity.TotalImpuestos + entity.Total + entity.Iva).ToString("N2")) : Convert.ToDecimal(entity.TotalImpuestos + entity.Total + entity.Iva).ToString("N2");
                }
                else
                    throw new Exception("El comprobante de compra no existe");
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerDetalleDelComprobante(int idCompra)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        List<Combo2ViewModel> select = new List<Combo2ViewModel>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {

                var tipoDetalleDelComprobante = new[] { "DDC" };

                var listaDdc = dbContext.Comprobantes.Where(w => w.IdCompraVinculada == idCompra && tipoDetalleDelComprobante.Contains(w.Tipo)).ToList();

                select = listaDdc
                   .OrderByDescending(x => x.IDComprobante)
                   .ToList()
                   .Select(x => new Combo2ViewModel()
                   {
                       ID = x.IDComprobante,
                       Nombre = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000") + " ($ " + x.ImporteTotalNeto.ToString("N2") + ")"
                   }).OrderBy(x => x.Nombre).ToList();

            }

            return select;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static void eliminarFoto(int idCheque)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Compras.Where(x => x.IDCompra == idCheque && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Compras/" + DateTime.Now.Year.ToString() + "/" + entity.Foto);

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

    [WebMethod(true)]
    public static void GenerarAsientosContables(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                ContabilidadCommon.AgregarAsientoDeCompra(id, usu);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    [WebMethod(true)]
    public static ArchivoAdjuntoComp descargarAdjunto(string id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (id != null)
                {
                    var dbContext = new ACHEEntities();
                    int idCompra = Convert.ToInt32(id);
                    Compras c = dbContext.Compras.Where(x => x.IDCompra == idCompra).FirstOrDefault();
                    if (c != null)
                    {
                        ArchivoAdjuntoComp d = new ArchivoAdjuntoComp();
                        d.NombreArchivo = "ADJUNTO" + c.IDCompra.ToString().PadLeft(8, '0') + ".PDF";
                        d.Contenido = c.Adjunto;
                        return d;
                    }
                    else
                        throw new Exception("El comprobante no posee archivo adjunto");
                }
                else
                    throw new Exception("Error al descargar el archivo");
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # comprase.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception("Error al descargar el archivo");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    public class ArchivoAdjuntoComp
    {
        public string NombreArchivo { get; set; }
        public byte[] Contenido { get; set; }
    }


    [WebMethod(true)]
    public static int vincularCompra(int id)
    {
        int numeroCompra = 0;
        decimal sumaComVinc = 0;
        string numeroFactura = "0000-00000001";

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (Comp != null)
            {
                sumaComVinc = Comp.ImporteTotalNeto - ((Comp.ImporteTotalNeto * usu.PorcentajeCompra) / 100);

                Compras ultFacCom = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.NroFactura).FirstOrDefault();

                if (ultFacCom != null)
                {
                    int numero = Convert.ToInt32(ultFacCom.NroFactura.Substring(5,8)) + 1;
                    numeroFactura = ultFacCom.NroFactura.Substring(0,4) + "-" + numero.ToString("D8");
                }

                numeroCompra = guardar(0, Comp.IDPersona, DateTime.Now.Date.ToShortDateString(), numeroFactura, "", "", "", "", "", "", "", sumaComVinc.ToString(),
                                "", "", "", "", "", "", "CDC", "", "", "", DateTime.Now.Date.ToShortDateString(), 0, null, DateTime.Now.Date.ToShortDateString(), "");

                Compras CompVinculado = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.IDCompra == numeroCompra).FirstOrDefault();
                if (CompVinculado != null)
                {
                    CompVinculado.IdComprobante = id;
                    dbContext.SaveChanges();
                }

            }

            return numeroCompra;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

}