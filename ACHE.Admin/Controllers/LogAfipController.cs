using ACHE.Admin.Models;
using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ACHE.Admin.Controllers
{
    public class LogAfipController : BaseController
    {
        // GET: LogAfip
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerLog(string cuit, string take)
        {
            ResultadosLogAfipViewModel resultado = new ResultadosLogAfipViewModel();

            try
            {
                using (var dbContext = new ACHEEntities())
                {

                     List<vLogServicios> results;

                    if (cuit.Equals("NuN")) {                        
                        results = dbContext.vLogServicios.OrderByDescending(x => x.IDLogServicio).Take(Convert.ToInt32(take)).ToList();
                    } else {
                        results = dbContext.vLogServicios.Where(c => c.CUIT == cuit).OrderByDescending(x => x.IDLogServicio).Take(Convert.ToInt32(take)).ToList();
                    }                    

                    var list = results.ToList()
                             .Select(x => new LogAfipViewModel()
                             {
                                 ID = x.IDLogServicio,
                                 Entidad = x.Entidad,
                                 Url = x.Url.Length > 10 ? x.Url.Substring(0, 10) + "..." : x.Url,
                                 Nombre = x.Nombre,
                                 Mensaje = x.Mensaje.Length > 10 ? x.Mensaje.Substring(0, 10) + "..." : x.Mensaje,
                                 FechaEmision = x.FechaEmision,
                                 UsuarioCUIT = x.CUIT,
                                 RazonSocial = x.RazonSocial,
                                 Envio = x.Envio,
                                 Respuesta = x.Respuesta.Length > 10 ? x.Respuesta.Substring(0, 10) + "..." : x.Respuesta,
                                 RespuestaExitosa = x.RespuestaExitosa,
                                 FechaRespuesta = x.FechaRespuesta
                             });
                    resultado.Items = list.ToList();
                }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        // GET: LogAfip
        public ActionResult View(int id)
        {
            var dbContext = new ACHEEntities();

            vLogServicios v = dbContext.vLogServicios.Where(x => x.IDLogServicio == id).FirstOrDefault();

            v.Respuesta = PrintXML(v.Respuesta.Trim());

            return View(v);
        }


        public static string PrintXML(string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
                return result;
            }
            catch (XmlException)
            {
                return xml;
            }
        }
    }
}