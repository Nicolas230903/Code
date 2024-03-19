using System;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;
using Image = System.Drawing.Image;
using System.Collections.Generic;

namespace ACHE.FacturaElectronica.Lib
{
    /// <summary>
    /// Crea documentos PDF y permite insertar contenido
    /// </summary>
    public class NFPDFWriter
    {
        private readonly BaseFont font = BaseFont.CreateFont(FontFactory.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        private readonly BaseFont font_titulo = BaseFont.CreateFont(FontFactory.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        private readonly Font font_sub_titulo = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        private readonly Font font_cell_titulo = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        private readonly Font font_cell_detalle = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        private readonly Font font_cell_totales = FontFactory.GetFont(FontFactory.HELVETICA_BOLDOBLIQUE, 8);
        private string fileName = "Documento.pdf";
        private Orientacion _orientacion = Orientacion.Vertical;
        private PdfContentByte cb;
        private Document document;
        private readonly MemoryStream mem = new MemoryStream();
        private string autor = "";
        private string titulo = "Generado por NFPDFWriter";
        private PdfWriter writer;
        private readonly string _titulo = "";
        private readonly string _subtitulo = "";


        public NFPDFWriter(MedidasDocumento medidaDocumento)
        {
            Inicializar(medidaDocumento, null, Orientacion.Vertical);
        }

        public NFPDFWriter(MedidasDocumento medidaDocumento, string titulo, string subtitulo)
        {
            _titulo = titulo;
            _subtitulo = subtitulo;
            Inicializar(medidaDocumento, null, Orientacion.Vertical);
        }

        public NFPDFWriter(MedidasDocumento medidaDocumento, Orientacion orientacion)
        {
            Inicializar(medidaDocumento, null, orientacion);
        }

        public NFPDFWriter(float width, float height)
        {
            Rectangle documentSize;
            documentSize = new Rectangle(width, height);
            Inicializar(documentSize, null, Orientacion.Vertical);
        }

        public NFPDFWriter(MedidasDocumento medidaDocumento, string fileTemplate)
        {
            Inicializar(medidaDocumento, fileTemplate, Orientacion.Vertical);
        }

        private void AgregarEncabezadoPagina(ref Document document, PdfWriter writer)
        {
            PdfContentByte cb;

            cb = writer.DirectContent;
            cb.BeginText();

            //imprimo el titulo
            cb.SetFontAndSize(font_titulo, 13);
            if (this._orientacion == Orientacion.Vertical)
                cb.ShowTextAligned(Element.ALIGN_CENTER, this._titulo, PageSize.A4.Width / 2, PageSize.A4.Height - 60, 0);
            else
                cb.ShowTextAligned(Element.ALIGN_CENTER, this._titulo, PageSize.A4.Rotate().Width / 2, PageSize.A4.Rotate().Height - 40, 0);

            //imprimo el subtitulo
            cb.SetFontAndSize(font_titulo, font_sub_titulo.Size);
            if (this._orientacion == Orientacion.Vertical)
                cb.ShowTextAligned(Element.ALIGN_CENTER, this._subtitulo, PageSize.A4.Width / 2, PageSize.A4.Height - 92, 0);
            else
                cb.ShowTextAligned(Element.ALIGN_CENTER, this._subtitulo, PageSize.A4.Rotate().Width / 2, PageSize.A4.Rotate().Height - 72, 0);


            //imprimo la fecha y hora de impresion
            cb.SetFontAndSize(font_titulo, 7);
            if (this._orientacion == Orientacion.Vertical)
                cb.ShowTextAligned(Element.ALIGN_RIGHT, "Generado el día " + DateTime.Now.ToString("dd/MM/yyyy") + " a las " + DateTime.Now.ToString("HH:mm"), PageSize.A4.Width - document.RightMargin, PageSize.A4.Height - 92, 0);
            else
                cb.ShowTextAligned(Element.ALIGN_RIGHT, "Generado el día " + DateTime.Now.ToString("dd/MM/yyyy") + " a las " + DateTime.Now.ToString("HH:mm"), PageSize.A4.Rotate().Width - document.RightMargin, PageSize.A4.Rotate().Height - 72, 0);

            cb.EndText();

            cb.SetLineWidth(0.5f);
            if (this._orientacion == Orientacion.Vertical)
            {
                cb.MoveTo(document.LeftMargin, PageSize.A4.Height - 95);
                cb.LineTo(PageSize.A4.Width - document.RightMargin, PageSize.A4.Height - 95);
            }
            else
            {
                cb.MoveTo(document.LeftMargin, PageSize.A4.Rotate().Height - 75);
                cb.LineTo(PageSize.A4.Rotate().Width - document.RightMargin, PageSize.A4.Rotate().Height - 75);
            }

            cb.Stroke();
        }

        private void Inicializar(MedidasDocumento medidaDocumento, string fileTemplate, Orientacion orientacion)
        {
            Rectangle documentSize;

            switch (medidaDocumento)
            {
                case MedidasDocumento.A4:
                    documentSize = PageSize.A4;
                    break;

                case MedidasDocumento.Carta:
                    documentSize = PageSize.LETTER;
                    break;

                case MedidasDocumento.Oficio:
                    documentSize = PageSize.LEGAL;
                    break;

                case MedidasDocumento.RemitoPolydem:
                    documentSize = new Rectangle(482, 652);
                    break;

                case MedidasDocumento.Etiquetas108mmX3:
                    documentSize = new Rectangle(425, 83);
                    break;

                case MedidasDocumento.Ticket:
                    documentSize = new Rectangle(0, 0, 156, 280);
                    break;

                case MedidasDocumento.Ticket80:
                    documentSize = new Rectangle(0, 0, 190, 280);
                    break;

                default:
                    throw new Exception("No se definio un tamaño");
            }

            Inicializar(documentSize, fileTemplate, orientacion);
        }

        private void Inicializar(Rectangle documentSize, string fileTemplate, Orientacion orientacion)
        {
            this._orientacion = orientacion;

            if (this._orientacion == Orientacion.Vertical)
                document = new Document(documentSize);
            else
                document = new Document(documentSize.Rotate());

            writer = PdfWriter.GetInstance(document, mem);

            document.AddAuthor(autor);
            document.AddCreationDate();
            document.AddCreator("NFPDFWriter");
            document.AddTitle(titulo);

            if (_titulo != "")
            {
                // create add the event handler
                MyPageEvents events = new MyPageEvents();
                writer.PageEvent = events;
            }

            document.Open();
            document.NewPage();
            cb = writer.DirectContent;

            if (_titulo != "")
                AgregarEncabezadoPagina(ref document, writer);

            if (fileTemplate != null)
            {
                PdfReader reader = new PdfReader(fileTemplate);
                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                cb.AddTemplate(page, 0, 0);
            }
        }

        #region Propiedades

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public Orientacion Orientacion
        {
            get { return _orientacion; }
        }

        public string Autor
        {
            get { return autor; }
            set { autor = value; }
        }

        public float Width
        {
            get { return document.PageSize.Width; }
        }

        public float Height
        {
            get { return document.PageSize.Height; }
        }

        #endregion

        #region Metodos

        public void Escribir(string texto, Alineado alineado)
        {
            int align = Element.ALIGN_LEFT;

            switch (alineado)
            {
                case Alineado.Centro:
                    align = Element.ALIGN_CENTER;
                    break;

                case Alineado.Justificado:
                    align = Element.ALIGN_JUSTIFIED;
                    break;

                case Alineado.Izquierda:
                    align = Element.ALIGN_LEFT;
                    break;

                case Alineado.Derecha:
                    align = Element.ALIGN_RIGHT;
                    break;
            }

            Paragraph p1 = new Paragraph();
            p1.Alignment = align;
            p1.Add(texto);
            document.Add(p1);
        }

        private MemoryStream PrepararPDF()
        {
            document.Close();
            return new MemoryStream(mem.ToArray());
        }

        private int Align(Alineado alineado)
        {
            int align = Element.ALIGN_LEFT;

            switch (alineado)
            {
                case Alineado.Centro:
                    align = Element.ALIGN_CENTER;
                    break;

                case Alineado.Justificado:
                    align = Element.ALIGN_JUSTIFIED;
                    break;

                case Alineado.Izquierda:
                    align = Element.ALIGN_LEFT;
                    break;

                case Alineado.Derecha:
                    align = Element.ALIGN_RIGHT;
                    break;
            }

            return align;
        }

        public void InsertarTablaDetalle(FEComprobante comp, int xInit, int yInit)
        {
            PdfPTable table = new PdfPTable(6);
            List<FEItemDetalle> ItemsDetalle = comp.ItemsDetalle;

            foreach (FEItemDetalle item in ItemsDetalle)
            {
                PdfPCell cellCantidad;
                PdfPCell cellCodigo;
                PdfPCell cellDescripcion;
                PdfPCell cellPrecio;
                PdfPCell cellBonificacion;
                PdfPCell cellTotal;

                cellCantidad = new PdfPCell(new Phrase(item.Cantidad.ToString(), font_cell_detalle));
                cellCodigo = new PdfPCell(new Phrase(item.Codigo, font_cell_detalle));
                cellDescripcion = new PdfPCell(new Phrase(item.Descripcion, font_cell_detalle));
                cellBonificacion = new PdfPCell(new Phrase(item.Bonificacion.ToString("N2") + " %", font_cell_detalle));

                

                if (comp.TipoComprobante == FETipoComprobante.FACTURAS_B &&
                    comp.CondicionIva.ToUpper() == "RESPONSABLE INSCRIPTO" &&
                    (comp.ClienteCondiionIva.ToUpper() == "CONSUMIDOR FINAL" || comp.ClienteCondiionIva.ToUpper() == "EXENTO"))
                {
                    cellPrecio = new PdfPCell(new Phrase("$ " + item.PrecioConIVA.ToString("N2"), font_cell_detalle));
                    cellTotal = new PdfPCell(new Phrase("$ " + item.TotalConIVA.ToString("N2"), font_cell_detalle));
                }
                else
                {
                    cellPrecio = new PdfPCell(new Phrase("$ " + item.Precio.ToString("N2"), font_cell_detalle));                    
                    cellTotal = new PdfPCell(new Phrase("$ " + item.Total.ToString("N2"), font_cell_detalle));
                }

                //PdfPCell cellCantidad = new PdfPCell(new Phrase(item.Cantidad.ToString(), font_cell_detalle));
                //PdfPCell cellCodigo = new PdfPCell(new Phrase(item.Codigo, font_cell_detalle));
                //PdfPCell cellDescripcion = new PdfPCell(new Phrase(item.Descripcion, font_cell_detalle));
                //PdfPCell cellPrecio = new PdfPCell(new Phrase("$ " + item.PrecioConIVA.ToString("N2"), font_cell_detalle));
                //PdfPCell cellBonificacion = new PdfPCell(new Phrase(item.Bonificacion.ToString("N2") + " %", font_cell_detalle));
                //PdfPCell cellTotal = new PdfPCell(new Phrase("$ " + item.TotalConIVA.ToString("N2"), font_cell_detalle));

                cellCantidad.BorderWidth = 0;
                cellCantidad.PaddingBottom = 5;
                cellCantidad.PaddingRight = 5;
                cellCantidad.PaddingLeft = 5;
                cellCantidad.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellCantidad);

                cellCodigo.BorderWidth = 0;
                cellCodigo.PaddingBottom = 5;
                cellCodigo.PaddingRight = 5;
                cellCodigo.PaddingLeft = 5;
                cellCodigo.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellCodigo);

                cellDescripcion.BorderWidth = 0;
                cellDescripcion.PaddingBottom = 5;
                cellDescripcion.PaddingRight = 5;
                cellDescripcion.PaddingLeft = 5;
                cellDescripcion.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellDescripcion);

                cellPrecio.BorderWidth = 0;
                cellPrecio.PaddingBottom = 5;
                cellPrecio.PaddingRight = 5;
                cellPrecio.PaddingLeft = 5;
                cellPrecio.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellPrecio);

