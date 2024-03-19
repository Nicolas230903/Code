using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;
using ACHE.Negocio.Helper;

public partial class modulos_reportes_comisiones : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionComisiones)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptComisionesViewModel getResults(int idVendedor, string fechaDesde, string fechaHasta, bool totales, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == "PDV" && x.ImporteComisionVendedor != null).AsQueryable();


                    if (idVendedor == -1)
                    {
                        results = results.Where(x => x.IDUsuario == usu.IDUsuario);
                    }
                    else
                    {
                        if (idVendedor > 0)
                            results = results.Where(x => x.IDUsuario == usu.IDUsuario && x.IdUsuarioAdicional == idVendedor);
                    }                     


                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaComprobante <= dtHasta);
                    }

                    var fecha = DateTime.Now.Date;

                    page--;
                    ResultadosRptComisionesViewModel resultado = new ResultadosRptComisionesViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var us = dbContext.UsuariosView.Where(w => w.IDUsuario == usu.IDUsuario).AsQueryable();

                    IEnumerable<RptComisionesViewModel> list;

                    if (totales)
                    {
                        List<RptComisionesViewModel> lista = new List<RptComisionesViewModel>();

                        var listPrevia = results.OrderBy(x => x.FechaComprobante).ToList()
                            .Select(x => new RptComisionesViewModel()
                            {
                                Vendedor = (us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == x.IdUsuarioAdicional).Any() ? us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == x.IdUsuarioAdicional).Select(s => s.Email).FirstOrDefault() : (us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == 0).Any() ? us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == 0).Select(s => s.Email).FirstOrDefault() : "")),
                                Fecha = x.FechaComprobante.ToString("dd/MM/yyyy"),
                                Cliente = x.Personas.RazonSocial,
                                NroComprobante = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                                Importe = x.ImporteTotalNeto.ToString("N2"),
                                Comision = ((decimal)x.ImporteComisionVendedor).ToString("N2"),
                                ComisionDecimal = (decimal)x.ImporteComisionVendedor
                            });

                        var listVendedores = listPrevia.Select(s => s.Vendedor).Distinct().ToList();

                        foreach(var vendedores in listVendedores)
                        {
                            decimal suma = listPrevia.Where(w => w.Vendedor.Equals(vendedores)).Sum(s => s.ComisionDecimal);

                            RptComisionesViewModel r = new RptComisionesViewModel();
                            r.Vendedor = vendedores;
                            r.Comision = suma.ToString("N2");
                            lista.Add(r);
                        }

                        list = lista.AsEnumerable().Skip(page * pageSize).Take(pageSize).ToList()
                                .Select(x => new RptComisionesViewModel()
                                {
                                    Vendedor = x.Vendedor,
                                    Comision = x.Comision
                                });
                    }
                    else
                    {
                        list = results.OrderBy(x => x.FechaComprobante).Skip(page * pageSize).Take(pageSize).ToList()
                            .Select(x => new RptComisionesViewModel()
                            {
                                Vendedor = (us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == x.IdUsuarioAdicional).Any() ? us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == x.IdUsuarioAdicional).Select(s => s.Email).FirstOrDefault() : (us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == 0).Any() ? us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == 0).Select(s => s.Email).FirstOrDefault() : "")),
                                Fecha = x.FechaComprobante.ToString("dd/MM/yyyy"),
                                Cliente = x.Personas.RazonSocial,
                                NroComprobante = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                                Importe = x.ImporteTotalNeto.ToString("N2"),
                                Comision = ((decimal)x.ImporteComisionVendedor).ToString("N2")
                            });
                    }


                    resultado.Items = list.ToList();
                    return resultado;
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

    [WebMethod(true)]
    public static string export(int idVendedor, string fechaDesde, string fechaHasta, bool totales)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Comisiones";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == "PDV" && x.ImporteComisionVendedor != null).AsQueryable();

                    if (idVendedor == -1)
                    {
                        results = results.Where(x => x.IDUsuario == usu.IDUsuario);
                    }
                    else
                    {
                        if (idVendedor > 0)
                            results = results.Where(x => x.IDUsuario == usu.IDUsuario && x.IdUsuarioAdicional == idVendedor);
                    }

                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaComprobante >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaComprobante <= dtHasta);
                    }

                    var us = dbContext.UsuariosView.Where(w => w.IDUsuario == usu.IDUsuario).AsQueryable();

                    dt = results.OrderBy(x => x.FechaAlta).ToList().Select(x => new
                    {                        
                        Vendedor = (us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == x.IdUsuarioAdicional).Any() ? us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == x.IdUsuarioAdicional).Select(s => s.RazonSocial).FirstOrDefault() : (us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == usu.IDUsuario).Any() ? us.Where(w => w.IDUsuario == usu.IDUsuario && w.IDUsuarioAdicional == usu.IDUsuario).Select(s => s.RazonSocial).FirstOrDefault() : "")),
                        Fecha = x.FechaComprobante.ToString("dd/MM/yyyy"),
                        Cliente = x.Personas.RazonSocial,
                        NroComprobante = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                        Importe = x.ImporteTotalNeto.ToString("N2"),
                        Comision = ((decimal)x.ImporteComisionVendedor).ToString("N2")
                    }).ToList().ToDataTable();

                    if (totales)
                    {
                        dt = dt.AsEnumerable()
                           .GroupBy(r => new { Totales = r["Comision"] })
                           .Select(g => g.OrderBy(r => r["Vendedor"]).First())
                           .CopyToDataTable();
                    }

                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return  (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~","");
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
}