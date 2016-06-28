using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.DataStructureV4;
using Greet.DataStructureV4.Interfaces;


namespace Greet.DataStructureV4.ResultsStorage
{
    [Serializable]
    public class TechnologyResult : IHaveResults
    {
        string name = "";
        Results results = new Results();

        public TechnologyResult(string name, int technoRef)
        {
            this.name = name;
            results.BottomDim = 0;
            results.CustomFunctionalUnitPreference = null;
            results.ObjectType = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Technology;
            results.ObjectID = technoRef;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Results Results
        {
            get { return results; }
            set { results = value; }
        }

        public Dictionary<IIO, Results> GetResults(GData data)
        {
            Dictionary<IIO, Results> processResults = new Dictionary<IIO, Results>();

            processResults.Add(null, results);

            return processResults;
        }



        public override string ToString()
        {
            if(!String.IsNullOrEmpty(this.name))
                return this.name;
            else
                return base.ToString();
        }

        Dictionary<IIO, Results> IHaveResults.GetResults(GData data)
        {
            throw new NotImplementedException();
        }
    }
}
