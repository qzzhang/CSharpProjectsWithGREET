using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class UnitConversionException : Exception
    {
        public UnitConversionException(string message) : base(message) { }
    }
}