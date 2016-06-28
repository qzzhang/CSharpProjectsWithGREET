using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Reflection;
using Greet.ConvenienceLib;
using Greet.LoggerLib;
using Greet.UnitLib3;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    /// <summary>
    /// Defines the reference to a single process. The objects of this class are used to store calculation results
    /// </summary>
    [Serializable]
    public class CanonicalProcess : IHaveResults, IProcessReference
    {
        #region public
        public string Name = "";
        /// <summary>
        /// Stores the results for each inputs relatively all outputs (no allocation or displacement)
        /// All the inputs results are stored here as they depend on the previous values in a pathway and cannot be stored in an instance of a process
        /// </summary>
        public Dictionary<Guid, CanonicalInput> InputsResults = new Dictionary<Guid, CanonicalInput>();
        /// <summary>
        /// Stores the results for each output by taking all the inputs amounts balanced to the respective outputs amounts
        /// or using allocation methods. Displaced outputs are not represented in this collection as they are not usable
        /// in downstream processes. Indexes are the resource ID associated to the upstrams at this point. For outputs all enems are balanced.
        /// </summary>
        public Dictionary<Guid, CanonicalOutput> OutputsResults = new Dictionary<Guid, CanonicalOutput>();
        /// <summary>
        /// Stores the credited amounts for displaced co-products. Used in order to be able to show these
        /// individual credits to the user in the co-products editor.
        /// </summary>
        public Dictionary<Guid, CanonicalOutput> DisplacedAmounts = new Dictionary<Guid, CanonicalOutput>();
        /// <summary>
        /// Vertex ID associated with that process reference in the pathway
        /// </summary>
        public Guid VertexId { get; set; }

        #endregion

        #region Constants
        public static KeyValuePair<string, bool> EmissionsAll = new KeyValuePair<string, bool>("Life Cycle", true);
        private static KeyValuePair<string, bool> ProcessFuelsUpStreamTechnologiesEmissions = new KeyValuePair<string, bool>("With Partial Upstream", true);
        private static KeyValuePair<string, bool> TechnologiesEmissionsOnly = new KeyValuePair<string, bool>("On Site", true);
        private static KeyValuePair<string, bool> LossesEmissions = new KeyValuePair<string, bool>("Losses", true);
        private static KeyValuePair<string, bool> OtherEmissions = new KeyValuePair<string, bool>("Other", true);
        private static KeyValuePair<string, bool> AdjustmentEmissions = new KeyValuePair<string, bool>("Adjustment", true);
        private static KeyValuePair<string, bool> CreditsEmissions = new KeyValuePair<string, bool>("Credits", true);
        public static KeyValuePair<string, bool> EnergyAllIncluded = new KeyValuePair<string, bool>("Life Cycle", true);
        private static KeyValuePair<string, bool> ProcessEnergyWithProcessFuelUpStream = new KeyValuePair<string, bool>("With Partial Upstream", true);
        private static KeyValuePair<string, bool> ProcessEnergyOnly = new KeyValuePair<string, bool>("On Site", true);
        private static KeyValuePair<string, bool> LossesEnergy = new KeyValuePair<string, bool>("Losses", true);
        private static KeyValuePair<string, bool> CreditsEnergy = new KeyValuePair<string, bool>("Credits", true);
        private static KeyValuePair<string, bool> TotalUrbanEmissions = new KeyValuePair<string, bool>("Life Cycle", true);
        private static KeyValuePair<string, bool> PartialUrbanEmissions = new KeyValuePair<string, bool>("With Partial Upstream", true);
        private static KeyValuePair<string, bool> DirectUrbanEmissions = new KeyValuePair<string, bool>("On Site", true);
        private static KeyValuePair<string, bool> HeaderEmissions = new KeyValuePair<string, bool>("Emissions", true);
        private static KeyValuePair<string, bool> HeaderEnergy = new KeyValuePair<string, bool>("Energy", true);
        private static KeyValuePair<string, bool> HeaderGeneral = new KeyValuePair<string, bool>("General", true);
        private static KeyValuePair<string, bool> HeaderUrbanEmissions = new KeyValuePair<string, bool>("Urban Emissions", true);
        #endregion

        #region attributes
        private int modelId;
        public string Notes {get;set;}
        public int pathwayReference = -1;
        #endregion attributes

        #region constructors

        public CanonicalProcess()
        {
        }

        public CanonicalProcess(int reference, int pathway_ref)
            : this()
        {
            this.modelId = reference;
            this.pathwayReference = pathway_ref;
        }

        #endregion constructors

        #region methods
        public List<CanonicalProcess> GetProcesses()
        {
            List<CanonicalProcess> toBeReturned = new List<CanonicalProcess>();
            toBeReturned.Add(this);
            return toBeReturned;
        }

        public override bool Equals(object obj)
        {
            CanonicalProcess pref = null;
            if (obj is CanonicalProcess)
                pref = (CanonicalProcess)obj;
            return pref != null && pref.modelId == this.modelId;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

       
        /// <summary>
        /// This function writes the results in a form of xml
        /// </summary>
        /// <returns>node which contains the calculation results</returns>
        public XmlNode ResultsToXml(XmlDocument doc)
        {
            //double amountRatio = GetAmountRatio();
            //LightValue CalculatedForOutput = GetCalculatedForOutput(amountRatio);
            XmlNode node = doc.CreateNode("process", doc.CreateAttr("id", this.ModelId));

            return node;
        }

        #endregion methods

        #region accessors

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int ModelId
        {
            get { return modelId; }
            set { modelId = value; }
        }

        #endregion accessors

        /// <summary>
        /// Returns results associated with all the outputs for that process
        /// </summary>
        /// <param name="data">Data where the ModelID process is available</param>
        /// <returns>Dictionary of outputs and results</returns>
        public Dictionary<IIO, Results> GetResults(GData data)
        {
            Dictionary<IIO, Results> processResults = new Dictionary<IIO, Results>();

            foreach (KeyValuePair<Guid, CanonicalOutput> pair in this.OutputsResults)
            {
                Results resultForOutput = new Results();

                AOutput currentOutput = pair.Value.Output;
                if (currentOutput != null)
                {
                    resultForOutput.BottomDim = currentOutput.AmountAfterLosses(data.ResourcesData).Dim;
                    resultForOutput.ObjectID = this.ModelId;
                    resultForOutput.BiongenicCarbonRatio = pair.Value.MassBiogenicCarbonRatio;
                    resultForOutput.ObjectType = Enumerators.ItemType.Process;

                    CanonicalOutput cOutput = pair.Value;
                    resultForOutput.CustomFunctionalUnitPreference = cOutput.Results.CustomFunctionalUnitPreference;

                    // life cycle
                    resultForOutput.wellToProductEnem = cOutput.Results.wellToProductEnem;
                    resultForOutput.wellToProductUrbanEmission = cOutput.Results.wellToProductUrbanEmission;
                    
                    // on site
                    resultForOutput.onsiteEmissions = cOutput.Results.onsiteEmissions;
                    resultForOutput.onsiteResources = cOutput.Results.onsiteResources;
                    resultForOutput.onsiteUrbanEmissions = cOutput.Results.onsiteUrbanEmissions;

                    processResults.Add(currentOutput, resultForOutput);
                }
            }

            return processResults;
        }

        public Dictionary<IIO, IResults> GetUpstreamResults(IData data)
        {
            Dictionary<IIO, IResults> toReturn = new Dictionary<IIO, IResults>();
            Dictionary<IIO, Results> currentResults = this.GetResults(data as GData);
            foreach (KeyValuePair<IIO, Results> pair in currentResults)
                toReturn.Add(pair.Key, pair.Value as IResults);
            return toReturn;
        }

        public override string ToString()
        {
            if(!String.IsNullOrEmpty(this.Name))
                return Name;
            else
                return "Process: " + this.modelId;
        }

        public void ClearAllResults()
        {
            this.InputsResults.Clear();
            this.OutputsResults.Clear();
            this.DisplacedAmounts.Clear();
        }

       
    }
}
