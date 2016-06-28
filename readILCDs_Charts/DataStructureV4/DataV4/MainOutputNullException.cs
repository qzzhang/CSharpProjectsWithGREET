using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class MainOutputNullException : Exception
    {
        public MainOutputNullException(string message) : base(message) { }
    }
}