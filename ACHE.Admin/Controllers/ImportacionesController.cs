using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Extensions;
using ACHE.Admin.Models;
using System.Configuration;
using System.Collections.Specialized;
using FileHelpers;
using ACHE.Model.ViewModels;
using System.Data;
using System.Data.SqlClient;
using ACHE.Admin;

namespace ACHE.Admin.Controllers
{
    public class ImportacionesController : BaseController
    {
        public ActionResult Index(int id)
        {
            Session["DataImport"] = null;
            var model = new ImportacionesViewModel();
            if (id == 0)
                model.IDUsuario = 0;
            else
                model.IDUsuario = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult leerArchivoCSVProductos(string nombre, string tipo, int idUsuario)
        {
            Session["DataImport"] = null;
            List<ProductosCSVTmp> listaproductosCSV = new List<ProductosCSVTmp>();
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaproductosCSV = ImportacionMasiva.LeerArchivoCSVProductos(tipo, idUsuario, path);

                if (!listaproductosCSV.Any())
                    throw new Exception("No se encontraron datos en el archivo.");

                Session["DataImport"] = listaproductosCSV;
                return Json(listaproductosCSV, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }  
        }

        [HttpPost]
        public ActionResult RealizarImportacionProductos()
        {
            try
            {
                var lista = (List<ProductosCSVTmp>)Session["DataImport"];
                ImportacionMasiva.RealizarImportacionProductos(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult leerArchivoCSVPersonas(string nombre, string tipo,int idUsuario)
        {

            Session["DataImport"] = null;
           
            List<PersonasCSVTmp> listaPersonasCSV = new List<PersonasCSVTmp>();
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaPersonasCSV = ImportacionMasiva.LeerArchivoCSVPersonas(tipo, idUsuario, path);

                if (!listaPersonasCSV.Any())
                    throw new Exception("No se encontraron datos en el archivo.");

                Session["DataImport"] = listaPersonasCSV;
                return Json(listaPersonasCSV, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public ActionResult RealizarImportacionPersonas()
        {
            try
            {
                var lista = (List<PersonasCSVTmp>)Session["DataImport"];
                ImportacionMasiva.RealizarImportacionPersonas(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
 
    }
}