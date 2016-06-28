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
    public class Processes : Dictionary<int, AProcess>, IGDataDictionary<int, IProcess>
    {
        #region attributes
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
        #endregion

        #region methods

        public bool ReadDB(GData data, XmlNode processeNode)
        {
            this.Clear();
            _idReadFromXML.Clear();
            this.fullyLoaded = true;
            try
            {
                foreach (XmlNode process in processeNode.ChildNodes)
                {
                    try
                    {
                        AProcess proc = null;
                        if (process.Name == "stationary")
                        {
                            proc = new StationaryProcess(data, process, "");
                            this.Add(proc.Id, proc);
                            _idReadFromXML.Add(proc.Id);
                        }
                        else if (process.Name == "transportation")
                        {
                            proc = new TransportationProcess(data, process, "");
                            this.Add(proc.Id, proc);
                            _idReadFromXML.Add(proc.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        this.fullyLoaded = false;
                        LogFile.Write("Error 28:" + e.Message + "While initializing process id: " + process.Attributes["id"].Value + " See " + e.StackTrace);
                    }
                }
                this.loaded = true;
                return this.loaded;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 27:" + "\r\n" +
                    e.Message + "\r\n");
                return false;
            }

        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {
                XmlNode root = xmlDoc.CreateNode("processes");

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

                foreach (int procID in _idReadFromXML)
                {
                    try
                    {
                        if (this.ContainsKey(procID))
                            root.AppendChild(this[procID].ToXmlNode(xmlDoc));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 26:" + "Error process id=" + procID + " \r\n" + e.Message);
                    }
                }

                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 26:" + "Error saving file \r\n" + e.Message);
                return null;
            }
        }
        public Dictionary<int, TransportationProcess> Transportation()
        {
            var processes =
                from p in this.Values
                where (p is TransportationProcess)
                select p;
            Dictionary<int, TransportationProcess> res = new Dictionary<int,TransportationProcess>();
            foreach (TransportationProcess p in processes)
                res.Add(p.Id, p);
            return res;
        }
        public Dictionary<int, StationaryProcess> Stationary()
        {
            var processes =
                from p in this.Values
                where (p is StationaryProcess)
                select p;
            Dictionary<int, StationaryProcess> res = new Dictionary<int, StationaryProcess>();
            foreach (StationaryProcess p in processes)
                res.Add(p.Id, p);
            return res;
        }
       
        #endregion

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IProcess value)
        {
            this.Add(value.Id, value as AProcess);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IProcess ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as IProcess;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IProcess CreateValue(IData data, int type = 0)
        {
            AProcess process;
            switch (type)
            {
                case 2: process = new TransportationProcess(data as GData);
                    break;
                case 1:
                default: process = new StationaryProcess(data as GData);
                    break;
            }
            return process;
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
        public IEnumerable<IProcess> AllValues
        {
            get { return this.Values as IEnumerable<IProcess>; }
        }

        #endregion

    }
}
