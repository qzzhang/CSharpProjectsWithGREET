using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Represent a operational mode for a defined vehicle
    /// The class is abstract and could be either CD or CS mode
    /// </summary>
    [Serializable]
    public class VehicleOperationalMode : IVehicleMode, IHaveAPicture
    {
        #region attributes
        /// <summary>
        /// Name of the vehicle operational mode: "cd" "cs" or "regular". "cs" and "regular" are using
        /// the same calculations so maybe we should avoid using two different names for defining the same 
        /// mode of functionnement
        /// </summary>
        string _name = "";
        /// <summary>
        /// Image name for the mode in the GUI
        /// </summary>
        string _pictureName = "empty.png";
        /// <summary>
        /// Unique ID for this mode among all the vehicle modes in the model
        /// </summary>
        Guid _uniqueId = Guid.NewGuid();
        /// <summary>
        /// Energy conversion plants (ICE, Motor, Rockets??)
        /// </summary>
        List<VehicleModePowerPlant> _plants = new List<VehicleModePowerPlant>();
        /// <summary>
        /// Notes relevant to that mode
        /// </summary>
        string _notes = "";
        /// <summary>
        /// Defines weather or not this mode is a template that can be dragged and dropped
        /// </summary>
        bool _isTemplate = false;

        /// <summary>
        /// Vehicle mile travelled share for this mode
        /// </summary>
        ParameterTS _modeVMTShare;
        #endregion attributes

        #region constructors

        /// <summary>
        /// Creates an empty mode
        /// </summary>
        public VehicleOperationalMode(GData data)
        {
            _modeVMTShare = new ParameterTS(data, "%", 0, 100);
        }

        public VehicleOperationalMode(GData data, XmlNode xmlNode, string optionalParamPrefix) : this(data)
        {
            _name = xmlNode.Attributes["name"].Value;
            _pictureName = xmlNode.Attributes["picture"].Value;
            _uniqueId = new Guid(xmlNode.Attributes["id"].Value);
            _modeVMTShare = new ParameterTS(data, xmlNode.SelectSingleNode("vmtShare"));
            if (xmlNode.Attributes["notes"] != null)
                this._notes = xmlNode.Attributes["notes"].Value;
            if (xmlNode.Attributes["isTemplate"] != null)
                _isTemplate = Convert.ToBoolean(xmlNode.Attributes["isTemplate"].Value);
            foreach (XmlNode fuel_node in xmlNode.SelectNodes("plant"))
            {
                try
                {
                    VehicleModePowerPlant pp = new VehicleModePowerPlant(data, fuel_node, optionalParamPrefix + "plant");
                    _plants.Add(pp);

                }
                catch (Exception e)
                {
                    LogFile.Write("Error 3:" + e.Message + "\r\n" + xmlNode.OwnerDocument.Name + "\r\n" + xmlNode.OuterXml + "\r\n");
                    throw e;
                }
            }
        }

        #endregion constructors

        #region accessors
        public ParameterTS  VMTShare
        {
            get { return _modeVMTShare; }
            set { _modeVMTShare = value; }
        }
        /// <summary>
        /// Powerplants for this mode
        /// </summary>
        public List<VehicleModePowerPlant> Plants
        {
            get { return _plants; }
            set { _plants = value; }
        }
        /// <summary>
        /// Name of the picture to be used for this mode
        /// </summary>
        /// <summary>
        /// Can be used as a template
        /// </summary>
        public bool IsTemplate
        {
            get { return _isTemplate; }
            set { _isTemplate = value; }
        }
        /// <summary>
        /// Picture name for that mode
        /// </summary>
        public string PictureName
        {
            get { return _pictureName; }
            set { _pictureName = value; }
        }
        /// <summary>
        /// Unique ID for this mode among all the vehicle modes in the model
        /// </summary>
        public Guid UniqueId
        {
            get { return _uniqueId; }
            set { _uniqueId = value; }
        }
        /// <summary>
        /// Name for the mode
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Notes associated with the mode
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        #endregion accessors

        #region public methods

        /// <summary>
        /// Returns the Sum of calculated operation energy for all the plants in the mode
        /// </summary>
        /// <returns></returns>
        public ResourceAmounts CalculatedOperationEnergy()
        {
            ResourceAmounts returned = new ResourceAmounts();
            returned.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
                returned += plant.CalculatedOperationEnergy;
            return returned;
        }

        /// <summary>
        /// Returns the Sum of calculated operation energy for all the plants in the mode
        /// </summary>
        /// <returns></returns>
        public ResourceAmounts CalculatedOperationEnergyWithUpstream()
        {
            ResourceAmounts returned = new ResourceAmounts();
            returned.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
                returned += plant.CalculatedTotalEnergy;
            return returned;
        }

        /// <summary>
        /// Returns the Sum of calculated operation emissions for all the plants in the mode
        /// </summary>
        /// <returns></returns>
        public EmissionAmounts CalculatedOperationEmissions()
        {
            EmissionAmounts returned = new EmissionAmounts();
            returned.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
                returned += plant.CalculatedOperationEmissions;
            return returned;
        }


        /// <summary>
        /// Returns the Sum of calculated operation emissions for all the plants in the mode
        /// </summary>
        /// <returns></returns>
        public EmissionAmounts CalculatedOperationEmissionsWithUpstream()
        {
            EmissionAmounts returned = new EmissionAmounts();
            returned.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
                returned += plant.CalculatedTotalEmissions;
            return returned;
        }

        /// <summary>
        /// Returns the Sum of calculated operation urban emissions for all the plants in the mode
        /// </summary>
        /// <returns></returns>
        public EmissionAmounts CalculatedOperationEmissionsUrban()
        {
            EmissionAmounts returned = new EmissionAmounts();
            returned.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
                returned += plant.CalculatedOperationEmissionsUrban;
            return returned;
        }

        /// <summary>
        /// Returns the Sum of calculated operation urban emissions for all the plants in the mode
        /// </summary>
        /// <returns></returns>
        public EmissionAmounts CalculatedOperationEmissionsUrbanWithUpstream()
        {
            EmissionAmounts returned = new EmissionAmounts();
            returned.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
                returned += plant.CalculatedTotalEmissionsUrban;
            return returned;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates xml node with name and notes as attribuets. The children are called fuel for each of the fules used and year for each of the year data for emissions
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        internal XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode modeNode = xmlDoc.CreateNode("mode", xmlDoc.CreateAttr("name", _name), xmlDoc.CreateAttr("id", _uniqueId.ToString()), xmlDoc.CreateAttr("picture", _pictureName), xmlDoc.CreateAttr("notes", _notes), xmlDoc.CreateAttr("isTemplate", _isTemplate));
            modeNode.AppendChild(_modeVMTShare.ToXmlNode(xmlDoc, "vmtShare"));
            foreach (VehicleModePowerPlant plant in _plants)
            {
                XmlNode plantNode = plant.ToXmlNode(xmlDoc);
                modeNode.AppendChild(plantNode);
            }

            return modeNode;
        }

        #endregion methods

        #region IVehicleMode
        /// <summary>
        /// Combines emissions and energy into a single Results object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IResults PumpToWheelsResults(IData data)
        {
            Results results = new Results();
            results.BottomDim = DimensionUtils.LENGTH;
            foreach (VehicleModePowerPlant plant in _plants)
            {
                results.onsiteEmissions += plant.CalculatedOperationEmissions;
                results.onsiteResources += plant.CalculatedOperationEnergy;
            }
            return results;
        }

        /// <summary>
        /// Combines emissions and energy into a single Results object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IResults WellToPumpResults(IData data)
        {
                Results results = new Results();
                results.BottomDim = DimensionUtils.LENGTH ;
                //foreach (VehicleModePowerPlant plant in _plants)
                //{
                //    results.wellToProductEnem.emissions = plant.SumFuelEmission();
                //    results.wellToProductEnem.materialsAmounts = plant;
                //}
                return results;
        }

        /// <summary>
        /// Returns a list of plants used in this mode
        /// </summary>
        IEnumerable<IVehicleModePlant> IVehicleMode.Plants
        {
            get 
            {
                List<IVehicleModePlant> plants = new List<IVehicleModePlant>();
                foreach (VehicleModePowerPlant plant in _plants)
                    plants.Add(plant as IVehicleModePlant);
                return plants;
            }
        }
        #endregion
    }
}
