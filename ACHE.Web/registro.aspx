<%@ Page Language="C#" AutoEventWireup="true" CodeFile="registro.aspx.cs" Inherits="registro" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= ConfigurationManager.AppSettings["Titulo"] %></title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="" />
    <link rel="shortcut icon" href="/images/favicon.png" type="image/png" />
    <link href="/css/style.default.css" rel="stylesheet" />
    <style type="text/css">
        small {
            font-size: 12px;
        }
        .lockedpanel {
            width: 280px;
            margin: 0% auto 0 auto;
        }
    </style>
    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
        <script src="/js/html5shiv.js"></script>
        <script src="/js/respond.min.js"></script>
    <![endif]-->
</head>
<body class="notfound">
    <section>

        <div class="lockedpanel">
            <div class="loginuser">
                <img src="images/logo-login.png" alt="axanweb" />
            </div>
            <div id="divRegistroForm">
                <div class="logged">
                    <h4>Registrate. Es gratis</h4>
                    <br />
                    <small>¿Ya sos un usuario? Hace click <a href="login.aspx"><strong>aquí</strong></a> para ingresar</small>
                </div>

                <form runat="server" id="frmRegistro">
                    <div class="alert alert-danger" id="divError" style="display: none"></div>
                    <asp:TextBox runat="server" ID="txtNroDocumento" CssClass="form-control required number validCuit" MaxLength="13" placeholder="Ingresá tu CUIT"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" CssClass="form-control required" MaxLength="128" Style="margin-top: 10px" placeholder="Ingresá tu email"></asp:TextBox>
                    <asp:TextBox runat="server" TextMode="Password" ID="txtPwd" CssClass="form-control required validPassword" MaxLength="20" MinLength="8" Style="margin-top: 10px" placeholder="Ingresá tu contraseña"></asp:TextBox>
                    <asp:TextBox runat="server" TextMode="Password" ID="txtPwd2" CssClass="form-control required validPassword2" MaxLength="20" MinLength="8" Style="margin-top: 10px" placeholder="Ingresá nuevamente tu contraseña"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtTelefono" CssClass="form-control required" MaxLength="30" MinLength="6" Style="margin-top: 10px" placeholder="+54911########"></asp:TextBox>
                    <small><br />¿Tenés un código de promoción? <a href="#" onclick="$('#txtCodigoPromocion').slideToggle();"><strong>Ingresalo aquí</strong></a></small>
                    <asp:TextBox runat="server" ID="txtCodigoPromocion" CssClass="form-control" MaxLength="150" Style="margin-top: 10px; display: none" placeholder="Ingresá el código de promoción"></asp:TextBox>

                    <br />
                    <small style="float: left; margin-top: 10px;">
                        <input type="checkbox" id="chkTyC" runat="server" />&nbsp;&nbsp;Acepto los <a href="fe/TerminosYCondiciones.pdf" target="_blank">Términos y Condiciones</a></small>
                    <br />
                    <br />
                    <a class="btn btn-success" id="lnkRegistrarme" onclick="grabar();">Registrarme gratis</a>
                    <a href="login.aspx"><small>volver</small></a>
                </form>

            </div>
        </div>

    </section>

    <script src="/js/jquery-1.11.1.min.js"></script>
    <script src="/js/jquery-migrate-1.2.1.min.js"></script>
    <script src="/js/jquery-ui-1.10.3.min.js"></script>
    <script src="/js/bootstrap.min.js"></script>
    <script src="/js/modernizr.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.numericInput.min.js"></script>
    <script src="/js/views/common.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/registro.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/placeholders.jquery.min.js"></script>
</body>
</html>
