using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    public class VehicleModePowerPlant : IVehicleModePlant
    {
        #region private memebers

        /// <summary>
        /// Display name for the power plant
        /// </summary>
        string _name = "";
        /// <summary>
        /// Unique ID for that power plant
        /// </summary>
        Guid _uniqueID = Guid.NewGuid();
        /// <summary>
        /// The emission factors for the vehicle usually expressed in kilograms per meter
        /// a different set of emission factors is selected for each year.
        /// </summary>
        Dictionary<int, ParameterTS> _emissionsFactors = new Dictionary<int, ParameterTS>();
        /// <summary>
        /// Resources used as fuel for this mode
        /// </summary>
        List<InputResourceReference> _fuelUsed = new List<InputResourceReference>();
        /// <summary>
        /// Fuel consumption for each of the fuels
        /// </summary>
        List<ParameterTS> _fuel_consumption = new List<ParameterTS>();
        /// <summary>
        /// Charging/Refueling efficiency for each of the fuels
        /// </summary>
        List<ParameterTS> _charging_efficiency = new List<ParameterTS>();
        /// <summary>
        /// Total WTW energy used by the vehicle
        /// </summary>
        ResourceAmounts _calculatedTotalEnergy = new ResourceAmounts();
        /// <summary>
        /// Upstream energy associated with each fuel used for the mode in J/m
        /// </summary>
        List<ResourceAmounts> _calcualtedFuelUpstreamEnergy = new List<ResourceAmounts>();
        /// <summary>
        /// Energy used for engine/motor for vehicle operations in J/m
        /// </summary>
        ResourceAmounts _calculatedOperationEnergy = new ResourceAmounts();
        /// <summary>
        /// Upstream emissions associated with each fuel used for the mode in g/m
        /// </summary>
        List<EmissionAmounts> _calculatedFuelUpstreamEmissions = new List<EmissionAmounts>();
        /// <summary>
        /// Energy used for engine/motor for vehicle operations in g/m
        /// </summary>
        EmissionAmounts _calculatedOperationEmissions = new EmissionAmounts();
        /// <summary>
        /// Total WTW
        /// </summary>
        EmissionAmounts _calculatedTotalEmissions = new EmissionAmounts();
        /// <summary>
        /// Urban WTP
        /// </summary>
        List<EmissionAmounts> _calculatedFuelEmissionsUrban = new List<EmissionAmounts>();
        /// <summary>
        /// PTW urban emissions
        /// </summary>
        EmissionAmounts _calculatedOperationEmissionsUrban = new EmissionAmounts();
        /// <summary>
        /// Calcualted total urban emission
        /// </summary>
        EmissionAmounts _calculatedTotalEmissionsUrban = new EmissionAmounts();
        /// <summary>
        /// Status flags for calculations
        /// </summary>
        bool _operationCorrectlyCalculated = false;
        /// <summary>
        /// Set to True if all calculations went well for that mode
        /// </summary>
        bool _upstreamCorrectlyCalculated = false;
        /// <summary>
        /// Errors messages that happened during the calculations
        /// </summary>
        string _errorMessages = "";
        /// <summary>
        /// Base vehicle used for emission and energy percentages calculations in the GUI
        /// Set to -1 if no base vehicle is used
        /// </summary>
        int _baseVehicle = -1;
        /// <summary>
        /// Base mode used for emission and energy percentages calculations in the GUI
        /// Set to 00000000-0000-0000-0000-000000000000 if not used
        /// </summary>
        Guid _baseMode = new Guid();
        /// <summary>
        /// Base plant used for emission and energy percentages calculations in the GUI
        /// Set to 00000000-0000-0000-0000-000000000000 if not used
        /// </summary>
        Guid _basePlant = new Guid();
        /// <summary>
        /// Notes associated with the mode
        /// </summary>
        string _notes = "";
        /// <summary>
        /// Defines weather or not this powerplant is a template that can be dragged and dropped
        /// </summary>
        bool _isTemplate = false;
        #endregion private memebers

        #region internal constructors
        /// <summary>
        /// Creates a mode with no base vehcicle
        /// </summary>
        /// <param name="baseVehicle"></param>
        /// <param name="baseMode"></param>
        public VehicleModePowerPlant(int baseVehicle = -1, Guid baseMode = new Guid(), Guid basePlant = new Guid())
        {
            _operationCorrectlyCalculated = false;
            _upstreamCorrectlyCalculated = false;
            _baseVehicle = baseVehicle;
            _baseMode = baseMode;
            _basePlant = basePlant;
        }

        public VehicleModePowerPlant(GData data, System.Xml.XmlNode xmlNode, string optionalParamPrefix = "")
        {
            _operationCorrectlyCalculated = false;
            _upstreamCorrectlyCalculated = false;
            
            // lzf add
            _name = xmlNode.Attributes["name"].Value;
            _uniqueID = new Guid(xmlNode.Attributes["id"].Value);
            if (xmlNode.Attributes["notes"] != null)
                this._notes = xmlNode.Attributes["notes"].Value;
            if (xmlNode.Attributes["isTemplate"] != null)
                _isTemplate = Convert.ToBoolean(xmlNode.Attributes["isTemplate"].Value);
            _basePlant = new Guid(xmlNode.Attributes["basePlantId"].Value);

            try
            {
                XmlNode cnode;
                int count = 0;
                foreach (XmlNode fuel_node in xmlNode.SelectNodes("fuel"))
                {
                    InputResourceReference vfuel = new InputResourceReference(fuel_node);
                    _fuelUsed.Add(vfuel);
                    cnode = fuel_node.SelectSingleNode("consumption");
                    if (cnode != null)
                        _fuel_consumption.Add(new ParameterTS(data, cnode));
                    else
                        _fuel_consumption.Add(new ParameterTS(data, "J/m", 1.0));
                    cnode = fuel_node.SelectSingleNode("charging_efficiency");
                    if (cnode != null)
                        _charging_efficiency.Add(new ParameterTS(data, cnode));
                    else
                        _charging_efficiency.Add(new ParameterTS(data, "%", 100));
                    count++;
                }

                this._emissionsFactors = new Dictionary<int, ParameterTS>();
                foreach (XmlNode emission in xmlNode.SelectNodes("emission"))
                {
                    try
                    {
                        cnode = emission.SelectSingleNode("value_ts");
                        this._emissionsFactors.Add(Convert.ToInt32(emission.Attributes["id"].Value), new ParameterTS(data, cnode));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 6:" + e.Message);
                        throw e;
                    }
                }
            }
            catch (Exception ex) 
            {
                LogFile.Write("Error 3:" + ex.Message + "\r\n" + xmlNode.OwnerDocument.Name + "\r\n" + xmlNode.OuterXml + "\r\n");
                throw ex;
            }
        }

        #endregion internal constructors

        #region public accessors

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Display name for the power plant
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        /// <summary>
        /// Unique ID for that power plant
        /// </summary>
        public Guid UniqueID
        {
            get { return _uniqueID; }
            set { _uniqueID = value; }
        }
        /// <summary>
        /// Can be used as a template
        /// </summary>
        public bool IsTemplate
        {
            get { return _isTemplate; }
            set { _isTemplate = value; }
        }
        public string ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }
        public bool UpstreamCorrectlyCalculated
        {
            get { return _upstreamCorrectlyCalculated; }
            set { _upstreamCorrectlyCalculated = value; }
        }
        public int BaseVehicle
        {
            get { return _baseVehicle; }
            set { _baseVehicle = value; }
        }
        public EmissionAmounts CalculatedTotalEmissions
        {
            get { return _calculatedTotalEmissions; }
            set { _calculatedTotalEmissions = value; }
        }
        public List<EmissionAmounts> CalculatedFuelUpstreamEmissions
        {
            get { return _calculatedFuelUpstreamEmissions; }
            set { _calculatedFuelUpstreamEmissions = value; }
        }
        public ResourceAmounts CalculatedOperationEnergy
        {
            get { return _calculatedOperationEnergy; }
            set { _calculatedOperationEnergy = value; }
        }
        public EmissionAmounts CalculatedOperationEmissions
        {
            get { return _calculatedOperationEmissions; }
            set { _calculatedOperationEmissions = value; }
        }

        /// <summary>
        /// Total WTW energy used by the vehicle
        /// </summary>
        public ResourceAmounts CalculatedTotalEnergy
        {
            get { return _calculatedTotalEnergy; }
            set { _calculatedTotalEnergy = value; }
        }
        public EmissionAmounts CalculatedOperationEmissionsUrban
        {
            get { return _calculatedOperationEmissionsUrban; }
            set { _calculatedOperationEmissionsUrban = value; }
        }
        public List<ResourceAmounts> CalcualtedFuelUpstreamEnergy
        {
            get { return _calcualtedFuelUpstreamEnergy; }
            set { _calcualtedFuelUpstreamEnergy = value; }
        }
        public EmissionAmounts CalculatedTotalEmissionsUrban
        {
            get { return _calculatedTotalEmissionsUrban; }
            set { _calculatedTotalEmissionsUrban = value; }
        }
        public bool OperationCorrectlyCalculated
        {
            get { return _operationCorrectlyCalculated; }
            set { _operationCorrectlyCalculated = value; }
        }
        public List<EmissionAmounts> CalculatedFuelEmissionsUrban
        {
            get { return _calculatedFuelEmissionsUrban; }
            set { _calculatedFuelEmissionsUrban = value; }
        }
        public Guid BaseMode
        {
            get { return _baseMode; }
            set { _baseMode = value; }
        }
        public Guid BasePlant
        {
            get { return _basePlant; }
            set { _basePlant = value; }
        }
        /// <summary>
        /// The emission factors for the vehicle usually expressed in kilograms per meter
        /// a different set of emission factors is selected for each year.
        /// </summary>
        public Dictionary<int, ParameterTS> EmissionFactors
        {
            get { return _emissionsFactors; }
            set { _emissionsFactors = value; }
        }
        /// <summary>
        /// Resources used as fuel for this mode
        /// </summary>
        public List<InputResourceReference> FuelUsed
        {
            get { return _fuelUsed; }
            set { _fuelUsed = value; }
        }
        /// <summary>
        /// Fuel consumption for each of the fuels
        /// </summary>
        public List<ParameterTS> FuelConsumptions
        {
            get { return _fuel_consumption; }
            set { _fuel_consumption = value; }
        }
        /// <summary>
        /// Charging/Refueling efficiency for each of the fuels
        /// </summary>
        public List<ParameterTS> ChargingEfficencies
        {
            get { return _charging_efficiency; }
            set { _charging_efficiency = value; }
        }
        /// <summary>
        /// Returns the fuels used by this vehicle as a List of Source(IInputResourceReference) and its volumetric share.
        /// Used primarly to return the Fuels in terms of Interface objects.
        /// </summary>
        public Dictionary<IInputResourceReference, double> FuelsUsed
        {
            get
            {
                Dictionary<IInputResourceReference, double> list = new Dictionary<IInputResourceReference, double>();
                foreach (InputResourceReference vf in this.FuelUsed)
                {
                    list.Add(vf, 1); //TODD
                }
                return list;
            }
        }
        /// <summary>
        /// Returns all the fuels used by the vehicle in this mode. It includes the fuels from the Base vehicle also.
        /// </summary>
        /// <param name="vehicles"></param>
        /// <returns></returns>
        public List<InputResourceReference> FuelBlend(Vehicles vehicles)
        {
            return this._fuelUsed;

        }
        #endregion

        #region internal methods
        internal XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n, n1;
            XmlNode plantNode = xmlDoc.CreateNode("plant", xmlDoc.CreateAttr("name", _name), xmlDoc.CreateAttr("id", _uniqueID), xmlDoc.CreateAttr("notes", _notes),xmlDoc.CreateAttr("isTemplate",_isTemplate),xmlDoc.CreateAttr("basePlantId",_basePlant));
            for (int i = 0; i < FuelsUsed.Count; i++)
            {
                n = xmlDoc.CreateNode("fuel");
                (FuelUsed[i] as InputResourceReference).ToXmlNode(xmlDoc, n);
                n.AppendChild(FuelConsumptions[i].ToXmlNode(xmlDoc, "consumption"));
                n.AppendChild(ChargingEfficencies[i].ToXmlNode(xmlDoc, "charging_efficiency"));
                plantNode.AppendChild(n);
            }
            foreach (int poluttant_id in this._emissionsFactors.Keys)
            {
                n = xmlDoc.CreateNode("emission", xmlDoc.CreateAttr("id", poluttant_id));
                n1 = this._emissionsFactors[poluttant_id].ToXmlNode(xmlDoc, "value_ts");
                n.AppendChild(n1);
                plantNode.AppendChild(n);
            }

            return plantNode;
        }
        #endregion internal methods

        #region public methods
        /// <summary>
        /// Summs upstream resources for all of the fules used in the mode
        /// </summary>
        /// <returns></returns>
        public ResourceAmounts SumCalcualtedFuelUpstreamEnergy()
        {
            ResourceAmounts res = new ResourceAmounts();
            res.BottomDim = DimensionUtils.LENGTH;
            foreach (ResourceAmounts ra in _calcualtedFuelUpstreamEnergy)
            {
                res += ra;
            }
            return res;
        }

        /// <summary>
        /// Summs upstream emissions for all of the fules used in the mode
        /// </summary>
        /// <returns></returns>
        public EmissionAmounts SumFuelEmission()
        {
            EmissionAmounts res = new EmissionAmounts();
            res.BottomDim = DimensionUtils.LENGTH;
            foreach (EmissionAmounts ra in this.CalculatedFuelUpstreamEmissions)
            {
                res += ra;
            }
            return res;
        }

        /// <summary>
        /// Summs upstream emissions for all of the fules used in the mode
        /// </summary>
        /// <returns></returns>
        public EmissionAmounts SumFuelEmissionUrban()
        {
            EmissionAmounts res = new EmissionAmounts();
            res.BottomDim = DimensionUtils.LENGTH;
            foreach (EmissionAmounts ra in this.CalculatedFuelEmissionsUrban)
            {
                res += ra;
            }
            return res;
        }
        #endregion

        #region IVehicleModePlant interface
        IEnumerable<IInputResourceReference> IVehicleModePlant.FuelUsed
        {
            get
            {
                List<IInputResourceReference> resourcesReferences = new List<IInputResourceReference>();
                foreach (InputResourceReference inpRef in _fuelUsed)
                    resourcesReferences.Add(inpRef as IInputResourceReference);
                return resourcesReferences;
            }
        }
        #endregion
    }
}
