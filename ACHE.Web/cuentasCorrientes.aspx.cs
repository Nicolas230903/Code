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
using ACHE.Negocio.Common;
using ACHE.Extensions;

public partial class cuentasCorrientes : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionCuentasCorrientes)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            var tipo = Request.QueryString["tipo"];
            hdnTipo.Value = tipo;

            if (CurrentUser.TipoUsuario == "B")
            {
                if (!PermisosModulos.mostrarPersonaSegunPermiso(tipo))
                    Response.Redirect("home.aspx");
            }
            if (tipo == "C")
            {
                litTitulo.Text = "<i class='fa fa-suitcase'></i> Cuenta Corriente Clientes";
                litPath.Text = "Clientes";
            }
            else if (tipo == "P")
            {
                litTitulo.Text = "<i class='fa fa-users'></i> Cuenta Corriente Proveedores";
                litPath.Text = "Proveedores";
            }
            else
                Response.Redirect("home.aspx");

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            hdnIDUsuario.Value = usu.IDUsuario.ToString();
            divConDatos.Visible = true;
            divSinDatos.Visible = false;
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosPersonasViewModel getResults(string condicion, string tipo, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return PersonasCommon.ObtenerPersonas(condicion, tipo, page, pageSize, usu);
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
    public static ResultadosCuentaCorrienteViewModel getResultsCuenta(string condicion, string tipo, bool saldoPendiente, bool deudaPorEDM, int page, int pageSize)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return PersonasCommon.ObtenerPersonasCuentaCorriente(condicion, tipo, saldoPendiente, deudaPorEDM, page, pageSize, usu);

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
    public static string exportResumen(string condicion, string tipo, bool saldoPendiente, bool deudaPorEDM)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "CuentasCorrientes";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();                    

                    if (!string.IsNullOrWhiteSpace(condicion))
                        results = results.Where(x => x.RazonSocial.Contains(condicion.Trim()) || x.NombreFantansia.Contains(condicion.Trim()) || x.NroDocumento.Contains(condicion.Trim()) || x.Codigo.Contains(condicion.Trim()));

                    var tipoComprobantesNoIncluidos = new[] { "PDC", "DDC", "NDP", "COT", "RCB", "RCC" };

                    var listaComprobantes = dbContext.Comprobantes
                                                .Include("Personas")
                                                .Where(x => x.IDUsuario == usu.IDUsuario && !tipoComprobantesNoIncluidos.Contains(x.Tipo)).ToList();

                    var listaCobranzas = dbContext.CobranzasDetalle.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario).ToList();

                    var listaCobranzasRetenciones = dbContext.CobranzasRetenciones.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario).ToList();

                    var listaCheques = dbContext.RptChequesAcciones.Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario && x.EsPropio == false).ToList();

                    List<CuentaCorrienteViewModel> lr = new List<CuentaCorrienteViewModel>();
                    foreach (Personas p in results)
                    {
                        CuentaCorrienteViewModel cuenta = new CuentaCorrienteViewModel();
                        ResultadosRptCcDetalleViewModel r = new ResultadosRptCcDetalleViewModel();
                        r.TotalPage = 1;// ((results.Count() - 1) / pageSize) + 1;
                        r.Items = new List<RptCcDetalleViewModel>();

                        if (tipo == "C")
                            PersonasCommon.verComoCliente(p.IDPersona, r, dbContext, deudaPorEDM, listaComprobantes, listaCobranzas, listaCobranzasRetenciones, listaCheques);
                        else
                            PersonasCommon.verComoProveedor(p.IDPersona, r, dbContext, deudaPorEDM);

                        if (r.Items.Count() > 0)
                        {
                            RptCcDetalleViewModel saldo = r.Items.Where(w => w.Cobrado == "Saldo").FirstOrDefault();

                            if (saldo != null)
                            {
                                cuenta.Saldo = saldo.Total;
                                cuenta.ID = p.IDPersona;
                                cuenta.RazonSocial = p.RazonSocial.ToUpper();
                                cuenta.NombreFantasia = (p.CondicionIva == "RI" || p.CondicionIva == "EX") ? p.NombreFantansia.ToUpper() : "";
                                cuenta.CondicionIva = p.CondicionIva;
                                cuenta.NroDoc = p.NroDocumento;
                                lr.Add(cuenta);
                            }
                        }
                    }

                    dt = lr.OrderBy(x => x.ID).ToList().ToDataTable();

                    if (saldoPendiente)                       
                        dt = lr.Where(w => !w.Saldo.Equals("0,00")).OrderBy(x => x.ID).ToList().ToDataTable();
                    else
                        dt = lr.OrderBy(x => x.ID).ToList().ToDataTable();

                    if (dt.Rows.Count > 0)
                        CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                    else
                        throw new Exception("No se encuentran datos para los filtros seleccionados");

                    return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");

                }   
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
    public static string exportDetalle(string condicion, string tipo, bool saldoPendiente, bool deudaPorEDM)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            string fileName = "CuentasCorrientes";
            string path = "~/tmp/";
            try
            {
                DataTable dt = new DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();


                    if (!string.IsNullOrWhiteSpace(condicion))
                        results = results.Where(x => x.RazonSocial.Contains(condicion.Trim()) || x.NombreFantansia.Contains(condicion.Trim()) || x.NroDocumento.Contains(condicion.Trim()) || x.Codigo.Contains(condicion.Trim()));

                    var tipoComprobantesNoIncluidos = new[] { "PDC", "DDC", "NDP", "COT", "RCB", "RCC" };

                    var listaComprobantes = dbContext.Comprobantes
                                                .Include("Personas")
                                                .Where(x => x.IDUsuario == usu.IDUsuario && !tipoComprobantesNoIncluidos.Contains(x.Tipo)).ToList();
                    var listaCobranzas = dbContext.CobranzasDetalle.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario).ToList();

                    var listaCobranzasRetenciones = dbContext.CobranzasRetenciones.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario).ToList();

                    var listaCheques = dbContext.RptChequesAcciones.Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario && x.EsPropio == false).ToList();

                    List<RptCcDetalleViewModel> lr = new List<RptCcDetalleViewModel>();
                    foreach (Personas p in results)
                    {
                        CuentaCorrienteViewModel cuenta = new CuentaCorrienteViewModel();
                        ResultadosRptCcDetalleViewModel r = new ResultadosRptCcDetalleViewModel();
                        r.Items = new List<RptCcDetalleViewModel>();

                        if (tipo == "C")
                            verComoCliente(p.IDPersona, r, dbContext, saldoPendiente, deudaPorEDM, listaComprobantes, listaCobranzas, listaCobranzasRetenciones, listaCheques);
                        else
                            verComoProveedor(p.IDPersona, r, dbContext, saldoPendiente, deudaPorEDM);

                        if (r.Items != null)
                        {
                            foreach (RptCcDetalleViewModel item in r.Items)
                            {
                                lr.Add(item);
                            }
                        }

                    }

                    dt = lr.ToList().ToDataTable();

                    if (dt.Rows.Count > 0)
                        CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                    else
                        throw new Exception("No se encuentran datos para los filtros seleccionados");

                    return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");

                }
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



    private static void verComoCliente(int idPersona, ResultadosRptCcDetalleViewModel resultado, ACHEEntities dbContext, 
                                            bool saldoPendiente, bool deudaPorEDM, List<Comprobantes> listaComprobantesDb,
                                            List<CobranzasDetalle> listaCobranzasDb, List<CobranzasRetenciones> listaCobranzasRetencionesDb,
                                            List<RptChequesAcciones> listaChequesDb)
    {
        decimal total = 0;
        RptCcDetalleViewModel detalleCc;
        RptCcDetalleViewModel detalleCob;
        RptCcDetalleViewModel detalleCobRet;

        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        string[] listaNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
        string[] listaNotasDebito = { "NDA", "NDB", "NDC", "NDAMP", "NDBMP", "NDCMP" };

        var listaComprobantes = listaComprobantesDb.Where(x => (x.IDPersona == idPersona || idPersona == -1)).ToList();
        var listaCobranzas = listaCobranzasDb.Where(x => (x.Cobranzas.IDPersona == idPersona || idPersona == -1)).ToList();

        if (deudaPorEDM)
        {

            var list = listaComprobantes
                        .Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario)
                        .OrderBy(x => x.FechaComprobante)
                        .ToList();
            var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
            resultado.TotalItems = list.Count();

            if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
            {
                resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                total = Convert.ToDecimal(persona.SaldoInicial);
                resultado.TotalItems++;
            }


            if (list.Any())
            {
                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                foreach (var detalle in list)
                {
                    if (!listaNotas.Contains(detalle.Tipo))
                    {

                        //Me fijo si hay comprobantes que esten vinculados a este.
                        var comprobantesVinculados = listaComprobantes
                                                        .Where(x => x.IdComprobanteVinculado == detalle.IDComprobante && x.IDUsuario == usu.IDUsuario && !x.Tipo.Equals("PDV"))
                                                        .GroupBy(a => a.IdComprobanteVinculado)
                                                        .Select(a => new { SumaNeto = a.Sum(b => b.ImporteTotalNeto), SumaBruto = a.Sum(b => b.ImporteTotalBruto) })
                                                        .OrderByDescending(a => a.SumaNeto)
                                                        .FirstOrDefault();

                        if (comprobantesVinculados != null)
                        {
                            detalleCc = new RptCcDetalleViewModel();

                            if (detalle.Tipo.Equals("PDV"))
                            {
                                //total += detalle.ImporteTotalBruto - facturaVinculada.SumaBruto;

                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = "";
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = "--";
                            }
                            else
                            {
                                if (detalle.Tipo.Equals("EDA"))
                                {
                                    //total += detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto;

                                    detalleCc.Raiz = "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = "--";
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = "--";
                                }
                            }

                            temp.Add(detalleCc);
                        }
                        else // No hay comprobantes que esten vinculados a este
                        {
                            if (detalle.Tipo.Equals("PDV"))
                            {
                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = "--";
                            }
                            else
                            {
                                if (detalle.Tipo.Equals("EDA"))
                                {
                                    total += detalle.ImporteTotalBruto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = "";
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }
                                else
                                {
                                    if (detalle.CAE != null)
                                    {
                                        total += detalle.ImporteTotalNeto;

                                        detalleCc = new RptCcDetalleViewModel();
                                        detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                        detalleCc.CAE = "SI";
                                        detalleCc.RazonSocial = persona.RazonSocial;
                                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                        detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                        detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.VaADeuda = (detalle.ImporteTotalNeto).ToString("N2");
                                        detalleCc.FechaCobro = "";
                                        detalleCc.Total = total.ToString("N2");
                                    }
                                    else
                                    {
                                        total += detalle.ImporteTotalBruto;

                                        detalleCc = new RptCcDetalleViewModel();
                                        detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                        detalleCc.CAE = "";
                                        detalleCc.RazonSocial = persona.RazonSocial;
                                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                        detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                        detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.IVA = "--";
                                        detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.FechaCobro = "";
                                        detalleCc.Total = total.ToString("N2");
                                    }
                                }
                            }

                            temp.Add(detalleCc);
                        }

                    }
                    else //Es Nota de credito o debito
                    {
                        if (detalle.CAE != null)
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalNeto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalNeto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "SI";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                            detalleCc.VaADeuda = "--";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }
                        else
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalBruto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalBruto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = "--";
                            detalleCc.VaADeuda = "--";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }

                        temp.Add(detalleCc);
                    }

                    var cobrazasList = listaCobranzas.Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                    foreach (var cobranza in cobrazasList)
                    {
                        total -= cobranza.Importe;

                        detalleCob = new RptCcDetalleViewModel();
                        detalleCob.Raiz = "-";
                        detalleCob.RazonSocial = persona.RazonSocial;
                        if (cobranza.Cobranzas.Tipo != "SIN")
                            detalleCob.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                        else
                            detalleCob.ComprobanteAplicado = "Sin comprobante";

                        detalleCob.Comprobante = "";
                        detalleCob.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                        detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        //{
                        //    var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                        //    total -= ret;
                        //    detalleCob.Cobrado = "- " + (cobranza.Importe + ret).ToString("N2");
                        //}
                        //else
                        //    detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        detalleCob.Total = total.ToString("N2");
                        temp.Add(detalleCob);


                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        if (listaCobranzasRetencionesDb.Where(w => w.IDCobranza == cobranza.IDCobranza).Any())
                        {
                            detalleCobRet = new RptCcDetalleViewModel();
                            var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                            total -= ret;
                            detalleCobRet.Cobrado = "- " + (ret).ToString("N2");
                            detalleCobRet.Raiz = "-";
                            detalleCobRet.RazonSocial = persona.RazonSocial;
                            if (cobranza.Cobranzas.Tipo != "SIN")
                                detalleCobRet.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                            else
                                detalleCobRet.ComprobanteAplicado = "Sin comprobante";

                            detalleCobRet.Comprobante = "";
                            detalleCobRet.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                            detalleCobRet.Total = total.ToString("N2");
                            temp.Add(detalleCobRet);
                            listaCobranzasRetencionesDb.RemoveAll(w => w.IDCobranza == cobranza.IDCobranza);
                        }
                    }
                }

                var chequesResto = listaChequesDb.Where(w => w.IdPersona == idPersona)
                            .Sum(a => a.Resto);

                if (chequesResto > 0)
                {
                    total += (decimal)chequesResto;

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.Cobrado = "Resto de cheques sin cobranza asignada";
                    detalleCc.Total = total.ToString("N2");
                    temp.Add(detalleCc);

                }

                foreach (RptCcDetalleViewModel r in temp)
                    resultado.Items.Add(r);

                detalleCc = new RptCcDetalleViewModel();
                detalleCc.RazonSocial = persona.RazonSocial;
                detalleCc.Cobrado = "Saldo";
                detalleCc.Total = total.ToString("N2");
                resultado.Items.Add(detalleCc);


            }
        }
        else // No es por Entrega de mercaderia
        {

            var list = listaComprobantes
                        .Where(x => x.IDPersona == idPersona && !x.Tipo.Equals("EDA"))
                        .OrderBy(x => x.FechaComprobante)
                        .ToList();
            var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
            resultado.TotalItems = list.Count();

            if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
            {
                resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                total = Convert.ToDecimal(persona.SaldoInicial);
                resultado.TotalItems++;
            }

            if (list.Any())
            {
                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                foreach (var detalle in list)
                {
                    if (!listaNotas.Contains(detalle.Tipo))
                    {
                        List<int?> listaEda = listaComprobantes.Where(x => x.IdComprobanteVinculado == detalle.IDComprobante && x.Tipo.Equals("EDA"))
                                                        .Select(a => (int?)a.IDComprobante)
                                                        .ToList();

                        listaEda.Add(detalle.IDComprobante);

                        var comprobantesVinculados = listaComprobantes
                                                        .Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F"))
                                                        .GroupBy(i => 1)
                                                        .Select(a => new { SumaNeto = a.Sum(b => b.ImporteTotalNeto), SumaBruto = a.Sum(b => b.ImporteTotalBruto) })
                                                        .OrderByDescending(a => a.SumaNeto)
                                                        .FirstOrDefault();

                        //List<int?> listaFac = listaComprobantes.Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F"))
                        //        .Select(a => (int?)a.IDComprobante)
                        //        .ToList();


                        //var comprobantesVinculadosNetos = listaComprobantes
                        //                                .Where(x => listaFac.Contains(x.IdComprobanteAsociado) && x.Tipo.Substring(0, 1).Equals("N"))
                        //                                .GroupBy(i => 1)
                        //                                .Select(a => new { SumaNeto = a.Sum(b => b.ImporteTotalNeto), SumaBruto = a.Sum(b => b.ImporteTotalBruto) })
                        //                                .OrderByDescending(a => a.SumaNeto)
                        //                                .FirstOrDefault();


                        if (comprobantesVinculados != null)
                        {

                            if (detalle.Tipo.Equals("PDV"))
                            {
                                //decimal tempSubTotal = detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto + (comprobantesVinculadosNetos != null ? comprobantesVinculadosNetos.SumaBruto : 0);
                                //decimal tempTotal = tempSubTotal >= 0 ? tempSubTotal : 0;
                                //total += tempTotal;

                                total += detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto);

                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                                detalleCc.IVA = "--";                                
                                detalleCc.VaADeuda = (detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto)).ToString("N2");
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = total.ToString("N2");
                            }
                            else
                            {
                                if (detalle.CAE != null)
                                {
                                    total += detalle.ImporteTotalNeto - ((detalle.ImporteTotalNeto - comprobantesVinculados.SumaNeto) < 0 ? detalle.ImporteTotalNeto : comprobantesVinculados.SumaNeto);

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = "-";
                                    detalleCc.CAE = "SI";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto)).ToString("N2");
                                    detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.VaADeuda = (detalle.ImporteTotalNeto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }
                                else
                                {
                                    total += detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto);

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto)).ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }

                            }

                            temp.Add(detalleCc);
                        }
                        else // El comprobante no está vinculado a otro comprobante
                        {
                            if (detalle.Tipo.Equals("PDV"))
                            {
                                total += detalle.ImporteTotalBruto;

                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = total.ToString("N2");
                            }
                            else
                            {

                                if (detalle.CAE != null)
                                {
                                    total += detalle.ImporteTotalNeto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                    detalleCc.CAE = "SI";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.VaADeuda = (detalle.IdComprobanteVinculado == null) ? "--" : (detalle.ImporteTotalNeto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }
                                else
                                {
                                    total += detalle.ImporteTotalBruto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = (detalle.IdComprobanteVinculado == null) ? "--" : (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }

                            }

                            temp.Add(detalleCc);
                        }

                    }
                    else // Es una nota de credito
                    {
                        if (detalle.CAE != null)
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalNeto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalNeto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "SI";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                            detalleCc.VaADeuda = "";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }
                        else
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalBruto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalBruto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalNeto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = "--";
                            detalleCc.VaADeuda = "";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }

                        temp.Add(detalleCc);
                    }

                    var cobrazasList = listaCobranzas.Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                    foreach (var cobranza in cobrazasList)
                    {
                        total -= cobranza.Importe;

                        detalleCob = new RptCcDetalleViewModel();
                        detalleCob.Raiz = "-";
                        detalleCob.RazonSocial = persona.RazonSocial;
                        if (cobranza.Cobranzas.Tipo != "SIN")
                            detalleCob.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                        else
                            detalleCob.ComprobanteAplicado = "Sin comprobante";

                        detalleCob.Comprobante = "";
                        detalleCob.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                        detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        //{
                        //    var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                        //    total -= ret;
                        //    detalleCob.Cobrado = "- " + (cobranza.Importe + ret).ToString("N2");
                        //}
                        //else
                        //    detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        detalleCob.Total = total.ToString("N2");
                        temp.Add(detalleCob);

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        if (listaCobranzasRetencionesDb.Where(w => w.IDCobranza == cobranza.IDCobranza).Any())
                        {
                            detalleCobRet = new RptCcDetalleViewModel();
                            var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                            total -= ret;
                            detalleCobRet.Cobrado = "- " + (ret).ToString("N2");
                            detalleCobRet.Raiz = "-";
                            detalleCobRet.RazonSocial = persona.RazonSocial;
                            if (cobranza.Cobranzas.Tipo != "SIN")
                                detalleCobRet.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                            else
                                detalleCobRet.ComprobanteAplicado = "Sin comprobante";

                            detalleCobRet.Comprobante = "";
                            detalleCobRet.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                            detalleCobRet.Total = total.ToString("N2");
                            temp.Add(detalleCobRet);
                            listaCobranzasRetencionesDb.RemoveAll(w => w.IDCobranza == cobranza.IDCobranza);
                        }
                    }
                }

                var chequesResto = listaChequesDb.Where(w => w.IdPersona == idPersona)
                            .Sum(a => a.Resto);

                if (chequesResto > 0)
                {
                    total += (decimal)chequesResto;

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.Cobrado = "Resto de cheques sin cobranza asignada";
                    detalleCc.Total = total.ToString("N2");
                    temp.Add(detalleCc);

                }

                foreach (RptCcDetalleViewModel r in temp)
                    resultado.Items.Add(r);

                detalleCc = new RptCcDetalleViewModel();
                detalleCc.RazonSocial = persona.RazonSocial;
                detalleCc.Cobrado = "Saldo";
                detalleCc.Total = total.ToString("N2");
                resultado.Items.Add(detalleCc);


            }

        }
    }

    private static RptCcDetalleViewModel AgregarSaldoInicialCliente(ACHEEntities dbContext, decimal? saldo)
    {
        var detalleCc = new RptCcDetalleViewModel();
        detalleCc.Comprobante = "Saldo inicial";
        detalleCc.Fecha = "";
        detalleCc.Importe = Convert.ToDecimal(saldo).ToString("N2");
        detalleCc.Total = Convert.ToDecimal(saldo).ToString("N2");

        return detalleCc;
    }

    private static void verComoProveedor(int idPersona, ResultadosRptCcDetalleViewModel resultado, ACHEEntities dbContext, bool saldoPendiente, bool deudaPorEDM)
    {
        decimal total = 0;
        RptCcDetalleViewModel detalleCc;
        RptCcDetalleViewModel detalleCob;
        //string[] listaNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
        string[] listaNotas = { "NCA", "NCB", "NCC", "NCAMP", "NCBMP", "NCCMP" };

        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        if (idPersona != -1)
        {
            var list = dbContext.Compras.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario && !x.Tipo.Equals("EDA"))
                    .OrderBy(x => x.Fecha)
                    .ToList();
            var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
            resultado.TotalItems = list.Count();

            if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
            {
                resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                total = Convert.ToDecimal(persona.SaldoInicial);
            }

            if (list.Any())
            {
                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                foreach (var detalle in list)
                {
                    var imp = detalle.TotalImpuestos;

                    if (listaNotas.Contains(detalle.Tipo))
                        total -= Convert.ToDecimal(detalle.Total + detalle.Iva + imp);
                    else
                        total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                    //total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                    detalleCc = new RptCcDetalleViewModel();
                    Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == detalle.IdComprobante).FirstOrDefault();
                    if (c != null)
                        detalleCc.PDC = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                    detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");

                    if (listaNotas.Contains(detalle.Tipo))
                        detalleCc.Importe = "-" + Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                    else
                        detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");

                    detalleCc.Total = total.ToString("N2");
                    temp.Add(detalleCc);

                    var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                    foreach (var item in pagosList)
                    {
                        total -= item.Importe;

                        detalleCob = new RptCcDetalleViewModel();
                        detalleCob.RazonSocial = persona.RazonSocial;
                        detalleCob.ComprobanteAplicado = "Sin comprobante";

                        detalleCob.Comprobante = "PAGO 000-" + item.IDPago.ToString("#00000000");
                        detalleCob.FechaCobro = item.Pagos.FechaPago.ToString("dd/MM/yyyy");

                        if (item.Pagos.PagosRetenciones.Any())
                        {
                            var ret = item.Pagos.PagosRetenciones.Sum(x => x.Importe);
                            total -= ret;
                            detalleCob.Cobrado = "- " + (item.Importe + ret).ToString("N2");
                        }
                        else
                            detalleCob.Cobrado = "- " + item.Importe.ToString("N2");

                        detalleCob.Total = total.ToString("N2");
                        temp.Add(detalleCob);
                    }
                }

                if (saldoPendiente)
                {
                    if (total > 0)
                    {
                        foreach (RptCcDetalleViewModel r in temp)
                            resultado.Items.Add(r);

                        detalleCc = new RptCcDetalleViewModel();
                        detalleCc.RazonSocial = persona.RazonSocial;
                        detalleCc.Cobrado = "Saldo";
                        detalleCc.Total = total.ToString("N2");
                        resultado.Items.Add(detalleCc);
                    }
                }
                else
                {
                    foreach (RptCcDetalleViewModel r in temp)
                        resultado.Items.Add(r);

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.RazonSocial = persona.RazonSocial;
                    detalleCc.Cobrado = "Saldo";
                    detalleCc.Total = total.ToString("N2");
                    resultado.Items.Add(detalleCc);
                }

            }
        }
        else
        {
            var personas = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).OrderBy(o => o.RazonSocial).ToList();

            foreach (Personas p in personas)
            {
                var list = dbContext.Compras.Where(x => x.IDPersona == p.IDPersona && x.IDUsuario == usu.IDUsuario && !x.Tipo.Equals("EDA"))
                    .OrderBy(x => x.Fecha)
                    .ToList();
                var persona = dbContext.Personas.Where(x => x.IDPersona == p.IDPersona).FirstOrDefault();
                resultado.TotalItems = list.Count();

                if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
                {
                    resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                    total = Convert.ToDecimal(persona.SaldoInicial);
                }

                if (list.Any())
                {
                    List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                    foreach (var detalle in list)
                    {
                        var imp = detalle.TotalImpuestos;

                        if (listaNotas.Contains(detalle.Tipo))
                            total -= Convert.ToDecimal(detalle.Total + detalle.Iva + imp);
                        else
                            total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                        //total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                        detalleCc = new RptCcDetalleViewModel();
                        detalleCc.RazonSocial = persona.RazonSocial;
                        Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == detalle.IdComprobante).FirstOrDefault();
                        if (c != null)
                            detalleCc.PDC = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                        detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");

                        if (listaNotas.Contains(detalle.Tipo))
                            detalleCc.Importe = "-" + Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                        else
                            detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");

                        detalleCc.Total = total.ToString("N2");
                        temp.Add(detalleCc);

                        var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                        foreach (var item in pagosList)
                        {
                            total -= item.Importe;

                            detalleCob = new RptCcDetalleViewModel();
                            detalleCob.RazonSocial = persona.RazonSocial;
                            detalleCob.ComprobanteAplicado = "Sin comprobante";

                            detalleCob.Comprobante = "PAGO 000-" + item.IDPago.ToString("#00000000");
                            detalleCob.FechaCobro = item.Pagos.FechaPago.ToString("dd/MM/yyyy");

                            if (item.Pagos.PagosRetenciones.Any())
                            {
                                var ret = item.Pagos.PagosRetenciones.Sum(x => x.Importe);
                                total -= ret;
                                detalleCob.Cobrado = "- " + (item.Importe + ret).ToString("N2");
                            }
                            else
                                detalleCob.Cobrado = "- " + item.Importe.ToString("N2");

                            detalleCob.Total = total.ToString("N2");
                            temp.Add(detalleCob);
                        }
                    }

                    if (saldoPendiente)
                    {
                        if (total > 0)
                        {
                            foreach (RptCcDetalleViewModel r in temp)
                                resultado.Items.Add(r);

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Cobrado = "Saldo";
                            detalleCc.Total = total.ToString("N2");
                            resultado.Items.Add(detalleCc);
                        }
                    }
                    else
                    {
                        foreach (RptCcDetalleViewModel r in temp)
                            resultado.Items.Add(r);

                        detalleCc = new RptCcDetalleViewModel();
                        detalleCc.RazonSocial = persona.RazonSocial;
                        detalleCc.Cobrado = "Saldo";
                        detalleCc.Total = total.ToString("N2");
                        resultado.Items.Add(detalleCc);
                    }
                }

                total = 0;

            }
        }


    }

}