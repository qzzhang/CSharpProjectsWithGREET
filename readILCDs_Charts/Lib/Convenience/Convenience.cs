using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace Greet.ConvenienceLib
{
    /// <summary>
    /// Methods used thoughout GREET for various purposes such as XML node manipulations, IDs creation and objects cloning
    /// </summary>
    public static class Convenience
    {
        
        private const string Nfi = "en-US";
        /// <summary>
        /// Returns a new attribute with the name and value set.
        /// </summary>
        /// <param name="xmlDoc">The document to create the attribute from.</param>
        /// <param name="name">The name of the attribute tag that will be displayed in the xml file.</param>
        /// <param name="value">The value of the attribute as an object, value.ToString will be used to convert.</param>
        /// <returns></returns>
        public static XmlAttribute CreateAttr(this XmlDocument xmlDoc, string name, object value, string ci_ = Convenience.Nfi)
        {
            CultureInfo ci = new CultureInfo(ci_);
            XmlAttribute attribute = xmlDoc.CreateAttribute(name);
            if (value == null)
                attribute.Value = "";
            else if (value is IXmlAttr)
                attribute = ((IXmlAttr)value).ToXmlAttribute(xmlDoc, name);
            else if (value is Double)
                attribute.Value = ((Double)value).ToString(ci);
            else if (value is DateTime)
                attribute.Value = ((DateTime)value).ToString(ci);
            else
                attribute.Value = value.ToString();
            return attribute;
        }
        /// <summary>
        /// Returns true if an attribute is not null nor empty
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static bool NotNullNOrEmpty(this XmlAttribute attr)
        {
            if (attr == null || attr.Value == "")
                return false;
            else
                return true;
        }
        /// <summary>
        /// Creates a new element node with the given name and the xmlDoc.NamespaceURI
        /// </summary>
        /// <param name="xmlDoc">The document to create the node from.</param>
        /// <param name="name">The name of the node which will display as a tag in the xml file.</param>
        /// <param name="children">Attributes and Nodes to append to the node.</param>
        /// <returns></returns>
        public static XmlNode CreateNode(this XmlDocument xmlDoc, string name, params XmlNode[] children)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, xmlDoc.NamespaceURI);
            foreach (XmlNode child in children)
            {
                if (child is XmlAttribute)
                    node.Attributes.Append(child as XmlAttribute);
                else
                    node.AppendChild(child);
            }
            return node;
        }
        /// <summary>
        /// Perform a Clone of the object
        /// </summary>
        /// <typeparam name="T">The type of object being cloned.</typeparam>
        /// <param name="RealObject">The object instance to clone.</param>
        /// <returns>The cloned object.</returns>
        public static T Clone<T>(T RealObject)
        {
            // No need to serialize (or clone) null object, simply return null (or the object itself)
            if (Object.ReferenceEquals(RealObject, null))
                return default(T);
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("The type must be marked Serializable", "RealObject");
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
        /// <summary>
        /// Contains sets of methods to create IDs
        /// </summary>
        public static class IDs
        {
            /// <summary>
            /// Gets the first ID starting from starting_value that is not in the array of ids
            /// 
            /// David Dieffenthaler
            /// </summary>
            /// <param name="ids"></param>
            /// <param name="starting_value"></param>
            /// <returns></returns>
            public static int GetIdUnused(IEnumerable<int> ids, int starting_value = 0)
            {
                //faster search for smallest unused ID (array is sorted and then the first empty gap is searched for)
                if (ids.Count() == 0)
                    return starting_value;

                int toReturn = starting_value;
                if (ids.Contains(toReturn))
                    toReturn = ids.Max() + 1;

                return toReturn;
            }

            /// <summary>
            /// Gets the first ID starting from starting_value that is not in the array of ids
            /// 
            /// David Dieffenthaler
            /// </summary>
            /// <param name="ids"></param>
            /// <param name="starting_value"></param>
            /// <returns></returns>
            public static long GetIdUnusedLong(IEnumerable<long> ids, long starting_value = 0)
            {
                //faster search for smallest unused ID (array is sorted and then the first empty gap is searched for)
                if (ids.Count() == 0)
                    return starting_value;

                long toReturn = starting_value;
                if (ids.Contains(toReturn))
                    toReturn = ids.Max() + 1;

                return toReturn;
            }
            /// <summary>
            /// Gets the first ID starting from DateTime.Now that is not in the array of ids
            /// 
            /// David Dieffenthaler
            /// </summary>
            /// <param name="keyCollection"></param>
            /// <returns></returns>
            public static int GetIdUnusedFromTimeStamp(IEnumerable<int> keyCollection)
            {
                int[] ids = new int[0];

                if (keyCollection != null)
                {
                    ids = keyCollection.ToArray();
                }

                return GetIdUnused(ids, ConvertToGREETTimestamp(DateTime.Now));
            }

            /// <summary>
            /// Gets the first ID starting from DateTime.Now that is not in the array of ids
            /// 
            /// David Dieffenthaler
            /// </summary>
            /// <param name="keyCollection"></param>
            /// <returns></returns>
            public static long GetIdUnusedFromTimeStampMillisecond(IEnumerable<long> keyCollection)
            {
                long[] ids = new long[0];

                if (keyCollection != null)
                {
                    ids = keyCollection.ToArray();
                }

                return GetIdUnusedLong(ids, ConvertToGREETTimestampMillisecond(DateTime.Now));
            }
        }

        /// <summary>
        /// Converts DateTime to GREET timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ConvertToGREETTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(2012, 12, 7, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return (int)diff.TotalSeconds;
        }

        /// <summary>
        /// Converts DateTime to GREET timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ConvertToGREETTimestampMillisecond(DateTime date)
        {
            DateTime origin = new DateTime(2012, 12, 7, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - origin;
            return (long)(diff.TotalMilliseconds);
        }

        /// <summary>
        /// Converts GREET timestamp to DateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertFromGREETTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(2012, 12, 7, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Returns the more digits possible for a double
        /// Try to not use because it's kind of slow, expecially the line temp.Contains('E')
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringFull(this double value)
        {
            CultureInfo ci = new CultureInfo("en-US");
            string temp = value.ToString("r", ci);
            if (temp.Contains('E'))
            {
                string[] split = temp.Split('E');
                split[0] = split[0].Replace(".", "");
                bool negative = split[0][0] == '-';
                if (negative)
                    split[0] = split[0].TrimStart('-');

                int eval = Convert.ToInt32(split[1]);

                if (eval > 0)
                {
                    if (split[0].Length - 1 - eval < 0)
                    {
                        for (int i = split[0].Length - 1 - eval; i < 0; i++)
                        {
                            split[0] = split[0] + "0";
                        }
                    }
                    else
                        split[0] = split[0].Insert(eval - 1, ".");
                }
                else
                {
                    for (int i = eval; i < 0; i++)
                    {
                        split[0] = "0" + split[0];
                    }
                    split[0] = split[0].Insert(1, ".");
                }

                if (negative)
                    return "-" + split[0];
                else
                    return split[0];
            }
            return temp;
        }

    }

}
