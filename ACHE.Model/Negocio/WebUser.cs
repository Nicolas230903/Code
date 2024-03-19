using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    public class WebUser
    {
        public int IDUsuario { get; set; }
        public int IDUsuarioAdicional { get; set; }
        public string TipoUsuario { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string CondicionIVA { get; set; }
        public string Email { get; set; }
        public string Theme { get; set; }
        public string Domicilio { get; set; }
        public string Pais { get; set; }
        public int IDProvincia { get; set; }
        public int IDCiudad { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string IIBB { get; set; }
        public string Logo { get; set; }
        public string TemplateFc { get; set; }
        public DateTime? FechaInicio { get; set; }
        public bool TieneFE { get; set; }
        public bool SetupFinalizado { get; set; }
        public int? IDUsuarioPadre { get; set; }
        public bool TieneMultiEmpresa { get; set; }
        public bool ModoQA { get; set; }
        public int IDPlan { get; set; }
        public string EmailAlerta { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public bool? AgentePercepcionIVA { get; set; }
        public bool? AgentePercepcionIIBB { get; set; }
        public bool? AgenteRetencionGanancia { get; set; }
        public bool? AgenteRetencion { get; set; }
        public bool PlanVigente { get; set; }
        public bool UsaFechaFinPlan { get; set; }
        public string ApiKey { get; set; }
        public bool? ExentoIIBB { get; set; }
        public bool UsaPrecioFinalConIVA { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool EnvioAutomaticoComprobante { get; set; }
        public bool EnvioAutomaticoRecibo { get; set; }
        public string IDJurisdiccion { get; set; }
        public bool UsaPlanCorporativo { get; set; }
        public bool PedidoDeVenta { get; set; }
        public string TiendaNubeIdTienda { get; set; }
        public string TiendaNubeToken { get; set; }
        public string CUITAfip { get; set; }
        public decimal PorcentajeCompra { get; set; }
        public decimal PorcentajeRentabilidad { get; set; }
        public bool ParaPDVSolicitarCompletarContacto { get; set; }
        public bool EsVendedor { get; set; }
        public decimal PorcentajeComision { get; set; }
        public bool FacturaSoloContraEntrega { get; set; }
        public bool UsaCantidadConDecimales { get; set; }

        public WebUser(int idUsuario, int idUsuarioAdicional, string tipoUsuario, string razonSocial, string cuit, string condicionIVA,
            string email, string theme, string domicilio, string pais, int idProvincia, int idCiudad, string telefono, string celular, bool tieneFE, string iibb, DateTime? fechaInicio,
            string logo, string templateFc, int? idUsuarioPadre, bool setupFinalizado, bool tieneMultiEmpresa, bool modoQA, int idPlan, string emailAlerta, string Provincia,
            string Ciudad, bool? AgentePercepcionIVA, bool? AgentePercepcionIIBB, bool? AgenteRetencionGanancia, bool? AgenteRetencion, bool planVigente, bool usaFechaFinPlan, string apiKey,
            bool? exentoIIBB, bool usaPrecioFinalConIVA, DateTime fechaAlta, bool envioAutomaticoComprobante, bool envioAutomaticoRecibo, string idJurisdiccion, 
            bool UsaPlanCorporativo, bool PedidoDeVenta, string tiendaNubeIdTienda, string tiendaNubeToken, string CUITAfip, decimal PorcentajeCompra, decimal PorcentajeRentabilidad,
            bool ParaPDVSolicitarCompletarContacto, bool esVendedor, decimal porcentajeComision, bool facturaSoloContraEntrega, bool usaCantidadConDecimales)
        {
            this.IDUsuario = idUsuario;
            this.IDUsuarioAdicional = idUsuarioAdicional;
            this.TipoUsuario = tipoUsuario;
            this.RazonSocial = razonSocial;
            this.CUIT = cuit;
            this.CondicionIVA = condicionIVA;
            this.Email = email;
            this.Theme = theme;
            this.Domicilio = domicilio;
            this.Pais = pais;
            this.IDProvincia = idProvincia;
            this.IDCiudad = idCiudad;
            this.Telefono = telefono;
            this.Celular = celular;
            this.TieneFE = tieneFE;
            this.IIBB = iibb;
            this.FechaInicio = fechaInicio;
            this.Logo = logo;
            this.TemplateFc = templateFc;
            this.SetupFinalizado = setupFinalizado;
            this.IDUsuarioPadre = idUsuarioPadre;
            this.TieneMultiEmpresa = tieneMultiEmpresa;
            this.ModoQA = modoQA;
            this.IDPlan = idPlan;
            this.EmailAlerta = emailAlerta;
            this.Provincia = Provincia;
            this.Ciudad = Ciudad;
            this.AgentePercepcionIVA = AgentePercepcionIVA;
            this.AgentePercepcionIIBB = AgentePercepcionIIBB;
            this.AgenteRetencionGanancia = AgenteRetencionGanancia;
            this.AgenteRetencion = AgenteRetencion;
            this.PlanVigente = planVigente;
            this.UsaFechaFinPlan = usaFechaFinPlan;
            this.ApiKey = apiKey;
            this.ExentoIIBB = exentoIIBB;
            this.UsaPrecioFinalConIVA = usaPrecioFinalConIVA;
            this.FechaAlta = fechaAlta;
            this.EnvioAutomaticoComprobante = envioAutomaticoComprobante;
            this.EnvioAutomaticoRecibo = envioAutomaticoRecibo;
            this.IDJurisdiccion = idJurisdiccion;
            this.UsaPlanCorporativo = UsaPlanCorporativo;
            this.PedidoDeVenta = PedidoDeVenta;
            this.TiendaNubeIdTienda = tiendaNubeIdTienda;
            this.TiendaNubeToken = tiendaNubeToken;
            this.CUITAfip = CUITAfip;
            this.PorcentajeCompra = PorcentajeCompra;
            this.PorcentajeRentabilidad = PorcentajeRentabilidad;
            this.ParaPDVSolicitarCompletarContacto = ParaPDVSolicitarCompletarContacto;
            this.EsVendedor = esVendedor;
            this.PorcentajeComision = porcentajeComision;
            this.FacturaSoloContraEntrega = facturaSoloContraEntrega;
            this.UsaCantidadConDecimales = usaCantidadConDecimales;
        }
    }
}