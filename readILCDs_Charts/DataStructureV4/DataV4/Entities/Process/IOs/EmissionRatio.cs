using System;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.UnitLib3;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4
{
    [Serializable]
    public class EmissionRatio
    {
        #region attributes

        private ParameterTS rate;
        private int gasRef;
        private LightValue calculatedEmission;
        #endregion

        #region Accessors
        public ParameterTS Rate
        {
            get { return rate; }
            set { rate = value; }
        }
       
        public int GasRef
        {
            get { return gasRef; }
            set { gasRef = value; }
        }

        public LightValue CalculatedEmission
        {
            get { return calculatedEmission; }
            set { calculatedEmission = value; }
        }

        #endregion

        #region constructors

        public EmissionRatio(GData data, XmlNode node, string parameterPrefix)
        {
            this.gasRef = Convert.ToInt16(node.Attributes["gas_id"].Value);

            if (node.Attributes["rate"] != null)
            {
                Parameter tempParam = data.ParametersData.CreateRegisteredParameter(node.Attributes["rate"], parameterPrefix + "emission_ratio_"+gasRef.ToString()+"_rate");
                this.rate = new ParameterTS();
                this.rate.Add(0, tempParam);
            }
            else if (node.SelectSingleNode("rate") != null)
                this.rate = new ParameterTS(data, node.SelectSingleNode("rate"), parameterPrefix + "emission_ratio_"+gasRef.ToString()+"_rate");
            else
                throw new Exception("no rate detected");
        }

        public EmissionRatio(GData data)
        {
            this.rate = new ParameterTS(data, "%", 0, 0);
        }


        #endregion

        #region methods

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("emission_ratio", xmlDoc.CreateAttr("gas_id", this.GasRef));
            if (this.Rate != null)
                node.AppendChild(Rate.ToXmlNode(xmlDoc, "rate"));

            return node;
        }

        #endregion
    }
}
