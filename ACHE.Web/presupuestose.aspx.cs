using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using ACHE.Negocio.Productos;
using ACHE.Model.Negocio;
using ACHE.Negocio.Presupuesto;
using ACHE.FacturaElectronica;
using ACHE.Negocio.Common;
using System.Collections.Generic;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Contabilidad;
using System.Collections.Specialized;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;

public partial class presupuestose : BasePage
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
                    if (!afu.ComercialPresupuestos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            hdnIDUsuario.Value = CurrentUser.IDUsuario.ToString();

            if (CurrentUser.ParaPDVSolicitarCompletarContacto)
                hdnParaPDVSolicitarCompletarContacto.Value = "1";
            else
                hdnParaPDVSolicitarCompletarContacto.Value = "0";

            litPath.Text = "Alta";
            ComprobanteCart.Retrieve().Items.Clear();

            txtFechaValidez.Text = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");

            if (CurrentUser.UsaPrecioFinalConIVA)
                liPrecioUnitario.Text = "<span id='spPrecioUnitario'> Precio Unit. con IVA</span> ";
            else
                liPrecioUnitario.Text = "<span id='spPrecioUnitario'> Precio Unit. sin IVA</span> ";

            hdnUsaPrecioConIVA.Value = (CurrentUser.UsaPrecioFinalConIVA) ? "1" : "0";
            hdnUsaCantidadConDecimales.Value = (Convert.ToBoolean(CurrentUser.UsaCantidadConDecimales)) ? "1" : "0";

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";
                }
            }
        }
    }

    [WebMethod(true)]
    public static int vincularPresupuesto(int id)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        int numeroComprobante = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];


            Presupuestos pres = dbContext.Presupuestos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPresupuesto == id).FirstOrDefault();
            if (pres != null)
            {
                if (!pres.Estado.Equals("A"))
                {
                    pres.Estado = "A";
                    dbContext.SaveChanges();
                }
                    //throw new Exception("El presupuesto debe estar en estado Aprobado para copiar a Pedido.");

                List<PresupuestoDetalle> ListPresDet = dbContext.PresupuestoDetalle.Where(x => x.IDPresupuesto == id).ToList();

                foreach (PresupuestoDetalle pd in ListPresDet)
                {
                    Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == pd.IDConcepto).FirstOrDefault();
                    if (con != null)
                    {
                        var tra = new ComprobantesDetalleViewModel();
                        tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                        tra.Concepto = pd.Concepto;
                        tra.Codigo = con.Codigo;
                        tra.Iva = pd.Iva;
                        tra.IdTipoIva = (int)pd.IdTipoIVA;
                        tra.Bonificacion = pd.Bonificacion;
                        tra.PrecioUnitario = pd.PrecioUnitario;
                        tra.Cantidad = pd.Cantidad;
                        tra.IDConcepto = pd.IDConcepto;
                        ComprobanteCart.Retrieve().Items.Add(tra);
                    }
                }

                int nro = 0;
                int idJurisdiccion = 0;

                int idPuntoDeVenta = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == usu.IDUsuario).Select(x => x.IDPuntoVenta).FirstOrDefault();

                //nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("PDV", usu.IDUsuario, idPuntoDeVenta));
                numeroComprobante = guardarComprobante(0, pres.IDPersona, "PDV", "T", DateTime.Now.ToShortDateString(),
                        "Efectivo", 3, DateTime.Now.AddDays(30).ToShortDateString(),
                        idPuntoDeVenta, nro.ToString(), pres.Observaciones, idJurisdiccion, 
                        pres.Nombre, pres.Nombre);

                Comprobantes CompVinculado = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == numeroComprobante).FirstOrDefault();
                if (CompVinculado != null)
                {
                    CompVinculado.IdPresupuestoVinculado = id;
                    dbContext.SaveChanges();
                }

            }

            return numeroComprobante;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int guardarComprobante(int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
        int tipoConcepto, string fechaVencimiento, int idPuntoVenta, string nroComprobante, 
        string obs, int idJurisdiccion, string nombre, string vendedor)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            if (ContabilidadCommon.ValidarCierreContable(usu, Convert.ToDateTime(fecha)))
                throw new CustomException("No puede agregar ni modificar una comprobante que se encuentre en un periodo cerrado.");
            if (idJurisdiccion == 0 && ComprobanteCart.Retrieve().PercepcionIIBB > 0)
                throw new CustomException("Si informa el importe de IIBB debe informar la jurisdicción.");
            if (idJurisdiccion > 0 && ComprobanteCart.Retrieve().PercepcionIIBB == 0)
                throw new CustomException("Si informa la jurisdicción debe informar el IIBB.");


            using (var dbContext = new ACHEEntities())
            {
                ComprobanteCartDto compCart = new ComprobanteCartDto();
                compCart.IDComprobante = id;
                compCart.IDPersona = idPersona;
                compCart.TipoComprobante = tipo;
                compCart.TipoConcepto = tipoConcepto.ToString();
                compCart.IDUsuario = usu.IDUsuario;
                compCart.Modo = modo;
                compCart.FechaComprobante = Convert.ToDateTime(fecha);
                compCart.FechaVencimiento = Convert.ToDateTime(fechaVencimiento);
                compCart.IDPuntoVenta = idPuntoVenta;
                compCart.Numero = nroComprobante;
                compCart.Observaciones = obs;
                compCart.CondicionVenta = condicionVenta;
                compCart.IDJuresdiccion = idJurisdiccion;
                compCart.Vendedor = vendedor;
                compCart.IDActividad = 0;

                compCart.Items = new List<ComprobantesDetalleViewModel>();
                compCart.ImporteNoGravado = ComprobanteCart.Retrieve().GetImporteNoGravado();
                compCart.ImporteExento = ComprobanteCart.Retrieve().GetImporteExento();
                compCart.PercepcionIVA = ComprobanteCart.Retrieve().PercepcionIVA;
                compCart.PercepcionIIBB = ComprobanteCart.Retrieve().PercepcionIIBB;
                compCart.Items = ComprobanteCart.Retrieve().Items;

                if (string.IsNullOrEmpty(nombre))
                {
                    string nombreComprobante = "";
                    int contadorConceptos = 2;
                    int codigoPersona = Convert.ToInt32(idPersona);
                    Personas p = dbContext.Personas.Where(x => x.IDPersona == codigoPersona).FirstOrDefault();
                    if (p != null)
                    {
                        nombreComprobante = p.RazonSocial.Substring(0, (p.RazonSocial.Length > 9 ? 10 : p.RazonSocial.Length)).ToUpper();
                    }

                    foreach (var det in compCart.Items)
                    {
                        nombreComprobante = nombreComprobante + " / " + det.Concepto.Substring(0, (det.Concepto.Length > 9 ? 10 : det.Concepto.Length)).ToUpper();
                        contadorConceptos--;
                        if (contadorConceptos == 0)
                            break;
                    }

                    compCart.Nombre = nombreComprobante;
                }
                else
                    compCart.Nombre = nombre.Trim().ToUpper();

                compCart.Vendedor = vendedor.Trim().ToUpper();

                var entity = ComprobantesCommon.Guardar(dbContext, compCart);

                if (entity.Modo != "E")
                    AlertaStock(dbContext, entity.IDComprobante);
                return entity.IDComprobante;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static void AlertaStock(ACHEEntities dbContext, int idComprobante)
    {
        var detalle = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == idComprobante).ToList();
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        var avisosYalertas = dbContext.AvisosVencimiento.Where(x => x.IDUsuario == usu.IDUsuario && x.TipoAlerta == "Stock" && x.Activa == true).FirstOrDefault();
        if (avisosYalertas != null)
        {
            if (PermisosModulos.tienePlan("alertas.aspx"))
            {
                foreach (var item in detalle)
                {
                    var conceptos = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                    if (conceptos != null)
                    {
                        if (item.Conceptos != null && item.Conceptos.Stock <= conceptos.StockMinimo)
                        {
                            var menssaje = "El Producto: " + item.Conceptos.Nombre + ", Código : " + item.Conceptos.Codigo + ", llegó a su stock mínimo:" + item.Conceptos.StockMinimo + ". Su stock actual es: " + item.Conceptos.Stock;
                            EnviarEmailAlertaStock("Alerta de stock mínimo", menssaje);
                        }
                    }
                }
            }
        }
    }

    private static void EnviarEmailAlertaStock(string alerta, string descripcion)
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

            bool send = EmailHelper.SendMessage(EmailTemplate.Alertas, replacements, usu.Email, "Alerta de stock minimo");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Presupuestos.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPresupuesto == id).FirstOrDefault();
            if (entity != null)
            {
                hdnIDPersona.Value = entity.IDPersona.ToString();

                txtFechaValidez.Text = entity.FechaValidez.ToString("dd/MM/yyyy");
                ddlEstado.SelectedValue = entity.Estado;
                txtNombre.Text = entity.Nombre.ToUpper();
                txtNumero.Text = entity.Numero.ToString("#00000000");
                ddlFormaPago.SelectedValue = entity.FormaDePago.ToString();
                txtObservaciones.Text = entity.Observaciones;
                txtVendedor.Text = entity.Vendedor;

                foreach (var det in entity.PresupuestoDetalle)
                {
                    var tra = new ComprobantesDetalleViewModel();
                    tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                    tra.Codigo = (det.Conceptos != null) ? det.Conceptos.Codigo : "";
                    tra.Concepto = det.Concepto;
                    tra.Iva = det.Iva;
                    tra.IdTipoIva = (int)det.IdTipoIVA;
                    tra.PrecioUnitario = det.PrecioUnitario;
                    tra.Bonificacion = det.Bonificacion;
                    tra.Cantidad = det.Cantidad;
                    tra.IDConcepto = det.IDConcepto;

                    ComprobanteCart.Retrieve().Items.Add(tra);
                }

                var comp = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IdPresupuestoVinculado == id).FirstOrDefault();
                if (comp != null)
                    this.hdnPresupuestoVinculado.Value = "1";
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, int idPersona, string fecha, string nombre, int numero, string condicionesPago, string obs, string estado, string vendedor)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            PresupuestoCartDto comprobanteCart = new PresupuestoCartDto();
            comprobanteCart.Items = ComprobanteCart.Retrieve().Items.ToList();

            comprobanteCart.IDPresupuesto = id;
            comprobanteCart.IDPersona = idPersona;
            comprobanteCart.Fecha = fecha;
            comprobanteCart.Nombre = nombre;
            comprobanteCart.Numero = numero;
            comprobanteCart.CondicionesPago = condicionesPago;
            comprobanteCart.Observaciones = obs;
            comprobanteCart.Estado = estado;
            comprobanteCart.Vendedor = vendedor.Trim().ToUpper();

            return PresupuestosCommon.GuardarPresupuesto(comprobanteCart, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    /*** ITEM ***/
    [WebMethod(true)]
    public static void agregarItem(int id, string idConcepto, string concepto, string iva, string precio, string bonif, string cantidad, int idPersona, string codigo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = ComprobanteCart.Retrieve().Items.Where(x => x.ID == id && x.Concepto == concepto).FirstOrDefault();
                ComprobanteCart.Retrieve().Items.Remove(aux);
            }

            var tra = new ComprobantesDetalleViewModel();
            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
            tra.Codigo = codigo;
            tra.Concepto = concepto;
            if (iva != string.Empty)
            {
                //tra.Iva = decimal.Parse(iva.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                tra.Iva = obtenerValorIVA(int.Parse(iva));
                tra.IdTipoIva = int.Parse(iva);
            }
            else
            {
                tra.Iva = 0;
                tra.IdTipoIva = 0;
            }

            if (bonif != string.Empty)
                tra.Bonificacion = decimal.Parse(bonif.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            else
                tra.Bonificacion = 0;

            tra.PrecioUnitario = decimal.Parse(precio.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            tra.Cantidad = decimal.Parse(cantidad.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            if (idConcepto != string.Empty)
                tra.IDConcepto = int.Parse(idConcepto);
            else
                tra.IDConcepto = null;

            tra.PrecioUnitario = ObtenerPrecioFinal(tra.PrecioUnitario, iva, idPersona);

            ComprobanteCart.Retrieve().Items.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static decimal ObtenerPrecioFinal(decimal PrecioUnitario, string iva, int idPersona)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            //var UsaPrecioConIva = dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && (x.CondicionIva == "MO" || x.CondicionIva == "CF"));
            //if (usu.UsaPrecioFinalConIVA || UsaPrecioConIva)
            if (usu.UsaPrecioFinalConIVA)
                return ConceptosCommon.ObtenerPrecioFinal(PrecioUnitario, iva);
            else
                return PrecioUnitario;
        }
    }

    [WebMethod(true)]
    public static void eliminarItem(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var aux = ComprobanteCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
            if (aux != null)
                ComprobanteCart.Retrieve().Items.Remove(aux);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static TotalesViewModel obtenerTotales()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            TotalesViewModel totales = new TotalesViewModel();
            totales.Iva = ComprobanteCart.Retrieve().GetIva().ToString("N2");
            totales.Subtotal = ComprobanteCart.Retrieve().GetSubTotal().ToString("N2");
            totales.Total = ComprobanteCart.Retrieve().GetTotal().ToString("N2");

            return totales;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerItems(int idPersona)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";

                        //if (!usu.CUIT.Equals("30654870868") && !usu.CUIT.Equals("20147963271") && !usu.CUIT.Equals("30716892286")) //FIOL
                        //{
                        //    html += "<td style='text-align:left'>" + Convert.ToInt32(detalle.Cantidad).ToString() + "</td>";
                        //}
                        //else
                        //{
                        //    html += "<td style='text-align:left'>" + detalle.Cantidad.ToString("N2") + "</td>";
                        //}

                        if (usu.UsaCantidadConDecimales)
                        {
                            html += "<td style='text-align:left'>" + detalle.Cantidad.ToString("N2") + "</td>";                            
                        }
                        else
                        {
                            html += "<td style='text-align:left'>" + Convert.ToInt32(detalle.Cantidad).ToString() + "</td>";
                        }

                        html += "<td style='text-align:left'>" + detalle.Codigo + "</td>";
                        html += "<td style='text-align:left'>" + detalle.Concepto + "</td>";
                        html += "<td style='text-align:right'>" + detalle.PrecioUnitario.ToString("N2") + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Bonificacion.ToString("N2") + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Iva.ToString("N2") + "</td>";
                        html += "<td style='text-align:right' hidden='hidden'>" + detalle.IdTipoIva.ToString() + "</td>";
                        html += "<td style='text-align:right'>" + detalle.TotalConIva.ToString("N2") + "</td>";

                        decimal preciFinal = 0;
                        //var UsaPrecioConIva = dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && (x.CondicionIva == "MO" || x.CondicionIva == "CF"));
                        //if (usu.UsaPrecioFinalConIVA || UsaPrecioConIva)
                        if (usu.UsaPrecioFinalConIVA)
                            preciFinal = Math.Round(decimal.Parse(detalle.PrecioUnitarioConIva.ToString()), 2);
                        else
                            preciFinal = Math.Round(decimal.Parse(detalle.PrecioUnitario.ToString()), 2);
                        html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:Presupuestos.eliminarItem(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                        //html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:Presupuestos.modificarItem(" + detalle.ID + ", '" + (detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "") + "' ,'" + detalle.Cantidad.ToString("").Replace(".", ",") + "','" + detalle.Concepto + "','" + preciFinal.ToString("").Replace(".", ",") + "','" + detalle.Iva.ToString("N2") + "','" + detalle.Bonificacion.ToString("").Replace(".", ",") + "');\"><i class='fa fa-edit'></i></a></td>";
                        html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:Presupuestos.modificarItem(" + detalle.ID + ", '" + (detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "") + "' ,'" + detalle.Cantidad.ToString("").Replace(".", ",") + "','" + detalle.Concepto + "','" + preciFinal.ToString("").Replace(".", ",") + "','" + detalle.IdTipoIva.ToString() + "','" + detalle.Bonificacion.ToString("").Replace(".", ",") + "');\"><i class='fa fa-edit'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='8' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static string previsualizar(int id, int idPersona, string fechaVencimiento, 
        string nombre, int numero, string condicionesPago, string obs, string vendedor)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var tipo = "PRE";
            var modo = "";
            var tipoConcepto = 1;
            var fecha = DateTime.Now.Date.ToString("dd/MM/yyyy");
            var nroPresupuesto = numero.ToString("#00000000");
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            var punto = UsuarioCommon.ObtenerPuntosDeVenta(usu.IDUsuario).FirstOrDefault();
            if (!ComprobanteCart.Retrieve().Items.Any())
                throw new Exception("Debe Ingresar al menos un Items a presupuestar.");

            var actividad = UsuarioCommon.ObtenerActividades(usu.IDUsuario).FirstOrDefault();

            Common.CrearComprobante(usu, id, idPersona, tipo, modo, fecha, condicionesPago, 
                tipoConcepto, fechaVencimiento, punto.ID, ref nroPresupuesto, obs, 0,
                vendedor,"",false, actividad.ID, "", Common.ComprobanteModo.Previsualizar);

            return usu.IDUsuario + "_prev.pdf";
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string GenerarRemito(int id)//Comprobantes entity, string fileNameRemito
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                Random rnd = new Random();
                string random = rnd.Next(1, 9999).ToString("#0000");
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var entity = dbContext.Presupuestos.Include("PresupuestoDetalle").Include("Personas").Where(x => x.IDPresupuesto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                var fileNameRemito = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.Numero.ToString("#00000000") + "-" + random + ".pdf";

                var pathRemito = HttpContext.Current.Server.MapPath("~/remitos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);
                if (!System.IO.File.Exists(pathRemito))
                {
                    ComprobanteCart.Retrieve().Items.Clear();
                    var contador = 0;
                    foreach (var item in entity.PresupuestoDetalle)
                    {
                        contador++;
                        var idconcepto = (item.IDConcepto == null) ? null : item.IDConcepto;
                        AgregarItem(contador, idconcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);
                    }
                    Common.GenerarRemito(usu, id, entity.IDPersona, entity.FechaAlta.ToString("dd/MM/yyyy"), 0, entity.Numero.ToString(), entity.FormaDePago, entity.Observaciones, entity.Vendedor,0, "PRE","", FETipoComprobante.REMITO, random,"","",true,"");
                }

                return fileNameRemito;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static string generarPresupuesto(int id, string extension)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                Random rnd = new Random();
                string random = rnd.Next(1, 9999).ToString("#0000");
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var entity = dbContext.Presupuestos.Include("PresupuestoDetalle").Include("Personas").Where(x => x.IDPresupuesto == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                var fileNameRemito = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.Numero.ToString("#00000000") + "-" + random + ".pdf";
                var pathRemito = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);

                if (System.IO.File.Exists(pathRemito))
                    System.IO.File.Delete(pathRemito);
                
                ComprobanteCart.Retrieve().Items.Clear();
                var contador = 0;
                foreach (var item in entity.PresupuestoDetalle)
                {
                    contador++;
                    var idconcepto = (item.IDConcepto == null) ? null : item.IDConcepto;
                    AgregarItem(contador, idconcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);
                }
                Common.GenerarRemito(usu, id,  entity.IDPersona, entity.FechaAlta.ToString("dd/MM/yyyy"), 0, entity.Numero.ToString(), entity.FormaDePago, entity.Observaciones, entity.Vendedor, 0, "PRE", "", FETipoComprobante.PRESUPUESTO, random,"","", true, "");
                

                if (extension.Equals("PNG"))
                {
                    Document pdfDocument = new Document(pathRemito);

                    // Create Resolution object
                    Resolution resolution = new Resolution(300);

                    // Create TiffSettings object
                    TiffSettings tiffSettings = new TiffSettings
                    {
                        Compression = CompressionType.None,
                        Depth = ColorDepth.Default,
                        Shape = ShapeType.Portrait,
                        SkipBlankPages = false
                    };

                    // Create TIFF device
                    TiffDevice tiffDevice = new TiffDevice(resolution, tiffSettings);

                    var fileNameRemitoTif = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.Numero.ToString("#00000000") + "-" + random + ".tif";
                    var pathRemitoTif = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario.ToString() + "/" + fileNameRemitoTif);


                    // Convert a particular page and save the image to stream
                    tiffDevice.Process(pdfDocument, pathRemitoTif);


                    var fileNameRemitoPng = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.Numero.ToString("#00000000") + "-" + random + "_TMP.png";
                    var pathRemitoPng = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);
                    
                    System.Drawing.Bitmap.FromFile(pathRemitoTif).Save(pathRemitoPng, System.Drawing.Imaging.ImageFormat.Png);


                    fileNameRemito = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.Numero.ToString("#00000000") + "-" + random + ".png";
                    pathRemito = HttpContext.Current.Server.MapPath("~/files/presupuestos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);

                    Bitmap source = new Bitmap(pathRemitoPng);
                    //create the destination (cropped) bitmap
                    Bitmap bmpCropped = new Bitmap(source.Width, source.Height);
                    //create the graphics object to draw with
                    Graphics g = Graphics.FromImage(bmpCropped);

                    Rectangle rectDestination = new Rectangle(0, 0, bmpCropped.Width, bmpCropped.Height);
                    Rectangle rectCropArea = new Rectangle(0, 103, bmpCropped.Width, bmpCropped.Height - 103);

                    //draw the rectCropArea of the original image to the rectDestination of bmpCropped
                    g.DrawImage(source, rectDestination, rectCropArea, GraphicsUnit.Pixel);
                    //release system resources

                    bmpCropped.Save(pathRemito, ImageFormat.Png);


                    //File.Delete(pathRemitoTif);


                    
                

                }

                return fileNameRemito;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public static Bitmap CropAtRect(Bitmap b, System.Drawing.Rectangle r)
    {
        Bitmap nb = new Bitmap(b.Width, b.Height);
        Graphics g = Graphics.FromImage(nb);
        g.DrawImage(b, -r.X, -r.Y);
        return nb;
    }



    public static Bitmap CropImage(Bitmap source, System.Drawing.Rectangle section)
    {
        Bitmap bmp = new Bitmap(section.Width, section.Height);
        Graphics g = Graphics.FromImage(bmp);

        g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

        return bmp;
    }


    private static void AgregarItem(int id, int? idConcepto, string concepto, decimal iva, decimal precio, decimal bonif, decimal cantidad)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = ComprobanteCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
                ComprobanteCart.Retrieve().Items.Remove(aux);
            }

            var tra = new ComprobantesDetalleViewModel();
            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
            tra.Concepto = concepto;

            if (iva.ToString() != null)
            {
                //tra.Iva = decimal.Parse(iva.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                tra.Iva = obtenerValorIVA(int.Parse(iva.ToString()));
                tra.IdTipoIva = int.Parse(iva.ToString());
            }
            else
            {
                tra.Iva = 0;
                tra.IdTipoIva = 0;
            }

            tra.Bonificacion = bonif;
            tra.PrecioUnitario = precio;
            tra.Cantidad = cantidad;
            tra.IDConcepto = idConcepto;
            ComprobanteCart.Retrieve().Items.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private static decimal obtenerValorIVA(int idTipoIVA)
    {
        switch (idTipoIVA)
        {
            case 1:
                return 0;
            case 2:
                return 0;
            case 3:
                return 0;
            case 4:
                return Convert.ToDecimal(10.5);
            case 5:
                return 21;
            case 6:
                return 27;
            case 8:
                return 5;
            case 9:
                return Convert.ToDecimal(2.5);
            default:
                return 0;
        }
    }

}