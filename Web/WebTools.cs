using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LightNovelSniffer.Exception;
using LightNovelSniffer.Resources;

namespace LightNovelSniffer.Web
{
    public static class WebTools
    {
        public static byte[] DownloadCover(string urlCover)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadData(new Uri(urlCover));
                }
                catch (System.Exception ex)
                {
                    if (ex is UriFormatException || ex is WebException)
                    {
                        throw new CoverException(String.Format(LightNovelSniffer_Strings.CoverDownloadExceptionMessage, urlCover));
                    }

                    throw;
                }
            }
        }
    }
}
