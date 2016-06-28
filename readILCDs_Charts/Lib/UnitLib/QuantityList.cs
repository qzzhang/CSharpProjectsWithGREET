using System;
using System.Collections.Generic;

namespace Greet.UnitLib
{
    /// <summary>
    /// This class inherits from List and implements indexing with a string for lookup in the list
    /// </summary>
    /// 
    [Serializable]
    public class QuantityList : Dictionary<string, BaseQuantity>
    {
        public QuantityList()
        {

        }
        /// <summary>
        /// Returns the group which name matches the passes string or if the name was a unit then it returns the units UnitGroup
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new BaseQuantity this[string name]
        {
            get
            {
                BaseQuantity value;
                if (this.TryGetValue(name, out value))
                    return value;

                if (String.IsNullOrEmpty(name) && this.TryGetValue("unitless", out value))
                    return value;

                //name wasn't a groupName, check and see if it was an expression of units and return that group
                DerivedQuantity newGroup = new DerivedQuantity(name);
                BaseQuantity match = newGroup.DefaultOnlyMatchedGroup;
                if (newGroup != match)  //Ensure the matched group didn't return itself, we don't want to end with a new group
                    return match;
                else
                    return newGroup;
            }
        }

        public BaseQuantity ContainsGroupName(string name)
        {
            if (String.IsNullOrEmpty(name) == false)
            {
                if (this.ContainsKey(name))
                    return this[name];
            }
            return null;
        }

        /// <summary>
        /// Searches the list of groups for the passed name and returns it if it finds it otherwise it assumes a unit expression
        /// was passed and tries to construct a DerivedGroup out of it. If the unit that was passed is not the same as the default for 
        /// the matched group then it converts valToConvert into the default unit from the passed unit.
        /// Ex. name = "Short Tons" val = 1
        /// val is converted to the default for the found group of mass so it becomes 907184 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valToConvert"></param>
        /// <returns></returns>
        public BaseQuantity MatchUnitAndConvert(string name, ref double valToConvert)
        {
            try
            {
                foreach (BaseQuantity g in this.Values)
                {   //Ignore case and underscores when finding value
                    if (g.Name.ToLower().Replace('_', ' ') == name.ToLower().Replace('_', ' '))
                        return g;
                }

                //name wasn't a groupName, check and see if it was an expression of units and return that group
                DerivedQuantity newGroup = new DerivedQuantity(name);
                BaseQuantity match = newGroup.DefaultOnlyMatchedGroup;

                foreach (DerivedQuantityBase b in newGroup.BaseGroups)
                {
                    if (b.OverrideUnit != b.DefaultUnit)
                    {
                        valToConvert = newGroup.ConvertFromOverrideToDefault(valToConvert);
                        break;
                    }
                }

                if (newGroup != match)  //Ensure the matched group didn't return itself, we don't want to end with a new group
                    return match;
                throw new Exception();
            }
            catch
            {
                throw new Exception("Group not found");
            }
        }
    }
}
