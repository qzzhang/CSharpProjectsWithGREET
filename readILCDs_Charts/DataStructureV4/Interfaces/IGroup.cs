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

using System.Collections.Generic;
using System.Reflection;

namespace Greet.DataStructureV4.Interfaces
{
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public interface IGroup : IXmlObj
    {
        /// <summary>
        /// Unique ID among all the groups in their parent object.
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        int Id { get; set; }

        /// <summary>
        /// Name for that specific instance of a group
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        string Name { get; set; }

        /// <summary>
        /// Defines parent groups if that instance should be included
        /// in the sum of other parent groups. As an example the Petroleum group
        /// is included into the Fossil Fuel group.
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        List<int> IncludeInGroups { get; set; }

        /// <summary>
        /// Notes that may be associated with that specific group
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        string Notes { get; set; }

        /// <summary>
        /// If set to true, all <see cref="IResults"/> instances will be returned containing the sum for that group
        /// under the <see cref="IResults.OnSiteEmissionsGroups"/> the <see cref="IResults.OnSiteResourcesGroups"/>
        /// and the <see cref="IResults.OnSiteUrbanEmissionsGroups"/>
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        bool ShowInResults { get; set; }


    }
}