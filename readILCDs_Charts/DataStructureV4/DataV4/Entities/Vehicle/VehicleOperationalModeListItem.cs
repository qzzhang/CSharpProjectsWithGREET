using System.Reflection;


// lzf: to list vehicle modes
namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used to list out Vehicles Modes for a datagridview representation
    /// </summary>
    public class VehicleOperationalModeListItem
    {
        #region Attributes
        /// <summary>
        /// The name of the vehicle (redundant data as we have the ID we could get the name though a database call)
        /// </summary>
        string vehicleName;

        /// <summary>
        /// The name of the vehicle modes 
        /// </summary>
        string vehicleModeName;

        /// <summary>
        /// Fuel used names comma separated for representation in the GUI 
        /// </summary>
        string fuelsUsed;

        /// <summary>
        /// The ID of the vehicle represented in the datagrid view
        /// </summary>
        int vehicleID;

        /// <summary>
        /// The ID of the vehicle mode represented in the datagrid view
        /// </summary>
        string vehicleModeID;
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
        /// String representing the name of the vehiclemode 
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleModeName
        {
            get { return vehicleModeName; }
            set { vehicleModeName = value; }
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

        /// <summary>
        /// Integer representing the id of this vehicle mode
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleModeID
        {
            get { return vehicleModeID; }
            set { vehicleModeID = value; }
        }
        #endregion
    }
}
