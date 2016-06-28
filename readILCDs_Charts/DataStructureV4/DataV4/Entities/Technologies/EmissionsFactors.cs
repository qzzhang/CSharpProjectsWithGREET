using System;
using System.Collections.Generic;
using System.Xml;
using Greet.DataStructureV4.Interfaces;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Emissions factors for a given year, and instance of this class holds one emission factor per gas
    /// </summary>
    [Serializable]
    public abstract class EmissionsFactors : IEmissionsFactors
    {
        /// <summary>
        /// The year for which this technology is defined, 0 is the default year for all technologies
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Notes that may be associated with that technology for that specific year
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Values that are going to be used during the calculations after pre-processing
        /// </summary>
        public Dictionary<int, LightValue> EmissionFactorsForCalculations = new Dictionary<int, LightValue>();
        /// <summary>
        /// Returns an XML node for that year, the node will contains the year value and emission factors for each gas
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract XmlNode ToXmlNode(XmlDocument doc, string name = "");

        public abstract bool CheckIntegrity(GData data, bool showIds, out string efErrMsg);
    }
}
