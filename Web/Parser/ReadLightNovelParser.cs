using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    public class ReadLightNovelParser : IParser
    {
        public LnChapter Parse(HtmlDocument doc)
        {
            HtmlNode node = doc
                .DocumentNode
                .SelectSingleNode("//body")
                .SelectNodes("//div")
                .First(d =>
                    d.Attributes["class"] != null &&
                    d.Attributes["class"].Value.Equals("desc"));

            if (node == null)
                return null;

            List<HtmlNode> paragraphs = node
                .ChildNodes
                .Where(b => b.Name != "br" && b.Name != "center" && !string.IsNullOrEmpty(b.InnerText.Trim()))
                .ToList();

            if (paragraphs.Count == 0)
                return null;

            string title = paragraphs.First().ParseHtmlNodeToString().Trim("\r\n ".ToCharArray());
            List<HtmlNode> list = paragraphs.Skip(1).ToList();
            list.ForEach(CleanHtmlNode);
            
            return new LnChapter(title, list.ToLnNodeList());
        }

        public bool CanParse(string url)
        {
            return url.ToLower().Contains("www.readlightnovel.org");
        }

        private static void CleanHtmlNode(HtmlNode node)
        {
            if (!node.InnerText.Contains("window.adsbygoogle")) return;

            List<HtmlNode> nodesToRemove = node.ChildNodes.Where(b => b.Name == "center").ToList();

            while (nodesToRemove.Count > 0)
            {
                HtmlNode tmp = nodesToRemove.First();
                nodesToRemove.Remove(tmp);
                tmp.Remove();
            }
            
            node.ChildNodes.ToList().ForEach(CleanHtmlNode);
        }
    }
}
