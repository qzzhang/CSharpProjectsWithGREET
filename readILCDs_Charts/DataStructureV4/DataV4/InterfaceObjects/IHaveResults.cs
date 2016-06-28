using System.Collections.Generic;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;


namespace Greet.DataStructureV4
{
    internal interface IHaveResults
    {
        Dictionary<IIO, Results> GetResults(GData data);
    }
}
