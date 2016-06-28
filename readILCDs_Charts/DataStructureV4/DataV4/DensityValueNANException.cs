using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class DensityValueNANException : Exception
    {
        public DensityValueNANException(string message) : base(message) { }
    }
}