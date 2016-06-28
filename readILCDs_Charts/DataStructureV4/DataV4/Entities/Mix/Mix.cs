using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;



namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// A mix object is a wrapper for blending multiple mixes or pathways together
    /// It allows the user to get weigh averaged results for a mix or blend of multiple pathways or multiple products
    /// </summary>
    [Serializable]
    public class Mix : IMix, IHaveResults, IHaveMetadata, IHaveAPicture, IGREETEntity
    {
        #region Constants
        private static KeyValuePair<string, bool> HeaderEmissions = new KeyValuePair<string, bool>("Emissions", true);
        private static KeyValuePair<string, bool> HeaderEnergy = new KeyValuePair<string, bool>("Energy", true);
        private static KeyValuePair<string, bool> HeaderGeneral = new KeyValuePair<string, bool>("General", true);
        private static KeyValuePair<string, bool> HeaderUrbanEmissions = new KeyValuePair<string, bool>("Urban Emissions", true);
        private static KeyValuePair<string, bool> EmissionsAll = new KeyValuePair<string, bool>("Well to Product", true);
        private static KeyValuePair<string, bool> EnergyAllIncluded = new KeyValuePair<string, bool>("Well to Product", true);
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region ennumerator

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum SharesType { energy, mass, volume };

        #endregion

        #region attributes

        /// <summary>
        /// Lists all the fuel production entities used in the mix
        /// They can be pathways or mixes. Circular references should be avoided
        /// </summary>
        List<FuelProductionEntity> _entities = new List<FuelProductionEntity>();
        /// <summary>
        /// The last calculated mix energy and emissions balance
        /// The results in that Enem, the bottom unit and the normalizingUnit attribute of this class should be the same
        /// </summary>
        CanonicalOutput _mixOutputResults = new CanonicalOutput(null);
        /// <summary>
        /// Notes relevant to that mix
        /// </summary>
        string _notes = "";
        /// <summary>
        /// If true, fixed values will be used for this mix rather than weight averaging the 
        /// fuel production items.
        /// </summary>
        public bool _usesFixedValues = false;
        /// <summary>
        /// Names for this mix that will be used on display
        /// </summary>
        string _name = "";
        /// <summary>
        /// Picture Name for this mix that will be used on display
        /// </summary>
        string _pictureName = "";
        /// <summary>
        /// Header for this mix to be used on display, we stored it in a variable
        /// as it changes often from mix to upstream
        /// </summary>
        string _header = "Mix" + ": ";
        /// <summary>
        /// Unique ID for this mix within the range of mixes IDs for a specific resource 
        /// </summary>
        int _id = -1;
        /// <summary>
        /// Help link associated to that mix
        /// </summary>
        string _help = "";
        /// <summary>
        /// Stores the last iteration number on which this mix was calculated
        /// this is used to save time and not calculated twice the same mix during the same iteration
        /// if that mix is referenced multiple times in the model
        /// </summary>
        public int _lastCalculatedOnIteration = -1;
        /// <summary>
        /// Fixed values to be used for the mix upstream.
        /// Only used if the usedFixedValues is set to True during the calculations
        /// </summary>
        private DefaultValuesIfNoPathway _fixedValues;
        /// <summary>
        /// Prefered functional unit for display results
        /// </summary>
        public FunctionalUnitPreference _functionalUnitResultsDisplay = new FunctionalUnitPreference();
        /// <summary>
        /// Defines how the shares are used for a mix. When all pathways outputs the same resources this doesn't matter
        /// but when we are mixing different products knowing if we are using volumetric, massic or energetic shares 
        /// is very important for the results.
        /// </summary>
        public SharesType _sharesType = SharesType.energy;
        /// <summary>
        /// Output for the mix, defines a unique GUID and resources
        /// </summary>
        public PMOutput output = new PMOutput();
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedBy = "";

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }

        #endregion attributes

        #region constructors

        public Mix(GData data)
        {
            this._id = Convenience.IDs.GetIdUnusedFromTimeStamp(data.MixesData.Keys);
            this._name = "New " + "Pathway Mix" + " " + this._id;
            this._entities = new List<FuelProductionEntity>();
            this._mixOutputResults = new CanonicalOutput(null);
            this._mixOutputResults.Results = new Results();
        }

        public Mix(GData data, XmlNode node, string optionalParamPrefix) : this(data)
        {
            this.FromXmlNode(data, node, optionalParamPrefix);
        }

       /// <summary>
        /// Checks if all the fuel production entities outputs can be converted to the desired shared type
        /// then assign the desired shared type to the normalizing unit for calculations
        /// </summary>
        /// <returns>The normalization unit chosen, empty string if errors occured</returns>
        private uint CheckFuelProductionEntitiesMaterialStates(GData data)
        {
            bool commonState = true;
            uint desiredDim = Units.QuantityList[_sharesType.ToString()].Dim;

            foreach (FuelProductionEntity fuelProductionEntity in _entities)
            {
                if (fuelProductionEntity.Exists(data))
                {
                    if (fuelProductionEntity is PathwayProductionEntity)
                    {
                        
                        if (data.Pathways.KeyExists((fuelProductionEntity as PathwayProductionEntity).PathwayReference))
                        {
                            Pathway path = data.PathwaysData[(fuelProductionEntity as PathwayProductionEntity).PathwayReference];
                            AOutput output = path.getOutputFromProcess(data, (fuelProductionEntity as PathwayProductionEntity).OutputReference);
                            int pathwayOutputResourceId = output.resourceId;
                            uint pathwayFunctionalDim = output.DesignAmount.CurrentValue.Dim;
                            if (data.ResourcesData[pathwayOutputResourceId].CanConvertTo(desiredDim, new LightValue(1.0, pathwayFunctionalDim)) == false)
                            {
                                commonState = false;
                                break;
                            }
                        }
                    }
                    else if (fuelProductionEntity is MixProductionEntity)
                    {
                        Mix mix = data.MixesData[(fuelProductionEntity as MixProductionEntity).MixReference];
                        int mixOutputResourceId = mix.output.ResourceId;
                        uint mixFunctionalDim = data.MixesData.GetMix((fuelProductionEntity as MixProductionEntity).MixReference).CheckFuelProductionEntitiesMaterialStates(data);
                        if (data.ResourcesData[mixOutputResourceId].CanConvertTo(desiredDim, new LightValue(1.0, mixFunctionalDim)) == false)
                        {
                            commonState = false;
                            break;
                        }
                    }
                    else
                        throw new Exception("Unkown fuel production entity type");
                }
            }

            if (commonState)
                return desiredDim;
            else
                return 0;
        }

        #endregion constructors

        #region accessors

        public CanonicalOutput getMainOutputResults()
        {
            return _mixOutputResults;
        }
        /// <summary>
        /// Fixed values to be used for the mix upstream.
        /// Only used if the usedFixedValues is set to True during the calculations
        /// </summary>
        public DefaultValuesIfNoPathway FixedValues
        {
            get { return _fixedValues; }
            set { _fixedValues = value; }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public Results MixBalance
        {
            get
            {
                return _mixOutputResults.Results;
            }
            set
            {
                _mixOutputResults.Results = value;
            }
        }
        #region For ICanHaveFunctionalUnitPreference
        /// <summary>
        /// Returns display unit preference for this process
        /// </summary>
        public FunctionalUnitPreference GetUnitPreference
        {
            get { return this._functionalUnitResultsDisplay; }
        }

        /// <summary>
        /// Returns the Resource id of the resource made by the Mix
        /// </summary>
        public int MainOutputResourceID
        {
            get { return this.output.ResourceId; }
            set { this.output.ResourceId = value; }
        }

        #endregion


        public List<FuelProductionEntity> Entities
        {
            get { return _entities; }
            set { _entities = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name
        {
            get
            { return _name; }
            set { _name = value; }
        }

        public string Header
        {
            get { return _header; }
        }

        public string Help
        {
            get { return _help; }
            set { _help = value; }
        }

        #endregion accessors

        #region methods
        /// <summary>
        /// Check the structural Integrity of the mix and return a string containing messages if errors were detected.
        /// </summary>
        /// <returns></returns>
        public bool CheckIntegrity(GData data, bool showIds, out string errors)
        {
            errors = "";

            if (!(this.FixedValues != null && this._usesFixedValues)
                && (this.Entities == null || this.Entities.Count == 0))
            {
                errors += "WARNING: The mix does not contains any pathways nor mixes \r\n";
                return true;
            }

            int outResource = -1;
            if (this.output != null)
                outResource = this.output.ResourceId;
            if (!data.ResourcesData.ContainsKey(outResource))
                errors += "ERROR: The resource produced by that mix is not defined properly" + Environment.NewLine;

            bool producedResourceIsCompatible = true;
            foreach (FuelProductionEntity entity in this.Entities)
            {
                int producedResource = -1;
                if (entity is PathwayProductionEntity)
                {
                    PathwayProductionEntity pref = entity as PathwayProductionEntity;
                    if (!data.PathwaysData.ContainsKey(pref.PathwayReference))
                        errors += "ERROR: Invalid pathway reference: " + pref.PathwayReference + "\r\n";
                    else
                    {
                        //checking that the feed of the pathway is not the same mix as it's used in
                        //we could take time to find the feed only but checking all vertices is faster and produces cleaner code here
                        foreach (Vertex vertex in data.PathwaysData[pref.PathwayReference].VerticesData.Values)
                        {
                            if (vertex.Type == 2 && vertex.ModelID == this.Id)
                                errors += "ERROR: The pathway \""
                                + data.PathwaysData[pref.PathwayReference].Name + "\" (" + data.PathwaysData[pref.PathwayReference].Id + ") "
                                + "is using his output as the feed \r\n THE CALCULATION WILL NEVER CONVERGE" + "\r\n";
                        }

                        //Check that the output reference used in the PathwayProductionEntity exists in the pathway
                        if (!data.PathwaysData[pref.PathwayReference].OutputsData.Any(o => o.Id == pref.OutputReference))
                        {
                            Guid mainOutput = data.PathwaysData[pref.PathwayReference].MainOutput;
                            if (data.PathwaysData[pref.PathwayReference].OutputsData.Any(o => o.Id == mainOutput))
                            {
                                pref.OutputReference = mainOutput;
                                errors += "FIXED: An output of the pathway " + data.PathwaysData[pref.PathwayReference].Name + " used in this mix, does not exists in the pathway\r\n"
                                    + "This has been fixed by using the main output of the pathway\r\n";
                            }
                            else
                            {
                                errors += "ERROR: An output of the pathway " + data.PathwaysData[pref.PathwayReference].Name + " used in this mix, does not exists in the pathway\r\n"
                                    + "This cannot be fixed automatically as the pathway main output is not defined properly there\r\n";
                            }
                        }
                        else
                        {
                            producedResource = data.PathwaysData[pref.PathwayReference].OutputsData.Single(o => o.Id == pref.OutputReference).ResourceId;
                        }
                    }
                }
                else if (entity is MixProductionEntity)
                {
                    MixProductionEntity matRef = entity as MixProductionEntity;
                    if (!data.MixesData.ContainsKey(matRef.MixReference))
                        errors += "Invalid Pathway Mix (" + matRef.MixReference + ")" + "\r\n";
                    else
                        producedResource = data.MixesData[matRef.MixReference].MainOutputResourceID;
                }

                if (!data.ResourcesData.ContainsKey(producedResource))
                    producedResourceIsCompatible = false;
                else if (data.ResourcesData.ContainsKey(outResource))
                {
                    ResourceData rd = data.ResourcesData[producedResource];
                    ResourceData mixOut = data.ResourcesData[outResource];
                    producedResourceIsCompatible &= mixOut.CompatibilityIds.Contains(rd.Id);
                }
            }

            // Awaiting instruction from David to uncomment the following 2 statements.
            //if (this.CheckFuelProductionEntitiesMaterialStates(data) != Units.QuantityList[_sharesType.ToString()].SIUnitStr)
            //    errors += "The shares for the entities cannot be defined for the current selection of share type";

            if (this.FixedValues != null && this._usesFixedValues)
            {
                if (this.FixedValues.PerOutputAmount.ValueInDefaultUnit <= 0)
                {
                    errors += "The default functional unit for the fixed values upstream is: " + this.FixedValues.PerOutputAmount.ValueInDefaultUnit + ". This parameter should be set to a positive value";
                }
                else
                {
                    double totalEnergy = 0;
                    double totalMass = 0;
                    double totalVolume = 0;
                    foreach (KeyValuePair<int, Parameter> dv in this.FixedValues.Energies)
                    {
                        totalEnergy += data.ResourcesData[dv.Key].ConvertToEnergy(dv.Value.ToLightValue()).Value;
                        totalMass += data.ResourcesData[dv.Key].ConvertToMass(dv.Value.ToLightValue()).Value;
                        totalVolume += data.ResourcesData[dv.Key].ConvertToVolume(dv.Value.ToLightValue()).Value;
                    }

                    bool lowEnergy = totalEnergy < data.ResourcesData[this.output.ResourceId].ConvertToEnergy(this.FixedValues.PerOutputAmount.ToLightValue()).Value;
                    bool lowMass = totalMass < data.ResourcesData[this.output.ResourceId].ConvertToMass(this.FixedValues.PerOutputAmount.ToLightValue()).Value;
                    bool lowVolume = totalVolume < data.ResourcesData[this.output.ResourceId].ConvertToVolume(this.FixedValues.PerOutputAmount.ToLightValue()).Value;

                    if ((lowEnergy || data.ResourcesData[this.output.ResourceId].CanConvertTo(DimensionUtils.ENERGY, this.FixedValues.PerOutputAmount) == false)
                        && (lowMass || data.ResourcesData[this.output.ResourceId].CanConvertTo(DimensionUtils.MASS, this.FixedValues.PerOutputAmount) == false)
                        && (lowVolume || data.ResourcesData[this.output.ResourceId].CanConvertTo(DimensionUtils.VOLUME, this.FixedValues.PerOutputAmount) == false))
                        errors += "The input total energy, mass or volume is lower than the Per Output Of. The model might never converge. Please verify the fixed input amounts";
                }
            }

            if (String.IsNullOrEmpty(errors) == false)
                errors = errors + "\r\n";

            return true;
        }

        public override string ToString()
        {
            return "Pathway Mix: " + this._name;
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode mix = xmlDoc.CreateNode("mix");

            if (this.Discarded)
            {
                mix.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                mix.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                mix.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                mix.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
            }

            mix.Attributes.Append(xmlDoc.CreateAttr("id", _id));
            mix.Attributes.Append(xmlDoc.CreateAttr("name", _name));
            mix.Attributes.Append(xmlDoc.CreateAttr("notes", this.Notes));
            mix.Attributes.Append(xmlDoc.CreateAttr("help", this._help));
            mix.Attributes.Append(xmlDoc.CreateAttr("use_default_values", this._usesFixedValues));
            mix.Attributes.Append( xmlDoc.CreateAttr("share_type", this._sharesType));
            mix.Attributes.Append( xmlDoc.CreateAttr("created_resource", this.output.ResourceId));
            mix.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            mix.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));

            foreach (FuelProductionEntity fp in this._entities)
                mix.AppendChild(fp.ToXmlNode(xmlDoc));

            //unit preferences
            if (_mixOutputResults != null
                && _mixOutputResults.Results != null
                && _mixOutputResults.Results.CustomFunctionalUnitPreference != null)
            {
                _functionalUnitResultsDisplay = _mixOutputResults.Results.CustomFunctionalUnitPreference;
                mix.AppendChild(this._functionalUnitResultsDisplay.ToXmlNode(xmlDoc));
            }
            
            //mix output
            mix.AppendChild(this.output.ToXmlNode(xmlDoc));
            
            if (this.FixedValues != null)
                mix.AppendChild(FixedValues.ToXmlNode(xmlDoc));

            return mix;
        }

        /// <summary>
        /// Compute the normalizing unit according to the Share Type (energy, mass or volume) and the resources involved into the mix. Check that all resources can be converted to that Share Type.
        /// If the results do not exist, create an empty object for it. If the results do exist, do not clear them unless the Share Type has changed since the last calculations.
        /// </summary>
        public void PrepareForNewCalculations(GData data)
        {
            uint normalizingDim = this.CheckFuelProductionEntitiesMaterialStates(data);

            _mixOutputResults = new CanonicalOutput(null);
            _mixOutputResults.Results = new Results();
            _mixOutputResults.Results.ObjectType = Enumerators.ItemType.Pathway_Mix;
            _mixOutputResults.Results.BottomDim = normalizingDim;
            _mixOutputResults.Results.BiongenicCarbonRatio = this._mixOutputResults.MassBiogenicCarbonRatio;
            _mixOutputResults.Results.CustomFunctionalUnitPreference = _functionalUnitResultsDisplay;

            if (this._entities.Count == 0)
                this._mixOutputResults.Results.wellToProductEnem.materialsAmounts.AddFuel(data.ResourcesData[this.output.ResourceId], new LightValue(1, this._mixOutputResults.Results.BottomDim));   
        }

        public XmlNode ToXmlResultsNode(XmlDocument doc, int resourceId)
        {

            //double amountRatio = GetAmountRatio();
           // LightValue CalculatedForOutput = GetCalculatedForOutput(amountRatio);

            XmlNode node = doc.CreateNode("mix");//, doc.CreateAttr("id", this._id), doc.CreateAttr("outputResource", resourceId), doc.CreateAttr("name", this._name), doc.CreateAttr("notes", this._notes), doc.CreateAttr("help", this._help), doc.CreateAttr("functional-unit", CalculatedForOutput));

            //XmlNode em_node = doc.CreateNode("emissions");
            //XmlNode en_node = doc.CreateNode("energy");
            //node.AppendChild(em_node);
            //node.AppendChild(en_node);
            //XmlNode temp_node;
            ////Add emission related results
            //temp_node = doc.CreateNode("life-cycle"); (this._mixBalance.lifeCycleEnem.emissions / amountRatio).AppendToXmlNode(doc, temp_node);
            //temp_node.Attributes.Append(doc.CreateAttr("sum", (this._mixBalance.lifeCycleEnem.emissions / amountRatio).Total()));
            //em_node.AppendChild(temp_node);
            ////Add energy related results
            //temp_node = doc.CreateNode("life-cycle"); (this._mixBalance.lifeCycleEnem.materialsAmounts / amountRatio).AppendToXmlNode(doc, temp_node);
            //temp_node.Attributes.Append(doc.CreateAttr("sum", this._mixBalance.lifeCycleEnem.materialsAmounts.TotalEnergy()));
            //en_node.AppendChild(temp_node);
            return node;
        }

        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            if (node.Attributes["discarded"] != null)
            {
                Discarded = Convert.ToBoolean(node.Attributes["discarded"].Value);
                DiscardedOn = Convert.ToDateTime(node.Attributes["discardedOn"].Value, GData.Nfi);
                DiscarededBy = node.Attributes["discardedBy"].Value;
                DiscardedReason = node.Attributes["discardedReason"].Value;
            }

            this._name = node.Attributes["name"].Value;
            this._id = Convert.ToInt32(node.Attributes["id"].Value);
            if (node.Attributes["use_default_values"] != null)
                this._usesFixedValues = Convert.ToBoolean(node.Attributes["use_default_values"].Value);
            if (node.SelectSingleNode("output") != null)
            {
                this.output = new PMOutput();
                this.output.FromXmlNode(node.SelectSingleNode("output"));
            }
            if (node.Attributes["notes"] != null)
                this._notes = node.Attributes["notes"].Value;
            if (node.Attributes["help"] != null)
                this._help = node.Attributes["help"].Value;
            if (node.Attributes["share_type"] != null)
                this._sharesType = (SharesType)Enum.Parse(typeof(SharesType), node.Attributes["share_type"].Value);

            if (node.Attributes[xmlAttrModifiedOn] != null)
                this.ModifiedOn = node.Attributes[xmlAttrModifiedOn].Value;
            if (node.Attributes[xmlAttrModifiedBy] != null)
                this.ModifiedBy = node.Attributes[xmlAttrModifiedBy].Value;

            //prefered units
            XmlNode prefered_units = node.SelectSingleNode("prefered_functional_unit");
            if (prefered_units != null)
                this._functionalUnitResultsDisplay = new FunctionalUnitPreference(prefered_units);

            int count = 0;
            foreach (XmlNode fuel_production_entity in node.ChildNodes)
            {
                FuelProductionEntity entity;
                if (fuel_production_entity.Name == "pathway")
                {
                    entity = new PathwayProductionEntity(data, fuel_production_entity, optionalParamPrefix + "_mix_" + this.Id + "_path_" + count);
                    this._entities.Add(entity);
                }
                else if (fuel_production_entity.Name == "resource")
                {
                    entity = new MixProductionEntity(data, fuel_production_entity, optionalParamPrefix + "_mix_" + this.Id + "_mix_" + count);
                    this._entities.Add(entity);
                }
                count++;
            }

        }

        /// <summary>
        /// Performs a deep search and find all references to pathways
        /// </summary>
        /// <param name="list">Initial list of production items, when called the first time this should be the production items of the Mix we want to deep search</param>
        /// <param name="mixesData"></param>
        /// <param name="mixPathwaysUsed"></param>
        /// <param name="mixUsed"></param>
        public static void FlattenPathways(List<IProductionItem> list, Dictionary<int, Mix> mixesData, ref List<int> mixPathwaysUsed, ref List<int> mixUsed)
        {
            foreach (IProductionItem prodItem in list)
            {
                if (prodItem.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway && !mixPathwaysUsed.Contains(prodItem.MixOrPathwayId))
                    mixPathwaysUsed.Add(prodItem.MixOrPathwayId);
                else if (prodItem.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
                {
                    if (!mixUsed.Contains(prodItem.MixOrPathwayId))
                        mixUsed.Add(prodItem.MixOrPathwayId);
                    Mix.FlattenPathways(mixesData[prodItem.MixOrPathwayId].FuelProductionEntities, mixesData, ref mixPathwaysUsed, ref mixUsed);
                }
            }
        }
        #endregion methods

        #region IMix

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IProductionItem> FuelProductionEntities
        {
            get
            {
                List<IProductionItem> fuelProductionEntities = new List<IProductionItem>();
                foreach (FuelProductionEntity fuelProductionEntity in this.Entities)
                {
                    fuelProductionEntities.Add(fuelProductionEntity as IProductionItem);
                }
                return fuelProductionEntities;
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }

        #endregion

        #region IHaveResults
        public Dictionary<IIO, Results> GetResults(GData data)
        {
            Dictionary<IIO, Results> mixResults = new Dictionary<IIO, Results>();
            if (this.MixBalance != null)
            {
                Results resultForOutput = new Results();
                resultForOutput.BottomDim = this.MixBalance.BottomDim;
                resultForOutput.CustomFunctionalUnitPreference = this.GetUnitPreference;
                resultForOutput.wellToProductEnem = this.MixBalance.wellToProductEnem;
                resultForOutput.wellToProductUrbanEmission = this.MixBalance.wellToProductUrbanEmission;
                resultForOutput.ObjectID = this._id;
                resultForOutput.BiongenicCarbonRatio = this.MixBalance.BiongenicCarbonRatio;
                resultForOutput.ObjectType = Greet.DataStructureV4.Interfaces.Enumerators.ItemType.Pathway_Mix;
                resultForOutput.onsiteEmissions = this.MixBalance.onsiteEmissions;
                resultForOutput.onsiteResources = this.MixBalance.onsiteResources;
                resultForOutput.onsiteUrbanEmissions = this.MixBalance.onsiteUrbanEmissions;
                mixResults.Add(this.output, resultForOutput);
            }
            return mixResults;
        }
        #endregion

        #region IHaveMetadata Members

        public string ModifiedBy { get { return this.modifiedOn; } set { this.modifiedOn = value; } }

        public string ModifiedOn { get { return this.modifiedBy; } set { this.modifiedBy = value; } }

        #endregion

        #region IMix
        public Dictionary<IIO, IResults> GetUpstreamResults(IData data)
        {
            Dictionary<IIO, IResults> toReturn = new Dictionary<IIO, IResults>();
            Dictionary<IIO, Results> currentResults = this.GetResults(data as GData);
            foreach (KeyValuePair<IIO, Results> pair in currentResults)
                toReturn.Add(pair.Key, pair.Value as IResults);
            return toReturn;
        }
        #endregion

        #region IHaveAPicture
        public string PictureName
        {
            get
            {
                return _pictureName;
            }
            set
            {
                _pictureName = value;
            }
        }
        #endregion

    }
}
