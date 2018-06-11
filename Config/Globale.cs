using System.Collections.Generic;

namespace LightNovelSniffer.Config
{
    public static class Globale
    {
        public static string OUTPUT_FOLDER;
        public static bool INTERACTIVE_MODE;
        public static string PUBLISHER;
        public static string DEFAULT_CHAPTER_TITLE;
        public static int MAX_CHAPTER_ON_ERROR_COUNT_BEFORE_STOP;
        public static List<LnParameters> LN_TO_RETRIEVE;
    }
}
