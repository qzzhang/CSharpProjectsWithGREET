// *********************************************************************** 
//  COPYRIGHT NOTIFICATION 
// 
//  Email contact: greet@anl.gov 
//  Copyright (c) 2012, UChicago Argonne, LLC 
//  All Rights Reserved
//  
//  THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
//  LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
//  CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
//  NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 
//  
//  ************************************************************************ 
//  ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
//  IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
//  ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
//  ENERGY. 
//  ************************************************************************
//   
//  ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    public enum VehicleFunctionalUnit { mj, hkm, mi, tonmi, tonnekm, passengermi, passengerkm };

    /// <summary>
    /// Inherits from Dictionary in order to store all the vehicles definitions from the database
    /// </summary>
    [Serializable]
    public class Vehicles : Dictionary<int, Vehicle>, IGDataDictionary<int, IVehicle>
    {
        #region Fields and Constants

        /// <summary>
        /// This will retain the IDs of the process read from the database. The purpose is that when we then save the file
        /// the order of writting XML nodes is guaranteed (List guarantee ordering, dictionary does not). Moreover this 
        /// allows us to add an extra feature which is to insert new pathway at random places in the collection. Thus when
        /// comparing the datafile on a revision control system, merging conflicts due to multiple inserts by many different people 
        /// is reduced
        /// </summary>
        List<int> _idReadFromXML = new List<int>();

        /// <summary>
        /// If we completed the database loading without incidents this flag is set to true
        /// </summary>
        public bool fullyLoaded;

        /// <summary>
        /// If we started loading the database this flag is set to true
        /// </summary>
        public bool loaded;

        /// <summary>
        /// Symbolize the average age of the vehicles on the road
        /// </summary>
        public Parameter vehicleTechnologyLag;

        #endregion

        #region Constructors

        #endregion

        #region Properties and Indexers

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IEnumerable<IVehicle> AllValues
        {
            get { return Values; }
        }

        #endregion

        #region Interfaces Implementation

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IVehicle value)
        {
            Add(value.Id, value as Vehicle);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IVehicle CreateValue(IData data, int type = 0)
        {
            Vehicle vehicle = new Vehicle();
            return vehicle;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool DeleteValue(IData data, int key)
        {
            if (KeyExists(key))
            {
                ToolsDataStructure.RemoveAllParameters(data, ValueForKey(key));
                return Remove(key);
            }
            return false;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool KeyExists(int key)
        {
            return ContainsKey(key);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IVehicle ValueForKey(int key)
        {
            if (KeyExists(key))
                return this[key];
            return null;
        }

        #endregion

        #region Members

        /// <summary>
        /// Reading the database in XML format to be loaded in the software, creates vehicles objects
        /// </summary>
        /// <param name="vehiclesNode"></param>
        /// <returns></returns>
        public bool ReadDB(GData data, XmlNode vehiclesNode)
        {
            vehicleTechnologyLag = data.ParametersData.CreateRegisteredParameter(vehiclesNode.SelectSingleNode("vehicle_technology_lag").Attributes["value"], "veh_techlag");

            //reading all the vehicles
            Clear();
            fullyLoaded = true;
            foreach (XmlNode xmlNode in vehiclesNode.SelectNodes("vehicle"))
            {
                try
                {
                    Vehicle vehicle = new Vehicle(data, xmlNode);
                    Add(vehicle.Id, vehicle);
                    _idReadFromXML.Add(vehicle.Id);
                }
                catch (Exception e)
                {
                    fullyLoaded = false;
                    LogFile.Write("Error 2:" + e.Message);
                }
            }

            loaded = true;
            return loaded;
        }

        /// <summary>
        /// Saves all the vehicles as an XML node that will be written in the data file
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {

                XmlNode root = xmlDoc.CreateNode("vehicles");

                XmlNode tech_lag = xmlDoc.CreateNode("vehicle_technology_lag", xmlDoc.CreateAttr("value", vehicleTechnologyLag));
                root.AppendChild(tech_lag);

                #region randomizing order of newly inserted processes/IDs in the XML file
                //First we find try to look for new processes/IDs that needs to be inserted in the database
                List<int> additionalIds = new List<int>();
                foreach (int id in Keys)
                    if (!_idReadFromXML.Contains(id))
                        additionalIds.Add(id);

                Random rnd = new Random();
                foreach (int id in additionalIds)
                {
                    int index = rnd.Next(0, _idReadFromXML.Count);
                    _idReadFromXML.Insert(index, id);
                }
                #endregion

                foreach (int vehId in _idReadFromXML)
                {
                    try
                    {
                        if(ContainsKey(vehId))
                            root.AppendChild(this[vehId].ToXmlNode(xmlDoc));
                    }
                    catch (Exception)
                    {
                        LogFile.Write("Failed to save vehicle id = " + vehId);
                    }

                }

                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 120:" + "Error saving file \r\n" + e.Message);
                return null;
            }
        }

        #endregion
    }
}
