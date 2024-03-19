using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Model;
using ACHE.Extensions;
using System.Text.RegularExpressions;
using System.Web.Services;
public partial class file_explorer : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (var dbContext = new ACHEEntities())
        {
            AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

            if (afu != null)
                if (!afu.HerramientasExploradorDeArchivos)
                    Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosFileExplorerViewModel getInfo(string path)
    {
        try
        {
            ResultadosFileExplorerViewModel resultado = new ResultadosFileExplorerViewModel();
            resultado.Folders = new List<FileExplorerViewModel>();
            resultado.Files = new List<FileExplorerViewModel>();

            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

                string basePath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/");

                var pathNavigation = "Ubicación actual:<a href=\"javascript:loadInfo('');\"><i class='fa fa-home'></i></a>";

                int cant = path.Split(@"//").Length;
                int index = 0;
                var ant = "";
                foreach (var aux in path.Split(@"//"))
                {
                    if (aux != string.Empty)
                    {
                        index++;
                        pathNavigation += "<span style='margin-left: -5px;margin-right: 5px;'>></span>";
                        if (index == cant)
                            pathNavigation += "<a class='active' href='#'>" + aux + "</a>";
                        else
                        {
                            pathNavigation += "<a href=\"javascript:loadInfo('" + ant + aux + "');\">" + aux + "</a>";
                            ant += aux + "//";
                        }
                    }
                }

                resultado.PathNavigation = pathNavigation;

                if (Directory.Exists(basePath))
                {
                    DirectoryInfo dir = new System.IO.DirectoryInfo(basePath + path);

                    foreach (System.IO.DirectoryInfo g in dir.GetDirectories())
                    {
                        FileExplorerViewModel folder = new FileExplorerViewModel();
                        folder.FechaAlta = g.CreationTime.ToString("dd-MM-yyyy");
                        folder.Nombre = g.Name;
                        folder.ID = g.GetHashCode();
                        folder.Icono = "/images/filetypes/folder.png";
                        folder.Tipo = "C";
                        folder.Path = g.FullName.Replace(basePath, "").ReplaceAll(@"\\", "/");
                        folder.Items = g.GetFiles("*.*", SearchOption.AllDirectories).Length + g.GetDirectories("*.*", SearchOption.AllDirectories).Length;
                        resultado.Folders.Add(folder);
                    }
                    foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
                    {
                        FileExplorerViewModel file = new FileExplorerViewModel();
                        file.FechaAlta = f.CreationTime.ToString("dd-MM-yyyy");
                        file.Nombre = f.Name;
                        file.ID = f.GetHashCode();
                        file.Tipo = "A";
                        file.Items = 0;

                        var ext = f.Extension.ToLower();
                        if (ext == ".jpg" || ext == ".gif" || ext == ".png" || ext == ".jpeg")
                        {
                            if (path == string.Empty)
                                file.Imagen = "/files/explorer/" + usu.IDUsuario + "/" + f.Name;
                            else
                                file.Imagen = "/files/explorer/" + usu.IDUsuario + "/" + path + "/" + f.Name;
                            file.Tipo = "I";
                            file.Icono = "/images/filetypes/" + ext.Replace(".", "") + ".png";
                        }
                        else
                            if (File.Exists(HttpContext.Current.Server.MapPath("~/images/filetypes/" + ext.Replace(".", "") + ".png")))
                                file.Icono = "/images/filetypes/" + ext.Replace(".", "") + ".png";
                            else
                                file.Icono = "/images/filetypes/_blank.png";
                        file.Path = f.FullName.Replace(basePath, "").ReplaceAll(@"\\", "/");
                        resultado.Files.Add(file);
                    }
                }

                return resultado;
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    public static void createFolder(string path, string name)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                string basePath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/");

                if (!Directory.Exists(basePath + path + "//" + name))
                {
                    Directory.CreateDirectory(basePath + path + "//" + name);
                }
                else
                    throw new Exception("El directorio ya existe");
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }
}