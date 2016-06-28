using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4.ResultsStorage
{
    [Serializable]
    public class InputResult : IHaveResults
    {
        #region Attributes
        /// <summary>
        /// This field is used to display the name for Results Grid. This is done here as the class doesnt have access to Data object of project.
        /// </summary>
        //Do not delete this field
        public string Name;
        public double MassBiogenicCarbonRatio = 0;
        public EmissionAmounts OnSiteEmissions = new EmissionAmounts();
        public EmissionAmounts RatioEmissions = new EmissionAmounts();
        public Enem LifeCycleEe = new Enem();
        public EmissionAmounts LifeCycleUrbanEmissions = new EmissionAmounts();
        public EmissionAmounts OnSiteUrbanEmissions = new EmissionAmounts();
        public ResourceAmounts OnSiteResources = new ResourceAmounts();
        public Dictionary<int, TechnologyResult> TechnologyResults = new Dictionary<int, TechnologyResult>();
        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pref"></param>
        public InputResult(string inpName)
            : base()
        {
            this.Name = inpName;
        }

        #endregion

        #region Accessors

        public Results GetResultsInstance()
        {
            Results results = new Results();
            results.BiongenicCarbonRatio = this.MassBiogenicCarbonRatio;
            results.ObjectType = Enumerators.ItemType.Input;
            results.wellToProductEnem = this.LifeCycleEe;
            results.wellToProductUrbanEmission = this.LifeCycleUrbanEmissions;
            results.onsiteEmissions = this.OnSiteEmissions;
            results.onsiteResources = this.OnSiteResources;
            results.onsiteUrbanEmissions = OnSiteUrbanEmissions;
            results.CustomFunctionalUnitPreference = null;

            return results;
        }

        #endregion

        #region IHaveResults Members

        /// <summary>
        /// Returns results associated with an Input. These results account for all the outputs of a process,
        /// therefore there is not Guid in the dictionary of results simply 0. As we do not define an 
        /// output we also do not define a functional unit. Functional unit is null in that case.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<IIO, Results> GetResults(GData data)
        {
            Dictionary<IIO, Results> processResults = new Dictionary<IIO, Results>();

            Results results = this.GetResultsInstance();

            processResults.Add(null, results);

            return processResults;
        }

        #endregion

        public override string ToString()
        {
            return "Input: " + Name;
        }

        
    }
}
