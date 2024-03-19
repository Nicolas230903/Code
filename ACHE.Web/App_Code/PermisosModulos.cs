using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Configuration;
using System.Reflection;


/// <summary>
/// Summary description for PermisosModulos
/// </summary>
public static class PermisosModulos
{
    ////////////////////////////////////////// PLANES //////////////////////////

    public static bool tienePlan(string formulario)
    {
        var listaFormularios = (List<Formularios>)HttpContext.Current.Session["ASPNETListaFormularios"];
        bool tieneAcceso = false;
        using (var dbContext = new ACHEEntities())
            tieneAcceso = listaFormularios.Any(x => x.Nombre.ToUpper() == formulario.Replace(".aspx", "").ToUpper());
        return tieneAcceso;
    }
    public static int ObtenerTodosLosFormularios(ACHEEntities dbContext, int idUsuario)
    {
        var idPlan = 0;
        var plan = ObtenerPlanActual(dbContext, idUsuario);

        if (plan != null)
            idPlan = plan.IDPlan;
        else
            idPlan = 1;

        var listaFormularios = new List<Formularios>();
        if (HttpContext.Current.Session["ASPNETListaFormularios"] == null)
        {
            listaFormularios = dbContext.Formularios.Where(x => x.Planes.IDPlan <= idPlan).ToList();
            HttpContext.Current.Session["ASPNETListaFormularios"] = listaFormularios;
        }
        else
            listaFormularios = (List<Formularios>)HttpContext.Current.Session["ASPNETListaFormularios"];

        return idPlan;
    }
    public static bool PlanVigente(ACHEEntities dbContext, int idUsuario)
    {
        var plan = ObtenerPlanActual(dbContext, idUsuario);
        if (plan.IDPlan == 1) //Plan Basico NO VENCE
        {
            return true;
        }
        else
        {
            if (plan != null && plan.FechaFinPlan >= DateTime.Now.Date)
            {
                //if (plan.IDPlan == 1)
                //{
                //    //Renuevo el plan basico
                //    //PlanesPagos p = new PlanesPagos();
                //    //p.IDUsuario = idUsuario;
                //    //p.IDPlan = 1; //Plan Basico
                //    //p.ImportePagado = 0;
                //    //p.PagoAnual = false;
                //    //p.FormaDePago = "-";
                //    //p.NroReferencia = "";
                //    //p.Estado = "Aceptado";
                //    //p.FechaDeAlta = DateTime.Now.Date;
                //    //p.FechaDePago = DateTime.Now.Date;
                //    //p.FechaInicioPlan = p.FechaDePago;
                //    //p.FechaFinPlan = Convert.ToDateTime(p.FechaInicioPlan).AddDays(30).Date;
                //    //dbContext.PlanesPagos.Add(p);
                //    return true;
                //}
                return true;
            }
            else
                return false;
        }
    }
    public static PlanesPagos ObtenerPlanActual(ACHEEntities dbContext, int idUsuario)
    {
        PlanesPagos plan;
        if (HttpContext.Current.Session["ASPNETPlanActual"] == null)
        {
            plan = PermisosModulosCommon.ObtenerPlanActual(dbContext, idUsuario);
            HttpContext.Current.Session["ASPNETPlanActual"] = plan;
        }
        else
        {
            plan = (PlanesPagos)HttpContext.Current.Session["ASPNETPlanActual"];
        }
        return plan;
    }

    //////////////// ROLES///////////////////////
    public static bool tieneAccesoAlModulo(string formulario)
    {
        bool tieneAcceso = false;
        //var listaRoles = (List<RolesUsuariosAdicionales>)HttpContext.Current.Session["ASPNETListaRoles"];

        //using (var dbContext = new ACHEEntities())
        //{
        //    foreach (var item in listaRoles)
        //    {
        //        tieneAcceso = item.Roles.Formularios.Any(x => x.Nombre.ToUpper() == formulario.Replace(".aspx", "").ToUpper());
        //        if (tieneAcceso)
        //            break;
        //    }
        //}
        return tieneAcceso;
    }
    public static bool ocultarHeader(int idRol)
    {
        //var listaRoles = (List<RolesUsuariosAdicionales>)HttpContext.Current.Session["ASPNETListaRoles"];
        bool ocultarHeader = false;
        //ocultarHeader = listaRoles.Any(x => x.IDRol == idRol);
        return ocultarHeader;
    }
    public static bool mostrarPersonaSegunPermiso(string tipo)
    {
        bool tieneAcceso = false;
        if (tipo == "c")
        {
            if (ocultarHeader(2))
                tieneAcceso = true;
        }
        else if (tipo == "p")
        {
            if (ocultarHeader(1))
                tieneAcceso = true;
        }
        return tieneAcceso;
    }
    public static void obtenerRolesUsuarioAdicional(ACHEEntities dbContext, int idUsuAdic)
    {
        //var listaRoles = new List<RolesUsuariosAdicionales>();
        //if (HttpContext.Current.Session["ASPNETListaRoles"] == null)
        //{
        //    listaRoles = dbContext.RolesUsuariosAdicionales.Include("Roles.Formularios").Where(x => x.IDUsuarioAdicional == idUsuAdic).ToList();
        //    HttpContext.Current.Session["ASPNETListaRoles"] = listaRoles;
        //}
        //else
        //   listaRoles = (List<RolesUsuariosAdicionales>)HttpContext.Current.Session["ASPNETListaRoles"];
    }

    //////////////////// ACCESO A FORMULARIO SEGUN PLAN
    public static bool tieneAccesoAlFormulario(string formulario)
    {
        bool tieneAcceso = false;
        var listaFormularios = (List<Formularios>)HttpContext.Current.Session["ASPNETListaFormularios"];

        using (var dbContext = new ACHEEntities())
        {
            foreach (var item in listaFormularios)
            {
                if (item.Nombre.ToString().ToUpper() == formulario.Replace(".aspx", "").ToUpper())
                {
                    tieneAcceso = true;
                    break;
                }
            }
        }
        return tieneAcceso;
    }



}