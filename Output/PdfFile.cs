using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LightNovelSniffer.Config;
using LightNovelSniffer.Web;
using PdfChapter = iTextSharp.text.Chapter;
using PdfParagraph = iTextSharp.text.Paragraph;
using PdfDocument = iTextSharp.text.Document;
using PdfImage = iTextSharp.text.Image;

namespace LightNovelSniffer.Output
{
    internal sealed class PdfFile : OutputFile
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
            pdf.AddAuthor(Globale.AUTHOR);
            pdf.AddCreationDate();
            pdf.AddLanguage(language);
            pdf.AddTitle(DocumentTitle);
            pdf.AddCreator(Globale.AUTHOR);
            pdfChapters = new List<Chapter>();
        }

        public override void AddChapter(LnChapter lnChapter)
        {
            if (lnChapter.title == null)
            {
                lnChapter.title = string.Format(Globale.DEFAULT_CHAPTER_TITLE, lnChapter.chapNumber.ToString().PadLeft(3, '0'));
            }

            PdfChapter pdfChapter = new PdfChapter(lnChapter.title, lnChapter.chapNumber);

            foreach (string paragraph in lnChapter.paragraphs.ParseHtmlNodeToStringList())
            {
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
            PdfWriter wri = PdfWriter.GetInstance(pdf, File.Create(Path.Combine(OutputFolder, FileName + ".pdf")));
            
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
            PdfImage pic = PdfImage.GetInstance(lnParameters.urlCover);

            if (pic.Height > pic.Width)
            {
                //Maximum height is 800 pixels.
                float percentage = 0.0f;
                percentage = 700 / pic.Height;
                pic.ScalePercent(percentage * 100);
            }
            else
            {
                //Maximum width is 600 pixels.
                float percentage = 0.0f;
                percentage = 540 / pic.Width;
                pic.ScalePercent(percentage * 100);
            }

            pic.Border = Rectangle.BOX;
            pic.BorderColor = BaseColor.BLACK;
            pic.BorderWidth = 3f;
            pdf.NewPage();
            pdf.Add(pic);
        }
    }
}
