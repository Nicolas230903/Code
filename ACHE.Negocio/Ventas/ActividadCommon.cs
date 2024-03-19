using ACHE.Model;
using ACHE.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Negocio.Facturacion
{
    public static class ActividadCommon
    {
        public static bool EliminarActividad(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    Actividad entity = dbContext.Actividad.Where(x => x.IdActividad == id && x.IdUsuario == usu.IDUsuario).FirstOrDefault();
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

        public static List<ActividadViewModel> ObtenerActividades(WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listaActividades = dbContext.Actividad.Where(x => x.IdUsuario == usu.IDUsuario).Select(x => new ActividadViewModel()
                    {
                        IDActividad = x.IdActividad,
                        Codigo = x.Codigo,
                        FechaDeAlta = x.FechaAlta,
                        FechaDeBaja = x.FechaBaja,
                        PorDefecto = x.PorDefecto,
                    }).ToList();
                    return listaActividades;
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

        public static void GuardarActividad(string codigo, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.Actividad.Any(x => x.Codigo == codigo && x.IdUsuario == usu.IDUsuario))
                        throw new CustomException("Ya existe el código");

                    Actividad entity = new Actividad();
                    entity.Codigo = codigo;
                    entity.IdUsuario = usu.IDUsuario;
                    entity.FechaAlta = DateTime.Now;
                    entity.PorDefecto = false;

                    dbContext.Actividad.Add(entity);
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
