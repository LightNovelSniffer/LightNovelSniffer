namespace LightNovelSniffer.Exception
{
    public class ParserException : System.Exception
    {
        public ParserException(string message, System.Exception e) : base(message, e)
        {
        }
    }
}
