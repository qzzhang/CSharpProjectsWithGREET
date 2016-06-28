using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    /// <summary>
    /// In input technoologies attribute is added to the base 
    /// </summary>
    public class Input : AIO, IInput
    {
        #region attributes

        /// <summary>
        /// Amount that will be used for the calculations (onsite resource usage and upstream) for that input
        /// this will not be saved to the database but serves as a placeholder for calculations after conversion to general in out
        /// </summary>
        public LightValue AmountForCalculations = new LightValue(0.0, DimensionUtils.ENERGY);

        /// <summary>
        /// Additional on site emissions. These emissions are going to be added to the onSite and lifeCycle emissions
        /// during the calculations. However this object is not saved to the database as the Amount for calculation isn't either
        /// </summary>
        public EmissionAmounts OnSiteEmissionsForCalculations = new EmissionAmounts();

        /// <summary>
        /// Additional on site URBAN emissions. These emissions are going to be added to the onSite and lifeCycle emissions
        /// during the calculations. However this object is not saved to the database as the Amount for calculation isn't either
        /// </summary>
        public EmissionAmounts OnSiteUrbanEmissionsForCalculations = new EmissionAmounts();

        /// <summary>
        /// Source of the input, indicates where to look for an upstream if any
        /// </summary>
        private Enumerators.SourceType source = Enumerators.SourceType.Well;

        /// <summary>
        /// Upstream Reference when Mix or Pathway is used as source
        /// </summary>
        private int sourceMixOrPathwayID = -1;

        /// <summary>
        /// List of technologies with shares ($T$)
        /// </summary>
        private List<TechnologyRef> technologyReferences = new List<TechnologyRef>();

        /// <summary>
        /// Emission Ratios (emission gas and rate)
        /// </summary>
        private List<EmissionRatio> emissionRatios = new List<EmissionRatio>();

        /// <summary>
        /// Sequestration boolean flag
        /// </summary>
        public bool sequestrationFlag = false;

        /// <summary>
        /// Attributes of the sequestation ptocess
        /// </summary>
        public Sequestration sequestration;

        /// <summary>
        /// Member to hold the calculated sequestrated amount in grams
        /// </summary>
        public double sequestratedCo2AmountInGrams;

        /// <summary>
        /// If not an internal products, an upstream can be defined for the calculations
        /// Otherwise if the input is considered as an internal resouce, no upstream is necessary
        /// </summary>
        public bool NotAnInternalProduct = true;

        /// <summary>
        /// For organizing the inputs in the dictionaries for the displayed results we need to know which input is considered as main
        /// If we know we can reproduce greet excel results, we were used to account as main input the input coming from previous, but because of 
        /// losses, it might happens that some fuel lost, are recycled and used by the process, and coming from previous, those fuels are however not considered
        /// as main inputs for the process
        /// </summary>
        private bool recognizedAsMainInput = false;

        #endregion attributes

        #region constructors
        internal Input()
            : base()
        {
            this.SourceType = Enumerators.SourceType.Mix;
        }
        public Input(GData data, XmlNode node, string optionalParamPrefix)
            : base(data, node, optionalParamPrefix)
        {
            if (node.Attributes["source"] != null)
                this.SourceType = (Enumerators.SourceType)Enum.Parse(typeof(Enumerators.SourceType), node.Attributes["source"].Value, true);


            if (this.SourceType == Enumerators.SourceType.Mix && node.Attributes["mix"] != null)
                this.SourceMixOrPathwayID = Convert.ToInt32(node.Attributes["mix"].Value);
            if (this.SourceType == Enumerators.SourceType.Pathway && node.Attributes["pathway"] != null)
                this.SourceMixOrPathwayID = Convert.ToInt32(node.Attributes["pathway"].Value);
            if (node.Attributes["notes"] != null)
                this.Notes = node.Attributes["notes"].Value;
            if (node.Attributes["accounted_in_energy_balance"] != null)
                this.NotAnInternalProduct = Convert.ToBoolean(node.Attributes["accounted_in_energy_balance"].Value);
            if (node.Attributes["considered_as_main"] != null)
                this.recognizedAsMainInput = Convert.ToBoolean(node.Attributes["considered_as_main"].Value);
            if (node.SelectSingleNode("sequestration") != null)
            {
                sequestration = new Sequestration(data, node.SelectSingleNode("sequestration"), optionalParamPrefix + "_seq");
                this.sequestrationFlag = true;
            }

            foreach (XmlNode ratio in node.SelectNodes("emission_ratio"))
            {
                this.emissionRatios.Add(new EmissionRatio(data, ratio, optionalParamPrefix));
            }

            foreach (XmlNode techno in node.SelectNodes("technology"))
            {
                this.technologyReferences.Add(new EntityTechnologyRef(data, techno, optionalParamPrefix));
            }
        }
        public Input(GData data,ParameterTS designAmount, int material_id, int mix_pathway, Enumerators.SourceType _source)
            : this()
        {
            this.DesignAmount = designAmount;
            this.resourceId = material_id;
            this.SetSource(_source, mix_pathway);
        }
        public Input(GData data, ParameterTS designAmount, int _material_id)
            : this()
        {
            this.DesignAmount = designAmount;
            this.resourceId = _material_id;
            Enumerators.SourceType _source = Enumerators.SourceType.Well;
            int _mix_pathway_id = -1;
            if(data.Resources != null && data.ResourcesData.ContainsKey(_material_id))
                data.ResourcesData[_material_id].GetMostFrequentSource(out _source, out _mix_pathway_id);
            this.SetSource(_source, _mix_pathway_id);
        }


        #endregion constructors

        #region methods
        private void SetSource(Enumerators.SourceType _source, int _sourceID)
        {
            this.source = _source;
            if (_source == Enumerators.SourceType.Pathway)
                this.SourceMixOrPathwayID = _sourceID;
        }

        public XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode input = doc.CreateNode("input", doc.CreateAttr("source", this.SourceType));
            if (this.recognizedAsMainInput == true)
            {
                XmlAttribute temp = doc.CreateAttr("considered_as_main", this.recognizedAsMainInput);
                input.Attributes.Append(temp);
            }
            this.ToXmlNode(input, doc);
            return input;
        }

        internal new void ToXmlNode(XmlNode input, XmlDocument doc)
        {
            if (this.SourceMixOrPathwayID != -1 && this.SourceType == Enumerators.SourceType.Mix)
                input.Attributes.Append(doc.CreateAttr("mix", this.SourceMixOrPathwayID));
            else if (this.SourceMixOrPathwayID != -1 && this.SourceType == Enumerators.SourceType.Pathway)
                input.Attributes.Append(doc.CreateAttr("pathway", this.SourceMixOrPathwayID));

            if (this.NotAnInternalProduct == false)
                input.Attributes.Append(doc.CreateAttr("accounted_in_energy_balance", this.NotAnInternalProduct));
            foreach (EmissionRatio er in this.EmissionRatios)
                input.AppendChild(er.ToXmlNode(doc));

            foreach (TechnologyRef technoref in Technologies)
            {
                if(technoref is EntityTechnologyRef)
                    input.AppendChild((technoref as EntityTechnologyRef).ToXmlNode(doc));
            }

            if (this.Notes != "")
                input.Attributes.Append(doc.CreateAttr("notes", this.Notes));
            if (this.sequestrationFlag)
                input.AppendChild(sequestration.ToXmlNode(doc));

            base.ToXmlNode(input, doc);
        }

        /// <summary>
        /// Checks the integrity of the output by making sure that the quantity and source are defined properly
        /// </summary>
        /// <param name="data">Dataset containing resources, pathways, mixes and processes</param>
        /// <param name="showIds">If True IDs will be shown in the human readable messages</param>
        /// <param name="fixFixableIssues">If True some of the issues will be automatically fixed</param>
        /// <param name="errorMessage">The human readable output of the method</param>
        /// <param name="processId">Optional process ID which will help fixing unit issues in the amount</param>
        /// <returns></returns>
        internal bool CheckIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage, int processId = -1)
        {
            bool canBeHandled = true;
            StringBuilder problems = new StringBuilder();

            //check that resource ID is in our database as well as possible references to mixes and pathways
            #region check source
            if (data.ResourcesData.ContainsKey(this.resourceId) == false)
                problems.AppendLine(" - Unknown Resource Id -" + this.resourceId);
            else
            {
                if (this.NotAnInternalProduct)
                {
                    if (this.SourceType == Enumerators.SourceType.Mix)
                    {
                        if (data.MixesData.ContainsKey(this.SourceMixOrPathwayID) == false)
                            problems.AppendLine(" - Invalid " + "Pathway Mix" +
                                (showIds ? " (id -" + this.SourceMixOrPathwayID + ")" : "") +
                                    ", for the resource: " + data.ResourcesData[this.resourceId].Name);
                    }
                    if (this.SourceType == Enumerators.SourceType.Well)
                    {
                        if (data.ResourcesData[this.resourceId].canBePrimaryResource == false)
                            problems.AppendLine(" - Resource " + data.ResourcesData[this.resourceId].Name + (showIds ? " (id -" + this.ResourceId + ")" : "") + " is used from well but not marked as primary resource");
                    }
                    if (this.source == Enumerators.SourceType.Pathway)
                    {
                        if (data.PathwaysData.ContainsKey(this.SourceMixOrPathwayID) == false)
                            problems.AppendLine(" - Invalid Pathway " + "Pathway Mix" +
                                (showIds ? " (id -" + this.SourceMixOrPathwayID + ")" : "") +
                                    ", for the resource: " + data.ResourcesData[this.resourceId].Name);
                    }
                }
            }
            #endregion

            #region check input amount
            if (this is InputWithShare)
            {
                Parameter param = (this as InputWithShare).Share.CurrentValue;

                if (Units.QuantityList.ByDim(param.Dim) != null
                    && param.Dim != DimensionUtils.RATIO)
                    problems.AppendLine("ERROR: Input with share contains a share that is not a ratio");
            }
            else
            {
                //check that stateless input cannot be defined in mass or volume units
                if (data.ResourcesData.ContainsKey(this.resourceId))
                {
                    ResourceData rd = data.ResourcesData[this.resourceId];
                    if (rd.State == Resources.PhysicalState.energy)
                    {
                        foreach (Parameter param in this.DesignAmount.Values)
                        {
                            if (Units.QuantityList.ByDim(param.Dim) != null
                                && (param.Dim == DimensionUtils.MASS
                                || param.Dim == DimensionUtils.VOLUME))
                            {
                                problems.AppendLine("Input contains an amount defined with a unit of mass or volume, however the resource is stateless.");
                            }
                        }
                    }
                }

                //checks that we do not use a composed unit for the inputs (as it was done in some older versions of the database)
                foreach (Parameter param in this.DesignAmount.Values)
                {
                    if (param.GreetValuePreferedExpression.Contains("/") || param.GreetValuePreferedExpression.Contains("*"))
                    { //detects something such as joules/grams or any composed unit                  
                        problems.AppendLine("Input contains an amount defined with a composed unit: " + DimensionUtils.ToMLTUnith(param.Dim));
                        if (fixFixableIssues)
                        {
                            if (!param.CurrentFormula.Contains("["))
                            {
                                if (data.ProcessesData.ContainsKey(processId) && (param.Dim == DimensionUtils.RATIO))
                                {
                                    AProcess process = data.ProcessesData[processId];
                                    AQuantity bqty = Units.QuantityList.ByDim(process.MainOutput.DesignAmount.CurrentValue.Dim);
                                    param._greetValueDim = bqty.Dim;
                                    param._greetValuePreferedUnitExpression = bqty.Units[bqty.PreferedUnitIdx].Expression;
                                
                                }
                                //foreach (IQuantity unitGroup in Units.QuantityList.Values)
                                foreach (AQuantity unitGroup in Units.QuantityList.Values)
                                {
                                    uint div = DimensionUtils.Minus(param.Dim, unitGroup.Dim);//perforns a substraction to see if only bottom units remains ==> meaning the top unit was the same : ex [J/kg]/[J] -> only kg^-1 remains, top was the same 
                                    int mass, dist, time, currency;
                                    DimensionUtils.ToMLT(div, out mass, out dist, out time, out currency);
                                    if (mass <= 0 && dist <= 0 && time <= 0 && currency <= 0)
                                    {
                                        param._greetValueDim = unitGroup.Dim;
                                        param._greetValuePreferedUnitExpression = unitGroup.Units[unitGroup.PreferedUnitIdx].Expression;
                                        problems.AppendLine("FIXED: This input has been fixed with a unit of " + unitGroup.Name);
                                    }
                                }
                            }
                            else
                            {
                                problems.AppendLine("ERROR: The issue cannot be fixed automatically, please check the forumla for that parameter");
                            }
                        }
                        else if (param.CurrentFormula.Contains("["))
                        {
                            problems.AppendLine("ERROR: The issue cannot be fixed automatically, please check the forumla for that parameter");
                        }
                    }
                }
            }
            #endregion

            #region check input amount compatibility with technology defined
            //Inputs that are inputwithshare does not need to following check.
            if (this.DesignAmount != null && this.DesignAmount.CurrentValue != null && !(this is InputWithShare))
            {
                this.DesignAmount.CurrentValue.UpdateBuffers(data);
                foreach (TechnologyRef techno in this.Technologies.Where(item => data.TechnologiesData.ContainsKey(item.Reference)))
                {
                    EmissionsFactors ef = data.TechnologiesData[techno.Reference].CurrentValue;
                    foreach (KeyValuePair<int, LightValue> emission_factor_pair in ef.EmissionFactorsForCalculations)
                    {
                        if (emission_factor_pair.Value != null)
                        {
                            int gasId = emission_factor_pair.Key;
                            LightValue emissionFactor = emission_factor_pair.Value;
                            uint product = DimensionUtils.Plus(emissionFactor.Dim, this.DesignAmount.CurrentValue.Dim);
                            if (product == DimensionUtils.MASS || product == DimensionUtils.VOLUME ||
                                product == DimensionUtils.ENERGY) continue;
                            if (data.ResourcesData[this.ResourceId].CanConvertTo(DimensionUtils.ENERGY,
                                this.DesignAmount.CurrentValue.ToLightValue())) //we check if we can convert the input to energy before combustion because the calculations are supporting that
                            {
                                product = DimensionUtils.Plus(emissionFactor.Dim, DimensionUtils.ENERGY);
                                if (product == DimensionUtils.MASS || product == DimensionUtils.VOLUME ||
                                product == DimensionUtils.ENERGY) continue;
                            }

                            problems.AppendLine(" - This input uses an amount that cannot be used for the emission factors defined in " + data.TechnologiesData[techno.Reference].Name);
                            break;
                        }
                    }
                }
            }


            #endregion
            //Add Input Resource Name to make it easier to understand the error message.
            if(problems.Length!=0)
            {
                if (data.ResourcesData.ContainsKey(this.resourceId))
                    errorMessage = "Input Name - " + data.ResourcesData[this.resourceId].Name + "\n" + problems.ToString();
                else
                    errorMessage = "Input Name - Unknown Resource\n" + problems.ToString();
            }
            else
                errorMessage =  problems.ToString();
            return canBeHandled;
        }

        #endregion methods

        #region accessors

        public Enumerators.SourceType SourceType
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// This attribute returns true if that input should be recognized as a main input for the results
        /// as in our results we usually sum up everything it does not really matters, but for reasons like energy used on site only
        /// without upstream we might want to account an input coming from previous as what was called "process fuel"
        /// </summary>
        public bool RecognizedAsMainInput
        {
            get { return recognizedAsMainInput; }
            set { recognizedAsMainInput = value; }
        }

        /// <summary>
        /// List of technologies and their shares associated with the input.
        /// </summary>
        [Browsable(false)]
        public List<TechnologyRef> Technologies
        {
            get { return technologyReferences; }
            set { technologyReferences = value; }
        }

        public int SourceMixOrPathwayID
        {
            get { return sourceMixOrPathwayID; }
            set { sourceMixOrPathwayID = value; }
        }

        [Browsable(false)]
        public IInputResourceReference ResourceReference
        {
            get
            {
                InputResourceReference irr = new InputResourceReference();
                if (this.source == Enumerators.SourceType.Pathway)
                    irr = new InputResourceReference(this.ResourceId, this.SourceMixOrPathwayID, Enumerators.SourceType.Pathway);
                else if (this.source == Enumerators.SourceType.Mix)
                    irr = new InputResourceReference(this.ResourceId, this.SourceMixOrPathwayID, Enumerators.SourceType.Mix);
                else
                    irr = new InputResourceReference(this.resourceId, -1, (Enumerators.SourceType)source);

                return irr;
            }
        }

        public Guid Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// For Interfaces plugins definition, this list cannot be modified
        /// </summary>
        [Browsable(false)]
        public List<int> TechnologyIds
        {
            get
            {
                List<int> techIds = new List<int>();
                foreach (TechnologyRef tr in this.technologyReferences)
                    techIds.Add(tr.Reference);
                return techIds;
            }
        }

        [Browsable(false)]
        public ISequestration sequestrationParameter
        {
            get
            {
                if (this.sequestrationFlag)
                    return sequestration;
                else
                    return null;
            }
        }

        [Browsable(false)]
        public List<EmissionRatio> EmissionRatios
        {
            get { return emissionRatios; }
            set { emissionRatios = value; }
        }

        public override string ToString()
        {
            return this.resourceId + ": " + this.AmountForCalculations.Value;
        }
        #endregion accesssors

        #region IInput

        ISequestration IInput.sequestrationParameter
        {
            get
            {
                return this.sequestration as ISequestration;
            }
            set
            {
                this.sequestration = value as Sequestration;
            }
        }

        public bool InternalProduct
        {
            get
            {
                return !NotAnInternalProduct;
            }
            set
            {
                this.NotAnInternalProduct = !value;
            }
        }

        public IParameter CurrentDesignAmount
        {
            get 
            {
                if (this.DesignAmount != null)
                    return this.DesignAmount.CurrentValue;
                else
                    return null;
            }
        }
        
        #endregion
    }
}
