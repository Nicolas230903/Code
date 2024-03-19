using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Configuration;


namespace ACHE.Extensions
{
    public enum EmailTemplate
    {
        RecuperoPwd,
        EnvioComprobante,
        Ayuda,
        Notificacion,
        Bienvenido,
        ModificacionPwd,
        Compras,
        Alertas,
        PagoPlanes,
        EnvioComprobanteConFoto
    }

    public static class EmailHelper
    {
        //public static readonly string HOST = ConfigurationManager.AppSettings["Email.Host"] ?? "No hay host definido";
        //public static readonly int PORT = int.Parse(ConfigurationManager.AppSettings["Email.Port"]);

        public static bool SendMessage(EmailTemplate template, ListDictionary replacements, string to, string subject)
        {
            string emailFrom = ConfigurationManager.AppSettings["Email.From"];
            string emailCC = ConfigurationManager.AppSettings["Email.CC"];
            MailMessage mailMessage = CreateMessage(template, replacements, to, emailFrom, emailCC, subject);

            return SendMailMessage(mailMessage);
        }

        /*public static bool SendMessage(EmailTemplate template, ListDictionary replacements, MailAddressCollection to, string subject)
        {
            string emailFrom = ConfigurationManager.AppSettings["Email.From"];
            string emailCC = ConfigurationManager.AppSettings["Email.CC"];
            MailMessage mailMessage = CreateMessage(template, replacements, to, emailFrom, emailCC, subject);

            return SendMailMessage(mailMessage);
        }*/

        public static bool SendMessage(EmailTemplate template, ListDictionary replacements, MailAddressCollection to, string from, string subject, List<string> attachments)
        {
            string emailFrom = from == string.Empty ? ConfigurationManager.AppSettings["Email.From"] : from;
            string emailCC = ConfigurationManager.AppSettings["Email.CCFc"];
            MailMessage mailMessage = CreateMessage(template, replacements, to, emailFrom, emailCC, subject, attachments);

            return SendMailMessage(mailMessage);
        }

        public static bool SendMessage(EmailTemplate template, ListDictionary replacements, MailAddressCollection to, string from, string cc, string subject, List<string> attachments)
        {
            string emailFrom = from == string.Empty ? ConfigurationManager.AppSettings["Email.From"] : from;
            //string emailCC = ConfigurationManager.AppSettings["Email.CCFc"];
            MailMessage mailMessage = CreateMessage(template, replacements, to, emailFrom, cc, subject, attachments);

            return SendMailMessage(mailMessage);
        }


        /*public static bool SendMessage(EmailTemplate template, ListDictionary replacements, MailAddressCollection to, string subject, List<string> attachments)
        {
            string emailFrom = ConfigurationManager.AppSettings["Email.From"];
            string emailCC = ConfigurationManager.AppSettings["Email.CC"];
            MailMessage mailMessage = CreateMessage(template, replacements, to, emailFrom, emailCC, subject, attachments);

            return SendMailMessage(mailMessage);
        }*/

        private static MailMessage CreateMessage(EmailTemplate template, ListDictionary replacements, MailAddressCollection to, string from, string cc, string subject)
        {
            string physicalPath = HttpContext.Current.Server.MapPath("~/App_Data/EmailTemplates/" + template.ToString() + ".txt");
            if (!File.Exists(physicalPath))
                throw new ArgumentException("Invalid EMail Template Passed into the NotificationManager", "template");

            string html = File.ReadAllText(physicalPath);
            foreach (DictionaryEntry param in replacements)
                html = html.Replace(param.Key.ToString(), param.Value.ToString());

            MailMessage mailMessage = new MailMessage();
            if (from != string.Empty)
                mailMessage.From = new MailAddress(from);
            mailMessage.Subject = subject;
            foreach (var mail in to)
                mailMessage.To.Add(mail);
            if (!string.IsNullOrEmpty(cc))
                mailMessage.CC.Add(new MailAddress(cc));

            string emailBCC = ConfigurationManager.AppSettings["Email.BCC"];
            if (!string.IsNullOrEmpty(emailBCC))
                mailMessage.Bcc.Add(new MailAddress(emailBCC));

            mailMessage.IsBodyHtml = true;
            mailMessage.Body = html;

            return mailMessage;
        }

