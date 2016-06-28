using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greet.Plugins.EcoSpold01.Entities
{
    class SourceRelation
    {
        string _flowName = "";
        int _greetResourceId = -1;
        /// <summary>
        /// 0 for well,  1 for pathway, 2 for mix, 4 for previous
        /// </summary>
        int _source = -1;
        int _mixOrPathwayID = -1;
        Guid _mixOrPathwayOutput = Guid.Empty;
    }
}
