using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using Greet.UnitLib3;
using System.Reflection;
using Greet.DataStructureV4.Entities;


namespace Greet.DataStructureV4
{
    /// <summary>
    /// Monitor class stores information to track the changes of a value. A monitored item could be a emission or an energy result of a process, pathway, mix, technology, etc.
    /// This class defines how to track this object, and sets alarms in case of a values get out the defined Min and Max boundaries.
    /// </summary>
    [Serializable]
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public class Monitor : AMonitor
    {
        #region attributes
        /// <summary>
        /// If we monitor different than total energy, the GasId or GasGroupId or ResourceId or ResourceGroupId is stored there
        /// </summary>
        private int resultArrayindex;

        /// <summary>
        /// The process ID of the monitor value if applicable
        /// </summary>
        private Guid processVertexId;

        /// <summary>
        /// The pathway ID of the monitor value
        /// </summary>
        private int pathwayId;

        /// <summary>
        /// The mix ID of the monitor value
        /// </summary>
        private int mixId;

        /// <summary>
        /// The material ID corresponding to the output selected for the particular entity(Pathway, Process, Mix) when multiple outputs are available
        /// </summary>
        private Guid outputId;

        /// <summary>
        /// The mean value for this monitor; this value is only defined if the monitor is an alert
        /// </summary>
        private Parameter mean;

        /// <summary>
        /// The tolerance value for this monitor; this value is only defined if the monitor is an alert
        /// If the monitored value differs from the mean by the tolerance percentage, an alert will be displayed
        /// </summary>
        private Parameter tolerence;

        /// <summary>
        /// A string used to identify the item being monitored in the GUI
        /// </summary>
        private string header;

        /// <summary>
        /// Many results are displayed in categories : Independent items ( CO2, NOx ... ) ; Groups (greehouse gases,... )
        /// This variable holds where we are looking at
        /// </summary>
        private ItemType itemType;

        /// <summary>
        /// Monitor type is equal to emission if we are watching an emission item, and energy if we are watching an energy item
        /// </summary>
        private MonitorCollectionType monitorType;

        /// <summary>
        /// For each calculations, we store a value here, so we can trace how the monitored item evolved 
        /// </summary>
        private Dictionary<int, LightValue> calculationResultsValues = new Dictionary<int, LightValue>();

        /// <summary>
        /// Holds the location of where exactly the data is coming from in the excel sheet.
        /// In addition stores the unit as it is in excel. 
        /// </summary>
        private MonitorExcelLocationData excelLocationData = new MonitorExcelLocationData();

        #endregion

        #region constructors

        /// <summary>
        /// Creates an instance of a Monitor with mean set to 0 and torerence to 0.5%
        /// This is the constructor that should be used from the GUI to build a new Monitor instance
        /// </summary>
        /// <param name="data">The database to which this monitor is going to be applied</param>
        /// <param name="meanUnit">The unit for the mean value, is it kilograms? joules?</param>
        public Monitor()
        {
            processVertexId = Guid.Empty;
            pathwayId = -1;
            outputId = new Guid();
            mixId = -1;
            monitorType = MonitorCollectionType.emission;
            uniqueId = Guid.NewGuid().ToString();
            resultArrayindex = -1;
        }

        /// <summary>
        /// Creates a new Monitor instance from an XML node read from the project file.
        /// </summary>
        /// <param name="node"></param>
        public Monitor(GData data, XmlNode node)
        {
            if (node.Attributes["path_id"] != null)
                this.pathwayId = Convert.ToInt32(node.Attributes["path_id"].Value);
            if (node.Attributes["vtex_id"] != null)
                this.processVertexId = Guid.Parse(node.Attributes["vtex_id"].Value);
            if (node.Attributes["output_id"].NotNullNOrEmpty())
                this.outputId = new Guid(node.Attributes["output_id"].Value);
            if (node.Attributes["mix_id"] != null)
                this.mixId = Convert.ToInt32(node.Attributes["mix_id"].Value);

            this.resultArrayindex = Convert.ToInt32(node.Attributes["item_id"].Value);
            this.itemType = (ItemType)Enum.Parse(typeof(ItemType), node.Attributes["itype"].Value);
            this.monitorType = (MonitorCollectionType)Enum.Parse(typeof(MonitorCollectionType), node.Attributes["mtype"].Value);

            if (node.SelectSingleNode("excel_location") != null)
                this.excelLocationData = new MonitorExcelLocationData(node.SelectSingleNode("excel_location"));

            if (node.Attributes["mean"].NotNullNOrEmpty())
            {
                this.mean = data.ParametersData.CreateRegisteredParameter(node.Attributes["mean"], "mnt_mean_" + mixId + resultArrayindex + pathwayId + processVertexId + outputId + itemType + monitorType);
            }
            if (node.Attributes["tolerence"].NotNullNOrEmpty())
                this.tolerence = data.ParametersData.CreateRegisteredParameter(node.Attributes["tolerence"], "mnt_tolerence_" + mixId + resultArrayindex + +pathwayId + processVertexId + outputId + itemType + monitorType);

            if (node.Attributes["uniqueid"] != null)
                this.uniqueId = node.Attributes["uniqueid"].Value;
            else
                this.uniqueId = Guid.NewGuid().ToString();
        }

        #endregion

        #region methods
        /// <summary>
        /// Outputs the data of the monitor value in XML format for saving
        /// </summary>
        /// <param name="doc">XML document being written too</param>
        /// <param name="exportingResults">A bool that if set true indicates that the calculation result values will be written to the save file.</param>
        /// <returns></returns>
        public override XmlNode ToXmlNode(XmlDocument doc, bool exportingResults = false)
        {
            XmlNode node = doc.CreateNode("monitor", doc.CreateAttr("path_id", pathwayId),
                doc.CreateAttr("output_id", outputId), doc.CreateAttr("mix_id", mixId), doc.CreateAttr("item_id", resultArrayindex),
                doc.CreateAttr("mean", mean), doc.CreateAttr("tolerence", tolerence), doc.CreateAttr("itype", itemType),
                doc.CreateAttr("mtype", monitorType), doc.CreateAttr("vtex_id", this.processVertexId),
                doc.CreateAttr("uniqueid", this.uniqueId));

            if (exportingResults && this.calculationResultsValues.Count > 0)
                doc.Attributes.Append(doc.CreateAttr("result_value", this.calculationResultsValues.OrderBy(item => item.Key).Last().Value.Value));

            node.AppendChild(excelLocationData.ToXmlNode(doc));

            return node;
        }

        public override LightValue GetCurrentCalculationValue()
        {
            if (this.calculationResultsValues.Count > 0)
            {
                return this.calculationResultsValues[this.calculationResultsValues.Keys.Max()];
            }
            else
            {
                return null;
            }
        }

        public override int LatestCalculationRunIndex
        {
            get
            {
                if (this.calculationResultsValues.Count > 0)
                {
                    return this.CalculationResultsValues.Keys.Max();
                }
                else
                {
                    return 0;
                }
            }
        }

        public string ToString(GData data)
        {
            try
            {
                string returnText = "";
                switch (monitorType)
                {
                    case MonitorCollectionType.emission:
                        returnText += "Emission";
                        switch (itemType)
                        {
                            case ItemType.Base:
                                returnText += "(" + data.GasesData[resultArrayindex].Name + ")"; break;
                            case ItemType.Groups:
                                returnText += "(" + data.GasesData.Groups[resultArrayindex].Name + ")"; break;
                            case ItemType.Total:
                                returnText += "(Total)"; break;
                        }
                        break;
                    case MonitorCollectionType.energy:
                        returnText += "Energy";
                        switch (itemType)
                        {
                            case ItemType.Base:
                                returnText += "(" + data.ResourcesData[resultArrayindex].Name + ")"; break;
                            case ItemType.Groups:
                                returnText += "(" + data.ResourcesData.Groups[resultArrayindex].Name + ")"; break;
                            case ItemType.Total:
                                returnText += "(Total)"; break;
                        }
                        break;
                }

                return returnText;
            }
            catch (Exception e)
            {//writes a message to the log file, we never have a catch method which does not output anything
                LogFile.Write(e.Message);
                return base.ToString();
            }
        }

        /// <summary>
        /// Creates a registered parameter for the mean value of a monitored item. This is used in cases
        /// where the monitored item was created without a mean value or when we create one with the set an alarm option
        /// </summary>
        /// <param name="data">The dataset containing all parameters</param>
        /// <param name="preferedUnitExpression">The prefered expression for visual representation</param>
        /// <param name="value">The value for the parameter</param>
        public void CreateMeanParameter(GData data, string preferedUnitExpression, double value)
        {
            this.mean = data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, value);
        }

