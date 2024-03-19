using ACHE.Model;
using ACHE.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Negocio.Facturacion
{
    public static class PuntoDeVentaCommon
    {
        public static bool EliminarPuntoDeVenta(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    PuntosDeVenta entity = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.FechaBaja = DateTime.Now;
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

        public static List<PuntoDeVentaViewModel> ObtenerPuntoDeVenta(WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listaPuntosDeVenta = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == usu.IDUsuario).Select(x => new PuntoDeVentaViewModel()
                    {
                        IDPuntoDeVenta = x.IDPuntoVenta,
                        PuntoDeVenta = x.Punto,
                        FechaDeAlta = x.FechaAlta,
                        FechaDeBaja = x.FechaBaja,
                        PorDefecto = x.PorDefecto,
                    }).ToList();
                    return listaPuntosDeVenta;
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

        public static void GuardarPuntoDeVenta(int punto, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    punto = Math.Abs(punto);
                    if (dbContext.PuntosDeVenta.Any(x => x.Punto == punto && x.IDUsuario == usu.IDUsuario))
                        throw new CustomException("Ya existe el punto de venta");

                    PuntosDeVenta entity = new PuntosDeVenta();
                    entity.Punto = punto;
                    entity.IDUsuario = usu.IDUsuario;
                    entity.FechaAlta = DateTime.Now;
                    entity.PorDefecto = false;

                    dbContext.PuntosDeVenta.Add(entity);
                    dbContext.SaveChanges();
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
