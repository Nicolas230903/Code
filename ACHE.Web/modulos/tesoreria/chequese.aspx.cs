using ACHE.Model;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.IO;

public partial class modulos_Tesoreria_chequese : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ".";//"."
    public const string SeparadorDeDecimales = ",";//","

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionCheques)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }
            litPath.Text = "Alta";
            Idusuario.Value = CurrentUser.IDUsuario.ToString();
            cargarBancos();
            cargaPersonas();
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

    private void cargarBancos()
    {
        using (var dbContext = new ACHEEntities())
        {
            var lista = dbContext.BancosBase.ToList();
            foreach (var item in lista)
                ddlBancos.Items.Add(new ListItem(item.Nombre, item.IDBancoBase.ToString()));
        }
    }

    private void cargaPersonas()
    {
        using (var dbContext = new ACHEEntities())
        {
            var lista = dbContext.Personas.Where(w => w.IDUsuario == CurrentUser.IDUsuario).OrderBy(o => o.RazonSocial).ToList();
            foreach (var item in lista)
                ddlPersona.Items.Add(new ListItem(item.RazonSocial, item.IDPersona.ToString()));
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Cheques.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDCheque == id).FirstOrDefault();
            if (entity != null)
            {
                ddlBancos.SelectedValue = Convert.ToInt32(entity.IDBanco).ToString();
                txtNumero.Text = entity.Numero.ToString();
                txtImporte.Text = entity.Importe.ToString();
                txtFechaEmision.Text = entity.FechaEmision.ToString("dd/MM/yyyy");
                txtFechaCobrar.Text = (entity.FechaCobro != null) ? Convert.ToDateTime(entity.FechaCobro).ToString("dd/MM/yyyy") : "";
                txtFechaVencimiento.Text = (entity.FechaVencimiento != null) ? Convert.ToDateTime(entity.FechaVencimiento).ToString("dd/MM/yyyy") : "";
                ddlEstado.SelectedValue = entity.Estado;
                txtEmisor.Text = entity.Emisor;
                txtObservaciones.Text = entity.Observaciones;
                txtCUIT.Text = entity.CUIT;
                if(entity.IdPersona != null)
                    ddlPersona.SelectedValue = Convert.ToInt32(entity.IdPersona).ToString();

                if (!string.IsNullOrWhiteSpace(entity.Foto))
                {
                    imgFoto.Src = "/files/explorer/" + CurrentUser.IDUsuario.ToString() + "/Cheques/" + entity.Foto;
                    hdnTieneFoto.Value = "1";
                }
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static int guardar(int id, int idBanco, string numero, string importe, string fechaEmision, 
                            string fechaCobro, string fechaVencimiento, string estado, string emisor, string observaciones, bool esPropio, 
                            int idChequePersona, string cuit, bool esPropioEmpresa)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];

            using (var dbContext = new ACHEEntities())
            {

                if (dbContext.Cheques.Any(x => x.IDUsuario == usu.IDUsuario && x.IDBanco == idBanco && x.Numero == numero && x.IDCheque != id))
                    throw new Exception("El nro de cheque ingresado ya se encuentra registrado.");

                Cheques entity;
                if (id > 0)
                    entity = dbContext.Cheques.Where(x => x.IDCheque == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    Cheques chequesRepetidos = dbContext.Cheques.Where(w => w.IDUsuario == usu.IDUsuario && w.Emisor.Equals(emisor) && w.Numero.Equals(numero)).FirstOrDefault();
                    if(chequesRepetidos != null)
                        throw new Exception("El nro de cheque ingresado y el emisor ya se encuentra registrado.");

                    entity = new Cheques();
                    entity.FechaAlta = DateTime.Now;
                    entity.IDUsuario = usu.IDUsuario;
                }

                entity.IDBanco = idBanco;
                entity.Numero = numero;
                entity.Importe = (importe != string.Empty) ? decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.FechaEmision = Convert.ToDateTime(fechaEmision);
                entity.FechaCobro = Convert.ToDateTime(fechaCobro);

                entity.FechaVencimiento = Convert.ToDateTime(entity.FechaCobro).AddDays(30);
                entity.Estado = estado;
                entity.Emisor = emisor;
                entity.Observaciones = observaciones;
                entity.EsPropio = esPropio;
                entity.EsPropioEmpresa = esPropioEmpresa;
                entity.CUIT = cuit.Trim();
                if (!entity.EsPropio && !entity.EsPropioEmpresa)
                {
                    entity.IdPersona = idChequePersona;
                }

                if (id > 0)
                {
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.Cheques.Add(entity);
                    dbContext.SaveChanges();
                }
                return entity.IDCheque;
            }
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }

    [WebMethod(true)]
    public static void eliminarFoto(int idCheque)
    {
        var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.Cheques.Where(x => x.IDCheque == idCheque && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
            if (entity != null)
            {
                string Serverpath = HttpContext.Current.Server.MapPath("~/files/explorer/" + usu.IDUsuario + "/Cheques/" + entity.Foto);

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