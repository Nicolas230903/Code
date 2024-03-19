using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.ViewModels;
using System.IO;
using System.Configuration;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using ACHE.Model.Negocio.TiendaNube;
using System.Dynamic;
using ACHE.Negocio.Helper;
using ACHE.Model.Negocio.Licencia;

namespace ACHE.Negocio.Common
{
    public class AuditoriaCommon
    {
        public const string formatoFecha = "dd/MM/yyyy hh:mm";//"dd/MM/yyyy"
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","

        public static ResultadosAuditoriaViewModel ObtenerRegistros(string condicion, string periodo,
            string fechaDesde, string fechaHasta, 
            int page, int pageSize, WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    List<Auditoria> la = new List<Auditoria>();

                    condicion = (!string.IsNullOrWhiteSpace(condicion)) ? condicion : "";
                    var results = dbContext.AuditoriaDeCambio
                                            .Where(x => x.IdUsuario == usu.IDUsuario).AsQueryable();
                  

                    if (condicion != "")
                    {
                        results = results.Where(w => w.ValorAnterior.Contains(condicion) ||
                                                    w.ValorNuevo.Contains(condicion) ||
                                                    w.Columna.Contains(condicion)).AsQueryable();
                    }                    

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
                            results = results.Where(x => x.Fecha >= dtDesde);
                        }
                        if (!string.IsNullOrWhiteSpace(fechaHasta))
                        {
                            DateTime dtHasta = DateTime.Parse(fechaHasta + " 12:59:59 pm");
                            results = results.Where(x => x.Fecha <= dtHasta);
                        }
                    }       
                    

                    page--;
                    ResultadosAuditoriaViewModel resultado = new ResultadosAuditoriaViewModel();
                    resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = results.Count();

                    var list = results.OrderBy(x => x.Fecha).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new AuditoriaViewModel()
                        {
                            ID = x.idRegistro,
                            ValorAnterior = x.ValorAnterior,
                            ValorNuevo = x.ValorNuevo,
                            Tabla = x.Tabla,
                            Columna = x.Columna,
                            Identificador = x.IdentificadorDeReferencia,
                            Usuario = x.Usuarios.RazonSocial,
                            Fecha = x.Fecha.ToString(formatoFecha)
                        });
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

    }
}
