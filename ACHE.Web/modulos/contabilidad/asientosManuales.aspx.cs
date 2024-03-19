using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Model;
using ACHE.Extensions;
using ACHE.Negocio.Contabilidad;
using System.Web.Services;
using ACHE.Model.ViewModels;

public partial class modulos_contabilidad_asientosManuales : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                var listaPlanDeCuenta = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.AdminiteAsientoManual);
                ddlPlanDeCuentas.Items.Add(new ListItem("", ""));

                foreach (var item in listaPlanDeCuenta)
                    ddlPlanDeCuentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));

                var idAsiento = 0;
                if (int.TryParse(Request.QueryString["id"], out idAsiento))
                {
                    var listaAsiento = dbContext.Asientos.Where(x => x.IDAsiento == idAsiento).FirstOrDefault();
                    if (listaAsiento.IDCobranza > 0)
                        Response.Redirect("/cobranzase.aspx?ID=" + listaAsiento.IDCobranza);
                    else if (listaAsiento.IDCompra > 0)
                        Response.Redirect("/comprase.aspx?ID=" + listaAsiento.IDCompra);
                    else if (listaAsiento.IDComprobante > 0)
                        Response.Redirect("/comprobantese.aspx?ID=" + listaAsiento.IDComprobante);
                    else if (listaAsiento.IDPago > 0)
                        Response.Redirect("/pagose.aspx?ID=" + listaAsiento.IDPago);
                    else if (listaAsiento.IDMovimientoDeFondo> 0)
                        Response.Redirect("/modulos/tesoreria/MovimientoDeFondose.aspx?ID=" + listaAsiento.IDMovimientoDeFondo);
                    else
                        hdnID.Value = idAsiento.ToString();
                }
                else
                    txtFecha.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
            }
        }
    }

    [WebMethod(true)]
    public static void guardar(List<AsientosManualesViewModel> listaAsientos, string leyenda, string fecha, int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            if (ContabilidadCommon.ValidarCierreContable(usu, Convert.ToDateTime(fecha)))
                throw new Exception("No puede agregar ni modificar un asiento que se encuentre en un periodo cerrado.");

            ContabilidadCommon.AgregarAsientoManual(listaAsientos, leyenda, fecha, id, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static ResultadoAsientosManualesViewModel ObtenerAsientosManuales(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                ResultadoAsientosManualesViewModel resultado = new ResultadoAsientosManualesViewModel();
                resultado.items = dbContext.rptImpositivoLibroDiario.Where(x => x.IDAsiento == id && x.IDUsuario == usu.IDUsuario).ToList()
                                                         .Select(x => new AsientosManualesViewModel()
                {
                    IDPlanDeCuenta = x.IDPlanDeCuenta,
                    NombreCuenta = x.Nombre,
                    Debe = x.Debe,
                    Haber = x.Haber
                }).ToList();

                var asiento = dbContext.rptImpositivoLibroDiario.Where(x => x.IDAsiento == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                resultado.Leyenda = asiento.Leyenda;
                resultado.Fecha = asiento.Fecha.ToString("dd/MM/yyyy");

                return resultado;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}