using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using LightNovelSniffer.Web;

namespace LightNovelSniffer
{
    internal static class ExtensionTools
    {
        internal static string ParseHtmlNodeToString(this HtmlNode node)
        {
            return WebUtility.HtmlDecode(node.InnerText);
        }

        internal static List<LnNode> ToLnNodeList(this IEnumerable<HtmlNode> node)
        {
            List<LnNode> res = new List<LnNode>();
            foreach (HtmlNode n in node)
            {
                res.Add(new LnNode(n.OuterHtml, n.InnerHtml, n.InnerText));
            }
            return res;
        }

        internal static string DecodeHtml(this string html)
        {
            return WebUtility.HtmlDecode(html);
        }

        internal static string GetCleanedHtml(this string html)
        {
            return html.Replace("<br>", "<br/>").Replace("<hr>", "<hr/>");
        }

        internal static bool ContainsInvarient(this string str, string recherche)
        {
            return str.ToLower().Contains(recherche.ToLower());
        }

        internal static bool ContainsType<T>(this IEnumerable<T> collection, Type type)
        {
            return collection.Any(i => i.GetType() == type);
        }
    }
}