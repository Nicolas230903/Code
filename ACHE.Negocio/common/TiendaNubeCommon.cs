using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Extensions;
using ACHE.Model;
using ACHE.Model.ViewModels;
using System.IO;
using System.Configuration;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using ACHE.Model.Negocio.TiendaNube;
using System.Dynamic;
using ACHE.Negocio.Helper;
using static ACHE.Model.Negocio.TiendaNube.ResponseGetOrders;
using static ACHE.Model.Negocio.TiendaNube.ResponseGetOrdersElum;

namespace ACHE.Negocio.Common
{
    public class TiendaNubeCommon
    {
        public const string SeparadorDeMiles = ".";//"."
        public const string SeparadorDeDecimales = ",";//","
        #region ABM

        public static int CrearProducto(string tiendaCodigo, string tiendaToken, string tiendaUserAgent, string identificadorUrl, string nombre, string categoria, string subCategoria, 
                                        string nombrePropiedad1, string valorPropiedad1, string nombrePropiedad2, string valorPropiedad2, string nombrePropiedad3, string valorPropiedad3,
                                        string precio, string precioPromocional, string peso, int stock, string sku, string codigoDeBarras, string mostrarEnTienda, string enviaSinCargo,
                                        string descripcion, string tags, string tituloParaSEO, string descripcionParaSEO, string marca, string esVariante, string imagen, 
                                        string idNubePadre, WebUser usu)
        {

            int idCategoria = 0;
            int idSubCategoria = 0;
            int idVariante = 0;
            int idImagen = 0;
            ProductsResponse prodPadre = new ProductsResponse();

            try
            {
                string urlTiendaNube = ConfigurationManager.AppSettings["TiendaNube.Url"];

                //Si el campo categoria tiene datos verifico si existe en tiendaNube sino lo creo.                
                if (!string.IsNullOrEmpty(categoria))
                {
                    bool existeCategoria = false;
                    List<Categories> lc = new List<Categories>();
                    bool paginadoCategoria = true;
                    int paginaCategoria = 1;
                    while (paginadoCategoria)
                    {
                        List<Categories> l = null;
                        var clientCategoriesGet = new RestClient(urlTiendaNube + tiendaCodigo + "/categories?page=" + paginaCategoria.ToString());
                        var requestCategoriesGet = new RestRequest(Method.GET);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        requestCategoriesGet.AddHeader("Content-Type", "application/json");
                        requestCategoriesGet.AddHeader("User-Agent", tiendaUserAgent);
                        requestCategoriesGet.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseCategoriesGet = clientCategoriesGet.Execute(requestCategoriesGet);
                        if (responseCategoriesGet.StatusCode != HttpStatusCode.OK)
                        {
                            if (responseCategoriesGet.StatusCode == HttpStatusCode.NotFound)
                                l = null;//No encontro la categoria entonces lo creo.
                            else
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseCategoriesGet.Content);
                        }
                        else
                            l = JsonConvert.DeserializeObject<List<Categories>>(responseCategoriesGet.Content);//La categoria existe, obtengo el id                        

                        if(l != null)
                        {
                            lc.AddRange(l);
                            if (l.Count < 30)
                                paginadoCategoria = false;
                            else
                                paginaCategoria++;
                        }
                        else
                        {
                            paginadoCategoria = false;
                        }
                    }

                    if (lc.Count > 0)
                    {
                        foreach(Categories c in lc)
                        {
                            if (c.name.es.Trim().ToUpper().Equals(categoria.Trim().ToUpper()) && c.parent == 0)
                            {
                                existeCategoria = true;
                                idCategoria = (int)c.id;
                                break;
                            }
                        }
                    }

                    if (!existeCategoria)
                    {
                        Categories rc = new Categories();
                        var clientCategoriesPost = new RestClient(urlTiendaNube + tiendaCodigo + "/categories");
                        var requestCategoriesPost = new RestRequest(Method.POST);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        requestCategoriesPost.AddJsonBody(                            
                            new
                            {
                                name = new
                                {
                                    es = categoria.Trim()
                                }
                            }                            
                        );
                        requestCategoriesPost.AddHeader("Content-Type", "application/json");
                        requestCategoriesPost.AddHeader("User-Agent", tiendaUserAgent);
                        requestCategoriesPost.AddHeader("Authentication", tiendaToken);
                        
                        IRestResponse responseCategoriesPost = clientCategoriesPost.Execute(requestCategoriesPost);

                        if (responseCategoriesPost.StatusCode != HttpStatusCode.Created)
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseCategoriesPost.Content);
                        else
                        {
                            //Se creó la categoria en tiendaNube y recupero el ID.
                            rc = JsonConvert.DeserializeObject<Categories>(responseCategoriesPost.Content);
                            idCategoria = (int)rc.id;
                        }
                    }

                    //Si el campo subCategoria tiene datos verifico si existe en tiendaNube sino lo creo.               
                    if (!string.IsNullOrEmpty(subCategoria))
                    {
                        bool existeSubCategoria = false;
                        if (existeCategoria)
                        {
                            if (lc.Count > 0)
                            {
                                foreach (Categories c in lc)
                                {
                                    if (c.name.es.Trim().ToUpper().Equals(subCategoria.Trim().ToUpper()) && c.parent == idCategoria)
                                    {
                                        existeSubCategoria = true;
                                        idSubCategoria = (int)c.id;
                                        break;
                                    }
                                }
                            }
                        }                        

                        if (!existeSubCategoria)
                        {
                            Categories rsc = new Categories();
                            var clientSubCategoriesPost = new RestClient(urlTiendaNube + tiendaCodigo + "/categories");
                            var requestSubCategoriesPost = new RestRequest(Method.POST);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            requestSubCategoriesPost.AddJsonBody(
                                new
                                {
                                    name = new
                                    {
                                        es = subCategoria.Trim()
                                    },
                                    parent = idCategoria.ToString()
                                }
                            );
                            requestSubCategoriesPost.AddHeader("Content-Type", "application/json");
                            requestSubCategoriesPost.AddHeader("User-Agent", tiendaUserAgent);
                            requestSubCategoriesPost.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseSubCategoriesPost = clientSubCategoriesPost.Execute(requestSubCategoriesPost);
                            if (responseSubCategoriesPost.StatusCode != HttpStatusCode.Created)
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseSubCategoriesPost.Content);
                            else
                            {
                                //Se creó la sub categoria en tiendaNube y recupero el ID.
                                rsc = JsonConvert.DeserializeObject<Categories>(responseSubCategoriesPost.Content);
                                idSubCategoria = (int)rsc.id;
                            }
                        }
                    }

                }

