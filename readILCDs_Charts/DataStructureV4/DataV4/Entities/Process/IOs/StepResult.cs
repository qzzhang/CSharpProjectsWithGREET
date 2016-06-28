using System;
using System.Collections.Generic;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;



namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is designed to be used as an agregator of input results for a transportation step
    /// A transportation step is decomposed in multiple inptus before calculations and needs to be re-assembled for results display in the WTP
    /// </summary>
    [Serializable]
    internal class StepResult : IHaveResults
    {
        #region Attributes
        public EmissionAmounts OnSiteEmissions = new EmissionAmounts();
        public ResourceAmounts OnSiteResources = new ResourceAmounts();
        public Enem LifeCycleEe = new Enem();
        public TransportationStep StepReference;
        public EmissionAmounts LifeCycleUrbanEmissions = new EmissionAmounts();
        public EmissionAmounts OnSiteUrbanEmissions = new EmissionAmounts();

        /// <summary>
        /// This field is used to display the name of the Step in Transportaion process. This is done as the class doesnt have access to Data object of project.
        /// </summary>
        //Do not delete this field
        public string stepName;
        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pref"></param>
        public StepResult(TransportationStep step)
            : base()
        {
            this.StepReference = step;
        }

        #endregion

        #region Accessors

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

            Results results = new Results();

            results.CustomFunctionalUnitPreference = null;
            results.ObjectID = this.StepReference.Reference;
            results.ObjectType = Enumerators.ItemType.Mode;
            results.wellToProductEnem = this.LifeCycleEe;
            results.wellToProductUrbanEmission = this.LifeCycleUrbanEmissions;
            results.onsiteEmissions = this.OnSiteEmissions;
            results.onsiteResources = this.OnSiteResources;
            results.onsiteUrbanEmissions = OnSiteUrbanEmissions;

            processResults.Add(null, results);

            return processResults;
        }

        #endregion

        public override string ToString()
        {
            return "Step: " + stepName;
        }

        public void Add(InputResult inpRes)
        {
            this.OnSiteEmissions += inpRes.OnSiteEmissions;
            this.OnSiteResources += inpRes.OnSiteResources;
            this.LifeCycleEe += inpRes.LifeCycleEe;
            this.LifeCycleUrbanEmissions += inpRes.LifeCycleUrbanEmissions;
            this.OnSiteUrbanEmissions += inpRes.OnSiteUrbanEmissions;
        }
    }
}
