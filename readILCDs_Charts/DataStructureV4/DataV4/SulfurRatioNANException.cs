using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class SulfurRatioNANException : Exception
    {
        public SulfurRatioNANException(string message) : base(message) { }
    }
}