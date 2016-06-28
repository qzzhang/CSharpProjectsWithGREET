using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class HeatingValueNANException: Exception
    {
        public HeatingValueNANException(string message) : base(message) { }
    }
}