        public void CreateToleranceParameter(GData data, string unit, double value)
        {
            this.tolerence = data.ParametersData.CreateRegisteredParameter(unit, value);
        }
        public override void Clear()
        {
            if (this.calculationResultsValues != null)
                this.calculationResultsValues.Clear();
        }

        public override Dictionary<int, IValue> Values
        {
            get
            {
                Dictionary<int, IValue> results = new Dictionary<int, IValue>();
                foreach (KeyValuePair<int, LightValue> pair in this.CalculationResultsValues)
                {
                    ResultValue resVal = new ResultValue();
                    resVal.UnitExpression = Units.QuantityList.ByDim(pair.Value.Dim).SiUnit.Expression;
                    resVal.Value = pair.Value.Value;
                    resVal.ValueSpecie = Greet.DataStructureV4.Interfaces.Enumerators.ResultType.resource;
                    resVal.SpecieId = pair.Key;
                    results.Add(pair.Key, resVal);
                }
                return results;
            }
        }

        public bool Equals(Monitor mnt)
        {
            if (this.ProcessVertexId == mnt.processVertexId && this.PathwayId == mnt.PathwayId && this.OutputId == mnt.OutputId && this.MixId == mnt.MixId && this.MonitorType == mnt.MonitorType && this.ItemType == mnt.ItemType && this.resultArrayindex == mnt.resultArrayindex)
                return true;
            return false;
        }
        #endregion

