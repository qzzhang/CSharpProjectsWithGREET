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
namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is a "mask" for the real pathway class which is used by a datagridview to display the right properties
    /// so the PropertyDescriptor of Pathway is not called, because it seems that there is an issue with it...
    /// But the property descriptor is working fine with the PropertyGrid... watch out when modifying the property descriptor or this
    /// http://stackoverflow.com/questions/1048570/binding-to-interface-and-displaying-properties-in-base-interface
    /// </summary>
    internal interface IPathwayForDataGridViewDisplay
    {
        string Name { get; set; }
        int Id { get; set; }
    }
}
