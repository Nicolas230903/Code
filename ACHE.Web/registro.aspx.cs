using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections.Specialized;
using ACHE.Negocio.Contabilidad;

public partial class registro : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        var nombre = Request.QueryString["nombre"];
        var email = Request.QueryString["email"];
        var empresa = Request.QueryString["empresa"];
        var cuit = Request.QueryString["cuit"];

        if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(empresa) && !string.IsNullOrEmpty(cuit))
        {
            //Response.Redirect("http://www.google.com.ar");
            HttpContext.Current.Response.Status = "301 Moved Permanently";
            HttpContext.Current.Response.AddHeader("Location", "http://www.google.com.ar");
        }
    }

    [WebMethod(true)]
    public static void guardar(string cuit, string email, string pwd, string telefono, string codigoPromocion)
    {
        if (!Common.validarPassword(pwd))
            throw new Exception("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");

        using (var dbContext = new ACHEEntities())
        {
            if (dbContext.Usuarios.Any(x => x.Email == email))
            {
                if (!email.Equals("serbal83@gmail.com") && !email.Equals("juannferro@gmail.com"))
                    throw new Exception("El E-mail ingresado ya se encuentra registrado.");
            }                
            else if (dbContext.Usuarios.Any(x => x.CUIT == cuit))
                throw new Exception("El CUIT ingresado ya se encuentra registrado.");
            if (!cuit.IsValidCUIT())
                throw new Exception("El CUIT es inválido.");
            else
            {
                Usuarios entity = new Usuarios();

                entity.RazonSocial = ""; //razonSocial;
                entity.FechaInicioActividades = null;
                entity.CondicionIva = ""; //condicionIva;
                entity.CUIT = cuit;
                entity.CUITAfip = "23185938999"; //Luego hay que definir cuando lo genere un contador
                entity.IIBB = string.Empty;
                entity.Personeria = ""; //personeria;
                entity.Email = email;
                entity.EmailAlertas = email;
                entity.Pwd = Common.MD5Hash(pwd);
                entity.Telefono = telefono;
                entity.Celular = string.Empty;
                entity.Contacto = string.Empty;

                //Domicilio
                entity.Pais = "Argentina";
                entity.IDProvincia = 0;//Ciudad de Buenos Aires
                entity.IDCiudad = 24071;//SIN IDENTIFICAR
                entity.Domicilio = string.Empty;
                entity.PisoDepto = string.Empty;
                entity.CodigoPostal = string.Empty;

                entity.Theme = "default";
                entity.TemplateFc = "default";
                entity.FechaAlta = DateTime.Now;
                entity.FechaUltLogin = DateTime.Now;
                entity.TieneFacturaElectronica = false;
                entity.Logo = null;
                entity.Activo = true;
                entity.IDPlan = 1; //Plan Basico
                entity.FechaFinPlan = DateTime.Now.AddDays(30);
                entity.UsaFechaFinPlan = true;
                entity.SetupRealizado = false;
                entity.UsaProd = ConfigurationManager.AppSettings["PROD"] == "1" ? true : false;

                entity.IDUsuarioPadre = null;
                entity.CodigoPromo = codigoPromocion;
                entity.ApiKey = Guid.NewGuid().ToString().Replace("-", "");

                entity.EsAgentePercepcionIVA = false;
                entity.EsAgentePercepcionIIBB = false;
                entity.IDJurisdiccion = entity.IDProvincia.ToString();
                entity.EsAgenteRetencionGanancia = false;
                entity.EsAgenteRetencion = false;
                entity.CantidadEmpresas = 4;
                entity.ExentoIIBB = false;
                entity.UsaPrecioFinalConIVA = true;
                entity.EnvioAutomaticoComprobante = false;
                entity.EnvioAutomaticoRecibo = false;
                entity.EsContador = false;
                entity.UsaPlanCorporativo = false;
                try
                {
                    PlanesPagos p = new PlanesPagos();
                    p.IDUsuario = entity.IDUsuario;
                    p.IDPlan = 1; //Plan Basico
                    p.ImportePagado = 0;
                    p.PagoAnual = false;
                    p.FormaDePago = "-";
                    p.NroReferencia = "";
                    p.Estado = "Aceptado";
                    p.FechaDeAlta = DateTime.Now.Date;
                    p.FechaDePago = DateTime.Now.Date;
                    p.FechaInicioPlan = p.FechaDePago;
                    p.FechaFinPlan = Convert.ToDateTime(p.FechaInicioPlan).AddDays(30).Date;

                    dbContext.PlanesPagos.Add(p);

                    //PuntosDeVenta punto = new PuntosDeVenta();
                    //punto.FechaAlta = DateTime.Now;
                    //punto.Punto = 1;
                    //punto.PorDefecto = true;
                    //entity.PuntosDeVenta.Add(punto);

                    //if (cuit.StartsWith("30"))
                    //{
                    //    punto = new PuntosDeVenta();
                    //    punto.FechaAlta = DateTime.Now;
                    //    punto.Punto = 2;
                    //    punto.PorDefecto = false;
                    //    entity.PuntosDeVenta.Add(punto);
                    //}

                    Actividad actividad = new Actividad();
                    actividad.FechaAlta = DateTime.Now;
                    actividad.Codigo = "000000";
                    actividad.PorDefecto = true;
                    entity.Actividad.Add(actividad);

                    Categorias cat = new Categorias();
                    cat.Nombre = "General";
                    entity.Categorias.Add(cat);

                    Categorias cat2 = new Categorias();
                    cat2.Nombre = "Honorarios";
                    entity.Categorias.Add(cat2);

                    Categorias cat3 = new Categorias();
                    cat3.Nombre = "Gastos varios";
                    entity.Categorias.Add(cat3);

                    Categorias cat4 = new Categorias();
                    cat4.Nombre = "Nafta";
                    entity.Categorias.Add(cat4);

                    Categorias cat5 = new Categorias();
                    cat5.Nombre = "Equipamiento";
                    entity.Categorias.Add(cat5);

                    Bancos banco = new Bancos();
                    banco.Moneda = "Pesos Argentinos";
                    banco.SaldoInicial = 0;
                    banco.NroCuenta = "";
                    banco.IDBancoBase = dbContext.BancosBase.Where(x => x.Nombre == "Default").FirstOrDefault().IDBancoBase;
                    banco.FechaAlta = DateTime.Now;
                    banco.Activo = true;
                    entity.Bancos.Add(banco);

                    dbContext.Usuarios.Add(entity);

                    /*Personas persona = new Personas();
                    persona.CondicionIva = "CF";
                    persona.Tipo = "C";
                    persona.IDUsuario = entity.IDUsuario;
                    persona.RazonSocial = "Cliente final";*/

                    //ListDictionary replacements = new ListDictionary();
                    //bool send = EmailHelper.SendMessage(EmailTemplate.Bienvenido, replacements, entity.Email, "axanweb: Bienvenido");

                    dbContext.SaveChanges();

                    PermisosModulos.ObtenerTodosLosFormularios(dbContext, entity.IDUsuario);

                    //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(dbContext, entity.IDUsuario, entity.CondicionIva);

                    HttpContext.Current.Session["CurrentUser"] = new WebUser(entity.IDUsuario, entity.IDUsuario, "A", entity.RazonSocial, entity.CUIT, entity.CondicionIva,
                        entity.Email, "", entity.Domicilio + " " + entity.PisoDepto, entity.Pais, entity.IDProvincia, entity.IDCiudad, entity.Telefono, entity.Celular,
                        entity.TieneFacturaElectronica, entity.IIBB, entity.FechaInicioActividades, "", entity.TemplateFc, entity.IDUsuarioPadre, entity.SetupRealizado, false,
                        !entity.UsaProd, entity.IDPlan, entity.EmailAlertas, "", "", entity.EsAgentePercepcionIVA, entity.EsAgentePercepcionIIBB,
                        entity.EsAgenteRetencionGanancia, entity.EsAgenteRetencion, true, entity.UsaFechaFinPlan, entity.ApiKey, entity.ExentoIIBB, entity.UsaPrecioFinalConIVA,
                        entity.FechaAlta, entity.EnvioAutomaticoComprobante, entity.EnvioAutomaticoRecibo, entity.IDJurisdiccion, entity.UsaPlanCorporativo, 
                        entity.PedidoDeVenta, entity.TiendaNubeIdTienda, entity.TiendaNubeToken, entity.CUITAfip, 
                        entity.PorcentajeCompra, entity.PorcentajeRentabilidad, entity.ParaPDVSolicitarCompletarContacto, entity.EsVendedor, entity.PorcentajeComision,
                        entity.FacturaSoloContraEntrega, entity.UsaCantidadConDecimales);

                }
                catch (Exception e)
                {
                    var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                    BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
                    throw e;
                }
            }
        }
    }
}