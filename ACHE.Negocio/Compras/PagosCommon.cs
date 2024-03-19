using System;
using System.Linq;
using ACHE.Model;
using ACHE.Model.Negocio;
using ACHE.Negocio.Contabilidad;

namespace ACHE.Negocio.Facturacion
{
    public class PagosCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static Pagos Guardar(ACHEEntities dbContext, PagosCartDto PagosCart, WebUser usu)
        {
            Personas persona = dbContext.Personas.Where(x => x.IDPersona == PagosCart.IDPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (persona == null)
                throw new Exception("El cliente/proveedor es inexistente");

            if (!PagosCart.FormasDePago.Any())
                throw new Exception("Ingrese una forma de pago");

            if (!PagosCart.Items.Any())
                throw new Exception("Ingrese un comprobante a pagar");

            Pagos entity;
            if (PagosCart.IDPago > 0)
            {
                entity = dbContext.Pagos
                    .Include("PagosDetalle").Include("PagosFormasDePago").Include("PagosRetenciones")
                    .Where(x => x.IDPago == PagosCart.IDPago && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            }
            else
            {
                entity = new Pagos();
                entity.FechaAlta = DateTime.Now.Date;
                entity.IDUsuario = usu.IDUsuario;
            }

            entity.ImporteNeto = 0;
            entity.ImporteTotal = 0;
            entity.Observaciones = PagosCart.Observaciones;
            entity.IDPersona = PagosCart.IDPersona;
            entity.FechaPago = Convert.ToDateTime(PagosCart.FechaPago);
            entity.EstadoCaja = "Cargado";
            entity.EstadoCajaFecha = DateTime.Now;

            #region Detalle, Formas y Retenciones

            //////////////PagosFormasDePago
            if (entity.PagosFormasDePago.Any())
                dbContext.PagosFormasDePago.RemoveRange(entity.PagosFormasDePago);

            decimal totalFormasDePago = 0;
            decimal totalRetenciones = 0;
            foreach (var item in PagosCart.FormasDePago.ToList())
            {
                PagosFormasDePago formas = new PagosFormasDePago();
                formas.FormaDePago = item.FormaDePago;
                formas.Importe = item.Importe;
                formas.NroReferencia = item.NroReferencia;
                formas.IDCheque = item.IDCheque;
                formas.IDBanco = item.IDBanco;
                entity.PagosFormasDePago.Add(formas);

                var cheques = dbContext.Cheques.Where(x => x.IDCheque == item.IDCheque).FirstOrDefault();
                if (cheques != null)
                    cheques.Estado = "Usado";

                totalFormasDePago += item.Importe;
            }
            //////////////PagosRetenciones
            if (entity.PagosRetenciones.Any())
                dbContext.PagosRetenciones.RemoveRange(entity.PagosRetenciones);

            foreach (var item in PagosCart.Retenciones.ToList())
            {
                PagosRetenciones restriccciones = new PagosRetenciones();
                restriccciones.Tipo = item.Tipo;
                restriccciones.NroReferencia = item.NroReferencia;
                restriccciones.Importe = item.Importe;

                totalRetenciones += item.Importe;

                entity.PagosRetenciones.Add(restriccciones);
            }
            //////////////PagosDetalle
            if (entity.PagosDetalle.Any())
                dbContext.PagosDetalle.RemoveRange(entity.PagosDetalle);

            decimal neto = 0;
            decimal total = 0;
            foreach (var item in PagosCart.Items)
            {
                neto += item.ImporteNeto;
                total += item.Importe;

                PagosDetalle pagoDetalle = new PagosDetalle();
                pagoDetalle.Importe = item.Importe;
                pagoDetalle.IDCompra = item.IDCompra;
                pagoDetalle.ImporteNeto = item.ImporteNeto;
                entity.PagosDetalle.Add(pagoDetalle);
            }
            entity.ImporteNeto = neto;
            entity.ImporteTotal = total;

            if (total == 0)
                throw new CustomException("No puede ingresar un pago con Importe 0");

            //if ((totalFormasDePago + totalRetenciones).ToString("N2") != total.ToString("N2"))
            //    throw new CustomException("La suma de las formas de pago y retenciones deben coincidir con el total a pagar");

            #endregion

            if (PagosCart.IDPago > 0)
                dbContext.SaveChanges();
            else
            {
                dbContext.Pagos.Add(entity);
                dbContext.SaveChanges();
            }

            foreach (var item in entity.PagosDetalle)
                dbContext.ActualizarSaldosPorCompra(item.IDCompra);

            return entity;
        }

        public static Pagos Guardar(PagosCartDto PagosCart, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                return Guardar(dbContext, PagosCart, usu);
            }
        }

        public static bool EliminarPago(int id, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Pagos.Include("PagosDetalle").Where(x => x.IDPago == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    if (ContabilidadCommon.ValidarCierreContable(usu, entity.FechaPago))
                        throw new CustomException("El comprobante no puede eliminarse ya que el año contable ya fue cerrado.");

                    var list = entity.PagosDetalle.Select(x => new { IDCompra = x.IDCompra }).ToList();

                    dbContext.Pagos.Remove(entity);
                    dbContext.SaveChanges();

                    foreach (var item in list)
                        dbContext.ActualizarSaldosPorCompra(item.IDCompra);

                    return true;
                }
                else
                    return false;
            }
        }

        public static ResultadosPagosViewModel ObtenerPagos(string condicion, string periodo, string fechaDesde, string fechaHasta, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Pagos.Include("Personas").Include("PagosDetalle").Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                Int32 numero = 0;
                if (Int32.TryParse(condicion, out numero))
                    results = results.Where(x => x.PagosDetalle.Any(y => y.Compras.NroFactura.Contains(condicion)));
                else if (!string.IsNullOrWhiteSpace(condicion))
                    results = results.Where(x => x.Personas.RazonSocial.Contains(condicion) || x.Personas.NombreFantansia.Contains(condicion));

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

                if (!string.IsNullOrWhiteSpace(fechaDesde))
                {
                    DateTime dtDesde = DateTime.Parse(fechaDesde);
                    results = results.Where(x => x.FechaPago >= dtDesde);
                }
                if (!string.IsNullOrWhiteSpace(fechaHasta))
                {
                    DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                    results = results.Where(x => x.FechaPago <= dtHasta);
                }

                page--;
                ResultadosPagosViewModel resultado = new ResultadosPagosViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var list = results.OrderByDescending(x => x.FechaPago).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new PagosViewModel()
                    {
                        ID = x.IDPago,
                        RazonSocial = (x.Personas.NombreFantansia == "" ? x.Personas.RazonSocial.ToUpper() : x.Personas.NombreFantansia.ToUpper()),
                        Fecha = x.FechaPago.ToString(formatoFecha),
                        Iva = (x.PagosDetalle.Sum(y => y.Compras.Iva)).ToString("N2"),
                        NoGravado = (x.PagosDetalle.Sum(y => y.Compras.NoGravado) + x.PagosDetalle.Sum(y => y.Compras.Exento)).ToString("N2"),
                        Retenciones = Convert.ToDecimal(x.PagosDetalle.Sum(y => y.Compras.TotalImpuestos)).ToString("N2"),
                        ImporteNeto = Convert.ToDecimal(x.ImporteTotal).ToString("N2"),
                        Total = (x.ImporteTotal).ToString("N2")
                    });
                resultado.Items = list.ToList();
                return resultado;
            }
        }
    }
}