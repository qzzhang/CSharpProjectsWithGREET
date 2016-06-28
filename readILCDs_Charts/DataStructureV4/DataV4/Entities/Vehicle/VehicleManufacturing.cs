using Greet.DataStructureV4.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greet.ConvenienceLib;
using System.Xml;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;

namespace Greet.DataStructureV4.Entities
{
    public class VehicleManufacturing
    {
        #region private members
        /// <summary>
        /// Name for the manufacturing category
        /// </summary>
        private string _name = "";
        /// <summary>
        /// List of materials used to make and operate the vehicle (metal, rubber, ...)
        /// </summary>
        private List<InputResourceReference> _materials = new List<InputResourceReference>();
        /// <summary>
        /// This would usually store the mass of the components, for example mass of the gearbox or mass of a tire
        /// </summary>
        private List<ParameterTS> _materials_unitary_quantity = new List<ParameterTS>();
        /// <summary>
        /// Number of units needed for the vehicle to operate, for example 1 gearbox or 4 tires
        /// </summary>
        private List<ParameterTS> _materials_units_needed = new List<ParameterTS>();
        /// <summary>
        /// Number of replacements for the vehicle components, for example if the units_needed is 4 items and replacements is set to 3, we then use 12 tires for the upstream calculations
        /// </summary>
        private List<ParameterTS> _materials_replacements = new List<ParameterTS>();
        /// <summary>
        /// Total energy and resources for vehicle fabrication allocated over lifetime of the vehicle. Includes the upstream for resources mixes and pathways
        /// List is ordered in the same order as Materials
        /// </summary>
        private List<ResourceAmounts> _calculatedMaterialsEnergy = new List<ResourceAmounts>();
        /// <summary>
        /// Total emissions for vehicle fabrication allocated over lifetime of the vehicle. Includes the upstream for resources mixes and pathways
        /// List is ordered in the same order as Materials
        /// </summary>
        private List<EmissionAmounts> _calculatedMaterialsEmissions = new List<EmissionAmounts>();
        /// <summary>
        /// Total emissions for vehicle fabrication allocated over lifetime of the vehicle. Includes the upstream for resources mixes and pathways
        /// List is ordered in the same order as Materials
        /// </summary>
        private List<EmissionAmounts> _calculatedMaterialsEmissionsUrban = new List<EmissionAmounts>();
        #endregion

        #region public constructors

        public VehicleManufacturing(GData data, System.Xml.XmlNode manufacturingNode)
        {
            _name = manufacturingNode.Attributes["name"].Value;
            foreach (XmlNode materialNode in manufacturingNode.SelectNodes("material"))
            {
                _materials.Add(new InputResourceReference(Convert.ToInt32(materialNode.Attributes["resource_id"].Value),
                                                  Convert.ToInt32(materialNode.Attributes["entity_id"].Value),
                                                  (Enumerators.SourceType)Enum.Parse(typeof(Enumerators.SourceType), materialNode.Attributes["source_type"].Value)
                                                  ));
                _materials_unitary_quantity.Add(new ParameterTS(data, materialNode.SelectSingleNode("quantity")));
                _materials_units_needed.Add(new ParameterTS(data, materialNode.SelectSingleNode("units")));
                _materials_replacements.Add(new ParameterTS(data, materialNode.SelectSingleNode("replacements")));
            }
        }

        public VehicleManufacturing(string name)
        {
            _name = name;
        }

        #endregion

        #region public accessors
        /// <summary>
        /// Name for the manufacturing category
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// List of materials used to make and operate the vehicle (metal, rubber, ...)
        /// </summary>
        public List<InputResourceReference> Materials
        {
            get { return _materials; }
            set { _materials = value; }
        }
        /// <summary>
        /// Quantity of each material required to make a vehicle. This list must be of the same lenght as materials
        /// </summary>
        public List<ParameterTS> MaterialsQuantity
        {
            get { return _materials_unitary_quantity; }
            set { _materials_unitary_quantity = value; }
        }

        public List<ParameterTS> UnitsRequired
        {
            get { return _materials_units_needed; }
            set { _materials_units_needed = value; }
        }

        /// <summary>
        /// Replcements of each material required to make a vehicle. This list must be of the same lenght as materials
        /// </summary>
        public List<ParameterTS> MaterialsReplacements
        {
            get { return _materials_replacements; }
            set { _materials_replacements = value; }
        }
        /// <summary>
        /// Ordered list of resources and eneergy associated with materials needed for the production of the car
        /// allocated over the total vehicle mileage
        /// </summary>
        public List<ResourceAmounts> CalculatedMaterialsEnergy
        {
            get { return _calculatedMaterialsEnergy; }
            set { _calculatedMaterialsEnergy = value; }
        }
        /// <summary>
        /// Ordered list of emissions associated with materials needed for the production of the car
        /// allocated over the total vehicle mileage
        /// </summary>
        public List<EmissionAmounts> CalculatedMaterialsEmissions
        {
            get { return _calculatedMaterialsEmissions; }
            set { _calculatedMaterialsEmissions = value; }
        }
        /// <summary>
        /// Ordered list of urban emissions associated with materials needed for the production of the car
        /// allocated over the total vehicle mileage
        /// </summary>
        public List<EmissionAmounts> CalculatedMaterialsEmissionsUrban
        {
            get { return _calculatedMaterialsEmissionsUrban; }
            set { _calculatedMaterialsEmissionsUrban = value; }
        }
        #endregion

        #region public methods
        internal System.Xml.XmlNode ToXmlNode(System.Xml.XmlDocument xmlDoc)
        {
            XmlNode manufNode = xmlDoc.CreateNode("manufacturing", xmlDoc.CreateAttr("name", _name));
            for (int i = 0; i < _materials.Count; i++)
            {
                XmlNode cnode = xmlDoc.CreateNode("material",
                    xmlDoc.CreateAttr("resource_id", _materials[i].ResourceId),
                    xmlDoc.CreateAttr("entity_id", _materials[i].SourceMixOrPathwayID),
                    xmlDoc.CreateAttr("source_type", _materials[i].SourceType.ToString())                        
                    );
                cnode.AppendChild(_materials_unitary_quantity[i].ToXmlNode(xmlDoc, "quantity"));
                cnode.AppendChild(_materials_replacements[i].ToXmlNode(xmlDoc, "replacements"));
                cnode.AppendChild(_materials_units_needed[i].ToXmlNode(xmlDoc, "units"));
                manufNode.AppendChild(cnode);
            }
            return manufNode;
        }
        #endregion
    }
}
