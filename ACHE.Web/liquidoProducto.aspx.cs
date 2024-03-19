using ACHE.Extensions;
using ACHE.Model;
using ACHE.Negocio.Facturacion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class liquidoProducto : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.HerramientasGeneracionLiquidoProducto)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
            hdnIDUsuario.Value = CurrentUser.IDUsuario.ToString();
        }
    }


    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosComprobantesViewModel getResults(string condicion, string periodo, string fechaDesde, 
                                                             string fechaHasta, string fechaUltimoLiquidoProducto, 
                                                             int page, int pageSize, string tipo)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobantes(condicion, periodo, fechaDesde, fechaHasta, fechaUltimoLiquidoProducto, page, pageSize, usu, tipo, false, false, "", false, false);
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
    public static string obtenerFechaUltimoLiquidoProducto()
    {
        var fechaEntrega = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Comprobantes c = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Equals("PDC")).OrderByDescending(o => o.FechaEntrega).FirstOrDefault();

                if (c != null)
                    fechaEntrega = c.FechaEntrega.Value.ToString("dd/MM/yyyy");
            }
            
        }
        return fechaEntrega;
    }

    [WebMethod(true)]
    public static string imprimir(int id, string tipoImpresion, string fechaEntrega)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                DateTime fEntrega = DateTime.Now;
                if (!fechaEntrega.Equals("null"))
                    if (!DateTime.TryParse(fechaEntrega, out fEntrega))                
                        throw new CustomException("Fecha de entrega invalida.");
                
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                List<Comprobantes> lc = new List<Comprobantes>();
                Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();
                if (!fechaEntrega.Equals("null"))
                {
                    c.FechaEntrega = fEntrega;
                    dbContext.SaveChanges();
                }                    
                lc.Add(c);               

                var fileNameLiquidoProducto = "Liquido_Producto_" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss") + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";

                var pathLiquidoProducto = HttpContext.Current.Server.MapPath("~/files/liquidoProducto/" + usu.IDUsuario.ToString() + "/" + fileNameLiquidoProducto);
                if (!System.IO.File.Exists(pathLiquidoProducto))
                {
                    Common.GenerarLiquidoProducto(usu, lc, fileNameLiquidoProducto, tipoImpresion);
                }
                return fileNameLiquidoProducto;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static string imprimirFiltrados(string condicion, string periodo, string fechaDesde, string fechaHasta, string fechaUltimoLiquidoProducto, int page, int pageSize, string tipo, string tipoImpresion, string fechaEntrega)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                DateTime fEntrega = DateTime.Now;
                if (!fechaEntrega.Equals("null"))
                    if (!DateTime.TryParse(fechaEntrega, out fEntrega))
                        throw new CustomException("Fecha de entrega invalida.");

                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                ResultadosComprobantesViewModel res = ComprobantesCommon.ObtenerComprobantes(condicion, periodo, fechaDesde, fechaHasta,
                    fechaUltimoLiquidoProducto, page, pageSize, usu, tipo,false,false, "", false,false);
                List<Comprobantes> lc = new List<Comprobantes>();
                foreach(ComprobantesViewModel cv in res.Items)
                {
                    Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == cv.ID).FirstOrDefault();
                    if (!fechaEntrega.Equals("null"))
                    {
                        c.FechaEntrega = fEntrega;
                        dbContext.SaveChanges();
                    }
                    lc.Add(c);
                }

                var fileNameLiquidoProducto = "Liquido_Producto_" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss") + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";

                var pathLiquidoProducto = HttpContext.Current.Server.MapPath("~/files/liquidoProducto/" + usu.IDUsuario.ToString() + "/" + fileNameLiquidoProducto);
                if (!System.IO.File.Exists(pathLiquidoProducto))
                {
                    Common.GenerarLiquidoProducto(usu, lc, fileNameLiquidoProducto, tipoImpresion);
                }
                return fileNameLiquidoProducto;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static LiquidoProductoMargenes getMargenes()
    {
        try
        {
            LiquidoProductoMargenes result = new LiquidoProductoMargenes();
            result.Horizontal = 0;
            result.Vertical = 0;
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                var dbContext = new ACHEEntities();

                LiquidoProductoMargenes lpm = dbContext.LiquidoProductoMargenes.Where(w => w.IdUsuario == usu.IDUsuario).FirstOrDefault();

                if(lpm == null)
                {
                    LiquidoProductoMargenes nlpm = new LiquidoProductoMargenes();
                    nlpm.IdUsuario = usu.IDUsuario;
                    nlpm.Horizontal = 0;
                    nlpm.Vertical = 0;
                    nlpm.Fecha = DateTime.Now;
                    dbContext.LiquidoProductoMargenes.Add(nlpm);
                    dbContext.SaveChanges();
                }
                else
                {
                    result.Horizontal = lpm.Horizontal;
                    result.Vertical = lpm.Vertical;
                }

                return result;
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
    public static void updateMargenes(string vertical, string horizontal)
    {
        try
        {

            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    LiquidoProductoMargenes lpm = dbContext.LiquidoProductoMargenes.Where(w => w.IdUsuario == usu.IDUsuario).FirstOrDefault();

                    if (lpm == null)
                    {
                        LiquidoProductoMargenes nlpm = new LiquidoProductoMargenes();
                        nlpm.IdUsuario = usu.IDUsuario;
                        nlpm.Horizontal = int.Parse(horizontal);
                        nlpm.Vertical = int.Parse(vertical);
                        nlpm.Fecha = DateTime.Now;
                        dbContext.LiquidoProductoMargenes.Add(nlpm);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        lpm.Horizontal = int.Parse(horizontal);
                        lpm.Vertical = int.Parse(vertical);
                        lpm.Fecha = DateTime.Now;
                        dbContext.SaveChanges();
                    }

                }

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