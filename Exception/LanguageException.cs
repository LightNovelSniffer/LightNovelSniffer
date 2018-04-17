using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightNovelSniffer.Exception
{
    public class LanguageException : System.Exception
    {
        public LanguageException(string message) : base(message){}
    }
}
