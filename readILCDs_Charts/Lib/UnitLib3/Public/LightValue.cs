using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Greet.UnitLib3
{
    /// <summary>
    /// To be used to perform calculations with physical quantities. 
    /// </summary>
    [Serializable]
    public class LightValue
    {
        private double _val;
        private uint _dim;

        #region constructors
        /// <summary>
        /// Creates a new LightValue object using a value in SI unit and an instance of a Dimension
        /// Warning the instance of the Dimension will be used as is as a reference. If one needs a new isntance of a Dimension it must be cloned first
        /// </summary>
        /// <param name="val">Value in SI unit</param>
        /// <param name="dim">Instance of a Dimension to be used by this LightValue</param>
        public LightValue(double val, uint dim)
        {
            this._val = val;
            this._dim = dim;
        }
        /// <summary>
        /// Creates a new LightValue object using a value and a unit expression
        /// Automatically converts the value to SI and calculate the proper dimension if the given expression is not an SI unit
        /// </summary>
        /// <param name="val">Value given in the same unit as the expression</param>
        /// <param name="expression">Unit expression</param>
        public LightValue(double val, string expression)
        {
            string siExp; string filteredUserExpression; uint eqDim; double slope, intercept;
            GuiUtils.FilterExpression(expression, out siExp, out filteredUserExpression, out eqDim, out slope, out intercept);
            _val = AQuantity.ConvertFromSpecificToSI(val, filteredUserExpression);
            _dim = eqDim;
        }

        #endregion

        #region accessors
        /// <summary>
        /// The value in SI units given the DIM (dimensionality) of the value
        /// </summary>
        public double Value
        {
            get { return _val; }
            set { _val = value; }
        }
        /// <summary>
        /// Dimension of the quantity associated with the value
        /// </summary>
        public uint Dim
        {
            get { return _dim; }
            set { _dim = value; }
        }
        #endregion

        #region operators
        public static LightValue operator *(LightValue a, LightValue b)
        {
            LightValue result = new LightValue(a.Value * b.Value, DimensionUtils.Plus(a.Dim, b.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }

        /// <summary>
        /// The preferred unit of the first summand will be used if any
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static LightValue operator +(LightValue a, LightValue b)
        {
            if (a.Dim != b.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be added");
            LightValue result = new LightValue(a.Value + b.Value, a.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator +(LightValue a, double b)
        {
            LightValue result = new LightValue(a.Value + b, a.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator -(LightValue a, LightValue b)
        {
            if (a.Dim != b.Dim)
                throw new System.ArgumentException("Values of different dimensionality cannot be added");
            LightValue result = new LightValue(a.Value - b.Value, a.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator -(LightValue a)
        {
            LightValue result = new LightValue(-a.Value, a.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator /(LightValue a, LightValue b)
        {
            LightValue result = new LightValue(a.Value / b.Value, DimensionUtils.Minus(a.Dim, b.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator /(LightValue a, double b)
        {
            LightValue result = new LightValue(a.Value / b, a.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator /(double a, LightValue b)
        {
            LightValue result = new LightValue(a / b.Value, DimensionUtils.Flip(b.Dim));

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator *(LightValue a, double b)
        {
            LightValue result = new LightValue(a.Value * b, a.Dim);

#if DEBUG
            double result_double = result.Value;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator *(double b, LightValue a)
        {
            LightValue result = new LightValue(a.Value * b, a.Dim);

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

        #endregion

        #region UnitLib API

        private string qname;

        /// <summary>
        /// Creates a light value from a unit or group name of the UnitLib1 to a UnitLib3.LightValue
        /// </summary>
        /// <param name="unitOrGroup">Unit or group name of the UnitLib1</param>
        /// <param name="value">Value in the given unit</param>
        [Obsolete("Used for old unit system compatibility, please use LightValue(double val, uint dim) or LightValue(double val, string expression) instead")]
        public LightValue(string unitOrGroup, double value)
        {
            ConversionFromOLDUnitLib.GetDimensionAndConvertToSi(unitOrGroup, value, out this._dim, out this._val);
        }

        public LightValue()
        {
            this._val = 0.0;
            this._dim = 0;
        }

        
        [Obsolete("Used for old unit system compatibility, use Dim instead")]
        public string QuantityName
        {
            get
            {
                if (String.IsNullOrEmpty(qname))
                    return ConversionFromOLDUnitLib.NEWQuantityName2OLDGroupName[Units.Dim2Quantities[this._dim][0].Name];
                else
                    return qname;
            }
            set
            {
                qname = value;
                if (!uint.TryParse(qname, out _dim))
                {
                    this._dim = uint.Parse(Units.OLDGroup2Dims[qname]);
                }
            }
        }
        /// <summary>
        /// The value in SI units
        /// </summary>
        [Obsolete("Used for old unit system compatibility, use Value instead")]
        public double ValueInDefaultUnit
        {
            get { return _val; }
            set { _val = value; }
        }

        public override string ToString()
        {
            return this._val.ToString() + " " + DimensionUtils.ToMLTUnith(this._dim);
        }
        
        #endregion
    }
}

