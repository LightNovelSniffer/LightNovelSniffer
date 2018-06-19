using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    public class GravityTaleParser : IParser
    {
        public bool CanParse(string baseUrl)
        {
            return baseUrl.ContainsInvarient("gravitytales.com");
        }

        public LnChapter Parse(HtmlDocument doc)
        {
            HtmlNode node = doc
                .DocumentNode
                .SelectSingleNode("//body")
                .SelectNodes("//div")
                .First(d =>
                    d.Attributes["id"] != null &&
                    d.Attributes["id"].Value.Equals("chapterContent"));

            if (node == null)
                return null;

            List<LnNode> paragraphs = node
                .ChildNodes
                .Where(b => b.Name != "#text")
                .ToLnNodeList();

            while (paragraphs.Count > 0 && string.IsNullOrEmpty(paragraphs.First().InnerText.DecodeHtml()))
            {
                paragraphs = paragraphs.Skip(1).ToList();
            }

            if (paragraphs.Count == 0)
                return null;

            string title = paragraphs.First().InnerText.DecodeHtml();

            if (title.ContainsInvarient("chapter"))
            {
                paragraphs = paragraphs.Skip(1).ToList();
            }
            else
            {
                title = null;
            }

            return new LnChapter(title, paragraphs);
        }
    }
}