using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class MarketValueNANException : Exception
    {
        public MarketValueNANException(string message) : base(message) { }
    }
}