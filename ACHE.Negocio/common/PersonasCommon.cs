using System;
using System.Collections.Generic;
using System.Linq;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Facturacion;
using ACHE.Model.Negocio;
using System.Configuration;
using System.Web;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using ACHE.FacturaElectronica.WSPersonaServiceA5v34;
using static iTextSharp.text.pdf.AcroFields;

namespace ACHE.Negocio.Common
{
    public static class PersonasCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","


        public static Personas Guardar(ACHEEntities dbContext, int idUsuario, int idCliente, string tipo)
        {
            try
            {
                Personas persona = new Personas();
                var usuContabilium = dbContext.Usuarios.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
                var Usuario = dbContext.Usuarios.Where(x => x.IDUsuario == idCliente).FirstOrDefault();
                var cliente = dbContext.Personas.Where(x => x.NroDocumento == Usuario.CUIT && x.IDUsuario == usuContabilium.IDUsuario).FirstOrDefault();

                if (cliente != null)
                    persona = cliente;
                else if (usuContabilium != null && Usuario != null)
                {
                    var entity = new Personas();
                    entity.IDUsuario = usuContabilium.IDUsuario;
                    entity.FechaAlta = DateTime.Now.Date;

                    entity.Tipo = tipo;
                    entity.RazonSocial = Usuario.RazonSocial.ToUpper();
                    entity.NombreFantansia = Usuario.RazonSocial.ToUpper();
                    entity.CondicionIva = Usuario.CondicionIva;
                    entity.TipoDocumento = "CUIT";
                    entity.NroDocumento = Usuario.CUIT;
                    entity.Personeria = Usuario.Personeria;
                    entity.Email = Usuario.Email.ToLower();
                    entity.EmailsEnvioFc = usuContabilium.EmailAlertas.ToLower();
                    entity.Telefono = Usuario.Telefono;
                    entity.Observaciones = Usuario.Observaciones;
                    entity.EmailsEnvioFc = Usuario.Email.ToLower();
                    entity.TipoComprobanteDefecto = "";
                    entity.AlicuotaIvaDefecto = "";
                    entity.Codigo = "";
                    //Domicilio
                    entity.IDProvincia = Usuario.IDProvincia;
                    entity.IDCiudad = Usuario.IDCiudad;
                    entity.Domicilio = Usuario.Domicilio;
                    entity.PisoDepto = Usuario.PisoDepto;
                    entity.CodigoPostal = Usuario.CodigoPostal;
                    entity.SaldoInicial = 0;
                    //lista Precio
                    entity.IDListaPrecio = null;

                    dbContext.Personas.Add(entity);
                    dbContext.SaveChanges();
                    persona = entity;
                }
                return persona;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static Personas ObtenerPersonaPorNroDoc(string nroDoc, int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                    return dbContext.Personas.Include("Provincias").Include("Ciudades").Where(x => x.IDUsuario == idUsuario && x.NroDocumento == nroDoc).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static ResultadosPersonasViewModel ObtenerPersonas(string condicion, string tipo, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipo.ToUpper()).AsQueryable();


                if (!string.IsNullOrWhiteSpace(condicion))
                    results = results.Where(x => x.RazonSocial.Contains(condicion.Trim()) || x.NombreFantansia.Contains(condicion.Trim()) || x.NroDocumento.Contains(condicion.Trim()) || x.Codigo.Contains(condicion.Trim()));


                var list = results.Select(x => new PersonasViewModel()
                {
                    ID = x.IDPersona,
                    RazonSocial = x.RazonSocial.ToUpper(),
                    //NombreFantasia = (x.CondicionIva == "RI" || x.CondicionIva == "EX") ? x.NombreFantansia.ToUpper() : "",
                    NombreFantasia = x.NombreFantansia.ToUpper(),
                    NroDoc = x.NroDocumento,
                    CondicionIva = x.CondicionIva,
                    Telefono = x.Telefono,
                    Email = x.Email.ToLower(),
                    TieneFoto = (x.Foto == null || x.Foto == "") ? "0" : "1",
                    Codigo = x.Codigo,
                    Provincia = (x.Provincias != null) ? x.Provincias.Nombre : "",
                    Ciudad = (x.Ciudades != null) ? x.Ciudades.Nombre : "",
                    Domicilio = x.Domicilio
                });

                page--;

                ResultadosPersonasViewModel resultado = new ResultadosPersonasViewModel();
                resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = list.Count();
                resultado.Items = list.OrderBy(x => x.RazonSocial).Skip(page * pageSize).Take(pageSize).ToList();

                return resultado;
            }
        }

        public static ResultadosCuentaCorrienteViewModel ObtenerPersonasCuentaCorriente(string condicion, string tipo, 
                                                                        bool saldoPendiente, bool deudaPorEDM,
                                                                        int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                var tipoComprobantesNoIncluidos = new[] { "PDC", "DDC", "NDP", "COT", "RCB", "RCC" };

                var listaComprobantes = dbContext.Comprobantes
                            .Include("Personas")
                            .Where(x => x.IDUsuario == usu.IDUsuario && !tipoComprobantesNoIncluidos.Contains(x.Tipo)).ToList();
                var listaCobranzas = dbContext.CobranzasDetalle.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario).ToList();

                var listaCobranzasRetenciones = dbContext.CobranzasRetenciones.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario).ToList();

