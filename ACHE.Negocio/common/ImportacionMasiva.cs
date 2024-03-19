using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACHE.Extensions;
using System.Text.RegularExpressions;
using FileHelpers;
using System.Data;
using System.Data.SqlClient;
using ACHE.Negocio.Contabilidad;
using ACHE.Model.Negocio;
using ACHE.Negocio.Facturacion;
using System.Configuration;
using ACHE.Negocio.Common;
using Org.BouncyCastle.Crypto;
using ACHE.FacturaElectronica;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ImportacionMasiva
/// </summary>
public static class ImportacionMasiva
{
    public const string formatoFecha = "dd/MM/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ".";//"."
    public const string SeparadorDeDecimales = ",";//","

    #region PRODUCTOS
    public static List<ProductosCSVTmp> LeerArchivoCSVProductos(string tipo, int idUsuario, string path)
    {
        List<ProductosCSVTmp> listaproductosCSV = new List<ProductosCSVTmp>();
        FileHelperEngine engine = new FileHelperEngine(typeof(ProductosCSV));
        engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
        // to Read use:
        ProductosCSV[] res = (ProductosCSV[])engine.ReadFile(path);

        ProductosCSVTmp prodCSV;
        foreach (var productos in res)
        {
            prodCSV = new ProductosCSVTmp();
            prodCSV.IDUsuario = idUsuario;
            prodCSV.nombre = productos.nombre.ToUpper().Trim();
            prodCSV.descripcion = productos.descripcion.Trim();
            prodCSV.codigo = productos.codigo.ToUpper().Trim();
            prodCSV.observaciones = productos.observaciones.Trim();
            prodCSV.precioUnitario = productos.precioUnitario;
            prodCSV.stock = productos.stock;
            prodCSV.iva = productos.iva.Replace(SeparadorDeMiles, SeparadorDeDecimales);
            prodCSV.tipo = (tipo == "Productos") ? "P" : "S";
            prodCSV.fechaAlta = DateTime.Now;
            prodCSV.CostoInterno = productos.CostoInterno;
            prodCSV.StockMinimo = (prodCSV.tipo == "P") ? productos.StockMinimo : "0";
            prodCSV.CodigoProveedor = productos.CodigoProveedor;
            prodCSV.resultados = ImportacionMasiva.ValidarProducto(prodCSV);

            if (listaproductosCSV.Any(x => x.codigo == productos.codigo))
                prodCSV.resultados = "El Código se encuentra repetido en la lista, ";

            prodCSV.Estado = prodCSV.resultados == string.Empty ? "A" : "I";

            if (prodCSV.resultados == string.Empty)
                prodCSV.resultados = "<span class='label label-success'>OK</span>";
            else
                prodCSV.resultados = "<span class='label label-danger' data-toggle='tooltip' title='" + prodCSV.resultados.Substring(0, prodCSV.resultados.Length - 2) + ".'>ERROR. Ver detalle</span>";

            listaproductosCSV.Add(prodCSV);
        }

        return listaproductosCSV.ToList();
    }
    public static string ValidarProducto(ProductosCSVTmp productos)
    {
        string resultado = string.Empty;

        if (string.IsNullOrEmpty(productos.nombre))
            resultado += "El campo Nombre es obligatorio , ";

        //if (string.IsNullOrEmpty(productos.descripcion))
        //    resultado += "El campo Descripción es obligatorio, ";

        if (string.IsNullOrEmpty(productos.codigo))
            resultado += "El campo Código es obligatorio, ";

        if (string.IsNullOrEmpty(productos.iva))
            resultado += "El campo Iva es obligatorio, ";

        if (productos.nombre.Length > 100)
            resultado += "El campo Nombre supera los 100 carácteres, ";

        if (productos.descripcion.Length > 500)
            resultado += "El campo Descripción supera los 500 carácteres, ";

        if (productos.codigo.Length > 50)
            resultado += "El campo Código supera los 50 carácteres, ";

        if (string.IsNullOrEmpty(productos.stock))
            resultado += "El campo Stock es obligatorio, ";

        if (string.IsNullOrEmpty(productos.precioUnitario))
            resultado += "El campo Precio Unitario es obligatorio, ";

        int Stock = 0;
        int.TryParse(productos.stock, out Stock);
        if (Stock == 0 && productos.stock != "0")
            resultado += "El campo Stock contiene carácteres invalidos, ";

        int StockMinimo = 0;
        int.TryParse(productos.stock, out StockMinimo);
        if (Stock == 0 && productos.stock != "0" && productos.stock != "")
            resultado += "El campo StockMinimo contiene carácteres invalidos, ";

        double PrecioUnitario = 0;
        double.TryParse(productos.precioUnitario, out PrecioUnitario);

        if (PrecioUnitario == 0 && productos.precioUnitario != "0")
            resultado += "El campo Precio Unitario contiene carácteres invalidos, ";

        double iva = 0;
        double.TryParse(productos.iva, out iva);

        if (iva != 0 && iva != 2.5 && iva != 5 && iva != 10.5 && iva != 21 && iva != 27)
            resultado += "El campo Iva sólo puede contener los siguientes valores '0','2,5','10,5','21','27', ";

        using (var dbContext = new ACHEEntities())
        {
            var results = dbContext.Conceptos.Where(x => x.Codigo.ToUpper() == productos.codigo.ToUpper() && x.Codigo != "" && x.IDUsuario == productos.IDUsuario).Any();
            resultado += (results) ? "El Código ingresado ya existe, " : "";


            if (productos.CodigoProveedor != "0" && productos.CodigoProveedor != "")
            {
                var persona = dbContext.Personas.Where(x => x.IDUsuario == productos.IDUsuario && x.Codigo == productos.CodigoProveedor).FirstOrDefault();

                if (persona != null)
                    productos.IDPersona = persona.IDPersona;
                else
                    resultado += "No se encontro ningun proveedor con el codigo asociado, ";
            }
        }
        return resultado;
    }
    public static void RealizarImportacionProductos(List<ProductosCSVTmp> lista, string ACHEString)
    {
        if (lista != null && lista.Any(x => x.Estado == "A"))
        {

            //int idUsuario = 0;
            //int idProveedorGenerico = 0;

            //IMPORTAR
            DataTable dt = new DataTable();
            dt.Columns.Add("IDUsuario", typeof(Int32));//0

            dt.Columns.Add("Nombre", typeof(string));//1
            dt.Columns.Add("Codigo", typeof(string));//2
            dt.Columns.Add("Descripcion", typeof(string));//3
            dt.Columns.Add("Stock", typeof(Int32));//4
            dt.Columns.Add("PrecioUnitario", typeof(decimal));//5
            dt.Columns.Add("FechaAlta", typeof(DateTime));//6
            dt.Columns.Add("Estado", typeof(string));//7
            dt.Columns.Add("Observaciones", typeof(string));//8
            dt.Columns.Add("Iva", typeof(decimal));//9
            dt.Columns.Add("Tipo", typeof(string));//10
            dt.Columns.Add("CostoInterno", typeof(decimal));//11
            dt.Columns.Add("StockMinimo", typeof(Int32));//12
            dt.Columns.Add("IDPersona", typeof(Int32));//13

            foreach (var prod in lista.Where(x => x.Estado == "A").ToList())
            {
                #region Armo DataRow
                try
                {
                    DataRow drNew = dt.NewRow();
                    drNew[0] = prod.IDUsuario;
                    drNew[1] = prod.nombre;
                    drNew[2] = prod.codigo;
                    drNew[3] = prod.descripcion;
                    drNew[4] = prod.stock;
                    drNew[5] = prod.precioUnitario;
                    drNew[6] = prod.fechaAlta;
                    drNew[7] = prod.Estado;
                    drNew[8] = prod.observaciones;
                    drNew[9] = prod.iva;
                    drNew[10] = prod.tipo;
                    if (prod.CostoInterno != "")
                        drNew[11] = prod.CostoInterno;
                    else
                        drNew[11] = 0;
                    if (prod.StockMinimo != "")
                        drNew[12] = prod.StockMinimo;
                    else
                        drNew[12] = 0;
                    drNew[13] = prod.IDPersona;
                    dt.Rows.Add(drNew);

                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                #endregion
            }

            if (dt.Rows.Count > 0)
            {
                using (var sqlConnection = new SqlConnection(ACHEString))
                {
                    sqlConnection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        bulkCopy.DestinationTableName = "Conceptos";
                        foreach (DataColumn col in dt.Columns)
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                        bulkCopy.WriteToServer(dt);
                    }                    
                }            

            }

        }
        else
            throw new Exception("no se encontraron datos para importar");
    }
   

    #endregion

