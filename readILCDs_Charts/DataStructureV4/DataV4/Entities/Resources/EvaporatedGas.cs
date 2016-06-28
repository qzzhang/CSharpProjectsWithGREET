using System;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class EvaporatedGas : IEvaporatedGas 
    {
        #region attributes

        /// <summary>
        /// ID of the gas/pollutant to be associated with that evaporation emission
        /// </summary>
        private int gasIdRef;
        /// <summary>
        /// Mass ratio for that pollutant ID beeing released when a resource is leaked/lost
        /// </summary>
        private Parameter massRatio;

        #endregion

        #region constructor
        public EvaporatedGas(GData data, XmlNode node, string optionalParamPrefix)
        {
            this.FromXmlNode(data, node, optionalParamPrefix);
            
        }

        public EvaporatedGas(GData data, int p)
        {
            this.gasIdRef = p;
            this.massRatio = data.ParametersData.CreateRegisteredParameter("%", 0);
        }
        #endregion

        #region methods

        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            this.gasIdRef = Convert.ToInt32(node.Attributes["ref"].Value);
            this.massRatio =data.ParametersData.CreateRegisteredParameter(node.Attributes["share"], optionalParamPrefix + "_" + gasIdRef);
        }

        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("gas", xmlDoc.CreateAttr("ref", this.GasIdReference), xmlDoc.CreateAttr("share", this.MassRatio));
            return node;
        }

        #endregion

        #region Accessor
        public int GasIdReference
        {
            get { return gasIdRef; }
            set { gasIdRef = value; }
        }

        public Parameter MassRatio
        {
            get { return massRatio; }
            set { massRatio = value; }
        }

        public IParameter Ratio
        {
            get
            {
                return massRatio as IParameter;
            }
            set
            {
                massRatio = value as Parameter;
            }

        }

        #endregion
    }
}
