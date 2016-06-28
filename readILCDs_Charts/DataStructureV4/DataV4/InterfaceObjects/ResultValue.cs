using System;
using Greet.DataStructureV4.Interfaces;
using Greet.UnitLib3;

namespace Greet.DataStructureV4
{
    public class ResultValue : IValue
    {
        #region private attributes
        /// <summary>
        /// A value that can be used for any type of results
        /// </summary>
        double _value;
        /// <summary>
        /// The unit associated with that unit
        /// </summary>
        string _unit;
        /// <summary>
        /// The name of the spiecie associated with that result, can be an emission, a primary resource ID or a group ID
        /// </summary>
        Greet.DataStructureV4.Interfaces.Enumerators.ResultType _valueSpiecie;
        /// <summary>
        /// The ID associated with that specie, can be an emission, a primary resource ID or a group ID
        /// </summary>
        int _spiecieID;

        private ResultValue(IValue iResultValue)
        {
            this._value = iResultValue.Value;
            this._unit = iResultValue.UnitExpression;
            this._valueSpiecie = iResultValue.ValueSpecie;
            this._spiecieID = iResultValue.SpecieId;
        }

        public ResultValue()
        {
            // TODO: Complete member initialization
        }

        #endregion

        #region public accessors
        /// <summary>
        /// A value that can be used for any type of results
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        /// <summary>
        /// The unit associated with that unit
        /// the unit can be "kg", "J" or "m^3"
        /// </summary>
        public string UnitExpression
        {
            get { return _unit; }
            set { _unit = value; }
        }
        /// <summary>
        /// The name of the spiecie associated with that result.
        /// It can be "emission", a "resource", an "emgroup" or an "regroup"
        /// </summary>
        public Greet.DataStructureV4.Interfaces.Enumerators.ResultType ValueSpecie
        {
            get { return _valueSpiecie; }
            set { _valueSpiecie = value; }
        }
        /// <summary>
        /// The ID associated with that specie
        /// </summary>
        public int SpecieId
        {
            get { return _spiecieID; }
            set { _spiecieID = value; }
        }
        #endregion

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ResultValue secondParameter = obj as ResultValue;
            if (secondParameter == null)
                throw new ArgumentException("Object is not a ResultValue");

            //Comparing Same Units
            else if (Units.QuantityList[this._unit].Dim == Units.QuantityList[secondParameter._unit].Dim)
                return this._value.CompareTo(secondParameter._value);

            // Comparing if different Units n Unit preference order Joules, Grams, Litres
            else if (Units.QuantityList[this._unit].Dim != Units.QuantityList[secondParameter._unit].Dim)
            {
                // When Comparing two values of different units with one being Joules, The value of the with 0.0 should follow the other

                if (Units.QuantityList[this._unit].Dim == DimensionUtils.ENERGY || Units.QuantityList[secondParameter._unit].Dim == DimensionUtils.ENERGY) //hardcoded
                    if (this._value == 0.0)
                        return -1;
                    else if (secondParameter._value == 0.0)
                        return 1;
                    else
                        return this._value.CompareTo(secondParameter._value);


                else if (Units.QuantityList[this._unit].Dim == DimensionUtils.MASS || Units.QuantityList[secondParameter._unit].Dim == DimensionUtils.MASS) //hardcoded
                    if (this._value == 0.0)
                        return -1;
                    else if (secondParameter._value == 0.0)
                        return 1;
                    else
                        return this._value.CompareTo(secondParameter._value);

                else if (Units.QuantityList[this._unit].Dim == DimensionUtils.VOLUME || Units.QuantityList[secondParameter._unit].Dim == DimensionUtils.VOLUME) //hardcoded
                    if (this._value == 0.0)
                        return -1;
                    else if (secondParameter._value == 0.0)
                        return 1;
                    else
                        return this._value.CompareTo(secondParameter._value);

                else
                    return 0;
            }
            else
                return 0;
        }
    }
}
