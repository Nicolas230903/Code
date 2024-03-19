using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Contabilidad;
using FileHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using Aspose.Pdf.Operators;
using Org.BouncyCastle.Math;
using System.Web.UI.HtmlControls;
public partial class importar : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.HerramientasImportacionMasiva)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            Session["DataImport"] = null;
            var tipo = Request.QueryString["tipo"];
            var idLista = Request.QueryString["lista"];
            

            if (!string.IsNullOrEmpty(tipo))
                this.ddlTipo.SelectedValue = tipo;

            if (!string.IsNullOrEmpty(tipo))
                this.hdnIDLista.Value = idLista;


            using (var dbContext = new ACHEEntities())
            {
                var tieneDatos = dbContext.PlanDeCuentas.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
                this.hdnTieneCuentasContables.Value = (tieneDatos) ? "1" : "0";

            }

        }
    }

    #region PRODUCTOS
    [WebMethod(true)]
    public static List<ProductosCSVTmp> leerArchivoCSVProductos(string nombre, string tipo)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        HttpContext.Current.Session["DataImport"] = null;
        List<ProductosCSVTmp> listaproductosCSV = new List<ProductosCSVTmp>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = HttpContext.Current.Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaproductosCSV = ImportacionMasiva.LeerArchivoCSVProductos(tipo, usu.IDUsuario, path);

                if (listaproductosCSV.Any())
                {
                    HttpContext.Current.Session["DataImport"] = listaproductosCSV;

                    return listaproductosCSV;
                }
                else
                {
                    throw new Exception("No se encontraron datos en el archivo.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static void RealizarImportacionProductos()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var lista = (List<ProductosCSVTmp>)HttpContext.Current.Session["DataImport"];
            ImportacionMasiva.RealizarImportacionProductos(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);
        }
        else
        {
            throw new Exception("Por favor, vuelva a iniciar sesión");
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]

    public static void LlenarConceptos_BackUP(int IdUsuario)
    {
        try
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

            SqlCommand comando = new SqlCommand("InsertarConceptos_BackUp_Vacio", cn);
            comando.CommandType = CommandType.StoredProcedure;

            //Parametros
            SqlParameter parIdUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int);
            parIdUsuario.Direction = ParameterDirection.Input;
            parIdUsuario.Value = Convert.ToInt32(IdUsuario);
            comando.Parameters.Add(parIdUsuario);
           

            comando.Connection.Open();
            comando.ExecuteNonQuery();
            comando.Connection.Close();
            comando.Connection.Dispose();
        }
        catch (Exception)
        {
            throw new Exception("Error al intentar copiar datos de la tabla Conceptos");
        }

    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string ExportLote(long NroLote)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = string.Empty;

           
            string path = "~/BackUp/";
 

            try
            {

                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {

                    if(NroLote == 1)
                    {
                        fileName = "Concepto_Actual_" + DateTime.Now.ToString("yyyyMMddhhmm");
                        var results = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                        dt = results.OrderBy(x => x.Nombre).ToList().Select(x => new
                        {
                            Codigo = x.Codigo.ToUpper(),
                            CostoInterno = ((decimal)x.CostoInterno).ToString("N2"),
                            Precio = ((decimal)x.PrecioUnitario).ToString("N2"),
                            Stock = x.Tipo == "S" ? "" : x.Stock.ToString(),
                        }).ToList().ToDataTable();
                    }
                    else
                    {
                        fileName = "Lote_" + NroLote.ToString();

                        var results = dbContext.ConceptosBackUp.Where(x => x.IDUsuario == usu.IDUsuario && x.NroLote == NroLote).AsQueryable();

                        dt = results.OrderBy(x => x.Nombre).ToList().Select(x => new
                        {
                            Codigo = x.Codigo.ToUpper(),
                            CostoInterno = ((decimal)x.CostoInterno).ToString("N2"),
                            Precio = ((decimal)x.PrecioUnitario).ToString("N2"),
                            Stock = x.Tipo == "S" ? "" : x.Stock.ToString(),
                        }).ToList().ToDataTable();
                    
                    }
                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivoSinFecha(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");



                return (path + fileName + ".xlsx").Replace("~", "");
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static List<SelectItem> LlenarDropDownListLotes()
    {
        var items = new List<SelectItem>();
        try
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

            SqlCommand comando = new SqlCommand("Obtener_Registros_lotes", cn);
            comando.CommandType = CommandType.StoredProcedure;

            //Parametros
            SqlParameter parIdUsuario = new SqlParameter("@IDUsuario", SqlDbType.Int);
            parIdUsuario.Direction = ParameterDirection.Input;
            parIdUsuario.Value = Convert.ToInt32(usu.IDUsuario);
            comando.Parameters.Add(parIdUsuario);

            SqlDataAdapter da = new SqlDataAdapter(comando);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows) 
            {
                items.Add(new SelectItem { Value = dr["NroLote"].ToString(), Text = dr["NroLote"].ToString() });
            }

            items.Insert(0, new SelectItem { Value = "", Text = "---Seleccione un numero de Lote---" });
            items.Insert(1, new SelectItem { Value = "1", Text = "Descargar Actual" });
            

            comando.Connection.Open();
            comando.ExecuteNonQuery();
            comando.Connection.Close();
            comando.Connection.Dispose();

        }
        catch (Exception ex)
        {
            throw new Exception("Error al intentar copiar datos de la tabla Conceptos " + ex.Message);
        }

        return items;
    }
    #endregion

    #region PERSONAS
    [WebMethod(true)]
    public static List<PersonasCSVTmp> leerArchivoCSVPersonas(string nombre, string tipo)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        HttpContext.Current.Session["DataImport"] = null;

        List<PersonasCSVTmp> listaPersonasCSV = new List<PersonasCSVTmp>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = HttpContext.Current.Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaPersonasCSV = ImportacionMasiva.LeerArchivoCSVPersonas(tipo, usu.IDUsuario, path);

                if (listaPersonasCSV.Any())
                {
                    HttpContext.Current.Session["DataImport"] = listaPersonasCSV;
                    return listaPersonasCSV;
                }
                else
                    throw new Exception("No se encontraron datos en el archivo.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static void RealizarImportacionPersonas()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if ((List<PersonasCSVTmp>)HttpContext.Current.Session["DataImport"] == null)
                throw new Exception("No se encontraron datos");
            var lista = (List<PersonasCSVTmp>)HttpContext.Current.Session["DataImport"];
            ImportacionMasiva.RealizarImportacionPersonas(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);
        }
        else
        {
            throw new Exception("Por favor, vuelva a iniciar sesión");
        }
    }
    #endregion

    #region LISTA DE PRECIOS
    [WebMethod(true)]
    public static List<ProductosPreciosCSVTmp> leerArchivoCSVListaPrecios(string nombre, string idLista)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        HttpContext.Current.Session["DataImport"] = null;

        List<ProductosPreciosCSVTmp> listaPreciosCSV = new List<ProductosPreciosCSVTmp>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = HttpContext.Current.Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaPreciosCSV = ImportacionMasiva.LeerArchivoCSVListaPrecios(idLista, usu.IDUsuario, path);

                if (listaPreciosCSV.Any())
                {
                    HttpContext.Current.Session["DataImport"] = listaPreciosCSV;

                    return listaPreciosCSV;
                }
                else
                {
                    throw new Exception("No se encontraron datos en el archivo.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static void RealizarImportacionListaPrecios()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
         
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var lista = (List<ProductosPreciosCSVTmp>)HttpContext.Current.Session["DataImport"];
            ImportacionMasiva.RealizarImportacionListaPrecios(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString,usu.IDUsuario);

        
        }
        else
        {
            throw new Exception("Por favor, vuelva a iniciar sesión");
        }

    }
    #endregion

    #region PLAN DE CUENTAS
    [WebMethod(true)]
    public static List<PlanDeCuentasCSVTmp> LeerArchivoCSVPlanDeCuentas(string nombre)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        HttpContext.Current.Session["DataImport"] = null;
        List<PlanDeCuentasCSVTmp> listaPlanDeCuentasCSV = new List<PlanDeCuentasCSVTmp>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = HttpContext.Current.Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaPlanDeCuentasCSV = ImportacionMasiva.LeerArchivoCSVPlanDeCuentas(usu.IDUsuario, path);
                if (listaPlanDeCuentasCSV.Any())
                {
                    HttpContext.Current.Session["DataImport"] = listaPlanDeCuentasCSV;
                    return listaPlanDeCuentasCSV;
                }
                else
                    throw new Exception("No se encontraron datos en el archivo.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static void RealizarImportacionPlanDeCuentas()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            try
            {
                var lista = (List<PlanDeCuentasCSVTmp>)HttpContext.Current.Session["DataImport"];
                ImportacionMasiva.RealizarImportacionPlanDeCuentas(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString, usu.IDUsuario);
                ImportacionMasiva.ReferenciarPlanDeCuentas(usu.IDUsuario, lista);
            }
            catch (Exception)
            {
                ContabilidadCommon.EliminarPlanDeCuentasActual(usu.IDUsuario);
                ContabilidadCommon.EliminarConfiguracionPlanDeCuenta(usu.IDUsuario);
                throw new Exception("El plan de cuentas no pudo ser importado.");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion

    #region FACTURAS
    [WebMethod(true)]
    public static List<FacturasCSVTmp> leerArchivoCSVFacturas(string nombre)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        HttpContext.Current.Session["DataImport"] = null;

        List<FacturasCSVTmp> listaFacturasCSV = new List<FacturasCSVTmp>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                string path = string.Empty;
                if (!string.IsNullOrEmpty(nombre))
                    path = HttpContext.Current.Server.MapPath("~/files/importaciones/Datos/" + nombre);

                listaFacturasCSV = ImportacionMasiva.LeerArchivoCSVFacturas(usu, path);

                if (listaFacturasCSV.Any())
                {
                    HttpContext.Current.Session["DataImport"] = listaFacturasCSV;
                    return listaFacturasCSV;
                }
                else
                {
                    throw new Exception("No se encontraron datos en el archivo.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); ;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static void RealizarImportacionFacturas()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                if ((List<FacturasCSVTmp>)HttpContext.Current.Session["DataImport"] == null)
                    throw new Exception("No se encontraron datos");
                var lista = (List<FacturasCSVTmp>)HttpContext.Current.Session["DataImport"];
                ImportacionMasiva.RealizarImportacionFacturas(lista, ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);
                ImportacionMasiva.ReferenciarFacturasClientes(lista, usu);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); ;
            }
        }
        else
        {
            throw new Exception("Por favor, vuelva a iniciar sesión");
        }
    }
    #endregion
}