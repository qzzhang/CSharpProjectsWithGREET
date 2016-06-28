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
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// <para>This class represents a generic time series.</para>
    /// <para>The integer key represent a given year such as 1992 or 2005</para>
    /// <para>We always look backwards to find the closets year available in the time series, therefore 0 can be assimilated as the 'default' year for time series</para>
    /// </summary>
    [Serializable]
    public class TimeSeries<Tvalue> : Series<int, Tvalue>
    {
        #region attributes

        Tvalue _value = default(Tvalue);
        /// <summary>
        /// The most recent data collected from reports or other sources
        /// After that the future values are estimates (forecasted values)
        /// </summary>
        public int _mostRecentData = 0;
        /// <summary>
        /// Notes associated with the TimesSeries data
        /// </summary>
        public string _notes = "";

        #endregion attributes

        #region constructors

        public TimeSeries()
            : base()
        {

        }

        protected TimeSeries(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {
            _value = (Tvalue)information.GetValue("value", typeof(Tvalue));
            _notes = information.GetString("description");
            _mostRecentData = information.GetInt32("mostRecent");
        }

        #endregion constructors

        #region accessors

        /// <summary>
        /// This accessor attempts to the value for the current year. 
        /// If the TS does not has a value defined for the current year an exception is thrown
        /// </summary>
        [Browsable(false)]
        public Tvalue CurrentValue
        {
            get
            {
                return this.Value((int)BasicParameters.SelectedYear.ValueInDefaultUnit);
            }
        }

        /// <summary>
        /// Returns the value for the closest year prior in the time series to the given year
        /// If the desired year cannot be found, uses the one right after
        /// Throws an exception if no years are available in the time series
        /// 
        /// </summary>
        /// <param name="year">Year to seek for</param>
        /// <returns>Returns a value, or throws an exception if no years are available in the time series</returns>
        public Tvalue Value(int year)
        {
            if (year == -1)
                year = _mostRecentData;

            if (this.Keys.Contains(year))
                _value = this[year];
            else if (this.Count >= 1) //try to find the closer value in the dictionary
            {
                //finding the closer
                int closestYear = CurrentYear(year);
                if (this.ContainsKey(closestYear))
                    _value = this[closestYear];
                else
                    throw new Exception("No data for the year " + year);
            }

            return _value;
        }

        /// <summary>
        /// Given a specific year, returns the closest year that preceeds or equals the year desired
        /// If the desired year cannot be found, uses the one right after
        /// If no years are defined in the time series, returns -1
        /// If the year seeked for is -1, then we look for the '_mostRecentData' attribute in order to get the year corresponding to a data collected value and not a forecasted one.
        /// </summary>
        /// <param name="targetYear">Desired year</param>
        /// <returns>Available year according to the current collection, or -1 if no years available in the time series</returns>
        public int CurrentYear(double targetYear)
        {
            if (targetYear == -1)
                targetYear = _mostRecentData;

            int closestBefore = Int32.MaxValue;
            int closestAfter = Int32.MinValue;
            foreach (int test in this.Keys)
            {
                double diff = targetYear - test;
                if (diff >= 0)
                {//tested value is before 
                    if (Math.Abs(targetYear - closestBefore) > Math.Abs(diff))
                        closestBefore = test;
                }
                else
                {//tested value is after
                    if (Math.Abs(targetYear - closestAfter) > Math.Abs(diff))
                        closestAfter = test;
                }
            }
            if (closestBefore != Int32.MaxValue)
                return closestBefore;
            else if (closestAfter != Int32.MinValue)
                return closestAfter;
            else
                return -1;
        }

   
        /// <summary>
        /// Returns the closest year that preceeds or equals the current year - p
        /// If the desired year cannot be found, uses the one right after
        /// Throws an exception if no years are available in the time series
        /// </summary>
        /// <param name="p">Lag value</param>
        /// <returns>Returns a value, or throws an exception if no years are available in the time series<</returns>
        public Tvalue LaggedValue(int p)
        {
            return this.Value((int)BasicParameters.SelectedYear.ValueInDefaultUnit - p);
        }

        #endregion accessors

        #region method

        public override void GetObjectData(SerializationInfo info,
                            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("value", _value, typeof(Tvalue));
            info.AddValue("mostRecent", _mostRecentData);
            info.AddValue("description", _notes);
        }

        #endregion
    }
}
