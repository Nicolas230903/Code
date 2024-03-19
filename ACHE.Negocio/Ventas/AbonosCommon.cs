using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACHE.Negocio.Abono
{
    public class AbonosCommon
    {
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static Abonos GuardarAbono(int id, string nombre, string frecuencia, string fechaInicio, string fechaFin, string estado, string precio, string iva, string obs, List<AbonosPersonasViewModel> personas, int tipo, WebUser usu,int idPlanDeCuenta)
        {
            using (var dbContext = new ACHEEntities())
            {
                Abonos entity;
                if (id > 0)
                    entity = dbContext.Abonos.Where(x => x.IDAbono == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new Abonos();
                    entity.IDUsuario = usu.IDUsuario;
                }

                entity.Frecuencia = frecuencia;
                entity.Nombre = nombre.ToUpper();
                entity.Estado = estado;
                entity.FechaInicio = DateTime.Parse(fechaInicio);
                if (fechaFin != string.Empty)
                    entity.FechaFin = DateTime.Parse(fechaFin);
                else
                    entity.FechaFin = null;
                entity.PrecioUnitario = decimal.Parse(precio.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                entity.Iva = decimal.Parse(iva.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                entity.Observaciones = obs;
                entity.Tipo = tipo;

                if (idPlanDeCuenta > 0)
                    entity.IDPlanDeCuenta = idPlanDeCuenta;

                foreach (var p in personas)
                {
                    AbonosPersona per = new AbonosPersona();
                    per.IDPersona = p.IDPersona;
                    per.IDAbono = p.IDAbono;
                    per.Cantidad = (p.Cantidad == "" || p.Cantidad == "0") ? 1 : Math.Abs(Convert.ToInt32(p.Cantidad));
                    dbContext.AbonosPersona.Add(per);
                }

                if (id > 0)
                {
                    dbContext.Database.ExecuteSqlCommand("DELETE AbonosPersona WHERE IDAbono=" + id, new object[] { });
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.Abonos.Add(entity);
                    dbContext.SaveChanges();
                }
                return entity;
            }
        }
    }
}