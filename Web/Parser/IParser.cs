using HtmlAgilityPack;

namespace LightNovelSniffer.Web.Parser
{
    public interface IParser
    {
        LnChapter Parse(HtmlDocument doc);
        bool CanParse(string url);
    }
}
