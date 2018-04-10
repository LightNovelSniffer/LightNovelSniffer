using System;
using LightNovelSniffer.Config;
using LightNovelSniffer.Web;

namespace LightNovelSniffer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ConfigTools.InitConf();
            }
            catch (ApplicationException e)
            {
                ConsoleTools.Log(e.Message, 1);
                return;
            }
            
            if (!ConsoleTools.Ask("Utiliser le dossier de sortie \"" + Globale.OUTPUT_FOLDER + "\" ?"))
            {
                string folder = ConsoleTools.AskInformation("Merci de saisir le chemin du dossier racine à utiliser : ");
                if (!string.IsNullOrEmpty(folder))
                    Globale.OUTPUT_FOLDER = folder;
            }
            
            ConsoleTools.Log("Début du programme", 1);

            foreach (LnParameters ln in Globale.LN_TO_RETRIEVE)
            {
                if (ConsoleTools.Ask(string.Format("Voulez-vous récupérer {0} ?", ln.name.ToUpper())))
                    GetNovel(ln);
            }

            if (Globale.INTERACTIVE_MODE && ConsoleTools.AskNegative("Fin du traitement des Light Novel prédéfinies. Voulez vous en traiter d'autres ?"))
            {
                LnParameters ln;
                do
                {
                    ln = BuildDynamicLn();
                    GetNovel(ln);
                } while (!string.IsNullOrEmpty(ln.name));

            }

            ConsoleTools.Log("Fin du programme", 1);
            if (Globale.INTERACTIVE_MODE)
                Console.ReadLine();
        }

        private static void GetNovel(LnParameters ln)
        {
            foreach (UrlParameter up in ln.urlParameters)
            {
                if (ConsoleTools.Ask(string.Format("Voulez-vous récupérer la version {0} ?", up.language)))
                {
                    if (string.IsNullOrEmpty(up.url))
                        up.url = ConsoleTools.AskUrl(string.Format("Aucune URL renseignée pour {0} dans cette langue. Merci de saisir l'url d'un chapitre ici: ", ln.name));

                    if (!string.IsNullOrEmpty(up.url))
                        WebCrawler.DownloadChapters(ln, up.language);
                    else
                        ConsoleTools.Log(string.Format("Pas d'URL pour la version {0}. Traitement arrêté", up.language), 1);
                }
            }
        }

        private static LnParameters BuildDynamicLn()
        {
            LnParameters ln = new LnParameters();

            ln.name = ConsoleTools.AskInformation("Nom de la LN (vide pour arrêter) : ");
            if (string.IsNullOrEmpty(ln.name))
                return ln;
            ln.urlCover = ConsoleTools.AskInformation("URL de l'image de cover : ");

            do
            {
                UrlParameter up = new UrlParameter();

                up.language = ConsoleTools.AskInformation("Entrez la langue de cette version (FR/EN/...) : ");
                up.url = ConsoleTools.AskInformation("Entrez l'url d'un des chapitres : ");
                int.TryParse(ConsoleTools.AskInformation("A quel chapitre voulez vous commencer (si vide, 1) : "), out up.firstChapterNumber);
                int.TryParse(ConsoleTools.AskInformation("A quel chapitre voulez vous vous arrêter (si vide, jusqu'au dernier paru) : "), out up.lastChapterNumber);

                if (!string.IsNullOrEmpty(up.language) && !string.IsNullOrEmpty(up.url))
                    ln.urlParameters.Add(up);
                else
                    ConsoleTools.Log("Pas d'URL ou de language pour cette version. Ajout impossible", 1);
            } while (ConsoleTools.Ask("Voulez vous ajouter une autre version pour cette LN ?"));

            return ln;
        }
    }
}
