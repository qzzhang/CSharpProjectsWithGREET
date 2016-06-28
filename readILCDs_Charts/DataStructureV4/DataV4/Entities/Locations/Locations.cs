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
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class Locations : Dictionary<int, LocationData>, IGDataDictionary<int, ILocation>
    {
        #region attributes
        /// <summary>
        /// Groups to categorise the Gases
        /// </summary>
        public Dictionary<int, Group> groups = new Dictionary<int, Group>();

        public bool loaded = false;
        public bool fullyLoaded = false;
        /// <summary>
        /// This will retain the IDs of the process read from the database. The purpose is that when we then save the file
        /// the order of writting XML nodes is guaranteed (List guarantee ordering, dictionary does not). Moreover this 
        /// allows us to add an extra feature which is to insert new pathway at random places in the collection. Thus when
        /// comparing the datafile on a revision control system, merging conflicts due to multiple inserts by many different people 
        /// is reduced
        /// </summary>
        List<int> _idReadFromXML = new List<int>();
        #endregion attributes

        #region constructors

        public Locations()
        {
        }

        #endregion constructors

        #region methods

        public bool ReadDB(GData data, XmlNode LocationsNode)
        {

            this.fullyLoaded = true;
            this.Clear();
            XmlNodeList locationNodes = LocationsNode.SelectNodes("location");
            foreach (XmlNode locati in locationNodes)
            {
                try
                {
                    LocationData ld = new LocationData(data, locati, "");
                    this.Add(ld.Id, ld);
                    _idReadFromXML.Add(ld.Id);
                }
                catch (Exception e)
                {
                    LogFile.Write("Error 57:" + e.Message);
                    this.fullyLoaded = false;
                }
            }
            XmlNodeList groups = LocationsNode.SelectNodes("groups/group");
            this.groups.Clear();
            foreach (XmlNode node in groups)
            {
                try
                {
                    Group matGroup = new Group(data, node);
                    this.groups.Add(matGroup.Id, matGroup);
                }
                catch (Exception e)
                {
                    this.fullyLoaded = false;
                    LogFile.Write("Error 49:" + e.Message);
                }
            }
            this.loaded = true;
            return this.loaded;

        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {


                XmlNode root = xmlDoc.CreateNode("locations");

                XmlNode nodeGroups = xmlDoc.CreateNode("groups");
                foreach (KeyValuePair<int, Group> group in this.groups)
                {
                    if (group.Value.Id != 0)
                        nodeGroups.AppendChild(group.Value.ToXmlNode(xmlDoc));
                }
                root.AppendChild(nodeGroups);

                #region randomizing order of newly inserted processes/IDs in the XML file
                //First we find try to look for new processes/IDs that needs to be inserted in the database
                List<int> additionalIds = new List<int>();
                foreach (int id in this.Keys)
                    if (!_idReadFromXML.Contains(id))
                        additionalIds.Add(id);

                Random rnd = new Random();
                foreach (int id in additionalIds)
                {
                    int index = rnd.Next(0, _idReadFromXML.Count);
                    _idReadFromXML.Insert(index, id);
                }
                #endregion

                foreach (int ldId in _idReadFromXML)
                {
                    try
                    {
                        if (this.ContainsKey(ldId))
                        {
                            if (ldId != 0)    //avoid saving the fake object to the xml file, fake object use fo undefined location
                                root.AppendChild(this[ldId].ToXmlNode(xmlDoc));
                        }
                    }
                    catch
                    {
                        LogFile.Write("Failed to save location id = " + ldId);
                    }
                }

                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 56:" + "Error saving file \r\n" + e.Message);
                return null;
            }
        }


        #endregion methods

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(ILocation value)
        {
            this.Add(value.Id, value as LocationData);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public ILocation ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as ILocation;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public ILocation CreateValue(IData data, int type = 0)
        {
            LocationData location = new LocationData(data as GData);
            return location as ILocation;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool KeyExists(int key)
        {
            return this.ContainsKey(key);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool DeleteValue(IData data, int key)
        {
            if (this.KeyExists(key))
            {
                ToolsDataStructure.RemoveAllParameters(data, this.ValueForKey(key));
                return this.Remove(key);
            }
            else
                return false;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IEnumerable<ILocation> AllValues
        {
            get { return this.Values as IEnumerable<ILocation>; }
        }

        #endregion

        public List<string> GetCurrentMembers(Group g)
        {
            List<string> list = new List<string>();
            List<int> idLookup = new List<int>();
            idLookup.Add(g.Id);
            foreach (Group gr in this.groups.Values.Where(item => item.IncludeInGroups.Contains(g.Id)))
                idLookup.Add(gr.Id);

            foreach (LocationData rd in this.Values)
                foreach (int id in idLookup)
                    if (rd.Memberships.Contains(id))
                        list.Add(rd.Name);
            return list;
        }
    }
   
}
