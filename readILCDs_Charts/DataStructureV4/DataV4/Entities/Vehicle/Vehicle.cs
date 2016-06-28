// *********************************************************************** 
//  COPYRIGHT NOTIFICATION 
// 
//  Email contact: greet@anl.gov 
//  Copyright (c) 2012, UChicago Argonne, LLC 
//  All Rights Reserved
//  
//  THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
//  LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
//  CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
//  NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 
//  
//  ************************************************************************ 
//  ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
//  IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
//  ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
//  ENERGY. 
//  ************************************************************************
//   
//  ***********************************************************************/

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
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Stores the data associated with a vehicle, its properties and its modes
    /// </summary>
    [Serializable]
    public class Vehicle : IHaveAPicture, IVehicle, IHaveMetadata, IGREETEntity
    {
        #region Fields and Constants

        const string xmlAttrModifiedBy = "modified-by";
        const string xmlAttrModifiedOn = "modified-on";

        /// <summary>
        /// True if the vehicle needs recalculations, because the inputs have been changed since last calculations
        /// </summary>
        private bool _anyParameterChangedSinceLastCalculated;

        /// <summary>
        /// True if the calculations went fine for this vehicle
        /// </summary>
        private bool _calculatedOk;

        /// <summary>
        /// Non combustions emissions calcualted for the vehicle (such as tire and brakes pads wearing off into dust and particle)
        /// </summary>
        private EmissionAmounts _calculatedTireBreakWearEmissions = new EmissionAmounts();

        /// <summary>
        /// Total emissions for vehicle operation and fuel production, this value is calculated from the energy necessary to move the vehicle and the upstream of the
        /// associated mixes or pathways.
        /// </summary>
        private EmissionAmounts _calculatedTotalEmissions = new EmissionAmounts();

        /// <summary>
        /// Total emissions for vehicle operation and fuel production in urban environment, this value is calculated from the energy necessary to move the vehicle and the upstream of the
        /// associated mixes or pathways.
        /// </summary>
        private EmissionAmounts _calculatedTotalEmissionsUrban = new EmissionAmounts();

        /// <summary>
        /// Total energy for vehicle operation and fuel production, this value is calculated from the energy necessary to move the vehicle and the upstream of the
        /// associated mixes or pathways.
        /// </summary>
        private ResourceAmounts _calculatedTotalEnergy = new ResourceAmounts();

        /// <summary>
        /// Calculated Emission Values
        /// </summary>
        private EmissionAmounts _calculatedUpstreamFuelEmissions = new EmissionAmounts();

        /// <summary>
        /// Calculated Emission associated with upstream Values
        /// </summary>
        private EmissionAmounts _calculatedUpstreamFuelEmissionsUrban = new EmissionAmounts();

        /// <summary>
        /// Calculated Energy Values for fuel production footprint and fuel energy content only
        /// </summary>
        private ResourceAmounts _calculatedUpstreamFuelEnergy = new ResourceAmounts();

        /// <summary>
        /// Calculated emissions necessary for vehicle operation only, no upstream accounted
        /// </summary>
        private EmissionAmounts _calculatedVehicleOperationEmissions = new EmissionAmounts();

        /// <summary>
        /// Calculated emissions necessary for vehicle operation in urban environment only, no upstream accounted
        /// </summary>
        private EmissionAmounts _calculatedVehicleOperationEmissionsUrban = new EmissionAmounts();

        /// <summary>
        /// Calculated energy necessary for vehicle operation only, no upstream accounted
        /// </summary>
        private ResourceAmounts _calculatedVehicleOperationEnergy = new ResourceAmounts();

        /// <summary>
        /// WTW emissions are opetaion + upstream but do not include vehicle cycle (components and recycling)
        /// </summary>
        private EmissionAmounts _calculatedWTWEmissions = new EmissionAmounts();

        /// <summary>
        /// WTW emissions urban are opetaion + upstream but do not include vehicle cycle (components and recycling)
        /// </summary>
        private EmissionAmounts _calculatedWTWEmissionsUrban = new EmissionAmounts();

        /// <summary>
        /// WTW resources are opetaion + upstream but do not include vehicle cycle (components and recycling)
        /// </summary>
        private ResourceAmounts _calculatedWTWEnergy = new ResourceAmounts();

        /// <summary>
        /// The all electric range of the vehicle, the data will be then adjusted for real world use
        /// </summary>
        private ParameterTS _electricRange;

        /// <summary>
        /// Error message that might happen during the calculations, this should probably be centralized or changed to a list
        /// </summary>
        private string _errorMessages = "";

        /// <summary>
        /// ID of the vehicle, IDs are unique thoughout the vehicles list
        /// </summary>
        private int _id = -1;

        /// <summary>
        /// This parameter is used to calculate the material LCA per mile
        /// </summary>
        private ParameterTS _lifetimeVMT;

        /// <summary>
        /// Components, fluids, adr, battery and others
        /// </summary>
        private List<VehicleManufacturing> _manufacturing = new List<VehicleManufacturing>();

        /// <summary>
        /// Stores the operational modes. The keys as CD or CS (why do we need the keys? can't the modes being self calculated and represented in the GUI using interfaces?)
        /// </summary>
        private List<VehicleOperationalMode> _modes = new List<VehicleOperationalMode>();

        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedBy = "";

        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedOn = "";

        /// <summary>
        /// Name of the vehicle
        /// </summary>
        private string _name = "";

        /// <summary>
        /// The emission factors for the vehicle usually expressed in kilograms per meter
        /// a different set of emission factors is selected for each year.
        /// </summary>
        Dictionary<int, ParameterTS> _nonCombustionEmissionsFactors = new Dictionary<int, ParameterTS>();

        /// <summary>
        /// Notes associated to that vehicle
        /// </summary>
        private string _notes = "";

        /// <summary>
        /// Number of passengers in the vehicle for displaying results in per vehicle mile
        /// </summary>
        private ParameterTS _passengers;

        /// <summary>
        /// Weight carried by the vehicle. Required for HDV
        /// </summary>
        private ParameterTS _payload;

        /// <summary>
        /// Picture name that can be used to represent the vehicle
        /// </summary>
        public string _pictureName = Constants.EmptyPicture;

        /// <summary>
        /// The prefered functional unit for this vehicle. This can be any of those units
        /// 1/100km, 1/mi, 1/(mi*ton), 1/(km*tonne), 1/(passenger*mi), 1/(passenger*km)
        /// </summary>
        private VehicleFunctionalUnit _preferedFunctionalUnit = VehicleFunctionalUnit.mi;

        /// <summary>
        /// Default Urban Share for a vehicle is 62% 
        /// </summary>
        private Parameter _urbanShare;

        /// <summary>
        /// Weigth of the vehicle (without payload)
        /// </summary>
        private ParameterTS _weight;

        /// <summary>
        /// Tags such as PHEV, EV, BEV, Light Duty, Heavy Duty for categorization
        /// </summary>
        private List<string> tags = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default vehicle constructor
        /// </summary>
        public Vehicle()
        {
            _manufacturing.Add(new VehicleManufacturing("Components"));
            _manufacturing.Add(new VehicleManufacturing("ADR"));
            _manufacturing.Add(new VehicleManufacturing("Fluids"));
            _manufacturing.Add(new VehicleManufacturing("Battery"));
            _manufacturing.Add(new VehicleManufacturing("Others"));
        }

        /// <summary>
        /// Builds a vehicle from a database XMLnode
        /// </summary>
        /// <param name="xmlNode"></param>
        internal Vehicle(GData data, XmlNode xmlNode)
            : this()
        {
            FromXmlNode(data, xmlNode);
        }

        #endregion

        #region Properties and Indexers

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

        public EmissionAmounts CalculatedWTWEmissions
        {
            get { return _calculatedWTWEmissions; }
            set { _calculatedWTWEmissions = value; }
        }

        public EmissionAmounts CalculatedWTWEmissionsUrban
        {
            get { return _calculatedWTWEmissionsUrban; }
            set { _calculatedWTWEmissionsUrban = value; }
        }

        public ResourceAmounts CalculatedWTWEnergy
        {
            get { return _calculatedWTWEnergy; }
            set { _calculatedWTWEnergy = value; }
        }

        public bool CalulatedCorrectly
        {
            get
            { return _calculatedOk; }
        }

        /// <summary>
        /// If set to true the UI will not show this vehicle in the selection controls
        /// </summary>
        public bool Discarded { get; set; }

        /// <summary>
        /// Date and time at which this item has been discarded
        /// </summary>
        public DateTime DiscardedOn { get; set; }

        /// <summary>
        /// Reason for that item to be discarded in the database
        /// </summary>
        public string DiscardedReason { get; set; }

        /// <summary>
        /// User registered for this copy of GREET that deleted the item
        /// </summary>
        public string DiscarededBy { get; set; }

        public ParameterTS ElectricRange
        {
            get { return _electricRange; }
            set { _electricRange = value; }
        }

        public string ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }

        [Browsable(true)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// This parameter is used to calculate the material LCA per mile
        /// </summary>
        public ParameterTS LifetimeVMT
        {
            get { return _lifetimeVMT; }
            set { _lifetimeVMT = value; }
        }

        /// <summary>
        /// Components, ADR, Fluids, Manufacturing
        /// </summary>
        public List<VehicleManufacturing> Manufacturing
        {
            get { return _manufacturing; }
            set { _manufacturing = value; }
        }

        public double MinYearDefinition
        {
            get
            {
                double min_year_definition = double.MaxValue;
                foreach (VehicleOperationalMode mode in _modes)
                {
                    foreach (VehicleModePowerPlant plant in mode.Plants)
                    {
                        foreach (ParameterTS ts in plant.FuelConsumptions)
                            min_year_definition = Math.Min(min_year_definition, ts.Keys.Min());

                        foreach (ParameterTS ts in plant.EmissionFactors.Values)
                            min_year_definition = Math.Min(min_year_definition, ts.Keys.Min());

                    }
                }

                foreach (VehicleManufacturing mf in _manufacturing)
                {
                    foreach (ParameterTS ts in mf.MaterialsQuantity)
                        min_year_definition = Math.Min(min_year_definition, ts.Keys.Min());
                }

                if (_weight != null)
                    min_year_definition = Math.Min(min_year_definition,_weight.Keys.Min());
                
                if (_payload != null)
                    min_year_definition = Math.Min(min_year_definition, _payload.Keys.Min());

                if (_lifetimeVMT != null)
                    min_year_definition = Math.Min(min_year_definition, _lifetimeVMT.Keys.Min());

                return min_year_definition;
            }
        }

        public List<VehicleOperationalMode> Modes
        {
            get { return _modes; }
            set { _modes = value; }
        }

        public string ModifiedBy { get { return _modifiedOn; } set { _modifiedOn = value; } }

        public string ModifiedOn { get { return _modifiedBy; } set { _modifiedBy = value; } }

        [Browsable(true)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Emission factors not related to the powerplant
        /// </summary>
        public Dictionary<int, ParameterTS> NonCombustionEmissionsFactors
        {
            get { return _nonCombustionEmissionsFactors; }
            set { _nonCombustionEmissionsFactors = value; }
        }

        [Browsable(true)]
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public List<IVehicleMode> OperatingModes
        {
            get
            {
                List<IVehicleMode> returnedModes = new List<IVehicleMode>();
                foreach (VehicleOperationalMode mode in _modes)
                    returnedModes.Add(mode);

                return returnedModes;
            }
        }

        public ParameterTS Passengers
        {
            get { return _passengers; }
            set { _passengers = value; }
        }

        /// <summary>
        /// Mass carried by the vehilce. Required for HDV
        /// </summary>
        public ParameterTS Payload
        {
            get { return _payload; }
            set { _payload = value; }
        }

        [Browsable(true)]
        public string PictureName
        {
            get { return _pictureName; }
            set { _pictureName = value; }
        }

        /// <summary>
        /// The prefered functional unit for this vehicle. This can be any of those units
        /// 1/100km, 1/mi, 1/(mi*ton), 1/(km*tonne), 1/(passenger*mi), 1/(passenger*km)
        /// </summary>
        public VehicleFunctionalUnit PreferedFunctionalUnit
        {
            get { return _preferedFunctionalUnit; }
            set { _preferedFunctionalUnit = value; }
        }

        /// <summary>
        /// List of tags assotiated witht he vehicle. For eaxmple HDV, EV, PHEV,...
        /// </summary>
        public List<string> Tags
        {
            get { return tags; }
            set { tags = value; }
        }

        public EmissionAmounts TireBreakWearEmissions
        {
            get { return _calculatedTireBreakWearEmissions; }
            set { _calculatedTireBreakWearEmissions = value; }
        }

        public EmissionAmounts TotalEmissions
        {
            get { return _calculatedTotalEmissions; }
            set { _calculatedTotalEmissions = value; }
        }

        public EmissionAmounts TotalEmissionsUrban
        {
            get { return _calculatedTotalEmissionsUrban; }
            set { _calculatedTotalEmissionsUrban = value; }
        }

        public ResourceAmounts TotalEnergy
        {
            get { return _calculatedTotalEnergy; }
            set { _calculatedTotalEnergy = value; }
        }

        public EmissionAmounts UpstreamFuelEmissions
        {
            get { return _calculatedUpstreamFuelEmissions; }
            set { _calculatedUpstreamFuelEmissions = value; }
        }

        public EmissionAmounts UpstreamFuelEmissionsUrban
        {
            get { return _calculatedUpstreamFuelEmissionsUrban; }
            set { _calculatedUpstreamFuelEmissionsUrban = value; }
        }

        public ResourceAmounts UpstreamFuelEnergy
        {
            get { return _calculatedUpstreamFuelEnergy; }
            set { _calculatedUpstreamFuelEnergy = value; }
        }

        [Browsable(true)]
        public Parameter UrbanShare
        {
            get
            {
                return _urbanShare;
            }
            set
            {
                _urbanShare = value;
            }
        }

        public EmissionAmounts VehicleOperationEmissions
        {
            get { return _calculatedVehicleOperationEmissions; }
            set { _calculatedVehicleOperationEmissions = value; }
        }

        public EmissionAmounts VehicleOperationEmissionsUrban
        {
            get { return _calculatedVehicleOperationEmissionsUrban; }
            set { _calculatedVehicleOperationEmissionsUrban = value; }
        }

        public ResourceAmounts VehicleOperationEnergy
        {
            get { return _calculatedVehicleOperationEnergy; }
            set { _calculatedVehicleOperationEnergy = value; }
        }

        /// <summary>
        /// Weight of the vehicle (without payload)
        /// </summary>
        public ParameterTS VehicleWeight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        #endregion

        #region Interfaces Implementation

        /// <summary>
        /// Get the WTW results
        /// </summary>
        /// <param name="data">Dataset containing resources and emissions entities</param>
        /// <param name="format">
        /// <para>0: Per 1 J</para>
        /// <para>1: Per m</para>
        /// <para>2: Per m</para>
        /// <para>3: Per kg*m</para>
        /// <para>4: Per kg*m</para>
        /// <para>5: Per passenger m</para>
        /// <para>6: Per passenger m</para>
        /// </param>
        /// <returns></returns>
        public Enem GetTotalWTWResults(IData data, int format = 0)
        {
            Enem resultDictionary = new Enem();

            resultDictionary.materialsAmounts = TotalEnergy;
            resultDictionary.emissions = TotalEmissions;
            resultDictionary.BottomDim = DimensionUtils.LENGTH;

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            if (format == 0)
            {//We want the results to be converted per energy 
                LightValue totalModeEnergy = VehicleOperationEnergy.TotalEnergy();
                LightValue totalModeEnergyPerDistance = new LightValue(totalModeEnergy.Value, DimensionUtils.Minus(totalModeEnergy.Dim, VehicleOperationEnergy.BottomDim));
                
                resultDictionary = resultDictionary / totalModeEnergyPerDistance;
                resultDictionary.BottomDim = DimensionUtils.ENERGY;

                return resultDictionary;
            }
            if (format == 1 || format == 2)
            {//results kept with a functional unit of distance
                return resultDictionary;
            }
            if (format == 3 || format == 4)
            {//change functional unit to energy mass by dividing the results by the payload 
                if (Payload == null)
                    throw new NullReferenceException("Payload is not defined for that vehicle");

                LightValue payload = Payload.CurrentValue.ToLightValue();

                resultDictionary = resultDictionary / payload;
                resultDictionary.BottomDim = DimensionUtils.Plus(DimensionUtils.LENGTH, DimensionUtils.MASS);

                return resultDictionary;
            }
            if (format == 5 || format == 6)
            {//change functional unit to energy mass by dividing the results by the payload 

                if (Passengers == null)
                    throw new NullReferenceException("Passenger is not defined for that vehicle");
                LightValue passengers = Passengers.CurrentValue.ToLightValue();

                resultDictionary = resultDictionary / passengers;
                resultDictionary.BottomDim = DimensionUtils.LENGTH;
            }

            #endregion

            return resultDictionary;
        }

        /// <summary>
        /// Get the WTW results
        /// </summary>
        /// <param name="data">Dataset containing resources and emissions entities</param>
        /// <param name="format">
        /// <para>0: Per 1 J</para>
        /// <para>1: Per m</para>
        /// <para>2: Per m</para>
        /// <para>3: Per kg*m</para>
        /// <para>4: Per kg*m</para>
        /// <para>5: Per passenger m</para>
        /// <para>6: Per passenger m</para>
        /// </param>
        /// <returns></returns>
        public EmissionAmounts GetTotalWTWUrbanEm(IData data, int format = 0)
        {
            EmissionAmounts resultDictionary = new EmissionAmounts();

            resultDictionary = TotalEmissionsUrban;
            resultDictionary.BottomDim = DimensionUtils.LENGTH;

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            if (format == 0)
            {//We want the results to be converted per energy 
                LightValue totalModeEnergy = VehicleOperationEnergy.TotalEnergy();
                LightValue totalModeEnergyPerDistance = new LightValue(totalModeEnergy.Value, DimensionUtils.Minus(totalModeEnergy.Dim, VehicleOperationEnergy.BottomDim));

                resultDictionary = resultDictionary / totalModeEnergyPerDistance;
                resultDictionary.BottomDim = DimensionUtils.ENERGY;

                return resultDictionary;
            }
            if (format == 1 || format == 2)
            {//results kept with a functional unit of distance
                return resultDictionary;
            }
            if (format == 3 || format == 4)
            {//change functional unit to energy mass by dividing the results by the payload 
                if (Payload == null)
                    throw new NullReferenceException("Payload is not defined for that vehicle");

                LightValue payload = Payload.CurrentValue.ToLightValue();

                resultDictionary = resultDictionary / payload;
                resultDictionary.BottomDim = DimensionUtils.Plus(DimensionUtils.LENGTH, DimensionUtils.MASS);

                return resultDictionary;
            }
            if (format == 5 || format == 6)
            {//change functional unit to energy mass by dividing the results by the payload 

                if (Passengers == null)
                    throw new NullReferenceException("Passenger is not defined for that vehicle");
                LightValue passengers = Passengers.CurrentValue.ToLightValue();

                resultDictionary = resultDictionary / passengers;
                resultDictionary.BottomDim = DimensionUtils.LENGTH;
            }

            #endregion

            return resultDictionary;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            FromXmlNode(data as GData, node, "");
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode errornode = xmlDoc.CreateNode("errornode-" + _id);
            try
            {
                XmlNode vehicleNode = xmlDoc.CreateNode("vehicle");
                if (Discarded)
                {
                    vehicleNode.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                    vehicleNode.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                    vehicleNode.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                    vehicleNode.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
                }

                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("id", _id));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("name", _name));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("picture", _pictureName));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("urban_share", _urbanShare));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("notes", _notes));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, ModifiedOn.ToString(GData.Nfi)));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, ModifiedBy));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("tags", string.Join(",", tags.ToArray())));
                vehicleNode.Attributes.Append(xmlDoc.CreateAttr("pref-unit", _preferedFunctionalUnit));
                if (_electricRange != null)
                    vehicleNode.AppendChild(_electricRange.ToXmlNode(xmlDoc, "electric_range"));

                for (int i = 0; i < _modes.Count; i++)
                {
                    XmlNode n = _modes[i].ToXmlNode(xmlDoc);
                    vehicleNode.AppendChild(n);
                }
               
                if (_weight != null)
                    vehicleNode.AppendChild(_weight.ToXmlNode(xmlDoc, "weight"));
                if (_passengers != null)
                    vehicleNode.AppendChild(_passengers.ToXmlNode(xmlDoc, "passengers"));
                if (_payload != null)
                    vehicleNode.AppendChild(_payload.ToXmlNode(xmlDoc, "payload"));
                if (_lifetimeVMT != null)
                    vehicleNode.AppendChild(_lifetimeVMT.ToXmlNode(xmlDoc, "lifetime_vmt"));

                foreach (VehicleManufacturing mf in _manufacturing)
                    vehicleNode.AppendChild(mf.ToXmlNode(xmlDoc));

                foreach (int poluttant_id in _nonCombustionEmissionsFactors.Keys)
                {
                    XmlNode n1 = _nonCombustionEmissionsFactors[poluttant_id].ToXmlNode(xmlDoc, "nonCombustionEmission");
                    n1.Attributes.Append(xmlDoc.CreateAttr("id", poluttant_id));
                    vehicleNode.AppendChild(n1);
                }
                return vehicleNode;
            }
            catch (Exception e)
            {
                LogFile.Write(e.Message);
                return errornode;
            }
        }

        #endregion

        #region Members

        public bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";

            //check fuel share
            foreach (VehicleOperationalMode mode in _modes)
            {
                foreach (VehicleModePowerPlant plant in mode.Plants)
                {
                    foreach (InputResourceReference fuel in plant.FuelUsed)
                    {

                        if (!data.ResourcesData.ContainsKey(fuel.ResourceId))
                            errorMessage += "ERROR: Contains a fuel references resource (" + fuel.ResourceId + ") that does not exist\r\n";

                        else
                        {
                            if (fuel.SourceType == Enumerators.SourceType.Pathway)
                            {
                                if (!data.PathwaysData.ContainsKey(fuel.SourceMixOrPathwayID))
                                    errorMessage += "ERROR: Contains a fuel references(" + mode.Name + "):" + "Pathway " + "(" + fuel.SourceMixOrPathwayID + ") that does not exist in database.\r\n";
                                else if (data.PathwayMainOutputResouce(fuel.SourceMixOrPathwayID) != fuel.ResourceId)
                                    errorMessage += "ERROR: Contains a fuel references(" + mode.Name + "):" + "Pathway " + "(" + fuel.SourceMixOrPathwayID + ") that does not produce the resource " + data.ResourcesData[fuel.ResourceId].Name + ".\r\n";
                            }
                            else if (fuel.SourceType == Enumerators.SourceType.Mix)
                                if (!data.MixesData.ContainsKey(fuel.SourceMixOrPathwayID))
                                    errorMessage += "ERROR: Contains a fuel references(" + mode.Name + "):" + "Pathway Mix" + "(" + fuel.SourceMixOrPathwayID + ") that does not exist in database.\r\n";
                                else if (data.MixesData[fuel.SourceMixOrPathwayID].MainOutputResourceID != fuel.ResourceId)
                                    errorMessage += "ERROR: Contains a fuel references(" + mode.Name + "):" + "Pathway Mix" + "(" + fuel.SourceMixOrPathwayID + ") that does not produce the resource " + data.ResourcesData[fuel.ResourceId].Name + ".\r\n";
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(errorMessage))
                errorMessage += "\r\n";

            return true;
        }

        private void FromXmlNode(GData data, XmlNode xmlNode, string optionalParamPrefix)
        {
            String status = "";
            try
            {
                if (xmlNode.Attributes["discarded"] != null)
                {
                    Discarded = Convert.ToBoolean(xmlNode.Attributes["discarded"].Value);
                    DiscardedOn = Convert.ToDateTime(xmlNode.Attributes["discardedOn"].Value, GData.Nfi);
                    DiscarededBy = xmlNode.Attributes["discardedBy"].Value;
                    DiscardedReason = xmlNode.Attributes["discardedReason"].Value;
                }

                status = "reading id";
                _id = Convert.ToInt32(xmlNode.Attributes["id"].Value);
                status = "reading name";
                _name = xmlNode.Attributes["name"].Value;
                status = "reading picture";
                if (xmlNode.Attributes["picture"].NotNullNOrEmpty())
                    _pictureName = xmlNode.Attributes["picture"].Value;
                status = "reading electric range";
                if (xmlNode.SelectSingleNode("electric_range") != null)
                    _electricRange = new ParameterTS(data, xmlNode.SelectSingleNode("electric_range"), "_veh_" + _id + "_elecrange");
                status = "reading urban_share";
                if (xmlNode.Attributes["urban_share"] != null)
                    _urbanShare = data.ParametersData.CreateRegisteredParameter(xmlNode.Attributes["urban_share"], "_veh_" + _id + "_urbshare");
                status = "reading notes";
                if (xmlNode.Attributes["notes"] != null)
                    _notes = xmlNode.Attributes["notes"].Value;
                status = "reading modified on";
                if (xmlNode.Attributes[xmlAttrModifiedOn] != null)
                    ModifiedOn = xmlNode.Attributes[xmlAttrModifiedOn].Value;
                status = "reading modified by";
                if (xmlNode.Attributes[xmlAttrModifiedBy] != null)
                    ModifiedBy = xmlNode.Attributes[xmlAttrModifiedBy].Value;
                status = "reading tags";
                if (xmlNode.Attributes["tags"] != null)
                {
                    foreach (string s in xmlNode.Attributes["tags"].Value.Split(','))
                        tags.Add(s);
                }
                XmlNode cnode;
                status = "reading lifetime_vmt";
                cnode = xmlNode.SelectSingleNode("lifetime_vmt");
                if (cnode != null)
                    _lifetimeVMT = new ParameterTS(data, cnode);

                status = "reading weight";
                cnode = xmlNode.SelectSingleNode("weight");
                if (cnode != null)
                    _weight = new ParameterTS(data, cnode);

                status = "reading passengers";
                cnode = xmlNode.SelectSingleNode("passengers");
                if (cnode != null)
                    _passengers = new ParameterTS(data, cnode);

                status = "reading payload";
                cnode = xmlNode.SelectSingleNode("payload");
                if (cnode != null)
                    _payload = new ParameterTS(data, cnode);
                _manufacturing.Clear(); ///clear the default manufactoring elements
                foreach (XmlNode manufacturingNode in xmlNode.SelectNodes("manufacturing"))
                {
                    _manufacturing.Add(new VehicleManufacturing(data, manufacturingNode));
                }
                status = "building modes";
                foreach (XmlNode modeNode in xmlNode.SelectNodes("mode"))
                {
                    VehicleOperationalMode mode = new VehicleOperationalMode(data, modeNode, "veh" + _id);
                    if (mode != null)
                    {
                        _modes.Add(mode);
                    }
                    else
                    {
                        LogFile.Write("Error: unreadable mode: " + modeNode.OuterXml);
                        throw new Exception("Unkown mode");
                    }
                }
                status = "reading prefered functional unit";
                if (xmlNode.Attributes["pref-unit"] != null)
                    _preferedFunctionalUnit = (VehicleFunctionalUnit) Enum.Parse(typeof(VehicleFunctionalUnit), xmlNode.Attributes["pref-unit"].Value);

                _nonCombustionEmissionsFactors = new Dictionary<int, ParameterTS>();
                foreach (XmlNode emission in xmlNode.SelectNodes("nonCombustionEmission"))
                {
                    try
                    {
                        _nonCombustionEmissionsFactors.Add(Convert.ToInt32(emission.Attributes["id"].Value), new ParameterTS(data, emission));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 6:" + e.Message);
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 1:" + e.Message + "\r\n" + status + "\r\n" + xmlNode.OwnerDocument + "\r\n" + xmlNode.OuterXml);
                throw e;
            }
        }

        public override string ToString()
        {
            return _name;
        }

        public XmlNode ToXmlResultsNode(XmlDocument doc)
        {

            XmlNode node = doc.CreateNode("vehicle", doc.CreateAttr("id", Id), doc.CreateAttr("name", Name), doc.CreateAttr("functional-unit", "1;distance"));
            XmlNode em_node = doc.CreateNode("emissions");
            XmlNode en_node = doc.CreateNode("energy");
            node.AppendChild(em_node);
            node.AppendChild(en_node);
            if (_calculatedOk)
            {
                XmlNode temp_node;
                //Add emission related results
                temp_node = doc.CreateNode("life-cycle"); _calculatedTotalEmissions.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", _calculatedTotalEmissions.Total()));
                em_node.AppendChild(temp_node);
                temp_node = doc.CreateNode("vehicle-only"); _calculatedVehicleOperationEmissions.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", _calculatedVehicleOperationEmissions.Total()));
                em_node.AppendChild(temp_node);
                //Add energy related results
                temp_node = doc.CreateNode("life-cycle"); _calculatedUpstreamFuelEnergy.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", _calculatedUpstreamFuelEnergy.TotalEnergy()));
                en_node.AppendChild(temp_node);
                temp_node = doc.CreateNode("vehicle-only"); _calculatedVehicleOperationEnergy.AppendToXmlNode(doc, temp_node);
                temp_node.Attributes.Append(doc.CreateAttr("sum", _calculatedVehicleOperationEnergy.TotalEnergy()));
                en_node.AppendChild(temp_node);
            }

            return node;
        }

        #endregion
    }
}
