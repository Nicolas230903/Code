using ACHE.Extensions;
using ACHE.FacturaElectronica;
using ACHE.Model;
using ACHE.Model.Negocio;
using ACHE.Negocio.Facturacion;
using ACHE.Negocio.Productos;
using ACHE.Negocio.Tesoreria;
using DocumentFormat.OpenXml.Office2013.Excel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Common.EntitySql;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class facturaAutomatica : BasePage
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
                if (!afu.HerramientasGeneracionFacturaAutomatica)
                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

        }
    }

    [WebMethod(true)]
    public static string obtenerFacturasDelMesPedidos(string periodo)
    {
        var importe = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int anio = Convert.ToInt32(periodo.Substring(0, 4));
                int mes = Convert.ToInt32(periodo.Substring(4, 2));
              
                decimal facturasDelMes = dbContext.Comprobantes
                            .Where(w => w.IDUsuario == usu.IDUsuario
                                        && w.Tipo.Contains("F")
                                        && w.FechaComprobante.Year == anio && w.FechaComprobante.Month == mes
                                        && w.CAE != null).Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                decimal notaDeCreditoMes = dbContext.Comprobantes
                                            .Where(w => w.IDUsuario == usu.IDUsuario
                                                        && w.Tipo.Contains("NC")
                                                        && w.FechaComprobante.Year == anio && w.FechaComprobante.Month == mes
                                                        && w.CAE != null).Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                decimal totalFacturaDelMes = facturasDelMes - notaDeCreditoMes;


                importe = totalFacturaDelMes.ToString("N2");
                
            }

        }
        return importe;
    }

    [WebMethod(true)]
    public static string obtenerComprasDelMesPedidos(string periodo)
    {
        var importe = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int anio = Convert.ToInt32(periodo.Substring(0, 4));
                int mes = Convert.ToInt32(periodo.Substring(4, 2));

                decimal totalCompraDelMes = (decimal)dbContext.Compras.Where(w => w.IDUsuario == usu.IDUsuario && w.FechaAlta.Year == anio && w.FechaAlta.Month == mes).Select(s => s.Total).DefaultIfEmpty(0).Sum();

                importe = totalCompraDelMes.ToString("N2");

            }

        }
        return importe;
    }

    [WebMethod(true)]
    public static string obtenerGastosGeneralesDelMesPedidos(string periodo)
    {
        var importe = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int anio = Convert.ToInt32(periodo.Substring(0, 4));
                int mes = Convert.ToInt32(periodo.Substring(4, 2));

                decimal totalGastosGeneralesDelMes = 0;
                List<GastosGenerales> g = dbContext.GastosGenerales.Where(w => w.IdUsuario == usu.IDUsuario && w.Periodo.Year == anio && w.Periodo.Month == mes).ToList();
                foreach (GastosGenerales x in g)
                {
                    totalGastosGeneralesDelMes = totalGastosGeneralesDelMes + x.Sueldos + x.SeguridadEHigiene + x.Municipales + x.Monotributos + x.AportesYContribuciones + x.Ganancias12 + x.CreditoBancario + x.RetencionesDeIIBB + x.PlanesAFIP + x.Gastos1 + x.Gastos2 + x.Gastos3 + x.F1 + x.F2;
                }

                importe = totalGastosGeneralesDelMes.ToString("N2");

            }

        }
        return importe;
    }


    [WebMethod(true)]
    public static string obtenerResumenPedidos(string periodo)
     {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int anio = Convert.ToInt32(periodo.Substring(0,4));
                int mes = Convert.ToInt32(periodo.Substring(4, 2));
                bool por0 = false;
                bool por20 = false;
                bool por40 = false;
                bool por60 = false;
                bool por80 = false;
                string html0 = string.Empty;
                string html20 = string.Empty;
                string html40 = string.Empty;
                string html60 = string.Empty;
                string html80 = string.Empty;
                decimal v0a20TotalImportePdv = 0, v21a40TotalImportePdv = 0, v41a60TotalImportePdv = 0, v61a80TotalImportePdv = 0, v81a100TotalImportePdv = 0;
                decimal v0a20TotalImporteFacturaBorrador = 0, v21a40TotalImporteFacturaBorrador = 0, v41a60TotalImporteFacturaBorrador = 0, v61a80TotalImporteFacturaBorrador = 0, v81a100TotalImporteFacturaBorrador = 0;
                decimal v0a20TotalImporteFacturaConCAE = 0, v21a40TotalImporteFacturaConCAE = 0, v41a60TotalImporteFacturaConCAE = 0, v61a80TotalImporteFacturaConCAE = 0, v81a100TotalImporteFacturaConCAE = 0;
                List<Comprobantes> lcTotal = new List<Comprobantes>();
                List<Comprobantes> lcFacturados = new List<Comprobantes>();
                List<Comprobantes> lcNotaDeCredito = new List<Comprobantes>();                

                lcTotal = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario 
                                                    && w.Tipo.Contains("PDV")
                                                    && w.FechaAlta.Year == anio && w.FechaAlta.Month == mes).ToList();


                lcFacturados = dbContext.Comprobantes
                                                    .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Contains("F"))
                                                    .ToList();

                lcNotaDeCredito = dbContext.Comprobantes
                                    .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Contains("NC") && w.CAE != null)
                                    .ToList();

                //DE 0% A 20%

                foreach (Comprobantes comprobante in lcTotal)
                {

                    decimal totalfacturasBorrador = lcFacturados
                                                    .Where(w => w.IdComprobanteVinculado == comprobante.IDComprobante
                                                            && w.CAE == null)
                                                    .Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                    decimal totalfacturasVinculadas = 0;
                    List<Comprobantes> facturasVinculadas = lcFacturados
                                                                .Where(w => w.IdComprobanteVinculado == comprobante.IDComprobante 
                                                                       && w.CAE != null)
                                                                .ToList();

                    decimal facturasCanceladasConNotaDeCredito = 0;
                    foreach (Comprobantes x in facturasVinculadas)
                    {
                        decimal notaDeCreditoDeLaFactura = lcNotaDeCredito
                            .Where(w => w.IdComprobanteAsociado == x.IDComprobante).Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                        facturasCanceladasConNotaDeCredito = facturasCanceladasConNotaDeCredito + notaDeCreditoDeLaFactura;
                    }

                    totalfacturasVinculadas = facturasVinculadas.Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum() - facturasCanceladasConNotaDeCredito;

                    decimal porcentaje = 100;
                    if ((comprobante.ImporteTotalNeto - totalfacturasVinculadas) > 0)
                    {
                        porcentaje = porcentaje - (100 * (comprobante.ImporteTotalNeto / (comprobante.ImporteTotalNeto - totalfacturasVinculadas)));
                    }                        

                    if (porcentaje >= 0 && porcentaje <= 20)
                    {
                        if (!por0)
                        {
                            html0 += "<tr style='BACKGROUND-COLOR: aquamarine' class='header'>";
                            html0 += "<th> <a class='btn btn-black' onclick=\"javascript:obtenerDetallePedidos('" + periodo + "','0a20');\">Detalles</a> </th>";
                            html0 += "<th> FACTURACION DE 0 A 20% </th>";
                            html0 += "<th></th>";
                            html0 += "<th></th>";
                            html0 += "<th><strong>0a20TotalImportePdv</strong></th>";
                            html0 += "<th><strong>0a20TotalImporteFacturaBorrador</strong></th>";
                            html0 += "<th><strong>0a20TotalImporteFacturaConCAE</strong></th>";
                            html0 += "<th></th>";
                            html0 += "</tr>";
                            por0 = true;
                        }

                        v0a20TotalImportePdv = v0a20TotalImportePdv + comprobante.ImporteTotalNeto;
                        v0a20TotalImporteFacturaBorrador = v0a20TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v0a20TotalImporteFacturaConCAE = v0a20TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 21 && porcentaje <= 40)
                    {
                        if (!por20)
                        {
                            html20 += "<tr style='BACKGROUND-COLOR: aquamarine'>";
                            html20 += "<th> <a class='btn btn-black' onclick=\"javascript:obtenerDetallePedidos('" + periodo + "','21a40');\">Detalles</a> </th>";
                            html20 += "<th> FACTURACION DE 21 A 40% </th>";
                            html20 += "<th></th>";
                            html20 += "<th></th>";
                            html20 += "<th><strong>21a40TotalImportePdv</strong></th>";
                            html20 += "<th><strong>21a40TotalImporteFacturaBorrador</strong></th>";
                            html20 += "<th><strong>21a40TotalImporteFacturaConCAE</strong></th>";
                            html20 += "<th></th>";
                            html20 += "</tr>";
                            por20 = true;
                        }

                        v21a40TotalImportePdv = v21a40TotalImportePdv + comprobante.ImporteTotalNeto;
                        v21a40TotalImporteFacturaBorrador = v21a40TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v21a40TotalImporteFacturaConCAE = v21a40TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 41 && porcentaje <= 60)
                    {
                        if (!por40)
                        {
                            html40 += "<tr style='BACKGROUND-COLOR: aquamarine'>";
                            html40 += "<th> <a class='btn btn-black' onclick=\"javascript:obtenerDetallePedidos('" + periodo + "','41a60');\">Detalles</a> </th>";
                            html40 += "<th> FACTURACION DE 41 A 60% </th>";
                            html40 += "<th></th>";
                            html40 += "<th></th>";
                            html40 += "<th><strong>41a60TotalImportePdv</strong></th>";
                            html40 += "<th><strong>41a60TotalImporteFacturaBorrador</strong></th>";
                            html40 += "<th><strong>41a60TotalImporteFacturaConCAE</strong></th>";
                            html40 += "<th></th>";
                            html40 += "</tr>";
                            por40 = true;
                        }

                        v41a60TotalImportePdv = v41a60TotalImportePdv + comprobante.ImporteTotalNeto;
                        v41a60TotalImporteFacturaBorrador = v41a60TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v41a60TotalImporteFacturaConCAE = v41a60TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 61 && porcentaje <= 80)
                    {
                        if (!por60)
                        {
                            html60 += "<tr style='BACKGROUND-COLOR: aquamarine'>";
                            html60 += "<th> <a class='btn btn-black' onclick=\"javascript:obtenerDetallePedidos('" + periodo + "','61a80');\">Detalles</a> </th>";
                            html60 += "<td> FACTURACION DE 61 A 80% </td>";
                            html60 += "<td></td>";
                            html60 += "<td></td>";
                            html60 += "<td><strong>61a80TotalImportePdv</strong></td>";
                            html60 += "<td><strong>61a80TotalImporteFacturaBorrador</strong></td>";
                            html60 += "<td><strong>61a80TotalImporteFacturaConCAE</strong></td>";
                            html60 += "<td></td>";
                            html60 += "</tr>";
                            por60 = true;
                        }

                        v61a80TotalImportePdv = v61a80TotalImportePdv + comprobante.ImporteTotalNeto;
                        v61a80TotalImporteFacturaBorrador = v61a80TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v61a80TotalImporteFacturaConCAE = v61a80TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 81 && porcentaje <= 100)
                    {
                        if (!por80)
                        {
                            html80 += "<tr style='BACKGROUND-COLOR: aquamarine'>";
                            html80 += "<th> <a class='btn btn-black' onclick=\"javascript:obtenerDetallePedidos('" + periodo + "','81a100');\">Detalles</a> </th>";
                            html80 += "<td> FACTURACION DE 81 A 100% </td>";
                            html80 += "<td></td>";
                            html80 += "<td></td>";
                            html80 += "<td><strong>81a100TotalImportePdv</strong></td>";
                            html80 += "<td><strong>81a100TotalImporteFacturaBorrador</strong></td>";
                            html80 += "<td><strong>81a100TotalImporteFacturaConCAE</strong></td>";
                            html80 += "<td></td>";
                            html80 += "</tr>";
                            por80 = true;
                        }

                        v81a100TotalImportePdv = v81a100TotalImportePdv + comprobante.ImporteTotalNeto;
                        v81a100TotalImporteFacturaBorrador = v81a100TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v81a100TotalImporteFacturaConCAE = v81a100TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                }

                html0 = html0.Replace("0a20TotalImportePdv", v0a20TotalImportePdv.ToString("N2"));
                html0 = html0.Replace("0a20TotalImporteFacturaBorrador", v0a20TotalImporteFacturaBorrador.ToString("N2"));
                html0 = html0.Replace("0a20TotalImporteFacturaConCAE", v0a20TotalImporteFacturaConCAE.ToString("N2"));
                html20 = html20.Replace("21a40TotalImportePdv", v21a40TotalImportePdv.ToString("N2"));
                html20 = html20.Replace("21a40TotalImporteFacturaBorrador", v21a40TotalImporteFacturaBorrador.ToString("N2"));
                html20 = html20.Replace("21a40TotalImporteFacturaConCAE", v21a40TotalImporteFacturaConCAE.ToString("N2"));
                html40 = html40.Replace("41a60TotalImportePdv", v41a60TotalImportePdv.ToString("N2"));
                html40 = html40.Replace("41a60TotalImporteFacturaBorrador", v41a60TotalImporteFacturaBorrador.ToString("N2"));
                html40 = html40.Replace("41a60TotalImporteFacturaConCAE", v41a60TotalImporteFacturaConCAE.ToString("N2"));
                html60 = html60.Replace("61a80TotalImportePdv", v61a80TotalImportePdv.ToString("N2"));
                html60 = html60.Replace("61a80TotalImporteFacturaBorrador", v61a80TotalImporteFacturaBorrador.ToString("N2"));
                html60 = html60.Replace("61a80TotalImporteFacturaConCAE", v61a80TotalImporteFacturaConCAE.ToString("N2"));
                html80 = html80.Replace("81a100TotalImportePdv", v81a100TotalImportePdv.ToString("N2"));
                html80 = html80.Replace("81a100TotalImporteFacturaBorrador", v81a100TotalImporteFacturaBorrador.ToString("N2"));
                html80 = html80.Replace("81a100TotalImporteFacturaConCAE", v81a100TotalImporteFacturaConCAE.ToString("N2"));
                html = html0 + html20 + html40 + html60 + html80;

            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes pedidos de venta generados.</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static string obtenerDetallePedidos(string periodo, string rango)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int anio = Convert.ToInt32(periodo.Substring(0, 4));
                int mes = Convert.ToInt32(periodo.Substring(4, 2));
                bool por0 = false;
                bool por20 = false;
                bool por40 = false;
                bool por60 = false;
                bool por80 = false;
                string html0 = string.Empty;
                string html20 = string.Empty;
                string html40 = string.Empty;
                string html60 = string.Empty;
                string html80 = string.Empty;
                decimal v0a20TotalImportePdv = 0, v21a40TotalImportePdv = 0, v41a60TotalImportePdv = 0, v61a80TotalImportePdv = 0, v81a100TotalImportePdv = 0;
                decimal v0a20TotalImporteFacturaBorrador = 0, v21a40TotalImporteFacturaBorrador = 0, v41a60TotalImporteFacturaBorrador = 0, v61a80TotalImporteFacturaBorrador = 0, v81a100TotalImporteFacturaBorrador = 0;
                decimal v0a20TotalImporteFacturaConCAE = 0, v21a40TotalImporteFacturaConCAE = 0, v41a60TotalImporteFacturaConCAE = 0, v61a80TotalImporteFacturaConCAE = 0, v81a100TotalImporteFacturaConCAE = 0;
                List<Comprobantes> lcTotal = new List<Comprobantes>();
                List<Comprobantes> lcFacturados = new List<Comprobantes>();
                List<Comprobantes> lcNotaDeCredito = new List<Comprobantes>();

                lcTotal = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario
                                                    && w.Tipo.Contains("PDV")
                                                    && w.FechaAlta.Year == anio && w.FechaAlta.Month == mes).OrderBy(o => o.Personas.RazonSocial).ToList();


                lcFacturados = dbContext.Comprobantes
                                                    .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Contains("F"))
                                                    .ToList();

                lcNotaDeCredito = dbContext.Comprobantes
                                    .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Contains("NC") && w.CAE != null)
                                    .ToList();

                //DE 0% A 20%

                foreach (Comprobantes comprobante in lcTotal)
                {

                    decimal totalfacturasBorrador = lcFacturados
                                                    .Where(w => w.IdComprobanteVinculado == comprobante.IDComprobante
                                                            && w.CAE == null)
                                                    .Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                    decimal totalfacturasVinculadas = 0;
                    List<Comprobantes> facturasVinculadas = lcFacturados
                                                                .Where(w => w.IdComprobanteVinculado == comprobante.IDComprobante
                                                                       && w.CAE != null)
                                                                .ToList();

                    decimal facturasCanceladasConNotaDeCredito = 0;
                    foreach (Comprobantes x in facturasVinculadas)
                    {
                        decimal notaDeCreditoDeLaFactura = lcNotaDeCredito
                            .Where(w => w.IdComprobanteAsociado == x.IDComprobante).Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                        facturasCanceladasConNotaDeCredito = facturasCanceladasConNotaDeCredito + notaDeCreditoDeLaFactura;
                    }

                    totalfacturasVinculadas = facturasVinculadas.Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum() - facturasCanceladasConNotaDeCredito;

                    decimal porcentaje = 100;
                    if ((comprobante.ImporteTotalNeto - totalfacturasVinculadas) > 0)
                    {
                        porcentaje = porcentaje - (100 * (comprobante.ImporteTotalNeto / (comprobante.ImporteTotalNeto - totalfacturasVinculadas)));
                    }

                    if (porcentaje >= 0 && porcentaje <= 20)
                    {
                        if (!por0)
                        {
                            html0 += "<tr style='BACKGROUND-COLOR: aquamarine'>";
                            html0 += "<th></th>";
                            html0 += "<th></th>";
                            html0 += "<th> FACTURACION DE 0 A 20% </th>";
                            html0 += "<th></th>";
                            html0 += "<th></th>";
                            html0 += "<th><strong>0a20TotalImportePdv</strong></th>";
                            html0 += "<th><strong>0a20TotalImporteFacturaBorrador</strong></th>";
                            html0 += "<th><strong>0a20TotalImporteFacturaConCAE</strong></th>";
                            html0 += "<th></th>";
                            html0 += "</tr>";
                            por0 = true;
                        }

                        switch (comprobante.Personas.IdRango)
                        {
                            case 0:
                                html0 += "<tr style='BACKGROUND-COLOR: lightpink'>";
                                break;
                            case 1:
                                html0 += "<tr style='BACKGROUND-COLOR: greenyellow'>";
                                break;
                            default:
                                html0 += "<tr style='BACKGROUND-COLOR: yellow'>";
                                break;
                        }                        
                        html0 += "<td>" + descripcionRango(comprobante.Personas.IdRango) + "</td>";
                        html0 += "<td>" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000") + "</td>";
                        html0 += "<td>" + (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper()) + "</td>";
                        html0 += "<td>" + comprobante.Nombre + "</td>";
                        html0 += "<td>" + comprobante.FechaAlta.ToString(formatoFecha) + "</td>";
                        html0 += "<td>" + comprobante.ImporteTotalNeto.ToString("N2") + " </td>";
                        html0 += "<td>" + totalfacturasBorrador.ToString("N2") + " </td>";
                        html0 += "<td>" + totalfacturasVinculadas.ToString("N2") + " </td>";
                        html0 += "<td><a onclick=\"editarPedidoDeVenta(" + comprobante.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Ver Comprobante'><i class='fa fa-pencil'></i></a></td>";
                        html0 += "</tr>";

                        v0a20TotalImportePdv = v0a20TotalImportePdv + comprobante.ImporteTotalNeto;
                        v0a20TotalImporteFacturaBorrador = v0a20TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v0a20TotalImporteFacturaConCAE = v0a20TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 21 && porcentaje <= 40)
                    {
                        if (!por20)
                        {
                            html20 += "<tr style='BACKGROUND-COLOR: aquamarine' >";
                            html20 += "<th></th>";
                            html20 += "<th></th>";
                            html20 += "<th> FACTURACION DE 21 A 40% </th>";
                            html20 += "<th></th>";
                            html20 += "<th></th>";
                            html20 += "<th><strong>21a40TotalImportePdv</strong></th>";
                            html20 += "<th><strong>21a40TotalImporteFacturaBorrador</strong></th>";
                            html20 += "<th><strong>21a40TotalImporteFacturaConCAE</strong></th>";
                            html20 += "<th></th>";
                            html20 += "</tr>";
                            por20 = true;
                        }

                        switch (comprobante.Personas.IdRango)
                        {
                            case 0:
                                html20 += "<tr style='BACKGROUND-COLOR: lightpink'>";
                                break;
                            case 2:
                                html20 += "<tr style='BACKGROUND-COLOR: greenyellow'>";
                                break;
                            default:
                                html20 += "<tr style='BACKGROUND-COLOR: yellow'>";
                                break;
                        }
                        html20 += "<td>" + descripcionRango(comprobante.Personas.IdRango) + "</td>";
                        html20 += "<td>" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000") + "</td>";
                        html20 += "<td>" + (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper()) + "</td>";
                        html20 += "<td>" + comprobante.Nombre + "</td>";
                        html20 += "<td>" + comprobante.FechaAlta.ToString(formatoFecha) + "</td>";
                        html20 += "<td>" + comprobante.ImporteTotalNeto.ToString("N2") + " </td>";
                        html20 += "<td>" + totalfacturasBorrador.ToString("N2") + " </td>";
                        html20 += "<td>" + totalfacturasVinculadas.ToString("N2") + " </td>";
                        html20 += "<td><a onclick=\"editarPedidoDeVenta(" + comprobante.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Ver Comprobante'><i class='fa fa-pencil'></i></a></td>";
                        html20 += "</tr>";

                        v21a40TotalImportePdv = v21a40TotalImportePdv + comprobante.ImporteTotalNeto;
                        v21a40TotalImporteFacturaBorrador = v21a40TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v21a40TotalImporteFacturaConCAE = v21a40TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 41 && porcentaje <= 60)
                    {
                        if (!por40)
                        {
                            html40 += "<tr style='BACKGROUND-COLOR: aquamarine' >";
                            html40 += "<th></th>";
                            html40 += "<th></th>";
                            html40 += "<th> FACTURACION DE 41 A 60% </th>";
                            html40 += "<th></th>";
                            html40 += "<th></th>";
                            html40 += "<th><strong>41a60TotalImportePdv</strong></th>";
                            html40 += "<th><strong>41a60TotalImporteFacturaBorrador</strong></th>";
                            html40 += "<th><strong>41a60TotalImporteFacturaConCAE</strong></th>";
                            html40 += "<th></th>";
                            html40 += "</tr>";
                            por40 = true;
                        }

                        switch (comprobante.Personas.IdRango)
                        {
                            case 0:
                                html40 += "<tr style='BACKGROUND-COLOR: lightpink'>";
                                break;
                            case 3:
                                html40 += "<tr style='BACKGROUND-COLOR: greenyellow'>";
                                break;
                            default:
                                html40 += "<tr style='BACKGROUND-COLOR: yellow'>";
                                break;
                        }
                        html40 += "<td>" + descripcionRango(comprobante.Personas.IdRango) + "</td>";
                        html40 += "<td>" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000") + "</td>";
                        html40 += "<td>" + (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper()) + "</td>";
                        html40 += "<td>" + comprobante.Nombre + "</td>";
                        html40 += "<td>" + comprobante.FechaAlta.ToString(formatoFecha) + "</td>";
                        html40 += "<td>" + comprobante.ImporteTotalNeto.ToString("N2") + " </td>";
                        html40 += "<td>" + totalfacturasBorrador.ToString("N2") + " </td>";
                        html40 += "<td>" + totalfacturasVinculadas.ToString("N2") + " </td>";
                        html40 += "<td><a onclick=\"editarPedidoDeVenta(" + comprobante.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Ver Comprobante'><i class='fa fa-pencil'></i></a></td>";
                        html40 += "</tr>";

                        v41a60TotalImportePdv = v41a60TotalImportePdv + comprobante.ImporteTotalNeto;
                        v41a60TotalImporteFacturaBorrador = v41a60TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v41a60TotalImporteFacturaConCAE = v41a60TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 61 && porcentaje <= 80)
                    {
                        if (!por60)
                        {
                            html60 += "<tr style='BACKGROUND-COLOR: aquamarine' >";
                            html60 += "<th></th>";
                            html60 += "<th></th>";
                            html60 += "<td> FACTURACION DE 61 A 80% </td>";
                            html60 += "<td></td>";
                            html60 += "<td></td>";
                            html60 += "<td><strong>61a80TotalImportePdv</strong></td>";
                            html60 += "<td><strong>61a80TotalImporteFacturaBorrador</strong></td>";
                            html60 += "<td><strong>61a80TotalImporteFacturaConCAE</strong></td>";
                            html60 += "<td></td>";
                            html60 += "</tr>";
                            por60 = true;
                        }

                        switch (comprobante.Personas.IdRango)
                        {
                            case 0:
                                html60 += "<tr style='BACKGROUND-COLOR: lightpink'>";
                                break;
                            case 4:
                                html60 += "<tr style='BACKGROUND-COLOR: greenyellow'>";
                                break;
                            default:
                                html60 += "<tr style='BACKGROUND-COLOR: yellow'>";
                                break;
                        }
                        html60 += "<td>" + descripcionRango(comprobante.Personas.IdRango) + "</td>";
                        html60 += "<td>" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000") + "</td>";
                        html60 += "<td>" + (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper()) + "</td>";
                        html60 += "<td>" + comprobante.Nombre + "</td>";
                        html60 += "<td>" + comprobante.FechaAlta.ToString(formatoFecha) + "</td>";
                        html60 += "<td>" + comprobante.ImporteTotalNeto.ToString("N2") + " </td>";
                        html60 += "<td>" + totalfacturasBorrador.ToString("N2") + " </td>";
                        html60 += "<td>" + totalfacturasVinculadas.ToString("N2") + " </td>";
                        html60 += "<td><a onclick=\"editarPedidoDeVenta(" + comprobante.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Ver Comprobante'><i class='fa fa-pencil'></i></a></td>";
                        html60 += "</tr>";

                        v61a80TotalImportePdv = v61a80TotalImportePdv + comprobante.ImporteTotalNeto;
                        v61a80TotalImporteFacturaBorrador = v61a80TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v61a80TotalImporteFacturaConCAE = v61a80TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                    if (porcentaje >= 81 && porcentaje <= 100)
                    {
                        if (!por80)
                        {
                            html80 += "<tr style='BACKGROUND-COLOR: aquamarine'>";
                            html80 += "<th></th>";
                            html80 += "<th></th>";
                            html80 += "<td> FACTURACION DE 81 A 100% </td>";
                            html80 += "<td></td>";
                            html80 += "<td></td>";
                            html80 += "<td><strong>81a100TotalImportePdv</strong></td>";
                            html80 += "<td><strong>81a100TotalImporteFacturaBorrador</strong></td>";
                            html80 += "<td><strong>81a100TotalImporteFacturaConCAE</strong></td>";
                            html80 += "<td></td>";
                            html80 += "</tr>";
                            por80 = true;
                        }

                        switch (comprobante.Personas.IdRango)
                        {
                            case 0:
                                html80 += "<tr style='BACKGROUND-COLOR: lightpink'>";
                                break;
                            case 5:
                                html80 += "<tr style='BACKGROUND-COLOR: greenyellow'>";
                                break;
                            default:
                                html80 += "<tr style='BACKGROUND-COLOR: yellow'>";
                                break;
                        }
                        html80 += "<td>" + descripcionRango(comprobante.Personas.IdRango) + "</td>";
                        html80 += "<td>" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000") + "</td>";
                        html80 += "<td>" + (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper()) + "</td>";
                        html80 += "<td>" + comprobante.Nombre + "</td>";
                        html80 += "<td>" + comprobante.FechaAlta.ToString(formatoFecha) + "</td>";
                        html80 += "<td>" + comprobante.ImporteTotalNeto.ToString("N2") + " </td>";
                        html80 += "<td>" + totalfacturasBorrador.ToString("N2") + " </td>";
                        html80 += "<td>" + totalfacturasVinculadas.ToString("N2") + " </td>";
                        html80 += "<td><a onclick=\"editarPedidoDeVenta(" + comprobante.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Ver Comprobante'><i class='fa fa-pencil'></i></a></td>";
                        html80 += "</tr>";

                        v81a100TotalImportePdv = v81a100TotalImportePdv + comprobante.ImporteTotalNeto;
                        v81a100TotalImporteFacturaBorrador = v81a100TotalImporteFacturaBorrador + totalfacturasBorrador;
                        v81a100TotalImporteFacturaConCAE = v81a100TotalImporteFacturaConCAE + totalfacturasVinculadas;
                    }
                }

                html0 = html0.Replace("0a20TotalImportePdv", v0a20TotalImportePdv.ToString("N2"));
                html0 = html0.Replace("0a20TotalImporteFacturaBorrador", v0a20TotalImporteFacturaBorrador.ToString("N2"));
                html0 = html0.Replace("0a20TotalImporteFacturaConCAE", v0a20TotalImporteFacturaConCAE.ToString("N2"));
                html20 = html20.Replace("21a40TotalImportePdv", v21a40TotalImportePdv.ToString("N2"));
                html20 = html20.Replace("21a40TotalImporteFacturaBorrador", v21a40TotalImporteFacturaBorrador.ToString("N2"));
                html20 = html20.Replace("21a40TotalImporteFacturaConCAE", v21a40TotalImporteFacturaConCAE.ToString("N2"));
                html40 = html40.Replace("41a60TotalImportePdv", v41a60TotalImportePdv.ToString("N2"));
                html40 = html40.Replace("41a60TotalImporteFacturaBorrador", v41a60TotalImporteFacturaBorrador.ToString("N2"));
                html40 = html40.Replace("41a60TotalImporteFacturaConCAE", v41a60TotalImporteFacturaConCAE.ToString("N2"));
                html60 = html60.Replace("61a80TotalImportePdv", v61a80TotalImportePdv.ToString("N2"));
                html60 = html60.Replace("61a80TotalImporteFacturaBorrador", v61a80TotalImporteFacturaBorrador.ToString("N2"));
                html60 = html60.Replace("61a80TotalImporteFacturaConCAE", v61a80TotalImporteFacturaConCAE.ToString("N2"));
                html80 = html80.Replace("81a100TotalImportePdv", v81a100TotalImportePdv.ToString("N2"));
                html80 = html80.Replace("81a100TotalImporteFacturaBorrador", v81a100TotalImporteFacturaBorrador.ToString("N2"));
                html80 = html80.Replace("81a100TotalImporteFacturaConCAE", v81a100TotalImporteFacturaConCAE.ToString("N2"));

                switch (rango)
                {
                    case "0a20":
                        html = html0;
                        break;
                    case "21a40":
                        html = html20;
                        break;
                    case "41a60":
                        html = html40;
                        break;
                    case "61a80":
                        html = html60;
                        break;
                    case "81a100":
                        html = html80;
                        break;
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes pedidos de venta generados.</td></tr>";

        }
        return html;
    }

    public static string descripcionRango(int idRango)
    {
        switch (idRango)
        {
            case 0:
                return "Sin Definir";
            case 1:
                return "0a20";
            case 2:
                return "21a40";
            case 3:
                return "41a60";
            case 4:
                return "61a80";
            case 5:
                return "81a100";
            default:
                return "Sin Datos";
        }
    }

    [WebMethod(true)]
    public static string exportarPedidoDeVentaDetalle(string periodo, string rango)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            DataTable tabla = new DataTable();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string fileName = "PDV_Detalle_" + periodo + "_" + rango;
            string path = "~/tmp/";
            try
            {

                using (var dbContext = new ACHEEntities())
                {
                    int anio = Convert.ToInt32(periodo.Substring(0, 4));
                    int mes = Convert.ToInt32(periodo.Substring(4, 2));
                    List<Comprobantes> lcTotal = new List<Comprobantes>();
                    List<Comprobantes> lcFacturados = new List<Comprobantes>();
                    List<Comprobantes> lcNotaDeCredito = new List<Comprobantes>();
                                        

                    // Variables para las columnas y las filas
                    DataColumn column;
                    DataRow row;

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Nro";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "RazonSocial";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Nombre";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "FechaAlta";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "ImporteTotalNeto";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "FacturasBorrador";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "FacturasConCAE";
                    tabla.Columns.Add(column);
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Rango";
                    tabla.Columns.Add(column);

                    lcTotal = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario
                                                        && w.Tipo.Contains("PDV")
                                                        && w.FechaAlta.Year == anio && w.FechaAlta.Month == mes).ToList();


                    lcFacturados = dbContext.Comprobantes
                                                        .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Contains("F"))
                                                        .ToList();

                    lcNotaDeCredito = dbContext.Comprobantes
                                        .Where(w => w.IDUsuario == usu.IDUsuario && w.Tipo.Contains("NC") && w.CAE != null)
                                        .ToList();

                    //DE 0% A 20%

                    foreach (Comprobantes comprobante in lcTotal)
                    {

                        decimal totalfacturasBorrador = lcFacturados
                                                        .Where(w => w.IdComprobanteVinculado == comprobante.IDComprobante
                                                                && w.CAE == null)
                                                        .Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                        decimal totalfacturasVinculadas = 0;
                        List<Comprobantes> facturasVinculadas = lcFacturados
                                                                    .Where(w => w.IdComprobanteVinculado == comprobante.IDComprobante
                                                                           && w.CAE != null)
                                                                    .ToList();

                        decimal facturasCanceladasConNotaDeCredito = 0;
                        foreach (Comprobantes x in facturasVinculadas)
                        {
                            decimal notaDeCreditoDeLaFactura = lcNotaDeCredito
                                .Where(w => w.IdComprobanteAsociado == x.IDComprobante).Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum();

                            facturasCanceladasConNotaDeCredito = facturasCanceladasConNotaDeCredito + notaDeCreditoDeLaFactura;
                        }

                        totalfacturasVinculadas = facturasVinculadas.Select(s => s.ImporteTotalNeto).DefaultIfEmpty(0).Sum() - facturasCanceladasConNotaDeCredito;

                        decimal porcentaje = 100;
                        if ((comprobante.ImporteTotalNeto - totalfacturasVinculadas) > 0)
                        {
                            porcentaje = porcentaje - (100 * (comprobante.ImporteTotalNeto / (comprobante.ImporteTotalNeto - totalfacturasVinculadas)));
                        }

                        switch (rango)
                        {
                            case "0a20":
                                if (porcentaje >= 0 && porcentaje <= 20)
                                {
                                    row = tabla.NewRow();
                                    row["Nro"] = comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                                    row["RazonSocial"] = (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper());
                                    row["Nombre"] = comprobante.Nombre;
                                    row["FechaAlta"] = comprobante.FechaAlta.ToString(formatoFecha);
                                    row["ImporteTotalNeto"] = comprobante.ImporteTotalNeto.ToString("N2");
                                    row["FacturasBorrador"] = totalfacturasBorrador.ToString("N2");
                                    row["FacturasConCAE"] = totalfacturasVinculadas.ToString("N2");
                                    row["Rango"] = "0a20";
                                    tabla.Rows.Add(row);
                                }
                                break;
                            case "21a40":
                                if (porcentaje >= 21 && porcentaje <= 40)
                                {
                                    row = tabla.NewRow();
                                    row["Nro"] = comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                                    row["RazonSocial"] = (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper());
                                    row["Nombre"] = comprobante.Nombre;
                                    row["FechaAlta"] = comprobante.FechaAlta.ToString(formatoFecha);
                                    row["ImporteTotalNeto"] = comprobante.ImporteTotalNeto.ToString("N2");
                                    row["FacturasBorrador"] = totalfacturasBorrador.ToString("N2");
                                    row["FacturasConCAE"] = totalfacturasVinculadas.ToString("N2");
                                    row["Rango"] = "21a40";
                                    tabla.Rows.Add(row);
                                }
                                break;
                            case "41a60":
                                if (porcentaje >= 41 && porcentaje <= 60)
                                {
                                    row = tabla.NewRow();
                                    row["Nro"] = comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                                    row["RazonSocial"] = (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper());
                                    row["Nombre"] = comprobante.Nombre;
                                    row["FechaAlta"] = comprobante.FechaAlta.ToString(formatoFecha);
                                    row["ImporteTotalNeto"] = comprobante.ImporteTotalNeto.ToString("N2");
                                    row["FacturasBorrador"] = totalfacturasBorrador.ToString("N2");
                                    row["FacturasConCAE"] = totalfacturasVinculadas.ToString("N2");
                                    row["Rango"] = "41a60";
                                    tabla.Rows.Add(row);
                                }
                                break;
                            case "61a80":
                                if (porcentaje >= 61 && porcentaje <= 80)
                                {
                                    row = tabla.NewRow();
                                    row["Nro"] = comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                                    row["RazonSocial"] = (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper());
                                    row["Nombre"] = comprobante.Nombre;
                                    row["FechaAlta"] = comprobante.FechaAlta.ToString(formatoFecha);
                                    row["ImporteTotalNeto"] = comprobante.ImporteTotalNeto.ToString("N2");
                                    row["FacturasBorrador"] = totalfacturasBorrador.ToString("N2");
                                    row["FacturasConCAE"] = totalfacturasVinculadas.ToString("N2");
                                    row["Rango"] = "61a80";
                                    tabla.Rows.Add(row);
                                }
                                break;
                            case "81a100":
                                if (porcentaje >= 81 && porcentaje <= 100)
                                {
                                    row = tabla.NewRow();
                                    row["Nro"] = comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                                    row["RazonSocial"] = (comprobante.Personas.NombreFantansia == "" ? comprobante.Personas.RazonSocial.ToUpper() : comprobante.Personas.NombreFantansia.ToUpper());
                                    row["Nombre"] = comprobante.Nombre;
                                    row["FechaAlta"] = comprobante.FechaAlta.ToString(formatoFecha);
                                    row["ImporteTotalNeto"] = comprobante.ImporteTotalNeto.ToString("N2");
                                    row["FacturasBorrador"] = totalfacturasBorrador.ToString("N2");
                                    row["FacturasConCAE"] = totalfacturasVinculadas.ToString("N2");
                                    row["Rango"] = "81a100";
                                    tabla.Rows.Add(row);
                                }
                                break;
                        }
                        
                    }

                }

                if (tabla.Rows.Count > 0)
                    CommonModel.GenerarArchivo(tabla, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
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

    [WebMethod(true)]
    public static string obtenerFacturaAutomaticaProcesados(long procesoFacturaAutomatica)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var tipoComprobantesIncluidos = new[] { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "RCAMP", "RCBMP", "RCCMP", "NDAMP", "NDBMP", "NDCMP" };
                List<Comprobantes> lc = new List<Comprobantes>();

                if (procesoFacturaAutomatica != 0)
                {
                    lc = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta").Where(x => x.IDUsuario == usu.IDUsuario && x.ProcesoFacturaAutomatica == procesoFacturaAutomatica).ToList();
                }
                else
                {
                    lc = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo)).Take(50).ToList();
                }


                if (lc.Any())
                {
                    int index = 1;
                    foreach (var detalle in lc)
                    {
                        html += "<tr>";
                        html += "<td>" + detalle.IDComprobante.ToString() + "</td>";
                        html += "<td>" + (detalle.Personas.NombreFantansia == "" ? detalle.Personas.RazonSocial.ToUpper() : detalle.Personas.NombreFantansia.ToUpper()) + "</td>";
                        html += "<td>" + detalle.FechaComprobante.ToString(formatoFecha) + "</td>";
                        html += "<td>" + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000") + " </td>";
                        html += "<td>" + detalle.ImporteTotalBruto.ToString("N2") + " </td>";
                        html += "<td>" + detalle.ImporteTotalNeto.ToString("N2") + " </td>";
                        if(detalle.CAE != null)
                        {
                            html += "<td><a onclick=\"editar(" + detalle.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Ver factura'><i class='fa fa-search'></i></a></td>";
                            html += "<td><a onclick=\"mostrarEnvioDesdeListado(" + detalle.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Enviar correo electrónico'><i class='fa fa-envelope'></i></a></td>";
                        }
                        else
                        {
                            html += "<td><a onclick=\"editar(" + detalle.IDComprobante.ToString() + ");\" style='cursor:pointer; font-size:16px' title='Editar'><i class='fa fa-pencil'></i></a></td>";
                            html += "<td></td>";
                        }                       
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes facturas procesadas automaticamente.</td></tr>";

        }
        return html;
    }

    [WebMethod(true)]
    public static string obtenerFacturasSinCAE(int idPersona, string fechaDesdeComprobante)
    {
        var html = string.Empty;
        DateTime dFechaDesdeComprobante = new DateTime(1900,01,01);
        if (!string.IsNullOrEmpty(fechaDesdeComprobante))
        {
            if (!DateTime.TryParse(fechaDesdeComprobante, out dFechaDesdeComprobante))
            {
                throw new CustomException("Fecha desde comprobante invalida.");
            }
        }

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var tipoComprobantesIncluidos = new[] { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "RCAMP", "RCBMP", "RCCMP", "NDAMP", "NDBMP", "NDCMP" };
                List<Comprobantes> list = new List<Comprobantes>();

                if (idPersona != 0)                
                    list = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.IDPersona == idPersona && x.FechaComprobante >= dFechaDesdeComprobante).ToList();
                else                
                    list = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.FechaComprobante >= dFechaDesdeComprobante).ToList();

                DataTable dt = list.GroupBy(g => g.FechaComprobante).ToList().Select(x => new
                     {
                         FechaComprobante = x.Key,
                         FechaComprobanteString = x.Key.ToString("dd/MM/yyyy"),
                         Cantidad = x.Count(),
                         ImporteTotal = x.Sum(s => s.ImporteTotalNeto)
                     }).OrderBy(x => x.FechaComprobante).ToList().ToDataTable();                                    

                if (dt.Rows.Count > 0)
                {
                    int index = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        html += "<tr>";
                        html += "<td>" + row["FechaComprobanteString"].ToString() + "</td>";
                        html += "<td>" + row["Cantidad"].ToString() + "</td>";
                        html += "<td>" + row["ImporteTotal"].ToString() + "</td>";

                        if (index == 1)
                        {
                            html += "<td><a class='btn btn-success' style='cursor:pointer; font-size:16px' onclick=\"procesar('" + row["FechaComprobanteString"].ToString() + "');\" title='Emitir comprobante electrónico' id='lnkProcesar'>Emitir comprobante electrónico</a></td>";
                            html += "<td><a class='btn btn-warning' style='cursor:pointer; font-size:16px' onclick=\"eliminarComprobante('" + row["FechaComprobanteString"].ToString() + "');\" title='Eliminar comprobante' id='lnkEliminarComprobante'>Eliminar comprobante</a></td>";
                            html += "<td><a class='btn btn-info' style='cursor:pointer; font-size:16px' onclick=\"modificarFechaComprobante('" + row["FechaComprobanteString"].ToString() + "');\" title='Modificar Fecha Comprobante' id='lnkModificarFechaComprobante'>Modificar Fecha Comprobante</a></td>";
                        }
                            

                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes facturas sin generar CAE.</td></tr>";

        }
        return html;
    }          

    [WebMethod(true)]
    public static string procesar(int idPersona, string fechaComprobante)
    {
        string html = string.Empty;
        DateTime fComprobante;
        List<int?> listaComprobantes = new List<int?>();
        long procesoFacturaAutomatica = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"));

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (!DateTime.TryParse(fechaComprobante, out fComprobante))
                {
                    throw new CustomException("Fecha de comprobante invalida.");
                }

                using (var dbContext = new ACHEEntities())
                {
                    string nroComprobante = "";
                    var tipoComprobantesIncluidos = new[] { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "RCAMP", "RCBMP", "RCCMP", "NDAMP", "NDBMP", "NDCMP" };

                    List<Comprobantes> listEmitirCAE = new List<Comprobantes>();
                    if (idPersona != 0)
                        listEmitirCAE = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.IDPersona == idPersona && x.FechaComprobante == fComprobante).ToList();
                    else
                        listEmitirCAE = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.FechaComprobante == fComprobante).ToList();


                    if (listEmitirCAE.Any()) //Recorro los comprobantes
                    {
                        foreach (Comprobantes comp in listEmitirCAE)
                        {
                            listaComprobantes.Add(comp.IDComprobante);
                            try
                            {
                                nroComprobante = "";
                                ComprobanteCart.Retrieve().Items.Clear();

                                List<ComprobantesDetalle> cd = dbContext.ComprobantesDetalle.Where(w => w.IDComprobante == comp.IDComprobante).ToList();

                                foreach (var det in cd)
                                {
                                    var tra = new ComprobantesDetalleViewModel();
                                    tra.ID = ComprobanteCart.Retrieve().Items.Count() + 1;
                                    tra.Concepto = det.Concepto;
                                    tra.Codigo = (det.Conceptos != null) ? det.Conceptos.Codigo : "";
                                    tra.CodigoPlanCta = (det.PlanDeCuentas == null) ? "" : det.PlanDeCuentas.Codigo + " - " + det.PlanDeCuentas.Nombre;
                                    tra.Iva = det.Iva;
                                    tra.PrecioUnitario = det.PrecioUnitario;
                                    tra.Bonificacion = det.Bonificacion;
                                    tra.Cantidad = det.Cantidad;
                                    tra.IDConcepto = det.IDConcepto;
                                    tra.IDPlanDeCuenta = det.IDPlanDeCuenta;
                                    tra.IdTipoIva = (int)det.IdTipoIVA;
                                    ComprobanteCart.Retrieve().Items.Add(tra);
                                }

                                Common.CrearComprobante(usu, comp.IDComprobante, comp.IDPersona, comp.Tipo, comp.Modo,
                                    comp.FechaComprobante.ToString("dd/MM/yyyy"), comp.CondicionVenta, comp.TipoConcepto,
                                    comp.FechaVencimiento.ToString("dd/MM/yyyy"), comp.IDPuntoVenta, 
                                    ref nroComprobante, comp.Observaciones, 0, "","",false, comp.IdActividad, comp.ModalidadPagoAFIP, Common.ComprobanteModo.Generar);

                                using (var dbContextProcesoFacturaAutomatica = new ACHEEntities())
                                {
                                    Comprobantes c = dbContextProcesoFacturaAutomatica.Comprobantes.Where(w => w.IDComprobante == comp.IDComprobante).FirstOrDefault();

                                    if (c != null)
                                    {
                                        if (c.CAE != null)
                                        {
                                            c.ProcesoFacturaAutomatica = procesoFacturaAutomatica;
                                            dbContextProcesoFacturaAutomatica.SaveChanges();
                                        }
                                    }
                                }

                            }
                            catch (CustomException ex)
                            {
                                try
                                {
                                    Comprobantes c = dbContext.Comprobantes.Where(x => x.IDComprobante == comp.IDComprobante).First();
                                    if (c != null)
                                    {
                                        c.Error = ex.Message;
                                        dbContext.SaveChanges();
                                    }
                                }
                                catch { }

                                throw new CustomException(ex.Message);

                            }

                        }

                    }
                    else
                        throw new CustomException("No se encontraron comprobantes.");


                    dbContext.SaveChanges();

                }

                using (var dbContextActualizado = new ACHEEntities())
                {
                    List<Comprobantes> listActualizado = dbContextActualizado.Comprobantes.Where(x => listaComprobantes.Contains(x.IDComprobante)).ToList();

                    foreach (Comprobantes comp in listActualizado)
                    {
                        html += "<tr>";
                        html += "<td>" + comp.Tipo + "</td>";
                        html += "<td>" + comp.PuntosDeVenta.Punto.ToString("#0000") + "-" + comp.Numero.ToString("#00000000") + "</td>";
                        if (usu.CUIT.Equals("30716909839"))
                        {
                            decimal Total1000 = comp.ImporteTotalNeto / 1000;
                            html += "<td>" + Total1000.ToString("N2") + "</td>";
                        }
                        else
                            html += "<td>" + comp.ImporteTotalNeto.ToString("N2") + "</td>";
                        html += "<td>" + comp.CAE + "</td>";
                        html += "<td>" + comp.Error + "</td>";
                        html += "</tr>";
                    }

                }

                return html;

            }
            catch (CustomException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # facturaAutomatica.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string modificarFechaComprobante(int idPersona, string fechaComprobante, string fechaComprobanteModificada)
    {
        string html = string.Empty;
        DateTime fComprobante, fComprobanteModificada;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {

                if (!DateTime.TryParse(fechaComprobanteModificada, out fComprobanteModificada))
                {
                    throw new CustomException("Fecha de comprobante modificada invalida.");
                }

                if (!DateTime.TryParse(fechaComprobante, out fComprobante))
                {
                    throw new CustomException("Fecha de comprobante invalida.");
                }

                if (fComprobante == fComprobanteModificada)
                {
                    throw new CustomException("La fecha ingresada es igual a la fecha de comprobante seleccionada.");
                }

                using (var dbContext = new ACHEEntities())
                {
                    var tipoComprobantesIncluidos = new[] { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "RCAMP", "RCBMP", "RCCMP", "NDAMP", "NDBMP", "NDCMP" };
                    List<Comprobantes> list = new List<Comprobantes>();

                    if (idPersona != 0)
                        list = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.IDPersona == idPersona && x.FechaComprobante == fComprobante).ToList();
                    else
                        list = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.FechaComprobante == fComprobante).ToList();

                    if (list.Any()) //Recorro los comprobantes
                    {
                        foreach (Comprobantes comp in list)
                        {
                            comp.FechaComprobante = fComprobanteModificada;
                            comp.FechaVencimiento = fComprobanteModificada.AddMonths(1);
                        }
                    }
                    dbContext.SaveChanges();

                }

                return html;

            }
            catch (CustomException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # facturaAutomatica.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string eliminarComprobante(int idPersona, string fechaComprobante)
    {
        string html = string.Empty;
        DateTime fComprobante;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {

                if (!DateTime.TryParse(fechaComprobante, out fComprobante))
                {
                    throw new CustomException("Fecha de comprobante invalida.");
                }

                using (var dbContext = new ACHEEntities())
                {
                    var tipoComprobantesIncluidos = new[] { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "RCAMP", "RCBMP", "RCCMP", "NDAMP", "NDBMP", "NDCMP" };
                    List<Comprobantes> list = new List<Comprobantes>();

                    if (idPersona != 0)
                        list = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.IDPersona == idPersona && x.FechaComprobante == fComprobante).ToList();
                    else
                        list = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && tipoComprobantesIncluidos.Contains(x.Tipo) && x.CAE == null && x.FechaComprobante == fComprobante).ToList();

                    if (list.Any()) //Recorro los comprobantes
                    {
                        foreach (Comprobantes comp in list)
                        {
                            var cobdet = dbContext.CobranzasDetalle.Where(w => w.IDComprobante == comp.IDComprobante).ToList();

                            foreach (CobranzasDetalle cd in cobdet)
                            {
                                var cob = dbContext.Cobranzas.Where(w => w.IDCobranza == cd.IDCobranza).FirstOrDefault();

                                if(cob != null)
                                {
                                    var cobdets = dbContext.CobranzasDetalle.Where(w => w.IDCobranza == cob.IDCobranza).ToList();
                                    if (cobdets != null)
                                        dbContext.CobranzasDetalle.RemoveRange(cobdets);

                                    var cobforms = dbContext.CobranzasFormasDePago.Where(w => w.IDCobranza == cob.IDCobranza).ToList();
                                    if (cobforms != null)
                                        dbContext.CobranzasFormasDePago.RemoveRange(cobforms);

                                    var cobrets = dbContext.CobranzasRetenciones.Where(w => w.IDCobranza == cob.IDCobranza).ToList();
                                    if (cobrets != null)
                                        dbContext.CobranzasRetenciones.RemoveRange(cobrets);

                                    dbContext.SaveChanges();
                                    dbContext.Cobranzas.Remove(cob);
                                    dbContext.SaveChanges();
                                }
                            }

                            var comdet = dbContext.ComprobantesDetalle.Where(w => w.IDComprobante == comp.IDComprobante).ToList();

                            if (comdet != null)
                            {
                                dbContext.ComprobantesDetalle.RemoveRange(comdet);
                                dbContext.SaveChanges();
                            }

                            dbContext.Comprobantes.Remove(comp);
                            dbContext.SaveChanges();
                        }
                    }

                }

                return html;

            }
            catch (CustomException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # facturaAutomatica.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



    [WebMethod(true)]
    public static string enviarFacturaPorCorreoElectronico(int idComprobante)
    {
        string html = string.Empty;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                //Common.EnviarComprobantePorMail(nombre, para, asunto, mensaje, file);

                using (var dbContext = new ACHEEntities())
                {
                    Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == idComprobante).FirstOrDefault();

                    if (c != null)
                    {

                        var file = c.Personas.RazonSocial.RemoverCaracteresParaPDF() + "_" + c.Tipo + "-" + c.PuntosDeVenta.Punto.ToString().PadLeft(4, '0') + "-" + c.Numero.ToString().PadLeft(8, '0') + ".pdf";
                        var asunto = "Factura " + c.PuntosDeVenta.Punto.ToString().PadLeft(4, '0') + "-" + c.Numero.ToString().PadLeft(8, '0');
                        var mensaje = "Se adjunta el comprobante del asunto.";
                        var nombre = usu.RazonSocial;

                        MailAddressCollection listTo = new MailAddressCollection();
                        foreach (var mail in c.Personas.Email.Split(','))
                        {
                            if (mail != string.Empty)
                                listTo.Add(new MailAddress(mail));
                        }

                        ListDictionary replacements = new ListDictionary();
                        replacements.Add("<NOTIFICACION>", mensaje);
                        replacements.Add("<USUARIO>", nombre);
                        replacements.Add("<LOGOEMPRESA>", "/files/usuarios/" + usu.Logo);

                        List<string> attachments = new List<string>();
                        attachments.Add(HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/" + file));

                        bool send = EmailHelper.SendMessage(EmailTemplate.EnvioComprobanteConFoto, replacements, listTo, ConfigurationManager.AppSettings["Email.Notifications"], usu.Email, asunto, attachments);
                        if (!send)
                            throw new Exception("El mensaje no pudo ser enviado. Por favor, intente nuevamente. En caso de continuar el error, escribenos a <a href='" + ConfigurationManager.AppSettings["Email.Ayuda"] + "'>" + ConfigurationManager.AppSettings["Email.Ayuda"] + "</a>");
                    }
                    else
                        throw new Exception("No se encontró el comprobante " + idComprobante.ToString());
                }

                return html;

            }
            catch (CustomException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # facturaAutomatica.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



}