using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class fileUpload : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (CurrentUser != null)
            {
                try
                {
                    if (Request.QueryString["path"] != null)
                    {
                        string basePath = Server.MapPath("~/files/explorer/" + CurrentUser.IDUsuario + "/");
                        var path = Request.QueryString["path"];
                        foreach (string s in Request.Files)
                        {
                            HttpPostedFile file = Request.Files[s];
                            int fileSizeInBytes = file.ContentLength;
                            string fileName = file.FileName;
                            string fileExtension = "";

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                fileExtension = Path.GetExtension(fileName).ToUpper();
                                if (fileExtension.ExtensionIsOK())
                                {
                                    if (!fileExtension.ExtensionIsDangerous())
                                        file.SaveAs(basePath + path + "//" + fileName);
                                    else
                                        throw new CustomException("La extecion es peligrosa");
                                }
                                else
                                    throw new CustomException("El archivo solo puede ser una imagen JPG, JPEG, PNG, GIF o un documento PDF, DOC, DOCX, RTF");
                            }
                        }
                    }
                }
                catch (CustomException ex)
                {
                    var serializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Write(serializar.Serialize(ex.Message) + "####");
                    HttpContext.Current.Response.StatusCode = 500;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}