using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.Cryptography;

public partial class login : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session.Remove("CurrentUser");
            Session.Remove("ASPNETPlanActual");
            Session.Remove("ASPNETListaFormularios");
            Session.Remove("ASPNETCobranzaCart");
            Session.Remove("ASPNETComprobanteCart");
            Session.Remove("ASPNETPagosCart");
            Session.Remove("ASPNETListaRoles");


            //LogOut
            if (!String.IsNullOrEmpty(Request.QueryString["logOut"]))
            {
                if (Request.QueryString["logOut"].Equals("true"))
                {

                    HttpCookie myCookie = new HttpCookie("axanwebRecordarme");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(myCookie);

                    myCookie = new HttpCookie("axanwebUsuario");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(myCookie);

                    myCookie = new HttpCookie("axanwebPwd");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(myCookie);
                }
            }
            else
            {
                if (Request.Cookies["axanwebRecordarme"] != null)
                {
                    chkRecordarme.Checked = (Request.Cookies["axanwebRecordarme"].Value == "T");
                    if ((chkRecordarme.Checked) && (Request.Cookies["axanwebUsuario"] != null))
                    {
                        txtUsuario.Value = Server.UrlDecode(Request.Cookies["axanwebUsuario"].Value);
                        if (Request.Cookies["axanwebPwd"] != null)
                        {
                            var pass = Server.UrlDecode(Request.Cookies["axanwebPwd"].Value);

                            try
                            {
                                txtPwd.Value = Encriptar.DesencriptarCadena(pass).Replace("\0", "");
                                tieneDatos.Value = "1";
                            }
                            catch (Exception)
                            {
                                txtPwd.Value = "";
                            }
                        }
                    }
                    else
                    {
                        txtUsuario.Value = "";
                        txtPwd.Value = "";
                    }
                }
            }
            txtPwd.Attributes["type"] = "password";
        }
    }

    [WebMethod(true)]
    public static void recuperar(string email)
    {


        if (!email.IsValidEmailAddress())
            throw new Exception("Email incorrecto.");

        using (var dbContext = new ACHEEntities())
        {
            var usuView = dbContext.UsuariosView.Where(x => x.Email == email && x.Activo).FirstOrDefault();
            if (usuView != null)
            {
                string newPwd = string.Empty;
                //newPwd = newPwd.GenerateRandom(10);
                newPwd = newPwd.GenerateRandomPassword();

                ListDictionary replacements = new ListDictionary();
                replacements.Add("<PASSWORD>", newPwd);
                if (usuView.IDUsuarioAdicional == 0)
                    replacements.Add("<USUARIO>", usuView.RazonSocial);
                else
                    replacements.Add("<USUARIO>", "usuario");

                bool send = EmailHelper.SendMessage(EmailTemplate.RecuperoPwd, replacements, usuView.Email, "axanweb: Recuperación de contraseña");
                if (!send)
                    throw new Exception("El email con tu nueva contraseña no pudo ser enviado.");
                else
                {
                    if (usuView.IDUsuarioAdicional == 0)
                    {
                        var usu = dbContext.Usuarios.Where(x => x.IDUsuario == usuView.IDUsuario).FirstOrDefault();
                        usu.CantIntentos = 0;
                        usu.EstaBloqueado = false;
                        usu.Pwd = Common.MD5Hash(newPwd);
                    }
                    else
                    {
                        var usu = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == usuView.IDUsuarioAdicional).FirstOrDefault();
                        usu.Pwd = Common.MD5Hash(newPwd);
                        usu.CantIntentos = 0;
                        usu.EstaBloqueado = false;
                    }
                    dbContext.SaveChanges();
                }
            }
        }
    }
}