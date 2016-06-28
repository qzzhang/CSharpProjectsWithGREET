using System;

namespace Greet.DataStructureV4.Exceptions
{
    public class IDNotFoundInDatabase : Exception
    {
        public IDNotFoundInDatabase(string message) : base(message) { }
    }
}