        private static MailMessage CreateMessage(EmailTemplate template, ListDictionary replacements, MailAddressCollection to, string from, string cc, string subject, List<string> attachments)
        {
            string physicalPath = HttpContext.Current.Server.MapPath("~/App_Data/EmailTemplates/" + template.ToString() + ".txt");
            if (!File.Exists(physicalPath))
                throw new ArgumentException("Invalid EMail Template Passed into the NotificationManager", "template");

            string html = File.ReadAllText(physicalPath);
            foreach (DictionaryEntry param in replacements)
                html = html.Replace(param.Key.ToString(), param.Value.ToString());

            MailMessage mailMessage = new MailMessage();
            if (from != string.Empty)
                mailMessage.From = new MailAddress(from);
            mailMessage.Subject = subject;
            foreach (var mail in to)
                mailMessage.To.Add(mail);
            if (!string.IsNullOrEmpty(cc))
                mailMessage.CC.Add(new MailAddress(cc));
            //string emailReply = ConfigurationManager.AppSettings["Email.ReplyTo"];
            string emailReply = cc;
            if (!string.IsNullOrEmpty(emailReply))
                mailMessage.ReplyToList.Add(new MailAddress(emailReply));

            if (attachments != null)
            {
                foreach (string attach in attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(attach));
                }
            }
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = html;
            if (from != string.Empty)
            {
                mailMessage.Headers.Add("Disposition-Notification-To", from);
                mailMessage.Headers.Add("Read-Receipt-To", from);
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.OnFailure;
            }

            return mailMessage;
        }

        private static MailMessage CreateMessage(EmailTemplate template, ListDictionary replacements, string to, string from, string cc, string subject)
        {
            string physicalPath = HttpContext.Current.Server.MapPath("~/App_Data/EmailTemplates/" + template.ToString() + ".txt");
            if (!File.Exists(physicalPath))
                throw new ArgumentException("Invalid EMail Template Passed into the NotificationManager", "template");

            string html = File.ReadAllText(physicalPath);
            if (replacements != null)
            {
                foreach (DictionaryEntry param in replacements)
                {
                    string val = param.Value == null ? "" : param.Value.ToString();
                    html = html.Replace(param.Key.ToString(), val);
                }
            }

            MailMessage mailMessage = new MailMessage();
            if (from != string.Empty)
                mailMessage.From = new MailAddress(from);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(cc))
                mailMessage.CC.Add(new MailAddress(cc));

            string emailBCC = ConfigurationManager.AppSettings["Email.BCC"];
            if (!string.IsNullOrEmpty(emailBCC))
                mailMessage.Bcc.Add(new MailAddress(emailBCC));

            mailMessage.IsBodyHtml = true;
            mailMessage.Body = html;

            return mailMessage;
        }

        private static bool SendMailMessage(MailMessage mailMessage)
        {
            bool send = true;

            try
            {
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                int usaSSL = Convert.ToInt32(ConfigurationManager.AppSettings["Email.UsaSSL"]);
                if(usaSSL == 1)
                    client.EnableSsl = true;
                else
                    client.EnableSsl = false;


                string mailFrom = ConfigurationManager.AppSettings["Email.From"];
                string mailPass = ConfigurationManager.AppSettings["Email.Password"];
                client.Credentials = new NetworkCredential(mailFrom, mailPass);


                client.Host = ConfigurationManager.AppSettings["Email.SMTP"];
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Email.Port"]);
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                send = false;
                //throw;// new Exception();
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, ex.ToString());
            }

            return send;
        }


        //private static bool SendMailMessage(MailMessage mailMessage)
        //{
        //    bool send = true;

        //    try
        //    {
        //        //From Address    
        //        string FromAddress = "myname@company.com";
        //        string FromAdressTitle = "My Name";
        //        //To Address    
        //        string ToAddress = email;
        //        string ToAdressTitle = "Microsoft ASP.NET Core";
        //        string Subject = subject;
        //        string BodyContent = message;

        //        //Smtp Server    
        //        string SmtpServer = "smtp.office365.com";
        //        //Smtp Port Number    
        //        int SmtpPortNumber = 587;

        //        //Generate Message 
        //        var mimeMessage = new MimeMessage();
        //        mimeMessage.From.Add(new MailboxAddress
        //                                (FromAdressTitle,
        //                                 FromAddress
        //                                 ));
        //        mimeMessage.To.Add(new MailboxAddress
        //                                 (ToAdressTitle,
        //                                 ToAddress
        //                                 ));
        //        mimeMessage.Subject = Subject; //Subject  
        //        mimeMessage.Body = new TextPart("plain")
        //        {
        //            Text = BodyContent
        //        };

        //        using (var client = new SmtpClient())
        //        {
        //            client.Connect(SmtpServer, SmtpPortNumber, false);
        //            client.Authenticate(
        //                "myname@company.com",
        //                "MYPassword"
        //                );
        //            await client.SendAsync(mimeMessage);
        //            Console.WriteLine("The mail has been sent successfully !!");
        //            Console.ReadLine();
        //            await client.DisconnectAsync(true);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        send = false;
        //        //throw;// new Exception();
        //        var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, ex.ToString());
        //    }

        //    return send;
        //}

    }


}