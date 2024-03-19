using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.TiendaNube
{
    public class Products
    {
        public int id { get; set; }
        public Name name { get; set; }
        public Description description { get; set; }
        public Handle handle { get; set; }
        public List<Attributes> attributes { get; set; }
        public string sku { get; set; }
        public bool published { get; set; }
        public bool free_shipping { get; set; }
        public string canonical_url { get; set; }
        public SeoTitle seo_title { get; set; }
        public SeoDescription seo_description { get; set; }
        public string brand { get; set; }
        public List<Variant> variants { get; set; }
        public string tags { get; set; }
        public List<int> categories { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<Images> images { get; set; }
    }
}
