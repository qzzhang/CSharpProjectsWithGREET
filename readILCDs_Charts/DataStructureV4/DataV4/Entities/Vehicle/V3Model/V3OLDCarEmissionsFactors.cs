using System;
using System.Collections.Generic;
using System.Reflection;

using System.Xml;
using Greet.DataStructureV4.Interfaces;


namespace Greet.DataStructureV4.Entities.Legacy
{
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public abstract class V3OLDCarEmissionsFactors : IEmissionsFactors
    {
        public abstract XmlNode ToXmlNode(XmlDocument doc, ref XmlNode yearNode);
        public abstract V3OLDCarEmissionValue this[int index] { get; set; }
        public abstract IEnumerator<KeyValuePair<int, V3OLDCarEmissionValue>> GetEnumerator();
        public abstract void Add(int key, V3OLDCarEmissionValue value, string notes);
        public abstract void Remove(int key);
        public abstract Dictionary<int, V3OLDCarEmissionValue>.KeyCollection Keys { get; }
        public abstract bool ContainsKey(int key);

        #region IEmissionsFactors

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int BaseTechnology
        {
            get
            {
                if (this is V3OLDCarBasedEmissionFactors)
                    return (this as V3OLDCarBasedEmissionFactors).baseTechno;
                else
                    return -1;
            }
        }

        #endregion

    }
}
