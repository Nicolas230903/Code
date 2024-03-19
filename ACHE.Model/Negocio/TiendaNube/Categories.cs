using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.TiendaNube
{
    public class Categories
    {
        public int? id { get; set; }
        public Name name { get; set; }
        public Description descripcion { get; set; }
        public Handle handle { get; set; }
        public int? parent { get; set; }
        public IList<int>  subcategories { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

    }
}
