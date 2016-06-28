/*********************************************************************** 
COPYRIGHT NOTIFICATION 

Email contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 

************************************************************************ 
ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
ENERGY. 
************************************************************************
 
***********************************************************************/ 
using System;
using System.Xml;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDCarEmissionValue
    {
        #region attributes

        /// <summary>
        /// Stores the calculated and non calculated values for the emissions as well as  the unit
        /// </summary>
        private Parameter param;

        /// <summary>
        /// Flag to specify if that emission is a balance and will need to be calculated
        /// </summary>
        private bool canCalculated;

        /// <summary>
        /// Integer represents the last time this thing has been calculated ( if it has been )
        /// </summary>
        private int hasBeenCalculatedOnRun = -1;

        /// <summary>
        /// Flag true if those emissions are based on a other technology emission factor.
        /// Used in technologies editor.
        /// </summary>
        public bool isBased = false;

        #endregion attributes

        public V3OLDCarEmissionValue(GData data, XmlAttribute xmlstring, bool canBeCalculated = false, string optionalValueId = "")
        {
            param = data.ParametersData.CreateRegisteredParameter(xmlstring, optionalValueId);
            this.canCalculated = canBeCalculated;
        }

        public V3OLDCarEmissionValue(GData data, string unitOrGroup, double value, bool canBeCalculated = false, string optionalValueId = "")
        {
            param = data.ParametersData.CreateRegisteredParameter(unitOrGroup, value, 0, optionalValueId);
            this.canCalculated = canBeCalculated;
        }

        #region Accessors
        /// <summary>
        /// Flag to specify if that emission is a balance and will need to be calculated
        /// </summary>
        public bool CanCalculated
        {
            get { return canCalculated; }
            set { canCalculated = value; }
        }
        /// <summary>
        /// Stores the calculated and non calculated values for the emissions as well as  the unit
        /// </summary>
        public Parameter EmParameter
        {
            get { return param; }
            set { param = value; }
        } /// <summary>
        /// Flag to specify if we are using the default ( or calculated ) greet value, or if we are using the override value
        /// </summary>
        public bool useCalculated
        {
            get { return this.EmParameter.UseOriginal; }
            set
            {
                this.EmParameter.UseOriginal = value;
            }
        }


        /// <summary>
        /// Integer represents the last time this thing has been calculated ( if it has been )
        /// </summary>
        public int HasBeenCalculatedOnRun
        {
            get { return hasBeenCalculatedOnRun; }
            set { hasBeenCalculatedOnRun = value; }
        }


        #endregion
    }
}