    #region PERSONAS
    public static List<PersonasCSVTmp> LeerArchivoCSVPersonas(string tipo, int idUsuario, string path)
    {
        List<PersonasCSVTmp> listaPersonasCSV = new List<PersonasCSVTmp>();
        FileHelperEngine engine = new FileHelperEngine(typeof(PersonasCSV));
        engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
        PersonasCSV[] res = (PersonasCSV[])engine.ReadFile(path);
        PersonasCSVTmp personaCSV;
        foreach (var Personas in res)
        {
            personaCSV = new PersonasCSVTmp();
            personaCSV.IDUsuario = idUsuario;
            personaCSV.Codigo = Personas.Codigo;
            personaCSV.Tipo = (tipo == "Clientes") ? "C" : "P";
            personaCSV.RazonSocial = Personas.RazonSocial.Trim();
            personaCSV.TipoDocumento = Personas.TipoDocumento.ToUpper();
            personaCSV.NroDocumento = Personas.NroDocumento.Replace("-", "").Trim();
            personaCSV.CondicionIva = Personas.CondicionIva.ToUpper();

            personaCSV.Telefono = Personas.Telefono.Trim();
            personaCSV.Celular = Personas.Celular.Trim();
            personaCSV.Web = Personas.Web.Trim();
            personaCSV.Email = Personas.Email.Trim();

            personaCSV.Provincia = Personas.Provincia.Trim();
            personaCSV.Ciudad = Personas.Ciudad.Trim();
            personaCSV.Domicilio = Personas.Domicilio.Trim();
            personaCSV.PisoDepto = Personas.PisoDepto.Trim();
            personaCSV.CodigoPostal = Personas.CodigoPostal.Trim();

            personaCSV.EmailsEnvioFc = Personas.EmailsEnvioFc;
            personaCSV.Personeria = Personas.Personeria.ToUpper();
            personaCSV.AlicuotaIvaDefecto = Personas.AlicuotaIvaDefecto;
            personaCSV.TipoComprobanteDefecto = Personas.TipoComprobanteDefecto;
            personaCSV.CBU = Personas.CBU;

            personaCSV.Banco = Personas.Banco.Trim();
            personaCSV.NombreFantansia = Personas.NombreFantansia.Trim();
            personaCSV.fechaAlta = DateTime.Now;
            personaCSV.Observaciones = Personas.Observaciones.Trim();
            personaCSV.Contacto = Personas.Contacto.Trim();

            personaCSV.resultados = ImportacionMasiva.ValidarPersona(personaCSV);

            if (listaPersonasCSV.Any(x => x.NroDocumento == Personas.NroDocumento))
                personaCSV.resultados += "El Nro de Documento se encuentra repetido en la lista, ";


            if (listaPersonasCSV.Any(x => x.Codigo == Personas.Codigo))
                personaCSV.resultados += "El Código se encuentra repetido en la lista, ";


            personaCSV.Estado = personaCSV.resultados == string.Empty ? "A" : "I";

            if (personaCSV.resultados == string.Empty)
                personaCSV.resultados = "<span class='label label-success'>OK</span>";
            else
                personaCSV.resultados = "<span class='label label-danger' data-toggle='tooltip' title='" + personaCSV.resultados.Substring(0, personaCSV.resultados.Length - 2) + ".'>ERROR. Ver detalle</span>";

            listaPersonasCSV.Add(personaCSV);
        }

        return listaPersonasCSV.ToList();
    }
    public static string ValidarPersona(PersonasCSVTmp Personas)
    {
        string resultado = string.Empty;

        /*Obligatorios*/
        resultado += (string.IsNullOrEmpty(Personas.RazonSocial)) ? "El campo Razón Social es obligatorio , " : "";
        resultado += (string.IsNullOrEmpty(Personas.TipoDocumento)) ? "El campo Tipo Documento es obligatorio , " : "";
        if (Personas.CondicionIva != "CF")
            resultado += (string.IsNullOrEmpty(Personas.NroDocumento)) ? "El campo Nro Documento es obligatorio , " : "";

        resultado += (string.IsNullOrEmpty(Personas.CondicionIva)) ? "El campo Condición Iva es obligatorio , " : "";
        resultado += (string.IsNullOrEmpty(Personas.Provincia)) ? "El campo Provincia es obligatorio , " : "";
        resultado += (string.IsNullOrEmpty(Personas.Personeria)) ? "El campo Personería es obligatorio , " : "";


        /*longitud  obligatoria*/
        resultado += (Personas.RazonSocial.Length > 128) ? "El campo Razón Social supera los 128 carácteres , " : "";
        resultado += (Personas.TipoDocumento.Length > 15) ? "El campo Tipo Documento supera los 15 carácteres , " : "";
        resultado += (Personas.NroDocumento.Length > 20) ? "El campo Nro Documento supera los 20 carácteres , " : "";

        resultado += (Personas.CondicionIva.Length > 50) ? "El campo Condición Iva supera los 50 carácteres , " : "";
        resultado += (Personas.Provincia.Length > 50) ? "El campo Provincia supera los 50 carácteres , " : "";
        resultado += (Personas.Ciudad.Length > 100) ? "El campo Ciudad supera los 100 carácteres , " : "";

        resultado += (Personas.Personeria.Length > 50) ? "El campo Personería supera los 50 carácteres , " : "";
        resultado += (Personas.AlicuotaIvaDefecto.Length > 10) ? "El campo Alicuota Iva Defecto supera los 10 carácteres , " : "";
        resultado += (Personas.TipoComprobanteDefecto.Length > 10) ? "El campo Tipo Comprobante Defecto supera los 10 carácteres , " : "";

        /*longitud no obligatoria*/
        resultado += (Personas.Telefono.Length > 50) ? "El campo Teléfono supera los 50 carácteres , " : "";
        resultado += (Personas.Celular.Length > 50) ? "El campo Celular supera los 50 carácteres , " : "";
        resultado += (Personas.Web.Length > 255) ? "El campo Web supera los 255 carácteres , " : "";
        resultado += (Personas.Email.Length > 128) ? "El campo Email supera los 128 carácteres , " : "";

        resultado += (Personas.Domicilio.Length > 100) ? "El campo Domicilio supera los 100 carácteres , " : "";
        resultado += (Personas.PisoDepto.Length > 10) ? "El campo Piso Depto supera los 10 carácteres , " : "";
        resultado += (Personas.CodigoPostal.Length > 10) ? "El campo Código Postal supera los 10 carácteres , " : "";

        resultado += (Personas.CBU.Length > 50) ? "El campo CBU supera los 50 carácteres , " : "";
        resultado += (Personas.Banco.Length > 50) ? "El campo Banco supera los 50 carácteres , " : "";
        resultado += (Personas.NombreFantansia.Length > 128) ? "El campo Nombre Fantasía supera los 128 carácteres , " : "";
        resultado += (Personas.Contacto.Length > 150) ? "El campo Contacto supera los 150 carácteres , " : "";
        resultado += (Personas.Codigo.Length > 50) ? "El campo Código supera 50 carácteres , " : "";

        /*solo determinados valores*/
        resultado += (Personas.CondicionIva.ToUpper() != "RI" && Personas.CondicionIva.ToUpper() != "CF" && Personas.CondicionIva.ToUpper() != "MO" && Personas.CondicionIva.ToUpper() != "EX") ? "El campo Condición Iva sólo puede contener los siguientes valores: RI,CF,MO,EX , " : "";
        resultado += (Personas.Personeria.ToUpper() != "F" && Personas.Personeria.ToUpper() != "J") ? "El campo Personería sólo puede contener los siguientes valores F,J , " : "";
        resultado += (Personas.TipoDocumento.ToUpper() != "DNI" && Personas.TipoDocumento.ToUpper() != "CUIT") ? "El campo Tipo Documento sólo puede contener los siguientes valores: DNI,CUIT , " : "";

        /*otras validaciones*/
        resultado += (!Personas.NroDocumento.IsValidCUIT() && Personas.TipoDocumento == "CUIT") ? "El CUIT ingresado es inválido , " : "";

        if (Personas.Email != "")
            resultado += (!Personas.Email.IsValidEmailAddress()) ? "El Email ingresado es inválido , " : "";
        if (Personas.EmailsEnvioFc != "")
            resultado += (!Personas.EmailsEnvioFc.IsValidEmailAddress()) ? "El EmailsEnvioFc ingresado es inválido , " : "";

        using (var dbContext = new ACHEEntities())
        {
            var documento = dbContext.Personas.Where(x => x.NroDocumento.ToUpper() == Personas.NroDocumento.ToUpper()
                                                && x.IDUsuario == Personas.IDUsuario
                                                && x.Tipo == Personas.Tipo).Any();

            resultado += (documento) ? "El Nro Documento ingresado ya existe, " : "";

            //var codigo = dbContext.Personas.Where(x => x.Codigo.ToUpper() == Personas.Codigo.ToUpper() && x.Codigo != "" && x.IDUsuario == Personas.IDUsuario).Any();
            //resultado += (codigo) ? "El Código ingresado ya existe, " : "";

            var Provincia = dbContext.Provincias.Any(x => x.Nombre.ToUpper() == Personas.Provincia.ToUpper());
            resultado += (!Provincia) ? "La Provincia ingresada no es correcta, " : "";
        }
        return resultado;
    }
    public static void RealizarImportacionPersonas(List<PersonasCSVTmp> lista, string ACHEString)
    {
        if (lista != null && lista.Any(x => x.Estado == "A"))
        {
            //IMPORTAR
            DataTable dt = new DataTable();

            dt.Columns.Add("Tipo", typeof(string));//0

            dt.Columns.Add("IDUsuario", typeof(Int32));//1
            dt.Columns.Add("RazonSocial", typeof(string));//2
            dt.Columns.Add("TipoDocumento", typeof(string));//3
            dt.Columns.Add("NroDocumento", typeof(string));//4
            dt.Columns.Add("CondicionIva", typeof(string));//5

            dt.Columns.Add("Telefono", typeof(string));//6
            dt.Columns.Add("Celular", typeof(string));//7
            dt.Columns.Add("Web", typeof(string));//8
            dt.Columns.Add("Email", typeof(string));//9
            dt.Columns.Add("Observaciones", typeof(string));//10
            dt.Columns.Add("IDProvincia", typeof(string));//11

            dt.Columns.Add("IDCiudad", typeof(string));//12
            dt.Columns.Add("Domicilio", typeof(string));//13
            dt.Columns.Add("PisoDepto", typeof(string));//14
            dt.Columns.Add("CodigoPostal", typeof(string));//15
            dt.Columns.Add("EmailsEnvioFc", typeof(string));//16

            dt.Columns.Add("Personeria", typeof(string));//17
            dt.Columns.Add("AlicuotaIvaDefecto", typeof(string));//18
            dt.Columns.Add("TipoComprobanteDefecto", typeof(string));//19
            dt.Columns.Add("FechaAlta", typeof(DateTime));//20
            dt.Columns.Add("CBU", typeof(string));//21

            dt.Columns.Add("Banco", typeof(string));//22
            dt.Columns.Add("NombreFantansia", typeof(string));//23
            dt.Columns.Add("Contacto", typeof(string));//24
            dt.Columns.Add("Codigo", typeof(string));//25

            var dbContext = new ACHEEntities();

            int idUsuario = lista.Where(x => x.Estado == "A").Select(s => s.IDUsuario).FirstOrDefault();

            var query = dbContext.Personas.Where(w => w.IDUsuario == idUsuario && w.Codigo != "" && w.Codigo != null)
                            .AsEnumerable()
                            .Where(m => Regex.IsMatch(m.Codigo, @"\d"))
                            .OrderByDescending(o => o.Codigo)
                            .Select(s => s.Codigo).FirstOrDefault();

            long codigoPersonaDisponible = Convert.ToInt64(query);

            foreach (var personas in lista.Where(x => x.Estado == "A").ToList())
            {
                #region Armo DataRow
                try
                {
                    ObtenerIDProvinciayCiudad(personas);
                    DataRow drNew = dt.NewRow();

                    drNew[0] = personas.Tipo;

                    drNew[1] = personas.IDUsuario;
                    drNew[2] = personas.RazonSocial;
                    drNew[3] = personas.TipoDocumento;
                    drNew[4] = personas.NroDocumento;
                    drNew[5] = personas.CondicionIva;

                    drNew[6] = personas.Telefono;
                    drNew[7] = personas.Celular;
                    drNew[8] = personas.Web;
                    drNew[9] = personas.Email;
                    drNew[10] = personas.Observaciones;
                    drNew[11] = personas.Provincia;

                    drNew[12] = personas.Ciudad;
                    drNew[13] = personas.Domicilio;
                    drNew[14] = personas.PisoDepto;
                    drNew[15] = personas.CodigoPostal;
                    drNew[16] = personas.EmailsEnvioFc;

                    drNew[17] = personas.Personeria;
                    drNew[18] = personas.AlicuotaIvaDefecto;
                    drNew[19] = personas.TipoComprobanteDefecto;
                    drNew[20] = personas.fechaAlta;
                    drNew[21] = personas.CBU;

                    drNew[22] = personas.Banco;
                    drNew[23] = personas.NombreFantansia;
                    drNew[24] = personas.Contacto;

                    var existe = dbContext.Personas.Where(w => w.IDUsuario == idUsuario && w.Codigo == personas.Codigo).Any();

                    if (existe)
                    {
                        personas.Codigo = (codigoPersonaDisponible + 1).ToString();
                        codigoPersonaDisponible++;
                    }
                    else
                        drNew[25] = personas.Codigo;

                    dt.Rows.Add(drNew);

                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                #endregion
            }

            if (dt.Rows.Count > 0)
            {

                using (var sqlConnection = new SqlConnection(ACHEString))
                {
                    sqlConnection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        bulkCopy.DestinationTableName = "Personas";
                        foreach (DataColumn col in dt.Columns)
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                        bulkCopy.WriteToServer(dt);
                    }
                }
            }
        }
        else
            throw new Exception("No se encontraron datos para importar");
    }
    private static void ObtenerIDProvinciayCiudad(PersonasCSVTmp personas)
    {
        using (var dbConcext = new ACHEEntities())
        {
            if (personas.Ciudad.ToUpper().Contains("CABA") || 
                personas.Ciudad.ToUpper().Contains("C.A.B.A.") || 
                personas.Ciudad.ToUpper().Contains("CAPITAL FEDERAL") ||
                personas.Ciudad.ToUpper().Contains("Cap. fed."))
            {
                personas.Provincia = "0";//Ciudad de Buenos Aires
                personas.Ciudad = "24071";//SIN IDENTIFICAR
            }
            else
            {
                try
                {
                    var provincia = dbConcext.Provincias.Where(x => x.Nombre.ToUpper() == personas.Provincia.ToUpper()).FirstOrDefault();
                    if (provincia != null)
                        personas.Provincia = provincia.IDProvincia.ToString();
                    else
                        personas.Provincia = "2";//Ciudad de buenos aires

                    //TODO: LA CIUDAD DEBE SER POR PROVINCIA!!!
                    var ciudad = dbConcext.Ciudades.Where(x => x.Nombre.ToUpper() == personas.Ciudad.ToUpper() && x.IDProvincia == provincia.IDProvincia).FirstOrDefault();
                    if (ciudad != null)
                        personas.Ciudad = ciudad.IDCiudad.ToString();
                    else
                        personas.Ciudad = dbConcext.Ciudades.Where(x => x.IDProvincia == provincia.IDProvincia && x.Nombre == "SIN IDENTIFICAR").FirstOrDefault().IDCiudad.ToString();//SIN IDENTIFICAR
                }
                catch 
                {
                    personas.Provincia = "0";//Ciudad de Buenos Aires
                    personas.Ciudad = "24071";//SIN IDENTIFICAR
                }
             
            }
        }
    }
    #endregion

