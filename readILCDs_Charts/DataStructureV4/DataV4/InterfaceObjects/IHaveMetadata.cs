
namespace Greet.DataStructureV4
{
    /// <summary>
    /// Describes an entity that has metadata that can be displayed in the Notes form or edited
    /// </summary>
    public interface IHaveMetadata
    {
        /// <summary>
        /// Notes associated with that entity
        /// </summary>
        string Notes { get; set; }
        /// <summary>
        /// User that modified this entity the latest time
        /// </summary>
        string ModifiedBy { get; set; }
        /// <summary>
        /// Time at which this was modified for the last time
        /// </summary>
        string ModifiedOn { get; set; }
    }
}
