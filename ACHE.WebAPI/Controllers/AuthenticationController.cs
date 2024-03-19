using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ACHE.Model;
using ACHE.Extensions;
using System.Text;
using System.Security.Cryptography;

namespace ACHE.WebAPI.Controllers
{
    public class AuthenticationController : ApiController
    {

        AuthenticationToken entity = new AuthenticationToken();

        public HttpResponseMessage Get(string apiKey)
        {
            string token = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(apiKey))
            {
                using (var dbContext = new ACHEEntities())
                {
                    var usuario = dbContext.Usuarios.Where(x => x.ApiKey.ToLower() == apiKey.ToLower().Trim() && x.Activo).FirstOrDefault();
                    if (usuario != null)
                    {
                        var entity = dbContext.AuthenticationToken.Where(x => x.IDUsuario == usuario.IDUsuario && x.FechaExpiracion < DateTime.Now).FirstOrDefault();
                        if (entity == null)
                        {
                            entity = new AuthenticationToken();
                            entity.IDUsuario = usuario.IDUsuario;
                            entity.Token = token;
                            entity.FechaExpiracion = DateTime.Now.AddHours(5);

                            dbContext.AuthenticationToken.Add(entity);
                            dbContext.SaveChanges();
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, entity.Token);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "No se ha encontrado ningún usuario con el ApiKey ingresado");
                }

            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe ingresar su ApiKey");
        }

        //[HttpPost]
        //public HttpResponseMessage ObtenerToken(string apiKey, string email)
        //{

        //    if (string.IsNullOrWhiteSpace(email))
        //        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar la email como parámetro");
        //    if (!email.IsValidEmailAddress())
        //        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El email es invalido");

        //    string token = Guid.NewGuid().ToString();
        //    if (!string.IsNullOrEmpty(apiKey))
        //    {
        //        using (var dbContext = new ACHEEntities())
        //        {
        //            var usuario = dbContext.Usuarios.Where(x => x.ApiKey.ToLower() == apiKey.ToLower().Trim() && x.Email.ToUpper() == email.ToUpper() && x.Activo).FirstOrDefault();
        //            if (usuario != null)
        //            {
        //                var fecha = DateTime.Now.Date;
        //                //var planValido = dbContext.UsuariosPlanesView.Any(x => x.IDUsuario == usuario.IDUsuario && x.IDPlan > 1 && x.FechaFinPlan > fecha);

        //                //if (!planValido)
        //                //    return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe tener un plan valido");

        //                var entity = dbContext.AuthenticationToken.Where(x => x.IDUsuario == usuario.IDUsuario && x.FechaExpiracion > DateTime.Now).FirstOrDefault();
        //                if (entity == null)
        //                {
        //                    entity = new AuthenticationToken();
        //                    entity.IDUsuario = usuario.IDUsuario;
        //                    entity.Token = token;
        //                    entity.FechaExpiracion = DateTime.Now.AddHours(5);

        //                    dbContext.AuthenticationToken.Add(entity);
        //                    dbContext.SaveChanges();
        //                }

        //                return Request.CreateResponse(HttpStatusCode.OK, entity.Token);
        //            }
        //            else
        //                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "No se ha encontrado ningún usuario con el ApiKey ingresado");
        //        }

        //    }
        //    else
        //        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe ingresar su ApiKey");
        //}


        [HttpPost]
        public HttpResponseMessage ObtenerToken(string email, string pwd)
        {

            if (string.IsNullOrWhiteSpace(email))
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe pasar el email como parámetro");
            if (!email.IsValidEmailAddress())
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "El email es invalido");

            string token = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(pwd))
            {
                using (var dbContext = new ACHEEntities())
                {
                    string pwdEncrypt = MD5Hash(pwd.Trim());

                    var usuario = dbContext.Usuarios.Where(x => x.Pwd == pwdEncrypt && x.Email.ToUpper() == email.ToUpper() && x.Activo).FirstOrDefault();
                    if (usuario != null)
                    {
                        var fecha = DateTime.Now.Date;
                        //var planValido = dbContext.UsuariosPlanesView.Any(x => x.IDUsuario == usuario.IDUsuario && x.IDPlan > 1 && x.FechaFinPlan > fecha);

                        //if (!planValido)
                        //    return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe tener un plan valido");

                        var entity = dbContext.AuthenticationToken.Where(x => x.IDUsuario == usuario.IDUsuario && x.FechaExpiracion > DateTime.Now).FirstOrDefault();
                        if (entity == null)
                        {
                            entity = new AuthenticationToken();
                            entity.IDUsuario = usuario.IDUsuario;
                            entity.Token = token;
                            entity.FechaExpiracion = DateTime.Now.AddHours(5);

                            dbContext.AuthenticationToken.Add(entity);
                            dbContext.SaveChanges();
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, entity.Token);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Email y/o password incorrecta");
                }

            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Debe ingresar su password");
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

    }
}