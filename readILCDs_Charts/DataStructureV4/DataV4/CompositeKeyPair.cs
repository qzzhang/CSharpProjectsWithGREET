
using System;
using System.Collections.Generic;

namespace Greet.DataStructureV4
{
    [Serializable]
    internal class CompositeKeyPair<TKey1, TKey2>
    {
        private TKey1 key1;

        internal TKey1 Key1
        {
            get { return key1; }
        }
        private TKey2 key2;

        internal TKey2 Key2
        {
            get { return key2; }
        }
        public CompositeKeyPair(TKey1 _key1, TKey2 _key2)
        {
            //if (_key1.GetType().GetMethod("Equals") == null || _key2.GetType().GetMethod("Equals") == null)
            //    throw new System.ArgumentException("All of the element of a composite key have to have Equal method defined");
            this.key1 = _key1;
            this.key2 = _key2;
        }

        public override int GetHashCode()
        {
            return key1.GetHashCode() ^ key2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Type[] obj_pars = (obj.GetType()).GetGenericArguments();
            if (obj.GetType() == this.GetType() && typeof(TKey1) == obj_pars[0] && typeof(TKey2) == obj_pars[1])
            {
                CompositeKeyPair<TKey1, TKey2> casted_obj = (CompositeKeyPair<TKey1, TKey2>)obj;
                // return (this.key1.Equals(casted_obj.key1) && this.key2.Equals(casted_obj.key2));
                return (EqualityComparer<TKey1>.Default.Equals(this.key1, casted_obj.key1) && EqualityComparer<TKey2>.Default.Equals(this.key2, casted_obj.key2));
            }

            return base.Equals(obj);
        }


    }
}
