namespace Rottytooth.Esolang.LightPattern.Tokens
{
    using System;
    using System.Runtime.CompilerServices;

    public class LiteralToken : ExpressionToken
    {
        public LiteralToken(string name) : base(name)
        {
        }

        public LpType Type { get; set; }
    }
}

