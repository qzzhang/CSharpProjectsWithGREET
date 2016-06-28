using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class Series<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public Series()
        {
        }

        protected Series(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {

        }
    }
}