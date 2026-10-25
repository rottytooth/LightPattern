using System;

namespace Rottytooth.Esolang.LightPattern
{
    public class ParserError : Exception
    {
        public ParserError(string errorMessage) : base(errorMessage)
        {
        }
    }
}

