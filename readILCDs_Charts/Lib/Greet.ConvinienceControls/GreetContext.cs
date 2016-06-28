using System;
using System.Collections.Generic;
using System.Linq;
using Greet.DataStructureV4.Interfaces;
using Greet.Model.Interfaces;

namespace Greet.ConvinienceControls
{
    /// <summary>
    /// This class is the wrapper around the GREET API for easier retrival of the data
    /// </summary>
    internal class GreetContext
    {
        #region members
        /// <summary>
        /// Greet controler is the first entry point
        /// </summary>
        private IGREETController _gc;
        #endregion

        #region constructors
        public GreetContext(IGREETController gc)
        {
            this._gc = gc;
        }
        #endregion

        #region internal methods

        /// <summary>
        /// Retrieves possible Upstream sources for a given resource ID
        /// Creates a list of all possible pathways and mixes that are creating that resource
        /// </summary>
        /// <param name="resource_id">Resource ID for which we want to retreive all possible upstream sources</param>
        /// <returns>List of possible upstream sources from the GREET database for the given resource ID</returns>
        internal List<UpstreamSource> GetUpstreamSources(int resource_id)
        {
            List<UpstreamSource> res = new List<UpstreamSource>();
            foreach (var item in _gc.CurrentProject.Data.Mixes.AllValues)
            {
                if (item.MainOutputResourceID == resource_id)
                    res.Add(new UpstreamSource(resource_id, item.Id, Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix));
            }
            foreach (var item in _gc.CurrentProject.Data.Pathways.AllValues)
            {
                foreach (IIO path_output in item.Outputs)
                    if (path_output.ResourceId == resource_id)
                        res.Add(new UpstreamSource(resource_id, item.Id, Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway));
            }
            return res;
        }

        #endregion
    }
}