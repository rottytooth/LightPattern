namespace Rottytooth.Esolang.LightPattern.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class CommandToken : Token
    {
        public CommandToken()
        {
            this.Expressions = new List<ExpressionToken>();
        }

        public CommandToken(string name) : base(name)
        {
            this.Expressions = new List<ExpressionToken>();
        }

        public List<ExpressionToken> Expressions { get; private set; }

        public VariableToken Variable { get; set; }
    }
}

