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
using System.Web.Services;
using ACHE.Negocio.Contabilidad;
using ACHE.Extensions;
using System.Xml.Linq;

public partial class modulos_Tesoreria_cheques : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionCheques)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");


                var TieneDatos = dbContext.Cheques.Any(x => x.IDUsuario == CurrentUser.IDUsuario);
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
            CargarBancos();
        }
    }

    private void CargarBancos()
    {
        using (var dbContext = new ACHEEntities())
        {
            var listaBancos = dbContext.Bancos.Where(x => x.IDUsuario == CurrentUser.IDUsuario).ToList();
            foreach (var item in listaBancos)
                ddlBancos.Items.Add(new ListItem(item.BancosBase.Nombre + " Nro:" + item.NroCuenta, item.IDBanco.ToString()));
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
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.PagosFormasDePago.Any(x => x.IDCheque == id))
                        throw new Exception("No puede eliminarse el cheque, ya que fue utilizado en un pago");
                    if (dbContext.CobranzasFormasDePago.Any(x => x.IDCheque == id))
                        throw new Exception("No puede eliminarse el cheque, ya que fue utilizado en una cobranza");
                    else
                    {
                        var entity = dbContext.Cheques.Where(x => x.IDCheque == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (entity != null)
                        {
                            dbContext.Cheques.Remove(entity);
                            dbContext.SaveChanges();
                        }
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosChequesViewModel getResults(string condicion, int page, int pageSize, string vencidos)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.RptChequesAcciones.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (condicion != string.Empty)
                        results = results.Where(x => x.Emisor.Contains(condicion) 
                                                || x.Banco.Contains(condicion) || x.Numero.Contains(condicion)
                                                || x.Cliente.Contains(condicion) || x.Emisor.Contains(condicion)
                                                || x.CUIT.Contains(condicion));

                    var fecha = DateTime.Now.Date;
                    switch (vencidos)
                    {
                        case "a vencer":
                            results = results.Where(x => x.FechaVencimiento >= fecha && x.Accion != "Rechazado");
                            break;
                        case "Rechazados":
                            results = results.Where(x => x.Accion == "Rechazado");
                            break;
                        case "vencidos":
                            results = results.Where(x => x.FechaVencimiento <= fecha && x.Accion != "Rechazado");
                            break;
                    }

                    page--;
                    ResultadosChequesViewModel resultado = new ResultadosChequesViewModel();

                    var personas = dbContext.Personas.Where(w => w.IDUsuario == usu.IDUsuario).ToList();

                    var list = results.OrderBy(x => x.IDCheque).Skip(page * pageSize).Take(pageSize).ToList()
                     .Select(x => new ChequesViewModel()
                     {
                         ID = x.IDCheque,
                         Banco = x.Banco.ToUpper(),
                         Numero = x.Numero,
                         Importe = x.Importe.ToString("N2"),
                         Estado = x.Accion,
                         FechaEmision = x.FechaEmision.ToString("dd/MM/yyyy"),
                         FechaCobro = Convert.ToDateTime(x.FechaCobro).ToString("dd/MM/yyyy"),
                         FechaVencimiento = Convert.ToDateTime(x.FechaVencimiento).ToString("dd/MM/yyyy"),
                         //CantDiasVencimientos = ((fecha - Convert.ToDateTime(x.FechaCobro)).Days < 0 || x.Accion != "") ? ((fecha - Convert.ToDateTime(x.FechaCobro)).Days * -1).ToString() : "-",
                         CantDiasVencimientos = ((Convert.ToDateTime(x.FechaCobro) - fecha).Days > 0 ) ? Math.Abs((Convert.ToDateTime(x.FechaCobro) - fecha).Days).ToString() : "-",
                         Emisor = x.Emisor,
                         Cliente = (x.IdPersona != null) ? personas.Where(w => w.IDPersona == x.IdPersona).Select(w => w.RazonSocial).FirstOrDefault() : ""
                     });

                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();
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
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string export(string condicion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "Tesoreria";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Cheques.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (condicion != string.Empty)
                        results = results.Where(x => x.Emisor.Contains(condicion) || x.BancosBase.Nombre.Contains(condicion) || x.Numero.Contains(condicion));

                    dt = results.OrderBy(x => x.IDCheque).ToList().Select(x => new
                    {
                        Banco = x.BancosBase.Nombre.ToUpper(),
                        Numero = x.Numero,
                        Importe = x.Importe,
                        Estado = x.ChequeAccion.Any() ? x.ChequeAccion.OrderByDescending(y => y.Fecha).First().Accion : "Cargado",
                        FechaEmision = x.FechaEmision.ToString("dd/MM/yyyy"),
                        Emisor = x.Emisor,
                        FechaAlta = x.FechaAlta.ToString("dd/MM/yyyy"),
                        Observaciones = x.Observaciones,
                        FechaCobro = x.FechaCobro != null ? ((DateTime)x.FechaCobro).ToString("dd/MM/yyyy") : "",
                        FechaVencimiento = x.FechaVencimiento != null ? ((DateTime)x.FechaVencimiento).ToString("dd/MM/yyyy") : "",
                        EsPropio = x.EsPropio
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

    #region//** ACCIONES**//
    [WebMethod(true)]
    public static int guardarAccion(int idCheque, string accion, string fechaDeposito, int idBanco)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var idGenerado = 0;
            using (var dbContext = new ACHEEntities())
            {
                ChequeAccion entity = new ChequeAccion();
                entity.FechaAlta = DateTime.Now;
                entity.IDUsuario = usu.IDUsuario;

                entity.IDCheque = idCheque;
                entity.Accion = accion;
                entity.Fecha = Convert.ToDateTime(fechaDeposito);

                if (accion == "Depositado")
                    entity.IDBanco = idBanco;

                dbContext.ChequeAccion.Add(entity);
                dbContext.SaveChanges();

                idGenerado = entity.IDChequeAccion;
            }
            if (accion == "Acreditado")
                ContabilidadCommon.AgregarAsientoChequeAccion(usu, idGenerado);
            return idGenerado;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    [WebMethod(true)]
    public static void deleteAccion(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.ChequeAccion.Where(x => x.IDChequeAccion == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.ChequeAccion.Remove(entity);
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

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosChequesViewModel getResultsAccion(int idCheque, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.ChequeAccion.Where(x => x.IDUsuario == usu.IDUsuario && x.IDCheque == idCheque).AsQueryable();

                    page--;
                    ResultadosChequesViewModel resultado = new ResultadosChequesViewModel();

                    var list = results.OrderBy(x => x.IDChequeAccion).Skip(page * pageSize).Take(pageSize).ToList()
                     .Select(x => new ChequesViewModel()
                     {
                         ID = x.IDChequeAccion,
                         Banco = x.Cheques.BancosBase.Nombre.ToUpper(),
                         Numero = x.Cheques.Numero,
                         Importe = x.Cheques.Importe.ToString("N2"),
                         Accion = x.Accion,
                         FechaEmision = x.Fecha.ToString("dd/MM/yyyy"),
                         Emisor = x.Cheques.Emisor
                     });

                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();
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
    public static ChequesViewModel cargarEntidadAccion(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                ChequesViewModel cheque = new ChequesViewModel();

                var entity = dbContext.ChequeAccion.Where(x => x.IDChequeAccion == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    cheque.ID = entity.IDChequeAccion;
                    cheque.Accion = entity.Accion;
                    cheque.IDCheque = entity.IDCheque;
                    cheque.FechaEmision = entity.Fecha.ToString("dd/MM/yyyy");
                    return cheque;
                }

                return cheque;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerChequesSegunAcciones(string accion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var listaAux = dbContext.RptChequesAcciones.Where(x => x.Accion != accion && x.IDUsuario == usu.IDUsuario).AsQueryable();
                switch (accion)
                {
                    case "Rechazado":
                        listaAux = listaAux.Where(x => x.Accion == "Depositado");
                        break;
                    case "Depositado":
                        listaAux = listaAux.Where(x => x.Accion == "");
                        break;
                    case "Acreditado":
                        var fecha = DateTime.Now.Date;
                        listaAux = listaAux.Where(x => x.Accion == "Depositado");//&& x.FechaCobro >= fecha
                        break;
                }

                var lista = listaAux.OrderBy(x => x.FechaEmision).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDCheque,
                        Nombre = x.Banco + " - Nro:" + x.Numero + "  $" + x.Importe.ToString()
                    }).ToList();

                return listaAux.OrderBy(x => x.FechaEmision).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDCheque,
                        Nombre = x.Banco + " - Nro:" + x.Numero + "  $" + x.Importe.ToString()
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    #endregion

}