    #region LISTA DE PRECIOS
    public static void RealizarImportacionListaPrecios(List<ProductosPreciosCSVTmp> lista, string ACHEString, int idUsuario)
    {
        if (lista != null && lista.Any(x => x.Estado == "A"))
        {
            //IMPORTAR
            DataTable dt = new DataTable();
            dt.Columns.Add("Codigo", typeof(string));//1
            dt.Columns.Add("Precio", typeof(decimal));//2
            dt.Columns.Add("IDUsuario", typeof(int));//3
            dt.Columns.Add("Stock", typeof(decimal));//4
            dt.Columns.Add("Costo", typeof(decimal));//2

            foreach (var prod in lista.Where(x => x.Estado == "A").ToList())
            {
                #region Armo DataRow
                try
                {
                    DataRow drNew = dt.NewRow();
                    drNew[0] = prod.Codigo;
                    drNew[1] = prod.Precio;
                    drNew[2] = prod.IDUsuario;
                    drNew[3] = prod.Stock;
                    drNew[4] = prod.Costo;
                    dt.Rows.Add(drNew);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                #endregion
            }

            if (dt.Rows.Count > 0)
            {
                EliminarTmp();
                using (var sqlConnection = new SqlConnection(ACHEString))
                {
                    sqlConnection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        bulkCopy.DestinationTableName = "ConceptosTmp";
                        foreach (DataColumn col in dt.Columns)
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                        bulkCopy.WriteToServer(dt);
                    }
                }

                ProcesarTmp();
                LlenarConceptos_BackUP(idUsuario);
            }
        }
        else
            throw new Exception("no se encontraron datos para importar");
    }
    public static string ValidarProductosPrecios(ProductosPreciosCSVTmp productos)
    {
        string resultado = string.Empty;

        resultado += (string.IsNullOrEmpty(productos.Codigo)) ? "El campo Código es obligatorio, " : "";
        resultado += (string.IsNullOrEmpty(productos.Precio)) ? "El campo Precio es obligatorio, " : "";
        resultado += (string.IsNullOrEmpty(productos.Costo)) ? "El campo Costo interno es obligatorio, " : "";

        resultado += (productos.Codigo.Length > 50) ? "El campo Código sobre pasa los 50 carácteres , " : "";
        resultado += (productos.Precio.Length > 10) ? "El campo Precio sobre pasa los 10 carácteres , " : "";
        resultado += (productos.Costo.Length > 10) ? "El campo Costo interno sobre pasa los 10 carácteres , " : "";

        decimal n;
        decimal.TryParse(productos.Precio, out n);
        resultado += (n == 0) ? "El campo Precio debe ser numérico y mayor a 0, " : "";

        if (!string.IsNullOrWhiteSpace(productos.Stock))
        {
            int Stock;
            int.TryParse(productos.Stock, out Stock);
            if (Stock == 0 && productos.Stock != "0")
                resultado += (n == 0) ? "El campo Stock debe ser mayor o igual 0, " : "";
        }

        decimal Costo;
        decimal.TryParse(productos.Costo, out Costo);
        if (Costo == 0 && productos.Costo != "0")
            resultado += (n == 0) ? "El campo Costo debe ser mayor o igual 0, " : "";

        using (var dbContext = new ACHEEntities())
        {
            if (productos.IDLista > 00)
            {
                var lista = dbContext.PreciosConceptos.Any(x => x.Conceptos.Codigo.ToUpper() == productos.Codigo.ToUpper() && x.IDListaPrecios == productos.IDLista && x.Conceptos.IDUsuario == productos.IDUsuario && x.Precio > 0);
                resultado += (!lista) ? "El Código ingresado no pertenece a la lista seleccionada, " : "";
            }
            var codigo = dbContext.Conceptos.Any(x => x.Codigo.ToUpper() == productos.Codigo.ToUpper() && x.IDUsuario == productos.IDUsuario);
            resultado += (!codigo) ? "El Código ingresado no existe, " : "";
        }
        return resultado;
    }
    public static List<ProductosPreciosCSVTmp> LeerArchivoCSVListaPrecios(string idLista, int idUsuario, string path)
    {
        List<ProductosPreciosCSVTmp> listaPreciosCSV = new List<ProductosPreciosCSVTmp>();

        FileHelperEngine engine = new FileHelperEngine(typeof(ProductosPreciosCSV));
        engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
        ProductosPreciosCSV[] res = (ProductosPreciosCSV[])engine.ReadFile(path);
        ProductosPreciosCSVTmp prodCSV;

        foreach (var productos in res)
        {
            prodCSV = new ProductosPreciosCSVTmp();
            prodCSV.IDUsuario = idUsuario;
            prodCSV.Codigo = productos.Codigo;
            prodCSV.Precio = productos.Precio;
            prodCSV.IDLista = Convert.ToInt32(idLista);

            prodCSV.Stock = productos.Stock;
            prodCSV.Costo = productos.Costo;

            prodCSV.resultados = ImportacionMasiva.ValidarProductosPrecios(prodCSV);

            if (listaPreciosCSV.Any(x => x.Codigo == productos.Precio))
                prodCSV.resultados = "El código se encuentra repetido en la lista, ";


            prodCSV.Estado = prodCSV.resultados == string.Empty ? "A" : "I";

            if (prodCSV.resultados == string.Empty)
                prodCSV.resultados = "<span class='label label-success'>OK</span>";
            else
                prodCSV.resultados = "<span class='label label-danger' data-toggle='tooltip' title='" + prodCSV.resultados.Substring(0, prodCSV.resultados.Length - 2) + ".'>ERROR. Ver detalle</span>";

            listaPreciosCSV.Add(prodCSV);
        }

        return listaPreciosCSV;
    }
  
