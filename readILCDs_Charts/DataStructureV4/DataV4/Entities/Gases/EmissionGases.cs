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
    [Serializable]
    public class EmissionGases : Dictionary<int, Gas>, IGDataDictionary<int, IGas>
    {
        #region attributes

        /// <summary>
        /// Groups to categorise the Gases
        /// </summary>
        private Dictionary<int, Group> groups;
        public bool loaded = false;
        public bool fullyLoaded = false;
        private Dictionary<supportedBalanceTypes, GasBalanceReference> _balancesIds;
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

        public EmissionGases()
        {
            this.groups = new Dictionary<int, Group>();
            //default balance gases ids for old versions
            _balancesIds = new Dictionary<supportedBalanceTypes, GasBalanceReference>();
            _balancesIds.Add(supportedBalanceTypes.carbon, new GasBalanceReference(supportedBalanceTypes.carbon, 9, "The gas ID for which the factor can be calculated from a carbon balance", new List<int> { 1, 14, 2, 7 }));
            _balancesIds.Add(supportedBalanceTypes.sulfur, new GasBalanceReference(supportedBalanceTypes.sulfur, 6, "The gas ID for which the factor can be calculated from a sulfur balance"));
            _balancesIds.Add(supportedBalanceTypes.biogenic, new GasBalanceReference(supportedBalanceTypes.biogenic, 9, "The gas ID for which the factor can be calculated from a biogenic carbon balance"));
        }

        protected EmissionGases(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.loaded = info.GetBoolean("loaded");
            this.fullyLoaded = info.GetBoolean("fullyLoaded");

            int fuelRefCount = info.GetInt32("groupCount");
            for (int i = 0; i < fuelRefCount; i++)
            {
                this.Add(info.GetInt32("key" + i), (Gas)info.GetValue("value" + i, typeof(Gas)));
            }
        }

        #endregion construcors

        #region accessors

        public Gas this[string name]
        {
            get
            {
                Gas selected = this.Single(item => item.Value.Name == name).Value;
                return selected;
            }
            set
            {
                Gas selected = this.Single(item => item.Value.Name == name).Value;
                selected = value;
            }
        }

        public Dictionary<int, Group> Groups
        {
            get { return groups; }
            set { groups = value; }
        }
        
        public Dictionary<supportedBalanceTypes, GasBalanceReference> BalancesIds
        {
            get { return _balancesIds; }
            set { _balancesIds = value; }
        }
        #endregion accessors

        #region serializer

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("groupCount", this.Count);
            int i = 0;
            foreach (KeyValuePair<int, Gas> pair in this)
            {
                info.AddValue("key" + i, pair.Key);
                info.AddValue("value" + i, pair.Value, typeof(Gas));
                i++;
            }
            info.AddValue("loaded", this.loaded);
            info.AddValue("fullyLoaded", this.fullyLoaded);
        }

        #endregion

        #region methods

        public bool ReadDB(GData data, XmlNode GasesNodes)
        {
            try
            {
                this.Clear();
                this.groups.Clear();

                this.fullyLoaded = true;
                XmlNodeList groups = GasesNodes.SelectNodes("groups/group");
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

                //gases
                XmlNodeList gasesNodes = GasesNodes.SelectNodes("gases/gas");
                foreach (XmlNode gas in gasesNodes)
                {
                    try
                    {
                        Gas gasData = new Gas(data, gas);
                        this.Add(gasData.Id, gasData);
                        _idReadFromXML.Add(gasData.Id);
                    }
                    catch (Exception e)
                    {
                        this.fullyLoaded = false;
                        LogFile.Write("Error 48:" + e.Message);
                    }
                }

                //balances types and IDs
                XmlNodeList balancesNodes = GasesNodes.SelectNodes("balanced/balance");
                foreach (XmlNode bal in balancesNodes)
                {
                    try
                    {
                        supportedBalanceTypes balType = (supportedBalanceTypes)Enum.Parse(typeof(supportedBalanceTypes), bal.Attributes["type"].Value);
                        int gasRef = Convert.ToInt32(bal.Attributes["gas"].Value);
                        string notes = bal.Attributes["notes"].Value;
                        List<int> parameters = new List<int>();
                        foreach (XmlNode gpara in bal.ChildNodes)
                        { 
                            int childGasRef = Convert.ToInt32(gpara.Attributes["ref"].Value);
                            parameters.Add(childGasRef);
                        }

                        GasBalanceReference balRef = new GasBalanceReference(balType, gasRef, notes, parameters);
                        if(_balancesIds.ContainsKey(balType))
                            _balancesIds[balType] = balRef;
                        else
                            _balancesIds.Add(balType, balRef);
                    }
                    catch(Exception e)
                    {
                        this.fullyLoaded = false;
                        LogFile.Write("Error 488:" + e.Message);
                    }
                }


                this.loaded = true;
                return loaded;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 47:" + e.Message);
                return false;
            }


        }
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            try
            {
                XmlNode root = xmlDoc.CreateNode("gases");
                XmlNode groups = xmlDoc.CreateNode("groups");
                XmlNode gases = xmlDoc.CreateNode("gases");
                foreach (Group group in this.groups.Values)
                    groups.AppendChild(group.ToXmlNode(xmlDoc));


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

                foreach (int gasId in _idReadFromXML)
                {
                    try
                    {
                        if(this.ContainsKey(gasId))
                            gases.AppendChild(this[gasId].ToXmlNode(xmlDoc));
                    }
                    catch 
                    {
                        LogFile.Write("Failed to save gas id = " + gasId);
                    }
                }

                root.AppendChild(groups);
                root.AppendChild(gases);

                //balances types and IDs
                XmlNode balanced = xmlDoc.CreateNode("balanced");
                foreach (var pair in _balancesIds)
                {
                    XmlNode balance = xmlDoc.CreateNode("balance"
                        , xmlDoc.CreateAttr("type", pair.Key.ToString())
                        , xmlDoc.CreateAttr("gas", pair.Value.GasRef)
                        , xmlDoc.CreateAttr("notes", pair.Value.Notes));
                    if (pair.Value.Parameters != null)
                    {
                        foreach (int parameter in pair.Value.Parameters)
                        {
                            XmlNode node = xmlDoc.CreateNode("parameter", xmlDoc.CreateAttr("ref", parameter));
                            balance.AppendChild(node);
                        }
                    }
                    balanced.AppendChild(balance);
                }
                root.AppendChild(balanced);
               
                return root;
            }
            catch (Exception e)
            {
                LogFile.Write("Error 46:" + e.Message);
                return null;
            }
        }
        public int[] GetEmissionGasesIds()
        {
            int[] emission_gas_ids = this.Values.Where(item1 => item1.Memberships.Contains(3)).Select(item1 => item1.Id).ToArray<int>().OrderBy(item1 => item1).ToArray<int>(); //HARDCODED
            return emission_gas_ids;
        }

        #endregion methods

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IGas value)
        {
            this.Add(value.Id, value as Gas);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGas ValueForKey(int key)
        {
            if (this.KeyExists(key))
                return this[key] as IGas;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGas CreateValue(IData data, int type = 0)
        {
            Gas gas = new Gas(data as GData);
            return gas as IGas;
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
        public IEnumerable<IGas> AllValues
        {
            get { return this.Values as IEnumerable<IGas>; }
        }

        #endregion

        /// <summary>
        /// Get a list of current members of a groups for this object, this is part of the IGroupAvailable interface
        /// This method allows us to reuse the same group editor for all objects that are implementing IGroupAvailable
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public List<string> GetCurrentMembers(Group g)
        {
            List<string> list = new List<string>();
            List<int> idLookup = new List<int>();
            idLookup.Add(g.Id);
            foreach (Group gr in this.Groups.Values.Where(item => item.IncludeInGroups.Contains(g.Id)))
                idLookup.Add(gr.Id);

            foreach (Gas gas in this.Values)
                foreach (int id in idLookup)
                    if (gas.Memberships.Contains(id))
                        list.Add(gas.Name);
            return list;
        }
    }
}