using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ACHE.Model;

namespace ACHE.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var model = new DashboardAdminViewModel();

            using (var dbContext = new ACHEEntities())
            {
                var fecha = DateTime.Now.Date;
                var Primero = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario != 2 && x.IDPlan == 6 && x.IDPlan != null).GroupBy(x => x.IDPlan).ToList().Select(x => new PlanViewModel()
                {
                    IDPlan = x.FirstOrDefault().IDPlan,
                    Nombre = x.FirstOrDefault().PlanActual,
                    TotalUsuarios = x.Count(y => y.Activo == true),
                    TotalActivos = x.Count(y => y.Activo == true && y.SetupRealizado == true && y.Estado == "Aceptado" && y.FechaFinPlan >= fecha),
                    TotalInactivos = x.Count(y => y.Activo == true && (y.SetupRealizado == false || y.FechaFinPlan < fecha || (y.Estado == "Aceptado" && y.FechaFinPlan < fecha))),
                    TotalPendienteDePago = x.Count(y => y.Activo == true && y.Estado == "Pendiente" && y.FechaFinPlan > fecha),
                    ClassMaxCantUsuarios = "danger",

                    ToolTipsTotalUsuarios = "Usuarios que no han sido dados de baja",
                    ToolTipsActivos = "Usuarios activos con setup finalizado ,  estado del plan aceptado y funcionando",
                    ToolTipsInactivos = " Usuarios activos y setup NO finalizado o con el plan vencido",
                    ToolTipsPendienteDePago = "Usuarios activos con el estado del plan pendiente de pago",
                }).ToList();

                var resto = dbContext.UsuariosPlanesView.Where(x => x.IDUsuario != 2 && x.IDPlan != 6 && x.IDPlan != null).GroupBy(x => x.IDPlan).ToList().Select(x => new PlanViewModel()
                {
                    IDPlan = x.FirstOrDefault().IDPlan,
                    Nombre = x.FirstOrDefault().PlanActual,
                    TotalUsuarios = x.Count(y => y.Activo == true),
                    TotalActivos = x.Count(y => y.Activo == true && y.SetupRealizado == true && y.Estado == "Aceptado" && y.FechaFinPlan >= fecha),
                    TotalInactivos = x.Count(y => y.Activo == true && (y.SetupRealizado == false || y.FechaFinPlan < fecha || (y.Estado == "Aceptado" && y.FechaFinPlan < fecha))),
                    TotalPendienteDePago = x.Count(y => y.Activo == true && y.Estado == "Pendiente" && y.FechaFinPlan > fecha),
                    ClassMaxCantUsuarios = "success",

                    ToolTipsTotalUsuarios = "Usuarios que no han sido dados de baja.",
                    ToolTipsActivos = "Usuarios activos con setup finalizado ,  estado del plan aceptado y funcionando.",
                    ToolTipsInactivos = " Usuarios activos y setup NO finalizado o con el plan vencido.",
                    ToolTipsPendienteDePago = "Usuarios activos con el estado del plan pendiente de pago.",
                }).ToList();

                model.Listaplanes = Primero.Union(resto).ToList();



            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ObtenerPlanes(string tiempo)
        {
            List<ChartXYZ> listaChart = new List<ChartXYZ>();
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var fecha = DateTime.Now.AddMonths(-6);
                    var listaPlanes = dbContext.PlanesPagos.Where(x => x.Estado == "Aceptado" && x.FechaDeAlta >= fecha).ToList();

                    var listaBasico = listaPlanes.Where(x => x.IDPlan == 1).GroupBy(x => new { x.FechaDeAlta.Month, x.IDPlan }).Select(x => new ChartXYZ()
                    {
                        Basico = x.Count(),
                        Fecha = Convert.ToInt32(x.Select(y => y.FechaDeAlta.ToString("MM")).FirstOrDefault())
                    }).ToList();

                    var listaProfesional = listaPlanes.Where(x => x.IDPlan == 2).GroupBy(x => new { x.FechaDeAlta.Month, x.IDPlan }).Select(x => new ChartXYZ()
                    {
                        Profesional = x.Count(),
                        Fecha = Convert.ToInt32(x.Select(y => y.FechaDeAlta.ToString("MM")).FirstOrDefault())
                    }).ToList();

                    var listaPyme = listaPlanes.Where(x => x.IDPlan == 3).GroupBy(x => new { x.FechaDeAlta.Month, x.IDPlan }).Select(x => new ChartXYZ()
                    {
                        Pyme = x.Count(),
                        Fecha = Convert.ToInt32(x.Select(y => y.FechaDeAlta.ToString("MM")).FirstOrDefault())
                    }).ToList();

                    var listaEmpresa = listaPlanes.Where(x => x.IDPlan == 4).GroupBy(x => new { x.FechaDeAlta.Month, x.IDPlan }).Select(x => new ChartXYZ()
                    {
                        Empresa = x.Count(),
                        Fecha = Convert.ToInt32(x.Select(y => y.FechaDeAlta.ToString("MM")).FirstOrDefault())
                    }).ToList();

                    var listaPrueba = listaPlanes.Where(x => x.IDPlan == 6).GroupBy(x => new { x.FechaDeAlta.Month, x.IDPlan }).Select(x => new ChartXYZ()
                    {
                        Prueba = x.Count(),
                        Fecha = Convert.ToInt32(x.Select(y => y.FechaDeAlta.ToString("MM")).FirstOrDefault())
                    }).ToList();

                    listaChart = listaChart.Union(listaBasico).Union(listaProfesional).Union(listaPyme).Union(listaEmpresa).Union(listaPrueba).ToList();

                    var aux = new List<ChartXYZ>();
                    aux.Add(new ChartXYZ() { Fecha = 1 });
                    aux.Add(new ChartXYZ() { Fecha = 2 });
                    aux.Add(new ChartXYZ() { Fecha = 3 });
                    aux.Add(new ChartXYZ() { Fecha = 4 });
                    aux.Add(new ChartXYZ() { Fecha = 5 });
                    aux.Add(new ChartXYZ() { Fecha = 6 });
                    aux.Add(new ChartXYZ() { Fecha = 7 });
                    aux.Add(new ChartXYZ() { Fecha = 8 });
                    aux.Add(new ChartXYZ() { Fecha = 9 });
                    aux.Add(new ChartXYZ() { Fecha = 10 });
                    aux.Add(new ChartXYZ() { Fecha = 11 });
                    aux.Add(new ChartXYZ() { Fecha = 12 });

                    aux = aux.Union(listaChart).ToList();
                    aux = aux.GroupBy(x => x.Fecha).Select(x => new ChartXYZ()
                    {
                        Fecha = Convert.ToInt32(x.FirstOrDefault().Fecha),
                        Basico = x.Sum(y => y.Basico),
                        Profesional = x.Sum(y => y.Profesional),
                        Pyme = x.Sum(y => y.Pyme),
                        Empresa = x.Sum(y => y.Empresa),
                        Prueba = x.Sum(y => y.Prueba)
                    }).OrderByDescending(x => x.Fecha).ToList();

                    int cont = 11;
                    foreach (var item in aux)
                    {
                        item.Fecha = cont;
                        cont--;
                    }

                    listaChart = aux;
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(listaChart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ObtenerPlanesDias() 
        {
            List<CharPlanes> listaChart = new List<CharPlanes>();
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    
                    var fecha = DateTime.Now.AddDays(-30);
                    var listaPlanes = dbContext.PlanesPagos.Where(x => x.Estado == "Aceptado" && x.FechaDeAlta >= fecha).ToList();

                    var listaBasico = listaPlanes.Where(x => x.IDPlan == 1).GroupBy(x => new { x.FechaDeAlta.Day, x.IDPlan }).Select(x => new CharPlanes()
                    {
                        Basico = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM-dd")).FirstOrDefault()
                    }).ToList();

                    var listaProfesional = listaPlanes.Where(x => x.IDPlan == 2).GroupBy(x => new { x.FechaDeAlta.Day, x.IDPlan }).Select(x => new CharPlanes()
                    {
                        Profesional = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM-dd")).FirstOrDefault()
                    }).ToList();

                    var listaPyme = listaPlanes.Where(x => x.IDPlan == 3).GroupBy(x => new { x.FechaDeAlta.Day, x.IDPlan }).Select(x => new CharPlanes()
                    {
                        Pyme = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM-dd")).FirstOrDefault()
                    }).ToList();

                    var listaEmpresa = listaPlanes.Where(x => x.IDPlan == 4).GroupBy(x => new { x.FechaDeAlta.Day, x.IDPlan }).Select(x => new CharPlanes()
                    {
                        Empresa = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM-dd")).FirstOrDefault()
                    }).ToList();

                    var listaPrueba = listaPlanes.Where(x => x.IDPlan == 6).GroupBy(x => new { x.FechaDeAlta.Day, x.IDPlan }).Select(x => new CharPlanes()
                    {
                        Prueba = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM-dd")).FirstOrDefault()
                    }).ToList();

                    listaChart = listaChart.Union(listaBasico).Union(listaProfesional).Union(listaPyme).Union(listaEmpresa).Union(listaPrueba).ToList();

                    listaChart = listaChart.GroupBy(x => x.Fecha).Select(x => new CharPlanes()
                    {
                        Fecha = x.FirstOrDefault().Fecha,
                        Basico = x.Sum(y => y.Basico),
                        Profesional = x.Sum(y => y.Profesional),
                        Pyme = x.Sum(y => y.Pyme),
                        Empresa = x.Sum(y => y.Empresa),
                        Prueba = x.Sum(y => y.Prueba)
                    }).ToList();

                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(listaChart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ObtenerFormasDePago() 
        {
            List<CharFormas> listaChart = new List<CharFormas>();
            try
            {
                using (var dbContext = new ACHEEntities())
                {

                    var fecha = DateTime.Now.AddMonths(-6);
                    var listaPlanes = dbContext.PlanesPagos.Where(x => x.Estado == "Aceptado" && x.FechaDeAlta >= fecha).ToList();

                    var listaMP = listaPlanes.Where(x => x.FormaDePago == "Mercado Pago").GroupBy(x => new { x.FechaDeAlta.Month}).Select(x => new CharFormas()
                    {
                        MercadoPago = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM")).FirstOrDefault()
                    }).ToList();

                    var listaTrans = listaPlanes.Where(x => x.FormaDePago == "Transferencia").GroupBy(x => new { x.FechaDeAlta.Month}).Select(x => new CharFormas()
                    {
                        Transferencia = x.Count(),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM")).FirstOrDefault()
                    }).ToList();

                    listaChart = listaChart.Union(listaMP).Union(listaTrans).ToList();
                    listaChart = listaChart.GroupBy(x => x.Fecha).Select(x => new CharFormas()
                    {
                        Fecha = x.FirstOrDefault().Fecha,
                        MercadoPago = x.Sum(y => y.MercadoPago),
                        Transferencia = x.Sum(y => y.Transferencia),
                    }).ToList();

                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(listaChart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ObtenerFacturacion()
        {
            List<CharFacturacion> listaChart = new List<CharFacturacion>();
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var fecha = DateTime.Now.AddMonths(-6);
                    var listaPlanes = dbContext.PlanesPagos.Where(x => x.Estado == "Aceptado" && x.FechaDeAlta >= fecha).ToList();

                    listaChart = listaPlanes.GroupBy(x => new { x.FechaDeAlta.Month }).Select(x => new CharFacturacion()
                    {
                        ImporteTotal = x.Sum(y => y.ImportePagado),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM")).FirstOrDefault()
                    }).ToList();
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(listaChart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ObtenerFacturacionDias()
        {
            List<CharFacturacion> listaChart = new List<CharFacturacion>();
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var fecha = DateTime.Now.AddDays(-31);
                    var listaPlanes = dbContext.PlanesPagos.Where(x => x.Estado == "Aceptado" && x.FechaDeAlta >= fecha).ToList();

                    listaChart = listaPlanes.GroupBy(x => new { x.FechaDeAlta.Day }).Select(x => new CharFacturacion()
                    {
                        ImporteTotal = x.Sum(y => y.ImportePagado),
                        Fecha = x.Select(y => y.FechaDeAlta.ToString("yyyy-MM-dd")).FirstOrDefault()
                    }).ToList();
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(listaChart, JsonRequestBehavior.AllowGet);
        }
    }
}