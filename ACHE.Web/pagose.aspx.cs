using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using ACHE.Model;
using System.Collections.Specialized;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using ACHE.Negocio.Contabilidad;
using ACHE.Extensions;
using System.Configuration;
using System.Data.Entity.SqlServer;

public partial class pagose : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ".";
    public const string SeparadorDeDecimales = ",";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.SuministroPagos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");


                Usuarios usu = dbContext.Usuarios.Where(w => w.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();
                if (usu != null)
                    hdnEsAgenteRetencionGanancia.Value = (bool)usu.EsAgenteRetencionGanancia ? "1" : "0";

            }

            PagosCart.Retrieve().Items.Clear();
            PagosCart.Retrieve().FormasDePago.Clear();
            PagosCart.Retrieve().Retenciones.Clear();

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            litPath.Text = "Alta";
            cargarDatosDesdeCompra();

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    int id = int.Parse(hdnID.Value);
                    lnkPrint2.Attributes.Add("onclick", "Common.imprimirArchivoDesdeIframe('');");

                    litPath.Text = "Edición";
                }
            }
        }
    }
    private void cargarDatosDesdeCompra()
    {
        var PagoParcial = Request.QueryString["Pago"];
        string resultado = "";
        if (!string.IsNullOrWhiteSpace(Request.QueryString["IDCompra"]) && !string.IsNullOrWhiteSpace(PagoParcial))
        {
            var idcompra = Convert.ToInt32(Request.QueryString["IDCompra"]);
            using (var dbContext = new ACHEEntities())
            {
                var compras = dbContext.Compras.Where(x => x.IDCompra == idcompra).FirstOrDefault();
                hdnIDPersona.Value = compras.IDPersona.ToString();

                if (PagoParcial == "100")
                    resultado = compras.Saldo.ToString();
                else if (PagoParcial == "50")
                    resultado = ((compras.Saldo * 50) / 100).ToString();
                else if (PagoParcial == "30")
                    resultado = ((compras.Saldo * 30) / 100).ToString();



                var Concepto = compras.Tipo + " " + compras.NroFactura + " (Neto: $ " + (Convert.ToDecimal(compras.Importe2 + compras.Importe5 + compras.Importe10 + compras.Importe21 + compras.Importe27)).ToString("N2") + " - Saldo:$ " + compras.Saldo.ToString() + ")";
                agregarItem(0, compras.IDCompra.ToString(), Concepto, resultado, (Convert.ToDecimal(compras.Importe2 + compras.Importe5 + compras.Importe10 + compras.Importe21 + compras.Importe27)).ToString(), compras.Saldo.ToString(),0);

                hdnCargarDatosDesdeCompra.Value = "1";
            }
        }
    }

    #region Items

    [WebMethod(true)]
    public static void agregarItem(int id, string idComprobante, string comprobante, string importe, string importeNeto, string importeCompra, int idPago)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = PagosCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
                PagosCart.Retrieve().Items.Remove(aux);
            }

            var tra = new PagosDetalleViewModel();
            tra.ID = PagosCart.Retrieve().Items.Count() + 1;
            tra.nroFactura = comprobante;
            tra.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            tra.ImporteNeto = decimal.Parse(importeNeto.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            tra.IDCompra = int.Parse(idComprobante);

            decimal totalPagos = 0;
            decimal totalImporteCompra = decimal.Parse(importeCompra.Replace("$ ","").Replace(".", "").Replace(SeparadorDeMiles, SeparadorDeDecimales));
            var list = PagosCart.Retrieve().Items.Where(w => w.nroFactura == tra.nroFactura).OrderBy(x => x.nroFactura).ToList();
            if (list.Any())
            {
                foreach (var detalle in list)
                {
                    totalPagos = totalPagos + detalle.Importe;
                }
            }
            totalPagos = totalPagos + tra.Importe;

            //if (totalPagos > totalImporteCompra)
            //    throw new Exception("Los pagos no pueden superar el importe de la compra");

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            if ((bool)usu.AgenteRetencionGanancia)
            {
                if (PagosCart.Retrieve().Items.Count > 0)
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        int idCompra1 = PagosCart.Retrieve().Items[0].IDCompra;
                        Compras c1 = dbContext.Compras.Where(w => w.IDCompra == idCompra1).FirstOrDefault();

                        Compras c2 = dbContext.Compras.Where(w => w.IDCompra == tra.IDCompra).FirstOrDefault();

                        if(c1.Rubro != c2.Rubro)
                            throw new Exception("El agente es retención de ganancias, los comprobantes deben pertenecer a un solo RUBRO.");

                    }
                }

                calcularRetencionPorGanancia(tra.IDCompra, tra.ImporteNeto, idPago);               

            }             


            PagosCart.Retrieve().Items.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public static void calcularRetencionPorGanancia(int idCompra, decimal importeNeto, int idPago)
    {
        using (var dbContext = new ACHEEntities())
        {
            decimal totalPagosAUnProveedor = 0;
            decimal parcialPagosAUnProveedor = 0;

            Compras compra = dbContext.Compras.Where(w => w.IDCompra == idCompra).FirstOrDefault();

            if (compra != null)
            {
                int AñoActual = DateTime.Now.Year;
                int MesActual = DateTime.Now.Month;
                int numeroRetencion = 1;

                string rubro = compra.Rubro;

                if (rubro.Equals("Bienes"))
                {
                    decimal montoRetencionGananciaBienes = Convert.ToDecimal(ConfigurationManager.AppSettings["MontoRetencionGananciaBienes"]);

                    var lista = PagosCart.Retrieve().Items.ToList();
                    if (lista.Any())
                    {
                        foreach (var detalle in lista)
                        {
                            parcialPagosAUnProveedor = parcialPagosAUnProveedor + detalle.ImporteNeto;
                        }
                    }
                    parcialPagosAUnProveedor = parcialPagosAUnProveedor + importeNeto;

                    List<Pagos> pa = dbContext.Pagos
                                            .Where(w => w.IDPersona == compra.IDPersona &&
                                                    w.FechaPago.Year == AñoActual &&
                                                    w.FechaPago.Month == MesActual &&
                                                    w.IDPago != idPago).ToList();

                    if (pa != null)
                        totalPagosAUnProveedor = pa.Sum(s => s.ImporteNeto);

                    if (totalPagosAUnProveedor > montoRetencionGananciaBienes)
                    {
                        decimal retencionFinal = (parcialPagosAUnProveedor * (decimal)0.02);

                        PagosRetenciones ret = (from p in dbContext.Pagos
                                                join pr in dbContext.PagosRetenciones on p.IDPago equals pr.IDPago
                                                where p.IDPersona == compra.IDPersona && p.FechaPago.Year == AñoActual
                                                select pr)
                                                .OrderByDescending(o => o.IDPagoRetenciones).FirstOrDefault();

                        if (ret != null)
                            numeroRetencion = int.Parse(ret.NroReferencia) + 1;

                        PagosCart.Retrieve().Retenciones.Clear();
                        agregarRetencion(1, "Ganancias", numeroRetencion.ToString(), retencionFinal.ToString());
                    }
                    else
                    {
                        if ((totalPagosAUnProveedor + parcialPagosAUnProveedor) > montoRetencionGananciaBienes)
                        {
                            decimal pago = (totalPagosAUnProveedor + parcialPagosAUnProveedor) - montoRetencionGananciaBienes;

                            decimal retencionFinal = (pago * (decimal)0.02);

                            PagosRetenciones ret = (from p in dbContext.Pagos
                                                    join pr in dbContext.PagosRetenciones on p.IDPago equals pr.IDPago
                                                    where p.IDPersona == compra.IDPersona && p.FechaPago.Year == AñoActual
                                                    select pr)
                                                    .OrderByDescending(o => o.IDPagoRetenciones).FirstOrDefault();

                            if (ret != null)
                                numeroRetencion = int.Parse(ret.NroReferencia) + 1;

                            PagosCart.Retrieve().Retenciones.Clear();
                            agregarRetencion(1, "Ganancias", numeroRetencion.ToString(), retencionFinal.ToString());
                        }
                    }
                }
                else // Locaciones y Servicios
                {
                    decimal montoRetencionGananciaLocacionesYServicios = Convert.ToDecimal(ConfigurationManager.AppSettings["MontoRetencionGananciaLocacionesYServicios"]);

                    var lista = PagosCart.Retrieve().Items.ToList();
                    if (lista.Any())
                    {
                        foreach (var detalle in lista)
                        {
                            parcialPagosAUnProveedor = parcialPagosAUnProveedor + detalle.ImporteNeto;
                        }
                    }
                    parcialPagosAUnProveedor = parcialPagosAUnProveedor + importeNeto;

                    List<Pagos> pa = dbContext.Pagos
                                            .Where(w => w.IDPersona == compra.IDPersona &&
                                                    w.FechaPago.Year == AñoActual &&
                                                    w.IDPago != idPago).ToList();

                    if (pa != null)
                        totalPagosAUnProveedor = pa.Sum(s => s.ImporteNeto);

                    if (totalPagosAUnProveedor > montoRetencionGananciaLocacionesYServicios)
                    {
                        decimal retencionFinal = (parcialPagosAUnProveedor * (decimal)0.02);

                        PagosRetenciones ret = (from p in dbContext.Pagos
                                                join pr in dbContext.PagosRetenciones on p.IDPago equals pr.IDPago
                                                where p.IDPersona == compra.IDPersona && p.FechaPago.Year == AñoActual
                                                select pr)
                                                .OrderByDescending(o => o.IDPagoRetenciones).FirstOrDefault();

                        if (ret != null)
                            numeroRetencion = int.Parse(ret.NroReferencia) + 1;

                        PagosCart.Retrieve().Retenciones.Clear();
                        agregarRetencion(1, "Ganancias", numeroRetencion.ToString(), retencionFinal.ToString());
                    }
                    else
                    {
                        if ((totalPagosAUnProveedor + parcialPagosAUnProveedor) > montoRetencionGananciaLocacionesYServicios)
                        {
                            decimal pago = (totalPagosAUnProveedor + parcialPagosAUnProveedor) - montoRetencionGananciaLocacionesYServicios;

                            decimal retencionFinal = (pago * (decimal)0.02);

                            PagosRetenciones ret = (from p in dbContext.Pagos
                                                    join pr in dbContext.PagosRetenciones on p.IDPago equals pr.IDPago
                                                    where p.IDPersona == compra.IDPersona && p.FechaPago.Year == AñoActual
                                                    select pr)
                                                    .OrderByDescending(o => o.IDPagoRetenciones).FirstOrDefault();

                            if (ret != null)
                                numeroRetencion = int.Parse(ret.NroReferencia) + 1;

                            PagosCart.Retrieve().Retenciones.Clear();
                            agregarRetencion(1, "Ganancias", numeroRetencion.ToString(), retencionFinal.ToString());
                        }
                    }
                }
            }

        }
    }


    [WebMethod(true)]
    public static void eliminarItem(int id, int idPago)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = PagosCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
            {
                calcularRetencionPorGanancia(aux.IDCompra, aux.ImporteNeto * -1, idPago);

                PagosCart.Retrieve().Items.Remove(aux);

            }
            
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    public static TotalesViewModel obtenerTotales()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            TotalesViewModel totales = new TotalesViewModel();
            totales.Total = PagosCart.Retrieve().GetTotal().ToString("N2");

            return totales;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    public static string obtenerItems()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                var list = PagosCart.Retrieve().Items.OrderBy(x => x.nroFactura).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td style='text-align:left'>" + detalle.nroFactura + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Importe.ToString("N2") + "</td>";

                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:eliminarItem(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarItem(" + detalle.ID + ", '" + detalle.IDCompra + "' ,'";
                        html += detalle.Importe.ToString("").Replace(".", ",") + "')\"";
                        html += "><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    #endregion
    #region Formas de Pago

    [WebMethod(true)]
    public static void agregarForma(int id, string forma, string nroRef, string importe, string idcheque, string idBanco)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = PagosCart.Retrieve().FormasDePago.Where(x => x.ID == id).FirstOrDefault();
                PagosCart.Retrieve().FormasDePago.Remove(aux);
            }

            var tra = new PagosFormasDePagoViewModel();
            tra.ID = PagosCart.Retrieve().FormasDePago.Count() + 1;
            tra.FormaDePago = forma;
            tra.NroReferencia = nroRef;
            tra.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales));

            if (!string.IsNullOrWhiteSpace(idcheque))
                tra.IDCheque = int.Parse(idcheque);

            if (!string.IsNullOrWhiteSpace(idBanco))
                tra.IDBanco = int.Parse(idBanco);

            PagosCart.Retrieve().FormasDePago.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarForma(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = PagosCart.Retrieve().FormasDePago.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
                PagosCart.Retrieve().FormasDePago.Remove(aux);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    public static string obtenerFormas()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = PagosCart.Retrieve().FormasDePago.OrderBy(x => x.FormaDePago).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td style='text-align:left'>";
                        if(detalle.FormaDePago.Equals("Cheque Tercero"))
                        {
                            html += "Cheque Propio" + "</td>";
                        }
                        else
                        {
                            if (detalle.FormaDePago.Equals("Cheque Propio"))
                            {
                                html += "Cheque Tercero" + "</td>";
                            }
                            else
                            {
                                html += detalle.FormaDePago + "</td>"; 
                            }
                        }
                        html += "<td style='text-align:left'>" + detalle.NroReferencia + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Importe.ToString("N2") + "</td>";
                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:eliminarForma(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarForma(" + detalle.ID + ", '" + detalle.FormaDePago + "', '" + detalle.NroReferencia + "','" + detalle.Importe.ToString("").Replace(".", ",") + "','" + detalle.IDBanco + "','" + detalle.IDCheque + "');\"><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='5' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    public static string obtenerFormasTotal()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var importeFormas = PagosCart.Retrieve().FormasDePago.ToList().Sum(x => x.Importe);
            var importeComprobantes = PagosCart.Retrieve().Items.ToList().Sum(x => x.Importe);

            return Math.Round((importeComprobantes - importeFormas), 2).ToString("N2");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    #endregion

    #region Retenciones

    [WebMethod(true)]
    public static void agregarRetencion(int id, string tipo, string nroRef, string importe)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = PagosCart.Retrieve().Retenciones.Where(x => x.ID == id).FirstOrDefault();
                PagosCart.Retrieve().Retenciones.Remove(aux);
            }

            var tra = new PagosRetencionesViewModel();
            tra.ID = PagosCart.Retrieve().Retenciones.Count() + 1;
            tra.Tipo = tipo;
            tra.NroReferencia = nroRef;
            tra.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales));

            PagosCart.Retrieve().Retenciones.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarRetencion(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = PagosCart.Retrieve().Retenciones.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
                PagosCart.Retrieve().Retenciones.Remove(aux);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    public static string obtenerRetenciones()
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = PagosCart.Retrieve().Retenciones.OrderBy(x => x.Tipo).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";
                        html += "<td style='text-align:left'>" + detalle.Tipo + "</td>";
                        html += "<td style='text-align:left'>" + detalle.NroReferencia + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Importe.ToString("N2") + "</td>";
                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:CobRetenciones.eliminarRet(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:CobRetenciones.modificarRet(" + detalle.ID + ", '" + detalle.Tipo + "', '" + detalle.NroReferencia + "','" + detalle.Importe.ToString("").Replace(".", ",") + "');\"><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='5' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    #endregion

    [WebMethod(true)]
    public static int guardar(int id, int idPersona, string obs, string fechaPago)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                if (ContabilidadCommon.ValidarCierreContable(usu, Convert.ToDateTime(fechaPago)))
                    throw new Exception("No puede agregar ni modificar una cobranza que se encuentre en un periodo cerrado.");

                PagosCartDto pagosCartdto = new PagosCartDto();
                pagosCartdto.IDPago = id;
                pagosCartdto.IDPersona = idPersona;
                pagosCartdto.Observaciones = obs;
                pagosCartdto.FechaPago = fechaPago;

                pagosCartdto.Items = PagosCart.Retrieve().Items.ToList();
                pagosCartdto.FormasDePago = PagosCart.Retrieve().FormasDePago.ToList();
                pagosCartdto.Retenciones = PagosCart.Retrieve().Retenciones.ToList();

                var pagos = PagosCommon.Guardar(pagosCartdto, usu);
                if (PermisosModulos.tienePlan("alertas.aspx"))
                    generarAlertas(pagos);

                ContabilidadCommon.AgregarAsientoDePago(usu, pagos.IDPago);
                return pagos.IDPago;
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [WebMethod(true)]
    public static PagosEditViewModel obtenerDatos(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Pagos entity = dbContext.Pagos
                    .Include("PagosFormasDePago").Include("PagosRetenciones")
                    .Where(x => x.IDPago == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    PagosEditViewModel result = new PagosEditViewModel();
                    result.ID = id;
                    result.IDPersona = entity.IDPersona;
                    result.Observaciones = entity.Observaciones;
                    result.Fecha = entity.FechaPago.ToShortDateString();

                    var personas = dbContext.Personas.Where(x => x.IDPersona == entity.IDPersona).FirstOrDefault();
                    PersonasEditViewModel PersonasEdit = new PersonasEditViewModel();
                    PersonasEdit.ID = id;
                    PersonasEdit.RazonSocial = personas.RazonSocial.ToUpper();
                    PersonasEdit.Email = personas.Email.ToLower();
                    PersonasEdit.CondicionIva = personas.CondicionIva;
                    PersonasEdit.Domicilio = personas.Domicilio.ToUpper() + " " + personas.PisoDepto;
                    PersonasEdit.Ciudad = personas.Ciudades.Nombre.ToUpper();
                    PersonasEdit.Provincia = personas.Provincias.Nombre;
                    PersonasEdit.TipoDoc = personas.TipoDocumento;
                    PersonasEdit.NroDoc = personas.NroDocumento;
                    result.Personas = PersonasEdit;
                    PersonasEdit.esAgenteRetencion = entity.Usuarios.EsAgenteRetencion;
                    foreach (var det in entity.PagosDetalle)
                    {
                        var tra = new PagosDetalleViewModel();
                        tra.ID = PagosCart.Retrieve().Items.Count() + 1;                        
                        tra.nroFactura = det.Compras.Tipo + " " + det.Compras.NroFactura + " (Neto : $ " + det.ImporteNeto.ToString("N2") + " - Total: $ " + det.Importe.ToString("N2") + ")";
                        tra.IDCompra = det.IDCompra;
                        tra.Importe = det.Importe;
                        tra.ImporteNeto = det.ImporteNeto;
                        tra.IDCompra = det.IDCompra;
                        PagosCart.Retrieve().Items.Add(tra);
                    }

                    foreach (var det in entity.PagosFormasDePago)
                    {
                        var tra = new PagosFormasDePagoViewModel();
                        tra.ID = PagosCart.Retrieve().FormasDePago.Count() + 1;
                        tra.FormaDePago = det.FormaDePago;
                        tra.NroReferencia = det.NroReferencia;
                        tra.Importe = det.Importe;
                        tra.IDCheque = det.IDCheque;
                        tra.IDBanco = det.IDBanco;

                        PagosCart.Retrieve().FormasDePago.Add(tra);
                    }

                    foreach (var det in entity.PagosRetenciones)
                    {
                        var tra = new PagosRetencionesViewModel();
                        tra.ID = PagosCart.Retrieve().Retenciones.Count() + 1;
                        tra.Tipo = det.Tipo;
                        tra.NroReferencia = det.NroReferencia;
                        tra.Importe = det.Importe;

                        PagosCart.Retrieve().Retenciones.Add(tra);
                    }

                    return result;
                }
                else
                    throw new Exception("Error al obtener los datos");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerComprasPendientes(int id, int idPago)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (idPago == 0)
                {
                    return dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == id && x.Saldo > 0).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDCompra,
                        Nombre = x.Tipo + " " + x.NroFactura + " (Neto: $ " + (Convert.ToDecimal(x.Importe2 + x.Importe5 + x.Importe10 + x.Importe21 + x.Importe27)).ToString("N2") + " - Saldo: $ " + (Convert.ToDecimal(x.Saldo)).ToString("N2") + ")"
                    }).OrderBy(x => x.Nombre).ToList();
                }
                else
                {
                    return dbContext.PagosDetalle.Where(x => x.Compras.IDUsuario == usu.IDUsuario && x.Compras.IDPersona == id && x.IDPago == idPago).ToList()
                   .Select(x => new Combo2ViewModel()
                   {
                       ID = x.Compras.IDCompra,
                       Nombre = x.Compras.Tipo + " " + x.Compras.NroFactura + " (Neto: $ " + (Convert.ToDecimal(x.Compras.Importe2 + x.Compras.Importe5 + x.Compras.Importe10 + x.Compras.Importe21 + x.Compras.Importe27)).ToString("N2") + " - Saldo: $ " + (Convert.ToDecimal(x.Compras.Saldo)).ToString("N2") + ")"
                   }).ToList();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static void generarAlertas(Pagos pagos)
    {
        using (var dbContext = new ACHEEntities())
        {
            var listaAlertas = dbContext.Alertas.Where(x => x.IDUsuario == pagos.IDUsuario).ToList();

            foreach (var alertas in listaAlertas)
            {
                if (alertas.AvisoAlerta == "El pago a un proveedor es")
                {
                    switch (alertas.Condicion)
                    {
                        case "Mayor o igual que":
                            if (pagos.ImporteTotal >= alertas.Importe)
                            {
                                insertarAlerta(dbContext, pagos, alertas);
                            }
                            break;
                        case "Menor o igual que":
                            if (pagos.ImporteTotal <= alertas.Importe)
                            {
                                insertarAlerta(dbContext, pagos, alertas);
                            }
                            break;
                    }
                }
            }
        }
    }

    private static void insertarAlerta(ACHEEntities dbContext, Pagos pagos, Alertas alertas)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        AlertasGeneradas entity = new AlertasGeneradas();

        entity.IDAlerta = alertas.IDAlerta;
        entity.IDUsuario = pagos.IDUsuario;
        entity.IDPersona = pagos.IDPersona;

        entity.ImportePagado = pagos.ImporteTotal;
        entity.Visible = true;
        entity.Fecha = Convert.ToDateTime(DateTime.Now.Date.ToShortDateString());
        entity.IDPagos = pagos.IDPago;
        entity.NroComprobante = "Se Pagaron los Comprobante/s: ";


        foreach (var item in pagos.PagosDetalle)
        {
            var compra = dbContext.Compras.Where(x => x.IDCompra == item.IDCompra).FirstOrDefault();
            entity.NroComprobante += compra.Tipo + " " + compra.NroFactura + " ; ";
        }

        entity.NroComprobante = entity.NroComprobante.Substring(0, entity.NroComprobante.Length - 3);
        entity.NroComprobante += ".";

        dbContext.AlertasGeneradas.Add(entity);
        dbContext.SaveChanges();

        var alerta = alertas.AvisoAlerta + " - " + alertas.Condicion + " - $" + alertas.Importe;
        var descripcion = entity.NroComprobante;
        enviarEmailAlerta(alerta, descripcion);
    }

    public static void enviarEmailAlerta(string alerta, string descripcion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            var email = (usu.EmailAlerta == "") ? usu.Email : usu.EmailAlerta;

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<ALERTA>", alerta);
            replacements.Add("<USUARIO>", usu.RazonSocial);
            replacements.Add("<DESCRIPCION>", descripcion);
            replacements.Add("<EMAIL>", email);


            bool send = EmailHelper.SendMessage(EmailTemplate.Alertas, replacements, usu.Email, "");

            if (!send)
                throw new Exception("Comprobante El mensaje no pudo ser enviado. Por favor, escribenos a <a href='mailto:ayuda@axanweb.com'>ayuda@axanweb.com</a>");

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string imprimir(int id, int idPersona, string tipo, string fecha, string obs)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {

            if (PagosCart.Retrieve().Items.Count == 0)
                throw new Exception("Ingrese un item");

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string numero = id.ToString();
            if(id != 0)
            {
                Common.CrearPago(usu, id, idPersona, tipo, ref numero, fecha, obs, Common.ComprobanteModo.Generar);
                var razonSocial = string.Empty;
                using (var dbContext = new ACHEEntities())
                    razonSocial = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault().RazonSocial;
                return usu.IDUsuario.ToString() + "/" + numero + ".pdf";
            }
            else
            {
                Common.CrearPago(usu, id, idPersona, tipo, ref numero, fecha, obs, Common.ComprobanteModo.Previsualizar);
                return usu.IDUsuario.ToString() + "/" + numero + "_prev.pdf";
            }           
          
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static string imprimirRetencionGanancia(int id, int idPersona, string tipo, string fecha, string obs)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string numero = id.ToString();
            if (id != 0)
            {
                Common.CrearRetencionGanancia(usu, id, idPersona, tipo, ref numero, fecha, obs, Common.ComprobanteModo.Generar);
                return usu.IDUsuario.ToString() + "/" + numero + ".pdf";
            }
            else
                throw new Exception("El pago debe estar generado");

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



}