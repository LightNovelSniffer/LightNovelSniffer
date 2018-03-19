using System.Linq;
using System.Reflection;

namespace LightNovelSniffer.Web.Parser
{
    internal class ParserFactory
    {
        private static WuxiaworldParser wwParser;
        private static XiaowazParser xiaowasParser;
        private static GravityTaleParser gravityTaleParser;

        public static WuxiaworldParser GetWuxiaworldParser()
        {
            return wwParser ?? (wwParser = new WuxiaworldParser());
        }

        public static XiaowazParser GetXiaowazParser()
        {
            return xiaowasParser ?? (xiaowasParser = new XiaowazParser());
        }

        public static GravityTaleParser GetGravityTaleParser()
        {
            return gravityTaleParser ?? (gravityTaleParser = new GravityTaleParser());
        }

        public static IParser GetParser(string baseUrl)
        {
            foreach (MethodInfo mi in typeof(ParserFactory).GetMethods().Where(mi => mi.Name.StartsWith("Get") && mi.Name.EndsWith("Parser")))
            {
                IParser p = (IParser) mi.Invoke(mi, new object[] {});
                
                if (p.CanParse(baseUrl))
                    return p;
            }
            return null;
        }
    }
}