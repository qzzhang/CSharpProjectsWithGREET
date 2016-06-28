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

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Greet.Lib.Scenarios
{

    public class Scenario
    {
        #region Fields and Constants

        /// <summary>
        /// IDs are prefered because they allow to rename scenarios without worrying about the results storage (they are stored by ID)
        /// however we use name to distinguish unique scenarios and associate them witha shapefile because GUIDs are never stored in the CSV file
        /// so this is a halfway solution that should be changed at some point
        /// </summary>
        Guid _id;

        string _layerName = "";
        string _name = "";
        List<ScenarioQuantityUse> _scenarioQuantityUse = new List<ScenarioQuantityUse>();
        string _shapeFileFeatureAttributeName = "";
        string _shapeFileFeatureAttributeValue = "";
        List<ValueModification> _valueModifications = new List<ValueModification>();

        #endregion

        #region Constructors

        public Scenario()
        {
            _id = Guid.NewGuid();
        }

        public Scenario(String name) : this()
        {
            _name = name;
        }

        #endregion

        #region Properties and Indexers

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string LayerName
        {
            get
            {
                return _layerName;
            }

            set
            {
                _layerName = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ShapeFileFeatureAttributeName
        {
            get
            {
                return _shapeFileFeatureAttributeName;
            }

            set
            {
                _shapeFileFeatureAttributeName = value;
            }
        }

        public string ShapeFileFeatureAttributeValue
        {
            get
            {
                return _shapeFileFeatureAttributeValue;
            }

            set
            {
                _shapeFileFeatureAttributeValue = value;
            }
        }

        public List<ValueModification> ValueModifications
        {
            get { return _valueModifications; }
            set { _valueModifications = value; }
        }

        #endregion

        #region Members

        public Scenario Clone()
        {
            Scenario scn = new Scenario
            {
                Name = _name,
                Id = _id,
                LayerName = _layerName,
                ShapeFileFeatureAttributeName = _shapeFileFeatureAttributeName,
                ShapeFileFeatureAttributeValue = _shapeFileFeatureAttributeValue
            };

            foreach (ValueModification val in ValueModifications)
                scn.ValueModifications.Add(val.Clone());
            foreach (ScenarioQuantityUse qty in _scenarioQuantityUse)
                scn._scenarioQuantityUse.Add(qty.Clone());


            return scn;
        }

        /// <summary>
        /// Returns a SHA256 based on the name of the scenario, parameter ID, values and units
        /// This is used in order to know if a scenario has been modified since the results for it have been calculated
        /// </summary>
        /// <returns></returns>
        public string GetSHAState()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(_name);
            foreach (ValueModification v in _valueModifications)
            {
                sb.Append(v.ParameterID);
                sb.Append(v.NewUserValue);
                sb.Append(v.NewExpression);
            }
            foreach (ScenarioQuantityUse s in _scenarioQuantityUse)
            {
               
            }
            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            SHA256 mySHA256 = SHA256.Create();
            byte[] hashValue = mySHA256.ComputeHash(buffer);
            string hashString = string.Empty;
            foreach (byte x in hashValue)
                hashString += String.Format("{0:x2}", x);
            return hashString;
        }

        public override string ToString()
        {
            return this._name;
        }

        #endregion
    }
}
