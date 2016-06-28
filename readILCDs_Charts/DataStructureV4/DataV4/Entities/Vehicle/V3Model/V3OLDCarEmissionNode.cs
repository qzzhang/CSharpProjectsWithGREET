using System;
using System.Runtime.Serialization;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.Entities.Legacy
{
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    internal class V3OLDCarEmissionNode
    {
        public int gasId;
        public V3OLDCarEmissionValue dfactor;
        public string notes;


        protected V3OLDCarEmissionNode(SerializationInfo info, StreamingContext context)
        {
            gasId = (int)info.GetValue("gasId", typeof(int));
            dfactor = (V3OLDCarEmissionValue)info.GetValue("dfactor", typeof(V3OLDCarEmissionValue));
            notes = (string)info.GetValue("notes", typeof(string));

        }
        public void GetObjectData(SerializationInfo info,
                                   StreamingContext context)
        {
            info.AddValue("gasId", gasId, typeof(int));
            info.AddValue("dfactor", dfactor, typeof(V3OLDCarEmissionValue));
            info.AddValue("notes", notes, typeof(string));
        }

        public V3OLDCarEmissionNode(int gasId, V3OLDCarEmissionValue dfactor, string notes)
        {
            this.gasId = gasId;
            this.dfactor = dfactor;
            this.notes = notes;
        }

    }
}
