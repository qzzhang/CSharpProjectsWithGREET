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
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// The collection of resources in the fuel dataset
    /// </summary>
    [Serializable]
    public class Resources : Dictionary<int, ResourceData>, IGDataDictionary<int, IResource>
    {
        #region attributes
        
        private int maxGroupId;
        private Dictionary<int, Group> groups;
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

        #region ennumerators
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum UsagePurpose { stationary, transportation, intermediate };
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum PhysicalState { liquid, solid, gaseous, energy, item };

        #endregion ennumerators

        #region constructors

        public Resources()
        {
            this.groups = new Dictionary<int, Group>();
            this.maxGroupId = 0;
        }

        #endregion constructors

        #region serializer

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("groupCount", this.groups.Count);
            int i = 0;
            foreach (KeyValuePair<int, ResourceData> pair in this)
            {
                info.AddValue("key" + i, pair.Key);
                info.AddValue("value" + i, pair.Value, typeof(ResourceData));
                i++;
            }
            info.AddValue("loaded", this.loaded);
            info.AddValue("fullyLoaded", this.fullyLoaded);
        }

        #endregion

        #region methods

        public void UpdateFrequencies(Processes processes)
        {
            foreach (ResourceData rd in this.Values)
                rd.freq_count.Clear();
            foreach (StationaryProcess p in processes.Stationary().Values)
            {
                if (p.Group != null)
                {
                    if (p.Group != null)
                    {
                        foreach (Input inp in p.Group.Inputs)
                        {
                            if (this.ContainsKey(inp.resourceId))
                                this[inp.resourceId].CountSourceFreq(inp);
                            else
                                LogFile.Write("Can't update frequencies for Resource id " + inp.resourceId + ". It does not exist in Holder");
                        }
                    }
                }
                if (p.OtherInputs != null)
                {
                    foreach (Input inp in p.OtherInputs)
                        if (this.ContainsKey(inp.resourceId))
                            this[inp.resourceId].CountSourceFreq(inp);
                        else
                            LogFile.Write("Can't update frequencies for Resource id " + inp.resourceId + ". It does not exist in Holder");
                }
             }
        }

        /// <summary>
        /// Reads the XML file and create the data objects
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool ReadDB(GData data, XmlNode resourcesNodes)
        {
            XmlDocument fxml = new XmlDocument();

            this.groups.Clear();
            this.Clear();

            this.fullyLoaded = true;
            foreach (XmlNode node in resourcesNodes.SelectNodes("groups/group"))
            {
                try
                {
                    Group matGroup = new Group(data, node);
                    this.groups.Add(matGroup.Id, matGroup);
                    if (matGroup.Id > maxGroupId) { maxGroupId = matGroup.Id; }
                }
                catch (Exception e)
                {
                    this.fullyLoaded = false;
                    LogFile.Write("Error 63:" + e.Message);
                }
            }

            XmlNodeList materials = resourcesNodes.SelectNodes("resources/resource");
            this.Clear();
            foreach (XmlNode material in materials)
            {
                try
                {
                    ResourceData fd = new ResourceData(data, material);
                    this.Add(fd.Id, fd);
                    _idReadFromXML.Add(fd.Id);
                }
                catch (Exception e)
                {
                    this.fullyLoaded = false;
                    LogFile.Write("Error 62:" + e.Message);
                }
            }

            

            this.loaded = true;
            return this.loaded;

        }

        /// <summary>
        /// Put the data objects in an XML file
        /// </summary>
        /// <param name="fileInfo">the fileinfo to write the data</param>
        /// <returns></returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {

                XmlNode root = xmlDoc.CreateNode("resources");

                XmlNode nodeGroups = xmlDoc.CreateNode("groups");
                XmlNode nodeMaterials = xmlDoc.CreateNode("resources");
                foreach (KeyValuePair<int, Group> group in this.groups)
                {
                    if (group.Value.Id != 0)
                        nodeGroups.AppendChild(group.Value.ToXmlNode(xmlDoc));
                }

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

                foreach (int resId in _idReadFromXML)
                {
                    try
                    {
                        if(this.ContainsKey(resId))
                            nodeMaterials.AppendChild(this[resId].ToXmlNode(xmlDoc));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 6161:" + "Failing to save resource id=" + resId + " \r\n" + e.Message);
                    }
                }
                root.AppendChild(nodeGroups);
                root.AppendChild(nodeMaterials);

                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 61:" + "Error saving file \r\n" + e.Message);
                return null;
            }
        }

        public ResourceData MaterialByName(string name_looked)
        {
            return this.Values.Single(item => item.Name == name_looked);
        }

        #endregion methods

        #region accessors

        public int MaxGroupId
        {
            get { return maxGroupId; }
            set { maxGroupId = value; }
        }

        public Dictionary<int, Group> Groups
        {
            get { return groups; }
            set { groups = value; }
        }

        #endregion accessors

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IResource value)
        {
            this.Add(value.Id, value as ResourceData);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IResource ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as IResource;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IResource CreateValue(IData data, int type = 0)
        {
            ResourceData resource = new ResourceData(data as GData);
            return resource as IResource;
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
        public IEnumerable<IResource> AllValues
        {
            get { return this.Values as IEnumerable<IResource>; }
        }

        #endregion

        public List<string> GetCurrentMembers(Group g)
        {
            List<string> list = new List<string>();
            List<int> idLookup = new List<int>();
            idLookup.Add(g.Id);
            foreach (Group gr in this.Groups.Values.Where(item => item.IncludeInGroups.Contains(g.Id)))
                idLookup.Add(gr.Id);

            foreach (ResourceData rd in this.Values/*.Where(item => item.Memberships.Contains(g.Id))*/)
                foreach (int id in idLookup)
                    if (rd.Memberships.Contains(id))
                        list.Add(rd.Name);
            return list;
        }

       
    }
}
