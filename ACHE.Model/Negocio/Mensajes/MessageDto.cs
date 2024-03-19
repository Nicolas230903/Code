using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.Mensajes
{
    public class MessageDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }
}