                //Busco por sku y me fijo que no exista un producto con el mismo sku.
                var clientProductsSkuGet = new RestClient(urlTiendaNube + tiendaCodigo + "/products/sku/" + sku.Trim());
                var requestProductsSkuGet = new RestRequest(Method.GET);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                requestProductsSkuGet.AddHeader("Content-Type", "application/json");
                requestProductsSkuGet.AddHeader("User-Agent", tiendaUserAgent);
                requestProductsSkuGet.AddHeader("Authentication", tiendaToken);
                IRestResponse responseProductsSkuGet = clientProductsSkuGet.Execute(requestProductsSkuGet);
                if (responseProductsSkuGet.StatusCode == HttpStatusCode.OK)
                {
                    throw new Exception("ERROR al acceder a tiendaNube: Ya existe el sku " + sku);
                }
                else
                {
                    if (responseProductsSkuGet.StatusCode != HttpStatusCode.NotFound && responseProductsSkuGet.StatusCode != HttpStatusCode.InternalServerError)
                        throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsSkuGet.Content);
                }

                //Creo un producto
                if (esVariante.Equals("NO"))
                {
                    //Busco por nombre y me fijo que no exista un producto con el mismo nombre.
                    bool paginadoProducto = true;
                    int paginaProducto = 1;
                    while (paginadoProducto)
                    {
                        List<ProductsResponse> lrp = new List<ProductsResponse>();
                        var clientProductsNombreGet = new RestClient(urlTiendaNube + tiendaCodigo + "/products?page=" + paginaProducto.ToString() +"&q=" + nombre.Trim());
                        var requestProductsNombreGet = new RestRequest(Method.GET);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        requestProductsNombreGet.AddHeader("Content-Type", "application/json");
                        requestProductsNombreGet.AddHeader("User-Agent", tiendaUserAgent);
                        requestProductsNombreGet.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseProductsNombreGet = clientProductsNombreGet.Execute(requestProductsNombreGet);
                        if (responseProductsNombreGet.StatusCode == HttpStatusCode.OK)
                        {
                            lrp = JsonConvert.DeserializeObject<List<ProductsResponse>>(responseProductsNombreGet.Content);
                            if (lrp != null)
                            {
                                foreach (ProductsResponse pr in lrp)
                                {
                                    if (pr.name.es.Trim().ToUpper().Equals(nombre.Trim().ToUpper()))
                                        throw new Exception("ERROR al acceder a tiendaNube: Ya existe el nombre " + nombre);
                                }

                                if (lrp.Count < 30)
                                    paginadoProducto = false;
                                else
                                    paginaProducto++;
                            }
                        }
                        else
                        {
                            paginadoProducto = false;
                            if (responseProductsNombreGet.StatusCode != HttpStatusCode.NotFound)
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsNombreGet.Content);
                        }                    
                    }

                    //Creo el producto en tiendanube
                    ProductsResponse rp = new ProductsResponse();
                    var clientProductsPost = new RestClient(urlTiendaNube + tiendaCodigo + "/products");
                    var requestProductsPost = new RestRequest(Method.POST);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    Products p = new Products();
                    Name n = new Name();
                    n.es = nombre.Trim();
                    n.en = nombre.Trim();
                    n.pt = nombre.Trim();
                    p.name = n;
                    if (!string.IsNullOrEmpty(descripcion))
                    {
                        Description d = new Description();
                        d.es = nombre.Trim();
                        d.en = nombre.Trim();
                        d.pt = nombre.Trim();
                        p.description = d;
                    }
                    Handle h = new Handle();
                    h.es = nombre.Trim();
                    p.handle = h;
                    if (!string.IsNullOrEmpty(subCategoria)) //Entiendo que ya esta creada la sub categoria.
                    {
                        List<int> lc = new List<int>();
                        lc.Add(idSubCategoria);
                        p.categories = lc;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(categoria)) //Entiendo que ya esta creada la categoria.
                        {
                            List<int> lc = new List<int>();
                            lc.Add(idCategoria);
                            p.categories = lc;
                        }
                    }                    
                    if (esVariante.Equals("SI"))
                    { //Es con variante, creo el producto con variante.
                        List<Attributes> la = new List<Attributes>();
                        if (!string.IsNullOrEmpty(nombrePropiedad1) && !string.IsNullOrEmpty(valorPropiedad1))
                        {
                            Attributes a1 = new Attributes();
                            a1.es = nombrePropiedad1.Trim();
                            a1.en = nombrePropiedad1.Trim();
                            a1.pt = nombrePropiedad1.Trim();
                            la.Add(a1);
                        }
                        if (!string.IsNullOrEmpty(nombrePropiedad2) && !string.IsNullOrEmpty(valorPropiedad2))
                        {
                            Attributes a2 = new Attributes();
                            a2.es = nombrePropiedad2.Trim();
                            a2.en = nombrePropiedad2.Trim();
                            a2.pt = nombrePropiedad2.Trim();
                            la.Add(a2);
                        }
                        if (!string.IsNullOrEmpty(nombrePropiedad3) && !string.IsNullOrEmpty(valorPropiedad3))
                        {
                            Attributes a3 = new Attributes();
                            a3.es = nombrePropiedad3.Trim();
                            a3.en = nombrePropiedad3.Trim();
                            a3.pt = nombrePropiedad3.Trim();
                            la.Add(a3);
                        }
                        if (la.Count > 0)
                            p.attributes = la;
                    }
                    p.sku = sku.Trim();
                    p.published = (mostrarEnTienda == "SI") ? true : false;
                    p.free_shipping = (enviaSinCargo == "SI") ? true : false;
                    p.canonical_url = @"https:\/\/elumtest.mitiendanube.com\/productos\/" + sku.Trim() + @"\/";
                    if (!string.IsNullOrEmpty(tituloParaSEO))
                    {
                        SeoTitle st = new SeoTitle();
                        st.es = tituloParaSEO.Trim();
                        st.en = tituloParaSEO.Trim();
                        st.pt = tituloParaSEO.Trim();
                        p.seo_title = st;
                    }
                    if (!string.IsNullOrEmpty(descripcionParaSEO))
                    {
                        SeoDescription sd = new SeoDescription();
                        sd.es = descripcionParaSEO.Trim();
                        sd.en = descripcionParaSEO.Trim();
                        sd.pt = descripcionParaSEO.Trim();
                        p.seo_description = sd;
                    }
                    p.brand = null;
                    List<Variant> lvariant = new List<Variant>();
                    Variant v = new Variant();
                    v.price = precio.Trim();
                    v.promotional_price = string.IsNullOrEmpty(precioPromocional) ? null : precioPromocional.Trim();
                    v.stock_management = true;
                    v.weight = string.IsNullOrEmpty(peso) ? null : peso.Trim();
                    v.sku = sku.Trim();
                    v.stock = stock;
                    v.position = 1;
                    if (esVariante.Equals("SI"))
                    { //Es con variante, creo el producto con variante.
                        List<Value> lv = new List<Value>();
                        if (!string.IsNullOrEmpty(nombrePropiedad1) && !string.IsNullOrEmpty(valorPropiedad1))
                        {
                            Value v1 = new Value();
                            v1.es = valorPropiedad1.Trim();
                            v1.en = valorPropiedad1.Trim();
                            v1.pt = valorPropiedad1.Trim();
                            lv.Add(v1);
                        }
                        if (!string.IsNullOrEmpty(nombrePropiedad2) && !string.IsNullOrEmpty(valorPropiedad2))
                        {
                            Value v2 = new Value();
                            v2.es = valorPropiedad2.Trim();
                            v2.en = valorPropiedad2.Trim();
                            v2.pt = valorPropiedad2.Trim();
                            lv.Add(v2);
                        }
                        if (!string.IsNullOrEmpty(nombrePropiedad3) && !string.IsNullOrEmpty(valorPropiedad3))
                        {
                            Value v3 = new Value();
                            v3.es = valorPropiedad3.Trim();
                            v3.en = valorPropiedad3.Trim();
                            v3.pt = valorPropiedad3.Trim();
                            lv.Add(v3);
                        }
                        if (lv.Count > 0)
                            v.values = lv;
                    }
                    lvariant.Add(v);
                    p.variants = lvariant;
                    p.tags = string.IsNullOrEmpty(tags) ? null : tags.Trim();
                    requestProductsPost.AddJsonBody(p);
                    requestProductsPost.AddHeader("Content-Type", "application/json");
                    requestProductsPost.AddHeader("User-Agent", tiendaUserAgent);
                    requestProductsPost.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseProductsPost = clientProductsPost.Execute(requestProductsPost);

                    if (responseProductsPost.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsPost.Content);
                    }
                    else
                    {
                        //Se creó el producto en tiendaNube y recupero el ID.
                        rp = JsonConvert.DeserializeObject<ProductsResponse>(responseProductsPost.Content);
                    }

                    prodPadre.id = rp.id;
                    foreach (Variant va in rp.variants)
                    {
                        idVariante = va.id;
                    }

                    //Me fijo si tengo que cargar una imagen
                    if (!string.IsNullOrEmpty(imagen))
                    {
                        Images ri = new Images();
                        var clientImagesPost = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/images");
                        var requestImagesPost = new RestRequest(Method.POST);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        Images i = new Images();
                        i.position = 1;
                        i.product_id = prodPadre.id;
                        i.attachment = imagen;
                        i.filename = sku.Trim() + ".jpeg";
                        requestImagesPost.AddJsonBody(i);
                        requestImagesPost.AddHeader("Content-Type", "application/json");
                        requestImagesPost.AddHeader("User-Agent", tiendaUserAgent);
                        requestImagesPost.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseImagesPost = clientImagesPost.Execute(requestImagesPost);

                        if (responseImagesPost.StatusCode != HttpStatusCode.Created)
                        {
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseImagesPost.Content);
                        }
                        else
                        {
                            //Se creó la categoria en tiendaNube y recupero el ID.
                            ri = JsonConvert.DeserializeObject<Images>(responseImagesPost.Content);
                            idImagen = ri.id;
                        }

                        if (esVariante.Equals("SI"))
                        { //Es con variante, asigno la imagen a la variable.

                            //Modifico la variante para asignarle la imagen
                            Variant rv = new Variant();
                            var clientVariantPut = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/variants/" + idVariante.ToString());
                            var requestVariantPut = new RestRequest(Method.PUT);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            Variant var = new Variant();
                            var.image_id = idImagen;
                            var.position = 1;
                            requestVariantPut.AddJsonBody(var);
                            requestVariantPut.AddHeader("Content-Type", "application/json");
                            requestVariantPut.AddHeader("User-Agent", tiendaUserAgent);
                            requestVariantPut.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseVariantPut = clientVariantPut.Execute(requestVariantPut);

                            if (responseVariantPut.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseVariantPut.Content);
                            }
                            else
                            {
                                //Se creó la categoria en tiendaNube y recupero el ID.
                                rv = JsonConvert.DeserializeObject<Variant>(responseVariantPut.Content);
                            }
                        }
                    }
                }
                else //Es Variante
                {
                    List<ProductsResponse> lpr = new List<ProductsResponse>();
                    var clientProductsGet = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idNubePadre.Trim());
                    var requestProductsGet = new RestRequest(Method.GET);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    requestProductsGet.AddHeader("Content-Type", "application/json");
                    requestProductsGet.AddHeader("User-Agent", tiendaUserAgent);
                    requestProductsGet.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseProductsGet = clientProductsGet.Execute(requestProductsGet);
                    if (responseProductsGet.StatusCode != HttpStatusCode.OK)
                    {
                        if (responseProductsGet.StatusCode != HttpStatusCode.NotFound)
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsGet.Content);
                    }
                    else
                    {
                        //El producto existe, obtengo el id
                        prodPadre = JsonConvert.DeserializeObject<ProductsResponse>(responseProductsGet.Content);
                    }

                    bool existeVariante = false;
                    foreach (Variant v in prodPadre.variants)
                    {
                        if (v.sku == sku)
                        {
                            existeVariante = true;
                            break;
                        }
                    }

                    //Si la variante que agrego tiene propiedades analizo los atributos del producto.
                    bool actualizarAtributos = false;
                    List<Attributes> la = new List<Attributes>();
                    if (prodPadre.attributes.Count >= 1)
                    {
                        if (!prodPadre.attributes[0].es.Equals(nombrePropiedad1))
                        {
                            throw new Exception("ERROR al asignar atributos a las variantes: La propiedad 1 de la variante no corresponde a la propiedad 1 del producto (" + prodPadre.attributes[0].es + ").");
                        }
                        else
                        {
                            Attributes a1 = new Attributes();
                            a1.es = nombrePropiedad1.Trim();
                            a1.en = nombrePropiedad1.Trim();
                            a1.pt = nombrePropiedad1.Trim();
                            la.Add(a1);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(nombrePropiedad1) && !string.IsNullOrEmpty(valorPropiedad1))
                        {
                            actualizarAtributos = true;
                            Attributes a1 = new Attributes();
                            a1.es = nombrePropiedad1.Trim();
                            a1.en = nombrePropiedad1.Trim();
                            a1.pt = nombrePropiedad1.Trim();
                            la.Add(a1);
                        }
                    }
                    if (prodPadre.attributes.Count >= 2)
                    {
                        if (!prodPadre.attributes[1].es.Equals(nombrePropiedad2))
                        {
                            throw new Exception("ERROR al asignar atributos a las variantes: La propiedad 2 de la variante no corresponde a la propiedad 2 del producto (" + prodPadre.attributes[1].es + ").");
                        }
                        else
                        {
                            Attributes a2 = new Attributes();
                            a2.es = nombrePropiedad2.Trim();
                            a2.en = nombrePropiedad2.Trim();
                            a2.pt = nombrePropiedad2.Trim();
                            la.Add(a2);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(nombrePropiedad2) && !string.IsNullOrEmpty(valorPropiedad2))
                        {
                            actualizarAtributos = true;
                            Attributes a2 = new Attributes();
                            a2.es = nombrePropiedad2.Trim();
                            a2.en = nombrePropiedad2.Trim();
                            a2.pt = nombrePropiedad2.Trim();
                            la.Add(a2);
                        }
                    }
                    if (prodPadre.attributes.Count >= 3)
                    {
                        if (!prodPadre.attributes[2].es.Equals(nombrePropiedad3))
                        {
                            throw new Exception("ERROR al asignar atributos a las variantes: La propiedad 3 de la variante no corresponde a la propiedad 3 del producto (" + prodPadre.attributes[2].es + ").");
                        }
                        else
                        {
                            Attributes a3 = new Attributes();
                            a3.es = nombrePropiedad3.Trim();
                            a3.en = nombrePropiedad3.Trim();
                            a3.pt = nombrePropiedad3.Trim();
                            la.Add(a3);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(nombrePropiedad3) && !string.IsNullOrEmpty(valorPropiedad3))
                        {
                            actualizarAtributos = true;
                            Attributes a3 = new Attributes();
                            a3.es = nombrePropiedad3.Trim();
                            a3.en = nombrePropiedad3.Trim();
                            a3.pt = nombrePropiedad3.Trim();
                            la.Add(a3);
                        }
                    }

                    //Modifico el producto para actualizar los atributos.
                    if (actualizarAtributos)
                    {
                        ProductsResponse rp = new ProductsResponse();
                        var clientProductsPut = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString());
                        var requestProductsPut = new RestRequest(Method.PUT);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        ProductsAttributes pa = new ProductsAttributes();
                        pa.attributes = la;
                        requestProductsPut.AddJsonBody(pa);
                        requestProductsPut.AddHeader("Content-Type", "application/json");
                        requestProductsPut.AddHeader("User-Agent", tiendaUserAgent);
                        requestProductsPut.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseProductsPut = clientProductsPut.Execute(requestProductsPut);
                        if (responseProductsPut.StatusCode != HttpStatusCode.OK)
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsPut.Content);
                        else
                            rp = JsonConvert.DeserializeObject<ProductsResponse>(responseProductsPut.Content); //Se creó el producto en tiendaNube y recupero el ID.
                    }


                    if (!existeVariante) //Si no existe creo la variante
                    {
                        //Creo la variante
                        Variant rv = new Variant();
                        var clientVariantPost = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/variants");
                        var requestVariantPost = new RestRequest(Method.POST);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        Variant v = new Variant();
                        v.price = precio.Trim();
                        v.promotional_price = string.IsNullOrEmpty(precioPromocional) ? null : precioPromocional.Trim();
                        v.stock_management = true;
                        v.weight = string.IsNullOrEmpty(peso) ? null : peso.Trim();
                        v.sku = sku.Trim();
                        v.stock = stock;
                        v.position = 1;
                        List<Value> lv = new List<Value>();
                        if (!string.IsNullOrEmpty(nombrePropiedad1) && !string.IsNullOrEmpty(valorPropiedad1))
                        {
                            Value v1 = new Value();
                            v1.es = valorPropiedad1.Trim();
                            v1.en = valorPropiedad1.Trim();
                            v1.pt = valorPropiedad1.Trim();
                            lv.Add(v1);
                        }
                        if (!string.IsNullOrEmpty(nombrePropiedad2) && !string.IsNullOrEmpty(valorPropiedad2))
                        {
                            Value v2 = new Value();
                            v2.es = valorPropiedad2.Trim();
                            v2.en = valorPropiedad2.Trim();
                            v2.pt = valorPropiedad2.Trim();
                            lv.Add(v2);
                        }
                        if (!string.IsNullOrEmpty(nombrePropiedad3) && !string.IsNullOrEmpty(valorPropiedad3))
                        {
                            Value v3 = new Value();
                            v3.es = valorPropiedad3.Trim();
                            v3.en = valorPropiedad3.Trim();
                            v3.pt = valorPropiedad3.Trim();
                            lv.Add(v3);
                        }
                        if (lv.Count > 0)
                            v.values = lv;
                        requestVariantPost.AddJsonBody(v);
                        requestVariantPost.AddHeader("Content-Type", "application/json");
                        requestVariantPost.AddHeader("User-Agent", tiendaUserAgent);
                        requestVariantPost.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseVariantPost = clientVariantPost.Execute(requestVariantPost);

                        if (responseVariantPost.StatusCode != HttpStatusCode.Created)
                        {
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseVariantPost.Content);
                        }
                        else
                        {
                            //Se creó la categoria en tiendaNube y recupero el ID.
                            rv = JsonConvert.DeserializeObject<Variant>(responseVariantPost.Content);
                            idVariante = rv.id;
                        }

                        //Me fijo si tengo que cargar una imagen
                        if (!string.IsNullOrEmpty(imagen))
                        {
                            Images ri = new Images();
                            var clientImagesPost = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/images");
                            var requestImagesPost = new RestRequest(Method.POST);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            Images i = new Images();
                            //i.position = 1;
                            i.product_id = prodPadre.id;
                            i.attachment = imagen;
                            i.filename = sku.Trim() + ".jpeg";
                            requestImagesPost.AddJsonBody(i);
                            requestImagesPost.AddHeader("Content-Type", "application/json");
                            requestImagesPost.AddHeader("User-Agent", tiendaUserAgent);
                            requestImagesPost.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseImagesPost = clientImagesPost.Execute(requestImagesPost);

                            if (responseImagesPost.StatusCode != HttpStatusCode.Created)
                            {
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseImagesPost.Content);
                            }
                            else
                            {
                                //Se creó la categoria en tiendaNube y recupero el ID.
                                ri = JsonConvert.DeserializeObject<Images>(responseImagesPost.Content);
                                idImagen = ri.id;
                            }

                            //Modifico la variante para asignarle la imagen
                            Variant rva = new Variant();
                            var clientVariantPut = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/variants/" + idVariante.ToString());
                            var requestVariantPut = new RestRequest(Method.PUT);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            Variant var = new Variant();
                            var.image_id = idImagen;
                            var.position = 1;
                            requestVariantPut.AddJsonBody(var);
                            requestVariantPut.AddHeader("Content-Type", "application/json");
                            requestVariantPut.AddHeader("User-Agent", tiendaUserAgent);
                            requestVariantPut.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseVariantPut = clientVariantPut.Execute(requestVariantPut);

                            if (responseVariantPut.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseVariantPut.Content);
                            }
                            else
                            {
                                //Se creó la categoria en tiendaNube y recupero el ID.
                                rva = JsonConvert.DeserializeObject<Variant>(responseVariantPut.Content);
                            }
                        }

                        prodPadre.id = idVariante;

                    }
                    else
                        throw new Exception("ERROR tiendaNube: Ya existe la variante.");

                }

                System.Threading.Thread.Sleep(1000);
                return prodPadre.id;
               
            }
            catch (Exception ex)
            {
                string msj = "";
                //Si dio error, me fijo si llego a crear el producto o variante e intento eliminarla.
                try
                {
                    bool resultado = false;

                    if (esVariante.Equals("NO")) 
                    {
                        if(prodPadre.id != 0) //Significa que se logro crear el producto en tiendaNube, intento eliminarlo
                        {
                            resultado = EliminarProducto(tiendaCodigo, tiendaToken, tiendaUserAgent, prodPadre.id.ToString(), prodPadre.id.ToString(), usu);
                        }
                    }
                    else
                    {
                        if (idVariante != 0) //Significa que se logro crear la variante en tiendaNube, intento eliminarla
                        {
                            resultado = EliminarProducto(tiendaCodigo, tiendaToken, tiendaUserAgent, idNubePadre, idVariante.ToString(), usu);
                        }
                    }

                    if (resultado)
                        msj = " - Se logró revertir la creación del producto/variante, intente nuevamente.";

                }
                catch (Exception exc)
                {
                    if (esVariante.Equals("NO"))
                        msj = " - Error al intentar deshacer la creación del producto: " + prodPadre.id.ToString() + " - " + exc.Message;
                    else
                        msj = " - Error al intentar deshacer la creación de la variante: " + idVariante.ToString() + " - " + exc.Message;
                }                

                throw new Exception(ex.Message + msj);
            }
        }

        public static bool EliminarProducto(string tiendaCodigo, string tiendaToken, string tiendaUserAgent, string idProducto, string idVariante, WebUser usu)
        {
            try
            {
                string urlTiendaNube = ConfigurationManager.AppSettings["TiendaNube.Url"];

                if (idProducto.Equals(idVariante))
                {
                    //Elimino el producto por el id
                    var clientProductsDelete = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idProducto.Trim());
                    var requestProductsDelete = new RestRequest(Method.DELETE);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    requestProductsDelete.AddHeader("Content-Type", "application/json");
                    requestProductsDelete.AddHeader("User-Agent", tiendaUserAgent);
                    requestProductsDelete.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseProductsDelete = clientProductsDelete.Execute(requestProductsDelete);
                    if (responseProductsDelete.StatusCode != HttpStatusCode.OK)
                    {
                        if (responseProductsDelete.StatusCode == HttpStatusCode.NotFound)
                            return false;
                        else
                            throw new Exception(responseProductsDelete.StatusCode.ToString());
                    }
                    else
                    {
                        //Se elimino correctamente el producto en tienda nube
                        return true;
                    }
                }
                else
                {
                    //Elimino la variante por el id
                    var clientVariantsDelete = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idProducto.Trim() + "/variants/" + idVariante.Trim());
                    var requestVariantsDelete = new RestRequest(Method.DELETE);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    requestVariantsDelete.AddHeader("Content-Type", "application/json");
                    requestVariantsDelete.AddHeader("User-Agent", tiendaUserAgent);
                    requestVariantsDelete.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseVariantsDelete = clientVariantsDelete.Execute(requestVariantsDelete);
                    if (responseVariantsDelete.StatusCode != HttpStatusCode.OK)
                    {
                        if (responseVariantsDelete.StatusCode == HttpStatusCode.NotFound)
                            return false;
                        else
                            throw new Exception(responseVariantsDelete.StatusCode.ToString());
                    }
                    else
                    {
                        //Se elimino correctamente el producto en tienda nube
                        return true;
                    }
                }

            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static ProductoTiendaNubeResponse ConsultarProducto(string tiendaCodigo, string tiendaToken, string tiendaUserAgent, string idProducto, string idVariante, WebUser usu)
        {
            try
            {
                string urlTiendaNube = ConfigurationManager.AppSettings["TiendaNube.Url"];

                ProductsResponse prod = new ProductsResponse();

                //Consulto el producto por el id
                var clientProductsGet = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idProducto.Trim());
                var requestProductsGet = new RestRequest(Method.GET);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                requestProductsGet.AddHeader("Content-Type", "application/json");
                requestProductsGet.AddHeader("User-Agent", tiendaUserAgent);
                requestProductsGet.AddHeader("Authentication", tiendaToken);
                IRestResponse responseProductsGet = clientProductsGet.Execute(requestProductsGet);
                if (responseProductsGet.StatusCode != HttpStatusCode.OK)
                {
                    if (responseProductsGet.StatusCode == HttpStatusCode.NotFound)
                        return null;
                    else
                        throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsGet.Content);
                }                       
                else
                    prod = JsonConvert.DeserializeObject<ProductsResponse>(responseProductsGet.Content);


                if (idProducto.Equals(idVariante))
                {
                    //Mapeo el resultado de getProducts de TiendaNube a un response para ELUM.
                    ProductoTiendaNubeResponse p = new ProductoTiendaNubeResponse();
                    p.nombre = prod.name.es;
                    p.sku = prod.sku;
                    p.mostrarEnTienda = prod.published ? "SI" : "NO";
                    p.enviaSinCargo = prod.free_shipping ? "SI" : "NO";
                    p.descripcion = prod.description.es;
                    p.tags = prod.tags;
                    p.tituloParaSEO = prod.seo_title.es;
                    p.descripcionParaSEO = prod.seo_description.es;
                    p.esVariante = "NO";
                    p.marca = prod.brand;
                    p.IdNubePadre = prod.id.ToString();

                    return p;                    

                }
                else
                {
                    foreach (Variant v in prod.variants)
                    {
                        if (v.id == Convert.ToInt64(idVariante))
                        {
                            //Mapeo el resultado de getProducts de TiendaNube a un response para ELUM.
                            ProductoTiendaNubeResponse p = new ProductoTiendaNubeResponse();
                            p.nombre = prod.name.es;
                            if (prod.attributes.Count >= 1)
                            {
                                if (prod.variants[0].values.Count >= 1)
                                {
                                    p.nombrePropiedad1 = prod.attributes[0].es;
                                    p.valorPropiedad1 = prod.variants[0].values[0].es;
                                }
                            }
                            if (prod.attributes.Count >= 2)
                            {
                                if (prod.variants[0].values.Count >= 2)
                                {
                                    p.nombrePropiedad2 = prod.attributes[1].es;
                                    p.valorPropiedad2 = prod.variants[0].values[1].es;
                                }
                            }
                            if (prod.attributes.Count >= 3)
                            {
                                if (prod.variants[0].values.Count >= 3)
                                {
                                    p.nombrePropiedad3 = prod.attributes[2].es;
                                    p.valorPropiedad3 = prod.variants[0].values[2].es;
                                }
                            }

                            p.precio = prod.variants[0].price;
                            p.precioPromocional = prod.variants[0].promotional_price;
                            p.peso = prod.variants[0].weight;
                            p.stock = prod.variants[0].stock.ToString();
                            p.sku = prod.variants[0].sku;
                            p.codigoDeBarras = prod.variants[0].barcode;
                            p.mostrarEnTienda = prod.published ? "SI" : "NO";
                            p.enviaSinCargo = prod.free_shipping ? "SI" : "NO";
                            p.descripcion = prod.description.es;
                            p.tags = prod.tags;
                            p.tituloParaSEO = prod.seo_title.es;
                            p.descripcionParaSEO = prod.seo_description.es;
                            p.esVariante = "SI";
                            p.marca = prod.brand;
                            p.IdNubePadre = prod.id.ToString();

                            return p;
                        }
                    }

                    return null;

                }

            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool ModificarProducto(string tiendaCodigo, string tiendaToken, string tiendaUserAgent, string idNubePadre, string sku,
                                             string precio, string precioPromocional, int stock, string mostrarEnTienda, string enviaSinCargo,
                                             string imagen, WebUser usu)
        {
            try
            {
                int idVariante = 0;
                int idImagen = 0;
                ProductsResponse prodPadre = new ProductsResponse();

                string urlTiendaNube = ConfigurationManager.AppSettings["TiendaNube.Url"];    
                
                //Me fijo si tengo que modificar el producto
                if (!string.IsNullOrEmpty(mostrarEnTienda) || !string.IsNullOrEmpty(enviaSinCargo))
                {
                    ProductsResponse rp = new ProductsResponse();
                    var clientProductsPut = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idNubePadre.Trim());
                    var requestProductsPut = new RestRequest(Method.PUT);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    bodyProductsPut b = new bodyProductsPut();
                    b.id = Convert.ToInt32(idNubePadre);
                    if(!string.IsNullOrEmpty(mostrarEnTienda))
                        b.published = (mostrarEnTienda == "SI") ? true : false;
                    if (!string.IsNullOrEmpty(enviaSinCargo))
                        b.free_shipping = (enviaSinCargo == "SI") ? true : false;
                    requestProductsPut.AddJsonBody(b);
                    requestProductsPut.AddHeader("Content-Type", "application/json");
                    requestProductsPut.AddHeader("User-Agent", tiendaUserAgent);
                    requestProductsPut.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseProductsPut = clientProductsPut.Execute(requestProductsPut);
                    if (responseProductsPut.StatusCode != HttpStatusCode.OK)
                        throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsPut.Content);
                }

                //Me fijo si tengo que modificar la variante
                if (!string.IsNullOrEmpty(precio) || !string.IsNullOrEmpty(precioPromocional) || stock != -1 || !string.IsNullOrEmpty(imagen))
                {
                    var clientProductsGet = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + idNubePadre.Trim());
                    var requestProductsGet = new RestRequest(Method.GET);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    requestProductsGet.AddHeader("Content-Type", "application/json");
                    requestProductsGet.AddHeader("User-Agent", tiendaUserAgent);
                    requestProductsGet.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseProductsGet = clientProductsGet.Execute(requestProductsGet);
                    if (responseProductsGet.StatusCode != HttpStatusCode.OK)
                    {
                        if (responseProductsGet.StatusCode != HttpStatusCode.NotFound)
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseProductsGet.Content);
                    }                
                    else
                        prodPadre = JsonConvert.DeserializeObject<ProductsResponse>(responseProductsGet.Content);

                    foreach (Variant v in prodPadre.variants)
                    {
                        if (v.sku == sku)
                        {
                            idVariante = v.id;
                            break;
                        }
                    }

                    if (idVariante != 0) //Si existe la modifico
                    {
                        if (!string.IsNullOrEmpty(precio) || !string.IsNullOrEmpty(precioPromocional) || stock != -1)
                        {
                            //Modifico la variante
                            Variant rv = new Variant();
                            var clientVariantPut = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/variants/" + idVariante.ToString());
                            var requestVariantPut = new RestRequest(Method.PUT);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            bodyVariantsPut b = new bodyVariantsPut();
                            if (!string.IsNullOrEmpty(precio))
                                b.price = precio;
                            if (!string.IsNullOrEmpty(precioPromocional))
                                b.promotional_price = precioPromocional;
                            if (stock != -1)
                                b.stock = stock;
                            requestVariantPut.AddJsonBody(b);
                            requestVariantPut.AddHeader("Content-Type", "application/json");
                            requestVariantPut.AddHeader("User-Agent", tiendaUserAgent);
                            requestVariantPut.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseVariantPut = clientVariantPut.Execute(requestVariantPut);
                            if (responseVariantPut.StatusCode != HttpStatusCode.OK)
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseVariantPut.Content);
                        }                        

                        //Me fijo si tengo que cargar una imagen
                        if (!string.IsNullOrEmpty(imagen))
                        {
                            //Me fijo si tiene imagen y la elimino
                            foreach (Variant v in prodPadre.variants)
                            {
                                if (v.sku == sku)
                                {
                                    if (v.image_id != null)
                                    {
                                        //Elimino la foto
                                        var clientImagenDelete = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/images/" + v.image_id.ToString());
                                        var requestImagenDelete = new RestRequest(Method.DELETE);
                                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                                        requestImagenDelete.AddHeader("Content-Type", "application/json");
                                        requestImagenDelete.AddHeader("User-Agent", tiendaUserAgent);
                                        requestImagenDelete.AddHeader("Authentication", tiendaToken);
                                        IRestResponse responseImagenDelete = clientImagenDelete.Execute(requestImagenDelete);
                                        if (responseImagenDelete.StatusCode != HttpStatusCode.OK)
                                        {
                                            if (responseImagenDelete.StatusCode == HttpStatusCode.NotFound)
                                                return false;
                                            else
                                                throw new Exception(responseImagenDelete.StatusCode.ToString());
                                        }
                                    }
                                    break;
                                }
                            }

                            //Subo la imagen
                            Images ri = new Images();
                            var clientImagesPost = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/images");
                            var requestImagesPost = new RestRequest(Method.POST);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            Images i = new Images();
                            //i.position = 1;
                            i.product_id = prodPadre.id;
                            i.attachment = imagen;
                            i.filename = sku.Trim() + ".jpeg";
                            requestImagesPost.AddJsonBody(i);
                            requestImagesPost.AddHeader("Content-Type", "application/json");
                            requestImagesPost.AddHeader("User-Agent", tiendaUserAgent);
                            requestImagesPost.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseImagesPost = clientImagesPost.Execute(requestImagesPost);

                            if (responseImagesPost.StatusCode != HttpStatusCode.Created)
                            {
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseImagesPost.Content);
                            }
                            else
                            {
                                //Se creó la categoria en tiendaNube y recupero el ID.
                                ri = JsonConvert.DeserializeObject<Images>(responseImagesPost.Content);
                                idImagen = ri.id;
                            }                        

                            //Modifico la variante para asignarle la imagen
                            Variant rva = new Variant();
                            var clientVariantImagenPut = new RestClient(urlTiendaNube + tiendaCodigo + "/products/" + prodPadre.id.ToString() + "/variants/" + idVariante.ToString());
                            var requestVariantImagenPut = new RestRequest(Method.PUT);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            Variant var = new Variant();
                            var.image_id = idImagen;
                            var.position = 1;
                            requestVariantImagenPut.AddJsonBody(var);
                            requestVariantImagenPut.AddHeader("Content-Type", "application/json");
                            requestVariantImagenPut.AddHeader("User-Agent", tiendaUserAgent);
                            requestVariantImagenPut.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseVariantImagenPut = clientVariantImagenPut.Execute(requestVariantImagenPut);
                            if (responseVariantImagenPut.StatusCode != HttpStatusCode.OK)
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseVariantImagenPut.Content);                    
                        }
                    }
                    else
                        return false;
                }

                return true;

            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<ResponseGetOrdersElum> ObtenerOrdenes(string tiendaCodigo, string tiendaToken, string tiendaUserAgent, int desdeIdOrden, int hastaIdOrden, string estado, WebUser usu)
        {
            try
            {
                string urlTiendaNube = ConfigurationManager.AppSettings["TiendaNube.Url"];

                List<ResponseGetOrders> lo = new List<ResponseGetOrders>();
                if (desdeIdOrden.Equals(hastaIdOrden)) //Se esta solicitanto una orden en especial.
                {
                    ResponseGetOrders o = null;
                    var clientOrderGet = new RestClient(urlTiendaNube + tiendaCodigo + "/orders/" + desdeIdOrden.ToString());
                    var requestOrderGet = new RestRequest(Method.GET);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    requestOrderGet.AddHeader("Content-Type", "application/json");
                    requestOrderGet.AddHeader("User-Agent", tiendaUserAgent);
                    requestOrderGet.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseOrderGet = clientOrderGet.Execute(requestOrderGet);
                    if (responseOrderGet.StatusCode != HttpStatusCode.OK)
                    {
                        if (responseOrderGet.StatusCode == HttpStatusCode.NotFound)
                            throw new Exception("ERROR al acceder a tiendaNube: No se encontraron ordenes.");
                        else
                            throw new Exception("ERROR al acceder a tiendaNube: " + responseOrderGet.Content);
                    }
                    else
                        o = JsonConvert.DeserializeObject<ResponseGetOrders>(responseOrderGet.Content);

                    lo.Add(o);

                    if (!estado.Equals("any")) //Si es diferentes a TODOS quitar los demas casos.                    
                        lo.RemoveAll(r => !r.status.ToUpper().Equals(estado.ToUpper()));                    

                }
                else //Se esta solicitanto desde una orden para adelante
                {
                    if (desdeIdOrden > 0 && !estado.Equals("any")) //Si se esta solicitando desde un id y el estado es distinto a todos, busco la fecha y traigo los anteriores
                    {
                        ResponseGetOrders os = null;
                        var clientOrderSpecificGet = new RestClient(urlTiendaNube + tiendaCodigo + "/orders/" + desdeIdOrden.ToString());
                        var requestOrderSpecificGet = new RestRequest(Method.GET);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        requestOrderSpecificGet.AddHeader("Content-Type", "application/json");
                        requestOrderSpecificGet.AddHeader("User-Agent", tiendaUserAgent);
                        requestOrderSpecificGet.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseOrderSpecificGet = clientOrderSpecificGet.Execute(requestOrderSpecificGet);
                        if (responseOrderSpecificGet.StatusCode != HttpStatusCode.OK)
                        {
                            if (responseOrderSpecificGet.StatusCode == HttpStatusCode.NotFound)
                                throw new Exception("ERROR al acceder a tiendaNube: No se encontraron ordenes.");
                            else
                                throw new Exception("ERROR al acceder a tiendaNube: " + responseOrderSpecificGet.Content);
                        }
                        else
                            os = JsonConvert.DeserializeObject<ResponseGetOrders>(responseOrderSpecificGet.Content);

                        DateTime fechaDeReferencia = new DateTime(1900,1,1,1,1,1);
                        switch (os.status)
                        {
                            case "closed":
                                fechaDeReferencia = Convert.ToDateTime(os.closed_at);
                                break;
                            case "cancelled":
                                fechaDeReferencia = Convert.ToDateTime(os.cancelled_at);
                                break;
                            default:
                                break;
                        }

                        List<ResponseGetOrders> lot = new List<ResponseGetOrders>();
                        bool paginadoOrden = true;
                        int paginaOrden = 1;
                        while (paginadoOrden)
                        {
                            List<ResponseGetOrders> o = null;
                            //Busco ordenes desde el idOrden que me llegue por parametro
                            var clientOrderGet = new RestClient(urlTiendaNube + tiendaCodigo + "/orders?page=" + paginaOrden.ToString() + "&status=" + estado);
                            var requestOrderGet = new RestRequest(Method.GET);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            requestOrderGet.AddHeader("Content-Type", "application/json");
                            requestOrderGet.AddHeader("User-Agent", tiendaUserAgent);
                            requestOrderGet.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseOrderGet = clientOrderGet.Execute(requestOrderGet);
                            if (responseOrderGet.StatusCode != HttpStatusCode.OK)
                            {
                                if (responseOrderGet.StatusCode == HttpStatusCode.NotFound)
                                    throw new Exception("ERROR al acceder a tiendaNube: No se encontraron ordenes.");
                                else
                                    throw new Exception("ERROR al acceder a tiendaNube: " + responseOrderGet.Content);
                            }
                            else
                                o = JsonConvert.DeserializeObject<List<ResponseGetOrders>>(responseOrderGet.Content);

                            if (o != null)
                            {
                                lot.AddRange(o);
                                if (o.Count < 30)
                                    paginadoOrden = false;
                                else
                                    paginaOrden++;
                            }
                            else
                            {
                                paginadoOrden = false;
                            }
                        }

                        lo = lot.Clone();
                        foreach(ResponseGetOrders rg in lot){
                            switch (estado)
                            {
                                case "closed":
                                    DateTime fechaClosed = Convert.ToDateTime(rg.closed_at);
                                    if (DateTime.Compare(fechaDeReferencia, fechaClosed) >= 0)
                                        lo.RemoveAll(r => r.id == rg.id);
                                    break;
                                case "cancelled":
                                    DateTime fechaCancelled = Convert.ToDateTime(rg.cancelled_at);
                                    if (DateTime.Compare(fechaDeReferencia, fechaCancelled) >= 0)
                                        lo.RemoveAll(r => r.id == rg.id);
                                    break;
                                default:
                                    break;
                            }
                        }                       

                        if (hastaIdOrden != -1) //Se esta solicitanto un rango de ordenes.
                        {
                            int t = Convert.ToInt32(hastaIdOrden);
                            lo.RemoveAll(r => r.id > t);
                        }
                    }
                    else
                    {
                        bool paginadoOrden = true;
                        int paginaOrden = 1;
                        while (paginadoOrden)
                        {
                            List<ResponseGetOrders> o = null;
                            //Busco ordenes desde el idOrden que me llegue por parametro
                            var clientOrderGet = new RestClient(urlTiendaNube + tiendaCodigo + "/orders?page=" + paginaOrden.ToString() + "&since_id=" + desdeIdOrden.ToString() + "&status=" + estado);
                            var requestOrderGet = new RestRequest(Method.GET);
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            requestOrderGet.AddHeader("Content-Type", "application/json");
                            requestOrderGet.AddHeader("User-Agent", tiendaUserAgent);
                            requestOrderGet.AddHeader("Authentication", tiendaToken);
                            IRestResponse responseOrderGet = clientOrderGet.Execute(requestOrderGet);
                            if (responseOrderGet.StatusCode != HttpStatusCode.OK)
                            {
                                if (responseOrderGet.StatusCode == HttpStatusCode.NotFound)
                                    throw new Exception("ERROR al acceder a tiendaNube: No se encontraron ordenes.");
                                else
                                    throw new Exception("ERROR al acceder a tiendaNube: " + responseOrderGet.Content);
                            }
                            else
                                o = JsonConvert.DeserializeObject<List<ResponseGetOrders>>(responseOrderGet.Content);

                            if (o != null)
                            {
                                lo.AddRange(o);
                                if (o.Count < 30)
                                    paginadoOrden = false;
                                else
                                    paginaOrden++;
                            }
                            else
                            {
                                paginadoOrden = false;
                            }
                        }
                        if (hastaIdOrden != -1) //Se esta solicitanto un rango de ordenes.
                        {
                            int t = Convert.ToInt32(hastaIdOrden);
                            lo.RemoveAll(r => r.id > t);
                        }
                    }                    
                }

                //Mapeo el resultado de getOrdenes de TiendaNube a un response para ELUM.
                List<ResponseGetOrdersElum> loe = new List<ResponseGetOrdersElum>();
                foreach (ResponseGetOrders l in lo)
                {
                    ResponseGetOrdersElum e = new ResponseGetOrdersElum();
                    e.idOrden = l.id.ToString();
                    e.subtotal = l.subtotal;
                    e.descuento = l.discount;
                    e.descuentoCupon = l.discount_coupon;
                    e.descuentoGateway = l.discount_gateway;
                    e.total = l.total;
                    e.fechaCreacion = l.created_at;
                    e.fechaModificacion = l.updated_at;
                    e.razonDeCancelacion = l.cancel_reason;
                    e.fechaCancelacion = l.cancelled_at;
                    e.fechaCerrado = l.closed_at;

                    switch (l.status)
                    {
                        case "open":
                            e.estado = "ABIERTO";
                            break;
                        case "closed":
                            e.estado = "CERRADO";
                            break;
                        case "cancelled":
                            e.estado = "CANCELADO";
                            break;
                        default:
                            e.estado = "OTRO";
                            break;
                    }
                    switch (l.payment_status)
                    {
                        case "pending":
                            e.estadoDePago = "PENDIENTE";
                            break;
                        case "authorized":
                            e.estadoDePago = "AUTORIZADO";
                            break;
                        case "paid":
                            e.estadoDePago = "PAGADO";
                            break;
                        case "voided":
                            e.estadoDePago = "ANULADO";
                            break;
                        case "refunded":
                            e.estadoDePago = "RECHAZADO";
                            break;
                        case "abandoned":
                            e.estadoDePago = "ABANDONADO";
                            break;
                        default:
                            e.estadoDePago = "SIN DATOS";
                            break;
                    }  
                    switch (l.shipping_status)
                    {
                        case "unpacked":
                            e.estadoDeEnvio = "DESEMPAQUETADO";
                            break;
                        case "unshipped":
                            e.estadoDeEnvio = "NO ENVIADO";
                            break;
                        case "shipped":
                            e.estadoDeEnvio = "ENVIADO";
                            break;
                        default:
                            e.estadoDeEnvio = "OTRO";
                            break;
                    }

                    e.nota = l.note;
                    e.opcionDeEnvio = l.shipping_option;
                    e.FechaEnvio = l.shipped_at;
                    e.fechaPago = l.paid_at;

                    List<Producto> lp = new List<Producto>();    

                    foreach(Product p in l.products)
                    {
                        Producto ep = new Producto();
                        ep.idProducto = p.product_id.ToString();
                        ep.idVariante = p.variant_id;
                        ep.nombre = p.name;
                        ep.precio = p.price;
                        ep.cantidad = p.quantity;
                        ep.sku = p.sku;
                        lp.Add(ep);
                    }

                    e.productos = lp;

                    if(l.customer != null)
                    {
                        Cliente c = new Cliente();
                        c.id = l.customer.id.ToString();
                        c.nombre = l.customer.name;
                        c.email = l.customer.email;
                        c.identificacion = l.customer.identification;
                        c.telefono = l.customer.phone;
                        c.domicilioDireccion = l.customer.billing_address;
                        c.domicilioNumero = l.customer.billing_number;
                        c.domicilioPiso = l.customer.billing_floor;
                        c.domicilioLocalidad = l.customer.billing_locality;
                        c.domicilioCodigoPostal = l.customer.billing_zipcode;
                        c.domicilioCiudad = l.customer.billing_city;
                        c.domicilioProvincia = l.customer.billing_province;
                        c.quienRetira = l.shipping_address.name;
                        e.cliente = c;
                    }

                    loe.Add(e);
                }

                return loe;
            
            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool ModificarEstadoOrden(string tiendaCodigo, string tiendaToken, string tiendaUserAgent, int idOrden, string estado, string motivo, WebUser usu)
        {
            try
            {
                string urlTiendaNube = ConfigurationManager.AppSettings["TiendaNube.Url"];

                if (estado.Equals("close"))
                {
                    //Paso la orden a pagado y enviado
                    var clientOrdenPagadoPut = new RestClient(urlTiendaNube + tiendaCodigo + "/orders/" + idOrden.ToString());
                    var requestOrdenPagadoPut = new RestRequest(Method.PUT);
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    requestOrdenPagadoPut.AddJsonBody(
                        new
                        {
                            status = "paid"
                        }
                    );
                    requestOrdenPagadoPut.AddHeader("Content-Type", "application/json");
                    requestOrdenPagadoPut.AddHeader("User-Agent", tiendaUserAgent);
                    requestOrdenPagadoPut.AddHeader("Authentication", tiendaToken);
                    IRestResponse responseOrdenPagadoPut = clientOrdenPagadoPut.Execute(requestOrdenPagadoPut);
                    if (responseOrdenPagadoPut.StatusCode != HttpStatusCode.OK)
                    {
                        if (responseOrdenPagadoPut.StatusCode == HttpStatusCode.NotFound)
                            return false;
                        else
                            throw new Exception(responseOrdenPagadoPut.StatusCode.ToString());
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(500);
                        //Paso la orden a pagado y enviado
                        var clientOrdenEnvidadoPost = new RestClient(urlTiendaNube + tiendaCodigo + "/orders/" + idOrden.ToString() + "/fulfill");
                        var requestOrdenEnvidadoPost = new RestRequest(Method.POST);
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        requestOrdenEnvidadoPost.AddHeader("Content-Type", "application/json");
                        requestOrdenEnvidadoPost.AddHeader("User-Agent", tiendaUserAgent);
                        requestOrdenEnvidadoPost.AddHeader("Authentication", tiendaToken);
                        IRestResponse responseOrdenEnvidadoPost = clientOrdenEnvidadoPost.Execute(requestOrdenEnvidadoPost);
                        if (responseOrdenEnvidadoPost.StatusCode != HttpStatusCode.OK)
                        {
                            if (responseOrdenEnvidadoPost.StatusCode == HttpStatusCode.NotFound)
                                return false;
                            else
                                throw new Exception(responseOrdenEnvidadoPost.StatusCode.ToString());
                        }
                    }
                }

                System.Threading.Thread.Sleep(500);
                var clientOrdenPost = new RestClient(urlTiendaNube + tiendaCodigo + "/orders/" + idOrden.ToString() + "/" + estado.Trim());
                var requestOrdenPost = new RestRequest(Method.POST);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                if (estado.Equals("cancel"))
                {
                    requestOrdenPost.AddJsonBody(
                        new
                        {
                            reason = motivo
                        }
                    );
                }
                requestOrdenPost.AddHeader("Content-Type", "application/json");
                requestOrdenPost.AddHeader("User-Agent", tiendaUserAgent);
                requestOrdenPost.AddHeader("Authentication", tiendaToken);
                IRestResponse responseOrdenPost = clientOrdenPost.Execute(requestOrdenPost);
                if (responseOrdenPost.StatusCode != HttpStatusCode.OK)
                {
                    if (responseOrdenPost.StatusCode == HttpStatusCode.NotFound)
                        return false;
                    else
                        throw new Exception(responseOrdenPost.StatusCode.ToString());
                }

                return true;

            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public class bodyProductsPut
        {
            public int id { get; set; }
            public bool? published { get; set; }
            public bool? free_shipping { get; set; }
        }
        public class bodyVariantsPut
        {
            public int id { get; set; }
            public int? stock { get; set; }
            public string price { get; set; }
            public string promotional_price { get; set; }
        }

        public static ResultadosProductosViewModel ObtenerProducto(string condicion, int page, int pageSize, WebUser usu)
        {
            using (var dbContext = new ACHEEntities())
            {
                var results = dbContext.Conceptos.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                if (!string.IsNullOrEmpty(condicion))
                    results = results.Where(x => x.Codigo.ToLower().Contains(condicion.ToLower()) || x.Nombre.ToLower().Contains(condicion.ToLower()));

                page--;
                ResultadosProductosViewModel resultado = new ResultadosProductosViewModel();
                resultado.TotalPage = ((results.Count() - 1) / pageSize) + 1;
                resultado.TotalItems = results.Count();

                var list = results.OrderBy(x => x.Nombre).Skip(page * pageSize).Take(pageSize).ToList()
                    .Select(x => new ConceptosViewModel()
                    {
                        ID = x.IDConcepto,
                        Tipo = x.Tipo == "S" ? "Servicio" : "Producto",
                        Nombre = x.Nombre.ToUpper(),
                        Codigo = x.Codigo.ToUpper(),
                        Descripcion = x.Descripcion,
                        Estado = x.Estado == "A" ? "Activo" : "Inactivo",
                        Precio = x.PrecioUnitario.ToString("N2"),
                        CostoInterno = x.CostoInterno.ToString(),
                        Iva = x.Iva.ToString("#0.00"),
                        Stock = x.Tipo == "S" ? "" : x.Stock.ToString()
                    });
                resultado.Items = list.ToList();

                return resultado;
            }
        }

        #endregion
        public static void RestarStock(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalle)
        {
            foreach (var item in comprobanteDetalle)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                    c.Stock = (c.Stock - item.Cantidad);
            }
        }
        public static void SumarStock(ACHEEntities dbContext, List<ComprobantesDetalle> comprobanteDetalle)
        {
            foreach (var item in comprobanteDetalle)
            {
                var c = dbContext.Conceptos.Where(x => x.IDConcepto == item.IDConcepto && x.Tipo == "P").FirstOrDefault();
                if (c != null)
                    c.Stock = (c.Stock + item.Cantidad);
            }
        }
        public static decimal ObtenerPrecioFinal(decimal precioUnitario, string iva)
        {
            decimal precioFinal = precioUnitario;
            var auxIVA = "1";

            switch (iva)
            {
                case "0,00":
                    auxIVA = "1";
                    break;
                case "2,50":
                    auxIVA = "1,025";
                    break;
                case "5,00":
                    auxIVA = "1,050";
                    break;
                case "10,50":
                    auxIVA = "1,105";
                    break;
                case "21,00":
                    auxIVA = "1,210";
                    break;
                case "27,00":
                    auxIVA = "1,270";
                    break;
            }

            decimal IVA = decimal.Parse(auxIVA);
            precioFinal = precioUnitario / IVA;

            return Math.Round(precioFinal, 2);
        }

        


    }
}
