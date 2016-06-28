using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class InputAmountException : Exception
    {
        public InputAmountException(string message) : base(message) { }
    }
}