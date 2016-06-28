using System;
using System.ComponentModel;

using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This abstract class regroups the common members and accessors for the EntityTechnologyReference and the CalculatedTechnologyReference<br/>
    /// </summary>
    [Serializable]
    public abstract class TechnologyRef
    {
        #region attributes

        /// <summary>
        /// Reference to the unique Technology ID<br/>
        /// </summary>
        protected int technologyRef;

        /// <summary>
        /// This field is used to display the name of the Technolgy name for the input. This is done as the class doesnt have access to Data object of project.
        /// </summary>
        //Do not delete this field
        public string technologyName;

        /// <summary>
        /// This urban share was here because for transportation processes urban shares can be different for modes<br/>
        /// </summary>
        private LightValue preprocessedUrbanShare;

        public bool accountInBalance = true;

        #endregion attributes

        #region accessors

        /// <summary>
        /// Abstract accessor that returns the current value of the share parameter for a technology reference<br/>
        /// As there are two types of extended classes EntityTechnologyRef and CalculatedTechnologyRef we use<br/>
        /// this accessor to access transparently a representation of an equivalent member of the classes.<br/>
        /// </summary>
        public abstract double ShareValueInDefaultUnit { get; }

        [DisplayName("Technology Reference"), Category("Misc")]
        public int Reference
        {
            get { return technologyRef; }
            set { technologyRef = value; }
        }

        /// <summary>
        /// This urban share was here because for transportation processes urban shares can be different for modes<br/>
        /// </summary>
        public LightValue PreprocessedUrbanShare
        {
            get { return preprocessedUrbanShare; }
            set { preprocessedUrbanShare = value; }
        }

        #endregion

    }
}
