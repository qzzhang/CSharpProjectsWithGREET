using System.Collections.Generic;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This struct is used to store a reference to a gas ID for the calculation balance purposes
    /// </summary>
    public struct GasBalanceReference
    {
        #region attributes
        /// <summary>
        /// The type of balance for which this gas reference is stored
        /// </summary>
        supportedBalanceTypes _type;
        /// <summary>
        /// The reference to the gas that is being balance
        /// </summary>
        int _gasRef;
        /// <summary>
        /// Notes to remember what is that gas reference about
        /// </summary>
        string _notes;
        /// <summary>
        /// A list of parameters for the balance, can be ids to substract or add, or null
        /// </summary>
        List<int> _parameters;

        #endregion
       
        #region constructors
        /// <summary>
        /// General constructor, creates the struct and initialize all memembers
        /// </summary>
        /// <param name="type">The type of balance that is done for that stored gas ID</param>
        /// <param name="reference">The gas ID that is used for the balance</param>
        /// <param name="notes">Notes to remember what is that gas reference about</param>
        /// <param name="parameters">Can represent a list of other gases IDs to use as parameters</param>
        public GasBalanceReference(supportedBalanceTypes type, int reference, string notes, List<int> parameters = null)
        {
            _notes = notes;
            _gasRef = reference;
            _type = type;
            _parameters = parameters;
        }
        #endregion

        #region accessors
        /// <summary>
        /// The type of balance that is done for that stored gas ID
        /// </summary>
        public supportedBalanceTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// The gas ID that is used for the balance
        /// </summary>
        public int GasRef
        {
            get { return _gasRef; }
            set { _gasRef = value; }
        }
        /// <summary>
        /// Notes to remember what is that gas reference about
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<int> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
        #endregion
    }
}
