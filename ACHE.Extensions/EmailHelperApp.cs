using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Configuration;

namespace ACHE.Model
{
    public enum EmailTemplateApp
    {
        NotificacionDosDias,
        NotificacionDosSemanas,
        NotificacionVeiticincoDias,
        NotificacionCincoDiasVencimiento,
        NotificacionAdministrador,
        Alertas,
        AvisosVencimiento
    }

    public static class EmailHelperApp
    {
        public static bool SendMessage(EmailTemplateApp template, ListDictionary replacements, string to, string subject)
        {
            string emailFrom = ConfigurationManager.AppSettings["Email.From"];
            MailMessage mailMessage = CreateMessage(template, replacements, to, emailFrom, subject);

            return SendMailMessage(mailMessage);
        }

        private static MailMessage CreateMessage(EmailTemplateApp template, ListDictionary replacements, string to, string from, string subject)
        {
            var pathBase = ConfigurationManager.AppSettings["PathBaseWeb"];

            string physicalPath = (Path.Combine(pathBase + "App_Data\\EmailTemplates\\") + template.ToString() + ".txt");
            if (!File.Exists(physicalPath))
                throw new ArgumentException("Invalid EMail Template Passed into the NotificationManager", "template");

            string html = File.ReadAllText(physicalPath);
            foreach (DictionaryEntry param in replacements)
                html = html.Replace(param.Key.ToString(), param.Value.ToString());

            MailMessage mailMessage = new MailMessage();
            if (from != string.Empty)
                mailMessage.From = new MailAddress(from);
            mailMessage.Subject = subject;

            foreach (var mail in to.Split(','))
            {
                if (mail != string.Empty)
                    mailMessage.To.Add(mail);
            }
            //mailMessage.To.Add(to);

            string cc = ConfigurationManager.AppSettings["Email.CC"];
            if (!string.IsNullOrEmpty(cc))
                mailMessage.CC.Add(new MailAddress(cc));

            var replyTo = ConfigurationManager.AppSettings["Email.ReplyTo"];
            if (!string.IsNullOrEmpty(replyTo))
                mailMessage.ReplyToList.Add(new MailAddress(replyTo));

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
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                send = false;
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(msg);
            }
            return send;
        }
    }
}