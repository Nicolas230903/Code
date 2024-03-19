using ACHE.Extensions;
using ACHE.FacturaElectronica;
using ACHE.FacturaElectronica.Lib;
using ACHE.FacturaElectronica.VEConsumerService;
using ACHE.Model;
using ACHE.Model.Negocio.ComunicacionesAfip;
using ACHE.Negocio.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace ACHE.Negocio.Common
{
    public class ComunicacionesAfipCommon
    {
        public static int RegistrarEnvioLog(string entidad, string url, string nombre, string mensaje, int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    List<LogServicios> logServicios = dbContext.LogServicios.Where(x => x.FechaEmision < DateTime.Now.AddMonths(-2)).ToList();
                    if (logServicios != null)
                    {
                        dbContext.LogServicios.RemoveRange(logServicios);
                        dbContext.SaveChanges();
                    }

                    LogServicios l = new LogServicios();
                    l.Entidad = entidad;
                    l.Url = url;
                    l.Nombre = nombre;
                    l.Mensaje = mensaje;
                    l.FechaEmision = DateTime.Now;
                    l.IdUsuario = idUsuario;
                    l.Envio = 0;
                    l.Respuesta = null;
                    l.RespuestaExitosa = false;
                    l.FechaRespuesta = null;
                    dbContext.LogServicios.Add(l);
                    dbContext.SaveChanges();
                    return l.IDLogServicio;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void RegistrarRespuestaLog(int idLogServicio, int envio, string respuesta, bool respuestaExitosa)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    LogServicios logServicio = dbContext.LogServicios.Where(x => x.IDLogServicio == idLogServicio).FirstOrDefault();
                    if (logServicio != null)
                    {
                        logServicio.Envio = envio;
                        logServicio.Respuesta = respuesta;
                        logServicio.RespuestaExitosa = respuestaExitosa;
                        logServicio.FechaRespuesta = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<ComunicacionesAfipConAdjunto> ObtenerComunicacionesAfipConAdjunto(string cuit, long desdeIdComunicacion, long hastaIdComunicacion, string desdeFecha, string hastaFecha, WebUser usu)
        {
            try
            {
                var dbContext = new ACHEEntities();

                AFIPVEConsumerService s = new AFIPVEConsumerService();

                var modo = usu.ModoQA ? "QA" : "PROD";
                var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);
                int CodigoLogServicio = RegistrarEnvioLog("AFIP", url, "ws_ve_consumer", "GetConsultarComunicaciones(" + cuit + ")", usu.IDUsuario);
                List<RespuestaPaginada> listResPag = null;

                try
                {
                    listResPag = s.GetComunicaciones(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo, desdeIdComunicacion, hastaIdComunicacion, desdeFecha, hastaFecha, false);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Equals("La cuitRepresentada informada no se encuentra autorizada para la consulta"))
                    {
                        try
                        {
                            listResPag = s.GetComunicaciones(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo, desdeIdComunicacion, true);
                        }
                        catch (Exception e)
                        {
                            RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + e.Message, false);
                            throw new Exception("ERROR: " + "Respuesta Afip: " + e.Message);
                        }
                    }
                    else
                    {
                        RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + ex.Message, false);
                        throw new Exception("ERROR: " + "Respuesta Afip: " + ex.Message);
                    }
                }

                List<ComunicacionesAFIP> nc = new List<ComunicacionesAFIP>();
                List<ComunicacionesAFIPAdjuntos> lca = new List<ComunicacionesAFIPAdjuntos>();

                if (listResPag.Count > 0)
                {
                    foreach (RespuestaPaginada rp in listResPag)
                    {
                        if (rp.items != null)
                        {
                            foreach (ComunicacionSimplificada i in rp.items)
                            {
                                ComunicacionesAFIP c = dbContext.ComunicacionesAFIP.Where(x => x.IdComunicacion == i.idComunicacion).FirstOrDefault();
                                if (c == null)
                                {
                                    ComunicacionesAFIP cn = new ComunicacionesAFIP();
                                    cn.IdComunicacion = i.idComunicacion;
                                    cn.CuitDestinatario = i.cuitDestinatario.ToString();
                                    cn.FechaPublicacion = i.fechaPublicacion;
                                    cn.fechaVencimiento = i.fechaVencimiento != null ? i.fechaVencimiento : "";
                                    cn.SistemaPublicador = i.sistemaPublicador;
                                    cn.sistemaPublicadorDesc = i.sistemaPublicadorDesc;
                                    cn.Estado = i.estado;
                                    cn.EstadoDesc = i.estadoDesc;
                                    
                                    cn.Prioridad = i.prioridad;
                                    if (i.prioridad == 1)
                                        cn.PrioridadDesc = "Alta";
                                    if (i.prioridad == 2)
                                        cn.PrioridadDesc = "Media";
                                    if (i.prioridad == 3)
                                        cn.PrioridadDesc = "Baja";                                    

                                    Comunicacion adj = s.GetAdjuntos(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), i.idComunicacion, modo, false);
                                    if (adj != null)
                                    {
                                        cn.Asunto = adj.mensaje;

                                        int contador = 0;

                                        foreach (adjunto a in adj.adjuntos)
                                        {
                                            ComunicacionesAFIPAdjuntos ca = new ComunicacionesAFIPAdjuntos();
                                            ca.Comprimido = a.compressed == true ? 1 : 0;
                                            ca.Contenido = a.content;
                                            ca.Encriptado = a.encrypted == true ? 1 : 0;
                                            ca.Firmado = a.signed == true ? 1 : 0;
                                            ca.IdComunicacion = i.idComunicacion;
                                            ca.MD5 = a.md5;
                                            ca.NombreArchivo = a.filename;
                                            ca.Procesado = a.processed == true ? 1 : 0;
                                            ca.Tamaño = a.contentSize;
                                            ca.Publico = a.@public == true ? 1 : 0;
                                            lca.Add(ca);
                                            contador++;
                                        }

                                        if(contador == 0)
                                            cn.TieneAdjunto = 0;
                                        else
                                            cn.TieneAdjunto = 1;

                                    }                                                                                                          


                                    cn.Visto = 0;
                                    cn.IdUsuario = usu.IDUsuario;
                                    cn.FechaRegistro = DateTime.Now;
                                    nc.Add(cn);
                                }
                            }
                        }
                    }
                    dbContext.ComunicacionesAFIP.AddRange(nc);
                    dbContext.SaveChanges();
                    dbContext.ComunicacionesAFIPAdjuntos.AddRange(lca);
                    dbContext.SaveChanges();
                }

                XmlSerializer xsSubmit = new XmlSerializer(typeof(List<RespuestaPaginada>));
                var subReq = new List<RespuestaPaginada>();
                var xml = "";
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, listResPag);
                        xml = sww.ToString(); // Your XML
                    }
                }
                RegistrarRespuestaLog(CodigoLogServicio, 1, xml, true);

                List<ComunicacionesAfipConAdjunto> respuestaComunicacion = new List<ComunicacionesAfipConAdjunto>();
                List<ComunicacionesAFIP> rnc = new List<ComunicacionesAFIP>();

                rnc = dbContext.ComunicacionesAFIP.Where(x => x.IdComunicacion >= desdeIdComunicacion && x.CuitDestinatario == cuit).ToList();

                if (hastaIdComunicacion != -1)
                {
                    rnc = rnc.Where(x => x.IdComunicacion <= hastaIdComunicacion).ToList();
                }

                if (!desdeFecha.Equals("19000101"))
                {
                    DateTime fechaDesde = new DateTime(Convert.ToInt16(desdeFecha.Substring(0, 4)), Convert.ToInt16(desdeFecha.Substring(4, 2)), Convert.ToInt16(desdeFecha.Substring(6, 2)),0,0,0);
                    rnc = rnc.Where(x => DateTime.Parse(x.FechaPublicacion) >= fechaDesde).ToList();
                }

                if (!hastaFecha.Equals("19000101"))
                {
                    DateTime fechaHasta = new DateTime(Convert.ToInt16(hastaFecha.Substring(0, 4)), Convert.ToInt16(hastaFecha.Substring(4, 2)), Convert.ToInt16(hastaFecha.Substring(6, 2)),23,59,59);
                    rnc = rnc.Where(x => DateTime.Parse(x.FechaPublicacion) <= fechaHasta).ToList();
                }           

                foreach (ComunicacionesAFIP i in rnc)
                {
                    ComunicacionesAfipConAdjunto cn = new ComunicacionesAfipConAdjunto();
                    cn.IdComunicacion = i.IdComunicacion;
                    cn.CuitDestinatario = i.CuitDestinatario;
                    cn.FechaPublicacion = i.FechaPublicacion;
                    cn.fechaVencimiento = i.fechaVencimiento;
                    cn.SistemaPublicador = i.SistemaPublicador;
                    cn.sistemaPublicadorDesc = i.sistemaPublicadorDesc;
                    cn.Estado = i.Estado;
                    cn.EstadoDesc = i.EstadoDesc;
                    cn.Asunto = i.Asunto;
                    cn.Prioridad = i.Prioridad;
                    cn.PrioridadDesc = i.PrioridadDesc;
                    cn.TieneAdjunto = i.TieneAdjunto;
                    cn.Visto = i.Visto;
                    cn.IdUsuario = i.IdUsuario;
                    cn.FechaRegistro = i.FechaRegistro;

                    List<ComunicacionesAFIPAdjuntos> rnca = new List<ComunicacionesAFIPAdjuntos>();

                    rnca = dbContext.ComunicacionesAFIPAdjuntos.Where(x => x.IdComunicacion == i.IdComunicacion).ToList();

                    List<ArchivoAdjunto> ladj = new List<ArchivoAdjunto>();

                    foreach (ComunicacionesAFIPAdjuntos r in rnca)
                    {
                        ArchivoAdjunto adj = new ArchivoAdjunto();
                        adj.NombreArchivo = r.NombreArchivo;
                        adj.Contenido = Convert.ToBase64String(r.Contenido, 0, Convert.ToInt32(r.Tamaño));
                        ladj.Add(adj);
                    }

                    cn.Adjuntos = ladj;

                    respuestaComunicacion.Add(cn);

                }

                return respuestaComunicacion;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # ComunicacionesAfipCommon.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception("ERROR: " + ex.Message);
            }

        }

        public static List<ComunicacionesAfipSinAdjunto> ObtenerComunicacionesAfipSinAdjunto(string cuit, long desdeIdComunicacion, long hastaIdComunicacion, string desdeFecha, string hastaFecha, WebUser usu)
        {
            try
            {
                var dbContext = new ACHEEntities();

                AFIPVEConsumerService s = new AFIPVEConsumerService();

                var modo = usu.ModoQA ? "QA" : "PROD";
                var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);
                int CodigoLogServicio = RegistrarEnvioLog("AFIP", url, "ws_ve_consumer", "GetConsultarComunicaciones(" + cuit + ")", usu.IDUsuario);
                List<RespuestaPaginada> listResPag = null;

                try
                {
                    listResPag = s.GetComunicaciones(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo, desdeIdComunicacion, hastaIdComunicacion, desdeFecha, hastaFecha, false);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Equals("La cuitRepresentada informada no se encuentra autorizada para la consulta"))
                    {
                        try
                        {
                            listResPag = s.GetComunicaciones(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo, desdeIdComunicacion, true);
                        }
                        catch (Exception e)
                        {
                            RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + e.Message, false);
                            throw new Exception("ERROR: " + "Respuesta Afip: " + e.Message);
                        }
                    }
                    else
                    {
                        RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + ex.Message, false);
                        throw new Exception("ERROR: " + "Respuesta Afip: " + ex.Message);
                    }
                }

                List<ComunicacionesAFIP> nc = new List<ComunicacionesAFIP>();
                List<ComunicacionesAFIPAdjuntos> lca = new List<ComunicacionesAFIPAdjuntos>();

                if (listResPag.Count > 0)
                {
                    foreach (RespuestaPaginada rp in listResPag)
                    {
                        if (rp.items != null)
                        {
                            foreach (ComunicacionSimplificada i in rp.items)
                            {
                                ComunicacionesAFIP c = dbContext.ComunicacionesAFIP.Where(x => x.IdComunicacion == i.idComunicacion).FirstOrDefault();
                                if (c == null)
                                {
                                    ComunicacionesAFIP cn = new ComunicacionesAFIP();
                                    cn.IdComunicacion = i.idComunicacion;
                                    cn.CuitDestinatario = i.cuitDestinatario.ToString();
                                    cn.FechaPublicacion = i.fechaPublicacion;
                                    cn.fechaVencimiento = i.fechaVencimiento != null ? i.fechaVencimiento : "";
                                    cn.SistemaPublicador = i.sistemaPublicador;
                                    cn.sistemaPublicadorDesc = i.sistemaPublicadorDesc;
                                    cn.Estado = i.estado;
                                    cn.EstadoDesc = i.estadoDesc;
                                    cn.Prioridad = i.prioridad;
                                    if (i.prioridad == 1)
                                        cn.PrioridadDesc = "Alta";
                                    if (i.prioridad == 2)
                                        cn.PrioridadDesc = "Media";
                                    if (i.prioridad == 3)
                                        cn.PrioridadDesc = "Baja";

                                    Comunicacion adj = s.GetAdjuntos(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), i.idComunicacion, modo, false);
                                    if (adj != null)
                                    {
                                        cn.Asunto = adj.mensaje;

                                        int contador = 0;

                                        foreach (adjunto a in adj.adjuntos)
                                        {
                                            ComunicacionesAFIPAdjuntos ca = new ComunicacionesAFIPAdjuntos();
                                            ca.Comprimido = a.compressed == true ? 1 : 0;
                                            ca.Contenido = a.content;
                                            ca.Encriptado = a.encrypted == true ? 1 : 0;
                                            ca.Firmado = a.signed == true ? 1 : 0;
                                            ca.IdComunicacion = i.idComunicacion;
                                            ca.MD5 = a.md5;
                                            ca.NombreArchivo = a.filename;
                                            ca.Procesado = a.processed == true ? 1 : 0;
                                            ca.Tamaño = a.contentSize;
                                            ca.Publico = a.@public == true ? 1 : 0;
                                            lca.Add(ca);
                                            contador++;
                                        }

                                        if (contador == 0)
                                            cn.TieneAdjunto = 0;
                                        else
                                            cn.TieneAdjunto = 1;

                                    }

                                    cn.Visto = 0;
                                    cn.IdUsuario = usu.IDUsuario;
                                    cn.FechaRegistro = DateTime.Now;
                                    nc.Add(cn);
                                }
                            }
                        }
                    }
                    dbContext.ComunicacionesAFIP.AddRange(nc);
                    dbContext.SaveChanges();
                    dbContext.ComunicacionesAFIPAdjuntos.AddRange(lca);
                    dbContext.SaveChanges();
                }

                XmlSerializer xsSubmit = new XmlSerializer(typeof(List<RespuestaPaginada>));
                var subReq = new List<RespuestaPaginada>();
                var xml = "";
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, listResPag);
                        xml = sww.ToString(); // Your XML
                    }
                }
                RegistrarRespuestaLog(CodigoLogServicio, 1, xml, true);

                List<ComunicacionesAfipSinAdjunto> respuestaComunicacion = new List<ComunicacionesAfipSinAdjunto>();
                List<ComunicacionesAFIP> rnc = new List<ComunicacionesAFIP>();

                rnc = dbContext.ComunicacionesAFIP.Where(x => x.IdComunicacion >= desdeIdComunicacion && x.CuitDestinatario == cuit).ToList();

                if (hastaIdComunicacion != -1)
                {
                    rnc = rnc.Where(x => x.IdComunicacion <= hastaIdComunicacion).ToList();
                }

                if (!desdeFecha.Equals("19000101"))
                {
                    DateTime fechaDesde = new DateTime(Convert.ToInt16(desdeFecha.Substring(0, 4)), Convert.ToInt16(desdeFecha.Substring(4, 2)), Convert.ToInt16(desdeFecha.Substring(6, 2)), 0, 0, 0);
                    rnc = rnc.Where(x => DateTime.Parse(x.FechaPublicacion) >= fechaDesde).ToList();
                }

                if (!hastaFecha.Equals("19000101"))
                {
                    DateTime fechaHasta = new DateTime(Convert.ToInt16(hastaFecha.Substring(0, 4)), Convert.ToInt16(hastaFecha.Substring(4, 2)), Convert.ToInt16(hastaFecha.Substring(6, 2)), 23, 59, 59);
                    rnc = rnc.Where(x => DateTime.Parse(x.FechaPublicacion) <= fechaHasta).ToList();
                }

                foreach (ComunicacionesAFIP i in rnc)
                {
                    ComunicacionesAfipSinAdjunto cn = new ComunicacionesAfipSinAdjunto();
                    cn.IdComunicacion = i.IdComunicacion;
                    cn.CuitDestinatario = i.CuitDestinatario;
                    cn.FechaPublicacion = i.FechaPublicacion;
                    cn.fechaVencimiento = i.fechaVencimiento;
                    cn.SistemaPublicador = i.SistemaPublicador;
                    cn.sistemaPublicadorDesc = i.sistemaPublicadorDesc;
                    cn.Estado = i.Estado;
                    cn.EstadoDesc = i.EstadoDesc;
                    cn.Asunto = i.Asunto;
                    cn.Prioridad = i.Prioridad;
                    cn.PrioridadDesc = i.PrioridadDesc;
                    cn.TieneAdjunto = i.TieneAdjunto;
                    cn.Visto = i.Visto;
                    cn.IdUsuario = i.IdUsuario;
                    cn.FechaRegistro = i.FechaRegistro;
                    respuestaComunicacion.Add(cn);
                }

                return respuestaComunicacion;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # ComunicacionesAfipCommon.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception("ERROR: " + ex.Message);
            }

        }

    }
}
