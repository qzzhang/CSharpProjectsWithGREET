using System;

namespace Greet.UnitLib3
{
    internal class ExpressionStringParingException : Exception
    {
        static string stdmsg = "The provided expression sting cannot be parsed. Please check if the units are defined in data.xml and string is properly formatted.";
        public ExpressionStringParingException() :
            base(stdmsg) { }
        public ExpressionStringParingException(string msg) :
            base(stdmsg + " " + msg) { }
    }
}