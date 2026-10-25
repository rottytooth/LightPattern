using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rottytooth.Esolang.LightPattern
{
    public class JavascriptConverter : CodeConverterBase
    {
        public JavascriptConverter()
        {
            this.BeginProgram = "var output = \"\"\nfunction {0}\n{{\n";
            this.EndProgram = "}\n";
            this.PauseProgram = ""; // no need for this in JS version, as is written to browser

            this.DeclareInt = "var";
            this.DeclareFloat = "var";
            this.DeclareChar = "var";
            this.DeclareString = "var";

            this.WriteToScreenBegin = "output += ";
            this.WriteToScreenEnd = "";
        }
    }
}

