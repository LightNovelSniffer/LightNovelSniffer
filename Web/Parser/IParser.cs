using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    interface IParser
    {
        LnChapter Parse(HtmlDocument doc);
        bool CanParse(string url);
    }
}
