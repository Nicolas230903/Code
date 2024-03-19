using ACHE.Extensions;
using ACHE.FacturaElectronica;
using ACHE.FacturaElectronica.Lib;
using ACHE.FacturaElectronica.VEConsumerService;
using ACHE.FacturaElectronica.WSPersonaServiceA5v34;
using ACHE.Model;
using ACHE.Model.Negocio;
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
    public class DatosPersonaAfipCommon
    {
        public static int RegistrarEnvioLog(string entidad, string url, string nombre, string mensaje, int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    DateTime dateTime = DateTime.Now.AddMonths(-1);
                    List<LogServicios> logServicios = dbContext.LogServicios.Where(x => x.FechaEmision < dateTime).ToList();
                    if (logServicios != null)
                    {
                        dbContext.LogServicios.RemoveRange(logServicios);
                        dbContext.SaveChanges();
                    }
                    List<DatosAFIPPersonas> logDatosAFIPPersonas = dbContext.DatosAFIPPersonas.Where(x => x.Fecha < dateTime).ToList();
                    if (logDatosAFIPPersonas != null)
                    {
                        dbContext.DatosAFIPPersonas.RemoveRange(logDatosAFIPPersonas);
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

        public static Boolean IsDate(string valor)
        {
            try
            {
                DateTime resultado = Convert.ToDateTime(valor);
                if (resultado < DateTime.Now.AddYears(-100))
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DatosAfipPersonasConGeo ObtenerDatosPersona(long cuit, WebUser usu)
        {
            try
            {
                var dbContext = new ACHEEntities();

                DatosAFIPPersonas d = new DatosAFIPPersonas();
                DatosAfipPersonasConGeo g = new DatosAfipPersonasConGeo();

                AFIPPersonaServiceA5v34 s = new AFIPPersonaServiceA5v34();

                var modo = usu.ModoQA ? "QA" : "PROD";
                var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_sr_padron_a5"] : ConfigurationManager.AppSettings["FE.PROD.ws_sr_padron_a5"]);
                int CodigoLogServicio = RegistrarEnvioLog("AFIP", url, "AFIPPersonaServiceA5", "GetPersona(" + cuit + ")", usu.IDUsuario);
                personaReturn persona = null;

                try
                {
                    persona = s.GetPersona_v2(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo);
                }
                catch (Exception ex)
                {
                    RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip v2: " + ex.Message, false);
                    if (ex.Message.Equals("No existe persona con ese Id"))
                        throw new Exception("ERROR: " + "Respuesta Afip v2: No encontramos datos en AFIP con el CUIT ingresado.");
                    else
                    {
                        try
                        {
                            persona = s.GetPersona_v2(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo);
                        }
                        catch (Exception ex2)
                        {
                            RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip v1: " + ex2.Message, false);
                            if (ex2.Message.Equals("No existe persona con ese Id"))
                                throw new Exception("ERROR: " + "Respuesta Afip v1: No encontramos datos en AFIP con el CUIT ingresado.");
                            else
                                throw new Exception("ERROR: " + "Respuesta Afip v1: No encontramos datos en AFIP con el CUIT ingresado.");
                        }
                    }
                }

                if (persona.datosGenerales != null)
                {
                    string condicionIva = "CF";
                    //Me fijo el dato que me llega del array DatosRegimenGeneral
                    if (persona.datosRegimenGeneral != null)
                    {
                        if (persona.datosRegimenGeneral.impuesto != null)
                        {
                            foreach (impuesto i in persona.datosRegimenGeneral.impuesto)
                            {
                                if (i.descripcionImpuesto.ToUpper().Contains("EXENTO"))
                                {
                                    condicionIva = "EX";
                                    break;
                                }
                                if (i.descripcionImpuesto.ToUpper().Equals("IVA"))
                                {
                                    condicionIva = "RI";
                                    break;
                                }
                                if (i.descripcionImpuesto.ToUpper().Contains("MONOTRIBUTO"))
                                {
                                    condicionIva = "MO";
                                    break;
                                }
                            }
                        }
                    }

                    //Me fijo el dato que me llega del array MONOTRIBUTO
                    if (persona.datosMonotributo != null)
                    {
                        if (persona.datosMonotributo.impuesto != null)
                        {
                            foreach (impuesto i in persona.datosMonotributo.impuesto)
                            {
                                if (i.descripcionImpuesto.ToUpper().Contains("MONOTRIBUTO"))
                                {
                                    condicionIva = "MO";
                                    break;
                                }
                            }
                            if (persona.datosMonotributo.categoriaMonotributo != null)
                            {
                                d.CategoriaMonotributoDescripcionCategoria = persona.datosMonotributo.categoriaMonotributo.descripcionCategoria;
                                d.CategoriaMonotributoIdCategoria = persona.datosMonotributo.categoriaMonotributo.idCategoria.ToString();
                                d.CategoriaMonotributoIdImpuesto = persona.datosMonotributo.categoriaMonotributo.idImpuesto.ToString();
                                d.CategoriaMonotributoPeriodo = persona.datosMonotributo.categoriaMonotributo.periodo.ToString();
                            }
                        }
                    }

                    //Registro en la tabla DatosAfipPersona los datos de afip del cuit ingresado
                    d.CategoriaImpositiva = condicionIva; //Sin Datos
                    d.CUIT = persona.datosGenerales.idPersona.ToString();
                    if (persona.datosGenerales.razonSocial != null)
                    {
                        d.RazonSocial = persona.datosGenerales.razonSocial;
                    }
                    else
                    {
                        if (persona.datosGenerales.apellido != null && persona.datosGenerales.nombre != null)
                        {
                            d.RazonSocial = persona.datosGenerales.apellido + " " + persona.datosGenerales.nombre;
                        }
                    }

                    if (persona.datosGenerales.fechaContratoSocial != null)
                    {
                        if (IsDate(persona.datosGenerales.fechaContratoSocial.ToString()))
                        {
                            d.FechaContratoSocial = persona.datosGenerales.fechaContratoSocial;
                            g.FechaInicioActividades = d.FechaContratoSocial.ToString();
                        }

                    }

                    d.Personeria = persona.datosGenerales.tipoPersona;
                    d.Fecha = DateTime.Now;
                    d.IdUsuario = usu.IDUsuario;
                    d.DomicilioFiscalCP = persona.datosGenerales.domicilioFiscal.codPostal;
                    d.DomicilioFiscalIdProvincia = persona.datosGenerales.domicilioFiscal.idProvincia;
                    d.DomicilioFiscalProvincia = persona.datosGenerales.domicilioFiscal.descripcionProvincia;
                    d.DomicilioFiscalCiudad = persona.datosGenerales.domicilioFiscal.localidad;
                    d.DomicilioFiscalDomicilio = persona.datosGenerales.domicilioFiscal.direccion;
                    dbContext.DatosAFIPPersonas.Add(d);
                    dbContext.SaveChanges();

                    g.CategoriaImpositiva = d.CategoriaImpositiva;
                    g.CUIT = d.CUIT;
                    g.RazonSocial = d.RazonSocial;
                    g.Personeria = d.Personeria;
                    g.Fecha = d.Fecha;
                    g.IdUsuario = d.IdUsuario;
                    g.DomicilioFiscalCP = d.DomicilioFiscalCP;
                    g.DomicilioFiscalIdProvincia = d.DomicilioFiscalIdProvincia;
                    g.DomicilioFiscalProvincia = d.DomicilioFiscalProvincia;
                    g.DomicilioFiscalCiudad = d.DomicilioFiscalCiudad;
                    g.DomicilioFiscalDomicilio = d.DomicilioFiscalDomicilio;
                    g.CategoriaMonotributoDescripcionCategoria = d.CategoriaMonotributoDescripcionCategoria;
                    g.CategoriaMonotributoIdCategoria = d.CategoriaMonotributoIdCategoria;
                    g.CategoriaMonotributoIdImpuesto = d.CategoriaMonotributoIdImpuesto;
                    g.CategoriaMonotributoPeriodo = d.CategoriaMonotributoPeriodo;


                    //Obtengo el codigo de provincia del sistema desde el codigo de afip
                    Provincias p = dbContext.Provincias.Where(x => x.IDProvincia == persona.datosGenerales.domicilioFiscal.idProvincia).FirstOrDefault();
                    if (p != null)
                    {
                        g.IdProvincia = p.IDProvincia;

                        Ciudades primeraCiudad = dbContext.Ciudades.Where(x => x.IDProvincia == p.IDProvincia).FirstOrDefault();

                        if (persona.datosGenerales.domicilioFiscal.localidad != null)
                        {
                            Ciudades c = dbContext.Ciudades.Where(x => x.IDProvincia == p.IDProvincia && x.Nombre == persona.datosGenerales.domicilioFiscal.localidad).FirstOrDefault();
                            if (c != null)
                                g.IdCiudad = c.IDCiudad;
                            else
                                g.IdCiudad = primeraCiudad.IDCiudad;
                        }
                        else
                        {
                            g.IdCiudad = primeraCiudad.IDCiudad;
                        }
                    }
                    else
                        g.IdProvincia = 0;


                    if (g.IdProvincia == 0)
                        g.IdCiudad = 24071; //Si la provincia es caba, se setea la localidad CABA y se deja deshabilitado.

                }

                XmlSerializer xsSubmit = new XmlSerializer(typeof(personaReturn));
                var subReq = new persona();
                var xml = "";
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, persona);
                        xml = sww.ToString(); // Your XML
                    }
                }
                RegistrarRespuestaLog(CodigoLogServicio, 1, xml, true);

                return g;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # DatosPersonaAfipCommon.cs # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception("ERROR: " + ex.Message);
            }

        }

    }
}
