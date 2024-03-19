using ACHE.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACHE.Model;
using ACHE.Extensions;
using System.Web.Security;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Configuration;

namespace ACHE.Admin.Controllers
{
    public class AccountController : BaseController
    {
        // GET: /Account/
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new ACHEEntities())
                {
                    var Usuario = model.Usuario;
                    var pass = model.Password;
                    var usuAdmin = ConfigurationManager.AppSettings["Admin.usu"];
                    var usuPwd = ConfigurationManager.AppSettings["Admin.pwd"];

                    if (usuAdmin != Usuario || usuPwd != pass)
                    {
                        ModelState.AddModelError("", "Usuario y/o contraseña incorrecto");
                        return View(model);
                    }
                    else
                    {
                        CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel("Admin", "", 1, "", "", "");
                        //Se deberia hacer el  Response.SetAuthCookie, sin enviar el mail. Sino cualquiera puede acceder a ese metodo desde la URL. NO es seguro
                        Response.SetAuthCookie("1", false, serializeModel);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        public ActionResult PrimerLogin()
        {
            //MAL: EL id no deberia ser por parametro. Deberia salir de la variable User!
            using (var dbContext = new ACHEEntities())
            {

                var pwd = dbContext.PersonasPwd.Where(x => x.IDPersonaPwd == User.IDPersonaPwd).FirstOrDefault();
                if (pwd != null)
                {
                    if (pwd.CambioPwd)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewBag.IdLogin = User.IDPersonaPwd;
            return View();
        }

        [HttpPost]
        public ActionResult CrearLogin(string pwd)
        {
            //MAL: EL id no deberia ser por parametro. Deberia salir de la variable User!

            string result = string.Empty;
            using (var dbContext = new ACHEEntities())
            {
                try
                {
                    var login = dbContext.PersonasPwd.Where(x => x.IDPersonaPwd == User.IDPersonaPwd).FirstOrDefault();
                    if (login != null)
                    {
                        if (!login.CambioPwd)
                        {
                            login.Pwd = pwd;
                            login.CambioPwd = true;
                            dbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception)
                {
                    result = "false";
                }
                result = "true";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecuperarPwd(string documento, string tipo)
        {
            string result = string.Empty;

            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var pwd = dbContext.PersonasPwd.Where(x => x.NroDocumento == documento && x.TipoDocumento == tipo).FirstOrDefault();
                    if (pwd != null)
                    {
                        string newPwd = string.Empty;
                        newPwd = newPwd.GenerateRandom(6);

                        pwd.Pwd = newPwd;

                        ListDictionary data = new ListDictionary();
                        data.Add("<PASSWORD>", newPwd);

                        bool send = EmailHelper.SendMessage(EmailTemplate.RecuperoPwd, data, pwd.Email, "AXAN: Recuperar contraseña");
                        if (!send)
                            result = "false";
                        else
                        {
                            dbContext.SaveChanges();
                            result = "true";
                        }
                    }
                    else
                        result = "pwd";
                }
            }
            catch (Exception)
            {
                result = "false";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CambiarPwd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarPwd(string pwdOld, string pwd)
        {
            string result = string.Empty;

            using (var dbContext = new ACHEEntities())
            {
                try
                {
                    //if (User != null)//MAL: Deberia funcionar automaticamente a travez del BaseController.¿Funciona?

                    //MAL. Si estpas logueado, ya tenés el IDPersonaPwd en user.IDPersona
                    //var usuario = dbContext.Personas.Where(x => x.IDUsuario == User.IDPersona).FirstOrDefault();
                    //if (usuario != null) {
                    var login = dbContext.PersonasPwd.Where(x => x.IDPersonaPwd == User.IDPersonaPwd).FirstOrDefault();
                    if (login != null)
                    {
                        if (!string.IsNullOrEmpty(pwdOld) && !string.IsNullOrEmpty(pwd))
                        {
                            string pass = login.Pwd;
                            if (pass != pwdOld)
                                result = "incorrecta";
                            else
                            {
                                login.Pwd = pwd;
                                dbContext.SaveChanges();
                                result = "true";
                            }
                        }
                    }
                    //}
                    //}
                    //else
                    //    result = "usuario";
                }
                catch (Exception)
                {
                    result = "false";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        private string generarToken(ACHEEntities dbContext, int idPersona)
        {
            var entity = dbContext.AuthenticationTokenClientes.Where(x => x.IDPersona == idPersona && x.FechaExpiracion < DateTime.Now).FirstOrDefault();
            if (entity == null || entity.FechaExpiracion < DateTime.Now)
            {
                string token = Guid.NewGuid().ToString();
                entity = new AuthenticationTokenClientes();
                entity.IDPersona = idPersona;
                entity.Token = token;
                entity.FechaExpiracion = DateTime.Now.AddHours(1);

                dbContext.AuthenticationTokenClientes.Add(entity);
                dbContext.SaveChanges();
                return token;
            }
            return entity.Token;
        }
    }
}