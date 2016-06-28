using System;

namespace Greet.UnitLib3
{
    /// <summary>
    /// Used by Dimension class
    /// </summary>
    internal class DimensionStateException : Exception
    {
        public DimensionStateException() :
            base("The instance of the Dimension class is in incorrect state") { }
        public DimensionStateException(string msg) :
            base("The instance of the Dimension class is in incorrect state. " + msg) { }
    }
}
