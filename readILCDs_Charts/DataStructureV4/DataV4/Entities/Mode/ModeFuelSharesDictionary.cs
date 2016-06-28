using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class ModeFuelSharesDictionary : Dictionary<InputResourceReference, ModeEnergySource>, ISerializable
    {
        #region constructor

        public ModeFuelSharesDictionary()
        { }

        #endregion

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected ModeFuelSharesDictionary(SerializationInfo information, StreamingContext context)
            : base(information, context)
        {
        }
    }
}
