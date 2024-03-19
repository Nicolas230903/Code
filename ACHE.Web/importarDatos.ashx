<%@ WebHandler Language="C#" Class="importarDatos" %>

using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

public class importarDatos : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            if (context.Request.QueryString["upload"] != null)
            {
                string pathrefer = context.Request.UrlReferrer.ToString();
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/importaciones/Datos/");

                var postedFile = context.Request.Files[0];

                string file;

                //For IE to get file name
                if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
                {
                    string[] files = postedFile.FileName.Split(new char[] { '\\' });
                    file = files[files.Length - 1];
                }
                else
                {
                    file = postedFile.FileName;
                }


                //if (!Directory.Exists(Serverpath))
                //    Directory.CreateDirectory(Serverpath);

                string fileDirectory = Serverpath;
                //if (context.Request.QueryString["fileName"] != null)
                //{
                //    file = context.Request.QueryString["fileName"];
                //    if (File.Exists(fileDirectory + "\\" + file))
                //    {
                //        File.Delete(fileDirectory + "\\" + file);
                //    }
                //}

                string ext = Path.GetExtension(fileDirectory + "\\" + file);
                file = Guid.NewGuid() + ext;

                fileDirectory = Serverpath + "\\" + file;

                context.Response.ContentType = "application/json";
                if (ext.ToLower() == ".csv")
                {
                    postedFile.SaveAs(fileDirectory);
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = file };
                    context.Response.Write(serializar.Serialize(resut));
                }
                else {
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var resut = new { name = "ERROR" };
                    context.Response.Write(serializar.Serialize(resut));
                }
            }
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

}