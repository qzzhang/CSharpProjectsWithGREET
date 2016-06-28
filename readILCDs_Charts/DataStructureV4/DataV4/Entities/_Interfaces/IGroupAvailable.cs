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
using System.Collections.Generic;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This interface specify memebers for an item that uses groups and that can be 
    /// edited from the GroupEditor control. That way we can pass lists of IGroupsAvailable
    /// to the control instead of creating a new control for each type of object that has memberships and groups
    /// </summary>
    public interface IGroupAvailable
    {
        /// <summary>
        /// Get or Set the list of memberships for that object
        /// </summary>
        List<int> Memberships { get; set; }
        /// <summary>
        /// The ID of the currently edited object
        /// </summary>
        int Id { get;  }
    }
}
