using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DABRAS_Software
{    
    [Serializable]
    public class QCCalResultNode
    {
        #region Data Members
        private DateTime Completed;
        private int Time;
        private FormQC.TypeOfQC Type;
        private double NetAlphaCPM;
        private double NetBetaCPM;
        private int BadgeNumber;
        bool Plottable;
        private string Comment;
        private string Name;
        #endregion

        #region Constructor
        /*ONLY USING THIS BECAUSE OF LEGACY DATA!*/
        public QCCalResultNode(DateTime _Completed, FormQC.TypeOfQC _Type, double _NACPM, double _NBCPM, int _BadgeNumber, bool _Plottable, string _Comment, string _Name)
        {
            this.Completed = _Completed;
            this.Type = _Type;
            this.NetAlphaCPM = _NACPM;
            this.NetBetaCPM = _NBCPM;
            this.BadgeNumber = _BadgeNumber;
            this.Plottable = _Plottable;
            this.Comment = _Comment;
            this.Name = _Name;
            this.Time = 600; //10 minutes
        }

        public QCCalResultNode(DateTime _Completed, FormQC.TypeOfQC _Type, double _NACPM, double _NBCPM, int _BadgeNumber, bool _Plottable, string _Comment, string _Name, int _Time)
        {
            this.Completed = _Completed;
            this.Type = _Type;
            this.NetAlphaCPM = _NACPM;
            this.NetBetaCPM = _NBCPM;
            this.BadgeNumber = _BadgeNumber;
            this.Plottable = _Plottable;
            this.Comment = _Comment;
            this.Name = _Name;
            this.Time = _Time;
        }
        #endregion

        #region Getters
        public int GetSampleTime()
        {
            return this.Time;
        }

        public int GetBadgeNo()
        {
            return this.BadgeNumber;
        }

        public DateTime GetDateTimeCompleted()
        {
            return this.Completed;
        }

        public FormQC.TypeOfQC GetTypeOfQC()
        {
            return this.Type;
        }

        public double GetNetAlphaCPM()
        {
            return this.NetAlphaCPM;
        }

        public double GetNetBetaCPM()
        {
            return this.NetBetaCPM;
        }

        public bool IsPlottable()
        {
            return this.Plottable;
        }

        public string GetComment()
        {
            return this.Comment;
        }

        public string GetName()
        {
            return this.Name;
        }

        public string[] GetCSVHeader()
        {
            string[] ReturnString = new string[9];

            ReturnString[0] = "Time Completed";
            ReturnString[1] = "Sample Time";
            ReturnString[2] = "Employee Name";
            ReturnString[3] = "Badge No.";
            ReturnString[4] = "Type of Count";
            ReturnString[5] = "Net Alpha CPM";
            ReturnString[6] = "Net Beta CPM";
            ReturnString[7] = "Plotted";
            ReturnString[8] = "Comment";

            return ReturnString;
        }

        public string[] GetCSVArray()
        {
            string[] ReturnString = new string[9];

            ReturnString[0] = Convert.ToString(this.Completed);
            ReturnString[1] = Convert.ToString(this.Time);
            ReturnString[2] = Convert.ToString(this.Name);
            ReturnString[3] = Convert.ToString(this.BadgeNumber);
            ReturnString[4] = Convert.ToString(this.Type);
            ReturnString[5] = Convert.ToString(this.NetAlphaCPM);
            ReturnString[6] = Convert.ToString(this.NetBetaCPM);
            ReturnString[7] = (this.Plottable ? "Yes" : "No");
            ReturnString[8] = this.Comment;

            return ReturnString;
        }

        #endregion

        #region Setters
        public bool SetPlottable(bool _B)
        {
            this.Plottable = _B;
            return true;
        }

        public bool SetComment(string _C)
        {
            this.Comment = _C;
            return true;
        }
        #endregion
    }
}
