using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Negocio.Common;
using ACHE.FacturaElectronica;
using System.Configuration;
using ACHE.FacturaElectronica.WSPersonaServiceA5;
using System.Xml.Serialization;
using System.Xml;
using ACHE.Negocio.Facturacion;

public partial class personase : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var tipo = Request.QueryString["tipo"];
            hdnTipo.Value = tipo;

            if (CurrentUser.TipoUsuario == "B")
            {
                if (!PermisosModulos.mostrarPersonaSegunPermiso(tipo))
                    Response.Redirect("home.aspx");
            }
            if (tipo == "c")
            {
                using (var dbContext = new ACHEEntities())
                {
                    AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                    if (afu != null)
                        if (!afu.ComercialClientes)
                            Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                }

                litTitulo.Text = "<i class='fa fa-suitcase'></i> Clientes";
                litPathPadre.Text = "<a href='/personas.aspx?tipo=c'>Clientes</a>";
                liDatosGenerales.Text = "clientes";
                libtnGuardar.Text = "Guardar cliente";
            }
            else if (tipo == "p")
            {
                using (var dbContext = new ACHEEntities())
                {
                    AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                    if (afu != null)
                        if (!afu.SuministroProveedores)
                            Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

                }


                litTitulo.Text = "<i class='fa fa-users'></i> Proveedores";
                litPathPadre.Text = "<a href='/personas.aspx?tipo=p'>Proveedores</a>";
                liDatosGenerales.Text = "proveedor";
                libtnGuardar.Text = "Guardar proveedor";
            }
            else
                Response.Redirect("home.aspx");

            hdnIDUsuario.Value = CurrentUser.IDUsuario.ToString();
            litPath.Text = "Alta";
            litImporteFacturado.Text = litSaldoPendiente.Text = litImportePagado.Text = "0,00";

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";
                }
            }
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Personas.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPersona == id).FirstOrDefault();
            if (entity != null)
            {
                txtRazonSocial.Text = entity.RazonSocial.ToUpper();
                txtNombreFantasia.Text = entity.NombreFantansia.ToUpper();
                ddlCondicionIva.SelectedValue = entity.CondicionIva;
                ddlTipoDoc.SelectedValue = entity.TipoDocumento;
                txtNroDocumento.Text = entity.NroDocumento;
                ddlPersoneria.SelectedValue = entity.Personeria;
                txtEmail.Text = entity.Email.ToLower();
                txtTelefono.Text = entity.Telefono;
                //txtCelular.Text = entity.Celular;
                //txtWeb.Text = entity.Web.ToLower();
                txtObservaciones.Text = entity.Observaciones;
                txtCodigo.Text = entity.Codigo;
                txtSaldo.Text = Convert.ToDecimal(entity.SaldoInicial).ToString("").Replace(",", ".");

                //Domicilio
                hdnProvincia.Value = entity.IDProvincia.ToString();
                hdnCiudad.Value = entity.IDCiudad.ToString();
                txtProvinciaDesc.Text = entity.ProvinciaDesc != null ? entity.ProvinciaDesc.ToUpper() : "";
                txtCiudadDesc.Text = entity.CiudadDesc != null ? entity.CiudadDesc.ToUpper() : "";
                txtDomicilio.Text = entity.Domicilio.ToUpper();
                hdnDireccion.Value = entity.Domicilio.ToLower() + ", " + entity.Provincias.Nombre;
                txtPisoDepto.Text = entity.PisoDepto;
                txtCp.Text = entity.CodigoPostal;
                txtPorcentajeDescuento.Text = Convert.ToDecimal(entity.PorcentajeDescuento).ToString("").Replace(",", ".");
                txtAvisos.Text = entity.Avisos != null ? entity.Avisos : "";
                ddlRango.SelectedValue = entity.IdRango.ToString();

                //txtBanco.Text = entity.Banco;
                //txtCbu.Text = entity.CBU;
                //txtContacto.Text = entity.Contacto;

                //Lista de Precios
                if (entity.ListaPrecios != null)
                    hdnIDListaPrecio.Value = entity.ListaPrecios.IDListaPrecio.ToString();

                litImporteFacturado.Text = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPersona == id)
                   .Select(x => x.ImporteTotalNeto).DefaultIfEmpty(0).Sum().ToString("N2");

                var aFavor = dbContext.Comprobantes.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPersona == id && x.Saldo > 0)
                   .Select(x => x.Saldo).DefaultIfEmpty(0).Sum();
                var enContra = dbContext.Compras.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPersona == id && x.Saldo > 0)
                   .Select(x => x.Saldo).DefaultIfEmpty(0).Sum();
                litSaldoPendiente.Text = (aFavor - enContra).ToString("N2");

                litImportePagado.Text = dbContext.Compras.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDPersona == id)
                   .Select(x => x.Total.Value).DefaultIfEmpty(0).Sum().ToString("N2");

                if (!string.IsNullOrWhiteSpace(entity.Foto))
                {
                    imgFoto.Src = "/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/Contactos/" + entity.Foto;
                    hdnTieneFoto.Value = "1";
                }
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, string razonSocial, string nombreFantasia, string condicionIva, string personeria, string tipoDoc, string nroDoc,
        string telefono, string email, string tipo,
        int idProvincia, int idCiudad, string provinciaDesc, string ciudadDesc, string domicilio, string pisoDepto, 
        string cp, string obs, int listaPrecio, string codigo, decimal saldoInicial, decimal porcentajeDescuento, string avisos, int idRango)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            if (nombreFantasia.Equals(""))
                nombreFantasia = razonSocial;

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var idPersona = PersonasCommon.GuardarPersonas(id, razonSocial, nombreFantasia, condicionIva, personeria, tipoDoc, nroDoc, telefono,
                email, tipo, idProvincia, idCiudad, provinciaDesc, ciudadDesc, 
                domicilio, pisoDepto, cp, obs, listaPrecio, codigo, saldoInicial, porcentajeDescuento, avisos, idRango, usu);

            return idPersona;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int guardarDomicilio(int id, int idPersona, string domicilio, string pisoDepto,
        string codigoPostal, int idProvincia, int idCiudad,
        string provinciaTexto, string ciudadTexto, string contacto, string telefono)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        { 

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var idPersonaDomicilio = PersonasCommon.GuardarPersonaDomicilio(id, idPersona, 
                                            idProvincia, idCiudad, domicilio, 
                                            pisoDepto, codigoPostal, 
                                            provinciaTexto, ciudadTexto, contacto, telefono,
                                            usu);

            return idPersonaDomicilio;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int guardarDomicilioTransporte(int id, int idPersona, string razonSocial, string domicilio, string pisoDepto,
    string codigoPostal, string provinciaTexto, string ciudadTexto, string contacto, string telefono)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var idPersonaDomicilioTransporte = PersonasCommon.GuardarPersonaDomicilioTransporte(id, idPersona,
                                            razonSocial, domicilio,
                                            pisoDepto, codigoPostal,
                                            provinciaTexto, ciudadTexto, contacto, telefono,
                                            usu);

            return idPersonaDomicilioTransporte;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarDomicilio(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                PersonasCommon.EliminarDomicilio(id, usu);
            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    public static void eliminarDomicilioTransporte(int id)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                PersonasCommon.EliminarDomicilioTransporte(id, usu);
            }
            else
                throw new CustomException("Por favor, vuelva a iniciar sesión");
        }
        catch (CustomException e)
        {
            throw new CustomException(e.Message);
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    public static void editarDescuento(int idPersona, decimal porcentajeDescuento)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                var p = dbContext.Personas.Where(w => w.IDPersona == idPersona).FirstOrDefault();
                if(p != null)
                {
                    p.PorcentajeDescuento = porcentajeDescuento;
                    dbContext.SaveChanges();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static PersonasViewModel obtenerDatos(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Personas entity = dbContext.Personas.Where(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    PersonasViewModel result = new PersonasViewModel();
                    result.ID = id;
                    result.RazonSocial = entity.RazonSocial.ToUpper();
                    result.Email = entity.Email.ToLower();
                    result.CondicionIva = entity.CondicionIva;
                    result.Domicilio = entity.Domicilio.ToUpper() + " " + entity.PisoDepto;
                    result.ProvinciaDesc = (entity.ProvinciaDesc == null) ? "" : entity.ProvinciaDesc;
                    result.CiudadDesc = (entity.CiudadDesc == null) ? "" : entity.CiudadDesc;
                    result.Ciudad = (entity.Ciudades == null) ? "" : entity.Ciudades.Nombre.ToUpper();
                    result.Provincia = (entity.Provincias == null) ? "" : entity.Provincias.Nombre;
                    result.TipoDoc = entity.TipoDocumento;
                    result.NroDoc = entity.NroDocumento;
                    result.PorcentajeDescuento = entity.PorcentajeDescuento;
                    result.Tipo = entity.Tipo.ToLower();
                    result.Telefono = (entity.Telefono == null) ? "" : entity.Telefono;
                    result.Avisos = (entity.Avisos == null) ? "" : entity.Avisos;
                    
                    return result;
                }
                else
                    throw new Exception("Error al obtener los datos");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerComprobantes(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.Comprobantes.Where(x => x.IDPersona == id && x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.FechaComprobante)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDComprobante,
                        Nombre = x.Tipo + " " + x.Numero.ToString("#00000000").ToUpper()
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarFoto(int idPersona)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Contactos/" + entity.Foto);

                if (File.Exists(Serverpath))
                {
                    File.Delete(Serverpath);

                    entity.Foto = "";
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new Exception("El cheque no tiene una imagen guardada");
                }
            }
        }
    }    
}