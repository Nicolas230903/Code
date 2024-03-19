using ACHE.Extensions;
using ACHE.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class comunicacionAfip : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                var TieneDatos = dbContext.ComunicacionesAFIP.Any(x => x.IdUsuario == CurrentUser.IDUsuario);
                if (TieneDatos)
                {
                    divConDatos.Visible = true;
                }
                else
                {
                    divConDatos.Visible = false;
                }
            }
        }
    }

    [System.Web.Script.Services.ScriptMethod(UseHttpGet = true)]
    [WebMethod(true)]
    public static string obtenerItems(int leidos)
    {
        var html = string.Empty;
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {
                var list = dbContext.ComunicacionesAFIP.Where(x => x.IdUsuario == usu.IDUsuario && x.Visto == leidos).ToList();
                if (list.Any())
                {
                    int index = 1;
                    foreach (var detalle in list)
                    {
                        html += "<tr>";                        
                        html += "<td>" + index.ToString() + "</td>";
                        html += "<td>" + detalle.IdComunicacion.ToString() + "</td>";
                        html += "<td>" + detalle.FechaPublicacion + "</td>";
                        html += "<td>" + detalle.fechaVencimiento + " </td>";
                        html += "<td>" + detalle.sistemaPublicadorDesc + " </td>";
                        html += "<td>" + detalle.EstadoDesc + " </td>";
                        html += "<td>" + detalle.Asunto + " </td>";
                        html += "<td>" + detalle.PrioridadDesc + " </td>";
                        if (detalle.TieneAdjunto == 1)
                        {
                            List<ComunicacionesAFIPAdjuntos> lc = dbContext.ComunicacionesAFIPAdjuntos.Where(x => x.IdComunicacion == detalle.IdComunicacion).ToList();
                            if (lc != null)
                            {
                                html += "<td>";
                                foreach (ComunicacionesAFIPAdjuntos c in lc)
                                {
                                    html += "<a class='fa fa-file-pdf-o fa-2x text-center' href='#' onclick='descargarAdjunto(" + c.IdComunicacionAFIPAdjunto.ToString() + ");'></a>";
                                }
                                html += "</td>";
                            }
                        }
                        else
                            html += "<td></td>";
                        if (leidos == 0)                        
                            html += "<td><input type='checkbox' name='comunicacionesSeleccionados' class='checkbox chkTodos' value='" + detalle.IdComunicacion.ToString() + "' /></td>";
                        html += "</tr>";

                        index++;
                    }
                }
            }
            if (html == "")
                html = "<tr><td colspan='4' style='text-align:center'>No tienes comunicaciones.</td></tr>";

        }
        return html;
    }


    [WebMethod(true)]
    public static void marcarComoLeidas(string[] id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (id != null)
                {
                    var dbContext = new ACHEEntities();
                    foreach (string item in id)
                    {
                        if (item != null)
                        {
                            long idComunicacion = Convert.ToInt64(item.ToString());
                            ComunicacionesAFIP c = dbContext.ComunicacionesAFIP.Where(x => x.IdComunicacion == idComunicacion).FirstOrDefault();
                            if (c != null)
                            {
                                c.Visto = 1;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # comunicacionAfip.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                return;
            }

        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static ArchivoAdjunto descargarAdjunto(string id)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            try
            {
                if (id != null)
                {
                    var dbContext = new ACHEEntities();
                    int idComunicacionAFIPAdjunto = Convert.ToInt32(id);
                    ComunicacionesAFIPAdjuntos c = dbContext.ComunicacionesAFIPAdjuntos.Where(x => x.IdComunicacionAFIPAdjunto == idComunicacionAFIPAdjunto).FirstOrDefault();
                    if (c != null)
                    {
                        ArchivoAdjunto d = new ArchivoAdjunto();
                        d.NombreArchivo = c.NombreArchivo;
                        d.Contenido = c.Contenido;
                        return d;
                    }
                    else
                        throw new Exception("Error al descargar el archivo");
                }
                else
                    throw new Exception("Error al descargar el archivo");
            }
            catch (Exception ex)
            {
                BasicLog.AppendToFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BasicLogError"]), ex.Message, " # comunicacionAfip.aspx # - " + DateTime.Now.ToString() + " - " + ex.InnerException);
                throw new Exception("Error al descargar el archivo");
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}

public class ArchivoAdjunto
{
    public string NombreArchivo { get; set; }
    public byte[] Contenido { get; set; }
}
