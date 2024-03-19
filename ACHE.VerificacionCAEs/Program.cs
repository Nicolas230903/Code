using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Negocio;
using ACHE.Model;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Configuration;
using ACHE.Model.ViewModels;
using ACHE.Extensions;

namespace ACHE.VerificacionCAEs
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
            //var ejecutarServicio = ConfigurationManager.AppSettings["EjecutarServicios"];

            BasicLog.AppendToFile(path, "Inicia función VerificarNumeroFacturas", "");
            VerificarNumeroFacturas();
            BasicLog.AppendToFile(path, "Termina función VerificarNumeroFacturas", "");
        }

        #region Verificar numeros de facturas
        
        private static void VerificarNumeroFacturas()
        {
            try
            {
                var numerosFaltantes = string.Empty;
                long contador = 0;
                var tipo = string.Empty;
                int idpunto = 0;
                using (var dbContext = new ACHEEntities())
                {
                    var listaComprobantes = dbContext.Comprobantes.Where(x => x.Modo == "E" && x.Numero > 0 && x.IDUsuario != 16).OrderBy(x => new { x.IDUsuario, x.Tipo, x.IDPuntoVenta, x.Numero }).ToList();
                    foreach (var item in listaComprobantes)
                    {
                        if (tipo == string.Empty)
                            tipo = item.Tipo;

                        if (idpunto == 0)
                            idpunto = item.IDPuntoVenta;
                        if (contador == 0)
                            contador = item.Numero;

                        if (contador == item.Numero && tipo == item.Tipo && idpunto == item.IDPuntoVenta)
                            contador++;
                        else
                        {
                            if (item.Numero < contador || tipo != item.Tipo || idpunto != item.IDPuntoVenta)
                            {
                                contador = item.Numero;
                                idpunto = item.IDPuntoVenta;
                                tipo = item.Tipo;
                                contador++;
                            }
                            else
                            {
                                numerosFaltantes += "IDUSUARIO :" + item.IDUsuario + ", usuario :" + item.Usuarios.RazonSocial + ", CUIT: " + item.Usuarios.CUIT + ", numero: " + item.Tipo + " " + item.PuntosDeVenta.Punto + " - " + contador + ", </br></br>";
                                contador = item.Numero;
                                contador++;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(numerosFaltantes))
                    EnviarEmailAdministrador(numerosFaltantes);
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }

        private static void EnviarEmailAdministrador(string mensaje)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    ListDictionary replacements = new ListDictionary();
                    replacements.Add("<USUARIO>", "Administrador");
                    replacements.Add("<ALERTA>", "Comprobantes electronicos que no tienen un CAE");
                    replacements.Add("<DESCRIPCION>", mensaje);
                    var correoAdmin = ConfigurationManager.AppSettings["Email.ReplyTo"];
                    EmailHelperApp.SendMessage(EmailTemplateApp.Alertas, replacements, correoAdmin, "Recuperación de CAE");
                }
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }
        
        #endregion
    }
}