using System;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used in order to know weather the emission factor needs to be a balanced item or is a value given by the user
    /// </summary>
    [Serializable]
    public class EmissionValue
    {
        private Parameter value = null;

        public Parameter Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public bool Balanced = false;

        public EmissionValue(Parameter value, bool balanced)
        {
            this.value = value;
            this.Balanced = balanced;
        }
    }
}