                cellBonificacion.BorderWidth = 0;
                cellBonificacion.PaddingBottom = 5;
                cellBonificacion.PaddingRight = 5;
                cellBonificacion.PaddingLeft = 5;
                cellBonificacion.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellBonificacion);

                cellTotal.BorderWidth = 0;
                cellTotal.PaddingBottom = 5;
                cellTotal.PaddingRight = 5;
                cellTotal.PaddingLeft = 5;
                cellTotal.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellTotal);
            }

            float[] cellWidths = new float[] { 59, 64, 213, 98, 58, 58 };

            table.SetWidths(cellWidths);
            table.TotalWidth = cellWidths.Sum();

            table.DefaultCell.Border = Rectangle.NO_BORDER;
            cb.SetFontAndSize(font, 10);
            table.WriteSelectedRows(0, ItemsDetalle.Count, xInit + 22, yInit + 555, cb);
        }

        public void InsertarTablaDetalleCobranza(FEComprobante comp, int xInit, int yInit)
        {
            PdfPTable table = new PdfPTable(3);
            List<FEItemDetalle> ItemsDetalle = comp.ItemsDetalle;

            foreach (FEItemDetalle item in ItemsDetalle)
            {
                PdfPCell cellFecha;                
                PdfPCell cellDescripcion;
                PdfPCell cellImporte;

                cellFecha = new PdfPCell(new Phrase(item.Fecha, font_cell_detalle));
                cellDescripcion = new PdfPCell(new Phrase(item.Descripcion, font_cell_detalle));
                cellImporte = new PdfPCell(new Phrase("$ " + item.Total.ToString("N2"), font_cell_detalle));

                cellFecha.BorderWidth = 0;
                cellFecha.PaddingBottom = 5;
                cellFecha.PaddingRight = 5;
                cellFecha.PaddingLeft = 5;
                cellFecha.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellFecha);


                cellDescripcion.BorderWidth = 0;
                cellDescripcion.PaddingBottom = 5;
                cellDescripcion.PaddingRight = 5;
                cellDescripcion.PaddingLeft = 5;
                cellDescripcion.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellDescripcion);

                cellImporte.BorderWidth = 0;
                cellImporte.PaddingBottom = 5;
                cellImporte.PaddingRight = 5;
                cellImporte.PaddingLeft = 5;
                cellImporte.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellImporte);
            }

            float[] cellWidths = new float[] { 75, 365, 86};

            table.SetWidths(cellWidths);
            table.TotalWidth = cellWidths.Sum();

            table.DefaultCell.Border = Rectangle.NO_BORDER;
            cb.SetFontAndSize(font, 10);
            table.WriteSelectedRows(0, ItemsDetalle.Count, xInit + 22, yInit + 555, cb);

        }

        public void InsertarTablaDetalleCobranzaFormasDePago(FEComprobante comp, int xInit, int yInit)
        {
            PdfPTable table = new PdfPTable(3);
            List<FEItemFormasDePago> ItemsFormasDePago = comp.ItemsFormasDePago;

            foreach (FEItemFormasDePago item in ItemsFormasDePago)
            {
                PdfPCell cellFecha;
                PdfPCell cellDescripcion;
                PdfPCell cellImporte;

                cellFecha = new PdfPCell(new Phrase(item.Fecha, font_cell_detalle));
                cellDescripcion = new PdfPCell(new Phrase(item.Descripcion, font_cell_detalle));
                cellImporte = new PdfPCell(new Phrase("$ " + item.Total.ToString("N2"), font_cell_detalle));

                cellFecha.BorderWidth = 0;
                cellFecha.PaddingBottom = 5;
                cellFecha.PaddingRight = 5;
                cellFecha.PaddingLeft = 5;
                cellFecha.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellFecha);


                cellDescripcion.BorderWidth = 0;
                cellDescripcion.PaddingBottom = 5;
                cellDescripcion.PaddingRight = 5;
                cellDescripcion.PaddingLeft = 5;
                cellDescripcion.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellDescripcion);

                cellImporte.BorderWidth = 0;
                cellImporte.PaddingBottom = 5;
                cellImporte.PaddingRight = 5;
                cellImporte.PaddingLeft = 5;
                cellImporte.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellImporte);
            }

            float[] cellWidths = new float[] { 75, 365, 86 };

            table.SetWidths(cellWidths);
            table.TotalWidth = cellWidths.Sum();

            table.DefaultCell.Border = Rectangle.NO_BORDER;
            cb.SetFontAndSize(font, 10);
            table.WriteSelectedRows(0, ItemsFormasDePago.Count, xInit + 22, yInit + 555, cb);

        }

        public void InsertarTablaDetalleRemitoTalonario(FEComprobante comp, int xInit, int yInit)
        {
            PdfPTable table = new PdfPTable(3);
            List<FEItemDetalle> ItemsDetalle = comp.ItemsDetalle;

            foreach (FEItemDetalle item in ItemsDetalle)
            {
                PdfPCell cellCantidad;
                PdfPCell cellCodigo;
                PdfPCell cellDescripcion;

                cellCantidad = new PdfPCell(new Phrase(item.Cantidad.ToString(), font_cell_detalle));
                cellCodigo = new PdfPCell(new Phrase(item.Codigo, font_cell_detalle));
                cellDescripcion = new PdfPCell(new Phrase(item.Descripcion, font_cell_detalle));


                cellCantidad.BorderWidth = 0;
                cellCantidad.PaddingBottom = 5;
                cellCantidad.PaddingRight = 5;
                cellCantidad.PaddingLeft = 5;
                cellCantidad.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellCantidad);

                cellCodigo.BorderWidth = 0;
                cellCodigo.PaddingBottom = 5;
                cellCodigo.PaddingRight = 5;
                cellCodigo.PaddingLeft = 5;
                cellCodigo.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellCodigo);

                cellDescripcion.BorderWidth = 0;
                cellDescripcion.PaddingBottom = 5;
                cellDescripcion.PaddingRight = 5;
                cellDescripcion.PaddingLeft = 5;
                cellDescripcion.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellDescripcion);

            }

            float[] cellWidths = new float[] { 59, 64, 213};

            table.SetWidths(cellWidths);
            table.TotalWidth = cellWidths.Sum();

            table.DefaultCell.Border = Rectangle.NO_BORDER;
            cb.SetFontAndSize(font, 10);
            table.WriteSelectedRows(0, ItemsDetalle.Count, xInit + 22, yInit + 665, cb);
        }

        public void InsertarTablaDetalleRemito(List<FEItemDetalle> ItemsDetalle, int xInit, int yInit)
        {
            PdfPTable table = new PdfPTable(3);

            foreach (FEItemDetalle item in ItemsDetalle)
            {
                PdfPCell cellCantidad = new PdfPCell(new Phrase(item.Cantidad.ToString(), font_cell_detalle));
                PdfPCell cellCodigo = new PdfPCell(new Phrase(item.Codigo, font_cell_detalle));
                PdfPCell cellDescripcion = new PdfPCell(new Phrase(item.Descripcion, font_cell_detalle));

                cellCantidad.BorderWidth = 0;
                cellCantidad.PaddingBottom = 5;
                cellCantidad.PaddingRight = 5;
                cellCantidad.PaddingLeft = 5;
                cellCantidad.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellCantidad);

                cellCodigo.BorderWidth = 0;
                cellCodigo.PaddingBottom = 5;
                cellCodigo.PaddingRight = 5;
                cellCodigo.PaddingLeft = 5;
                cellCodigo.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellCodigo);

                cellDescripcion.BorderWidth = 0;
                cellDescripcion.PaddingBottom = 5;
                cellDescripcion.PaddingRight = 5;
                cellDescripcion.PaddingLeft = 5;
                cellDescripcion.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellDescripcion);
            }

            float[] cellWidths = new float[] { 59, 64, 400 };

            table.SetWidths(cellWidths);
            table.TotalWidth = cellWidths.Sum();

            table.DefaultCell.Border = Rectangle.NO_BORDER;
            cb.SetFontAndSize(font, 10);
            table.WriteSelectedRows(0, ItemsDetalle.Count, -xInit + 22, -yInit + 555, cb);
        }

        private BaseColor GetColor(NFPDFColor color)
        {
            switch (color)
            {
                case NFPDFColor.BLACK:
                    return BaseColor.BLACK;
                case NFPDFColor.BLUE:
                    return BaseColor.BLUE;
                case NFPDFColor.CYAN:
                    return BaseColor.CYAN;
                case NFPDFColor.DARK_GRAY:
                    return BaseColor.DARK_GRAY;
                case NFPDFColor.GRAY:
                    return BaseColor.GRAY;
                case NFPDFColor.GREEN:
                    return BaseColor.GREEN;
                case NFPDFColor.LIGHT_GRAY:
                    return BaseColor.LIGHT_GRAY;
                case NFPDFColor.MAGENTA:
                    return BaseColor.MAGENTA;
                case NFPDFColor.ORANGE:
                    return BaseColor.ORANGE;
                case NFPDFColor.PINK:
                    return BaseColor.PINK;
                case NFPDFColor.RED:
                    return BaseColor.RED;
                case NFPDFColor.WHITE:
                    return BaseColor.WHITE;
                case NFPDFColor.YELLOW:
                    return BaseColor.YELLOW;
                default:
                    return BaseColor.BLACK;
            }
        }

        public void EscribirXY(string texto, int x, int y, int size, Alineado alineado)
        {
            EscribirXY(texto, x, y, size, alineado, NFPDFColor.BLACK);
        }

        public void EscribirXY(string texto, int x, int y, int size, Alineado alineado, NFPDFColor color)
        {
            int align = Align(alineado);

            cb.BeginText();
            cb.SetColorFill(GetColor(color));
            cb.SetFontAndSize(font, size);
            cb.ShowTextAligned(align, texto, x, document.PageSize.Height - y, 0);
            cb.EndText();
        }


        public void EscribirBoxXY(string texto, int x, int y, int size, float width)
        {
            string[] palabras = texto.Split(' ');
            string oracion = "";
            int i = 0;

            cb.BeginText();
            cb.SetFontAndSize(font, size);
            foreach (string palabra in palabras)
            {
                if (this.cb.GetEffectiveStringWidth(oracion + " " + palabra, false) <= width)
                    oracion = oracion + " " + palabra;
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, oracion.Trim(), x, document.PageSize.Height - (y + i), 0);
                    i += size;
                    oracion = palabra;
                }
            }

            if (oracion.Trim() != "")
                cb.ShowTextAligned(Element.ALIGN_LEFT, oracion.Trim(), x, document.PageSize.Height - (y + i), 0);

            cb.EndText();
        }

        public void InsertarImagenXY(iTextSharp.text.Image imagen, int x, int y)
        {
            InsertarImagenXY(imagen, x, y, 100);
        }

        public void InsertarImagenXY(iTextSharp.text.Image imagen, int x, int y, float porcentajeTamaño)
        {
            imagen.ScalePercent(porcentajeTamaño);
            imagen.SetAbsolutePosition(x, y);
            cb.AddImage(imagen);
            cb.Stroke();
        }

        public void InsertarImagenXY(Image imagen, int x, int y)
        {
            InsertarImagenXY(imagen, x, y, 100);
        }

        public void InsertarImagenXY(Image imagen, int x, int y, float porcentajeTamaño)
        {
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagen, ImageFormat.Jpeg);

            img.ScalePercent(porcentajeTamaño);
            img.SetAbsolutePosition(x, y);
            cb.AddImage(img);
            cb.Stroke();
        }

        public void InsertarImagenXYConTransparencia(string pathImage, int x, int y, float porcentajeTamaño)
        {
            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(pathImage);
            imagen.ScaleAbsolute(200f, 60f);

            //imagen.ScalePercent(porcentajeTamaño);
            imagen.SetAbsolutePosition(x, y);
            cb.AddImage(imagen);
            cb.Stroke();
        }

        /// <summary>
        /// Genera el PDF y lo graba en el disco
        /// </summary>
        /// <param name="Path">Ubicacion fisica donde se generara el PDF, por ejemplo "C:\TEMP"</param>
        public void GenerarPDFEnDisco(string Path)
        {
            FileStream outStream = File.OpenWrite(Path + @"\" + this.FileName);
            PrepararPDF().WriteTo(outStream);
            outStream.Flush();
            outStream.Close();
        }

        public void NuevaPagina()
        {
            document.NewPage();
        }

        /// <summary>
        /// Genera el PDF y lo envia al explorador como download "Guardar como...", debe usarse Helpers.AddPostbackEvent(btnReporte); para que fuerze un postback
        /// </summary>
        public void GenerarPDF()
        {
            MemoryStream mems;

            mems = PrepararPDF();

            Byte[] byteArray = mems.ToArray();
            mems.Flush();
            mems.Close();

            HttpContext.Current.Response.BufferOutput = true;
            // Clear all content output from the buffer stream
            HttpContext.Current.Response.Clear();

            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + this.FileName);

            // Set the HTTP MIME type of the output stream
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            // Write the data
            HttpContext.Current.Response.BinaryWrite(byteArray);
            HttpContext.Current.Response.End();
        }

        public Stream GenerarPDFStream()
        {
            return PrepararPDF();
        }

        private static bool EsNumerico(string cadena)
        {
            decimal numero;
            return decimal.TryParse(cadena, out numero);
        }

        #endregion
    }
}