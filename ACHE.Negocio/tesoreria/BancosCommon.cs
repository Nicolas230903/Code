using ACHE.Model;
using System;
using System.Linq;

namespace ACHE.Negocio.Banco
{
    public class BancosCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        #region ABM Bancos
        public static int GuardarBanco(int id, int idBancoBase, string nroCuenta, string moneda, int activo, string saldoInicial, string ejecutivo, string direccion, string telefono, string email, string observacion, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (!(idBancoBase > 0))
                        throw new CustomException("El nombre del banco es obligatorio");
                    if (dbContext.Bancos.Any(x => x.IDUsuario == usu.IDUsuario && x.IDBancoBase == idBancoBase && x.NroCuenta == nroCuenta && x.IDBanco != id))
                        throw new CustomException("El nro de cuenta ingresado ya se encuentra registrado.");

                    Bancos entity;
                    if (id > 0)
                        entity = dbContext.Bancos.Where(x => x.IDBanco == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new Bancos();
                        entity.FechaAlta = DateTime.Now;
                        entity.IDUsuario = usu.IDUsuario;
                    }

                    entity.IDBancoBase = idBancoBase;
                    entity.NroCuenta = nroCuenta.ToString();
                    entity.Moneda = moneda;
                    entity.Activo = Convert.ToBoolean(activo);

                    entity.Ejecutivo = ejecutivo;
                    entity.Direccion = direccion;
                    entity.Telefono = telefono;
                    entity.Email = email;
                    entity.Observaciones = observacion;

                    entity.SaldoInicial = (!string.IsNullOrWhiteSpace(saldoInicial)) ? decimal.Parse(saldoInicial.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;

                    if (id == 0)
                        dbContext.Bancos.Add(entity);

                    dbContext.SaveChanges();
                    return entity.IDBanco;
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
        public static bool EliminarBancos(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.GastosBancarios.Any(x => x.IDBanco == id))
                        throw new CustomException("No se puede eliminar el banco, por tener Gastos bancarios asociados");
                    else if (dbContext.PagosFormasDePago.Any(x => x.IDBanco == id))
                        throw new CustomException("No se puede eliminar el banco, por tener pagos asociados");
                    else if (dbContext.CobranzasFormasDePago.Any(x => x.IDBanco == id))
                        throw new CustomException("No se puede eliminar el banco, por tener cobranzas asociadas");

                    var banco = dbContext.BancosPlanDeCuenta.Where(x => x.IDUsuario == usu.IDUsuario && x.IDBanco == id).FirstOrDefault();
                    if (banco != null)
                    {
                        if (dbContext.AsientoDetalle.Any(x => x.Asientos.IDUsuario == usu.IDUsuario && x.IDPlanDeCuenta == banco.IDPlanDeCuenta))
                            throw new CustomException("No se puede eliminar por tener asientos contables asociados");

                        var cuenta = dbContext.PlanDeCuentas.Where(x => x.IDPlanDeCuenta == banco.IDPlanDeCuenta).FirstOrDefault();
                        dbContext.BancosPlanDeCuenta.Remove(banco);
                        dbContext.PlanDeCuentas.Remove(cuenta);
                    }

                    var entity = dbContext.Bancos.Where(x => x.IDBanco == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.Bancos.Remove(entity);
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
        public static ResultadosBancosViewModel ObtenerBancos(string condicion, int page, int pageSize, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var results = dbContext.Bancos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                    if (!string.IsNullOrWhiteSpace(condicion))
                        results = results.Where(x => x.BancosBase.Nombre.Contains(condicion) || x.NroCuenta.Contains(condicion));

                    page--;
                    ResultadosBancosViewModel resultado = new ResultadosBancosViewModel();

                    var list = results.OrderBy(x => x.BancosBase.Nombre).Skip(page * pageSize).Take(pageSize).ToList()
                     .Select(x => new BancosViewModel()
                     {
                         ID = x.IDBanco,
                         Nombre = x.BancosBase.Nombre.ToUpper(),
                         NroCuenta = x.NroCuenta,
                         Moneda = x.Moneda,
                         Activo = (x.Activo) ? "SI" : "NO",
                         SaldoInicial = x.SaldoInicial.ToString("N2"),
                         SaldoActual = ObtenerSaldoActual(dbContext, x.IDBanco, usu),
                         Ejecutivo = x.Ejecutivo,
                         Telefono = x.Telefono,
                         Direccion = x.Direccion,
                         Email = x.Email,
                         Observacion = x.Observaciones,
                         IDBancoBase = x.IDBancoBase
                     });

                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();
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
        private static string ObtenerSaldoActual(ACHEEntities dbContext, int idBanco, WebUser usu)
        {
            string sql = string.Empty;
            sql = "select SUM(Importe) from RptBancarioView where IDUsuario=" + usu.IDUsuario + " and IDBanco=" + idBanco + " and tipoMovimiento='Ingreso'";
            var Ingresos = dbContext.Database.SqlQuery<decimal?>(sql, new object[] { }).FirstOrDefault();

            sql = "select SUM(Importe) from RptBancarioView where IDUsuario=" + usu.IDUsuario + " and IDBanco=" + idBanco + " and tipoMovimiento='Egreso'";
            var Egresos = dbContext.Database.SqlQuery<decimal?>(sql, new object[] { }).FirstOrDefault();

            var saldo = (Convert.ToDecimal(Ingresos) - Convert.ToDecimal(Egresos)).ToString("N2");
            return saldo;
        }
        #endregion
    }
}