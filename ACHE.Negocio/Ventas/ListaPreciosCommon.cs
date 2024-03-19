using ACHE.Model;
using ACHE.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACHE.Negocio.Productos
{
    public class ListaPreciosCommon
    {
        public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        #region ABM Lista de Precios
        public static void GuardarListaDePrecio(int id, string nombre, string Observaciones, int activo, List<PreciosConceptos> listaDePrecios, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    if (dbContext.ListaPrecios.Any(x => x.IDUsuario == usu.IDUsuario && x.Nombre == nombre && x.IDListaPrecio != id))
                        throw new CustomException("El nombre de la cuenta ya se encuentra ingresado.");

                    ListaPrecios entity;
                    if (id > 0)
                        entity = dbContext.ListaPrecios.Where(x => x.IDListaPrecio == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    else
                    {
                        entity = new ListaPrecios();
                        entity.IDUsuario = usu.IDUsuario;
                    }

                    entity.Nombre = nombre.ToUpper();
                    entity.Observaciones = Observaciones;
                    entity.Activa = Convert.ToBoolean(activo);

                    if (id > 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.ListaPrecios.Add(entity);
                        dbContext.SaveChanges();
                    }

                    ParceListaDePrecios(dbContext, listaDePrecios, entity.IDListaPrecio);
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
        public static bool EliminarListaDePrecio(int id, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.ListaPrecios.Where(x => x.IDListaPrecio == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    if (entity != null)
                    {
                        dbContext.ListaPrecios.Remove(entity);
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
        /// <summary>
        /// Obtiene todas las listas de precios
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="usu"></param>
        /// <returns></returns>
        public static ResultadoslistaPreciosViewModel ObtenerListasDePrecios(string nombre, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.ListaPrecios.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();
                if (!string.IsNullOrWhiteSpace(nombre))
                    results = results.Where(x => x.Nombre.Contains(nombre));

                page--;
                ResultadoslistaPreciosViewModel resultado = new ResultadoslistaPreciosViewModel();

                var list = results.OrderByDescending(x => x.IDListaPrecio).Skip(page * pageSize).Take(pageSize).ToList()
                 .Select(x => new listaPreciosViewModel()
                 {
                     ID = x.IDListaPrecio,
                     Nombre = x.Nombre.ToUpper(),
                     Observaciones = x.Observaciones,
                     Activa = (x.Activa) ? "SI" : "NO"
                 });

                resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = list.Count();
                resultado.Items = list.ToList();

                return resultado;
            }
        }
        /// <summary>
        /// Obtiene los productos para ser modificados en la lista de precios
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usu"></param>
        /// <returns></returns>
        public static ResultadoslistaPreciosViewModel ListaDePrecios(int id, WebUser usu)
        {
            try
            {
                ResultadoslistaPreciosViewModel resultado = new ResultadoslistaPreciosViewModel();
                using (var dbContext = new ACHEEntities())
                {
                    var listaPreciosConceptos = dbContext.PreciosConceptos.Where(x => x.IDListaPrecios == id).AsQueryable();
                    var listaConceptosSinLista = new List<Conceptos>();

                    foreach (var item in listaPreciosConceptos)
                        listaConceptosSinLista.Add(item.Conceptos);

                    var listaConceptos = dbContext.Conceptos.Where(X => X.IDUsuario == usu.IDUsuario).ToList().Except(listaConceptosSinLista).Select(x => new listaPreciosConceptosViewModel()
                    {
                        IDConcepto = x.IDConcepto,
                        Nombre = x.Nombre.ToUpper(),
                        Codigo = x.Codigo,
                        Tipo = (x.Tipo == "P") ? "Productos" : "Servicios",
                        Precio = x.PrecioUnitario.ToString("N2"),
                        PrecioLista = "0"
                    });

                    var list = listaPreciosConceptos.OrderByDescending(x => x.Conceptos.IDConcepto).ToList()
                     .Select(x => new listaPreciosConceptosViewModel()
                     {
                         ID = x.IDPrecioConcepto,
                         IDConcepto = x.Conceptos.IDConcepto,
                         Nombre = x.Conceptos.Nombre.ToUpper(),
                         Codigo = x.Conceptos.Codigo,
                         Tipo = (x.Conceptos.Tipo == "P") ? "Productos" : "Servicios",
                         Precio = x.Conceptos.PrecioUnitario.ToString("N2"),
                         //PrecioLista = x.Precio.ToString("N2")
                         PrecioLista = x.Precio.ToString().Replace(",",".")
                     });
                    resultado.Conceptos = list.Union(listaConceptos).OrderBy(x => x.IDConcepto).ToList();
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
        private static void ParceListaDePrecios(ACHEEntities dbContext, List<PreciosConceptos> listaDePrecios, int idListaPrecio)
        {
            //PreciosConceptos pc;
            //List<PreciosConceptos> ListaPC = new List<PreciosConceptos>();
            //var precios = listaDePrecios.Split('-').ToList();
            //precios.RemoveAt(0);

            //for (int i = 0; i < precios.Count(); i++)
            //{
            //    var precio = precios[i].Split('#').ToList();
            //    precio.RemoveAt(0);

            //    pc = new PreciosConceptos();
            //    pc.IDPrecioConcepto = Convert.ToInt32(precio[0]);
            //    pc.IDConceptos = Convert.ToInt32(precio[1]);
            //    pc.Precio = decimal.Parse(precio[2].Replace(SeparadorDeMiles, SeparadorDeDecimales));
            //    ListaPC.Add(pc);
            //}

            foreach (var item in listaDePrecios)
            {
                PreciosConceptos entity;
                if (item.IDPrecioConcepto > 0)
                    entity = dbContext.PreciosConceptos.Where(x => x.IDPrecioConcepto == item.IDPrecioConcepto).FirstOrDefault();
                else
                {
                    entity = new PreciosConceptos();
                }
                entity.IDListaPrecios = idListaPrecio;
                entity.IDConceptos = item.IDConceptos;

                entity.Precio = item.Precio;

                if (item.IDPrecioConcepto == 0)
                    dbContext.PreciosConceptos.Add(entity);

                dbContext.SaveChanges();
            }
        }
    }
}