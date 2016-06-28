using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// Stores the data associated with a V3OLDVehicle, its properties and its modes
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDVehicle : IHaveAPicture, IHaveMetadata
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region attributes

        /// <summary>
        /// ID of the V3OLDVehicle, IDs are unique thoughout the V3OLDVehicles list
        /// </summary>
        private int _id;
        /// <summary>
        /// The base V3OLDVehicle for energy consumption
        /// </summary>
        private int _baseMpg;
        /// <summary>
        /// The base V3OLDVehicle for emissions
        /// </summary>
        private int _baseEmission;
        /// <summary>
        /// Name of the V3OLDVehicle
        /// </summary>
        private string _name;
        /// <summary>
        /// Notes associated to that V3OLDVehicle
        /// </summary>
        private string _notes = "";
        /// <summary>
        /// Picture name that can be used to represent the V3OLDVehicle
        /// </summary>
        public string _pictureName = Constants.EmptyPicture;
        /// <summary>
        /// Flag set to true if the V3OLDVehicle is grid connected (we should try to avoid that and handle this situation from the Modes directly)
        /// </summary>
        public Boolean _gridConnected;
        /// <summary>
        /// Default Urban Share for a V3OLDVehicle is 62% 
        /// </summary>
        private Parameter _urbanShare;
        /// <summary>
        /// Stores the operational modes. The keys as CD or CS (why do we need the keys? can't the modes being self calculated and represented in the GUI using interfaces?)
        /// </summary>
        private Dictionary<string, V3OLDVehicleOperationalMode> _modes;
        /// <summary>
        /// The Charger efficiency in case we are using a gridConnected V3OLDVehicle
        /// </summary>
        private Parameter _chargerEfficiency;
        /// <summary>
        /// The all electric range of the V3OLDVehicle, the data will be then adjusted for real world use
        /// </summary>
        private Parameter _electricRange;
        /// <summary>
        /// The minimum year for which modes are defined in the V3OLDVehicles (why is this an attribute, it could be a accessor, is that a buffer ?)
        /// </summary>
        private double _minYearDefinition;
        /// <summary>
        /// Error message that might happen during the calculations, this should probably be centralized or changed to a list
        /// </summary>
        private string _errorMessages = "";
        /// <summary>
        /// Calculated Energy Values for fuel production footprint and fuel energy content only
        /// </summary>
        private ResourceAmounts _upstreamFuelEnergy;
        /// <summary>
        /// Calculated energy necessary for V3OLDVehicle operation only, no upstream accounted
        /// </summary>
        private ResourceAmounts _vehicleOperationEnergy;
        /// <summary>
        /// Total energy for V3OLDVehicle operation and fuel production, this value is calculated from the energy necessary to move the V3OLDVehicle and the upstream of the
        /// associated mixes or pathways.
        /// </summary>
        private ResourceAmounts _totalEnergy;
        /// <summary>
        /// Calculated Emission Values
        /// </summary>
        private EmissionAmounts _upstreamFuelEmissions, _vehicleOperationEmissions, _totalEmissions, _upstreamFuelEmissionsUrban, _vehicleOperationEmissionsUrban, _totalEmissionsUrban;
        /// <summary>
        /// True if the calculations went fine for this V3OLDVehicle
        /// </summary>
        private bool _calculatedOk = false;
        /// <summary>
        /// True if the V3OLDVehicle needs recalculations, because the inputs have been changed since last calculations
        /// </summary>
        private bool _anyParameterChangedSinceLastCalculated = false;
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedBy = "";
        /// <summary>
        /// List of materials used to make and operate the V3OLDVehicle (metal, rubber, ...)
        /// </summary>
        private List<InputResourceReference> _materials;
        /// <summary>
        /// Quantity of each material required to make a V3OLDVehicle. This list must be of the same lenght as materials
        /// </summary>
        private List<ParameterTS> _materials_quantity;
        /// <summary>
        /// Mass carried by the V3OLDVehicle. Required for HDV
        /// </summary>
        private ParameterTS _payload;
        /// <summary>
        /// This parameter is used to calculate the material LCA per mile
        /// </summary>
        private ParameterTS _lifespan;
        /// <summary>
        /// This parameter is used to calculate the material LCA per mile
        /// </summary>
        private ParameterTS _yearly_milage;


        #endregion attributes

        #region constructors

        /// <summary>
        /// Default V3OLDVehicle constructor
        /// </summary>
        public V3OLDVehicle()
        {
            this._modes = new Dictionary<string, V3OLDVehicleOperationalMode>();
            this._totalEnergy = new ResourceAmounts();
        }

        /// <summary>
        /// Builds a V3OLDVehicle from a database XMLnode
        /// </summary>
        /// <param name="xmlNode"></param>
        internal V3OLDVehicle(GData data, XmlNode xmlNode)
            : this()
        {
            this.FromXmlNode(data, xmlNode, "");
        }

        #endregion constructors

        #region accessors
        /// <summary>
        /// This parameter is used to calculate the material LCA per mile
        /// </summary>
        public ParameterTS Yearly_milage
        {
            get { return _yearly_milage; }
            set { _yearly_milage = value; }
        }
        /// <summary>
        /// This parameter is used to calculate the material LCA per mile
        /// </summary>
        public ParameterTS Lifespan
        {
            get { return _lifespan; }
            set { _lifespan = value; }
        }
        /// <summary>
        /// Mass carried by the vehilce. Required for HDV
        /// </summary>
        public ParameterTS Payload
        {
            get { return _payload; }
            set { _payload = value; }
        }
        /// <summary>
        /// Quantity of each material required to make a V3OLDVehicle. This list must be of the same lenght as materials
        /// </summary>
        public List<ParameterTS> Materials_quantity
        {
            get { return _materials_quantity; }
            set { _materials_quantity = value; }
        }
        /// <summary>
        /// List of materials used to make and operate the V3OLDVehicle (metal, rubber, ...)
        /// </summary>
        public List<InputResourceReference> Materials
        {
            get { return _materials; }
            set { _materials = value; }
        }

        public bool AnyParameterChangedSinceLastCalculated
        {
            get { return _anyParameterChangedSinceLastCalculated; }
            set { _anyParameterChangedSinceLastCalculated = value; }
        }
        public bool CalculatedOk
        {
            get { return _calculatedOk; }
            set { _calculatedOk = value; }
        }
        public EmissionAmounts TotalEmissionsUrban
        {
            get { return _totalEmissionsUrban; }
            set { _totalEmissionsUrban = value; }
        }

        public EmissionAmounts VehicleOperationEmissionsUrban
        {
            get { return _vehicleOperationEmissionsUrban; }
            set { _vehicleOperationEmissionsUrban = value; }
        }

        public EmissionAmounts UpstreamFuelEmissionsUrban
        {
            get { return _upstreamFuelEmissionsUrban; }
            set { _upstreamFuelEmissionsUrban = value; }
        }

        public EmissionAmounts TotalEmissions
        {
            get { return _totalEmissions; }
            set { _totalEmissions = value; }
        }

        public EmissionAmounts VehicleOperationEmissions
        {
            get { return _vehicleOperationEmissions; }
            set { _vehicleOperationEmissions = value; }
        }

        public EmissionAmounts UpstreamFuelEmissions
        {
            get { return _upstreamFuelEmissions; }
            set { _upstreamFuelEmissions = value; }
        }
        public ResourceAmounts TotalEnergy
        {
            get { return _totalEnergy; }
            set { _totalEnergy = value; }
        }
        public ResourceAmounts VehicleOperationEnergy
        {
            get { return _vehicleOperationEnergy; }
            set { _vehicleOperationEnergy = value; }
        }
        public ResourceAmounts UpstreamFuelEnergy
        {
            get { return _upstreamFuelEnergy; }
            set { _upstreamFuelEnergy = value; }
        }
        public string ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }
        public Parameter ElectricRange
        {
            get { return _electricRange; }
            set { _electricRange = value; }
        }
        
        public double MinYearDefinition
        {
            get
            {
                return this._minYearDefinition;
            }
            set { this._minYearDefinition = value; }
        }

        public Parameter ChargerEfficiency
        {
            get { return _chargerEfficiency; }
            set { _chargerEfficiency = value; }
        }

        /// <summary>
        /// The base V3OLDVehicle for energy consumption
        /// </summary>
        public int BaseMpgVehicleId
        {
            get { return _baseMpg; }
            set { _baseMpg = value; }
        }
        public Boolean GridConnected
        {
            get { return _gridConnected; }
            set { _gridConnected = value; }
        }

        /// <summary>
        /// The base V3OLDVehicle for emissions
        /// </summary>
        public int BaseEmissionVehicleId
        {
            get { return _baseEmission; }
            set { _baseEmission = value; }
        }
        public Dictionary<string, V3OLDVehicleOperationalMode> Modes
        {
            get { return _modes; }
            set { _modes = value; }
        }

        [Browsable(true)]
        public string PictureName
        {
            get { return this._pictureName; }
            set { this._pictureName = value; }
        }
        [Browsable(true)]
        public int Id
        {
            get { return _id; }
            set
            {

                _id = value;
                foreach (V3OLDVehicleOperationalMode mode in this.Modes.Values)
                    mode._vehicleReferenceId = this._id;

            }
        }
        [Browsable(true)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Browsable(true)]
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        [Browsable(true)]
        public double UrbanShare
        {
            get
            {
                return _urbanShare.ValueInDefaultUnit;
            }
            set { }
        }

        public Parameter UrbanShareDV
        {
            get
            {
                return _urbanShare;
            }
            set { _urbanShare = value; }
        }

        public override string ToString()
        {
            return this._name;
        }

        #endregion accessors

        #region methods

        private void FromXmlNode(GData data, XmlNode xmlNode, string optionalParamPrefix)
        {
            String status = "";
            try
            {
                status = "reading id";
                this._id = Convert.ToInt32(xmlNode.Attributes["id"].Value);
                status = "reading name";
                this._name = xmlNode.Attributes["name"].Value;
                status = "reading picture";
                if (xmlNode.Attributes["picture"].NotNullNOrEmpty())
                {
                    this._pictureName = xmlNode.Attributes["picture"].Value;
                }
                status = "reading gird connected";
                this._gridConnected = Convert.ToBoolean(xmlNode.Attributes["grid_connected"].Value);
                status = "reading base mpg";
                if (xmlNode.Attributes["mpg_base_vehicle_id"] != null)
                    this._baseMpg = Convert.ToInt32(xmlNode.Attributes["mpg_base_vehicle_id"].Value);
                else
                    this._baseMpg = this._id;
                status = "reading base emissions";
                if (xmlNode.Attributes["emission_base_vehicle_id"] != null)
                    this._baseEmission = Convert.ToInt32(xmlNode.Attributes["emission_base_vehicle_id"].Value);
                else
                    this._baseEmission = this._id;

                status = "reading charger efficiency";
                if (xmlNode.Attributes["charger_efficiency"] != null)
                    this._chargerEfficiency = data.ParametersData.CreateRegisteredParameter(xmlNode.Attributes["charger_efficiency"], "_veh_" + this._id + "_charger");
                status = "reading electric range";
                if (xmlNode.Attributes["electric_range"] != null)
                    this._electricRange = data.ParametersData.CreateRegisteredParameter(xmlNode.Attributes["electric_range"], "_veh_" + this._id + "_elecrange");
                status = "reading urban_share";
                if (xmlNode.Attributes["urban_share"] != null)
                    this._urbanShare = data.ParametersData.CreateRegisteredParameter(xmlNode.Attributes["urban_share"], "_veh_" + this._id + "_urbshare");
                status = "reading notes";
                if (xmlNode.Attributes["notes"] != null)
                    this._notes = xmlNode.Attributes["notes"].Value;
                status = "reading modified on";
                if (xmlNode.Attributes[xmlAttrModifiedOn] != null)
                    this.ModifiedOn = xmlNode.Attributes[xmlAttrModifiedOn].Value;
                status = "reading modified by";
                if (xmlNode.Attributes[xmlAttrModifiedBy] != null)
                    this.ModifiedBy = xmlNode.Attributes[xmlAttrModifiedBy].Value;

                
                XmlNode cnode;
                status = "reading yearly_milage";
                cnode = xmlNode.SelectSingleNode("yearly_milage");
                if (cnode!=null)
                    this._yearly_milage = new ParameterTS(data, cnode);

                status = "reading lifespan";
                cnode = xmlNode.SelectSingleNode("lifespan");
                    if (cnode != null)
                        this._lifespan = new ParameterTS(data, cnode);
                status = "reading payload";
                cnode = xmlNode.SelectSingleNode("payload");
                if (cnode != null)
                    this._payload = new ParameterTS(data, cnode);
                foreach (XmlNode materialNode in xmlNode.SelectNodes("material"))
                {
                    _materials.Add(new InputResourceReference(Convert.ToInt32(materialNode.Attributes["resource_id"]), 
                                                      Convert.ToInt32(materialNode.Attributes["entity_id"]),
                                                      (Enumerators.SourceType)Enum.Parse(typeof(Enumerators.SourceType), materialNode.Attributes["source_type"].Value)
                                                      ));
                    _materials_quantity.Add(new ParameterTS(data, materialNode.SelectSingleNode("quantity")));
                }
                status = "building modes";
                foreach (XmlNode modeNode in xmlNode.SelectNodes("mode"))
                {
                    V3OLDVehicleOperationalMode mode = null;
                    if (modeNode.Attributes["mode"].Value.ToLower() == "cd")
                        mode = new V3OLDCDMode(data, modeNode, this._id, "veh_cd_" + this._id);
                    else if (modeNode.Attributes["mode"].Value.ToLower() == "cs")
                        mode = new V3OLDRegularMode(data, modeNode, this._id, "veh_cs_" + this._id);
                    else if (modeNode.Attributes["mode"].Value.ToLower() == "regular")
                        mode = new V3OLDRegularMode(data, modeNode, this._id, "veh_reg" + this._id);

                    if (mode != null)
                    {
                        this._modes.Add(mode._name, mode);
                        if (modeNode.Attributes["notes"] != null)
                            mode.Notes = modeNode.Attributes["notes"].Value;
                    }
                    else
                        throw new Exception("Unkown mode");

                }

                status = "compute min year definition"; //should be moved to post processing or as an accessor
                this.InitializeMinYearDefinition();

            }
            catch (Exception e)
            {
                LogFile.Write("Error 1:" + e.Message + "\r\n" + status + "\r\n" + xmlNode.OwnerDocument + "\r\n" + xmlNode.OuterXml);
                throw e;
            }
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode errornode = xmlDoc.CreateNode("errornode-" + this._id.ToString());
            try
            {
                if (_id == 67)
                { }
                XmlNode V3OLDVehicleNode = xmlDoc.CreateNode("V3OLDVehicle"
                    , xmlDoc.CreateAttr("grid_connected", _gridConnected)
                    , xmlDoc.CreateAttr("id", _id), xmlDoc.CreateAttr("name", _name)
                    , xmlDoc.CreateAttr("picture", _pictureName)
                    , xmlDoc.CreateAttr("mpg_base_vehicle_id", BaseMpgVehicleId)
                    , xmlDoc.CreateAttr("emission_base_vehicle_id", BaseEmissionVehicleId)
                    , xmlDoc.CreateAttr("urban_share", this._urbanShare)
                    , xmlDoc.CreateAttr("notes", this._notes)
                    , xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi))
                    , xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));

                if (this._gridConnected)
                {
                    V3OLDVehicleNode.Attributes.Append(xmlDoc.CreateAttr("electric_range", this._electricRange));
                    V3OLDVehicleNode.Attributes.Append(xmlDoc.CreateAttr("charger_efficiency", this._chargerEfficiency));
                }

                foreach (V3OLDVehicleOperationalMode mode in this._modes.Values)
                    V3OLDVehicleNode.AppendChild(mode.ToXmlNode(xmlDoc));
                V3OLDVehicleNode.AppendChild(_payload.ToXmlNode(xmlDoc, "payload"));
                V3OLDVehicleNode.AppendChild(_lifespan.ToXmlNode(xmlDoc, "lifespan"));
                V3OLDVehicleNode.AppendChild(_yearly_milage.ToXmlNode(xmlDoc, "yearly_milage"));
                XmlNode cnode;
                for (int i = 0; i < this._materials.Count; i++)
                {
                    cnode = xmlDoc.CreateNode("material",
                        xmlDoc.CreateAttr("resource_id", this._materials[i].ResourceId),
                        xmlDoc.CreateAttr("entity_id", this._materials[i].SourceMixOrPathwayID),
                        xmlDoc.CreateAttr("source_type", this._materials[i].SourceType.ToString())                        
                        );
                    cnode.AppendChild(this._materials_quantity[i].ToXmlNode(xmlDoc, "quantity"));
                    V3OLDVehicleNode.AppendChild(cnode);
                }
                return V3OLDVehicleNode;
            }
            catch (Exception)
            {
                return errornode;
            }
        } 

        /// <summary>
        /// This method loops thought the modes and find the minimum year for which all the modes (emissions and energies V3OLDConsumptions) are defined
        /// The value is stored in the minYearDefinition attribute of a V3OLDVehicle
        /// </summary>
        public void InitializeMinYearDefinition()
        {
            double min_year_definition = double.MaxValue;
            foreach (V3OLDVehicleOperationalMode mode in this._modes.Values)
            {
                List<double> mins = new List<double>();
                if (mode is V3OLDCDMode)
                    mins.Add((mode as V3OLDCDMode).Consumptions.Keys.Min());
                mins.Add(mode.Technologies.Keys.Min());
                min_year_definition = Math.Min(min_year_definition, mins.Min());
            }
            this._minYearDefinition = min_year_definition;
        }

        public bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";
            if (!data.VehiclesData.ContainsKey(this.BaseEmissionVehicleId))
                errorMessage += "ERROR: Contains a base emission references (" + this.BaseEmissionVehicleId + ") that does not exist\r\n";
            if (!data.VehiclesData.ContainsKey(this.BaseMpgVehicleId))
                errorMessage += "ERROR: Contains a base MPG references (" + this.BaseMpgVehicleId + ") that does not exist\r\n";

            //check fuel share
            foreach (V3OLDVehicleOperationalMode mode in this.Modes.Values)
            {
                List<V3OLDVehicleFuel> tempFuelList = mode.FuelsUsedWithoutBaseFuels;
                foreach (V3OLDVehicleFuel fuel in tempFuelList)
                {
                    if (!data.ResourcesData.ContainsKey(fuel.InputResourceRef.ResourceId))
                        errorMessage += "ERROR: Contains a fuel references resource (" + fuel.InputResourceRef.ResourceId + ") that does not exist\r\n";

                    else
                    {
                        if (fuel.InputResourceRef.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway)
                        {
                            if (!data.PathwaysData.ContainsKey(fuel.InputResourceRef.SourceMixOrPathwayID))
                                errorMessage += "ERROR: Contains a fuel references(" + mode.ModeType() + "):" + "Pathway " + "(" + fuel.InputResourceRef.SourceMixOrPathwayID + ") that does not exist in database.\r\n";
                            else if (data.PathwayMainOutputResouce(fuel.InputResourceRef.SourceMixOrPathwayID) != fuel.InputResourceRef.ResourceId)
                                errorMessage += "ERROR: Contains a fuel references(" + mode.ModeType() + "):" + "Pathway " + "(" + fuel.InputResourceRef.SourceMixOrPathwayID + ") that does not produce the resource " + data.ResourcesData[fuel.InputResourceRef.ResourceId].Name + ".\r\n";
                        }
                        else if (fuel.InputResourceRef.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
                            if (!data.MixesData.ContainsKey(fuel.InputResourceRef.SourceMixOrPathwayID))
                                errorMessage += "ERROR: Contains a fuel references(" + mode.ModeType() + "):" + "Pathway Mix" + "(" + fuel.InputResourceRef.SourceMixOrPathwayID + ") that does not exist in database.\r\n";
                            else if (data.MixesData[fuel.InputResourceRef.SourceMixOrPathwayID].MainOutputResourceID != fuel.InputResourceRef.ResourceId)
                                errorMessage += "ERROR: Contains a fuel references(" + mode.ModeType() + "):" + "Pathway Mix" + "(" + fuel.InputResourceRef.SourceMixOrPathwayID + ") that does not produce the resource " + data.ResourcesData[fuel.InputResourceRef.ResourceId].Name + ".\r\n";
                    }
                }
            }

            if (!String.IsNullOrEmpty(errorMessage))
                errorMessage += "\r\n";

            return true;
        }

        public XmlNode ToXmlResultsNode(XmlDocument doc)
        {

            XmlNode node = doc.CreateNode("Vehicle", doc.CreateAttr("id", this.Id), doc.CreateAttr("name", this.Name), doc.CreateAttr("functional-unit", "1;distance"));
            XmlNode em_node = doc.CreateNode("emissions");
            XmlNode en_node = doc.CreateNode("energy");
            node.AppendChild(em_node);
            node.AppendChild(en_node);
            if (this._calculatedOk)
            {
                XmlNode temp_node;
                //Add emission related results
                temp_node = doc.CreateNode("life-cycle"); this._totalEmissions.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", this._totalEmissions.Total()));
                em_node.AppendChild(temp_node);
                temp_node = doc.CreateNode("vehicle-only"); this._vehicleOperationEmissions.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", this._vehicleOperationEmissions.Total()));
                em_node.AppendChild(temp_node);
                //Add energy related results
                temp_node = doc.CreateNode("life-cycle"); this._upstreamFuelEnergy.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", this._upstreamFuelEnergy.TotalEnergy()));
                en_node.AppendChild(temp_node);
                temp_node = doc.CreateNode("vehicle-only"); this._vehicleOperationEnergy.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", this._vehicleOperationEnergy.TotalEnergy()));
                en_node.AppendChild(temp_node);
            }

            return node;
        }

        #endregion methods

        #region IHaveMetadata Members

        public string ModifiedBy { get { return this._modifiedOn; } set { this._modifiedOn = value; } }

        public string ModifiedOn { get { return this._modifiedBy; } set { this._modifiedBy = value; } }

        #endregion
    }
}
