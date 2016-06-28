using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Greet.DataStructureV4.Interfaces;
using Greet.UnitLib3;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4
{
    #region Enumerators
    /// <summary>
    /// Specifies what type of result is being stored from WTP.  
    /// </summary>
    [Serializable, Obfuscation(Feature = "renaming", Exclude = true)]
    public enum ItemType { Base, Groups, Total };

    /// <summary>
    /// Specifies what type of unit the result has 
    /// </summary>
    [Serializable, Obfuscation(Feature = "renaming", Exclude = true)]
    public enum MonitorCollectionType { emission, energy, urbanemission };

    /// <summary>
    /// Specifies the source of the result from WTW
    /// </summary>
    [Serializable, Obfuscation(Feature = "renaming", Exclude = true)]
    public enum MonitorReferenceType { process_reference, pathway, mix, vehicle };

    /// <summary>
    /// Specifies what the monitored value's state is
    /// </summary>
    [Serializable, Obfuscation(Feature = "renaming", Exclude = true)]
    public enum MonitorState { BelowLimit, WithinLimit, AboveLimit };

    #endregion

    /// <summary>
    /// Interface for monitor values. It contains the mehtods and properties that should
    /// be implemented by a monitor value. 
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public abstract class AMonitor : IMonitor
    {
        /// <summary>
        /// A unique name for the Monitor item whithin the list of all monitored item
        /// </summary>
        protected string uniqueId;

        /// <summary>
        /// Outputs the data of the monitor value in XML format for saving
        /// </summary>
        /// <param name="doc">XML document being written too</param>
        /// <param name="exportingResults">A bool that if set true indicates that the calculation result values will be written to the save file.</param>
        /// <returns></returns>
        public abstract XmlNode ToXmlNode(XmlDocument doc, bool exportingResults = false);

        /// <summary>
        /// Returns the current value for the lastest calculation run 
        /// </summary>
        /// <returns></returns>
        public abstract LightValue GetCurrentCalculationValue();

        /// <summary>
        /// returns the index of the latest calculation stored in this monitor value
        /// </summary>
        public abstract int LatestCalculationRunIndex { get; }

        /// <summary>
        /// For each calculations, we store a value here, so we can trace how the monitored item evolved 
        /// </summary>
        public abstract Dictionary<int, LightValue> CalculationResultsValues { get; }

        /// <summary>
        /// Stores the sheet name, cell and unit of the corresponding
        /// value found in the Greet Base Excel Sheet. 
        /// </summary>
        public abstract MonitorExcelLocationData ExcelLocationData { get; set; }

        public abstract bool IsAlert { get; set; }

        public abstract Parameter Mean { get; set; }

        public abstract Parameter Tolerance { get; set; }

        /// <summary>
        /// Indicates the type of value we are monitoring (emission or energy). 
        /// </summary>
        public abstract MonitorCollectionType MonitorType { get; set; }

        #region IMonitor Members

        public abstract void Clear();

        public abstract Dictionary<int, IValue> Values { get; }
       
        public int ResultsCount
        {
            get { return this.Values.Count; }
        }

        /// <summary>
        /// A name that is unique to that monitored item
        /// </summary>
        public string UniqueId
        {
            get { return this.uniqueId; }
            set { this.uniqueId = value; }
        }
        #endregion

    }
}
