using System;
using System.Collections.Generic;
using System.Linq;

namespace Greet.UnitLib3
{
    /// <summary>
    /// The dimension is represented as a single unsigned integer and each basic SI unit dimension exponent  "occupies" 5 bits, i.e. the range for the exponent is -16..15. 
    /// The system is based on the modular arithmetics so to calculate the result of the multiplication the two integers representing dimension simply need to be added, 
    /// thus making operations on parameters with units extremely fast.
    /// </summary>
    public static class DimensionUtils
    {
        // for 5-bit system the range is -16..15
        private static int n_bit = 6; //5 bits for the number itself and 1 carry over bit is added
        private static uint modulo = (uint)Math.Pow(2, n_bit - 1);
        private static uint half_modulo = (uint)Math.Pow(2, n_bit - 2);
        private static uint yes = 1;
        private static uint no = 0;
        private static uint meaningful_bits_mask = 0x7DF7DF; // 011111 011111 011111 011111

        /// <summary>
        /// Dimensionless
        /// </summary>
        public const uint RATIO = 0; //DimensionUtils.FromMLT(0, 0, 0);
        public const uint ENERGY = 123009; //DimensionUtils.FromMLT(1, 2, -2);
        public const uint MASS = 1; //DimensionUtils.FromMLT(1, 0, 0);
        public const uint VOLUME = 192; //DimensionUtils.FromMLT(0, 3, 0);
        public const uint LENGTH = 64; //DimensionUtils.FromMLT(0, 1, 0);
        public const uint CURRENCY = 262144; //DimensionUtils.FromMLT(0, 0, 0, 1);
        /// <summary>
        /// J/(kg m)
        /// </summary>
        public const uint EI = 127040; //DimensionUtils.FromMLT(0, 1, -1);
        /// <summary>
        /// 1/(m^2)
        /// </summary>
        public const uint FECONOMY = 1920; //DimensionUtils.FromMLT(0, -2, 0);
        /// <summary>
        /// J/m
        /// </summary>
        public const uint VENERGY = 122945; //DimensionUtils.FromMLT(1, 1, -2);
        /// <summary>
        /// J/(m^3)
        /// </summary>
        public const uint HVV = 124865; //DimensionUtils.FromMLT(1, -1, -2);
        /// <summary>
        /// J/kg
        /// </summary>
        public const uint HVM = 123008; //DimensionUtils.FromMLT(0, 2, -2);
        /// <summary>
        /// Converts from natural units to a single integer that represents the dimension.
        /// int --> uint
        ///  0 --> 0
        ///  1 --> 1
        ///  ...
        ///  15 --> 15
        /// -16	-->	16
        /// -15	-->	17
        /// -14	-->	18
        /// -13	-->	19
        /// -12	-->	20
        /// -11	-->	21
        /// -10	-->	22
        /// -9	-->	23
        /// -8	-->	24
        /// -7	-->	25
        /// -6	-->	26
        /// -5	-->	27
        /// -4	-->	28
        /// -3	-->	29
        /// -2	-->	30
        /// -1	-->	31
        /// </summary>
        /// <param name="kg">Mass</param>
        /// <param name="m">Length</param>
        /// <param name="s">Seconds</param>
        /// <param name="c">Currency</param>
        /// <returns></returns>
        public static uint FromMLT(int kg, int m, int s, int c = 0)
        {
            uint res = 0;

            res += (uint)(((kg < 0) ? yes : no) * modulo + kg) << (0 * n_bit);
            res += (uint)(((m < 0) ? yes : no) * modulo + m) << (1 * n_bit);
            res += (uint)(((s < 0) ? yes : no) * modulo + s) << (2 * n_bit);
            res += (uint)(((c < 0) ? yes : no) * modulo + c) << (3 * n_bit);
            return res;
        }
        /// <summary>
        /// Calculates 1/dim
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static uint Flip(uint dim)
        {
            int m, l, t, c;
            ToMLT(dim, out m, out l, out t, out c);
            return FromMLT(-m, -l, -t, -c);
        }
        /// <summary>
        /// Calculates the result of sum of dimensions. To be used to calculate the resulting dimension of multiplication of two quantities
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static uint Plus(uint a, uint b)
        {
            return ((a & meaningful_bits_mask) + (b & meaningful_bits_mask)) & meaningful_bits_mask;
        }

        /// <summary>
        /// Calculates the result of subtraction of dimensions. To be used to calculate the resulting dimension of dividing of two quantities
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static uint Minus(uint a, uint b)
        {
            return Plus(a, Flip(b));// (a & meaningful_bits_mask) - (b & meaningful_bits_mask);
        }
        /// <summary>
        /// Calculates the result of multiplying dimensions by an integer, to be used to calculate the dimension of the power operation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static uint Times(uint a, int factor)
        {
            uint res = 0;
            if (factor == 0)
                return res;
            if (factor < 0)
            {
                for (int i = 0; i < -factor; i++)
                    res = Minus(res, a);
                return res;
            }
            for (int i = 0; i < factor; i++)
                res = Plus(res, a);
            return res;
        }
        /// <summary>
        /// Converts uint representation to convert to MLT dimensions
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="kg"></param>
        /// <param name="m"></param>
        /// <param name="s"></param>
        /// <param name="c"></param>

