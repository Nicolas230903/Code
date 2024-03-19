using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace ACHE.Admin.Models
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            if (Roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CustomPrincipal(string Username)
        {
            this.Identity = new GenericIdentity(Username);
        }

        public int IDPersonaPwd { get; set; }
        public string RazonSocial { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string[] Roles { get; set; }
        public string Token { get; set; }
    } 

    public class CustomPrincipalSerializeModel
    {
        public int IDPersonaPwd { get; set; }
        public string RazonSocial { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string[] Roles { get; set; }
        public string Token { get; set; }

        public CustomPrincipalSerializeModel(string razonSocial, string email, int idPersonaPwd, string documento, string tipo, string token)
        {
            this.IDPersonaPwd = idPersonaPwd;
            this.RazonSocial = razonSocial;
            this.Email = email;
            this.Documento = documento;
            this.TipoDocumento = tipo;
            this.Token = token;
        }
    }
}