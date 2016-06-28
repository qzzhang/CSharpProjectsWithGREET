using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class FormulaResultNANException : Exception
    {
        public FormulaResultNANException(string message) : base(message) { }
    }
}