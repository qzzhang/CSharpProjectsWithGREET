using Greet.DataStructureV4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Greet.ConvenienceLib;
using System.IO;
using System.Text.RegularExpressions;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// An input table object stores rows, columns and table input objects. The table input objects can be either
    /// DoubleValue or DoubleValueTS
    /// </summary>
    [Serializable]
    public class InputTable : IInputTable, IXmlObj, IHaveMetadata, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion
        #region attributes
        public string idName;
        public string help;
        /// <summary>
        /// tabId contains the ID to a tab in which this table needs to be represented
        /// If no tabs are used, this table will be placed in the first tab and this attribute will be left as -1
        /// </summary>
        private int _tabId = -1;


        private string notes;
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedBy = "";

        /// <summary>
        /// The list of column names for the table
        /// </summary>
        private List<InputTableColumn> columns = new List<InputTableColumn>();

        /// <summary>
        /// A dictionary of rows for the tables, the key is the row header the value is the list of doublevalues that populate the table
        /// </summary>
        private Dictionary<int, InputTableRow> rows = new Dictionary<int, InputTableRow>();

        #endregion
        #region constructors
        internal InputTable(GData data, XmlNode tableNode)
        {
            this.FromXmlNode(data, tableNode);
        }

        public InputTable()
        {
            this.idName = "";
            this.TabId = 1;
        }

        public InputTable(string name, int tabId)
        {
            this.idName = name;
            this._tabId = tabId;
        }

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }
        #endregion
        #region methods
        /// <summary>
        /// Clears the rows and columns, then populate them again using the column and row names provided as parameters
        /// Use the value given as parameters to populate the data for each row.
        /// </summary>
        /// <param name="columnNames">The new desired column names</param>
        /// <param name="rowNames">The new desired row names</param>
        /// <param name="values">Nested list of data objects, first level are the rows, second levels are the columns</param>
        public void SetGridData(List<string> columnNames, List<string> rowNames, List<List<object>> values)
        {
            InputTableRow.idCounter = 1;
            InputTableColumn.idCounter = 1;
            int maxCol = MaxColumnSize(values);
            Columns = new List<InputTableColumn>();
            Rows = new Dictionary<int, InputTableRow>();

            //Add column names
            if (columnNames != null) foreach (string col in columnNames)
                    Columns.Add(new InputTableColumn(col));
            else for (int i = 0; i < maxCol; i++)
                    Columns.Add(new InputTableColumn(""));
            //Add row names
            if (rowNames != null) for (int i = 0; i < rowNames.Count; i++)
                    Rows.Add(i + 1, new InputTableRow(rowNames[i]));
            else for (int i = 0; i < values.Count; i++)
                    Rows.Add(i + 1, new InputTableRow(""));

            int rowNum = 1;
            foreach (List<object> row in values)
            {
                int colNum = 1;
                foreach (object value in row)
                {
                    InputTableObject input = new InputTableObject();
                    if (value is Parameter || value is ParameterTS)
                        input.Value = value;

                    Rows[rowNum].Add(colNum, input);

                    colNum++;
                }
                rowNum++;
            }
        }
        int MaxColumnSize(List<List<object>> values)
        {
            int max = 0;
            foreach (List<object> row in values)
                if (row.Count > max)
                    max = row.Count;
            return max;
        }
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode inputNode = xmlDoc.CreateNode("input");
            if (this.Discarded)
            {
                inputNode.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                inputNode.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                inputNode.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                inputNode.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
            }

            inputNode.Attributes.Append(xmlDoc.CreateAttr("id", this.idName));
            inputNode.Attributes.Append(xmlDoc.CreateAttr("tabid", _tabId));
            inputNode.Attributes.Append(xmlDoc.CreateAttr("notes", this.notes));
            inputNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            inputNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));


            foreach (InputTableColumn col in Columns)
            {
                XmlNode colNode = xmlDoc.CreateNode("column", xmlDoc.CreateAttr("name", col.Name),
                    xmlDoc.CreateAttr("id", col.Id));

                foreach (InputTableRow row in Rows.Values)
                {
                    if (row.ContainsKey(col.Id))
                    {
                        XmlNode paramNode = xmlDoc.CreateNode("param", xmlDoc.CreateAttr("name", row.Name), xmlDoc.CreateAttr("id", row.Id));
                        colNode.AppendChild(paramNode);

                        if (row[col.Id].Value is Parameter)
                            paramNode.Attributes.Append(xmlDoc.CreateAttr("value", row[col.Id].Value));
                        else if (row[col.Id].Value is ParameterTS)
                        {
                            XmlNode valuesNode = (row[col.Id].Value as ParameterTS).ToXmlNode(xmlDoc, "values");
                            paramNode.AppendChild(valuesNode);
                        }

                        paramNode.Attributes.Append(xmlDoc.CreateAttr("notes", row[col.Id].Notes));
                        paramNode.Attributes.Append(xmlDoc.CreateAttr("help", row[col.Id].Help));

                    }
                }
                inputNode.AppendChild(colNode);
            }
            return inputNode;
        }
        private void FromXmlNode(GData data, XmlNode tableNode, string optionalParamPrefix)
        {
            if (tableNode.Attributes["discarded"] != null)
            {
                Discarded = Convert.ToBoolean(tableNode.Attributes["discarded"].Value);
                DiscardedOn = Convert.ToDateTime(tableNode.Attributes["discardedOn"].Value, GData.Nfi);
                DiscarededBy = tableNode.Attributes["discardedBy"].Value;
                DiscardedReason = tableNode.Attributes["discardedReason"].Value;
            }

            idName = tableNode.Attributes["id"].Value;
            _tabId = Convert.ToInt32(tableNode.Attributes["tabid"].Value);
            if (tableNode.Attributes["notes"] != null)
                this.notes = tableNode.Attributes["notes"].Value;
            if (tableNode.Attributes[xmlAttrModifiedOn] != null)
                this.ModifiedOn = tableNode.Attributes[xmlAttrModifiedOn].Value;
            if (tableNode.Attributes[xmlAttrModifiedBy] != null)
                this.ModifiedBy = tableNode.Attributes[xmlAttrModifiedBy].Value;

            foreach (XmlNode columnNode in tableNode.ChildNodes)
            {
                InputTableColumn col = new InputTableColumn(columnNode);
                Columns.Add(col);
                foreach (XmlNode rowNode in columnNode.ChildNodes)
                {
                    int id = Convert.ToInt32(rowNode.Attributes["id"].Value);
                    if (!Rows.ContainsKey(id))
                        Rows.Add(id, new InputTableRow(rowNode));

                    object temp = null;

                    if (rowNode.Attributes["value"] != null && String.IsNullOrEmpty(rowNode.Attributes["value"].Value) == false)
                        temp = data.ParametersData.CreateRegisteredParameter(rowNode.Attributes["value"], "table_" + idName + "_col_" + col.Id + "_row_" + id);
                    else if (rowNode.SelectSingleNode("values") != null)
                        temp = new ParameterTS(data, rowNode.SelectSingleNode("values"), "table_" + idName + "_col_" + col.Id + "_row_" + id);

                    InputTableObject input = new InputTableObject();
                    input.Value = temp;

                    if (rowNode.Attributes["notes"] != null)
                        input.Notes = rowNode.Attributes["notes"].Value;
                    if (rowNode.Attributes["help"] != null)
                        input.Help = rowNode.Attributes["help"].Value;

                    Rows[id].Add(col.Id, input);
                }
            }
        }
        public override string ToString()
        {
            return idName + " [ " + Columns.Count + "x" + Rows.Count + " ] ";
        }
        public void SetTableValuesFromText(GData data, string pastedText)
        {
            InputTableRow.idCounter = 1;
            InputTableColumn.idCounter = 1;

            StringReader reader = new StringReader(pastedText);
            string[] columnNames = reader.ReadLine().Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string col in columnNames)
                Columns.Add(new InputTableColumn(col));
            int rowNum = 1;
            while (reader.Peek() > -1)
            {
                string[] line = reader.ReadLine().Split("\t".ToCharArray());//, StringSplitOptions.RemoveEmptyEntries);
                Rows.Add(rowNum, new InputTableRow(line[0]));
                for (int i = 1; i < line.Length; i++)
                {
                    int j = 0;
                    foreach (char c in line[i])
                    {
                        if (c >= 46 && c <= 57 && c != 47)
                            j++;
                    }

                    string t = line[i];

                    string dvalue = t.Substring(0, j);
                    string preferedUnitExpression = t.Substring(j, t.Length - j).Trim();

                    InputTableObject input = new InputTableObject();

                    if (dvalue == "")
                        input.Value = line[i];
                    else
                        input.Value = data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, Convert.ToDouble(dvalue));

                    Rows[rowNum].Add(i, input);
                }
                rowNum++;
            }
        }
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }
        #endregion
        #region accessors
        /// <summary>
        /// Get or Set the ID of the tab to which this table is associated.
        /// </summary>
        public int TabId
        {
            get { return _tabId; }
            set { _tabId = value; }
        }
        private InputTableObject this[int colIndex, int rowIndex]
        {
            get
            {
                if (Rows.ContainsKey(rowIndex) && Rows[rowIndex].ContainsKey(colIndex))
                    return Rows[rowIndex][colIndex];
                else
                    return null;
            }
        }
        private InputTableObject this[string colName, string rowName]
        {
            get
            {

                foreach (InputTableRow row in Rows.Values)
                    if (row.Name == rowName)
                        foreach (InputTableColumn col in Columns)
                            if (col.Name == colName)
                                return row[col.Id];
                return null;
            }
        }
        private InputTableObject this[string colIndex, int rowIndex]
        {
            get
            {
                return this[ToolsDataStructure.ColumnLettersToInt(colIndex), rowIndex];
            }
        }
        internal InputTableObject this[string p]
        {
            get
            {
                string pattern = "([A-Za-z]*)([0-9]*)";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                Match m = r.Match(p);
                if (m.Success)
                {
                    return this[ToolsDataStructure.ColumnLettersToInt(m.Groups[1].Value), Convert.ToInt16(m.Groups[2].Value)];
                }
                else return null;
            }
        }
        public string Id
        {
            get { return this.idName; }
            set { this.idName = value; }
        }
        /// <summary>
        /// Takes ID name and simply removes the underscores and replaces them with spaces so it looks nice for GUI. 
        /// </summary>
        public string Name
        {
            get
            {
                string dispName = "";
                foreach (char c in this.idName.ToCharArray())
                {
                    if (c == '_')
                        dispName = dispName + " ";
                    else
                        dispName = dispName + c;
                }

                return dispName;
            }
            set { }
        }
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        /// <summary>
        /// The list of column names for the table
        /// </summary>
        public List<InputTableColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        /// <summary>
        /// A dictionary of rows for the tables, the key is the row header the value is the list of doublevalues that populate the table
        /// </summary>
        public Dictionary<int, InputTableRow> Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        #endregion
        #region IHaveMetadata Members

        public string ModifiedBy { get { return this.modifiedOn; } set { this.modifiedOn = value; } }

        public string ModifiedOn { get { return this.modifiedBy; } set { this.modifiedBy = value; } }

        #endregion

    }
}
