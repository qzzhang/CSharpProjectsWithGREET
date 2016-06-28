using System;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class ConventionalProducts
    {
        #region attributes

        InputResourceReference mk;
        Parameter ratio;

        #endregion attributes

        #region constructors

        public ConventionalProducts(GData data, InputResourceReference mk, Parameter ratio)
        {
            this.mk = mk;
            this.ratio = ratio;
        }

        public ConventionalProducts(GData data)
        {
            this.mk = new InputResourceReference();
            this.ratio = data.ParametersData.CreateRegisteredParameter("%", 0);
        }

        #endregion constructors

        #region Accessors

        public InputResourceReference MaterialKey
        {

            get
            {
                return mk;
            }
            set
            {
                mk = value;
            }

        }

        public Parameter DispRatio
        {
            get
            {
                return ratio;
            }
            set
            {
                ratio = value;
            }
        }
        #endregion
    }
}
