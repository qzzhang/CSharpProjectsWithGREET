using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DABRAS_Software
{
    [Serializable]
    public class User
    {
        #region Data Members
        private int BadgeNo;
        private string Name;
        #endregion

        #region Constructor
        public User(int _Badge, string _Name)
        {
            this.BadgeNo = _Badge;
            this.Name = _Name;
            return;
        }
        #endregion

        #region Getters
        public int GetBadgeNo()
        {
            return this.BadgeNo;
        }

        public string GetName()
        {
            return this.Name;
        }
        #endregion

        #region Setters
        public bool SetBadgeNo(int _BN)
        {
            this.BadgeNo = _BN;
            return true;
        }

        public bool SetName(string _Name)
        {
            this.Name = _Name;
            return true;
        }
        #endregion

    }
}
