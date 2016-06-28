using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.DataStructureV4;
using Greet.UnitLib3;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    /// <summary>
    /// This object stores a reference to an input for the calculations as well as upstream results associated with that input for a vertex in a specific pathway
    /// </summary>
    [Serializable]
    public class CanonicalInput
    {
        /// <summary>
        /// Results stored from the calculations for that input in a specific vertex in a specific pathway
        /// </summary>
        public InputResult Results = new InputResult("");
        /// <summary>
        /// Reference to an instance of the input used for the calculations here
        /// </summary>
        public Input Input = null;

        /// <summary>
        /// Creates a new instance and assign a reference to the Input object
        /// </summary>
        /// <param name="reference">Instance of an Input to be used by the calculations</param>
        public CanonicalInput(Input reference)
        {
            this.Input = reference;
        }
    }

   
}
