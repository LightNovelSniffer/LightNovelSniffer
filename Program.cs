using System.Collections.Generic;
using System.Linq;
using LightNovelSniffer.Config;
using LightNovelSniffer.Libs;
using LightNovelSniffer.Output;
using LightNovelSniffer.Resources;
using LightNovelSniffer.Web;

namespace LightNovelSniffer
{
    public class Program
    {
        private IOutput output;
        private IInput input;
        private WebCrawler webCrawler;

        public Program(IOutput output, IInput input)
        {
            this.output = output;
            this.input = input;
            this.webCrawler = new WebCrawler(output, input);
        }

        public void ProcessLightNovel(LnParameters ln, string languageToProcess)
        {
            output.Log(string.Format(LightNovelSniffer_Strings.LogBeginLnLanguage, ln.name.ToUpper(), languageToProcess.ToUpper()));

            UrlParameter urlParameter = ln.GetUrlParameter(languageToProcess);

            PdfFile pdf = new PdfFile(ln, languageToProcess);
            EPubFile epub = new EPubFile(ln, languageToProcess);
            JsonFile json = new JsonFile(ln, languageToProcess);

            if (Globale.RESUME_EXISTING_FILE)
            {
                JsonFile jsonFile = JsonFileFactory.InitiateFromFile(json.OutputFullPath());
                if (jsonFile != null)
                {
                    json.chapters = jsonFile.chapters;
                    urlParameter.firstChapterNumber = json.chapters.Last().chapNumber + 1;
                }
            }
            
            List<LnChapter> downloadedLnChapters = webCrawler.DownloadChapters(urlParameter);

            if (downloadedLnChapters.Count > 0)
            {
                json.AddChapters(downloadedLnChapters);
                output.Log(LightNovelSniffer_Strings.LogOpeningJsonFile);
                json.SaveDocument();
                output.Log(LightNovelSniffer_Strings.LogClosingJsonFile);
                json.Close();

                pdf.AddChapters(json.chapters);
                output.Log(LightNovelSniffer_Strings.LogOpeningPdfFile);
                pdf.SaveDocument();
                output.Log(LightNovelSniffer_Strings.LogClosingPdfFile);
                pdf.Close();

                epub.AddChapters(json.chapters);
                output.Log(LightNovelSniffer_Strings.LogOpeningEpubFile);
                epub.SaveDocument();
                output.Log(LightNovelSniffer_Strings.LogClosingEpubFile);
                epub.Close();
            }
            output.Log(string.Format(LightNovelSniffer_Strings.LogEndLnLanguage, ln.name.ToUpper(), languageToProcess.ToUpper()));
        }
    }
}
