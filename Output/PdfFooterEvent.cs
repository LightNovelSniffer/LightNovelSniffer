using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LightNovelSniffer.Output
{
    internal class PdfFooterEvent : PdfPageEventHelper
    {
        private readonly string title;

        public PdfFooterEvent(string chapterTitle)
        {
            title = chapterTitle;
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            base.OnEndPage(writer, doc);
            BaseColor grey = new BaseColor(128, 128, 128);
            Font font = FontFactory.GetFont("Arial", 9, Font.NORMAL, grey);
            //tbl footer
            PdfPTable footerTbl = new PdfPTable(1);
            footerTbl.TotalWidth = doc.PageSize.Width;

            //numero de la page
            Chunk myFooter = new Chunk(title, FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, grey));
            PdfPCell footer = new PdfPCell(new Phrase(myFooter))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            footerTbl.AddCell(footer);

            footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin), writer.DirectContent);
        }
    }
}
