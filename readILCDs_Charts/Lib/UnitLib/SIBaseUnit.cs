using System;

namespace Greet.UnitLib
{
    /// <summary>
    /// The dimension is represented as a single unsigned integer and each basic SI unit dimension exponent  "occupies" 5 bits, i.e. the range for the exponent is -16..15. 
    /// The system is based on the modular arithmetics so to calculate the result of the multiplication the two integers representing dimension simply need to be added, 
    /// thus making operations on parameters with units extremely fast.
    /// </summary>
    internal static class SIBaseUnit
    {
        // for 5-bit system the range is -16..15
        private static int n_bit = 6; //5 bits for the number itself and 1 carry over bit is added
        private static uint modulo = (uint)Math.Pow(2, n_bit - 1);
        private static uint half_modulo = (uint)Math.Pow(2, n_bit - 2);
        private static uint yes = 1;
        private static uint no = 0;
        private static uint meaningful_bits_mask = 0x7DF7DF; // 011111 011111 011111 011111


        internal static uint FromMLT(int kg, int m, int s, int K = 0)
        {
            uint res = 0;

            res += (uint)(((kg < 0) ? yes : no) * modulo + kg) << (0 * n_bit);
            res += (uint)(((m < 0) ? yes : no) * modulo + m) << (1 * n_bit);
            res += (uint)(((s < 0) ? yes : no) * modulo + s) << (2 * n_bit);
            res += (uint)(((K < 0) ? yes : no) * modulo + K) << (3 * n_bit);
            return res;
        }

        internal static uint Plus(uint a, uint b)
        {
            return (a & meaningful_bits_mask) + (b & meaningful_bits_mask);
        }

        internal static void ToMLT(uint dim, out int kg, out int m, out int s, out int K)
        {
            uint temp;
            temp = (uint)(dim & ((int)Math.Pow(2, 1 * n_bit) - 1)) >> (0 * n_bit); temp = (temp % modulo); kg = (int)((temp < half_modulo) ? temp : temp - modulo);
            temp = (uint)(dim & ((int)Math.Pow(2, 2 * n_bit) - 1)) >> (1 * n_bit); temp = (temp % modulo); m = (int)((temp < half_modulo) ? temp : temp - modulo);
            temp = (uint)(dim & ((int)Math.Pow(2, 3 * n_bit) - 1)) >> (2 * n_bit); temp = (temp % modulo); s = (int)((temp < half_modulo) ? temp : temp - modulo);
            temp = (uint)(dim & ((int)Math.Pow(2, 4 * n_bit) - 1)) >> (3 * n_bit); temp = (temp % modulo); K = (int)((temp < half_modulo) ? temp : temp - modulo);
        }

        /// <summary>
        /// Human readable dimensions string
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        internal static string ToMLTh(uint dim)
        {
            int m, l, t, K;
            ToMLT(dim, out m, out l, out t, out K);
            return String.Format("[mass]^{0}[length]^{1}[time]^{2}", m, l, t);
        }
    }
}
