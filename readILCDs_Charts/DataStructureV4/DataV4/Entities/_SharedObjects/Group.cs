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
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class Group : IGroup
    {
        #region attributes

        /// <summary>
        /// The id of that group
        /// </summary>
        int id;
        /// <summary>
        /// The name of that group
        /// </summary>
        string name;
        List<int> includeInGroups;
        /// <summary>
        /// Notes are displayed in the description of the property grid
        /// </summary>
        private string notes = "";

        /// <summary>
        /// Defines if that group has to be displayed in the results, default is true for new groups
        /// we do not need to display some groups which are only used for gases classification
        /// </summary>
        private bool showInResults = true;

        #endregion attributes

        #region constructors

        public Group(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.includeInGroups = new List<int>();
        }

        public Group(IData data, XmlNode xmlNode)
        {
            this.FromXmlNode(data, xmlNode);
        }

        #endregion constructors

        #region accessors

        public List<int> IncludeInGroups
        {
            get { return includeInGroups; }
            set { includeInGroups = value; }
        }

        public override string ToString()
        {
            return this.name;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Notes are displayed in the description of the property grid
        /// </summary>
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        /// <summary>
        /// Boolean to indicate if the results should be shown for this group in results.
        /// </summary>
        public bool ShowInResults
        {
            get { return showInResults; }
            set { showInResults = value; }
        }

        #endregion accessors

        #region methods

        public void FromXmlNode(IData data, XmlNode xmlNode)
        {
            string status = "";
            this.includeInGroups = new List<int>();

            try
            {
                status = "reading id";
                this.id = Convert.ToInt32(xmlNode.Attributes["id"].Value);
                status = "reading name";
                if (xmlNode.Attributes["name"] != null)
                    this.name = xmlNode.Attributes["name"].Value;
                
                status = "readinf includes";
                if (xmlNode.Attributes["include_in"] != null)
                {
                    string[] split = xmlNode.Attributes["include_in"].Value.Split(',');
                    foreach (string group_reference in split)
                        this.includeInGroups.Add(Convert.ToInt32(group_reference));
                }
                status = "Reading showInResults";

                if (xmlNode.Attributes["showInResults"] != null)
                    this.ShowInResults = Convert.ToBoolean(xmlNode.Attributes["showInResults"].Value);


            }
            catch (Exception e)
            {
                LogFile.Write("Error 16:" + xmlNode.OwnerDocument.BaseURI + "\r\n" + xmlNode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("group",
                xmlDoc.CreateAttr("id", id),
                xmlDoc.CreateAttr("name", this.name),
                xmlDoc.CreateAttr("notes", this.Notes));
            if (!ShowInResults)
                node.Attributes.Append(xmlDoc.CreateAttr("showInResults", ShowInResults));

            if (includeInGroups.Count > 0)
                node.Attributes.Append(xmlDoc.CreateAttr("include_in", ListToString(includeInGroups)));

            return node;
        }

        private static string ListToString(object collection)
        {
            string str = "";
            if (collection is List<int>)
            {
                foreach (int num in collection as List<int>)
                {
                    str += num.ToString() + ',';
                }
                str = str.TrimEnd(",".ToCharArray());
            }
            else if (collection is List<string>)
            {
                foreach (string name in collection as List<string>)
                {
                    str += name.ToString() + ',';
                }
                str = str.TrimEnd(",".ToCharArray());
            }
            else if (collection is object[])
            {
                foreach (object name in collection as object[])
                {
                    str += name.ToString() + ',';
                }
                str = str.TrimEnd(",".ToCharArray());
            }

            return str;
        }

        #endregion methods

    }
}
