/*********************************************************************** 
mail contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this
  list of conditions and the following disclaimer in the documentation and/or
  other materials provided with the distribution.

* Neither the name of the {organization} nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
***********************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// <para>A pathway represents a flow of resources though processes in series. Each process is represented by a IProcessReference</para>
    /// <para>In order to get the results, or upstream associated with the production of a resource from a pathway, the prefered way is to get the results associated with the last process reference in that pathway</para>
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IPathway : IXmlObj
    {
        /// <summary>
        /// Name for this pathway as it is going to show on the graphical interface
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        string Name { get; set; }
        /// <summary>
        /// Picture for this pathway as it is going to show on the graphical interface
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        string PictureName { get; set; }
        /// <summary>
        /// Unique ID for this pathway among the pathways
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        int Id { get; set; }
        /// <summary>
        /// Returns and ordered list of the process IDs used in this pathway
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        List<IProcessReference> Processes { get; }
        /// <summary>
        /// Upstream results associated to that pathway
        /// <para>This is the prefered way to get the upstream for a product from any pathway. 
        /// This is equivalent of getting the upstream of the last process in this pathway</para>
        /// </summary>
        /// <param name="data">Data object needed for references</param>
        /// <returns>Dictionary containing the allocated results for each output of pathway</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        Dictionary<IIO, IResults> GetUpstreamResults(IData data);
        /// <summary>
        /// Each pathway must define one main output, especially usefull when the pathway has multiple outputs for the same resource ID
        /// This is used for simplification reasons, the user can chose a pathway as an upstream and the main intput will be automatically
        /// selected as the upstream associated with that pathway. Thus saving the user from chosing which outputs he desires.
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        Guid MainOutput { get; set; }
        /// <summary>
        /// <para>Lists outputs that are proper to the pathway</para>
        /// <para>This are not the outputs of the processes. The output of the processes are linked to these outputs using Edges but are not the same instances</para>
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        List<IIO> Outputs { get; }
        /// <summary>
        /// Vertices for that Pathway, vertices can hold a reference to a process, a pathway or a mix
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        List<IVertex> Vertices { get; }
        /// <summary>
        /// Edges that represents connections in between the vertices inputs and outputs
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        List<IEdge> Edges { get; }
        /// <summary>
        /// Returns the resource ID associated with the main output of the pathway.
        /// This accessor finds the Output for which the ID match the MainOutput guid and reutrns the resource defined for that output.
        /// Returns -1 if the main output cannot be found.
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        int MainOutputResourceID { get; }
        /// <summary>
        /// Indicates if the pathway resides in the data but has been discarded
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        bool Discarded { get; }
        /// <summary>
        /// Indicates who discarded that pathway
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        string DiscarededBy { get; }
        /// <summary>
        /// Indicates when this pathway has been discarded
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        DateTime DiscardedOn { get; }
        /// <summary>
        /// Indicates the reason for that item to be discarded
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        string DiscardedReason { get; }
    }
}
