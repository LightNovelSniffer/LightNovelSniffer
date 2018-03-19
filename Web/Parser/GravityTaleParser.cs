using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    class GravityTaleParser : IParser
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

            List<HtmlNode> paragraphs = node
                .ChildNodes
                .Where(b => b.Name != "#text")
                .ToList();

            while (paragraphs.Count > 0 && string.IsNullOrEmpty(paragraphs.First().ParseHtmlNodeToString()))
            {
                paragraphs = paragraphs.Skip(1).ToList();
            }

            if (paragraphs.Count == 0)
                return null;

            string title = paragraphs.First().ParseHtmlNodeToString();

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