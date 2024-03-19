using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Data;
using System.IO;

public partial class modulos_Tesoreria_gastosBancarios : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            cargarBancos();
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionGastos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");


                var TieneDatos = dbContext.GastosBancarios.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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

    private void cargarBancos()
    {
        using (var dbContext = new ACHEEntities())
        {
            foreach (var item in dbContext.Bancos.Where(x => x.IDUsuario == CurrentUser.IDUsuario).ToList())
                ddlBanco.Items.Add(new ListItem(item.BancosBase.Nombre + " - " + item.NroCuenta, item.IDBanco.ToString()));
        }
        ddlBanco.Items.Insert(0, new ListItem("TODOS", "-1"));
    }

    [System.Web.Services.WebMethod(true)]
    public static void delete(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.GastosBancarios.Where(x => x.IDGastosBancarios == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.GastosBancarios.Remove(entity);
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

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosGastosBancariosViewModel getResults(int idBanco, string fechaDesde, string fechaHasta, string periodo,int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.GastosBancarios.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (idBanco > 0)
                        results = results.Where(x => x.IDBanco == idBanco);


                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                    }

                    if (fechaDesde != string.Empty)
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.Fecha >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Fecha <= dtHasta);
                    }


                    page--;

                    ResultadosGastosBancariosViewModel resultado = new ResultadosGastosBancariosViewModel();

                    var list = results.OrderByDescending(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                      .Select(x => new GastosBancariosViewModel()
                      {
                          ID = x.IDGastosBancarios,
                          NombreBanco = x.Bancos.BancosBase.Nombre.ToUpper(),
                          Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                          Concepto = x.Concepto,
                          Importe = x.Importe,
                          IVA = x.IVA,
                          Debito = x.Debito,
                          Credito = x.Credito,
                          IIBB = x.IIBB,
                          Importe21 = x.Importe21,
                          CreditoComputable = x.CreditoComputable,
                          Importe105 = x.Importe10,
                          PercepcionIVA = x.PercepcionIVA,
                          SIRCREB = x.SIRCREB,
                          Otros = x.Otros
                      });

                    resultado.Items = list.ToList();
                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();

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

    [System.Web.Services.WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(int idBanco, string fechaDesde, string fechaHasta, string periodo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "tesoreria";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.GastosBancarios.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                    if (idBanco > 0)
                        results = results.Where(x => x.IDBanco == idBanco);

                    DateTime dtDesde = new DateTime();
                    DateTime dtHasta = new DateTime();

                    if (fechaDesde != string.Empty)
                    {
                        dtDesde = DateTime.Parse(fechaDesde);
                        results = results.Where(x => x.Fecha >= dtDesde);
                    }
                    if (fechaHasta != string.Empty)
                    {
                        dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        results = results.Where(x => x.Fecha <= dtHasta);
                    }

                    dt = results.OrderByDescending(x => x.Fecha).ToList().Select(x => new
                    {
                        Banco = x.Bancos.BancosBase.Nombre.ToUpper(),
                        Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                        Concepto = x.Concepto,
                        Importe = x.Importe,
                        IVA = x.IVA,
                        Debito = x.Debito,
                        Credito = x.Credito,
                        IIBB = x.IIBB,
                        Importe21 = x.Importe21,
                        CreditoComputable = x.CreditoComputable,
                        Otros = x.Otros,
                        Importe105 = x.Importe10,
                        PercepcionIVA = x.PercepcionIVA,
                        SIRCREB = x.SIRCREB,
                        Total = x.Otros + x.CreditoComputable + x.Importe21 + x.IIBB + x.Credito + x.Debito + x.IVA + x.Importe + x.Importe10 + x.PercepcionIVA + x.SIRCREB
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