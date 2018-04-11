using System.Collections.Generic;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web
{
    public class LnChapter
    {
        public string title;
        public List<HtmlNode> paragraphs;
        public int chapNumber;

        public LnChapter(string title, List<HtmlNode> paragraphs)
        {
            this.title = title;
            this.paragraphs = paragraphs;
        }
    }
}
