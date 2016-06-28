using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// Represents a vertex in a pathway, may be holding a reference to a process, a pathway or a mix
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IVertex
    {
        /// <summary>
        /// Unique ID for that vertex
        /// </summary>
        Guid ID { get; }
        /// <summary>
        /// Location if defined for representation on a graph, could be null if the user never place that item on the graph manually
        /// </summary>
        PointF Location { get; }
        /// <summary>
        /// Reference to the process model unique ID used for this vertex
        /// </summary>
        int ModelID { get; }
        /// <summary>
        /// 0 for a process, 1 for a pathway, 2 for a mix, 3 for an output (used in rare cases)
        /// </summary>
        int Type { get; }
    }
}
