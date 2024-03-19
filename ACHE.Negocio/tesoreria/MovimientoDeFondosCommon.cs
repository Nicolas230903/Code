using ACHE.Model;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Contabilidad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACHE.Negocio.Banco
{
    public static class MovimientoDeFondosCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        #region ABM MovimientoDeFondos

        public static int GuardarMovimientoDeFondos(int id, string idOrigen, string idDestino, string importe, string fechaMovimiento, string observaciones, WebUser usu)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idOrigen))
                    throw new CustomException("La cuenta origen es obligaroria");
                else if (string.IsNullOrWhiteSpace(idDestino))
                    throw new CustomException("La cuenta destino es obligaroria");
                else if (idOrigen == idDestino)
                    throw new CustomException("No se puede ingresar un movimiento con la misma cuenta de origen que destino");
                else if (idOrigen.Contains("CAJA") && idDestino.Contains("CAJA"))
                    throw new CustomException("Los movimientos de fondos no pueden ser entre cajas");

                using (var dbContext = new ACHEEntities())
                {
                    MovimientoDeFondos entity;
                    if (id > 0)
                        entity = dbContext.MovimientoDeFondos.Where(x => x.IDMovimientoDeFondo == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new MovimientoDeFondos();
                        entity.FechaDeAlta = DateTime.Now;
                        entity.IDUsuario = usu.IDUsuario;
                    }

                    entity.Origen = (idOrigen.Split('_')[0].Contains("BANCO")) ? "BANCO" : "CAJA";
                    entity.Destino = (idDestino.Split('_')[0].Contains("BANCO")) ? "BANCO" : "CAJA";

                    if (entity.Origen == "BANCO")
                        entity.IDBancoOrigen = Convert.ToInt32(idOrigen.Split('_')[1]);
                    else
                        entity.CajaOrigen = idOrigen.Split('_')[1];
                    if (entity.Destino == "BANCO")
                        entity.IDBancoDestino = Convert.ToInt32(idDestino.Split('_')[1]);
                    else
                        entity.CajaDestino = idDestino.Split('_')[1];

                    entity.Importe = decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales)); ;
                    entity.FechaMovimiento = Convert.ToDateTime(fechaMovimiento);
                    entity.EstadoCaja = "Cargado";
                    entity.EstadoCajaFecha = DateTime.Now;
                    entity.Observaciones = observaciones;

                    if (id == 0)
                        dbContext.MovimientoDeFondos.Add(entity);

                    dbContext.SaveChanges();
                    ContabilidadCommon.AgregarAsientoDeCaja(entity.IDMovimientoDeFondo, usu);
                    return entity.IDMovimientoDeFondo;
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

        public static bool EliminarMovimientoDeFondos(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.MovimientoDeFondos.Where(x => x.IDMovimientoDeFondo == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {                      
                        dbContext.MovimientoDeFondos.Remove(entity);
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                        return false;
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

        public static ResultadosMovimientoDeFondosViewModel ObtenerMovimientoDeFondos(string condicion, string periodo, string fechaDesde, string fechaHasta, int? page, int? pageSize, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    List<MovimientoDeFondosViewModel> list;
                    ResultadosMovimientoDeFondosViewModel resultado = new ResultadosMovimientoDeFondosViewModel();

                    var results = dbContext.vMovimientoDeFondos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    switch (periodo)
                    {
                        case "30":
                            fechaDesde = DateTime.Now.AddDays(-30).ToShortDateString();
                            break;
                        case "15":
                            fechaDesde = DateTime.Now.AddDays(-15).ToShortDateString();
                            break;
                        case "7":
                            fechaDesde = DateTime.Now.AddDays(-7).ToShortDateString();
                            break;
                        case "1":
                            fechaDesde = DateTime.Now.AddDays(-1).ToShortDateString();
                            break;
                        case "0":
                            fechaDesde = DateTime.Now.ToShortDateString();
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(condicion))
                    {                        
                        results = results.Where(x => x.BancoOrigenNombre.Contains(condicion) || x.BancoDestinoNombre.Contains(condicion) || x.Origen.ToUpper().Contains(condicion.ToUpper()) || x.Destino.ToUpper().Contains(condicion.ToUpper()));
                    }

                    if (!periodo.Equals("-2"))
                    {
                        if (!string.IsNullOrWhiteSpace(fechaDesde))
                        {
                            DateTime dtDesde = DateTime.Parse(fechaDesde);
                            results = results.Where(x => x.FechaMovimiento >= dtDesde);
                        }
                        if (!string.IsNullOrWhiteSpace(fechaHasta))
                        {
                            DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                            results = results.Where(x => x.FechaMovimiento <= dtHasta);
                        }
                    }

                    if (page != null && pageSize != null)
                    {
                        page--;
                        list = results.OrderBy(x => x.FechaMovimiento).Skip(Convert.ToInt32(page) * Convert.ToInt32(pageSize)).Take(Convert.ToInt32(pageSize)).ToList()
                        .Select(x => new MovimientoDeFondosViewModel()
                        {
                            ID = x.IDMovimientoDeFondo,

                            CuentaOrigen = (x.Origen == "BANCO") ? x.BancoOrigenNroCuenta + " - " + x.BancoOrigenNombre : "Caja",
                            CuentaDestino = (x.Destino == "BANCO") ? x.BancoDestinoNroCuenta + " - " + x.BancoDestinoNombre : "Caja",
                            IDBancoOrigen = (x.Origen == "BANCO") ? Convert.ToInt32(x.IDBancoOrigen) : 0,
                            IDBancoDestino = (x.Destino == "BANCO") ? Convert.ToInt32(x.IDBancoDestino) : 0,
                            Observaciones = x.Observaciones,
                            FechaMovimiento = x.FechaMovimiento.ToString(formatoFecha),
                            Importe = x.Importe.ToString("N2")
                        }).ToList();

                        resultado.TotalPage = ((list.Count() - 1) / Convert.ToInt32(pageSize)) + 1;
                        resultado.TotalItems = list.Count();
                    }
                    else
                    {
                        list = results.OrderBy(x => x.FechaMovimiento).ToList()
                        .Select(x => new MovimientoDeFondosViewModel()
                        {
                            ID = x.IDMovimientoDeFondo,

                            CuentaOrigen = (x.Origen == "BANCO") ? x.BancoOrigenNroCuenta + " - " + x.BancoOrigenNombre : "Caja",
                            CuentaDestino = (x.Destino == "BANCO") ? x.BancoDestinoNroCuenta + " - " + x.BancoDestinoNombre : "Caja",
                            IDBancoOrigen = (x.Origen == "BANCO") ? Convert.ToInt32(x.IDBancoOrigen) : 0,
                            IDBancoDestino = (x.Destino == "BANCO") ? Convert.ToInt32(x.IDBancoDestino) : 0,
                            Observaciones = x.Observaciones,
                            FechaMovimiento = x.FechaMovimiento.ToString(formatoFecha),
                            Importe = x.Importe.ToString("N2")
                        }).ToList();
                    }
                    resultado.Items = list.ToList();
                    return resultado;
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
        #endregion

        //public static int GuardarMovimientoDeFondos(int id, string idCuentaOrigen, string idCuentaDestino, decimal importe, string fechaMovimiento, string observaciones, WebUser usu)
        //{
        //    var IDMovimiento = 0;
        //    string nombreCocepto = string.Empty;

        //    if (string.IsNullOrWhiteSpace(idCuentaOrigen))
        //        throw new CustomException("La cuenta origen es obligaroria");
        //    else if (string.IsNullOrWhiteSpace(idCuentaDestino))
        //        throw new CustomException("La cuenta destino es obligaroria");
        //    else if (idCuentaOrigen == idCuentaDestino)
        //        throw new CustomException("No se puede ingresar un movimiento con la misma cuenta de origen que destino");
        //    else if (idCuentaOrigen.Contains("CAJA") && idCuentaDestino.Contains("CAJA"))
        //        throw new CustomException("Los movimientos de fondos no pueden ser entre cajas");

        //    using (var dbContext = new ACHEEntities())
        //    {
        //        Bancos banco;
        //        if (idCuentaOrigen.Contains("BANCO"))
        //        {
        //            var idBancoO = Convert.ToInt32(idCuentaOrigen.Split('_')[1]);
        //            banco = dbContext.Bancos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDBanco == idBancoO).FirstOrDefault();
        //            nombreCocepto = banco.BancosBase.Nombre + " Cuenta: " + banco.NroCuenta;
        //        }

        //        if (idCuentaDestino.Contains("BANCO"))
        //        {
        //            var idBancoD = Convert.ToInt32(idCuentaDestino.Split('_')[1]);
        //            banco = dbContext.Bancos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDBanco == idBancoD).FirstOrDefault();
        //            nombreCocepto = banco.BancosBase.Nombre + " Cuenta: " + banco.NroCuenta;
        //        }
        //    }

        //    idCuentaOrigen = GuardarCaja(id, idCuentaOrigen, importe, fechaMovimiento, "ORIGEN", nombreCocepto, usu);
        //    idCuentaDestino = GuardarCaja(id, idCuentaDestino, importe, fechaMovimiento, "DESTINO", nombreCocepto, usu);

        //    IDMovimiento = Guardar(id, idCuentaOrigen, idCuentaDestino, importe, fechaMovimiento, observaciones, usu);
        //    ContabilidadCommon.AgregarAsientoDeCaja(IDMovimiento, usu);

        //    return IDMovimiento;
        //}

        //private static string GuardarCaja(int id, string idCuenta, decimal importe, string fechaMovimiento, string tipo, string concepto, WebUser usu)
        //{
        //    try
        //    {
        //        var Cuenta = (idCuenta.Split('_')[0].Contains("BANCO")) ? "BANCO" : "CAJA";

        //        if (Cuenta == "CAJA")
        //        {
        //            var idCaja = 0;
        //            using (var dbContext = new ACHEEntities())
        //            {
        //                if (id > 0)
        //                {
        //                    var cajaOld = dbContext.MovimientoDeFondos.Where(x => x.IDUsuario == usu.IDUsuario && x.IDMovimientoDeFondo == id).FirstOrDefault();
        //                    if (cajaOld != null)
        //                    {
        //                        //if (tipo == "ORIGEN")
        //                        //    idCaja = Convert.ToInt32(cajaOld.IDCajaOrigen);
        //                        //else if (tipo == "DESTINO")
        //                        //    idCaja = Convert.ToInt32(cajaOld.IDCajaDestino);
        //                    }
        //                }
        //            }

        //            CajaViewModel caja = new CajaViewModel();

        //            caja.ID = idCaja;
        //            caja.Estado = "Cargado";
        //            caja.tipoMovimiento = (tipo == "ORIGEN") ? "Egreso" : "Ingreso";
        //            caja.Importe = importe;
        //            caja.Fecha = fechaMovimiento;
        //            caja.Concepto = (tipo == "ORIGEN") ? "De caja a " + concepto : "De " + concepto + " a caja";

        //            caja.Observaciones = "Movimiento de fondo ";
        //            caja.MedioDePago = "Efectivo";
        //            caja.Ticket = "";

        //            return "CAJA_" + CajaCommon.GuardarCajaMovimiento(caja, usu);
        //        }
        //        else
        //            return idCuenta;
        //    }
        //    catch (CustomException ex)
        //    {
        //        throw new CustomException(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
    }
}