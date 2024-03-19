using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using ACHE.Model.ViewModels;
using ACHE.Negocio.Contabilidad;
using Org.BouncyCastle.Utilities.Encoders;

namespace ACHE.Negocio.Facturacion
{
    public static class GastosGeneralesCommon 
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static GastosGenerales Guardar(int id, string fechaPeriodo, string Sueldos,
                string SeguridadEHigiene, string Municipales, string Monotributos, string AportesYContribuciones,
                string Ganancias12, string CreditoBancario, string RetencionesDeIIBB, string PlanesAFIP,
                string Gastos1, string Gastos2, string Gastos3, string F1, string F2, int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();

                    bool GastoGeneralExistente = false;

                    DateTime periodo = DateTime.Now;

                    try
                    {
                        periodo = Convert.ToDateTime("01/" + fechaPeriodo.Substring(4, 2) + "/" + fechaPeriodo.Substring(0, 4));
                    }
                    catch 
                    {
                        throw new Exception("El periodo ingresado no es valido, el formato de AÑOMES EJ: 202210");
                    }
                    
                    GastosGenerales entity;
                    if (id > 0)
                    {
                        entity = dbContext.GastosGenerales.Where(x => x.IdGastosGenerales == id && x.IdUsuario == usu.IDUsuario).FirstOrDefault();
                    }
                    else
                    {
                        GastoGeneralExistente = dbContext.GastosGenerales.Any(x => x.Periodo.Year == periodo.Year && x.Periodo.Month == periodo.Month && x.IdUsuario == usu.IDUsuario);
                        entity = new GastosGenerales();                       
                    }

                    if (GastoGeneralExistente)
                        throw new Exception("Ya existe un registro para el periodo ingresado");


                    entity.FechaAlta = DateTime.Now.Date;
                    entity.IdUsuario = usu.IDUsuario;
                    entity.Periodo = periodo;

                    entity.Sueldos = Sueldos != string.Empty ? decimal.Parse(Sueldos) : 0;
                    entity.SeguridadEHigiene = SeguridadEHigiene != string.Empty ? decimal.Parse(SeguridadEHigiene) : 0;
                    entity.Municipales = Municipales != string.Empty ? decimal.Parse(Municipales) : 0;
                    entity.Monotributos = Monotributos != string.Empty ? decimal.Parse(Monotributos) : 0;
                    entity.AportesYContribuciones = AportesYContribuciones != string.Empty ? decimal.Parse(AportesYContribuciones) : 0;
                    entity.Ganancias12 = Ganancias12 != string.Empty ? decimal.Parse(Ganancias12) : 0;
                    entity.CreditoBancario = CreditoBancario != string.Empty ? decimal.Parse(CreditoBancario) : 0;
                    entity.RetencionesDeIIBB = RetencionesDeIIBB != string.Empty ? decimal.Parse(RetencionesDeIIBB) : 0;
                    entity.PlanesAFIP = (PlanesAFIP != string.Empty) ? decimal.Parse(PlanesAFIP) : 0;
                    entity.Gastos1 = (Gastos1 != string.Empty) ? decimal.Parse(Gastos1) : 0;
                    entity.Gastos2 = (Gastos2 != string.Empty) ? decimal.Parse(Gastos2) : 0;
                    entity.Gastos3 = (Gastos3 != string.Empty) ? decimal.Parse(Gastos3) : 0;
                    entity.F1 = (F1 != string.Empty) ? decimal.Parse(F1) : 0;
                    entity.F2 = (F2 != string.Empty) ? decimal.Parse(F2) : 0;

                    if (id > 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.GastosGenerales.Add(entity);
                        dbContext.SaveChanges();
                    }

                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool EliminarGastoGeneral(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.GastosGenerales.Where(x => x.IdGastosGenerales == id && x.IdUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.GastosGenerales.Remove(entity);
                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                        return false;
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

        public static ResultadosGastosGeneralesViewModel ObtenerGastosGenerales(string periodo, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.GastosGenerales.Where(x => x.IdUsuario == usu.IDUsuario).AsQueryable();

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

                    if (!periodo.Equals("-2"))
                    {
                        if (!string.IsNullOrWhiteSpace(fechaDesde))
                        {
                            DateTime dtDesde = DateTime.Parse(fechaDesde);
                            results = results.Where(x => x.Periodo >= dtDesde);
                        }
                        if (!string.IsNullOrWhiteSpace(fechaHasta))
                        {
                            DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                            results = results.Where(x => x.Periodo <= dtHasta);
                        }
                    }


                    page--;
                    ResultadosGastosGeneralesViewModel resultado = new ResultadosGastosGeneralesViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderByDescending(x => x.Periodo).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new GastosGeneralesViewModel()
                        {
                            ID = x.IdGastosGenerales,
                            Periodo = x.Periodo.Year.ToString() + x.Periodo.Month.ToString("D2"),
                            Total = (x.Sueldos + x.SeguridadEHigiene + x.Municipales + x.Monotributos + x.AportesYContribuciones + x.Ganancias12 + x.CreditoBancario + x.RetencionesDeIIBB + x.PlanesAFIP + x.Gastos1 + x.Gastos2 + x.Gastos3 + x.F1 + x.F2).ToString("N2"),
                        });
                    resultado.Items = list.ToList();

                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