                var listaCheques = dbContext.RptChequesAcciones.Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario && x.EsPropio == false).ToList();

                var listaPersonas = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).ToList();


                if (!string.IsNullOrWhiteSpace(condicion))
                    results = results.Where(x => x.RazonSocial.Contains(condicion.Trim()) || x.NombreFantansia.Contains(condicion.Trim()) || x.NroDocumento.Contains(condicion.Trim()) || x.Codigo.Contains(condicion.Trim()));


                List<CuentaCorrienteViewModel> lr = new List<CuentaCorrienteViewModel>();
                foreach (Personas p in results)
                {
                    CuentaCorrienteViewModel cuenta = new CuentaCorrienteViewModel();
                    ResultadosRptCcDetalleViewModel r = new ResultadosRptCcDetalleViewModel();
                    r.TotalPage = 1;// ((results.Count() - 1) / pageSize) + 1;
                    r.Items = new List<RptCcDetalleViewModel>();

                    if (tipo == "C")
                        verComoClienteResumido(p.IDPersona, r, dbContext, deudaPorEDM, listaComprobantes, listaCobranzas, listaCobranzasRetenciones, listaCheques, listaPersonas);
                    else
                        verComoProveedor(p.IDPersona, r, dbContext, deudaPorEDM);

                    if(r.Items.Count() > 0)
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
           
                page--;

                ResultadosCuentaCorrienteViewModel resultado = new ResultadosCuentaCorrienteViewModel();
                resultado.TotalPage = ((lr.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = lr.Count();
                if(saldoPendiente)
                    resultado.Items = lr.Where(w => !w.Saldo.Equals("0,00")).OrderBy(x => x.RazonSocial).Skip(page * pageSize).Take(pageSize).ToList();
                else
                    resultado.Items = lr.OrderBy(x => x.RazonSocial).Skip(page * pageSize).Take(pageSize).ToList();

                return resultado;
            }
        }

        public static void verComoClienteResumido(int idPersona, ResultadosRptCcDetalleViewModel resultado,
                                    ACHEEntities dbContext, bool deudaPorEDM, List<Comprobantes> listaComprobantesDb,
                                    List<CobranzasDetalle> listaCobranzasDb, List<CobranzasRetenciones> listaCobranzasRetenciones,
                                    List<RptChequesAcciones> listaChequesDb, List<Personas> listaPersonasDb)
        {
            decimal total = 0;
            RptCcDetalleViewModel detalleCc;

            string[] listaNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
            string[] listaNotasDebito = { "NDA", "NDB", "NDC", "NDAMP", "NDBMP", "NDCMP" };

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            if (deudaPorEDM)
            {
                var listaComprobantes = listaComprobantesDb
                                            .Where(x => (x.IDPersona == idPersona || idPersona == -1)).ToList();
                var listaCobranzas = listaCobranzasDb.Where(x => (x.Cobranzas.IDPersona == idPersona || idPersona == -1)).ToList();

                var list = listaComprobantes
                            .OrderBy(x => x.FechaComprobante)
                            .ToList();
                var persona = listaPersonasDb.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                resultado.TotalItems = list.Count();

                if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
                {
                    resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                    total = Convert.ToDecimal(persona.SaldoInicial);
                    resultado.TotalItems++;
                }


                if (list.Any())
                {
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
                            }
                            else // No hay comprobantes que esten vinculados a este
                            {
                                if (detalle.Tipo.Equals("PDV"))
                                {
                                }
                                else
                                {
                                    if (detalle.Tipo.Equals("EDA"))
                                    {
                                        total += detalle.ImporteTotalBruto;
                                    }
                                    else
                                    {
                                        if (detalle.CAE != null)
                                        {
                                            total += detalle.ImporteTotalNeto;
                                        }
                                        else
                                        {
                                            total += detalle.ImporteTotalBruto;
                                        }
                                    }
                                }
                            }

                        }
                        else //Es Nota de credito o debito
                        {
                            if (detalle.CAE != null)
                            {
                                if (listaNotasDebito.Contains(detalle.Tipo))
                                    total += detalle.ImporteTotalNeto;
                                else
                                    total -= detalle.ImporteTotalNeto;
                            }
                            else
                            {
                                if (listaNotasDebito.Contains(detalle.Tipo))
                                    total += detalle.ImporteTotalBruto;
                                else
                                    total -= detalle.ImporteTotalBruto;
                            }
                        }

                        var cobrazasList = listaCobranzas.Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                        foreach (var cobranza in cobrazasList)
                        {
                            total -= cobranza.Importe;

                            if(listaCobranzasRetenciones.Where(x => x.IDCobranza == cobranza.IDCobranza).Any())
                            {
                                total -= cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                                listaCobranzasRetenciones.RemoveAll(w => w.IDCobranza == cobranza.IDCobranza);
                            }                            
                        }
                    }

                    var chequesResto = listaChequesDb.Where(w => w.IdPersona == idPersona).Sum(a => a.Resto);

                    if (chequesResto != 0)
                    {
                        total += (decimal)chequesResto;
                    }

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.RazonSocial = persona.RazonSocial;
                    detalleCc.Cobrado = "Saldo";
                    detalleCc.Total = total.ToString("N2");
                    resultado.Items.Add(detalleCc);


                }

            }
            else // No es por Entrega de mercaderia
            {
                var listaComprobantes = listaComprobantesDb.Where(x => (x.IDPersona == idPersona || idPersona == -1)).ToList();
                var listaCobranzas = listaCobranzasDb.Where(x => (x.Cobranzas.IDPersona == idPersona || idPersona == -1)).ToList();

                var list = listaComprobantes
                            .Where(x => !x.Tipo.Equals("EDA"))
                            .OrderBy(x => x.FechaComprobante)
                            .ToList();
                var persona = listaPersonasDb.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                resultado.TotalItems = list.Count();

                if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
                {
                    resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                    total = Convert.ToDecimal(persona.SaldoInicial);
                    resultado.TotalItems++;
                }

                if (list.Any())
                {
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
                            //                                .Select(a => (int?)a.IDComprobante)
                            //                                .ToList();


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
                                }
                                else
                                {
                                    if (detalle.CAE != null)
                                    {
                                        total += detalle.ImporteTotalNeto - ((detalle.ImporteTotalNeto - comprobantesVinculados.SumaNeto) < 0 ? detalle.ImporteTotalNeto : comprobantesVinculados.SumaNeto);
                                    }
                                    else
                                    {
                                        total += detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto);
                                    }

                                }

                            }
                            else // El comprobante no está vinculado a otro comprobante
                            {
                                if (detalle.Tipo.Equals("PDV"))
                                {
                                    total += detalle.ImporteTotalBruto;
                                }
                                else
                                {

                                    if (detalle.CAE != null)
                                    {
                                        total += detalle.ImporteTotalNeto;
                                    }
                                    else
                                    {
                                        total += detalle.ImporteTotalBruto;
                                    }

                                }

                            }

                        }
                        else // Es una nota de credito o debito
                        {
                            if (detalle.CAE != null)
                            {
                                if (listaNotasDebito.Contains(detalle.Tipo))
                                    total += detalle.ImporteTotalNeto;
                                else
                                    total -= detalle.ImporteTotalNeto;
                            }
                            else
                            {
                                if (listaNotasDebito.Contains(detalle.Tipo))
                                    total += detalle.ImporteTotalBruto;
                                else
                                    total -= detalle.ImporteTotalBruto;
                            }

                        }

                        var cobrazasList = listaCobranzas.Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                        foreach (var cobranza in cobrazasList)
                        {
                            total -= cobranza.Importe;
                            if (listaCobranzasRetenciones.Where(x => x.IDCobranza == cobranza.IDCobranza).Any())
                            {
                                total -= cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                                listaCobranzasRetenciones.RemoveAll(w => w.IDCobranza == cobranza.IDCobranza);
                            }
                        }
                    }

                    var chequesResto = listaChequesDb.Where(w => w.IdPersona == idPersona).Sum(a => a.Resto);

                    if (chequesResto > 0)
                    {
                        total += (decimal)chequesResto;
                    }

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.RazonSocial = persona.RazonSocial;
                    detalleCc.Cobrado = "Saldo";
                    detalleCc.Total = total.ToString("N2");
                    resultado.Items.Add(detalleCc);


                }

            }

        }




        public static void verComoCliente(int idPersona, ResultadosRptCcDetalleViewModel resultado, 
                                            ACHEEntities dbContext, bool deudaPorEDM, List<Comprobantes> listaComprobantesDb, 
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

            if (deudaPorEDM)
            {

                var listaComprobantes = listaComprobantesDb
                                            .Where(x => (x.IDPersona == idPersona || idPersona == -1)).ToList();
                var listaCobranzas = listaCobranzasDb.Where(x => (x.Cobranzas.IDPersona == idPersona || idPersona == -1)).ToList();

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
                        else //Es Nota de credito
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

                    var chequesResto = listaChequesDb.Where(w => w.IdPersona == idPersona).Sum(a => a.Resto);

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
                var listaComprobantes = listaComprobantesDb.Where(x => (x.IDPersona == idPersona || idPersona == -1)).ToList();
                var listaCobranzas = listaCobranzasDb.Where(x => (x.Cobranzas.IDPersona == idPersona || idPersona == -1)).ToList();

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
                            //                                .Select(a => (int?)a.IDComprobante)
                            //                                .ToList();


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
                        else // Es una nota de credito o debito
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
                                detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
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

                    var chequesResto = listaChequesDb.Where(w => w.IdPersona == idPersona).Sum(a => a.Resto);

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


        public static RptCcDetalleViewModel AgregarSaldoInicialCliente(ACHEEntities dbContext, decimal? saldo)
        {
            var detalleCc = new RptCcDetalleViewModel();
            detalleCc.Comprobante = "Saldo inicial";
            detalleCc.Fecha = "";
            detalleCc.Importe = Convert.ToDecimal(saldo).ToString("N2");
            detalleCc.Total = Convert.ToDecimal(saldo).ToString("N2");

            return detalleCc;
        }

        public static void verComoProveedor(int idPersona, ResultadosRptCcDetalleViewModel resultado, 
                                            ACHEEntities dbContext, bool deudaPorEDM)
        {

            decimal total = 0;
            RptCcDetalleViewModel detalleCc;
            RptCcDetalleViewModel detalleCob;
            //string[] listaNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
            string[] listaNotas = { "NCA", "NCB", "NCC", "NCAMP", "NCBMP", "NCCMP" };

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
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
                

                foreach (var detalle in list)
                {
                    var imp = detalle.TotalImpuestos;

                    if (listaNotas.Contains(detalle.Tipo))
                        total -= Convert.ToDecimal(detalle.Total + detalle.Iva + imp);
                    else
                        total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                    detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");

                    if (listaNotas.Contains(detalle.Tipo))
                        detalleCc.Importe = "-" + Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                    else
                        detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");


                    detalleCc.Total = total.ToString("N2");
                    resultado.Items.Add(detalleCc);

                    

                    var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                    foreach (var item in pagosList)
                    {
                        total -= item.Importe;

                        detalleCob = new RptCcDetalleViewModel();
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
                        resultado.Items.Add(detalleCob);                     

                    }


                }

                //var chequesResto = dbContext.RptChequesAcciones
                //            .Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario && x.EsPropio == true)
                //            .Sum(a => a.Resto);

                //if (usu.CUIT.Equals("30716909839"))
                //{
                //    chequesResto = chequesResto / 1000;
                //}


                //if (chequesResto != null)
                //{

                //    total += (decimal)chequesResto;

                //    detalleCc = new RptCcDetalleViewModel();
                //    detalleCc.Cobrado = "Resto de cheques sin pago asignado";
                //    detalleCc.Total = total.ToString("N2");
                //    resultado.Items.Add(detalleCc);

                //}

                detalleCc = new RptCcDetalleViewModel();
                detalleCc.Cobrado = "Saldo";
                detalleCc.Total = total.ToString("N2");
                resultado.Items.Add(detalleCc);
            }
        }

        public static bool EliminarPersona(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.AbonosPersona.Any(x => x.IDPersona == id))
                    throw new CustomException("No se puede eliminar por tener Abonos asociados.");
                else if (dbContext.TrackingHoras.Any(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario))
                    throw new CustomException("No se puede eliminar por tener Tracking de horas asociadas.");
                else if (dbContext.Presupuestos.Any(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario))
                    throw new CustomException("No se puede eliminar por tener Presupuestos asociados.");
                else if (dbContext.Comprobantes.Any(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario))
                    throw new CustomException("No se puede eliminar por tener Ventas asociadas.");
                else if (dbContext.Compras.Any(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario))
                    throw new CustomException("No se puede eliminar por tener Compras asociados.");
                else if (dbContext.Pagos.Any(x => x.IDPersona == id))
                    throw new CustomException("No se puede eliminar por tener Pagos asociados.");
                else if (dbContext.Cobranzas.Any(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario))
                    throw new CustomException("No se puede eliminar por tener Cobranzas asociadas.");
                else
                {
                    var entity = dbContext.PersonaDomicilio.Where(x => x.IDPersona == id).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.PersonaDomicilio.Remove(entity);
                        dbContext.SaveChanges();
                        var enty = dbContext.Personas.Where(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (enty != null)
                        {
                            dbContext.Personas.Remove(enty);
                            dbContext.SaveChanges();
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        var enty = dbContext.Personas.Where(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (enty != null)
                        {
                            dbContext.Personas.Remove(enty);
                            dbContext.SaveChanges();
                            return true;
                        }
                        else
                            return false;

                    }
                                      
                }
            }
        }

        public static bool EliminarDomicilio(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.Comprobantes.Any(x => x.IdDomicilio == id))
                    throw new CustomException("No se puede eliminar por tener comprobantes asociados.");              
                else
                {
                    var entity = dbContext.PersonaDomicilio.Where(x => x.IdPersonaDomicilio == id).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.PersonaDomicilio.Remove(entity);
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
        }

        public static bool EliminarDomicilioTransporte(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.Comprobantes.Any(x => x.IdTransportePersona == id))
                    throw new CustomException("No se puede eliminar por tener comprobantes asociados.");
                else
                {
                    var entity = dbContext.TransportePersona.Where(x => x.IdTransportePersona == id).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.TransportePersona.Remove(entity);
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
        }

        #region Crear datos para el cliente
        public static void CrearDatosParaElCliente(string idMP, int IdUsuario, ACHEEntities dbContext, PlanesPagos planesPagos, Comprobantes comprobante, string nroComprobante)
        {
            // ** CREACION DE PROVEEDORES FACTURA Y COBRANZA ** //
            var idContabilium = Convert.ToInt32(ConfigurationManager.AppSettings["Usu.idContabilium"]);
            var usuario = dbContext.Usuarios.Where(x => x.IDUsuario == IdUsuario).FirstOrDefault();

            // ** CREACION DE USUARIO ** //
            var proveedor = PersonasCommon.Guardar(dbContext, IdUsuario, idContabilium, "P");
            // ** CREACION DE COMPROBANTE DE COMPRA ** //
            var compra = CrearCompra(dbContext, comprobante, nroComprobante, planesPagos, proveedor, usuario);
            // ** CREACION DE PAGO ** //
            CrearPago(dbContext, compra, proveedor, idMP, planesPagos, usuario);
        }
        private static Compras CrearCompra(ACHEEntities dbContext, Comprobantes comprobante, string nroComprobante, PlanesPagos planesPagos, Personas proveedor, Usuarios usuario)
        {
            var precioUnitario = comprobante.ImporteTotalBruto;

            int id = 0;
            int idPersona = proveedor.IDPersona;
            string fecha = DateTime.Now.Date.ToString(formatoFecha);
            string fechaPrimerVencimiento = DateTime.Now.AddDays(15).Date.ToString(formatoFecha);
            string fechaSegundoVencimiento = DateTime.Now.AddDays(30).Date.ToString(formatoFecha);
            string nroFactura = nroComprobante;
            string iva = "0";
            string importe2 = "0";
            string importe5 = "0";
            string importe10 = "0";
            string importe21 = "0";
            string importe27 = "0";
            string noGrav = "0";
            string importeMon = "0";
            string impNacional = "0";
            string impMunicipal = "0";
            string impInterno = "0";

            string percepcionIva = "0";
            string otros = "0";
            string obs = "Compra generada automáticamente por Contabilium, " + "Plan: " + planesPagos.Planes.Nombre;
            string tipo = comprobante.Tipo;
            string idCategoria = "";
            string rubro = "Servicios";
            string exento = "0";
            string FechaEmision = DateTime.Now.Date.ToString(formatoFecha);
            int idUsuario = usuario.IDUsuario;
            int idPlanDeCuenta = 0;
            List<JurisdiccionesViewModel> Jurisdicciones = new List<JurisdiccionesViewModel>();
            switch (usuario.CondicionIva)
            {
                case "RI":
                    importe21 = precioUnitario.ToString();
                    iva = ((precioUnitario * 21) / 100).ToString();
                    break;
                default:
                    importeMon = (precioUnitario + ((precioUnitario * 21) / 100)).ToString();
                    break;
            }

            var compras = ComprasCommon.Guardar(id, idPersona, fecha, nroFactura, iva, importe2, importe5, importe10, importe21, importe27, noGrav, importeMon,
            impNacional, impMunicipal, impInterno, percepcionIva, otros, obs, tipo, idCategoria, rubro, exento, FechaEmision, idPlanDeCuenta, idUsuario, Jurisdicciones, fechaPrimerVencimiento, fechaSegundoVencimiento, null);

            return compras;
        }
        private static void CrearPago(ACHEEntities dbContext, Compras compra, Personas Proveedor, string idMp, PlanesPagos planesPagos, Usuarios usuario)
        {
            var usu = TokenCommon.ObtenerWebUser(usuario.IDUsuario);

            PagosCartDto PagosCart = new PagosCartDto();
            PagosCart.IDPago = 0;
            PagosCart.IDPersona = Proveedor.IDPersona;
            PagosCart.Observaciones = "Pago generado automaticamente desde Contabilium";
            PagosCart.FechaPago = DateTime.Now.Date.ToString(formatoFecha);

            PagosCart.Items = new List<PagosDetalleViewModel>();
            PagosCart.FormasDePago = new List<PagosFormasDePagoViewModel>();
            PagosCart.Retenciones = new List<PagosRetencionesViewModel>();

            PagosCart.Items.Add(new PagosDetalleViewModel()
            {
                ID = 1,
                IDCompra = compra.IDCompra,
                Importe = Convert.ToDecimal(planesPagos.ImportePagado),
                nroFactura = compra.NroFactura
            });
            PagosCart.FormasDePago.Add(new PagosFormasDePagoViewModel()
            {
                ID = 1,
                Importe = Convert.ToDecimal(planesPagos.ImportePagado),
                NroReferencia = idMp,
                FormaDePago = planesPagos.FormaDePago
            });

            PagosCommon.Guardar(dbContext, PagosCart, usu);
        }

        public static bool ValidaCuit(string cuit)
        {
            if (cuit == null)
            {
                return false;
            }
            //Quito los guiones, el cuit resultante debe tener 11 caracteres.
            cuit = cuit.Replace("-", string.Empty);
            if (cuit.Length != 11)
            {
                return false;
            }
            else
            {
                int calculado = CalcularDigitoCuit(cuit);
                int digito = int.Parse(cuit.Substring(10));
                return calculado == digito;
            }
        }

        public static int CalcularDigitoCuit(string cuit)
        {
            int[] mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            char[] nums = cuit.ToCharArray();
            int total = 0;
            for (int i = 0; i < mult.Length; i++)
            {
                total += int.Parse(nums[i].ToString()) * mult[i];
            }
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

        public static string sugerirNumeroCuitGenerico(int idUsuario)
        {
            string nro = null;
            long doc, cuit = 90000000000;

            using (var dbContext = new ACHEEntities())
            {
                List<Personas> lp = dbContext.Personas.Where(x => x.IDUsuario == idUsuario).Where(x => x.NroDocumento != "").ToList();

                nro = (from c in lp
                        orderby long.Parse(c.NroDocumento) descending
                        select c.NroDocumento).FirstOrDefault();

                if (nro != null)
                {
                    doc = Convert.ToInt64(nro);
                    if (doc < cuit)
                        doc = cuit;
                    else
                        doc++;
                }
                else
                    doc = cuit;

                while (!ValidaCuit(doc.ToString()))
                    doc++;

                nro = doc.ToString();
            }

            return nro;
        }


        public static int GuardarPersonas(int id, string razonSocial, string nombreFantasia, 
            string condicionIva, string personeria, string tipoDoc, string nroDoc,
            string telefono, string email, string tipo,
            int idProvincia, int idCiudad, string provinciaDesc, string ciudadDesc,
            string domicilio, string pisoDepto, 
            string cp, string obs, int listaPrecio, string codigo, decimal saldoInicial, 
            decimal porcentajeDescuento, string avisos, int idRango, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (tipoDoc == "CUIT" && dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.NroDocumento == nroDoc && x.IDPersona != id && x.NroDocumento != ""))
                        throw new CustomException("El CUIT ingresado ya se encuentra registrado.");
                    else if (dbContext.Personas.Any(x => x.Codigo.ToUpper() == codigo.ToUpper() && x.Codigo != "" && x.IDPersona != id && x.IDUsuario == usu.IDUsuario))
                        throw new CustomException("El Código ingresado ya se encuentra registrado.");
                    else if (tipoDoc == "CUIT" && !nroDoc.IsValidCUIT())
                        throw new CustomException("El CUIT es inválido.");

                    Personas entity;
                    if (id > 0)
                        entity = dbContext.Personas.Where(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new Personas();
                        entity.FechaAlta = DateTime.Now;
                        entity.IDUsuario = usu.IDUsuario;
                    }

                    if (nombreFantasia.Equals(""))
                        nombreFantasia = razonSocial;

                    entity.Tipo = tipo.ToUpper();
                    entity.RazonSocial = razonSocial.ToUpper();
                    entity.NombreFantansia = nombreFantasia.ToUpper();
                    entity.CondicionIva = (condicionIva == null || condicionIva == "" || personeria == "null") ? "CF" : condicionIva;


                    if (tipoDoc.Equals("SIN CUIT"))
                    {
                        entity.TipoDocumento = "CUIT";
                        nroDoc = sugerirNumeroCuitGenerico(usu.IDUsuario);
                        while (dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.NroDocumento == nroDoc && x.Tipo == tipo))
                        {
                            long doc = Convert.ToInt64(nroDoc);
                            while (!ValidaCuit(doc.ToString()))
                                doc++;

                            nroDoc = doc.ToString();
                        }
                        entity.NroDocumento = nroDoc;
                    }
                    else
                    {
                        entity.TipoDocumento = tipoDoc;
                        entity.NroDocumento = nroDoc;
                    }

                    entity.Personeria = (personeria == null || personeria == "" || personeria == "null") ? "F" : personeria;
                    entity.Email = email.ToLower();
                    entity.Telefono = telefono;
                    entity.Observaciones = obs;
                    entity.EmailsEnvioFc = "";
                    entity.TipoComprobanteDefecto = "";
                    entity.AlicuotaIvaDefecto = "";
                    entity.Codigo = (codigo == "") ? null : codigo;
                    //Domicilio
                    //entity.IDProvincia = (idProvincia == 0) ? 2 : idProvincia;
                    //entity.IDCiudad = (idCiudad == 0) ? 5003 : idCiudad;
                    entity.IDProvincia = idProvincia;
                    entity.IDCiudad = (idProvincia == 1 && idCiudad == 0) ? 24071 : idCiudad;
                    entity.ProvinciaDesc = provinciaDesc.ToUpper().Trim();
                    entity.CiudadDesc = ciudadDesc.ToUpper().Trim();
                    entity.Domicilio = domicilio.ToUpper();
                    entity.PisoDepto = pisoDepto;
                    entity.CodigoPostal = cp;
                    entity.SaldoInicial = saldoInicial;
                    entity.PorcentajeDescuento = porcentajeDescuento;
                    entity.Avisos = avisos.Trim();
                    entity.IdRango = idRango;

                    //lista Precio
                    if (listaPrecio > 0 && entity.Tipo == "C")
                        entity.IDListaPrecio = listaPrecio;
                    else
                        entity.IDListaPrecio = null;

                    if (id > 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.Personas.Add(entity);
                        dbContext.SaveChanges();
                    }

                    return entity.IDPersona;
                }
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
        #endregion

        public static int GuardarPersonaDomicilio(int id, int idPersona,
            int idProvincia, int idCiudad, string domicilio, string pisoDepto, string cp,
            string provinciaTexto, string ciudadTexto, string contacto, string telefono,
            WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    PersonaDomicilio entity;
                    if (id > 0)
                        entity = dbContext.PersonaDomicilio.Where(x => x.IdPersonaDomicilio == id).FirstOrDefault();
                    else
                    {
                        entity = new PersonaDomicilio();
                        entity.FechaAlta = DateTime.Now;
                        entity.IDPersona = idPersona;
                    }

                    entity.IDProvincia = idProvincia;
                    entity.IDCiudad = (idProvincia == 1 && idCiudad == 0) ? 24071 : idCiudad;
                    entity.Domicilio = domicilio.ToUpper();
                    entity.PisoDepto = pisoDepto;
                    entity.CodigoPostal = cp;
                    entity.Provincia = provinciaTexto.ToUpper();
                    entity.Ciudad = ciudadTexto.ToUpper();
                    entity.Contacto = contacto;
                    entity.Telefono = telefono;


                    if (id > 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.PersonaDomicilio.Add(entity);
                        dbContext.SaveChanges();
                    }

                    return entity.IdPersonaDomicilio;
                }
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

        public static int GuardarPersonaDomicilioTransporte(int id, int idPersona,
            string razonSocial, string domicilio, string pisoDepto, string cp,
            string provinciaTexto, string ciudadTexto, string contacto, string telefono,
            WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    TransportePersona entity;
                    if (id > 0)
                        entity = dbContext.TransportePersona.Where(x => x.IdTransportePersona == id).FirstOrDefault();
                    else
                    {
                        entity = new TransportePersona();
                        entity.FechaAlta = DateTime.Now;
                        entity.IdPersona = idPersona;
                    }

                    entity.RazonSocial = razonSocial.ToUpper();
                    entity.Domicilio = domicilio.ToUpper();
                    entity.PisoDepto = pisoDepto;
                    entity.CodigoPostal = cp;
                    entity.Provincia = provinciaTexto.ToUpper();
                    entity.Ciudad = ciudadTexto.ToUpper();
                    entity.Contacto = contacto;
                    entity.Telefono = telefono;


                    if (id > 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.TransportePersona.Add(entity);
                        dbContext.SaveChanges();
                    }

                    return entity.IdTransportePersona;
                }
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

    }
}