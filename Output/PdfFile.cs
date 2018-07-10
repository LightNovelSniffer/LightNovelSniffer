using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LightNovelSniffer.Config;
using LightNovelSniffer.Exception;
using LightNovelSniffer.Web;
using PdfChapter = iTextSharp.text.Chapter;
using PdfParagraph = iTextSharp.text.Paragraph;
using PdfDocument = iTextSharp.text.Document;
using PdfImage = iTextSharp.text.Image;

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

            footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin - 5), writer.DirectContent);
        }
    }

    public sealed class PdfFile : OutputFile
    {
        private PdfDocument pdf;

        private List<PdfChapter> pdfChapters;

        public PdfFile(LnParameters lnParam, string language)
        {
            InitiateDocument(lnParam, language);
        }

        protected override void InitiateDocument(LnParameters lnParam, string language)
        {
            this.lnParameters = lnParam;
            this.currentLanguage = language;

            pdf = new PdfDocument(PageSize.A4);
            foreach (string author in lnParam.authors)
                pdf.AddAuthor(author);
            pdf.AddCreationDate();
            pdf.AddLanguage(language);
            pdf.AddTitle(DocumentTitle);
            pdf.AddCreator(Globale.PUBLISHER);
            pdfChapters = new List<Chapter>();
        }

        public override void AddChapter(LnChapter lnChapter)
        {
            PdfChapter pdfChapter = new PdfChapter(lnChapter.title, lnChapter.chapNumber);

            foreach (LnNode paragraphNode in lnChapter.paragraphs)
            {
                string paragraph = paragraphNode.InnerText.DecodeHtml();
                pdfChapter.Add(
                    new PdfParagraph(paragraph)
                    {
                        Alignment = Element.ALIGN_JUSTIFIED,
                        SpacingBefore = 15
                    });
            }

            pdfChapters.Add(pdfChapter);
        }

        public override void SaveDocument()
        {
            base.SaveDocument();
            PdfWriter wri = PdfWriter.GetInstance(pdf, File.Create(OutputFullPath()));

            pdf.Open();

            if (!string.IsNullOrEmpty(lnParameters.urlCover))
                AddCover();

            foreach (PdfChapter pdfChapter in pdfChapters)
            {
                pdf.NewPage();
                wri.PageEvent = null;
                wri.PageEvent = new PdfFooterEvent(pdfChapter.Title.Content);
                pdf.Add(pdfChapter);
            }

            pdf.Close();
            pdf.Dispose();
        }

        private void AddCover()
        {
            try
            {
                byte[] cover = WebTools.DownloadCover(lnParameters.urlCover);

                PdfImage pic = PdfImage.GetInstance(cover);

                if (pic.Height > pic.Width)
                {
                    //Maximum height is 800 pixels.
                    float percentage = 0.0f;
                    percentage = 700/pic.Height;
                    pic.ScalePercent(percentage*100);
                }
                else
                {
                    //Maximum width is 600 pixels.
                    float percentage = 0.0f;
                    percentage = 540/pic.Width;
                    pic.ScalePercent(percentage*100);
                }

                pic.Border = Rectangle.BOX;
                pic.BorderColor = BaseColor.BLACK;
                pic.BorderWidth = 3f;
                pdf.NewPage();
                pdf.Add(pic);
            }
            catch (CoverException)
            {
            }
        }

        public override string OutputFullPath()
        {
            return Path.Combine(OutputFolder, FileName + ".pdf");
        }

        public override void Close()
        {
            base.Close();
            this.pdf.Close();
            this.pdf.Dispose();
            this.pdf = null;
            this.pdfChapters = null;
        }
    }
}