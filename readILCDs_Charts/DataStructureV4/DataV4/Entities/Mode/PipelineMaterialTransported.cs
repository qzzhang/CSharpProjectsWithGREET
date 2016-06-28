using Greet.LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class PipelineMaterialTransported
    {
        #region attributes

        private ParameterTS energyIntensity;
        private string name;
        private int reference;

        #endregion attributes

        #region constructors

        public PipelineMaterialTransported(GData data, XmlNode node, string optionalParamPrefix = "")
        {
            string state = "";
            try
            {
                this.reference = Convert.ToInt32(node.Attributes["ref"].Value);     
                this.name = node.Attributes["name"].Value;
                if (node.SelectSingleNode("ei") != null)
                    this.energyIntensity = new ParameterTS(data, node.SelectSingleNode("ei"), optionalParamPrefix + "_pipeei_" + this.reference);
            }
            catch (Exception e)
            {
                LogFile.Write("Error 64:" + node.OwnerDocument.BaseURI + "\r\n" + node.OuterXml + "\r\n" + e.Message + "\r\n" + state);
                throw e;
            }
        }

        public PipelineMaterialTransported(GData data, int resId)
        {
            reference = resId;
            Parameter param = data.ParametersData.CreateRegisteredParameter("J/(kg m)", 0);
            energyIntensity = new ParameterTS();
            energyIntensity.Add(0, param);
        }

        #endregion constructors

        #region accessors

        public int Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        public ParameterTS EnergyIntensity
        {
            get { return energyIntensity; }
            set { energyIntensity = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion accessors

        #region methods

        internal XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode technoNode = xmlDoc.CreateNode("material_transported");
            technoNode.Attributes.Append(xmlDoc.CreateAttr("name", this.name));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("ref", this.reference));
            technoNode.AppendChild(this.energyIntensity.ToXmlNode(xmlDoc, "ei"));
            return technoNode;
        }

        public override string ToString()
        {
            return this.name;
        }

        #endregion methods
    }
}
