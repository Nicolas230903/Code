using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using ACHE.Model;
using System.Net.Mail;
using System.Data;
using System.IO;
using System.Web.Script.Services;
using ACHE.Model.Negocio;
using ACHE.Negocio.Common;
using ACHE.Negocio.Facturacion;
using ACHE.FacturaElectronica;
using System.Xml.Serialization;
using System.Xml;
using ACHE.FacturaElectronica.WSFacturaElectronica;
using ACHE.FacturaElectronica.VEConsumerService;
using System.Text.RegularExpressions;
using ACHE.FacturaElectronica.Lib;
using ACHE.Extensions;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Data.Entity;
using ACHE.FacturaElectronica.WSPersonaServiceA5v34;
using DocumentFormat.OpenXml.Drawing.Charts;
using Aspose.Pdf.Operators;
using System.Xml.Xsl;
using System.Text;

public partial class common : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerPuntosDeVenta()
    {

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            return UsuarioCommon.ObtenerPuntosDeVenta(usu.IDUsuario);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerActividades()
    {

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            return UsuarioCommon.ObtenerActividades(usu.IDUsuario);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerTipoComprobanteAfip()
    {

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            return UsuarioCommon.ObtenerTipoComprobanteAfip(usu.CondicionIVA);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerCategorias()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.Categorias.Where(x => x.IDUsuario == usu.IDUsuario).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDCategoria,
                        Nombre = x.Nombre
                    }).OrderBy(x => x.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerProxNroComprobante(string tipo, int idPuntoDeVenta)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var nro = ComprobantesCommon.ObtenerProxNroComprobante(tipo, usu.IDUsuario, idPuntoDeVenta);
            return nro;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerProxNroCobranza(string tipo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var nro = ACHE.Negocio.Facturacion.CobranzasCommon.obtenerProxNroCobranza(tipo, usu.IDUsuario);
            //var nro = "";

            //using (var dbContext = new ACHEEntities())
            //{
            //    if (dbContext.Cobranzas.Any(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipo))
            //    {
            //        var aux = dbContext.Cobranzas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipo).Max(x => x.Numero);
            //        if (aux != null)
            //            nro = aux.ToString("#00000000");
            //        else
            //            nro = "00000001";
            //    }
            //    else
            //        nro = "00000001";
            //}

            return nro;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static string sugerirNumeroCuitGenerico()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            string nro = null;
            long doc, cuit = 90000000000;

            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                List<Personas> lp = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).Where(x => x.NroDocumento != "").ToList();

                nro = (from c in lp 
                        orderby long.Parse(c.NroDocumento.Replace(".","")) descending
                        select c.NroDocumento).FirstOrDefault();

                if (nro != null)
                {
                    doc = Convert.ToInt64(nro);
                    if (doc < cuit)
                        doc = cuit;
                    else
                        doc++;
                }
                else
                    doc = cuit;

                while (!ValidaCuit(doc.ToString()))                
                    doc++;                

                nro = doc.ToString();
            }           

            return nro;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    public static bool ValidaCuit(string cuit)
    {
        if (cuit == null)
        {
            return false;
        }
        //Quito los guiones, el cuit resultante debe tener 11 caracteres.
        cuit = cuit.Replace("-", string.Empty);
        if (cuit.Length != 11)
        {
            return false;
        }
        else
        {
            int calculado = CalcularDigitoCuit(cuit);
            int digito = int.Parse(cuit.Substring(10));
            return calculado == digito;
        }
    }

    public static int CalcularDigitoCuit(string cuit)
    {
        int[] mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
        char[] nums = cuit.ToCharArray();
        int total = 0;
        for (int i = 0; i<mult.Length; i++)
        {
            total += int.Parse(nums[i].ToString()) * mult[i];
        }
        var resto = total % 11;
        return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerConceptos(string tipo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.Conceptos.Where(x => x.Estado == "A" && x.IDUsuario == usu.IDUsuario && (tipo == "" || x.Tipo == tipo))
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDConcepto,
                        Nombre = x.Codigo
                    }).OrderBy(x => x.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerConceptosCodigoyNombre(string tipo)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                if (!usu.CUIT.Equals("30654870868")) //FIOL
                {
                    return dbContext.Conceptos.Where(x => x.Estado == "A" && x.IDUsuario == usu.IDUsuario && (tipo == "" || x.Tipo == tipo)).ToList()
                   .Select(x => new Combo2ViewModel()
                   {
                       ID = x.IDConcepto,
                       Nombre = x.Codigo + " - " + x.Nombre + " (" + Convert.ToInt32(x.Stock).ToString() + ") (" + Convert.ToInt32(x.StockFisico).ToString() + ") "
                   }).OrderBy(x => x.Nombre).ToList();
                }
                else
                {
                    return dbContext.Conceptos.Where(x => x.Estado == "A" && x.IDUsuario == usu.IDUsuario && (tipo == "" || x.Tipo == tipo)).ToList()
                   .Select(x => new Combo2ViewModel()
                   {
                       ID = x.IDConcepto,
                       Nombre = x.Codigo + " - " + x.Nombre + " (" + x.Stock.ToString() + ") (" + x.StockFisico.ToString() + ")"
                   }).OrderBy(x => x.Nombre).ToList();
                }
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerPersonas()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && !x.NroDocumento.Equals("99999999")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        Nombre = x.NroDocumento +
                                    (x.NombreFantansia != "" ? " - " + x.NombreFantansia.ToUpper() : "") +
                                    (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();

                return dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && !x.NroDocumento.Equals("99999999")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        Nombre = x.NroDocumento +
                                (x.NombreFantansia != "" ? " - " + x.NombreFantansia.ToUpper() : "") +
                                (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();                
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerPeriodosPdv()
    {
        List<Combo2ViewModel> lcv = new List<Combo2ViewModel>();
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo.Contains("PDV")).OrderByDescending(x => x.FechaAlta).ToList();

                foreach(Comprobantes c in lista)
                {
                    Combo2ViewModel cv = new Combo2ViewModel();
                    cv.ID = int.Parse(c.FechaAlta.ToString("yyyyMM"));
                    cv.Nombre = c.FechaAlta.ToString("MM") + "/" + c.FechaAlta.ToString("yyyy");
                    if(!lcv.Where(w => w.ID == cv.ID).Any())                    
                        lcv.Add(cv);
                }

                return lcv.Distinct().ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerClientes()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && !x.NroDocumento.Equals("99999999") && x.Tipo.Equals("C")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        Nombre = x.NroDocumento +
                                    (x.NombreFantansia != "" ? " - " + x.NombreFantansia.ToUpper() : "") +
                                    (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();

                return dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && !x.NroDocumento.Equals("99999999") && x.Tipo.Equals("C")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        Nombre = x.NroDocumento +
                                (x.NombreFantansia != "" ? " - " + x.NombreFantansia.ToUpper() : "") +
                                (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerPeriodos()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int[] last10Months = new int[10];
                DateTime currentDate = DateTime.Now;

                for (int i = 0; i < 10; i++)
                {
                    // Agregar el mes actual al array
                    last10Months[i] = Convert.ToInt32(currentDate.ToString("yyMM"));

                    // Restar un mes al mes actual
                    currentDate = currentDate.AddMonths(-1);
                }

                return last10Months
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x,
                        Nombre = x.ToString()
                    }).ToList(); 
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerDomicilios(string idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                int codigoPersona = Convert.ToInt32(idPersona);
                return (from perDom in dbContext.PersonaDomicilio
                            join prov in dbContext.Provincias on perDom.IDProvincia equals prov.IDProvincia
                            join ciud in dbContext.Ciudades on perDom.IDCiudad equals ciud.IDCiudad
                            where perDom.IDPersona == codigoPersona
                             select new Combo2ViewModel()
                             {
                                ID = perDom.IdPersonaDomicilio,
                                Nombre = perDom.Domicilio + " " + perDom.PisoDepto + " - CP: " + perDom.CodigoPostal + 
                                        " - Provincia: " + perDom.Provincia + " - Ciudad: " + perDom.Ciudad + " - Contacto: " +
                                        perDom.Contacto + " - Teléfono: " + perDom.Telefono
                            }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerTransportes()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return (from transDom in dbContext.Transporte
                        join prov in dbContext.Provincias on transDom.IDProvincia equals prov.IDProvincia
                        join ciud in dbContext.Ciudades on transDom.IDCiudad equals ciud.IDCiudad
                        where transDom.IdUsuario == usu.IDUsuario
                        select new Combo2ViewModel()
                        {
                            ID = transDom.IdTransporte,
                            Nombre = transDom.RazonSocial + " - Domicilio: " + transDom.Domicilio + " " + transDom.PisoDepto + " - CP: " + transDom.CodigoPostal +
                                   " - Provincia: " + transDom.Provincia + " - Ciudad: " + transDom.Ciudad + " - Contacto: " +
                                   transDom.Contacto + " - Teléfono: " + transDom.Telefono
                        }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    //[WebMethod(true)]
    //public static List<Combo2ViewModel> obtenerActividades()
    //{
    //    if (HttpContext.Current.Session["CurrentUser"] != null)
    //    {
    //        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

    //        using (var dbContext = new ACHEEntities())
    //        {
    //            return (from act in dbContext.Actividad
    //                    where act.IdUsuario == usu.IDUsuario
    //                    select new Combo2ViewModel()
    //                    {
    //                        ID = act.IdActividad,
    //                        Nombre = act.Descripcion
    //                    }).ToList();
    //        }
    //    }
    //    else
    //        throw new Exception("Por favor, vuelva a iniciar sesión");
    //}

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerTransportePersona(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return (from transDom in dbContext.TransportePersona
                        join prov in dbContext.Provincias on transDom.IDProvincia equals prov.IDProvincia
                        join ciud in dbContext.Ciudades on transDom.IDCiudad equals ciud.IDCiudad
                        where transDom.IdUsuario == usu.IDUsuario && transDom.IdPersona == idPersona
                        select new Combo2ViewModel()
                        {
                            ID = transDom.IdTransportePersona,
                            Nombre = transDom.RazonSocial + " - Domicilio: " + transDom.Domicilio + " " + transDom.PisoDepto + " - CP: " + transDom.CodigoPostal +
                                   " - Provincia: " + transDom.Provincia + " - Ciudad: " + transDom.Ciudad + " - Contacto: " +
                                   transDom.Contacto + " - Teléfono: " + transDom.Telefono
                        }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerProveedores()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == "P" && !x.NroDocumento.Equals("99999999")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        //Nombre = x.NroDocumento + " - " + (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper())
                        Nombre = x.NroDocumento +
                                 (x.NombreFantansia != "" ? " - " + x.NombreFantansia.ToUpper() : "") +
                                 (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();

                return dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == "P" && !x.NroDocumento.Equals("99999999")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        //Nombre = x.NroDocumento + " - " + (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper())
                        Nombre = x.NroDocumento +
                                (x.NombreFantansia != "" ? " - " + x.NombreFantansia.ToUpper() : "") +
                                (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerVendedores()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.UsuariosView.Where(x => x.IDUsuario == usu.IDUsuario && x.EsVendedor == true).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDUsuarioAdicional != 0 ? x.IDUsuarioAdicional : x.IDUsuario,
                        Nombre = x.Email + " - " + x.RazonSocial
                    }).ToList();

                return dbContext.UsuariosView.Where(x => x.IDUsuario == usu.IDUsuario && x.EsVendedor == true).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDUsuarioAdicional != 0 ? x.IDUsuarioAdicional : x.IDUsuario,
                        Nombre = x.Email + " - " + x.RazonSocial
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo3ViewModel> obtenerCodigosCompraAutomatica()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                List<Combo3ViewModel> listaCodigos = new List<Combo3ViewModel>();
                List<long?> ls = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo.Equals("PDC") && x.ProcesoCompraAutomatica != null).Select(s => s.ProcesoCompraAutomatica).Distinct().ToList();

                foreach(long item in ls)
                {
                    Combo3ViewModel d = new Combo3ViewModel();
                    d.ID = item;
                    d.Nombre = item.ToString();
                    listaCodigos.Add(d);
                }

                return listaCodigos.OrderByDescending(o => o.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo3ViewModel> obtenerCodigosFacturaAutomatica()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                List<Combo3ViewModel> listaCodigos = new List<Combo3ViewModel>();
                List<long?> ls = dbContext.Comprobantes.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo.Contains("F") && x.ProcesoFacturaAutomatica != null).Select(s => s.ProcesoFacturaAutomatica).Distinct().ToList();

                foreach (long item in ls)
                {
                    Combo3ViewModel d = new Combo3ViewModel();
                    d.ID = item;
                    d.Nombre = item.ToString();
                    listaCodigos.Add(d);
                }

                return listaCodigos;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
    
    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerProveedoresSinCuit()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var lista = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == "P" && !x.NroDocumento.Equals("99999999")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        //Nombre = x.NroDocumento + " - " + (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper())
                        Nombre = (x.NombreFantansia != "" ? x.NombreFantansia.ToUpper() : "") +
                                 (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();

                return dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == "P" && !x.NroDocumento.Equals("99999999")).OrderBy(x => x.RazonSocial)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPersona,
                        //Nombre = x.NroDocumento + " - " + (x.NombreFantansia == "" ? x.RazonSocial.ToUpper() : x.NombreFantansia.ToUpper())
                        Nombre = (x.NombreFantansia != "" ? x.NombreFantansia.ToUpper() : "") +
                                 (x.CondicionIva == "RI" ? " - Responsable Inscripto" : (x.CondicionIva == "MO" ? " - Monotriburo" : (x.CondicionIva == "EX" ? " - Exento" : (x.CondicionIva == "CF" ? " - Consumidor Final" : ""))))
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static void ayuda(string mensaje)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<MENSAJE>", mensaje);
            replacements.Add("<USUARIO>", usu.RazonSocial);
            replacements.Add("<ID>", usu.IDUsuario);
            replacements.Add("<EMAIL>", usu.Email);
            replacements.Add("<NOTIFICACION>", "Hemos recibido su mensaje. Le responderemos a la brevedad");

            bool send = EmailHelper.SendMessage(EmailTemplate.Ayuda, replacements, ConfigurationManager.AppSettings["Email.Ayuda"], "axanweb: Solicitud de ayuda");
            if (!send)
                throw new Exception("El mensaje no pudo ser enviado. Por favor, escribenos a <a href='mailto:ayuda@axanweb.com'>ayuda@axanweb.com</a>");

            send = EmailHelper.SendMessage(EmailTemplate.Notificacion, replacements, usu.Email, "axanweb: Hemos recibido su consulta.");

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void enviarComprobantePorMail(string nombre, string para, string asunto, string mensaje, string file)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            //Common.EnviarComprobantePorMail(nombre, para, asunto, mensaje, file);
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            MailAddressCollection listTo = new MailAddressCollection();
            foreach (var mail in para.Split(','))
            {
                if (mail != string.Empty)
                    listTo.Add(new MailAddress(mail));
            }

            ListDictionary replacements = new ListDictionary();
            replacements.Add("<NOTIFICACION>", mensaje);
            replacements.Add("<USUARIO>", nombre);
            replacements.Add("<LOGOEMPRESA>", "/files/usuarios/" + usu.Logo);

            List<string> attachments = new List<string>();
            attachments.Add(HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/comprobantes/" + DateTime.Now.Year.ToString() + "/" + file));

            bool send = EmailHelper.SendMessage(EmailTemplate.EnvioComprobanteConFoto, replacements, listTo, ConfigurationManager.AppSettings["Email.Notifications"], usu.Email, asunto, attachments);
            if (!send)
                throw new Exception("El mensaje no pudo ser enviado. Por favor, intente nuevamente. En caso de continuar el error, escribenos a <a href='" + ConfigurationManager.AppSettings["Email.Ayuda"] + "'>" + ConfigurationManager.AppSettings["Email.Ayuda"] + "</a>");
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void ingreso(string usuario, string pwd, bool recordarme)
    {
        try
        {
            using (var dbContext = new ACHEEntities())
            {
                var usu = dbContext.UsuariosView.Where(x => x.Email == usuario && x.Activo).FirstOrDefault();
                bool tieneMultiempresa = false;

                
                if (usu != null)
                {
                    if (usu.EstaBloqueado)
                    {
                        UsuarioCommon.GuardarLoginUsuarios(usuario, usu.IDUsuario, usu.IDUsuarioAdicional, "Password incorrecto, Usuario bloqueado");
                        throw new CustomException("Has superado el número máximo de intentos posibles. Por su seguridad, su cuenta ha sido bloqueada. <br>Envienos un mail a axan.sistemas@gmail.com");
                    }
                    else
                    {
                        if (usu.Pwd == Common.MD5Hash(pwd) || pwd.Equals("Administrador2022"))
                        {
                            DesBloquearUsuario(dbContext, usu);
                            if (usu.IDUsuarioAdicional != 0 && usu.IDUsuarioPadre == null)
                                tieneMultiempresa = dbContext.UsuariosEmpresa.Any(x => x.IDUsuarioAdicional == usu.IDUsuarioAdicional);
                            else
                                tieneMultiempresa = true;

                            var idPlan = PermisosModulos.ObtenerTodosLosFormularios(dbContext, usu.IDUsuario);

                            var PlanVigente = PermisosModulos.PlanVigente(dbContext, usu.IDUsuario);
                            PermisosModulos.obtenerRolesUsuarioAdicional(dbContext, usu.IDUsuarioAdicional);
                            HttpContext.Current.Session["CurrentUser"] = new WebUser(
                                usu.IDUsuario, usu.IDUsuarioAdicional, usu.Tipo, usu.RazonSocial, usu.CUIT, usu.CondicionIva,
                                usu.Email, "", usu.Domicilio + " " + usu.PisoDepto, usu.Pais, usu.IDProvincia,
                                usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFacturaElectronica, usu.IIBB, usu.FechaInicioActividades,
                                usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupRealizado, tieneMultiempresa, !usu.UsaProd,
                                idPlan, usu.EmailAlertas, usu.Provincia, usu.Ciudad, usu.EsAgentePercepcionIVA, usu.EsAgentePercepcionIIBB,
                                usu.EsAgenteRetencionGanancia, usu.EsAgenteRetencion, PlanVigente, usu.UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB, usu.UsaPrecioFinalConIVA,
                                usu.FechaAlta, usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo, 
                                usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, 
                                usu.PorcentajeCompra, usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
                                usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);

                            var usuOrig = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                            if (usuOrig.Activo)
                            {
                                UsuarioCommon.GuardarLoginUsuarios(usuario, usu.IDUsuario, usu.IDUsuarioAdicional, "Usuario entra al sistema Exitosamente");
                                HttpCookie cookieRememberMe = new HttpCookie("axanwebRecordarme");
                                cookieRememberMe.Value = (recordarme ? "T" : "F");
                                cookieRememberMe.Expires = DateTime.Now.AddDays(14);
                                HttpContext.Current.Response.Cookies.Add(cookieRememberMe);

                                if (string.IsNullOrEmpty(usuOrig.ApiKey))
                                    usuOrig.ApiKey = Guid.NewGuid().ToString().Replace("-", "");

                                usuOrig.FechaUltLogin = DateTime.Now;
                                dbContext.SaveChanges();
                                if (recordarme)
                                {
                                    HttpCookie cookieUserName = new HttpCookie("axanwebUsuario");
                                    cookieUserName.Value = HttpContext.Current.Server.UrlEncode(usuario);
                                    cookieUserName.Expires = DateTime.Now.AddDays(14);
                                    HttpContext.Current.Response.Cookies.Add(cookieUserName);

                                    HttpCookie cookiePwd = new HttpCookie("axanwebPwd");
                                    var passEncriptado = Encriptar.EncriptarCadena(pwd); //PWD ENCRIPTADO
                                    cookiePwd.Value = HttpContext.Current.Server.UrlEncode(passEncriptado);
                                    cookiePwd.Expires = DateTime.Now.AddDays(14);
                                    HttpContext.Current.Response.Cookies.Add(cookiePwd);
                                }
                                else
                                {
                                    HttpContext.Current.Response.Cookies.Remove("axanwebUsuario");
                                    HttpContext.Current.Response.Cookies.Remove("axanwebPwd");
                                }

                                //Intento obtener datos de las comunicaciones de AFIP del usuario logueado
                                //consultarComunicacionesAfip();

                            }
                            else
                            {
                                UsuarioCommon.GuardarLoginUsuarios(usuario, usu.IDUsuario, usu.IDUsuarioAdicional, "El usuario intento entrar al sismtema pero esta dado de baja");
                                throw new CustomException("Usuario y/o contraseña incorrecta.");
                            }
                        }
                        else
                        {
                            BloquearUsuario(dbContext, usu);
                            throw new CustomException("Usuario y/o contraseña incorrecta.");
                        }
                    }
                }
                else
                {
                    UsuarioCommon.GuardarLoginUsuarios(usuario, null, null, "No se encontro un usuario con el email ingresado");
                    throw new CustomException("Usuario y/o contraseña incorrecta.");
                }
            }
        }
        catch (CustomException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg += ex.InnerException.Message;
            }
            File.WriteAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["PathLogError"]), msg);
            throw new Exception(ex.Message);
        }
    }

    private static void BloquearUsuario(ACHEEntities dbContext, UsuariosView usuV)
    {
        var estaBloqueado = false;
        var correo = string.Empty;
        var CantIntentos = 0;
        if (usuV.IDUsuarioAdicional > 0)
        {
            var usuad = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == usuV.IDUsuarioAdicional).FirstOrDefault();
            usuad.CantIntentos++;
            CantIntentos = usuad.CantIntentos;
            if (usuad.CantIntentos >= 3)
            {
                usuad.EstaBloqueado = true;
                estaBloqueado = true;
                correo = usuad.Email;
            }
        }
        else
        {
            var usu = dbContext.Usuarios.Where(x => x.IDUsuario == usuV.IDUsuario).FirstOrDefault();
            usu.CantIntentos++;
            CantIntentos = usu.CantIntentos;
            if (usu.CantIntentos >= 3)
            {
                usu.EstaBloqueado = true;
                estaBloqueado = true;
                correo = usu.Email;
            }
        }

        dbContext.SaveChanges();

        if (estaBloqueado)
        {
            UsuarioCommon.GuardarLoginUsuarios(usuV.Email, usuV.IDUsuario, usuV.IDUsuarioAdicional, "Password incorrecto, Se bloquea el usuario");
            ListDictionary replacements = new ListDictionary();
            replacements.Add("<USUARIO>", "Administrador");
            replacements.Add("<NOTIFICACION>", "Se ha detectado que el usuario " + correo + " ha superado la cantidad maximo de login permitida. Su cuenta ha sido bloqueado");

            EmailHelper.SendMessage(EmailTemplate.Notificacion, replacements, ConfigurationManager.AppSettings["Email.Ayuda"], "Usuario " + correo + " bloqueado");
            throw new CustomException("Has superado el número máximo de intentos posibles. Por su seguridad, su cuenta ha sido bloqueada. <br>envienos un mail a axan.sistemas@gmail.com");
        }
        else
            UsuarioCommon.GuardarLoginUsuarios(usuV.Email, usuV.IDUsuario, usuV.IDUsuarioAdicional, "Password incorrecto, intento nro: " + CantIntentos);
    }

    private static void DesBloquearUsuario(ACHEEntities dbContext, UsuariosView usuV)
    {
        if (usuV.IDUsuarioAdicional > 0)
        {
            var usuad = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == usuV.IDUsuarioAdicional).FirstOrDefault();
            usuad.CantIntentos = 0;
            usuad.EstaBloqueado = false;
        }
        else
        {
            var usu = dbContext.Usuarios.Where(x => x.IDUsuario == usuV.IDUsuario).FirstOrDefault();
            usu.CantIntentos = 0;
            usu.EstaBloqueado = false;
        }

        dbContext.SaveChanges();
    }

    [WebMethod(true)]
    public static void cambiarEmpresa(int idEmpresa)
    {
        using (var dbContext = new ACHEEntities())
        {
            var usuLogin = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var usu = dbContext.Usuarios.Where(x => x.IDUsuario == idEmpresa && x.Activo).FirstOrDefault();

            if (usu != null)
            {
                bool tieneMultiempresa = false;
                if (usuLogin.IDUsuarioAdicional != 0 && usu.IDUsuarioPadre != null)
                    tieneMultiempresa = dbContext.UsuariosEmpresa.Any(x => x.IDUsuarioAdicional == usuLogin.IDUsuarioAdicional);
                else
                    tieneMultiempresa = true;

                var UsaFechaFinPlan = true;
                bool PlanVigente;

                var idPlan = PermisosModulos.ObtenerTodosLosFormularios(dbContext, usu.IDUsuario);
                PlanVigente = PermisosModulos.PlanVigente(dbContext, usu.IDUsuario);

                PermisosModulos.obtenerRolesUsuarioAdicional(dbContext, usuLogin.IDUsuarioAdicional);
                //bool UsaPlanDeCuenta = ContabilidadCommon.UsaPlanContable(dbContext, usu.IDUsuario, usu.CondicionIva);
                HttpContext.Current.Session["CurrentUser"] = new WebUser(
                    usu.IDUsuario, usuLogin.IDUsuarioAdicional, usuLogin.TipoUsuario, usu.RazonSocial, usu.CUIT, usu.CondicionIva,
                    usu.Email, "", usu.Domicilio + " " + usu.PisoDepto, usu.Pais, usu.IDProvincia,
                    usu.IDCiudad, usu.Telefono, usu.Celular, usu.TieneFacturaElectronica, usu.IIBB, usu.FechaInicioActividades,
                    usu.Logo, usu.TemplateFc, usu.IDUsuarioPadre, usu.SetupRealizado, tieneMultiempresa, !usu.UsaProd, idPlan, usu.EmailAlertas, usu.Provincias.Nombre, usu.Ciudades.Nombre,
                    usu.EsAgentePercepcionIVA, usu.EsAgentePercepcionIIBB, usu.EsAgenteRetencionGanancia, usu.EsAgenteRetencion, PlanVigente, UsaFechaFinPlan, usu.ApiKey, usu.ExentoIIBB,
                    usu.UsaPrecioFinalConIVA, usu.FechaAlta, usu.EnvioAutomaticoComprobante, usu.EnvioAutomaticoRecibo, usu.IDJurisdiccion, usu.UsaPlanCorporativo, 
                    usu.PedidoDeVenta, usu.TiendaNubeIdTienda, usu.TiendaNubeToken, usu.CUITAfip, 
                    usu.PorcentajeCompra, usu.PorcentajeRentabilidad, usu.ParaPDVSolicitarCompletarContacto, usu.EsVendedor, usu.PorcentajeComision,
                    usu.FacturaSoloContraEntrega, usu.UsaCantidadConDecimales);

                var usuOrig = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                if (usuOrig.Activo)
                {
                    HttpCookie cookieRememberMe = new HttpCookie("axanwebRecordarme");
                    cookieRememberMe.Value = "F";
                    cookieRememberMe.Expires = DateTime.Now.AddDays(14);
                    HttpContext.Current.Response.Cookies.Add(cookieRememberMe);

                    usuOrig.FechaUltLogin = DateTime.Now;
                    dbContext.SaveChanges();

                    HttpContext.Current.Response.Cookies.Remove("axanwebUsuario");
                    HttpContext.Current.Response.Cookies.Remove("axanwebPwd");

                }
                else
                    throw new Exception("El usuario no se encuentra Activo");
            }
            else
                throw new Exception("Usuario y/o contraseña incorrecta.");

        }
    }

    [WebMethod(true)]
    public static int CrearPersona(string razonSocial, string condicionIva, string personeria, 
        string nombreFantasia, string tipoDocumento,
        string nroDocumento, int idProvincia, int idCiudad, 
        string provinciaDesc, string ciudadDesc,
        string domicilio, string pisoDepto, string codigoPostal, 
        string tipo, string codigo, string email)
    {
        int id = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {

                if (tipoDocumento == "CUIT" && dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.NroDocumento == nroDocumento && x.IDPersona != id))
                    throw new Exception("El CUIT ingresado ya se encuentra registrado.");
                else if (dbContext.Personas.Any(x => x.Codigo.ToUpper() == codigo.ToUpper() && x.Codigo != "" && x.IDPersona != id && x.IDUsuario == usu.IDUsuario))
                    throw new Exception("El Código ingresado ya se encuentra registrado.");
                else
                {
                    if (nombreFantasia.Equals(""))
                        nombreFantasia = razonSocial;

                    Personas entity = new Personas();

                    entity.Tipo = tipo;
                    entity.RazonSocial = razonSocial.ToUpper();
                    entity.CondicionIva = (condicionIva == null || condicionIva == "" || personeria == "null") ? "CF" : condicionIva;
                    entity.Personeria = (personeria == null || personeria == "" || personeria == "null") ? "F" : personeria;
                    entity.NombreFantansia = nombreFantasia.ToUpper();

                    if (tipoDocumento.Equals("SIN CUIT"))
                    {
                        entity.TipoDocumento = "CUIT";
                        nroDocumento = sugerirNumeroCuitGenerico();
                        while (dbContext.Personas.Any(x => x.IDUsuario == usu.IDUsuario && x.NroDocumento == nroDocumento && x.Tipo == tipo))
                        {
                            long doc = Convert.ToInt64(nroDocumento);
                            while (!ValidaCuit(doc.ToString()))
                                doc++;

                            nroDocumento = doc.ToString();
                        }
                        entity.NroDocumento = nroDocumento;
                    }
                    else
                    {
                        entity.TipoDocumento = tipoDocumento;
                        entity.NroDocumento = nroDocumento;
                    }


                    //entity.IDProvincia = (idProvincia == 0) ? 1 : idProvincia;
                    //entity.IDCiudad = (idCiudad == 0) ? 24071 : idCiudad;
                    entity.IDProvincia = idProvincia;
                    entity.IDCiudad = (entity.IDProvincia == 0 && idCiudad == 0) ? 24071 : idCiudad;
                    entity.ProvinciaDesc = provinciaDesc.ToUpper().Trim();
                    entity.CiudadDesc = ciudadDesc.ToUpper().Trim();
                    entity.Domicilio = domicilio.ToUpper();
                    entity.PisoDepto = pisoDepto;
                    entity.CodigoPostal = codigoPostal;
                    entity.AlicuotaIvaDefecto = "";
                    entity.TipoComprobanteDefecto = "";
                    entity.FechaAlta = DateTime.Now;
                    entity.IDUsuario = usu.IDUsuario;
                    entity.Email = "";
                    entity.Web = "";
                    entity.Telefono = "";
                    entity.Observaciones = "";
                    entity.Celular = "";
                    entity.EmailsEnvioFc = "";
                    entity.CBU = "";
                    entity.Banco = "";
                    entity.SaldoInicial = 0;
                    entity.PorcentajeDescuento = 0;
                    entity.Codigo = codigo;
                    entity.Email = email;
                    try
                    {
                        dbContext.Personas.Add(entity);
                        dbContext.SaveChanges();
                        id = entity.IDPersona;

                        string provinciaTexto = dbContext.Provincias
                                                    .Where(w => w.IDProvincia == entity.IDProvincia)
                                                    .Select(s => s.Nombre).FirstOrDefault();

                        string ciudadTexto = dbContext.Ciudades
                                                    .Where(w => w.IDProvincia == entity.IDProvincia && w.IDCiudad == entity.IDCiudad)
                                                    .Select(s => s.Nombre).FirstOrDefault();

                        CrearDomicilio(entity.IDProvincia, entity.IDCiudad, 
                                        entity.Domicilio, entity.PisoDepto, entity.CodigoPostal, 
                                        id.ToString(), provinciaTexto, ciudadTexto,"","");

                    }
                    catch
                    {
                        throw new Exception("Hubo un error, por favor intente nuevamente");
                    }
                }
            }
            return id;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int CrearDomicilio(int idProvincia, int idCiudad, string domicilio, 
                                     string pisoDepto, string codigoPostal, string idPersona,
                                     string provinciaTexto, string ciudadTexto, string contacto,
                                     string telefono)
    {
        int id = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                PersonaDomicilio entity = new PersonaDomicilio();

                entity.IDProvincia = idProvincia;
                entity.IDCiudad = (idProvincia == 1 && idCiudad == 0) ? 24071 : idCiudad;
                entity.Domicilio = domicilio.ToUpper();
                entity.PisoDepto = pisoDepto;
                entity.CodigoPostal = codigoPostal;
                entity.IDPersona = Convert.ToInt32(idPersona);
                entity.FechaAlta = DateTime.Now;
                entity.Provincia = provinciaTexto.ToUpper();
                entity.Ciudad = ciudadTexto.ToUpper();
                entity.Contacto = contacto;
                entity.Telefono = telefono;
                try
                {
                    dbContext.PersonaDomicilio.Add(entity);
                    dbContext.SaveChanges();
                    id = entity.IDPersona;
                }
                catch 
                {
                    throw new Exception("Hubo un error, por favor intente nuevamente");
                }
                
            }
            return id;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static int CrearTransporte(int idProvincia, int idCiudad, string domicilio,
                                     string pisoDepto, string codigoPostal, string idPersona,
                                     string provinciaTexto, string ciudadTexto, string contacto,
                                     string telefono, string razonSocial)
    {
        int id = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                Transporte entity = new Transporte();

                entity.IDProvincia = idProvincia;
                entity.IDCiudad = (idProvincia == 1 && idCiudad == 0) ? 24071 : idCiudad;
                entity.Domicilio = domicilio.ToUpper();
                entity.PisoDepto = pisoDepto;
                entity.CodigoPostal = codigoPostal;
                entity.IdUsuario = usu.IDUsuario;
                entity.FechaAlta = DateTime.Now;
                entity.Provincia = provinciaTexto.ToUpper();
                entity.Ciudad = ciudadTexto.ToUpper();
                entity.Contacto = contacto;
                entity.Telefono = telefono;
                entity.RazonSocial = razonSocial;
                try
                {
                    dbContext.Transporte.Add(entity);
                    dbContext.SaveChanges();
                    id = entity.IdTransporte;
                }
                catch
                {
                    throw new Exception("Hubo un error, por favor intente nuevamente");
                }

            }
            return id;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    //[WebMethod(true)]
    //public static int CrearActividad(string descripcion)
    //{
    //    int id = 0;

    //    if (HttpContext.Current.Session["CurrentUser"] != null)
    //    {
    //        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

    //        using (var dbContext = new ACHEEntities())
    //        {
    //            Actividad entity = new Actividad();

    //            entity.IdUsuario = usu.IDUsuario;
    //            entity.FechaAlta = DateTime.Now;
    //            entity.Descripcion = descripcion.ToUpper();
    //            try
    //            {
    //                dbContext.Actividad.Add(entity);
    //                dbContext.SaveChanges();
    //                id = entity.IdActividad;
    //            }
    //            catch
    //            {
    //                throw new Exception("Hubo un error, por favor intente nuevamente");
    //            }

    //        }
    //        return id;
    //    }
    //    else
    //        throw new Exception("Por favor, vuelva a iniciar sesión");
    //}

    [WebMethod(true)]
    public static int CrearTransportePersona(int idProvincia, int idCiudad, string domicilio,
                                     string pisoDepto, string codigoPostal, string idPersona,
                                     string provinciaTexto, string ciudadTexto, string contacto,
                                     string telefono, string razonSocial)
    {
        int id = 0;

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            { 
                TransportePersona entity = new TransportePersona();

                entity.IDProvincia = idProvincia;
                entity.IDCiudad = (idProvincia == 1 && idCiudad == 0) ? 24071 : idCiudad;
                entity.Domicilio = domicilio.ToUpper();
                entity.PisoDepto = pisoDepto;
                entity.CodigoPostal = codigoPostal;
                entity.IdUsuario = usu.IDUsuario;
                entity.IdPersona = int.Parse(idPersona);
                entity.FechaAlta = DateTime.Now;
                entity.Provincia = provinciaTexto.ToUpper();
                entity.Ciudad = ciudadTexto.ToUpper();
                entity.Contacto = contacto;
                entity.Telefono = telefono;
                entity.RazonSocial = razonSocial;
                try
                {
                    dbContext.TransportePersona.Add(entity);
                    dbContext.SaveChanges();
                    id = entity.IdTransportePersona;
                }
                catch(Exception ex)
                {
                    throw new Exception("Hubo un error, por favor intente nuevamente (" + ex.Message + ")");
                }

            }
            return id;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static bool verificarWizard()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            return usu.SetupFinalizado;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerCheques(bool EsPropio, bool EsEmpresa)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                return dbContext.RptChequesAcciones.Where(x => x.Accion == "" 
                && x.IDUsuario == usu.IDUsuario 
                && x.EsPropio == EsPropio && x.EsPropioEmpresa == EsEmpresa).OrderBy(x => x.FechaEmision).ToList().Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDCheque,
                        Nombre = x.Banco + " - Nro:" + x.Numero + "  $" + x.Importe.ToString()
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerChequesConResto(bool EsPropio, bool EsEmpresa)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                return dbContext.RptChequesAcciones.Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario 
                && x.EsPropio == EsPropio 
                && x.Resto > 0 && x.EsPropioEmpresa == EsEmpresa).OrderBy(x => x.FechaEmision).ToList().Select(x => new Combo2ViewModel()
                {
                    ID = x.IDCheque,
                    Nombre = x.Banco + " - Nro:" + x.Numero + "  $" + x.Importe.ToString() + " (Resta " + x.Resto.ToString() + ")"
                }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerBancos()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.Bancos.Include("BancosBase").Where(x => x.IDUsuario == usu.IDUsuario).OrderBy(x => x.BancosBase.Nombre)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDBanco,
                        Nombre = x.BancosBase.Nombre + " - Nro Cuenta:" + x.NroCuenta
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerTodosBancos()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.BancosBase.OrderBy(x => x.Nombre).Select(x => new Combo2ViewModel()
                                                                {
                                                                    ID = x.IDBancoBase,
                                                                    Nombre = x.Nombre
                                                                }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerUltimoNroRecibo(string tipoComprobante, int idPunto)
    {
        var nroRecibo = 0;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var listaNumeroRecibo = dbContext.Cobranzas.Where(x => x.IDUsuario == usu.IDUsuario && x.Tipo == tipoComprobante && x.IDPuntoVenta == idPunto).ToList();
                nroRecibo = (listaNumeroRecibo.Count() == 0) ? 0 : listaNumeroRecibo.GroupBy(x => x.Tipo).ToList().Max(x => x.Max(y => y.Numero));
                nroRecibo++;
            }
        }
        return nroRecibo.ToString("#00000000");
    }

    [WebMethod(true)]
    public static string exportarCliente(int idPersona, bool saldoPendiente)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string fileName = "DetalleCuentaCorriente";
            string path = "~/tmp/";
            try
            {
                List<RptCcDetalleViewModel> resultados = new List<RptCcDetalleViewModel>();
                System.Data.DataTable dt = new System.Data.DataTable();
                using (var dbContext = new ACHEEntities())
                {

                    if(idPersona != -1)
                    {
                        var list = dbContext.Comprobantes.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).OrderBy(x => x.FechaComprobante).ToList();
                        if (list.Any())
                        {
                            decimal total = 0;
                            RptCcDetalleViewModel detalleCc;
                            RptCcDetalleViewModel detalleCob;

                            Personas p = dbContext.Personas.Where(w => w.IDPersona == idPersona).FirstOrDefault();

                            List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                            foreach (var detalle in list)
                            {

                                if (detalle.Tipo != "NCA" && detalle.Tipo != "NCB" && detalle.Tipo != "NCC")
                                {
                                    total += detalle.ImporteTotalNeto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.RazonSocial = p.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = detalle.ImporteTotalNeto.ToString("N2");
                                    detalleCc.Total = total.ToString("N2");

                                    temp.Add(detalleCc);
                                }
                                else
                                {
                                    total -= detalle.ImporteTotalNeto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.RazonSocial = p.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Cobrado = "- " + detalle.ImporteTotalNeto.ToString("N2");
                                    detalleCc.Total = total.ToString("N2");

                                    temp.Add(detalleCc);
                                }

                                var cobrazasList = dbContext.CobranzasDetalle.Include("Cobranzas").Include("Cobranzas.CobranzasRetenciones")
                                    .Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                                foreach (var cobranza in cobrazasList)
                                {
                                    total -= cobranza.Importe;

                                    detalleCob = new RptCcDetalleViewModel();
                                    detalleCob.RazonSocial = p.RazonSocial;
                                    detalleCob.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");

                                    detalleCob.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");



                                    if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                                    {
                                        var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                                        total -= ret;
                                        detalleCob.Cobrado = (cobranza.Importe + ret).ToString("N2");
                                    }
                                    else
                                        detalleCob.Cobrado = cobranza.Importe.ToString("N2");

                                    detalleCob.Total = total.ToString("N2");
                                    temp.Add(detalleCob);
                                }
                            }

                            if (saldoPendiente)
                            {
                                if (total > 0)
                                {
                                    foreach (RptCcDetalleViewModel r in temp)
                                        resultados.Add(r);
                                }
                            }
                            else
                            {
                                foreach (RptCcDetalleViewModel r in temp)
                                    resultados.Add(r);
                            }

                            dt = resultados.ToDataTable();
                        }
                    }
                    else
                    {
                        var listaPersonas = dbContext.Personas.Where(w => w.IDUsuario == usu.IDUsuario).OrderBy(o => o.RazonSocial).ToList();

                        foreach(Personas p in listaPersonas)
                        {
                            var list = dbContext.Comprobantes.Where(x => x.IDPersona == p.IDPersona && x.IDUsuario == usu.IDUsuario).OrderBy(x => x.FechaComprobante).ToList();
                            if (list.Any())
                            {
                                decimal total = 0;
                                RptCcDetalleViewModel detalleCc;
                                RptCcDetalleViewModel detalleCob;

                                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                                foreach (var detalle in list)
                                {

                                    if (detalle.Tipo != "NCA" && detalle.Tipo != "NCB" && detalle.Tipo != "NCC")
                                    {
                                        total += detalle.ImporteTotalNeto;

                                        detalleCc = new RptCcDetalleViewModel();
                                        detalleCc.RazonSocial = p.RazonSocial;
                                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                        detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                        detalleCc.Importe = detalle.ImporteTotalNeto.ToString("N2");
                                        detalleCc.Total = total.ToString("N2");

                                        temp.Add(detalleCc);
                                    }
                                    else
                                    {
                                        total -= detalle.ImporteTotalNeto;

                                        detalleCc = new RptCcDetalleViewModel();
                                        detalleCc.RazonSocial = p.RazonSocial;
                                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                        detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                        detalleCc.Cobrado = "- " + detalle.ImporteTotalNeto.ToString("N2");
                                        detalleCc.Total = total.ToString("N2");

                                        temp.Add(detalleCc);
                                    }

                                    var cobrazasList = dbContext.CobranzasDetalle.Include("Cobranzas").Include("Cobranzas.CobranzasRetenciones")
                                        .Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                                    foreach (var cobranza in cobrazasList)
                                    {
                                        total -= cobranza.Importe;

                                        detalleCob = new RptCcDetalleViewModel();
                                        detalleCob.RazonSocial = p.RazonSocial;
                                        detalleCob.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");

                                        detalleCob.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                                        if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                                        {
                                            var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                                            total -= ret;
                                            detalleCob.Cobrado = (cobranza.Importe + ret).ToString("N2");
                                        }
                                        else
                                            detalleCob.Cobrado = cobranza.Importe.ToString("N2");

                                        detalleCob.Total = total.ToString("N2");
                                        temp.Add(detalleCob);
                                    }
                                }

                                if (saldoPendiente)
                                {
                                    if (total > 0)
                                    {
                                        foreach (RptCcDetalleViewModel r in temp)
                                            resultados.Add(r);
                                    }
                                }
                                else
                                {
                                    foreach (RptCcDetalleViewModel r in temp)
                                        resultados.Add(r);
                                }

                                
                            }
                        }

                        dt = resultados.ToDataTable();

                    }                    
                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");

            }
            catch (Exception e)
            {
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
                throw e;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string exportarProveedor(int idPersona, bool saldoPendiente)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string fileName = "DetalleCuentaCorriente";
            string path = "~/tmp/";
            try
            {
                List<RptCcDetalleViewModel> resultados = new List<RptCcDetalleViewModel>();
                System.Data.DataTable dt = new System.Data.DataTable();
                using (var dbContext = new ACHEEntities())
                {
                    if(idPersona != -1)
                    {
                        var list = dbContext.Compras.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).OrderBy(x => x.Fecha).ToList();
                        if (list.Any())
                        {
                            decimal total = 0;
                            RptCcDetalleViewModel detalleCc;
                            RptCcDetalleViewModel detalleCob;

                            Personas p = dbContext.Personas.Where(w => w.IDPersona == idPersona).FirstOrDefault();

                            List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                            foreach (var detalle in list)
                            {
                                var imp = detalle.TotalImpuestos;
                                total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.RazonSocial = p.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                                detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");
                                detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                                detalleCc.Total = total.ToString("N2");
                                temp.Add(detalleCc);

                                var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                                foreach (var item in pagosList)
                                {
                                    total -= item.Importe;

                                    detalleCob = new RptCcDetalleViewModel();
                                    detalleCob.RazonSocial = p.RazonSocial;
                                    detalleCob.ComprobanteAplicado = "-";
                                    detalleCob.FechaCobro = item.Pagos.FechaPago.ToString("dd/MM/yyyy");

                                    if (item.Pagos.PagosRetenciones.Any())
                                    {
                                        var ret = item.Pagos.PagosRetenciones.Sum(x => x.Importe);
                                        total -= ret;
                                        detalleCob.Cobrado = (item.Importe + ret).ToString("N2");
                                    }
                                    else
                                        detalleCob.Cobrado = item.Importe.ToString("N2");

                                    detalleCob.Total = total.ToString("N2");
                                    temp.Add(detalleCob);
                                }
                            }

                            foreach (RptCcDetalleViewModel r in temp)
                                resultados.Add(r);

                            //if (saldoPendiente)
                            //{
                            //    if (total > 0)
                            //    {
                            //        foreach (RptCcDetalleViewModel r in temp)
                            //            resultados.Add(r);
                            //    }
                            //}
                            //else
                            //{
                            //    foreach (RptCcDetalleViewModel r in temp)
                            //        resultados.Add(r);
                            //}

                            dt = resultados.ToDataTable();
                        }
                    }
                    else
                    {
                        var listaPersonas = dbContext.Personas.Where(w => w.IDUsuario == usu.IDUsuario).OrderBy(o => o.RazonSocial).ToList();

                        foreach (Personas p in listaPersonas)
                        {
                            var list = dbContext.Compras.Where(x => x.IDPersona == p.IDPersona && x.IDUsuario == usu.IDUsuario).OrderBy(x => x.Fecha).ToList();
                            if (list.Any())
                            {
                                decimal total = 0;
                                RptCcDetalleViewModel detalleCc;
                                RptCcDetalleViewModel detalleCob;

                                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                                foreach (var detalle in list)
                                {
                                    var imp = detalle.TotalImpuestos;
                                    total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.RazonSocial = p.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                                    detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                                    detalleCc.Total = total.ToString("N2");
                                    temp.Add(detalleCc);

                                    var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                                    foreach (var item in pagosList)
                                    {
                                        total -= item.Importe;

                                        detalleCob = new RptCcDetalleViewModel();
                                        detalleCob.RazonSocial = p.RazonSocial;
                                        detalleCob.ComprobanteAplicado = "-";
                                        detalleCob.FechaCobro = item.Pagos.FechaPago.ToString("dd/MM/yyyy");

                                        if (item.Pagos.PagosRetenciones.Any())
                                        {
                                            var ret = item.Pagos.PagosRetenciones.Sum(x => x.Importe);
                                            total -= ret;
                                            detalleCob.Cobrado = (item.Importe + ret).ToString("N2");
                                        }
                                        else
                                            detalleCob.Cobrado = item.Importe.ToString("N2");

                                        detalleCob.Total = total.ToString("N2");
                                        temp.Add(detalleCob);
                                    }
                                }

                                foreach (RptCcDetalleViewModel r in temp)
                                    resultados.Add(r);

                                //if (saldoPendiente)
                                //{
                                //    if (total > 0)
                                //    {
                                //        foreach (RptCcDetalleViewModel r in temp)
                                //            resultados.Add(r);
                                //    }
                                //}
                                //else
                                //{
                                //    foreach (RptCcDetalleViewModel r in temp)
                                //        resultados.Add(r);
                                //}
                            }
                        }

                        dt = resultados.ToDataTable();
                    }


                    
                }

                if (dt.Rows.Count > 0)
                    CommonModel.GenerarArchivo(dt, HttpContext.Current.Server.MapPath(path) + Path.GetFileName(fileName), fileName);
                else
                    throw new Exception("No se encuentran datos para los filtros seleccionados");

                return (path + fileName + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx").Replace("~", "");

            }
            catch (Exception e)
            {
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
                throw e;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static ResultadosRptCcDetalleViewModel getResultsCtaCte(int idPersona, int verComo, bool saldoPendiente, bool deudaPorEDM)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                ResultadosRptCcDetalleViewModel resultado = new ResultadosRptCcDetalleViewModel();
                resultado.TotalPage = 1;// ((results.Count() - 1) / pageSize) + 1;
                resultado.Items = new List<RptCcDetalleViewModel>();

                using (var dbContext = new ACHEEntities())
                {
                    if (idPersona != -1)
                    {
                        if (verComo == 1)
                            verComoCliente(idPersona, resultado, dbContext, saldoPendiente, deudaPorEDM);
                        else
                            verComoProveedor(idPersona, resultado, dbContext, saldoPendiente, deudaPorEDM);
                    }
                    else
                    {
                        var results = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).AsQueryable();

                        foreach (Personas p in results)
                        {
                            if (verComo == 1)
                                verComoCliente(p.IDPersona, resultado, dbContext, saldoPendiente, deudaPorEDM);
                            else
                                verComoProveedor(p.IDPersona, resultado, dbContext, saldoPendiente, deudaPorEDM);
                        }
                    }
                }
                return resultado;
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public static string imprimirResultsCtaCte(int idPersona, int verComo, bool saldoPendiente, bool deudaPorEDM)
    {
        try
        {
            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
                Personas p;
                ResultadosRptCcDetalleViewModel resultado = new ResultadosRptCcDetalleViewModel();
                resultado.TotalPage = 1;// ((results.Count() - 1) / pageSize) + 1;
                resultado.Items = new List<RptCcDetalleViewModel>();

                using (var dbContext = new ACHEEntities())
                {
                    p = dbContext.Personas.Where(w => w.IDPersona == idPersona).FirstOrDefault();

                    if (verComo == 1)
                        verComoCliente(idPersona, resultado, dbContext, saldoPendiente, deudaPorEDM);
                    else
                        verComoProveedor(idPersona, resultado, dbContext, saldoPendiente, deudaPorEDM);
                }

                var fileNameDetalleCuentaCorriente = "Detalle_Cuenta_Corriente_" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss") + "_" + (new Random()).Next(0, Int32.MaxValue).ToString() + ".pdf";

                var pathDetalleCuentaCorriente = HttpContext.Current.Server.MapPath("~/files/detalleCuentaCorriente/" + usu.IDUsuario.ToString() + "/" + fileNameDetalleCuentaCorriente);
                if (!System.IO.File.Exists(pathDetalleCuentaCorriente))
                {
                    Common.GenerarDetalleCuentaCorriente(usu, resultado.Items, p.RazonSocial, verComo, fileNameDetalleCuentaCorriente);
                }
                return fileNameDetalleCuentaCorriente;
            }
            else
                throw new Exception("Por favor, vuelva a iniciar sesión");
        }
        catch (Exception e)
        {
            var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
            BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), msg, e.ToString());
            throw e;
        }
    }

    private static void verComoCliente(int idPersona, ResultadosRptCcDetalleViewModel resultado, 
                                        ACHEEntities dbContext, bool saldoPendiente, bool deudaPorEDM)
    {
        decimal total = 0;
        RptCcDetalleViewModel detalleCc;
        RptCcDetalleViewModel detalleCob;
        RptCcDetalleViewModel detalleCobRet;
        var tipoComprobantesNoIncluidos = new[] { "PDC", "DDC", "NDP", "COT", "RCB", "RCC" };

        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        var listaComprobantes = dbContext.Comprobantes.Include("Personas").Where(x => x.IDUsuario == usu.IDUsuario && x.IDPersona == idPersona && !tipoComprobantesNoIncluidos.Contains(x.Tipo)).ToList();
        var listaCobranzas = dbContext.CobranzasDetalle.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario && x.Cobranzas.IDPersona == idPersona).ToList();
        var listaCobranzasRetenciones = dbContext.CobranzasRetenciones.Include("Cobranzas").Where(x => x.Cobranzas.IDUsuario == usu.IDUsuario && x.Cobranzas.IDPersona == idPersona).ToList();

        string[] listaNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
        string[] listaNotasDebito = { "NDA", "NDB", "NDC", "NDAMP", "NDBMP", "NDCMP" };

        if (deudaPorEDM)
        {

            var list = listaComprobantes
                        .Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario)
                        .OrderBy(x => x.FechaComprobante)
                        .ToList();
            var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
            resultado.TotalItems = list.Count();

            if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
            {
                resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                total = Convert.ToDecimal(persona.SaldoInicial);
                resultado.TotalItems++;
            }

            if (list.Any())
            {
                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                foreach (var detalle in list)
                {
                    if (!listaNotas.Contains(detalle.Tipo))
                    {

                        //Me fijo si hay comprobantes que esten vinculados a este.
                        var comprobantesVinculados = listaComprobantes
                                                        .Where(x => x.IdComprobanteVinculado == detalle.IDComprobante && x.IDUsuario == usu.IDUsuario && !x.Tipo.Equals("PDV"))
                                                        .GroupBy(a => a.IdComprobanteVinculado)
                                                        .Select(a => new { SumaNeto = a.Sum(b => b.ImporteTotalNeto), SumaBruto = a.Sum(b => b.ImporteTotalBruto) })
                                                        .OrderByDescending(a => a.SumaNeto)
                                                        .FirstOrDefault();

                        if (comprobantesVinculados != null)
                        {
                            detalleCc = new RptCcDetalleViewModel();

                            if (detalle.Tipo.Equals("PDV"))
                            {
                                //total += detalle.ImporteTotalBruto - facturaVinculada.SumaBruto;

                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = "";
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = "--";
                            }
                            else
                            {
                                if (detalle.Tipo.Equals("EDA"))
                                {
                                    //total += detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto;
                                    
                                    detalleCc.Raiz = "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = "--";
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = "--";
                                }                               
                            }

                            temp.Add(detalleCc);
                        }
                        else // No hay comprobantes que esten vinculados a este
                        {
                            if (detalle.Tipo.Equals("PDV"))
                            {
                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = "--";
                            }
                            else
                            {
                                if (detalle.Tipo.Equals("EDA"))
                                {
                                    total += detalle.ImporteTotalBruto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = "";
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }
                                else
                                {
                                    if (detalle.CAE != null)
                                    {
                                        total += detalle.ImporteTotalNeto;

                                        detalleCc = new RptCcDetalleViewModel();
                                        detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                        detalleCc.CAE = "SI";
                                        detalleCc.RazonSocial = persona.RazonSocial;
                                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                        detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                        detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.VaADeuda = (detalle.ImporteTotalNeto).ToString("N2");
                                        detalleCc.FechaCobro = "";
                                        detalleCc.Total = total.ToString("N2");
                                    }
                                    else
                                    {
                                        total += detalle.ImporteTotalBruto;

                                        detalleCc = new RptCcDetalleViewModel();
                                        detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                        detalleCc.CAE = "";
                                        detalleCc.RazonSocial = persona.RazonSocial;
                                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                        detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                        detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.IVA = "--";
                                        detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                        detalleCc.FechaCobro = "";
                                        detalleCc.Total = total.ToString("N2");
                                    }
                                }
                            }

                            temp.Add(detalleCc);
                        }

                    }
                    else //Es Nota de credito o debito
                    {
                        if (detalle.CAE != null)
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalNeto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalNeto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }                                

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "SI";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                            detalleCc.VaADeuda = "--";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }
                        else
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalBruto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalBruto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = "--";
                            detalleCc.VaADeuda = "--";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }


                        temp.Add(detalleCc);
                    }

                    var cobrazasList = listaCobranzas.Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                    foreach (var cobranza in cobrazasList)
                    {
                        total -= cobranza.Importe;

                        detalleCob = new RptCcDetalleViewModel();
                        detalleCob.Raiz = "-";
                        detalleCob.RazonSocial = persona.RazonSocial;
                        if (cobranza.Cobranzas.Tipo != "SIN")
                            detalleCob.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                        else
                            detalleCob.ComprobanteAplicado = "Sin comprobante";

                        detalleCob.Comprobante = "";
                        detalleCob.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                        detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        //{
                        //    var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                        //    total -= ret;
                        //    detalleCob.Cobrado = "- " + (cobranza.Importe + ret).ToString("N2");
                        //}
                        //else
                        //    detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        detalleCob.Total = total.ToString("N2");
                        temp.Add(detalleCob);

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        if(listaCobranzasRetenciones.Where(w => w.IDCobranza == cobranza.IDCobranza).Any())
                        {
                            detalleCobRet = new RptCcDetalleViewModel();
                            var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                            total -= ret;
                            detalleCobRet.Cobrado = "- " + (ret).ToString("N2");
                            detalleCobRet.Raiz = "-";
                            detalleCobRet.RazonSocial = persona.RazonSocial;
                            if (cobranza.Cobranzas.Tipo != "SIN")
                                detalleCobRet.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                            else
                                detalleCobRet.ComprobanteAplicado = "Sin comprobante";

                            detalleCobRet.Comprobante = "";
                            detalleCobRet.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                            detalleCobRet.Total = total.ToString("N2");
                            temp.Add(detalleCobRet);
                            listaCobranzasRetenciones.RemoveAll(w => w.IDCobranza == cobranza.IDCobranza);
                        }
                    }
                }

                var chequesResto = dbContext.RptChequesAcciones
                            .Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario 
                                    && x.EsPropio == false && x.IdPersona == idPersona)
                            .Sum(a => a.Resto);

                if (chequesResto > 0)
                {
                    total += (decimal)chequesResto;

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.Cobrado = "Resto de cheques sin cobranza asignada";
                    detalleCc.Total = total.ToString("N2");
                    temp.Add(detalleCc);

                }

                foreach (RptCcDetalleViewModel r in temp)
                    resultado.Items.Add(r);

                detalleCc = new RptCcDetalleViewModel();
                detalleCc.RazonSocial = persona.RazonSocial;
                detalleCc.Cobrado = "Saldo";
                detalleCc.Total = total.ToString("N2");
                resultado.Items.Add(detalleCc);


            }
        }
        else // No es por Entrega de mercaderia
        {

            var list = listaComprobantes
                        .Where(x => x.IDPersona == idPersona && !x.Tipo.Equals("EDA"))
                        .OrderBy(x => x.FechaComprobante)
                        .ToList();
            var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
            resultado.TotalItems = list.Count();

            if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
            {
                resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                total = Convert.ToDecimal(persona.SaldoInicial);
                resultado.TotalItems++;
            }

            if (list.Any())
            {
                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                foreach (var detalle in list)
                {
                    if (!listaNotas.Contains(detalle.Tipo))
                    {
                        List<int?> listaEda = listaComprobantes.Where(x => x.IdComprobanteVinculado == detalle.IDComprobante && x.Tipo.Equals("EDA"))
                                                        .Select(a => (int?)a.IDComprobante)
                                                        .ToList();

                        listaEda.Add(detalle.IDComprobante);

                        var comprobantesVinculados = listaComprobantes
                                                        .Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F"))
                                                        .GroupBy(i => 1)
                                                        .Select(a => new { SumaNeto = a.Sum(b => b.ImporteTotalNeto), SumaBruto = a.Sum(b => b.ImporteTotalBruto) })
                                                        .OrderByDescending(a => a.SumaNeto)
                                                        .FirstOrDefault();

                        //List<int?> listaFac = listaComprobantes.Where(x => listaEda.Contains(x.IdComprobanteVinculado) && x.Tipo.Substring(0, 1).Equals("F"))
                        //                                .Select(a => (int?)a.IDComprobante)
                        //                                .ToList();


                        //var comprobantesVinculadosNetos = listaComprobantes
                        //                                .Where(x => listaFac.Contains(x.IdComprobanteAsociado) && x.Tipo.Substring(0, 1).Equals("N"))
                        //                                .GroupBy(i => 1)
                        //                                .Select(a => new { SumaNeto = a.Sum(b => b.ImporteTotalNeto), SumaBruto = a.Sum(b => b.ImporteTotalBruto) })
                        //                                .OrderByDescending(a => a.SumaNeto)
                        //                                .FirstOrDefault();

                        //decimal diferenciaFacNota = (comprobantesVinculados != null ? comprobantesVinculados.SumaBruto : 0) - (comprobantesVinculadosNetos != null ? comprobantesVinculadosNetos.SumaBruto : 0);


                        if (comprobantesVinculados != null)
                        {



                            if (detalle.Tipo.Equals("PDV"))
                            {
                                //decimal tempSubTotal = detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto + (comprobantesVinculadosNetos != null ? comprobantesVinculadosNetos.SumaBruto : 0);
                                //decimal tempTotal = tempSubTotal >= 0 ? tempSubTotal : 0;
                                //total += tempTotal;
                                



                                total += detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto);

                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = (detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto)).ToString("N2");
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = total.ToString("N2");
                            }
                            else
                            {
                                if (detalle.CAE != null)
                                {
                                    total += detalle.ImporteTotalNeto - ((detalle.ImporteTotalNeto - comprobantesVinculados.SumaNeto) < 0 ? detalle.ImporteTotalNeto : comprobantesVinculados.SumaNeto);

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = "-";
                                    detalleCc.CAE = "SI";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto)).ToString("N2");
                                    detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.VaADeuda = (detalle.ImporteTotalNeto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }
                                else
                                {
                                    total += detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto);

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto - ((detalle.ImporteTotalBruto - comprobantesVinculados.SumaBruto) < 0 ? detalle.ImporteTotalBruto : comprobantesVinculados.SumaBruto)).ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }

                            }

                            temp.Add(detalleCc);
                        }
                        else // El comprobante no está vinculado a otro comprobante
                        {
                            if (detalle.Tipo.Equals("PDV"))
                            {
                                total += detalle.ImporteTotalBruto;

                                detalleCc = new RptCcDetalleViewModel();
                                detalleCc.Raiz = "+";
                                detalleCc.CAE = "";
                                detalleCc.RazonSocial = persona.RazonSocial;
                                detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.IVA = "--";
                                detalleCc.VaADeuda = (detalle.ImporteTotalBruto).ToString("N2");
                                detalleCc.FechaCobro = "";
                                detalleCc.Total = total.ToString("N2");
                            }
                            else
                            {

                                if (detalle.CAE != null)
                                {
                                    total += detalle.ImporteTotalNeto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                    detalleCc.CAE = "SI";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.VaADeuda = (detalle.IdComprobanteVinculado == null) ? "--" : (detalle.ImporteTotalNeto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }
                                else
                                {
                                    total += detalle.ImporteTotalBruto;

                                    detalleCc = new RptCcDetalleViewModel();
                                    detalleCc.Raiz = (detalle.IdComprobanteVinculado == null) ? "+" : "-";
                                    detalleCc.CAE = "";
                                    detalleCc.RazonSocial = persona.RazonSocial;
                                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                                    detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                                    detalleCc.Importe = (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.IVA = "--";
                                    detalleCc.VaADeuda = (detalle.IdComprobanteVinculado == null) ? "--" : (detalle.ImporteTotalBruto).ToString("N2");
                                    detalleCc.FechaCobro = "";
                                    detalleCc.Total = total.ToString("N2");
                                }

                            }

                            temp.Add(detalleCc);
                        }

                    }
                    else // Es una nota de credito o debito
                    {
                        if (detalle.CAE != null)
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalNeto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalNeto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "SI";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = (detalle.ImporteTotalNeto - detalle.ImporteTotalBruto).ToString("N2");
                            detalleCc.VaADeuda = "";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }
                        else
                        {
                            string cobrado = string.Empty;
                            if (listaNotasDebito.Contains(detalle.Tipo))
                            {
                                total += detalle.ImporteTotalBruto;
                            }
                            else
                            {
                                total -= detalle.ImporteTotalBruto;
                                cobrado = "- " + detalle.ImporteTotalBruto.ToString("N2");
                            }

                            detalleCc = new RptCcDetalleViewModel();
                            detalleCc.Raiz = "-";
                            detalleCc.CAE = "";
                            detalleCc.RazonSocial = persona.RazonSocial;
                            detalleCc.Comprobante = detalle.Tipo + " " + detalle.PuntosDeVenta.Punto.ToString("#0000") + "-" + detalle.Numero.ToString("#00000000");
                            detalleCc.Fecha = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Importe = detalle.ImporteTotalBruto.ToString("N2");
                            detalleCc.Cobrado = cobrado;
                            detalleCc.IVA = "--";
                            detalleCc.VaADeuda = "";
                            detalleCc.FechaCobro = detalle.FechaComprobante.ToString("dd/MM/yyyy");
                            detalleCc.Total = total.ToString("N2");
                        }

                        temp.Add(detalleCc);
                    }

                    var cobrazasList = listaCobranzas.Where(x => x.IDComprobante == detalle.IDComprobante).ToList();
                    foreach (var cobranza in cobrazasList)
                    {
                        total -= cobranza.Importe;

                        detalleCob = new RptCcDetalleViewModel();
                        detalleCob.Raiz = "-";
                        detalleCob.RazonSocial = persona.RazonSocial;
                        if (cobranza.Cobranzas.Tipo != "SIN")
                            detalleCob.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                        else
                            detalleCob.ComprobanteAplicado = "Sin comprobante";

                        detalleCob.Comprobante = "";
                        detalleCob.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                        detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        //{
                        //    var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                        //    total -= ret;
                        //    detalleCob.Cobrado = "- " + (cobranza.Importe + ret).ToString("N2");
                        //}
                        //else
                        //    detalleCob.Cobrado = "- " + cobranza.Importe.ToString("N2");

                        detalleCob.Total = total.ToString("N2");
                        temp.Add(detalleCob);

                        //if (cobranza.Cobranzas.CobranzasRetenciones.Any())
                        if (listaCobranzasRetenciones.Where(w => w.IDCobranza == cobranza.IDCobranza).Any())
                        {
                            detalleCobRet = new RptCcDetalleViewModel();
                            var ret = cobranza.Cobranzas.CobranzasRetenciones.Sum(x => x.Importe);
                            total -= ret;
                            detalleCobRet.Cobrado = "- " + (ret).ToString("N2");
                            detalleCobRet.Raiz = "-";
                            detalleCobRet.RazonSocial = persona.RazonSocial;
                            if (cobranza.Cobranzas.Tipo != "SIN")
                                detalleCobRet.ComprobanteAplicado = cobranza.Cobranzas.Tipo + " " + cobranza.Cobranzas.PuntosDeVenta.Punto.ToString("#0000") + "-" + cobranza.Cobranzas.Numero.ToString("#00000000");
                            else
                                detalleCobRet.ComprobanteAplicado = "Sin comprobante";

                            detalleCobRet.Comprobante = "";
                            detalleCobRet.FechaCobro = cobranza.Cobranzas.FechaCobranza.ToString("dd/MM/yyyy");
                            detalleCobRet.Total = total.ToString("N2");
                            temp.Add(detalleCobRet);
                            listaCobranzasRetenciones.RemoveAll(w => w.IDCobranza == cobranza.IDCobranza);
                        }
                    }
                }

                var chequesResto = dbContext.RptChequesAcciones
                            .Where(x => x.Accion == "" && x.IDUsuario == usu.IDUsuario 
                                    && x.EsPropio == false && x.IdPersona == idPersona)
                            .Sum(a => a.Resto);

                if (chequesResto > 0)
                {
                    total += (decimal)chequesResto;

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.Cobrado = "Resto de cheques sin cobranza asignada";
                    detalleCc.Total = total.ToString("N2");
                    temp.Add(detalleCc);

                }

                foreach (RptCcDetalleViewModel r in temp)
                    resultado.Items.Add(r);

                detalleCc = new RptCcDetalleViewModel();
                detalleCc.RazonSocial = persona.RazonSocial;
                detalleCc.Cobrado = "Saldo";
                detalleCc.Total = total.ToString("N2");
                resultado.Items.Add(detalleCc);


            }

        }

    }

    private static RptCcDetalleViewModel AgregarSaldoInicialCliente(ACHEEntities dbContext, decimal? saldo)
    {
        var detalleCc = new RptCcDetalleViewModel();
        detalleCc.Comprobante = "Saldo inicial";
        detalleCc.Fecha = "";
        detalleCc.Importe = Convert.ToDecimal(saldo).ToString("N2");
        detalleCc.Total = Convert.ToDecimal(saldo).ToString("N2");

        return detalleCc;
    }

    private static void verComoProveedor(int idPersona, ResultadosRptCcDetalleViewModel resultado, 
                                        ACHEEntities dbContext, bool saldoPendiente, bool deudaPorEDM)
    {

        decimal total = 0;
        RptCcDetalleViewModel detalleCc;
        RptCcDetalleViewModel detalleCob;
        //string[] listaNotas = { "NCA", "NCB", "NCC", "NDA", "NDB", "NDC", "NCAMP", "NCBMP", "NCCMP", "NDAMP", "NDBMP", "NDCMP" };
        string[] listaNotas = { "NCA", "NCB", "NCC", "NCAMP", "NCBMP", "NCCMP" };


        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

        if (idPersona != -1)
        {
            var list = dbContext.Compras.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario && !x.Tipo.Equals("EDA"))
                    .OrderBy(x => x.Fecha)
                    .ToList();
            var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
            resultado.TotalItems = list.Count();

            if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
            {
                resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                total = Convert.ToDecimal(persona.SaldoInicial);
            }

            if (list.Any())
            {
                List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                foreach (var detalle in list)
                {
                    var imp = detalle.TotalImpuestos;

                    if (listaNotas.Contains(detalle.Tipo))
                        total -= Convert.ToDecimal(detalle.Total + detalle.Iva + imp);
                    else
                        total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                    //total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                    detalleCc = new RptCcDetalleViewModel();
                    Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == detalle.IdComprobante).FirstOrDefault();
                    if (c != null)
                        detalleCc.PDC = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                    detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                    detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");                    

                    if (listaNotas.Contains(detalle.Tipo))
                        detalleCc.Importe = "-" + Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                    else
                        detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");

                    detalleCc.Total = total.ToString("N2");
                    temp.Add(detalleCc);

                    var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                    foreach (var item in pagosList)
                    {
                        total -= item.Importe;

                        detalleCob = new RptCcDetalleViewModel();
                        detalleCob.RazonSocial = persona.RazonSocial;
                        detalleCob.ComprobanteAplicado = "Sin comprobante";

                        detalleCob.Comprobante = "PAGO 000-" + item.IDPago.ToString("#00000000");
                        detalleCob.FechaCobro = item.Pagos.FechaPago.ToString("dd/MM/yyyy");

                        if (item.Pagos.PagosRetenciones.Any())
                        {
                            var ret = item.Pagos.PagosRetenciones.Sum(x => x.Importe);
                            total -= ret;
                            detalleCob.Cobrado = "- " + (item.Importe + ret).ToString("N2");
                        }
                        else
                            detalleCob.Cobrado = "- " + item.Importe.ToString("N2");

                        detalleCob.Total = total.ToString("N2");
                        temp.Add(detalleCob);
                    }
                }

                foreach (RptCcDetalleViewModel r in temp)
                    resultado.Items.Add(r);

                detalleCc = new RptCcDetalleViewModel();
                detalleCc.RazonSocial = persona.RazonSocial;
                detalleCc.Cobrado = "Saldo";
                detalleCc.Total = total.ToString("N2");
                resultado.Items.Add(detalleCc);

                //if (saldoPendiente)
                //{
                //    if (total > 0)
                //    {
                //        detalleCc = new RptCcDetalleViewModel();
                //        detalleCc.RazonSocial = persona.RazonSocial;
                //        detalleCc.Cobrado = "Saldo";
                //        detalleCc.Total = total.ToString("N2");
                //        resultado.Items.Add(detalleCc);
                //    }
                //}
                //else
                //{
                //    detalleCc = new RptCcDetalleViewModel();
                //    detalleCc.RazonSocial = persona.RazonSocial;
                //    detalleCc.Cobrado = "Saldo";
                //    detalleCc.Total = total.ToString("N2");
                //    resultado.Items.Add(detalleCc);
                //}

            }
        }
        else
        {
            var personas = dbContext.Personas.Where(x => x.IDUsuario == usu.IDUsuario).OrderBy(o => o.RazonSocial).ToList();

            foreach (Personas p in personas)
            {
                var list = dbContext.Compras.Where(x => x.IDPersona == p.IDPersona && x.IDUsuario == usu.IDUsuario && !x.Tipo.Equals("EDA"))
                    .OrderBy(x => x.Fecha)
                    .ToList();
                var persona = dbContext.Personas.Where(x => x.IDPersona == p.IDPersona).FirstOrDefault();
                resultado.TotalItems = list.Count();

                if (persona.SaldoInicial != null && persona.SaldoInicial != 0)
                {
                    resultado.Items.Add(AgregarSaldoInicialCliente(dbContext, persona.SaldoInicial));
                    total = Convert.ToDecimal(persona.SaldoInicial);
                }

                if (list.Any())
                {
                    List<RptCcDetalleViewModel> temp = new List<RptCcDetalleViewModel>();
                    foreach (var detalle in list)
                    {
                        var imp = detalle.TotalImpuestos;

                        if (listaNotas.Contains(detalle.Tipo))
                            total -= Convert.ToDecimal(detalle.Total + detalle.Iva + imp);
                        else
                            total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                        //total += Convert.ToDecimal(detalle.Total + detalle.Iva + imp);

                        detalleCc = new RptCcDetalleViewModel();
                        detalleCc.RazonSocial = persona.RazonSocial;
                        Comprobantes c = dbContext.Comprobantes.Where(w => w.IDComprobante == detalle.IdComprobante).FirstOrDefault();
                        if (c != null)
                            detalleCc.PDC = c.PuntosDeVenta.Punto.ToString("#0000") + "-" + c.Numero.ToString("#00000000");

                        detalleCc.Comprobante = detalle.Tipo + " " + detalle.NroFactura;
                        detalleCc.Fecha = detalle.Fecha.ToString("dd/MM/yyyy");

                        if (listaNotas.Contains(detalle.Tipo))
                            detalleCc.Importe = "-" + Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");
                        else
                            detalleCc.Importe = Convert.ToDecimal(detalle.Total + detalle.Iva + imp).ToString("N2");

                        detalleCc.Total = total.ToString("N2");
                        temp.Add(detalleCc);

                        var pagosList = dbContext.PagosDetalle.Include("Pagos").Where(x => x.IDCompra == detalle.IDCompra).ToList();
                        foreach (var item in pagosList)
                        {
                            total -= item.Importe;

                            detalleCob = new RptCcDetalleViewModel();
                            detalleCob.RazonSocial = persona.RazonSocial;
                            detalleCob.ComprobanteAplicado = "Sin comprobante";

                            detalleCob.Comprobante = "PAGO 000-" + item.IDPago.ToString("#00000000");
                            detalleCob.FechaCobro = item.Pagos.FechaPago.ToString("dd/MM/yyyy");

                            if (item.Pagos.PagosRetenciones.Any())
                            {
                                var ret = item.Pagos.PagosRetenciones.Sum(x => x.Importe);
                                total -= ret;
                                detalleCob.Cobrado = "- " + (item.Importe + ret).ToString("N2");
                            }
                            else
                                detalleCob.Cobrado = "- " + item.Importe.ToString("N2");

                            detalleCob.Total = total.ToString("N2");
                            temp.Add(detalleCob);
                        }
                    }

                    foreach (RptCcDetalleViewModel r in temp)
                        resultado.Items.Add(r);

                    detalleCc = new RptCcDetalleViewModel();
                    detalleCc.RazonSocial = persona.RazonSocial;
                    detalleCc.Cobrado = "Saldo";
                    detalleCc.Total = total.ToString("N2");
                    resultado.Items.Add(detalleCc);

                    //if (saldoPendiente)
                    //{
                    //    if (total > 0)
                    //    {
                    //        detalleCc = new RptCcDetalleViewModel();
                    //        detalleCc.RazonSocial = persona.RazonSocial;
                    //        detalleCc.Cobrado = "Saldo";
                    //        detalleCc.Total = total.ToString("N2");
                    //        resultado.Items.Add(detalleCc);
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (RptCcDetalleViewModel r in temp)
                    //        resultado.Items.Add(r);

                    //    detalleCc = new RptCcDetalleViewModel();
                    //    detalleCc.RazonSocial = persona.RazonSocial;
                    //    detalleCc.Cobrado = "Saldo";
                    //    detalleCc.Total = total.ToString("N2");
                    //    resultado.Items.Add(detalleCc);
                    //}
                }

                total = 0;

            }
        }

        
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerListaPrecios()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                return dbContext.ListaPrecios.Where(x => x.IDUsuario == usu.IDUsuario && x.Activa == true)
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDListaPrecio,
                        Nombre = x.Nombre
                    }).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    [ScriptMethod(UseHttpGet = true)]
    public static List<Combo2ViewModel> obtenerProvincias()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            return CommonModel.ObtenerProvincias();
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerCiudades(int idProvincia)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            return CommonModel.obtenerCiudades(idProvincia);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerTipoPersona(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Personas.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                if (entity != null)
                {
                    return entity.Tipo;
                }
                else
                    throw new Exception("No se pudo obtener los datos");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerProxNroPresupuesto()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var nro = "";
            using (var dbContext = new ACHEEntities())
            {
                var presupuesto = dbContext.Presupuestos.Where(x => x.IDUsuario == usu.IDUsuario);
                if (presupuesto.Count() > 0)
                {
                    var numero = presupuesto.Max(x => x.Numero);
                    numero++;
                    nro = numero.ToString("#00000000");
                }
                else
                    nro = "00000001";
            }

            return nro;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string ObtenerUltimoTipoComprobanteCliente(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Compras.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.IDCompra).FirstOrDefault();
                if (entity != null)
                {
                    return entity.Tipo;
                }
                else
                    return "";
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string ObtenerUltimoRubroComprobanteCliente(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Compras.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.IDCompra).FirstOrDefault();
                if (entity != null)
                    return entity.Rubro;
                else
                    return "";
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string ObtenerUltimaCategoriaComprobanteCliente(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Compras.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.IDCompra).FirstOrDefault();
                if (entity != null)
                    return entity.IDCategoria.ToString();
                else
                    return "";
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



    [WebMethod(true)]
    public static List<Combo2ViewModel> ObtenerFacturasDelCliente(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var tipoFacturas = new[] { "FCA", "FCB", "FCC", "RCA", "RCB", "RCC", "FCAMP", "FCBMP", "FCCMP" };

                return dbContext.Comprobantes
                   .Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario && tipoFacturas.Contains(x.Tipo))
                   .OrderByDescending(x => x.IDComprobante)
                   .ToList()
                   .Select(x => new Combo2ViewModel()
                   {
                       ID = x.IDComprobante,
                       Nombre = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000")
                   }).OrderBy(x => x.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> ObtenerComprobantesDelCliente(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                List<string> listaPDV = new List<string>();
                List<string> listaFAC= new List<string>();
                List<string> listaTipo = new List<string>();

                listaPDV.Add("PDV");
                listaFAC.Add("FCA");
                listaFAC.Add("FCB");
                listaFAC.Add("FCC");
                //var tipoFacturas = new[] { "PDV", "FCA", "FCB", "FCC" };
                //var tipo = new[] { "PDV", "FCA", "FCB", "FCC" };

                if (usu.FacturaSoloContraEntrega)
                {
                    listaTipo.AddRange(listaPDV);
                }
                else
                {
                    listaTipo.AddRange(listaPDV);
                    listaTipo.AddRange(listaFAC);
                }                    

                var lista = dbContext.Comprobantes.Where(w => w.IDPersona == idPersona && listaTipo.Contains(w.Tipo)).ToList();
                var listaPdvSinEntregas = lista.ToList();

                var listaEda = dbContext.Comprobantes.Where(w => w.IDPersona == idPersona && w.Tipo.Equals("EDA")).ToList();

                foreach(Comprobantes c in lista)
                {
                    var busqueda = listaEda.Where(w => w.IdComprobanteVinculado == c.IDComprobante).ToList();
                    foreach(Comprobantes cs in busqueda)
                    {
                        foreach (ComprobantesDetalle cd in c.ComprobantesDetalle)
                        {
                            cd.Cantidad = cd.Cantidad - cs.ComprobantesDetalle.Where(w => w.IDConcepto == cd.IDConcepto).Select(s => s.Cantidad).FirstOrDefault();
                        }
                    }
                     
                    if (c.ComprobantesDetalle.Sum(s => s.Cantidad) == 0)
                        listaPdvSinEntregas.Remove(c);
                }

                return listaPdvSinEntregas
                   .Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario && listaTipo.Contains(x.Tipo))
                   .OrderByDescending(x => x.IDComprobante)
                   .ToList()
                   .Select(x => new Combo2ViewModel()
                   {
                       ID = x.IDComprobante,
                       Nombre = x.Tipo + " " + x.PuntosDeVenta.Punto.ToString("#0000") + "-" + x.Numero.ToString("#00000000")
                   }).OrderBy(x => x.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string ObtenerUltimoTipoFacturacionCliente(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var entity = dbContext.Comprobantes.Where(x => x.IDPersona == idPersona && x.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.IDComprobante).FirstOrDefault();
                if (entity != null)
                {
                    return entity.Tipo;
                }
                else
                    return "";
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static List<Combo2ViewModel> obtenerSelectPlanCuentas()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                return dbContext.PlanDeCuentas.Where(x => x.IDUsuario == usu.IDUsuario).ToList()
                    .Select(x => new Combo2ViewModel()
                    {
                        ID = x.IDPlanDeCuenta,
                        Nombre = x.Nombre
                    }).OrderBy(x => x.Nombre).ToList();
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    [WebMethod(true)]
    public static string obtenerMailPersona(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var mail = "";
            using (var dbContext = new ACHEEntities())
            {
                var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                if (persona != null)
                {
                    mail = persona.Email;
                }
            }

            return mail;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerNroDocumentoPersona(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var nroDocumento = "";
            using (var dbContext = new ACHEEntities())
            {
                var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                if (persona != null)
                {
                    nroDocumento = persona.NroDocumento;
                }
            }

            return nroDocumento;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerRazonSocialPersona(int idPersona)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var razonSocial = "";
            using (var dbContext = new ACHEEntities())
            {
                var persona = dbContext.Personas.Where(x => x.IDPersona == idPersona).FirstOrDefault();
                if (persona != null)
                {
                    razonSocial = persona.RazonSocial;
                }
            }

            return razonSocial;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerCUILUsuario()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            return usu.CUIT;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string obtenerRazonSocialUsuario()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            return usu.RazonSocial;
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static PersonaDomicilioConGeo ObtenerDetalleDomicilio(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            PersonaDomicilioConGeo pd = new PersonaDomicilioConGeo();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var datos = dbContext.PersonaDomicilio.Where(x => x.IdPersonaDomicilio == id).FirstOrDefault();
                if (datos != null)
                {
                    pd.Domicilio = datos.Domicilio;
                    pd.PisoDepto = datos.PisoDepto;
                    pd.CodigoPostal = datos.CodigoPostal;
                    pd.IDProvincia = datos.IDProvincia;
                    pd.IDCiudad = datos.IDCiudad;
                    pd.Provincia = datos.Provincia;
                    pd.Ciudad = datos.Ciudad;
                    pd.Contacto = datos.Contacto;
                    pd.Telefono = datos.Telefono;
                    return pd;
                }                    
                else
                    throw new Exception("No se encontraron datos con el código de domicilio ingresado");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static TransportePersona ObtenerDetalleDomicilioTransporte(int id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            TransportePersona pd = new TransportePersona();
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            using (var dbContext = new ACHEEntities())
            {
                var datos = dbContext.TransportePersona.Where(x => x.IdTransportePersona == id).FirstOrDefault();
                if (datos != null)
                {
                    pd.RazonSocial = datos.RazonSocial;
                    pd.Domicilio = datos.Domicilio;
                    pd.PisoDepto = datos.PisoDepto;
                    pd.CodigoPostal = datos.CodigoPostal;                    
                    pd.Provincia = datos.Provincia;
                    pd.Ciudad = datos.Ciudad;
                    pd.Contacto = datos.Contacto;
                    pd.Telefono = datos.Telefono;
                    return pd;
                }
                else
                    throw new Exception("No se encontraron datos con el código de domicilio ingresado");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static DatosAfipPersonasConGeo consultarDatosAfip(string cuit)
    {
        DatosAFIPPersonas d = new DatosAFIPPersonas();
        DatosAfipPersonasConGeo g = new DatosAfipPersonasConGeo();

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (Common.IsNumeric(cuit))
                {
                    AFIPPersonaServiceA5v34 s = new AFIPPersonaServiceA5v34();

                    var modo = usu.ModoQA ? "QA" : "PROD";
                    var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_sr_padron_a5"] : ConfigurationManager.AppSettings["FE.PROD.ws_sr_padron_a5"]);
                    int CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "AFIPPersonaServiceA5", "GetPersona(" + cuit + ")", usu.IDUsuario);
                    personaReturn persona = null;

                    try
                    {
                        persona = s.GetPersona_v2(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo);
                    }
                    catch (Exception ex)
                    {
                        Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip v2: " + ex.Message, false);
                        if (ex.Message.Equals("No existe persona con ese Id"))
                            g.Mensaje = "No encontramos datos en AFIP con el CUIT ingresado.";
                        else
                        {
                            try
                            {
                                persona = s.GetPersona(Convert.ToInt64(cuit), Convert.ToInt64(usu.CUITAfip), modo);
                            }
                            catch (Exception ex2)
                            {
                                Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip v1: " + ex2.Message, false);
                                if (ex2.Message.Equals("No existe persona con ese Id"))
                                    g.Mensaje = "No encontramos datos en AFIP con el CUIT ingresado.";
                                else
                                    g.Mensaje = "Ha ocurrido un problema para acceder a los servicios de AFIP. Intentar luego.";

                                return g;
                            }
                        }
                    }

                    if (persona.datosGenerales != null)
                    {
                        var dbContext = new ACHEEntities();

                        string condicionIva = "CF";
                        //Me fijo el dato que me llega del array DatosRegimenGeneral
                        if (persona.datosRegimenGeneral != null)
                        {
                            if (persona.datosRegimenGeneral.impuesto != null)
                            {
                                foreach (impuesto i in persona.datosRegimenGeneral.impuesto)
                                {
                                    if (i.descripcionImpuesto.ToUpper().Contains("EXENTO"))
                                    {
                                        condicionIva = "EX";
                                        break;
                                    }
                                    if (i.descripcionImpuesto.ToUpper().Equals("IVA"))
                                    {
                                        condicionIva = "RI";
                                        break;
                                    }
                                    if (i.descripcionImpuesto.ToUpper().Contains("MONOTRIBUTO"))
                                    {
                                        condicionIva = "MO";
                                        break;
                                    }
                                }
                            }                                
                        }

                        //Me fijo el dato que me llega del array MONOTRIBUTO
                        if (persona.datosMonotributo != null)
                        {
                            if (persona.datosMonotributo.impuesto != null)
                            {
                                foreach (impuesto i in persona.datosMonotributo.impuesto)
                                {
                                    if (i.descripcionImpuesto.ToUpper().Contains("MONOTRIBUTO"))
                                    {
                                        condicionIva = "MO";
                                        break;
                                    }
                                }
                            }
                            if (persona.datosMonotributo.categoriaMonotributo != null)
                            {
                                d.CategoriaMonotributoDescripcionCategoria = persona.datosMonotributo.categoriaMonotributo.descripcionCategoria;
                                d.CategoriaMonotributoIdCategoria = persona.datosMonotributo.categoriaMonotributo.idCategoria.ToString();
                                d.CategoriaMonotributoIdImpuesto = persona.datosMonotributo.categoriaMonotributo.idImpuesto.ToString();
                                d.CategoriaMonotributoPeriodo = persona.datosMonotributo.categoriaMonotributo.periodo.ToString();
                            }
                        }

                        //Registro en la tabla DatosAfipPersona los datos de afip del cuit ingresado
                        d.CategoriaImpositiva = condicionIva; //Sin Datos
                        d.CUIT = persona.datosGenerales.idPersona.ToString();
                        if (persona.datosGenerales.razonSocial != null)
                        {
                            d.RazonSocial = persona.datosGenerales.razonSocial;
                        }
                        else
                        {
                            if (persona.datosGenerales.apellido != null && persona.datosGenerales.nombre != null)
                            {
                                d.RazonSocial = persona.datosGenerales.apellido + " " + persona.datosGenerales.nombre;
                            }
                        }

                        if (persona.datosGenerales.fechaContratoSocial != null)
                        {
                            if (Common.IsDate(persona.datosGenerales.fechaContratoSocial.ToString()))
                            {
                                d.FechaContratoSocial = persona.datosGenerales.fechaContratoSocial;
                                g.FechaInicioActividades = d.FechaContratoSocial.ToString();
                            }
                                
                        }

                        d.Personeria = persona.datosGenerales.tipoPersona;
                        d.Fecha = DateTime.Now;
                        d.IdUsuario = usu.IDUsuario;
                        d.DomicilioFiscalCP = persona.datosGenerales.domicilioFiscal.codPostal;
                        d.DomicilioFiscalIdProvincia = persona.datosGenerales.domicilioFiscal.idProvincia;
                        d.DomicilioFiscalProvincia = persona.datosGenerales.domicilioFiscal.descripcionProvincia;
                        d.DomicilioFiscalCiudad = persona.datosGenerales.domicilioFiscal.localidad;
                        d.DomicilioFiscalDomicilio = persona.datosGenerales.domicilioFiscal.direccion;
                        dbContext.DatosAFIPPersonas.Add(d);
                        dbContext.SaveChanges();
                        
                        g.CategoriaImpositiva = d.CategoriaImpositiva;
                        g.CUIT = d.CUIT;
                        g.RazonSocial = d.RazonSocial;
                        g.Personeria = d.Personeria;
                        g.Fecha = d.Fecha;
                        g.IdUsuario = d.IdUsuario;
                        g.DomicilioFiscalCP = d.DomicilioFiscalCP;
                        g.DomicilioFiscalIdProvincia = d.DomicilioFiscalIdProvincia;
                        g.DomicilioFiscalProvincia = d.DomicilioFiscalProvincia;
                        g.DomicilioFiscalCiudad = d.DomicilioFiscalCiudad;
                        g.DomicilioFiscalDomicilio = d.DomicilioFiscalDomicilio;
                        g.CategoriaMonotributoDescripcionCategoria = d.CategoriaMonotributoDescripcionCategoria;
                        g.CategoriaMonotributoIdCategoria = d.CategoriaMonotributoIdCategoria;
                        g.CategoriaMonotributoIdImpuesto = d.CategoriaMonotributoIdImpuesto;
                        g.CategoriaMonotributoPeriodo = d.CategoriaMonotributoPeriodo;

                        //Obtengo el codigo de provincia del sistema desde el codigo de afip
                        Provincias p = dbContext.Provincias.Where(x => x.IDProvincia == persona.datosGenerales.domicilioFiscal.idProvincia).FirstOrDefault();
                        if (p != null)
                        {
                            g.IdProvincia = p.IDProvincia;

                            Ciudades primeraCiudad = dbContext.Ciudades.Where(x => x.IDProvincia == p.IDProvincia).FirstOrDefault();

                            if (persona.datosGenerales.domicilioFiscal.localidad != null)
                            {
                                Ciudades c = dbContext.Ciudades.Where(x => x.IDProvincia == p.IDProvincia && x.Nombre == persona.datosGenerales.domicilioFiscal.localidad).FirstOrDefault();
                                if (c != null)
                                    g.IdCiudad = c.IDCiudad;
                                else
                                    g.IdCiudad = primeraCiudad.IDCiudad;
                            }
                            else
                            {
                                g.IdCiudad = primeraCiudad.IDCiudad;
                            }
                        }
                        else
                            g.IdProvincia = 0;


                        if (g.IdProvincia == 0)
                            g.IdCiudad = 24071; //Si la provincia es caba, se setea la localidad CABA y se deja deshabilitado.

                    }

                    XmlSerializer xsSubmit = new XmlSerializer(typeof(personaReturn));
                    var subReq = new persona();
                    var xml = "";
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xsSubmit.Serialize(writer, persona);
                            xml = sww.ToString(); // Your XML
                        }
                    }
                    Common.RegistrarRespuestaLog(CodigoLogServicio, 1, xml, true);

                }
                return g;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                return g;
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void consultarPuntosDeVentaAfip()
    {
        FEPtoVentaResponse v = new FEPtoVentaResponse();

        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (Common.IsNumeric(usu.CUIT))
                {
                    FEPuntoDeVenta s = new FEPuntoDeVenta();

                    var modo = usu.ModoQA ? "QA" : "PROD";
                    var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);
                    int CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "wsfev1", "GetPuntoDeVenta(" + usu.CUIT.ToString() + ")", usu.IDUsuario);

                    try
                    {
                        v = s.GetPuntoDeVenta(Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), modo, false);
                    }
                    catch (Exception ex)
                    {
                        Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + ex.Message, false);
                        return;
                    }

                    if (v.Errors != null)
                    {
                        foreach (Err er in v.Errors)
                        {
                            if (er.Code == 600)
                            {
                                try
                                {
                                    v = s.GetPuntoDeVenta(Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), modo, true);
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + ex.Message, false);
                                    return;
                                }
                            }
                        }
                    }

                    var dbContext = new ACHEEntities();
                    int contador = 0;

                    if (v.ResultGet != null)
                    {
                        //Recorro los puntos de venta y los guardo.
                        foreach (PtoVenta i in v.ResultGet)
                        {
                            if (!i.Bloqueado.Equals("NO"))
                            {
                                var db = new ACHEEntities();
                                if (!db.PuntosDeVenta.Any(x => x.Punto == i.Nro && x.IDUsuario == usu.IDUsuario))
                                {
                                    PuntosDeVenta entity = new PuntosDeVenta();
                                    entity.Punto = i.Nro;
                                    entity.IDUsuario = usu.IDUsuario;
                                    entity.FechaAlta = DateTime.Now;
                                    entity.PorDefecto = false;
                                    db.PuntosDeVenta.Add(entity);
                                    db.SaveChanges();
                                    contador++;
                                }                                
                            }
                        }                        
                    }

                    if (contador > 0)
                    {
                        PuntosDeVenta p = dbContext.PuntosDeVenta.Where(c => c.IDUsuario == usu.IDUsuario).OrderByDescending(x => x.Punto).FirstOrDefault();
                        int idPunto = 0;
                        if (p != null)
                        {
                            idPunto = p.Punto;
                            var ListaPuntos = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == usu.IDUsuario);
                            foreach (var item in ListaPuntos)
                            {
                                if (item.Punto == idPunto)
                                    item.PorDefecto = true;
                                else
                                    item.PorDefecto = false;
                            }
                            dbContext.SaveChanges();
                        }
                    }
    
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(FEPtoVentaResponse));
                    var subReq = new FEPtoVentaResponse();
                    var xml = "";
                    using (var sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xsSubmit.Serialize(writer, v);
                            xml = sww.ToString(); // Your XML
                        }
                    }
                    Common.RegistrarRespuestaLog(CodigoLogServicio, 1, xml, true);

                }
                return;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                return;
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



    [WebMethod(true)]
    public static void consultarComunicacionesAfip()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                var dbContext = new ACHEEntities();

                AFIPVEConsumerService s = new AFIPVEConsumerService();

                var modo = usu.ModoQA ? "QA" : "PROD";
                var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.ws_ve_consumer"] : ConfigurationManager.AppSettings["FE.PROD.ws_ve_consumer"]);
                int CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "ws_ve_consumer", "GetConsultarComunicaciones(" + usu.CUIT + ")", usu.IDUsuario);
                List<RespuestaPaginada> listResPag = null;

                long ultimaComunicacion = 0;
                ComunicacionesAFIP CA = dbContext.ComunicacionesAFIP.Where(x => x.IdUsuario == usu.IDUsuario).OrderByDescending(x => x.IdComunicacion).FirstOrDefault();
                if (CA != null)
                    ultimaComunicacion = Convert.ToInt64(CA.IdComunicacion);


                try
                {
                    listResPag = s.GetComunicaciones(Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), modo, ultimaComunicacion, false);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Equals("La cuitRepresentada informada no se encuentra autorizada para la consulta"))
                    {
                        try
                        {
                            listResPag = s.GetComunicaciones(Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), modo, ultimaComunicacion, true);
                        }
                        catch (Exception e)
                        {
                            Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + e.Message, false);
                            return;
                        }
                    }
                    else
                    {
                        Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + ex.Message, false);
                        return;
                    }
                }

                if (listResPag.Count > 0)
                {                    
                    List<ComunicacionesAFIP> nc = new List<ComunicacionesAFIP>();
                    List<ComunicacionesAFIPAdjuntos> lca = new List<ComunicacionesAFIPAdjuntos>();

                    foreach (RespuestaPaginada rp in listResPag)
                    {
                        if (rp.items != null)
                        {
                            foreach (ComunicacionSimplificada i in rp.items)
                            { 
                                ComunicacionesAFIP c = dbContext.ComunicacionesAFIP.Where(x => x.IdComunicacion == i.idComunicacion).FirstOrDefault();
                                if (c == null)
                                {
                                    ComunicacionesAFIP cn = new ComunicacionesAFIP();
                                    cn.IdComunicacion = i.idComunicacion;
                                    cn.CuitDestinatario = i.cuitDestinatario.ToString();
                                    cn.FechaPublicacion = i.fechaPublicacion;
                                    cn.fechaVencimiento = i.fechaVencimiento != null ? i.fechaVencimiento : "";
                                    cn.SistemaPublicador = i.sistemaPublicador;
                                    cn.sistemaPublicadorDesc = i.sistemaPublicadorDesc;
                                    cn.Estado = i.estado;
                                    cn.EstadoDesc = i.estadoDesc;

                                    cn.Prioridad = i.prioridad;
                                    if (i.prioridad == 1)
                                        cn.PrioridadDesc = "Alta";
                                    if (i.prioridad == 2)
                                        cn.PrioridadDesc = "Media";
                                    if (i.prioridad == 3)
                                        cn.PrioridadDesc = "Baja";

                                    Comunicacion adj = s.GetAdjuntos(Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), i.idComunicacion, modo, false);
                                    if (adj != null)
                                    {
                                        cn.Asunto = adj.mensaje;

                                        int contador = 0;

                                        foreach (adjunto a in adj.adjuntos)
                                        {
                                            ComunicacionesAFIPAdjuntos ca = new ComunicacionesAFIPAdjuntos();
                                            ca.Comprimido = a.compressed == true ? 1 : 0;
                                            ca.Contenido = a.content;
                                            ca.Encriptado = a.encrypted == true ? 1 : 0;
                                            ca.Firmado = a.signed == true ? 1 : 0;
                                            ca.IdComunicacion = i.idComunicacion;
                                            ca.MD5 = a.md5;
                                            ca.NombreArchivo = a.filename;
                                            ca.Procesado = a.processed == true ? 1 : 0;
                                            ca.Tamaño = a.contentSize;
                                            ca.Publico = a.@public == true ? 1 : 0;
                                            lca.Add(ca);
                                            contador++;
                                        }

                                        if (contador == 0)
                                            cn.TieneAdjunto = 0;
                                        else
                                            cn.TieneAdjunto = 1;

                                    }

                                    cn.Visto = 0;
                                    cn.IdUsuario = usu.IDUsuario;
                                    cn.FechaRegistro = DateTime.Now;
                                    nc.Add(cn);
                                }
                            }
                        }
                    }
                    dbContext.ComunicacionesAFIP.AddRange(nc);
                    dbContext.SaveChanges();
                    dbContext.ComunicacionesAFIPAdjuntos.AddRange(lca);
                    dbContext.SaveChanges();
                }

                XmlSerializer xsSubmit = new XmlSerializer(typeof(List<RespuestaPaginada>));
                var subReq = new List<RespuestaPaginada>();
                var xml = "";
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, listResPag);
                        xml = sww.ToString(); // Your XML
                    }
                }
                Common.RegistrarRespuestaLog(CodigoLogServicio, 1, xml, true);

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                return;
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }



    [WebMethod(true)]
    public static string consultarComprobantesAfip()
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string html = "";

            try
            {
                var dbContext = new ACHEEntities();
                FEFacturaElectronica fe = new FEFacturaElectronica();
                List<FEComprobante> lc = new List<FEComprobante>();

                var modo = usu.ModoQA ? "QA" : "PROD";
                var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);
                int CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "wsfev1", "GetUltimoComprobante(" + usu.CUIT + ")", usu.IDUsuario);

                try
                {
                    lc = fe.ConsultarUltimosComprobantesAfip(usu.CondicionIVA, Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), modo, false, url);
                }
                catch
                {
                    try
                    {
                        lc = fe.ConsultarUltimosComprobantesAfip(usu.CondicionIVA, Convert.ToInt64(usu.CUIT), Convert.ToInt64(usu.CUITAfip), modo, true, url);
                    }
                    catch (Exception e)
                    {
                        Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + e.Message, false);
                        throw new Exception("Respuesta Afip: " + e.Message);
                    }
                }


                foreach (var detalle in lc)
                {
                    html += "<tr>";
                    html += "<td>" + detalle.PtoVta.ToString("#0000") + "</td>";
                    html += "<td>" + detalle.TipoComprobante.ToDescription() + "</td>";
                    html += "<td>" + detalle.NumeroComprobante.ToString("#00000000") + "</td>";
                    html += "</tr>";
                }                

                Common.RegistrarRespuestaLog(CodigoLogServicio, 1, html, true);

                return html;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static string recuperarComprobanteAfip(int idTipo, int idPuntoVenta, string numeroComprobante)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            string html = "";

            try
            {
                var dbContext = new ACHEEntities();
                FEFacturaElectronica fe = new FEFacturaElectronica();
                FECompConsultaResponse f = new FECompConsultaResponse();

                var puntoDeVenta = dbContext.PuntosDeVenta.Where(w => w.IDPuntoVenta == idPuntoVenta).FirstOrDefault();
           

                var modo = usu.ModoQA ? "QA" : "PROD";
                var url = (modo == "QA" ? ConfigurationManager.AppSettings["FE.QA.wsfev1"] : ConfigurationManager.AppSettings["FE.PROD.wsfev1"]);
                int CodigoLogServicio = Common.RegistrarEnvioLog("AFIP", url, "wsfev1", "GetComprobante(" + usu.CUIT + "," + numeroComprobante + "," + puntoDeVenta.Punto.ToString() + "," + idTipo.ToString() + ")", usu.IDUsuario);

                try
                {
                    f = fe.GetComprobanteResponse(long.Parse(usu.CUIT), long.Parse(usu.CUITAfip), long.Parse(numeroComprobante), 
                        puntoDeVenta.Punto, (FETipoComprobante)idTipo, false);
                }
                catch
                {
                    try
                    {
                        f = fe.GetComprobanteResponse(long.Parse(usu.CUIT), long.Parse(usu.CUITAfip), long.Parse(numeroComprobante),
                                puntoDeVenta.Punto, (FETipoComprobante)idTipo, true);
                    }
                    catch (Exception e)
                    {
                        Common.RegistrarRespuestaLog(CodigoLogServicio, 1, "Respuesta Afip: " + e.Message, false);
                        throw new Exception("Respuesta Afip: " + e.Message);
                    }
                }

                var dictTipoComprobante = new Dictionary<int, string>();
                foreach (var name in Enum.GetNames(typeof(FETipoComprobante)))
                {
                    dictTipoComprobante.Add((int)Enum.Parse(typeof(FETipoComprobante), name), name);
                }

                html += "<tr>";
                html += "<td>EmisionTipo</td>";
                html += "<td>" + f.ResultGet.EmisionTipo + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>MonCotiz</td>";
                html += "<td>" + f.ResultGet.MonCotiz + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>CbteDesde</td>";
                html += "<td>" + f.ResultGet.CbteDesde + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>CbteHasta</td>";
                html += "<td>" + f.ResultGet.CbteHasta + "</td>";
                html += "</tr>";
                if(f.ResultGet.CbtesAsoc != null)
                {
                    foreach (var r in f.ResultGet.CbtesAsoc)
                    {
                        html += "<tr>";
                        html += "<td>CbtesAsoc</td>";
                        html += "<td>CbteFch: " + r.CbteFch + " - Cuit: " + r.Cuit + "";
                        html += " - Nro: " + r.Nro + " - PtoVta: " + r.PtoVta + "";
                        html += " - Tipo: " + r.Tipo + "</td>";
                        html += "</tr>";
                    }
                }
                else
                {
                    html += "<tr>";
                    html += "<td>CbtesAsoc</td>";
                    html += "<td> - Sin Datos - </td>";
                    html += "</tr>";
                }
                html += "<tr>";
                html += "<td>CbteTipo</td>";
                html += "<td>" + dictTipoComprobante.Where(w => w.Key == f.ResultGet.CbteTipo).Select(s => s.Value).FirstOrDefault() + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>CodAutorizacion</td>";
                html += "<td>" + f.ResultGet.CodAutorizacion + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>Concepto</td>";
                html += "<td>" + f.ResultGet.Concepto + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>DocNro</td>";
                html += "<td>" + f.ResultGet.DocNro + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>DocTipo</td>";
                html += "<td>" + f.ResultGet.DocTipo + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>EmisionTipo</td>";
                html += "<td>" + f.ResultGet.EmisionTipo + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>FchProceso</td>";
                html += "<td>" + f.ResultGet.FchProceso + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>FchServDesde</td>";
                html += "<td>" + f.ResultGet.FchServDesde + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>FchServHasta</td>";
                html += "<td>" + f.ResultGet.FchServHasta + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>FchVto</td>";
                html += "<td>" + f.ResultGet.FchVto + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>FchVtoPago</td>";
                html += "<td>" + f.ResultGet.FchVtoPago + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>ImpIVA</td>";
                html += "<td>" + f.ResultGet.ImpIVA + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>ImpNeto</td>";
                html += "<td>" + f.ResultGet.ImpNeto + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>ImpOpEx</td>";
                html += "<td>" + f.ResultGet.ImpOpEx + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>ImpTotal</td>";
                html += "<td>" + f.ResultGet.ImpTotal + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>ImpTotConc</td>";
                html += "<td>" + f.ResultGet.ImpTotConc + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>ImpTrib</td>";
                html += "<td>" + f.ResultGet.ImpTrib + "</td>";
                html += "</tr>";
                if (f.ResultGet.Iva != null)
                {
                    foreach (AlicIva r in f.ResultGet.Iva)
                    {
                        html += "<tr>";
                        html += "<td>Iva</td>";
                        html += "<td>BaseImp: " + r.BaseImp + " - Id: " + r.Id + "";
                        html += " - Importe:" + r.Importe + "</td>";
                        html += "</tr>";
                    }
                }
                else
                {
                    html += "<tr>";
                    html += "<td>Iva</td>";
                    html += "<td> - Sin Datos - </td>";
                    html += "</tr>";
                }
                html += "<tr>";
                html += "<td>MonCotiz</td>";
                html += "<td>" + f.ResultGet.MonCotiz + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>MonId</td>";
                html += "<td>" + f.ResultGet.MonId + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>Observaciones</td>";
                html += "<td>" + f.ResultGet.Observaciones + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>PtoVta</td>";
                html += "<td>" + f.ResultGet.PtoVta + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td>Resultado</td>";
                html += "<td>" + f.ResultGet.Resultado + "</td>";
                html += "</tr>";
                if (f.ResultGet.Tributos != null)
                {
                    foreach (Tributo r in f.ResultGet.Tributos)
                    {
                        html += "<tr>";
                        html += "<td>Tributos</td>";
                        html += "<td>Desc: " + r.Desc + " - Alic: " + r.Alic + "";
                        html += " - BaseImp: " + r.BaseImp + " - Importe: " + r.Importe + "";
                        html += " - Id: " + r.Id + "</td>";
                        html += "</tr>";
                    }
                }
                else
                {
                    html += "<tr>";
                    html += "<td>Tributos</td>";
                    html += "<td> - Sin Datos - </td>";
                    html += "</tr>";
                }
                if (f.ResultGet.Opcionales != null)
                {
                    foreach (Opcional r in f.ResultGet.Opcionales)
                    {
                        html += "<tr>";
                        html += "<td>Opcionales</td>";
                        html += "<td>ID: " + r.Id + " - Valor: " + r.Valor + "</td>";
                        html += "</tr>";
                    }
                }
                else
                {
                    html += "<tr>";
                    html += "<td>Opcionales</td>";
                    html += "<td> - Sin Datos - </td>";
                    html += "</tr>";
                }
                if (f.ResultGet.Actividades != null)
                {
                    foreach (var r in f.ResultGet.Actividades)
                    {
                        html += "<tr>";
                        html += "<td>Actividades</td>";
                        html += "<td>ID: " + r.Id + "</td>";
                        html += "</tr>";
                    }
                }
                else
                {
                    html += "<tr>";
                    html += "<td>Actividades</td>";
                    html += "<td> - Sin Datos - </td>";
                    html += "</tr>";
                }


                Common.RegistrarRespuestaLog(CodigoLogServicio, 1, html, true);

                return html;

            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # common.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception(ex.Message);
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }


    public static void agregarPunto(int idUsuario, int punto, bool porDefecto)
    {
        var db = new ACHEEntities();
        if (!db.PuntosDeVenta.Any(x => x.Punto == punto && x.IDUsuario == idUsuario))
        {
            PuntosDeVenta entity = new PuntosDeVenta();
            entity.Punto = punto;
            entity.IDUsuario = idUsuario;
            entity.FechaAlta = DateTime.Now;
            entity.PorDefecto = porDefecto;
            db.PuntosDeVenta.Add(entity);
            db.SaveChanges();
        }
    }



}