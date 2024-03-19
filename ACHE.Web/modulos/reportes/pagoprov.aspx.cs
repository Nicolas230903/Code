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

public partial class modulos_reportes_pagoprov : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.InfoGestionPagoAProveedores)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("dd/MM/yyyy");
            txtFechaHasta.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptPagoProvViewModel getResults(int idPersona, string fechaDesde, string fechaHasta, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];


                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptPagoProveedores.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaPago >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaPago <= dtHasta);
                    }

                    page--;
                    ResultadosRptPagoProvViewModel resultado = new ResultadosRptPagoProvViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.FechaPago).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new RptPagoProvViewModel()
                        {
                            Fecha = x.FechaPago.ToString("dd/MM/yyyy"),
                            Proveedor = x.Proveedor,
                            TipoDocumento = x.TipoDocumento,
                            NroDocumento = x.NroDocumento,
                            CondicionIVA = x.CondicionIVA,
                            NroFactura = x.NroFactura,
                            Importe = x.Importe.ToString("N2"),
                            Iva = x.Iva.ToString("N2"),
                            Total = (x.Total).ToString("N2"),
                            TotalAbonado = x.TotalAbonado.ToString("N2")
                        });
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
    public static string export(int idPersona, string fechaDesde, string fechaHasta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Pago_a_proveedores";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptPagoProveedores.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (idPersona > 0)
                        results = results.Where(x => x.IDPersona == idPersona);
                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.FechaPago >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.FechaPago <= dtHasta);
                    }

                    dt = results.OrderBy(x => x.FechaPago).ToList().Select(x => new 
                    {
                        Fecha = x.FechaPago.ToString("dd/MM/yyyy"),
                        Proveedor = x.Proveedor,
                        TipoDocumento = x.TipoDocumento,
                        NroDocumento = x.NroDocumento,
                        CondicionIVA = x.CondicionIVA,
                        NroFactura = x.NroFactura,
                        ImporteNetoGrav = x.Importe,
                        Iva = x.Iva,
                        Total = (x.Total),
                    }).ToList().ToDataTable();
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