using System.Reflection;


namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// Sequestration object that might be used with an input
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface ISequestration
    {
        /// <summary>
        /// Returns the source of the Sequestration which might be coming from a Pathway, Pathway Mix, Well, Previous Process
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        IInputResourceReference ResourceReference { get; }

    }
}
