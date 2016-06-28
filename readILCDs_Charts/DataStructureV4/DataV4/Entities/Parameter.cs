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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// A class used to store a parameter of an entity for the GREET model. Can be displayed in a DBTextBox in the GUI.
    /// All Parameters must have a unique ID and be registered to the list of Parameters in the database.
    /// Parameters should never be created as the result of an formula, if a result is necessary use LightValue instead.
    /// </summary>
    [DescriptionAttribute("Expand to see the values."),
    Serializable]
    public class Parameter : ISerializable, IComparable, IParameter, IHaveMetadata, IXmlAttr
    {
        #region delegates

        public delegate void ValueChangedDelegate();
        [NonSerialized]
        public ValueChangedDelegate ValueChanged;

        #endregion

        #region attributes

        protected virtual void OnEvent(EventArgs e)
        {
            if (ValueChanged != null) ValueChanged();
        }
        /// <summary>
        /// Stores the actual value in default unit for the ValueInDefualtUnit accessesor
        /// This is used to keep the calculation pretty fast and don't parse strings everytime
        /// </summary>
        private double _bufferForGreetValueInDefaultUnit = 0;
        /// <summary>
        /// The quantity used to store the GREET value in SI units
        /// WARNING: Change only if you have full understanding of how this attribute is used!
        /// </summary>
        public uint _greetValueDim = 0;
        /// <summary>
        /// The unit the user prefers to see when displaying the GREET value for that parameter
        /// WARNING: Change only if you have full understanding of how this attribute is used!
        /// </summary>
        public string _greetValuePreferedUnitExpression = "";
        /// <summary>
        /// Stores the actual value in default unit for the ValueInDefualtUnit accessesor
        /// This is used to keep the calculation pretty fast and don't parse strings everytime
        /// </summary>
        private double _bufferForUserValueInDefaultUnit = 0;
        /// <summary>
        /// The quantity used to store the GREET value in SI units
        /// </summary>
        private uint _userValueDim = 0;
        /// <summary>
        /// The unit the user prefers to see when displaying the GREET value for that parameter
        /// </summary>
        private string _userValuePreferedExpression = "";
        /// <summary>
        /// The value originally stored in this double value from the database or a default value
        /// </summary>
        internal string _greetFormulaString = "";
        /// <summary>
        /// The user defined value that can be used instead of the original value.
        /// </summary>
        internal string _userFormulaString = "";
        /// <summary>
        /// Determines whether to use original or overriden value 
        /// </summary>
        private bool _useGreet = true;
        /// <summary>
        /// A description text string to be used in labels for this doublevalue. Ex. Share: 
        /// </summary>
        private string _notes = "";
        /// <summary>
        /// Unique ID among all the parameters, used to track each parameter for the stochastic simulations or reporting
        /// </summary>
        private string _id = "";
        /// <summary>
        /// Unique Name among all the parameters, used as Alias to each parameter for the stochastic simulations or reporting
        /// </summary>
        private string _name = "";
        /// <summary>
        /// If used:
        /// Tracer in order to avoid parsing the formulas for the same parameter multiple times (saves lot of calculation time)
        /// </summary>
        private Guid _tracer = new Guid();

        #endregion attributes

        #region constructors

        public Parameter()
        {

        }

        internal void UpdateFrom(Parameter n)
        {
            _bufferForGreetValueInDefaultUnit = n._bufferForGreetValueInDefaultUnit;
            _greetValueDim = n._greetValueDim;
            _greetValuePreferedUnitExpression = n._greetValuePreferedUnitExpression;
            _bufferForUserValueInDefaultUnit = n._bufferForGreetValueInDefaultUnit;
            _userValueDim = n._userValueDim;
            _userValuePreferedExpression = n._userValuePreferedExpression;
            _greetFormulaString = n._greetFormulaString;
            _userFormulaString = n._userFormulaString;
            _notes = n._notes;
            _id = n._id;
            _name = n._name;
            ModifiedBy = n.ModifiedBy;
            ModifiedOn = n.ModifiedOn;
        }
        
       /// <summary>
       /// Compares all members except the buffers. Return true if all members compared are the equals.
       /// </summary>
       /// <param name="obj"></param>
       /// <returns>True if objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Parameter))
                return false;

            Parameter n = obj as Parameter;
            return
            _greetValueDim == n._greetValueDim &&
            _greetValuePreferedUnitExpression == n._greetValuePreferedUnitExpression &&
            _userValueDim == n._userValueDim &&
            _userValuePreferedExpression == n._userValuePreferedExpression &&
            _greetFormulaString == n._greetFormulaString &&
            _userFormulaString == n._userFormulaString &&
            _notes == n._notes &&
            _id == n._id &&
            _name == n._name &&
            ModifiedBy == n.ModifiedBy &&
            ModifiedOn == n.ModifiedOn;
        }

        /// <summary>
        /// Creats a DoubleValue with param set based on paramName, and Values set to value
        /// This following code is based on the other constructor :  public DoubleValue(string str, bool add_to_master_list)
        /// but transforming the parameter value which is provided here into a string to reuse that other constructor was making the calculations running 2%
        /// slower, so the following code is a simplified version of the other one.
        /// </summary>
        /// <param name="preferedUnitExpression">The prefered unit for graphical representation</param>
        /// <param name="GREETValue">The default (GREET) value in the prefered unit. Enter 100 if the prefed unit is % for a value representing 100%</param>
        /// <param name="USERValue">The user value in the prefered unit. Enter 100 if the prefed unit is % for a value representing 100%</param>
        /// <param name="useGREET">If true use the original GREET value, otherwise use the user value</param>
        internal Parameter(string preferedUnitExpression, double GREETValue, double USERValue = 0, bool useGREET = true)
        {
            string siExp; string filteredUserExpression; uint eqDim; double slope, intercept;
            GuiUtils.FilterExpression(preferedUnitExpression, out siExp, out filteredUserExpression, out eqDim, out slope, out intercept);
            _greetValuePreferedUnitExpression = filteredUserExpression;
            _greetValueDim = _userValueDim = eqDim;
            _bufferForGreetValueInDefaultUnit = GREETValue * slope + intercept;
            _bufferForUserValueInDefaultUnit = USERValue * slope + intercept;

            _useGreet = useGREET;
        }

        internal Parameter(XmlAttribute attribute)
        {
            FromString(attribute.Value);
        }

        /// <summary>
        /// Constructor for derserialisation, avoid cloning the unit group
        /// see the GetObjectData method
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public Parameter(SerializationInfo info, StreamingContext text)
            : this()
        {
            _greetFormulaString = info.GetString("original_value_string");
            _bufferForGreetValueInDefaultUnit = info.GetDouble("bufferoriginal");
            _bufferForUserValueInDefaultUnit = info.GetDouble("bufferuser");
            _userFormulaString = info.GetString("override_value_string");
            _useGreet = info.GetBoolean("use_original");
            _notes = info.GetString("notes");
            _greetValueDim = info.GetUInt32("greetDim");
            _userValueDim = info.GetUInt32("userDim");
            _greetValuePreferedUnitExpression = info.GetString("greetPrefUnit");
            _userValuePreferedExpression = info.GetString("userPrefUnit");
            ModifiedBy = info.GetString("modifiedby");
            ModifiedOn = info.GetString("modifiedon");
            _id = info.GetString("id");
            Name = info.GetString("name");
        }

        /// <summary>
        /// Serializer, serialise everything we do that because we want to override the deserializer which clones the unitgroup
        /// we dont want to clone the unit group so basically we specify how to serialize, and how to deserialize
        /// See Deserializer public Parameter(SerializationInfo info, StreamingContext text)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("original_value_string", _greetFormulaString);
            info.AddValue("bufferoriginal", _bufferForGreetValueInDefaultUnit);
            info.AddValue("bufferuser", _bufferForUserValueInDefaultUnit);
            info.AddValue("override_value_string", _userFormulaString);
            info.AddValue("use_original", _useGreet);
            info.AddValue("notes", _notes);
            info.AddValue("greetDim",_greetValueDim);
            info.AddValue("userDim",_userValueDim);
            info.AddValue("greetPrefUnit",_greetValuePreferedUnitExpression);
            info.AddValue("userPrefUnit", _userValuePreferedExpression);
            info.AddValue("modifiedby", ModifiedBy);
            info.AddValue("modifiedon", ModifiedOn);
            info.AddValue("id", _id);
            info.AddValue("name", _name);
        }

        /// <summary>
        /// Should only be called for building from an XMLAttribute value
        // 1: Greet value in SI unit
        // 2: Greet value prefered unit (used to determine quantity as well)
        // 3: User value in SI unit
        // 4: User value prefered unit (used to determine quantity as well)
        // 3: Boolean (which one to use)
        // 5: Notes
        // 6: Id (unique ID of the parameter)
        // 7: Created by
        // 8: Created on
        // 9: Name (to override ID with a meaningfull name in formulas)
        /// </summary>
        /// <param name="str"></param>
        private void FromString(String str)
        {
            if (String.IsNullOrEmpty(str) == false)
            {
                string[] double_value_string_split = str.Split(';');

                if (double_value_string_split.Length >= 9)
                {
                    //now we split the default and override values string, because they might contain some maring of error or some significant digits informations
                    string[] greet_val_split = double_value_string_split[0].Split("#".ToCharArray());
                    string[] user_val_split = double_value_string_split[2].Split("#".ToCharArray());

                    if (greet_val_split[0].Contains("["))
                        _greetFormulaString = greet_val_split[0];
                    else
                        _bufferForGreetValueInDefaultUnit = MathParse.Parse(greet_val_split[0]).Value;

                    string siExp; string filteredUserExpression; uint eqDim; double slope, intercept;
                    GuiUtils.FilterExpression(double_value_string_split[1], out siExp, out filteredUserExpression, out eqDim, out slope, out intercept);
                    _greetValuePreferedUnitExpression = filteredUserExpression;
                    _greetValueDim = eqDim;

                    if (user_val_split[0].Contains("["))
                        _userFormulaString = user_val_split[0];
                    else
                        _bufferForUserValueInDefaultUnit = MathParse.Parse(user_val_split[0]).Value;

                    GuiUtils.FilterExpression(double_value_string_split[3], out siExp, out filteredUserExpression, out eqDim, out slope, out intercept);
                    _userValuePreferedExpression = filteredUserExpression;
                    _userValueDim = eqDim;

                    //read the use_default attribute
                    _useGreet = Convert.ToBoolean(double_value_string_split[4]);

                    if (double_value_string_split.Length >= 5)
                        _notes = double_value_string_split[5];
                    if (double_value_string_split.Length >= 6)
                        _id = double_value_string_split[6];
                    if (double_value_string_split.Length >= 7)
                        ModifiedBy = double_value_string_split[7];
                    if (double_value_string_split.Length >= 8)
                        ModifiedOn = double_value_string_split[8];
                    if (double_value_string_split.Length >= 9)
                        _name = double_value_string_split[9];
                }
                else
                    throw new Exception("This value is not correctly defined, we cannot have only two fields separated by semi columns to define a double value");
            }
        }

        #endregion constructors

        #region accessors
        /// <summary>
        /// Unique ID among all the parameters, used to track each parameter for the stochastic simulations or reporting
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Unique Name among all the parameters, used as Alias to each parameter for the stochastic simulations or reporting
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value.Replace(";", ""); }
        }

        /// <summary>
        /// Returns the dimension of the current value (depends on the UseOriginal boolean flag)
        /// </summary>
        [Browsable(false)]
        public uint Dim
        {
            get 
            {
                if (UseOriginal)
                    return _greetValueDim;
                else
                    return _userValueDim;
            }
        }

        /// <summary>
        /// Returns the current dim for the user value.
        /// WARNING : Setting this will also replace the UserValueQuantityName without perfoming any conversion on the value. The quantity will be selected based on the new prefered unit
        /// </summary>
        public uint UserDim
        {
            get { return _userValueDim; }
            set { _userValueDim = value; }
        }

        /// <summary>
        /// Returns either the GREET or User quantity name depending on the UseOriginal boolean flag
        /// If no quantity can be found that corresponds to the Dim, then an empty string is returned.
        /// </summary>
        [Obsolete("Dim should be used instead of that attributed only here to satisfy the older API")]
        public string UnitGroupName
        {
            get
            {
                AQuantity qty = Units.QuantityList.ByDim(Dim);
                if (qty != null)
                    return qty.Name;
                else
                    return "";
            }
        }

        /// <summary>
        /// Returns the current prefered unit of the user value.
        /// </summary>
        public string GreetValuePreferedExpression
        {
            get
            {
                return _greetValuePreferedUnitExpression;
            }
        }

        /// <summary>
        /// Returns the current prefered unit of the user value.
        /// WARNING : Setting this will also replace the UserValueQuantityName without perfoming any conversion on the value. The quantity will be selected based on the new prefered unit
        /// </summary>
        public string UserValuePreferedExpression
        {
            get 
            {
                return _userValuePreferedExpression;
            }
            set 
            {
                _userValuePreferedExpression = value;
                _userValueDim = DimensionUtils.FromString(value);
            }
        }

        public string ValuePreferedUnitExpression
        {
            get
            {
                if (_useGreet)
                    return _greetValuePreferedUnitExpression;
                else
                    return _userValuePreferedExpression;
            }
            set 
            {
                if (_useGreet)
                    _greetValuePreferedUnitExpression = value;
                else
                    _userValuePreferedExpression = value;
            }
        }

        /// <summary>
        /// <para>Get the GREET or User value depending on the UseOriginal flag</para>
        /// <para>Sets the User value and change to false the UseOriginal flag</para>
        /// </summary>
        [Browsable(false)]
        public double ValueInDefaultUnit
        {
            get
            {
                if (_useGreet)
                    return _bufferForGreetValueInDefaultUnit;
                else
                    return _bufferForUserValueInDefaultUnit;
            }
            set
            {
                UseOriginal = false;

                _bufferForUserValueInDefaultUnit = value;

                if (ValueChanged != null)
                    ValueChanged();
            }
        }

        /// <summary>
        /// Converts default Value to the unit defined in the preferences and returns it as a double. Setting it does the back conversion
        /// </summary>
        [Browsable(false)]
        public virtual double GreetValueInOverrideUnitDouble
        {
            get
            {
                AQuantity qty = Units.QuantityList.ByDim(_greetValueDim);
                int idxPreferedUnit = qty.PreferedUnitIdx;
                Unit preferedUnit = qty.Units[idxPreferedUnit];

                return AQuantity.ConvertFromSIToSpecific(_bufferForGreetValueInDefaultUnit, preferedUnit.Expression);
            }
            set
            {
                AQuantity qty = Units.QuantityList.ByDim(_greetValueDim);
                int idxPreferedUnit = qty.PreferedUnitIdx;
                Unit preferedUnit = qty.Units[idxPreferedUnit];

                _bufferForGreetValueInDefaultUnit = AQuantity.ConvertFromSpecificToSI(value, preferedUnit.Expression);
            }
        }

        /// <summary>
        /// Converts User Value to the unit defined in the preferences and returns it as a double. Setting it does the back conversion
        /// </summary>
        [Browsable(false)]
        public virtual double UserValueInPreferedUnitDouble
        {
            get
            {
                AQuantity qty = Units.QuantityList.ByDim(_greetValueDim);
                int idxPreferedUnit = qty.PreferedUnitIdx;
                Unit preferedUnit = qty.Units[idxPreferedUnit];

                return AQuantity.ConvertFromSIToSpecific(_bufferForUserValueInDefaultUnit, preferedUnit.Expression);
            }
            set
            {
                AQuantity qty = Units.QuantityList.ByDim(_greetValueDim);
                int idxPreferedUnit = qty.PreferedUnitIdx;
                Unit preferedUnit = qty.Units[idxPreferedUnit];

                _bufferForUserValueInDefaultUnit = AQuantity.ConvertFromSpecificToSI(value, preferedUnit.Expression);
            }
        }

        /// <summary>
        /// Outputs the value in a user specified unit, the unis has to be in the same unit group as the current unit group of that doubleValue
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public virtual double ValueInSpecificUnit(Unit unit)
        {
            return AQuantity.ConvertFromSIToSpecific(ValueInDefaultUnit, unit.Expression);
        }

        public string CurrentFormula
        {
            get
            {
                if (_useGreet)
                    return _greetFormulaString;
                else
                    return _userFormulaString;
            }
            set
            {
                _useGreet = false;
                double temp;
                string beforeChange = CurrentFormula;
                if (Double.TryParse(value, out temp))
                {
                    _bufferForUserValueInDefaultUnit = temp;
                    _userFormulaString = "";
                }
                else
                    _userFormulaString = value;

                if (ValueChanged != null && beforeChange != _userFormulaString)
                    ValueChanged();
            }
        }

        /// <summary>
        /// The value originally stored in this double value from the database or a default value
        /// </summary>
        [Browsable(true), ReadOnlyAttribute(true), DisplayName("Original")]
        public double GreetValue
        {
            get
            {
                return _bufferForGreetValueInDefaultUnit;
            }
            set
            {
                //commenting this line out will save a lot of time as we are converting a double to string for nothing in most of the cases
                double beforeChange = _bufferForGreetValueInDefaultUnit;
                _bufferForGreetValueInDefaultUnit = value;
                if (ValueChanged != null && beforeChange != _bufferForGreetValueInDefaultUnit && _useGreet)
                    ValueChanged();
            }
        }

        /// <summary>
        /// The user defined value that can be used instead of the original value.
        /// </summary>
        [Browsable(true), ReadOnlyAttribute(false), DisplayName("Override")]
        public double UserValue
        {
            get
            {
                return _bufferForUserValueInDefaultUnit;
            }
            set
            {
                //commenting this line out will save a lot of time as we are converting a double to string for nothing in most of the cases.
                double beforeChange = _bufferForUserValueInDefaultUnit;
                _bufferForUserValueInDefaultUnit = value;
                _userFormulaString = "";
                if (ValueChanged != null && beforeChange != _bufferForGreetValueInDefaultUnit && !_useGreet)
                    ValueChanged();
            }
        }

        /// <summary>
        /// Determines whether to use original or user value 
        /// </summary>
        [Browsable(true), ReadOnlyAttribute(false), DisplayName("Use Original")]
        public bool UseOriginal
        {
            get
            {
                return _useGreet;
            }
            set
            {
                bool beforeChange = _useGreet;
                _useGreet = value;
                if (ValueChanged != null && beforeChange != _useGreet)
                    ValueChanged();
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                if (value != null)
                    _notes = value.Replace(";", ".");
                else
                    _notes = "";
            }
        }

        /// <summary>
        /// Returns Id if no Name is defined else returns Name.
        /// Sets the Name 
        /// </summary>
        public string Identifier
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                    return _id;
                else
                    return _name;
            }
            set
            {
                if (value != _id)
                    _name = value;
            }
        }

        public string DisplayForFormula
        {
            get
            {
                if (String.IsNullOrEmpty(CurrentFormula))
                    return ValueInDefaultUnit.ToStringFull();
                else
                    return CurrentFormula;
            }
            set
            {
                double val;
                if (Double.TryParse(value, out val))
                    ValueInDefaultUnit = val;

                CurrentFormula = value;
            }
        }
       
        #endregion accessors

        #region methods

        /// <summary>
        /// Compare the actual double value to another double value given as a parameter
        /// </summary>
        /// <param name="obj">The double value to compare to</param>
        /// <returns>Returns 0 if their values are equal, -1 if the value of the current one is less than the parameter, and +1 if the current one is greated than the parameter</returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Parameter secondParameter = obj as Parameter;
            if (secondParameter == null)
                throw new ArgumentException("Object is not a Parameter");

            //Comparing Same Units
            else if (Dim == secondParameter.Dim)
                return ValueInDefaultUnit.CompareTo(secondParameter.ValueInDefaultUnit);

            // Comparing if different Units n Unit preference order Joules, Grams, Litres
            else if (Dim != secondParameter.Dim)
            {
                // When Comparing two values of different units with one being Joules, The value of the with 0.0 should follow the other

                if (Dim == DimensionUtils.ENERGY || secondParameter.Dim == DimensionUtils.ENERGY)
                    if (ValueInDefaultUnit == 0.0)
                        return -1;
                    else if (secondParameter.ValueInDefaultUnit == 0.0)
                        return 1;
                    else
                        return ValueInDefaultUnit.CompareTo(secondParameter.ValueInDefaultUnit);


                else if (Dim == DimensionUtils.MASS ||secondParameter.Dim == DimensionUtils.MASS)
                    if (ValueInDefaultUnit == 0.0)
                        return -1;
                    else if (secondParameter.ValueInDefaultUnit == 0.0)
                        return 1;
                    else
                        return ValueInDefaultUnit.CompareTo(secondParameter.ValueInDefaultUnit);

                else if (Dim == DimensionUtils.VOLUME || secondParameter.Dim == DimensionUtils.VOLUME)
                    if (ValueInDefaultUnit == 0.0)
                        return -1;
                    else if (secondParameter.ValueInDefaultUnit == 0.0)
                        return 1;
                    else
                        return ValueInDefaultUnit.CompareTo(secondParameter.ValueInDefaultUnit);

                else
                    return 0;
            }
            else
                return 0;
        }

        /// <summary>
        /// Override for ToString, returns Value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string to_return = ValueInDefaultUnit.ToString() + " " + DimensionUtils.ToMLTh(Dim);
            if (_useGreet)
                to_return += " (original used)";
            else
                to_return += " (override used)";
            return to_return;
        }

        /// Creates an XML node to save that value in the DB  files
        public XmlAttribute ToXmlAttribute(XmlDocument xmlDocument, String attribute_name)
        {
            return xmlDocument.CreateAttr(attribute_name, ToXmlString());
        }

        internal string ToXmlString()
        {
#if DEBUG
            if (_useGreet)
                return (!String.IsNullOrEmpty(_greetFormulaString) ? _greetFormulaString : _bufferForGreetValueInDefaultUnit.ToStringFull())
                    + ";" + (!String.IsNullOrEmpty(_greetFormulaString) ? "" : _greetValuePreferedUnitExpression)//do not store any unit for a formula, the quantity is calculated and unit selected from the preferences 
                    + ";0;" + _userValuePreferedExpression + ";True" 
                    + ";" + _notes.Replace(";", ".") 
                    + ";" + _id 
                    + ";" + ModifiedBy
                    + ";" + ModifiedOn
                    + ";" + Name.Replace(";", "");
            else
                return (!String.IsNullOrEmpty(_userFormulaString) ? _userFormulaString : _bufferForUserValueInDefaultUnit.ToStringFull())
                    + ";" + (!String.IsNullOrEmpty(_userFormulaString) ? "" : _userValuePreferedExpression)//do not store any unit for a formula, the quantity is calculated and unit selected from the preferences
                    + ";0;" + _userValuePreferedExpression + ";True"
                    + ";" + _notes.Replace(";", ".") 
                    + ";" + _id 
                    + ";" + ModifiedBy
                    + ";" + ModifiedOn
                    + ";" + Name.Replace(";", "");
#else
            string str = (!String.IsNullOrEmpty(_greetFormulaString) ? _greetFormulaString : _bufferForGreetValueInDefaultUnit.ToStringFull())
                + ";" + (!String.IsNullOrEmpty(_greetFormulaString) ? "" : _greetValuePreferedUnitExpression)//do not store any unit for a formula, the quantity is calculated and unit selected from the preferences
                + ";" + (!String.IsNullOrEmpty(_userFormulaString) ? _userFormulaString : _bufferForUserValueInDefaultUnit.ToStringFull()) 
                + ";" + (!String.IsNullOrEmpty(_userFormulaString) ? "" :  _userValuePreferedExpression)//do not store any unit for a formula, the quantity is calculated and unit selected from the preferences
                + ";" + _useGreet.ToString()
                + ";" + (_notes != null ? _notes.Replace(";", ".") : "")
                + ";" + _id
                + ";" + (this.ModifiedBy != null ? this.ModifiedBy.Replace(";", "") : "")
                + ";" + (this.ModifiedOn != null ? this.ModifiedOn.Replace(";", "") : "")
                + ";" + (_name != null ? _name.Replace(";", "") : "");
            return str;
#endif

        }

        /// <summary>
        /// Evaluates all formulas in the parameter and populates the buffers
        /// </summary>
        /// <param name="data">The datacontext to find potential references to other parameters</param>
        /// <param name="guid">A GUID that will be used to parse all parameters, used to avoid parsing twice the same parameter if used in multiple formulas</param>
        public void UpdateBuffers(GData data, Guid tracer = new Guid())
        {
            if (!String.IsNullOrEmpty(CurrentFormula) && (tracer == new Guid() || tracer !=  _tracer))
            {
                _tracer = tracer;
                try
                {
                    KeyValuePair<uint, double> tempo = MathParseWithReferences.Parse(CurrentFormula, data, tracer);          
                    AQuantity qty = Units.QuantityList.ByDim(tempo.Key);                   
                    if (qty == null)
                    {
                        string human = DimensionUtils.ToMLTh(tempo.Key);
                    }
                    if (_useGreet)
                    {
                        _greetValueDim = qty.Dim;
                        _greetValuePreferedUnitExpression = qty.Units[qty.PreferedUnitIdx].Expression;
                    }
                    else
                    {
                        _userValueDim = qty.Dim;
                        _userValuePreferedExpression = qty.Units[qty.PreferedUnitIdx].Expression;
                    }

                    if (_useGreet)
                        _bufferForGreetValueInDefaultUnit = tempo.Value;
                    else
                        _bufferForUserValueInDefaultUnit = tempo.Value;
                }
                catch (Exception e)
                {
                    _bufferForGreetValueInDefaultUnit = Double.NaN;
                    throw e;
                }       
            }
        }

        /// <summary>
        /// Uses the value in default unit of the parameter and it's dim to create a light value
        /// Selects GREET or user value depending on the UserGREET flag
        /// </summary>
        /// <returns></returns>
        public LightValue ToLightValue()
        {
            LightValue lv = new LightValue(ValueInDefaultUnit, Dim);
            return lv;
        }

        /// <summary>
        /// <para>Copies all the members from the Parameter given as a parameter to this instance of a Parameter</para>
        /// <para>All copied except the ID</para>
        /// </summary>
        /// <param name="parameter">Parameter from which values are copied from</param>
        internal void CopyFrom(Parameter parameter)
        { 
            _bufferForGreetValueInDefaultUnit = parameter._bufferForGreetValueInDefaultUnit;
            _bufferForUserValueInDefaultUnit = parameter._bufferForUserValueInDefaultUnit;
            _greetValueDim = parameter._greetValueDim;
            _userValueDim = parameter._userValueDim;
            _greetValuePreferedUnitExpression = parameter._greetValuePreferedUnitExpression;
            _userValuePreferedExpression = parameter._userValuePreferedExpression;
            _greetFormulaString = parameter._greetFormulaString;
            _userFormulaString = parameter._userFormulaString;
            _useGreet = parameter._useGreet;
            _notes = parameter._notes;
            _name = parameter._name;
        }
        #endregion methods

        #region operators

        public static LightValue operator +(Parameter d1, Parameter d2)
        {
            if (d1.Dim != d2.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be added");
            LightValue result = new LightValue(d1.ValueInDefaultUnit - d2.ValueInDefaultUnit, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator +(Parameter d1, LightValue d2)
        {
            if (d1.Dim != d2.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be added");
            LightValue result = new LightValue(d1.ValueInDefaultUnit + d2.Value, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator +(LightValue d1, Parameter d2)
        {
            if (d1.Dim != d2.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be added");
            LightValue result = new LightValue(d1.Value + d2.ValueInDefaultUnit, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator +(Parameter d1, double d2)
        {
            LightValue result = new LightValue(d1.ValueInDefaultUnit + (double)d2, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator +(double d1, Parameter d2)
        {
            LightValue result = new LightValue(d2.ValueInDefaultUnit + (double)d1, d2.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator -(Parameter d1, Parameter d2)
        {
            if (d1.Dim != d2.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be substracted");
            LightValue result = new LightValue(d1.ValueInDefaultUnit - d2.ValueInDefaultUnit, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator -(Parameter d1, LightValue d2)
        {
            if (d1.Dim != d2.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be substracted");
            LightValue result = new LightValue(d1.ValueInDefaultUnit - d2.Value, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator -(LightValue d1, Parameter d2)
        {
            if (d1.Dim != d2.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be substracted");
            LightValue result = new LightValue(d1.Value - d2.ValueInDefaultUnit, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator -(Parameter d1, double d2)
        {
            LightValue result = new LightValue(d1.ValueInDefaultUnit - (double)d2, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator -(double d1, Parameter d2)
        {
            LightValue result = new LightValue((double)d1 - d2.ValueInDefaultUnit, d2.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator -(Parameter d1)
        {
            LightValue result = new LightValue(-d1.ValueInDefaultUnit, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(Parameter d1, Parameter d2)
        {
            LightValue result = new LightValue(d2.ValueInDefaultUnit * d1.ValueInDefaultUnit, DimensionUtils.Plus(d1.Dim, d2.Dim));
#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(Parameter d1, LightValue d2)
        {
            LightValue result = new LightValue(d2.Value * d1.ValueInDefaultUnit, DimensionUtils.Plus(d1.Dim, d2.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(LightValue d1, Parameter d2)
        {
            LightValue result = new LightValue(d2.ValueInDefaultUnit * d1.Value, DimensionUtils.Plus(d1.Dim, d2.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(double d1, Parameter d2)
        {
            LightValue result = new LightValue(d2.ValueInDefaultUnit * (double)d1, d2.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(Parameter d1, double d2)
        {
            LightValue result = new LightValue(d1.ValueInDefaultUnit * d2, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(Parameter d1, Parameter d2)
        {
            LightValue result = new LightValue(d1.ValueInDefaultUnit / d2.ValueInDefaultUnit, DimensionUtils.Minus(d1.Dim, d2.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(Parameter d1, LightValue d2)
        {
            LightValue result = new LightValue(d1.ValueInDefaultUnit / d2.Value, DimensionUtils.Minus(d1.Dim, d2.Dim));
#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(LightValue d1, Parameter d2)
        {
            LightValue result = new LightValue(d1.Value / d2.ValueInDefaultUnit, DimensionUtils.Minus(d1.Dim, d2.Dim));
#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(Parameter d1, double d2)
        {
            LightValue result = new LightValue(d1.ValueInDefaultUnit / (double)d2, d1.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(double d1, Parameter d2)
        {
            LightValue result = new LightValue((double)d1 / d2.ValueInDefaultUnit, DimensionUtils.Minus(0, d2.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif
            return result;
        }
#if DEBUG
        private static void TestResult(double result)
        {
            if (Double.IsNaN(result))
                NotANumberOperation();
            if (Double.IsNegativeInfinity(result) || Double.IsPositiveInfinity(result))
                InfinityOperation();
        }

        /// <summary>
        /// Used for debugging to see which operations are outptuting a NaN value
        /// </summary>
        private static void NotANumberOperation()
        {
        }
        private static void InfinityOperation()
        {
        }
#endif
        #endregion operators

        #region IHaveMetadata Members


        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }

        #endregion
    }
}