using System;

using System.Xml;


using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;


namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class Sequestration : ISequestration
    {
        #region attributes

        /// <summary>
        /// The mix or pathway used for the energy used to sequestrate co2, usually a mix of electricity
        /// </summary>
        private int pathwayOrMix = -1;


        /// <summary>
        /// The energy used to sequestrate the co2
        /// </summary>
        private int materialId = -1;

        /// <summary>
        /// The source of the energy used to seuqestrate the co2
        /// </summary>
        private Enumerators.SourceType source;

        /// <summary>
        /// Energy ratio, how much energy is used to remove 1kg of co2 from the emissions
        /// </summary>
        private Parameter energyPerKgSequestrated;


        /// <summary>
        /// The ratio of co2 removed from the emissions, usually arround 90% is captured
        /// </summary>
        private Parameter ratioCo2Removed;
       
        #endregion attributes

        #region constructors
        public Sequestration(GData data) {
            this.ratioCo2Removed = data.ParametersData.CreateRegisteredParameter("%", 100);
            this.energyPerKgSequestrated = data.ParametersData.CreateRegisteredParameter("Btu", 10);
        
        }
        public Sequestration(GData data, XmlNode node, string optionalParamPrefix)
        {
            if (node.Attributes["ratio"] != null)
            {
                this.ratioCo2Removed = data.ParametersData.CreateRegisteredParameter(node.Attributes["ratio"], optionalParamPrefix + "_ratio");
            }
            if (node.Attributes["amount"] != null)
            {
                this.energyPerKgSequestrated = data.ParametersData.CreateRegisteredParameter(node.Attributes["amount"], optionalParamPrefix + "_amt");
            }
            if (node.Attributes["ref"] != null)
            {
                this.materialId = Convert.ToInt32(node.Attributes["ref"].Value);
            }
            if (node.Attributes["mix"] != null)
            {
                this.pathwayOrMix = Convert.ToInt32(node.Attributes["mix"].Value);
                this.source = Enumerators.SourceType.Mix;
            }
            if (node.Attributes["pathway"] != null)
            {
                this.pathwayOrMix = Convert.ToInt32(node.Attributes["pathway"].Value);
                this.source = Enumerators.SourceType.Pathway;
            }
        }
        #endregion constructors

        #region Accessors
        public XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode input;
            if (this.source == Enumerators.SourceType.Mix)
                input = doc.CreateNode("sequestration", doc.CreateAttr("source", this.source), doc.CreateAttr("mix", this.pathwayOrMix), doc.CreateAttr("ratio",
                this.RatioCo2Removed), doc.CreateAttr("ref", this.materialId), doc.CreateAttr("amount", this.EnergyPerKgSequestrated));
            else
                input = doc.CreateNode("sequestration", doc.CreateAttr("source", this.source), doc.CreateAttr("pathway", this.pathwayOrMix), doc.CreateAttr("ratio",
                this.RatioCo2Removed), doc.CreateAttr("ref", this.materialId), doc.CreateAttr("amount", this.EnergyPerKgSequestrated));

            return input;
        }

        /// <summary>
        /// The ratio of co2 removed from the emissions, usually arround 90% is captured
        /// </summary>
        public Parameter RatioCo2Removed
        {
            get { return ratioCo2Removed; }
            set { ratioCo2Removed = value; }
        }

        /// <summary>
        /// Energy ratio, how much energy is used to remove 1kg of co2 from the emissions
        /// </summary>
        public Parameter EnergyPerKgSequestrated
        {
            get { return energyPerKgSequestrated; }
            set { energyPerKgSequestrated = value; }
        }

        public int PathwayOrMix
        {
            get { return this.pathwayOrMix; }
            set { this.pathwayOrMix = value; }
        }

        public Greet.DataStructureV4.Interfaces.Enumerators.SourceType SourceType
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public int MaterialId
        {
            get { return this.materialId; }
            set { this.materialId = value; }
        }

        public IInputResourceReference ResourceReference
        {

            get
            {
                return new InputResourceReference(materialId, pathwayOrMix, source);
            }
        }
        #endregion
    }

}
