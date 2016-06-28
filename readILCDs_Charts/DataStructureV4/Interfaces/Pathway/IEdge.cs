using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// A edge represents a connection between two vertices inputs and outputs in a pahtway
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IEdge
    {
        /// <summary>
        /// "Source" end of the connection
        /// </summary>
        Guid OutputVertexID { get; }
        /// <summary>
        /// "Source" end of the connection
        /// </summary>
        Guid OutputID { get; }
        /// <summary>
        /// "Destination" end of the connection
        /// </summary>
        Guid InputVertexID { get; }
        /// <summary>
        /// "Destination" end of the connection
        /// </summary>
        Guid InputID { get; }
    }
}
