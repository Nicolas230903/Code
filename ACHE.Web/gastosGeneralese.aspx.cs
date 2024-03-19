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

public partial class gastosGeneralese : BasePage
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
                    if (!afu.AdministracionGastosGenerales)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            litPath.Text = "Alta";
            hdnIdusuario.Value = CurrentUser.IDUsuario.ToString();


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
            var entity = dbContext.GastosGenerales.Where(x => x.IdUsuario == CurrentUser.IDUsuario && x.IdGastosGenerales == id).FirstOrDefault();
            if (entity != null)
            {
                txtFechaPeriodo.Text = entity.Periodo.ToString("yyyyMM");

                txtSueldos.Text = entity.Sueldos.ToString("N2");
                txtSeguridadEHigiene.Text = entity.SeguridadEHigiene.ToString("N2");
                txtMunicipales.Text = entity.Municipales.ToString("N2");
                txtMonotributos.Text = entity.Monotributos.ToString("N2");
                txtAportesYContribuciones.Text = entity.AportesYContribuciones.ToString("N2");
                txtGanancias12.Text = entity.Ganancias12.ToString("N2");
                txtCreditoBancario.Text = entity.CreditoBancario.ToString("N2");
                txtRetencionesDeIIBB.Text = entity.RetencionesDeIIBB.ToString("N2");
                txtPlanesAFIP.Text = entity.PlanesAFIP.ToString("N2");
                txtGastos1.Text = entity.Gastos1.ToString("N2");
                txtGastos2.Text = entity.Gastos2.ToString("N2");
                txtGastos3.Text = entity.Gastos3.ToString("N2");
                txtF1.Text = entity.F1.ToString("N2");
                txtF2.Text = entity.F2.ToString("N2");
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, string fechaPeriodo, string Sueldos,
        string SeguridadEHigiene, string Municipales, string Monotributos, string AportesYContribuciones,
        string Ganancias12, string CreditoBancario, string RetencionesDeIIBB, string PlanesAFIP,
        string Gastos1, string Gastos2, string Gastos3, string F1, string F2)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            int idGastoGeneral = 0;
            using (var dbContext = new ACHEEntities())
            {
                var gastosGenerales = GastosGeneralesCommon.Guardar(id, fechaPeriodo,  Sueldos,
                                         SeguridadEHigiene,  Municipales,  Monotributos,  AportesYContribuciones,
                                         Ganancias12,  CreditoBancario,  RetencionesDeIIBB,  PlanesAFIP,
                                         Gastos1,  Gastos2,  Gastos3, F1, F2, usu.IDUsuario);

                idGastoGeneral = gastosGenerales.IdGastosGenerales;

            }
            return idGastoGeneral;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

}