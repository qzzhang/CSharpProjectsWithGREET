using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Defines a class that holds ratios to a real emission factors in another technology
    /// </summary>
    [Serializable]
    public class BasedEmissionFactors : EmissionsFactors
    {
        #region attributes
        /// <summary>
        /// Ratios to the base technology that are going to be applied for any of the years
        /// </summary>
        private Dictionary<int, EmissionValue> ratios = new Dictionary<int, EmissionValue>();
        #endregion attributes

        #region constructor

        public BasedEmissionFactors(int year)
        {
            this.Year = year;
        }

        public BasedEmissionFactors(GData data, XmlNode node, string optionalParamPrefix)
        {
            if (node.Attributes["value"] != null)
                this.Year = Convert.ToInt32(node.Attributes["value"].Value);
            else
                this.Year = 0; //default year 
            if (node.Attributes["notes"] != null)
                this.Notes = node.Attributes["notes"].Value;

            foreach (XmlNode gas in node.SelectNodes("gas | gas_ratio"))
            {
                int gasId = Convert.ToInt32(gas.Attributes["ref"].Value);
                EmissionValue dfactor;
                if (gas.Attributes["calculated"] != null)
                {
                    string[] split = gas.Attributes["calculated"].Value.Split(';');
                    if (split.Count() == 3 && Convert.ToBoolean(split[1]) == false)
                    {//old version of the database 
                        Parameter param = data.ParametersData.CreateRegisteredParameter(split[2], 0, Convert.ToDouble(split[0]), optionalParamPrefix + "_year" + this.Year + "_fact_" + gasId);
                        param.UseOriginal = false;
                        //we set the value to a percentage if it's not
                        param.UserValuePreferedExpression = "%";
                        dfactor = new EmissionValue(param, false);
                    }
                    else
                        dfactor = new EmissionValue(null, true);
                }
                else
                {
                    XmlAttribute ratioNode = null;
                    if (gas.Attributes["ratio"] != null)
                        ratioNode = gas.Attributes["ratio"];
                    else if (gas.Attributes["factor"] != null)
                        ratioNode = gas.Attributes["factor"];
                    Parameter param = data.ParametersData.CreateRegisteredParameter(ratioNode, optionalParamPrefix + "_year" + this.Year + "_fact_" + gasId);
                    //we set the value to a percentage if it's not
                    param.UserValuePreferedExpression = "%";
                    dfactor = new EmissionValue(param, false);
                }

                this.ratios.Add(gasId, dfactor);
            }
        }

        protected BasedEmissionFactors(SerializationInfo info, StreamingContext context)
        {
        
        }

        #endregion constuctor

        #region methods

        public void GetObjectData(SerializationInfo info,
                                   StreamingContext context)
        {

        }

        /// <summary>
        /// Returns an xmlNode containing all the emission factors defined for this instance
        /// </summary>
        /// <param name="doc">Parent document for creating new node</param>
        /// <param name="name">Optional name, the default name of the node is year</param>
        /// <returns></returns>
        public override XmlNode ToXmlNode(XmlDocument doc, string name = "year")
        {
            XmlNode yearNode = doc.CreateNode(name, doc.CreateAttr("value", this.Year), doc.CreateAttr("notes", this.Notes));
            foreach (KeyValuePair<int, EmissionValue> node in this.ratios)
            {
                XmlNode gasFactor = doc.CreateNode("gas", doc.CreateAttr("ref", node.Key));

                if (node.Value.Balanced)
                    gasFactor.Attributes.Append(doc.CreateAttr("calculated", true));
                else
                {
                    Parameter val = node.Value.Value;
                    XmlAttribute factor = val.ToXmlAttribute(doc, "ratio");
                    gasFactor.Attributes.Append(factor);
                }

                yearNode.AppendChild(gasFactor);
            }
            return yearNode;
        }

        #endregion

        #region accessors

        public Dictionary<int, EmissionValue> Ratios
        {
            get { return this.ratios; }
            set { this.ratios = value; }
        }

        #endregion accessors

        /// <summary>
        /// Finds recursively all the dependent referenced technologies use as a base and calculate emission factors for this given year
        /// If for any of the year the current year is not found, the default year will be used
        /// If the default year does not exists in the refered technology returns no emission factors
        /// If the refered technology does not exists returns no emission factors
        /// </summary>
        /// <param name="technologiesData">Collection of available technologies in the database</param>
        /// <param name="baseTechnologyID">Technology that needs to be used as a reference for this year</param>
        /// <returns></returns>
        public Dictionary<int, LightValue> CalculateBaseEmissionFactors(Technologies technologiesData, int baseTechnologyID)
        {
            Dictionary<int, LightValue> toBeReturned = new Dictionary<int, LightValue>();
            if (technologiesData.ContainsKey(baseTechnologyID))
            {
                TechnologyData baseTech = technologiesData[baseTechnologyID];
                if (baseTech.BaseTechnology == -1)
                {//the refered technology contains emission factors
                    RealEmissionsFactors yearColumnToUse = null;
                    if (baseTech.ContainsKey(this.Year))
                        yearColumnToUse = baseTech[this.Year] as RealEmissionsFactors;
                    else if (baseTech.ContainsKey(0))
                        yearColumnToUse = baseTech[0] as RealEmissionsFactors;
                    else
                        yearColumnToUse = baseTech.CurrentValue as RealEmissionsFactors;

                    //returns an empty dictionary of emission factors if the years cannot be found in the referenced technology
                    if(yearColumnToUse == null)
                        return toBeReturned;

                    //fill up the emission based on the found technology and year
                    foreach (KeyValuePair<int, EmissionValue> ef in yearColumnToUse.EmissionFactors)
                    {
                        if (this.ratios.ContainsKey(ef.Key))
                        {
                            if (this.ratios[ef.Key].Balanced == false && yearColumnToUse.EmissionFactors[ef.Key].Balanced == false)
                                toBeReturned.Add(ef.Key,  ef.Value.Value.ToLightValue() * this.Ratios[ef.Key].Value);
                            else
                                toBeReturned.Add(ef.Key, null);
                        }
                    }
                    return toBeReturned;
                }
                else
                {//the refered technology contains other ratios to a different based technology
                    BasedEmissionFactors yearColumnToUse = null;
                    if (baseTech.ContainsKey(this.Year))
                        yearColumnToUse = baseTech[this.Year] as BasedEmissionFactors;
                    else if (baseTech.ContainsKey(0))
                        yearColumnToUse = baseTech[0] as BasedEmissionFactors;

                    //returns an empty dictionary of emission factors if the years cannot be found in the referenced technology
                    if (yearColumnToUse == null)
                        return toBeReturned;

                    //calculated recursively the based emission factors for this technology
                    Dictionary<int, LightValue> calculated = yearColumnToUse.CalculateBaseEmissionFactors(technologiesData, baseTech.BaseTechnology); 

                    //fill up the emission based on the found technology and year
                    foreach (KeyValuePair<int, LightValue> ef in calculated)
                        if (this.ratios.ContainsKey(ef.Key))
                        {
                            if (this.ratios[ef.Key].Balanced == false && ef.Value != null)
                                toBeReturned.Add(ef.Key, ef.Value * this.Ratios[ef.Key].Value);
                            else
                                toBeReturned.Add(ef.Key, null);
                        }
                    return toBeReturned;
                }
            }
            return toBeReturned;
        }

        public override bool CheckIntegrity(GData data, bool showIds, out string efErrMsg)
        {
            efErrMsg = "";
            return true;
        }
    }
}
