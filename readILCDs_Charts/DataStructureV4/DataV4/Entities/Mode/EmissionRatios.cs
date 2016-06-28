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
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    internal class EmissionRatios : Dictionary<int, Parameter>
    {
        int fuel_ref;

        public EmissionRatios(GData data, XmlNode node, string optionalParamPrefix = "")
        {
            fuel_ref = Convert.ToInt32(node.Attributes["fuel_ref"].Value);
            int count = 0;
            foreach (XmlNode ratio in node.SelectNodes("gas_ratio"))
            {
                this.Add(Convert.ToInt32(ratio.Attributes["gas_ref"].Value), data.ParametersData.CreateRegisteredParameter(ratio.Attributes["ratio"], optionalParamPrefix + "emratio" + count));
                count++;
            }
        }
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("derived_fuel", xmlDoc.CreateAttr("fuel_ref", this.fuel_ref));
            foreach (KeyValuePair<int, Parameter> pair in this)
            {
                node.AppendChild(xmlDoc.CreateNode("gas_ratio", xmlDoc.CreateAttr("gas_ref", pair.Key), xmlDoc.CreateAttr("ratio", pair.Value)));
            }
            return node;
        }
    }
}
