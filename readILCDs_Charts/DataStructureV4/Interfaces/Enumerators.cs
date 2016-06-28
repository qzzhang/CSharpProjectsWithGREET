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
using System.Reflection;

namespace Greet.DataStructureV4.Interfaces
{
    /// <summary>
    /// Enumerators used across the plugin interfaces
    /// </summary>
    public class Enumerators
    {

        /// <summary>
        /// Those are possible options for the main input source
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum SourceType { Well = 1, Mix = 2, Previous = 3, Pathway = 5 };

        /// <summary>
        /// Those are possible options for the results species
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum ResultType { emission, resource, emissionGroup, resourceGroup };

        /// <summary>
        /// Possible type for DependentItem
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum ItemType { Vehicle, Pathway, Pathway_Mix, Monitor, Process, Vehicle_Monitor, Technology, Mode, Resource, Input, Output, Parameter };

        /// <summary>
        /// Possible calculations run types
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum CalculationType { Fuels, Vehicles };

        /// <summary>
        /// Possible types of dependencies for losses on inputs and outputs
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum LossDependency { none, distance, time };
    }
}
