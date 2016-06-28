using System.Reflection;


// lzf: to list vehicle modes
namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used to list out Vehicles-Mode-Plants for a datagridview representation
    /// </summary>
    public class VehicleModePowerPlantListItem
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
        /// The name of the vehicle mode power plant 
        /// </summary>
        string vehicleModePlantName;

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

        /// <summary>
        /// The ID of the vehicle mode plant represented in the datagrid view
        /// </summary>
        string vehicleModePlantID;
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
        /// String representing the name of the vehiclemodeplant 
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleModePlantName
        {
            get { return vehicleModePlantName; }
            set { vehicleModePlantName = value; }
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
        /// string representing the id of this vehicle mode
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleModeID
        {
            get { return vehicleModeID; }
            set { vehicleModeID = value; }
        }

        /// <summary>
        /// string representing the id of this vehicle mode plant
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string VehicleModePlantID
        {
            get { return vehicleModePlantID; }
            set { vehicleModePlantID = value; }
        }
        #endregion
    }
}
