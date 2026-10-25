namespace Rottytooth.Esolang.LightPattern.Tokens
{
    using System;
    using System.Runtime.CompilerServices;

    public class VariableToken : ExpressionToken
    {
        public VariableToken(string name) : base(name)
        {
        }

        public LpType Type { get; set; }

        public string VariableName { get; set; }
    }
}

