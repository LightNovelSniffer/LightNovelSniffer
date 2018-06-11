using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using LightNovelSniffer.Config;
using LightNovelSniffer.Exception;
using LightNovelSniffer.Libs;
using LightNovelSniffer.Output;
using LightNovelSniffer.Web.Parser;
using LightNovelSniffer.Resources;

namespace LightNovelSniffer.Web
{
    public class WebCrawler
    {
        private IOutput output;
        private IInput input;

        public WebCrawler(IOutput output, IInput input)
        {
            this.output = output;
            this.input = input;
        }

        public void DownloadChapters(LnParameters ln, string language)
        {
            output.Log(string.Format(LightNovelSniffer_Strings.LogBeginLnLanguage, ln.name.ToUpper(), language.ToUpper()));
            UrlParameter urlParameter = ln.GetUrlParameter(language);
            int i = urlParameter.firstChapterNumber;
            PdfFile pdf =  new PdfFile(ln, language);
            EPubFile epub = new EPubFile(ln, language);
            string baseUrl = urlParameter.url;
            IParser parser = new ParserFactory().GetParser(baseUrl);

            if (parser == null)
            {
                output.Log(LightNovelSniffer_Strings.LogNoParserAvailable);
                return;
            }

            List<LnChapter> lnChapters = new List<LnChapter>();

            int chapterOnErrorCountBeforeStop = 0;

            while (urlParameter.lastChapterNumber <= -1 || i <= urlParameter.lastChapterNumber)
            {
                try
                {
                    var currentUrl = String.Format(baseUrl, i);
                    output.Progress(
                        string.Format(LightNovelSniffer_Strings.LogRetrieveChapterProgress
                            , i
                            , (urlParameter.lastChapterNumber > 0 
                                ? urlParameter.lastChapterNumber.ToString() 
                                : "?"))
                        );

                    byte[] pageContent = null;

                    try
                    {
                        pageContent = new WebClient().DownloadData(currentUrl);
                    }
                    catch (WebException e)
                    {
                        if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
                            throw new NotExistingChapterException();
                    }

                    HtmlDocument page = new HtmlDocument();
                    page.LoadHtml(Encoding.UTF8.GetString(pageContent));

                    LnChapter lnChapter = parser.Parse(page);
                    if (lnChapter == null || lnChapter.paragraphs.Count == 0)
                    {
                        throw new NotExistingChapterException();
                    }

                    lnChapter.chapNumber = i;
                    lnChapters.Add(lnChapter);
                    chapterOnErrorCountBeforeStop = 0;
                }
                catch (NotExistingChapterException)
                {
                    if (!Globale.INTERACTIVE_MODE)
                        output.Log(LightNovelSniffer_Strings.LogNotExistingChapter);
                    chapterOnErrorCountBeforeStop++;
                    if (    (!Globale.INTERACTIVE_MODE && chapterOnErrorCountBeforeStop > Globale.MAX_CHAPTER_ON_ERROR_COUNT_BEFORE_STOP) 
                            || (Globale.INTERACTIVE_MODE && !input.Ask(string.Format(LightNovelSniffer_Strings.LogChapterDoesntExist_AskForNext,i))))
                        break;
                } catch (System.Exception e)
                {
                    if (!Globale.INTERACTIVE_MODE)
                        output.Log(string.Format(LightNovelSniffer_Strings.LogErrorProcessingUrlByParser, string.Format(baseUrl, i), parser.GetType()));
                    chapterOnErrorCountBeforeStop++;
                    if (    (!Globale.INTERACTIVE_MODE && chapterOnErrorCountBeforeStop > Globale.MAX_CHAPTER_ON_ERROR_COUNT_BEFORE_STOP)
                            || (Globale.INTERACTIVE_MODE && !input.Ask(string.Format(LightNovelSniffer_Strings.LogChapterParserException_AskForNext, i))))
                        break;
                }
                i++;
            }
            
            if (lnChapters.Count == 0)
            {
                output.Log(LightNovelSniffer_Strings.LogNoChapterAvailableAtThisUrl);
                return;
            }

            pdf.AddChapters(lnChapters);
            epub.AddChapters(lnChapters);

            output.Log(LightNovelSniffer_Strings.LogOpeningPdfFile);
            pdf.SaveDocument();
            output.Log(LightNovelSniffer_Strings.LogClosingPdfFile);

            output.Log(LightNovelSniffer_Strings.LogOpeningEpubFile);
            epub.SaveDocument();
            output.Log(LightNovelSniffer_Strings.LogClosingEpubFile);

            output.Log(string.Format(LightNovelSniffer_Strings.LogEndLnLanguage, ln.name.ToUpper(), language.ToUpper()));
        }

        public static byte[] DownloadCover(string urlCover)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadData(new Uri(urlCover));
                }
                catch (System.Exception ex)
                {
                    if (ex is UriFormatException || ex is WebException)
                    {
                        throw new CoverException(string.Format(LightNovelSniffer_Strings.CoverDownloadExceptionMessage, urlCover));
                    }

                    throw;
                }
            }
        }
    }
}

