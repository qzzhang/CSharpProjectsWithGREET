using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class InvalidParameterReferenceException : Exception
    {
        public InvalidParameterReferenceException(string message) : base(message) { }
    }
}