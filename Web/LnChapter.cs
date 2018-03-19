using System.Collections.Generic;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web
{
    internal class LnChapter
    {
        internal string title;
        internal List<HtmlNode> paragraphs;
        internal int chapNumber;

        internal LnChapter(string title, List<HtmlNode> paragraphs)
        {
            this.title = title;
            this.paragraphs = paragraphs;
        }
    }
}
