using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class ParameterTS : TimeSeries<Parameter>
    {
        #region constructors
        private ParameterTS(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {

        }
        /// <summary>
        /// Creates an empty object, NO Parameters instances created
        /// </summary>
        public ParameterTS()
        { 
        }

        /// <summary>
        /// Creates a new ParameterTS and registers the created unique parameter for year 0
        /// </summary>
        /// <param name="data"></param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="greetVal">The desired value for the value in the given prefered unit expression, will be set as the user value for the parameter</param>
        /// <param name="optionalParamPrefix"></param>
        public ParameterTS(GData data, string preferedUnitExpression, double greetVal, double userVal = 0, string optionalParamPrefix = "")
        {
            this.SetUniqueValueTS(data, preferedUnitExpression, greetVal, userVal, optionalParamPrefix);
        }
        /// <summary>
        /// Creates a new ParameterTS and registers all created parameters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="node"></param>
        /// <param name="optionalParamPrefix"></param>
        internal ParameterTS(GData data, XmlNode node, string optionalParamPrefix = "")
        {
            this.FromXmlNode(data, node, optionalParamPrefix);
        }

        #endregion constructors

        #region accessors
     
        private bool UseOriginal
        {
            set
            {
                foreach (Parameter dv in this.Values)
                    dv.UseOriginal = value;
            }
        }
        #endregion

        #region methods

        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix = "")
        {
            foreach (XmlNode yearNode in node.SelectNodes("year"))
            {
                int year = Convert.ToInt32(yearNode.Attributes["year"].Value);
                Parameter value = data.ParametersData.CreateRegisteredParameter(yearNode.Attributes["value"], optionalParamPrefix + "_" + year );
                this.Add(year, value);
            }

            if (node.Attributes["notes"] != null)
                _notes = node.Attributes["notes"].Value;
            if (node.Attributes["mostRecent"] != null)
                _mostRecentData = Convert.ToInt32(node.Attributes["mostRecent"].Value);
        }
        /// <summary>
        /// Creates a Parameter for year 0 (the default year) of the time series object
        /// </summary>
        /// <param name="data">Data for registering the parameter</param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation of the heating value, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="greetVal">The desired valuen the given prefered unit expression, will be set as the user value for the parameter</param>
        /// <param name="userVal">The desired valuen the given prefered unit expression, will be set as the user value for the parameter</param>
        /// <param name="optionalParamPrefix">Prefix for the parameter unique ID creation</param>
        private void SetUniqueValueTS(GData data, string preferedUnitExpression, double greetVal, double userVal, string optionalParamPrefix = "")
        {
            this.Add(0, data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, greetVal, userVal, optionalParamPrefix));
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc, string node_name)
        {
            XmlNode node = xmlDoc.CreateNode(node_name, xmlDoc.CreateAttr("mostRecent", _mostRecentData), xmlDoc.CreateAttr("notes", _notes));
            if (this.Count > 0)
            {
                foreach (KeyValuePair<int, Parameter> eff in this)
                    node.AppendChild(xmlDoc.CreateNode("year", xmlDoc.CreateAttr("value", eff.Value), xmlDoc.CreateAttr("year", eff.Key)));
            }
 
            return node;
        }
        #endregion methods
    }
}