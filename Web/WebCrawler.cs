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
    internal class WebCrawler
    {
        private IOutput output;
        private IInput input;

        internal WebCrawler(IOutput output, IInput input)
        {
            this.output = output;
            this.input = input;
        }

        internal List<LnChapter> DownloadChapters(UrlParameter urlParameter)
        {
            string baseUrl = urlParameter.url;
            int i = urlParameter.firstChapterNumber;
            List<LnChapter> lnChapters = new List<LnChapter>();
            IParser parser = new ParserFactory().GetParser(baseUrl);

            if (parser == null)
            {
                output.Log(LightNovelSniffer_Strings.LogNoParserAvailable);
                return lnChapters;
            }
            
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
                    if (lnChapter.title == null)
                    {
                        lnChapter.title = string.Format(Globale.DEFAULT_CHAPTER_TITLE, lnChapter.chapNumber.ToString().PadLeft(3, '0'));
                    }

                    lnChapter.chapNumber = i;
                    lnChapters.Add(lnChapter);
                    chapterOnErrorCountBeforeStop = 0;
                }
                catch (NotExistingChapterException)
                {
                    if (!Globale.INTERACTIVE_MODE)
                        output.Log(string.Format(LightNovelSniffer_Strings.LogNotExistingChapter, i));
                    chapterOnErrorCountBeforeStop++;
                    if (    (!Globale.INTERACTIVE_MODE && chapterOnErrorCountBeforeStop > Globale.MAX_CHAPTER_ON_ERROR_COUNT_BEFORE_STOP) 
                            || (Globale.INTERACTIVE_MODE && !input.Ask(string.Format(LightNovelSniffer_Strings.LogChapterDoesntExist_AskForNext,i))))
                        break;
                } catch (System.Exception e)
                {
                    if (!Globale.INTERACTIVE_MODE)
                        output.Log(string.Format(LightNovelSniffer_Strings.LogErrorProcessingUrlByParser, string.Format(i + " (" + baseUrl + ")", i), parser.GetType()));
                    chapterOnErrorCountBeforeStop++;
                    if (    (!Globale.INTERACTIVE_MODE && chapterOnErrorCountBeforeStop > Globale.MAX_CHAPTER_ON_ERROR_COUNT_BEFORE_STOP)
                            || (Globale.INTERACTIVE_MODE && !input.Ask(string.Format(LightNovelSniffer_Strings.LogChapterParserException_AskForNext, string.Format(i + " ("+baseUrl+")", i)))))
                        break;
                }
                i++;
            }
            
            if (lnChapters.Count == 0)
            {
                output.Log(LightNovelSniffer_Strings.LogNoChapterAvailableAtThisUrl);
                return lnChapters;
            }

            return lnChapters;
        }
    }
}

