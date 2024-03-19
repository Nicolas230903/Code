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
public static class PermisosModulosCommon
{
    ////////////////////////////////////////// PLANES //////////////////////////

    public static PlanesPagos ObtenerPlanActual(ACHEEntities dbContext, int idUsuario)
    {
        PlanesPagos plan = new PlanesPagos();
        var usu = dbContext.UsuariosView.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
        var Empresa = dbContext.Usuarios.Where(x => x.IDUsuarioPadre != null && x.IDUsuario == idUsuario).FirstOrDefault();
               
        if (usu != null && usu.IDUsuarioAdicional > 0 && (usu.IDUsuarioPadre == null || usu.IDUsuarioPadre == 0))
        {
            plan = dbContext.PlanesPagos.Include("Planes").Where(x => x.IDUsuario == usu.IDUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault(); //&& x.Estado == "Aceptado"
        }
        else
        {
            if (Empresa != null)
            {
                plan = dbContext.PlanesPagos.Include("Planes").Where(x => x.IDUsuario == Empresa.IDUsuarioPadre && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault(); //&& x.Estado == "Aceptado"
            }
            else
            {                
                plan = dbContext.PlanesPagos.Include("Planes").Where(x => x.IDUsuario == idUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault(); //&& x.Estado == "Aceptado"
            }
        }

        if (plan == null)
            plan = dbContext.PlanesPagos.Include("Planes").Where(x => x.IDUsuario == idUsuario && x.FechaInicioPlan <= DateTime.Now && x.IDPlan == 1).FirstOrDefault(); //Plan Basico NO VENCE

        if (plan != null && plan.Estado == "Pendiente")
        {
            var aux = plan;
            plan = new PlanesPagos();
            plan.Estado = aux.Estado;
            plan.FechaDeAlta = aux.FechaDeAlta;
            plan.FechaDePago = aux.FechaDePago;
            plan.FechaInicioPlan = Convert.ToDateTime(aux.FechaInicioPlan);
            plan.FechaFinPlan = Convert.ToDateTime(aux.FechaFinPlan);
            plan.IDPlan = 1;
            plan.Planes = dbContext.Planes.Where(x => x.IDPlan == plan.IDPlan).FirstOrDefault();
        }
        return plan;
    }

    public static bool AlertaPlanPendiente(int idUsuario, ref string nombre)
    {
        using (var dbContext = new ACHEEntities())
        {
            var fecha = DateTime.Now.Date;
            var planes = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario == idUsuario && x.Activo == true && x.Estado == "Pendiente" && x.FechaFinPlan > fecha).FirstOrDefault();

            if (planes == null)
                return false;

            nombre = planes.PlanActual;
            return true;
        }
    }

    public static bool VerificarCantComprobantes(PlanesPagos planActual, WebUser usu)
    {
        using (var dbContext = new ACHEEntities())
        {
            var plan = planActual;
            var CantComprobantes = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.FechaComprobante >= plan.FechaInicioPlan && x.FechaComprobante <= plan.FechaFinPlan).Count();
            var CantCompras = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.Fecha >= plan.FechaInicioPlan && x.Fecha <= plan.FechaFinPlan).Count();


            if (plan.IDPlan == 1 && (CantCompras + CantComprobantes) >= 100)
                return true;
            else
                return false;
        }
    }
    public static void verificarPlanPendiente() 
    {

    }
}