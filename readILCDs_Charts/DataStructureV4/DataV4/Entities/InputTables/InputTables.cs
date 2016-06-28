/*********************************************************************** 
COPYRIGHT NOTIFICATION 

Email contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 

************************************************************************ 
ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
ENERGY. 
************************************************************************
 
***********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    ///<summary>
    ///This class stores the input tables and tabs organization for the tables
    ///The tables are used for storing values that can be refered from any other places, or use in ExcelLike calculations
    ///</summary>
    [Serializable]
    public class InputTables
    {
        #region private attributes
        /// <summary>
        /// Collection of the tabs objects, tabs are used to categorize input tables
        /// </summary>
        private Dictionary<int, InputTableTab> _inputTabs;
        /// <summary>
        /// Collection of input tables, that can be used for referring values in other places
        /// </summary>
        private InputTablesDictionary _inputTables;

        
        /// <summary>
        /// Returns true if the data was patially or fully loaded
        /// </summary>
        private bool _loaded = false;
        /// <summary>
        /// Returns false if the data is not loaded or only partially loaded
        /// </summary>
        private bool _fullyLoaded = false;

        #endregion attributes
        #region constructors
        /// <summary>
        /// Default constructuor, initialize empty dictionaries for the tabs and tables collections
        /// </summary>
        public InputTables()
        {
            this._inputTables = new InputTablesDictionary();
            this._inputTabs = new Dictionary<int, InputTableTab>();
        }
        #endregion constructors
        #region public accessors
        /// <summary>
        /// Returns true if this data was successfully loaded from the XML document
        /// if not loaded or only partially loaded, this will return false
        /// </summary>
        public bool FullyLoaded
        {
            get { return _fullyLoaded; }
            set { _fullyLoaded = value; }
        }
        /// <summary>
        /// The tables are organized by tabs. So in order to access a table object directly we need to know
        /// in which tab the table is located.
        /// </summary>
        public Dictionary<int, InputTableTab> Tabs
        {
            get { return _inputTabs; }
            set { _inputTabs = value; }
        }
        /// <summary>
        /// Collection of input tables.
        /// </summary>
        public InputTablesDictionary Tables
        {
            get { return _inputTables; }
            set { _inputTables = value; }
        }
        #endregion
        #region methods
        /// <summary>
        /// Reads the data from an Input xmlNode
        /// </summary>
        /// <param name="inputsNode">The input xml node to parse and read data from</param>
        /// <returns>Return True if the file is successfully loaded, false if some error happened</returns>
        public bool ReadDB(GData data, XmlNode inputsNode)
        {

            this.Clear();
            string status = "";
            this._fullyLoaded = true;
            try
            {
                //reading tabs
                _inputTabs.Clear();
                foreach (XmlNode table_tab in inputsNode.SelectNodes("tab"))
                {
                    try
                    {
                        InputTableTab tab = new InputTableTab(table_tab);
                        _inputTabs.Add(tab.Id, tab);
                    }
                    catch (Exception e2)
                    {
                        this._fullyLoaded = false;
                        LogFile.Write("Error 5253 :\r\n" + e2.Message + "\r\n" + status + "\r\n");
                        //ToolsData.Messaging.Print("Exception occured when reading the Inputs" + e2.Message);
                    }
                }

                //reading tables
                foreach (XmlNode inputTable in inputsNode.SelectNodes("input"))
                {
                    try
                    {
                        InputTable input_table = new InputTable(data, inputTable);
                        _inputTables.Add(input_table.idName, input_table);
                    }
                    catch (Exception e1)
                    {
                        this._fullyLoaded = false;
                        LogFile.Write("Error 5252 :\r\n" + e1.Message + "\r\n" + status + "\r\n");
                        //ToolsData.Messaging.Print("Exception occured when reading the Inputs\r\nStatus:\r\n" + e1.Message);
                    }
                }

                _loaded = true;
                _fullyLoaded = true;

                return _loaded;
            }
            catch (Exception e)
            {
                this._fullyLoaded = false;
                LogFile.Write("Error 52:" + "\r\n" +
                        e.Message + "\r\n" + status + "\r\n");
                //ToolsData.Messaging.Print("Exception occured when reading the Inputs \r\n" + "\r\nStatus:\r\n" + status + "\r\nError:\r\n" + e.Message);
            }
            return false;

        }
        /// <summary>
        /// Saves the data of this object as an XML node
        /// </summary>
        /// <param name="xmlDoc">The XMLDocument object necessary for namespace URI</param>
        /// <returns>The XMLNode representation for this object</returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {

                XmlNode root = xmlDoc.CreateNode("inputs");

                foreach (var table in _inputTabs.Values)
                    root.AppendChild(table.ToXmlNode(xmlDoc));

                foreach (InputTable tab in _inputTables.Values)
                    root.AppendChild(tab.ToXmlNode(xmlDoc));

                return root;

            }
            catch (Exception e)
            {
                LogFile.Write("Error 51:" + "Error saving file \r\n" + e.Message);
                return null;
            }
        }
        /// <summary>
        /// Get a value from the table inputs. Returns a table input object that can store either a 
        /// DoubleValue object or a DoubleValueTS object
        /// </summary>
        /// <param name="valueReference">The value that needs to be retrieved, format used is [tableName ! Col-Letter Row-Number]</param>
        /// <returns>Returns an input table object, the Value attribute of that input table object will contain either a DoubleValue or a DoubleValueTS</returns>
        public InputTableObject GetValue(string valueReference)
        {
            string[] split = valueReference.Split("!".ToCharArray());
            if (split.Length == 1)
                return null;
            else
            {
                string table_name = split[0].Trim("[]".ToCharArray());
                if (this._inputTables.ContainsKey(table_name))
                {
                    InputTable table = this._inputTables[split[0].Trim("[]".ToCharArray())];
                    InputTableObject obj = table[split[1].Trim("[]".ToCharArray())];
                    if (obj != null)
                        return obj;
                    else
                        return null;
                }
                return null;
            }
        }

        /// <summary>
        /// Clears the collection of input tables objects and input tabs objects
        /// </summary>
        internal void Clear()
        {
            _inputTables.Clear();
            _inputTabs.Clear();
        }
        #endregion methods
    }
}
