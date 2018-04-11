using System.Collections.Generic;
using System.IO;
using LightNovelSniffer.Config;
using LightNovelSniffer.Web;

namespace LightNovelSniffer.Output
{
    public abstract class OutputFile
    {
        protected LnParameters lnParameters;
        protected string currentLanguage;

        protected abstract void InitiateDocument(LnParameters ln, string language);
        public abstract void AddChapter(LnChapter lnChapter);

        public void AddChapters(ICollection<LnChapter> lnChapters)
        {
            foreach (LnChapter chapter in lnChapters)
                AddChapter(chapter);
        }

        public virtual void SaveDocument()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(OutputFolder, FileName)));
        }

        public string DocumentTitle
        {
            get { return lnParameters.name + " - " + currentLanguage; }
        }

        public string OutputFolder
        {
            get { return Path.Combine(Globale.OUTPUT_FOLDER, lnParameters.name, currentLanguage); }
        }

        public string FileName
        {
            get { return lnParameters.name + " " + currentLanguage; }
        }
    }
}
