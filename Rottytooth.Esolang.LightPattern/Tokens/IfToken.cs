namespace Rottytooth.Esolang.LightPattern.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class IfToken : CommandToken
    {
        public IfToken()
        {
            this.Else = new List<Token>();
        }

        public IfToken(string name) : base(name)
        {
            this.Else = new List<Token>();
        }

        public List<Token> Else { get; set; }
    }
}

