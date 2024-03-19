using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Extensions;
using ACHE.Model;
using System.Web.Services;
using ACHE.Negocio.Productos;

public partial class conceptos : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {

                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.ComercialProductosYServicios)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                var TieneDatos = dbContext.Conceptos.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
                if (TieneDatos)
                {
                    divConDatos.Visible = true;
                    divSinDatos.Visible = false;
                }
                else
                {
                    divConDatos.Visible = false;
                    divSinDatos.Visible = true;
                }
            }

        }
    }

    [WebMethod(true)]
    public static void delete(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var archivo = string.Empty;
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Conceptos.Where(x => x.IDConcepto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(entity.Foto))
                        archivo = entity.Foto;
                }
                ConceptosCommon.EliminarConcepto(id, usu);
            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosProductosViewModel getResults(string condicion, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ConceptosCommon.ObtenerConceptos(condicion, page, pageSize, usu);
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(string condicion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Productos";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (condicion != string.Empty)
                        results = results.Where(x => x.Codigo.ToLower().Contains(condicion.ToLower()) || x.Nombre.ToLower().Contains(condicion.ToLower()));

                    dt = results.OrderBy(x => x.Nombre).ToList().Select(x => new
                    {
                        Nombre = x.Nombre.ToUpper(),
                        Tipo = x.Tipo == "S" ? "Servicio" : "Producto",
                        Codigo = x.Codigo.ToUpper(),
                        Descripcion = x.Descripcion,
                        Estado = x.Estado == "A" ? "Activo" : "Inactivo",
                        Precio = x.PrecioUnitario.ToString("N2"),
                        CostoInterno = ((decimal)x.CostoInterno).ToString("N2"),
                        Iva = x.Iva.ToString("#0.00"),
                        Stock = x.Tipo == "S" ? "" : x.Stock.ToString(),
                        Observaciones = x.Observaciones,
                        CodigoProveedor = (x.IDPersona != null ? x.IDPersona.ToString() : ""),
                        nombreProveedor = (x.Personas != null ? (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()) : ""), 
                    }).ToList().ToDataTable();

                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");



                return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");
            }
            catch (Exception e)
            {
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
                throw e;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    //[WebMethod(true)]
    //public static void Duplicar(int id)
    //{
    //    try
    //    {
    //        using (var dbContext = new ACHEEntities())
    //        {
    //            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
    //            var concepto = dbContext.Conceptos.Where(x => x.IDConcepto == id).FirstOrDefault();
    //            var maxIdCocnepto = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == id).Max(x => x.IDConcepto);

    //            var cd = new Conceptos();
    //            cd.Codigo = maxIdCocnepto.ToString();
    //            cd.CostoInterno = concepto.CostoInterno;
    //            cd.Descripcion = concepto.Descripcion;
    //            cd.Estado = concepto.Estado;
    //            cd.FechaAlta = DateTime.Now.Date;
    //            cd.IDUsuario = usu.IDUsuario;
    //            cd.Iva = concepto.Iva;
    //            cd.Nombre = concepto.Nombre;
    //            cd.Observaciones = concepto.Observaciones;
    //            cd.PrecioUnitario = concepto.PrecioUnitario;
    //            cd.Stock = concepto.Stock;
    //            cd.StockMinimo = concepto.StockMinimo;
    //            cd.Tipo = concepto.Tipo;
    //            cd.Foto = concepto.Foto;


    //            dbContext.Conceptos.Add(cd);
    //            dbContext.SaveChanges();

    //            if (!string.IsNullOrWhiteSpace(concepto.Foto))
    //            {
    //                var pathOrigen = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario.ToString() + "/Productos-Servicios/" + concepto.Foto);

    //                if (File.Exists(pathOrigen))
    //                {
    //                    var filename = "concepto-" + cd.IDConcepto.ToString() + Path.GetExtension(pathOrigen);
    //                    var pathdestino = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario.ToString() + "/Productos-Servicios/" + filename);
    //                    File.Copy(pathOrigen, pathdestino);
    //                    cd.Foto = filename;
    //                }
    //            }
    //        }
    //    }
    //    catch (CustomException ex)
    //    {
    //        throw new CustomException(ex.Message);
    //    }
    //}
}