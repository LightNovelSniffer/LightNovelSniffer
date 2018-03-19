using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using LightNovelSniffer.Config;
using LightNovelSniffer.Output;
using LightNovelSniffer.Web.Parser;

namespace LightNovelSniffer.Web
{
    internal static class WebCrawler
    {
        internal static void DownloadChapters(LnParameters ln, string language)
        {
            ConsoleTools.Log(string.Format("Début {0} {1}", ln.name.ToUpper(), language.ToUpper()), 3);
            UrlParameter urlParameter = ln.GetUrlParameter(language);
            int i = urlParameter.firstChapterNumber;
            PdfFile pdf =  new PdfFile(ln, language);
            EPubFile epub = new EPubFile(ln, language);
            string baseUrl = urlParameter.url;
            IParser parser = ParserFactory.GetParser(baseUrl);

            if (parser == null)
            {
                ConsoleTools.Log("Aucun parser disponible pour cette URL", 3);
                return;
            }

            List<LnChapter> lnChapters = new List<LnChapter>();

            while (urlParameter.lastChapterNumber <= -1 || i <= urlParameter.lastChapterNumber)
            {
                string currentUrl = "";
                try
                {
                    currentUrl = String.Format(baseUrl, i);
                    ConsoleTools.Progress(
                        string.Format("Récupération du chapitre {0}/{1}"
                            , i
                            , (urlParameter.lastChapterNumber > 0 
                                ? urlParameter.lastChapterNumber.ToString() 
                                : "?"))
                        , 3);
                    HtmlDocument page = new HtmlDocument();
                    page.LoadHtml(
                        Encoding.UTF8.GetString(
                            new WebClient().DownloadData(
                                currentUrl)));

                    LnChapter lnChapter = parser.Parse(page);

                    if (lnChapter == null || lnChapter.paragraphs.Count == 0)
                    {
                        if (Globale.INTERACTIVE_MODE &&
                            !ConsoleTools.Ask(
                                string.Format(
                                    "Le chapitre {0} ne semble pas exister. Voulez vous vérifier la présence du suivant ?",
                                    i)))
                            break;
                    }
                    else
                    {
                        lnChapter.chapNumber = i;
                        lnChapters.Add(lnChapter);
                    }
                }
                catch (WebException)
                {
                    ConsoleTools.Log(string.Format("Erreur lors du traitement de l'URL \"{0}\"", currentUrl), 3);
                    return;
                }
                i++;
            }
            
            if (lnChapters.Count == 0)
            {
                ConsoleTools.Log("Aucun chapitre récupéré à cette URL", 3);
                return;
            }

            pdf.AddChapters(lnChapters);
            epub.AddChapters(lnChapters);

            ConsoleTools.Log("Ouverture du PDF", 3);
            pdf.SaveDocument();
            ConsoleTools.Log("Fermeture du PDF", 3);

            ConsoleTools.Log("Creation de l'ePub", 3);
            epub.SaveDocument();
            ConsoleTools.Log("Fin de creation de l'ePub", 3);

            ConsoleTools.Log(string.Format("Fin {0} {1}", ln.name.ToUpper(), language.ToUpper()), 3);
        }

        internal static byte[] DownloadCover(string urlCover)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadData(new Uri(urlCover));
            }
        }
    }
}

