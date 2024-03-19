using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Extensions;
using ACHE.WebClientes.Models;
using System.Configuration;
using ACHE.MercadoPago;

namespace ACHE.WebClientes.Controllers
{
    public class HomeController : BaseController
    {

        private HtmlString integracionMercadoPago(Comprobantes comprobante)
        {
            var btnMP = string.Empty;
            try
            {
                //BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MP.LogError"]), "boton pagar entro ","");
                if (comprobante.Saldo > 0)
                {
                    API.ClientIDMercadoPago = comprobante.Usuarios.MercadoPagoClientID;
                    API.PasswordMercadoPago = comprobante.Usuarios.MercadoPagoClientSecret;
                    if (!string.IsNullOrEmpty(API.ClientIDMercadoPago) && !string.IsNullOrEmpty(API.PasswordMercadoPago))
                    {
                        var factura = comprobante.Tipo + "-" + comprobante.PuntosDeVenta.Punto.ToString("#0000") + "-" + comprobante.Numero.ToString("#00000000");
                        var mpRef = API.AddPreference(comprobante.IDComprobante.ToString(), "Pago de la Factura: " + factura, string.Empty, 1, comprobante.Saldo, API.Mondeda.ARS, string.Empty);
                        btnMP = "<a class='btn btn-success' id='lnkComprar' href='" + mpRef + "&payer.name=" + comprobante.Usuarios.RazonSocial + "&payer.surname=" + comprobante.Usuarios.RazonSocial + "&payer.email=" + comprobante.Usuarios.Email + "' name='MP-Checkout' mp-mode='modal' onreturn='pagoCompleto'>Pagar</a>";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new HtmlString(btnMP);
        }

        public ActionResult Index()
        {
            var model = new FacturasViewModel();
            var token = User.Token;
            using (var dbContext = new ACHEEntities())
            {
                var pwd = dbContext.PersonasPwd.Where(x => x.IDPersonaPwd == User.IDPersonaPwd).FirstOrDefault();
                if (pwd != null)
                {
                    if (!pwd.CambioPwd)
                        return RedirectToAction("PrimerLogin", "Account");
                }

                var urlApi = ConfigurationManager.AppSettings["Api.Path"];

                var listaAux = dbContext.Comprobantes.Include("Personas").Include("PuntosDeVenta")
                    .Where(x => x.NroDocumento == User.Documento && x.TipoDocumento == User.TipoDocumento && x.Usuarios.PortalClientes)
                    .OrderByDescending(x => x.FechaComprobante).ToList()
                    .Select(x => new FacturaViewModel()
                    {
                        IDUsuario = x.IDUsuario,
                        IDFactura = x.IDComprobante,
                        Emisor = x.Usuarios.RazonSocial,
                        NroComprobante = x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000"),
                        TipoComprobante = x.Tipo,
                        CAE = x.CAE,
                        FechaFacturacion = x.FechaComprobante,
                        Importe = x.ImporteTotalBruto,
                        Iva = x.ImporteTotalBruto - x.ImporteTotalNeto,
                        Total = x.ImporteTotalNeto,
                        Saldo = x.Saldo,
                        Modo = x.Modo,
                        Descargar = urlApi + "/request/getFactura/" + token + "/" + x.IDComprobante + "/" + x.IDPersona,
                        BtnPago = integracionMercadoPago(x)
                    }).ToList();
                model.listaFacturas = ValidarPlan(dbContext, listaAux);
            }
            return View(model);
        }

        private List<FacturaViewModel> ValidarPlan(ACHEEntities dbContext, List<FacturaViewModel> listAux)
        {
            List<int> listUsuarios = new List<int>();
            foreach (var fc in listAux)
            {
                var UsuAdicional = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == fc.IDUsuario).FirstOrDefault();
                var Empresa = dbContext.Usuarios.Where(x => x.IDUsuarioPadre != null && x.IDUsuario == fc.IDUsuario).FirstOrDefault();
                PlanesPagos planesPagos = new PlanesPagos();

                if (UsuAdicional != null)
                    planesPagos = dbContext.PlanesPagos.Where(x => x.IDUsuario == UsuAdicional.IDUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();
                else if (Empresa != null)
                    planesPagos = dbContext.PlanesPagos.Where(x => x.IDUsuario == Empresa.IDUsuarioPadre && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();
                else
                    planesPagos = dbContext.PlanesPagos.Where(x => x.IDUsuario == fc.IDUsuario && x.FechaInicioPlan <= DateTime.Now && x.FechaFinPlan >= DateTime.Now).FirstOrDefault();

                if (planesPagos != null && planesPagos.FechaFinPlan >= DateTime.Now.Date && planesPagos.IDPlan <= 3)
                    listUsuarios.Add(fc.IDUsuario);
            }

            foreach (var item in listUsuarios)
                listAux.RemoveAll(x => x.IDUsuario == item);

            return listAux;
        }
    }
}