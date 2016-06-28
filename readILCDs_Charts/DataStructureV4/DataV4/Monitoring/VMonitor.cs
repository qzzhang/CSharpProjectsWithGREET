// *********************************************************************** 
//  COPYRIGHT NOTIFICATION 
// 
//  Email contact: greet@anl.gov 
//  Copyright (c) 2012, UChicago Argonne, LLC 
//  All Rights Reserved
//  
//  THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
//  LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
//  CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
//  NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 
//  
//  ************************************************************************ 
//  ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
//  IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
//  ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
//  ENERGY. 
//  ************************************************************************
//   
//  ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.Interfaces;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.DataV4.Monitoring
{
    /// <summary>
    /// This class represents a monitored value for vehicle results
    /// </summary>
    public class VMonitor : AMonitor
    {
        /// <summary>
        /// Specifies what type of unit the result has 
        /// </summary>
        [Serializable, Obfuscation(Feature = "renaming", Exclude = true)]
        public enum VehicleMonitorItemType { wtp, mode, wtw, total };

        #region Fields and Constants
        /// <summary>
        /// The mean value for this monitor; this value is only defined if the monitor is an alert
        /// </summary>
        private Parameter _mean;

        /// <summary>
        /// The tolerance value for this monitor; this value is only defined if the monitor is an alert
        /// If the monitored value differs from the mean by the tolerance percentage, an alert will be displayed
        /// </summary>
        private Parameter _tolerence;
        /// <summary>
        /// For each calculations, we store a value here, so we can trace how the monitored item evolved 
        /// </summary>
        private Dictionary<int, LightValue> _calculationResultsValues = new Dictionary<int, LightValue>();

        /// <summary>
        /// If we monitor different than total energy, the GasId or GasGroupId or ResourceId or ResourceGroupId is stored there
        /// </summary>
        private int _monitoredItemId;

        /// <summary>
        /// Holds the location of where exactly the data is coming from in the excel sheet.
        /// In addition stores the unit as it is in excel. 
        /// </summary>
        private MonitorExcelLocationData _excelLocationData = new MonitorExcelLocationData();

        public int VehicleId;

        public VMonitor()
        {

        }

        public VMonitor(GData data, XmlNode node)
        {
            if(node.Attributes["veh_id"] != null)
                this.VehicleId = Convert.ToInt32(node.Attributes["veh_id"].Value);
            if (node.Attributes["veh_res"] != null)
                VehicleResultType = (VehicleMonitorItemType)Enum.Parse(typeof(VehicleMonitorItemType), node.Attributes["veh_res"].Value, true);
            if (node.Attributes["mode_id"].NotNullNOrEmpty())
                ModeId = new Guid(node.Attributes["mode_id"].Value);

            MonitoredItemID = Convert.ToInt32(node.Attributes["item_id"].Value);
            ItemType = (ItemType)Enum.Parse(typeof(ItemType), node.Attributes["itype"].Value);
            MonitorType = (MonitorCollectionType)Enum.Parse(typeof(MonitorCollectionType), node.Attributes["mtype"].Value);

            if (node.SelectSingleNode("excel_location") != null)
                _excelLocationData = new MonitorExcelLocationData(node.SelectSingleNode("excel_location"));

            if (node.Attributes["mean"].NotNullNOrEmpty())
                this._mean = data.ParametersData.CreateRegisteredParameter(node.Attributes["mean"], "mnt_mean_" + VehicleId + MonitoredItemID + VehicleResultType + ItemType + MonitorType);
            
            if (node.Attributes["tolerence"].NotNullNOrEmpty())
                this._tolerence = data.ParametersData.CreateRegisteredParameter(node.Attributes["tolerence"], "mnt_tolerence_" + VehicleId + MonitoredItemID + VehicleResultType + ItemType + MonitorType);

            if (node.Attributes["uniqueid"] != null)
                this.uniqueId = node.Attributes["uniqueid"].Value;
            else
                this.uniqueId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties and Indexers

        /// <summary>
        /// For each calculations, we store a value here, so we can trace how the monitored item evolved 
        /// </summary>
        public override Dictionary<int, LightValue> CalculationResultsValues
        {
            get { return _calculationResultsValues; }
        }

        /// <summary>
        /// Stores the sheet name, cell and unit of the corresponding
        /// value found in the Greet Base Excel Sheet. 
        /// </summary>
        public override MonitorExcelLocationData ExcelLocationData
        {
            get { return _excelLocationData; }
            set { _excelLocationData = value; }
        }

        public override bool IsAlert { get; set; }
        public ItemType ItemType { get; set; }

        /// <summary>
        /// returns the index of the latest calculation stored in this monitor value
        /// </summary>
        public override int LatestCalculationRunIndex
        {
            get
            {
                if (_calculationResultsValues != null &&
                    _calculationResultsValues.Count > 0)
                {
                    return _calculationResultsValues.Keys.Max();
                }
                else
                {
                    return 0;
                }
            }
        }

        public override Parameter Mean
        {
            get { return _mean; }
            set { _mean = value; }
        }
        public bool Equals(VMonitor mnt)
        {
            if (this.VehicleId == mnt.VehicleId && this.VehicleResultType == mnt.VehicleResultType && this.MonitorType == mnt.MonitorType && this.ItemType == mnt.ItemType && this.MonitoredItemID == mnt.MonitoredItemID)
                return true;
            return false;
        }

        public Guid ModeId { get; set; }

        /// <summary>
        /// Indicates the type of value we are monitoring (emission or energy). 
        /// </summary>
        public override MonitorCollectionType MonitorType { get; set; }

        public MonitorReferenceType ReferenceType
        {
            get { return MonitorReferenceType.vehicle; }
        }

        /// <summary>
        /// Index of monitored item in results dictionary.
        /// </summary>
        public int MonitoredItemID
        {
            get { return _monitoredItemId; }
            set { _monitoredItemId = value; }
        }

        public override Parameter Tolerance {
            get { return _tolerence; }
            set { _tolerence = value; }
        }

        /// <summary>
        /// Results stored from previous simulations
        /// </summary>
        public override Dictionary<int, IValue> Values
        {
            get
            {
                Dictionary<int, IValue> results = new Dictionary<int, IValue>();
                foreach (KeyValuePair<int, LightValue> pair in this.CalculationResultsValues)
                {
                    ResultValue resVal = new ResultValue
                    {
                        UnitExpression = Units.QuantityList.ByDim(pair.Value.Dim).SiUnit.Expression,
                        Value = pair.Value.Value,
                        ValueSpecie = Enumerators.ResultType.resource,
                        SpecieId = pair.Key
                    };
                    results.Add(pair.Key, resVal);
                }
                return results;
            }
        }


        public VehicleMonitorItemType VehicleResultType { get; set; }

        #endregion

        #region Members

        /// <summary>
        /// Clears all values stored from previous simulations
        /// </summary>
        public override void Clear()
        {
            if (this._calculationResultsValues != null)
                this._calculationResultsValues.Clear();
        }

        /// <summary>
        /// Returns the current value for the lastest calculation run 
        /// </summary>
        /// <returns></returns>
        public override LightValue GetCurrentCalculationValue()
        {
            if (this._calculationResultsValues.Count > 0)
            {
                return this._calculationResultsValues[this._calculationResultsValues.Keys.Max()];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Outputs the data of the monitor value in XML format for saving
        /// </summary>
        /// <param name="doc">XML document being written too</param>
        /// <param name="exportingResults">A bool that if set true indicates that the calculation result values will be written to the save file.</param>
        /// <returns></returns>
        public override XmlNode ToXmlNode(XmlDocument doc, bool exportingResults = false)
        {
            XmlNode node = doc.CreateNode("vmonitor", doc.CreateAttr("veh_id", VehicleId), doc.CreateAttr("veh_res", VehicleResultType),
                doc.CreateAttr("mode_id", ModeId), doc.CreateAttr("mean", _mean), doc.CreateAttr("tolerence", _tolerence), doc.CreateAttr("itype", ItemType),
                doc.CreateAttr("mtype", MonitorType), doc.CreateAttr("uniqueid", this.uniqueId), doc.CreateAttr("item_id", MonitoredItemID));

            if (exportingResults && this._calculationResultsValues.Count > 0)
                doc.Attributes.Append(doc.CreateAttr("result_value",
                    this._calculationResultsValues.OrderBy(item => item.Key).Last().Value.Value));

            node.AppendChild(_excelLocationData.ToXmlNode(doc));

            return node;
        }

        #endregion
    }
}