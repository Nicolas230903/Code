using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Services;
using System.Web.Script.Services;
using System.Configuration;
using System.Globalization;
using ACHE.Negocio.Common;
using ACHE.FacturaElectronica;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using System.Xml.Serialization;
using System.Xml;
using ACHE.FacturaElectronica.VEConsumerService;
using DocumentFormat.OpenXml.Presentation;

public partial class home : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            var nombrePlan = string.Empty;
            var cantDias = 0;

            if (!CurrentUser.TieneFE) // && CurrentUser.ModoQA)// && CurrentUser.CondicionIVA == "RI")
                divMensajeFCE.Visible = true;
            else
                divMensajeFCE.Visible = false;

            //Permiso de Venta y Compra 
            if (CurrentUser.TipoUsuario == "B" && PermisosModulos.ocultarHeader(1) && PermisosModulos.ocultarHeader(2))
                hdnPanelDeControl.Value = "1";
            else if (CurrentUser.TipoUsuario == "A")
                hdnPanelDeControl.Value = "1";
            else
                hdnPanelDeControl.Value = "0";



            string basePath = Server.MapPath("~/files/explorer");
            if (!Directory.Exists(basePath + "//" + CurrentUser.IDUsuario))
            {
                UsuarioCommon.CreateFolders(CurrentUser.IDUsuario, basePath);
            }

            if (CurrentUser.CondicionIVA != "MO" && !Directory.Exists(basePath + "//" + CurrentUser.IDUsuario + "//Balances"))
            {
                UsuarioCommon.CreateFoldersRI(CurrentUser.IDUsuario, basePath);
            }

            basePath = Server.MapPath("~/files/explorer/" + CurrentUser.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString());
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            basePath = Server.MapPath("~/files/explorer/" + CurrentUser.IDUsuario + "/Compras/" + DateTime.Now.Year.ToString());
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            basePath = Server.MapPath("~/files/comprobantes");
            if (!Directory.Exists(basePath + "//" + CurrentUser.IDUsuario))
            {
                Directory.CreateDirectory(basePath + "//" + CurrentUser.IDUsuario);
            }

            basePath = Server.MapPath("~/files/remitos");
            if (!Directory.Exists(basePath + "//" + CurrentUser.IDUsuario))
                Directory.CreateDirectory(basePath + "//" + CurrentUser.IDUsuario);

            basePath = Server.MapPath("~/files/Presupuestos");
            if (!Directory.Exists(basePath + "//" + CurrentUser.IDUsuario))
                Directory.CreateDirectory(basePath + "//" + CurrentUser.IDUsuario);

            using (var dbContext = new ACHEEntities())
            {
                var tieneDatosOK = dbContext.Usuarios.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Domicilio != "" && x.Personeria != "" && x.CondicionIva != "");
                if (!tieneDatosOK)
                    Response.Redirect("~/finRegistro.aspx");


                if (Common.AlertaPlan(dbContext, CurrentUser.IDUsuario, ref nombrePlan, ref cantDias))
                {
                    var txtDias = string.Empty;
                    if (cantDias == 1)
                        txtDias = cantDias.ToString() + " dia.";
                    else
                        txtDias = cantDias.ToString() + " dias.";

                    linombrePlan.Text = nombrePlan;
                    liCantDias.Text = txtDias;
                    hdnNombrePlanActual.Value = nombrePlan;
                    divPagarPlan.Visible = true;
                }
                else
                    divPagarPlan.Visible = false;

                //var puntoVenta99 = dbContext.PuntosDeVenta.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.Punto == 99);
                //if (!puntoVenta99)
                //    Response.Redirect("~/finRegistro.aspx");


            }


            if (PermisosModulosCommon.AlertaPlanPendiente(CurrentUser.IDUsuario, ref nombrePlan))
            {
                divPlanPendiente.Visible = true;
                linombrePlanPendiente.Text = nombrePlan;
            }
            else
                divPlanPendiente.Visible = false;

            dashBoard();
            //PopulateReport();

            if (CurrentUser.CondicionIVA.Equals("MO")) 
                divTituloVentasTotal.Visible = true;
             else 
                divTituloVentasConIVA.Visible = true;            

        }
    }

    private void PopulateReport()
    {
        //using (var dbContext = new ACHEEntities())
        //{
        //    var v = dc.SalesDatas.ToList();


        //    gvSales.DataSource = v;
        //    gvSales.DataBind();

        //    Chart1.DataSource = v;
        //    Chart1.DataBind();
        //}
    }

    private void dashBoard()
    {
        var mesDesde = DateTime.Now.GetFirstDayOfMonth();
        var mesHasta = DateTime.Now.GetLastDayOfMonth();

        var mesDesdeAnt = DateTime.Now.AddMonths(-1).GetFirstDayOfMonth();
        var mesHastaAnt = DateTime.Now.AddMonths(-1).GetLastDayOfMonth();

        var anioDesde = DateTime.Now.GetFirstDayOfYear();
        var anioHasta = DateTime.Now.GetLastDayOfYear();

      

        using (var dbContext = new ACHEEntities())
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            int usuario = CurrentUser.IDUsuario;
            //int usuario = 3153; //Fijo usuario con datos historicos para los nuevos usuarios.

            var Comprobantes = dbContext.Comprobantes.Where(x => x.IDUsuario == usuario).ToList();
            //var Comprobantes = dbContext.Comprobantes.Where(x => x.IDUsuario == usuario && x.Tipo == "PDV").ToList();
            //quitarNC(dbContext, Comprobantes);

            var Compras = dbContext.Compras.Where(x => x.IDUsuario == usuario).ToList();

            var Personas = dbContext.Personas.Where(x => x.IDUsuario == usuario).ToList();

            decimal comprasMes = (decimal)Compras.Where(x => x.IDUsuario == usuario
                && x.Fecha >= mesDesde && x.Fecha <= mesHasta)
                .Select(x => x.Total).DefaultIfEmpty(0).Sum();

            decimal facturaMes = Comprobantes.Where(x => x.IDUsuario == usuario
               && x.FechaComprobante >= mesDesde && x.FechaComprobante <= mesHasta && x.Tipo.Contains("F"))
                .Select(x => x.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

            decimal facturaMesNDC = Comprobantes.Where(x => x.IDUsuario == usuario
               && x.FechaComprobante >= mesDesde && x.FechaComprobante <= mesHasta && x.Tipo.Contains("N"))
                .Select(x => x.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

            //Ingresos
            decimal ingresosMesAnt = Comprobantes.Where(x => x.IDUsuario == usuario
                && x.FechaComprobante >= mesDesdeAnt && x.FechaComprobante <= mesHastaAnt && x.Tipo == "PDV")
                .Select(x => x.ImporteTotalBruto).DefaultIfEmpty(0).Sum();

            decimal ingresosMes = Comprobantes.Where(x => x.IDUsuario == usuario
                && x.FechaComprobante >= mesDesde && x.FechaComprobante <= mesHasta && x.Tipo == "PDV")
                .Select(x => x.ImporteTotalBruto).DefaultIfEmpty(0).Sum();

            decimal ingresosAnio = Comprobantes.Where(x => x.IDUsuario == usuario
                && x.FechaComprobante >= anioDesde && x.FechaComprobante <= anioHasta && x.Tipo == "PDV")
                .Select(x => x.ImporteTotalBruto).DefaultIfEmpty(0).Sum();

            AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

            if (afu != null)
            {
                if (afu.HomeValores)
                {
                    litIngresosMesAnterior.Text = ingresosMesAnt.ToString("N2").Replace(",00", "");
                    litIngresosMes.Text = ingresosMes.ToString("N2").Replace(",00", "");
                    litIngresosAnio.Text = ingresosAnio.ToString("N2").Replace(",00", "");

                    //Saldos
                    var fechaVenc = DateTime.Now.AddDays(-60);


                    ResultadosCuentaCorrienteViewModel cuentaCorriente = PersonasCommon.ObtenerPersonasCuentaCorriente("", "C", true, false, 1, 50, usu);

                    decimal porCobrar = 0;
                    foreach(CuentaCorrienteViewModel r in cuentaCorriente.Items)
                    {
                        porCobrar = porCobrar + decimal.Parse(r.Saldo);
                    }

                    litPorCobrar.Text = porCobrar.ToString("N2").Replace(",00", "");

                    //litPorCobrar.Text = Comprobantes.Where(x => x.IDUsuario == usuario && x.Saldo > 0)
                    //    .Select(x => x.Saldo).DefaultIfEmpty(0).Sum().ToString("N2").Replace(",00", "");


                    //.AsEnumerable().Sum(x => x.Saldo).ToString("N2");
                    litPorCobrarUrgente.Text = Comprobantes.Where(x => x.IDUsuario == usuario && x.Saldo > 0 && x.FechaVencimiento <= fechaVenc)
                        .Select(x => x.Saldo).DefaultIfEmpty(0).Sum().ToString("N2").Replace(",00", "");

                    //Compra del Mes
                    litCompraMes.Text = comprasMes.ToString("N2").Replace(",00", "");

                    //Cuenta Corriente Cliente
                    litCuentaCorrienteCliente.Text = ((decimal)Personas.Where(x => x.IDUsuario == usuario
                            && x.Tipo.Equals("C")).Select(x => x.SaldoInicial).DefaultIfEmpty(0).Sum()).ToString("N2").Replace(",00", "");

                    //Cuenta Corriente Proveedor
                    litCuentaCorrienteProveedor.Text = ((decimal)Personas.Where(x => x.IDUsuario == usuario
                            && x.Tipo.Equals("P")).Select(x => x.SaldoInicial).DefaultIfEmpty(0).Sum()).ToString("N2").Replace(",00", "");

                    //Factura del Mes
                    litFacturadoDeVentasMes.Text = (facturaMes - facturaMesNDC).ToString("N2").Replace(",00", "");

                }
                else
                {
                    litIngresosMesAnterior.Text = "";
                    litIngresosMes.Text = "";
                    litIngresosAnio.Text = "";
                    litCompraMes.Text = "";
                    litCuentaCorrienteCliente.Text = "";
                    litCuentaCorrienteProveedor.Text = "";
                    litFacturadoDeVentasMes.Text = "";

                    //Saldos
                    var fechaVenc = DateTime.Now.AddDays(-60);

                    litPorCobrar.Text = "";
                    litPorCobrarUrgente.Text = "";
                    lnkCobranzasPendientes.NavigateUrl = "";
                }
            }                
        }
    }

    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    public static ResultadosComprobantesViewModel obtenerFacturasPendientes()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //var usuario = usu.IDUsuario;
            var usuario = 3153; //Fijo usuario con datos historicos para los nuevos usuarios.

            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Compras.Where(x => x.IDUsuario == usuario && x.Saldo != 0 
                                    && x.Tipo != "NCA" && x.Tipo != "NCB" && x.Tipo != "NCC").AsQueryable();

                ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();
                resultado.TotalItems = results.Count();

                var list = results.OrderBy(x => x.Fecha).Take(5).ToList()
                    .Select(x => new ComprobantesViewModel()
                    {
                        ID = x.IDCompra,
                        Fecha = x.Fecha.ToString("dd/MM/yyyy"),
                        RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                        Numero = x.Tipo + " " + x.NroFactura,
                        ImporteTotalNeto = x.Saldo.ToString("N2")
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public static ResultadosComprobantesViewModel obtenerVentasPendientes()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //var usuario = usu.IDUsuario;
            var usuario = 3153; //Fijo usuario con datos historicos para los nuevos usuarios.

            using (var dbContext = new ACHEEntities())
            {
                var listaComprobantesPagados = new List<Comprobantes>();
                var listaCobranzas = dbContext.CobranzasDetalle
                                .Where(x => x.Comprobantes.IDUsuario == usuario && x.Comprobantes.Tipo != "COT" 
                                        && x.Comprobantes.Tipo != "NCA" && x.Comprobantes.Tipo != "NCB" && x.Comprobantes.Tipo != "NCC")
                                .OrderBy(x => x.Comprobantes.FechaVencimiento).ToList();
                foreach (var item in listaCobranzas)
                    listaComprobantesPagados.Add(item.Comprobantes);

                var listaComprobantesNoPagados = dbContext.Comprobantes
                                                .Where(x => x.IDUsuario == usuario && x.Tipo != "COT" 
                                                    && x.Tipo != "NCA" && x.Tipo != "NCB" && x.Tipo != "NCC")
                                                .OrderBy(x => x.FechaVencimiento).ToList()
                                                .Except(listaComprobantesPagados).ToList();

                //var results = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                ResultadosComprobantesViewModel resultado = new ResultadosComprobantesViewModel();
                resultado.TotalItems = listaComprobantesNoPagados.Count();

                var list = listaComprobantesNoPagados.OrderBy(x => x.FechaComprobante).Take(5).ToList()
                    .Select(x => new ComprobantesViewModel()
                    {
                        ID = x.IDComprobante,
                        RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                        Fecha = x.FechaComprobante.ToString("dd/MM/yyyy"),
                        Numero = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    /*** VENTAS VS COMPRAS ***/
    #region
    
    [WebMethod(true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static DashboardViewModel obtenerVentasVsCompras()
    {
        List<ChartYXZ> listaCompras = new List<ChartYXZ>();
        List<ChartYXZ> listaComprobantes = new List<ChartYXZ>();

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            //var usuario = usu.IDUsuario;
            var usuario = 3153; //Fijo usuario con datos historicos para los nuevos usuarios.

            DateTime fechaDesde = new DateTime();
            DateTime fechaHasta = new DateTime();

            using (var dbContext = new ACHEEntities())
            {
                fechaDesde = DateTime.Now.AddMonths(-12);
                fechaHasta = DateTime.Now.GetLastDayOfYear();

                var Comprobantes = dbContext.Comprobantes.Where(x => x.IDUsuario == usuario
                                                                && x.Tipo != "COT" 
                                                                && x.Tipo != "NCA" && x.Tipo != "NCB" && x.Tipo != "NCC"                                                                 
                                                                && x.FechaComprobante >= fechaDesde && x.FechaComprobante <= fechaHasta)
                                                         .OrderBy(x => x.FechaComprobante).ToList();


                quitarNC(dbContext, Comprobantes);
                var tipo = (Comprobantes.Any(x => x.FechaComprobante.Date < DateTime.Now.Date.AddMonths(-3))) ? "MES" : "DIA";

                AgruparComprobantes(listaComprobantes, Comprobantes, tipo);
                var Compras = dbContext.Compras.Where(x => x.IDUsuario == usuario && x.Fecha >= fechaDesde && x.Fecha <= fechaHasta).OrderBy(x => x.Fecha).ToList();
                AgruparCompras(listaCompras, Compras, tipo);
            }
        }
        DashboardViewModel d = new DashboardViewModel();
        d.Items = JoinResultados(listaCompras, listaComprobantes);
        return d;
    }

    private static void quitarNC(ACHEEntities dbContext, List<ACHE.Model.Comprobantes> Comprobantes)
    {
        //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        //var usuario = usu.IDUsuario;
        var usuario = 3153; //Fijo usuario con datos historicos para los nuevos usuarios.

        var listNC = dbContext.CobranzasFormasDePago.Where(x => x.IDNotaCredito != null && x.Cobranzas.IDUsuario == usuario).ToList()
                                                    .Select(y => y.Cobranzas.CobranzasDetalle).ToList();
        foreach (var NC in listNC)
        {
            foreach (var comprobante in NC)
                Comprobantes.RemoveAll(x => x.IDComprobante == comprobante.Comprobantes.IDComprobante);
        }
    }

    private static List<ChartYXZ> JoinResultados(List<ChartYXZ> listaCompras, List<ChartYXZ> listaComprobantes)
    {
        var aux = listaComprobantes.Union(listaCompras).ToList();

        return aux.GroupBy(x => x.Fecha).Select(x => new ChartYXZ()
        {
            Fecha = x.FirstOrDefault().Fecha,
            Uno = x.Sum(y => y.Uno),
            Dos = x.Sum(y => y.Dos)
        }).ToList();
    }
    
    private static void AgruparCompras(List<ChartYXZ> listaCompras, List<Compras> lista, string tipo)
    {
        var listafechas = new List<CharFacturacion>();
        if (tipo == "DIA")
        {
            listafechas = lista.GroupBy(x => new { x.Fecha.Month, x.Fecha.Year, x.Fecha.Day }).Select(x => new CharFacturacion()
           {
               ImporteTotal = Convert.ToDecimal(x.Sum(y => y.Total)),
               Fecha = x.Select(y => y.Fecha.ToString("yyyy-MM-dd")).FirstOrDefault()
           }).ToList();
        }
        else
        {
            listafechas = lista.GroupBy(x => new { x.Fecha.Month, x.Fecha.Year }).Select(x => new CharFacturacion()
           {
               ImporteTotal = Convert.ToDecimal(x.Sum(y => y.Total)),
               Fecha = x.Select(y => y.Fecha.ToString("yyyy-MM")).FirstOrDefault()
           }).ToList();
        }

        foreach (var item in listafechas)
        {
            ChartYXZ chart = new ChartYXZ();
            chart.Fecha = item.Fecha;
            chart.Dos = (decimal)item.ImporteTotal;
            listaCompras.Add(chart);
        }
    }
    
    private static void AgruparComprobantes(List<ChartYXZ> listaComprobantes, List<Comprobantes> lista, string tipo)
    {
        var listafechas = new List<CharFacturacion>();
        if (tipo == "DIA")
        {
            listafechas = lista.GroupBy(x => new { x.FechaComprobante.Month, x.FechaComprobante.Year, x.FechaComprobante.Day }).Select(x => new CharFacturacion()
           {
               ImporteTotal = x.Sum(y => y.ImporteTotalBruto),
               Fecha = x.Select(y => y.FechaComprobante.ToString("yyyy-MM-dd")).FirstOrDefault()
           }).ToList();
        }
        else
        {
            listafechas = lista.GroupBy(x => new { x.FechaComprobante.Month, x.FechaComprobante.Year }).Select(x => new CharFacturacion()
           {
               ImporteTotal = x.Sum(y => y.ImporteTotalBruto),
               Fecha = x.Select(y => y.FechaComprobante.ToString("yyyy-MM")).FirstOrDefault()
           }).ToList();
        }


        foreach (var item in listafechas)
        {
            ChartYXZ chart = new ChartYXZ();
            chart.Fecha = item.Fecha;
            chart.Uno = item.ImporteTotal;
            listaComprobantes.Add(chart);
        }
    }

    #endregion
}