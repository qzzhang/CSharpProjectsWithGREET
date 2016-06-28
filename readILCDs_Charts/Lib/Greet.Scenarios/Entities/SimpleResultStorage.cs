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
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;

namespace Greet.Lib.Scenarios.Entities
{
    public class SimpleResultStorage
    {
        #region Fields and Constants

        public uint BottomDim;
        public FunctionalUnitPreference CustomFunctionalUnit;
        public EmissionAmounts FinalEm = new EmissionAmounts();
        public EmissionAmounts FinalEmUr = new EmissionAmounts();
        public ResourceAmounts FinalRe = new ResourceAmounts();
        public string SHA256ScenarioState = "";

        #endregion

        #region Constructors

        public SimpleResultStorage()
        {

        }

        public SimpleResultStorage(Enem finalEnem, EmissionAmounts urban, uint bottomDim, FunctionalUnitPreference customFunctionalUnit, string SHA256State)
        {
            FinalRe = finalEnem.materialsAmounts;
            FinalEm = finalEnem.emissions;
            FinalEmUr = urban;
            BottomDim = bottomDim;
            CustomFunctionalUnit = customFunctionalUnit;
            SHA256ScenarioState = SHA256State;
        }

        #endregion

        #region Members

        internal void FromXmlNode(XmlNode node)
        {
            if (node.Attributes["denominator"] != null)
                BottomDim = Convert.ToUInt32(node.Attributes["denominator"].Value);
            if (node.Attributes["scenarioHash"] != null)
                SHA256ScenarioState = node.Attributes["scenarioHash"].Value;

            foreach (XmlNode r in node.SelectNodes("resources/r"))
            {
                int id = Convert.ToInt32(r.Attributes["i"].Value);
                double value = Convert.ToDouble(r.Attributes["v"].Value);
                uint unit = Convert.ToUInt32(r.Attributes["u"].Value);

                if (FinalRe.ContainsKey(id))
                    FinalRe[id] = new LightValue(value, unit);
                else
                    FinalRe.Add(id, new LightValue(value, unit));
            }

            foreach (XmlNode r in node.SelectNodes("emissions/e"))
            {
                int id = Convert.ToInt32(r.Attributes["i"].Value);
                double value = Convert.ToDouble(r.Attributes["v"].Value);

                if (FinalEm.ContainsKey(id))
                    FinalEm[id] = value;
                else
                    FinalEm.Add(id, value);
            }

            foreach (XmlNode r in node.SelectNodes("urbanemissions/e"))
            {
                int id = Convert.ToInt32(r.Attributes["i"].Value);
                double value = Convert.ToDouble(r.Attributes["v"].Value);

                if (FinalEmUr.ContainsKey(id))
                    FinalEmUr[id] = value;
                else
                    FinalEmUr.Add(id, value);
            }

        }

        internal XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode root = xmlDoc.CreateNode("results", xmlDoc.CreateAttr("denominator", BottomDim), xmlDoc.CreateAttr("scenarioHash", SHA256ScenarioState));
            if (CustomFunctionalUnit != null)
                root.AppendChild(CustomFunctionalUnit.ToXmlNode(xmlDoc));

            XmlNode finalReNode = xmlDoc.CreateNode("resources");
            root.AppendChild(finalReNode);
            foreach (KeyValuePair<int, LightValue> pair in FinalRe)
            {
                XmlNode gas = xmlDoc.CreateNode("r", xmlDoc.CreateAttr("i", pair.Key), xmlDoc.CreateAttr("v", pair.Value.Value), xmlDoc.CreateAttr("u", pair.Value.Dim));
                finalReNode.AppendChild(gas);
            }

            XmlNode finalEmNode = xmlDoc.CreateNode("emissions");
            root.AppendChild(finalEmNode);
            foreach (KeyValuePair<int, double> pair in FinalEm)
            {
                XmlNode gas = xmlDoc.CreateNode("e", xmlDoc.CreateAttr("i", pair.Key), xmlDoc.CreateAttr("v", pair.Value));
                finalEmNode.AppendChild(gas);
            }

            XmlNode finalEmUrNode = xmlDoc.CreateNode("urbanemissions");
            root.AppendChild(finalEmUrNode);
            foreach (KeyValuePair<int, double> pair in FinalEmUr)
            {
                XmlNode gas = xmlDoc.CreateNode("e", xmlDoc.CreateAttr("i", pair.Key), xmlDoc.CreateAttr("v", pair.Value));
                finalEmUrNode.AppendChild(gas);
            }

            return root;
        }

        #endregion
    }
}
