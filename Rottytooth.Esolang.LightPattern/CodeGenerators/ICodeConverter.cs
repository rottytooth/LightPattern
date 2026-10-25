using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rottytooth.Esolang.LightPattern.Tokens;

namespace Rottytooth.Esolang.LightPattern
{
    public interface ICodeConverter
    {
        string Convert(List<Token> tokenList, string programName, bool includePause);

        void GetCommandSet(List<Token> tokenList, StringBuilder retProgram);

        void GetExpression(List<ExpressionToken> tokenList, StringBuilder retProgram);
    }
}
