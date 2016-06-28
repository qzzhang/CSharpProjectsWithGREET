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
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Reads the transportation.xml database. This dictionary maps a mode id to AMode object
    /// </summary>
    [Serializable]
    public class Modes : Dictionary<int, AMode>, IGDataDictionary<int, IAMode>
    {
        #region enumerators
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum ModeType { TankerBarge = 1, Truck = 2, Pipeline = 3, Rail = 4, MagicMove = 5 };

        #endregion enumerators

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
        #endregion attributes

        #region Constructors
        #endregion

        #region methods

        public bool ReadDB(GData data, XmlNode modeNode)
        {
            this.fullyLoaded = true;

            this.Clear();

            XmlNodeList modesnodes = modeNode.SelectNodes("mode");

            foreach (XmlNode mode in modesnodes)
            {
                string status = "";
                try
                {
                    status = "can't get type";
                    int type = Convert.ToInt32(mode.Attributes["type"].Value);
                    try
                    {
                        AMode modeToAdd;
                        switch (type)
                        {
                            case 1:
                                modeToAdd = new ModeTankerBarge(data, mode, "");
                                break;
                            case 2:
                                modeToAdd = new ModeTruck(data, mode, "");
                                break;
                            case 3:
                                modeToAdd = new ModePipeline(data, mode, "");
                                break;
                            case 4:
                                modeToAdd = new ModeRail(data, mode, "");
                                break;
                            case 5:
                                modeToAdd = new ModeConnector(data, mode, "");
                                break;
                            default:
                                modeToAdd = null;
                                break;
                        }


                        this.Add(modeToAdd.Id, modeToAdd);
                        _idReadFromXML.Add(modeToAdd.Id);
                    }
                    catch (Exception e1)
                    {
                        this.fullyLoaded = false;
                        LogFile.Write("Error 1986:" + mode.OwnerDocument.BaseURI + "\r\n" + mode.OuterXml + "\r\n" + e1.Message + "\r\n" + status + "\r\n");
                    }
                }
                catch (Exception e)
                {
                    this.fullyLoaded = false;
                    LogFile.Write("Error 86:" + mode.OwnerDocument.BaseURI + "\r\n" + mode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                }
            }

            this.loaded = true;
            return this.loaded;
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {

                XmlNode root = xmlDoc.CreateNode("modes");

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

                foreach (int modId in _idReadFromXML)
                {
                    try
                    {
                        if (this.ContainsKey(modId))
                            root.AppendChild(this[modId].ToXmlNode(xmlDoc));
                    }
                    catch 
                    {
                        LogFile.Write("Failed to save mode id = " + modId);
                    }
                }

                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 85:" + "Error saving file \r\n" + e.Message);
                return null;
            }
        }

     

        #endregion methods

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IAMode value)
        {
            this.Add(value.Id, value as AMode);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IAMode ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as IAMode;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IAMode CreateValue(IData data, int type = 0)
        {
            AMode mode;
            switch (type)
            {
                case 1: mode = new ModeTankerBarge(data as GData); 
                    break;
                case 2: mode = new ModeTruck(data as GData);
                    break;
                case 3: mode = new ModePipeline(data as GData);
                    break;
                case 4: mode = new ModeRail(data as GData);
                    break;
                case 5:
                default: mode = new ModeConnector(data as GData);
                    break;
            }

            return mode;
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
        public IEnumerable<IAMode> AllValues
        {
            get { return this.Values as IEnumerable<IAMode>; }
        }

        #endregion

    }
}
