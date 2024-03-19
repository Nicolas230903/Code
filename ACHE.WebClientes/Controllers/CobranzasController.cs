using ACHE.WebClientes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Extensions;
using System.Web.Security;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace ACHE.WebClientes.Controllers
{
    public class CobranzasController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Cobranzas(string tipo, string external_reference)
        {
            var model = new FacturaViewModel();

            var resultado = string.Empty;
            var MensajeResultado = string.Empty;

            switch (tipo)
            {
                case "1":
                    resultado = "Exitoso";
                    MensajeResultado = "El pago fue realizado correctamente";
                    break;

                case "2":
                    resultado = "Pendiente";
                    MensajeResultado = "El pago se encuentra Pendiente";
                    break;
                case "5":
                    resultado = "Fallido";
                    MensajeResultado = "El pago no pudo ser realizado correctamente";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(external_reference))
            {
                var idComprobante = Convert.ToInt32(external_reference);
                using (var dbContext = new ACHEEntities())
                {
                    var comprobante = dbContext.Comprobantes.Where(x => x.IDComprobante == idComprobante).FirstOrDefault();
                    model.IDFactura = comprobante.IDComprobante;
                    model.Emisor = comprobante.Personas.RazonSocial;
                    model.NroComprobante = comprobante.Tipo + "-" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                    model.Importe = comprobante.ImporteTotalNeto;

                    model.ResultadoOperacion = resultado;
                    model.MensajeResultadoOperacion = MensajeResultado;
                }
            }
            return View(model);
        }
    }
}