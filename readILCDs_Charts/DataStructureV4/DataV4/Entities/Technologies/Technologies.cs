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
    /// Container for all of the technology data
    /// </summary>
    [Serializable]
    public class Technologies : Dictionary<int, TechnologyData>, IGDataDictionary<int, ITechnology>
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
        #endregion attributes

        #region constructors

        #endregion constructors

        #region methods

        /// <summary>
        /// Create instances of technologies from the XML data
        /// Supports reading from old technology tree structure and new flatten out structure before June 2013
        /// </summary>
        /// <param name="technologyNode"></param>
        /// <returns></returns>
        public bool ReadDB(GData data, XmlNode technologyNode)
        {

            this.fullyLoaded = true;
            this.Clear();

            foreach (XmlNode techno in technologyNode.SelectNodes("technology"))
            {
                try
                {
                    TechnologyData technolo = new TechnologyData(data, techno, "new_");
                    this.Add(technolo.Id, technolo);
                    _idReadFromXML.Add(technolo.Id);
                }
                catch (Exception e) { LogFile.Write("Error 8:" + e.Message); }
            }

            this.loaded = true;
            return this.loaded;
        }

        /// <summary>
        /// Creates an XMLNode from the data in that class so this object
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>The data of this instance as an XMLNode</returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {
                XmlNode root = xmlDoc.CreateNode("technologies");

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

                foreach (int techId in this.Keys)
                {
                    try
                    {
                        if(this.ContainsKey(techId))
                            root.AppendChild(this[techId].ToXmlNode(xmlDoc));
                    }
                    catch
                    {
                        LogFile.Write("Failed to save technology id=" + techId);
                    }
                }

                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 9:" + "Error saving file \r\n" + e.Message);
                return null;
            }

        }

        /// <summary>
        /// Returns the name of the technology from the techId
        /// </summary>
        /// <param name="techId"></param>
        /// <returns></returns>
        public string GetTechnologyName(int techId)
        {
            if (this.ContainsKey(techId))
                return this[techId].Name;
            else
                return "Technology ID " + techId.ToString() + " does not exits";
        }

        /// <summary>
        /// Changes a technology ID to a new ID and all the references in other objects
        /// that refers to it
        /// </summary>
        /// <param name="resourceId">The fuel ID for which this technology is used, or input resource of the technology</param>
        /// <param name="oldId">The old ID of the technology</param>
        /// <param name="newId">The new ID for the technology that will be also updating all references</param>
        private int ChangeTechnologyReferences(int resourceId, int oldId, int newId, GData database)
        {
            int changes = 0;
            #region Dependencies in Processes
            foreach (AProcess process in database.ProcessesData.Values)
            {
                if (process is TransportationProcess)
                {
                    foreach (TransportationStep step in (process as TransportationProcess).TransportationSteps.Values)
                    {
                        if (database.ModesData.ContainsKey(step.ModeReference)
                            && database.ModesData[step.ModeReference].FuelSharesData.ContainsKey(step.FuelShareRef))
                        {
                            foreach (ModeEnergySource fuelShare in database.ModesData[step.modeReference].FuelSharesData[step.FuelShareRef].ProcessFuels.Values)
                            {
                                if (fuelShare.ResourceReference.ResourceId == resourceId)
                                {
                                    if (fuelShare.TechnologyFrom == oldId)
                                    {
                                        fuelShare.TechnologyFrom = newId;
                                        changes++;
                                    }
                                    if (fuelShare.TechnologyTo == oldId)
                                    {
                                        fuelShare.TechnologyTo = newId;
                                        changes++;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (process is StationaryProcess)
                {
                    foreach (Input inp in (process as StationaryProcess).OtherInputs)
                    {
                        if (inp.ResourceId == resourceId)
                        {
                            foreach (TechnologyRef tref in inp.Technologies)
                            {
                                if (tref.Reference == oldId)
                                {
                                    changes++;
                                    tref.Reference = newId;
                                }
                            }
                        }
                    }
                    if ((process as StationaryProcess).Group != null)
                    {
                        foreach (Input inp in (process as StationaryProcess).Group.Group_amount_inputs)
                        {
                            if (inp.ResourceId == resourceId)
                            {
                                foreach (TechnologyRef tref in inp.Technologies)
                                {
                                    if (tref.Reference == oldId)
                                    {
                                        tref.Reference = newId;
                                        changes++;
                                    }
                                }
                            }
                        }
                        foreach (Input inp in (process as StationaryProcess).Group.Shares)
                        {
                            if (inp.ResourceId == resourceId)
                            {
                                foreach (TechnologyRef tref in inp.Technologies)
                                {
                                    if (tref.Reference == oldId)
                                    {
                                        tref.Reference = newId;
                                        changes++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Dependent Technologies

            foreach (TechnologyData techno in database.TechnologiesData.Values)
            {
                if (techno.BaseTechnology == oldId)
                    techno.BaseTechnology = newId;
            }
            
            #endregion

            return changes;
        }
        
        #endregion methods

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(ITechnology value)
        {
            this.Add(value.Id, value as TechnologyData);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public ITechnology ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as ITechnology;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public ITechnology CreateValue(IData data, int type = 0)
        {
            TechnologyData technology = new TechnologyData(data as GData);
            return technology as ITechnology;
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
        public IEnumerable<ITechnology> AllValues
        {
            get { return this.Values as IEnumerable<ITechnology>; }
        }

        #endregion

    }
}