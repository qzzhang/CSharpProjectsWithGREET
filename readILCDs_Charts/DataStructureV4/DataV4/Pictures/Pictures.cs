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
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4
{
    [Serializable]
    public class Pictures : Dictionary<string, Picture>, IGDataDictionary<string, IPicture>
    {

        #region constructors

        public Pictures() { }

        public Pictures(GData data, XmlNode picturesNode,string optionalParamPrefix)
        {
            this.FromXmlNode(data, picturesNode, optionalParamPrefix);
        }

        #endregion constructors

        #region Methods

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode rootNode = xmlDoc.CreateNode("pictures");

            foreach (Picture image in this.Values)
            {
                XmlNode picNode = image.ToXmlNode(xmlDoc);
                rootNode.AppendChild(picNode);
            }

            return rootNode;
        }

        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {

            foreach (XmlNode pictureNode in node.SelectNodes("picture"))
            {
                Picture image = new Picture(data, pictureNode, optionalParamPrefix);

                this.Add(image.Name.ToLower(), image);
            }
        }

        #endregion
        
        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IPicture value)
        {
            this.Add(value.Name.ToLower(), value as Picture);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IPicture ValueForKey(string key)
        {
            if (this.KeyExists(key))
                return this[key] as IPicture;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IPicture CreateValue(IData data, int type = 0)
        {
            Picture picture = new Picture();
            return picture as IPicture;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool KeyExists(string key)
        {
            return this.ContainsKey(key);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool DeleteValue(IData data, string key)
        {
            if (this.KeyExists(key))
            {
                ToolsDataStructure.RemoveAllParameters(data, this.ValueForKey(key), 2);
                return this.Remove(key);
            }
            else
                return false;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IEnumerable<IPicture> AllValues
        {
            get { return this.Values as IEnumerable<IPicture>; }
        }

        #endregion

    }
}