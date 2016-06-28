﻿/*********************************************************************** 
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

using System.Collections.Generic;

using System.Reflection;


namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// A reference to the definition of a process as used in a pathway
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IProcessReference
    {
        /// <summary>
        /// Reference to the ID of the process to be used in the pathway
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        int ModelId { get; set; }
        /// <summary>
        /// Upstream results associated to that process reference at it's position in the pathway
        /// <para>This is the prefered way to get the upstream for a product from any process reference. The upstream associated with a IPathway is equal the the upstream associated with the last process reference of this given pathway.</para>
        /// </summary>
        /// <param name="data">Data object needed for references</param>
        /// <returns>Dictionary containing the allocated results for each output of the IProcess refered by this process reference in a IPathway</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        Dictionary<IIO, IResults> GetUpstreamResults(IData data);

    }
}
