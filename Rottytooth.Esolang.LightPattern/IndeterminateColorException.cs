using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rottytooth.Esolang.LightPattern
{
    public class IndeterminateColorException : Exception
    {
        public IndeterminateColorException(string message) : base(message)
        {
        }
    }
}
