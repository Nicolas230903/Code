<%@ WebHandler Language="C#" Class="subirImagenes" %>

using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ACHE.Negocio.Common;

public class subirImagenes : IHttpHandler, IReadOnlySessionState
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var usu = (WebUser)context.Session["CurrentUser"];
            var opcionUpload = context.Request.QueryString["opcionUpload"];

            switch (opcionUpload)
            {
                case "cheques":
                    var idCheque = Convert.ToInt32(context.Request.QueryString["idCheque"]);
                    guardarCheque(context, idCheque, usu.IDUsuario);
                    break;

                case "LogoEmpresas":
                    var idEmpresa = Convert.ToInt32(context.Request.QueryString["IDUsuario"]);
                    guardarLogoEmpresas(context, idEmpresa);
                    break;

                case "conceptos":
                    var idConcepto = Convert.ToInt32(context.Request.QueryString["idconcepto"]);
                    guardarFotoConcepto(context, idConcepto, usu.IDUsuario);
                    break;
                case "compras":
                    var idCompras = Convert.ToInt32(context.Request.QueryString["idCompras"]);
                    guardarFotoCompras(context, idCompras, usu.IDUsuario);
                    break;
                case "empleados":
                    var idEmpleado = Convert.ToInt32(context.Request.QueryString["idEmpleado"]);
                    guardarFotoEmpleado(context, idEmpleado, usu.IDUsuario);
                    break;
                case "persona":
                    var idPersona = Convert.ToInt32(context.Request.QueryString["idPersona"]);
                    guardarFotoPersona(context, idPersona, usu.IDUsuario);
                    break;
                case "PagosPlan":
                    var idPagosPlan = Convert.ToInt32(context.Request.QueryString["idPagosPlan"]);
                    guardarFotoTransferenciaBancaria(context, idPagosPlan, usu.IDUsuario);
                    break;
                case "Caja":
                    var idCaja = Convert.ToInt32(context.Request.QueryString["idCaja"]);
                    guardarFotoCaja(context, idCaja, usu.IDUsuario);
                    break;
                case "MovimientoDeFondos":
                    var idMovimientoDeFondos = Convert.ToInt32(context.Request.QueryString["idMovimientoDeFondos"]);
                    guardarMovimientoDeFondos(context, idMovimientoDeFondos, usu.IDUsuario);
                    break;
            }
        }
        catch (CustomException exp)
        {
            context.Response.Write(exp.Message);
        }
        catch (Exception exp)
        {
            context.Response.Write(exp.Message);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private void guardarMovimientoDeFondos(HttpContext context, int idMovimientoDeFondos, int idUsuario)
    {
        if (idMovimientoDeFondos != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/MovimientosDeFondos/");
            string file;
            string fileName = "MovimientoDeFondos-" + idMovimientoDeFondos.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;

            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";

            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            // busco el archivo para ser eliminado y replazado por el actual
            string[] fileOld = Directory.GetFiles(Serverpath, fileName + ".jpeg");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);
                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var mov = dbContext.MovimientoDeFondos.Where(x => x.IDMovimientoDeFondo == idMovimientoDeFondos).FirstOrDefault();
                        mov.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero un MovimientoDeFondos");
    }

    private void guardarLogoEmpresas(HttpContext context, int idUsuario)
    {
        if (idUsuario != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/usuarios/");
            string file;
            string fileName = idUsuario.ToString();
            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + "\\" + idUsuario.ToString() + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");



            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);

                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = idUsuario.ToString() + ext };
                    context.Response.Write(serializar.Serialize(resut));

                    using (var dbContext = new ACHEEntities())
                    {
                        var usu = dbContext.Usuarios.Where(x => x.IDUsuario == idUsuario).FirstOrDefault();
                        usu.Logo = resut.name;
                        dbContext.SaveChanges();
                    }
                    
                    HttpContext.Current.Session["CurrentUser"] = TokenCommon.ObtenerWebUser(idUsuario);
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
    }

    private void guardarCheque(HttpContext context, int idCheque, int idUsuario)
    {
        if (idCheque != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/Cheques/");
            string file;
            string fileName = "Cheque-" + idCheque.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            // busco el archivo para ser eliminado y replazado por el actual
            string[] fileOld = Directory.GetFiles(Serverpath, fileName + ".jpeg");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);
                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var cheque = dbContext.Cheques.Where(x => x.IDCheque == idCheque).FirstOrDefault();
                        cheque.Foto = resut.name;
                        dbContext.SaveChanges();

                        //  context.Response.Redirect("~/modulos/Tesoreria/cheques.aspx");
                    }
                }
            }
            else
                throw new Exception("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new Exception("Ingrese primero un cheque");
    }

    private void guardarFotoConcepto(HttpContext context, int idConcepto, int idUsuario)
    {
        if (idConcepto != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/Productos-Servicios/");
            string file;
            string fileName = "concepto-" + idConcepto.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);

                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var concepto = dbContext.Conceptos.Where(x => x.IDConcepto == idConcepto).FirstOrDefault();
                        concepto.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero una compra");
    }

    private void guardarFotoCompras(HttpContext context, int idCompra, int idUsuario)
    {
        if (idCompra != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/Compras/" + DateTime.Now.Year.ToString() + "/");
            string file;
            string fileName = "compra-" + idCompra.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif" || ext.ToLower() == ".pdf" || ext.ToLower() == ".rtf" || ext.ToLower() == ".doc")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);

                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var compra = dbContext.Compras.Where(x => x.IDCompra == idCompra).FirstOrDefault();
                        compra.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero un Concepto");
    }

    private void guardarFotoEmpleado(HttpContext context, int idEmpleado, int idUsuario)
    {
        if (idEmpleado != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/Empleados/");
            string file;
            string fileName = "empleado-" + idEmpleado.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);

                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var empleado = dbContext.Empleados.Where(x => x.IDEmpleados == idEmpleado).FirstOrDefault();
                        empleado.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero un Concepto");
    }

    private void guardarFotoPersona(HttpContext context, int idPersona, int idUsuario)
    {
        if (idPersona != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/Contactos/");
            string file;
            string fileName = "Contactos-" + idPersona.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                eliminarExistente(Serverpath, fileName);

                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                        persona.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero un Concepto");
    }

    private static void guardarFotoTransferenciaBancaria(HttpContext context, int idPlanesPago, int idUsuario)
    {
        if (idPlanesPago != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/transferencias/");
            string file;
            string fileName = "usu_ " + idUsuario.ToString() + "_" + "planesPagos_" + idPlanesPago.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy");

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;

            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";

            double peso = (postedFile.ContentLength / 1024);
            if (peso > 2048) //2 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var planesPagos = dbContext.PlanesPagos.Where(x => x.IDPlanesPagos == idPlanesPago).FirstOrDefault();
                        planesPagos.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero un Concepto");
    }

    private void guardarFotoCaja(HttpContext context, int idCaja, int idUsuario)
    {
        if (idCaja != 0)
        {
            string pathrefer = context.Request.UrlReferrer.ToString();
            string Serverpath = HttpContext.Current.Server.MapPath("~/files/Explorer/" + idUsuario + "/Caja/" + "/");
            string file;
            string fileName = "caja-" + idCaja.ToString();

            var postedFile = context.Request.Files[0];
            //For IE to get file name
            if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
            {
                string[] files = postedFile.FileName.Split(new char[] { '\\' });
                file = files[files.Length - 1];
            }
            else
                file = postedFile.FileName;


            string fileDirectory = Serverpath;
            string ext = Path.GetExtension(fileDirectory + "\\" + file);
            fileDirectory = Serverpath + fileName + ext;
            context.Response.ContentType = "application/json";


            double peso = (postedFile.ContentLength / 1024);
            if (peso > 1024) //1 MB
                throw new CustomException("El archivo no puede superar los 1 MB ");

            if (ext.ToLower() == ".jpeg" || ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif" || ext.ToLower() == ".pdf")
            {
                if (!ext.ToUpper().ExtensionIsDangerous())
                {
                    eliminarExistente(Serverpath, fileName);

                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = fileName + ext };
                    context.Response.Write(serializar.Serialize("OK"));

                    using (var dbContext = new ACHEEntities())
                    {
                        var caja = dbContext.Caja.Where(x => x.IDCaja == idCaja).FirstOrDefault();
                        caja.Foto = resut.name;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
                throw new CustomException("El formato solo puede ser JPEG, JPG, PNG y GIF");
        }
        else
            throw new CustomException("Ingrese primero un Concepto");
    }

    private static void eliminarExistente(string Serverpath, string fileName)
    {
        // busco el archivo para ser eliminado y replazado por el actual
        string[] fileOld = Directory.GetFiles(Serverpath, fileName + ".jpeg");

        string fileEliminar = (Serverpath + fileName + ".jpeg");
        if (fileOld.Length == 0)
        {
            fileOld = Directory.GetFiles(Serverpath, fileName + ".jpg");
            fileEliminar = (Serverpath + fileName + ".jpg");
            if (fileOld.Length == 0)
            {
                fileOld = Directory.GetFiles(Serverpath, fileName + ".png");
                fileEliminar = (Serverpath + fileName + ".png");
                if (fileOld.Length == 0)
                {
                    fileOld = Directory.GetFiles(Serverpath, fileName + ".gif");
                    fileEliminar = (Serverpath + fileName + ".gif");
                }
            }
        }

        if (File.Exists(fileEliminar))
            File.Delete(fileEliminar);
    }
}