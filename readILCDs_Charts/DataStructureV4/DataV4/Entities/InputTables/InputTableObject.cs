
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class InputTableObject
    {
        private object value = null;

        public string Notes = "";
        public string Help = "";
        public override string ToString()
        {
            return Value != null ? Value.ToString() : "";
        }

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
