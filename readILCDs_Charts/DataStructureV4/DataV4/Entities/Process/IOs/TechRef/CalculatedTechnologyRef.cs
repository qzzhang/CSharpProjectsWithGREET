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

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is intended to be used for "created" or "calculated" technologies when we convert the processes<br/>
    /// to their canonical form. This class help us preveting the creation of unecessary registered parameters for the<br/>
    /// share of each technology references. Here the share is simply as double as calculated by the conversion methods<br/>
    /// </summary>
    [Serializable]
    public class CalculatedTechnologyRef : TechnologyRef
    {
        #region attributes

        private double share;

        #endregion attributes

        #region constructors

        public CalculatedTechnologyRef()
        {
            
        }

        public CalculatedTechnologyRef (int technologyId, double share) : this()
        {
            this.share = share;
            this.technologyRef = technologyId;
        }

        #endregion constructors

        #region accessors

        public override double ShareValueInDefaultUnit
        {
            get { return Share; }
        }

        public double Share
        {
            get { return share; }
            set { share = value; }
        }
        #endregion accessors


    }
}
