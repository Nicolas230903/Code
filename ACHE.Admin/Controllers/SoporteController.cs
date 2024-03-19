using System;
using System.Configuration;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Negocio.Facturacion;


namespace ACHE.Admin.Controllers
{
    public class SoporteController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarComprobante(string nroComprobante, string cuitUsuario, string tipoComprobante, int punto)
        {
            var dto = new ErrorViewModel();
            try
            {
                var existe = ComprobantesCommon.ExisteComprobante(Convert.ToInt64(cuitUsuario), Convert.ToInt64(nroComprobante), punto, tipoComprobante);
                if (!existe)
                {
                    long cuitRep = Convert.ToInt64(ConfigurationManager.AppSettings["FE.QA.CUIL"]);
                    var comprobante = ComprobantesCommon.ObtenerComprobanteElectronica(Convert.ToInt64(cuitUsuario), cuitRep, Convert.ToInt64(nroComprobante), punto, tipoComprobante);
                    ComprobantesCommon.InsertarComprobanteRecuperado(comprobante, tipoComprobante, cuitUsuario.ToString());
                    dto.TieneError = false;
                }
                else
                {
                    dto.TieneError = true;
                    dto.Mensaje = "Ya exise un comprobante para los datos seleccionados.";
                }
            }
            catch (Exception ex)
            {
                dto.TieneError = true;
                dto.Mensaje = ex.Message;
            }

            return Json(dto, JsonRequestBehavior.AllowGet);
        }
    }
}