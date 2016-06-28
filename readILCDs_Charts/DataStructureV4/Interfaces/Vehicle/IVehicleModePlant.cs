using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// An operating mode for a vehicle, usually can be CD or CS mode
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IVehicleModePlant
    {
        /// <summary>
        /// List of fuels references used by that plant for that mode
        /// </summary>
        IEnumerable<IInputResourceReference> FuelUsed { get;}
    }
}
