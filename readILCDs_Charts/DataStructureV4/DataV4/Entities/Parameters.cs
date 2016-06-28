using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Stores all the Parameter indexed by their unique ID, so they can be retreived from anywhere in the code using the created database.
    /// </summary>
    public class Parameters : Dictionary<string, Parameter>, IGDataDictionary<string, IParameter>
    {
        #region public methods

        /// <summary>
        /// Creates and adds a new parameter to the model. If the XML definition of the parameter contains an ID already
        /// the uniqueID parameter will not be used. However if the ID already exists in the list of parameters a new one
        /// will be created automatically.
        /// </summary>
        /// <param name="xml">Xml Attribute containing parameter data</param>
        /// <param name="uniqueID">Unique ID for the parameter, otherwise will be automatically created. 
        /// If the XMLAttribute contains an ID this parameter will not be used</param>
        /// <returns>Parameter created, null in case of an error</returns>
        public Parameter CreateRegisteredParameter(XmlAttribute xml, string uniqueID = "1")
        {
            Parameter dv = new Parameter(xml);
            if (dv != null)
            {
                dv.Id = this.FindId(String.IsNullOrEmpty(dv.Id) ? uniqueID : dv.Id);
                this.Add(dv.Id, dv);
            }
            return dv;
        }
        /// <summary>
        /// Creates a new parameter but do not add it to the model. This parameter is not going to have a unique identifier
        /// </summary>
        /// <param name="data">The data is needed for evaluating the formula of the Parameter and refreshing buffers</param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="defaultValue"></param>
        /// <param name="userValue"></param>
        /// <param name="useOriginal"></param>
        /// <returns></returns>
        public Parameter CreateUnregisteredParameter(GData data, string preferedUnitExpression, double defaultValue, double userValue = 0, bool useOriginal = true)
        {
            Parameter dv = new Parameter(preferedUnitExpression, defaultValue, userValue, useOriginal);
            //dv.UpdateBuffers(data);
            return dv;
        }
        /// <summary>
        /// Creates and adds a new parameter to the model 
        /// assigns the the parameter the ID given or a random integer
        /// assigns the CreatedBy CreatedOn ModifiedBy ModifiedOn attributes of the parameter
        /// </summary>
        /// <param name="preferedUnitExpression">The prefered unit for graphical representation</param>
        /// <param name="value">The default (GREET) value in the prefered unit. Enter 100 if the prefed unit is % for a value representing 100%</param>
        /// <param name="uniqueID">Unique ID for the parameter, otherwise will be GUID based</param>
        /// <returns>Parameter created, null in case of an error</returns>
        public Parameter CreateRegisteredParameter(string preferedUnitExpression, double defaultValue, double userValue = 0, string uniqueID = "1")
        {
            Parameter dv = new Parameter(preferedUnitExpression, defaultValue, userValue);
            dv.Id = uniqueID;
            if (dv != null)
            {
                dv.Id = this.FindId(dv.Id);
                this.Add(dv.Id, dv);
            }
            return dv;
        }
       
        public string FindId(string uniqueID)
        {
            if (!String.IsNullOrEmpty(uniqueID) && !this.ContainsKey(uniqueID))
                return uniqueID;
            else
            {
                return Guid.NewGuid().ToString();
            }
        }

        #endregion

        #region IGDataDictionary

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void AddValue(IParameter value)
        {
            this.Add(value.Id, value as Parameter);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IParameter ValueForKey(string key)
        {
            return this[key] as IParameter;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IParameter CreateValue(IData data, int type = 0)
        {
            Parameter parameter = new Parameter();
            return parameter as IParameter;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool KeyExists(string key)
        {
            return this.ContainsKey(key);
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool DeleteValue(IData data, string key)
        {
            if (this.KeyExists(key))
            {
                ToolsDataStructure.RemoveAllParameters(data, this.ValueForKey(key));
                return this.Remove(key);
            }
            else
                return false;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IEnumerable<IParameter> AllValues
        {
            get { return this.Values as IEnumerable<IParameter>; }
        }

        #endregion
    }
}
