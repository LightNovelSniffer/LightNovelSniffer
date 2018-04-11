using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightNovelSniffer.Libs
{
    public interface IInput
    {
        bool Ask(string question);
        bool AskNegative(string question);
        string AskInformation(string question);
        string AskUrl(string question);
    }
}
