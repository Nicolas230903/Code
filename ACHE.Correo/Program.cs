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
namespace ACHE.EnviosMail
{
    class Program
    {
        private static List<TotalesNotificacionesCorreoViewModel> TOTALNOTIFICACIONES { get; set; }
        static void Main(string[] args)
        {
            var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
            //var ejecutarServicio = ConfigurationManager.AppSettings["EjecutarServicios"];

            BasicLog.AppendToFile(path, "****** Inicia proceso de envio de email ******", "");
            TOTALNOTIFICACIONES = new List<TotalesNotificacionesCorreoViewModel>();

            BasicLog.AppendToFile(path, "Inicia función ObtenerUsuariosPrueba", "");
            ObtenerUsuariosPrueba();
            BasicLog.AppendToFile(path, "Termina función ObtenerUsuariosPrueba", "");

            BasicLog.AppendToFile(path, "Inicia función ObtenerUsuariosPagos", "");
            ObtenerUsuariosPagos();
            BasicLog.AppendToFile(path, "Termina función ObtenerUsuariosPagos", "");

            BasicLog.AppendToFile(path, "Inicia función ObtenerUsuariosCheques", "");
            ObtenerUsuariosCheques();
            BasicLog.AppendToFile(path, "Termina función ObtenerUsuariosCheques", "");

            BasicLog.AppendToFile(path, "Inicia función ObtenerUsuariosAvisosVencimientos", "");
            if (ConfigurationManager.AppSettings["Ejecutar.AvisosVencimientos"] == "1")
                ObtenerUsuariosAvisosVencimientos();
            BasicLog.AppendToFile(path, "Termina función ObtenerUsuariosAvisosVencimientos", "");

            BasicLog.AppendToFile(path, "Inicia función EnviarEmailAdministrador", "");
            EnviarEmailAdministrador();
            BasicLog.AppendToFile(path, "Termina función EnviarEmailAdministrador", "");

            BasicLog.AppendToFile(path, "****** Termina proceso de envio de email ******", "");
        }

        #region Envio de corres usuarios registrados
        private static void ObtenerUsuariosPrueba()
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listUsuarios = dbContext.UsuariosPlanesView.Where(x => x.Activo && x.IDPlan == 6 && x.AntiguedadMeses <= 1).OrderByDescending(x => x.FechaAlta).ToList();
                    var subject = string.Empty;

