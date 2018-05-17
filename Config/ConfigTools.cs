using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Globalization;
using System.Resources;
using System.util;
using LightNovelSniffer.Exception;
using LightNovelSniffer.Resources;

namespace LightNovelSniffer.Config
{
    public static class ConfigTools
    {
        public static ICollection<CultureInfo> GetAvailableLanguage()
        {
            List<CultureInfo> res = new List<CultureInfo>();

            ResourceManager rm = new ResourceManager(typeof(LightNovelSniffer_Strings));

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (culture.ThreeLetterWindowsLanguageName.Equals("IVL"))
                    continue;
                try
                {
                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs!=null)
                        res.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                }
            }

            CultureInfo en = CultureInfo.GetCultureInfo("en");

            if (!res.Contains(en))
                res.Add(en);

            return res;
        }

        public static CultureInfo GetCurrentLanguage()
        {
            return (CultureInfo) CultureInfo.CurrentUICulture.Clone();
        }

        public static void SetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new LanguageException(LightNovelSniffer_Strings.EmptyLanguageExceptionMessage);

            try
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(language);
                SetLanguage(cultureInfo);
            }
            catch (CultureNotFoundException)
            {
                throw new LanguageException(string.Format(LightNovelSniffer_Strings.GetCultureFromLanguageExceptionMessage, language));
            }
        }

        public static void SetLanguage(CultureInfo language)
        {
            if (language == null)
                throw new LanguageException(LightNovelSniffer_Strings.CultureInfoNullExceptionMessage);

            try
            {
                CultureInfo.DefaultThreadCurrentUICulture = language;
            }
            catch (System.Exception)
            {
                throw new LanguageException(string.Format(LightNovelSniffer_Strings.CultureInfoExceptionMessage, language));
            }
        }

        public static void InitLightNovels(CultureInfo language = null)
        {
            if (language != null)
                SetLanguage(language);

            string path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            XmlDocument lnXml = new XmlDocument();
            lnXml.Load(path + "\\LightNovels.xml");

            if (lnXml.DocumentElement == null)
                throw new ApplicationException(LightNovelSniffer_Strings.UnableToReadLightNovelsFileExceptionMessage);

            Globale.LN_TO_RETRIEVE = new List<LnParameters>();

            XmlNodeList lnNodes = lnXml.DocumentElement.SelectNodes("ln");
            if (lnNodes != null)
            {
                foreach (XmlNode ln in lnNodes)
                {
                    LnParameters lnParameter = new LnParameters();

                    XmlNode nameNode = ln.SelectSingleNode("name");
                    XmlNode coverNode = ln.SelectSingleNode("urlCover");
                    XmlNode authorsListNode = ln.SelectSingleNode("authors");
                    XmlNode versionsNode = ln.SelectSingleNode("versions");

                    if (nameNode != null)
                        lnParameter.name = nameNode.InnerText;
                    if (coverNode != null)
                        lnParameter.urlCover = coverNode.InnerText;

                    if (authorsListNode != null)
                    {
                        XmlNodeList authorNodes = authorsListNode.SelectNodes("author");
                        if (authorNodes != null)
                        {
                            foreach (XmlNode authorNode in authorNodes)
                                lnParameter.authors.Add(authorNode.InnerText);
                        }
                    }


                    if (versionsNode != null)
                    {
                        foreach (XmlNode version in versionsNode.SelectNodes("version"))
                        {
                            UrlParameter up = new UrlParameter();

                            XmlNode urlNode = version.SelectSingleNode("url");
                            XmlNode languageNode = version.SelectSingleNode("language");
                            XmlNode fcNode = version.SelectSingleNode("firstChapterNumber");
                            XmlNode lcNode = version.SelectSingleNode("lastChapterNumber");

                            if (urlNode != null)
                                up.url = urlNode.InnerText;

                            if (languageNode != null)
                                up.language = languageNode.InnerText;

                            if (fcNode != null)
                            {
                                if (!int.TryParse(fcNode.InnerText, out up.firstChapterNumber))
                                    up.firstChapterNumber = 1;
                            }

                            if (lcNode != null)
                            {
                                if (!int.TryParse(lcNode.InnerText, out up.lastChapterNumber))
                                    up.lastChapterNumber = -1;
                            }

                            lnParameter.urlParameters.Add(up);
                        }
                    }

                    Globale.LN_TO_RETRIEVE.Add(lnParameter);
                }
            }
        }

        public static void InitConf(CultureInfo language = null)
        {
            if (language != null)
                SetLanguage(language);

            string path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            XmlDocument configXml = new XmlDocument();
            configXml.Load(path + "\\Config.xml");

            if (configXml.DocumentElement == null)
                throw new ApplicationException(LightNovelSniffer_Strings.UnableToReadConfigFileExceptionMessage);

            XmlNode ofNode = configXml.DocumentElement.SelectSingleNode("outputFolder");
            XmlNode imNode = configXml.DocumentElement.SelectSingleNode("interactiveMode");
            XmlNode publisherNode = configXml.DocumentElement.SelectSingleNode("publisher");
            XmlNode dctNode = configXml.DocumentElement.SelectSingleNode("defaultChapterTitle");
            
            if (ofNode != null)
                Globale.OUTPUT_FOLDER = ofNode.InnerText;

            if (imNode != null)
            {
                if (!bool.TryParse(imNode.InnerText, out Globale.INTERACTIVE_MODE))
                    Globale.INTERACTIVE_MODE = true;
            }

            if (publisherNode != null)
                Globale.PUBLISHER = publisherNode.InnerText;

            if (dctNode != null)
                Globale.DEFAULT_CHAPTER_TITLE = dctNode.InnerText;
        }
    }
}