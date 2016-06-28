using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// To hold the emission factors read from the database, and stored as a double Value
    /// </summary>
    [Serializable]
    public class RealEmissionsFactors : EmissionsFactors
    {
        #region private members
        /// <summary>
        /// Emission factors that can be used directly in the calculations
        /// </summary>
        private Dictionary<int, EmissionValue> emissionFactors = new Dictionary<int, EmissionValue>();
        #endregion

        #region constructors
        /// <summary>
        /// Create an empty dictionary of emission factors
        /// </summary>
        public RealEmissionsFactors()
            : base()
        { }

        /// <summary>
        /// Initialize a specific value for each available gas in the database
        /// </summary>
        /// <param name="init_val"></param>
        public RealEmissionsFactors(int year)
            : base()
        {
            this.Year = year;
        }

        public RealEmissionsFactors(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status;
            status = "creating gases dictionary";

            if (node.Attributes["value"] != null)
                this.Year = Convert.ToInt32(node.Attributes["value"].Value);
            else
                this.Year = 0; //default year 
            if (node.Attributes["notes"] != null)
                this.Notes = node.Attributes["notes"].Value;

            foreach (XmlNode gas in node.SelectNodes("emission"))
            {
                try
                {
                    status = "reading gas id";
                    int gasId = Convert.ToInt32(gas.Attributes["ref"].Value);
                    status = "reading gas factor";
                    EmissionValue dfactor;
                    if (gas.Attributes["calculated"] != null)
                    {
                        string[] split = gas.Attributes["calculated"].Value.Split(';');
                        if (split.Count() == 3 && Convert.ToBoolean(split[1]) == false)
                        {//old version of the database 
                            Parameter param = data.ParametersData.CreateRegisteredParameter(split[2], 0, Convert.ToDouble(split[0]), optionalParamPrefix + "_year" + this.Year + "_fact_" + gasId);
                            param.UseOriginal = false;
                            dfactor = new EmissionValue(param, false);
                        }
                        else
                            dfactor = new EmissionValue(null, true);
                    }
                    else
                    {
                        Parameter param = data.ParametersData.CreateRegisteredParameter(gas.Attributes["factor"], optionalParamPrefix + "_year" + this.Year + "_fact_" + gasId);
                        dfactor = new EmissionValue(param, false);
                    }
                    status = "add gas factor to the dictionary";

                    this.emissionFactors.Add(gasId, dfactor);
                }
                catch (Exception e)
                {
                    LogFile.Write("Error 12:" + e.Message + "\r\n" + status + "\r\n" + gas.OuterXml + "\r\n" + gas.OwnerDocument.Name + "\r\n");
                }
            }
        }

        protected RealEmissionsFactors(SerializationInfo info, StreamingContext context)
        {
            
        }

        #endregion

        #region methods

        public void GetObjectData(SerializationInfo info,
                                   StreamingContext context)
        {

        }

        public override XmlNode ToXmlNode(XmlDocument doc, string name = "year")
        {
            XmlNode yearNode = doc.CreateNode(name, doc.CreateAttr("value", this.Year), doc.CreateAttr("notes", this.Notes));

            this.AppendXmlNodes(doc, yearNode);
           
            return yearNode;
        }

        public void AppendXmlNodes(XmlDocument doc, XmlNode root)
        {
            foreach (KeyValuePair<int, EmissionValue> node in this.emissionFactors)
            {
                XmlNode gasFactor = doc.CreateNode("emission", doc.CreateAttr("ref", node.Key));

                if (node.Value.Balanced)
                    gasFactor.Attributes.Append(doc.CreateAttr("calculated", true));
                else
                {
                    Parameter val = node.Value.Value;
                    XmlAttribute factor = val.ToXmlAttribute(doc, "factor");
                    gasFactor.Attributes.Append(factor);
                }

                root.AppendChild(gasFactor);
            }
        }

        #endregion

        #region accessors

        public Dictionary<int, EmissionValue> EmissionFactors
        {
            get { return emissionFactors; }
            set { emissionFactors = value; }
        }

        #endregion accessors


        public override bool CheckIntegrity(GData data, bool showIds, out string efErrMsg)
        {
            efErrMsg = "";
            return true;
        }
    }
}
