<%@ WebHandler Language="C#" Class="pdfGenerator" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACHE.Model;
using System.Web.SessionState;

public class pdfGenerator : IHttpHandler, IReadOnlySessionState
{
    public void ProcessRequest(HttpContext context)
    {
        string file = context.Request.QueryString["file"];
        string tipoArchivo = context.Request.QueryString["tipoDeArchivo"];
        if (file != string.Empty)
        {
            string extension = file.Split('.')[1].ToLower();
            string type = context.Request.QueryString["type"];

            var usu = (WebUser)context.Session["CurrentUser"];
            string filePath =string.Empty;
            if (usu != null)
            {
                if (!string.IsNullOrWhiteSpace(tipoArchivo))
                {
                    switch (tipoArchivo)
                    {
                        case "remito":
                            filePath = "~/files/remitos/" + usu.IDUsuario + "/" + file;
                            break;
                        case "remitoSinLogo":
                            filePath = "~/files/remitosSinLogo/" + usu.IDUsuario + "/" + file;
                            break;
                        case "presupuestos":
                            filePath = "~/files/presupuestos/" + usu.IDUsuario + "/" + file;
                            break;
                        case "compras":
                            filePath = "~/files/explorer/" + usu.IDUsuario + "/Compras/" + DateTime.Now.Year.ToString() + "/" + file;
                            break;
                        case "Caja":
                            filePath = "~/files/explorer/" + usu.IDUsuario + "/Caja/" + file;
                            break;
                        case "liquidoProducto":
                            filePath = "~/files/liquidoProducto/" + usu.IDUsuario + "/" + file;
                            break;
                    }
                }
                else
                    filePath = "~/files/explorer/" + usu.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/" + file;
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Por favor, vuelva a iniciar sesión");
            }

            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(context.Server.MapPath(filePath)))
            {
                context.Response.Clear();
                context.Response.ContentType = "application/octet-stream";
                //I have set the ContentType to "application/octet-stream" which cover any type of file
                context.Response.AddHeader("content-disposition", "attachment;filename=" + System.IO.Path.GetFileName(file));
                context.Response.WriteFile(context.Server.MapPath(filePath));
                //here you can do some statistic or tracking
                //you can also implement other business request such as delete the file after download
                context.Response.End();

                //ACHE.Model.Downloader.DownloadFile(context, context.Server.MapPath(filePath));
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Archivo desconocido!");
            }
        }
        else
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Archivo desconocido!");
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