        #region accessors

        /// <summary>
        /// Index of monitored item in results dictionary.
        /// </summary>
        public int ResultArrayindex
        {
            get { return resultArrayindex; }
            set { resultArrayindex = value; }
        }

        /// <summary>
        /// The material ID corresponding to the output selected for the particular entity(Pathway, Process, Mix) when multiple outputs are available
        /// </summary>
        public Guid OutputId
        {
            get { return outputId; }
            set { outputId = value; }
        }

        /// <summary>
        /// <para>If the monitored item is used to monitor the output of a Process within a pathway</para>
        /// <para>Then this value represents the Process ID for which the output is monitored, -1 otherwise</para>
        /// </summary>
        public Guid ProcessVertexId
        {
            get { return processVertexId; }
            set { processVertexId = value; }
        }

        /// <summary>
        /// <para>If the monitored item is used to monitor the output of a Pathway, or when used to monitor a ProcessReference within a Pathway</para>
        /// <para>Then this value represents the Pathway ID for which the output is monitored, -1 otherwise</para>
        /// </summary>
        public int PathwayId
        {
            get { return pathwayId; }
            set { pathwayId = value; }
        }

        /// <summary>
        /// <para>If the monitored item is used to monitor the output of a Mix</para>
        /// <para>Then this value represents the Mix ID for which the output is monitored, -1 otherwise</para>
        /// </summary>
        public int MixId
        {
            get { return mixId; }
            set { mixId = value; }
        }

        /// <summary>
        /// The material ID corresponding to the Main Output of the monitored entity(Pathway, Process, Mix)
        /// </summary>
        public int ResourceId(GData data)
        {
            if (mixId != -1)
            {
                return data.MixesData[this.mixId].MainOutputResourceID;
            }
            else if (ProcessVertexId != Guid.Empty)
            {
                return data.ProcessesData[data.PathwaysData[this.PathwayId].CanonicalProcesses[this.processVertexId].ModelId].FlattenAllocatedOutputList.Single(outp => outp.Id == this.OutputId).ResourceId;
            }
            else
                return data.PathwaysData[this.PathwayId].MainOutputResourceID;
            
        }


        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        /// <summary>
        /// Defines which type of Item is being monitored
        /// </summary>
        public ItemType ItemType
        {
            get { return itemType; }
            set { itemType = value; }
        }

        /// <summary>
        /// <para>Results for each iterations of the calculations</para>
        /// <para>Keys are the simulation number, values are the normalized results in default unit wihtout being adapted for a specific functional unit</para>
        /// </summary>
        public override Dictionary<int, LightValue> CalculationResultsValues
        {
            get { return this.calculationResultsValues; }
        }

        /// <summary>
        /// <para>Used if the user wants to speicify a location in a spreadsheet, was used for automated testing but has not other uses than that</para>
        /// </summary>
        public override MonitorExcelLocationData ExcelLocationData
        {
            get { return this.excelLocationData; }
            set { this.excelLocationData = value; }
        }

        /// <summary>
        /// If True an alert is going to be thrown if the monitored value is too high or too low comparing to the defined average value and tolerence
        /// </summary>
        public override bool IsAlert
        {
            get { return mean != null && tolerence != null; }
            set { }
        }

        /// <summary>
        /// Mean value used to calculate weather or not an alert should be thrown
        /// </summary>
        public override Parameter Mean
        {
            get { return this.mean; }
            set { this.mean = value; }
        }

        /// <summary>
        /// Tolerence to calculate the min and max boundaries for checking if the current value is within acceptable limits
        /// </summary>
        public override Parameter Tolerance
        {
            get { return this.tolerence; }
            set { this.tolerence = value; }
        }

        /// <summary>
        /// Specifies if the monitored item is an emission or a resource
        /// </summary>
        public override MonitorCollectionType MonitorType
        {
            get { return this.monitorType; }
            set { this.monitorType = value; }
        }

        public String FunctionalUnitExpression { get; set; }
        public String PreferedFunctionalUnitExpression { get; set; }
        public double PreferedFunctionalAmount { get; set; }

        /// <summary>
        /// Returns the state of the monitor item or which type of output is being monitored
        /// </summary>
        /// <returns>MonitorReferenceType detailling the current ReferenceType for this Monitor instance</returns>
        public MonitorReferenceType ReferenceType
        {
            get
            {
                if (this.pathwayId != -1 && this.mixId == -1)
                {
                    if (this.processVertexId != Guid.Empty)
                        return MonitorReferenceType.process_reference;
                    else
                        return MonitorReferenceType.pathway;
                }
                else if (this.mixId != -1 && this.pathwayId == -1)
                {
                    return MonitorReferenceType.mix;
                }
                else
                    throw new Exception("Inconsistent state, mixID and pathwayID cannot be defined at the same time");
            }
        }
        #endregion
    }
}
