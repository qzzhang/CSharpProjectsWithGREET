using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class CarbonRatioNANException : Exception
    {
        public CarbonRatioNANException(string message) : base(message) { }
    }
}