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
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Pathways : Dictionary<int, Pathway>, IGDataDictionary<int, IPathway>
    {
        #region attributes
        public bool loaded = false;
        public bool fullyLoaded = false;
        public string notes = "";
        /// <summary>
        /// This will retain the IDs of the process read from the database. The purpose is that when we then save the file
        /// the order of writting XML nodes is guaranteed (List guarantee ordering, dictionary does not). Moreover this 
        /// allows us to add an extra feature which is to insert new pathway at random places in the collection. Thus when
        /// comparing the datafile on a revision control system, merging conflicts due to multiple inserts by many different people 
        /// is reduced
        /// </summary>
        List<int> _idReadFromXML = new List<int>();
        #endregion

        #region methods

        /// <summary>
        /// Read the xml file and create an array of pathways
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool ReadDB(GData data, XmlNode pathways)
        {

            this.fullyLoaded = true;
            this.Clear();

            foreach (XmlNode pathway in pathways.SelectNodes("pathway"))
            {
                try
                {
                    Pathway pathwayData = new Pathway(data, pathway);
                    this.Add(pathwayData.Id, pathwayData);
                    _idReadFromXML.Add(pathwayData.Id);
                }
                catch (Exception e)
                {
                    this.fullyLoaded = false;
                    LogFile.Write("Error 91:" + e.Message);
                }
            }


            this.loaded = true;

            return this.loaded;
        }

        /// <summary>
        /// Saves the data as an XML file
        /// </summary>
        /// <param name="fileInfo">full path to the file</param>
        /// <returns></returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {
                XmlNode root = xmlDoc.CreateNode("pathways");

                XmlNode groups = xmlDoc.CreateNode("groups", xmlDoc.CreateAttr("notes", this.notes));
                root.AppendChild(groups);

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

                foreach (int pathID in _idReadFromXML)
                {
                    try
                    {
                        if (this.ContainsKey(pathID))
                            root.AppendChild(this[pathID].ToXmlNode(xmlDoc));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 263:" + "Error pathway id=" + pathID + " \r\n" + e.Message);
                    }
                }
                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 90:" + "Error saving file \r\n" + e.Message);
                return null;
            }

        }

        /// <summary>
        /// Return the first unused id for the pathways
        /// </summary>
        /// <returns></returns>
        public int GetFirstIdUnused(int[] ids)
        {
            int min_unused = ids.Max() + 1;

            for (int i = 1; i < ids.Max(); i++)
            {
                if (ids.Contains<int>(i) == false)
                    min_unused = Math.Min(min_unused, i);
            }

            return min_unused;
        }

  
        #endregion methods

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IPathway value)
        {
            this.Add(value.Id, value as Pathway);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IPathway ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as IPathway;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IPathway CreateValue(IData data, int type = 0)
        {
            Pathway pathway = new Pathway((data as GData).PathwaysData.Keys.ToArray<int>());
            return pathway as IPathway;
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
        public IEnumerable<IPathway> AllValues
        {
            get { return this.Values as IEnumerable<IPathway>; }
        }

        #endregion

    }
}
