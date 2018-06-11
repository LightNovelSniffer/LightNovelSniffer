using System.IO;
using System.Linq;
using LightNovelSniffer.Config;
using LightNovelSniffer.Exception;
using LightNovelSniffer.Web;
using Document = LightNovelSniffer.Libs.DotNetEpub.Document;

namespace LightNovelSniffer.Output
{
    public sealed class EPubFile : OutputFile
    {
        private Document epub;
        private int nbChapInEpub;

        public EPubFile(LnParameters lnParam, string language)
        {
            InitiateDocument(lnParam, language);
        }

        protected override void InitiateDocument(LnParameters lnParam, string language)
        {
            this.lnParameters = lnParam;
            this.currentLanguage = language;
            this.nbChapInEpub = 1;

            epub = new Document();
            foreach (string author in lnParam.authors)
                epub.AddAuthor(author);
            epub.AddPublisher(Globale.PUBLISHER);
            epub.AddLanguage(language);
            epub.AddTitle(DocumentTitle);
            
            if (!string.IsNullOrEmpty(lnParam.urlCover))
            {
                AddCover();
            }
        }

        private void AddCover()
        {
            try
            {
                byte[] cover = WebCrawler.DownloadCover(lnParameters.urlCover);
                if (cover != null && cover.Length > 0)
                {
                    string coverFilename = "cover." + lnParameters.urlCover.Split('.').Last();

                    epub.AddImageData(coverFilename, cover);
                    epub.AddMetaItem("cover", coverFilename);
                    epub.AddImageData("." + coverFilename, cover);
                    epub.AddMetaItem(".cover", coverFilename);
                }
            } catch (CoverException e)
            {}
        }

        public override void AddChapter(LnChapter lnChapter)
        {
            lnChapter.title = string.IsNullOrEmpty(lnChapter.title)
                ? string.Format(Globale.DEFAULT_CHAPTER_TITLE, lnChapter.chapNumber)
                : lnChapter.title;

            string content = 
                GetHeader(lnParameters.name, lnChapter.title) 
                + string.Join("\r\n", lnChapter.paragraphs.Select(p => p.OuterHtml.Replace("<br>", "<br/>").Replace("<hr>", "<hr/>"))) 
                + GetFooter();
            string chapFilename = "chap" + nbChapInEpub + ".html";
            
            epub.AddXhtmlData(chapFilename, content);
            epub.AddNavPoint(lnChapter.title, chapFilename, nbChapInEpub);
            
            nbChapInEpub++;
        }

        public override void SaveDocument()
        {
            base.SaveDocument();
            epub.Generate(Path.Combine(OutputFolder, FileName + ".epub"));
        }

        private string GetHeader(string lnTitle, string chapterTitle)
        {
            return 
@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<title>" + lnTitle + @"</title>
<meta http-equiv=""Content-Type"" content=""application/xhtml+xml; charset=utf-8"" />
<meta name=""EPB-UUID"" content="""" />
<style type=""text/css"">p { padding-top: 10px; }</style>
</head>
<body>
<h2>" + chapterTitle + "</h2>" +
"";
        }

        private string GetFooter()
        {
            return @"</body></html>";
        }
    }
}