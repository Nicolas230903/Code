using ACHE.Admin.Models;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACHE.Admin.Controllers
{
    public class SistemasController : Controller
    {
        public ActionResult Index()
        {
            var model = new SistemasViewModel();
            using (var dbContext = new ACHEEntities())
            {
                model = dbContext.LicenciaTemp.Select(x => new SistemasViewModel()
                {
                    Vigencia = x.Vigencia,
                    Clave = x.Clave,
                    Modulo1_Nombre = x.Modulo1_Nombre,
                    Modulo1_Version = x.Modulo1_Version,
                    Modulo1_UrlInstalador32 = x.Modulo1_UrlInstalador32,
                    Modulo1_UrlInstalador64 = x.Modulo1_UrlInstalador64,
                    Modulo2_Nombre = x.Modulo2_Nombre,
                    Modulo2_Version = x.Modulo2_Version,
                    Modulo2_UrlInstalador32 = x.Modulo2_UrlInstalador32,
                    Modulo2_UrlInstalador64 = x.Modulo2_UrlInstalador64,
                    Modulo3_Nombre = x.Modulo3_Nombre,
                    Modulo3_Version = x.Modulo3_Version,
                    Modulo3_UrlInstalador32 = x.Modulo3_UrlInstalador32,
                    Modulo3_UrlInstalador64 = x.Modulo3_UrlInstalador64,
                    Modulo4_Nombre = x.Modulo4_Nombre,
                    Modulo4_Version = x.Modulo4_Version,
                    Modulo4_UrlInstalador32 = x.Modulo4_UrlInstalador32,
                    Modulo4_UrlInstalador64 = x.Modulo4_UrlInstalador64,
                    Modulo5_Nombre = x.Modulo5_Nombre,
                    Modulo5_Version = x.Modulo5_Version,
                    Modulo5_UrlInstalador32 = x.Modulo5_UrlInstalador32,
                    Modulo5_UrlInstalador64 = x.Modulo5_UrlInstalador64
                }).FirstOrDefault();               

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Guardar(string vigencia, string clave, 
            string modulo1_Nombre, string modulo1_Version, string modulo2_Nombre, string modulo2_Version, 
            string modulo3_Nombre, string modulo3_Version, string modulo4_Nombre, string modulo4_Version, 
            string modulo5_Nombre, string modulo5_Version)
        {
            bool resultado = true;
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var licencias = dbContext.LicenciaTemp.ToList();

                    foreach(LicenciaTemp l in licencias)
                    {
                        l.Vigencia = vigencia;
                        l.Clave = clave;
                        l.Modulo1_Nombre = modulo1_Nombre;
                        l.Modulo1_Version = modulo1_Version;
                        l.Modulo2_Nombre = modulo2_Nombre;
                        l.Modulo2_Version = modulo2_Version;
                        l.Modulo3_Nombre = modulo3_Nombre;
                        l.Modulo3_Version = modulo3_Version;
                        l.Modulo4_Nombre = modulo4_Nombre;
                        l.Modulo4_Version = modulo4_Version;
                        l.Modulo5_Nombre = modulo5_Nombre;
                        l.Modulo5_Version = modulo5_Version;

                        dbContext.SaveChanges();
                    }
                }
            }
            catch 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


    }
}