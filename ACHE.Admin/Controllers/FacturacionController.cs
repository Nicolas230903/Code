using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Admin.Models;
using System.Data;
using System.IO;

namespace ACHE.Admin.Controllers
{
    public class FacturacionController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerUsuarios(string condicion, string periodo, int page, int pageSize)
        {
            ResultadosUsuarioViewModel resultado = new ResultadosUsuarioViewModel();
            try
            {
                using (var dbContext = new ACHEEntities())
                {

                    var results = new List<UsuariosPlanesView>();
                    var fecha = DateTime.Now.Date;

                    switch (periodo)
                    {
                        case "1": //Planes vencidos
                            fecha = DateTime.Now.Date;
                            results = dbContext.UsuariosPlanesView.Where(x => x.FechaFinPlan <= fecha).OrderByDescending(x => x.FechaFinPlan).ToList();
                            break;
                        case "2": // Planes a punto de vencer (5 dias)
                            var fdesde = DateTime.Now.Date;
                            fecha = DateTime.Now.AddDays(+5);
                            results = dbContext.UsuariosPlanesView.Where(x => x.FechaFinPlan > fdesde && x.FechaFinPlan <= fecha).OrderByDescending(x => x.FechaFinPlan).ToList();
                            break;
                    }

                    page--;

                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.GroupBy(x => x.IDUsuario).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new UsuarioViewModel()
                        {
                            ID = x.FirstOrDefault().IDUsuario,
                            RazonSocial = x.FirstOrDefault().RazonSocial,
                            CUIT = x.FirstOrDefault().CUIT,
                            Telefono = x.FirstOrDefault().Telefono,
                            Email = x.FirstOrDefault().Email,
                            SetupRealizado = (x.FirstOrDefault().SetupRealizado) ? "SI" : "NO",
                            PlanActual = x.FirstOrDefault().PlanActual,
                            AntiguedadMeses = Convert.ToInt32(x.FirstOrDefault().AntiguedadMeses),
                            CondicionIva = x.FirstOrDefault().CondicionIva,
                            FechaUltLogin = x.FirstOrDefault().FechaUltLogin.ToString("dd/MM/yyyy"),
                            Baja = x.FirstOrDefault().Activo ? "NO" : "SI",
                            FechaAltaDesc = x.FirstOrDefault().FechaAlta.ToString("dd/MM/yyyy")
                        });
                    resultado.Items = list.ToList();
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult export(string condicion, string periodo)
        {
            string fileName = "facturacion";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {

                    var results = new List<UsuariosPlanesView>();
                    var fecha = DateTime.Now.Date;

                    switch (periodo)
                    {
                        case "1": //Planes vencidos
                            fecha = DateTime.Now.Date;
                            results = dbContext.UsuariosPlanesView.Where(x => x.FechaFinPlan <= fecha).OrderByDescending(x => x.FechaFinPlan).ToList();
                            break;
                        case "2": // Planes a punto de vencer (5 dias)
                            var fdesde = DateTime.Now.Date;
                            fecha = DateTime.Now.AddDays(+5);
                            results = dbContext.UsuariosPlanesView.Where(x => x.FechaFinPlan >= fdesde && x.FechaFinPlan <= fecha).OrderByDescending(x => x.FechaFinPlan).ToList();
                            break;
                    }


                    dt = results.GroupBy(x => x.IDUsuario)
                        .Select(x => new
                        {
                            ID = x.FirstOrDefault().IDUsuario,
                            RazonSocial = x.FirstOrDefault().RazonSocial,
                            CUIT = x.FirstOrDefault().CUIT,
                            Telefono = x.FirstOrDefault().Telefono,
                            Email = x.FirstOrDefault().Email,
                            SetupRealizado = (x.FirstOrDefault().SetupRealizado) ? "SI" : "NO",
                            PlanActual = x.FirstOrDefault().PlanActual,
                            AntiguedadMeses = Convert.ToInt32(x.FirstOrDefault().AntiguedadMeses),
                            CondicionIva = x.FirstOrDefault().CondicionIva,
                            FechaUltLogin = x.FirstOrDefault().FechaUltLogin.ToString("dd/MM/yyyy"),
                            Baja = x.FirstOrDefault().Activo ? "NO" : "SI",
                            FechaAltaDesc = x.FirstOrDefault().FechaAlta.ToString("dd/MM/yyyy")
                        }).ToList().ToDataTable();

                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                var archivo = (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");
                return Json(archivo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}