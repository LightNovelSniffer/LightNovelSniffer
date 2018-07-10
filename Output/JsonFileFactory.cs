using System.IO;
using LightNovelSniffer.Libs;

namespace LightNovelSniffer.Output
{
    public static class JsonFileFactory
    {
        public static JsonFile InitiateFromFile(string file)
        {
            try
            {
                string content = File.ReadAllText(file);
                return JsonTools.Deserialize<JsonFile>(content);
            }
            catch (System.Exception)
            {
            }
            return null;
        }
    }
}
