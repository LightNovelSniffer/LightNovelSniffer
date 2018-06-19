using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    public class WuxiaworldParser : IParser
    {
        public bool CanParse(string url)
        {
            return url.ContainsInvarient("wuxiaworld.com");
        }

        public LnChapter Parse(HtmlDocument doc)
        {
            HtmlNode node = doc
                .DocumentNode
                .SelectSingleNode("//body")
                .SelectNodes("//div")
                .First(d =>
                    d.Attributes["class"] != null &&
                    d.Attributes["class"].Value.Equals("fr-view"));

            if (node == null)
                return null;

            ICollection<LnNode> paragraphs = node
                .ChildNodes
                .Where(b => b.Name != "#text")
                .ToLnNodeList();

            paragraphs = paragraphs.Take(paragraphs.Count - 3).ToList();

            if (paragraphs.Count == 0)
                return null;

            return new LnChapter(paragraphs.First().InnerText.DecodeHtml(), paragraphs.Skip(1).ToList());
        }
    }
}