using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.FacturaElectronica;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using ACHE.Model.ViewModels;
using Ionic.Zip;


public partial class modulos_reportes_citiVentas : BasePage
{
    private static ResultadosRicvViewModel resultado = new ResultadosRicvViewModel();
    private static List<RegInfoCVventasCBTEViewModel> listaRegInfoVentas;
    private static List<RegInfoCVComprasCBTEViewModel> listaRegInfoCompras;
    private static List<RegInfoAlicuotasViewModel> listaAlicuotaVenta;
    private static List<RegInfoAlicuotasViewModel> listaAlicuotaCompras;

    private const string Const_CodigoMoneda = "PES";
    private const string Const_TipoCambio = "0001000000";
    private static string[] tipoComprobanteNoValidos = { "PDC", "EDA", "PDV", "COT", "CDC", "DDC" };

    protected void Page_Load(object sender, EventArgs e)
    {
        using (var dbContext = new ACHEEntities())
        {
            AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

            if (afu != null)
                if (!afu.InfoImpositivosCITIComprasYVentas)
                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

        }
        txtFechaDesde.Text = DateTime.Now.GetFirstDayOfMonth().ToString("MMMMMMMMM yyyy");
    }

    [WebMethod(true)]
    public static ResultadosRicvViewModel generarExportaciones(string periodo)
    {
        try
        {
            var periodos = Convert.ToDateTime(periodo);
            var fechaDesde = periodos.GetFirstDayOfMonth();
            var fechaHasta = periodos.GetLastDayOfMonth();

            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                using (var dbContext = new ACHEEntities())
                {
                    resultado.Items = new List<RicvViewModel>();

                    listaRegInfoVentas = new List<RegInfoCVventasCBTEViewModel>();
                    listaRegInfoCompras = new List<RegInfoCVComprasCBTEViewModel>();
                    listaAlicuotaVenta = new List<RegInfoAlicuotasViewModel>();
                    listaAlicuotaCompras = new List<RegInfoAlicuotasViewModel>();

                    generarRegInfo_Cv_Ventas_Cbte(fechaDesde, fechaHasta);
                    generarRegInfo_Cv_Ventas_Alicuotas();
                    generarRegInfoCv_Compras_Cbte(fechaDesde, fechaHasta);
                    generarRegInfoCv_compras_Alicuotas();

                    generarArchivoRegInfo_Cv_Ventas_Cbte();
                    generarArchivoRegInfo_Cv_Ventas_Alicuotas();
                    generarArchivoRegInfo_Cv_Compras_Cbte();
                    generarArchivoRegInfo_Cv_Compras_Alicuotas();

                    ValidarExportacion();
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

    #region/// GENERACION DE LISTAS CON LOS DATOS PARA EXPORTAR A LA AFIP
    private static void generarRegInfo_Cv_Ventas_Cbte(DateTime fechaDesde, DateTime fechaHasta)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        using (var dbContext = new ACHEEntities())
        {
            var lista = dbContext.Comprobantes.Include("ComprobantesDetalle").Where(x => x.IDUsuario == usu.IDUsuario && x.FechaComprobante >= fechaDesde && x.FechaComprobante <= fechaHasta && !tipoComprobanteNoValidos.Contains(x.Tipo) ).ToList();

            //lista = lista.Where(x => x.Modo == "T" || (x.Modo == "E" && x.CAE != null && x.CAE != "")).ToList();
            lista = lista.Where(x => x.CAE != null && x.CAE != "").ToList();
            foreach (var item in lista)
            {
                RegInfoCVventasCBTEViewModel citiVentas = new RegInfoCVventasCBTEViewModel();

                //1
                citiVentas.FechaComprobante = item.FechaComprobante.Date.ToString("yyyyMMdd");
                //2
                #region TipoComprobante
                switch (item.Tipo)
                {
                    case "FCA":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_A).ToString("#000");
                        break;
                    case "FCB":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_B).ToString("#000");
                        break;
                    case "FCC":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_C).ToString("#000");
                        break;
                    case "NCA":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_CREDITO_A).ToString("#000");
                        break;
                    case "NCB":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_CREDITO_B).ToString("#000");
                        break;
                    case "NCC":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_CREDITO_C).ToString("#000");
                        break;
                    case "NDA":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_DEBITO_A).ToString("#000");
                        break;
                    case "NDB":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_DEBITO_B).ToString("#000");
                        break;
                    case "NDC":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_DEBITO_C).ToString("#000");
                        break;
                    case "RCA":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.RECIBO_A).ToString("#000");
                        break;
                    case "RCB":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.RECIBO_B).ToString("#000");
                        break;
                    case "RCC":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.RECIBO_C).ToString("#000");
                        break;
                    case "FCAMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_A_MiPyMEs).ToString("#000");
                        break;
                    case "FCBMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_B_MiPyMEs).ToString("#000");
                        break;
                    case "FCCMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_C_MiPyMEs).ToString("#000");
                        break;
                    case "NCAMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_CREDITO_A_MiPyMEs).ToString("#000");
                        break;
                    case "NCBMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_CREDITO_B_MiPyMEs).ToString("#000");
                        break;
                    case "NCCMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_CREDITO_C_MiPyMEs).ToString("#000");
                        break;
                    case "NDAMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_DEBITO_A_MiPyMEs).ToString("#000");
                        break;
                    case "NDBMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_DEBITO_B_MiPyMEs).ToString("#000");
                        break;
                    case "NDCMP":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_DEBITO_C_MiPyMEs).ToString("#000");
                        break;
                    case "SIN":
                    case "COT":
                        citiVentas.TipoComprobante = Convert.ToInt32(FETipoComprobante.SIN_DEFINIR).ToString("#000");
                        break;
                }
                #endregion
                //3
                citiVentas.PuntoVenta = item.PuntosDeVenta.Punto.ToString("#00000");
                //4
                citiVentas.NroComprobante = item.Numero.ToString("#00000000000000000000");
                //5
                citiVentas.NroComprobanteHasta = item.Numero.ToString("#00000000000000000000");
                //6
                citiVentas.ClienteDocTipo = item.Personas.TipoDocumento == "DNI" ? 96 : 80;
                //7
                if (item.Personas.NroDocumento == "")
                {
                    citiVentas.ClienteDocTipo = 99;
                    item.Personas.NroDocumento = "0";
                }
                citiVentas.ClienteDocNro = long.Parse(item.Personas.NroDocumento).ToString("#00000000000000000000");
                //8
                citiVentas.ClienteNombre = item.Personas.RazonSocial.RemoverAcentos().PadRight(30);
                citiVentas.ClienteNombre = (citiVentas.ClienteNombre.Length > 30) ? citiVentas.ClienteNombre.Substring(0, 30) : citiVentas.ClienteNombre;
                //9
                citiVentas.ImporteTotal = CompletarConCerosDecimales(Convert.ToDouble(item.ImporteTotalNeto), 15);
                //10
                citiVentas.ImporteNoGravado = CompletarConCerosDecimales(Convert.ToDouble(item.ImporteNoGravado), 15);
                //11
                if (item.PercepcionIVA > 0)
                    citiVentas.ImportePercepcionNoCategorizada = CompletarConCerosDecimales(Convert.ToDouble((item.PercepcionIVA * item.ImporteTotalBruto) / 100), 15);
                else
                    citiVentas.ImportePercepcionNoCategorizada = CompletarConCerosDecimales(0, 15);
                //12
                citiVentas.ImporteOperacionesExentas = CompletarConCerosDecimales(Convert.ToDouble(0), 15); // ---------------------------------------FALTAN COMPLETAR  IVA 0---------------------------------------
                //13
                citiVentas.ImportePercepcionesNacionales = CompletarConCerosDecimales(Convert.ToDouble(0), 15);// No se usa
                //14
                if (item.PercepcionIIBB > 0)
                    citiVentas.ImportePercepcionesIngresoBruto = CompletarConCerosDecimales(Convert.ToDouble((item.PercepcionIIBB * item.ImporteTotalBruto) / 100), 15);
                else
                    citiVentas.ImportePercepcionesIngresoBruto = CompletarConCerosDecimales(0,15);
                //15
                citiVentas.ImportePercepcionesMunicipales = CompletarConCerosDecimales(Convert.ToDouble(0), 15);// no se usan
                //16
                citiVentas.ImportePercepcionesInternos = CompletarConCerosDecimales(Convert.ToDouble(0), 15);// no se usan
                //17 
                citiVentas.CodigoMoneda = Const_CodigoMoneda;
                //18
                citiVentas.tipoCambio = Const_TipoCambio;
                //19
                citiVentas.CantAlicuotaIVA = item.ComprobantesDetalle.GroupBy(x => x.Iva).Count();
                //20
                citiVentas.CodigoOperacion = (item.ComprobantesDetalle.Any(x => x.Iva == 0) && citiVentas.TipoComprobante == Convert.ToInt32(FETipoComprobante.FACTURAS_A).ToString("#000")) ? CodigoOperacion.NoGravado : "0";
                //21
                citiVentas.OtrosTributos = CompletarConCerosDecimales(Convert.ToDouble(0), 15);// no se usa

                #region Seteo los datos del IVA, estos datos son nesesarios para generar el registro de IVA

                citiVentas.DetalleIva = new List<FERegistroIVA>();

                var list = item.ComprobantesDetalle.ToList();
                if (list.Any())
                {
                    foreach (var detalle in list)
                    {
                        //if (usu.CondicionIVA == "RI" && item.Personas.CondicionIva == "RI")
                        if (usu.CondicionIVA == "RI")
                        {
                            //totalizar importe e IVA
                            switch (detalle.Iva.ToString("N2"))
                            {
                                case "21,00":
                                    agregarDetalleIvaTotalizado(ref citiVentas, detalle, FETipoIva.Iva21);
                                    break;
                                case "27,00":
                                    agregarDetalleIvaTotalizado(ref citiVentas, detalle, FETipoIva.Iva27);
                                    break;
                                case "10,50":
                                    agregarDetalleIvaTotalizado(ref citiVentas, detalle, FETipoIva.Iva10_5);
                                    break;
                                case "5,00":
                                    agregarDetalleIvaTotalizado(ref citiVentas, detalle, FETipoIva.Iva5);
                                    break;
                                case "2,50":
                                    agregarDetalleIvaTotalizado(ref citiVentas, detalle, FETipoIva.Iva2_5);
                                    break;
                                case "0,00":
                                    agregarDetalleIvaTotalizado(ref citiVentas, detalle, FETipoIva.Iva0);
                                    break;
                            }
                        }
                        //else
                        //{
                        //    if (citiVentas.DetalleIva.Any(x => x.TipoIva == FETipoIva.Iva0))
                        //        citiVentas.DetalleIva.Where(x => x.TipoIva == FETipoIva.Iva0).FirstOrDefault().BaseImp += Math.Round(double.Parse(detalle.Comprobantes.ImporteTotalNeto.ToString()), 2);
                        //    else
                        //        citiVentas.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(detalle.Comprobantes.ImporteTotalBruto.ToString()), 2), TipoIva = FETipoIva.Iva21 });
                        //        //citiVentas.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(detalle.Comprobantes.ImporteTotalBruto.ToString()), 2), TipoIva = FETipoIva.Iva0 });
                        //}
                    }
                }

                #endregion
                //22
                citiVentas.FechaVencimientoPago = item.FechaVencimiento.ToString("yyyyMMdd");

                listaRegInfoVentas.Add(citiVentas);
            }
        }
    }
    private static void generarRegInfoCv_Compras_Cbte(DateTime fechaDesde, DateTime fechaHasta)
    {
        try
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.Fecha >= fechaDesde && x.Fecha <= fechaHasta && x.Tipo != "COT" && x.Tipo != "BOR" && x.NroFactura != string.Empty).ToList();

                foreach (var item in lista)
                {

                    RegInfoCVComprasCBTEViewModel citiCompras = new RegInfoCVComprasCBTEViewModel();

                    //1
                    citiCompras.FechaComprobante = item.Fecha.Date.ToString("yyyyMMdd");
                    //2
                    #region TipoComprobante
                    switch (item.Tipo)
                    {
                        case "FCA":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_A).ToString("#000");
                            break;
                        case "FCB":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_B).ToString("#000");
                            break;
                        case "FCC":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_C).ToString("#000");
                            break;
                        case "NCA":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_CREDITO_A).ToString("#000");
                            break;
                        case "NCB":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_CREDITO_B).ToString("#000");
                            break;
                        case "NCC":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_CREDITO_C).ToString("#000");
                            break;
                        case "NDA":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_DEBITO_A).ToString("#000");
                            break;
                        case "NDB":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_DEBITO_B).ToString("#000");
                            break;
                        case "NDC":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTAS_DEBITO_C).ToString("#000");
                            break;
                        case "RCA":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.RECIBO_A).ToString("#000");
                            break;
                        case "RCB":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.RECIBO_B).ToString("#000");
                            break;
                        case "RCC":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.RECIBO_C).ToString("#000");
                            break;
                        case "FCAMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_A_MiPyMEs).ToString("#000");
                            break;
                        case "FCBMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_B_MiPyMEs).ToString("#000");
                            break;
                        case "FCCMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.FACTURAS_C_MiPyMEs).ToString("#000");
                            break;
                        case "NCAMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_CREDITO_A_MiPyMEs).ToString("#000");
                            break;
                        case "NCBMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_CREDITO_B_MiPyMEs).ToString("#000");
                            break;
                        case "NCCMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_CREDITO_C_MiPyMEs).ToString("#000");
                            break;
                        case "NDAMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_DEBITO_A_MiPyMEs).ToString("#000");
                            break;
                        case "NDBMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_DEBITO_B_MiPyMEs).ToString("#000");
                            break;
                        case "NDCMP":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.NOTA_DEBITO_C_MiPyMEs).ToString("#000");
                            break;
                        case "SIN":
                        case "COT":
                            citiCompras.TipoComprobante = Convert.ToInt32(FETipoComprobante.SIN_DEFINIR).ToString("#000");
                            break;
                    }
                    #endregion
                    //3
                    citiCompras.PuntoVenta = int.Parse(item.NroFactura.Split("-")[0]).ToString("#00000");
                    //4
                    citiCompras.NroComprobante = long.Parse(item.NroFactura.Split("-")[1]).ToString("#00000000000000000000");
                    //5
                    citiCompras.DespachoImportacion = "                ";// No se hacen importaciones de productos.
                                                                         //6
                    citiCompras.CodigoDocVendedor = item.Personas.TipoDocumento == "DNI" ? 96 : 80;
                    //7
                    citiCompras.DocNroVendedor = long.Parse(item.Personas.NroDocumento == "" ? "0" : item.Personas.NroDocumento).ToString("#00000000000000000000");
                    //8
                    citiCompras.NombreVendedor = item.Personas.RazonSocial.PadRight(30);
                    citiCompras.NombreVendedor = (citiCompras.NombreVendedor.Length > 30) ? citiCompras.NombreVendedor.Substring(0, 30) : citiCompras.NombreVendedor;
                    //9
                    citiCompras.ImporteTotal = CompletarConCerosDecimales(Convert.ToDouble(item.Total + item.Iva + item.TotalImpuestos), 15);
                    //10
                    citiCompras.ImporteNoGravado = CompletarConCerosDecimales(Convert.ToDouble(item.NoGravado), 15);
                    //11
                    citiCompras.ImporteOperacionesExentas = CompletarConCerosDecimales(Convert.ToDouble(0), 15);//item.Exento), 15); // ver que en las facturas A con iva exento va en total de la operacion y no en exento.
                                                                                                                //12
                    citiCompras.ImportePercepcionImpuestoValorAgregado = CompletarConCerosDecimales(Convert.ToDouble(item.PercepcionIVA), 15);
                    //13
                    citiCompras.ImportePercepcionesNacionales = CompletarConCerosDecimales(Convert.ToDouble(item.ImpNacional), 15);
                    //14
                    citiCompras.ImportePercepcionesIngresoBruto = CompletarConCerosDecimales(Convert.ToDouble(item.IIBB), 15);
                    //15
                    citiCompras.ImportePercepcionesMunicipales = CompletarConCerosDecimales(Convert.ToDouble(item.ImpMunicipal), 15);
                    //16
                    citiCompras.ImportePercepcionesInternos = CompletarConCerosDecimales(Convert.ToDouble(item.ImpInterno), 15);
                    //17 
                    citiCompras.CodigoMoneda = Const_CodigoMoneda;
                    //18
                    citiCompras.tipoCambio = Const_TipoCambio;
                    //19
                    var contador = 0;
                    if (item.Importe2 != 0)
                        contador++;
                    if (item.Importe5 != 0)
                        contador++;
                    if (item.Importe10 != 0)
                        contador++;
                    if (item.Importe21 != 0)
                        contador++;
                    if (item.Importe27 != 0)
                        contador++;
                    if (item.Exento != 0)
                        contador++;

                    citiCompras.CantAlicuotaIVA = (contador);
                    //20
                    citiCompras.CodigoOperacion = (item.Iva == 0 && citiCompras.TipoComprobante == Convert.ToInt32(FETipoComprobante.FACTURAS_A).ToString("#000")) ? CodigoOperacion.NoGravado : "0";
                    //21
                    #region Seteo los datos del IVA, estos datos son nesesarios para generar el registro de IVA

                    citiCompras.DetalleIva = new List<FERegistroIVA>();


                    if (usu.CondicionIVA == "RI" && item.Personas.CondicionIva == "RI")
                    {
                        if (item.ImporteMon > 0 || item.Exento > 0)
                            citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse((item.ImporteMon + item.Exento).ToString()), 2), TipoIva = FETipoIva.Iva0 });
                        if (item.Importe2 > 0)
                            citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(item.Importe2.ToString()), 2), TipoIva = FETipoIva.Iva2_5 });
                        if (item.Importe5 > 0)
                            citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(item.Importe5.ToString()), 2), TipoIva = FETipoIva.Iva5 });
                        if (item.Importe10 > 0)
                            citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(item.Importe10.ToString()), 2), TipoIva = FETipoIva.Iva10_5 });
                        if (item.Importe21 > 0)
                            citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(item.Importe21.ToString()), 2), TipoIva = FETipoIva.Iva21 });
                        if (item.Importe27 > 0)
                            citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(item.Importe27.ToString()), 2), TipoIva = FETipoIva.Iva27 });

                    }
                    else
                    {
                        // solo las facturas A
                        //citiCompras.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(item.ImporteMon.ToString()), 2), TipoIva = FETipoIva.Iva0 });
                    }


                    #endregion
                    citiCompras.CreditoFiscalComputable = CompletarConCerosDecimales(Convert.ToDouble(citiCompras.DetalleIva.Sum(x => x.Importe)), 15);
                    //22
                    citiCompras.OtrosTributos = CompletarConCerosDecimales(Convert.ToDouble(item.Otros), 15);

                    #region // solo si el comprobante es de codigo 033,058,059,060,063 -(ninguno de estos comprobantesmaneja el sistema)
                    //23
                    citiCompras.CUITEmisorCorredor = "00000000000";
                    //24
                    citiCompras.DenominacionEmisorCorredor = "                              ";
                    //25
                    citiCompras.IVAComision = "000000000000000";
                    #endregion
                    listaRegInfoCompras.Add(citiCompras);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }        
    }
    private static void generarRegInfo_Cv_Ventas_Alicuotas()
    {
        RegInfoAlicuotasViewModel VentasAlicuota;

        foreach (var item in listaRegInfoVentas)
        {
            foreach (var iva in item.DetalleIva)
            {
                VentasAlicuota = new RegInfoAlicuotasViewModel();
                VentasAlicuota.TipoComprobante = item.TipoComprobante;
                VentasAlicuota.Puntoventa = item.PuntoVenta;
                VentasAlicuota.NroComprobante = item.NroComprobante;
                VentasAlicuota.alicuotaIVA = Convert.ToInt32(iva.TipoIva).ToString("#0000");

                VentasAlicuota.ImporteNetoGravado = CompletarConCerosDecimales(iva.BaseImp, 15);
                VentasAlicuota.ImpuestoLiquidado = CompletarConCerosDecimales(iva.Importe, 15);

                listaAlicuotaVenta.Add(VentasAlicuota);
            }
        }
    }
    private static void generarRegInfoCv_compras_Alicuotas()
    {
        RegInfoAlicuotasViewModel Alicuota;

        foreach (var item in listaRegInfoCompras)
        {
            if (!(item.TipoComprobante != "001" && item.TipoComprobante != "002" && item.TipoComprobante != "003"))
            {
                foreach (var iva in item.DetalleIva)
                {
                    Alicuota = new RegInfoAlicuotasViewModel();
                    Alicuota.TipoComprobante = item.TipoComprobante;
                    Alicuota.Puntoventa = item.PuntoVenta;
                    Alicuota.NroComprobante = item.NroComprobante;
                    Alicuota.alicuotaIVA = Convert.ToInt32(iva.TipoIva).ToString("#0000");
                    Alicuota.ImporteNetoGravado = CompletarConCerosDecimales(iva.BaseImp, 15);
                    Alicuota.ImpuestoLiquidado = CompletarConCerosDecimales(iva.Importe, 15);
                    Alicuota.DocNroVendedor = item.DocNroVendedor;
                    Alicuota.CodigoDocVendedor = item.CodigoDocVendedor;
                    listaAlicuotaCompras.Add(Alicuota);
                }
            }
        }
    }
    #endregion

    #region/// GENERACION DE LOS ARCHIVOS PARA DESCARGAR
    private static void generarArchivoRegInfo_Cv_Compras_Cbte()
    {
        string fileName = "REGINFO_CV_COMPRAS_CBTE";
        string path = "~/tmp/";
        var destino = HttpContext.Current.Server.MapPath(path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt");

        using (StreamWriter escritor = new StreamWriter(destino))
        {
            foreach (var item in listaRegInfoCompras)
            {
                escritor.WriteLine(item.FechaComprobante + item.TipoComprobante + item.PuntoVenta + item.NroComprobante
                    + item.DespachoImportacion + item.CodigoDocVendedor + item.DocNroVendedor + item.NombreVendedor
                    + item.ImporteTotal + item.ImporteNoGravado + item.ImporteOperacionesExentas + item.ImportePercepcionImpuestoValorAgregado
                    + item.ImportePercepcionesNacionales + item.ImportePercepcionesIngresoBruto + item.ImportePercepcionesMunicipales + item.ImportePercepcionesInternos
                    + item.CodigoMoneda + item.tipoCambio + item.CantAlicuotaIVA + item.CodigoOperacion + item.CreditoFiscalComputable + item.OtrosTributos
                    + item.CUITEmisorCorredor + item.DenominacionEmisorCorredor + item.IVAComision);
            }
        }

        RicvViewModel ricv = new RicvViewModel();
        ricv.NombreArchivo = fileName;
        ricv.Errores = "";
        ricv.URL = (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt").Replace("~", "");
        //ricv.URL = GuardarenZip(fileName, destino);
        resultado.Items.Add(ricv);
    }
    private static void generarArchivoRegInfo_Cv_Ventas_Cbte()
    {
        string fileName = "REGINFO_CV_VENTAS_CBTE";
        string path = "~/tmp/";
        var destino = HttpContext.Current.Server.MapPath(path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt");

        using (StreamWriter escritor = new StreamWriter(destino))
        {
            foreach (var item in listaRegInfoVentas)
            {
                escritor.WriteLine(item.FechaComprobante + item.TipoComprobante + item.PuntoVenta + item.NroComprobante
                    + item.NroComprobanteHasta + item.ClienteDocTipo + item.ClienteDocNro + item.ClienteNombre
                    + item.ImporteTotal + item.ImporteNoGravado + item.ImportePercepcionNoCategorizada + item.ImporteOperacionesExentas
                    + item.ImportePercepcionesNacionales + item.ImportePercepcionesIngresoBruto + item.ImportePercepcionesMunicipales + item.ImportePercepcionesInternos
                    + item.CodigoMoneda + item.tipoCambio + item.CantAlicuotaIVA + item.CodigoOperacion + item.OtrosTributos + item.FechaVencimientoPago);
            }
        }

        RicvViewModel ricv = new RicvViewModel();
        ricv.NombreArchivo = fileName;
        ricv.Errores = "";
        ricv.URL = (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt").Replace("~", "");
        //ricv.URL = GuardarenZip(fileName, destino);
        resultado.Items.Add(ricv);

    }
    private static void generarArchivoRegInfo_Cv_Compras_Alicuotas()
    {
        string fileName = "REGINFO_CV_COMPRAS_ALICUOTA";
        string path = "~/tmp/";
        var destino = HttpContext.Current.Server.MapPath(path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt");

        using (StreamWriter escritor = new StreamWriter(destino))
        {
            foreach (var item in listaAlicuotaCompras)
            {
                escritor.WriteLine(item.TipoComprobante + item.Puntoventa + item.NroComprobante
                                 + item.CodigoDocVendedor + item.DocNroVendedor
                                 + item.ImporteNetoGravado + item.alicuotaIVA + item.ImpuestoLiquidado);
            }
        }

        RicvViewModel ricv = new RicvViewModel();
        ricv.NombreArchivo = fileName;
        ricv.Errores = "";
        ricv.URL = (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt").Replace("~", "");
        //ricv.URL = GuardarenZip(fileName, destino);
        resultado.Items.Add(ricv);
    }
    private static void generarArchivoRegInfo_Cv_Ventas_Alicuotas()
    {
        string fileName = "REGINFO_CV_VENTAS_ALICUOTA";
        string path = "~/tmp/";
        var destino = HttpContext.Current.Server.MapPath(path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt");

        using (StreamWriter escritor = new StreamWriter(destino))
        {
            foreach (var item in listaAlicuotaVenta)
            {
                escritor.WriteLine(item.TipoComprobante + item.Puntoventa + item.NroComprobante
                                  + item.ImporteNetoGravado + item.alicuotaIVA + item.ImpuestoLiquidado);
            }
        }

        RicvViewModel ricv = new RicvViewModel();
        ricv.NombreArchivo = fileName;
        ricv.Errores = "";
        ricv.URL = (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".txt").Replace("~", "");
        //ricv.URL = GuardarenZip(fileName, destino);
        resultado.Items.Add(ricv);

    }
    private static string GuardarenZip(string fileName, string destino)
    {
        string path = "~/tmp/";
        ZipFile zip = new ZipFile();
        zip.AddFile(destino, "");
        destino = HttpContext.Current.Server.MapPath(path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".zip");
        zip.Save(destino);
        return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".zip").Replace("~", "");
    }
    #endregion

    #region  ////OTRAS FUNCIONES NECESARIAS////
    /// <summary>
    ///  TOTALIZA EL IVA PARA PRESENTAR UN SOLO IVA CON SU IMPORTE, EJ; SI TENEMOS MUCHOS PRODCUTOS CON EL MISMO IVA, PRESENTO UN SOLO IVA Y SU TOTAL.
    /// </summary>
    /// <param name="citiVentas"></param>
    /// <param name="detalle"></param>
    /// <param name="tipoiva"></param>
    private static void agregarDetalleIvaTotalizado(ref RegInfoCVventasCBTEViewModel citiVentas, ComprobantesDetalle detalle, FETipoIva tipoiva)
    {
        var tra = new ComprobantesDetalleViewModel();
        tra.Iva = detalle.Iva;
        tra.Bonificacion = detalle.Bonificacion;
        tra.PrecioUnitario = detalle.PrecioUnitario;
        tra.Cantidad = detalle.Cantidad;
        ComprobanteCart.Retrieve().Items.Add(tra);
        var lista = ComprobanteCart.Retrieve().Items.ToList().FirstOrDefault();

        if (citiVentas.DetalleIva.Any(x => x.TipoIva == tipoiva))
        {
            citiVentas.DetalleIva.Where(x => x.TipoIva == tipoiva).FirstOrDefault().BaseImp += Math.Round(double.Parse(lista.TotalSinIva.ToString()), 2);
        }
        else
        {
            citiVentas.DetalleIva.Add(new FERegistroIVA() { BaseImp = Math.Round(double.Parse(lista.TotalSinIva.ToString()), 2), TipoIva = tipoiva });
        }

        ComprobanteCart.Retrieve().Items.Clear();
    }
    private static string CompletarConCerosDecimales(double input, int Cantidad)
    {
        var inputs = String.Format("{0:n}", input);
        inputs = inputs.Replace(",", "");
        inputs = inputs.Replace(".", "");
        var res = string.Empty;
        for (int i = 0; i < (Cantidad - inputs.Length); i++)
            res += "0";

        return res + inputs;
    }
    public struct CodigoOperacion
    {
        public const string ExportacionesZonaFranca = "Z";
        public const string ExportacionesAlExterior = "X";
        public const string OperacionesExentas = "E";
        public const string NoGravado = "N";
        public const string OperacionesDeCanje = "C";
    }
    private static void ValidarExportacion()
    {
        //REGINFO_CV_COMPRAS_CBTE
        if (listaRegInfoCompras.Any(x => x.PuntoVenta == "00000"))
        {
            var listAux =listaRegInfoCompras.Where(x => x.PuntoVenta == "00000").ToList();

            foreach (var item in listAux)
            {
                var error = "Nro de comprobante: " + Convert.ToInt32(item.PuntoVenta).ToString("0000") + "-" + Convert.ToInt32(item.NroComprobante).ToString("00000000") + ": El punto de venta no puede ser 0000";
                resultado.Items.Where(x => x.NombreArchivo == "REGINFO_CV_COMPRAS_CBTE").FirstOrDefault().Errores = error + ". ";
            }
        }

        // ACA AGREGAR MAS VALIDACIONES
        //
        //
        //
        //........
    }
    #endregion
}