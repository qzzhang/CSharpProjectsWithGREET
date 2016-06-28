using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class ValueIncorrect : Exception
    {
        public ValueIncorrect(string message) : base(message) { }
    }
}