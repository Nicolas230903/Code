using System;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ACHE.FacturaElectronica.Lib
{
    internal class MyPageEvents : PdfPageEventHelper
    {
        // we will keep a list of speakers
        private SortedList speakers = new SortedList();

        // This is the contentbyte object of the writer
        private PdfContentByte cb;

        // we will put the final number of pages in a template
        private PdfTemplate template;

        // this is the BaseFont we are going to use for the header / footer
        private BaseFont bf;


        // we override the onOpenDocument method


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }


        // we override the onEndPage method
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            int pageN = writer.PageNumber;
            float w;
            String text = "Página " + pageN + " de ";
            float len = bf.GetWidthPoint(text, 8);

            if (document.PageSize.Width > document.PageSize.Height)
                w = PageSize.A4.Rotate().Width/2 - 15;
            else
                w = PageSize.A4.Width/2 - 15;

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(w, 20);
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, w + len, 20);
        }

        // we override the onCloseDocument method
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.ShowText((writer.PageNumber - 1).ToString());
            template.EndText();
        }
    }
}