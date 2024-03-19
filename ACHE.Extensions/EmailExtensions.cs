using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Configuration;

namespace ACHE.Extensions
{
    public class EmailExtensions
    {
        public string mailServidor = ConfigurationManager.AppSettings["mailServidor"];
        public string contraseniaMailServidor = ConfigurationManager.AppSettings["contraseniaMailServidor"];
        public string ipLocalServidor = ConfigurationManager.AppSettings["ipLocalServidor"];
        public string hostMailServidor = ConfigurationManager.AppSettings["hostMailServidor"];
        public string mailCopiasServidor = ConfigurationManager.AppSettings["mailCopiasServidor"];

        public void enviarMail(string destinario, string copiasOcultas, string asunto, string body, bool esHTML, string rutaImagen = "", string HTML = "", string fromMail = null)
        {
            string mailHTML = "";
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential = new NetworkCredential(mailServidor, contraseniaMailServidor);
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress(mailServidor);

                smtpClient.Host = hostMailServidor;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;
                smtpClient.EnableSsl = true;

                message.From = fromAddress;
                message.Subject = asunto;
                message.IsBodyHtml = esHTML;
                message.Body = body;
                message.To.Add(destinario);
                if(!mailCopiasServidor.Equals(""))
                    message.Bcc.Add(mailCopiasServidor);

                if (esHTML)
                {
                    mailHTML = this.prepararMail(HTML);

                    AlternateView plainView = AlternateView.CreateAlternateViewFromString("", Encoding.UTF8, MediaTypeNames.Text.Plain);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mailHTML, Encoding.UTF8, MediaTypeNames.Text.Html);

                    if (rutaImagen != "")
                    {
                        LinkedResource img = new LinkedResource(rutaImagen, MediaTypeNames.Image.Jpeg); img.ContentId = "imagen";
                        htmlView.LinkedResources.Add(img);
                    }

                    message.AlternateViews.Add(plainView);
                    message.AlternateViews.Add(htmlView);
                }

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string prepararMail(string contenido)
        {
            StringBuilder htmlBody;
            try
            {
                htmlBody = new StringBuilder();

                htmlBody.Append("<html>");
                htmlBody.Append("<head>");
                htmlBody.Append("</head>");
                htmlBody.Append("<body>");
                htmlBody.Append(contenido);
                htmlBody.Append("</body>");
                htmlBody.Append("</html>");

                return htmlBody.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
