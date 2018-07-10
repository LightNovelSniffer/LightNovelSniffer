using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using LightNovelSniffer.Config;
using LightNovelSniffer.Libs;
using LightNovelSniffer.Web;

namespace LightNovelSniffer.Output
{
    public sealed class JsonFile : OutputFile
    {
        public List<LnChapter> chapters = new List<LnChapter>();

        public JsonFile(LnParameters ln, string language)
        {
            InitiateDocument(ln, language);
        }

        protected override void InitiateDocument(LnParameters ln, string language)
        {
            lnParameters = ln;
            currentLanguage = language;
        }

        public override void AddChapter(LnChapter chapter)
        {
            chapters.Add(chapter);
        }


        public override void SaveDocument()
        {
            base.SaveDocument();
            File.WriteAllText(
                OutputFullPath(),
                JsonTools.Serialize(this));
        }
        
        public override string OutputFullPath()
        {
            return Path.Combine(OutputFolder, FileName + ".json");
        }

        public override void Close()
        {
            base.Close();
        }
    }
}