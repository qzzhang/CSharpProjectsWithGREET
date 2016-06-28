using System;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is used to hold a single process emission 
    /// </summary>
    [Serializable]
    public class ProcessStaticEmissionItem
    {
        #region Attributes
        private int gasId;
        private ParameterTS param;
        private string notes;
        #endregion

        #region Constructors
        public ProcessStaticEmissionItem(GData data, int gasId)
        {
            this.gasId = gasId;
            this.param = new ParameterTS(data, "kg", 0);
        }

        /// <summary>
        /// Constructor to be used from the Model library
        /// </summary>
        /// <param name="gasId">Gas ID for the emission</param>
        /// <param name="dfactor">Amount of gas released in kilograms</param>
        /// <param name="notes">Notes for that non combustion emission</param>
        internal ProcessStaticEmissionItem(int gasId, ParameterTS dfactor, string notes)
        {
            this.gasId = gasId;
            this.param = dfactor;
            this.notes = notes;
        }
        #endregion

        #region Accessors

        public int GasId
        {
            get { return gasId; }
            set { gasId = value; }
        }
        public ParameterTS EmParameter
        {
            get { return param; }
            set { param = value; }
        }
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        #endregion

    }
}
