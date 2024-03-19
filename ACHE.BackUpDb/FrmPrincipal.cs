using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ACHE.BackUpDb
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                var processFolder = System.AppDomain.CurrentDomain.BaseDirectory + "Proceso\\";
                VaciarCarpeta(processFolder);
                               
                string NombreServidorDB = ConfigurationManager.AppSettings["NombreServidorDB"];
                string NombreDB = ConfigurationManager.AppSettings["NombreDB"];
                var backupFolder = ConfigurationManager.AppSettings["RutaCarpetaBK"];
                var backupHistoryFolder = ConfigurationManager.AppSettings["RutaCarpetaBKHistorico"];
                var cantidadArchivosPermitidosBackUp = ConfigurationManager.AppSettings["CantidadArchivosPermitidosBackUp"];

                string[] Bases = NombreDB.Split(',');

                foreach (string nameDb in Bases)
                {
                    var connectionString = "Data Source=" + NombreServidorDB + "; Initial Catalog=" + nameDb + "; Integrated Security=True";

                    var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

                    // set backupfilename (you will get something like: "C:/temp/MyDatabase-2013-12-07.bak")
                    var backupFileName = String.Format("{0}{1}-{2}.bak",
                        processFolder, sqlConStrBuilder.InitialCatalog,
                        DateTime.Now.ToString("yyyy-MM-dd"));

                    var FileName = sqlConStrBuilder.InitialCatalog + "-" + DateTime.Now.ToString("yyyy-MM-dd");

                    using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                    {
                        var query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'",
                            sqlConStrBuilder.InitialCatalog, backupFileName);

                        using (var command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.CommandTimeout = 0;                            
                            command.ExecuteNonQuery();
                        }
                    }

                    // Zipear con pass
                    string PassZip = ConfigurationManager.AppSettings["PassZip"];

                    using (ZipFile zip = new ZipFile())
                    {
                        zip.Password = PassZip;
                        zip.AddFile(backupFileName, FileName + ".bak");
                        zip.Save(processFolder + FileName + ".zip");
                    }

                    //Move Zip to file for drive 

                    //File.Copy(processFolder + FileName + ".zip", backupFolder + FileName + ".zip", true);
                    File.Copy(processFolder + FileName + ".zip", backupHistoryFolder + FileName + ".zip", true);

                    var sortedFiles = new DirectoryInfo(backupFolder).GetFiles()
                                  .OrderBy(f => f.CreationTime)
                                  .ToList();

                    while (sortedFiles.Count > int.Parse(cantidadArchivosPermitidosBackUp))
                    {
                        File.Delete(sortedFiles[0].FullName);

                        sortedFiles = new DirectoryInfo(backupFolder).GetFiles()
                                                      .OrderBy(f => f.CreationTime)
                                                      .ToList();
                    }

                    if(DateTime.Now.Day == 1)
                        File.Copy(backupHistoryFolder + FileName + ".zip", backupFolder + FileName + ".zip", true);

                }

                //var directory = new DirectoryInfo(backupFolder);
                //long sizeDirectory = DirSize(directory);
                //long sizeGb = (((sizeDirectory / 1024) / 1024) / 1024);

                //if(sizeGb > 14)
                //{
                //    string pattern = "*.zip";
                //    var myFile = (from f in directory.GetFiles(pattern)
                //                  orderby f.LastWriteTime ascending
                //                  select f).First();

                //    if (myFile != null)
                //    {
                //        File.Move(myFile.FullName, backupHistoryFolder + myFile.Name);
                //    }
                //}

            }
            catch (Exception ex)
            {

                try
                {
                    enviarMailError(ex.Message);

                }
                catch (Exception exMail)
                {
                    GuardarArchivoLog(" Error Mail Backup Database - " + DateTime.Now.ToShortDateString() + " - " + exMail.Message);
                }

                GuardarArchivoLog(" Error Backup Database - " + DateTime.Now.ToShortDateString() + " - " + ex.Message);
            }

            this.Close();
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            
            return size;
        }

        public static void enviarMailError(string mensajeError)
        {
            string mailFrom = ConfigurationManager.AppSettings["Email.From"];

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(mailFrom);
            mailMessage.Subject = "Error proceso BackUp ELUM";

            MailAddressCollection listTo = new MailAddressCollection();
            string to = ConfigurationManager.AppSettings["Email.To"];
            foreach (var mail in to.Split(','))
            {
                if (mail != string.Empty)
                    mailMessage.To.Add(new MailAddress(mail));
            }

            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Detalle del error: " + mensajeError;


            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            int usaSSL = Convert.ToInt32(ConfigurationManager.AppSettings["Email.UsaSSL"]);
            if (usaSSL == 1)
                client.EnableSsl = true;
            else
                client.EnableSsl = false;
            
            string mailPass = ConfigurationManager.AppSettings["Email.Password"];
            client.Credentials = new NetworkCredential(mailFrom, mailPass);

            client.Host = ConfigurationManager.AppSettings["Email.SMTP"];
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Email.Port"]);
            client.Send(mailMessage);

        }

        public static void VaciarCarpeta(string ruta)
        {
            DateTime fecha = DateTime.Now.AddDays(-1);

            String[] files = Directory.GetFiles(@ruta).Where(x => new FileInfo(x).Extension != ".txt").ToArray();
            foreach (string file in files)
            {
                File.Delete(file);
            }
            foreach (string item in Directory.GetDirectories(ruta))
                Directory.Delete(item, true);

        }

        public void GuardarArchivoLog(String mensaje)
        {
            string ubicaciónArchivo = System.AppDomain.CurrentDomain.BaseDirectory + "Log\\LogFile.txt";

            //Lo creo
            FileStream fs = null;
            if (!File.Exists(ubicaciónArchivo))
            {
                using (fs = File.Create(ubicaciónArchivo))
                {

                }
            }

            //Escribo
            if (File.Exists(ubicaciónArchivo))
            {
                using (StreamWriter sw = new StreamWriter(ubicaciónArchivo, true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + mensaje);
                }
            }

        }

    }
}