                    foreach (var usu in listUsuarios)
                    {
                        var fechaActual = DateTime.Now.Date;
                        var dias = (fechaActual - usu.FechaAlta.Date).Days;

                        var totalesNotificaciones = new TotalesNotificacionesCorreoViewModel();
                        var emailUsuario = (string.IsNullOrWhiteSpace(usu.EmailAlertas)) ? usu.Email : usu.EmailAlertas;
                        totalesNotificaciones.Usuarios = "IDUSUARIO : " + usu.IDUsuario + ", EMAIL: " + emailUsuario + ", RAZON SOCIAL: " + usu.RazonSocial;

                        if (dias == 2 && ConfigurationManager.AppSettings["Ejecutar.DosDias"] == "1")
                        {
                            subject = ConfigurationManager.AppSettings["subject.NotificacionDosDias"];
                            var send = EnvioCorreos(dbContext, usu, EmailTemplateApp.NotificacionDosDias, subject, "NotificacionDosDias");
                            totalesNotificaciones.TotalCorreos++;
                            if (send)
                                totalesNotificaciones.TotalCorreosEnviados++;
                            totalesNotificaciones.TipoNotificacion = "NotificacionDosDias";
                            TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                        }
                        else if (dias == 14 && ConfigurationManager.AppSettings["Ejecutar.DosSemanas"] == "1")
                        {
                            subject = ConfigurationManager.AppSettings["subject.NotificacionDosSemanas"];
                            var send = EnvioCorreos(dbContext, usu, EmailTemplateApp.NotificacionDosSemanas, subject, "NotificacionDosSemanas");
                            totalesNotificaciones.TotalCorreos++;
                            if (send)
                                totalesNotificaciones.TotalCorreosEnviados++;
                            totalesNotificaciones.TipoNotificacion = "NotificacionDosSemanas";
                            TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                        }
                        else if (dias == 25 && ConfigurationManager.AppSettings["Ejecutar.VeiticincoDias"] == "1")
                        {
                            subject = ConfigurationManager.AppSettings["subject.NotificacionVeiticincoDias"];
                            var send = EnvioCorreos(dbContext, usu, EmailTemplateApp.NotificacionVeiticincoDias, subject, "NotificacionVeiticincoDias");
                            totalesNotificaciones.TotalCorreos++;
                            if (send)
                                totalesNotificaciones.TotalCorreosEnviados++;
                            totalesNotificaciones.TipoNotificacion = "NotificacionVeiticincoDias";
                            TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }

        private static void ObtenerUsuariosPagos()
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listUsuarios = dbContext.UsuariosPlanesView.Where(x => x.Activo && x.IDPlan != 6 && x.IDPlan != 1).OrderByDescending(x => x.FechaFinPlan).ToList();
                    var subject = string.Empty;
                    foreach (var usu in listUsuarios)
                    {
                        var fechaActual = DateTime.Now.Date;
                        var dias = (Convert.ToDateTime(usu.FechaFinPlan).Date - fechaActual).Days;

                        var totalesNotificaciones = new TotalesNotificacionesCorreoViewModel();
                        var emailUsuario = (string.IsNullOrWhiteSpace(usu.EmailAlertas)) ? usu.Email : usu.EmailAlertas;
                        totalesNotificaciones.Usuarios = "IDUSUARIO : " + usu.IDUsuario + ", EMAIL: " + emailUsuario + ", RAZON SOCIAL: " + usu.RazonSocial;

                        if (dias == 5 && ConfigurationManager.AppSettings["Ejecutar.CincoDiasVencimiento"] == "1")
                        {
                            subject = ConfigurationManager.AppSettings["subject.NotificacionCincoDiasVencimiento"];
                            var send = EnvioCorreos(dbContext, usu, EmailTemplateApp.NotificacionCincoDiasVencimiento, subject, "NotificacionCincoDiasVencimiento");

                            totalesNotificaciones.TotalCorreos++;
                            if (send)
                                totalesNotificaciones.TotalCorreosEnviados++;
                            totalesNotificaciones.TipoNotificacion = "NotificacionCincoDiasVencimiento";
                            TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }

        private static bool EnvioCorreos(ACHEEntities dbContext, UsuariosPlanesView usu, EmailTemplateApp template, string subject, string tipoNotificacion)
        {
            var usuario = (usu.RazonSocial != "") ? usu.RazonSocial : "usuario";
            bool send = false;
            try
            {
                var emailUsuario = (string.IsNullOrWhiteSpace(usu.EmailAlertas)) ? usu.Email : usu.EmailAlertas;
                ListDictionary replacements = new ListDictionary();
                replacements.Add("<USUARIO>", usuario);
                send = EmailHelperApp.SendMessage(template, replacements, emailUsuario, subject);

                if (send)
                {
                    var mensaje = "Envios de correo finalizados correctamente";
                    GuardarObservacionesUsuario(dbContext, mensaje, false, "", usu.IDUsuario, tipoNotificacion);
                    return true;
                }
                else
                    throw new Exception("El correo no fue enviado");
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                GuardarObservacionesUsuario(dbContext, "Error usu: " + usuario, true, msg, usu.IDUsuario, tipoNotificacion);

                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                BasicLog.AppendToFile(path, msg, ex.ToString());
            }

            return false;
        }
        #endregion

        #region Envio de correo recordatorios de cheques

        private static void ObtenerUsuariosCheques()
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listCheques = dbContext.RptChequesAcciones.Where(x => x.Accion == "" && x.EsPropio == false && x.IDPlan > 3).GroupBy(x => new { x.IDUsuario, x.FechaCobro }).ToList();
                    foreach (var item in listCheques)
                    {
                        var fechaActual = DateTime.Now.Date;
                        var dias = (Convert.ToDateTime(item.FirstOrDefault().FechaCobro).Date - fechaActual).Days;
                        var descripcion = string.Empty;

                        foreach (var itemdetalle in item)
                            descripcion += "Emisor:" + itemdetalle.Emisor + " - Cheque nro: " + itemdetalle.Numero + "</br> ";
                        
                        var razonSocial = (item.FirstOrDefault().RazonSocial != "") ? item.FirstOrDefault().RazonSocial : "usuario";
                        var emailUsuario = (string.IsNullOrWhiteSpace(item.FirstOrDefault().EmailAlertas)) ? item.FirstOrDefault().Email : item.FirstOrDefault().EmailAlertas;
                        
                        var totalesNotificaciones = new TotalesNotificacionesCorreoViewModel();
                        totalesNotificaciones.Usuarios = "IDUSUARIO : " + item.FirstOrDefault().IDUsuario + ", EMAIL: " + emailUsuario + ", RAZON SOCIAL: " + item.FirstOrDefault().RazonSocial + ", Datos del o los cheques: " + descripcion;
                        
                        if (dias == 1 && ConfigurationManager.AppSettings["Ejecutar.ChequesACobrar"] == "1")
                        {
                            var send = EnvioCorreosCheques(dbContext, emailUsuario, item.FirstOrDefault().IDUsuario, razonSocial, EmailTemplateApp.Alertas, "Mañana tendra disponibles los siguientes cheques para su depósito: ", descripcion, "NotificacionChequesACobrar");

                            totalesNotificaciones.TotalCorreos++;
                            if (send)
                                totalesNotificaciones.TotalCorreosEnviados++;
                            totalesNotificaciones.TipoNotificacion = "NotificacionChequesACobrar";
                            TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                        }
                        else if (dias == -10 && ConfigurationManager.AppSettings["Ejecutar.ChequesAVencer"] == "1")
                        {
                            var send = EnvioCorreosCheques(dbContext, emailUsuario, item.FirstOrDefault().IDUsuario, razonSocial, EmailTemplateApp.Alertas, "Los siguientes cheques aún no se han depositado: ", descripcion, "NotificacionChequesAVencer");

                            totalesNotificaciones.TotalCorreos++;
                            if (send)
                                totalesNotificaciones.TotalCorreosEnviados++;
                            totalesNotificaciones.TipoNotificacion = "NotificacionChequesAVencer";
                            TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }
        private static bool EnvioCorreosCheques(ACHEEntities dbContext, string email, int idUsuario,string razonSocial, EmailTemplateApp template, string subject, string descripcion, string tipoNotificacion)
        {
            bool send = false;
            try
            {
                ListDictionary replacements = new ListDictionary();
                replacements.Add("<USUARIO>", razonSocial);
                replacements.Add("<ALERTA>", subject);
                replacements.Add("<DESCRIPCION>", descripcion);
                send = EmailHelperApp.SendMessage(template, replacements, email, subject);

                if (send)
                {
                    var mensaje = "Envio de la notificación de cheques finalizado correctamente";
                    GuardarObservacionesUsuario(dbContext, mensaje, false, "", idUsuario, tipoNotificacion);
                    return true;
                }
                else
                    throw new Exception("El correo no fue enviado");
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                GuardarObservacionesUsuario(dbContext, "Error usu: " + razonSocial, true, msg, idUsuario, tipoNotificacion);
            }

            return false;
        }
        #endregion

        #region Envio de correo a los clientes segun AvisosVencimientos

        private static void ObtenerUsuariosAvisosVencimientos()
        {
            using (var dbContext = new ACHEEntities())
            {
                var listaAvisos = dbContext.AvisosVencimientosView.Where(x => x.IDPlan >= 4 && x.Saldo > 0 && x.Activa == true && x.EmailPersona != "").ToList();

                foreach (var item in listaAvisos)
                {
                    var dia = (item.FechaVencimiento - DateTime.Now.Date).Days;
                    //Modo envio=1 es antes. O= despues.
                    if (item.ModoDeEnvio == "1" && dia == item.CantDias && item.Activa == true)
                    {
                        var totalesNotificaciones = new TotalesNotificacionesCorreoViewModel();
                        totalesNotificaciones.Usuarios = "IDUSUARIO : " + item.IDUsuario + ", EMAIL: " + item.EmailUsuario + ", RAZON SOCIAL: " + item.RazonSocialUsuario + ", TIPO de ALERTA: AvisosVencimiento: " + item.TipoAlerta;
                        totalesNotificaciones.TotalCorreos++;

                        var mensaje = item.Mensaje.ReplaceAll("\n", "<br/>");
                        var send = EnvioCorreosAvisosVencimientos(dbContext, item, EmailTemplateApp.AvisosVencimiento, item.Asunto, mensaje, "AvisosVencimiento: " + item.TipoAlerta);

                        if (send)
                            totalesNotificaciones.TotalCorreosEnviados++;
                        totalesNotificaciones.TipoNotificacion = "AvisosVencimiento" + item.TipoAlerta;
                        TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                    }
                    else if (item.ModoDeEnvio == "0" && dia == -item.CantDias && item.Activa == true)
                    {
                        var totalesNotificaciones = new TotalesNotificacionesCorreoViewModel();
                        totalesNotificaciones.Usuarios = "IDUSUARIO : " + item.IDUsuario + ", EMAIL: " + item.EmailUsuario + ", RAZON SOCIAL: " + item.RazonSocialUsuario + ", TIPO de ALERTA: AvisosnVecimiento: " + item.TipoAlerta;
                        totalesNotificaciones.TotalCorreos++;
                        var mensaje = item.Mensaje.ReplaceAll("\n", "<br/>");
                        var send = EnvioCorreosAvisosVencimientos(dbContext, item, EmailTemplateApp.AvisosVencimiento, item.Asunto, mensaje, "AvisosVencimiento: " + item.TipoAlerta);

                        if (send)
                            totalesNotificaciones.TotalCorreosEnviados++;
                        totalesNotificaciones.TipoNotificacion = "AvisosVencimiento" + item.TipoAlerta;
                        TOTALNOTIFICACIONES.Add(totalesNotificaciones);
                    }
                }
            }
        }

        private static bool EnvioCorreosAvisosVencimientos(ACHEEntities dbContext, AvisosVencimientosView avisos, EmailTemplateApp template, string subject, string descripcion, string tipoNotificacion)
        {
            try
            {
                ListDictionary replacements = new ListDictionary();
                replacements.Add("<CLIENTE>", avisos.RazonSocialPersona);
                replacements.Add("<COMPROBANTE>", avisos.Tipo + "-" + avisos.Punto.ToString("#0000") + "-" + avisos.Numero.ToString("#00000000"));
                replacements.Add("<DESCRIPCION>", descripcion);

                var send = EmailHelperApp.SendMessage(template, replacements, avisos.EmailPersona, subject);

                if (send)
                    return true;
                else
                    throw new Exception("El correo no fue enviado");
            }
            catch (Exception ex)
            {
                var msg = tipoNotificacion;
                msg += ex.InnerException != null ? " - " + ex.InnerException.Message : " - " + ex.Message;

                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                BasicLog.AppendToFile(path, msg, ex.ToString());
                return false;
            }
        }

        #endregion

        private static void EnviarEmailAdministrador()
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    ListDictionary replacements = new ListDictionary();

                    var listaFinal = TOTALNOTIFICACIONES.GroupBy(x => x.TipoNotificacion).Select(x => new TotalesNotificacionesCorreoViewModel()
                    {
                        TipoNotificacion = x.FirstOrDefault().TipoNotificacion,
                        TotalCorreos = x.Sum(y => y.TotalCorreos),
                        TotalCorreosEnviados = x.Sum(y => y.TotalCorreosEnviados),
                        Usuarios = String.Join(" <br/> ", x.Select(n => n.Usuarios))
                    }).ToList();

                    foreach (var item in listaFinal)
                    {
                        switch (item.TipoNotificacion)
                        {
                            case "NotificacionDosDias":
                                replacements.Add("<TOTALCORREOSDOSDIAS>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSDOSDIAS>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSDOSDIAS>", item.Usuarios);
                                break;
                            case "NotificacionDosSemanas":
                                replacements.Add("<TOTALCORREOSDOSSEMANAS>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSDOSSEMANAS>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSDOSDOSSEMANAS>", item.Usuarios);
                                break;
                            case "NotificacionVeiticincoDias":
                                replacements.Add("<TOTALCORREOSVEITICINCODIAS>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSVEITICINCODIAS>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSVEITICINCODIAS>", item.Usuarios);
                                break;
                            case "NotificacionCincoDiasVencimiento":
                                replacements.Add("<TOTALCORREOSCINCODIASVENCIMIENTOS>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSCINCODIASVENCIMIENTOS>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSCINCODIASVENCIMIENTOS>", item.Usuarios);
                                break;
                            case "NotificacionChequesACobrar":
                                replacements.Add("<TOTALCORREOSCHEQUESACOBRAR>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSCHEQUESACOBRAR>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSCHEQUESACOBRAR>", item.Usuarios);
                                break;
                            case "NotificacionChequesAVencer":
                                replacements.Add("<TOTALCORREOSCHEQUESAVENCER>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSCHEQUESAVENCER>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSCHEQUESAVENCER>", item.Usuarios);
                                break;
                            case "AvisosVencimientoPrimer aviso":
                                replacements.Add("<TOTALCORREOSPRIMERAVISO>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSPRIMERAVISO>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSPRIMERAVISO>", item.Usuarios);
                                break;

                            case "AvisosVencimientoSegundo aviso":
                                replacements.Add("<TOTALCORREOSSEGUNDOAVISO>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSSEGUNDOAVISO>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSSEGUNDOAVISO>", item.Usuarios);
                                break;

                            case "AvisosVencimientoTercer aviso":
                                replacements.Add("<TOTALCORREOSTERCERAVISO>", item.TotalCorreos.ToString());
                                replacements.Add("<TOTALCORREOSENVIADOSTERCERAVISO>", item.TotalCorreosEnviados.ToString());
                                replacements.Add("<DATOSUSUARIOSTERCERAVISO>", item.Usuarios);
                                break;
                        }
                    }
                    var correoAdmin = ConfigurationManager.AppSettings["Email.ReplyTo"];
                    EmailHelperApp.SendMessage(EmailTemplateApp.NotificacionAdministrador, replacements, correoAdmin, "Resultados de los envios de correo");
                }
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }

        private static void GuardarObservacionesUsuario(ACHEEntities dbContext, string mensaje, bool tieneError, string mensajeError, int idUSuario, string tipoNotificacion)
        {
            try
            {
                var ObsUsu = new ObservacionesUsuario();
                ObsUsu.Fecha = DateTime.Now;
                ObsUsu.Observacion = mensaje;
                ObsUsu.TieneError = tieneError;
                ObsUsu.MensajeError = mensajeError;
                ObsUsu.IDUsuario = idUSuario;
                ObsUsu.TipoNotificacion = tipoNotificacion;
                ObsUsu.Proyecto = "ACHE.EnviosMails";

                dbContext.ObservacionesUsuario.Add(ObsUsu);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                var path = ConfigurationManager.AppSettings["NotificacionesCorreoLogError"];
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(path, msg, e.ToString());
            }
        }
    }
}