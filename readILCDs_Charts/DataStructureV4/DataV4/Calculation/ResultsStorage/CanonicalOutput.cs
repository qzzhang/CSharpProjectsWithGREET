using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.UnitLib3;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    /// <summary>
    /// This object stores a reference to an output for the calculations as well as associated upstream results with that output for a vertex in a specific pathway
    /// </summary>
    [Serializable]
    public class CanonicalOutput
    {
        /// <summary>
        /// Results stored from the calculations for that input in a specific vertex in a specific pathway
        /// </summary>
        public Results Results = new Results();
        /// <summary>
        /// Reference to an instance of the output used for the calculations here
        /// </summary>
        public AOutput Output = null;
        /// <summary>
        /// Biogenic carbon content for this output
        /// </summary>
        public double MassBiogenicCarbonRatio = 0;

        /// <summary>
        /// <para>Creates a new instance and assign a reference to the Output object</para>
        /// <para>Results are created Empty, Biogenic content set to zero</para>
        /// </summary>
        /// <param name="reference">Instance of an Input to be used by the calculations</param>
        public CanonicalOutput(AOutput reference)
        {
            this.Output = reference;
        }
    }


}
