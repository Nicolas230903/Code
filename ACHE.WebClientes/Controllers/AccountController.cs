using ACHE.WebClientes.Models;
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

namespace ACHE.WebClientes.Controllers
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
                    var persona = dbContext.Personas.Where(x => x.NroDocumento == model.Documento && x.TipoDocumento == model.TipoDocumento).FirstOrDefault();
                    var pwd = dbContext.PersonasPwd.Where(x => x.TipoDocumento == model.TipoDocumento && x.NroDocumento == model.Documento).FirstOrDefault();
                    if (persona != null)
                    {
                        var Token = generarToken(dbContext, persona.IDPersona);

                        if (pwd != null)
                        {

                            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel(persona.RazonSocial, persona.Email, pwd.IDPersonaPwd, persona.NroDocumento, persona.TipoDocumento, Token);
                            //string userData = JsonConvert.SerializeObject(serializeModel);
                            //FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, persona.Email, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);

                            //CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel(user.Nombre, user.Apellido, user.Email, user.IDAfiliado, user.IDEntidad);
                            Response.SetAuthCookie(persona.IDPersona.ToString(), false, serializeModel);

                            if (!pwd.CambioPwd)
                                return RedirectToAction("PrimerLogin");
                            else if (pwd.Pwd == model.Password)
                                return RedirectToAction("Index", "Home");
                            else
                                ModelState.AddModelError("", "Nro de documento y/o contraseña incorrecto");
                        }
                        else
                        {
                            int punto = 0;
                            int nro = 0;

                            if (model.Password.Split("-").Length == 2)
                            {
                                int.TryParse(model.Password.Split("-")[0], out  punto);
                                int.TryParse(model.Password.Split("-")[1], out  nro);

                                if (dbContext.Comprobantes.Any(x => x.IDPersona == persona.IDPersona && x.PuntosDeVenta.Punto == punto && x.Numero == nro))
                                {

                                    PersonasPwd nuevoPwd = new PersonasPwd();
                                    nuevoPwd.TipoDocumento = persona.TipoDocumento;
                                    nuevoPwd.NroDocumento = persona.NroDocumento;
                                    nuevoPwd.FechaAlta = DateTime.Now;
                                    nuevoPwd.CambioPwd = false;
                                    nuevoPwd.Email = persona.Email;

                                    try
                                    {
                                        dbContext.PersonasPwd.Add(nuevoPwd);
                                        dbContext.SaveChanges();

                                    }
                                    catch (Exception)
                                    {
                                        ModelState.AddModelError("", "Hubo un error en el login, por favor intente nuevamente");
                                    }

                                    //Se deberia hacer el  Response.SetAuthCookie, sin enviar el mail. Sino cualquiera puede acceder a ese metodo desde la URL. NO es seguro
                                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel(persona.RazonSocial, persona.Email, nuevoPwd.IDPersonaPwd, persona.NroDocumento, persona.TipoDocumento, Token);
                                    Response.SetAuthCookie(persona.IDPersona.ToString(), false, serializeModel);

                                    return RedirectToAction("PrimerLogin", "Account", new { id = nuevoPwd.IDPersonaPwd });
                                }
                                else
                                    ModelState.AddModelError("", "Nro de documento y/o contraseña incorrecto");
                            }
                            else
                                ModelState.AddModelError("", "Nro de documento y/o contraseña incorrecto");
                        }
                    }
                    else
                        ModelState.AddModelError("", "Nro de documento y/o contraseña incorrecto");
                }
                return View(model);
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
                }
                catch (Exception )
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