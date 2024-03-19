<%@ WebHandler Language="C#" Class="fileHandler" %>

using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Linq;
using System.Web;
using System.Web.SessionState;

public class fileHandler : IHttpHandler, IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        if (context.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)context.Session["CurrentUser"];
            string basePath = context.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/");

            string action = context.Request.QueryString["action"];
            string path = context.Request.QueryString["path"];
            string tipo = context.Request.QueryString["tipo"];

            if (path.Contains("../"))
                context.Response.Redirect("/file-explorer.aspx");

            switch (action)
            {
                case "download":
                    download(context, basePath, tipo, path);
                    break;
                case "downloadAll":
                    downloadAll(context, basePath, tipo, path);
                    break;
                case "deletePath":
                    deletePath(context, basePath, tipo, path);
                    break;
                case "deleteAll":
                    deleteAll(context, basePath, tipo, path);
                    break;
                case "rename":
                    rename(context, basePath, tipo, path, context.Request.QueryString["newName"]);
                    break;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public void download(HttpContext context, string basePath, string tipo, string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            string filePath = basePath + path.ReplaceAll(@"//", "\\");
            string name = filePath.Split(@"\\")[filePath.Split(@"\\").Length - 1];

            if (tipo != "C")
            {
                if (File.Exists(basePath + path))
                {
                    context.Response.Clear();
                    context.Response.ClearHeaders();
                    context.Response.BufferOutput = false;  // for large files
                    context.Response.ContentType = "application/octet-stream";
                    //I have set the ContentType to "application/octet-stream" which cover any type of file
                    context.Response.AddHeader("content-disposition", "attachment;filename=" + name.Replace(",", ""));
                    context.Response.WriteFile(filePath);
                    context.Response.End();
                }
            }
            else
            {
                if (Directory.Exists(basePath + path))
                {
                    context.Response.Clear();
                    context.Response.ClearHeaders();
                    context.Response.BufferOutput = false;  // for large files
                    context.Response.ContentType = "application/zip";
                    context.Response.AddHeader("content-disposition", "attachment;filename=" + name.Replace(",", "") + ".zip");

                    using (ZipFile zip = new ZipFile())
                    {
                        if (Directory.Exists(basePath + path))
                            zip.AddDirectory(filePath, "");

                        zip.Save(context.Response.OutputStream);
                    }
                    context.Response.End();
                }
            }
        }
    }

    public void downloadAll(HttpContext context, string basePath, string tipos, string paths)
    {
        string[] aux = tipos.Trim().Split("X-X");
        string[] aux2 = paths.Trim().Split("X-X");
        if (aux.Length == aux2.Length)
        {
            int index = 0;

            context.Response.Clear();
            context.Response.BufferOutput = false;  // for large files
            context.Response.ContentType = "application/zip";
            //I have set the ContentType to "application/octet-stream" which cover any type of file
            context.Response.AddHeader("content-disposition", "attachment;filename=" + DateTime.Now.ToString("ddMMyyyy") + ".zip");

            using (ZipFile zip = new ZipFile())
            {
                foreach (var s in aux)
                {
                    if (s != string.Empty)
                    {
                        string filePath = basePath + aux2[index].ReplaceAll(@"//", "\\");
                        string file = filePath.Split(@"\\")[filePath.Split(@"\\").Length - 1];

                        //Create a zip entry for each attachment
                        if (s == "C")
                        {
                            if (Directory.Exists(basePath + aux2[index]))
                                zip.AddDirectory(filePath, file);
                        }
                        else
                        {
                            if (File.Exists(basePath + aux2[index]))
                                zip.AddFile(filePath, "");
                        }
                        index++;
                    }
                }

                zip.Save(context.Response.OutputStream);
            }
            context.Response.Close();
        }
        else
            throw new Exception("Error inesperado");
    }

    public void deleteAll(HttpContext context, string basePath, string tipos, string paths)
    {
        string[] aux = tipos.Trim().Split("X-X");
        string[] aux2 = paths.Trim().Split("X-X");
        if (aux.Length == aux2.Length)
        {
            int index = 0;
            foreach (var s in aux)
            {
                if (s != string.Empty)
                {
                    if (s == "C")
                    {
                        if (Directory.Exists(basePath + aux2[index]))
                        {
                            FileExtensions.ClearFolder(basePath + aux2[index]);//Directory.Delete(basePath + name, true);
                            Directory.Delete(basePath + aux2[index], true);
                        }
                    }
                    else
                    {
                        if (File.Exists(basePath + aux2[index]))
                            File.Delete(basePath + aux2[index]);
                    }
                    index++;
                }
            }
        }
        else
            throw new Exception("Error inesperado");
    }

    public void deletePath(HttpContext context, string basePath, string tipo, string path)
    {
        if (tipo == "C")
        {
            if (Directory.Exists(basePath + path))
            {
                FileExtensions.ClearFolder(basePath + path);//Directory.Delete(basePath + name, true);
                Directory.Delete(basePath + path, true);
            }
            else
                throw new Exception("El directorio ya fue eliminado");
        }
        else
        {
            if (File.Exists(basePath + path))
                File.Delete(basePath + path);
            else
                throw new Exception("El directorio ya fue eliminado");
        }
    }


    public void rename(HttpContext context, string basePath, string tipo, string path, string newName)
    {
        try
        {
            if (!string.IsNullOrEmpty(path))
            {
                string filePath = basePath + path.ReplaceAll(@"/", "\\");
                string fileName = filePath.Split(@"\\")[filePath.Split(@"\\").Length - 1];
                string newFilePath = filePath.Replace(fileName, "") + newName;

                if (tipo != "C")
                {
                    if (File.Exists(basePath + path))
                    {
                        string fileExtension = Path.GetExtension(newName).ToUpper();
                        if (fileExtension.ExtensionIsOK())
                        {
                            if (fileExtension.ExtensionIsDangerous())
                            {
                                context.Response.Write("Extensión no permitida");
                                context.Response.End();
                            }
                            else
                                FileExtensions.MoveFileWithReplace(filePath, newFilePath);
                        }
                        else
                            throw new CustomException("El archivo tiene una extencion no permitida");
                    }
                }
                else
                {
                    if (Directory.Exists(basePath + path))
                        FileExtensions.MoveDirectoryWithReplace(filePath, newFilePath);
                }
            }
        }
        catch (CustomException ex)
        {
            var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(serializar.Serialize(ex.Message));
            HttpContext.Current.Response.StatusCode = 500;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}