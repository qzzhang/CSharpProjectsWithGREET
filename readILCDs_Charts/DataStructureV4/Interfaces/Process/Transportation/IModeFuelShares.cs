using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// A predefined set of fuel shares to be used by a mode
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IModeFuelShares
    {
        /// <summary>
        /// Unique ID for the mode fuel shares instance
        /// </summary>
        int Id { get; set; }
    }
}
