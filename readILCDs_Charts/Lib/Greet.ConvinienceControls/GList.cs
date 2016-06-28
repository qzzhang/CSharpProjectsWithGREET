using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.ConvinienceControls
{
    class GList<T> : List<T> where T : IComparable
    {
        public static GList<T> operator +(GList<T> a, GList<T> b)
        {
            var res = new GList<T>();
            if (a.Count != b.Count)
                throw new Exception("GList objects are of different length");
            for (int i=0; i<a.Count; i++)
                res[i] = (dynamic)a[i] + (dynamic)b[i];
            return res;
        }

        public static GList<T> operator -(GList<T> a, GList<T> b) 
        {
            var res = new GList<T>();
            if (a.Count != b.Count)
                throw new Exception("GList objects are of different length");
            for (int i = 0; i < a.Count; i++)
                res[i] = (dynamic)a[i] - (dynamic)b[i];
            return res;
        }

        public static GList<T> operator *(double a, GList<T> b)
        {
            var res = new GList<T>();
            for (int i = 0; i < b.Count; i++)
                res[i] = a * (dynamic)b[i];
            return res;
        }
    }
}
