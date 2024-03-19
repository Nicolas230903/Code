<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= ConfigurationManager.AppSettings["Titulo"] %></title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="" />
    <link rel="shortcut icon" href="/images/favicon.png" type="image/png" />
    <link href="/css/style.default.css?v=1" rel="stylesheet" />
    <style type="text/css">
        small {
            font-size: 12px;
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


            <div id="divLogin">

                <div id="divLoginLoading" style="text-align: center; display: none" class="form">
                    <h3 style="color: #2b9b8f; font-family: Arial, Helvetica, sans-serif;">Un momento por favor</h3>
                    <p>Estamos verificando la información ingresada...</p>
                    <br />
                    <img src="images/loaders/greenLoader.gif" />
                </div>

                <div id="divLoginForm">
                    <div class="logged">
                        <small class="text-muted">Ingresa tu email y contraseña para ingresar al sistema.</small>
                    </div>
                    <form id="frmLogin" class="form">

                        <div class="alert alert-danger" id="divError" style="display: none"></div>

                        <input type="email" class="form-control required" id="txtUsuario" runat="server" placeholder="e-mail" maxlength="128" />
                        <input class="form-control required" id="txtPwd" runat="server" placeholder="contraseña" style="margin-top: 10px" maxlength="20" />

                        <small style="float: left; margin-top: 10px;">
                            <input type="checkbox" id="chkRecordarme" runat="server" />&nbsp;&nbsp;Recordarme</small>
                        <br />
                        <br />
                        <a class="btn btn-success btn-block" style="color: #fff;" onclick="Acceso.login();">Ingresar</a>
                        <br />
                        <div align="center">
                            <a style="width: 100px !important; display: inline-block; vertical-align: middle;" href="#" onclick="Acceso.showForm('recupero');"><small>¿Olvidaste tu contraseña?</small></a>
                            <a style="width: 100px !important; display: inline-block; vertical-align: middle;" href="registro.aspx"><small>Registrate gratis.</small></a>
                        </div>
                        <input type="hidden" value="0" runat="server" id="tieneDatos" />
                    </form>
                </div>
            </div>
            <div id="divRecupero" style="display: none">

                <div id="divRecuperoLoading" style="text-align: center; display: none" class="form">
                    <h3 style="color: #2b9b8f; font-family: Arial, Helvetica, sans-serif;">Verificando...</h3>
                    <br />
                    <br />
                    <img src="images/loaders/greenLoader.gif" />
                    <br />
                    <br />
                </div>

                <div id="divRecuperoFin" style="text-align: center; display: none" class="form">
                    <h3 style="color: #2b9b8f; font-family: Arial, Helvetica, sans-serif;">El restablecimiento de tu contraseña ha sido realizado</h3>
                    <p>Te enviamos un email con tu nueva contraseña.</p>
                    <small>(Si luego de unos minutos no lo recibes, por favor verifica en tu Correo No Deseado)</small>
                    <br />
                    <br />
                    <a href="#" onclick="Acceso.showForm('login');"><small>volver</small></a>
                </div>

                <div id="divRecuperoForm">
                    <div class="logged">
                        <small class="text-muted">Ingresa tu email para recuperar tu contraseña.</small>
                    </div>

                    <form id="frmRecupero">
                        <div class="alert alert-danger" id="divError2" style="display: none"></div>

                        <input type="email" class="form-control uname required" placeholder="email" id="txtEmail" maxlength="128" />
                        <a class="btn btn-success btn-block" onclick="Acceso.recuperar();">Recuperar</a>
                        <a href="#" onclick="Acceso.showForm('login');"><small>volver</small></a>
                    </form>
                </div>
            </div>
        </div>
        <!-- signin -->

    </section>

    <script src="/js/jquery-1.11.1.min.js"></script>
    <script src="/js/jquery-migrate-1.2.1.min.js"></script>
    <script src="/js/bootstrap.min.js"></script>
    <script src="/js/modernizr.min.js"></script>
    <script src="/js/jquery-ui-1.10.3.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/login.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/placeholders.jquery.min.js"></script>
</body>
</html>
