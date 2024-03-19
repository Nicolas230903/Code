using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Model;
using ACHE.Extensions;
using System.Web.Services;
using System.Configuration;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Contabilidad;

public partial class modulos_contabilidad_planDeCuentas : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(true)]
    public static List<PlanDeCuentasViewModel> ObtenerPlanDeCuentas()
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            List<PlanDeCuentasViewModel> listaPlanDeCuenta = new List<PlanDeCuentasViewModel>();
            using (var dbContext = new ACHEEntities())
            {
                var planDeCuenta = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario).OrderBy(x => x.Codigo).ToList();
                listaPlanDeCuenta = planDeCuenta.Where(x => x.IDPadre == null).Select(x => new PlanDeCuentasViewModel()
                {
                    id = x.IDPlanDeCuenta,
                    text = x.Codigo + " - " + x.Nombre + ((x.Codigo == "1") ? "<span class='spanDetalle' style='float: right; margin-right: 20px;'>Débitos / Créditos</span>" : ""),
                    Codigo = x.Codigo,
                    Nombre = x.Nombre,
                    icon = "",
                    AdminiteAsientoManual = (x.AdminiteAsientoManual) ? "SI" : "NO",
                    children = ObtenerHijo(dbContext, x.IDPlanDeCuenta, planDeCuenta)
                }).ToList();
            }

            return listaPlanDeCuenta;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static List<PlanDeCuentasViewModel> ObtenerHijo(ACHEEntities dbContext, int id, List<PlanDeCuentas> planDeCuenta)
    {
        List<PlanDeCuentasViewModel> listaPlanAux = new List<PlanDeCuentasViewModel>();

        var list = planDeCuenta.Where(x => x.IDPadre == id).Select(x => new PlanDeCuentasViewModel()
        {
            id = x.IDPlanDeCuenta,
            text = EsCuentaPadre(dbContext, x.IDPlanDeCuenta) ? x.Codigo + " - " + x.Nombre : x.Codigo + " - " + x.Nombre + ObtenerTextoSaldo(x.IDPlanDeCuenta),
            Codigo = x.Codigo,
            Nombre = x.Nombre,
            children = (planDeCuenta.Any(y => y.IDPadre == x.IDPlanDeCuenta)) ? ObtenerHijo(dbContext, x.IDPlanDeCuenta, planDeCuenta) : null,
            icon = (planDeCuenta.Any(y => y.IDPadre == x.IDPlanDeCuenta)) ? "" : "fa fa-file-text",
            AdminiteAsientoManual = (x.AdminiteAsientoManual) ? "SI" : "NO",
        }).ToList();

        return list;
    }

    private static string ObtenerTextoSaldo(int idPlanDeCuenta)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        string saldos = string.Empty;
        var debito = string.Empty;
        var credito = string.Empty;

        var lista = ContabilidadCommon.ObtenerBalanceDeResultados(usu, "", "", 1, 10000);

        debito = (lista.Asientos.Any(x => x.IDPlanDeCuenta == idPlanDeCuenta)) ? lista.Asientos.Where(x => x.IDPlanDeCuenta == idPlanDeCuenta).FirstOrDefault().TotalDebe : "0";
        credito = (lista.Asientos.Any(x => x.IDPlanDeCuenta == idPlanDeCuenta)) ? lista.Asientos.Where(x => x.IDPlanDeCuenta == idPlanDeCuenta).FirstOrDefault().TotalHaber : "0";

        var aux = (debito != "" || credito != "") ? " / " : "";

        return "<span class='spanDetalle' style='float: right; margin-right: 20px;'> " + debito + aux + credito + "</span>";
    }

    [WebMethod(true)]
    public static PlanDeCuentasViewModel ObtenerCuenta(int idPlanDeCuenta)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            PlanDeCuentasViewModel Cuenta = new PlanDeCuentasViewModel();
            using (var dbContext = new ACHEEntities())
            {
                return dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPlanDeCuenta == idPlanDeCuenta).Select(x => new PlanDeCuentasViewModel()
                {
                    id = x.IDPlanDeCuenta,
                    Codigo = x.Codigo,
                    Nombre = x.Nombre,
                    text = x.Codigo + " - " + x.Nombre,
                    AdminiteAsientoManual = (x.AdminiteAsientoManual) ? "SI" : "NO",
                    IDPadre = (x.IDPadre == null) ? 0 : (int)x.IDPadre,
                    TipoDeCuenta = x.TipoDeCuenta.ToUpper()
                }).FirstOrDefault();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void Guardar(int id, string codigo, string nombre, int idPadre, string adminiteAsientoManual,string tipoDeCuenta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.PlanDeCuentas.Any(x => x.Codigo == codigo && x.IDUsuario == usu.IDUsuario && x.IDPlanDeCuenta != id))
                    throw new Exception("Ya existe una cuenta con el codigo ingresado");
                if (dbContext.PlanDeCuentas.Any(x => x.Nombre == nombre && x.IDUsuario == usu.IDUsuario && x.IDPlanDeCuenta != id))
                    throw new Exception("Ya existe una cuenta con el nombre ingresado");
                //else if (id > 0 && EsCuentaPadre(dbContext, id))
                //    throw new Exception("No se puede modificar las las cuentas superiores");
                else if (ContabilidadCommon.VerificarCodigo(codigo))
                    throw new Exception("El código contiene caracteres invalidos");

                PlanDeCuentas entity;
                if (id > 0)
                    entity = dbContext.PlanDeCuentas.Where(x => x.IDPlanDeCuenta == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new PlanDeCuentas();
                    entity.IDUsuario = usu.IDUsuario;
                }
                entity.Codigo = codigo;
                entity.Nombre = nombre;

                if (idPadre != 0)
                {
                    entity.IDPadre = idPadre;
                    entity.TipoDeCuenta = dbContext.PlanDeCuentas.Where(x => x.IDPlanDeCuenta == idPadre && x.IDUsuario == usu.IDUsuario).FirstOrDefault().TipoDeCuenta;
                }
                else
                    entity.TipoDeCuenta = tipoDeCuenta;

                entity.AdminiteAsientoManual = (adminiteAsientoManual == "SI") ? true : false;

                if (id > 0)
                    dbContext.SaveChanges();
                else
                {
                    dbContext.PlanDeCuentas.Add(entity);
                    dbContext.SaveChanges();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static bool EsCuentaPadre(ACHEEntities dbContext, int id)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        var ctaPadres = dbContext.PlanDeCuentas.Where(x => x.IDPadre == null && x.IDUsuario == usu.IDUsuario).ToList();
        var cta = dbContext.PlanDeCuentas.Where(x => x.IDPlanDeCuenta == id).FirstOrDefault();
        var listaCtas = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario).ToList();

        if (cta.IDPadre == null)
            return true;
        else
            return listaCtas.Any(x => x.IDPadre == cta.IDPlanDeCuenta);
    }
    
    [WebMethod(true)]
    public static void delete(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.PlanDeCuentas.Any(x => x.IDPadre == id))
                        throw new CustomException("No se puede eliminar por tener asientos cuentas hijas asociados");
                    else if (dbContext.AsientoDetalle.Any(x => x.IDPlanDeCuenta == id))
                        throw new CustomException("No se puede eliminar por tener asientos contables asociados");
                    //else if (id > 0 && EsCuentaPadre(dbContext, id))
                    //    throw new CustomException("No se puede eliminar las las cuentas superiores");

                    var entity = dbContext.PlanDeCuentas.Where(x => x.IDPlanDeCuenta == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.PlanDeCuentas.Remove(entity);
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw new Exception(e.Message);
        }
    }

    [WebMethod(true)]
    public static void CierreContable()
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                var fechaDesde = DateTime.Now.Date.ToString("dd/MM/yyyy");
                var fechaHasta = DateTime.Now.Date.ToString("dd/MM/yyyy");

                ContabilidadCommon.AgregarAsientoCierreDelEjercicio(ContabilidadCommon.ObtenerBalanceDeResultados(usu, "01/01/2015", fechaHasta, 1, 1000000), usu);
                ContabilidadCommon.AgregarAsientoInicioDelEjercicio(usu);
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }
}