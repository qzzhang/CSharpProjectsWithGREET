using System;
using System.Reflection;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// This class is used to list out V3OLDVehicles for a datagridview representation
    /// </summary>
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDVehicleListItem
    {
        #region Attributes
        /// <summary>
        /// The name of the V3OLDVehicle (redundant data as we have the ID we could get the name though a database call)
        /// </summary>
        string _vehicleName;

        /// <summary>
        /// Fuel used names comma separated for representation in the GUI (could probably be removed and use a database call with the V3OLDVehicle ID)
        /// </summary>
        string _fuelsUsed;

        /// <summary>
        /// The ID of the V3OLDVehicle represented in the datagrid view
        /// </summary>
        int _vehicleID;
        #endregion

        #region Accessors

        /// <summary>
        /// String representing the name of the V3OLDVehicle
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleName
        {
            get { return _vehicleName; }
            set { _vehicleName = value; }
        }

        /// <summary>
        /// String representing fuel names, comma separated for the V3OLDVehicle
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string FuelsUsed
        {
            get { return _fuelsUsed; }
            set { _fuelsUsed = value; }
        }

        /// <summary>
        /// Integer representing the id of this V3OLDVehicle
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int VehicleID
        {
            get { return _vehicleID; }
            set { _vehicleID = value; }
        }
        #endregion
    }
}