        public static void ToMLT(uint dim, out int kg, out int m, out int s, out int c)
        {
            uint temp;
            temp = (uint)(dim & ((int)Math.Pow(2, 1 * n_bit) - 1)) >> (0 * n_bit); temp = (temp % modulo); kg = (int)((temp < half_modulo) ? temp : temp - modulo);
            temp = (uint)(dim & ((int)Math.Pow(2, 2 * n_bit) - 1)) >> (1 * n_bit); temp = (temp % modulo); m = (int)((temp < half_modulo) ? temp : temp - modulo);
            temp = (uint)(dim & ((int)Math.Pow(2, 3 * n_bit) - 1)) >> (2 * n_bit); temp = (temp % modulo); s = (int)((temp < half_modulo) ? temp : temp - modulo);
            temp = (uint)(dim & ((int)Math.Pow(2, 4 * n_bit) - 1)) >> (3 * n_bit); temp = (temp % modulo); c = (int)((temp < half_modulo) ? temp : temp - modulo);
        }

        /// <summary>
        /// Human readable dimensions string
        /// </summary>
        /// <param name="dim"></param>
        /// <returns>Example: [mass]^1[length]^-1}]</returns>
        public static string ToMLTh(uint dim)
        {
            int m, l, t, c;
            ToMLT(dim, out m, out l, out t, out c);
            return String.Format("[mass]^{0}[length]^{1}[time]^{2}", m, l, t);
        }
        /// <summary>
        /// Human readable SI unit expression
        /// </summary>
        /// <param name="dim"></param>
        /// <returns>Example: kg/mi</returns>
        public static string ToMLTUnith(uint dim)
        {
            int m, l, t, c;
            List<string> numerator = new List<string>();
            List<string> denominator = new List<string>();
            ToMLT(dim, out m, out l, out t, out c);
            if (m == 1) numerator.Add("kg");
            if (l == 1) numerator.Add("m");
            if (t == 1) numerator.Add("s");
            if (c == 1) numerator.Add("$");
            if (m == -1) denominator.Add("kg");
            if (l == -1) denominator.Add("m");
            if (t == -1) denominator.Add("s");
            if (c == -1) denominator.Add("$");
            if (m > 1) numerator.Add(String.Format("kg^{0}", m.ToString()));
            if (m < -1) denominator.Add(String.Format("kg^{0}", (-m).ToString()));
            if (l > 1) numerator.Add(String.Format("m^{0}", l.ToString()));
            if (l < -1) denominator.Add(String.Format("m^{0}", (-l).ToString()));
            if (t > 1) numerator.Add(String.Format("s^{0}", t.ToString()));
            if (t < -1) denominator.Add(String.Format("s^{0}", (-t).ToString()));
            if (c > 1) numerator.Add(String.Format("$^{0}", c.ToString()));
            if (c < -1) denominator.Add(String.Format("C^{0}", (-c).ToString()));
            if (numerator.Count == 0 && denominator.Count > 0)
                numerator.Add("1");
            string top = String.Join(" ", numerator);
            string bot = String.Join(" ", denominator);
            if (denominator.Count > 1)
                bot = "(" + bot + ")";
            if (numerator.Count > 1)
                top = "(" + top + ")";


            if (denominator.Count > 0)
                return top + "/" + bot;
            else
                return top;
        }

        /// <summary>
        /// Returns only the numerator units
        /// For example passing a uint representing J/kg will return J
        /// </summary>
        /// <param name="dim">The uint representing a dimension composed of a numerator and a denominator</param>
        /// <returns>Numerator part only of the dimension parameter</returns>
        public static uint Numerator(uint dim)
        {
            int m, l, t, u = 0;
            ToMLT(dim, out m, out l, out t, out u);
            return FromMLT(Math.Max(0, m),
                Math.Max(0, l),
                Math.Max(0, t),
                Math.Max(0, u));
        }

        /// <summary>
        /// Returns only the denominator units
        /// For example passing a uint representing J/kg will return kg
        /// </summary>
        /// <param name="dim">The uint representing a dimension composed of a numerator and a denominator</param>
        /// <returns>Denominator part only of the dimension parameter</returns>
        public static uint Denominator(uint dim)
        { 
         int m, l, t, u = 0;
            ToMLT(dim, out m, out l, out t, out u);
            return FromMLT(-Math.Max(0, m),
                -Math.Min(0, l),
                -Math.Min(0, t),
                -Math.Min(0, u));
        }

        /// <summary>
        /// Breaks the units with exponents into multiple instances of a simpler unit
        /// For example break m^2 in m*m
        /// </summary>
        /// <param name="units">Listof units we want to break</param>
        /// <returns>List containing more occurences of the units that needed to be broken up</returns>
        private static List<string> BreakExponents(List<string> units)
        {
            List<string> returned = new List<string>();
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Contains("^"))
                {
                    string[] split = units[i].Split('^');
                    int exp = 1;
                    Int32.TryParse(split[1], out exp);
                    for (int rep = 0; rep < exp; rep++)
                    {
                        returned.Add(split[0]);
                    }
                }
                else
                    returned.Add(units[i]);
            }
            return returned;
        }

        public static uint FromString(string p)
        {
            string siExp, filteredUserExpression; uint eqDim; double slope, intercept;

            GuiUtils.FilterExpression(p, out siExp, out filteredUserExpression, out eqDim, out slope, out intercept);

            return eqDim;
        }
    }
}
