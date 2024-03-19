using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.TiendaNube
{
    public class ResponseGetOrdersElum
    {
        public string idOrden { get; set; }       
        public string subtotal { get; set; }
        public string descuento { get; set; }
        public string descuentoCupon { get; set; }
        public string descuentoGateway { get; set; }
        public string total { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaModificacion { get; set; }
        public Cliente cliente { get; set; }
        public List<Producto> productos { get; set; }
        public string razonDeCancelacion { get; set; }
        public string fechaCancelacion { get; set; }
        public string fechaCerrado { get; set; }
        public string estado { get; set; }
        public string estadoDePago { get; set; }
        public string estadoDeEnvio { get; set; }
        public string opcionDeEnvio { get; set; }
        public string FechaEnvio { get; set; }
        public string fechaPago { get; set; }
        public string nota { get; set; }         
        
        public class Cliente
        {
            public string id { get; set; }
            public string nombre { get; set; }
            public string email { get; set; }
            public string identificacion { get; set; }
            public string telefono { get; set; }
            public string domicilioDireccion { get; set; }
            public string domicilioNumero { get; set; }
            public string domicilioPiso { get; set; }
            public string domicilioLocalidad { get; set; }
            public string domicilioCodigoPostal { get; set; }
            public string domicilioCiudad { get; set; }
            public string domicilioProvincia { get; set; }
            public string quienRetira { get; set; }
        }
     
        public class Producto
        {
            public string idProducto { get; set; }
            public string idVariante { get; set; }
            public string nombre { get; set; }
            public string precio { get; set; }
            public string cantidad { get; set; }
            public string sku { get; set; }

        }

    }
}
