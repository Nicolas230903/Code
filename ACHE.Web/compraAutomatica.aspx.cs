using ACHE.Extensions;
using ACHE.FacturaElectronica;
using ACHE.Model;
using ACHE.Model.Negocio;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Productos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class compraAutomatica : BasePage
{
    public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ".";//"."
    public const string SeparadorDeDecimales = ",";//","

    protected void Page_Load(object sender, EventArgs e)
    {
        using (var dbContext = new ACHEEntities())
        {
            AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

            if (afu != null)
                if (!afu.HerramientasGeneracionCompraAutomatica)
                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

        }
    }


    [WebMethod(true)]
    public static int obtenerCantidadDeProveedoresDeLaSeleccion(string[] id)
    {
        int cantidadProveedores = 0;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                List<int?> listaComprobantes = new List<int?>();

                if (id != null)
                {
                    foreach (string item in id)
                    {
                        if (item != null)
                        {
                            int idComprobante = Convert.ToInt32(item.ToString());
                            listaComprobantes.Add(idComprobante);
                        }
                    }
                }

                List<int?> listaProveedores = dbContext.vPdvPendientesDeProcesarDetalle.
                        Where(x => x.IdUsuario == usu.IDUsuario && listaComprobantes.Contains(x.IDComprobante))
                        .Select(s => s.IdPersonaConcepto)
                        .Distinct()
                        .ToList();


                cantidadProveedores = listaProveedores.Count();

            }
            
           
        }
        return cantidadProveedores;
    }



    [WebMethod(true)]
    public static string obtenerPedidosDeVentaPendientesDeProcesar(int idProveedor)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                List<vPdvPendientesDeProcesar> list = new List<vPdvPendientesDeProcesar>();

                if (idProveedor != 0)
                {
                    List<int> listaDetComp = dbContext.vPdvPendientesDeProcesarDetalle.
                            Where(x => x.IdUsuario == usu.IDUsuario && x.IdPersonaConcepto == idProveedor)
                            .Select(s => s.IDComprobante)
                            .Distinct()
                            .ToList();
                    list = dbContext.vPdvPendientesDeProcesar.Where(x => x.IdUsuario == usu.IDUsuario && listaDetComp.Contains(x.IDComprobante)).ToList();
                }
                else
                {
                    list = dbContext.vPdvPendientesDeProcesar.Where(x => x.IdUsuario == usu.IDUsuario).ToList();
                }                

                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        bool tieneItemsEnMasDeUnPedidoDeVenta = itemsEnMasDeUnPedidoDeVenta(detalle.IDComprobante, 15, usu);

                        html += "<tr>";
                        if (tieneItemsEnMasDeUnPedidoDeVenta)                        
                            html += "<td></td>";
                        else
                            if (detalle.Estado.Equals("Pendiente"))
                                html += "<td><input type='checkbox' name='pedidosSeleccionados' class='checkbox chkTodos' value='" + detalle.IDComprobante.ToString() + "' /></td>";
                            else
                                html += "<td></td>";
                        html += "<td>" + detalle.IDComprobante.ToString() + "</td>";
                        html += "<td>" + (detalle.NombreFantansia == "" ? detalle.RazonSocial.ToUpper() :detalle.NombreFantansia.ToUpper()) + "</td>";
                        html += "<td>" + detalle.FechaComprobante.ToString(formatoFecha) + "</td>";
                        html += "<td>" + detalle.Nombre + " </td>";
                        html += "<td>" + detalle.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000") + " </td>";
                        html += "<td>" + detalle.ImporteTotalBruto.ToString("N2") + " </td>";
                        html += "<td>" + detalle.ImporteTotalNeto.ToString("N2") + " </td>";
                        if (tieneItemsEnMasDeUnPedidoDeVenta)
                            html += "<td><span style='background-color:orange;font-weight: bold;color:white'>Ya existe el concepto en otro pedido</span></td>";
                        else
                            if (detalle.Estado.Equals("Pendiente"))
                                html += "<td><span style='background-color:yellow;font-weight: bold'>" + detalle.Estado + "</span></td>";
                            else
                                html += "<td><span style='background-color:red;font-weight: bold;color:white'>" + detalle.Estado + "</span></td>";
                        html += "<td><a onclick=\"editar(" + detalle.IDComprobante.ToString() + ",'PDV');\" style='cursor:pointer; font-size:16px' title='Editar'><i class='fa fa-pencil'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes pedidos de venta pendientes de procesar.</td></tr>";

        }
        return html;
    }

    public static bool itemsEnMasDeUnPedidoDeVenta(int idComprobante, int largo, WebUser usu)
    {
        var dbContext = new ACHEEntities();

        List<ComprobantesDetalle> cd = dbContext.ComprobantesDetalle.Where(w => w.IDComprobante == idComprobante).ToList();

        foreach (ComprobantesDetalle i in cd)
        {

            string busqueda = i.Concepto.Length < largo ? i.Concepto : i.Concepto.Substring(0, largo);

            var listaComprobantesDetalle = (from comDet in dbContext.ComprobantesDetalle
                                            join com in dbContext.Comprobantes on comDet.IDComprobante equals com.IDComprobante
                                            where com.IDUsuario == usu.IDUsuario
                                            && comDet.Concepto.Substring(0, comDet.Concepto.Length < largo ? comDet.Concepto.Length : largo) == busqueda
                                            && com.Tipo.Equals("PDV")
                                            select new
                                            {
                                                com.IDComprobante
                                            }).Distinct().ToList();

            if (listaComprobantesDetalle != null)            
                if (listaComprobantesDetalle.Count > 1)
                    return true;

        }

        return false;
    }



    [WebMethod(true)]
    public static string obtenerPedidosDeVentaProcesados(long procesoCompraAutomatica, int idPersona)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                List<vPdvProcesados> list = new List<vPdvProcesados>();

                if (procesoCompraAutomatica != 0 && idPersona != 0)
                {
                    list = dbContext.vPdvProcesados.Where(x => x.IdUsuario == usu.IDUsuario && x.ProcesoCompraAutomatica == procesoCompraAutomatica && x.IDPersona == idPersona).ToList();
                }
                else
                {
                    if (procesoCompraAutomatica != 0 && idPersona == 0)
                    {
                        list = dbContext.vPdvProcesados.Where(x => x.IdUsuario == usu.IDUsuario && x.ProcesoCompraAutomatica == procesoCompraAutomatica).ToList();
                    }
                    else
                    {
                        if (procesoCompraAutomatica == 0 && idPersona != 0)
                        {
                            list = dbContext.vPdvProcesados.Where(x => x.IdUsuario == usu.IDUsuario && x.IDPersona == idPersona).ToList();
                        }
                        else
                        {
                            list = dbContext.vPdvProcesados.Where(x => x.IdUsuario == usu.IDUsuario).ToList();
                        }
                    }
                }


                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";
                        html += "<td>" + detalle.IDComprobante.ToString() + "</td>";
                        html += "<td>" + (detalle.NombreFantansia == "" ? detalle.RazonSocial.ToUpper() : detalle.NombreFantansia.ToUpper()) + "</td>";
                        html += "<td>" + detalle.FechaComprobante.ToString(formatoFecha) + "</td>";
                        //html += "<td>" + detalle.Nombre + " </td>";
                        html += "<td>" + detalle.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000") + " </td>";
                        html += "<td>" + detalle.ImporteTotalBruto.ToString("N2") + " </td>";
                        html += "<td>" + detalle.ImporteTotalNeto.ToString("N2") + " </td>";
                        html += "<td><a onclick=\"eliminar(" + detalle.IDComprobante.ToString() + ",'" + detalle.Nombre + "');\" style='cursor:pointer; font-size:16px' class='delete-row' title='Eliminar'><i class='fa fa-trash-o'></i></a></td>";
                        html += "<td><a onclick=\"documentosVinculados(" + detalle.IDComprobante.ToString() + ",'" + detalle.Nombre + "');\" style='cursor:pointer; font-size:16px' title='Comprobantes Vinculados'><i class='fa fa-link'></i></a></td>";
                        html += "<td><a onclick=\"editar(" + detalle.IDComprobante.ToString() + ",'PDC');\" style='cursor:pointer; font-size:16px' title='Editar'><i class='fa fa-pencil'></i></a></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes pedidos procesados.</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosComprobantesViewModel getResultsPedidoDeVenta(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                return ComprobantesCommon.ObtenerComprobantesVinculadosAUnPedidoDeCompra(id, usu);
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


    [WebMethod(true)]
    public static long procesar(string[] id, string fechaEntrega)
    {
        int numeroComprobante = 0;
        DateTime fEntrega;
        long procesoCompraAutomatica = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"));

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (!DateTime.TryParse(fechaEntrega, out fEntrega))
                {
                    throw new CustomException("Fecha de entrega invalida.");
                }


                using (var dbContext = new ACHEEntities())
                {

                    var existeProcesoCompraAutomatica = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && w.ProcesoCompraAutomatica == procesoCompraAutomatica).Any();
                    if (existeProcesoCompraAutomatica)
                        throw new CustomException("Ya existe el codigo de proceso de compra automatica, aguardo un momento e intente nuevamente.");



                    List<int?> listaComprobantes = new List<int?>();

                    if (id != null)
                    {
                        foreach (string item in id)
                        {
                            if (item != null)
                            {
                                int idComprobante = Convert.ToInt32(item.ToString());
                                listaComprobantes.Add(idComprobante);
                            }
                        }
                    }

                    List<int?> listaProveedores = dbContext.vPdvPendientesDeProcesarDetalle.
                            Where(x => x.IdUsuario == usu.IDUsuario && listaComprobantes.Contains(x.IDComprobante))
                            .Select(s => s.IdPersonaConcepto)
                            .Distinct()
                            .ToList();


                    if (listaProveedores.Any()) //Recorro los proveedores dentro de los clientes
                    {
                        foreach (var prov in listaProveedores)
                        {
                            var listaDetComp = dbContext.vPdvPendientesDeProcesarDetalle.
                                                        Where(x => x.IdUsuario == usu.IDUsuario && x.IdPersonaConcepto == prov && listaComprobantes.Contains(x.IDComprobante))
                                                        .Select(s => s.IDDetalle)
                                                        .ToList();

                            int idComprobante = 0;
                            decimal sumaTotal = 0;
                            int idTipoIva = 0;
                            ComprobanteCart.Retrieve().Items.Clear();

                            Personas p = dbContext.Personas.Where(w => w.IDPersona == prov).FirstOrDefault();

                            if (p != null)
                            {
                                switch (p.CondicionIva)
                                {
                                    case "RI":
                                        idTipoIva = 5;
                                        break;
                                    default:
                                        idTipoIva = 3;
                                        break;
                                }
                            }

                            if (listaDetComp != null) // Recorro los items de un proveedor de un cliente
                            {
                                foreach (var detCom in listaDetComp)
                                {
                                    ComprobantesDetalle cd = dbContext.ComprobantesDetalle.Where(x => x.IDDetalle == detCom).FirstOrDefault();

                                    if (cd != null)
                                    {
                                        Conceptos con = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDConcepto == cd.IDConcepto).FirstOrDefault();
                                        if (con != null)
                                        {
                                            var tra = new ComprobantesDetalleViewModel();
                                            tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                                            tra.Concepto = cd.Concepto;
                                            tra.Codigo = con.Codigo;
                                            tra.CodigoPlanCta = cd.IDPlanDeCuenta.ToString();
                                            tra.Iva = obtenerValorIVA(int.Parse(idTipoIva.ToString())); ;
                                            tra.IdTipoIva = idTipoIva;
                                            tra.Bonificacion = cd.Bonificacion;
                                            tra.PrecioUnitario = cd.PrecioUnitario;
                                            sumaTotal += (cd.PrecioUnitario * cd.Cantidad);
                                            tra.Cantidad = cd.Cantidad;
                                            tra.IDConcepto = cd.IDConcepto;
                                            tra.IDPlanDeCuenta = cd.IDPlanDeCuenta;
                                            idComprobante = cd.IDComprobante;
                                            ComprobanteCart.Retrieve().Items.Add(tra);
                                        }
                                    }
                                }
                            }

                            decimal comision = sumaTotal * usu.PorcentajeCompra;
                            decimal comisionTotal = comision / 100;

                            Conceptos conComision;
                            int idConcepto;

                            conComision = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario
                                                                                && x.IDPersona == prov
                                                                                && x.Nombre == "Comisión").FirstOrDefault();


                            if (conComision == null)
                            {
                                idConcepto = ConceptosCommon.GuardarConcepto(0, "Comisión", "", "P", "", "A",
                                        comisionTotal.ToString(), "3", "0", "", comisionTotal.ToString(),
                                        "0", (int)prov, usu.IDUsuario);

                                conComision = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario
                                                    && x.IDConcepto == idConcepto).FirstOrDefault();
                            }


                            var traCom = new ComprobantesDetalleViewModel();
                            traCom.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                            traCom.Concepto = conComision.Nombre;
                            traCom.Codigo = conComision.Codigo;
                            traCom.Iva = obtenerValorIVA(int.Parse(idTipoIva.ToString()));
                            traCom.IdTipoIva = idTipoIva;
                            traCom.Bonificacion = 0;
                            //decimal importeTemp = IVA / 100;
                            //decimal importeUnitarioTemp = comisionTotal * importeTemp;
                            //decimal importeUnitario = comisionTotal - importeUnitarioTemp;
                            traCom.PrecioUnitario = -comisionTotal;
                            traCom.Cantidad = 1;
                            traCom.IDConcepto = conComision.IDConcepto;
                            traCom.IDPlanDeCuenta = 0;

                            ComprobanteCart.Retrieve().Items.Add(traCom);

                            Comprobantes Comp = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == idComprobante).FirstOrDefault();
                            if (Comp != null)
                            {

                                // Genero Pedido de compra

                                int nro = 0;
                                int idJurisdiccion = 0;
                                nro = Convert.ToInt32(ComprobantesCommon.ObtenerProxNroComprobante("PDC", usu.IDUsuario, Convert.ToInt32(Comp.IDPuntoVenta)));

                                ComprobanteCartDto compCart = new ComprobanteCartDto();
                                compCart.IDComprobante = 0;
                                compCart.IDPersona = (int)prov;
                                compCart.TipoComprobante = "PDC";
                                compCart.TipoConcepto = Comp.TipoConcepto.ToString();
                                compCart.IDUsuario = usu.IDUsuario;
                                compCart.Modo = "T";
                                compCart.FechaComprobante = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                                compCart.FechaVencimiento = Convert.ToDateTime(DateTime.Now.AddDays(30).ToShortDateString());
                                compCart.IDPuntoVenta = Comp.IDPuntoVenta;
                                compCart.Numero = nro.ToString();
                                compCart.Observaciones = Comp.Observaciones;
                                compCart.CondicionVenta = "Efectivo";
                                compCart.IDJuresdiccion = idJurisdiccion;
                                compCart.IDActividad = Comp.IdActividad;

                                compCart.Items = new List<ComprobantesDetalleViewModel>();
                                compCart.ImporteNoGravado = ComprobanteCart.Retrieve().GetImporteNoGravado();
                                compCart.ImporteExento = ComprobanteCart.Retrieve().GetImporteExento();
                                compCart.PercepcionIVA = ComprobanteCart.Retrieve().PercepcionIVA;
                                compCart.PercepcionIIBB = ComprobanteCart.Retrieve().PercepcionIIBB;
                                compCart.Items = ComprobanteCart.Retrieve().Items;

                                if (string.IsNullOrEmpty(Comp.Nombre))
                                {
                                    string nombreComprobante = "";
                                    int contadorConceptos = 2;
                                    int codigoPersona = Convert.ToInt32(prov);
                                    Personas pe = dbContext.Personas.Where(x => x.IDPersona == codigoPersona).FirstOrDefault();
                                    if (pe != null)
                                    {
                                        nombreComprobante = pe.RazonSocial.Substring(0, (pe.RazonSocial.Length > 9 ? 10 : pe.RazonSocial.Length)).ToUpper();
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
                                    compCart.Nombre = Comp.Nombre.Trim().ToUpper();

                                compCart.Vendedor = string.IsNullOrEmpty(Comp.Vendedor) ? "" : Comp.Vendedor.Trim().ToUpper();
                                compCart.Envio = string.IsNullOrEmpty(Comp.Envio) ? "" : Comp.Envio.Trim().ToUpper();
                                compCart.FechaEntrega = fEntrega;
                                compCart.ProcesoCompraAutomatica = procesoCompraAutomatica;

                                var entity = ComprobantesCommon.Guardar(dbContext, compCart);

                                numeroComprobante = entity.IDComprobante;

                                var listaComp = dbContext.vPdvPendientesDeProcesarDetalle.
                                                            Where(x => x.IdUsuario == usu.IDUsuario && x.IdPersonaConcepto == prov && listaComprobantes.Contains(x.IDComprobante))
                                                            .Select(s => s.IDComprobante)
                                                            .Distinct()
                                                            .ToList();

                                if (listaComp != null)
                                {
                                    foreach (int compr in listaComp)
                                    {
                                        Comprobantes CompVinculado = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == compr).FirstOrDefault();
                                        if (CompVinculado != null)
                                        {
                                            CompVinculado.IdComprobanteVinculado = numeroComprobante;
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }

                                ComprobanteCart.Retrieve().Items.Clear();

                                // Fin Pedido de compra


                                //Genero Comprobante de compra

                                string numeroFactura = "0000-00000001";

                                Compras ultFacCom = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.NroFactura).FirstOrDefault();

                                if (ultFacCom != null)
                                {
                                    int numero = Convert.ToInt32(ultFacCom.NroFactura.Substring(5, 8)) + 1;
                                    numeroFactura = ultFacCom.NroFactura.Substring(0, 4) + "-" + numero.ToString("D8");
                                }

                                //if (usu.CUIT.Equals("30716909839"))
                                //{
                                //    sumaTotal = sumaTotal / 1000;
                                //}

                                decimal sumaTotalIVA21 = entity.ImporteTotalBruto * Convert.ToDecimal(0.21);
                                string sumaTotalIVA21Final = "";
                                if (idTipoIva == 5)
                                    sumaTotalIVA21Final = sumaTotalIVA21.ToString("N2");


                                Compras numeroCompra = ComprasCommon.Guardar(0, (int)prov, DateTime.Now.Date.ToShortDateString(), numeroFactura, "", "", "", "", 
                                    sumaTotalIVA21Final.Replace(".",""), "", "", entity.ImporteTotalBruto.ToString().Replace(".", ""),
                                    "", "", "", "", "", "", "CDC", "", "", "", DateTime.Now.Date.ToShortDateString(), 0,
                                    usu.IDUsuario, null, DateTime.Now.Date.ToShortDateString(), "", null);

                                Compras CompraVinculada = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.IDCompra == numeroCompra.IDCompra).FirstOrDefault();
                                if (CompraVinculada != null)
                                {
                                    CompraVinculada.IdComprobante = numeroComprobante;
                                    dbContext.SaveChanges();
                                }

                                //Fin generar comprobante de compra

                            }
                        }

                        return procesoCompraAutomatica;
                    }
                    else
                        throw new CustomException("No se encontraron proveedores.");
                }
            }
            catch (CustomException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # compraAutomatica.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
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
                    Compras Compra = dbContext.Compras.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobante == id).FirstOrDefault();

                    if (Compra != null)
                        dbContext.Compras.Remove(Compra);

                    dbContext.SaveChanges();

                    Comprobantes Comprobante = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IDComprobante == id).FirstOrDefault();

                    if (Comprobante != null)
                        dbContext.Comprobantes.Remove(Comprobante);

                    dbContext.SaveChanges();

                    List<Comprobantes> CompVinculado = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.IdComprobanteVinculado == id).ToList();

                    if(CompVinculado != null)
                    {
                        foreach(Comprobantes c in CompVinculado)
                        {
                            c.IdComprobanteVinculado = null;
                            dbContext.SaveChanges();
                        }                      

                    }

                }                   

            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

}