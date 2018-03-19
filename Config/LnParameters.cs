using System.Collections.Generic;

namespace LightNovelSniffer.Config
{
    internal class LnParameters
    {
        public string name;
        public string urlCover;
        public List<UrlParameter> urlParameters;

        public LnParameters()
        {
            urlParameters = new List<UrlParameter>();
        }

        public UrlParameter GetUrlParameter(string language)
        {
            foreach (UrlParameter up in urlParameters)
            {
                if (language.Equals(up.language))
                    return up;
            }
            return null;
        }
    }
}
