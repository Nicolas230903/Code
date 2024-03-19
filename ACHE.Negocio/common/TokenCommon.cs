using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Model;
using ACHE.Negocio.Contabilidad;

namespace ACHE.Negocio.Common
{
    public class TokenCommon
    {
        public static int validarToken(string token)
        {
            int id = 0;

            using (var dbContext = new ACHEEntities())
            {
                var auth = dbContext.AuthenticationToken.Where(x => x.Token == token).FirstOrDefault();
                if (auth != null)
                {
                    var fecha = DateTime.Now;
                    if (auth.FechaExpiracion > fecha)
                        id = auth.IDUsuario;
                }
            }

            return id;
        }

        public static WebUser ObtenerWebUser(int idusuario)
        {
            using (var dbContext = new ACHEEntities())
            {
                var usu = dbContext.UsuariosView.Where(x => x.IDUsuario == idusuario).FirstOrDefault();

                //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(dbContext, usu.IDUsuario, usu.CondicionIva);


                if (usu != null)
                {
                    bool tieneMultiempresa = (usu.IDUsuarioAdicional != 0 && usu.IDUsuarioPadre == null) ? dbContext.UsuariosEmpresa.Any(x => x.IDUsuarioAdicional == usu.IDUsuarioAdicional) : true;

                    return new WebUser(usu.IDUsuario, usu.IDUsuarioAdicional, usu.Tipo, usu.RazonSocial, usu.CUIT, usu.CondicionIva,
                                   usu.Email, "", usu.Domicilio + " " + usu.PisoDepto, usu.Pais, usu.IDProvincia,
                                   usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFacturaElectronica, usu.IIBB, usu.FechaInicioActividades,
                                   usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupRealizado, tieneMultiempresa, !usu.UsaProd,
                                   4, usu.EmailAlertas, usu.Provincia, usu.Ciudad, usu.EsAgentePercepcionIVA, usu.EsAgentePercepcionIIBB,
                                   usu.EsAgenteRetencionGanancia, usu.EsAgenteRetencion, true/*planVigente*/, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA,
                                   usu.FechaAlta, usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo, 
                                   usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, usu.PorcentajeCompra,
                                   usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
                                   usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);

                }
                else
                {
                    throw new Exception("El usuario se encuentra deshabilitado.");
                }               

            }
        }
    }
}