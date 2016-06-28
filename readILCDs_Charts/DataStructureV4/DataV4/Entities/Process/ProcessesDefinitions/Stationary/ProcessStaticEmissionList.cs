using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using System.IO;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class holds the other emissions of the process like the otherEmissions
    /// </summary>
    [Serializable]
    public class ProcessStaticEmissionList
    {
        private List<ProcessStaticEmissionItem> staticEmissions;
        /// <summary>
        /// This attribute is used to ensure that there is no repeatition of the same gas in the list of nodes.
        /// </summary>
        public bool ensureDistinction = false;

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ensureDistinct">This attribute is used to ensure that there is no repeatition of the same gas in the list of nodes.</param>
        public ProcessStaticEmissionList(bool ensureDistinct)
        {
            staticEmissions = new List<ProcessStaticEmissionItem>();
            this.ensureDistinction = ensureDistinct;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">The xml node from which the emissions are needed to be calculated</param>
        /// <param name="ensureDistinct">This attribute is used to ensure that there is no repeatition of the same gas in the list of nodes.</param>
        public ProcessStaticEmissionList(GData data, XmlNode node, bool ensureDistinct, string optionalParamPrefix)
            : this(ensureDistinct)
        {
            int count = 0;
            foreach (XmlNode gas in node.SelectNodes("emission"))
            {
                string status2 = "";
                string notes;
                try
                {
                    status2 = "reading gas id";
                    int gasId = Convert.ToInt32(gas.Attributes["ref"].Value);
                    status2 = "reading gas amount";
                    ParameterTS dfactor = new ParameterTS(data, gas, optionalParamPrefix + "_stem_" + count + "_gas_" + gasId);
                    if (gas.Attributes["notes"] != null)
                        notes = gas.Attributes["notes"].Value;
                    else
                        notes = "";

                    ProcessStaticEmissionItem pse = new ProcessStaticEmissionItem(gasId, dfactor, notes);
                    this.staticEmissions.Add(pse);

                }
                catch (Exception e)
                {
                    LogFile.Write("Error 12:" + e.Message + "\r\n" + status2 + "\r\n" + gas.OuterXml + "\r\n" + gas.OwnerDocument.Name + "\r\n");
                }
                count++;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Convert that object to a xmlNode for the stationary_processes.xml file
        /// Returns a non_combustin_emission object, cannot be used for other kind of emissions
        /// </summary>
        /// <param name="processDoc"></param>
        /// <param name="name">the name of the xml mode</param>
        /// <returns></returns>
        internal System.Xml.XmlNode toXmlNode(System.Xml.XmlDocument processDoc, string name)
        {
            XmlNode nc = processDoc.CreateNode(name);
            foreach (ProcessStaticEmissionItem node in staticEmissions)
            {
                XmlNode gas = node.EmParameter.ToXmlNode(processDoc, "emission");
                gas.Attributes.Append(processDoc.CreateAttr("ref", node.GasId));
                gas.Attributes.Append(processDoc.CreateAttr("notes", node.Notes));

                nc.AppendChild(gas);
            }

            return nc;
        }

        /// <summary>
        /// Creates a new Static emission with an amount of 0 and add it to the list of static emissions
        /// 
        /// Can throw a ArgumentException in case the ID passed as an argument already exists in the list of static emissions
        /// </summary>
        /// <param name="data">The database to which the parameters for that static emission are going to be registered</param>
        /// <param name="id">The ID for the gas to be created for the static emission</param>
        public void CreateStaticEmission(GData data, int id)
        {
            if (ensureDistinction && this.staticEmissions.FindAll(item => item.GasId == id).Count > 0)
                throw new ArgumentException("An non combustion emission already exists for the same gas ID");
            else
                staticEmissions.Add(new ProcessStaticEmissionItem(data, id));

        }

        public void Remove(int idx)
        {
            staticEmissions.RemoveAll(item => item.GasId == idx);
        }

        public void Remove(ProcessStaticEmissionItem en)
        {
            staticEmissions.Remove(en);
        }

        public bool ContainsKey(int index)
        {
            if (this.staticEmissions.FindAll(item => item.GasId == index).Count > 0)
                return true;
            return false;
        }

        public List<ProcessStaticEmissionItem> StaticEmissions
        {
            get
            {
                return this.staticEmissions;
            }
        }

        #endregion

        #region Accessors
        public int Count
        {
            get
            {
                return this.staticEmissions.Count;
            }
        }
        #endregion

        #region Operators
        public static EmissionAmounts operator +(EmissionAmounts e1, ProcessStaticEmissionList e2)
        {
            EmissionAmounts emissionResult = new EmissionAmounts();

            foreach (KeyValuePair<int, double> pair in e1)
            {
                emissionResult.Add(pair.Key, pair.Value);
                foreach (ProcessStaticEmissionItem oen in e2.StaticEmissions.FindAll(item => item.GasId == pair.Key))
                    emissionResult[pair.Key] += oen.EmParameter.CurrentValue.ValueInDefaultUnit;
            }

            foreach (ProcessStaticEmissionItem en in e2.StaticEmissions)
            {
                if (!emissionResult.ContainsKey(en.GasId))
                {
                    double val = 0.0;
                    foreach (ProcessStaticEmissionItem oen in e2.StaticEmissions.FindAll(item => item.GasId == en.GasId))
                        val += oen.EmParameter.CurrentValue.ValueInDefaultUnit;
                    emissionResult.Add(en.GasId, val);
                }
            }

            return emissionResult;
        }
        public static EmissionAmounts operator *(ProcessStaticEmissionList e2, Parameter param)
        {
            EmissionAmounts emissionResult = new EmissionAmounts();

            foreach (ProcessStaticEmissionItem en in e2.StaticEmissions)
            {
                if (emissionResult.ContainsKey(en.GasId))
                    emissionResult[en.GasId] += (en.EmParameter.CurrentValue.ValueInDefaultUnit * param.ValueInDefaultUnit);
                else
                    emissionResult.Add(en.GasId, en.EmParameter.CurrentValue.ValueInDefaultUnit * param.ValueInDefaultUnit);
            }
            return emissionResult;
        }
        #endregion
    }
}
