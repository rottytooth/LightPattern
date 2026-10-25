using System;
using System.Collections.Generic;
using System.Text;
using Rottytooth.Esolang.LightPattern.Tokens;

namespace Rottytooth.Esolang.LightPattern
{
    public class CSharpConverter : CodeConverterBase
    {
        public CSharpConverter()
        {
            this.BeginProgram = "using System;\n\nnamespace LightPattern.{0}\n{{\npublic class MainProgram \n{{\npublic static void Main()\n{{\n";
            this.EndProgram = "}\n}\n}\n";
            this.PauseProgram = "Console.ReadLine();";

            this.DeclareInt = "int";
            this.DeclareFloat = "double";
            this.DeclareChar = "char";
            this.DeclareString = "string";

            this.WriteToScreenBegin = "Console.Write(";
            this.WriteToScreenEnd = ");";
        }
    }
}

