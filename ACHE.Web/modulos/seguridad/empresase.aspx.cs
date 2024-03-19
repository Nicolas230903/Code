using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ACHE.Extensions;

public partial class empresase : BasePage
{
    static int idEmpresa { get; set; }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        var currentUsu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        if (!currentUsu.TieneMultiEmpresa)
        {
            Response.Redirect("~/home.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                txtEmail.Enabled = true;
                litPath.Text = (String.IsNullOrEmpty(Request.QueryString["ID"])) ? "Alta" : "Edición";
                IDEmpresa.Value = "0";

                using (var dbContext = new ACHEEntities())
                {
                    idEmpresa = Convert.ToInt32(Request.QueryString["ID"]);
                    IDEmpresa.Value = idEmpresa.ToString();
                    var usu = dbContext.Usuarios.Where(x => x.IDUsuario == idEmpresa).FirstOrDefault();
                    if (usu != null)
                    {
                        txtRazonSocial.Text = usu.RazonSocial;
                        ddlCondicionIva.SelectedValue = usu.CondicionIva;
                        txtCuit.Text = usu.CUIT;
                        txtIIBB.Text = usu.IIBB;
                        if (usu.FechaInicioActividades.HasValue)
                            txtFechaInicioAct.Text = usu.FechaInicioActividades.Value.ToString("dd/MM/yyyy");
                        ddlPersoneria.SelectedValue = usu.Personeria;
                        txtEmail.Text = usu.Email;
                        txtEmailAlertas.Text = usu.EmailAlertas;
                        txtContacto.Text = usu.Contacto;
                        txtCelular.Text = usu.Celular;

                        //Domicilio
                        //ddlProvincia.SelectedValue = usu.IDProvincia.ToString();
                        //txtCiudad.Text = usu.IDCiudad.ToString();
                        hdnProvincia.Value = usu.IDProvincia.ToString();
                        hdnCiudad.Value = usu.IDCiudad.ToString();

                        txtDomicilio.Text = usu.Domicilio;
                        txtPisoDepto.Text = usu.PisoDepto;
                        txtCp.Text = usu.CodigoPostal;
                        txtTelefono.Text = usu.Telefono;

                        litRazonSocial.Text = usu.RazonSocial;

                        chkPortalClientes.Checked = Convert.ToBoolean(usu.PortalClientes);
                        ChkCorreoPortal.Checked = Convert.ToBoolean(usu.CorreoPortal);

                        if (string.IsNullOrWhiteSpace(usu.Logo))
                        {
                            imgLogo.Src = "/files/usuarios/no-photo.png";
                        }
                        else
                        {
                            imgLogo.Src = "/files/usuarios/" + usu.Logo;
                        }



                        if (usu.Domicilio != string.Empty)
                            idLocation.InnerText = usu.Domicilio + ", " + usu.Ciudades.Nombre + ", " + usu.Provincias.Nombre;
                        else
                            idLocation.InnerText = "Domicilio fiscal no informado";
                        idPosition.InnerText = "CUIT: " + usu.CUIT;
                        txtEmail.Enabled = false;

                        if (usu.TemplateFc == "default")
                        {
                            default1.Checked = true;
                        }
                        else if (usu.TemplateFc == "default2")
                        {
                            default2.Checked = true;
                        }
                        else
                        {
                            default3.Checked = true;
                        }
                    }
                }
            }
        }
    }

    [WebMethod(true)]
    public static void guardar(string razonSocial, string condicionIva, string cuit, string iibb, string fechaInicio,
        string personeria, string email, string emailAlertas,
        string telefono, string celular, string contacto,
        string idProvincia, string idCiudad, string domicilio, string pisoDepto, string cp)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (!cuit.IsValidCUIT())
                    throw new Exception("El CUIT ingresado es incorrecto");
                else
                {
                    Usuarios entity;

                    if (dbContext.Usuarios.Any(x => x.Email == email && x.IDUsuario != idEmpresa))
                        throw new Exception("El E-mail ingresado ya se encuentra registrado.");
                    else if (dbContext.Usuarios.Any(x => x.CUIT == cuit && x.IDUsuario != idEmpresa))
                        throw new Exception("El CUIT ingresado ya se encuentra registrado.");
                    if (dbContext.UsuariosAdicionales.Any(x => x.Email == email))
                        throw new Exception("El E-mail ingresado ya se encuentra registrado.");
                    else
                    {

                        if (idEmpresa > 0)
                            entity = entity = dbContext.Usuarios.Where(x => x.IDUsuario == idEmpresa).FirstOrDefault();
                        else
                        {
                            var currentUsuario = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                            
                            entity = new Usuarios();
                            entity.FechaAlta = DateTime.Now;
                            entity.IDUsuarioPadre = usu.IDUsuario;
                            entity.Theme = "default";
                            entity.Pais = "Argentina";
                            entity.TemplateFc = "default";
                            entity.Pwd = "";
                            entity.TieneFacturaElectronica = false;
                            entity.Logo = null;
                            entity.Activo = true;
                            entity.IDPlan = 3;
                            entity.FechaUltLogin = DateTime.Now;
                            entity.SetupRealizado = true;
                            entity.CorreoPortal = currentUsuario.CorreoPortal;
                            entity.PortalClientes = currentUsuario.PortalClientes;
                            entity.FechaFinPlan = DateTime.Now.AddDays(30);
                            entity.UsaProd = currentUsuario.UsaProd;
                            entity.UsaFechaFinPlan = false;
                            PuntosDeVenta punto = new PuntosDeVenta();
                            punto.FechaAlta = DateTime.Now;
                            punto.Punto = 1;
                            punto.PorDefecto = true;
                            entity.PuntosDeVenta.Add(punto);
                        }


                        entity.RazonSocial = razonSocial;
                        entity.FechaInicioActividades = DateTime.Parse(fechaInicio);
                        entity.CondicionIva = condicionIva;
                        entity.CUIT = cuit;
                        entity.IIBB = iibb;
                        entity.Personeria = personeria;
                        entity.Email = email;
                        entity.EmailAlertas = emailAlertas;
                        entity.Telefono = telefono;
                        entity.Celular = celular;
                        entity.Contacto = contacto;
                        //Domicilio
                        entity.IDProvincia = Convert.ToInt32(idProvincia);
                        entity.IDCiudad = Convert.ToInt32(idCiudad);
                        entity.Domicilio = domicilio;
                        entity.PisoDepto = pisoDepto;
                        entity.CodigoPostal = cp;

                        if (idEmpresa > 0)
                        {
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            dbContext.Usuarios.Add(entity);
                            dbContext.SaveChanges();
                            idEmpresa = entity.IDUsuario;
                        }

                    }
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void agregarPunto(int punto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                if (dbContext.PuntosDeVenta.Any(x => x.Punto == punto && x.IDUsuario == idEmpresa))
                    throw new Exception("Ya existe el punto de venta");

                PuntosDeVenta entity = new PuntosDeVenta();
                entity.Punto = punto;
                entity.IDUsuario = idEmpresa;
                entity.FechaAlta = DateTime.Now;
                entity.PorDefecto = false;
                dbContext.PuntosDeVenta.Add(entity);
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarPunto(int punto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                PuntosDeVenta entity = dbContext.PuntosDeVenta.Where(x => x.IDPuntoVenta == punto && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                    entity.FechaBaja = DateTime.Now;

                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerPuntos()
    {
        var html = string.Empty;
        if (idEmpresa != 0)
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                using (var dbContext = new ACHEEntities())
                {
                    var list = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == idEmpresa).ToList();
                    if (list.Any())
                    {
                        int index = 1;
                        foreach (var punto in list)
                        {
                            html += "<tr>";
                            html += "<td>" + index + "</td>";
                            html += "<td>" + punto.Punto.ToString("#0000") + "</td>";
                            html += "<td>" + punto.FechaAlta.ToString("dd/MM/yyyy") + "</td>";
                            if (punto.FechaBaja.HasValue)
                                html += "<td>" + punto.FechaBaja.Value.ToString("dd/MM/yyyy") + "</td>";
                            else
                                html += "<td></td>";
                            html += "<td>" + ((punto.PorDefecto) ? "SI" : "NO") + "</td>";

                            if (!punto.PorDefecto && !punto.FechaBaja.HasValue)
                                html += "<td><a href='#' title='Dar de baja' style='font-size: 16px;' onclick='Empresa.eliminarPunto(" + punto.IDPuntoVenta + ");'><i class='fa fa-times'></i></a> <a href='#' title='Poner por defecto' style='font-size: 16px;' onclick='Empresa.ponerPorDefecto(" + punto.IDPuntoVenta + ");'><i class='fa fa-check' style='color: green;'></i></a></td>";
                            else
                                html += "<td><a href='#' title='Dar de baja' style='font-size: 16px;' onclick='Empresa.eliminarPunto(" + punto.IDPuntoVenta + ");'><i class='fa fa-times'></i></a></td>";
                            index++;
                            html += "</tr>";
                        }
                    }
                }
                if (html == "")
                    html = "<tr><td colspan='5' style='text-align:center'>No tienes puntos de venta registrados</td></tr>";

            }
        }
        return html;
    }

    [WebMethod(true)]
    public static string GuardarPorDefecto(int idPunto)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var ListaPuntos = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == idEmpresa).ToList();

                foreach (var item in ListaPuntos)
                {
                    if (item.IDPuntoVenta == idPunto)
                        item.PorDefecto = true;
                    else
                        item.PorDefecto = false;
                }
                dbContext.SaveChanges();
            }

            return obtenerPuntos();
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static ResultadosEmpresasViewModel ObtenerDatosEmpresa()
    {
        using (var dbContext = new ACHEEntities())
        {
            var results = dbContext.Usuarios.Where(x => x.IDUsuario== idEmpresa).AsQueryable();
           

            var list = results.Select(x => new EmpresasViewModel()
            {
                ID = x.IDUsuario,
                RazonSocial = x.RazonSocial.ToUpper(),
                CUIT = x.CUIT,
                Domicilio = x.Domicilio,
                Ciudad =x.Ciudades.Nombre,
                Provincia =x.Provincias.Nombre,
                CorreoPortal = x.CorreoPortal,
                PortalClientes = x.PortalClientes
            });

            ResultadosEmpresasViewModel resultado = new ResultadosEmpresasViewModel();
            resultado.Items = list.OrderBy(x => x.RazonSocial).ToList();

            return resultado;
        }
    }

    [WebMethod(true)]
    public static void portalClientes(bool ChkCorreoPortal, bool chkPortalClientes)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                Usuarios entity = dbContext.Usuarios.Where(x => x.IDUsuario == idEmpresa).FirstOrDefault();

                entity.CorreoPortal = ChkCorreoPortal;
                entity.PortalClientes = chkPortalClientes;

                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void ActualizarTemplate(string ddlTemplate)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            using (var dbContext = new ACHEEntities())
            {
                Usuarios entity = dbContext.Usuarios.Where(x => x.IDUsuario == idEmpresa).FirstOrDefault();

                entity.TemplateFc = ddlTemplate;
                dbContext.SaveChanges();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}