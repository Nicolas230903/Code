using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Configuration;
using System.Web.Script.Serialization;

namespace ACHE.MercadoPago
{
    public class API
    {

        #region Aux

        /// <summary>
        /// Enumerador usado para limitar los 3 tipos de monedas permitidos por mercado pago.
        /// </summary>
        public enum Mondeda
        {
            ARS,
            USD,
            BRL
        }

        /// <summary>
        /// Devuelve el cliend_id registrado en Mercado Pago a partir de los datos almacenados en el archivo de configuracion.
        /// </summary>
        public static string ClientIDMercadoPago { get; set; } //Client_id
        //{
        //    get { return ConfigurationManager.AppSettings.Get("Client_id"); }
        //}

        /// <summary>
        /// Devuelve el client_secret registrado en Mercado Pago a partir de los datos almacenados en el archivo de configuracion.
        /// </summary>
        public static string PasswordMercadoPago { get; set; } //Client_secret
        //{
        //    get { return ConfigurationManager.AppSettings.Get("Client_secret"); }
        //}

        /// <summary>
        /// Esta clase se utiliza para crear una nueva preferencia.
        /// </summary>
        private class PreferenceStruct
        {
            public string external_reference { get; set; }
            public List<PreferenceItemStruct> items { get; set; }
            public PreferenceUrlsStruct back_urls { get; set; }
        }

        /// <summary>
        /// Esta clase se utiliza para crear una nueva preferencia.
        /// </summary>
        private class PreferenceItemStruct
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int quantity { get; set; }
            public decimal unit_price { get; set; }
            public string currency_id { get; set; }
            public string picture_url { get; set; }
        }

        /// <summary>
        /// Esta clase se utiliza para crear o actualizar una nueva preferencia.
        /// </summary>
        private class PreferenceUrlsStruct
        {
            public string pending { get; set; }
            public string success { get; set; }
            public string failure { get; set; }
        }


        #endregion Aux

        #region Private Methods

        /// <summary>
        /// Recupera y devuelve un token de acuerdo a los datos del usuario.
        /// </summary>
        /// <returns>Token utilizado para trabajar con la API de Mercado Pago</returns>
        private static string GetToken()
        {
            string jsonResult = string.Empty;
            string getTokenUrl = string.Format("https://api.mercadolibre.com/oauth/token?grant_type=client_credentials&client_id={0}&client_secret={1}", ClientIDMercadoPago, PasswordMercadoPago);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(getTokenUrl);
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                jsonResult = streamReader.ReadToEnd();
            }

