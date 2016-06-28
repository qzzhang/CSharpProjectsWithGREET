using System.Reflection;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used to list out Vehicles for a datagridview representation
    /// </summary>
    public class VehicleListItem
    {
        #region Attributes
        /// <summary>
        /// The name of the vehicle (redundant data as we have the ID we could get the name though a database call)
        /// </summary>
        string vehicleName;

        /// <summary>
        /// Fuel used names comma separated for representation in the GUI (could probably be removed and use a database call with the vehicle ID)
        /// </summary>
        string fuelsUsed;

        /// <summary>
        /// The ID of the vehicle represented in the datagrid view
        /// </summary>
        int vehicleID;
        #endregion

        #region Accessors

        /// <summary>
        /// String representing the name of the vehicle
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleName
        {
            get { return vehicleName; }
            set { vehicleName = value; }
        }

        /// <summary>
        /// String representing fuel names, comma separated for the vehicle
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string FuelsUsed
        {
            get { return fuelsUsed; }
            set { fuelsUsed = value; }
        }

        /// <summary>
        /// Integer representing the id of this vehicle
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int VehicleID
        {
            get { return vehicleID; }
            set { vehicleID = value; }
        }
        #endregion
    }
}
