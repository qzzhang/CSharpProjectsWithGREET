using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Exceptions;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public sealed class ResourceData : IGraphRepresented, IComparable, IGroupAvailable, IHaveAPicture, IResource, IHaveMetadata, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region attributes

        #region physical parameters
        /// <summary>
        /// Mass carbon ratio
        /// </summary>
        private Parameter cRatio;
        /// <summary>
        /// Mass sulfur ratio
        /// </summary>
        private ParameterTS sRatio;
        /// <summary>
        /// Density
        /// </summary>
        private Parameter density;
        /// <summary>
        /// Higher heating value
        /// </summary>
        private Parameter heatingValueHhv;
        /// <summary>
        /// Lower heating value
        /// </summary>
        private Parameter heatingValueLhv;
        /// <summary>
        /// Physical state given for STP or the temp and pressure defined in the physical properties
        /// </summary>
        private Resources.PhysicalState state;
        /// <summary>
        /// Market value per mass, volume or energy
        /// </summary>
        private Parameter marketValue;
        /// <summary>
        /// Temperature placeholder, never used in the model yet
        /// </summary>
        private Parameter temperature;
        /// <summary>
        /// Pressure placeholder, never used in the model yet
        /// </summary>
        private Parameter pressure;
        #endregion physical parameters

        #region object properties
        /// <summary>
        /// ID unique among all resources
        /// </summary>
        private int id;
        /// <summary>
        /// Compatible resources that can be blended in a Mix
        /// </summary>
        private List<int> compatibilityIds = new List<int>();
        /// <summary>
        /// Name for the resource
        /// </summary>
        private string name = "";
        /// <summary>
        /// Notes associated with the resource
        /// </summary>
        private string notes = "";
        /// <summary>
        /// Picture name to be used for the resource
        /// </summary>
        private string pictureName = Constants.EmptyPicture;
        /// <summary>
        /// Nicknames that can be used to find the resource when looking in a search box
        /// </summary>
        private List<string> nickNames = new List<string>();
        /// <summary>
        /// Group memberships, used to sum up group energies like 'Petroleum' in the results
        /// </summary>
        private List<int> memberships = new List<int>();
        /// <summary>
        /// If set to true the resource can be used without any upstream in the stationary process editor as an input from Well
        /// Otherwise a pathway upstream must be defined. This allows some consistency check for the user when building a new process
        /// </summary>
        public bool canBePrimaryResource = false;
        /// <summary>
        /// Gases evaporated when a leak/loss is defined, defines which pollutants and their mass share are emitted
        /// </summary>
        public List<EvaporatedGas> evaporatedGases = new List<EvaporatedGas>();
        /// <summary>
        /// If set to true the HeatingValue accessor will return the lower heating value
        /// </summary>
        public bool UseLHV = true;
        /// <summary>
        /// Familly for groupping in the WTP explorer if we want to put that resource under some familly category
        /// </summary>
        private List<string> family = new List<string>();
        /// <summary>
        /// Recovered material ID, this is a legacy feature used in 2013 and prior versions where some lost material was recovered by ocean tankers carrying LNG to powerup their engines
        /// This is not done anymore as in 2014 and this parameter is useless
        /// </summary>
        private int recoveredMaterialId;
        /// <summary>
        /// Frequency count for that resource, used in order to populate the most frequently used resources in some of the editors.
        /// This is probably not the right location for that member, this should be a variable in the data context not in an entity
        /// </summary>
        internal Dictionary<CompositeKeyPair<int, int>, int> freq_count = new Dictionary<CompositeKeyPair<int, int>, int>();
        /// <summary>
        /// Frequency count for that resource, used in order to populate the most frequently used resources in some of the editors.
        /// This is probably not the right location for that member, this should be a variable in the data context not in an entity
        /// </summary>
        public int use_as_input_count = 0;
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedBy = "";
        /// <summary>
        /// If set to True, the resource will not be represented in the GUI so the user cannot create new items using that resource
        /// </summary>
        public bool Discarded { get; set; }
        /// <summary>
        /// Readon for discarding this item
        /// </summary>
        public string DiscardedReason { get; set; }
        /// <summary>
        /// Date at which the item has been discarded
        /// </summary>
        public DateTime DiscardedOn { get; set; }
        /// <summary>
        /// Name of the user that discarded this item
        /// </summary>
        public string DiscarededBy { get; set; }
        /// <summary>
        /// Default quantity in case where this resource is used with an amount of 1 unitess somewhere
        /// This is typicall used for vehicle components such as the mass of one gearbox. If there is a default
        /// quantity, we'll use 1 unit when this object is dragged and dropped as a resource and use that quantity to
        /// match up with the pathway upstream.
        /// </summary>
        internal Parameter unitDefaultQuanity;
        #endregion object properties

        #endregion attributes

        #region constructors

        public ResourceData(GData data, XmlNode node)
        {
            this.FromXmlNode(data, node);
        }

        public ResourceData(GData data)
        {
            this.id = Convenience.IDs.GetIdUnusedFromTimeStamp(data.ResourcesData.Keys.ToArray());
        }

        #endregion constructors

        #region accessors
        public IParameter Density
        {
            get { return (IParameter)density; }
            set { density = (Parameter)value; }
        }

        public IParameter CarbonRatio
        {
            get { return (IParameter)CRatio; }
            set { cRatio = (Parameter)value; }
        }
        public IParameter SulfurRatio
        {
            get
            {
                if (SRatio != null)//check if SRatio is null in order to avoid null reference exceptions
                    return (IParameter)SRatio.CurrentValue;
                else
                    return null;
            }
        }

        public IParameter HigherHeatingValue
        {
            get { return (IParameter)heatingValueHhv; }
            set { heatingValueHhv = (Parameter)value; }
        }
        public IParameter LowerHeatingValue
        {
            get { return (IParameter)heatingValueLhv; }
            set { heatingValueHhv = (Parameter)value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public Resources.PhysicalState State
        {
            get { return state; }
            set { state = value; }
        }

        public override string ToString()
        {
            return this.name;
        }

        [Browsable(false)]
        public List<string> NickNames
        {
            get { return nickNames; }
            set { nickNames = value; }
        }

        /// <summary>
        /// Returns a single string with all the nicknames seperated by comma.
        /// </summary>
        [Browsable(false)]
        public string NickNamesSingleString
        {
            get
            {
                string nick_names = "";
                foreach (String s in this.NickNames)
                {
                    if (nick_names != "")
                        nick_names = nick_names + ", ";
                    nick_names = nick_names + s;
                }
                return nick_names;

            }
        }

        [Browsable(false)]
        public string PictureName
        {
            get { return pictureName; }
            set { pictureName = value; }
        }

        [Browsable(true),
        Category("Ratios"),
        DisplayName("Carbon Ratio")]
        public Parameter CRatio
        {
            get { return cRatio; }
            set { cRatio = value; }
        }
        [Browsable(true),
        Category("Market Value")]
        public Parameter MarketValue
        {
            get { return marketValue; }
            set { marketValue = value; }
        }

        [Browsable(true),
        Category("Density")]
        public Parameter DensityAsParameter
        {
            get { return density; }
            set { density = value; }
        }

        [Browsable(true),
        Category("Conditions")]
        public Parameter Pressure
        {
            get { return pressure; }
            set { pressure = value; }
        }

        [Browsable(true),
        Category("Conditions")]
        public Parameter Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }

        [Browsable(true),
        Category("Heating"),
        DisplayName("Higher Heating Value")]
        public Parameter HeatingValueHhv
        {
            get { return heatingValueHhv; }
            set { heatingValueHhv = value; }
        }

        [Browsable(true),
        Category("Heating"),
        DisplayName("Lower Heating Value")]
        public Parameter HeatingValueLhv
        {
            get { return heatingValueLhv; }
            set { heatingValueLhv = value; }
        }

        /// <summary>
        /// Returns the HHV or the LHV depending on the option selected in the general parameters
        /// </summary>
        [Browsable(false)]
        public Parameter HeatingValue
        {
            get
            {
                if (this.UseLHV)
                    return this.heatingValueLhv;
                else
                    return this.heatingValueHhv;
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<int> CompatibilityIds
        {
            get { return compatibilityIds; }
            set { compatibilityIds = value; }
        }

        [Browsable(true), Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Browsable(false)]
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        [Browsable(true),
        Category("Ratios"),
        DisplayName("Suffer Ratio")]
        public ParameterTS SRatio
        {
            get
            {
                return this.sRatio;
            }
            set
            {
                this.sRatio = value;
            }
        }

        /// <summary>
        /// The top level group for the material to be sorted under
        /// </summary>
        [Browsable(false)]
        public List<string> Family
        {
            get { return this.family; }
            set { this.family = value; }
        }

        [Browsable(false)]
        public List<int> Memberships
        {
            get { return this.memberships; }
            set { this.memberships = value; }
        }

        //THIS DOES NOT HAVE IT"S PLACE HERE
        //[Browsable(false)]
        //public Dictionary<int, Group> AvailableGroups
        //{
        //    get
        //    {
        //        if (this.availableGroups == null && Holder.Project != null)
        //            this.availableGroups = Holder.Project.Dataset.ResourcesData.Groups;
        //        return availableGroups;
        //    }
        //    set { this.availableGroups = value; }
        //}

        public List<IEvaporatedGas> EvaporatedGasess
        {
            get
            {
                List<IEvaporatedGas> returnList = new List<IEvaporatedGas>();
                foreach (EvaporatedGas eg in this.evaporatedGases)
                    returnList.Add(eg as IEvaporatedGas);

                return returnList;
            }

            set
            {
                foreach (IEvaporatedGas iEg in value)
                    this.evaporatedGases.Add(iEg as EvaporatedGas);
            }
        }

        [Browsable(false)]
        public int RecoveredMaterialId
        {
            get
            {
                if (recoveredMaterialId == 0)
                    return this.id;
                else
                    return recoveredMaterialId;
            }
            set { recoveredMaterialId = value; }
        }
        #endregion accessors

        #region methods
        /// <summary>
        /// This method updates a frequency using the source of the inp and the corresponding object_id
        /// </summary>
        /// <param name="inp"></param>
        internal void CountSourceFreq(Input inp)
        {
            this.use_as_input_count += 1;
            if (inp.SourceType == Enumerators.SourceType.Mix)
            {
                CompositeKeyPair<int, int> key = new CompositeKeyPair<int, int>((int)inp.SourceType, inp.SourceMixOrPathwayID);
                if (this.freq_count.ContainsKey(key))
                    this.freq_count[key] += 1;
                else
                    this.freq_count[key] = 1;
            }
            if (inp.SourceType == Enumerators.SourceType.Pathway)
            {
                CompositeKeyPair<int, int> key = new CompositeKeyPair<int, int>((int)inp.SourceType, inp.SourceMixOrPathwayID);
                if (this.freq_count.ContainsKey(key))
                    this.freq_count[key] += 1;
                else
                    this.freq_count[key] = 1;
            }
            foreach (Enumerators.SourceType st in Enum.GetValues(typeof(Enumerators.SourceType)))
            {
                if (st == Enumerators.SourceType.Mix || st == Enumerators.SourceType.Pathway || inp.SourceType != st)
                    continue;
                CompositeKeyPair<int, int> key = new CompositeKeyPair<int, int>((int)st, 0);
                if (this.freq_count.ContainsKey(key))
                    this.freq_count[key] += 1;
                else
                    this.freq_count[key] = 1;
            }
        }
        /// <summary>
        /// Get the source and associated object id which is most frequently used for an input. Make sure you call UpdateSourceFrequencies some time before you call this function
        /// </summary>
        /// <param name="source"></param>
        /// <param name="object_id">Id of the corresponding object. If source is Mix then object_id is id of the Corresponding mixe, the same logic for pathway.
        /// However, if source is well, previous or a feed, then object_id is meaningless is is set to 0
        /// </param>
        public void GetMostFrequentSource(out Enumerators.SourceType source, out int object_id)
        {
            if (this.freq_count != null && this.freq_count.Count() > 0)
            {
                CompositeKeyPair<int, int> key_of_max = this.freq_count.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                source = (Enumerators.SourceType)key_of_max.Key1;
                object_id = key_of_max.Key2;
            }
            else
            {
                source = Enumerators.SourceType.Mix;
                object_id = -1;
            }
        }
        /// <summary>
        /// Returns the best unit expression to be used when creating a new parameter or value of that resource.
        /// This method checks what kind of resource it is (Physical state) as well as physical properties of the resources
        /// and returns the most appropriate unit so that unit conversion and calculations can be performed
        /// </summary>
        /// <param name="returnSI">If set to false, retured the prefered unit rather than the SI unit</param>
        /// <returns>Unit expression to be used by default</returns>
        public string DefaultQuantityExpression(bool returnSI = true)
        {
            AQuantity preferedQuantity = null;
            if (this.state == Resources.PhysicalState.energy)
                preferedQuantity = Units.QuantityList.ByDim(DimensionUtils.ENERGY);
            else if (this.state == Resources.PhysicalState.item)
            {
                preferedQuantity = Units.QuantityList.ByDim(DimensionUtils.RATIO);
                if (preferedQuantity.Units.Any(un => un.Expression == "item")) //hardcoded unit
                    return "item";//hardcoded unit
            }
            else
            {
                LightValue mass = new LightValue(0.0, DimensionUtils.MASS);
                LightValue volume = new LightValue(0.0, DimensionUtils.VOLUME);
                LightValue energy = new LightValue(0.0, DimensionUtils.ENERGY);

                int massVote = 0;
                int volumeVote = 0;
                int energyVote = 0;

                if (this.CanConvertTo(DimensionUtils.ENERGY, mass) == false)
                    massVote++;
                else
                    energyVote++;
                if (this.CanConvertTo(DimensionUtils.ENERGY, volume) == false)
                    volumeVote++;
                else
                    energyVote++;

                if (energyVote >= massVote && energyVote >= volumeVote)
                    preferedQuantity = Units.QuantityList.ByDim(DimensionUtils.ENERGY);
                else if (massVote >= energyVote && massVote >= volumeVote)
                    preferedQuantity = Units.QuantityList.ByDim(DimensionUtils.MASS);
                else if (volumeVote >= energyVote && volumeVote >= massVote)
                    preferedQuantity = Units.QuantityList.ByDim(DimensionUtils.VOLUME);
                else
                    throw new Exception("Exception in determining the best quantity for the resource " + name);
            }

            if (returnSI)
                return preferedQuantity.SiUnit.Expression;
            else
                return preferedQuantity.Units[preferedQuantity.PreferedUnitIdx].Expression;
        }
        /// <summary>
        /// Returns TRUE if an amount of that material can be a mass
        /// </summary>
        /// <returns></returns>
        public bool CanBeAMass()
        {
            return this.CanConvertTo(DimensionUtils.MASS, new LightValue(1, this.DefaultQuantityExpression()));
        }

        public int CompareTo(object otherMaterial)
        {
            if (otherMaterial is ResourceData)
                return this.name.CompareTo((otherMaterial as ResourceData).name);
            else
                return this.name.CompareTo(otherMaterial.ToString());
        }

        internal bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }
        /// <summary>
        /// Converts the material object to an xmlNode to store in the data file
        /// </summary>
        /// <param name="xmlDoc">the docuement, needed for namespaceURI</param>
        /// <returns>An Xml node which represents the material object</returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {
                XmlNode material = xmlDoc.CreateNode("resource");

                if (this.Discarded)
                {
                    material.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                    material.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                    material.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                    material.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
                }

                material.Attributes.Append(xmlDoc.CreateAttr("can_be_primary", this.canBePrimaryResource));
                if (cRatio != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("c_ratio", this.cRatio));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("c_ratio"));
                if (marketValue != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("market_value", this.marketValue));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("market_value"));
                if (this.density != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("density", this.density));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("density"));
                if (this.heatingValueHhv != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("heating_value_hhv", this.heatingValueHhv));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("heating_value_hhv"));
                if (heatingValueLhv != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("heating_value_lhv", this.heatingValueLhv));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("heating_value_lhv"));
                if (this.temperature != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("temperature", this.temperature));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("temperature"));
                if (this.pressure != null)
                    material.Attributes.Append(xmlDoc.CreateAttr("pressure", this.pressure));
                else
                    material.Attributes.Append(xmlDoc.CreateAttribute("pressure"));

                material.Attributes.Append(xmlDoc.CreateAttr("id", id));
                material.Attributes.Append(xmlDoc.CreateAttr("name", name));
                material.Attributes.Append(xmlDoc.CreateAttr("notes", notes));
                material.Attributes.Append(xmlDoc.CreateAttr("picture", pictureName));

                if (sRatio != null)
                    if (((ParameterTS)sRatio).Count == 1)
                        material.Attributes.Append(xmlDoc.CreateAttr("s_ratio", this.sRatio.CurrentValue));
                    else if (((ParameterTS)sRatio).Count > 1)
                        material.AppendChild(sRatio.ToXmlNode(xmlDoc, "s_ratio"));
                    else
                        material.Attributes.Append(xmlDoc.CreateAttribute("s_ratio"));
                material.Attributes.Append(xmlDoc.CreateAttr("state", state.ToString()));
                material.Attributes.Append(xmlDoc.CreateAttr("family", string.Join(",", family.ToArray())));

                material.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
                material.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));

                foreach (string nname in this.nickNames)
                    material.AppendChild(xmlDoc.CreateNode("nick_name", xmlDoc.CreateTextNode(nname)));

                foreach (int mem in this.memberships)
                    material.AppendChild(xmlDoc.CreateNode("membership", xmlDoc.CreateAttr("group_id", mem)));

                foreach (int comp in this.compatibilityIds)
                    material.AppendChild(xmlDoc.CreateNode("compatibility", xmlDoc.CreateAttr("mat_id", comp)));

                if (recoveredMaterialId != 0)
                    material.AppendChild(xmlDoc.CreateNode("recovered", xmlDoc.CreateAttr("material_id", recoveredMaterialId)));

                if (this.evaporatedGases.Count != 0)
                {
                    XmlNode evaporation = xmlDoc.CreateNode("evaporation");
                    foreach (EvaporatedGas gas_id in this.evaporatedGases)
                    {
                        XmlNode gas_evap = gas_id.ToXmlNode(xmlDoc);
                        evaporation.AppendChild(gas_evap);
                    }
                    material.AppendChild(evaporation);
                }

                return material;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Creates a resource object from an xmlNode in the data file
        /// </summary>
        /// <param name="node">the node to create the resource from</param>
        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status = "";
            try
            {
                if (node.Attributes["discarded"] != null)
                {
                    Discarded = Convert.ToBoolean(node.Attributes["discarded"].Value);
                    DiscardedOn = Convert.ToDateTime(node.Attributes["discardedOn"].Value, GData.Nfi);
                    DiscarededBy = node.Attributes["discardedBy"].Value;
                    DiscardedReason = node.Attributes["discardedReason"].Value;
                }

                #region building

                //mandatory attributes
                status = "Reading name";
                this.name = node.Attributes["name"].Value;
                status = "Reading id";
                this.id = Convert.ToInt32(node.Attributes["id"].Value);

                //optional attributes
                status = "Reading physical state";
                if (node.Attributes["state"].NotNullNOrEmpty())
                    this.state = (Resources.PhysicalState)Enum.Parse(typeof(Resources.PhysicalState), node.Attributes["state"].Value, true);
                status = "Reading c_ratio";
                if (node.Attributes["c_ratio"].NotNullNOrEmpty())
                    this.cRatio = data.ParametersData.CreateRegisteredParameter(node.Attributes["c_ratio"], "res_" + this.id + "_cratio");
                status = "Reading market_value";
                if (node.Attributes["market_value"].NotNullNOrEmpty())
                    this.marketValue = data.ParametersData.CreateRegisteredParameter(node.Attributes["market_value"], "res_" + this.id + "_mkval");
                status = "Reading family";
                if (node.Attributes["family"].NotNullNOrEmpty())
                    this.family = node.Attributes["family"].Value.Split(',').ToList<string>();
                status = "Reading density";
                if (node.Attributes["density"].NotNullNOrEmpty())
                    this.density = data.ParametersData.CreateRegisteredParameter(node.Attributes["density"], "res_" + this.id + "_density");
                status = "Reading hhv";
                if (node.Attributes["heating_value_hhv"].NotNullNOrEmpty())
                    this.heatingValueHhv = data.ParametersData.CreateRegisteredParameter(node.Attributes["heating_value_hhv"], "res_" + this.id + "_hhv");
                status = "Reading lhv";
                if (node.Attributes["heating_value_lhv"].NotNullNOrEmpty())
                    this.heatingValueLhv = data.ParametersData.CreateRegisteredParameter(node.Attributes["heating_value_lhv"], "res_" + this.id + "_lhv");
                status = "Reading s ratio";
                if (node.Attributes["notes"] != null)
                    this.notes = node.Attributes["notes"].Value;
                status = "Reading temperature";
                if (node.Attributes["temperature"].NotNullNOrEmpty())
                    this.temperature = data.ParametersData.CreateRegisteredParameter(node.Attributes["temperature"], "res_" + this.id + "_temp");
                status = "Reading pressure";
                if (node.Attributes["pressure"].NotNullNOrEmpty())
                    this.pressure = data.ParametersData.CreateRegisteredParameter(node.Attributes["pressure"], "res_" + this.id + "_pres");
                status = "Reading can be primary";
                if (node.Attributes["can_be_primary"].NotNullNOrEmpty())
                    this.canBePrimaryResource = Convert.ToBoolean(node.Attributes["can_be_primary"].Value);

                if (node.SelectSingleNode("s_ratio") != null)
                    this.sRatio = new ParameterTS(data, node.SelectSingleNode("s_ratio"), "res_" + this.id + "_sratio");
                else if (node.Attributes["s_ratio"].NotNullNOrEmpty())
                {
                    this.sRatio = new ParameterTS();
                    this.sRatio.Add(0, data.ParametersData.CreateRegisteredParameter(node.Attributes["s_ratio"], "res_" + this.id + "_sratio"));
                    this.sRatio._notes = "SRatio";
                }
                else if (node.SelectSingleNode("s_ratio") != null)
                {
                    this.sRatio = new ParameterTS(data, node.SelectSingleNode("s_ratio"), "res_" + this.id + "_sratio");
                    this.sRatio._notes = "SRatio";
                }
                status = "Reading picture";
                if (node.Attributes["picture"].NotNullNOrEmpty())
                    this.pictureName = node.Attributes["picture"].Value;

                #endregion building

                #region nicknames
                foreach (XmlNode nnode in node.SelectNodes("nick_name"))
                {
                    this.nickNames.Add(nnode.InnerText);
                }
                #endregion nicknames

                #region memberships
                foreach (XmlNode nnode in node.SelectNodes("membership"))
                {
                    int group_id = Convert.ToInt32(nnode.Attributes["group_id"].Value);
                    if (this.memberships.Contains(group_id) == false)
                        this.memberships.Add(group_id);
                }
                #endregion memberships

                #region compatability
                foreach (XmlNode nnode in node.SelectNodes("compatibility"))
                {
                    int mat_id = Convert.ToInt32(nnode.Attributes["mat_id"].Value);
                    this.compatibilityIds.Add(mat_id);
                }
                #endregion compatability

                #region evaporation

                foreach (XmlNode nnode in node.SelectNodes("evaporation/gas"))
                {
                    EvaporatedGas evap = new EvaporatedGas(data, nnode, "res_" + this.id + "_evapgas");
                    this.evaporatedGases.Add(evap);
                }

                #endregion evaportation

                #region Recovery

                if (node.SelectSingleNode("recovered") != null && node.SelectSingleNode("recovered").Attributes["material_id"] != null)
                {
                    recoveredMaterialId = Convert.ToInt32(node.SelectSingleNode("recovered").Attributes["material_id"].Value);
                }

                #endregion evaportation


                if (node.Attributes[xmlAttrModifiedOn] != null)
                    this.ModifiedOn = node.Attributes[xmlAttrModifiedOn].Value;
                if (node.Attributes[xmlAttrModifiedBy] != null)
                    this.ModifiedBy = node.Attributes[xmlAttrModifiedBy].Value;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 58:" + node.OwnerDocument.BaseURI + "\r\n" + node.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }
        /// <summary>
        /// This method return the amount of carbon contained in the "amount" units of the material.
        /// The result must kave [kilograms] dimenstion
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public LightValue CarbonContent(Parameter amount)
        {
            return this.cRatio * this.ConvertTo(DimensionUtils.MASS, new LightValue(amount.ValueInDefaultUnit, amount.Dim));
        }
        /// <summary>
        /// This method return the amount of carbon contained in the "amount" units of the material.
        /// The result must kave [kilograms] dimenstion
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public LightValue CarbonContent(LightValue amount)
        {
            return this.cRatio * this.ConvertTo(DimensionUtils.MASS, amount);
        }
        /// <summary>
        /// This method return the amount of carbon contained in the "amount" units of the material.
        /// The result must kave [kilograms] dimenstion
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public LightValue SulfurContent(LightValue amount)
        {
            return this.sRatio.CurrentValue * this.ConvertTo(DimensionUtils.MASS, amount);
        }

        #endregion methods

        #region conversion

        #region to energy

        /// <summary>
        /// Take the DoubleValue, reads the value and the unit, try to convert it into a Energy
        /// depending of the availabe inputs parameters ( HV, density )
        /// </summary>
        /// <param name="val">The double value to convert</param>
        /// <returns></returns>
        public LightValue ConvertToEnergy(LightValue val)
        {
            if (val.Value != 0)
            {
                if (val.Dim == DimensionUtils.ENERGY)//hard_unit
                    return new LightValue(val.Value, val.Dim);
                else if (val.Dim == DimensionUtils.MASS)//hard_unit
                {
                    if (this.HeatingValue != null && DimensionUtils.Plus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.ENERGY) //HARDCODED
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        return val * this.HeatingValue;
                    }
                    else if (this.HeatingValue != null && this.DensityAsParameter != null && this.DensityAsParameter.ValueInDefaultUnit != 0
                        && DimensionUtils.Plus(val.Dim, DimensionUtils.Minus(this.HeatingValue.Dim, this.DensityAsParameter.Dim)) == DimensionUtils.ENERGY) //hard_unit TESTED
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        if (Double.IsNaN(this.DensityAsParameter.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        return new LightValue((val.Value * this.HeatingValue.ValueInDefaultUnit) / this.density.ValueInDefaultUnit, DimensionUtils.ENERGY);
                    }
                    else
                        return new LightValue(0.0, DimensionUtils.ENERGY);//hard_unit
                }
                else if (val.Dim == DimensionUtils.VOLUME)//hard_unit
                {
                    if (this.HeatingValue != null && DimensionUtils.Plus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.ENERGY)
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        return new LightValue(val.Value * this.HeatingValue.ValueInDefaultUnit, DimensionUtils.ENERGY);
                    }
                    else if (this.HeatingValue != null && this.density != null
                       && DimensionUtils.Plus(val.Dim, DimensionUtils.Plus(this.HeatingValue.Dim, this.DensityAsParameter.Dim)) == DimensionUtils.ENERGY) //hard_unit
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        if (Double.IsNaN(this.DensityAsParameter.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        return new LightValue(val.Value * this.HeatingValue.ValueInDefaultUnit * this.density.ValueInDefaultUnit, DimensionUtils.ENERGY);
                    }
                    else
                        return new LightValue(0.0, DimensionUtils.ENERGY);//hard_unit
                }
                else if (val.Dim == DimensionUtils.CURRENCY)
                {
                    if (this.marketValue != null)
                    {
                        if (Double.IsNaN(this.MarketValue.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.ENERGY)
                            return val / this.marketValue;
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.MASS)
                            return this.ConvertToEnergy(val / this.marketValue); //recursive way is simpler than enumerating all cases...
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.VOLUME)
                            return this.ConvertToEnergy(val / this.marketValue);
                        else
                            return new LightValue(0.0, DimensionUtils.ENERGY);//hard_unit
                    }
                    return new LightValue(0.0, DimensionUtils.ENERGY);//hard_unit
                }
                else
                    throw new UnitConversionException("Cannot convert a value in " + DimensionUtils.ToMLTh(val.Dim) + " to an energy value");
            }
            else
                return new LightValue(0.0, DimensionUtils.ENERGY);//hard_unit
        }

        /// <summary>
        /// This method is used to convert a Enem balance where the bottom unit is not energy into a balance where the bottom unit is energy
        /// </summary>
        /// <param name="balance">Balance to convert for 1 joule which is actually not</param>
        /// <returns>The balance equivalent for 1 joule</returns>
        public Enem ConvertBottomToEnergy(Enem balance)
        {
            if (balance.BottomDim != DimensionUtils.ENERGY)//hard_unit
            {
                //gets the amount ratio between the current unit and a joule of this same material
                double amount_ratio = 1;
                LightValue val = new LightValue(1.0, balance.BottomDim);
                Enem ma = new Enem();
                if (this.CanConvertTo(DimensionUtils.ENERGY, val))//hard_unit
                {
                    LightValue converted = this.ConvertTo(DimensionUtils.ENERGY, val);//hard_unit
                    amount_ratio = val.Value / converted.Value;
                    ma = balance * amount_ratio;
                    ma.BottomDim = DimensionUtils.ENERGY;//hard_unit
                    return ma;
                }
                else
                {
                    ma.BottomDim = DimensionUtils.ENERGY;//hard_unit
                    return ma;
                }
            }
            else
                return balance;
        }
        /// <summary>
        /// This method is used to convert a EmissionResults balance where the bottom unit is not energy into a balance where the bottom unit is energy
        /// </summary>
        /// <param name="balance">Balance to convert for 1 joule which is actually not</param>
        /// <returns>The balance equivalent for 1 joule</returns>
        public EmissionAmounts ConvertToEnergy(EmissionAmounts balance)
        {
            if (balance.BottomDim != DimensionUtils.ENERGY)//hard_unit
            {
                //gets the amount ratio between the current unit and a joule of this same material
                double amount_ratio = 1;
                LightValue val = new LightValue(1.0, balance.BottomDim);
                EmissionAmounts ma = new EmissionAmounts();
                if (this.CanConvertTo(DimensionUtils.ENERGY, val))//hard_unit
                {
                    LightValue converted = this.ConvertTo(DimensionUtils.ENERGY, val);//hard_unit
                    amount_ratio = val.Value / converted.Value;
                    ma = balance * amount_ratio;
                    ma.BottomDim = DimensionUtils.ENERGY;//hard_unit
                    return ma;
                }
                else
                {
                    ma.BottomDim = DimensionUtils.ENERGY;//hard_unit
                    return ma;
                }
            }
            else
                return balance;
        }
        #endregion

        #region to volume

        /// <summary>
        /// Take the DoubleValue, reads the value and the unit, try to convert it into a Volume
        /// depending of the availabe inputs parameters ( HV, density )
        /// </summary>
        /// <param name="val">The double value to convert</param>
        /// <returns></returns>
        public LightValue ConvertToVolume(LightValue val)
        {
            if (val.Value != 0)
            {
                if (val.Dim == DimensionUtils.VOLUME)//hard_unit
                    return new LightValue(val.Value, val.Dim);
                else if (val.Dim == DimensionUtils.ENERGY)//hard_unit
                {
                    if (this.HeatingValue != null && this.density != null && this.HeatingValue.ValueInDefaultUnit != 0 && this.DensityAsParameter.ValueInDefaultUnit != 0
                        && DimensionUtils.Minus(val.Dim, DimensionUtils.Plus(this.density.Dim, this.HeatingValue.Dim)) == DimensionUtils.VOLUME)//hard_unit
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        if (Double.IsNaN(this.DensityAsParameter.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        return val / (this.density * this.HeatingValue);
                    }
                    else if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0
                       && DimensionUtils.Minus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.VOLUME)
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        return val / this.HeatingValue;
                    }
                    else
                        return new LightValue(0.0, DimensionUtils.VOLUME);//hard_unit
                }
                else if (val.Dim == DimensionUtils.MASS)//hard_unit
                {
                    if (this.density != null && this.density.ValueInDefaultUnit != 0)
                    {
                        if (Double.IsNaN(this.DensityAsParameter.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        return val / this.density;
                    }
                    else
                        return new LightValue(0.0, DimensionUtils.VOLUME);//hard_unit
                }
                else if (val.Dim == DimensionUtils.CURRENCY)
                {
                    if (this.marketValue != null)
                    {
                        if (Double.IsNaN(this.MarketValue.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.VOLUME)
                            return val / this.marketValue;
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.ENERGY)
                            return this.ConvertToVolume(val / this.marketValue); //recursive way is simpler than enumerating all cases...
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.MASS)
                            return this.ConvertToVolume(val / this.marketValue);
                        else
                            return new LightValue(0.0, DimensionUtils.VOLUME);//hard_unit
                    }
                    return new LightValue(0.0, DimensionUtils.VOLUME);//hard_unit
                }
                else
                    return new LightValue(0.0, DimensionUtils.VOLUME);//hard_unit
            }
            else
                return new LightValue(0.0, DimensionUtils.VOLUME);//hard_unit
        }
        #endregion

        #region to mass

        /// <summary>
        /// Take the DoubleValue, reads the value and the unit, try to convert it into a Mass
        /// depending of the availabe inputs parameters ( HV, density )
        /// </summary>
        /// <param name="val">The double value to convert</param>
        /// <returns></returns>
        public LightValue ConvertToMass(LightValue val)
        {
            if (val.Value != 0)
            {
                if (val.Dim == DimensionUtils.MASS)//hard_unit
                    return new LightValue(val.Value, val.Dim);
                else if (val.Dim == DimensionUtils.ENERGY)//hard_unit
                {
                    if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0
                        && DimensionUtils.Minus(val.Dim, HeatingValue.Dim) == DimensionUtils.MASS) //hard_unit
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        return new LightValue(val.Value / this.HeatingValue.ValueInDefaultUnit, DimensionUtils.MASS);
                    }
                    else if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0 && this.DensityAsParameter != null && this.DensityAsParameter.ValueInDefaultUnit != 0
                       && DimensionUtils.Plus(val.Dim, DimensionUtils.Minus(density.Dim, HeatingValue.Dim)) == DimensionUtils.MASS)
                    {
                        if (Double.IsNaN(this.HeatingValue.ValueInDefaultUnit))
                            throw new HeatingValueNANException(this.name + " has a NaN heating value");
                        if (Double.IsNaN(this.DensityAsParameter.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        return new LightValue(val.Value * this.density.ValueInDefaultUnit / this.HeatingValue.ValueInDefaultUnit, DimensionUtils.MASS);
                    }
                    else
                        return new LightValue(0.0, DimensionUtils.MASS);//hard_unit
                }
                else if (val.Dim == DimensionUtils.VOLUME)//hard_unit
                {
                    if (this.density != null)
                    {
                        if (Double.IsNaN(this.DensityAsParameter.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        return new LightValue(val.Value * this.density.ValueInDefaultUnit, DimensionUtils.MASS);
                    }
                    else
                        return new LightValue(0.0, DimensionUtils.MASS);//hard_unit
                }
                else if (val.Dim == DimensionUtils.CURRENCY)
                {
                    if (this.marketValue != null)
                    {
                        if (Double.IsNaN(this.MarketValue.ValueInDefaultUnit))
                            throw new DensityValueNANException(this.name + " has a NaN density");
                        if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.MASS)
                            return val / this.marketValue;
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.VOLUME)
                            return this.ConvertToMass(val / this.marketValue); //recursive way is simpler than enumerating all cases...
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.ENERGY)
                            return this.ConvertToMass(val / this.marketValue);
                        else
                            return new LightValue(0.0, DimensionUtils.MASS);//hard_unit
                    }
                    return new LightValue(0.0, DimensionUtils.MASS);//hard_unit
                }
                else
                    return new LightValue(0.0, DimensionUtils.MASS);//hard_unit
            }
            else
            {
                return new LightValue(0.0, DimensionUtils.MASS);//hard_unit
            }
        }

        #endregion

        #region to market value
        /// <summary>
        /// Converts the val to Market Value
        /// </summary>
        /// <param name="val">The double value to convert</param>
        /// <returns></returns>
        public LightValue ConvertToMarketValue(LightValue val)
        {
            if (this.marketValue != null && DimensionUtils.Plus(val.Dim, this.marketValue.Dim) == DimensionUtils.CURRENCY)
                return val * this.marketValue;
            else if (this.marketValue != null
                && DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.ENERGY) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.ENERGY, val))
                return this.ConvertTo(DimensionUtils.ENERGY, val) * this.marketValue;
            else if (this.marketValue != null
                && DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.MASS) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.MASS, val))
                return this.ConvertTo(DimensionUtils.MASS, val) * this.marketValue;
            else if (this.marketValue != null
                && DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.VOLUME) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.VOLUME, val))
                return this.ConvertTo(DimensionUtils.VOLUME, val) * this.marketValue;
            else
                throw new UnitConversionException("Cannot convert this value to an currency value");
        }

        #endregion

        #region others

        #region boolean tests

        /// <summary>
        /// Returns a boolean indicating if that conversion can be performed
        /// </summary>
        /// <param name="unit_name">Expects "joules", "kilograms", "cu_meters" or "us_dollar"</param>
        /// <param name="val">Parameter to be converted</param>
        /// <returns>True if conversion can be performed</returns>
        [Obsolete("This method is obsolete and one should now use the method CanConvertTo(uint toUnitDim, Parameter val)")]
        public bool CanConvertTo(string to_unit_name, Parameter val)
        {
            if (to_unit_name == "joules") //hard_unit
                return this.CanConvertTo(DimensionUtils.ENERGY, val);
            else if (to_unit_name == "kilograms")//hard_unit
                return this.CanConvertTo(DimensionUtils.MASS, val);
            else if (to_unit_name == "cu_meters")//hard_unit
                return this.CanConvertTo(DimensionUtils.VOLUME, val);
            else if (to_unit_name == "us_dollar")
                return this.CanConvertTo(DimensionUtils.CURRENCY, val);
            else
                throw new Exception("Unknown group name, cannot convert");
        }

        /// <summary>
        /// Returns a boolean indicating if that conversion can be performed
        /// </summary>
        /// <param name="unit_name">Expects an unsigned int representing a quantity of energy, mass, volume or currency</param>
        /// <param name="val">Parameter to be converted</param>
        /// <returns>True if conversion can be performed</returns>
        public bool CanConvertTo(uint to_unit_name, Parameter val)
        {
            if (to_unit_name == DimensionUtils.ENERGY)
            {
                #region toEnergy
                if (val.Dim == DimensionUtils.ENERGY)
                    return true;
                else if (val.Dim == DimensionUtils.MASS)
                {
                    if (this.HeatingValue != null && DimensionUtils.Plus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.ENERGY)
                        return true;
                    else if (this.HeatingValue != null && this.DensityAsParameter != null && this.DensityAsParameter.ValueInDefaultUnit != 0
                        && DimensionUtils.Plus(val.Dim, DimensionUtils.Minus(this.HeatingValue.Dim, this.DensityAsParameter.Dim)) == DimensionUtils.ENERGY)
                        return true;
                }
                else if (val.Dim == DimensionUtils.VOLUME)
                {
                    if (this.HeatingValue != null && DimensionUtils.Plus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.ENERGY)
                        return true;
                    else if (this.HeatingValue != null && this.density != null
                       && DimensionUtils.Plus(val.Dim, DimensionUtils.Plus(this.HeatingValue.Dim, this.DensityAsParameter.Dim)) == DimensionUtils.ENERGY)
                        return true;
                }

                return false;
                #endregion
            }
            else if (to_unit_name == DimensionUtils.MASS)
            {
                #region toMass
                if (val.Dim == DimensionUtils.MASS)
                    return true;
                else if (val.Dim == DimensionUtils.ENERGY)
                {
                    if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0
                        && DimensionUtils.Minus(val.Dim, HeatingValue.Dim) == DimensionUtils.MASS) //hard_unit
                        return true;
                    else if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0 && this.DensityAsParameter != null && this.DensityAsParameter.ValueInDefaultUnit != 0
                       && DimensionUtils.Plus(val.Dim, DimensionUtils.Minus(density.Dim, HeatingValue.Dim)) == DimensionUtils.MASS)
                        return true;
                }
                else if (val.Dim == DimensionUtils.VOLUME)
                {
                    if (this.density != null)
                        return true;
                }

                return false;
                #endregion
            }
            else if (to_unit_name == DimensionUtils.VOLUME)//hard_unit
            {
                #region toVolume
                if (val.Dim == DimensionUtils.VOLUME)
                    return true;
                else if (val.Dim == DimensionUtils.ENERGY)//hard_unit
                {

                    if (this.HeatingValue != null && this.density != null && this.HeatingValue.ValueInDefaultUnit != 0 && this.DensityAsParameter.ValueInDefaultUnit != 0
                            && DimensionUtils.Minus(val.Dim, DimensionUtils.Plus(this.density.Dim, this.HeatingValue.Dim)) == DimensionUtils.VOLUME)//hard_unit
                        return true;
                    else if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0
                       && DimensionUtils.Minus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.VOLUME)
                        return true;
                }
                else if (val.Dim == DimensionUtils.MASS)//hard_unit
                {
                    if (this.density != null && this.density.ValueInDefaultUnit != 0)
                        return true;
                }

                return false;
                #endregion
            }
            else if (to_unit_name == DimensionUtils.CURRENCY)//hard_unit
            {
                #region toCurrency
                if (this.marketValue != null && DimensionUtils.Plus(val.Dim, this.marketValue.Dim) == DimensionUtils.CURRENCY)
                    return true;
                else if (this.marketValue != null &&
                (DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.ENERGY) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.ENERGY, val)
                    || DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.MASS) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.MASS, val)
                    || DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.VOLUME) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.VOLUME, val)))
                    return true;

                return false;
                #endregion
            }
            else
                throw new Exception("Unkown unit dimension, cannot convert");
        }

        /// <summary>
        /// Returns a boolean indicating if that conversion can be performed
        /// </summary>
        /// <param name="unit_name">Expects "joules", "kilograms", "cu_meters" or "us_dollar"</param>
        /// <param name="val">LightValue to be converted</param>
        /// <returns>True if conversion can be performed</returns>
        [Obsolete("This method is obsolete and one should now use the method CanConvertTo(uint toUnitDim, LightValue val)")]
        public bool CanConvertTo(string toUnitName, LightValue val)
        {
            if (toUnitName == "joules") //hard_unit
                return this.CanConvertTo(DimensionUtils.ENERGY, val);
            else if (toUnitName == "kilograms")//hard_unit
                return this.CanConvertTo(DimensionUtils.MASS, val);
            else if (toUnitName == "cu_meters")//hard_unit
                return this.CanConvertTo(DimensionUtils.VOLUME, val);
            else if (toUnitName == "us_dollar")
                return this.CanConvertTo(DimensionUtils.CURRENCY, val);
            else
                throw new Exception("Unknown group name, cannot convert");
        }

        /// <summary>
        /// Returns a boolean indicating if that conversion can be performed
        /// </summary>
        /// <param name="unit_name">Expects an unsigned int representing a quantity of energy, mass, volume or currency</param>
        /// <param name="val">LightValue to be converted</param>
        /// <returns>True if conversion can be performed</returns>
        public bool CanConvertTo(uint toUnitDim, LightValue val)
        {
            if (toUnitDim == DimensionUtils.ENERGY)
            {
                #region toEnergy
                if (val.Dim == DimensionUtils.ENERGY)
                    return true;
                else if (val.Dim == DimensionUtils.MASS)
                {
                    if (this.HeatingValue != null && DimensionUtils.Plus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.ENERGY)
                        return true;
                    else if (this.HeatingValue != null && this.DensityAsParameter != null && this.DensityAsParameter.ValueInDefaultUnit != 0
                        && DimensionUtils.Plus(val.Dim, DimensionUtils.Minus(this.HeatingValue.Dim, this.DensityAsParameter.Dim)) == DimensionUtils.ENERGY)
                        return true;
                }
                else if (val.Dim == DimensionUtils.VOLUME)
                {
                    if (this.HeatingValue != null && DimensionUtils.Plus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.ENERGY)
                        return true;
                    else if (this.HeatingValue != null && this.density != null
                       && DimensionUtils.Plus(val.Dim, DimensionUtils.Plus(this.HeatingValue.Dim, this.DensityAsParameter.Dim)) == DimensionUtils.ENERGY)
                        return true;
                }
                else if (val.Dim == DimensionUtils.CURRENCY)
                {
                    if (this.marketValue != null)
                        if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.ENERGY)
                            return true;
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.VOLUME)
                            return this.CanConvertTo(toUnitDim, new LightValue(1, DimensionUtils.VOLUME)); //recursive way is simpler than enumerating all cases...
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.MASS)
                            return this.CanConvertTo(toUnitDim, new LightValue(1, DimensionUtils.MASS));
                        else
                            return false;
                }
                return false;
                #endregion
            }
            else if (toUnitDim == DimensionUtils.MASS)
            {
                #region toMass
                if (val.Dim == DimensionUtils.MASS)
                    return true;
                else if (val.Dim == DimensionUtils.ENERGY)
                {
                    if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0
                        && DimensionUtils.Minus(val.Dim, HeatingValue.Dim) == DimensionUtils.MASS) //hard_unit
                        return true;
                    else if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0 && this.DensityAsParameter != null && this.DensityAsParameter.ValueInDefaultUnit != 0
                       && DimensionUtils.Plus(val.Dim, DimensionUtils.Minus(density.Dim, HeatingValue.Dim)) == DimensionUtils.MASS)
                        return true;
                }
                else if (val.Dim == DimensionUtils.VOLUME)
                {
                    if (this.density != null)
                        return true;
                }
                else if (val.Dim == DimensionUtils.CURRENCY)
                {
                    if (this.marketValue != null)
                        if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.MASS)
                            return true;
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.VOLUME)
                            return this.CanConvertTo(toUnitDim, new LightValue(1, DimensionUtils.VOLUME)); //recursive way is simpler than enumerating all cases...
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.ENERGY)
                            return this.CanConvertTo(toUnitDim, new LightValue(1, DimensionUtils.ENERGY));
                        else
                            return false;
                }

                return false;
                #endregion
            }
            else if (toUnitDim == DimensionUtils.VOLUME)//hard_unit
            {
                #region toVolume
                if (val.Dim == DimensionUtils.VOLUME)
                    return true;
                else if (val.Dim == DimensionUtils.ENERGY)//hard_unit
                {

                    if (this.HeatingValue != null && this.density != null && this.HeatingValue.ValueInDefaultUnit != 0 && this.DensityAsParameter.ValueInDefaultUnit != 0
                            && DimensionUtils.Minus(val.Dim, DimensionUtils.Plus(this.density.Dim, this.HeatingValue.Dim)) == DimensionUtils.VOLUME)//hard_unit
                        return true;
                    else if (this.HeatingValue != null && this.HeatingValue.ValueInDefaultUnit != 0
                       && DimensionUtils.Minus(val.Dim, this.HeatingValue.Dim) == DimensionUtils.VOLUME)
                        return true;
                }
                else if (val.Dim == DimensionUtils.MASS)//hard_unit
                {
                    if (this.density != null && this.density.ValueInDefaultUnit != 0)
                        return true;
                }
                else if (val.Dim == DimensionUtils.CURRENCY)
                {
                    if (this.marketValue != null)
                        if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.VOLUME)
                            return true;
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.ENERGY)
                            return this.CanConvertTo(toUnitDim, new LightValue(1, DimensionUtils.ENERGY)); //recursive way is simpler than enumerating all cases...
                        else if (DimensionUtils.Minus(val.Dim, this.marketValue.Dim) == DimensionUtils.MASS)
                            return this.CanConvertTo(toUnitDim, new LightValue(1, DimensionUtils.MASS));
                        else
                            return false;
                }

                return false;
                #endregion
            }
            else if (toUnitDim == DimensionUtils.CURRENCY)//hard_unit
            {
                #region toCurrency
                if (this.marketValue != null && DimensionUtils.Plus(val.Dim, this.marketValue.Dim) == DimensionUtils.CURRENCY)
                    return true;
                else if (this.marketValue != null &&
                (DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.ENERGY) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.ENERGY, val)
                    || DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.MASS) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.MASS, val)
                    || DimensionUtils.Plus(this.marketValue.Dim, DimensionUtils.VOLUME) == DimensionUtils.CURRENCY && this.CanConvertTo(DimensionUtils.VOLUME, val)))
                    return true;

                return false;
                #endregion
            }
            else
                return false;
        }

        #endregion

        /// <summary>
        /// Converts to energy, mass, volume or currency
        /// This method is to accomodate with the older unit system and it's use should be avoided, prefer to use ConvertTo(uint toUnitDim, LightValue val)
        /// </summary>
        /// <param name="toUnitName">Expects "joules", "kilograms", "cu_meters" or "us_dollar"</param>
        /// <param name="val">LightValue to be converted</param>
        /// <returns>A new instance of the converted value</returns>
        [Obsolete("This method is obsolete and one should now use the method ConvertTo(uint toUnitDim, LightValue val)")]
        public LightValue ConvertTo(string toUnitName, LightValue val)
        {
            if (toUnitName == "joules") //hard_unit
                return this.ConvertTo(DimensionUtils.ENERGY, val);
            else if (toUnitName == "kilograms")//hard_unit
                return this.ConvertTo(DimensionUtils.MASS, val);
            else if (toUnitName == "cu_meters")//hard_unit
                return this.ConvertTo(DimensionUtils.VOLUME, val);
            else if (toUnitName == "us_dollar")
                return this.ConvertTo(DimensionUtils.CURRENCY, val);
            else
                throw new Exception("Unknown group name, cannot convert");
        }

        /// <summary>
        /// Converts to energy, mass, volume or currency
        /// </summary>
        /// <param name="toUnitDim">Expects an unsigned integer representing a quantity of energy, mass, volume or currency from UnitLib3</param>
        /// <param name="val">LightValue to be converted</param>
        /// <returns>A new instance of the converted value</returns>
        public LightValue ConvertTo(uint toUnitDim, LightValue val)
        {
            LightValue result;
            if (toUnitDim == DimensionUtils.ENERGY) //hard_unit
                result = this.ConvertToEnergy(val);
            else if (toUnitDim == DimensionUtils.MASS)//hard_unit
                result = this.ConvertToMass(val);
            else if (toUnitDim == DimensionUtils.VOLUME)//hard_unit
                result = this.ConvertToVolume(val);
            else if (toUnitDim == DimensionUtils.CURRENCY)
                result = this.ConvertToMarketValue(val);
            else
                throw new Exception("Unknown group name, cannot convert");

            return result;
        }

        /// <summary>
        /// Converts the bottom part of a Enem of this materials
        /// </summary>
        /// <param name="unitDim"></param>
        /// <param name="enem"></param>
        /// <returns></returns>
        public Enem ConvertBottomTo(uint unitDim, Enem enem)
        {
            if (enem.BottomDim == unitDim)
                return enem;
            else
            {
                ResourceAmounts converted_energy = new ResourceAmounts(this.ConvertBottomTo(unitDim, (DVDict)enem.materialsAmounts));
                EmissionAmounts converted_emissions = new EmissionAmounts(this.ConvertBottomTo(unitDim, (Dict)enem.emissions));

                return new Enem(converted_energy, converted_emissions);
            }
        }

        /// <summary>
        /// Converts the bottom part of an Results object and all containing Enems, Dict and DVDict
        /// </summary>
        /// <param name="unitDim">The desired future bottom</param>
        /// <param name="enem">The Result object to convert</param>
        /// <returns></returns>
        public Results ConvertBottomTo(uint unitDim, Results enem)
        {
            Results opRes = new Results();
            if (enem.wellToProductEnem.BottomDim == unitDim)
            {
                opRes = enem;
            }
            else if (enem.wellToProductEnem.emissions.Count > 0
                && enem.wellToProductEnem.materialsAmounts.Count > 0)
            {

                opRes.wellToProductEnem = this.ConvertBottomTo(unitDim, enem.wellToProductEnem);
                opRes.onsiteEmissions = new EmissionAmounts(this.ConvertBottomTo(unitDim, enem.onsiteEmissions));
                opRes.onsiteResources = new ResourceAmounts(this.ConvertBottomTo(unitDim, enem.onsiteResources));
                opRes.lossesEmissions = new EmissionAmounts(this.ConvertBottomTo(unitDim, enem.lossesEmissions));
                opRes.lossesAmounts = new ResourceAmounts(this.ConvertBottomTo(unitDim, enem.lossesAmounts));
                opRes.staticEmissions = new EmissionAmounts(this.ConvertBottomTo(unitDim, enem.staticEmissions));
                opRes.wellToProductUrbanEmission = new EmissionAmounts(this.ConvertBottomTo(unitDim, enem.wellToProductUrbanEmission));
                opRes.onsiteUrbanEmissions = new EmissionAmounts(this.ConvertBottomTo(unitDim, enem.onsiteUrbanEmissions));
                return opRes;
            }
            return opRes;
        }

        /// <summary>
        /// Converts the bottom unit of a dictionary to another bottom unit 
        /// Modify all the values in the dictionary to accomodate for the change in the functional unit
        /// </summary>
        /// <param name="unitDim"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        public Dict ConvertBottomTo(uint unitDim, Dict dico)
        {
            //creates one unit of the bottom of the dictionary
            LightValue bottom_value = new LightValue(1.0, dico.BottomDim);

            //converts the bottom value to desired value 
            LightValue converted_bottom = this.ConvertTo(unitDim, bottom_value);

            //divide all the values by the ratio of the conversion
            Dict converted_dictionary = new Dict(converted_bottom.Dim);
            foreach (KeyValuePair<int, double> value in dico)
                converted_dictionary.Add(value.Key, value.Value / converted_bottom.Value);

            return converted_dictionary;

        }

        /// <summary>
        /// Converts the bottom unit of a dictionary to another bottom unit 
        /// Modify all the values in the dictionary to accomodate for the change in the functional unit
        /// </summary>
        /// <param name="unitDim"></param>
        /// <param name="dico"></param>
        /// <returns></returns>
        public DVDict ConvertBottomTo(uint unitDim, DVDict dico)
        {
            //creates one unit of the bottom of the dictionary
            LightValue bottom_value = new LightValue(1.0, dico.BottomDim);

            //converts the bottom value to desired value 
            LightValue converted_bottom = this.ConvertTo(unitDim, bottom_value);

            //divide all the values by the ratio of the conversion
            DVDict converted_dictionary = new DVDict(converted_bottom.Dim);
            foreach (KeyValuePair<int, LightValue> value in dico)
                converted_dictionary.Add(value.Key, value.Value / converted_bottom.Value);

            return converted_dictionary;
        }

        #endregion

        #endregion

        #region IResource
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }

        /// <summary>
        /// Converts a result value in a certain unit into another result value in the desired unit
        /// </summary>
        /// <param name="toUnitName">Expects "joules", "kilograms", "cu_meters" or "us_dollar"</param>
        /// <param name="value">Value to be converted</param>
        /// <returns>Converted value in desired unit</returns>
        [Obsolete("This method is obsolete and one should now use the method ConvertTo(uint toUnitDim, LightValue val)")]
        public IValue ConvertTo(string toUnitName, IValue value)
        {
            LightValue toConvert = new LightValue(value.Value, value.UnitExpression);
            LightValue result = this.ConvertTo(toUnitName, toConvert);
            ResultValue toReturn = new ResultValue();
            toReturn.Value = result.Value;
            toReturn.UnitExpression = Units.QuantityList[result.QuantityName].SIUnitStr;
            toReturn.SpecieId = value.SpecieId;
            toReturn.ValueSpecie = value.ValueSpecie;
            return toReturn;
        }

        /// <summary>
        /// Creates a converted IValue from a Parameter to the desired unit.
        /// The GREET value or USER value will be used depending on the UseOriginal attribute of the IParameter
        /// </summary>
        /// <param name="toUnitName">Expects "joules", "kilograms", "cu_meters" or "us_dollar"</param>
        /// <param name="value">Value to be used for the conversion</param>
        /// <returns>Converted value in desired unit</returns>
        [Obsolete("This method is obsolete and one should now use the method ConvertTo(uint toUnitDim, LightValue val)")]
        public IValue ConvertTo(string toUnitName, IParameter value)
        {
            LightValue toConvert = new LightValue(value.UseOriginal ? value.GreetValue : value.UserValue, value.Dim);
            LightValue result = this.ConvertTo(toUnitName, toConvert);
            ResultValue toReturn = new ResultValue();
            toReturn.Value = result.Value;
            toReturn.UnitExpression = Units.QuantityList.ByDim(result.Dim).SiUnit.Expression;
            return toReturn;
        }

        #endregion

        #region IHaveMetadata Members

        public string ModifiedBy { get { return this.modifiedOn; } set { this.modifiedOn = value; } }

        public string ModifiedOn { get { return this.modifiedBy; } set { this.modifiedBy = value; } }

        #endregion
    }
}
