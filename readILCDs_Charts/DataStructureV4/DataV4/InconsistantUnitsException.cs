using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class InconsistantUnitsException : Exception
    {
        public InconsistantUnitsException(string message) : base(message) { }
    }
}