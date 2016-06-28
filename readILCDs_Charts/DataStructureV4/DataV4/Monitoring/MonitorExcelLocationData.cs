using System;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4
{
    /// <summary>
    /// This class holds the location information of 
    /// where the results of the monitor value can be 
    /// found in the Greet Excel Sheet. 
    /// </summary>
    [Serializable]
    public class MonitorExcelLocationData
    {
        #region attributes
        /// <summary>
        /// Stores the cell in which the data is located. 
        /// For example if data is located in cell A1. The value
        /// of this attribute would be "A1"
        /// </summary>
        private String cell;

        /// <summary>
        /// Stores the sheet name in which the cell is located.
        /// </summary>
        private String sheetName;

        /// <summary>
        /// The unit group used by the data inside of the excel cell
        /// we are looking at. We will eventually use this unit group 
        /// to construct a DoubleValue.
        /// </summary>
        private String unit;
        #endregion

        #region constructors
        /// <summary>
        /// Default constructor. Attributes are given generic default values. After using this constructor
        /// the attributes still will need to be properly initialized via the accessors. 
        /// </summary>
        public MonitorExcelLocationData()
        {
            this.cell = "";
            this.sheetName = "";
            this.unit = "";
        }

        /// <summary>
        /// Constructor that creates the object and properly initializes the attributes via the 
        /// methods parameters. Parameter names are self explanatory. 
        /// </summary>
        /// <param name="_cell"></param>
        /// <param name="_sheetNumber"></param>
        /// <param name="_unit"></param>
        public MonitorExcelLocationData(String _cell, String _sheetName, String _unit)
        {
            this.cell = _cell;
            this.sheetName = _sheetName;
            this.unit = _unit;
        }

        /// <summary>
        /// XML constructor. Uses the data of the xml node to initialize the object's attributes. 
        /// </summary>
        /// <param name="node"></param>
        public MonitorExcelLocationData(XmlNode node)
        {

            this.cell = node.Attributes["excel_cell"].Value;
            this.sheetName = node.Attributes["excel_sheet"].Value;
            this.unit = node.Attributes["excel_unit"].Value;

        }
        #endregion

        #region methods
        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("excel_location", doc.CreateAttr("excel_sheet", this.sheetName), doc.CreateAttr("excel_cell", this.cell), doc.CreateAttr("excel_unit", this.unit));

            return node;
        }
        #endregion

        #region accessors
        public String Cell
        {
            get { return this.cell; }
            set { this.cell = value; }
        }

        public String SheetName
        {
            get { return this.sheetName; }
            set { this.sheetName = value; }
        }

        public String Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }
        #endregion
    }
}
