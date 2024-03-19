using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.TiendaNube
{
    public class Variant
    {
        public int id { get; set; }
        public int? image_id { get; set; }
        public int product_id { get; set; }
        public int position { get; set; }
        public string price { get; set; }
        public string promotional_price { get; set; }
        public bool stock_management { get; set; }
        public int? stock { get; set; }
        public string weight { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string depth { get; set; }
        public string sku { get; set; }
        public IList<Value> values { get; set; }
        public string barcode { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
