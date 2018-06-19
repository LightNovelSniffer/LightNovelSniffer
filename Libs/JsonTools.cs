using System.IO;
using Newtonsoft.Json;

namespace LightNovelSniffer.Libs
{
    public static class JsonTools
    {
        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public static string Serialize<T>(T @object)
        {
            return JsonConvert.SerializeObject(
                @object,
                Formatting.Indented,
                GetSettings());
        }

        public static T Deserialize<T>(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return Deserialize<T>(reader.ReadToEnd());
        }

        public static T Deserialize<T>(string fileName, string folderName)
        {
            return Deserialize<T>(File.ReadAllText(Path.Combine(folderName, fileName)));
        }

        public static T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content, GetSettings());
        }
    }
}