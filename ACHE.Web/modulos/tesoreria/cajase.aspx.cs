using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Data;
using System.IO;
using System.Web.Services;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Tesoreria;
using ACHE.Extensions;

public partial class modulos_tesoreria_cajase : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ",";//"."
    public const string SeparadorDeDecimales = ".";//","

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionCaja)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            litPath.Text = "Alta";
            txtFecha.Value = DateTime.Now.ToString("dd/MM/yyyy");
            CargarPlanDeCuentas();
            cargarConceptos();
            hdnID.Value = Request.QueryString["ID"];
            int id = 0;
            if (int.TryParse(hdnID.Value, out id))
            {
                CargarEntidad(id);
                litPath.Text = "Edición";
            }
        }
    }
    private void CargarPlanDeCuentas()
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                if (CurrentUser.UsaPlanCorporativo) //Plan Corporativo
                {
                    hdnUsaPlanCorporativo.Value = "1";

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

                        //var listaAux = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.AdminiteAsientoManual).ToList();
                        //ddlPlanDeCuentas.Items.Add(new ListItem("", ""));
                        //foreach (var item in listaAux)
                        //    ddlPlanDeCuentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void cargarConceptos() 
    {
        using(var dbContext = new ACHEEntities())
        {
            var entity = dbContext.ConceptosCaja.Where(x => x.IDUsuario == CurrentUser.IDUsuario).ToList();
            ddlConcepto.Items.Add( new ListItem("",""));
            foreach (var item in entity)
                ddlConcepto.Items.Add(new ListItem(item.Nombre, item.IDConceptoCaja.ToString()));
        }
    }
    private void CargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Caja.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDCaja == id).FirstOrDefault();

            ddlConcepto.SelectedValue = entity.IDConceptosCaja.ToString();
            txtObservaciones.Value = entity.Observaciones;
            txtImporte.Value = entity.Importe.ToString().Replace(",", ".");
            txtTipoMovimiento.Value = entity.TipoMovimiento;
            txtFecha.Value = Convert.ToDateTime(entity.Fecha).ToString("dd/MM/yyyy");
            ddlMedioPago.Value = entity.MedioDePago;
            txtTicket.Value = entity.Ticket;
            ddlPlanDeCuentas.SelectedValue = Convert.ToInt32(entity.IDPlanDeCuenta).ToString();

            if (!string.IsNullOrWhiteSpace(entity.Foto))
            {
                hdnFileName.Value = entity.Foto;
                hdnTieneFoto.Value = "1";

                lnkDescarga.NavigateUrl = "/pdfGenerator.ashx?file=" + entity.Foto + "&tipoDeArchivo=Caja";
            }
        }
    }

    #region///*** ABM CAJA  ***////


    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static CajaViewModel cargarEntidad(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                CajaViewModel cajaViewModel = new CajaViewModel();
                var entity = dbContext.Caja.Where(x => x.IDCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    cajaViewModel.tipoMovimiento = entity.TipoMovimiento;
                    cajaViewModel.Fecha = Convert.ToDateTime(entity.Fecha).ToString("dd/MM/yyyy");
                    cajaViewModel.Concepto = (entity.ConceptosCaja == null) ? "" : entity.ConceptosCaja.IDConceptoCaja.ToString();
                    cajaViewModel.Importe = Math.Abs(entity.Importe);
                    cajaViewModel.Estado = entity.Estado;
                    cajaViewModel.Observaciones = entity.Observaciones;
                    cajaViewModel.ID = entity.IDCaja;
                    cajaViewModel.MedioDePago = entity.MedioDePago;
                    cajaViewModel.Ticket = entity.Ticket;
                    cajaViewModel.FileName = (entity.Foto == "" || entity.Foto == null) ? "" : entity.Foto;
                    cajaViewModel.IDPlanDeCuenta = (entity.IDPlanDeCuenta == null) ? 0 : Convert.ToInt32(entity.IDPlanDeCuenta);
                    if (!string.IsNullOrWhiteSpace(entity.Foto))
                        cajaViewModel.TieneFoto = "1";
                    else
                        cajaViewModel.TieneFoto = "0";
                }
                return cajaViewModel;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static int guardar(CajaViewModel caja)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return CajaCommon.GuardarCaja(caja, usu);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
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
                ContabilidadCommon.AgregarAsientoDeCaja(usu, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    private static int anularMovimientos(ACHEEntities dbContext, int id, string tipoMovimiento, string Fecha, string Concepto, string Importe, string Estado, string Observaciones, string MedioDePago, string ticket)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        Caja entity;
        if (id > 0)
        {
            entity = dbContext.Caja.Where(x => x.IDCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            entity.Fecha = DateTime.Now.Date;
            entity.Estado = "Anulado";
            dbContext.SaveChanges();
            entity.TipoMovimiento = (entity.TipoMovimiento == "Ingreso") ? "Egreso" : "Ingreso";
            entity.Observaciones = "";
            entity.MedioDePago = "";
            entity.IDConceptosCaja = null;
            dbContext.Caja.Add(entity);
            dbContext.SaveChanges();
        }
        entity = new Caja();
        entity.FechaAlta = DateTime.Now.Date;
        entity.Estado = "Cargado";
        entity.IDUsuario = usu.IDUsuario;
        entity.TipoMovimiento = tipoMovimiento;
        entity.Fecha = Convert.ToDateTime(Fecha);

        if (Concepto == "")
            entity.IDConceptosCaja = null;
        else
            entity.IDConceptosCaja = Convert.ToInt32(Concepto);

        entity.Importe = (Importe != string.Empty) ? Convert.ToDecimal(Importe.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
        entity.Importe = Math.Abs(entity.Importe);
        entity.Observaciones = Observaciones;
        entity.MedioDePago = MedioDePago;
        entity.Ticket = ticket;

        dbContext.Caja.Add(entity);
        dbContext.SaveChanges();

        return entity.IDCaja;
    }

    [WebMethod(true)]
    public static void eliminarFoto(int id)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Caja.Where(x => x.IDCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/caja/" + entity.Foto);
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
    #endregion
    #region///*** ABM CONCEPTOS  ***////
    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static string ObtenerConceptosCaja()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var html = "";
            using (var dbContext = new ACHEEntities())
            {
                var list = dbContext.ConceptosCaja.Where(x => x.IDUsuario == usu.IDUsuario)
                    .OrderBy(x => x.Nombre).ToList();
                if (list.Any())
                {
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        //html += "<td>" + detalle.IDConcepto + "</td>";
                        html += "<td>" + detalle.Nombre + "</td>";
                        html += "<td><a href='#' title='Editar' style='font-size: 16px;' onclick=\"caja.editarConcepto(" + detalle.IDConceptoCaja + ",'" + detalle.Nombre.Trim() + "');\"><i class='fa fa-pencil'></i></a>&nbsp;<a href='#' title='Eliminar' style='font-size: 16px;' onclick='caja.eliminarConcepto(" + detalle.IDConceptoCaja + ");'><i class='fa fa-times'></i></a></td>";
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
    public static void guardarConcepto(int id, string nombre)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (id == 0 && dbContext.ConceptosCaja.Any(x => x.Nombre.ToLower() == nombre.ToLower() && x.IDUsuario == usu.IDUsuario))
                    throw new Exception("El nombre ingresado ya se encuentra creado");

                ConceptosCaja entity;
                if (id > 0)
                    entity = dbContext.ConceptosCaja.Where(x => x.IDConceptoCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new ConceptosCaja();
                    entity.IDUsuario = usu.IDUsuario;
                }
                entity.Nombre = nombre;

                if (id > 0)
                {
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.ConceptosCaja.Add(entity);
                    dbContext.SaveChanges();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarConcepto(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (!dbContext.Caja.Any(x => x.IDConceptosCaja == id && x.IDUsuario == usu.IDUsuario))
                {
                    ConceptosCaja entity = dbContext.ConceptosCaja.Where(x => x.IDConceptoCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.ConceptosCaja.Remove(entity);
                        dbContext.SaveChanges();
                    }
                }
                else
                    throw new Exception("El concepto se encuentra asociado a 1 o más ingresos/egresos registrados.");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static List<Combo2ViewModel> ObtenerSelectConceptos()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                return dbContext.ConceptosCaja.Where(x => x.IDUsuario == usu.IDUsuario).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDConceptoCaja,
                        Nombre = x.Nombre
                    }).OrderBy(x => x.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion
}