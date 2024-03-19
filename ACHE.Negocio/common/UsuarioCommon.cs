using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Negocio.Facturacion;
using System.IO;
using ACHE.Extensions;
using System.Collections.Specialized;
using ACHE.FacturaElectronica;
using System.Text.RegularExpressions;

namespace ACHE.Negocio.Common
{
    public static class UsuarioCommon
    {
        public static List<Combo2ViewModel> ObtenerPuntosDeVenta(int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listaPorDefecto = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == idUsuario && !x.FechaBaja.HasValue && x.PorDefecto == true).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.IDPuntoVenta,
                            Nombre = x.Punto.ToString("#0000")
                        }).OrderBy(x => x.Nombre).ToList();

                    var listaDemasElementos = dbContext.PuntosDeVenta.Where(x => x.IDUsuario == idUsuario && !x.FechaBaja.HasValue && x.PorDefecto != true).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.IDPuntoVenta,
                            Nombre = x.Punto.ToString("#0000")
                        }).OrderBy(x => x.Nombre).ToList();

                    return listaPorDefecto.Union(listaDemasElementos).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Combo2ViewModel> ObtenerActividades(int idUsuario)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var listaPorDefecto = dbContext.Actividad.Where(x => x.IdUsuario == idUsuario && !x.FechaBaja.HasValue && x.PorDefecto == true).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.IdActividad,
                            Nombre = x.Codigo
                        }).OrderBy(x => x.Nombre).ToList();

                    var listaDemasElementos = dbContext.Actividad.Where(x => x.IdUsuario == idUsuario && !x.FechaBaja.HasValue && x.PorDefecto != true).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.IdActividad,
                            Nombre = x.Codigo
                        }).OrderBy(x => x.Nombre).ToList();

                    return listaPorDefecto.Union(listaDemasElementos).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Combo2ViewModel> ObtenerTipoComprobanteAfip(string condicionIVA)
        {
            try
            {

                var dict = new Dictionary<int, string>();
                foreach (var name in Enum.GetNames(typeof(FETipoComprobante)))
                {
                    dict.Add((int)Enum.Parse(typeof(FETipoComprobante), name), name);
                }

                if (condicionIVA.Equals("RI"))
                {
                    int[] listaComprobantes = { 1, 2, 3, 4, 6, 7, 8, 9, 201, 202, 203, 206, 207, 208 };
                    return dict.Where(w => listaComprobantes.Contains(w.Key)).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.Key,
                            Nombre = x.Value
                        }).OrderBy(x => x.Nombre).ToList();
                }
                else
                {
                    int[] listaComprobantes = { 11, 12, 13, 15, 211, 212, 213 };
                    return dict.Where(w => listaComprobantes.Contains(w.Key)).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.Key,
                            Nombre = x.Value
                        }).OrderBy(x => x.Nombre).ToList();
                }
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<EmpresasViewModel> ListaEmpresasDisponibles(WebUser usu, ACHEEntities dbContext)
        {
            List<EmpresasViewModel> list = new List<EmpresasViewModel>();
            if (usu.IDUsuarioAdicional != 0)
            {
                //Carga los datos si es una usuario Adicional
                var results = dbContext.UsuariosEmpresa.Include("Usuarios").Where(x => x.IDUsuarioAdicional == usu.IDUsuarioAdicional).ToList();
                Usuarios usuarioPadre;
                if (string.IsNullOrWhiteSpace(usu.IDUsuarioPadre.ToString()))
                {
                    usuarioPadre = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario && x.IDUsuarioPadre == null).FirstOrDefault();
                }
                else
                {
                    usuarioPadre = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuarioPadre).FirstOrDefault();
                }
                list = esUsuAdicional(list, results, usuarioPadre);
            }
            else
            {
                IList<Usuarios> results = null;
                if (string.IsNullOrWhiteSpace(usu.IDUsuarioPadre.ToString()))
                {
                    //Soy un usuario Padre y busco todas mis empresas
                    results = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == usu.IDUsuario).ToList();
                    list = esUsuarioPadre(list, results, usu);
                }
                else
                {
                    //Soy un usuario Padre pero estoy logiado con otra de mis  empresas y busco todas mis empresas
                    results = dbContext.Usuarios.Where(x => x.IDUsuarioPadre == usu.IDUsuarioPadre).ToList();
                    var usuarioPadre = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuarioPadre).FirstOrDefault();
                    list = esCambioSesion(list, results, usuarioPadre);
                }
            }
            return list;
        }

        private static List<EmpresasViewModel> esCambioSesion(List<EmpresasViewModel> list, IList<Usuarios> results, Usuarios usuarioPadre)
        {
            list = results.Select(x => new EmpresasViewModel()
            {
                ID = x.IDUsuario,
                RazonSocial = x.RazonSocial.ToUpper(),
                CUIT = x.CUIT,
                CondicionIva = UsuarioCommon.GetCondicionIvaDesc(x.CondicionIva),
                Email = x.Email.ToLower(),
                Domicilio = x.Domicilio,
                Ciudad = x.Ciudades.Nombre,
                Provincia = x.Provincias.Nombre,
                Logo = (string.IsNullOrEmpty(x.Logo)) ? "/files/usuarios/" + "no-photo.png" : "/files/usuarios/" + usuarioPadre.Logo
            }).ToList();

            var empresa = new EmpresasViewModel();
            empresa.ID = usuarioPadre.IDUsuario;
            empresa.RazonSocial = usuarioPadre.RazonSocial.ToUpper();
            empresa.CUIT = usuarioPadre.CUIT;
            empresa.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usuarioPadre.CondicionIva);
            empresa.Email = usuarioPadre.Email.ToLower();
            empresa.Domicilio = usuarioPadre.Domicilio;
            empresa.Ciudad = usuarioPadre.Ciudades.Nombre;
            empresa.Provincia = usuarioPadre.Provincias.Nombre;
            empresa.Logo = (string.IsNullOrEmpty(usuarioPadre.Logo)) ? "/files/usuarios/" + "no-photo.png" : "/files/usuarios/" + usuarioPadre.Logo;
            list.Add(empresa);

            return list;
        }

        private static List<EmpresasViewModel> esUsuarioPadre(List<EmpresasViewModel> list, IList<Usuarios> results, WebUser usu)
        {
            list = results.Select(x => new EmpresasViewModel()
            {
                ID = x.IDUsuario,
                RazonSocial = x.RazonSocial.ToUpper(),
                CUIT = x.CUIT,
                CondicionIva = UsuarioCommon.GetCondicionIvaDesc(x.CondicionIva),
                Email = x.Email.ToLower(),
                Domicilio = x.Domicilio,
                Ciudad = x.Ciudades.Nombre,
                Provincia = x.Provincias.Nombre,
                Logo = (string.IsNullOrEmpty(x.Logo)) ? "/files/usuarios/" + "no-photo.png" : "/files/usuarios/" + x.Logo
            }).ToList();


            //Agrego la empresa actual
            var empresa = new EmpresasViewModel();
            empresa.ID = usu.IDUsuario;
            empresa.RazonSocial = usu.RazonSocial.ToUpper();
            empresa.CUIT = usu.CUIT;
            empresa.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usu.CondicionIVA);
            empresa.Email = usu.Email.ToLower();
            empresa.Domicilio = usu.Domicilio.ToLower();
            empresa.Ciudad = usu.Ciudad.ToLower();
            empresa.Provincia = usu.Provincia.ToLower();
            empresa.Logo = (string.IsNullOrEmpty(usu.Logo)) ? "/files/usuarios/" + "no-photo.png" : "/files/usuarios/" + usu.Logo;
            list.Add(empresa);

            return list;
        }

        private static List<EmpresasViewModel> esUsuAdicional(List<EmpresasViewModel> list, List<UsuariosEmpresa> results, Usuarios usuarioPadre)
        {
            list = results.Select(x => new EmpresasViewModel()
            {
                ID = x.IDUsuario,
                RazonSocial = x.Usuarios.RazonSocial.ToUpper(),
                CUIT = x.Usuarios.CUIT,
                CondicionIva = UsuarioCommon.GetCondicionIvaDesc(x.Usuarios.CondicionIva),
                Email = x.Usuarios.Email.ToLower(),
                Domicilio = x.Usuarios.Domicilio,
                Ciudad = x.Usuarios.Ciudades.Nombre,
                Provincia = x.Usuarios.Provincias.Nombre,
                Logo = (string.IsNullOrWhiteSpace(x.Usuarios.Logo)) ? "/files/usuarios/" + "no-photo.png" : "/files/usuarios/" + usuarioPadre.Logo
            }).ToList();

            if (usuarioPadre != null)
            {
                var empresa = new EmpresasViewModel();
                empresa.ID = usuarioPadre.IDUsuario;
                empresa.RazonSocial = usuarioPadre.RazonSocial.ToUpper();
                empresa.CUIT = usuarioPadre.CUIT;
                empresa.CondicionIva = UsuarioCommon.GetCondicionIvaDesc(usuarioPadre.CondicionIva);
                empresa.Email = usuarioPadre.Email.ToLower();
                empresa.Domicilio = usuarioPadre.Domicilio.ToLower();
                empresa.Ciudad = usuarioPadre.Ciudades.Nombre.ToLower();
                empresa.Provincia = usuarioPadre.Provincias.Nombre.ToLower();
                empresa.Logo = (string.IsNullOrWhiteSpace(usuarioPadre.Logo)) ? "/files/usuarios/" + "no-photo.png" : "/files/usuarios/" + usuarioPadre.Logo;
                list.Add(empresa);
            }

            return list;
        }

        public static void CreateFolders(int idUsuario, string basePath)
        {
            Directory.CreateDirectory(basePath + "//" + idUsuario);
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Contactos/");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Empleados/");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Compras/");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Comprobantes/");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Productos-Servicios");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Cheques");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Caja");
        }

        public static void CreateFoldersRI(int idUsuario, string basePath)
        {
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Balances");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Cargas-Sociales");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos/Bienes-Acciones-Participaciones");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos/IB");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos/Iva");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos/Ganancias");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos/Monotributo");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Impuestos/Sicore");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Societarios");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Societarios/Estatutos");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Societarios/Socios");
            Directory.CreateDirectory(basePath + "//" + idUsuario + "/Societarios/Apoderados");
        }

        public static string GetCondicionIvaDesc(string tipo)
        {
            if (tipo == "RI")
                return "Responsable Inscripto";
            else if (tipo == "MO")
                return "Monotributo";
            else if (tipo == "EX")
                return "Exento";
            else if (tipo == "NI")
                return "Responsable NO inscripto";
            else
                return "Consumidor final";
        }

        public static bool TienePersonas(int idUsuario)
        {
            try
            {
                bool tiene = false;

                using (var dbContext = new ACHEEntities())
                {
                    tiene = dbContext.Personas.Any(x => x.IDUsuario == idUsuario);
                }

                return tiene;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static Usuarios ObtenerUsuarioPorNroDoc(string nroCUIT)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                    return dbContext.Usuarios.Include("Provincias").Include("Ciudades").Where(x => x.CUIT == nroCUIT).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void CambiarPassword(string passwordActual, string PasswordNuevo, string Passwordverificado, WebUser usu)
        {
            try
            {
                if (PasswordNuevo != Passwordverificado)
                    throw new CustomException("El password ingresado no coinciden entre si");

                using (var dbContext = new ACHEEntities())
                {
                    bool esAdicional = usu.IDUsuarioAdicional != 0;
                    if (!esAdicional)
                    {
                        var entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                        if (entity.Pwd != passwordActual)
                            throw new CustomException("La contraseña actual ingresada es incorrecta");
                        else
                            entity.Pwd = PasswordNuevo;

                        dbContext.SaveChanges();
                        avisarCambioDePwd(entity.Email, entity.RazonSocial);
                    }
                    else
                    {
                        var entity = dbContext.UsuariosAdicionales.Where(x => x.IDUsuarioAdicional == usu.IDUsuarioAdicional).First();
                        if (entity.Pwd != passwordActual)
                            throw new CustomException("La contraseña actual ingresada es incorrecta");
                        else
                            entity.Pwd = PasswordNuevo;

                        dbContext.SaveChanges();
                        avisarCambioDePwd(entity.Email, "usuario/a");
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
        private static void avisarCambioDePwd(string email, string nombre)
        {
            try
            {
                if (!email.IsValidEmailAddress())
                    throw new CustomException("Email incorrecto.");

                ListDictionary replacements = new ListDictionary();
                replacements.Add("<USUARIO>", nombre);

                bool send = EmailHelper.SendMessage(EmailTemplate.ModificacionPwd, replacements, email, "AXAN: Modificación de contraseña");
                if (!send)
                    throw new Exception("El email avisando el cambio de contraseña no pudo ser enviado");
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

        public static void GuardarConfiguracion(string razonSocial, string condicionIva, 
            string cuit, string iibb, string fechaInicio, string personeria, string emailAlertas, 
            string telefono, string celular, string contacto, string idProvincia, string idCiudad, 
            string domicilio, string pisoDepto, string cp, bool esAgentePersepcionIVA, 
            bool esAgentePersepcionIIBB, bool esAgenteRetencionGanancia, bool esAgenteRetencion, 
            bool exentoIIBB, string fechaCierreContable, string idJurisdiccion, 
            string cbu, string textoFinalFactura,
            WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(idCiudad))
                        throw new CustomException("Debe ingresar la ciudad del Domicilio Fiscal.");
                    if (dbContext.Usuarios.Any(x => x.CUIT == cuit && x.IDUsuario != usu.IDUsuario))
                        throw new CustomException("El CUIT ingresado ya se encuentra registrado.");
                    if (!cuit.IsValidCUIT())
                        throw new CustomException("El CUIT es invalido.");
                    else
                    {
                        entity.RazonSocial = razonSocial;
                        if (!string.IsNullOrWhiteSpace(fechaInicio))
                            entity.FechaInicioActividades = DateTime.Parse(fechaInicio);
                        else
                            entity.FechaInicioActividades = null;

                        entity.CondicionIva = condicionIva;
                        entity.CUIT = cuit;
                        entity.IIBB = iibb;
                        entity.Personeria = personeria;
                        entity.EmailAlertas = emailAlertas;
                        entity.Telefono = telefono;
                        entity.Celular = celular;
                        entity.Contacto = contacto;
                        entity.ExentoIIBB = exentoIIBB;
                        //Domicilio
                        entity.IDProvincia = Convert.ToInt32(idProvincia);
                        entity.IDCiudad = Convert.ToInt32(idCiudad);
                        entity.Domicilio = domicilio;
                        entity.PisoDepto = pisoDepto;
                        entity.CodigoPostal = cp;

                        entity.EsAgentePercepcionIVA = esAgentePersepcionIVA;
                        entity.EsAgentePercepcionIIBB = esAgentePersepcionIIBB;
                        entity.EsAgenteRetencionGanancia = esAgenteRetencionGanancia;
                        entity.EsAgenteRetencion = esAgenteRetencion;
                        entity.IDJurisdiccion = idJurisdiccion.Trim();

                        if (!string.IsNullOrWhiteSpace(fechaCierreContable))
                            entity.FechaCierreContable = Convert.ToDateTime(fechaCierreContable);
                        else
                            entity.FechaCierreContable = null;

                        //if (entity.CondicionIva == "MO" && !entity.SetupRealizado)
                        if (entity.CondicionIva == "MO" && !entity.SetupRealizado)
                            entity.UsaPrecioFinalConIVA = true;

                        if (!string.IsNullOrEmpty(cbu))
                        {
                            if(!ValidarCBU(cbu))
                                throw new CustomException("El CBU no es válido.");

                            entity.CBU = cbu;
                        }

                        if (!string.IsNullOrEmpty(textoFinalFactura))
                        {
                            entity.TextoFinalFactura = textoFinalFactura;
                        }

                        dbContext.SaveChanges();
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

        static bool ValidarCBU(string cbu)
        {
            // Expresión regular para validar el formato del CBU
            string patronCBU = @"^\d{22}$";

            // Verificar si el CBU coincide con el formato esperado
            return Regex.IsMatch(cbu, patronCBU);
        }

        public static UsuariosAPIViewModel ObtenerConfiguracion(WebUser usu)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    var entity = dbContext.Usuarios.Where(x => x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                    UsuariosAPIViewModel usuv = new UsuariosAPIViewModel();

                    usuv.RazonSocial = entity.RazonSocial;
                    usuv.CondicionIva = entity.CondicionIva;
                    usuv.CUIT = entity.CUIT;
                    usuv.Email = entity.Email;
                    usuv.EmailAlertas = entity.EmailAlertas;
                    usuv.FechaInicioActividades = entity.FechaInicioActividades;
                    usuv.Personeria = entity.Personeria;
                    usuv.ExentoIIBB = entity.ExentoIIBB;
                    usuv.IIBB = entity.IIBB;
                    usuv.Contacto = entity.Contacto;
                    usuv.Celular = entity.Celular;

                    usuv.IDProvincia = entity.IDProvincia;
                    usuv.IDCiudad = entity.IDCiudad;
                    usuv.Domicilio = entity.Domicilio;
                    usuv.PisoDepto = entity.PisoDepto;
                    usuv.CodigoPostal = entity.CodigoPostal;
                    usuv.Telefono = entity.Telefono;

                    usuv.EsAgentePercepcionIVA = entity.EsAgentePercepcionIVA;
                    usuv.EsAgentePercepcionIIBB = entity.EsAgentePercepcionIIBB;
                    usuv.EsAgenteRetencion = entity.EsAgenteRetencion;                    
                    usuv.IDJurisdiccion = entity.IDJurisdiccion;

                    return usuv;
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

        public static void GuardarLoginUsuarios(string email, int? idUsuario, int? idUsuarioAdicional, string Observaciones)
        {
            using (ACHEEntities entities = new ACHEEntities())
            {
                LoginUsuarios usuarios = new LoginUsuarios();
                usuarios.EmailLogin=email;
                usuarios.Observacion=Observaciones;
                usuarios.FechaLogin = DateTime.Now.Date;
                if (idUsuario != 0)
                {
                    usuarios.IDUsuario=idUsuario;
                }
                else
                {
                    usuarios.IDUsuario=null;
                }
                if (idUsuarioAdicional != 0)
                {
                    usuarios.IDUsuarioAdicional = idUsuarioAdicional;
                }
                else
                {
                    usuarios.IDUsuarioAdicional = null;
                }
                entities.LoginUsuarios.Add(usuarios);
                entities.SaveChanges();
            }
        }
    }
}