using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    public class Mixes : Dictionary<int, Mix>, IGDataDictionary<int, IMix>
    {
        #region attributes
        /// <summary>
        /// This will retain the IDs of the process read from the database. The purpose is that when we then save the file
        /// the order of writting XML nodes is guaranteed (List guarantee ordering, dictionary does not). Moreover this 
        /// allows us to add an extra feature which is to insert new pathway at random places in the collection. Thus when
        /// comparing the datafile on a revision control system, merging conflicts due to multiple inserts by many different people 
        /// is reduced
        /// </summary>
        List<int> _idReadFromXML = new List<int>();
        #endregion

        public Mixes()
        {

        }
        /// <summary>
        /// Reads the XML file and create the data objects for mixes
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool ReadDB(GData data, XmlNode dataNode)
        {
            XmlDocument fxml = new XmlDocument();
            this.Clear();

            //the new way of loading mixes
            foreach (XmlNode mixNode in dataNode.SelectNodes("mix"))
            {
                try
                {
                    Mix mix = new Mix(data, mixNode, "");
                    this.Add(mix.Id, mix);
                    _idReadFromXML.Add(mix.Id);
                }
                catch (Exception e)
                {
                    LogFile.Write("Error 2228:" + e.Message + "While initializing mix: " + mixNode.ToString() + " See " + e.StackTrace);
                }
            }

            return true;
        }

        /// <summary>
        /// Put the data objects in an XML file
        /// </summary>
        /// <param name="fileInfo">the fileinfo to write the data</param>
        /// <returns></returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode mNode = xmlDoc.CreateNode("mixes");

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

            foreach (int mixId in _idReadFromXML)
            {
                try
                {
                    if(this.ContainsKey(mixId))
                        mNode.AppendChild(this[mixId].ToXmlNode(xmlDoc));
                }
                catch 
                {
                    LogFile.Write("Failed to save mix id = " + mixId);
                }
            }
            return mNode;
        }

        /// <summary>
        /// Get a mix object by a reference to the mix id
        /// </summary>
        /// <param name="mixId">the id of the mix withing the specified resrouce</param>
        /// <returns>Mix object from the database or null if cannot be found</returns>
        public Mix GetMix(int mixId)
        {
            if (!this.ContainsKey(mixId))
                return null;
            return this[mixId];
        }

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IMix value)
        {
            this.Add(value.Id, value as Mix);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IMix ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as IMix;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IMix CreateValue(IData data, int type = 0)
        {
            Mix mix = new Mix(data as GData);
            return mix as IMix;
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
        public IEnumerable<IMix> AllValues
        {
            get { return this.Values as IEnumerable<IMix>; }
        }

        #endregion

    }
}
