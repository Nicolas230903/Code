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
using ACHE.Negocio.Common;
using ACHE.Negocio.Facturacion;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using ACHE.Negocio.Productos;
using ACHE.FacturaElectronica.WSPersonaServiceA5v34;
using System.Drawing;

public partial class comprobantesv : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                }
                else
                    Response.Redirect("/comprobantes.aspx");
            }
        }
    }

    [WebMethod(true)]
    public static void verificarArchivoComprobante(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            string[] tipoComprobante = { "FCA", "FCAMP" };

            var entity = dbContext.Comprobantes.Include("PuntosDeVenta").Include("Personas").Include("ComprobantesDetalle")
                .Where(x => x.IDComprobante == id).FirstOrDefault();
            if (entity != null)
            {
                if (entity.FechaCAE.HasValue)
                {
                    var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                    //Configuro botones
                    var fileName = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + entity.Tipo + "-" + entity.PuntosDeVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000") + ".pdf";

                    var pathComprobante = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario.ToString() + "/Comprobantes/" + entity.FechaAlta.Year.ToString() + "/" + fileName);
                    if (tipoComprobante.Contains(entity.Tipo) && entity.Personas.CondicionIva.ToUpper().Equals("MONOTRIBUTO"))
                    {
                        fileName = GenerarPDF(entity);
                    }
                    else
                    {
                        if (!System.IO.File.Exists(pathComprobante))
                            fileName = GenerarPDF(entity);
                    }                    
                }
                else
                    throw new Exception("El comprobante no tiene CAE");
            }
            else
                throw new Exception("No existe el comprobante");
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            string[] tipoComprobante = { "FCA", "FCAMP" };

            var entity = dbContext.Comprobantes.Include("PuntosDeVenta").Include("Personas").Include("ComprobantesDetalle")
                .Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (entity != null)
            {
                if (entity.FechaCAE.HasValue)
                {
                    hdnIDUsuario.Value = CurrentUser.IDUsuario.ToString();
                    //Configuro botones
                    var fileName = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + entity.Tipo + "-" + entity.PuntosDeVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000") + ".pdf";

                    var pathComprobante = HttpContext.Current.Server.MapPath("~/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/Comprobantes/" + entity.FechaAlta.Year.ToString() + "/" + fileName);
                    //if(tipoComprobante.Contains(entity.Tipo) && entity.Personas.CondicionIva.ToUpper().Equals("MO"))
                    //{
                    //}
                    //else
                    //{
                    //    if (!System.IO.File.Exists(pathComprobante))
                    //        fileName = GenerarPDF(entity);
                    //}

                    if (System.IO.File.Exists(pathComprobante))
                        System.IO.File.Delete(pathComprobante);
                    fileName = GenerarPDF(entity);

                    hdnTipoComprobante.Value = entity.Tipo;
                    hdnRazonSocial.Value = entity.Personas.RazonSocial;
                    hdnIDActividad.Value = entity.IdActividad.ToString();
                    txtEnvioPara.Value = entity.Personas.Email;
                    hdnFileName.Value = fileName;
                    ifrPdf.Attributes.Add("src", "/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/Comprobantes/" + entity.FechaAlta.Year.ToString() + "/" + fileName + "#zoom=100&view=FitH,top");
                    lnkDescargar2.NavigateUrl = lnkDescargar.NavigateUrl = "/pdfGenerator.ashx?file=" + fileName;
                    lnkPreview.Attributes.Add("onclick", "Common.previsualizarComprobanteGenerado('" + fileName + "');");
                    lnkPrint.Attributes.Add("onclick", "Common.imprimirArchivoDesdeIframe('" + fileName + "');");
                    lnkPrint2.Attributes.Add("onclick", "Common.imprimirArchivoDesdeIframe('" + fileName + "');");

                    litRazonSocial.Text = CurrentUser.RazonSocial + " - " + CurrentUser.CUIT;
                    litDomicilio.Text = CurrentUser.Domicilio;
                    litPaisCiudad.Text = CurrentUser.Provincia + ", " + CurrentUser.Ciudad;
                    litTelefono.Text = CurrentUser.Telefono;
                    litEmail.Text = CurrentUser.Email;

                    //Datos de la fc
                    litComprobante.Text = entity.Tipo + " " + entity.PuntosDeVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000");
                    litPersonaRazonSocial.Text = entity.Personas.RazonSocial;
                    litPersonaDomicilio.Text = entity.Personas.Domicilio;
                    litPersonaPaisCiudad.Text = entity.Personas.Provincias.Nombre + ", " + entity.Personas.Ciudades.Nombre;
                    //litPersonaEmail.Text = entity.Personas.Email;
                    //litPersonaTelefono.Text = entity.Personas.Telefono;
                    litFecha.Text = entity.FechaComprobante.ToLongDateString();
                    //litFechaVencimiento.Text = entity.FechaVencimiento.ToLongDateString();
                    litPersonaCondicionIva.Text = UsuarioCommon.GetCondicionIvaDesc(entity.Personas.CondicionIva);

                    var detalle = entity.ComprobantesDetalle.OrderBy(x => x.Concepto).Select(x => new ComprobantesDetalleViewModel
                    {
                        Codigo = (x.Conceptos == null) ? "" : x.Conceptos.Codigo,
                        Concepto = x.Concepto,
                        Cantidad = x.Cantidad,
                        PrecioUnitario = x.PrecioUnitario,
                        Bonificacion = x.Bonificacion,
                        Iva = x.Iva
                    });

                    rptDetalle.DataSource = detalle;
                    rptDetalle.DataBind();

                    //Precio Neto = Es el precio del artículo más todos los gravamenes o impuestos que este genere. 
                    //Precio Bruto= Es el precio base, sin incluir los gravamenes o impuestos.


                    litSubtotal.Text = entity.ImporteTotalBruto.ToString("N2");
                    litIva.Text = (entity.ImporteTotalNeto - entity.ImporteTotalBruto).ToString("N2");
                    litTotal.Text = entity.ImporteTotalNeto.ToString("N2");

                    //Datos para envio de mail
                    var datosPersona = dbContext.Personas.Where(x => x.IDPersona == entity.IDPersona).FirstOrDefault();
                    var datosUsuario = dbContext.Usuarios.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault();

                    if (datosPersona != null && datosUsuario != null)
                    {
                        this.txtEnvioPara.Value = datosPersona.Email;
                        this.txtEnvioAsunto.Value = datosUsuario.CUIT.ToString() + " - " + datosUsuario.RazonSocial.ToString().ToUpper() + " - " + litComprobante.Text;
                        this.txtEnvioMensaje.Value = "SE ADJUNTA COMPROBANTE: " + datosPersona.RazonSocial.ToString().ToUpper() + " (" + datosPersona.NroDocumento.ToString() + ")";
                    }                    

                    if (entity.Observaciones != string.Empty)
                        litObservaciones.Text = entity.Observaciones;
                    else
                        litObservaciones.Text = "El comprobante no tiene observaciones";
                }
                else
                    Response.Redirect("/error.aspx");
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static string previsualizar(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string domicilioTransporteCliente = "";

            var pathPdf = HttpContext.Current.Server.MapPath("/files/facturas/" + usu.IDUsuario + "_prev.pdf");
            if (System.IO.File.Exists(pathPdf))
                System.IO.File.Delete(pathPdf);

            using (var dbContext = new ACHEEntities())
            {
                Comprobantes entity = dbContext.Comprobantes.Include("Personas").Include("ComprobantesDetalle").Include("PuntosDeVenta")
                    .Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                if (entity != null)
                {

                    FEFacturaElectronica fe = new FEFacturaElectronica();
                    FEComprobante comprobante = new FEComprobante();

                    comprobante.TipoComprobante = ComprobantesCommon.ObtenerTipoComprobante(entity.Tipo);

                    comprobante.IDComprobante = id.ToString();
                    comprobante.Cuit = long.Parse(usu.CUIT);
                    comprobante.PtoVta = entity.PuntosDeVenta.Punto;
                    comprobante.Concepto = (FEConcepto)entity.TipoConcepto;
                    comprobante.Fecha = entity.FechaComprobante;
                    comprobante.FchServDesde = null;
                    comprobante.FchServHasta = null;
                    comprobante.FchVtoPago = entity.FechaVencimiento;
                    comprobante.CodigoMoneda = "PES";
                    comprobante.CotizacionMoneda = 1;

                    //Seteo los datos del facturante
                    comprobante.CondicionVenta = usu.CondicionIVA;
                    comprobante.Domicilio = usu.Domicilio;
                    comprobante.CiudadProvincia = usu.Ciudad + ", " + usu.Provincia;
                    comprobante.RazonSocial = usu.RazonSocial;
                    comprobante.Telefono = usu.Telefono;
                    comprobante.Celular = usu.Celular;
                    comprobante.IIBB = usu.IIBB;
                    if (usu.CondicionIVA == "MO")
                        comprobante.CondicionIva = "Monotributista";
                    else if (usu.CondicionIVA == "RI")
                        comprobante.CondicionIva = "IVA Responsable Inscripto";
                    else if (usu.CondicionIVA == "EX")
                        comprobante.CondicionIva = "IVA Exento";
                    else
                        comprobante.CondicionIva = "Responsable No Inscripto";
                    comprobante.FechaInicioActividades = usu.FechaInicio.HasValue ? usu.FechaInicio.Value.ToString("dd/MM/yyyy") : "";

                    //Seteo los datos de la persona
                    comprobante.DocTipo = entity.Personas.TipoDocumento == "DNI" ? 96 : 80;
                    comprobante.DocNro = long.Parse(entity.Personas.NroDocumento);
                    comprobante.CondicionVenta = entity.CondicionVenta;
                    comprobante.ClienteNombre = entity.Personas.RazonSocial;
                    if (entity.Personas.CondicionIva == "MO")
                        comprobante.ClienteCondiionIva = "Monotributista";
                    else if (entity.Personas.CondicionIva == "RI")
                        comprobante.ClienteCondiionIva = "Responsable Inscripto";
                    else if (entity.Personas.CondicionIva == "EX")
                        comprobante.ClienteCondiionIva = "Exento";
                    else
                        comprobante.ClienteCondiionIva = "Responsable No Inscripto";

                    comprobante.ClienteDomicilio = entity.Personas.Domicilio + " " + entity.Personas.PisoDepto;
                    comprobante.ClienteLocalidad = entity.Personas.Ciudades.Nombre + ", " + entity.Personas.Provincias.Nombre;
                    comprobante.Observaciones = entity.Observaciones;
                    comprobante.NumeroComprobante = entity.Numero;


                    //Seteo los datos de la factura
                    //comprobante.ImpTotal = 2;
                    comprobante.ImpTotConc = 0;
                    comprobante.ImpOpEx = 0;
                    comprobante.DetalleIva = new List<FERegistroIVA>();
                    comprobante.Tributos = new List<FERegistroTributo>();
                    comprobante.ItemsDetalle.Add(new FEItemDetalle() { Cantidad = 1, Descripcion = "Producto 1", Precio = 1, Codigo = "55422" });
                    comprobante.ItemsDetalle.Add(new FEItemDetalle() { Cantidad = 1, Descripcion = "Producto 2", Precio = 1, Codigo = "554223" });

                    //var pathCertificado = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["CertificadoAFIP"]);
                    var pathTemplateFc = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FE.Template"]);
                    fe.GrabarEnDisco(comprobante, pathPdf, pathTemplateFc, domicilioTransporteCliente);

                    return usu.IDUsuario + "_prev.pdf";
                }
                else
                    throw new Exception("Comprobante inválido");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    
    [WebMethod(true)]
    public static string GenerarRemito(int id, bool conLogo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                string domicilioTransporteCliente = string.Empty;
                Random rnd = new Random();
                string random = rnd.Next(1, 9999).ToString("#0000");
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var entity = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                var fileNameRemito = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.PuntosDeVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000") + "-" + random + ".pdf";

                var pathRemito = HttpContext.Current.Server.MapPath("~/files/remitos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);
                if (System.IO.File.Exists(pathRemito))
                    System.IO.File.Delete(pathRemito);

                //ComprobanteCart.Retrieve().Items.Clear();
                //var contador = 0;
                //foreach (var item in entity.ComprobantesDetalle)
                //{
                //    contador++;
                //    var idconcepto = (item.IDConcepto == null) ? null : item.IDConcepto;
                //    AgregarItem(contador, idconcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);
                //}
                int idDomicilio = entity.IdDomicilio != null ? (int)entity.IdDomicilio : 0;
                string fechaEntrega = entity.FechaEntrega != null ? ((DateTime)entity.FechaEntrega).ToString("dd/MM/yyyy") : "";

                string NroPedidoDeVenta = "";
                string NroPresupuesto = "";

                if (entity.Tipo.Equals("EDA"))
                {
                    if (entity.IdComprobanteVinculado != null)
                    {
                        Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == entity.IdComprobanteVinculado).FirstOrDefault();

                        if (c != null)
                        {
                            NroPedidoDeVenta = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                            if (c.IdPresupuestoVinculado != null)
                            {
                                Presupuestos p = dbContext.Presupuestos.Where(w => w.IDPresupuesto == c.IdPresupuestoVinculado).FirstOrDefault();

                                if (p != null)
                                    NroPresupuesto = p.Numero.ToString("#00000000");
                            }
                        }
                    }
                }

                if (entity.IdTransportePersona != null)
                {
                    domicilioTransporteCliente = (from transDom in dbContext.TransportePersona
                                                  join prov in dbContext.Provincias on transDom.IDProvincia equals prov.IDProvincia
                                                  join ciud in dbContext.Ciudades on transDom.IDCiudad equals ciud.IDCiudad
                                                  where transDom.IdUsuario == usu.IDUsuario && transDom.IdTransportePersona == entity.IdTransportePersona
                                                  select
                                                      transDom.RazonSocial + "|" + transDom.Domicilio + " " + transDom.PisoDepto + " - CP " + transDom.CodigoPostal +
                                                      " - " + transDom.Provincia + ", " + transDom.Ciudad + "|Contacto: " +
                                                      transDom.Contacto + " - Tel: " + transDom.Telefono
                            ).FirstOrDefault().ToString();
                }

                Common.GenerarRemito(usu, id, entity.IDPersona, entity.FechaComprobante.ToString("dd/MM/yyyy"), 
                    entity.IDPuntoVenta, entity.Numero.ToString("#00000000"), entity.CondicionVenta, 
                    entity.Observaciones, entity.Vendedor, idDomicilio, entity.Tipo, 
                    fechaEntrega, FETipoComprobante.REMITO, random, NroPedidoDeVenta, NroPresupuesto, conLogo, domicilioTransporteCliente);

                return fileNameRemito;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void updateMargenesRemitoSinLogo(string vertical, string horizontal)
    {
        try
        {

            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                string tipo = "SIN LOGO";
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    RemitoComprobanteMargenes lpm = dbContext.RemitoComprobanteMargenes.Where(w => w.IdUsuario == usu.IDUsuario && w.Tipo.Equals(tipo)).FirstOrDefault();

                    if (lpm == null)
                    {
                        RemitoComprobanteMargenes nlpm = new RemitoComprobanteMargenes();
                        nlpm.IdUsuario = usu.IDUsuario;
                        nlpm.Horizontal = int.Parse(horizontal);
                        nlpm.Vertical = int.Parse(vertical);
                        nlpm.Fecha = DateTime.Now;
                        nlpm.Tipo = tipo;
                        dbContext.RemitoComprobanteMargenes.Add(nlpm);
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

    [WebMethod(true)]
    public static string GenerarRemitoSinLogo(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                Random rnd = new Random();
                string random = rnd.Next(1, 9999).ToString("#0000");
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var entity = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                var fileNameRemito = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.PuntosDeVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000") + "-" + random + ".pdf";

                var pathRemito = HttpContext.Current.Server.MapPath("~/files/remitos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);
                if (System.IO.File.Exists(pathRemito))
                    System.IO.File.Delete(pathRemito);

                //ComprobanteCart.Retrieve().Items.Clear();
                //var contador = 0;
                //foreach (var item in entity.ComprobantesDetalle)
                //{
                //    contador++;
                //    var idconcepto = (item.IDConcepto == null) ? null : item.IDConcepto;
                //    AgregarItem(contador, idconcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);
                //}
                int idDomicilio = entity.IdDomicilio != null ? (int)entity.IdDomicilio : 0;
                string fechaEntrega = entity.FechaEntrega != null ? ((DateTime)entity.FechaEntrega).ToString("dd/MM/yyyy") : "";

                string NroPedidoDeVenta = "";
                string NroPresupuesto = "";

                if (entity.Tipo.Equals("EDA"))
                {
                    if (entity.IdComprobanteVinculado != null)
                    {
                        Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == entity.IdComprobanteVinculado).FirstOrDefault();

                        if (c != null)
                        {
                            NroPedidoDeVenta = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                            if (c.IdPresupuestoVinculado != null)
                            {
                                Presupuestos p = dbContext.Presupuestos.Where(w => w.IDPresupuesto == c.IdPresupuestoVinculado).FirstOrDefault();

                                if (p != null)
                                    NroPresupuesto = p.Numero.ToString("#00000000");
                            }
                        }
                    }
                }

                Common.GenerarRemitoSinLogo(usu, id, entity.IDPersona, entity.FechaAlta.ToString("dd/MM/yyyy"), entity.IDPuntoVenta, entity.Numero.ToString("#00000000"), entity.CondicionVenta, entity.Observaciones, entity.Vendedor, idDomicilio, entity.Tipo, fechaEntrega, FETipoComprobante.REMITO, random, NroPedidoDeVenta, NroPresupuesto);

                return fileNameRemito;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void updateMargenesRemitoTalonario(string vertical, string horizontal)
    {
        try
        {

            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                string tipo = "TALONARIO";
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                using (var dbContext = new ACHEEntities())
                {
                    RemitoComprobanteMargenes lpm = dbContext.RemitoComprobanteMargenes.Where(w => w.IdUsuario == usu.IDUsuario && w.Tipo.Equals(tipo)).FirstOrDefault();

                    if (lpm == null)
                    {
                        RemitoComprobanteMargenes nlpm = new RemitoComprobanteMargenes();
                        nlpm.IdUsuario = usu.IDUsuario;
                        nlpm.Horizontal = int.Parse(horizontal);
                        nlpm.Vertical = int.Parse(vertical);
                        nlpm.Fecha = DateTime.Now;
                        nlpm.Tipo = tipo;
                        dbContext.RemitoComprobanteMargenes.Add(nlpm);
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

    [WebMethod(true)]
    public static string GenerarRemitoTalonario(int id, bool verTotal)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                string domicilioTransporteCliente = string.Empty;
                Random rnd = new Random();
                string random = rnd.Next(1, 9999).ToString("#0000");
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var entity = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                var fileNameRemito = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + entity.PuntosDeVenta.Punto.ToString("#0000") + "-" + entity.Numero.ToString("#00000000") + "-" + random + ".pdf";

                var pathRemito = HttpContext.Current.Server.MapPath("~/files/remitos/" + usu.IDUsuario.ToString() + "/" + fileNameRemito);
                if (System.IO.File.Exists(pathRemito))
                    System.IO.File.Delete(pathRemito);

                //ComprobanteCart.Retrieve().Items.Clear();
                //var contador = 0;
                //foreach (var item in entity.ComprobantesDetalle)
                //{
                //    contador++;
                //    var idconcepto = (item.IDConcepto == null) ? null : item.IDConcepto;
                //    AgregarItem(contador, idconcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);
                //}
                int idDomicilio = entity.IdDomicilio != null ? (int)entity.IdDomicilio : 0;
                string fechaEntrega = entity.FechaEntrega != null ? ((DateTime)entity.FechaEntrega).ToString("dd/MM/yyyy") : "";

                string NroPedidoDeVenta = "";
                string NroPresupuesto = "";

                if (entity.Tipo.Equals("EDA"))
                {
                    if (entity.IdComprobanteVinculado != null)
                    {
                        Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == entity.IdComprobanteVinculado).FirstOrDefault();

                        if (c != null)
                        {
                            NroPedidoDeVenta = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                            if (c.IdPresupuestoVinculado != null)
                            {
                                Presupuestos p = dbContext.Presupuestos.Where(w => w.IDPresupuesto == c.IdPresupuestoVinculado).FirstOrDefault();

                                if (p != null)
                                    NroPresupuesto = p.Numero.ToString("#00000000");
                            }
                        }
                    }
                }

                if(entity.IdTransportePersona != null)
                {
                    domicilioTransporteCliente = (from transDom in dbContext.TransportePersona
                            join prov in dbContext.Provincias on transDom.IDProvincia equals prov.IDProvincia
                            join ciud in dbContext.Ciudades on transDom.IDCiudad equals ciud.IDCiudad
                            where transDom.IdUsuario == usu.IDUsuario && transDom.IdTransportePersona == entity.IdTransportePersona
                            select
                                transDom.RazonSocial + "|" + transDom.Domicilio + " " + transDom.PisoDepto + " - CP " + transDom.CodigoPostal +
                                " - " + transDom.Provincia + ", " + transDom.Ciudad + "|Contacto: " +
                                transDom.Contacto + " - Tel: " + transDom.Telefono                            
                            ).FirstOrDefault().ToString();
                }

                Common.GenerarRemitoTalonario(usu, id, entity.IDPersona, entity.FechaComprobante.ToString("dd/MM/yyyy"), entity.IDPuntoVenta, 
                                        entity.Numero.ToString("#00000000"), entity.CondicionVenta, entity.Observaciones, entity.Vendedor, 
                                        idDomicilio, entity.Tipo, fechaEntrega, FETipoComprobante.REMITO, random, NroPedidoDeVenta, 
                                        NroPresupuesto, verTotal, domicilioTransporteCliente);

                return fileNameRemito;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string GenerarTicket(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                var entity = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                string nroComprobante = entity.Numero.ToString();
                var fileNameTicket = entity.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + entity.Tipo + "-" + entity.PuntosDeVenta.Punto.ToString().PadLeft(4, '0') + "-" + entity.Numero.ToString().PadLeft(8, '0') + "_Ticket.pdf";

                var pathTicket = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Comprobantes/" + DateTime.Now.Year.ToString() + "/" + fileNameTicket);

                if (!System.IO.File.Exists(fileNameTicket))
                {
                    ComprobanteCart.Retrieve().Items.Clear();
                    var contador = 0;
                    foreach (var item in entity.ComprobantesDetalle)
                    {
                        contador++;
                        var idconcepto = (item.IDConcepto == null) ? null : item.IDConcepto;
                        //AgregarItem(contador, idconcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);

                        agregarItem(0, (int)item.IDConcepto, item.Concepto, (int)item.IdTipoIVA, item.PrecioUnitario, item.Bonificacion,
                        item.Cantidad, entity.IDPersona, item.Conceptos.Codigo, 0, "", false, 0);


                    }
                    Common.CrearComprobanteTicket(usu, id, entity.IDPersona, entity.Tipo, entity.Modo, 
                                                  entity.FechaAlta.ToShortDateString(), entity.CondicionVenta, entity.TipoConcepto,
                                                  entity.FechaVencimiento.ToShortDateString(), entity.IDPuntoVenta, ref nroComprobante, 
                                                  entity.Observaciones, Common.ComprobanteModo.GenerarPDF);                    
                }
                return fileNameTicket;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    //private static void AgregarItem(int id, int? idConcepto, string concepto, decimal iva, decimal precio, decimal bonif, decimal cantidad)
    //{
    //    if (HttpContext.Current.Session["CurrentUser"] != null)
    //    {
    //        if (id != 0)
    //        {
    //            var aux = ComprobanteCart.Retrieve().Items.Where(x => x.ID == id).FirstOrDefault();
    //            ComprobanteCart.Retrieve().Items.Remove(aux);
    //        }

    //        var tra = new ComprobantesDetalleViewModel();
    //        tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
    //        tra.Concepto = concepto;
    //        tra.Iva = iva;
    //        tra.Bonificacion = bonif;
    //        tra.PrecioUnitario = precio;
    //        tra.Cantidad = cantidad;
    //        tra.IDConcepto = idConcepto;
    //        ComprobanteCart.Retrieve().Items.Add(tra);
    //    }
    //    else
    //        throw new Exception("Por favor, vuelva a iniciar sesión");
    //}

    public static void agregarItem(int id, int idConcepto, string concepto, int idIva,
                                    decimal precio, decimal bonif, decimal cantidad, int idPersona,
                                    string codigo, int idPlanDeCuenta, string codigoCta, bool ajuste,
                                    decimal ajusteSubtotal)
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
            tra.Concepto = concepto;
            tra.Codigo = codigo;
            tra.CodigoPlanCta = codigoCta;

            if (idIva != 0)
            {
                //tra.Iva = decimal.Parse(iva.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                tra.Iva = obtenerValorIVA(idIva);
                tra.IdTipoIva = idIva;
            }
            else
            {
                tra.Iva = 0;
                tra.IdTipoIva = 0;
            }


            if (bonif != 0)
                tra.Bonificacion = bonif;
            else
                tra.Bonificacion = 0;

            tra.PrecioUnitario = precio;
            tra.Cantidad = cantidad;
            if (idConcepto != 0)
                tra.IDConcepto = idConcepto;
            else
                tra.IDConcepto = null;

            if (idPlanDeCuenta != 0)
                tra.IDPlanDeCuenta = idPlanDeCuenta;

            tra.Ajuste = ajuste;
            if (tra.Ajuste)
                tra.SubTotalAjustado = ajusteSubtotal;
            else
                tra.PrecioUnitario = ObtenerPrecioFinal(tra.PrecioUnitario, tra.Iva.ToString(), idPersona);


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

    private static string GenerarPDF(Comprobantes comprobante)
    {

        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        var nroComprobante = comprobante.Numero.ToString("#00000000");
        ComprobanteCart.Retrieve().Items.Clear();
        var aux = 0;
        foreach (var item in comprobante.ComprobantesDetalle)
        {
            //AgregarItem(0, item.IDConcepto, item.Concepto, item.Iva, item.PrecioUnitario, item.Bonificacion, item.Cantidad);

            agregarItem(0, (int)item.IDConcepto, item.Concepto, (int)item.IdTipoIVA, item.PrecioUnitario, item.Bonificacion,
            item.Cantidad, comprobante.IDPersona, item.Conceptos.Codigo, 0, "", false, 0);

            aux++;
        }
        if (comprobante.IdComprobanteAsociado == null)
            comprobante.IdComprobanteAsociado = 0;
        string fechaEntrega = "";
        if (comprobante.FechaEntrega != null)
            fechaEntrega = comprobante.FechaEntrega.Value.ToString("dd/MM/yyy");


        Common.CrearComprobante(usu, comprobante.IDComprobante, comprobante.IDPersona, comprobante.Tipo, 
            comprobante.Modo, comprobante.FechaComprobante.ToString("dd/MM/yyy"), comprobante.CondicionVenta, 
            comprobante.TipoConcepto, comprobante.FechaVencimiento.ToString("dd/MM/yyy"), comprobante.IDPuntoVenta, 
            ref nroComprobante, comprobante.Observaciones, (int)comprobante.IdComprobanteAsociado, 
            comprobante.Vendedor, fechaEntrega, false, comprobante.IdActividad, comprobante.ModalidadPagoAFIP, Common.ComprobanteModo.GenerarPDF);
        var numero = comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
        var pathPdf = comprobante.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + comprobante.Tipo + "-" + numero + ".pdf";
        return pathPdf;
    }


    [WebMethod(true)]
    public static ComprobantesDescargasViewModel obtenerDatosParaEnvioDeCorreo(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var archivos = new ComprobantesDescargasViewModel();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Comprobantes comp = dbContext.Comprobantes.Where(w => w.IDComprobante == id).FirstOrDefault();

                if(comp != null)
                {
                    archivos.Comprobante = comp.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + comp.Tipo + "-" + comp.PuntosDeVenta.Punto.ToString("#0000") + "-" + comp.Numero.ToString("#00000000") + ".pdf";
                    archivos.Remito = comp.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + comp.Numero.ToString("") + ".pdf";
                    archivos.IdPersona = comp.Personas.IDPersona.ToString();
                    archivos.RazonSocialCliente = comp.Personas.RazonSocial.ToUpper();

                    verificarArchivoComprobante(id);

                }
                else
                    throw new Exception("El comprobante no existe");

            }
            return archivos;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


}