using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using LightNovelSniffer.Config;

namespace LightNovelSniffer
{
    internal static class ConsoleTools
    {
        private static bool isProgressOngoing = false;

        private static void CheckProgress()
        {
            if (isProgressOngoing)
            {
                Console.WriteLine("");
                isProgressOngoing = false;
            }
        }

        private static void OutputString(string str)
        {
            Console.Write(DateTime.Now.ToString("HH:mm:ss") + " : " + str);
        }

        private static string Indent(string text, int tab)
        {
            while (tab > 1)
            {
                text = "  " + text;
                tab--;
            }

            return text;
        }

        internal static void Log(string text, int tab)
        {
            CheckProgress();
            OutputString(Indent(text, tab) + "\r\n");
        }

        internal static void Progress(string text, int tab)
        {
            isProgressOngoing = true;
            Console.Write("\r" + DateTime.Now.ToString("HH:mm:ss") + " : " + Indent(text, tab));
        }

        internal static bool Ask(string question)
        {
            string input = AskInformation(question + " O/n ");
            return input != null && !input.ToUpper().Equals("N");
        }

        internal static string AskInformation(string question)
        {
            if (!Globale.INTERACTIVE_MODE)
                return "";
            CheckProgress();
            OutputString(question);
            return Console.ReadLine();
        }

        internal static string AskUrl(string question)
        {
            string url = AskInformation(question);

            if (string.IsNullOrEmpty(url))
                return null;
            
            url = url.TrimEnd('/');
            
            while (Regex.Match(url, @"\d$").Success)
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url + "{0}";
        }
    }
}
