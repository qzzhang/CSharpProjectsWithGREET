// *********************************************************************** 
//  COPYRIGHT NOTIFICATION 
// 
//  Email contact: greet@anl.gov 
//  Copyright (c) 2012, UChicago Argonne, LLC 
//  All Rights Reserved
//  
//  THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
//  LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
//  CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
//  NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 
//  
//  ************************************************************************ 
//  ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
//  IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
//  ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
//  ENERGY. 
//  ************************************************************************
//   
//  ***********************************************************************/
namespace Greet.Lib.Scenarios
{
    public class ValueModification
    {
        #region Fields and Constants

        string _newExpression = "";
        double _newUserValue;
        string _notes = "";
        string _parameterID = "";
        string _parentInfo = "";

        #endregion

        #region Constructors

        public ValueModification()
        {
        }

        public ValueModification(string parameterID, string parameterUnit, string parameterValue, string parentInfo)
        {
            // TODO: Complete member initialization
            _parameterID = parameterID;
            _newExpression = parameterUnit;
            _parentInfo = parentInfo;
            double.TryParse(parameterValue, out _newUserValue);
        }

        #endregion

        #region Properties and Indexers

        public string NewExpression
        {
            get { return _newExpression; }
            set { _newExpression = value; }
        }

        public double NewUserValue
        {
            get { return _newUserValue; }
            set { _newUserValue = value; }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public string ParameterID
        {
            get { return _parameterID; }
            set { _parameterID = value; }
        }

        public string ParentInfo
        {
            get { return _parentInfo; }
            set { _parentInfo = value; }
        }

        #endregion

        #region Members

        public ValueModification Clone()
        {
            return new ValueModification
            {
                ParameterID = ParameterID,
                NewUserValue = NewUserValue,
                NewExpression = NewExpression,
                Notes = Notes,
                ParentInfo = ParentInfo
            };
        }

        #endregion
    }
}
