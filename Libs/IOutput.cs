using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightNovelSniffer.Libs
{
    public interface IOutput
    {
        void Log(string text, int indentationLevel);
        void Progress(string text, int indentationLevel);
    }
}
