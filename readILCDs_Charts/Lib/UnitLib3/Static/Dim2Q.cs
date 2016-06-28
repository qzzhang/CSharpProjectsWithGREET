using System.Collections.Generic;

namespace Greet.UnitLib3
{
    /// <summary>
    /// Converts integer representation of a dimenstion to a string like mass, energy, volume
    /// </summary>
    public class Dim2Q : Dictionary<uint, string>
    {
        new public string this[uint dim]
        {
            get
            {
                if (dim == DimensionUtils.FromMLT(1, 0, 0))
                    return "mass";
                else if (dim == DimensionUtils.FromMLT(0, 3, 0))
                    return "volume";
                else if (dim == DimensionUtils.FromMLT(1, 3, -1))
                    return "energy";
                else if (dim == 0)
                    return "unitless";
                else
                    return dim.ToString();

            }
        }
    }
}