using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ACHE.Model.ViewModels
{
    public class ComprobantesVinculadosCart
    {
        #region Properties

        public string Tipo { get; set; }
        public List<int> Items { get; set; }

        #endregion

        public static ComprobantesVinculadosCart Instance;

        public static ComprobantesVinculadosCart Retrieve()
        {
            if (HttpContext.Current.Session["ASPNETComprobantesVinculadosCart"] == null)
            {
                Instance = new ComprobantesVinculadosCart();
                Instance.Items = new List<int>();
                HttpContext.Current.Session["ASPNETComprobantesVinculadosCart"] = Instance;
            }
            else
            {
                Instance = (ComprobantesVinculadosCart)HttpContext.Current.Session["ASPNETComprobantesVinculadosCart"];
            }

            return Instance;
        }

        // A protected constructor ensures that an object can't be created from outside  
        protected ComprobantesVinculadosCart() { } 

      
    }
}
