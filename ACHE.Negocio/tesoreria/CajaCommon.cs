using ACHE.Model;
using ACHE.Negocio.Contabilidad;
using System;
using System.Linq;
using ACHE.Extensions;

namespace ACHE.Negocio.Tesoreria
{
    public class CajaCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","


        #region ABM CAJA
        public static int GuardarCaja(CajaViewModel caja, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    Caja entity;
                    if (caja.ID > 0)
                        entity = dbContext.Caja.Where(x => x.IDCaja == caja.ID && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new Caja();
                        entity.FechaAlta = DateTime.Now.Date;
                        entity.Estado = "Cargado";
                        entity.EstadoFecha = DateTime.Now.Date;
                        entity.TipoMovimiento = caja.tipoMovimiento;
                        entity.Importe = Math.Abs(caja.Importe);
                        entity.Fecha = Convert.ToDateTime(caja.Fecha);
                    }
                    if (string.IsNullOrWhiteSpace(caja.Concepto))
                        entity.IDConceptosCaja = null;
                    else
                        entity.IDConceptosCaja = Convert.ToInt32(caja.Concepto);

                    entity.IDUsuario = usu.IDUsuario;
                    entity.Observaciones = caja.Observaciones;
                    entity.MedioDePago = caja.MedioDePago;
                    entity.Ticket = caja.Ticket;

                    if (caja.IDPlanDeCuenta != 0)
                        entity.IDPlanDeCuenta = caja.IDPlanDeCuenta;

                    if (caja.ID == 0)
                        dbContext.Caja.Add(entity);
                    dbContext.SaveChanges();

                    return entity.IDCaja;
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

        public static int GuardarCajaMovimiento(CajaViewModel caja, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    Caja entity;
                    if (caja.ID > 0)
                        entity = dbContext.Caja.Where(x => x.IDCaja == caja.ID && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new Caja();
                        entity.FechaAlta = DateTime.Now.Date;
                        entity.Estado = "Cargado";
                        entity.TipoMovimiento = caja.tipoMovimiento;
                        entity.Fecha = Convert.ToDateTime(caja.Fecha);
                    }

                    var concepto = dbContext.ConceptosCaja.Where(x => x.IDUsuario == usu.IDUsuario && x.Nombre == caja.Concepto).FirstOrDefault();
                    if (concepto == null)
                        entity.ConceptosCaja = new ConceptosCaja { IDUsuario = usu.IDUsuario, Nombre = caja.Concepto };
                    else
                        entity.IDConceptosCaja = concepto.IDConceptoCaja;

                    entity.Importe = Math.Abs(caja.Importe);
                    entity.IDUsuario = usu.IDUsuario;
                    entity.Observaciones = caja.Observaciones;
                    entity.MedioDePago = caja.MedioDePago;
                    entity.Ticket = caja.Ticket;

                    if (caja.IDPlanDeCuenta != 0)
                        entity.IDPlanDeCuenta = caja.IDPlanDeCuenta;

                    if (caja.ID == 0)
                        dbContext.Caja.Add(entity);
                    dbContext.SaveChanges();

                    return entity.IDCaja;
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

        public static void EliminarCaja(int id, string motivo, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Caja.Where(x => x.IDCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        if (entity.Estado == "Cargado")
                        {
                            entity.Fecha = DateTime.Now.Date;
                            entity.Estado = "Anulado";
                            dbContext.SaveChanges();
                            entity.TipoMovimiento = (entity.TipoMovimiento == "Ingreso") ? "Egreso" : "Ingreso";
                            entity.Observaciones = motivo;
                            entity.IDConceptosCaja = null;
                            entity.MedioDePago = "";
                            dbContext.Caja.Add(entity);
                            dbContext.SaveChanges();

                            if (usu.UsaPlanCorporativo) //Plan Corporativo
                                ContabilidadCommon.InvertirAsientoDeCaja(usu, id);
                        }
                        else
                            throw new CustomException("El movimiento no se puede eliminar ya que se encuentra conciliado");
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

        public static void EliminarCajaFisica(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Caja.Where(x => x.IDCaja == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.Caja.Remove(entity);
                        dbContext.SaveChanges();
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
        public static ResultadosCajaViewModel ObtenerCaja(string tipoMovimiento, string fechaDesde, string fechaHasta, string periodo, string medioDePago, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.CajaView.Where(x => x.IDUsuario == usu.IDUsuario).OrderBy(x => x.Fecha).AsQueryable();

                if (!string.IsNullOrWhiteSpace(tipoMovimiento))
                    results = results.Where(x => x.TipoMovimiento == tipoMovimiento);

                if (!medioDePago.Equals("Todos"))
                    results = results.Where(x => x.MedioDePago == medioDePago);

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

                page--;
                ResultadosCajaViewModel resultado = new ResultadosCajaViewModel();

                var list = results.OrderByDescending(x => x.Fecha).ThenByDescending(n => n.EstadoFecha).ToList()
                 .Select(x => new CajaViewModel()
                 {
                     ID = x.IDCaja,
                     tipoMovimiento = x.TipoMovimiento,
                     Fecha = Convert.ToDateTime(x.Fecha).ToString(formatoFecha),
                     Concepto = (x.ConceptosCaja == null) ? "" : x.ConceptosCaja,
                     Estado = x.Estado,
                     Observaciones = x.Observaciones,
                     Ingreso = (x.TipoMovimiento == "Ingreso") ? Math.Abs(x.Importe).ToString("N2") : "",
                     Egreso = (x.TipoMovimiento == "Egreso") ? (-1 * Math.Abs(x.Importe)).ToString("N2") : "",
                     MedioDePago = x.MedioDePago
                 }).ToList();

                decimal saldo = 0;

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var ingreso = Convert.ToDecimal((list[i].Ingreso == "") ? "0" : list[i].Ingreso);
                    var egreso = Convert.ToDecimal((list[i].Egreso == "") ? "0" : list[i].Egreso);
                    saldo = saldo + (Math.Abs(ingreso) - Math.Abs(egreso));
                    list[i].Saldo = saldo.ToString("N2");
                }


                if (!periodo.Equals("-2"))
                {
                    if (!string.IsNullOrWhiteSpace(fechaDesde))
                    {
                        DateTime dtDesde = DateTime.Parse(fechaDesde);
                        list = list.Where(x => Convert.ToDateTime(x.Fecha) >= dtDesde).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(fechaHasta))
                    {
                        DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                        list = list.Where(x => Convert.ToDateTime(x.Fecha) <= dtHasta).ToList();
                    }
                }

                resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = list.Count();
                list = list.Skip(page * pageSize).Take(pageSize).ToList();

                var listaAux = list.GroupBy(x => new { Convert.ToDateTime(x.Fecha).Date.Month, Convert.ToDateTime(x.Fecha).Date.Year }).ToList().Select(x => new ConceptosMeses()
                {
                    concepto = Convert.ToDateTime(x.FirstOrDefault().Fecha).ToString("MMMMMMMMM yyyy").ToProperCase(),
                    Conceptos = x.ToList()
                });

                resultado.Items = listaAux.ToList();

                var totalIngreso = dbContext.CajaView.Where(x => x.IDUsuario == usu.IDUsuario && x.TipoMovimiento == "Ingreso").ToList();
                var totalEgreso = dbContext.CajaView.Where(x => x.IDUsuario == usu.IDUsuario && x.TipoMovimiento == "Egreso").ToList();
                var ingresos = (totalIngreso.Count() > 0) ? totalIngreso.Sum(x => Math.Abs(x.Importe)) : 0;
                var egresos = (totalEgreso.Count() > 0) ? totalEgreso.Sum(x => Math.Abs(x.Importe)) : 0;

                resultado.TotalSinConsolidar = (ingresos - egresos).ToString("N2");

                return resultado;
            }
        }
        #endregion
    }
}