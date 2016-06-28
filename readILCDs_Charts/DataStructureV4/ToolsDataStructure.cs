using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4
{
    public static class ToolsDataStructure
    {
        /// <summary>
        /// Returns a dictionary of all paramters found in an object and their unique IDs
        /// NOTE: All Parameters and their corresponding parents until the root object should be defined as public Accessors instead of public fields.
        /// </summary>
        /// <param name="parameters">An empty dictionary that is going to be populated with the parameters that are found in the RootObject</param>
        /// <param name="parents">An empty list which will be populated during recursive algorithm to avoid infinite loops, just used for recursion</param>
        /// <param name="rootObject">The object from which we are trying to extract all parameter, goes though all Public members and accessors to find them</param>
        /// <param name="maxLvl">Max level is a safety against infinite loop or to stop when the object gets really deep</param>
        public static void FindAllParameters(ref Dictionary<string, Parameter> parameters, List<object> parents, object rootObject, int maxLvl = 25)
        {
            if (rootObject is IEnumerable)
            {
                foreach (object valueIn in (rootObject as IEnumerable))
                {
                    if (valueIn != null)
                        FindAllParameters(ref parameters, parents, valueIn, maxLvl);
                }
            }

            Type t = rootObject.GetType();
            List<PropertyInfo> l = new List<PropertyInfo>();
            while (t != typeof(object))
            {//recusive search for base types (if class extends from some other abstract class)
                l.AddRange(t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                t = t.BaseType;
            }

            foreach (var item in l)
            {
                if (item.CanRead && item.GetIndexParameters().Length == 0)
                {
                    object child = item.GetValue(rootObject, null);
                    if (child != null)
                    {
                        Type ct = child.GetType();
                        if (!ct.IsPrimitive && ct != typeof(String)
                            && ct != typeof(Double[,]) && ct != typeof(Double) && ct != typeof(Boolean[,]))
                        {
                            if (child is Parameter && !parameters.ContainsKey((child as Parameter).Id))
                                parameters.Add((child as Parameter).Id, child as Parameter);
                            else if (parents.Count < maxLvl && rootObject != child && !parents.Contains(child))
                            {
                                List<object> par = new List<object>();
                                par.AddRange(parents);
                                par.Add(rootObject);
                                if (child is IEnumerable)
                                {
                                    foreach (object valueIn in (child as IEnumerable))
                                    {
                                        if (valueIn != null)
                                            FindAllParameters(ref parameters, par, valueIn, maxLvl);
                                    }
                                }
                                else if (child != null)
                                    FindAllParameters(ref parameters, par, child, maxLvl);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Searches recursively though an object for all instances of parameters. Then replaces the instances
        /// of parameters in the master list of parameter with these newly founded ones.
        /// </summary>
        /// <param name="parameters">Mast list of parameters in which instances of parameters are going to be updated</param>
        /// <param name="obj">Object containing cloned parameters instances</param>
        /// <returns>List of parameters ID that have been modified (Parameters.Equals(Object obj) or added to the master list</returns>
        public static List<string> UpdateAllParameters(Parameters parameters, object obj)
        {
            List<string> modified = new List<string>();
            Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
            FindAllParameters(ref parameterList, new List<object>(), obj);

            foreach (KeyValuePair<string, Parameter> pair in parameterList)
            {
                if (parameters.ContainsKey(pair.Key))
                {//update existing instance of a parameter
                    if (!parameters[pair.Key].Equals(pair.Value))
                        modified.Add(pair.Key);

                    parameters.Remove(pair.Key);
                    parameters.Add(pair.Key, pair.Value);
                }
                else
                {//add the parameter to the master list as we couldn't find it there
                    parameters.Add(pair.Key, pair.Value);
                    modified.Add(pair.Key);
                }
            }

            return modified;
        }
        /// <summary>
        /// Searches recursively though an object for all instances of parameters. 
        /// Then insterts them in the master list and change their IDs if the ID is already present in the master list
        /// </summary>
        /// <param name="parameters">Mast list of parameters in which instances of parameters are going to be added with different IDs</param>
        /// <param name="obj">Object containing cloned parameters instances that needs to have new IDs</param>
        /// <returns>List of parameters ID that have been modified (Parameters.Equals(Object obj) or added to the master list</returns>
        public static List<string> RenameAllParameters(Parameters parameters, object obj)
        {
            List<string> modified = new List<string>();
            Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
            FindAllParameters(ref parameterList, new List<object>(), obj);

            try
            {
                foreach (KeyValuePair<string, Parameter> pair in parameterList)
                {
                    pair.Value.Id = parameters.FindId(pair.Key);
                    parameters.Add(pair.Value.Id, pair.Value);
                    modified.Add(pair.Value.Id);
                }
            }
            catch
            {

            }

            return modified;
        }
        
        /// <summary>
        /// Searches for parameters IDs in the object upto a depth of 15 and removes the parameters with the same ID from the master list of parameters.
        /// </summary>
        /// <param name="data">Data from which the parameters should be removed</param>
        /// <param name="obj">Object from which parameters should be found</param>
        /// <param name="maxLvl">Max level is a safety against infinite loop or to stop when the object gets really deep</param>
        /// <returns>Number of parameters that are removed</returns>
        public static int RemoveAllParameters(IData data, object obj, int maxLvl = 15)
        {
            int i = 0;
            Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
            FindAllParameters(ref parameterList, new List<object>(), obj, maxLvl);
            foreach (KeyValuePair<string, Parameter> pair in parameterList)
            {
                (data as GData).ParametersData.Remove(pair.Key);
                i++;
            }
            return i;
        }
        /// <summary>
        /// Converts a number in base 26 [A-Z]* or [a-z]* into one in base 10;
        /// </summary>
        /// <param name="col_string"></param>
        /// <returns></returns>
        internal static int ColumnLettersToInt(string col_string)
        {
            double pow = 0;
            int value = 0;
            foreach (char c in col_string.ToUpper())
            {
                value += (int)((double)((int)c - 64) * Math.Pow(26, pow));
                pow++;
            }
            return value;
        }
    }
}
