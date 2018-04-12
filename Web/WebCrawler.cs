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
            output.Log(string.Format("Début {0} {1}", ln.name.ToUpper(), language.ToUpper()));
            UrlParameter urlParameter = ln.GetUrlParameter(language);
            int i = urlParameter.firstChapterNumber;
            PdfFile pdf =  new PdfFile(ln, language);
            EPubFile epub = new EPubFile(ln, language);
            string baseUrl = urlParameter.url;
            IParser parser = new ParserFactory().GetParser(baseUrl);

            if (parser == null)
            {
                output.Log("Aucun parser disponible pour cette URL");
                return;
            }

            List<LnChapter> lnChapters = new List<LnChapter>();

            while (urlParameter.lastChapterNumber <= -1 || i <= urlParameter.lastChapterNumber)
            {
                try
                {
                    var currentUrl = String.Format(baseUrl, i);
                    output.Progress(
                        string.Format("Récupération du chapitre {0}/{1}"
                            , i
                            , (urlParameter.lastChapterNumber > 0 
                                ? urlParameter.lastChapterNumber.ToString() 
                                : "?"))
                        );
                    HtmlDocument page = new HtmlDocument();
                    page.LoadHtml(
                        Encoding.UTF8.GetString(
                            new WebClient().DownloadData(
                                currentUrl)));

                    LnChapter lnChapter = parser.Parse(page);
                    if (lnChapter == null || lnChapter.paragraphs.Count == 0)
                    {
                        throw new NotExistingChapterException();
                    }

                    lnChapter.chapNumber = i;
                    lnChapters.Add(lnChapter);
                }
                catch (NotExistingChapterException)
                {
                    if (!Globale.INTERACTIVE_MODE ||
                            !input.Ask(
                                string.Format(
                                    "Le chapitre {0} ne semble pas exister. Voulez vous vérifier la présence du suivant ?",
                                    i)))
                        break;
                } catch (System.Exception e)
                {
                    throw new ParserException(string.Format("Erreur lors du traitement de la page {0} par le parser {1}", string.Format(baseUrl, i), parser.GetType()), e);
                }
                i++;
            }
            
            if (lnChapters.Count == 0)
            {
                output.Log("Aucun chapitre récupéré à cette URL");
                return;
            }

            pdf.AddChapters(lnChapters);
            epub.AddChapters(lnChapters);

            output.Log("Ouverture du PDF");
            pdf.SaveDocument();
            output.Log("Fermeture du PDF");

            output.Log("Creation de l'ePub");
            epub.SaveDocument();
            output.Log("Fin de creation de l'ePub");

            output.Log(string.Format("Fin {0} {1}", ln.name.ToUpper(), language.ToUpper()));
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
                        throw new CoverException(string.Format("Impossible de télécharger la cover {0}", urlCover));
                    }

                    throw;
                }
            }
        }
    }
}

