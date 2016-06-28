using Greet.DataStructureV4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class InputTablesDictionary : Dictionary<string, InputTable>, IGDataDictionary<string, IInputTable>
    {
        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IInputTable value)
        {
            this.Add(value.Id, value as InputTable);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IInputTable ValueForKey(string key)
        {
            if (this.KeyExists(key))
                return this[key] as IInputTable;
            else
                return null;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IInputTable CreateValue(IData data, int type = 0)
        {
            InputTable inputTable = new InputTable();
            return inputTable as IInputTable;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool KeyExists(string key)
        {
            return this.ContainsKey(key);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool DeleteValue(IData data, string key)
        {
            if (this.KeyExists(key))
            {
                ToolsDataStructure.RemoveAllParameters(data, this.ValueForKey(key));
                return this.Remove(key);
            }
            else
                return false;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IEnumerable<IInputTable> AllValues
        {
            get { return this.Values as IEnumerable<IInputTable>; }
        }

        #endregion
    }
}
