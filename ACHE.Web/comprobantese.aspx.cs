using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using ACHE.Negocio.Productos;
using ACHE.Negocio.Contabilidad;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using System.Configuration;
using System.Web.Script.Services;
using ACHE.Model.ViewModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;

public partial class comprobantese : BasePage
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
                //var plan = PermisosModulos.ObtenerPlanActual(dbContext, CurrentUser.IDUsuario);
                //if (PermisosModulosCommon.VerificarCantComprobantes(plan, CurrentUser))
                //    Response.Redirect("~/modulos/seguridad/elegir-plan.aspx?upgrade=2");

                hdnEnvioFE.Value = (CurrentUser.EnvioAutomaticoComprobante == true) ? "1" : "0";

                if (CurrentUser.UsaPrecioFinalConIVA)
                    liPrecioUnitario.Text = "<span id='spPrecioUnitario'> Precio Unit. con IVA</span> ";
                else
                    liPrecioUnitario.Text = "<span id='spPrecioUnitario'> Precio Unit. sin IVA</span> ";

                hdnUsaPrecioConIVA.Value = (CurrentUser.UsaPrecioFinalConIVA) ? "1" : "0";

                ComprobanteCart.Retrieve().Items.Clear();                

                ComprobantesVinculadosCart.Retrieve().Items.Clear();

                int idPresupuesto = 0;
                if (Request.QueryString["IDPresupuesto"] != string.Empty)
                    idPresupuesto = Convert.ToInt32(Request.QueryString["IDPresupuesto"]);


                txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtFechaVencimiento.Text = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");

                hdnPercepcionIIBB.Value = (Convert.ToBoolean(CurrentUser.AgentePercepcionIIBB)) ? "1" : "0";
                hdnPercepcionIVA.Value = (Convert.ToBoolean(CurrentUser.AgentePercepcionIVA)) ? "1" : "0";

                hdnFacturaSoloContraEntrega.Value = (Convert.ToBoolean(CurrentUser.FacturaSoloContraEntrega)) ? "1" : "0";
                hdnUsaCantidadConDecimales.Value = (Convert.ToBoolean(CurrentUser.UsaCantidadConDecimales)) ? "1" : "0";                

                //ddlModo.Items.Add(new ListItem("Otro", "O"));
                ddlModo.Items.Add(new ListItem("Talonario preimpreso", "T"));
                if (CurrentUser.TieneFE)
                {
                    ddlModo.Items.Add(new ListItem("Electrónica", "E"));
                    ddlModo.SelectedValue = "E";
                    hdnTieneFE.Value = "1";
                }
                else
                    hdnTieneFE.Value = "0";

                lnkGenerarCAE.Visible = (CurrentUser.TieneFE);
                litPath.Text = "Alta";
                string idPersona = Request.QueryString["IDPersona"];
                if (!string.IsNullOrWhiteSpace(idPersona)) {
                    hdnIDPersona.Value = Request.QueryString["IDPersona"];
                }

                string tipo = Request.QueryString["tipo"];
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    hdnTipo.Value = Request.QueryString["tipo"];
                    var user = (WebUser)Session["CurrentUser"];

                    AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == user.IDUsuario && w.IdUsuarioAdicional == user.IDUsuarioAdicional).FirstOrDefault();
                    
                    if (afu != null)
                    {
                        switch (tipo)
                        {
                            case "EDA":
                                if (!afu.ComercialEntregas)
                                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");
                                break;
                            case "PDV":
                                if (!afu.ComercialPedidoDeVenta)
                                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");
                                break;
                            case "PDC":
                                if (!afu.SuministroComprobanteDeCompra)
                                     Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");
                                break;
                            case "FAC":
                                if (!afu.ComercialFacturaDeVenta)
                                     Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");
                                break;
                            default:
                                break;
                        }

                        hdnHabilitarCambioIvaEnArticulosDesdeComprobante.Value = (Convert.ToBoolean(afu.HabilitarCambioIvaEnArticulosDesdeComprobante)) ? "1" : "0";
                    }

                    this.txtCantidad.Style.Remove("type");
                    this.txtCantidad.Style.Add("type", "number");

                }              
                    

                hdnIDUsuario.Value = CurrentUser.IDUsuario.ToString();

                hdnMasDeUnaActividad.Value = (dbContext.Actividad.Where(w => w.IdUsuario == CurrentUser.IDUsuario).ToList().Count() > 1) ? "1" : "0";

                if (CurrentUser.ParaPDVSolicitarCompletarContacto)
                    hdnParaPDVSolicitarCompletarContacto.Value = "1";
                else
                    hdnParaPDVSolicitarCompletarContacto.Value = "0";

                if (idPresupuesto > 0)
                {
                    hdnPresupuesto.Value = idPresupuesto.ToString();

                    var presupuesto = dbContext.Presupuestos.Include("PresupuestoDetalle").Where(x => x.IDPresupuesto == idPresupuesto).FirstOrDefault();
                    if (presupuesto != null)
                    {
                        foreach (var det in presupuesto.PresupuestoDetalle)
                        {
                            var tra = new ComprobantesDetalleViewModel();
                            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                            tra.Concepto = det.Concepto;
                            tra.Iva = det.Iva;
                            tra.IdTipoIva = (int)det.IdTipoIVA;
                            tra.PrecioUnitario = det.PrecioUnitario;
                            tra.Bonificacion = det.Bonificacion;
                            tra.Cantidad = det.Cantidad;
                            tra.IDConcepto = det.IDConcepto;

                            ComprobanteCart.Retrieve().Items.Add(tra);
                        }
                        hdnIDPersona.Value = presupuesto.IDPersona.ToString();
                    }

                }

                if (CurrentUser.UsaPlanCorporativo)
                    cargarCuentasActivo();

                //valido si se puede modificar
                if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
                {
                    hdnID.Value = Request.QueryString["ID"];
                    if (hdnID.Value != "0")
                    {
                        lnkAnular.Visible = true;
                        var id = Convert.ToInt32(hdnID.Value);
                        
                        bool CompEnviado = dbContext.ComprobantesEnviados.Any(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDComprobante == id && x.Resultado == true);
                        if (!CompEnviado && CurrentUser.EnvioAutomaticoComprobante)
                            hdnEnvioFE.Value = "1";
                        else
                            hdnEnvioFE.Value = "0";

                        Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == id && x.Adjunto != null).FirstOrDefault();
                        if (c == null)                        
                            this.lnkDescargarAdjunto.Style.Add("Display", "none");                      


                        var entity = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDComprobante == id).FirstOrDefault();
                        if (entity != null)
                        {
                            hdnIDPersona.Value = entity.IDPersona.ToString();                            

                            if (entity.FechaCAE.HasValue)
                                Response.Redirect("/comprobantesv.aspx?ID=" + id);
                        }

                        litPath.Text = "Edición";

                    }
                }

                //valido si se puede modificar
                if (!string.IsNullOrEmpty(Request.QueryString["IdCompra"]) && hdnTipo.Value.Equals("DDC"))
                {
                    hdnIdCompraVinculada.Value = Request.QueryString["IdCompra"];
                    if (hdnIdCompraVinculada.Value != "0")
                    {
                        var idCompra = Convert.ToInt32(hdnIdCompraVinculada.Value);

                        var entity = dbContext.Compras.Where(x => x.IDCompra == idCompra).FirstOrDefault();
                        if (entity != null)
                        {
                            hdnIDPersona.Value = entity.IDPersona.ToString();
                        }
                    }
                }

                //Me fijo si viene de un pedido de venta y quieren realizar una precarga de la entrega.
                if (!string.IsNullOrEmpty(Request.QueryString["IDPDV"]) || !string.IsNullOrEmpty(Request.QueryString["IDFAC"]))
                {
                    hdnIdPedidoDeVenta.Value = !string.IsNullOrEmpty(Request.QueryString["IDPDV"]) ? Request.QueryString["IDPDV"] : Request.QueryString["IDFAC"];
                    var id = Convert.ToInt32(hdnIdPedidoDeVenta.Value);
                    Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == id).FirstOrDefault();
                    if (c != null)
                        hdnIDPersona.Value = c.IDPersona.ToString();

                }

            }
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosComprobantesViewModel getResultsJuntarComprobantes(int idPersona)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobantesParaJuntar(idPersona, usu);
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
    public static void juntarComprobantes(string[] id, int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.Concepto).ToList();
                if (!list.Any())
                {
                    if (id != null)
                    {
                        List<ComprobantesDetalle> cdu = new List<ComprobantesDetalle>();
                        List<ComprobantesDetalle> cdua = new List<ComprobantesDetalle>();
                        var dbContext = new ACHEEntities();
                        foreach (string item in id)
                        {
                            if (item != null)
                            {
                                int idComprobante = Convert.ToInt32(item.ToString());
                                List<ComprobantesDetalle> cd = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == idComprobante).ToList();
                                if (cd != null)
                                    cdu.AddRange(cd);
                            }
                        }

                        foreach (ComprobantesDetalle c in cdu)
                        {
                            bool existeConcepto = false;

                            foreach (ComprobantesDetalle d in cdua)
                            {
                                if (c.IDConcepto == d.IDConcepto)
                                {
                                    existeConcepto = true;
                                    d.Cantidad = d.Cantidad + c.Cantidad;
                                    if (c.PrecioUnitario > d.PrecioUnitario)                                                                            
                                        d.PrecioUnitario = c.PrecioUnitario;
                                    if (c.Iva > d.Iva)
                                        d.Iva = c.Iva;
                                }
                                   
                            }

                            if (!existeConcepto)                            
                                cdua.Add(c);
                        }

                        foreach (ComprobantesDetalle e in cdua)
                        {
                            Conceptos con = dbContext.Conceptos.Where(x => x.IDConcepto == e.IDConcepto).FirstOrDefault();
                            if(con != null)
                                agregarItem(0,e.IDConcepto.ToString(),e.Concepto,e.Iva.ToString(),e.PrecioUnitario.ToString(),e.Bonificacion.ToString(),
                                            e.Cantidad.ToString(), idPersona, con.Codigo,e.IDPlanDeCuenta.ToString(),"",false, "");
                            else
                                throw new Exception("Uno de los articulos no se encontro en la base de datos.");
                        }

                        foreach (string item in id)
                        {
                            int idComprobante = Convert.ToInt32(item.ToString());
                            Comprobantes cp = dbContext.Comprobantes.Where(x => x.IDComprobante == idComprobante).FirstOrDefault();
                            ComprobantesVinculadosCart.Retrieve().Items.Add(idComprobante);
                            if (cp != null)                            
                                cp.Tipo = "NDP";
                            else
                                throw new Exception("Uno de los comprobantes no se encontro en la base de datos.");
                        }

                        dbContext.SaveChanges();

                    }
                }
                else                
                    throw new Exception("El comprobante no debe tener items agregados previamente.");
                
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # comprobantese.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                return;
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    private void cargarCuentasActivo()
    {
        using (var dbContext = new ACHEEntities())
        {
            if (dbContext.ConfiguracionPlanDeCuenta.Any(x => x.IDUsuario == CurrentUser.IDUsuario))
            {
                var idctas = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == CurrentUser.IDUsuario).FirstOrDefault().CtasFiltroVentas.Split(',');
                List<Int32> cuentas = new List<Int32>();
                for (int i = 0; i < idctas.Length; i++)
                    cuentas.Add(Convert.ToInt32(idctas[i]));

                var listaAux = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && cuentas.Contains(x.IDPlanDeCuenta)).ToList();
                ddlPlanDeCuentas.Items.Add(new ListItem("", ""));

                foreach (var item in listaAux)
                    ddlPlanDeCuentas.Items.Add(new ListItem(item.Nombre, item.IDPlanDeCuenta.ToString()));
            }
            hdnUsaPlanCorporativo.Value = "1";
        }
    }

    [WebMethod(true)]
    public static void agregarItem(int id, string idConcepto, string concepto, string iva, 
                                    string precio, string bonif, string cantidad, int idPersona, 
                                    string codigo, string idPlanDeCuenta, string codigoCta, bool ajuste, string ajusteSubtotal)
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

            if (idPlanDeCuenta != "")
                tra.IDPlanDeCuenta = Convert.ToInt32(idPlanDeCuenta);

            tra.Ajuste = ajuste;
            if (tra.Ajuste)
                tra.SubTotalAjustado = decimal.Parse(ajusteSubtotal.Replace(SeparadorDeMiles, SeparadorDeDecimales));            
            else            
                tra.PrecioUnitario = ObtenerPrecioFinal(tra.PrecioUnitario, iva, idPersona);
            

            ComprobanteCart.Retrieve().Items.Add(tra);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerSubtotal(int id, string idConcepto, string concepto, string iva,
                                    string precio, string bonif, string cantidad, int idPersona,
                                    string codigo, string idPlanDeCuenta, string codigoCta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var tra = new ComprobantesDetalleViewModel();
            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
            tra.Concepto = concepto;
            tra.Codigo = codigo;
            tra.CodigoPlanCta = codigoCta;

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

            if (idPlanDeCuenta != "")
                tra.IDPlanDeCuenta = Convert.ToInt32(idPlanDeCuenta);


            tra.PrecioUnitario = ObtenerPrecioFinal(tra.PrecioUnitario, iva, idPersona);

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                if (usu.UsaPrecioFinalConIVA)
                    return tra.TotalConIva.ToString("N2");
                else
                    return tra.TotalSinIva.ToString("N2");
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void moverItem(int id, string concepto, string accion)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = ComprobanteCart.Retrieve().Items.Where(x => x.ID == id && x.Concepto == concepto).FirstOrDefault();
                if (aux != null)
                {
                    int posicion;
                    if (accion.Equals("subir"))
                        posicion = aux.ID - 1;                    
                    else                    
                        posicion = aux.ID + 1;
                    
                    if (ComprobanteCart.Retrieve().Items.Any(x => x.ID == posicion))
                    {
                        var auxNext = ComprobanteCart.Retrieve().Items.Where(x => x.ID == posicion).FirstOrDefault();
                        aux.ID = posicion;
                        auxNext.ID = id;
                    }
                }               
            }
            
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int getEntregasDeUnPedido(int id)
    {
        int idComprobante = 0;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var dbContext = new ACHEEntities();

                var comp = dbContext.Comprobantes.Where(w => w.Tipo.Equals("EDA") && w.IdComprobanteVinculado == id).FirstOrDefault();

                if (comp != null)
                {
                    idComprobante = comp.IDComprobante;
                }

                return idComprobante;

            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static RemitoComprobanteMargenes getMargenesRemitoSinLogo()
    {
        try
        {
            string tipo = "SIN LOGO";
            RemitoComprobanteMargenes result = new RemitoComprobanteMargenes();
            result.Horizontal = 0;
            result.Vertical = 0;
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                var dbContext = new ACHEEntities();

                RemitoComprobanteMargenes lpm = dbContext.RemitoComprobanteMargenes.Where(w => w.IdUsuario == usu.IDUsuario && w.Tipo.Equals(tipo)).FirstOrDefault();

                if (lpm == null)
                {
                    RemitoComprobanteMargenes nlpm = new RemitoComprobanteMargenes();
                    nlpm.IdUsuario = usu.IDUsuario;
                    nlpm.Horizontal = 0;
                    nlpm.Vertical = 0;
                    nlpm.Tipo = tipo;
                    nlpm.Fecha = DateTime.Now;
                    dbContext.RemitoComprobanteMargenes.Add(nlpm);
                    dbContext.SaveChanges();
                }
                else
                {
                    result.Horizontal = lpm.Horizontal;
                    result.Vertical = lpm.Vertical;
                }

                return result;
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
    public static RemitoComprobanteMargenes getMargenesRemitoTalonario()
    {
        try
        {
            string tipo = "TALONARIO";
            RemitoComprobanteMargenes result = new RemitoComprobanteMargenes();
            result.Horizontal = 0;
            result.Vertical = 0;
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                var dbContext = new ACHEEntities();

                RemitoComprobanteMargenes lpm = dbContext.RemitoComprobanteMargenes.Where(w => w.IdUsuario == usu.IDUsuario && w.Tipo.Equals(tipo)).FirstOrDefault();

                if (lpm == null)
                {
                    RemitoComprobanteMargenes nlpm = new RemitoComprobanteMargenes();
                    nlpm.IdUsuario = usu.IDUsuario;
                    nlpm.Horizontal = 0;
                    nlpm.Vertical = 0;
                    nlpm.Tipo = tipo;
                    nlpm.Fecha = DateTime.Now;
                    dbContext.RemitoComprobanteMargenes.Add(nlpm);
                    dbContext.SaveChanges();
                }
                else
                {
                    result.Horizontal = lpm.Horizontal;
                    result.Vertical = lpm.Vertical;
                }

                return result;
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
    public static void agregarItemPorCodigoYConcepto(int id, string idConcepto, string concepto, string iva, 
                                                    string precio, string bonif, string cantidad, int idPersona, string codigo, 
                                                    string idPlanDeCuenta, string codigoCta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (id != 0)
            {
                var aux = ComprobanteCart.Retrieve().Items.Where(x => x.Codigo == codigo && x.Concepto == concepto).FirstOrDefault();
                ComprobanteCart.Retrieve().Items.Remove(aux);
            }

            var tra = new ComprobantesDetalleViewModel();
            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
            tra.Concepto = concepto;
            tra.Codigo = codigo;
            tra.CodigoPlanCta = codigoCta;
            
            if (iva != string.Empty)
            {
                //tra.Iva = decimal.Parse(iva.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                tra.Iva = obtenerValorIVA(int.Parse(iva));
                tra.IdTipoIva = int.Parse(iva);
            }
            else
            {
                tra.Iva = 0;
                tra.IdTipoIva= 0;
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

            if (idPlanDeCuenta != "")
                tra.IDPlanDeCuenta = Convert.ToInt32(idPlanDeCuenta);
            
            tra.PrecioUnitario = ObtenerPrecioFinal(tra.PrecioUnitario, iva, idPersona);

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
    public static void aplicarDescuento(string descuento)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            ComprobanteCart.Retrieve().Descuento = decimal.Parse(descuento.Replace(SeparadorDeMiles, SeparadorDeDecimales));
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void modificarActividad(int idComprobante, int idActividad)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                var comp = dbContext.Comprobantes.Where(w => w.IDComprobante == idComprobante).FirstOrDefault();
                if (comp != null)
                { 
                    comp.IdActividad = idActividad;
                    dbContext.SaveChanges();
                }
                else
                    throw new Exception("El comprobante " + idComprobante.ToString() + " no existe");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static TotalesViewModel obtenerTotales(string percepcionesIIBB, string percepcionesIVA, string descuento)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            TotalesViewModel totales = new TotalesViewModel();
            
            ComprobanteCart.Retrieve().PercepcionIVA = Convert.ToDecimal(percepcionesIVA.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            ComprobanteCart.Retrieve().PercepcionIIBB = Convert.ToDecimal(percepcionesIIBB.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            ComprobanteCart.Retrieve().Descuento = Convert.ToDecimal(descuento.Replace(SeparadorDeMiles, SeparadorDeDecimales));


            if (usu.CUIT.Equals("30716909839"))
            {
                totales.Iva = (ComprobanteCart.Retrieve().GetIva() / 1000).ToString("N2");
                totales.Subtotal = (ComprobanteCart.Retrieve().GetSubTotal() / 1000).ToString("N2");
                totales.ImporteExento = (ComprobanteCart.Retrieve().GetImporteExento() / 1000).ToString("N2");
                totales.ImporteNoGravado = (ComprobanteCart.Retrieve().GetImporteNoGravado() / 1000).ToString("N2");
                totales.PercepcionIVA = (ComprobanteCart.Retrieve().GetPercepcionIVA() / 1000).ToString("N2");
                totales.PercepcionIIBB = (ComprobanteCart.Retrieve().GetPercepcionIIBB() / 1000).ToString("N2");
                totales.Descuento = (ComprobanteCart.Retrieve().GetDescuento() / 1000).ToString("N2");
                totales.Total = (ComprobanteCart.Retrieve().GetTotal() / 1000).ToString("N2");
            }
            else
            {
                totales.Iva = ComprobanteCart.Retrieve().GetIva().ToString("N2");
                totales.Subtotal = ComprobanteCart.Retrieve().GetSubTotal().ToString("N2");
                totales.ImporteExento = ComprobanteCart.Retrieve().GetImporteExento().ToString("N2");
                totales.ImporteNoGravado = ComprobanteCart.Retrieve().GetImporteNoGravado().ToString("N2");
                totales.PercepcionIVA = ComprobanteCart.Retrieve().GetPercepcionIVA().ToString("N2");
                totales.PercepcionIIBB = ComprobanteCart.Retrieve().GetPercepcionIIBB().ToString("N2");
                totales.Descuento = ComprobanteCart.Retrieve().GetDescuento().ToString("N2");                
                totales.Total = ComprobanteCart.Retrieve().GetTotal().ToString("N2");
            }

            return totales;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerItems(int idPersona, string tipo)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = ComprobanteCart.Retrieve().Items.OrderBy(x => x.ID).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + index + "</td>";


                        //if (!usu.CUIT.Equals("30654870868") && !usu.CUIT.Equals("20147963271") && !usu.CUIT.Equals("30716892286")) //FIOL
                        //{
                        //    if (tipo.Equals("EDA"))
                        //        html += "<td style='text-align:left'><input type='tel' class='form - control' maxlength='6' id='txtCantidad' value='" + Convert.ToInt32(detalle.Cantidad).ToString("") + "' /></td>";
                        //    else
                        //        html += "<td style='text-align:left'>" + Convert.ToInt32(detalle.Cantidad).ToString("") + "</td>";
                        //}                            
                        //else
                        //{
                        //    if (tipo.Equals("EDA"))
                        //        html += "<td style='text-align:left'><input type='tel' class='form - control' maxlength='6' id='txtCantidad' value='" + detalle.Cantidad.ToString("N2") + "' /></td>";
                        //    else
                        //        html += "<td style='text-align:left'>" + detalle.Cantidad.ToString("N2") + "</td>";
                        //}

                        if (usu.UsaCantidadConDecimales)
                        {
                            if (tipo.Equals("EDA"))
                                html += "<td style='text-align:left'><input type='tel' class='form - control' maxlength='6' id='txtCantidad' value='" + detalle.Cantidad.ToString("N2") + "' /></td>";
                            else
                                html += "<td style='text-align:left'>" + detalle.Cantidad.ToString("N2") + "</td>";
                        }
                        else
                        {
                            if (tipo.Equals("EDA"))
                                html += "<td style='text-align:left'><input type='tel' class='form - control' maxlength='6' id='txtCantidad' value='" + Convert.ToInt32(detalle.Cantidad).ToString("") + "' /></td>";
                            else
                                html += "<td style='text-align:left'>" + Convert.ToInt32(detalle.Cantidad).ToString("") + "</td>";
                        }

                        html += "<td style='text-align:left'>" + detalle.Codigo + "</td>";
                        if(detalle.IDConcepto != null)
                            html += "<td style='text-align:left'><input type='hidden' value='" + detalle.IDConcepto.ToString() + "' /><a href=\"javascript:verConcepto(" + detalle.IDConcepto.ToString() + ");\" style='cursor:pointer; font-size:16px' title='" + detalle.Concepto + "'>" + detalle.Concepto + "</a></td>";
                        else
                            html += "<td style='text-align:left'>" + detalle.Concepto + "</td>";
                        if (detalle.Concepto.ToUpper().Equals("COMISIÓN"))
                        {
                            decimal PrecioUnitario1000 = detalle.PrecioUnitario / 1000;
                            html += "<td style='text-align:right'>" + PrecioUnitario1000.ToString("N2") + "</td>";
                        }                            
                        else
                            html += "<td style='text-align:right'>" + detalle.PrecioUnitario.ToString("N2") + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Bonificacion.ToString("N2") + "</td>";
                        html += "<td style='text-align:right'>" + detalle.Iva.ToString("N2") + "</td>";
                        html += "<td style='text-align:right' hidden='hidden'>" + detalle.IdTipoIva.ToString() + "</td>";
                        html += "<td class='divPlanDeCuentas' style='text-align:left'>" + detalle.CodigoPlanCta + "</td>";

                        if(detalle.Ajuste)
                        {
                            if (usu.CUIT.Equals("30716909839"))
                            {
                                decimal TotalConIva1000 = detalle.SubTotalAjustado / 1000;
                                html += "<td style='text-align:right'>" + TotalConIva1000.ToString("N2") + "</td>";
                            }
                            else
                                html += "<td style='text-align:right'>" + detalle.SubTotalAjustado.ToString("N2") + "</td>";
                        }
                        else
                        {
                            if (usu.CUIT.Equals("30716909839"))
                            {
                                decimal TotalConIva1000 = detalle.TotalConIva / 1000;
                                html += "<td style='text-align:right'>" + TotalConIva1000.ToString("N2") + "</td>";
                            }
                            else
                                html += "<td style='text-align:right'>" + detalle.TotalConIva.ToString("N2") + "</td>";
                        }

                        decimal preciFinal = 0;
                        //var UsaPrecioConIva = dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && (x.CondicionIva == "MO" || x.CondicionIva == "CF"));
                        //if (usu.UsaPrecioFinalConIVA || UsaPrecioConIva)
                        if (usu.UsaPrecioFinalConIVA)
                            preciFinal = Math.Round(decimal.Parse(detalle.PrecioUnitarioConIva.ToString()), 2);
                        else
                            preciFinal = Math.Round(decimal.Parse(detalle.PrecioUnitario.ToString()), 2);

                        if (!tipo.Equals("EDA"))
                        {
                            html += "<td><a title='Eliminar' style='font-size: 16px' href='javascript:eliminarItem(" + detalle.ID + ");'><i class='fa fa-times'></i></a>&nbsp;&nbsp;";
                            //html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarItem(" + detalle.ID + ", '" + (detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "") + "' ,'" + detalle.Cantidad + "','" + detalle.Concepto + "','" + preciFinal.ToString("").Replace(".", ",") + "','" + detalle.Iva.ToString() + "','" + detalle.Bonificacion.ToString("").Replace(".", ",") + "','" + detalle.IDPlanDeCuenta.ToString() + "');\"><i class='fa fa-edit'></i></a></td>";
                            //html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarItem(" + detalle.ID + ", '" + detalle.Codigo + "', '" + (detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "") + "' ,'" + detalle.Cantidad.ToString("N2").Replace(".", ",") + "','" + detalle.Concepto + "','" + preciFinal.ToString("").Replace(".", ",") + "','" + detalle.Iva.ToString("N2") + "','" + detalle.Bonificacion.ToString("N2").Replace(".", ",") + "','" + detalle.IDPlanDeCuenta.ToString() + "');\"><i class='fa fa-edit'></i></a></td>";
                            html += "<a title='Modificar' style='font-size: 16px' href=\"javascript:modificarItem(" + detalle.ID + ", '" + detalle.Codigo + "', '" + (detalle.IDConcepto.HasValue ? detalle.IDConcepto.Value.ToString() : "") + "' ,'" + detalle.Cantidad.ToString("N2").Replace(".", ",") + "','" + detalle.Concepto + "','" + preciFinal.ToString("").Replace(".", ",") + "','" + detalle.IdTipoIva.ToString() + "','" + detalle.Bonificacion.ToString("N2").Replace(".", ",") + "','" + detalle.IDPlanDeCuenta.ToString() + "');\"><i class='fa fa-edit'></i></a>&nbsp;&nbsp;";
                            html += "<a title='Subir' style='font-size: 16px' href=\"javascript:moverItem(" + detalle.ID + ",'" + detalle.Concepto + "','subir');\"><i class='fa fa-arrow-up'></i></a>&nbsp;&nbsp;";
                            html += "<a title='Bajar' style='font-size: 16px' href=\"javascript:moverItem(" + detalle.ID + ",'" + detalle.Concepto + "','bajar');\"><i class='fa fa-arrow-down'></i></a></td>";
                        }
                        else
                            html += "<td></td>";


                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='9' style='text-align:center'>No tienes items agregados</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static void CargarDatosDelComprobanteAsociado(int idComprobante)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        ComprobantesVinculadosCart.Retrieve().Items.Clear();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Comprobantes entity = dbContext.Comprobantes.Include("ComprobantesDetalle")
                        .Where(x => x.IDComprobante == idComprobante && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                ComprobanteCart.Retrieve().IDPuntoVenta = entity.IDPuntoVenta;

                var listaEda = dbContext.Comprobantes.Where(w => w.IdComprobanteVinculado == entity.IDComprobante && w.Tipo.Equals("EDA")).ToList();

                foreach (var det in entity.ComprobantesDetalle)
                {
                    var tra = new ComprobantesDetalleViewModel();
                    tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                    tra.Concepto = det.Concepto;
                    tra.Codigo = (det.Conceptos != null) ? det.Conceptos.Codigo : "";
                    tra.CodigoPlanCta = (det.PlanDeCuentas == null) ? "" : det.PlanDeCuentas.Codigo + " - " + det.PlanDeCuentas.Nombre;
                    tra.Iva = det.Iva;
                    tra.IdTipoIva = (int)det.IdTipoIVA;
                    tra.PrecioUnitario = det.PrecioUnitario;
                    tra.Bonificacion = det.Bonificacion;
                    tra.Cantidad = det.Cantidad;
                    foreach (Comprobantes cs in listaEda)
                    {
                        tra.Cantidad = tra.Cantidad - cs.ComprobantesDetalle.Where(w => w.IDConcepto == det.IDConcepto).Sum(s => s.Cantidad);
                    }
                    tra.IDConcepto = det.IDConcepto;
                    tra.IDPlanDeCuenta = det.IDPlanDeCuenta;
                    ComprobanteCart.Retrieve().Items.Add(tra);
                }
            }
        }

    }

    [WebMethod(true)]
    public static int obtenerPuntoDeVenta()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            return ComprobanteCart.Retrieve().IDPuntoVenta;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static bool sumatoriaDeFacturasDeUnPedido(int id)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        bool resultado = false;
        decimal sumaComVinc = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            List<Comprobantes> CompVinc = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id).ToList();
            if (CompVinc != null)
            {
                foreach (Comprobantes c in CompVinc)
                {
                    // busco si tiene una nota de credito
                    Comprobantes NC = dbContext.Comprobantes.Where(w => w.IdComprobanteAsociado == c.IDComprobante).FirstOrDefault();
                    if (NC == null)
                    {
                        sumaComVinc = sumaComVinc + c.ImporteTotalNeto;
                    }
                }
            }

            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (Comp != null)
            {
                if (Comp.ImporteTotalNeto < (sumaComVinc + Comp.ImporteTotalNeto) && sumaComVinc != 0)
                    resultado = true;

            }

            return resultado;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static int vincularComprobante(int id)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        int numeroComprobante = 0;
        decimal sumaComVinc = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            List<Comprobantes> CompVinc = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id).ToList();
            if (CompVinc != null)
            {
                foreach (Comprobantes c in CompVinc)
                {
                    // busco si tiene una nota de credito
                    Comprobantes NC = dbContext.Comprobantes.Where(w => w.IdComprobanteAsociado == c.IDComprobante).FirstOrDefault();
                    if (NC == null)
                    {
                        sumaComVinc = sumaComVinc + c.ImporteTotalNeto;
                    }                    
                }
            }            

            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (Comp != null)
            {
                //if (Comp.ImporteTotalNeto < (sumaComVinc + Comp.ImporteTotalNeto) && sumaComVinc != 0)
                //    throw new Exception("La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.");


                CobranzasDetalle cobranza = dbContext.CobranzasDetalle.Where(x => x.IDComprobante == id).FirstOrDefault();

                if (cobranza != null)
                    throw new Exception("Ya se generó una cobranza para este pedido de venta, cancelar la cobranza y volver a intentar.");

                List<ComprobantesDetalle> ListCompDet = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == id).ToList();

                foreach (ComprobantesDetalle cd in ListCompDet)
                {
                    Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                    if (con != null)
                    {
                        var tra = new ComprobantesDetalleViewModel();
                        tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                        tra.Concepto = cd.Concepto;
                        tra.Codigo = con.Codigo;
                        tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                        tra.Iva = cd.Iva;
                        tra.IdTipoIva = (int)cd.IdTipoIVA;
                        tra.Bonificacion = cd.Bonificacion;
                        tra.PrecioUnitario = cd.PrecioUnitario;
                        tra.Cantidad = cd.Cantidad;
                        tra.IDConcepto = cd.IDConcepto;
                        tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                        ComprobanteCart.Retrieve().Items.Add(tra);
                    }
                }

                Comprobantes compFact = dbContext.Comprobantes.Where(w => w.IdComprobanteVinculado == id && w.Tipo.Contains("F") && w.CAE == null).FirstOrDefault();
                if (compFact != null)
                {
                    throw new Exception("El PDV ya tiene una factura borrador vinculada.");
                }
                else
                {
                    int nro = 0;
                    int idJurisdiccion = Comp.IDJurisdiccion != null ? (int)Comp.IDJurisdiccion : 0;
                    nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("FCA", usu.IDUsuario, Convert.ToInt32(Comp.IDPuntoVenta)));
                    numeroComprobante = guardar(0, Comp.IDPersona, "FCA", "E", DateTime.Now.ToShortDateString(),
                            Comp.CondicionVenta, Comp.TipoConcepto, DateTime.Now.AddDays(30).ToShortDateString(),
                            Comp.IDPuntoVenta, nro.ToString(), Comp.Observaciones, idJurisdiccion, Comp.Nombre,
                            Comp.Vendedor, Comp.Envio, Comp.FechaEntrega.ToString(), 
                            Comp.FechaAlta.ToString(), 0, 0, 0, 0, 0, 0, "", 0, Comp.IdActividad, Comp.ModalidadPagoAFIP);

                    Comprobantes CompVinculado = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == numeroComprobante).FirstOrDefault();
                    if (CompVinculado != null)
                    {
                        CompVinculado.IdComprobanteVinculado = id;
                        Comp.Saldo = Comp.Saldo - CompVinculado.ImporteTotalNeto;
                        dbContext.SaveChanges();
                    }
                }
               
            }

            return numeroComprobante;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int vincularComprobanteDivididoGenerar(int id, int rangoFacturas, int idPuntoVentaFac1, int idPuntoVentaFac2)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        int numeroComprobante = 0;
        decimal sumaComVinc = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            List<Comprobantes> CompVinc = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id).ToList();
            if (CompVinc != null)
            {
                foreach (Comprobantes c in CompVinc)
                {
                    // busco si tiene una nota de credito
                    Comprobantes NC = dbContext.Comprobantes.Where(w => w.IdComprobanteAsociado == c.IDComprobante).FirstOrDefault();
                    if (NC == null)
                    {
                        sumaComVinc = sumaComVinc + c.ImporteTotalNeto;
                    }
                }
            }

            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (Comp != null)
            {
                //if (Comp.ImporteTotalNeto < (sumaComVinc + Comp.ImporteTotalNeto) && sumaComVinc != 0)
                //    throw new Exception("La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.");


                CobranzasDetalle cobranza = dbContext.CobranzasDetalle.Where(x => x.IDComprobante == id).FirstOrDefault();

                if (cobranza != null)
                    throw new Exception("Ya se generó una cobranza para este pedido de venta, cancelar la cobranza y volver a intentar.");


                Comprobantes compFact = dbContext.Comprobantes.Where(w => w.IdComprobanteVinculado == id && w.Tipo.Contains("F") && w.CAE == null).FirstOrDefault();
                if (compFact != null)
                {
                    throw new Exception("El PDV ya tiene una factura borrador vinculada.");
                }
                else
                {

                    List<ComprobantesDetalle> ListCompDet = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == id).ToList();

                    foreach (ComprobantesDetalle cd in ListCompDet)
                    {
                        Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                        if (con != null)
                        {
                            var tra = new ComprobantesDetalleViewModel();
                            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                            tra.Concepto = cd.Concepto;
                            tra.Codigo = con.Codigo;
                            tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                            tra.Iva = cd.Iva ;
                            tra.IdTipoIva = (int)cd.IdTipoIVA;
                            tra.Bonificacion = cd.Bonificacion;
                            tra.PrecioUnitario = cd.PrecioUnitario;
                            tra.Cantidad = ((cd.Cantidad * rangoFacturas) / 100);
                            tra.IDConcepto = cd.IDConcepto;
                            tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                            ComprobanteCart.Retrieve().Items.Add(tra);
                        }
                    }

                    int nro = 0;
                    int idJurisdiccion = Comp.IDJurisdiccion != null ? (int)Comp.IDJurisdiccion : 0;
                    nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("FCA", usu.IDUsuario, Convert.ToInt32(Comp.IDPuntoVenta)));
                    numeroComprobante = guardar(0, Comp.IDPersona, "FCA", "E", DateTime.Now.ToShortDateString(),
                            Comp.CondicionVenta, Comp.TipoConcepto, DateTime.Now.AddDays(30).ToShortDateString(),
                            Convert.ToInt32(idPuntoVentaFac1), nro.ToString(), Comp.Observaciones, idJurisdiccion, Comp.Nombre,
                            Comp.Vendedor, Comp.Envio, Comp.FechaEntrega.ToString(), 
                            Comp.FechaAlta.ToString(), 0, 0, 0, 0, 0, 0, "", 0, Comp.IdActividad, Comp.ModalidadPagoAFIP);

                    Comprobantes CompVinculado1 = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == numeroComprobante).FirstOrDefault();
                    if (CompVinculado1 != null)
                    {
                        CompVinculado1.IdComprobanteVinculado = id;
                        Comp.Saldo = Comp.Saldo - CompVinculado1.ImporteTotalNeto;
                        dbContext.SaveChanges();
                    }

                    ComprobanteCart.Retrieve().Items.Clear();

                    int restoRango = 100 - rangoFacturas;

                    foreach (ComprobantesDetalle cd in ListCompDet)
                    {
                        Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                        if (con != null)
                        {
                            var tra = new ComprobantesDetalleViewModel();
                            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                            tra.Concepto = cd.Concepto;
                            tra.Codigo = con.Codigo;
                            tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                            tra.Iva = cd.Iva;
                            tra.IdTipoIva = (int)cd.IdTipoIVA;
                            tra.Bonificacion = cd.Bonificacion;
                            tra.PrecioUnitario = cd.PrecioUnitario;
                            tra.Cantidad = ((cd.Cantidad * restoRango) / 100);
                            tra.IDConcepto = cd.IDConcepto;
                            tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                            ComprobanteCart.Retrieve().Items.Add(tra);
                        }
                    }

                    nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("FCA", usu.IDUsuario, Convert.ToInt32(Comp.IDPuntoVenta)));
                    numeroComprobante = guardar(0, Comp.IDPersona, "FCA", "E", DateTime.Now.ToShortDateString(),
                            Comp.CondicionVenta, Comp.TipoConcepto, DateTime.Now.AddDays(30).ToShortDateString(),
                            Convert.ToInt32(idPuntoVentaFac2), nro.ToString(), Comp.Observaciones, idJurisdiccion, Comp.Nombre,
                            Comp.Vendedor, Comp.Envio, Comp.FechaEntrega.ToString(), 
                            Comp.FechaAlta.ToString(), 0, 0, 0, 0, 0, 0, "", 0, Comp.IdActividad, Comp.ModalidadPagoAFIP);

                    Comprobantes CompVinculado2 = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == numeroComprobante).FirstOrDefault();
                    if (CompVinculado2 != null)
                    {
                        CompVinculado2.IdComprobanteVinculado = id;
                        Comp.Saldo = Comp.Saldo - CompVinculado2.ImporteTotalNeto;
                        dbContext.SaveChanges();
                    }


                }

            }

            return numeroComprobante;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int vincularComprobanteEdaFac(int id, int idComprobanteOrigen)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        int numeroComprobante = 0;
        decimal sumaComVinc = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            List<Comprobantes> CompVinc = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == idComprobanteOrigen).ToList();
            if (CompVinc != null)
            {
                foreach (Comprobantes c in CompVinc)
                {
                    // busco si tiene una nota de credito
                    Comprobantes NC = dbContext.Comprobantes.Where(w => w.IdComprobanteAsociado == c.IDComprobante).FirstOrDefault();
                    if (NC == null)
                    {
                        sumaComVinc = sumaComVinc + c.ImporteTotalNeto;
                    }
                }
            }

            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (Comp != null)
            {
                //if (Comp.ImporteTotalNeto < (sumaComVinc + Comp.ImporteTotalNeto) && sumaComVinc != 0)
                //    throw new Exception("La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.");


                CobranzasDetalle cobranza = dbContext.CobranzasDetalle.Where(x => x.IDComprobante == idComprobanteOrigen).FirstOrDefault();

                if (cobranza != null)
                    throw new Exception("Ya se generó una cobranza para este pedido de venta, cancelar la cobranza y volver a intentar.");

                List<ComprobantesDetalle> ListCompDet = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == id).ToList();

                foreach (ComprobantesDetalle cd in ListCompDet)
                {
                    Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                    if (con != null)
                    {
                        var tra = new ComprobantesDetalleViewModel();
                        tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                        tra.Concepto = cd.Concepto;
                        tra.Codigo = con.Codigo;
                        tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                        tra.Iva = cd.Iva;
                        tra.IdTipoIva = (int)cd.IdTipoIVA;
                        tra.Bonificacion = cd.Bonificacion;
                        tra.PrecioUnitario = cd.PrecioUnitario;
                        tra.Cantidad = cd.Cantidad;
                        tra.IDConcepto = cd.IDConcepto;
                        tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                        ComprobanteCart.Retrieve().Items.Add(tra);
                    }
                }

                Comprobantes compFact = dbContext.Comprobantes.Where(w => w.IdComprobanteVinculado == idComprobanteOrigen && w.Tipo.Contains("F") && w.CAE == null).FirstOrDefault();
                if (compFact != null)
                {
                    throw new Exception("El PDV ya tiene una factura borrador vinculada.");
                }
                else
                {
                    int nro = 0;
                    int idJurisdiccion = Comp.IDJurisdiccion != null ? (int)Comp.IDJurisdiccion : 0;
                    nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("FCA", usu.IDUsuario, Convert.ToInt32(Comp.IDPuntoVenta)));
                    numeroComprobante = guardar(0, Comp.IDPersona, "FCA", "E", DateTime.Now.ToShortDateString(),
                            Comp.CondicionVenta, Comp.TipoConcepto, DateTime.Now.AddDays(30).ToShortDateString(),
                            Comp.IDPuntoVenta, nro.ToString(), Comp.Observaciones, idJurisdiccion, Comp.Nombre,
                            Comp.Vendedor, Comp.Envio, Comp.FechaEntrega.ToString(), 
                            Comp.FechaAlta.ToString(), 0, 0, 0, 0, 0, 0, "", 0, Comp.IdActividad, Comp.ModalidadPagoAFIP);

                    Comprobantes CompVinculado = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == numeroComprobante).FirstOrDefault();
                    if (CompVinculado != null)
                    {
                        CompVinculado.IdComprobanteVinculado = id;
                        Comp.Saldo = Comp.Saldo - CompVinculado.ImporteTotalNeto;
                        dbContext.SaveChanges();
                    }
                }

            }

            return numeroComprobante;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static void desvincularComprobante(int id)
    {
        ComprobanteCart.Retrieve().Items.Clear();
        int numeroComprobante = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];


            List<Comprobantes> CompVinc = dbContext.Comprobantes.Where(w => w.IdComprobanteVinculado == id && w.Tipo.Equals("NDP")).ToList();
            if (CompVinc != null)
            {
                foreach (Comprobantes Comp in CompVinc)
                {
                    List<ComprobantesDetalle> ListCompDet = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == Comp.IDComprobante).ToList();

                    foreach (ComprobantesDetalle cd in ListCompDet)
                    {
                        Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                        if (con != null)
                        {
                            var tra = new ComprobantesDetalleViewModel();
                            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                            tra.Concepto = cd.Concepto;
                            tra.Codigo = con.Codigo;
                            tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                            tra.Iva = cd.Iva;
                            tra.IdTipoIva = (int)cd.IdTipoIVA;
                            tra.Bonificacion = cd.Bonificacion;
                            tra.PrecioUnitario = cd.PrecioUnitario;
                            tra.Cantidad = cd.Cantidad;
                            tra.IDConcepto = cd.IDConcepto;
                            tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                            ComprobanteCart.Retrieve().Items.Add(tra);
                        }
                    }

                    int nro = 0;
                    int idJurisdiccion = Comp.IDJurisdiccion != null ? (int)Comp.IDJurisdiccion : 0;
                    nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("PDV", usu.IDUsuario, Convert.ToInt32(Comp.IDPuntoVenta)));
                    numeroComprobante = guardar(0, Comp.IDPersona, "PDV", "T", DateTime.Now.ToShortDateString(),
                            Comp.CondicionVenta, Comp.TipoConcepto, DateTime.Now.AddDays(30).ToShortDateString(),
                            Comp.IDPuntoVenta, nro.ToString(), Comp.Observaciones, idJurisdiccion, Comp.Nombre, 
                            Comp.Vendedor, Comp.Envio, Comp.FechaEntrega.ToString(), 
                            Comp.FechaAlta.ToString(), 0, 0, 0, 0, 0, 0, "", 0, Comp.IdActividad, Comp.ModalidadPagoAFIP);                        
                    
                }
            }
            else
                throw new Exception("La factura seleccionada no contiene comprobantes vinculados.");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static int actualizarPrecios(int id)
    {
        int numeroComprobante = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var dbContext = new ACHEEntities();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();
            if (Comp != null)
            {
                List<ComprobantesDetalle> ListCompDet = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == id).ToList();

                foreach (ComprobantesDetalle cd in ListCompDet)
                {
                    Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                    if (con != null)
                    {

                        var aux = ComprobanteCart.Retrieve().Items.Where(x => x.IDConcepto == con.IDConcepto).FirstOrDefault();

                        if (aux != null)
                        {
                            ComprobanteCart.Retrieve().Items.Remove(aux);

                            var tra = new ComprobantesDetalleViewModel();
                            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                            tra.Concepto = cd.Concepto;
                            tra.Codigo = con.Codigo;
                            tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                            tra.Iva = cd.Iva;
                            tra.IdTipoIva = (int)cd.IdTipoIVA;
                            tra.Bonificacion = cd.Bonificacion;
                            tra.PrecioUnitario = con.PrecioUnitario;
                            tra.Cantidad = cd.Cantidad;
                            tra.IDConcepto = cd.IDConcepto;
                            tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                            ComprobanteCart.Retrieve().Items.Add(tra);

                        }

                    }
                }
              
                dbContext.SaveChanges();

                int idJurisdiccion = Comp.IDJurisdiccion != null ? (int)Comp.IDJurisdiccion : 0;
                string vendedor = Comp.Vendedor != null ? Comp.Vendedor : "";
                string envio = Comp.Envio != null ? Comp.Envio : "";
                numeroComprobante = guardar(Comp.IDComprobante, Comp.IDPersona, Comp.Tipo, Comp.Modo, DateTime.Now.ToShortDateString(),
                       Comp.CondicionVenta, Comp.TipoConcepto, DateTime.Now.AddDays(30).ToShortDateString(),
                       Comp.IDPuntoVenta, Comp.Numero.ToString(), Comp.Observaciones, idJurisdiccion, Comp.Nombre, 
                       vendedor, envio, Comp.FechaEntrega.ToString(), 
                       Comp.FechaAlta.ToString(), 0, 0, 0, 0, 0, 0, "", 0, Comp.IdActividad, Comp.ModalidadPagoAFIP);

            }

            return numeroComprobante;

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int guardar(int id, int idPersona, string tipo, string modo, string fecha, 
        string condicionVenta, int tipoConcepto, string fechaVencimiento, int idPuntoVenta, 
        string nroComprobante, string obs, int idJurisdiccion, string nombre, string vendedor, 
        string envio, string fechaEntrega, string fechaAlta, int idComprobanteAsociado, 
        int idComprobanteOrigen, int idDomicilio, int idTransporte, int idTransportePersona, 
        int idVendedorComision, string estado, int idCompraVinculada, int idActividad, string modalidadPagoAfip)
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
                compCart.IDUsuarioAdicional = idVendedorComision;
                compCart.Modo = modo;
                compCart.FechaComprobante = Convert.ToDateTime(fecha);

                if (!string.IsNullOrEmpty(fechaAlta) && tipo.Equals("PDV"))
                    compCart.FechaComprobante = Convert.ToDateTime(fechaAlta);

                compCart.FechaVencimiento = Convert.ToDateTime(fechaVencimiento);
                compCart.IDPuntoVenta = idPuntoVenta;
                compCart.Numero = nroComprobante;
                compCart.Observaciones = obs;
                compCart.CondicionVenta = condicionVenta;
                compCart.IDJuresdiccion = idJurisdiccion;

                compCart.Items = new List<ComprobantesDetalleViewModel>();
                compCart.ImporteNoGravado = ComprobanteCart.Retrieve().GetImporteNoGravado();
                compCart.ImporteExento = ComprobanteCart.Retrieve().GetImporteExento();
                compCart.PercepcionIVA = ComprobanteCart.Retrieve().PercepcionIVA;
                compCart.PercepcionIIBB = ComprobanteCart.Retrieve().PercepcionIIBB;
                compCart.Descuento = ComprobanteCart.Retrieve().Descuento;

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
                compCart.Envio = envio.Trim().ToUpper();
                if (!string.IsNullOrEmpty(fechaEntrega))
                    compCart.FechaEntrega = Convert.ToDateTime(fechaEntrega);
                else
                    compCart.FechaEntrega = DateTime.Now;

                compCart.IDComprobanteAsociado = idComprobanteAsociado;
                compCart.IDDomicilio = idDomicilio;
                compCart.IDTransporte = idTransporte;
                compCart.IDTransportePersona = idTransportePersona;
                compCart.IDCompraVinculada = idCompraVinculada;
                compCart.Estado = estado.Trim();
                compCart.IDActividad = idActividad;
                compCart.ModalidadPagoAfip = modalidadPagoAfip;
                

                var entity = ComprobantesCommon.Guardar(dbContext, compCart);

                var list = ComprobantesVinculadosCart.Retrieve().Items.ToList();
                if(list != null)
                {
                    foreach (int v in list)
                    {
                        Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == v).FirstOrDefault();
                        if (Comp != null)
                        {
                            Comp.IdComprobanteVinculado = entity.IDComprobante;
                            dbContext.SaveChanges();
                        }
                    }
                }     
                
                if(compCart.TipoComprobante.Equals("EDA"))
                {
                    Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == entity.IDComprobante).FirstOrDefault();
                    if (Comp != null)
                    {
                        Comp.IdComprobanteVinculado = idComprobanteOrigen;
                        dbContext.SaveChanges();
                    }
                }

                if (entity.Modo != "E")
                    AlertaStock(dbContext, entity.IDComprobante);

                bool existeRepetidos = dbContext.Comprobantes
                                            .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo == "PDV")
                                            .GroupBy(g => new { g.IDUsuario, g.IDPuntoVenta, g.Numero }).Any();

                if (existeRepetidos)
                {
                    BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), "Se generaron comprobantes de PDV repetidos.", " # comprobantese.aspx # - " + DateTime.Now.ToString() + " - " + nroComprobante);
                }


                return entity.IDComprobante;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void EnviarComprobanteAutomaticamente(int idComprobante)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            if (usu.EnvioAutomaticoComprobante == true)
            {
                using (var dbContext = new ACHEEntities())
                {
                    try
                    {
                        var plandeCuenta = PermisosModulosCommon.ObtenerPlanActual(dbContext, usu.IDUsuario);
                        if (plandeCuenta.IDPlan >= 3)//Plan Pyme
                        {
                            //TODO: PASAR A NEGOCIO
                            bool CompEnviado = dbContext.ComprobantesEnviados.Any(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == idComprobante && x.Resultado == true);
                            if (!CompEnviado)
                            {
                                var avisos = dbContext.AvisosVencimiento.Where(x => x.IDUsuario == usu.IDUsuario && x.TipoAlerta == "Envio FE").FirstOrDefault();
                                if (avisos != null)
                                {
                                    var c = dbContext.Comprobantes.Include("PuntosDeVenta").Where(x => x.IDComprobante == idComprobante && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                                    if (c.Personas.Email != null && c.Personas.Email != "")
                                    {
                                        var mensaje = avisos.Mensaje.ReplaceAll("\n", "<br/>");
                                        var file = c.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + c.Tipo + "-" + c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000") + ".pdf";
                                        Common.EnviarComprobantePorMail(c.Personas.RazonSocial, c.Personas.Email, avisos.Asunto, mensaje, file);
                                        ComprobantesCommon.GuardarComprobantesEnviados(dbContext, idComprobante, null, "Comprobante enviado correctamente", true, usu);
                                    }
                                    else
                                        throw new Exception("El cliente no tiene configurado el correo electrónico");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ComprobantesCommon.GuardarComprobantesEnviados(dbContext, idComprobante, null, "Comprobante: " + ex.Message, false, usu);
                        throw new Exception("No se pudo enviar automaticamente la factura electrónonica");
                    }
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string previsualizar(int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
        int tipoConcepto, string fechaVencimiento, int idPuntoVenta, string nroComprobante, string obs, int idComprobanteAsociado, string fechaEntrega,
        string notaDeCreditoPorServicio, int idActividad, string modalidadPagoAfip
        )
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            Common.CrearComprobante(usu, id, idPersona, tipo, modo, fecha, condicionVenta, 
                tipoConcepto, fechaVencimiento, idPuntoVenta, ref nroComprobante, obs, 
                idComprobanteAsociado, "", fechaEntrega, (notaDeCreditoPorServicio == "1" ? true : false), idActividad, modalidadPagoAfip, Common.ComprobanteModo.Previsualizar);
            return usu.IDUsuario + "_prev.pdf";
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string previsualizarTicket(int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
    int tipoConcepto, string fechaVencimiento, int idPuntoVenta, string nroComprobante, string obs)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            Common.CrearComprobanteTicket(usu, id, idPersona, tipo, modo, fecha, condicionVenta, tipoConcepto, fechaVencimiento, idPuntoVenta, ref nroComprobante, obs, Common.ComprobanteModo.Previsualizar);
            return usu.IDUsuario + "_prevTicket.pdf";
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static ComprobantesDescargasViewModel generarCae(int id, int idPersona, string tipo, string modo, string fecha, string condicionVenta,
        int tipoConcepto, string fechaVencimiento, int idPuntoVenta, string nroComprobante, string obs, int idComprobanteAsociado, 
        string fechaEntrega, string notaDeCreditoPorServicio, int idActividad, string modalidadPagoAfip)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var archivos = new ComprobantesDescargasViewModel();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            Common.CrearComprobante(usu, id, idPersona, tipo, modo, fecha, condicionVenta, 
                tipoConcepto, fechaVencimiento, idPuntoVenta, ref nroComprobante, obs, 
                idComprobanteAsociado, "", fechaEntrega, (notaDeCreditoPorServicio == "1" ? true : false), idActividad, modalidadPagoAfip , Common.ComprobanteModo.Generar);

            using (var dbContext = new ACHEEntities())
            {
                var persona = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona).FirstOrDefault();

                archivos.Comprobante = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + tipo + "-" + nroComprobante + ".pdf";
                archivos.Remito = persona.RazonSocial.RemoverCaracteresParaPDF() + "_" + "R-" + nroComprobante + ".pdf";

                if (modo == "E")
                    AlertaStock(dbContext, id);
            }
            return archivos;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static ComprobantesEditViewModel obtenerDatos(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Comprobantes entity = dbContext.Comprobantes.Include("ComprobantesDetalle")
                    .Where(x => x.IDComprobante == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    ComprobantesEditViewModel result = new ComprobantesEditViewModel();
                    result.ID = id;
                    result.IDPersona = entity.IDPersona;
                    result.Fecha = entity.FechaComprobante.ToString("dd/MM/yyyy");
                    result.FechaVencimiento = entity.FechaVencimiento.ToString("dd/MM/yyyy");
                    result.Modo = entity.Modo;
                    result.Tipo = entity.Tipo;
                    result.Numero = entity.Numero.ToString("#00000000");
                    result.TipoConcepto = entity.TipoConcepto;
                    result.CondicionVenta = entity.CondicionVenta;
                    result.IDPuntoVenta = entity.IDPuntoVenta;
                    result.Observaciones = entity.Observaciones;
                    result.Nombre = entity.Nombre;
                    result.Vendedor = entity.Vendedor;
                    result.Envio = entity.Envio;
                    result.FechaEntrega = entity.FechaEntrega != null ? ((DateTime)entity.FechaEntrega).ToString("dd/MM/yyyy") : "";
                    result.FechaAlta = entity.FechaComprobante != null ? ((DateTime)entity.FechaComprobante).ToString("dd/MM/yyyy") : "";
                    result.IDDomicilio = entity.IdDomicilio != null ? (int)entity.IdDomicilio : 0;
                    result.IDTransporte = entity.IdTransporte != null ? (int)entity.IdTransporte : 0;
                    result.IDTransportePersona = entity.IdTransportePersona != null ? (int)entity.IdTransportePersona : 0;
                    result.IDUsuarioAdicional = entity.IdUsuarioAdicional != null ? (int)entity.IdUsuarioAdicional : 0;
                    result.IDCompraVinculada = entity.IdCompraVinculada != null ? (int)entity.IdCompraVinculada : 0;
                    result.Estado = entity.Estado != null ? entity.Estado : "";
                    result.Descuento = entity.Descuento.ToString().Replace(",", ".");
                    result.IDActividad = entity.IdActividad;
                    result.ModalidadPagoAFIP = entity.ModalidadPagoAFIP;

                    result.IDComprobanteAsociado = entity.IdComprobanteAsociado != null ? (int)entity.IdComprobanteAsociado : 0;                    
                    if (entity.IdComprobanteAsociado != null)
                    {
                        Comprobantes comAso = dbContext.Comprobantes.Where(w => w.IDComprobante == entity.IdComprobanteAsociado).FirstOrDefault();

                        if(comAso != null)
                        {
                            string numeroComprobanteAsociado = comAso.Tipo + " " + comAso.PuntosDeVenta.Punto.ToString("#0000") + "-" + comAso.Numero.ToString("#00000000");
                            result.NumeroComprobanteAsociado = numeroComprobanteAsociado;
                        }

                    }

                    result.IDComprobanteVinculado = entity.IdComprobanteVinculado != null ? (int)entity.IdComprobanteVinculado : 0;
                    if (entity.IdComprobanteVinculado != null)
                    {
                        Comprobantes comAso = dbContext.Comprobantes.Where(w => w.IDComprobante == entity.IdComprobanteVinculado).FirstOrDefault();

                        if (comAso != null)
                        {
                            string numeroComprobanteVinculado = comAso.Tipo + " " + comAso.PuntosDeVenta.Punto.ToString("#0000") + "-" + comAso.Numero.ToString("#00000000");
                            result.NumeroComprobanteVinculado = numeroComprobanteVinculado;
                        }

                    }

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

                    if ((bool)usu.AgentePercepcionIIBB)
                    {
                        PersonasEdit.PercepcionIIBB = entity.PercepcionIIBB.ToString().Replace(",", ".");
                        PersonasEdit.IDJuresdiccion = Convert.ToInt32(entity.IDJurisdiccion);
                    }
                    if ((bool)usu.AgentePercepcionIVA)
                        PersonasEdit.PercepcionIVA = entity.PercepcionIVA.ToString().Replace(",", ".");

                    PersonasEdit.ImporteNoGravado = entity.ImporteNoGravado.ToString();
                    result.Personas = PersonasEdit;

                    foreach (var det in entity.ComprobantesDetalle)
                    {
                        var tra = new ComprobantesDetalleViewModel();
                        tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                        tra.Concepto = det.Concepto;
                        tra.Codigo = (det.Conceptos != null) ? det.Conceptos.Codigo : "";
                        tra.CodigoPlanCta = (det.PlanDeCuentas == null) ? "" : det.PlanDeCuentas.Codigo + " - " + det.PlanDeCuentas.Nombre;
                        tra.Iva = det.Iva;
                        tra.IdTipoIva = (int)det.IdTipoIVA;
                        tra.PrecioUnitario = det.PrecioUnitario;
                        tra.Bonificacion = det.Bonificacion;
                        tra.Cantidad = det.Cantidad;
                        tra.IDConcepto = det.IDConcepto;
                        tra.IDPlanDeCuenta = det.IDPlanDeCuenta;
                        ComprobanteCart.Retrieve().Items.Add(tra);
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

    [WebMethod(true)]
    public static void GenerarAsientosContables(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            try
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                ContabilidadCommon.AgregarAsientoDeVentas(usu, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    [WebMethod(true)]
    public static ArchivoAdjuntoComp descargarAdjunto(string id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (id != null)
                {
                    var dbContext = new ACHEEntities();
                    int idComprobante = Convert.ToInt32(id);
                    Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == idComprobante).FirstOrDefault();
                    if (c != null)
                    {
                        ArchivoAdjuntoComp d = new ArchivoAdjuntoComp();
                        d.NombreArchivo = "ADJUNTO" + c.IDComprobante.ToString().PadLeft(8,'0') + ".PDF";
                        d.Contenido = c.Adjunto;
                        return d;
                    }
                    else
                        throw new Exception("El comprobante no posee archivo adjunto");
                }
                else
                    throw new Exception("Error al descargar el archivo");
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # comprobantese.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception("Error al descargar el archivo");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public class ArchivoAdjuntoComp
    {
        public string NombreArchivo { get; set; }
        public byte[] Contenido { get; set; }
    }

}

