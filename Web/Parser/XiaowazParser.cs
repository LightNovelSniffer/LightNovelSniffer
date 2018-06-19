using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    public class XiaowazParser : IParser
    {
        public bool CanParse(string url)
        {
            return url.ContainsInvarient("xiaowaz.fr");
        }

        public LnChapter Parse(HtmlDocument doc)
        {
            List<LnNode> paragraphs = doc
                .DocumentNode
                .SelectSingleNode("//body")
                .SelectNodes("//div")
                .First(d =>
                    d.Id == "content")
                .SelectNodes("//div")
                .Skip(1)
                .First()
                .SelectSingleNode("//div")
                .SelectNodes("//p")
                .Skip(6)
                .Where(b => b.Name != "#text")
                .TakeWhile(p => 
                    p.Attributes["class"] == null ||
                    !p.Attributes["class"].Value.Equals("nav-previous"))
                .ToLnNodeList();

            if (paragraphs.Count == 0)
                return null;

            return new LnChapter(null, paragraphs);
        }
    }
}