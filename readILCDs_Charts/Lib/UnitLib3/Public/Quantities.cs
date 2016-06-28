using System;
using System.Collections.Generic;
using System.Linq;

namespace Greet.UnitLib3
{
    /// <summary>
    /// This class inherits from List and implements indexing with a string for lookup in the list
    /// </summary>
    /// 
    [Serializable]
    public class Quantities : Dictionary<string, AQuantity>
    {
        public Quantities() : base()
        {

        }

        public AQuantity ByDim(uint dim)
        {
            return this.Values.FirstOrDefault(item => item.Dim == dim);
        }
    }
}
