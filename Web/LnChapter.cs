using System.Collections.Generic;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web
{
    public class LnNode
    {
        public string OuterHtml;
        public string InnerHtml;
        public string InnerText;

        public LnNode(string OuterHtml, string InnerHtml, string InnerText)
        {
            this.OuterHtml = OuterHtml.GetCleanedHtml();
            this.InnerHtml = InnerHtml;
            this.InnerText = InnerText;
        }
    }

    public class LnChapter
    {
        public string title;
        public List<LnNode> paragraphs;
        public int chapNumber;

        public LnChapter(string title, List<LnNode> paragraphs)
        {
            this.title = title;
            this.paragraphs = paragraphs;
        }
    }
}
