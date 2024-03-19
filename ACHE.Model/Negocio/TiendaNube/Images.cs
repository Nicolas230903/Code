using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.TiendaNube
{
    public class Images
    {
        public int id { get; set; }
        public string src { get; set; }
        public string filename { get; set; }
        public string attachment { get; set; }
        public int? position { get; set; }
        public int product_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
