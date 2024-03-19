<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="faq.aspx.cs" Inherits="ayuda_faq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="pageheader">
        <h2><i class="glyphicon glyphicon-question-sign"></i>Centro de ayuda <span>Preguntas frecuentes</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Ayuda</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row">
            <div class="col-sm-3 col-lg-2">
                <h5 class="subtitle">Secciones</h5>
                <ul class="nav nav-pills nav-stacked nav-email mb20">
                    <li><a href="#monotributistas"><i class="fa fa-dot-circle-o"></i>Monotributistas</a></li>
                    <li><a href="#responsables"><i class="fa fa-dot-circle-o"></i>Responsables Inscriptos</a></li>
                    <li><a href="#desarrolladores"><i class="fa fa-dot-circle-o"></i>Desarrolladores</a></li>
                    <li class="hide"><a href="#seguridad"><i class="fa fa-dot-circle-o"></i>Seguridad</a></li>
                    <li><a href="#facturacionElectronica"><i class="fa fa-dot-circle-o"></i>Facturación electronica</a></li>
                    <li><a href="#facturacion"><i class="fa fa-dot-circle-o"></i>Facturación</a></li>
                </ul>
            </div>

            <div class="col-sm-9 col-lg-10">

                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4 class="panel-title"><a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#monotributistas">Monotributistas</a></h4>
                    </div>
                    <div id="monotributistas" class="panel-body panel-collapse collapse">
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon1">¿Todos los monotributistas estamos obligados a emitir facturas electrónicas?
                                </a>
                            </div>
                            <div id="mon1" class="panel-collapse collapse">
                                <div class="panel-body">
                                    No, sólo aquellos que están dentro de las categorías H, I, J, K, y L
                                </div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon2">¿Si estoy dentro de las categorías alcanzadas estoy obligado?
                                </a>
                            </div>
                            <div id="mon2" class="panel-collapse collapse">
                                <div class="panel-body">Si, excepto para aquellos comprobantes que respalden operaciones con consumidores finales, en las que se haya entregado el bien o prestado el servicio en el local, oficina o establecimiento</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon3">¿Cuál es la norma que generó esta nueva obligación para Monotributistas?
                                </a>
                            </div>
                            <div id="mon3" class="panel-collapse collapse">
                                <div class="panel-body">La norma que genero la obligación para Monotribustistas es la Resolución General 3067/2011.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon4">¿Desde cuándo estamos obligados los monotributistas H y superior?
                                </a>
                            </div>
                            <div id="mon4" class="panel-collapse collapse">
                                <div class="panel-body">Es obligatorio para comprobantes emitidos a partir del 1 de Mayo de 2011.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon5">¿Debo empadronarme en el régimen de factura electrónica?
                                </a>
                            </div>
                            <div id="mon5" class="panel-collapse collapse">
                                <div class="panel-body">No, los monotributistas quedan eximidos de cumplir con el empadronamiento. Si comienzo a emitir facturas electrónicas por ser categoría H, y en el próximo reempadronamiento recategorizó como G.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon6">¿Debo seguir emitiendo facturas electrónicas?
                                </a>
                            </div>
                            <div id="mon6" class="panel-collapse collapse">
                                <div class="panel-body">Ya no debería emitir facturas electrónicas, salvo que por su actividad o cierta situación especial también este alcanzado por otras Resoluciones Generales.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#mon7">¿Si mi categoría es inferior a H, puedo optar por ingresar al régimen de factura electrónica?
                                </a>
                            </div>
                            <div id="mon7" class="panel-collapse collapse">
                                <div class="panel-body">Si, se puede optar por ingresar al régimen</div>
                            </div>
                        </div>
                    </div>


                </div>

                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4 class="panel-title"><a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#responsables">Responsables Inscriptos</a></h4>
                    </div>
                    <div id="responsables" class="panel-body panel-collapse collapse">
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#resp1">¿Qué normativas sobre Factura electrónica es la que cumple axanweb?
                                </a>
                            </div>
                            <div id="resp1" class="panel-collapse collapse">
                                <div class="panel-body">Cumple con lo requerido para la gran mayoría de las empresas obligadas por la R.G. 2485 y sus modificatorias. No cubre las R.G. sobre emisión de comprobantes electrónicos de Exportación, sujetos con beneficios de bonos fiscales, ni sujetos obligados por juez administrativo competente.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#resp2">¿Estoy obligado a emitir facturas electrónicas?
                                </a>
                            </div>
                            <div id="resp2" class="panel-collapse collapse">
                                <div class="panel-body">A partir del 01/07/2015 todos están obligados.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#resp3">¿axanweb cumple con la R.G. 1361?
                                </a>
                            </div>
                            <div id="resp3" class="panel-collapse collapse">
                                <div class="panel-body">Si. axanweb cumple con lo establecido por la R.G. 1361, generando el archivo de duplicados electrónicos, como así también el registro de sus operaciones de Ventas.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#resp4">¿Qué impuestos puedo facturar con axanweb?
                                </a>
                            </div>
                            <div id="resp4" class="panel-collapse collapse">
                                <div class="panel-body">axanweb te permite administrar diferentes tipos de impuestos: IVA, Percepciones de IVA, Percepciones de Ingresos Brutos, Impuestos Internos.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#resp5">¿Emite el libro IVA Ventas?
                                </a>
                            </div>
                            <div id="resp5" class="panel-collapse collapse">
                                <div class="panel-body">Si, se imprime el libro IVA Ventas contemplando todos los tipos de comprobantes (electrónicos, preimpresos, y manuales).</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#resp6">¿Emite el libro IVA Compras?
                                </a>
                            </div>
                            <div id="resp6" class="panel-collapse collapse">
                                <div class="panel-body">Si, se imprime el libro IVA Compras contemplando todos los tipos de comprobantes (electrónicos, preimpresos, y manuales).</div>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4 class="panel-title"><a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#desarrolladores">Desarrolladores</a></h4>
                    </div>
                    <div id="desarrolladores" class="panel-body panel-collapse collapse">
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des1">¿Puedo integrar aplicaciones externas con axanweb?
                                </a>
                            </div>
                            <div id="des1" class="panel-collapse collapse">
                                <div class="panel-body">Sí, con axanweb podes integrar tu aplicación con tu cuenta de axanweb.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des2">¿Existen API's para utilizar axanweb ?
                                </a>
                            </div>
                            <div id="des2" class="panel-collapse collapse">
                                <div class="panel-body">Sí, axanweb cuenta con API's y ejemplos de integración.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des3">¿Tengo documentación disponible de axanweb?
                                </a>
                            </div>
                            <div id="des3" class="panel-collapse collapse">
                                <div class="panel-body">Sí, axanweb te brindará ejemplos y asistencia.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des4">¿Puedo facturar desde mis aplicaciones con axanweb?
                                </a>
                            </div>
                            <div id="des4" class="panel-collapse collapse">
                                <div class="panel-body">Sí, axanweb cuenta con métodos que permite emitir comprobantes desde las aplicaciones registradas.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des5">¿Puedo consultar datos de axanweb desde mis aplicaciones?
                                </a>
                            </div>
                            <div id="des5" class="panel-collapse collapse">
                                <div class="panel-body">Sí, axanweb expone la información registrada en el sistema para ser utilizada desde tus aplicaciones.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des6">¿Qué tecnologías necesito saber para utilizar axanweb ?
                                </a>
                            </div>
                            <div id="des6" class="panel-collapse collapse">
                                <div class="panel-body">axanweb es un servicio REST, la información es accedida utilizando el verbo POST de HTTP y se puede consumir con cualquier tecnología. La API y los ejemplos están desarrollados con Microsoft C#.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des7">¿Mis datos de axanweb pueden ser leídos por otras aplicaciones?
                                </a>
                            </div>
                            <div id="des7" class="panel-collapse collapse">
                                <div class="panel-body">
                                    No, únicamente las aplicaciones que autorizas pueden leer los datos de tu empresa. Si querés que una aplicación deje de tener acceso a tus datos, desde axanweb la debés desactivar.
                                </div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des8">¿Para qué utilizaría axanweb ?
                                </a>
                            </div>
                            <div id="des8" class="panel-collapse collapse">
                                <div class="panel-body">axanweb te resuelve la necesidad de emitir comprobantes sin tener que invertir tiempo en todo el ciclo de desarrollo de un módulo de facturación.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#des9">¿Puedo desarrollar aplicaciones para consultar datos de axanweb?
                                </a>
                            </div>
                            <div id="des9" class="panel-collapse collapse">
                                <div class="panel-body">Sí, podés desarrollar aplicaciones que consulten la información y emitan comprobantes de axanweb sin estar registrado. Para que tu aplicación se integre, un usuario de axanweb deberá autorizar el acceso de tu aplicación a sus datos.</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default hide">
                    <div class="panel-body">
                        <h4 class="panel-title"><a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#seguridad">Seguridad</a></h4>
                    </div>
                    <div id="seguridad" class="panel-body panel-collapse collapse">
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#seg1">Will you be providing continuous support and adding new features?
                                </a>
                            </div>
                            <div id="seg1" class="panel-collapse collapse">
                                <div class="panel-body">Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid.</div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#seg2">Will you be providing continuous support and adding new features?
                                </a>
                            </div>
                            <div id="seg2" class="panel-collapse collapse">
                                <div class="panel-body">Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor brunch. Food truck quinoa nesciunt laborum eiusmod. Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry richardson ad squid.</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4 class="panel-title"><a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#facturacionElectronica">Facturación electronica</a></h4>
                    </div>
                    <div id="facturacionElectronica" class="panel-body panel-collapse collapse">
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#face1">¿Como crear un punto de venta?
                                </a>
                            </div>
                            <div id="face1" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Paso 1: Habilitar su punto de venta electrónico. Puede descargar los pasos a seguir desde 
                                    <a href="/ayuda/manuales/Guia-paso-a-paso-para-configurar-nuevo-punto-de-venta-AFIP.pdf" download>aquí</a><br />
                                </div>
                            </div>
                        </div>
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#face2">¿Como habilitar su CUIT en AFIP?
                                </a>
                            </div>
                            <div id="face2" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Paso 2: Habilitar su CUIT en AFIP. Puede descargar los pasos a seguir desde 
                                    <a href="/ayuda/manuales/Guia-paso-a-paso-para-configurar-FE-en-axanweb.pdf" download>aquí</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panel panel-default">
                    <div class="panel-body">
                        <h4 class="panel-title"><a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#facturacion">Facturación</a></h4>
                    </div>
                    <div id="facturacion" class="panel-body panel-collapse collapse">
                        <div class="panel">
                            <div class="panel-heading">
                                <a class="accordion-toggle accordion-icon link-unstyled" data-toggle="collapse" href="#fac1">¿Como crear una nota de crédito?
                                </a>
                            </div>
                            <div id="fac1" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Paso 1: Ingrese a ventas y al hacer su factura seleccione el tipo de comprobante Nota de crédito. Genere el comprobante.<br />
                                    Paso 2: Ingrese a la pantalla de cobranzas y seleccione el o los comprobantes que desea anular con la nota de crédito. E ingrese el importe en 0<br />
                                    Paso 3: En la sección de formas de pago elija la nota de crédito. Genere la cobranza.<br />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>

