using ACHE.Model;
using ACHE.Negocio.Contabilidad;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

public partial class modulos_Tesoreria_gastosBancariose : BasePage
{
    public const string formatoFecha = "MM/dd/yyyy";//"dd/MM/yyyy"
    public const string SeparadorDeMiles = ",";//"."
    public const string SeparadorDeDecimales = ".";//","

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            using (var dbContext = new ACHEEntities())
            {
                AccesoFormularioUsuario afu = dbContext.AccesoFormularioUsuario.Where(w => w.IdUsuario == CurrentUser.IDUsuario && w.IdUsuarioAdicional == CurrentUser.IDUsuarioAdicional).FirstOrDefault();

                if (afu != null)
                    if (!afu.AdministracionGastos)
                        Response.Redirect("~/Modulos/Seguridad/AccesoDenegado.aspx");

            }

            litPath.Text = "Alta";
            litTotal.Text = "0.00";

            cargarBancos();

            if (!String.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                hdnID.Value = Request.QueryString["ID"];
                if (hdnID.Value != "0")
                {
                    cargarEntidad(int.Parse(hdnID.Value));
                    litPath.Text = "Edición";
                }
            }
            else
                txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }
    }

    private void cargarBancos()
    {
        using (var dbContext = new ACHEEntities())
        {
            foreach (var item in dbContext.Bancos.Where(x => x.IDUsuario == CurrentUser.IDUsuario).ToList())
                ddlBanco.Items.Add(new ListItem(item.BancosBase.Nombre + " - " + item.NroCuenta, item.IDBanco.ToString()));
        }
    }

    private void cargarEntidad(int id)
    {
        using (var dbContext = new ACHEEntities())
        {
            var entity = dbContext.GastosBancarios.Where(x => x.IDUsuario == CurrentUser.IDUsuario && x.IDGastosBancarios == id).FirstOrDefault();
            if (entity != null)
            {
                ddlBanco.SelectedValue = entity.IDBanco.ToString();
                txtFecha.Text = entity.Fecha.ToString("dd/MM/yyyy");

                txtImporte.Text = entity.Importe.ToString().Replace(",", ".");
                txtIVA.Text = entity.IVA.ToString().Replace(",", ".");
                txtDebito.Text = entity.Debito.ToString().Replace(",", ".");
                txtCredito.Text = entity.Credito.ToString().Replace(",", ".");
                txtIIBB.Text = entity.IIBB.ToString().Replace(",", ".");
                txtOtros.Text = entity.Otros.ToString().Replace(",", ".");
                txtImporte21.Text = entity.Importe21.ToString().Replace(",", ".");
                txtCreditoComputable.Text = entity.CreditoComputable.ToString().Replace(",", ".");

                txtPercepcionIVA.Text = entity.PercepcionIVA.ToString().Replace(",", ".");
                txtSIRCREB.Text = entity.SIRCREB.ToString().Replace(",", ".");
                txtImporte10.Text = entity.Importe10.ToString().Replace(",", ".");
                


                txtConcepto.Text = entity.Concepto;

                litTotal.Text = (entity.Importe + entity.IVA + entity.Debito + entity.Credito
                                + entity.IIBB + entity.Otros + entity.Importe21 + entity.CreditoComputable
                                + entity.PercepcionIVA + entity.SIRCREB + entity.Importe10).ToString().Replace(",", ".");
            }
            else
                Response.Redirect("/error.aspx");
        }
    }

    [WebMethod(true)]
    public static void guardar(int id, int IdBanco, string fecha, string importe, string iva, string debito,
        string credito, string IIBB, string otros, string Importe21, string creditoComputable, string concepto,
        string percepcionIVA, string SIRCREB, string Importe10)
    {
        if (HttpContext.Current.Session["CurrentUser"] != null)
        {
            var usu = (WebUser)HttpContext.Current.Session["CurrentUser"];
            var idGastosBancarios = 0;
            using (var dbContext = new ACHEEntities())
            {

                GastosBancarios entity;
                if (id > 0)
                    entity = dbContext.GastosBancarios.Where(x => x.IDGastosBancarios == id && x.IDUsuario == usu.IDUsuario).FirstOrDefault();
                else
                {
                    entity = new GastosBancarios();
                    entity.FechaAlta = DateTime.Now;
                    entity.IDUsuario = usu.IDUsuario;
                }

                entity.IDBanco = IdBanco;
                entity.Fecha = Convert.ToDateTime(fecha);

                entity.Importe = (importe != string.Empty) ? decimal.Parse(importe.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.IVA = (iva != string.Empty) ? decimal.Parse(iva.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.Debito = (debito != string.Empty) ? decimal.Parse(debito.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.Credito = (credito != string.Empty) ? decimal.Parse(credito.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.IIBB = (IIBB != string.Empty) ? decimal.Parse(IIBB.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.Otros = (otros != string.Empty) ? decimal.Parse(otros.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.Importe21 = (Importe21 != string.Empty) ? decimal.Parse(Importe21.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.CreditoComputable = (creditoComputable != string.Empty) ? decimal.Parse(creditoComputable.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;

                entity.PercepcionIVA = (IIBB != string.Empty) ? decimal.Parse(percepcionIVA.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.SIRCREB = (otros != string.Empty) ? decimal.Parse(SIRCREB.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;
                entity.Importe10 = (Importe10 != string.Empty) ? decimal.Parse(Importe10.Replace(SeparadorDeMiles, SeparadorDeDecimales)) : 0;

                entity.Concepto = concepto;

                if (id > 0)
                {
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.GastosBancarios.Add(entity);
                    dbContext.SaveChanges();
                }

                idGastosBancarios = entity.IDGastosBancarios;
            }

            ContabilidadCommon.AgregarAsientoDeGastoBancario(idGastosBancarios, usu);
        }
        else
            throw new Exception("Por favor, vuelva a iniciar sesión");
    }
}