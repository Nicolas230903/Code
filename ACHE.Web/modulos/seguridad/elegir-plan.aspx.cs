using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class modulos_seguridad_elegir_plan : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (verificarEstadoPendiente(dbContext, CurrentUser.IDUsuario))
                {
                    liTitulo.Text = "Tu pago se encuentra pendiente de verificacion. Hasta que el mismo no se verifique, sólo tendrás acceso a las funcionalidades del plan básico.";
                    PlanBasico.Visible = false;
                    planProfesional.Visible = false;
                    planPyme.Visible = false;
                    planEmpresa.Visible = false;
                }
                else
                {
                    var NombrePlan = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault().PlanActual;
                    var tipo = Request.QueryString["tipo"];
                    var upgrade = Request.QueryString["upgrade"];
                    
                    switch (upgrade)
                    {
                        case "0":
                            liTitulo.Text = "Tu plan actual es el " + NombrePlan + ". Puede actualizarse a cualquiera de estos planes: ";
                            //PlanBasico.Visible = false;
                            break;
                        case "1":
                            //liTitulo.Text = "Tu plan actual es el " + NombrePlan + ". Puede actualizarse a cualquiera de estos planes: ";
                            //PlanBasico.Visible = false;
                            liTitulo.Text = "Tu plan actual es el " + NombrePlan + ". La funcionalidad a la que intentas acceder requiere un cambio de plan, para más información por favor envíe un correo a axan.sistemas@gmail.com.";
                            break;
                        case "2":
                            liTitulo.Text = "Alcanzo los 100 comprobantes permitidos, para continuar facturando debe actualizarse a cualquiera de los siguientes planes: ";
                            PlanBasico.Visible = false;
                            break;
                        case "3":
                            liTitulo.Text = "Por favor elige el modo con el que quieres continuar: ";
                              var planActual= PermisosModulos.ObtenerPlanActual(dbContext, CurrentUser.IDUsuario);
                              MostrarPlan(planActual.IDPlan);
                            break;
                        case "4":
                            liTitulo.Text = "Tu plan actual es el " + NombrePlan + ". La funcionalidad a la que intentas acceder requiere que tengas alguno de los siguientes planes:";
                            break;
                        default:
                            //liTitulo.Text = "Tu plan " + NombrePlan + " ha vencido. Por favor elige el plan con el que quieres continuar: ";
                            liTitulo.Text = "Tu plan actual es el " + NombrePlan + ". La funcionalidad a la que intentas acceder requiere un cambio de plan, para más información por favor envíe un correo a axan.sistemas@gmail.com.";
                            break;
                    }
                    //SetearPreciosPlanes();
                    //if (!string.IsNullOrWhiteSpace(tipo))
                    //{
                    //    var formulario = dbContext.Formularios.Where(x => x.Nombre.ToUpper() == tipo.ToUpper()).FirstOrDefault();
                    //    var plan = formulario.IDPlan;
                    //    MostrarPlan(plan);
                    //}
                }
            }
        }
    }

    private void SetearPreciosPlanes()
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];// hardcodeado por estos 6 meses para los usuarios que ya fueron creados antes de 31/07/2015
        var fecha = Convert.ToDateTime("31/07/2015");
        if (usu.FechaAlta <= fecha)
        {
            liPlanProfesionalMes.Text = "Mensual: 149  + IVA por mes";
            liPlanProfesionalAnual.Text = "Anual: Ahorrás $178,8 y pagás $1609,20 + IVA";
        }
        else // FIN hardcodeado 
        {
            liPlanProfesionalMes.Text = "Mensual: 199 + IVA por mes";
            liPlanProfesionalAnual.Text = "Anual: Ahorrás $238.8 y pagás $2388+ IVA";
        }

        liPlanPymeMes.Text = "Mensual: $299 + IVA por mes";
        liPlanPymeAnual.Text = "Anual: Ahorrás $358,8 y pagás $3229,20 + IVA";
        liPlanEmpresaMes.Text = "Mensual: 399 + IVA por mes";
        liPlanEmpresaAnual.Text = "Anual: Ahorrás $478,8 y pagás $4309,20 + IVA";
    }

    private void MostrarPlan(int plan) 
    {
        switch (plan)
        {
            case 2:
                PlanBasico.Visible = false;
                break;
            case 3:
                PlanBasico.Visible = false;
                planProfesional.Visible = false;
                break;

            case 4:
                PlanBasico.Visible = false;
                planProfesional.Visible = false;
                planPyme.Visible = false;
                break;
        }
    }


    private bool verificarEstadoPendiente(ACHEEntities dbContext, int idUsuario)
    {
        var usu = dbContext.UsuariosView.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
        //var UsuAdicional = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == idUsuario).FirstOrDefault();
        var Empresa = dbContext.Usuarios.Where(x => x.IDUsuarioPadre != null && x.IDUsuario == idUsuario).FirstOrDefault();
        PlanesPagos plan = new PlanesPagos();

        if (usu.IDUsuarioAdicional > 0)
            plan = dbContext.PlanesPagos.Where(x => x.IDUsuario == usu.IDUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();
        else if (Empresa != null)
            plan = dbContext.PlanesPagos.Where(x => x.IDUsuario == Empresa.IDUsuarioPadre && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();
        else
            plan = dbContext.PlanesPagos.Where(x => x.IDUsuario == idUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();


        if (plan != null && plan.Estado == "Pendiente")
            return true;
        else
            return false;        
    }
}