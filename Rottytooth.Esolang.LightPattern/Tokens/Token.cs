namespace Rottytooth.Esolang.LightPattern.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public abstract class Token
    {
        public Token()
        {
            this.Children = new List<Token>();
        }

        public Token(string tokenName)
        {
            this.TokenName = tokenName;
            this.Children = new List<Token>();
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public List<Token> Children { get; set; }

        public string TokenName { get; set; }

        public object Value { get; set; }
    }
}

