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
using System.Xml;
using Greet.ConvenienceLib;
using Greet.Lib.Scenarios.Entities;

namespace Greet.Lib.Scenarios
{
    public class RecordedEntityResults
    {
        #region Fields and Constants

        private List<double> _amounts = new List<double>();
        string _exportFunctionalUnit = "";
        int _id = -1;
        Dictionary<Guid, SimpleResultStorage> _results = new Dictionary<Guid, SimpleResultStorage>();

        /// <summary>
        /// p=pathway, m=mix, v=vehicle
        /// </summary>
        char _type = 'p';

        #endregion

        #region Constructors

        public RecordedEntityResults()
        { }


        public RecordedEntityResults(char p, int itemId, string exportFU = "")
        {
            _type = p;
            _id = itemId;
            _exportFunctionalUnit = exportFU;
            Amounts = new List<double>();
        }

        public RecordedEntityResults(char p, int itemId, List<double> amts, string exportFU = "")
        {
            _type = p;
            _id = itemId;
            _exportFunctionalUnit = exportFU;
            Amounts = amts;
        }

        #endregion

        #region Properties and Indexers

        public List<double> Amounts
        {
            get { return _amounts; }
            set { _amounts = value; }
        }

        public string ExportFunctionalUnit
        {
            get { return _exportFunctionalUnit; }
            set { _exportFunctionalUnit = value; }
        }


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Dictionary<Guid, SimpleResultStorage> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        /// <summary>
        /// p=pathway, m=mix, v=vehicle
        /// </summary>
        public char Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion

        #region Members

        internal void FromXmlNode(XmlNode node, ScenariosData scenarios)
        {
            if (node.Attributes["id"] != null)
                _id = Convert.ToInt32(node.Attributes["id"].Value);
            else
                return;

            if (node.Attributes["type"] != null)
                _type = Convert.ToChar(node.Attributes["type"].Value);
            else
                return;

            if (node.Attributes["functionalUnit"] != null)
                _exportFunctionalUnit = node.Attributes["functionalUnit"].Value;

            foreach (XmlNode sNode in node.SelectNodes("scenario"))
            {
                string sName = "";
                if (sNode.Attributes["name"] != null)
                    sName = sNode.Attributes["name"].Value;
                else
                    continue;

                if (!String.IsNullOrEmpty(sName))
                {
                    Scenario sSenario = scenarios.Scenarios.SingleOrDefault(item => item.Name == sName);
                    if (sSenario != null)
                    {
                        Guid sid = sSenario.Id;
                        SimpleResultStorage scenarioResults = new SimpleResultStorage();
                        scenarioResults.FromXmlNode(sNode.SelectSingleNode("results"));

                        if (Results.ContainsKey(sid))
                            _results[sid] = scenarioResults;
                        else
                            _results.Add(sid, scenarioResults);
                    }
                }
            }
        }

        public override string ToString()
        {
            string name = _type.ToString();
            if (_type == 'p')
                name = "Pathway";
            else if (_type == 'm')
                name = "Mix";
            else if (_type == 'v')
                name = "Vehicle";

            return name + " - " + _id;
        }

        internal XmlNode ToXmlNode(XmlDocument xmlDoc, ScenariosData scenariosData)
        {
            XmlNode resNode = xmlDoc.CreateNode("recorded", xmlDoc.CreateAttr("id", _id), xmlDoc.CreateAttr("type", _type), xmlDoc.CreateAttr("functionalUnit", _exportFunctionalUnit));
            foreach (KeyValuePair<Guid, SimpleResultStorage> pair in _results)
            {
                Guid scenarioID = pair.Key;
                Scenario scenario = scenariosData.Scenarios.SingleOrDefault(item => item.Id == scenarioID);
                if (scenario != null && pair.Value != null)
                {
                    string scenarioName = scenario.Name;
                    SimpleResultStorage scenarioResults = pair.Value;

                    XmlNode scenarioResultNode = xmlDoc.CreateNode("scenario", xmlDoc.CreateAttr("name", scenarioName));
                    resNode.AppendChild(scenarioResultNode);

                    XmlNode resultsNode = scenarioResults.ToXmlNode(xmlDoc);
                    scenarioResultNode.AppendChild(resultsNode);
                }
            }
            return resNode;
        }

        #endregion
    }
}
