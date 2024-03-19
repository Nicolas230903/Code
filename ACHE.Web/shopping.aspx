<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="shopping.aspx.cs" Inherits="shopping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link rel="stylesheet" href="/css/prettyPhoto.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="pageheader">
        <h2><i class="fa fa-shopping-cart"></i>Shopping <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Shopping</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row mb15">        
            <div class="alert alert-info fade in nomargin">
                <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
                <h4>Puedes añadir más módulos a tu contabilidad para añadir funcionalidad extra.!</h4>
                <p>Para cada módulo tienes una descripción de para qué sirve.<br />
                Algunos módulos son de pago, pueden ser mensuales, anuales o de pago único.<br />
                Si adquieres un módulo de pago tienes 15 dias para probarlo.<br />
                El pago en Contability es prepago, es decir, compras un bono (con tu tarjeta o mercadopago), y nosotros iremos descontando de tu saldo todos los meses el importe de cada módulo.<br />
                Si te quedas sin dinero, podrás volver a comprar un bono, o bien, el módulo ya no será visible y tendrás que adquirirlo de nuevo.<br />
                Pincha en Recargar Saldo para comprar un bono.</p>
            </div>
        </div>
        
        <div class="row mb15">
            <div class="row filemanager">
                <div class="col-xs-6 col-sm-4 col-md-2"  onclick="showModal('Multi usuario')">
                    <div class="thmb">
                        <div class="thmb-prev">
                            <a href="/images/photos/media5.png" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                            </a>
                        </div>
                        <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#modalMultiUsuario">Multi usuario</a></h5>
                        <small class="text-muted">Costo: $10 x mes</small>
                    </div>  
                </div>
                <div class="col-xs-6 col-sm-4 col-md-2"  onclick="showModal('Acceso cliente')">
                    <div class="thmb">
                        <div class="thmb-prev">
                            <a href="/images/photos/media5.png" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                            </a>
                        </div>
                        <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#modalAccesoCliente">Acceso cliente</a></h5>
                        <small class="text-muted">Costo: $10 x año</small>
                    </div>  
                </div>
                <div class="col-xs-6 col-sm-4 col-md-2"  onclick="showModal('Recibos de RRHH')">
                    <div class="thmb">
                        <div class="thmb-prev">
                            <a href="/images/photos/media5.png" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                            </a>
                        </div>
                        <h5 class="fm-title"><a href="#"  data-toggle="modal" data-target="#modalRecibosRRHH">Recibos de RRHH</a></h5>
                        <small class="text-muted">Costo: $10 x mes</small>
                    </div>  
                </div>
                <div class="col-xs-6 col-sm-4 col-md-2"  onclick="showModal('Caja diaria')">
                    <div class="thmb">
                        <div class="thmb-prev">
                            <a href="/images/photos/media5.png" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                            </a>
                        </div>
                        <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#modalCajaDiaria">Caja diaria</a></h5>
                        <small class="text-muted">Costo: $10 x mes</small>
                    </div>  
                </div>
                <div class="col-xs-6 col-sm-4 col-md-2" onclick="showModal('Tracking de horas y tareas')">
                    <div class="thmb">
                        <div class="thmb-prev">
                            <a href="/images/photos/media5.png" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                            </a>
                        </div>
                        <h5 class="fm-title"><a href="#" >Tracking de horas y tareas</a></h5>
                        <small class="text-muted">Costo: $10 x mes</small>
                    </div>  
                </div>
                <div class="col-xs-6 col-sm-4 col-md-2"  onclick="showModal('Constatación de comprobantes en AFIP')">
                    <div class="thmb">
                        <div class="thmb-prev">
                            <a href="/images/photos/media5.png" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                            </a>
                        </div>
                        <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#modalComprobantesAFIP">Constatación de comprobantes en AFIP</a></h5>
                        <small class="text-muted">Costo: $10 x mes</small>
                    </div>  
                </div>

            </div>
        </div>

    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalShopping" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle"> <span id="titDetalleShopping"></span></h4>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger" id="divShoppingError" style="display:none">
                        <strong>Lo sentimos! </strong><span id="msgShoppingError"></span>
                    </div>
                    
                    <div class="row">
                        <div class="col-sm-12">
                            <p>Por favor, indicanos en qué te podemos ayudar</p>
                            <textarea rows="5" id="txtShoppingMensaje" class="form-control"></textarea>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" onclick="enviarPaquetesShopping();">Enviar</button>
                    <a style="margin-left:20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>


</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" Runat="Server">
    <script src="/js/jquery.prettyPhoto.js"></script>
    <script src="/js/views/shopping.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //Replaces data-rel attribute to rel.
            //We use data-rel because of w3c validation issue
            $('a[data-rel]').each(function () {
                $(this).attr('rel', $(this).data('rel'));
            });

            $("a[rel^='prettyPhoto']").prettyPhoto({ social_tools: false });
        });

    </script>
</asp:Content>

