using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACHE.Model;
using ACHE.Model.ViewModels;
namespace ACHE.Admin.Models
{
    public class UsuarioViewModel
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Personeria { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string CUIT { get; set; }
        public string IIBB { get; set; }
        public DateTime FechaInicioActividades { get; set; }
        public string CondicionIva { get; set; }
        public string Domicilio { get; set; }
        public string PisoDepto { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string EmailAlertas { get; set; }
        public string Logo { get; set; }
        public string Theme { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public string FechaUltLogin { get; set; }
        public string Contacto { get; set; }
        public bool TieneFacturaElectronica { get; set; }
        public string TemplateFc { get; set; }
        public string Pais { get; set; }
        public bool CorreoPortal { get; set; }
        public bool PortalClientes { get; set; }
        public string SetupRealizado { get; set; }
        public bool UsaProd { get; set; }
        public int IDUsuarioPadre { get; set; }
        public string ApiKey { get; set; }
        public string CodigoPromo { get; set; }
        public int IDPlan { get; set; }
        public bool EsAgentePercepcionIVA { get; set; }
        public bool EsAgentePercepcionIIBB { get; set; }
        public bool EsAgenteRetencion { get; set; }
        public int IDProvincia { get; set; }
        public int IDCiudad { get; set; }
        public string MercadoPagoClientID { get; set; }
        public string MercadoPagoClientSecret { get; set; }
        public DateTime FechaFinPlan { get; set; }
        public bool UsaFechaFinPlan { get; set; }
        public List<PuntosDeVenta> listaPuntos { get; set; }
        public List<PlanesPagos> listaPlanesPagos { get; set; }
        public string Observaciones { get; set; }
        public string PlanActual { get; set; }
        public string FechaFinPlanActual { get; set; }
        public string PlanEstado { get; set; }
        public List<EmpresasViewModel> ListaEmpresas { get; set; }
        public List<UsuariosAdViewModel> ListaUsuariosAd { get; set; }
        public string ProvinciaNombre { get; set; }
        public string CiudadNombre { get; set; }
        public int CantFacturas { get; set; }
        public int CantEmpresasHabilitadas { get; set; }
        public bool? ExentoIIBB { get; set; }
        public int AntiguedadMeses { get; set; }
        public EstadisticasViewModel Estadisticas { get; set; }
        public bool UsaPrecioFinalConIVA { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string MotivoBaja { get; set; }
        public bool EstaBloqueado { get; set; }
        public bool TienePlanDeCuentas { get; set; }

        public string FechaAltaDesc { get; set; }
        public string Baja { get; set; }
        public bool EsContador { get; set; }
        public bool UsaPlanCorporativo { get; set; }
        public string CUITAfip { get; set; }
        public string CertificadoPfxActual { get; set; }

        public bool EstaBloqueadoAd { get; set; }
        public List<LoginUsuarioViewModel> listaLoginUsuarios { get; set; }
        public List<LoginUsuarioViewModel> listaLoginUsuariosAdicionales { get; set; }

        public AccesoFormularioUsuario acceso { get; set; }

        public bool accesoGestor { get; set; }

        public bool EsVendedor { get; set; }
        public decimal PorcentajeComision { get; set; }
    }

    public class EstadisticasViewModel
    {
        public int CantClientes { get; set; }
        public int CantProveedores { get; set; }
        public int CantVentasTotal { get; set; }
        public int CantVentasMes { get; set; }
        public int CantAbonosTotal { get; set; }
        public int CantAbonosMes { get; set; }
        public int CantProductosTotal { get; set; }
        public int CantProductosMes { get; set; }
        public int CantComprasTotal { get; set; }
        public int CantComprasMes { get; set; }
        public int CantPagosTotal { get; set; }
        public int CantPagosMes { get; set; }
        public int CantCobranzasTotal { get; set; }
        public int CantCobranzasMes { get; set; }
        public int CantPresupuestosMes { get; set; }
        public int CantMovcajaTotal { get; set; }
        public int CantChequesTotal { get; set; }
        public int CantPresupuestosTotal { get; set; }
        public int CantChequesMes { get; set; }
        public int CantMovcajaMes { get; set; }
    }
    public class UsuariosAdViewModel
    {
        public string Correo { get; set; }
        public string Activo { get; set; }
        public string NivelSeguridad { get; set; }
        public string Empresas { get; set; }
    }

    public class ResultadosUsuarioViewModel
    {
        public IList<UsuarioViewModel> Items { get; set; }
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}