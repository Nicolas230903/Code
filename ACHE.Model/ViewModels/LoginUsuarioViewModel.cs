using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.ViewModels
{
    public class LoginUsuarioViewModel
    {
        public int IDLoginUsuario { get; set; }
        public int IDUsuario { get; set; }
        public int IDUsuarioAdicional { get; set; }

        public string Email { get; set; }
        public string RazonSocial { get; set; }
        public string Observaciones { get; set; }
        public DateTime Fecha { get; set; }

    }
}