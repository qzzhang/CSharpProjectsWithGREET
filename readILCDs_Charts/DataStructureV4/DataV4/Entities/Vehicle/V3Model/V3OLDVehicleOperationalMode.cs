using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// Represent a operational mode for a defined V3OLDVehicle
    /// The class is abstract and could be either CD or CS mode
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public abstract class V3OLDVehicleOperationalMode
    {
        #region attributes
        /// <summary>
        /// The MPG tim series associated with that mode for each year defined.
        /// Be carefull: for V3OLDVehicles we do not use the current year, but the lagged value.
        /// </summary>
        internal V3OLDMPGsTS _mpg;
        /// <summary>
        /// Name of the V3OLDVehicle operational mode: "cd" "cs" or "regular". "cs" and "regular" are using
        /// the same calulations so maybe we should avoid using two different names for defining the same 
        /// mode of functionnement
        /// </summary>
        internal string _name;
        /// <summary>
        /// The emission factors for the V3OLDVehicle usually expressed in kilograms per meter
        /// a different set of emission factors is selected for each year.
        /// </summary>
        public V3OLDVehicleOperatingModeEmissionsTSData _technologies;
        /// <summary>
        /// If multiple modes are used and we are calculating modes of a plugin hybrid electric V3OLDVehicle
        /// The utility factor is calculated using a polynom which takes as a parameter the full electric range of the V3OLDVehicle
        /// and tells us what percentage of the time the V3OLDVehicle is running on CD mode.
        /// </summary>
        public double _utilityFactor;
        /// <summary>
        /// Resources used for fuel production per meter for this mode(PTW results per meter)
        /// </summary>
        public ResourceAmounts _fuelEnergy;
        /// <summary>
        /// Resources used for the operation of the V3OLDVehicle for this mode without includung any upstream (WTP results per meter)
        /// </summary>
        public ResourceAmounts _operationEnergy;
        /// <summary>
        /// Resources used for the operation and the fuel production for this mode (WTW results per meter)
        /// </summary>
        public ResourceAmounts _totalEnergy;
        public EmissionAmounts _fuelEmissions;
        public EmissionAmounts _operationEmissions;
        public EmissionAmounts _totalEmissions;
        public EmissionAmounts _fuelEmissionsUrban;
        public EmissionAmounts _operationEmissionsUrban;
        public EmissionAmounts _totalEmissionsUrban;

        private List<V3OLDVehicleFuel> _fuelUsed;
        public bool _operationCorrectlyCalculated;
        public bool _upstreamCorrectlyCalculated;
        public bool _usesBaseFuelShares;
        public string _errorMessages = "";
        public int _vehicleReferenceId;

        private string _notes = "";

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        /// <summary>
        /// Holds a integer that represents the resource used for calculating the mpgge value
        /// this resource physical properties are going to be used for the conversions. Usually this will be Conventional Gasoline
        /// </summary>
        public int _mpggeRef = 24; //hardcoded for old databases where this attribute do not exists

        #endregion attributes

        #region constructors

        protected V3OLDVehicleOperationalMode(int V3OLDVehicle_reference)
        {
            this._operationCorrectlyCalculated = false;
            this._upstreamCorrectlyCalculated = false;
            this._vehicleReferenceId = V3OLDVehicle_reference;
            this._fuelUsed = new List<V3OLDVehicleFuel>();
        }

        protected V3OLDVehicleOperationalMode(GData data, XmlNode xmlNode, int V3OLDVehicle_reference, string optionalParamPrefix)
        {
            if (V3OLDVehicle_reference == 54)
            { }
            this._operationCorrectlyCalculated = false;
            this._upstreamCorrectlyCalculated = false;
            this._vehicleReferenceId = V3OLDVehicle_reference;
            String status = "";
            try
            {
                status = "reading name";
                this._name = xmlNode.Attributes["mode"].Value;
                if (xmlNode.Attributes["notes"] != null)
                    this._notes = xmlNode.Attributes["notes"].Value;
                status = "building fuels";
                this._fuelUsed = new List<V3OLDVehicleFuel>();
                int count = 0;
                foreach (XmlNode fuel_node in xmlNode.SelectNodes("fuel"))
                {
                    V3OLDVehicleFuel vfuel = new V3OLDVehicleFuel(data, fuel_node, optionalParamPrefix + "_fuel_" + count);
                    _fuelUsed.Add(vfuel);
                    count++;
                }
                status = "reading mpgge ref";
                if (xmlNode.Attributes["mpgge_ref"] != null)
                    _mpggeRef = Convert.ToInt32(xmlNode.Attributes["mpgge_ref"].Value);

                status = "building technology emission factors"; //should be moved to post processing
                this._technologies = new V3OLDVehicleOperatingModeEmissionsTSData(data, xmlNode, this._vehicleReferenceId, this._name, optionalParamPrefix + "_tef");
            }
            catch (Exception e)
            {
                LogFile.Write("Error 3:" + e.Message + "\r\n" + status + "\r\n" + xmlNode.OwnerDocument.Name + "\r\n" + xmlNode.OuterXml + "\r\n");
                throw e;
            }
        }

        #endregion constructors

        #region accessors
        /// <summary>
        /// the emission factors time series
        /// </summary>
        public V3OLDVehicleOperatingModeEmissionsTSData Technologies
        {
            get { return _technologies; }
            set { _technologies = value; }
        }
        /// <summary>
        /// The mpg time series
        /// </summary>
        public V3OLDMPGsTS Mpg
        {
            get { return _mpg; }
            set { _mpg = value; }
        }

        /// <summary>
        /// Return utility factor which is calculated when CD mode is calculated
        /// </summary>
        public double UtilityFactor
        {
            get { return _utilityFactor; }
        }
        /// <summary>
        /// Return energy associated with fuel production
        /// </summary>
        public ResourceAmounts FuelEnergy
        {
            get { return _fuelEnergy; }
        }
        /// <summary>
        /// Return energy associated with V3OLDVehicle operation
        /// </summary>
        public ResourceAmounts OperationEnergy
        {
            get { return _operationEnergy; }
        }
        /// <summary>
        /// Return total energy for the mode
        /// </summary>
        public ResourceAmounts TotalEnergy
        {
            get { return _totalEnergy; }
        }
        /// <summary>
        /// Return emissions associated with fuel production
        /// </summary>
        public EmissionAmounts FuelEmissions
        {
            get { return _fuelEmissions; }
        }
        /// <summary>
        /// Return urban emissions associated with fuel production
        /// </summary>
        public EmissionAmounts FuelEmissionsUrban
        {
            get { return _fuelEmissionsUrban; }
        }
        /// <summary>
        /// Return emissions associated with V3OLDVehicle operation
        /// </summary>
        public EmissionAmounts OperationEmissions
        {
            get { return _operationEmissions; }
        }
        /// <summary>
        /// Return urban emissions associated with V3OLDVehicle operation
        /// </summary>
        public EmissionAmounts OperationEmissionsUrban
        {
            get { return _operationEmissionsUrban; }
        }
        /// <summary>
        /// Return total emissions for the mode
        /// </summary>
        public EmissionAmounts TotalEmissions
        {
            get { return _totalEmissions; }
        }
        /// <summary>
        /// Return total urban emissions for the mode
        /// </summary>
        public EmissionAmounts TotalEmissionsUrban
        {
            get { return _totalEmissionsUrban; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Returns the fuels used by the V3OLDVehicle in this mode. It does not include the fuels from the base V3OLDVehicle.
        /// </summary>
        public List<V3OLDVehicleFuel> FuelsUsedWithoutBaseFuels
        {
            get { return _fuelUsed; }
            set { _fuelUsed = value; }
        }
       
        /// <summary>
        /// Returns all the fuels used by the V3OLDVehicle in this mode. It includes the fuels from the Base V3OLDVehicle also.
       /// </summary>
       /// <param name="V3OLDVehicles"></param>
       /// <returns></returns>
        public List<V3OLDVehicleFuel> FuelBlend(V3OLDVehicles V3OLDVehicles)
        {

            List<V3OLDVehicleFuel> fuelBlend = new List<V3OLDVehicleFuel>();
            double share_from_non_base_fuels = 0;
            foreach (V3OLDVehicleFuel fuel in this._fuelUsed)
                if (fuel.UsedForCarbonBalance)
                {
                    share_from_non_base_fuels += fuel.VolumeShare.ValueInDefaultUnit;
                    fuelBlend.Add(fuel);
                }

            //gets the base V3OLDVehicles for energy
            if (V3OLDVehicles.ContainsKey(this._vehicleReferenceId)
                && V3OLDVehicles.ContainsKey(V3OLDVehicles[this._vehicleReferenceId].BaseMpgVehicleId)
                && this._vehicleReferenceId != V3OLDVehicles[this._vehicleReferenceId].BaseMpgVehicleId)
            {
                V3OLDVehicle base_vehicle_energy = V3OLDVehicles[V3OLDVehicles[this._vehicleReferenceId].BaseMpgVehicleId];
                V3OLDVehicleOperationalMode base_mode_energy = base_vehicle_energy.Modes["regular"];
                foreach (V3OLDVehicleFuel fuel in base_mode_energy._fuelUsed)
                {
                    V3OLDVehicleFuel copy_fuel = Convenience.Clone<V3OLDVehicleFuel>(fuel);
                    copy_fuel.VolumeShare.ValueInDefaultUnit = ((1 - share_from_non_base_fuels) * fuel.VolumeShare).Value;
                    if (copy_fuel.VolumeShare.ValueInDefaultUnit < 0 || share_from_non_base_fuels < 0)
                        copy_fuel.VolumeShare.ValueInDefaultUnit = 0;

                    copy_fuel.IsFuelFromBaseVehicle = true;
                    fuelBlend.Add(copy_fuel);
                    _usesBaseFuelShares = true;

                }
            }

            return fuelBlend.ToList<V3OLDVehicleFuel>();
        }
       /// <summary>
       /// Returns the mode of the current object.
       /// </summary>
       /// <returns>CDMode for CD Mode, RegularMode for regular mode </returns>
        public string ModeType()
        {
            if (this is V3OLDCDMode)
                return "CDMode";
            else
                return "RegularMode";
        }

        #endregion accessors

        #region methods

        internal abstract XmlNode ToXmlNode(XmlDocument xmlDoc);

        #endregion methods

        #region IV3OLDVehicleMode

        /// <summary>
        /// Returns the fuels used by this V3OLDVehicle as a List of Source(IInputResourceReference) and its volumetric share.
        /// Used primarly to return the Fuels in terms of Interface objects.
        /// </summary>
        public Dictionary<IInputResourceReference, double> FuelsUsed
        {
            get
            {
                Dictionary<IInputResourceReference, double> list = new Dictionary<IInputResourceReference, double>();
                foreach (V3OLDVehicleFuel vf in this.FuelsUsedWithoutBaseFuels)
                {
                    list.Add(vf.InputResourceRef, vf.VolumeShare.ValueInDefaultUnit);
                }
                return list;
            }
        }

        public IResults PumpToWheelsResults(IData data)
        {
                Results results = new Results();
                results.BottomDim = DimensionUtils.LENGTH ;
                results.onsiteEmissions = this.OperationEmissions;
                results.onsiteResources = this.OperationEnergy;
                
                return results;
        }

        public IResults WellToPumpResults(IData data)
        {
                Results results = new Results();
                results.BottomDim = DimensionUtils.LENGTH ;
                results.wellToProductEnem.emissions = this.FuelEmissions;
                results.wellToProductEnem.materialsAmounts = this.FuelEnergy;

                return results;
        }

        public IProductionItem CDElectrictyUsed
        {
            get
            {

                if (this is V3OLDCDMode)
                {
                    V3OLDCDMode thisAsV3OLDCDMode = this as V3OLDCDMode;
                    V3OLDVehicleFuel vf = thisAsV3OLDCDMode.ElectricityUsed;

                    if (vf.InputResourceRef.SourceType == Enumerators.SourceType.Mix)
                    {
                        ParameterTS ts = new ParameterTS();
                        ts.Add(0, vf.VolumeShare);
                        return new MixProductionEntity(vf.InputResourceRef.SourceMixOrPathwayID, ts);
                    }
                    else if (vf.InputResourceRef.SourceType == Enumerators.SourceType.Pathway)
                    {
                        ParameterTS ts = new ParameterTS();
                        ts.Add(0, vf.VolumeShare);
                        return new PathwayProductionEntity(vf.InputResourceRef.SourceMixOrPathwayID, Guid.Empty, ts);
                    }
                    else
                        throw new Exception("CD Electricity used is incorrectly defined");
                }
                else
                    return null;

            }

        }
        #endregion

        public virtual void AddNewYear(GData data, int year, bool baseMpg, bool baseEmission)
        {
            if (this.Mpg != null)
            {
                if (baseMpg)
                    this.Mpg.Add(year, data.ParametersData.CreateRegisteredParameter("mi/gal", 19131505.09737243));
                else
                    this.Mpg.Add(year, data.ParametersData.CreateRegisteredParameter("%", 100));

                this.Mpg.notes.Add(year, "");
            }

            if (baseEmission)
                this.Technologies.Add(year, new V3OLDCarYearEmissionsFactors(data, year, this.Technologies, true, true));
            else
                this.Technologies.Add(year, new V3OLDCarYearEmissionsFactors(data, year, this.Technologies, true, false));
        }
    }
}