            var dynamicObject = System.Web.Helpers.Json.Decode(jsonResult);
            return dynamicObject.access_token;
        }

        /// <summary>
        /// Devuelve un string en formato json con las properties especificadas para configurar la preference.
        /// </summary>
        /// <param name="referenciaExterna">Una referencia nuestra acerca de la preferencia que estamos configurando. Ej: ID de la oferta.</param>
        /// <param name="titulo">Titulo del item a agregar en la preferencia. Ej: Titulo de la oferta.</param>
        /// <param name="descripcion">Descripcion del item a agregar en la preferencia. Ej: Descripcion de la oferta.</param>
        /// <param name="cantidad">Cantidad de items relacionados con la preferencia.</param>
        /// <param name="precioUnitario">Precio unitario del item a agregar en la preferencia. Ej: Precio de la oferta.</param>
        /// <param name="moneda">Moneda del item a agregar en la preferencia (ARS, USD, BRL)</param>
        /// <param name="urlImagen">Url de la imagen a mostrar asociada al item</param>
        /// <returns>Json con las properties necesarias para crear la preferencia.</returns>
        private static string GetJsonPreference(string referenciaExterna, string titulo, string descripcion, int cantidad, decimal precioUnitario, Mondeda moneda, string urlImagen)
        {
            var urlSite = ConfigurationManager.AppSettings["MP.urlSite"];
            PreferenceStruct preference = new PreferenceStruct();
            preference.external_reference = referenciaExterna;
            preference.items = new List<PreferenceItemStruct>();
            preference.items.Add(new PreferenceItemStruct()
            {
                id = referenciaExterna,
                title = titulo,
                description = descripcion,
                quantity = cantidad,
                unit_price = precioUnitario,
                currency_id = moneda.ToString(),
                picture_url = urlSite + "/images/logo.png"
            });


            preference.back_urls = new PreferenceUrlsStruct()
            {
                success = urlSite + ConfigurationManager.AppSettings["MP.success"],
                pending = urlSite + ConfigurationManager.AppSettings["MP.pending"],
                failure = urlSite + ConfigurationManager.AppSettings["MP.failure"]
            };


            return System.Web.Helpers.Json.Encode(preference);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Configura una preferencia y devuelve su init point.
        /// </summary>
        /// <param name="referenciaExterna">Una referencia nuestra acerca de la preferencia que estamos configurando. Ej: ID de la oferta.</param>
        /// <param name="titulo">Titulo del item a agregar en la preferencia. Ej: Titulo de la oferta.</param>
        /// <param name="descripcion">Descripcion del item a agregar en la preferencia. Ej: Descripcion de la oferta.</param>
        /// <param name="cantidad">Cantidad de items relacionados con la preferencia.</param>
        /// <param name="precioUnitario">Precio unitario del item a agregar en la preferencia. Ej: Precio de la oferta.</param>
        /// <param name="moneda">Moneda del item a agregar en la preferencia (ARS, USD, BRL)</param>
        /// <param name="urlImagen">Url de la imagen a mostrar asociada al item</param>
        /// <returns>Devuelve el init point con el cual se hara refernecia a la preferencia cuando necesitemos usarla.</returns>
        public static string AddPreference(string referenciaExterna, string titulo, string descripcion, int cantidad, decimal precioUnitario, Mondeda moneda, string urlImagen)
        {
            string initPoint = string.Empty;
            string addPreferenceUrl = string.Format("https://api.mercadolibre.com/checkout/preferences?access_token={0}", GetToken());

            HttpWebRequest request = null;

            //try {
            request = (HttpWebRequest)WebRequest.Create(addPreferenceUrl);
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            //}
            //catch (Exception ex) {
            //    APILog.AppErrorToFile("Error al crear el request", ex.Message);
            //}

            //try {
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = GetJsonPreference(referenciaExterna, titulo, descripcion, cantidad, precioUnitario, moneda, urlImagen);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            //}
            //catch (Exception ex) {
            //    APILog.AppErrorToFile("Error al crear GetJsonPreference", ex.Message);
            //}

            //try {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResult = streamReader.ReadToEnd();
                var dynamicObject = System.Web.Helpers.Json.Decode(jsonResult);
                initPoint = dynamicObject.init_point;
            }
            //}
            //catch (Exception ex) {
            //    APILog.AppErrorToFile("Error al obtener la respuesta", ex.Message);
            //}

            return initPoint;
        }

        /// <summary>
        /// Configura una preferencia y devuelve su init point.
        /// </summary>
        /// <param name="referenciaExterna">Una referencia nuestra acerca de la preferencia que estamos configurando. Ej: ID de la oferta.</param>
        /// <param name="titulo">Titulo del item a agregar en la preferencia. Ej: Titulo de la oferta.</param>
        /// <param name="descripcion">Descripcion del item a agregar en la preferencia. Ej: Descripcion de la oferta.</param>
        /// <param name="cantidad">Cantidad de items relacionados con la preferencia.</param>
        /// <param name="precioUnitario">Precio unitario del item a agregar en la preferencia. Ej: Precio de la oferta.</param>
        /// <param name="moneda">Moneda del item a agregar en la preferencia (ARS, USD, BRL)</param>
        /// <param name="urlImagen">Url de la imagen a mostrar asociada al item</param>
        /// <param name="urlSuccess">Url donde se debe redireccionar luego de un pago exitoso</param>
        /// <param name="urlPending">Url donde se debe redireccionar luego de un pago pendiente de aprobacion</param>
        /// <param name="urlFailure">Url donde se debe redireccionar luego de un pago que ha fallado</param>
        /// <returns>Devuelve el init point con el cual se hara refernecia a la preferencia cuando necesitemos usarla.</returns>
        public static string UpdatePreference(string idPreferencia, string referenciaExterna, string titulo, string descripcion, int cantidad,
            decimal precioUnitario, Mondeda moneda, string urlImagen)
        {//, string urlSuccess, string urlPending, string urlFailure
            string initPoint = string.Empty;
            string addPreferenceUrl = string.Format("https://api.mercadolibre.com/checkout/preferences/{0}?access_token={1}/", idPreferencia, GetToken());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addPreferenceUrl);
            request.Method = "PUT";
            request.Accept = "application/json";
            request.ContentType = "application/json";

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = GetJsonPreference(referenciaExterna, titulo, descripcion, cantidad, precioUnitario, moneda, urlImagen);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResult = streamReader.ReadToEnd();
                var dynamicObject = System.Web.Helpers.Json.Decode(jsonResult);
                initPoint = dynamicObject.init_point;
            }

            return initPoint;
        }


        /// <summary>
        /// Recupero el detalle de un pago a partir del id de MP.
        /// </summary>
        /// <param name="idPago">Id del pago de MP</param>
        /// <returns>Devuelve un json con el detalle del pago obtenido desde la API de Mercado Pago.</returns>
        public static string GetPago(string idPago)
        {
            string jsonResult = string.Empty;
            string getPagoUrl = string.Format("https://api.mercadolibre.com/collections/notifications/{0}?access_token={1}", idPago, GetToken());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getPagoUrl);
            request.Method = "GET";
            request.Accept = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                jsonResult = streamReader.ReadToEnd();
            }

            return jsonResult;
        }

        public static string GetPagos()
        {
            string jsonResult = string.Empty;
            string getPagosUrl = string.Format("https://api.mercadolibre.com/collections/search?access_token={0}", GetToken());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getPagosUrl);
            request.Method = "GET";
            request.Accept = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                jsonResult = streamReader.ReadToEnd();
            }

            return jsonResult;
        }

        #endregion Public Methods
    }
}
