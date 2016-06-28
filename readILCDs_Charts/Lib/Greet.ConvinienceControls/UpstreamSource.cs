using System;
using Greet.DataStructureV4.Interfaces;

namespace Greet.ConvinienceControls
{
    /// <summary>
    /// Class that stores information in order to retreive upstream results from a pathway or a mix in GREET
    /// This class stores resource id, pathway or mix id and a switch to define weather we refer to a pathway or a mix
    /// </summary>
    [Serializable]
    public class UpstreamSource : IInputResourceReference
    {
        #region private members
        /// <summary>
        /// Resource id that will match a pathway main output or mix output
        /// </summary>
        private int resource_id;
        /// <summary>
        /// Id of a pathway or mix
        /// </summary>
        private int entity_id;
        /// <summary>
        /// Defines weather the entity_id is a pathway or a mix
        /// </summary>
        private Greet.DataStructureV4.Interfaces.Enumerators.SourceType et;
        #endregion

        #region public constructor
        /// <summary>
        /// Creates a new upstream source with resource id and entity id set to -1
        /// The source type et is set as null
        /// </summary>
        public UpstreamSource()
        {
            resource_id = -1;
            entity_id = -1;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Sets the resource id and entity id to -1
        /// Do not do anything with the source type
        /// </summary>
        public void Clear()
        {
            resource_id = -1;
            entity_id = -1;
        }
        /// <summary>
        /// Sets the private members of the class for resource id, entity id and source type accordingly to the given parameters
        /// </summary>
        /// <param name="_resrouce_id">Desired resource id to be set</param>
        /// <param name="_entity_id">Desired entity id to be set</param>
        /// <param name="_et">Desired source type to be set</param>
        public UpstreamSource(int _resrouce_id, int _entity_id, Greet.DataStructureV4.Interfaces.Enumerators.SourceType _et)
        {
            resource_id = _resrouce_id;
            entity_id = _entity_id;
            et = _et;
        }
        /// <summary>
        /// Returns a human readable source type for use in the user interface
        /// </summary>
        /// <returns></returns>
        public string ToString(IData data)
        {
            if (et == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
                return String.Format("{0} ({1}: {2})", data.Resources.ValueForKey(resource_id).Name, "Mix", data.Mixes.ValueForKey(entity_id).Name);
            if (et == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway)
                return String.Format("{0} ({1}: {2})", data.Resources.ValueForKey(resource_id).Name, "Pathway", data.Pathways.ValueForKey(entity_id).Name);
            else
                return "empty";
        }
        /// <summary>
        /// Returns True if all parameters are set for this instance (resource and entity id must be different than -1)
        /// Does not performs checks on the SourceType
        /// </summary>
        /// <returns></returns>
        public bool Filled()
        {
            return (resource_id != -1 && entity_id != -1);
        }
        #endregion

        #region accessors
        /// <summary>
        /// Returns a human readable source type for use in the user interface
        /// Similar to the .ToString() method
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return this.ToString();
            }

        }
        #endregion

        #region IInputResourceReference
        /// <summary>
        /// The resource Id the Feed is outputing
        /// </summary>
        public int ResourceId
        {
            get
            {
                return resource_id;
            }
            set
            {
                resource_id = value;
            }
        }

        /// <summary>
        /// Returns the Mix Id if SourceType is Mix 
        /// or 
        /// Returns Pathway Id if SourceType is Feed
        /// </summary>
        public int SourceMixOrPathwayID
        {
            get
            {
                return entity_id;
            }
            set
            {
                entity_id = value;
            }
        }

        /// <summary>
        /// Returns the type of source of feed.
        /// </summary>
        public Greet.DataStructureV4.Interfaces.Enumerators.SourceType SourceType
        {
            get
            {
                return et;
            }
            set
            {
                et = value;
            }

        }

        #endregion
    };
}