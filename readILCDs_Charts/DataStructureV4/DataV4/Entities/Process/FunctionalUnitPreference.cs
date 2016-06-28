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
using System.Xml;
using Greet.ConvenienceLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Holds the object used for functional unit preference in the WTP results
    /// The same object is used by processes/pathways/mixes
    /// </summary>
    [Serializable]
    public class FunctionalUnitPreference
    {
        /// <summary>
        /// Functional amount expressed in functional unit
        /// </summary>
        private double _amount = 1;
        /// <summary>
        /// Functional unit
        /// </summary>
        private string _preferredUnitExpression = "mmBtu";
        /// <summary>
        /// If enabled we are using it, otherwise we use the default global parameter
        /// </summary>
        public bool enabled = false;
        
        /// <summary>
        /// Prefered functional unit expression such as mmBtu or MJ or kg
        /// </summary>
        public String PreferredUnitExpression
        {
            get { return _preferredUnitExpression; }
            set { _preferredUnitExpression = value; }
        }

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public string notes;

        public FunctionalUnitPreference()
        { }

        public FunctionalUnitPreference(XmlNode node)
        {
            try
            {
                _preferredUnitExpression = node.Attributes["unit"].Value;
                _amount = Convert.ToDouble(node.Attributes["amount"].Value, GData.Nfi);
                if (node.Attributes["notes"] != null)
                    notes = node.Attributes["notes"].Value;
                this.enabled = Convert.ToBoolean(node.Attributes["enabled"].Value);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode unit_pref_node = xmlDoc.CreateNode("prefered_functional_unit", xmlDoc.CreateAttr("unit", this._preferredUnitExpression), xmlDoc.CreateAttr("amount", _amount), xmlDoc.CreateAttr("enabled", this.enabled));
            return unit_pref_node;
        }

        public override string ToString()
        {
            return _amount + " " + this.PreferredUnitExpression;
        }
    }
}