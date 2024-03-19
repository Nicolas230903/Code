<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="comprobantese.aspx.cs" Inherits="comprobantese" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .modal.modal-wide .modal-dialog {
            width: 90%;
            max-width: 900px;
        }

        .modal-wide .modal-body {
            overflow-y: auto;
        }

        .txtPercepciones {
            text-align: right;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>
            <label id="lblTitulo"></label>
        </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li>
                    <a href="/comprobantes.aspx">
                        <span id="lblSubtitulo"></span>
                    </a>
                </li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row">
            <form id="frmEdicion" runat="server" class="col-sm-12">
                <div class="panel panel-default">
                    <div class="alert alert-danger" id="divError" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgError"></span>
                    </div>
                    <div class="alert alert-success" id="divOk" style="display: none">
                        <strong>Bien hecho! </strong><span id="msgOk"></span>
                    </div>

                    <div class="panel-body">

                        <asp:Panel runat="server" ID="pnlContenido">

                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <h3>1. Datos generales</h3>                                    
                                        <span id="lblDatosGenerales" style="font-weight:normal;font-style:normal;color:#4a535e;" ></span>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-10 col-md-8 col-lg-5">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span>                                             
                                                Proveedor/Cliente

                                        </label>
                                        <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
                                            <option value=""></option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-1" id="divModalNuevoCliente">
                                    <div class="form-group">
                                        <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCliente" style="margin: 29px -10px; padding: 7px 15px">
                                            <i class="glyphicon glyphicon-plus"></i>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-sm-10" id="divClienteDatosPendientes" style="display: none">
                                    <div class="form-group">
                                        <div id="divClienteDatosPendientesTexto" style="color:red"></div>
                                        <label class="control-label" style="color:red" id="lblClienteDatosPendientes"></label>
                                    </div>
                                </div>
                                <div class="col-sm-10" id="divClienteAvisos" style="display: none">
                                    <div class="form-group">
                                        <div class="alert alert-warning" role="alert">                                             
                                            <div id="divClienteAvisosTexto"></div>
                                        </div>                                       
                                    </div>
                                </div>
                            </div>
                            <div id="divFactura" style="display: none">
                                <div class="row mb15">
                                    <div class="col-sm-4 col-md-2">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Tipo de comprobante</label>
                                            <select class="form-control required" id="ddlTipo" onchange="changeTipoComprobante();" data-placeholder="Seleccione un tipo">
                                                <option value=""></option>
                                                <option value="FCA">Factura A</option>
                                                <option value="FCB">Factura B</option>
                                                <option value="FCC">Factura C</option>
                                                <option value="NCA">Nota crédito A</option>
                                                <option value="NCB">Nota crédito B</option>
                                                <option value="NCC">Nota crédito C</option>
                                                <option value="NDA">Nota débito A</option>
                                                <option value="NDB">Nota débito B</option>
                                                <option value="NDC">Nota débito C</option>
                                                <option value="COT">Cotización</option>                                                
                                                <option value="PDV">Pedido de venta</option>
                                                <option value="PDC">Pedido de compra</option>
                                                <option value="EDA">Entrega de articulos</option>
                                                <option value="DDC">Detalle del comprobante</option>

                                                <option value="FCAMP">Factura A Mi PyMES</option>
                                                <option value="FCBMP">Factura B Mi PyMES</option>
                                                <option value="FCCMP">Factura C Mi PyMES</option>
                                                <option value="NCAMP">Nota crédito A Mi PyMES</option>
                                                <option value="NCBMP">Nota crédito B Mi PyMES</option>
                                                <option value="NCCMP">Nota crédito C Mi PyMES</option>
                                                <option value="NDAMP">Nota débito A Mi PyMES</option>
                                                <option value="NDBMP">Nota débito B Mi PyMES</option>
                                                <option value="NDCMP">Nota débito C Mi PyMES</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4 col-md-2" id="divModo">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Modo</label>
                                            <asp:DropDownList runat="server" ID="ddlModo" CssClass="form-control required" onchange="changeModo();">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-sm-4 col-md-2" id="divCondicionVenta">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> <span id="lblCondicion"></span></label>
                                            <select class="form-control required" id="ddlCondicionVenta">
                                                <option value="Efectivo">Efectivo</option>
                                                <option value="Cuenta corriente">Cuenta corriente</option>
                                                <option value="Tarjeta de debito">Tarjeta de debito</option>
                                                <option value="Tarjeta de credito">Tarjeta de credito</option>
                                                <option value="Cheque">Cheque</option>
                                                <option value="Ticket">Ticket</option>
                                                <option value="Otro">Otro</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4 col-md-2" id="divPuntoVenta">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Punto de venta</label>
                                            <select id="ddlPuntoVenta" class="form-control required" onchange="changePuntoDeVenta();">
                                            </select>
                                        </div>
                                    </div>
<%--                                    <div class="col-sm-4 col-md-2" id="divActividad">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Actividad</label>
                                            <select id="ddlActividad" class="form-control required">
                                            </select>
                                        </div>
                                    </div>--%>
                                    <div class="col-sm-4 col-md-2" id="divNroComprobante">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Nro. Comprobante</label>
                                            <asp:TextBox runat="server" ID="txtNumero" CssClass="form-control required" MaxLength="9"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-4 col-md-2" id="divComprobanteAsociado">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Comprobante Asociado</label>
                                            <select id="ddlComprobanteAsociado" class="form-control required" onchange="changeComprobanteAsociado();">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4 col-md-2" id="divComprobantePersona">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Comprobantes</label>
                                            <select id="ddlComprobantePersona" class="form-control required" onchange="changeComprobantePersona();">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4 col-md-4" id="divModalidadPagoAFIP">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Modalidad de pago AFIP</label>
                                            <select class="form-control" id="ddlModalidadPagoAFIP">
                                                <option value=""></option>
                                                <option value="SCA">Transferencia al sistema de Circulación Abierta</option>
                                                <option value="ADC">Agente de deposito Colectivo</option>                                                
                                            </select>
                                        </div>
                                    </div>
                                </div>                               
                                <div id="divFechas">
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <hr />
                                            <h3>2. Período facturado</h3>
                                            <label class="control-label">Acá van la fecha de emisión y la fecha de Vencimiento del cobro de la factura.</label> 
                                        </div>
                                    </div>
                                    <div class="row mb15">
                                        <div class="col-sm-2 col-md-2 hide">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Producto o Servicio</label>
                                                <select id="ddlProducto" class="form-control required" onchange="Common.obtenerConceptosCodigoyNombre('ddlProductos', this.value, true);changeConcepto();">
                                                    <option value="2">Servicio</option>
                                                    <option value="1" selected="selected">Producto</option>
                                                    <option value="3">Producto y servicio</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-md-2">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Fecha</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                    <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control required validDate validFechaActual" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-md-2">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Vencimiento del cobro</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                    <asp:TextBox runat="server" ID="txtFechaVencimiento" CssClass="form-control required validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divPercepciones">
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <hr />
                                            <h3>3. Percepciones especiales</h3>
                                            <label class="control-label">Estas son las percepciones especiales que aplican a tu perfil y data fiscal.</label>
                                        </div>
                                    </div>
                                    <div class="row row mb15">
                                        <div class="col-sm-4 col-md-2" id="divPercepcionesIVA">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Percepción IVA %</label>
                                                <asp:TextBox runat="server" ID="txtPercepcionIVA" CssClass="form-control txtPercepciones" MaxLength="10" ClientIDMode="Static" Text="0" onBlur="obtenerTotales();"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div id="divPercepcionesIIBB">
                                            <div class="col-sm-4 col-md-2">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Percepción IIBB %</label>
                                                    <asp:TextBox runat="server" ID="txtPercepcionIIBB" CssClass="form-control txtPercepciones" MaxLength="10" ClientIDMode="Static" Text="0" onBlur="obtenerTotales();"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-sm-4 col-md-3">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Jurisdicción</label>
                                                    <asp:DropDownList ID="ddlJuresdiccion" runat="server" CssClass="select3" data-placeholder="Seleccione una jurisdicción">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="mb40"></div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <hr />
                                        <h3 id="htextoFactura"></h3>                                        
                                            <span id="lblTextoFactura"></span>
                                    </div>
                                </div>
                                
                                <div id="divAgregarArticulo">
                                    <div class="well">
                                        <div class="alert alert-danger" id="divErrorDetalle" style="display: none">
                                            <strong>Lo siento! </strong><span id="msgErrorDetalle"></span>
                                        </div>

                                        <div class="row">
                                            <div class="col-sm-1 col-md-1">
                                                <span class="asterisk">*</span> Cantidad
                                                <input type="tel" class="form-control" maxlength="10" id="txtCantidad" value="1" runat="server"/>
                                            </div>
                                            <div class="col-sm-2 col-md-2">
                                                <span class="asterisk">*</span> 
                                                    Producto/Servicio/Concepto
                                                <select class="select3" data-placeholder="Seleccione un concepto" id="ddlProductos" onchange="changeConcepto();">
                                                </select>
                                            </div>
                                            <div class="col-sm-2 col-md-2" id="divDescripcion">
                                                Descripción
                                                <input type="text" class="form-control" maxlength="200" id="txtConcepto" />
                                            </div>
                                            <div class="col-sm-2 col-md-2">
                                                <span class="asterisk">*</span>
                                                <asp:Literal ID="liPrecioUnitario" runat="server"></asp:Literal>
                                                <input type="tel" class="form-control" maxlength="10" id="txtPrecio" />
                                            </div>
                                            <div class="col-sm-1 col-md-1" id="divBonif" hidden="hidden">
                                                Bonif %
                                                <input type="tel" class="form-control" maxlength="6" id="txtBonificacion" />
                                            </div>
                                            <div class="col-sm-1 col-md-1" id="divIva" hidden="hidden">
                                                <span class="asterisk">*</span> IVA %
                                                <select class="select2" id="ddlIva">
                                                    <option value="1">NG</option>
                                                    <option value="2">Exento</option>
                                                    <option value="3">0</option>
                                                    <option value="4">10,5</option>
                                                    <option value="5" selected="selected">21</option>
                                                    <option value="6">27</option>
                                                    <option value="8">5</option>
                                                    <option value="9">2,5</option>
                                                </select>
                                            </div>
                                            <div class="col-sm-1 col-md-1 divPlanDeCuentas">
                                                <div class="form-group">
                                                      <span class="asterisk">*</span> Cuenta contable
                                                    <asp:DropDownList runat="server" ID="ddlPlanDeCuentas" CssClass="select3" data-placeholder="Seleccione una cuenta...">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-sm-2 col-md-2">
                                                <br />
                                                <a class="btn btn-default btn-sm" id="btnAgregarItem" onclick="agregarItem();">Agregar</a>
                                                <a class="btn btn-default btn-sm" id="btnCancelarItem" onclick="cancelarItem();">Cancelar</a>
                                                <input type="hidden" runat="server" id="hdnIDItem" value="0" />                      
                                                <label id="lblAjusteSubtotal" hidden="hidden"><input type="checkbox" id="chkAjusteSubtotal" /><span>&nbsp;Ajustar</span></label> 
                                            </div>
                                        </div>
                                    </div>
                                </div>                                

                                <div class="table-responsive">
                                    <table class="table table-invoice" id="tbItems">
                                        <thead>
                                            <tr>
                                                <th>#</th>
                                                <th style="text-align: left">Cantidad</th>
                                                <th style="text-align: left">Código del Producto</th>
                                                <th style="text-align: left">Concepto</th>
                                                <th style="text-align: right" id="tdPrecio">Precio unitario sin IVA</th>
                                                <th style="text-align: right">Bonificación %</th>
                                                <th style="text-align: right">IVA %</th>
                                                <th style="text-align: right" hidden="hidden">TipoIVA</th>
                                                <th class="divPlanDeCuentas" style="text-align: left">Cuenta</th>
                                                <th style="text-align: right">Subtotal</th>
                                            </tr>
                                        </thead>
                                        <tbody id="bodyDetalle">
                                        </tbody>
                                    </table>
                                </div>
                                
                                


                                <div class="col-sm-12" id="divTotales">
                                    <table class="table table-total">
                                        <tbody>    
<%--                                            <tr id="trImpNoGravado">
                                                <td><strong>Neto no gravado :</strong></td>                                                
                                                <td id="Td1">
                                                    <asp:TextBox runat="server" ID="txtImporteNoGravado" CssClass="form-control txtPercepciones" MaxLength="10" ClientIDMode="Static" Text="0" onBlur="obtenerTotales();"></asp:TextBox>
                                                </td>
                                            </tr>  
                                            <tr id="trImpExento">
                                                <td><strong>Exento :</strong></td>                                                
                                                <td id="Td2">
                                                    <asp:TextBox runat="server" ID="txtImporteExento" CssClass="form-control txtPercepciones" MaxLength="10" ClientIDMode="Static" Text="0" onBlur="obtenerTotales();"></asp:TextBox>
                                                </td>
                                            </tr> --%>

                                            <tr>
                                                <td><strong>Neto no gravado :</strong></td>
                                                <td id="divImporteNoGravado">$ 0</td>
                                            </tr>
                                            <tr>
                                                <td><strong>Exento :</strong></td>
                                                <td id="divExento">$ 0</td>
                                            </tr>
                                            <tr>
                                                <td><strong>Neto gravado :</strong></td>
                                                <td id="divImporteGravado">$ 0</td>
                                            </tr>
                                            <tr>
                                                <td><strong>IVA :</strong></td>
                                                <td id="divIVA">$ 0</td>
                                            </tr>

                                            <tr id="trIVA">
                                                <td><strong>Percepción IVA :</strong></td>
                                                <td id="trIVATotal">$ 0</td>
                                                <%--<asp:TextBox runat="server" ID="txtPercepcionIVA" CssClass="form-control txtPercepciones" MaxLength="10" ClientIDMode="Static"></asp:TextBox></td>--%>
                                            </tr>
                                            <tr id="trIIBB">
                                                <td><strong>Percepción IIBB :</strong></td>
                                                <td id="trIIBBTotal">$ 0</td>
                                                <%--<asp:TextBox runat="server" ID="txtPercepcionIIBB" CssClass="form-control txtPercepciones" MaxLength="10" ClientIDMode="Static"></asp:TextBox></td>--%>
                                            </tr>

                                            <tr id="trDescuento">
                                                <td><strong>DESCUENTO : </strong><a class="btn btn-default btn-sm" id="btnDescuento" onclick="descuento();"><i class="fa fa-edit"></i></a></td>
                                                <td id="divDescuento">% 0</td>
                                            </tr>

                                            <tr>
                                                <td><strong>TOTAL :</strong></td>
                                                <td id="divTotal" style="color: green">$ 0</td>
                                            </tr>

                                            <tr id="trTotalCompras">
                                                <td><strong>TOTAL COMPRAS:</strong></td>
                                                <td id="divTotalCompras" style="color: green">$ 0</td>
                                            </tr>
                                        </tbody>
                                    </table>

                                </div>
                                <div class="mb40"></div>
                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <hr />
                                        <h3>3. Información Adicional</h3>                                        
                                    </div>
                                </div>
                                <div id="divDomicilio">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span>                                             
                                                        Domicilio de entrega
                                                </label>
                                                <select class="select4" data-placeholder="Seleccione un domicilio..." id="ddlDomicilio">
                                                    <option value=""></option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-1">
                                            <div class="form-group">
                                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoDomicilio" style="margin: 29px -10px; padding: 7px 15px">
                                                    <i class="glyphicon glyphicon-plus"></i>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>  
                                <div id="divTransporte">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span>                                             
                                                        Domicilio de Transporte
                                                </label>
                                                <select class="select5" data-placeholder="Seleccione un domicilio de transporte..." id="ddlTransporte">
                                                    <option value=""></option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-1">
                                            <div class="form-group">
                                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoTransporte" style="margin: 29px -10px; padding: 7px 15px">
                                                    <i class="glyphicon glyphicon-plus"></i>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>  
                                <div id="divTransportePersona">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span>                                             
                                                        Domicilio de transportes del cliente
                                                </label>
                                                <select class="select6" data-placeholder="Seleccione un domicilio de transporte..." id="ddlTransportePersona">
                                                    <option value=""></option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-1">
                                            <div class="form-group">
                                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoTransportePersona" style="margin: 29px -10px; padding: 7px 15px">
                                                    <i class="glyphicon glyphicon-plus"></i>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>  
                                <div id="divActividad">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label">                                             
                                                        Actividad
                                                </label>
                                                <select class="select6" data-placeholder="Seleccione una actividad..." id="ddlActividad">
                                                    <option value=""></option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div>  
                                <div class="row mb15">
                                    <div class="col-sm-12" id="div">
                                        <div class="form-group">
                                            <label class="control-label">Observaciones</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" Rows="6" ID="txtObservaciones" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div id="divNombre" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Nombre</label>
                                                <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divVendedor" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Vendedor</label>
                                                <asp:TextBox runat="server" ID="txtVendedor" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divEnvio" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-4 col-md-2">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Envio</label>
                                                <select class="form-control required" id="ddlEnvio">
                                                    <option value=""></option>
                                                    <option value="Con Envío">Con Envío</option>
                                                    <option value="Retiran">Retiran</option>
                                                    <option value="Retirado">Retirado</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divFechaEntrega" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-4 col-md-2">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Fecha Entrega</label>
                                                <asp:TextBox runat="server" ID="txtFechaEntrega" CssClass="form-control validDateFull" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divFechaAlta" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-4 col-md-2">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Fecha Alta</label>
                                                <asp:TextBox runat="server" ID="txtFechaAlta" CssClass="form-control validDateFull" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divEstado" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-4 col-md-2">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span> Estado</label>
                                                <select class="form-control required" id="ddlEstado">
                                                    <option value="Iniciado" selected="selected">Iniciado</option>
                                                    <option value="En curso">En curso</option>
                                                    <option value="Terminado">Terminado</option>
                                                    <option value="Cerrado">Cerrado</option>
                                                    <option value="Cancelado">Cancelado</option>
                                                    <option value="Suspendido">Suspendido</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="divVendedorComision" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-10 col-md-8 col-lg-5">
                                            <div class="form-group">
                                                <label class="control-label"><span class="asterisk">*</span>                                             
                                                        Vendedor con comisión
                                                </label>
                                                <select class="select6" data-placeholder="Seleccione un vendedor con comisión..." id="ddlVendedorComision">
                                                    <option value=""></option>
                                                </select>
                                            </div>
                                        </div>                           
                                    </div>
                                </div>  
                                <div class="mb40"></div>
                            </div>
                        </asp:Panel>
                    </div>                        
                    

                    <div class="panel-footer" runat="server" id="divFooter">
                        <a class="btn btn-success" onclick="grabar(false);" id="lnkAceptar">Aceptar</a>
                        <asp:HyperLink runat="server" ID="lnkGenerarCAE" onclick="grabar(true);" CssClass="btn btn-info mr5"><i class="fa fa-dollar mr5"></i>Emitir factura eléctronica</asp:HyperLink>
                        <asp:HyperLink Visible="false" runat="server" ID="lnkAnular" onclick="anular();" CssClass="btn btn-danger mr5">Anular</asp:HyperLink>
                        <a href="#" onclick="cancelar();" style="margin-left: 20px">Cancelar</a> 
                        <a runat="server" class="btn btn-white" onclick="descargarAdjunto();" style="float: right;" id="lnkDescargarAdjunto"><i class="fa fa-file fa-fw mr5"></i>Descargar adjunto</a>                        
                        <a class="btn btn-white" onclick="previsualizar();" style="float: right;" id="lnkPrevisualizar"><i class="fa fa-desktop fa-fw mr5"></i>Previsualizar en PDF</a>
                        <div class="btn-group dropup" style="float:right; padding-right: 10px; display: none" id="divMasOpciones">
                            <button type="button" class="btn btn-warning">Más opciones</button>
                            <button class="btn btn-warning dropdown-toggle" data-toggle="dropdown"><span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li id="liVincularComprobante"><a href="javascript:vincularComprobante('')">Copiar a Factura Completa</a></li>
                                <li id="liVincularComprobanteDividido"><a href="javascript:vincularComprobanteDividido('')">Copiar a Factura en mitades</a></li>
                                <li id="liVincularCompra"><a href="javascript:vincularCompra()">Copiar a Comprobante de Compra</a></li>
                                <li id="liJuntarComprobantes"><a href="javascript:JuntarComprobantes()">Juntar Comprobantes</a></li>
                                <li id="liDesvincularComprobantes"><a href="javascript:DesvincularComprobantes()">Desvincular Comprobantes</a></li>
                                <li id="liActualizarPrecios"><a href="javascript:ActualizarPrecios()">Actualizar Precios</a></li>
                                <li id="liImprimirPreLiquidoProducto"><a href="javascript:ImprimirLiquidoProducto('PreLiquidoProducto')">Imprimir Pre Liquido Producto</a></li>
                                <li id="liImprimirLiquidoProducto"><a href="javascript:ImprimirLiquidoProducto('LiquidoProducto')">Imprimir Liquido Producto</a></li>
                                <li id="liImprimirRemito"><a href="javascript:generarRemito('',true)">Imprimir Remito</a></li>
                                <li id="liImprimirRemitoSinLogo"><a href="javascript:abrirModalRemitoSinLogo('')">Imprimir Remito Sin Logo</a></li>
                                <li id="liImprimirRemitoTalonario"><a href="javascript:abrirModalRemitoTalonario('')">Imprimir Remito Talonario</a></li>
                                <li id="liGenerarNotaDeCreditoPorServicio"><a href="javascript:generarNotaDeCreditoPorServicio()">Generar Nota de crédito por servicio</a></li>
                                <li id="liVincularComprobanteEdaFac"><a href="javascript:vincularComprobanteEdaFac('')">Copiar a Factura desde EDA</a></li>
                            </ul>
                        </div>
                    </div>

                    <asp:HiddenField runat="server" ID="hdnEnvioFE" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnUsaPlanCorporativo" Value="" />
                    <asp:HiddenField runat="server" ID="hdnCodigo" Value="" />
                    <asp:HiddenField runat="server" ID="hdnUsaPrecioConIVA" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnPercepcionIIBB" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnPercepcionIVA" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnPresupuesto" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIDUsuario" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnTieneFE" Value="0" ClientIDMode="Static" />
                    <asp:HiddenField runat="server" ID="hdnPedidoDeVenta" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnTipo" Value="" />     
                    <asp:HiddenField runat="server" ID="hdnIdDomicilio" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnDescuento" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnParaPDVSolicitarCompletarContacto" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnHabilitarCambioIvaEnArticulosDesdeComprobante" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnNotaDeCreditoPorServicio" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIdTransporte" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIdTransportePersona" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIdPedidoDeVenta" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnPorcentajeDescuentoCliente" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIdComprobanteVinculado" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIdCompraVinculada" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnFacturaSoloContraEntrega" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnUsaCantidadConDecimales" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnMasDeUnaActividad" Value="0" />
                    <input type="hidden" id="hdnRazonSocial" runat="server" />
                    <input type="hidden" id="hdnClienteTipo" runat="server" />
                </div>
            </form>
        </div>
    </div>

    <div class="modal modal fade" id="modalOk" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="cerrarModelOk();">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Comprobante emitido correctamente. </h4>
                </div>
                <div class="modal-body" style="min-height: 250px;">
                    <div class="alert alert-danger" id="divErrorModalOk" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorModalOk"></span>
                    </div>
                    <div class="alert alert-success" id="divOkModalOk" style="display: none">
                        <strong>Bien hecho! </strong><span id="msgOkModalOk"></span>
                    </div>

                    <div class="alert alert-danger" id="divErrorAsientos" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorAsientos"></span>
                    </div>
                    <div class="alert alert-success" id="divOkAsientos" style="display: none">
                        <strong>Bien hecho! </strong>Asientos Contables generados correctamente
                    </div>

                    <div class="alert alert-danger" id="divErrorEnvioFE" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorEnvioFE"></span>
                    </div>
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>

                    <div id="divToolbar">
                        <div id="divSendMail" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="Toolbar.mostrarEnvio();">
                            <a id="lnkSendMail">
                                <span class="glyphicon glyphicon-envelope" id="imgMailEnvio" style="font-size: 30px;"></span>
                                <br />
                                <span id="spSendMail">Enviar por Email</span>&nbsp;<i style="color: #17a08c" id="iCheckEnvio" title="El correo automatico fue enviado correctamente"></i>
                            </a>
                        </div>
                        <div id="divDownloadPdfTicket" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPdfTicket" onclick="generarTicket();">
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Descargar Ticket
                            </a>
                        </div>
                        <div id="divDownloadPdf" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPdf" download>
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Descargar en PDF
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL hide">
                            <i class="glyphicon glyphicon-usd" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                                Cobrar
                        </div>
                        <div id="divVincularComprobante" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkVincularComprobante" onclick="vincularComprobante('ModalOk');">
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Vincular a Factura
                            </a>
                        </div>
                        <div id="divVincularCompra" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkVincularCompra" onclick="vincularCompra('ModalOk');">
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Vincular a Comprobante de compra
                            </a>
                        </div>
                        <div id="divPrintpdf" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="Common.imprimirArchivoDesdeIframe('');">
                            <a id="lnkPrintPdf">
                                <span class="glyphicon glyphicon-print" style="font-size: 30px;"></span>
                                <br />
                                Imprimir
                            </a>
                        </div>
                        <div id="divComprobante" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="cobrarComprobante();">
                            <a id="lnkcobrarComprobante">
                                <i class="glyphicon glyphicon-usd" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                                <br />
                                Realizar cobranza
                            </a>
                        </div>
                        <div id="divRemito" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadRemito" onclick="generarRemito('ModalOk',true);">
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Descargar Remito
                            </a>
                        </div>
                        <div id="divRemitoSinLogo" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadRemitoSinLogo" onclick="abrirModalRemitoSinLogo('ModalOk');">
                                <span class="fa fa-file-text" style="font-size: 25px;"></span>
                                <br />
                                Descargar Remito Sin Logo
                            </a>
                        </div>
                        <div id="divCargarEntrega" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkCargarEntrega" onclick="generarEntrega('ModalOk');">
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Cargar entrega
                            </a>
                        </div>
                        <div id="divCopiarAFacturaEdaFac" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkCopiarAFacturaEdaFac" onclick="vincularComprobanteEdaFac('ModalOk');">
                                <span class="fa fa-file-text" style="font-size: 25px;"></span>
                                <br />
                                Copiar a factura desde EDA
                            </a>
                        </div>
                    </div>
                    <div id="divSendEmail" style="display: none">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="alert alert-danger" id="divErrorMail" style="display: none">
                                    <strong>Lo sentimos! </strong><span id="msgErrorMail"></span>
                                </div>
                                <form id="frmSendMail">
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Para: <small>(separa las direcciones mediante una coma) </small></p>
                                        <input id="txtEnvioPara" class="form-control required multiemails" type="text" runat="server" />
                                        <%--<span id="msgErrorEnvioPara" class="help-block" style="display: none">Una de las direcciones ingresadas es inválida.</span>--%>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Asunto: </p>
                                        <input id="txtEnvioAsunto" class="form-control required" type="text" maxlength="150" runat="server" />
                                        <span id="msgErrorEnvioAsunto" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Mensaje: </p>
                                        <textarea rows="5" id="txtEnvioMensaje" class="form-control required" runat="server"></textarea>
                                        <span id="msgErrorEnvioMensaje" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <input type="hidden" id="hdnFile" />
                                    <br />
                                    <a id="btnEnviar" type="button" class="btn btn-success" onclick="Toolbar.enviarComprobantePorMail();">Enviar</a>
                                    <a style="margin-left: 20px" href="#" onclick="Toolbar.cancelarEnvio();">Cancelar</a>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div id="modalPdfTicket" style="display: none">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                                    <h4 class="modal-title">Visualización de ticket</h4>
                                </div>
                                <div class="modal-body">
                                    <div>
                                        <div class="alert alert-danger" id="divErrorPdfTicket" style="display: none">
                                            <strong>Lo sentimos! </strong><span id="msgErrorPdfTicket"></span>
                                        </div>

                                        <iframe id="ifrPdfTicket" src="" width="900px" height="500px" frameborder="0"></iframe>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal" onclick="cerrarModelOk();">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

        <!-- Modal -->
    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización de comprobante</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorCat" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCat"></span>
                        </div>

                        <iframe id="ifrPdf" src="" width="900px" height="500px" frameborder="0"></iframe>
                    </div>
                </div>
                <div class="modal-footer">
                    <div id="divPrevisualizarComprobante" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="cobrarComprobante();">
                        <a id="lnkPrevisualizarCobrarComprobante">
                            <i class="glyphicon glyphicon-usd" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Realizar cobranza
                        </a>
                    </div>
                    <div id="divPrevisualizarCerrar" class="col-sm-3 CAJA_BLANCA_AZUL">
                        <a id="lnkPrevisualizarCerrar" data-dismiss="modal">
                            <i class="glyphicon glyphicon-log-in" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cerrar
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    
    <div class="modal modal fade" id="modalJuntarComprobantes" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none">
        <div class="modal-dialog" >
            <div class="modal-content" style="position:fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Juntar Comprobantes</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorJuntarComprobantes" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorJuntarComprobantes"></span>
                        </div>

                        <div class="table-responsive">
                                <table class="table mb30" id="tablaJuntarComprobantes">
                                    <thead>
                                        <tr>     
                                                <th></th>
                                                <th>Nombre</th>
                                                <th>Fecha</th>
                                                <th>Modo</th>
                                                <th>Imp. Neto Grav</th>
                                                <th>Total Fact.</th>                                  
                                           
                                        </tr>
                                    </thead>
                                    <tbody id="resultsContainerJuntarComprobantes">
                                    </tbody>
                                </table>
                            </div>

                        <script id="resultTemplateJuntarComprobantes" type="text/x-jQuery-tmpl">
                            {{each results}}
                            <tr>
                                <td><input type='checkbox' name='comprobantesSeleccionados' class='checkbox' value='${ID}' /></td>
                                <td>${Nombre}</td>
                                <td>${Fecha}</td>
                                <td>${Modo}</td>
                                <td>${ImporteTotalBruto}</td>
                                <td>${ImporteTotalNeto}</td>                               
                            </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateJuntarComprobantes" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>                    
                </div>
                <div class="modal-footer">
                    <a href="#" type="button" style="margin-left: 20px"  onclick="grabarJuntarComprobantes();" class="btn btn-success">Juntar</a> 
                    <a href="#" type="button" style="margin-left: 20px"  data-dismiss="modal" class="btn btn-danger mr5">Cancelar</a> 
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalRemitoSinLogo" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Impresión de remito sin logo</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-4 col-md-4" style="text-align: right;">
                            <label class="control-label">                                     
                                    Margen Superior
                            </label>
                        </div>
                        <div class="col-sm-2 col-md-2">
                            <input type="text" class="form-control" id="txtRemitoSinLogoMargenVertical" maxlength="999" />
                        </div>
                        <div class="col-sm-4 col-md-4" style="text-align: right;">
                            <label class="control-label">                                     
                                    Margen Izquierdo
                            </label>
                        </div>
                        <div class="col-sm-2 col-md-2">
                            <input type="text" class="form-control" id="txtRemitoSinLogoMargenHorizontal" maxlength="999" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div id="divImprimirRemitoSinLogo" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="imprimirRemitoSinLogo();">
                        <a id="lnkImprimirRemitoSinLogo">
                            <i class="glyphicon glyphicon-print" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Imprimir
                        </a>
                    </div>
                    <div id="divImprimirRemitoSinLogoCerrar" class="col-sm-3 CAJA_BLANCA_AZUL">
                        <a id="lnkImprimirRemitoSinLogoCerrar" data-dismiss="modal">
                            <i class="glyphicon glyphicon-log-in" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cerrar
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalRemitoTalonario" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Impresión de remito Talonario</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-4 col-md-4" style="text-align: right;">
                            <label class="control-label">                                     
                                    Margen Superior
                            </label>
                        </div>
                        <div class="col-sm-2 col-md-2">
                            <input type="text" class="form-control" id="txtRemitoTalonarioMargenVertical" maxlength="999" />
                        </div>
                        <div class="col-sm-4 col-md-4" style="text-align: right;">
                            <label class="control-label">                                     
                                    Margen Izquierdo
                            </label>
                        </div>
                        <div class="col-sm-2 col-md-2">
                            <input type="text" class="form-control" id="txtRemitoTalonarioMargenHorizontal" maxlength="999" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-4 col-md-4" style="text-align: right;">
                            <label class="control-label">                                     
                                    Ver Total
                            </label>
                        </div>
                        <div class="col-sm-2 col-md-2">
                            <input type="checkbox" id="chkRemitoTalonarioVerTotal" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div id="divImprimirRemitoTalonario" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="imprimirRemitoTalonario();">
                        <a id="lnkImprimirRemitoTalonario">
                            <i class="glyphicon glyphicon-print" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Imprimir
                        </a>
                    </div>
                    <div id="divImprimirRemitoTalonarioCerrar" class="col-sm-3 CAJA_BLANCA_AZUL">
                        <a id="lnkImprimirRemitoTalonarioCerrar" data-dismiss="modal">
                            <i class="glyphicon glyphicon-log-in" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cerrar
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalFacturaDividida" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Copiar a factura en mitades</h4>
                </div>
                <div class="alert alert-success" id="divOkFacturaDividida" style="display: none">
                    <strong>Bien hecho! </strong><span id="msgOkFacturaDividida"></span>
                </div>
                <div class="alert alert-danger" id="divErrorFacturaDividida" style="display: none">
                    <strong>Lo sentimos! </strong><span id="msgErrorFacturaDividida"></span>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-3 col-md-3" style="text-align: right;">
                            <label class="control-label">                                     
                                    Porcentaje FAC 1 / FAC 2
                            </label>
                        </div>
                        <div class="col-sm-6 col-md-6">
                            <input type="range" class="form-control" min='1' max='100' value='50' id="txtRangoFacturas" oninput="RangoFacturasId.value=txtRangoFacturas.value"/>
                        </div>
                        <div class="col-sm-3 col-md-3">                            
                            <output id="RangoFacturasId">50</output>
                        </div>
                        <br /><br />
                        <div class="col-sm-3 col-md-3" style="text-align: right;">
                            <label class="control-label">                                     
                                    Punto de venta FAC 1
                            </label>
                        </div>
                        <div class="col-sm-6 col-md-6">
                            <select id="ddlPuntoVentaFac1" class="form-control required">
                            </select>
                        </div>
                        <br /><br />
                        <div class="col-sm-3 col-md-3" style="text-align: right;">
                            <label class="control-label">                                     
                                    Punto de venta FAC 2
                            </label>
                        </div>
                        <div class="col-sm-6 col-md-6">
                            <select id="ddlPuntoVentaFac2" class="form-control required">
                            </select>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnSuperaSuma" />
                <div class="modal-footer">
                    <div id="divFacturaDivididaGenerar" class="col-sm-3 CAJA_BLANCA_AZUL" onclick="vincularComprobanteDivididoGenerar('FacturaDividida');">
                        <a id="lnkFacturaDivididaGenerar">
                            <i class="glyphicon glyphicon-ok" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Generar
                        </a>
                    </div>
                    <div id="divFacturaDivididaCerrar" class="col-sm-3 CAJA_BLANCA_AZUL">
                        <a id="lnkFacturaDivididaCerrar" data-dismiss="modal">
                            <i class="glyphicon glyphicon-log-in" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cerrar
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/comprobantes.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/nuevaPersona.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/nuevoDomicilio.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/nuevoTransporte.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/nuevoTransportePersona.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>


    <UC:NuevaPersona runat="server" ID="ucCliente" />
    <UC:NuevoDomicilio runat="server" ID="ucDomicilio" />
    <UC:NuevoTransporte runat="server" ID="ucTransporte" />
    <UC:NuevoTransportePersona runat="server" ID="ucTransportePersona" />

    <script>
        jQuery(document).ready(function () {            
            configForm();
            var tipoComprobante = getParameterByName('tipo');    
            if (tipoComprobante == "PDC") {
                $("#hdnNuevaPersonaTipo").val("P");
            } else {
                $("#hdnNuevaPersonaTipo").val("C");
            }            
            $('#modalNuevoCliente').on('hidden.bs.modal', function (e) {   
                $("#hdnIDPersona").val($("#ddlPersona").val());
                Common.obtenerClientes("ddlPersona", $("#hdnIDPersona").val(), false);
                //limpiarControles();
            });
            $('#modalNuevoDomicilio').on('show.bs.modal', function (e) {   
                $("#hdnNuevoDomicilioIdPersona").val($("#ddlPersona").val());
            });
            $('#modalNuevoDomicilio').on('hidden.bs.modal', function (e) {   
                $("#hdnIdPersona").val($("#ddlPersona").val());
                Common.obtenerDomicilios("ddlDomicilio", $("#hdnIDDomicilio").val(), false, $("#ddlPersona").val());
                //limpiarControles();
            });
            $('#modalNuevoTransporte').on('show.bs.modal', function (e) {   
                $("#hdnNuevoTransporteIdUsuario").val($("#hdnIDUsuario").val());
            });
            $('#modalNuevoTransporte').on('hidden.bs.modal', function (e) {   
                Common.obtenerTransportes("ddlTransporte", $("#hdnIDTransporte").val(), false);
                //limpiarControles();
            });
            $('#modalNuevoTransportePersona').on('show.bs.modal', function (e) {   
                $("#hdnNuevoTransportePersonaIdUsuario").val($("#ddlPersona").val());
            });
            $('#modalNuevoTransportePersona').on('hidden.bs.modal', function (e) {   
                Common.obtenerTransportePersona("ddlTransportePersona", $("#hdnIDTransportePersona").val(), false, $("#ddlPersona").val());
                //limpiarControles();
            });
        });
    </script>
</asp:Content>