    private static void EliminarTmp()
    {
        try
        {
            using (var dbContext = new ACHEEntities())
                dbContext.deleteConceptosTmp();
        }
        catch (Exception)
        {
            throw new Exception("Error al eliminar los datos de la tabla temporal");
        }
    }
    public static void LlenarConceptos_BackUP(int IdUsuario)
    {
        try
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ACHEString"].ConnectionString);

            SqlCommand comando = new SqlCommand("InsertarConceptos_BackUp_Vacio", cn);
            comando.CommandType = CommandType.StoredProcedure;

            //Parametros
            SqlParameter parIdUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int);
            parIdUsuario.Direction = ParameterDirection.Input;
            parIdUsuario.Value = Convert.ToInt32(IdUsuario);
            comando.Parameters.Add(parIdUsuario);


            comando.Connection.Open();
            comando.ExecuteNonQuery();
            comando.Connection.Close();
            comando.Connection.Dispose();
        }
        catch (Exception)
        {
            throw new Exception("Error al intentar copiar datos de la tabla Conceptos");
        }
    }
   
    private static void ProcesarTmp()
    {
        try
        {
            using (var dbContext = new ACHEEntities())
                dbContext.ProcesarConceptos();
        }
        catch (Exception)
        {
            throw new Exception("Error al actualizar los precios");
        }
    }
    #endregion

    #region PLAN DE CUENTAS NEGOCIO
    public static List<PlanDeCuentasCSVTmp> LeerArchivoCSVPlanDeCuentas(int idUsuario, string path)
    {
        List<PlanDeCuentasCSVTmp> listaPlanDeCuentasCSV = new List<PlanDeCuentasCSVTmp>();
        FileHelperEngine engine = new FileHelperEngine(typeof(PlanDeCuentasCSV));
        engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
        // to Read use:
        PlanDeCuentasCSV[] res = (PlanDeCuentasCSV[])engine.ReadFile(path);

        PlanDeCuentasCSVTmp pcCSV;
        foreach (var pc in res)
        {
            pcCSV = new PlanDeCuentasCSVTmp();
            pcCSV.IDUsuario = idUsuario;
            pcCSV.Codigo = pc.Codigo;
            pcCSV.Nombre = pc.Nombre;
            pcCSV.AdmiteAsientoManual = pc.AdmiteAsientoManual;
            pcCSV.TipoDeCuenta = (pc.TipoDeCuenta.ToUpper() == "PN") ? "RESULTADO" : pc.TipoDeCuenta.ToUpper();
            pcCSV.CodigoPadre = pc.CodigoPadre;

            pcCSV.resultados = ValidarPlanDeCuentas(pcCSV);

            if (listaPlanDeCuentasCSV.Any(x => x.Codigo == pc.Codigo))
                pcCSV.resultados = "El Código se encuentra repetido en la lista, ";

            pcCSV.Estado = pcCSV.resultados == string.Empty ? "A" : "I";

            if (pcCSV.resultados == string.Empty)
                pcCSV.resultados = "<span class='label label-success'>OK</span>";
            else
                pcCSV.resultados = "<span class='label label-danger' data-toggle='tooltip' title='" + pcCSV.resultados.Substring(0, pcCSV.resultados.Length - 2) + ".'>ERROR. Ver detalle</span>";

            listaPlanDeCuentasCSV.Add(pcCSV);
        }

        return listaPlanDeCuentasCSV.ToList();
    }
    public static string ValidarPlanDeCuentas(PlanDeCuentasCSVTmp planDeCuentas)
    {
        string resultado = string.Empty;
        resultado += (string.IsNullOrEmpty(planDeCuentas.Codigo)) ? "El campo Codigo es obligatorio, " : "";
        resultado += (string.IsNullOrEmpty(planDeCuentas.Nombre)) ? "El campo Nombre es obligatorio, " : "";
        resultado += (string.IsNullOrEmpty(planDeCuentas.AdmiteAsientoManual)) ? "El campo AdmiteAsientoManual es obligatorio, " : "";
        resultado += (string.IsNullOrEmpty(planDeCuentas.TipoDeCuenta)) ? "El campo TipoDeCuenta es obligatorio, " : "";
        resultado += ContabilidadCommon.VerificarCodigo(planDeCuentas.Codigo) ? "El código contiene caracteres invalidos, " : "";
        if (planDeCuentas.CodigoPadre != "")
            resultado += ContabilidadCommon.VerificarCodigo(planDeCuentas.CodigoPadre) ? "El código padre contiene caracteres invalidos, " : "";

        resultado += (planDeCuentas.Nombre.Length > 100) ? "El campo Nombre supera los 100 carácteres, " : "";
        resultado += (planDeCuentas.TipoDeCuenta.Length > 100) ? "El campo Tipo De Cuenta supera los 100 carácteres, " : "";
        resultado += (planDeCuentas.Codigo.Length > 100) ? "El campo Código supera los 100 carácteres, " : "";
        resultado += (planDeCuentas.AdmiteAsientoManual != "SI" && planDeCuentas.AdmiteAsientoManual != "NO") ? "El campo admite asiento manual solo puede contener el valor SI o NO, " : "";
        resultado += (planDeCuentas.TipoDeCuenta.ToUpper() != "ACTIVO" && planDeCuentas.TipoDeCuenta.ToUpper() != "PASIVO" && planDeCuentas.TipoDeCuenta.ToUpper() != "RESULTADO" && planDeCuentas.TipoDeCuenta.ToUpper() != "PN") ? "El campo admite asiento manual solo puede contener el valor Activo, Pasivo, Resultado o PN, " : "";

        return resultado;
    }
    public static void RealizarImportacionPlanDeCuentas(List<PlanDeCuentasCSVTmp> lista, string ACHEString, int idusuario)
    {
        try
        {
            ContabilidadCommon.EliminarPlanDeCuentasActual(idusuario);
            ContabilidadCommon.EliminarConfiguracionPlanDeCuenta(idusuario);

            if (lista != null && lista.Any(x => x.Estado == "A"))
            {
                //IMPORTAR
                DataTable dt = new DataTable();
                dt.Columns.Add("IDUsuario", typeof(Int32));//0

                dt.Columns.Add("Nombre", typeof(string));//1
                dt.Columns.Add("Codigo", typeof(string));//2
                dt.Columns.Add("AdminiteAsientoManual", typeof(Boolean));//3
                dt.Columns.Add("TipoDeCuenta", typeof(string));//4

                foreach (var prod in lista.Where(x => x.Estado == "A").ToList())
                {
                    #region Armo DataRow
                    try
                    {
                        DataRow drNew = dt.NewRow();
                        drNew[0] = idusuario;
                        drNew[1] = prod.Nombre;
                        drNew[2] = prod.Codigo;
                        drNew[3] = (prod.AdmiteAsientoManual == "SI") ? 1 : 0;
                        drNew[4] = prod.TipoDeCuenta;

                        dt.Rows.Add(drNew);

                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                    }
                    #endregion
                }

                if (dt.Rows.Count > 0)
                {
                    using (var sqlConnection = new SqlConnection(ACHEString))
                    {
                        sqlConnection.Open();
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                        {
                            bulkCopy.DestinationTableName = "PlanDeCuentas";
                            foreach (DataColumn col in dt.Columns)
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                            bulkCopy.WriteToServer(dt);
                        }
                    }
                }
            }
            else
                throw new CustomException("No se encontraron datos para importar.");
        }
        catch (CustomException e)
        {
            throw new Exception(e.Message);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    public static void ReferenciarPlanDeCuentas(int idUsuario, List<PlanDeCuentasCSVTmp> lista)
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                var planDeCuenta = dbContext.PlanDeCuentas.Where(x => x.IDUsuario == idUsuario).OrderBy(x => x.Codigo).ToList();
                List<PlanDeCuentasCSVTmp> listaAux = new List<PlanDeCuentasCSVTmp>();
                foreach (var item in planDeCuenta)
                {
                    var CtasHijas = lista.Where(x => x.CodigoPadre == item.Codigo).ToList();
                    foreach (var Hijas in CtasHijas)
                    {
                        if (Hijas != null)
                        {
                            Hijas.IDPadre = item.IDPlanDeCuenta;
                            listaAux.Add(Hijas);
                        }
                    }
                }
                foreach (var item in listaAux)
                {
                    var CtasHijas = planDeCuenta.Where(x => x.Codigo == item.Codigo).FirstOrDefault();
                    CtasHijas.IDPadre = item.IDPadre;
                }
                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    #endregion

    #region FACTURAS
    public static List<FacturasCSVTmp> LeerArchivoCSVFacturas(WebUser usu, string path)
    {
        List<FacturasCSVTmp> listaFacturasCSV = new List<FacturasCSVTmp>();
        string[] listaTipoComprobanteFactura = { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "NDA", "NDB", "NDC", "FCAMP", "FCBMP", "FCCMP", "RCAMP", "RCBMP", "RCCMP", "NDAMP", "NDBMP", "NDCMP" };

        FileHelperEngine engine = new FileHelperEngine(typeof(FacturasCSV));
        engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
        FacturasCSV[] res = (FacturasCSV[])engine.ReadFile(path);
        FacturasCSVTmp facturasCSV;

        var dbContext = new ACHEEntities();
        
        List<PuntosDeVenta> puntosDeVenta = dbContext.PuntosDeVenta.Where(w => w.IDUsuario == usu.IDUsuario).ToList();
        List<Comprobantes> comprobantes = dbContext.Comprobantes.Where(w => w.IDUsuario == usu.IDUsuario && listaTipoComprobanteFactura.Contains(w.Tipo)).ToList();
        List<Provincias> provincias = dbContext.Provincias.ToList();
        List<Ciudades> ciudades = dbContext.Ciudades.ToList();
        List<Conceptos> conceptos = dbContext.Conceptos.Where(w => w.IDUsuario == usu.IDUsuario).ToList();
        List<PlanDeCuentas> planesDeCuenta = dbContext.PlanDeCuentas.Where(w => w.IDUsuario == usu.IDUsuario).ToList();
        List<ConfiguracionPlanDeCuenta> configuracionesDePlanesDeCuenta = dbContext.ConfiguracionPlanDeCuenta.Where(w => w.IDUsuario == usu.IDUsuario).ToList();


        foreach (var FC in res)
        {
            facturasCSV = new FacturasCSVTmp();
            facturasCSV.IDUsuario = usu.IDUsuario;
            facturasCSV.Tipo = "C";
            facturasCSV.RazonSocial = FC.RazonSocial;
            facturasCSV.NombreFantasia = FC.NombreFantasia;
            facturasCSV.Email = FC.Email;
            facturasCSV.TipoDocumento = FC.TipoDocumento;
            facturasCSV.NroDocumento = FC.NroDocumento.ReplaceAll("-", "");
            facturasCSV.CondicionIva = FC.CondicionIva;
            if (facturasCSV.NroDocumento.Length > 8)
                facturasCSV.Personeria = (facturasCSV.NroDocumento.Substring(0, 2) == "30") ? "J" : "F";
            else
                facturasCSV.Personeria = "F";
            facturasCSV.Provincia = (FC.Provincia.ToUpper().Contains("CIUDAD")) ? "Ciudad de Buenos Aires" : FC.Provincia;
            facturasCSV.Ciudad = FC.Ciudad;
            facturasCSV.Domicilio = FC.Domicilio;

            facturasCSV.Web = FC.Web;

            facturasCSV.Fecha = FC.Fecha;
            facturasCSV.TipoComprobante = FC.TipoComprobante.ToUpper();
            facturasCSV.PuntoDeVenta = FC.PuntoDeVenta;
            facturasCSV.NroComprobante = FC.NroComprobante;
            facturasCSV.CAE = FC.CAE;
            //facturasCSV.Modo = (string.IsNullOrWhiteSpace(FC.CAE)) ? "T" : "E";
            //facturasCSV.Modo = (string.IsNullOrWhiteSpace(FC.CAE)) ? "T" : "T";
            facturasCSV.Modo = FC.Modo;
            if (!string.IsNullOrWhiteSpace(facturasCSV.Fecha))
                facturasCSV.FechaCAE = (string.IsNullOrWhiteSpace(FC.CAE)) ? Convert.ToDateTime(facturasCSV.Fecha).AddDays(30).ToString(formatoFecha) : null;

            facturasCSV.ImporteNeto = FC.ImporteNeto;
            facturasCSV.ImporteNoGravado = FC.ImporteNoGravado;
            facturasCSV.IVA2700 = FC.IVA2700;
            facturasCSV.IVA2100 = FC.IVA2100;
            facturasCSV.IVA1005 = FC.IVA1005;
            facturasCSV.IVA0205 = FC.IVA0205;
            facturasCSV.IVA0500 = FC.IVA0500;
            facturasCSV.IVA0000 = FC.IVA0000;
            facturasCSV.PercepcionesIVA = FC.PercepcionesIVA;
            facturasCSV.PercepcionesIIBB = FC.PercepcionesIIBB;
            facturasCSV.Total = FC.Total;
            facturasCSV.MontoPagado = FC.MontoPagado;
            facturasCSV.FechaDePago = FC.FechaDePago;
            facturasCSV.CodigoCuentaContable = FC.CodigoCuentaContable;
            facturasCSV.Observaciones = FC.Observaciones;
            facturasCSV.CodigoConcepto = FC.CodigoConcepto;


            facturasCSV.resultados = ValidarFacturas(facturasCSV, usu, puntosDeVenta,
                                                      comprobantes, provincias, ciudades, conceptos,
                                                      planesDeCuenta, configuracionesDePlanesDeCuenta);

            facturasCSV.resultados = ObtenerConceptos(conceptos, facturasCSV, usu, facturasCSV.resultados);

            if (listaFacturasCSV.Any(x => x.NroComprobante == FC.NroComprobante && x.TipoComprobante == FC.TipoComprobante && x.PuntoDeVenta == FC.PuntoDeVenta && x.CAE != null))
                facturasCSV.resultados += "El Comprobante se encuentra repetido en la lista, ";
            facturasCSV.Estado = facturasCSV.resultados == string.Empty ? "A" : "I";

            if (facturasCSV.resultados == string.Empty)
                facturasCSV.resultados = "<span class='label label-success'>OK</span>";
            else
                facturasCSV.resultados = "<span class='label label-danger' data-toggle='tooltip' title='" + facturasCSV.resultados.Substring(0, facturasCSV.resultados.Length - 2) + ".'>ERROR. Ver detalle</span>";

            listaFacturasCSV.Add(facturasCSV);
        }

        return listaFacturasCSV.ToList();
    }
    private static string ValidarFacturas(FacturasCSVTmp facturas, 
        WebUser usu, List<PuntosDeVenta> puntosDeVenta, List<Comprobantes> comprobantes,
        List<Provincias> provincias, List<Ciudades> ciudades, List<Conceptos> conceptos,
        List<PlanDeCuentas> planesDeCuenta, List<ConfiguracionPlanDeCuenta> configuracionesDePlanesDeCuenta)
    {
        string resultado = string.Empty;

        #region/*Obligatorios*/
        resultado += (string.IsNullOrEmpty(facturas.RazonSocial)) ? "El campo Razón Social es obligatorio , " : "";

        if (facturas.CondicionIva != "CF")
            resultado += (string.IsNullOrEmpty(facturas.NroDocumento)) ? "El campo Nro Documento es obligatorio , " : "";
        resultado += (string.IsNullOrEmpty(facturas.CondicionIva)) ? "El campo CondicionIva es obligatorio , " : "";

        resultado += (string.IsNullOrEmpty(facturas.Fecha)) ? "El campo Fecha es obligatorio , " : "";
        resultado += (string.IsNullOrEmpty(facturas.TipoComprobante)) ? "El campo TipoComprobante es obligatorio , " : "";
        resultado += (string.IsNullOrEmpty(facturas.PuntoDeVenta)) ? "El campo PuntoDeVenta es obligatorio , " : "";

        if (facturas.TipoDocumento.Equals("DNI"))
        {
            resultado += (facturas.NroComprobante.Length > 8) ? "El campo Nro Documento supera los 8 carácteres , " : "";
        }

        if (facturas.TipoDocumento.Equals("CUIT"))
        {
            resultado += (facturas.NroComprobante.Length == 11) ? "El campo Nro Documento debe contener 11 carácteres , " : "";
        }

        //if (facturas.TipoDocumento.Equals("DNI"))
        //{
        //    int numeroDni = -1;
        //    int.TryParse(facturas.NroDocumento, out numeroDni);
        //    if (numeroDni == -1)
        //        resultado += "El campo Numero de documento contiene carácteres invalidos, ";
        //    else
        //    {

        //    }
        //    if (facturas.TipoDocumento.)
        //    resultado += (string.IsNullOrEmpty(facturas.NroComprobante)) ? "El campo NroComprobante es obligatorio , " : "";
        //}
        //else
        //{
        //    facturas.NroComprobante = "0";
        //}

        if (!string.IsNullOrEmpty(facturas.CAE))
        {
            resultado += (string.IsNullOrEmpty(facturas.NroComprobante)) ? "El campo NroComprobante es obligatorio , " : "";
        }
        else
        {
            facturas.NroComprobante = "0";            
        }       

        resultado += (string.IsNullOrEmpty(facturas.CodigoConcepto)) ? "El campo CodigoConcepto es obligatorio , " : "";

        if (facturas.MontoPagado != "0")
            resultado += (string.IsNullOrEmpty(facturas.FechaDePago)) ? "El campo FechaDePago es obligatorio , " : "";

        var fecha = new DateTime();
        if (!DateTime.TryParse(facturas.Fecha, out fecha))
            resultado += "El campo Fecha no tiene un formato valido , ";

        if (facturas.MontoPagado != "0")
        {
            var FechaDePago = new DateTime();
            if (!DateTime.TryParse(facturas.FechaDePago, out FechaDePago))
                resultado += "El campo fecha de pago no tiene un formato valido , ";
        }
        #endregion

        #region  /*longitud  obligatoria*/
        resultado += (facturas.TipoComprobante.Length > 3) ? "El campo TipoComprobante supera los 3 carácteres , " : "";
        resultado += (facturas.PuntoDeVenta.Length > 4) ? "El campo PuntoDeVenta supera los 8 carácteres , " : "";
        resultado += (facturas.NroComprobante.Length > 8) ? "El campo NroComprobante supera los 8 carácteres , " : "";

        resultado += (facturas.ImporteNeto.Length > 10) ? "El campo ImporteNeto supera los 10 carácteres , " : "";
        resultado += (facturas.ImporteNoGravado.Length > 10) ? "El campo ImporteNoGravado supera los 10 carácteres , " : "";
        resultado += (facturas.PercepcionesIVA.Length > 10) ? "El campo PercepcionesIVA supera los 10 carácteres , " : "";
        resultado += (facturas.PercepcionesIIBB.Length > 10) ? "El campo PercepcionesIIBB supera los 10 carácteres , " : "";

        resultado += (facturas.IVA2700.Length > 10) ? "El campo IVA 27,00% supera los 10 carácteres , " : "";
        resultado += (facturas.IVA2100.Length > 10) ? "El campo IVA 21,00% supera los 10 carácteres , " : "";
        resultado += (facturas.IVA1005.Length > 10) ? "El campo IVA 10,50% supera los 10 carácteres , " : "";
        resultado += (facturas.IVA0205.Length > 10) ? "El campo IVA 02,50% supera los 10 carácteres , " : "";
        resultado += (facturas.IVA0500.Length > 10) ? "El campo IVA 00,50% supera los 10 carácteres , " : "";
        resultado += (facturas.IVA0000.Length > 10) ? "El campo IVA 00,00% supera los 10 carácteres , " : "";
        #endregion

        #region/* Numeros*/
        decimal ImporteNeto = 0;
        decimal.TryParse(facturas.ImporteNeto.Replace(SeparadorDeMiles, SeparadorDeDecimales), out ImporteNeto);
        if (ImporteNeto == 0 && facturas.ImporteNeto != "0")
            resultado += "El campo ImporteNeto contiene carácteres invalidos, ";

        decimal Total = 0;
        decimal.TryParse(facturas.Total.Replace(SeparadorDeMiles, SeparadorDeDecimales), out Total);
        if (Total == 0 && facturas.Total != "0")
            resultado += "El campo Total contiene carácteres invalidos, ";

        decimal ImporteNoGravado = 0;
        decimal.TryParse(facturas.ImporteNoGravado.Replace(SeparadorDeMiles, SeparadorDeDecimales), out ImporteNoGravado);
        if (ImporteNoGravado == 0 && facturas.ImporteNoGravado != "0")
            resultado += "El campo ImporteNoGravado contiene carácteres invalidos, ";

        decimal PercepcionesIVA = 0;
        decimal.TryParse(facturas.PercepcionesIVA.Replace(SeparadorDeMiles, SeparadorDeDecimales), out PercepcionesIVA);
        if (PercepcionesIVA == 0 && facturas.PercepcionesIVA != "0")
            resultado += "El campo PercepcionesIVA contiene carácteres invalidos, ";

        decimal PercepcionesIIBB = 0;
        decimal.TryParse(facturas.PercepcionesIIBB.Replace(SeparadorDeMiles, SeparadorDeDecimales), out PercepcionesIIBB);
        if (PercepcionesIIBB == 0 && facturas.PercepcionesIIBB != "0")
            resultado += "El campo PercepcionesIIBB contiene carácteres invalidos, ";


        decimal IVA2700 = 0;
        decimal.TryParse(facturas.IVA2700.Replace(SeparadorDeMiles, SeparadorDeDecimales), out IVA2700);
        if (IVA2700 == 0 && facturas.IVA2700 != "0")
            resultado += "El campo IVA 27,00% contiene carácteres invalidos, ";

        decimal IVA2100 = 0;
        decimal.TryParse(facturas.IVA2100.Replace(SeparadorDeMiles, SeparadorDeDecimales), out IVA2100);
        if (IVA2100 == 0 && facturas.IVA2100 != "0")
            resultado += "El campo IVA 21,00% contiene carácteres invalidos, ";

        decimal IVA1005 = 0;
        decimal.TryParse(facturas.IVA1005.Replace(SeparadorDeMiles, SeparadorDeDecimales), out IVA1005);
        if (IVA1005 == 0 && facturas.IVA1005 != "0")
            resultado += "El campo IVA 10,50% contiene carácteres invalidos, ";

        decimal IVA0500 = 0;
        decimal.TryParse(facturas.IVA0500.Replace(SeparadorDeMiles, SeparadorDeDecimales), out IVA0500);
        if (IVA0500 == 0 && facturas.IVA0500 != "0")
            resultado += "El campo IVA 05,00% contiene carácteres invalidos, ";

        decimal IVA0205 = 0;
        decimal.TryParse(facturas.IVA0205.Replace(SeparadorDeMiles, SeparadorDeDecimales), out IVA0205);
        if (IVA0205 == 0 && facturas.IVA0205 != "0")
            resultado += "El campo IVA 02,05% contiene carácteres invalidos, ";

        decimal IVA0000 = 0;
        decimal.TryParse(facturas.IVA0000.Replace(SeparadorDeMiles, SeparadorDeDecimales), out IVA0000);
        if (IVA0000 == 0 && facturas.IVA0000 != "0")
            resultado += "El campo IVA 00,00% contiene carácteres invalidos, ";

        decimal MontoPagado = 0;
        decimal.TryParse(facturas.MontoPagado.Replace(SeparadorDeMiles, SeparadorDeDecimales), out MontoPagado);
        if (MontoPagado == 0 && facturas.MontoPagado != "0")
            resultado += "El campo monto pagado contiene carácteres invalidos, ";
        #endregion

        #region otras logicas
        switch (facturas.CondicionIva)
        {
            case "EX":
            case "CF":
            case "MO":
                if(facturas.TipoComprobante != "FCA" && facturas.TipoComprobante != "FCC" 
                    && facturas.TipoComprobante != "FCB" && facturas.TipoComprobante != "NCA" 
                    && facturas.TipoComprobante != "NCC" && facturas.TipoComprobante != "NCB" 
                    && facturas.TipoComprobante != "NDC" && facturas.TipoComprobante != "NDB" 
                    && facturas.TipoComprobante != "NDA" && facturas.TipoComprobante != "RCC"
                    && facturas.TipoComprobante != "RCB" && facturas.TipoComprobante != "RCA"
                    && facturas.TipoComprobante != "FCAMP" && facturas.TipoComprobante != "FCCMP"
                    && facturas.TipoComprobante != "FCBMP" && facturas.TipoComprobante != "NCAMP"
                    && facturas.TipoComprobante != "NCCMP" && facturas.TipoComprobante != "NCBMP"
                    && facturas.TipoComprobante != "NDCMP" && facturas.TipoComprobante != "NDBMP"
                    && facturas.TipoComprobante != "NDAMP"
                    )
                {
                    resultado += "El campo TipoComprobante  solo puede tener los siguientes valores: FCC,FCB,NCC,NCB,NDC,NDB,RCA,RCB,RCC carácteres , ";
                }
                else
                {
                    resultado += "";
                }
                break;
            case "RI":
                if (usu.CondicionIVA == "RI")
                {
                    if (facturas.TipoComprobante != "FCA" && facturas.TipoComprobante != "FCB" 
                        && facturas.TipoComprobante != "NCA" && facturas.TipoComprobante != "NCB" 
                        && facturas.TipoComprobante != "NDA" && facturas.TipoComprobante != "NDB"
                        && facturas.TipoComprobante != "RCB" && facturas.TipoComprobante != "RCA"
                        && facturas.TipoComprobante != "FCAMP" && facturas.TipoComprobante != "FCBMP"
                        && facturas.TipoComprobante != "NCAMP" && facturas.TipoComprobante != "NDBMP" 
                        && facturas.TipoComprobante != "NDAMP" && facturas.TipoComprobante != "NDBMP"
                        )
                    {
                        resultado += "El campo TipoComprobante  solo puede tener los siguientes valores: FCA,FCB,NCA,NCB,NDA,NDB,RCA,RCB carácteres , ";
                    }
                    else
                    {
                        resultado += "";
                    }
                }
                else if (usu.CondicionIVA == "MO" || usu.CondicionIVA == "EX")
                {
                    if (facturas.TipoComprobante != "FCC" && facturas.TipoComprobante != "NCC" 
                        && facturas.TipoComprobante != "NDC" && facturas.TipoComprobante != "RCC"
                        && facturas.TipoComprobante != "FCCMP" && facturas.TipoComprobante != "NCCMP" 
                        && facturas.TipoComprobante != "NDCMP")
                    {
                        resultado += "El campo TipoComprobante  solo puede tener los siguientes valores: FCC,NCC,NDC,RCC carácteres , ";
                    }
                    else
                    {
                        resultado += "";
                    }
                }
                break;
        }

        if (facturas.Modo != "T" && facturas.Modo != "E")
            resultado += "Valor no valido en campo Modo, solo (T= Talonario o E=Electronico), ";
        

        if (usu.CondicionIVA == "RI")
        {
            if(ImporteNoGravado == 0)
            {
                resultado += (IVA0000 == 0 && IVA0205 == 0 && IVA0500 == 0 && IVA1005 == 0 && IVA2700 == 0 && IVA2100 == 0) ? "Su condición frente al IVA no admite productos sin IVA, " : "";
            }
        }            
        else
        {
            resultado += (IVA0000 > 0 || IVA0205 > 0 || IVA0500 > 0 || IVA1005 > 0 || IVA2700 > 0 || IVA2100 > 0) ? "Su condición frente al IVA no admite productos con IVA, " : "";
            resultado += (PercepcionesIIBB > 0 || PercepcionesIIBB > 0 || ImporteNoGravado > 0) ? "Su condición frente al IVA no admite comprobantes con percepciones, " : "";
        }
        #endregion

        #region/*validaciones contra la db*/

        int punto;
        if (!int.TryParse(facturas.PuntoDeVenta, out punto))
            resultado += "El campo Punto de venta no tiene un formato valido , ";

        int numero;
        if (!int.TryParse(facturas.NroComprobante, out numero))
            resultado += "El campo Número comprobante no tiene un formato valido , ";

        //var numero = Convert.ToInt32(facturas.NroComprobante);
        //var punto = Convert.ToInt32(facturas.PuntoDeVenta);

        var puntoValido = puntosDeVenta.Where(x => x.Punto == punto && x.FechaBaja == null).FirstOrDefault();
        var documento = comprobantes.Where(x => x.Tipo.ToUpper() == facturas.TipoComprobante.ToUpper() && x.PuntosDeVenta.Punto == punto && x.Numero == numero).Any();
        var Provincia = provincias.Where(x => x.Nombre.ToUpper() == facturas.Provincia.ToUpper()).FirstOrDefault();
        var ciudad = ciudades.Where(x => x.Nombre.ToUpper() == facturas.Ciudad.ToUpper()).FirstOrDefault();
        var concepto = conceptos.Where(x => x.Codigo == facturas.CodigoConcepto).FirstOrDefault();


        PlanDeCuentas planDeCuenta = new PlanDeCuentas();
        if (usu.UsaPlanCorporativo)
        {
            planDeCuenta = planesDeCuenta.Where(x => x.Codigo == facturas.CodigoCuentaContable).FirstOrDefault();

            var configPlanDeCuenta = configuracionesDePlanesDeCuenta.Any();
            resultado += (!configPlanDeCuenta) ? "No tiene configurado el plan de cuentas, " : "";
        }

        if (!string.IsNullOrEmpty(facturas.CAE))
        {
            resultado += (documento) ? "El Comprobante ingresado ya existe, " : "";
        }           

        if (puntoValido == null)
            resultado += "El punto de venta ingresado no es correcto, ";
        else
            facturas.IDPuntoDeVenta = puntoValido.IDPuntoVenta;

        if (Provincia == null)
            resultado += "La Provincia ingresada no es correcta, ";
        else
            facturas.IDProvincia = Provincia.IDProvincia;

        if (ciudad != null)
            facturas.IDCiudad = ciudad.IDCiudad;
        else
            facturas.IDCiudad = ciudades.Where(w => w.IDProvincia == facturas.IDProvincia).Select(s => s.IDCiudad).FirstOrDefault();

        if (concepto == null)
            resultado += "El concepto ingresado no existe, ";
        else
            facturas.CodigoConcepto = concepto.IDConcepto.ToString();

        if (usu.UsaPlanCorporativo)
        {
            if (planDeCuenta == null)
                resultado += "El codigo de la cuenta contable ingresado no es correcta, ";
            else
                facturas.idPlanDeCuenta = planDeCuenta.IDPlanDeCuenta;
        }
        
        #endregion
        return resultado;
    }

    public static void RealizarImportacionFacturas(List<FacturasCSVTmp> lista, string ACHEString)
    {
        if (lista != null && lista.Any(x => x.Estado == "A"))
        {
            //IMPORTAR
            DataTable dt = new DataTable();

            dt.Columns.Add("IDUsuario", typeof(Int32));//0
            dt.Columns.Add("Tipo", typeof(string));//0

            dt.Columns.Add("RazonSocial", typeof(string));//1
            dt.Columns.Add("NombreFantasia", typeof(string));//2
            dt.Columns.Add("Email", typeof(string));//3
            dt.Columns.Add("TipoDocumento", typeof(string));//4
            dt.Columns.Add("NroDocumento", typeof(string));//5

            dt.Columns.Add("CondicionIva", typeof(string));//6
            dt.Columns.Add("Provincia", typeof(Int32));//7
            dt.Columns.Add("Ciudad", typeof(Int32));//8
            dt.Columns.Add("Domicilio", typeof(string));//9
            dt.Columns.Add("Web", typeof(string));//10
            dt.Columns.Add("Personeria", typeof(string));//10

            foreach (var facturas in lista.Where(x => x.Estado == "A").ToList())
            {
                #region Armo DataRow
                try
                {
                    //if (!facturas.RazonSocial.Equals("CONSUMIDOR FINAL") && facturas.TipoDocumento.Equals("DNI") && facturas.NroDocumento.Equals(""))
                    //{
                        DataRow drNew = dt.NewRow();
                        drNew[0] = facturas.IDUsuario;
                        drNew[1] = facturas.Tipo;

                        drNew[2] = facturas.RazonSocial;
                        drNew[3] = facturas.NombreFantasia;
                        drNew[4] = facturas.Email;
                        drNew[5] = facturas.TipoDocumento;
                        drNew[6] = facturas.NroDocumento;

                        drNew[7] = facturas.CondicionIva;
                        drNew[8] = facturas.IDProvincia;
                        drNew[9] = facturas.IDCiudad;
                        drNew[10] = facturas.Domicilio;
                        drNew[11] = facturas.Web;
                        drNew[12] = facturas.Personeria;
                        dt.Rows.Add(drNew);
                    //}
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                #endregion
            }

            if (dt.Rows.Count > 0)
            {
                using (var sqlConnection = new SqlConnection(ACHEString))
                {
                    sqlConnection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                    {
                        bulkCopy.DestinationTableName = "PersonasTemp";
                        foreach (DataColumn col in dt.Columns)
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                        bulkCopy.WriteToServer(dt);
                    }
                }
            }

            using (var dbContext = new ACHEEntities())
            {
                dbContext.ObtenerDatosPersonasTmp();
            }
        }
        else
            throw new Exception("No se encontraron datos para importar");
    }
    public static void ReferenciarFacturasClientes(List<FacturasCSVTmp> listaFacturas, WebUser usu)
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                var listaPersonas = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).ToList();
                List<FacturasCSVTmp> facturas = new List<FacturasCSVTmp>();
                foreach (var item in listaPersonas)
                {
                    if (string.IsNullOrWhiteSpace(item.NroDocumento))
                        facturas = listaFacturas.Where(x => x.RazonSocial.ToUpper() == item.RazonSocial.ToUpper() && x.TipoDocumento == item.TipoDocumento && x.NroDocumento == item.NroDocumento && x.Estado == "A").ToList();
                    else
                        facturas = listaFacturas.Where(x => x.TipoDocumento == item.TipoDocumento && x.NroDocumento == item.NroDocumento && x.Estado == "A").ToList();

                    foreach (var fc in facturas)
                        fc.IDPersona = item.IDPersona;

                    GuardarFacturas(dbContext, facturas, usu);
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

    private static void GuardarFacturas(ACHEEntities dbContext, List<FacturasCSVTmp> listaFacturas, WebUser usu)
    {
        try
        {
            Usuarios usuario = dbContext.Usuarios.Where(w => w.IDUsuario == usu.IDUsuario).FirstOrDefault();
            List<PuntosDeVenta> puntosDeVenta = dbContext.PuntosDeVenta.Where(w => w.IDUsuario == usu.IDUsuario).ToList();
            List<Conceptos> conceptos = dbContext.Conceptos.Where(w => w.IDUsuario == usu.IDUsuario).ToList();
            List<Personas> personas = dbContext.Personas.Where(w => w.IDUsuario == usu.IDUsuario).ToList();
            List<UsuariosAdicionales> usuariosAdicionales = dbContext.UsuariosAdicionales.Where(w => w.IDUsuario == usu.IDUsuario).ToList();

            foreach (var item in listaFacturas.Where(x => x.Estado == "A"))
            {
                var TipoConcepto = "2";
                var IDJuresdiccion = 0;
                var CondicionVenta = "Efectivo";
                ComprobanteCartDto compCart = new ComprobanteCartDto();
                compCart.Items = new List<ComprobantesDetalleViewModel>();
                compCart.IDComprobante = 0;
                compCart.IDPersona = item.IDPersona;
                compCart.TipoComprobante = item.TipoComprobante;
                compCart.TipoConcepto = TipoConcepto;
                compCart.IDUsuario = item.IDUsuario;
                compCart.Modo = item.Modo;
                compCart.FechaComprobante = Convert.ToDateTime(item.Fecha);
                compCart.FechaVencimiento = Convert.ToDateTime(item.Fecha).AddDays(30).Date;
                compCart.IDPuntoVenta = item.IDPuntoDeVenta;

                compCart.Numero = item.NroComprobante;
                //if (!string.IsNullOrEmpty(item.CAE))
                //{
                //    compCart.Numero = item.NroComprobante;
                //}
                //else
                //{
                //    string numeroNumeroComprobante = ComprobantesCommon.ObtenerProxNroComprobante(item.TipoComprobante,usu.IDUsuario, item.IDPuntoDeVenta);
                //    compCart.Numero = numeroNumeroComprobante;
                //}   
                
                compCart.Observaciones = item.Observaciones;
                compCart.CondicionVenta = CondicionVenta;
                compCart.IDJuresdiccion = IDJuresdiccion;
                compCart.Items = new List<ComprobantesDetalleViewModel>();
                compCart.ImporteNoGravado = Math.Abs(Convert.ToDecimal(item.ImporteNoGravado.Replace(SeparadorDeMiles, SeparadorDeDecimales)));
                compCart.PercepcionIVA = Math.Abs(Convert.ToDecimal(item.PercepcionesIVA.Replace(SeparadorDeMiles, SeparadorDeDecimales)));
                compCart.PercepcionIIBB = Math.Abs(Convert.ToDecimal(item.PercepcionesIIBB.Replace(SeparadorDeMiles, SeparadorDeDecimales)));
                compCart.Items = item.Items;
                compCart.IDActividad = 0;

                Comprobantes entity;
                if (compCart.Numero.Equals("0") && string.IsNullOrEmpty(item.CAE))
                {
                    entity = ComprobantesCommon.GuardarAltaMasiva(dbContext, compCart,
                                        usuario, personas, puntosDeVenta, conceptos, usuariosAdicionales);
                }
                else
                {
                    entity = ComprobantesCommon.Guardar(dbContext, compCart);
                }                             

                if (Convert.ToDecimal(item.MontoPagado) > 0)
                    GuardarCobranza(dbContext, entity, item, usu);
            }
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message);
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), "GuardarFacturas" + msg, ex.ToString());

            //BasicLog.AppendToFile(ConfigurationManager.AppSettings["PathBaseWeb"] + ConfigurationManager.AppSettings["BasicLogError"].Replace("~", ""), "GuardarFacturas" + msg, ex.ToString());
            throw new Exception(ex.Message);
        }
    }
    private static void GuardarCobranza(ACHEEntities dbContext, Comprobantes comp, FacturasCSVTmp csvTemp, WebUser usu)
    {
        try
        {
            var punto = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == comp.IDPuntoVenta).FirstOrDefault();
            CobranzaCartDto cobrCartdto = new CobranzaCartDto();
            cobrCartdto.IDCobranza = 0;
            cobrCartdto.IDPersona = csvTemp.IDPersona;
            cobrCartdto.Tipo = "RC";
            cobrCartdto.Fecha = DateTime.Now.Date.ToString(formatoFecha);
            cobrCartdto.IDPuntoVenta = comp.IDPuntoVenta;
            cobrCartdto.NumeroCobranza = CobranzasCommon.obtenerProxNroCobranza(cobrCartdto.Tipo, usu.IDUsuario);
            cobrCartdto.Observaciones = "";


            cobrCartdto.Items = new List<CobranzasDetalleViewModel>();
            cobrCartdto.FormasDePago = new List<CobranzasFormasDePagoViewModel>();
            cobrCartdto.Retenciones = new List<CobranzasRetencionesViewModel>();

            cobrCartdto.Items.Add(new CobranzasDetalleViewModel()
            {
                ID = 1,
                Comprobante = comp.Tipo + " " + punto.Punto.ToString("#0000") + "-" + comp.Numero.ToString("#00000000"),
                Importe = comp.Saldo,
                IDComprobante = comp.IDComprobante
            });
            cobrCartdto.FormasDePago.Add(new CobranzasFormasDePagoViewModel()
            {
                ID = 1,
                Importe = Convert.ToDecimal(csvTemp.MontoPagado.Replace(SeparadorDeMiles, SeparadorDeDecimales)),
                FormaDePago = "Efectivo",
                NroReferencia = ""
            });

            var entity = CobranzasCommon.Guardar(cobrCartdto, usu);
        }
        catch (CustomException ex)
        {
            throw new CustomException(ex.Message);
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            BasicLog.AppendToFile(ConfigurationManager.AppSettings["BasicLogError"], "GuardarCobranza" + msg, ex.ToString());
            throw new Exception(ex.Message);
        }
    }
    private static string ObtenerConceptos(List<Conceptos> conceptos, FacturasCSVTmp facturas, WebUser usu, string resultado)
    {
        /* Items */
        if (string.IsNullOrWhiteSpace(resultado))
        {
            var importeNeto = Convert.ToDecimal(facturas.ImporteNeto.Replace(SeparadorDeMiles, SeparadorDeDecimales));

            var listaItems = new List<ComprobantesDetalleViewModel>();
            
            if (usu.CondicionIVA == "MO" || usu.CondicionIVA == "EX")
            {
                listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, importeNeto, 3, facturas.idPlanDeCuenta));
            }
            else if (usu.CondicionIVA == "RI")
            {
                var iva2700 = Convert.ToDecimal(facturas.IVA2700.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                var iva2100 = Convert.ToDecimal(facturas.IVA2100.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                var iva1050 = Convert.ToDecimal(facturas.IVA1005.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                var iva0500 = Convert.ToDecimal(facturas.IVA0500.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                var iva2050 = Convert.ToDecimal(facturas.IVA0205.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                var iva0000 = Convert.ToDecimal(facturas.IVA0000.Replace(SeparadorDeMiles, SeparadorDeDecimales));
                var importeNoGravado = Convert.ToDecimal(facturas.ImporteNoGravado.Replace(SeparadorDeMiles, SeparadorDeDecimales));

                if (iva2700 > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, iva2700, 6, facturas.idPlanDeCuenta));
                if (iva2100 > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, iva2100, 5, facturas.idPlanDeCuenta));
                if (iva1050 > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, iva1050, 4, facturas.idPlanDeCuenta));
                if (iva0500 > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, iva0500, 8, facturas.idPlanDeCuenta));
                if (iva2050 > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, iva2050, 9, facturas.idPlanDeCuenta));
                if (iva0000 > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, iva0000, 3, facturas.idPlanDeCuenta));
                if (importeNoGravado > 0)
                    listaItems.Add(AgregarItem(conceptos, facturas.CodigoConcepto, importeNoGravado, 1, facturas.idPlanDeCuenta));
            }

            /* percepciones */
            var PercepcionesIIBB = Convert.ToDecimal(facturas.PercepcionesIIBB.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            var PercepcionesIVA = Convert.ToDecimal(facturas.PercepcionesIVA.Replace(SeparadorDeMiles, SeparadorDeDecimales));
            var ImporteNoGravado = Convert.ToDecimal(facturas.ImporteNoGravado.Replace(SeparadorDeMiles, SeparadorDeDecimales));


            var PIIBB = (importeNeto * PercepcionesIIBB) / 100;
            var PIVA = (importeNeto * PercepcionesIVA) / 100;
            var Tributos = PIIBB + PIVA + ImporteNoGravado;

            //if (listaItems.Sum(x => x.TotalConIva) + Tributos != Convert.ToDecimal(facturas.Total))
            //    resultado += "No coinciden el importe total Con la suma de los conceptos, ";
            //else
                facturas.Items = listaItems;
        }
        return resultado;
    }
    private static ComprobantesDetalleViewModel AgregarItem(List<Conceptos> conceptos, string codigoConcepto, decimal importeConIVA, int idTipoIva, int idPlanDeCuenta)
    {
        decimal importe = 0;



        if (obtenerValorIVA(idTipoIva) > 0)
            importe = importeConIVA * 100 / obtenerValorIVA(idTipoIva);
            //importe = Math.Abs(importeConIVA - ((importeConIVA * obtenerValorIVA(idTipoIva)) / 100));
        else
            importe = Math.Abs(importeConIVA);

        var dbContext = new ACHEEntities();
        int idConcepto = Convert.ToInt32(codigoConcepto);
        Conceptos concepto = conceptos.Where(x => x.IDConcepto == idConcepto).FirstOrDefault(); 

        var Item = new ComprobantesDetalleViewModel()
        {
            Codigo = concepto.Codigo,
            PrecioUnitario = Math.Abs(importe),
            Iva = obtenerValorIVA(idTipoIva),
            IdTipoIva = idTipoIva,
            Concepto = concepto.Nombre,
            Cantidad = 1,
            Bonificacion = 0,
            IDPlanDeCuenta = idPlanDeCuenta,
            IDConcepto = idConcepto
        };

        return Item;
    }
    private static decimal obtenerValorIVA(int idTipoIVA)
    {
        switch (idTipoIVA)
        {
            case 1:
                return 0;
            case 2:
                return 0;
            case 3:
                return 0;
            case 4:
                return Convert.ToDecimal(10.5);
            case 5:
                return 21;
            case 6:
                return 27;
            case 8:
                return 5;
            case 9:
                return Convert.ToDecimal(2.5);
            default:
                return 0;
        }
    }
    #endregion
}