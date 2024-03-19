using System;
using System.Collections.Generic;
using System.Linq;
using ACHE.Model;
using ACHE.Model.ViewModels;

namespace ACHE.Negocio.Contabilidad
{
    public static class ContabilidadCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static void AgregarAsientoDeCompra(int idCompra, WebUser user)
        {
            try
            {
                if (user.UsaPlanCorporativo)//Plan Corporativo
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        Asientos asiento;
                        var compra = dbContext.Compras.Where(x => x.IDCompra == idCompra && x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDCompra == idCompra && x.IDUsuario == user.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = user.IDUsuario;
                            asiento.Leyenda = "Compra - Factura " + compra.Tipo + " " + compra.NroFactura;
                            asiento.IDCompra = idCompra;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        var Debe = (compra.Tipo != "NCA" && compra.Tipo != "NCB" && compra.Tipo != "NCC") ? "D" : "H";
                        var Haber = (compra.Tipo != "NCA" && compra.Tipo != "NCB" && compra.Tipo != "NCC") ? "H" : "D";

                        // cta que selecciona el usuario
                        var total = Convert.ToDecimal(compra.Total) + compra.ImpInterno + compra.ImpMunicipal + compra.ImpNacional + compra.Otros;
                        var totalProveedores = (total + compra.IIBB + compra.PercepcionIVA + compra.Iva + compra.NoGravado);
                        if (compra.IDPlanDeCuenta != null)
                            dbContext.AsientoDetalle.Add(insertarCuenta(total, (int)compra.IDPlanDeCuenta, asiento, Debe));
                        // cta IVA credito fiscal
                        if (compra.Iva > 0)
                            dbContext.AsientoDetalle.Add(insertarCuenta(compra.Iva, config.IDCtaIVACreditoFiscal, asiento, Debe));
                        // cta Percepcion IVA
                        if (compra.PercepcionIVA > 0)
                            dbContext.AsientoDetalle.Add(insertarCuenta(compra.PercepcionIVA, config.IDCtaPercepcionIVA, asiento, Debe));
                        // cta IIBB
                        if (compra.IIBB > 0)
                            dbContext.AsientoDetalle.Add(insertarCuenta(compra.IIBB, config.IDCtaIIBB, asiento, Debe));

                        // cta No gravado
                        if (compra.NoGravado > 0)
                            dbContext.AsientoDetalle.Add(insertarCuenta(compra.NoGravado, config.IDCtaConceptosNoGravadosxCompras, asiento, Debe));

                        // cta proveedores
                        dbContext.AsientoDetalle.Add(insertarCuenta(totalProveedores, config.IDCtaProveedores, asiento, Haber));

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoDePago(WebUser user, int idPago)
        {
            try
            {
                if (user.UsaPlanCorporativo)//Plan Corporativo
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        Asientos asiento;
                        asiento = dbContext.Asientos.Where(x => x.IDPago == idPago && x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        var pago = dbContext.Pagos.Where(x => x.IDPago == idPago && x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == user.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = user.IDUsuario;

                            var nombreComprobantes = string.Empty;
                            foreach (var item in pago.PagosDetalle)
                                nombreComprobantes = item.Compras.Tipo + " " + item.Compras.NroFactura + ", ";

                            asiento.Leyenda = "Pago de los comprobantes : " + nombreComprobantes;
                            asiento.IDPago = idPago;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        var total = pago.PagosDetalle.Sum(x => x.Importe) + pago.PagosRetenciones.Sum(x => x.Importe);
                        dbContext.AsientoDetalle.Add(insertarCuenta(total, config.IDCtaProveedores, asiento, "D"));

                        foreach (var item in pago.PagosFormasDePago)
                        {
                            //Cta banco que el usuario elija
                            if (item.FormaDePago == "Cheque Propio" || item.FormaDePago == "Débito" || item.FormaDePago == "Transferencia")
                            {
                                if (item.IDBanco == null)
                                    throw new CustomException("La cuenta banco no esta vinculada a ninguna cuenta.");
                                var idCuenta = item.Bancos.BancosPlanDeCuenta.Where(x => x.IDBanco == item.IDBanco).FirstOrDefault().IDPlanDeCuenta;
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, idCuenta, asiento, "H"));
                            }
                            //Cta Valores a depositar
                            if (item.FormaDePago == "Cheque de Terceros")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDCtaValoresADepositar, asiento, "H"));
                            //Cta Caja
                            if (item.FormaDePago == "Depósito" || item.FormaDePago == "Efectivo")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDCtaCaja, asiento, "H"));
                        }
                        foreach (var item in pago.PagosRetenciones)
                        {
                            //Cta Ganancias
                            if (item.Tipo == "Ganancias")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDctaRetGanancias, asiento, "H"));
                            //Cta IIBB
                            if (item.Tipo == "IIBB")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDctaRetIIBB, asiento, "H"));
                            //Cta IVA
                            if (item.Tipo == "IVA")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDctaRetIVA, asiento, "H"));
                        }

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new CustomException("Factura generada correctamente, pero los asientos contables asociados a este comprobante no pudieron ser guardados");
                    }
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
        public static void AgregarAsientoDeVentas(WebUser usu, int idComprobante)
        {
            try
            {
                if (usu.UsaPlanCorporativo)//Plan Corporativo
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        Asientos asiento;
                        var Venta = dbContext.ComprobantesDetalle.Where(x => x.IDComprobante == idComprobante && x.Comprobantes.IDUsuario == usu.IDUsuario).ToList();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDComprobante == idComprobante && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = usu.IDUsuario;
                            asiento.Leyenda = "Venta - Factura " + Venta.FirstOrDefault().Comprobantes.Tipo + " " + Venta.FirstOrDefault().Comprobantes.PuntosDeVenta.Punto.ToString("#0000") + "-" + Venta.FirstOrDefault().Comprobantes.Numero.ToString("#00000000");
                            asiento.IDComprobante = idComprobante;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        var Debe = (Venta.FirstOrDefault().Comprobantes.Tipo != "NCA" && Venta.FirstOrDefault().Comprobantes.Tipo != "NCB" && Venta.FirstOrDefault().Comprobantes.Tipo != "NCC") ? "D" : "H";
                        var Haber = (Venta.FirstOrDefault().Comprobantes.Tipo != "NCA" && Venta.FirstOrDefault().Comprobantes.Tipo != "NCB" && Venta.FirstOrDefault().Comprobantes.Tipo != "NCC") ? "H" : "D";

                        //Cuenta de deudores por venta
                        var totalDebe = Venta.FirstOrDefault().Comprobantes.ImporteTotalNeto;
                        dbContext.AsientoDetalle.Add(insertarCuenta(totalDebe, config.IDCtaDeudoresPorVentas, asiento, Debe));

                        // cta que de IVA debito fiscal
                        decimal totalIVA = 0;
                        foreach (var item in Venta)
                        {
                            if (item.Iva > 0)
                                totalIVA += Math.Round((item.Cantidad * item.PrecioUnitario * item.Iva)) / 100;
                        }
                        dbContext.AsientoDetalle.Add(insertarCuenta(Math.Round(totalIVA, 2), config.IDCtaIVADebitoFiscal, asiento, Haber));

                        decimal totalPercepcionesIVA = 0;
                        decimal totalPercepcionesIIBB = 0;
                        // cta que de percepcion IVA
                        if (Venta.FirstOrDefault().Comprobantes.PercepcionIVA > 0)
                        {
                            totalPercepcionesIVA = (Venta.FirstOrDefault().Comprobantes.PercepcionIVA * Venta.FirstOrDefault().Comprobantes.ImporteTotalBruto) / 100;
                            dbContext.AsientoDetalle.Add(insertarCuenta(Math.Round(totalPercepcionesIVA, 2), config.IDCtaPercepcionIVA, asiento, Haber));
                        }
                        // cta que de percepcion IIBB
                        if (Venta.FirstOrDefault().Comprobantes.PercepcionIIBB > 0)
                        {
                            totalPercepcionesIIBB = (Venta.FirstOrDefault().Comprobantes.PercepcionIIBB * Venta.FirstOrDefault().Comprobantes.ImporteTotalBruto) / 100;
                            dbContext.AsientoDetalle.Add(insertarCuenta(Math.Round(totalPercepcionesIIBB, 2), config.IDCtaIIBB, asiento, Haber));
                        }
                        // cta que de No gravado
                        if (Venta.FirstOrDefault().Comprobantes.ImporteNoGravado > 0)
                        {
                            dbContext.AsientoDetalle.Add(insertarCuenta(Math.Round(Venta.FirstOrDefault().Comprobantes.ImporteNoGravado, 2), config.IDCtaConceptosNoGravadosxVentas, asiento, Haber));
                        }
                        foreach (var item in Venta.GroupBy(x => x.IDPlanDeCuenta))
                        {
                            // cta que selecciona el usuario
                            if (item.FirstOrDefault().IDPlanDeCuenta != null)
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Sum(x => (x.Cantidad * x.PrecioUnitario)), (int)item.FirstOrDefault().IDPlanDeCuenta, asiento, Haber));
                        }

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new CustomException("Factura generada correctamente, pero los asientos contables asociados a este comprobante no pudieron ser guardados");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoDeCobranza(WebUser user, int idCobranza)
        {
            try
            {
                if (user.UsaPlanCorporativo)//Plan Corporativo
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        Asientos asiento;
                        asiento = dbContext.Asientos.Where(x => x.IDCobranza == idCobranza && x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        var cobranza = dbContext.Cobranzas.Where(x => x.IDCobranza == idCobranza && x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = user.IDUsuario;

                            var nombreComprobantes = string.Empty;
                            foreach (var item in cobranza.CobranzasDetalle)
                                nombreComprobantes = item.Comprobantes.Tipo + " " + item.Comprobantes.Numero.ToString("#00000000") + ", ";

                            asiento.Leyenda = "Cobranza de los comprobantes : " + nombreComprobantes;
                            asiento.IDCobranza = idCobranza;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        foreach (var item in cobranza.CobranzasFormasDePago)
                        {
                            //Cta banco que el usuario elija
                            if (item.FormaDePago == "Cheque Propio" || item.FormaDePago == "Débito" || item.FormaDePago == "Transferencia")
                            {
                                var idCuenta = item.Bancos.BancosPlanDeCuenta.Where(x => x.IDBanco == item.IDBanco).FirstOrDefault().IDPlanDeCuenta;
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, idCuenta, asiento, "D"));
                            }
                            //Cta Valores a depositar
                            if (item.FormaDePago == "Cheque Tercero")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDCtaValoresADepositar, asiento, "D"));
                            //Cta Caja
                            if (item.FormaDePago == "Depósito" || item.FormaDePago == "Efectivo")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDCtaCaja, asiento, "D"));
                        }
                        foreach (var item in cobranza.CobranzasRetenciones)
                        {
                            //Cta Ganancias
                            if (item.Tipo == "Ganancias")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDctaRetGanancias, asiento, "D"));
                            //Cta IIBB
                            if (item.Tipo == "IIBB")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDctaRetIIBB, asiento, "D"));
                            //Cta IVA
                            if (item.Tipo == "IVA")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDctaRetIVA, asiento, "D"));
                            //Cta SUSS
                            if (item.Tipo == "SUSS")
                                dbContext.AsientoDetalle.Add(insertarCuenta(item.Importe, config.IDCtaRetSUSS, asiento, "D"));
                        }

                        var total = cobranza.CobranzasDetalle.Sum(x => x.Importe) + cobranza.CobranzasRetenciones.Sum(x => x.Importe);
                        dbContext.AsientoDetalle.Add(insertarCuenta(total, config.IDCtaDeudoresPorVentas, asiento, "H"));

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Factura generada correctamente, pero los asientos contables asociados a este comprobante no pudieron ser guardados");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoDeGastoBancario(int idGastosBancarios, WebUser usu)
        {
            try
            {
                if (usu.UsaPlanCorporativo)//Plan Corporativo
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        Asientos asiento;
                        var gastoBancario = dbContext.GastosBancarios.Where(x => x.IDGastosBancarios == idGastosBancarios && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDGastosBancarios == idGastosBancarios && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = usu.IDUsuario;
                            asiento.Leyenda = "Gasto Bancario - " + gastoBancario.Concepto;
                            asiento.IDGastosBancarios = idGastosBancarios;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        // cta Gastos bancarios
                        dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(gastoBancario.TotalImportes), config.IDCtaGastosBancarios, asiento, "D"));

                        // cta banco que selecciono el usuario
                        var bancosPlanDeCuenta = gastoBancario.Bancos.BancosPlanDeCuenta.FirstOrDefault();
                        if (bancosPlanDeCuenta != null)
                            dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(gastoBancario.TotalImportes), bancosPlanDeCuenta.IDPlanDeCuenta, asiento, "H"));

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new CustomException("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoChequeAccion(WebUser user, int idChequeAccion)
        {
            try
            {
                if (user.UsaPlanCorporativo)//Plan Corporativo
                {
                    using (var dbContext = new ACHEEntities())
                    {
                        Asientos asiento;
                        var chequeAccion = dbContext.ChequeAccion.Where(x => x.IDChequeAccion == idChequeAccion && x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == user.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDCheque == chequeAccion.IDCheque && x.IDUsuario == user.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = user.IDUsuario;
                            if (chequeAccion.Accion == "Acreditado")
                                asiento.Leyenda = "Acreditación del cheque nro: " + chequeAccion.Cheques.Numero.ToString();
                            asiento.IDCheque = chequeAccion.IDCheque;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        switch (chequeAccion.Accion)
                        {
                            case "Rechazado":
                                break;
                            case "Depositado":
                                break;
                            case "Acreditado":
                                var idBancoAccion = dbContext.ChequeAccion.Where(x => x.IDCheque == chequeAccion.IDCheque && x.Accion == "Depositado").FirstOrDefault().IDBanco;
                                var idCuenta = dbContext.BancosPlanDeCuenta.Where(x => x.IDBanco == idBancoAccion).FirstOrDefault().IDPlanDeCuenta;
                                dbContext.AsientoDetalle.Add(insertarCuenta(chequeAccion.Cheques.Importe, idCuenta, asiento, "D"));
                                dbContext.AsientoDetalle.Add(insertarCuenta(chequeAccion.Cheques.Importe, config.IDCtaValoresADepositar, asiento, "H"));
                                break;
                        }

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoManual(List<AsientosManualesViewModel> listaAsientos, string leyenda, string fecha, int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    Asientos entity;
                    if (id > 0)
                    {
                        entity = dbContext.Asientos.Where(x => x.IDAsiento == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        var lAsientos = entity.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                        var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                        foreach (var item in asientosPartes)
                            dbContext.AsientoDetalle.Remove(item);
                    }
                    else
                    {
                        entity = new Asientos();
                        entity.IDUsuario = usu.IDUsuario;
                    }
                    entity.Fecha = Convert.ToDateTime(fecha);
                    entity.Leyenda = leyenda;

                    if (id == 0)
                        dbContext.Asientos.Add(entity);

                    foreach (var item in listaAsientos)
                    {
                        if (item.Debe > 0)
                        {
                            dbContext.AsientoDetalle.Add(insertarCuenta(item.Debe, item.IDPlanDeCuenta, entity, "D"));
                        }
                        else
                            dbContext.AsientoDetalle.Add(insertarCuenta(item.Haber, item.IDPlanDeCuenta, entity, "H"));
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoCierreDelEjercicio(ResultadosLibroMayorViewModel BalanceGeneral, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (usu.UsaPlanCorporativo)//Plan Corporativo
                    {
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        var TieneAsiento = dbContext.Asientos.Where(x => x.EsAsientoCierre && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (TieneAsiento != null)
                        {
                            dbContext.Asientos.Remove(TieneAsiento);
                            dbContext.SaveChanges();
                        }

                        var asiento = new Asientos();
                        asiento.Fecha = DateTime.Now.Date;
                        asiento.IDUsuario = usu.IDUsuario;
                        asiento.Leyenda = "Cierre del ejercicio contable fecha: " + DateTime.Now.Date.ToString(formatoFecha);
                        asiento.EsAsientoCierre = true;
                        asiento.EsAsientoInicio = false;
                        dbContext.Asientos.Add(asiento);

                        foreach (var item in BalanceGeneral.Asientos)
                        {
                            if (item.TipoDeCuenta.ToUpper() == "ACTIVO" && Convert.ToDecimal(item.TotalActivo) > 0)
                                dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(item.TotalActivo), item.IDPlanDeCuenta, asiento, "H"));
                        }

                        foreach (var item in BalanceGeneral.Asientos)
                        {
                            if (item.TipoDeCuenta.ToUpper() == "RESULTADO" && Convert.ToDecimal(item.TotalPerdidas) > 0 && Convert.ToDecimal(item.TotalGanancias) == 0)
                                dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(item.TotalPerdidas), item.IDPlanDeCuenta, asiento, "H"));
                        }

                        foreach (var item in BalanceGeneral.Asientos)
                        {
                            if (item.TipoDeCuenta.ToUpper() == "PASIVO" && Convert.ToDecimal(item.TotalPasivo) > 0)
                                dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(item.TotalPasivo), item.IDPlanDeCuenta, asiento, "D"));
                        }

                        foreach (var item in BalanceGeneral.Asientos)
                        {
                            if (item.TipoDeCuenta.ToUpper() == "RESULTADO" && Convert.ToDecimal(item.TotalGanancias) > 0 && Convert.ToDecimal(item.TotalPerdidas) == 0)
                                dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(item.TotalGanancias), item.IDPlanDeCuenta, asiento, "D"));
                        }

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void AgregarAsientoInicioDelEjercicio(WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (usu.UsaPlanCorporativo)//Plan Corporativo
                    {
                        var asientoCierre = dbContext.Asientos.Include("AsientoDetalle").Where(x => x.EsAsientoCierre).OrderByDescending(x => x.IDAsiento).FirstOrDefault();

                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        var TieneAsiento = dbContext.Asientos.Where(x => x.EsAsientoInicio && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (TieneAsiento != null)
                        {
                            dbContext.Asientos.Remove(TieneAsiento);
                            dbContext.SaveChanges();
                        }

                        var asiento = new Asientos();
                        asiento.Fecha = DateTime.Now.Date;
                        asiento.IDUsuario = usu.IDUsuario;
                        asiento.Leyenda = "Inicio del ejercicio contable fecha: " + DateTime.Now.Date.ToString(formatoFecha);
                        asiento.EsAsientoCierre = false;
                        asiento.EsAsientoInicio = true;

                        dbContext.Asientos.Add(asiento);

                        foreach (var item in asientoCierre.AsientoDetalle)
                        {
                            if (item.PlanDeCuentas.TipoDeCuenta == "Activo")
                                dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(item.Importe), item.IDPlanDeCuenta, asiento, "D"));
                        }

                        foreach (var item in asientoCierre.AsientoDetalle)
                        {
                            if (item.PlanDeCuentas.TipoDeCuenta == "Pasivo")
                                dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(item.Importe), item.IDPlanDeCuenta, asiento, "H"));
                        }

                        var TotalDebe = asiento.AsientoDetalle.Where(x => x.TipoDeAsiento == "D").Sum(x => x.Importe);
                        var TotalHaber = asiento.AsientoDetalle.Where(x => x.TipoDeAsiento == "H").Sum(x => x.Importe);
                        var resultado = TotalDebe - TotalHaber;

                        if (resultado > 0)
                            dbContext.AsientoDetalle.Add(insertarCuenta(resultado, config.IDCtaResultadoEjercicio, asiento, "H"));
                        else
                            dbContext.AsientoDetalle.Add(insertarCuenta(Math.Abs(resultado), config.IDCtaResultadoEjercicio, asiento, "D"));


                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void AgregarAsientoDeCaja(WebUser usu, int idCaja)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (usu.UsaPlanCorporativo)//Plan Corporativo
                    {
                        Asientos asiento;
                        var caja = dbContext.Caja.Where(x => x.IDCaja == idCaja && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDCaja == idCaja && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = usu.IDUsuario;
                            if (caja.ConceptosCaja != null)
                                asiento.Leyenda = "Caja - " + caja.ConceptosCaja.Nombre;
                            else
                                asiento.Leyenda = "Caja - " + caja.TipoMovimiento;
                            asiento.IDCaja = idCaja;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        var Debe = (caja.TipoMovimiento == "Ingreso") ? "D" : "H";
                        var Haber = (caja.TipoMovimiento == "Ingreso") ? "H" : "D";

                        // cta Caja 
                        dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(caja.Importe), config.IDCtaCaja, asiento, Debe));

                        // cta Caja que selecciono el usuario
                        dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(caja.Importe), Convert.ToInt32(caja.IDPlanDeCuenta), asiento, Haber));

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void InvertirAsientoDeCaja(WebUser usu, int idCaja)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (usu.UsaPlanCorporativo)//Plan Corporativo
                    {
                        Asientos asiento;
                        var caja = dbContext.Caja.Where(x => x.IDCaja == idCaja && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDCaja == idCaja && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        var asientoDebe = dbContext.AsientoDetalle.Where(x => x.IDAsiento == asiento.IDAsiento && x.TipoDeAsiento == "D").FirstOrDefault();
                        var asientoHaber = dbContext.AsientoDetalle.Where(x => x.IDAsiento == asiento.IDAsiento && x.TipoDeAsiento == "H").FirstOrDefault();

                        if (asiento != null)
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = usu.IDUsuario;
                            asiento.Leyenda = "Caja - Anulada";
                            asiento.IDCaja = idCaja;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        // cta Caja 
                        dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(caja.Importe), asientoDebe.IDPlanDeCuenta, asiento, "H"));

                        // cta Caja que selecciono el usuario
                        dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(caja.Importe), Convert.ToInt32(asientoHaber.IDPlanDeCuenta), asiento, "D"));

                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void AgregarAsientoDeCaja(int idMovimiento, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (usu.UsaPlanCorporativo)//Plan Corporativo
                    {
                        Asientos asiento;
                        var movFondo = dbContext.MovimientoDeFondos.Where(x => x.IDMovimientoDeFondo == idMovimiento && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        asiento = dbContext.Asientos.Where(x => x.IDMovimientoDeFondo == idMovimiento && x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                        if (asiento != null)
                        {
                            var lAsientos = asiento.AsientoDetalle.ToList().Select(x => x.IDAsiento);
                            var asientosPartes = dbContext.AsientoDetalle.Where(x => lAsientos.Contains(x.IDAsiento)).ToList();
                            foreach (var item in asientosPartes)
                                dbContext.AsientoDetalle.Remove(item);
                        }
                        else
                        {
                            asiento = new Asientos();
                            asiento.Fecha = DateTime.Now.Date;
                            asiento.IDUsuario = usu.IDUsuario;

                            asiento.Leyenda = "Movimiento de fondo";
                            asiento.IDMovimientoDeFondo = idMovimiento;
                            asiento.EsAsientoCierre = false;
                            asiento.EsAsientoInicio = false;
                            dbContext.Asientos.Add(asiento);
                        }

                        if (movFondo.Destino == "BANCO")
                        {
                            var ctaBanco = dbContext.BancosPlanDeCuenta.Where(x => x.IDBanco == movFondo.IDBancoDestino).FirstOrDefault().IDPlanDeCuenta;
                            dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(movFondo.Importe), Convert.ToInt32(ctaBanco), asiento, "D"));
                        }
                        else
                            dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(movFondo.Importe), Convert.ToInt32(config.IDCtaCaja), asiento, "D"));

                        if (movFondo.Origen == "BANCO")
                        {
                            var ctaBanco = dbContext.BancosPlanDeCuenta.Where(x => x.IDBanco == movFondo.IDBancoOrigen).FirstOrDefault().IDPlanDeCuenta;
                            dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(movFondo.Importe), Convert.ToInt32(ctaBanco), asiento, "H"));
                        }
                        else
                            dbContext.AsientoDetalle.Add(insertarCuenta(Convert.ToDecimal(movFondo.Importe), Convert.ToInt32(config.IDCtaCaja), asiento, "H"));


                        if (validarPartidaDoble(asiento))
                            dbContext.SaveChanges();
                        else
                            throw new Exception("Los cuentas del debe no son iguales al haber.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void CrearCuentaBancos(int idBancos, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (usu.UsaPlanCorporativo && usu.CondicionIVA == "RI") //Plan Corporativo
                    {
                        if (!dbContext.BancosPlanDeCuenta.Any(x => x.IDBanco == idBancos))
                        {
                            var config = dbContext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                            var banco = dbContext.Bancos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDBanco == idBancos).FirstOrDefault();
                            var padre = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPlanDeCuenta == config.IDCtaBancos).FirstOrDefault();
                            var maxCodigo = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario && x.IDPadre == padre.IDPlanDeCuenta).ToList().Max(x => x.Codigo);

                            var codAux = Convert.ToInt32(maxCodigo.Split('.')[(maxCodigo.Split('.').Length - 1)]) + 1;
                            var codigo = padre.Codigo + "." + (codAux).ToString();
                            var nombreCuenta = "Cta cte banco " + banco.BancosBase.Nombre;

                            var entity = new PlanDeCuentas();

                            entity.Codigo = codigo;
                            entity.Nombre = nombreCuenta;
                            entity.IDUsuario = usu.IDUsuario;
                            entity.IDPadre = padre.IDPlanDeCuenta;
                            entity.AdminiteAsientoManual = true;
                            entity.TipoDeCuenta = "Activo";
                            dbContext.PlanDeCuentas.Add(entity);

                            var bancosPlanDeCta = new BancosPlanDeCuenta();
                            bancosPlanDeCta.IDBanco = idBancos;
                            bancosPlanDeCta.PlanDeCuentas = entity;
                            bancosPlanDeCta.IDUsuario = usu.IDUsuario;
                            entity.BancosPlanDeCuenta.Add(bancosPlanDeCta);

                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void ObtenerLibroDiario()
        {

        }
        public static void ObtenerLibroMayor()
        {

        }
        public static ResultadosLibroMayorViewModel ObtenerBalanceDeResultados(WebUser usu, string fechaDesde, string fechaHasta, int page, int pageSize)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.rptImpositivoLibroDiario.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                if (fechaDesde != string.Empty)
                {
                    DateTime dtDesde = DateTime.Parse(fechaDesde);
                    results = results.Where(x => x.Fecha >= dtDesde);
                }
                if (fechaHasta != string.Empty)
                {
                    DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                    results = results.Where(x => x.Fecha <= dtHasta);
                }

                page--;
                ResultadosLibroMayorViewModel resultado = new ResultadosLibroMayorViewModel();
                resultado.TotalPage = ((results.GroupBy(x => x.Codigo).Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.GroupBy(x => x.Codigo).Count();

                resultado.Asientos = results.GroupBy(x => x.Codigo).OrderBy(x => x.FirstOrDefault().Codigo).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new CuentasViewModel()
                    {
                        nroCuenta = x.FirstOrDefault().Codigo,
                        NombreCuenta = x.FirstOrDefault().Nombre,
                        TipoDeCuenta = x.FirstOrDefault().TipoDeCuenta,
                        IDPlanDeCuenta = x.FirstOrDefault().IDPlanDeCuenta,
                        TotalDebe = x.Sum(y => y.Debe).ToString("N2"),
                        TotalHaber = x.Sum(y => y.Haber).ToString("N2"),
                        Saldo = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2"),
                        TotalDeudor = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) > 0) ? (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2") : "0",
                        TotalAcreedor = (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) < 0) ? Math.Abs(x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2") : "0",

                        TotalActivo = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "ACTIVO") ? (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2") : "0",
                        TotalPasivo = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "PASIVO") ? Math.Abs(x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2") : "0",

                        TotalPerdidas = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "RESULTADO" && (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) > 0)) ? (x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2") : "0",
                        TotalGanancias = (x.FirstOrDefault().TipoDeCuenta.ToUpper() == "RESULTADO" && (x.Sum(y => y.Debe) - x.Sum(y => y.Haber) < 0)) ? Math.Abs(x.Sum(y => y.Debe) - x.Sum(y => y.Haber)).ToString("N2") : "0",
                    }).ToList();

                resultado.TotalDebe = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalDebe)).ToString("N2");
                resultado.TotalHaber = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalHaber)).ToString("N2");

                resultado.TotalDeudor = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalDeudor)).ToString("N2");
                resultado.TotalAcreedor = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalAcreedor)).ToString("N2");

                resultado.TotalActivo = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalActivo)).ToString("N2");
                resultado.TotalPasivo = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalPasivo)).ToString("N2");

                resultado.TotalPerdidas = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalPerdidas)).ToString("N2");
                resultado.TotalGanancias = resultado.Asientos.Sum(x => Convert.ToDecimal(x.TotalGanancias)).ToString("N2");
                return resultado;
            }
        }

        public static bool VerificarCodigo(string codigo)
        {
            var listaCodigos = codigo.Split('.').ToList();

            foreach (var item in listaCodigos)
            {
                var n = 0;
                if (!Int32.TryParse(item, out n))
                    return true;
            }

            return false;
        }
        public static bool ValidarCierreContable(WebUser usu, DateTime fecha)
        {
            using (var dbContext = new ACHEEntities())
            {
                var añoContableFinalizado = dbContext.Asientos.Where(x => x.EsAsientoCierre && x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.Fecha).FirstOrDefault();
                if (añoContableFinalizado != null)
                {
                    if (fecha <= añoContableFinalizado.Fecha)
                        return true;
                }
            }

            return false;
        }
        private static AsientoDetalle insertarCuenta(decimal importe, int idCuenta, Asientos asiento, string tipoAsiente)
        {
            var asientoP = new AsientoDetalle();
            asientoP.Asientos = asiento;
            asientoP.TipoDeAsiento = tipoAsiente;
            asientoP.Importe = importe;
            asientoP.IDPlanDeCuenta = idCuenta;
            return asientoP;
        }
        private static bool validarPartidaDoble(Asientos asiento)
        {
            var TotalDebe = asiento.AsientoDetalle.Where(x => x.TipoDeAsiento == "D").Sum(x => x.Importe);
            var TotalHaber = asiento.AsientoDetalle.Where(x => x.TipoDeAsiento == "H").Sum(x => x.Importe);

            if (TotalDebe == TotalHaber)
                return true;
            else
                return false;
        }

        public static void EliminarConfiguracionPlanDeCuenta(int idUsuario)
        {
            try
            {
                using (var dbcontext = new ACHEEntities())
                {
                    if (dbcontext.Asientos.Any(x => x.IDUsuario == idUsuario))
                        throw new CustomException("La copnfiguración del plan de cuentas no se puede ser eliminado por tener asientos contables.");

                    var config = dbcontext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
                    if (config != null)
                    {
                        dbcontext.ConfiguracionPlanDeCuenta.Remove(config);
                        dbcontext.SaveChanges();
                    }
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void EliminarPlanDeCuentasActual(int idUsuario)
        {
            try
            {
                using (var dbcontext = new ACHEEntities())
                {
                    if (dbcontext.Asientos.Any(x => x.IDUsuario == idUsuario))
                        throw new CustomException("El plan de cuentas actual no se puede eliminar por tener asientos contables.");

                    var listaCuentas = dbcontext.PlanDeCuentas.Where(x => x.IDUsuario == idUsuario).ToList();
                    var config = dbcontext.ConfiguracionPlanDeCuenta.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
                    var bancos = dbcontext.BancosPlanDeCuenta.Where(x => x.IDUsuario == idUsuario).ToList();


                    foreach (var item in listaCuentas)
                        dbcontext.PlanDeCuentas.Remove(item);

                    foreach (var item in bancos)
                        dbcontext.BancosPlanDeCuenta.Remove(item);

                    if (config != null)
                        dbcontext.ConfiguracionPlanDeCuenta.Remove(config);

                    dbcontext.SaveChanges();
                }
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        //public static bool UsaPlanContable(ACHEEntities dbContext, int idusuario, string CondicionIVA)
        //{
        //    bool UsaPlanDeCuenta = false;
        //    var plandeCuenta = PermisosModulosCommon.ObtenerPlanActual(dbContext, idusuario);
        //    if (plandeCuenta != null)
        //    {
        //        if (plandeCuenta.IDPlan == 5 && CondicionIVA == "RI") //Plan Corporativo
        //            UsaPlanDeCuenta = true;
        //    }
        //    return UsaPlanDeCuenta;
        //}
        //public static bool UsaPlanContable(int idusuario, string CondicionIVA)
        //{
        //    using (var dbContext = new ACHEEntities())
        //    {
        //        bool UsaPlanDeCuenta = false;
        //        var plandeCuenta = PermisosModulosCommon.ObtenerPlanActual(dbContext, idusuario);
        //        if (plandeCuenta != null)
        //        {
        //            if (plandeCuenta.IDPlan == 5 && CondicionIVA == "RI") //Plan Corporativo
        //                UsaPlanDeCuenta = true;
        //        }
        //        return UsaPlanDeCuenta;
        //    }
        //}
    }
}