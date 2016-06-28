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
using System.Runtime.Serialization;


namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// This class is a base class for TechnologyData class (Technologies file) and the V3OLDV3OLDVehicleOperationalMode class (V3OLDVehicle data)
    /// It contains a TimeSeries object which holds different TechnologyEmissionsFactors for each year
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDCarEmissionsTimeSeries : TimeSeries<V3OLDCarYearEmissionsFactors>
    {
        #region attributes

        

        #endregion attributes

        #region accessors
       

        #endregion accessors

        public V3OLDCarEmissionsTimeSeries()
        {
        }

        protected V3OLDCarEmissionsTimeSeries(SerializationInfo information, StreamingContext context)
            : base(information, context)
        {
            
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void GetObjectData(SerializationInfo info,
                       StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }
}
