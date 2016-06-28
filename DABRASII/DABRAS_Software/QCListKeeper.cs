using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DABRAS_Software
{
    [Serializable]
    public class QCListKeeper
    {

        #region Data Members
        private List<QCCalResultNode> ListOfCurrentNodes;
        private List<QCCalResultNode> ListOfDefunctNodes;
        private bool ListModified = false;
        private bool Locked = false;
        #endregion

        #region Constructor
        public QCListKeeper()
        {
            ListOfCurrentNodes = new List<QCCalResultNode>();
            ListOfDefunctNodes = new List<QCCalResultNode>();
            return;
        }
        #endregion

        #region Test

        public void MakeDummyList()
        {
            /*This data was ripped from the old DABRAS.*/
            /*Use the C data converter to go from excel to this! Do NOT type this over and over again!*/
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2009"), FormQC.TypeOfQC.Background, -4, 22222, 1, false, "Godzilla Attacked!!!!", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2009"), FormQC.TypeOfQC.Background, 1.70, 260.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2009"), FormQC.TypeOfQC.Alpha, 11618.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2009"), FormQC.TypeOfQC.Beta, 1, 33489.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/18/2009"), FormQC.TypeOfQC.Background, 1.00, 243.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/18/2009"), FormQC.TypeOfQC.Alpha, 11261.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/18/2009"), FormQC.TypeOfQC.Beta, 1, 32836.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/19/2009"), FormQC.TypeOfQC.Background, 1.20, 250.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/19/2009"), FormQC.TypeOfQC.Alpha, 11402.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/19/2009"), FormQC.TypeOfQC.Beta, 1, 32512.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/21/2009"), FormQC.TypeOfQC.Background, 1.30, 254.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/21/2009"), FormQC.TypeOfQC.Alpha, 11765.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/21/2009"), FormQC.TypeOfQC.Beta, 1, 32593.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/22/2009"), FormQC.TypeOfQC.Background, 2.00, 247.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/22/2009"), FormQC.TypeOfQC.Alpha, 11631.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/22/2009"), FormQC.TypeOfQC.Beta, 1, 32692.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/23/2009"), FormQC.TypeOfQC.Background, 2.00, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/23/2009"), FormQC.TypeOfQC.Alpha, 11273.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/23/2009"), FormQC.TypeOfQC.Beta, 1, 32452.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/24/2009"), FormQC.TypeOfQC.Background, 1.40, 245.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/24/2009"), FormQC.TypeOfQC.Alpha, 11402.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/24/2009"), FormQC.TypeOfQC.Beta, 1, 31983.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/25/2009"), FormQC.TypeOfQC.Background, 1.00, 238.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/25/2009"), FormQC.TypeOfQC.Alpha, 11497.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/25/2009"), FormQC.TypeOfQC.Beta, 1, 32626.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/28/2009"), FormQC.TypeOfQC.Background, 2.20, 264.20, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/28/2009"), FormQC.TypeOfQC.Alpha, 11776.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/28/2009"), FormQC.TypeOfQC.Beta, 1, 32894.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/29/2009"), FormQC.TypeOfQC.Background, 0.90, 259.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/29/2009"), FormQC.TypeOfQC.Alpha, 11456.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/29/2009"), FormQC.TypeOfQC.Beta, 1, 32577.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/30/2009"), FormQC.TypeOfQC.Background, 1.70, 251.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/30/2009"), FormQC.TypeOfQC.Alpha, 11353.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/30/2009"), FormQC.TypeOfQC.Beta, 1, 32448.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/1/2009"), FormQC.TypeOfQC.Background, 1.90, 255.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/1/2009"), FormQC.TypeOfQC.Alpha, 11128.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/1/2009"), FormQC.TypeOfQC.Beta, 1, 32582.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/2/2009"), FormQC.TypeOfQC.Background, 1.70, 254.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/2/2009"), FormQC.TypeOfQC.Alpha, 11854.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/2/2009"), FormQC.TypeOfQC.Beta, 1, 32855.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/5/2009"), FormQC.TypeOfQC.Background, 1.50, 255.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/5/2009"), FormQC.TypeOfQC.Alpha, 11371.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/5/2009"), FormQC.TypeOfQC.Beta, 1, 32063.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/6/2009"), FormQC.TypeOfQC.Background, 1.30, 248.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/6/2009"), FormQC.TypeOfQC.Alpha, 11643.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/6/2009"), FormQC.TypeOfQC.Beta, 1, 32580.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/7/2009"), FormQC.TypeOfQC.Background, 1.20, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/7/2009"), FormQC.TypeOfQC.Alpha, 11717.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/7/2009"), FormQC.TypeOfQC.Beta, 1, 32619.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/8/2009"), FormQC.TypeOfQC.Background, 1.60, 250.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/8/2009"), FormQC.TypeOfQC.Alpha, 11731.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/8/2009"), FormQC.TypeOfQC.Beta, 1, 31978.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/9/2009"), FormQC.TypeOfQC.Background, 1.40, 251.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/9/2009"), FormQC.TypeOfQC.Alpha, 11728.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/9/2009"), FormQC.TypeOfQC.Beta, 1, 31813.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/12/2009"), FormQC.TypeOfQC.Background, 0.80, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/12/2009"), FormQC.TypeOfQC.Alpha, 11240.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/12/2009"), FormQC.TypeOfQC.Beta, 1, 31750.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/13/2009"), FormQC.TypeOfQC.Background, 0.90, 254.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/13/2009"), FormQC.TypeOfQC.Alpha, 11108.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/13/2009"), FormQC.TypeOfQC.Beta, 1, 32794.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/14/2009"), FormQC.TypeOfQC.Background, 1.70, 251.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/14/2009"), FormQC.TypeOfQC.Alpha, 11083.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/14/2009"), FormQC.TypeOfQC.Beta, 1, 32374.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/15/2009"), FormQC.TypeOfQC.Background, 1.30, 259.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/15/2009"), FormQC.TypeOfQC.Alpha, 11632.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/15/2009"), FormQC.TypeOfQC.Beta, 1, 32197.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/16/2009"), FormQC.TypeOfQC.Background, 1.30, 251.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/16/2009"), FormQC.TypeOfQC.Alpha, 11569.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/16/2009"), FormQC.TypeOfQC.Beta, 1, 32886.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/18/2009"), FormQC.TypeOfQC.Background, 1.60, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/18/2009"), FormQC.TypeOfQC.Alpha, 11380.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/18/2009"), FormQC.TypeOfQC.Beta, 1, 32109.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/19/2009"), FormQC.TypeOfQC.Background, 1.30, 257.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/19/2009"), FormQC.TypeOfQC.Alpha, 11441.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/19/2009"), FormQC.TypeOfQC.Beta, 1, 32454.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/20/2009"), FormQC.TypeOfQC.Background, 1.00, 246.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/20/2009"), FormQC.TypeOfQC.Alpha, 11413.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/20/2009"), FormQC.TypeOfQC.Beta, 1, 32082.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/21/2009"), FormQC.TypeOfQC.Background, 1.60, 258.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/21/2009"), FormQC.TypeOfQC.Alpha, 11533.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/21/2009"), FormQC.TypeOfQC.Beta, 1, 31776.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/22/2009"), FormQC.TypeOfQC.Background, 1.50, 253.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/22/2009"), FormQC.TypeOfQC.Alpha, 11374.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/22/2009"), FormQC.TypeOfQC.Beta, 1, 32274.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/23/2009"), FormQC.TypeOfQC.Background, 1.30, 253.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/23/2009"), FormQC.TypeOfQC.Alpha, 11798.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/23/2009"), FormQC.TypeOfQC.Beta, 1, 32137.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/24/2009"), FormQC.TypeOfQC.Background, 1.30, 257.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/24/2009"), FormQC.TypeOfQC.Alpha, 11514.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/24/2009"), FormQC.TypeOfQC.Beta, 1, 32518.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/26/2009"), FormQC.TypeOfQC.Background, 0.80, 247.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/26/2009"), FormQC.TypeOfQC.Alpha, 11285.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/26/2009"), FormQC.TypeOfQC.Beta, 1, 32601.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/27/2009"), FormQC.TypeOfQC.Background, 1.70, 261.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/27/2009"), FormQC.TypeOfQC.Alpha, 11341.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/27/2009"), FormQC.TypeOfQC.Beta, 1, 32317.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/28/2009"), FormQC.TypeOfQC.Background, 0.90, 252.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/28/2009"), FormQC.TypeOfQC.Alpha, 11609.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/28/2009"), FormQC.TypeOfQC.Beta, 1, 32128.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/29/2009"), FormQC.TypeOfQC.Background, 1.50, 247.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/29/2009"), FormQC.TypeOfQC.Alpha, 11226.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/29/2009"), FormQC.TypeOfQC.Beta, 1, 32657.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/30/2009"), FormQC.TypeOfQC.Background, 0.80, 255.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/30/2009"), FormQC.TypeOfQC.Alpha, 11653.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/30/2009"), FormQC.TypeOfQC.Beta, 1, 32379.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/2/2009"), FormQC.TypeOfQC.Background, 1.10, 244.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/2/2009"), FormQC.TypeOfQC.Alpha, 11536.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/2/2009"), FormQC.TypeOfQC.Beta, 1, 32484.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/3/2009"), FormQC.TypeOfQC.Background, 1.00, 252.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/3/2009"), FormQC.TypeOfQC.Alpha, 11210.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/3/2009"), FormQC.TypeOfQC.Beta, 1, 32462.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/4/2009"), FormQC.TypeOfQC.Background, 0.90, 256.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/4/2009"), FormQC.TypeOfQC.Alpha, 11377.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/4/2009"), FormQC.TypeOfQC.Beta, 1, 32134.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/5/2009"), FormQC.TypeOfQC.Background, 0.90, 250.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/5/2009"), FormQC.TypeOfQC.Alpha, 11259.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/5/2009"), FormQC.TypeOfQC.Beta, 1, 32190.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/6/2009"), FormQC.TypeOfQC.Background, 1.50, 248.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/6/2009"), FormQC.TypeOfQC.Alpha, 11295.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/6/2009"), FormQC.TypeOfQC.Beta, 1, 32358.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/9/2009"), FormQC.TypeOfQC.Background, 2.20, 239.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/9/2009"), FormQC.TypeOfQC.Alpha, 11527.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/9/2009"), FormQC.TypeOfQC.Beta, 1, 31206.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/10/2009"), FormQC.TypeOfQC.Background, 1.10, 253.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/10/2009"), FormQC.TypeOfQC.Alpha, 11502.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/10/2009"), FormQC.TypeOfQC.Beta, 1, 32407.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/11/2009"), FormQC.TypeOfQC.Background, 1.40, 254.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/11/2009"), FormQC.TypeOfQC.Alpha, 11340.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/11/2009"), FormQC.TypeOfQC.Beta, 1, 32306.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/12/2009"), FormQC.TypeOfQC.Background, 1.40, 248.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/12/2009"), FormQC.TypeOfQC.Alpha, 11409.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/12/2009"), FormQC.TypeOfQC.Beta, 1, 32628.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/13/2009"), FormQC.TypeOfQC.Background, 1.10, 251.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/13/2009"), FormQC.TypeOfQC.Alpha, 11470.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/13/2009"), FormQC.TypeOfQC.Beta, 1, 32155.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/14/2009"), FormQC.TypeOfQC.Background, 0.80, 257.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/14/2009"), FormQC.TypeOfQC.Alpha, 11667.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/14/2009"), FormQC.TypeOfQC.Beta, 1, 32452.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/16/2009"), FormQC.TypeOfQC.Background, 1.50, 256.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/16/2009"), FormQC.TypeOfQC.Alpha, 11630.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/16/2009"), FormQC.TypeOfQC.Beta, 1, 32117.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/17/2009"), FormQC.TypeOfQC.Background, 1.00, 245.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/17/2009"), FormQC.TypeOfQC.Alpha, 11790.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/17/2009"), FormQC.TypeOfQC.Beta, 1, 32195.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/18/2009"), FormQC.TypeOfQC.Background, 1.40, 247.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/18/2009"), FormQC.TypeOfQC.Alpha, 11811.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/18/2009"), FormQC.TypeOfQC.Beta, 1, 32141.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/19/2009"), FormQC.TypeOfQC.Background, 1.20, 252.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/19/2009"), FormQC.TypeOfQC.Alpha, 11617.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/19/2009"), FormQC.TypeOfQC.Beta, 1, 31591.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/20/2009"), FormQC.TypeOfQC.Background, 1.70, 251.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/20/2009"), FormQC.TypeOfQC.Alpha, 11704.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/20/2009"), FormQC.TypeOfQC.Beta, 1, 32600.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/23/2009"), FormQC.TypeOfQC.Background, 1.90, 244.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/23/2009"), FormQC.TypeOfQC.Alpha, 11403.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/23/2009"), FormQC.TypeOfQC.Beta, 1, 32237.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/24/2009"), FormQC.TypeOfQC.Background, 1.70, 245.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/24/2009"), FormQC.TypeOfQC.Alpha, 11614.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/24/2009"), FormQC.TypeOfQC.Beta, 1, 31994.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/25/2009"), FormQC.TypeOfQC.Background, 1.00, 252.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/25/2009"), FormQC.TypeOfQC.Alpha, 11681.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/25/2009"), FormQC.TypeOfQC.Beta, 1, 32116.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/30/2009"), FormQC.TypeOfQC.Background, 0.90, 260.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/30/2009"), FormQC.TypeOfQC.Alpha, 11508.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("11/30/2009"), FormQC.TypeOfQC.Beta, 1, 32051.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/1/2009"), FormQC.TypeOfQC.Background, 1.40, 250.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/1/2009"), FormQC.TypeOfQC.Alpha, 12007.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/1/2009"), FormQC.TypeOfQC.Beta, 1, 32724.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/2/2009"), FormQC.TypeOfQC.Background, 1.30, 258.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/2/2009"), FormQC.TypeOfQC.Alpha, 11703.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/2/2009"), FormQC.TypeOfQC.Beta, 1, 32318.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/3/2009"), FormQC.TypeOfQC.Background, 0.60, 257.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/3/2009"), FormQC.TypeOfQC.Alpha, 11301.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/3/2009"), FormQC.TypeOfQC.Beta, 1, 32354.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/4/2009"), FormQC.TypeOfQC.Background, 1.00, 262.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/4/2009"), FormQC.TypeOfQC.Alpha, 11624.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/4/2009"), FormQC.TypeOfQC.Beta, 1, 31964.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/7/2009"), FormQC.TypeOfQC.Background, 0.90, 251.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/7/2009"), FormQC.TypeOfQC.Alpha, 11502.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/7/2009"), FormQC.TypeOfQC.Beta, 1, 32167.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/8/2009"), FormQC.TypeOfQC.Background, 1.00, 246.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/8/2009"), FormQC.TypeOfQC.Alpha, 11696.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/8/2009"), FormQC.TypeOfQC.Beta, 1, 32477.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/9/2009"), FormQC.TypeOfQC.Background, 1.50, 267.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/9/2009"), FormQC.TypeOfQC.Alpha, 11911.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/9/2009"), FormQC.TypeOfQC.Beta, 1, 32674.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/10/2009"), FormQC.TypeOfQC.Background, 1.60, 257.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/10/2009"), FormQC.TypeOfQC.Alpha, 11661.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/10/2009"), FormQC.TypeOfQC.Beta, 1, 32204.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/11/2009"), FormQC.TypeOfQC.Background, 1.00, 254.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/11/2009"), FormQC.TypeOfQC.Alpha, 11737.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/11/2009"), FormQC.TypeOfQC.Beta, 1, 32530.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/12/2009"), FormQC.TypeOfQC.Background, 1.00, 251.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/12/2009"), FormQC.TypeOfQC.Alpha, 11140.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/12/2009"), FormQC.TypeOfQC.Beta, 1, 32704.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/14/2009"), FormQC.TypeOfQC.Background, 1.00, 261.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/14/2009"), FormQC.TypeOfQC.Alpha, 11801.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/14/2009"), FormQC.TypeOfQC.Beta, 1, 32576.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/15/2009"), FormQC.TypeOfQC.Background, 1.30, 263.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/15/2009"), FormQC.TypeOfQC.Alpha, 11398.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/15/2009"), FormQC.TypeOfQC.Beta, 1, 32184.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/16/2009"), FormQC.TypeOfQC.Background, 0.90, 260.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/16/2009"), FormQC.TypeOfQC.Alpha, 11471.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/16/2009"), FormQC.TypeOfQC.Beta, 1, 32478.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/17/2009"), FormQC.TypeOfQC.Background, 1.90, 254.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/17/2009"), FormQC.TypeOfQC.Alpha, 11121.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/17/2009"), FormQC.TypeOfQC.Beta, 1, 32725.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/18/2009"), FormQC.TypeOfQC.Background, 1.20, 263.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/18/2009"), FormQC.TypeOfQC.Alpha, 11805.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/18/2009"), FormQC.TypeOfQC.Beta, 1, 32846.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/21/2009"), FormQC.TypeOfQC.Background, 1.00, 258.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/21/2009"), FormQC.TypeOfQC.Alpha, 11388.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/21/2009"), FormQC.TypeOfQC.Beta, 1, 32287.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/22/2009"), FormQC.TypeOfQC.Background, 1.50, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/22/2009"), FormQC.TypeOfQC.Alpha, 11502.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/22/2009"), FormQC.TypeOfQC.Beta, 1, 32630.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/23/2009"), FormQC.TypeOfQC.Background, 2.10, 250.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/23/2009"), FormQC.TypeOfQC.Alpha, 11737.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("12/23/2009"), FormQC.TypeOfQC.Beta, 1, 32564.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/5/2010"), FormQC.TypeOfQC.Background, 1.90, 261.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/5/2010"), FormQC.TypeOfQC.Alpha, 11477.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/5/2010"), FormQC.TypeOfQC.Beta, 1, 32059.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/6/2010"), FormQC.TypeOfQC.Background, 1.80, 253.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/6/2010"), FormQC.TypeOfQC.Alpha, 11326.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/6/2010"), FormQC.TypeOfQC.Beta, 1, 32183.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/7/2010"), FormQC.TypeOfQC.Background, 1.10, 256.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/7/2010"), FormQC.TypeOfQC.Alpha, 11628.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/7/2010"), FormQC.TypeOfQC.Beta, 1, 32348.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/8/2010"), FormQC.TypeOfQC.Background, 1.10, 255.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/8/2010"), FormQC.TypeOfQC.Alpha, 11444.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/8/2010"), FormQC.TypeOfQC.Beta, 1, 32355.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/11/2010"), FormQC.TypeOfQC.Background, 1.70, 250.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/11/2010"), FormQC.TypeOfQC.Alpha, 11816.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/11/2010"), FormQC.TypeOfQC.Beta, 1, 32263.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/12/2010"), FormQC.TypeOfQC.Background, 1.80, 249.20, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/12/2010"), FormQC.TypeOfQC.Alpha, 11159.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/12/2010"), FormQC.TypeOfQC.Beta, 1, 32431.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/13/2010"), FormQC.TypeOfQC.Background, 1.60, 246.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/13/2010"), FormQC.TypeOfQC.Alpha, 11572.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/13/2010"), FormQC.TypeOfQC.Beta, 1, 31838.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/14/2010"), FormQC.TypeOfQC.Background, 1.70, 254.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/14/2010"), FormQC.TypeOfQC.Alpha, 11762.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/14/2010"), FormQC.TypeOfQC.Beta, 1, 32289.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/15/2010"), FormQC.TypeOfQC.Background, 1.40, 256.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/15/2010"), FormQC.TypeOfQC.Alpha, 11428.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/15/2010"), FormQC.TypeOfQC.Beta, 1, 32166.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/16/2010"), FormQC.TypeOfQC.Background, 1.00, 254.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/16/2010"), FormQC.TypeOfQC.Alpha, 11670.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/16/2010"), FormQC.TypeOfQC.Beta, 1, 32382.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/17/2010"), FormQC.TypeOfQC.Background, 2.00, 258.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/17/2010"), FormQC.TypeOfQC.Alpha, 11847.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/17/2010"), FormQC.TypeOfQC.Beta, 1, 32434.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/18/2010"), FormQC.TypeOfQC.Background, 2.00, 250.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/18/2010"), FormQC.TypeOfQC.Alpha, 11777.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/18/2010"), FormQC.TypeOfQC.Beta, 1, 31935.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/19/2010"), FormQC.TypeOfQC.Background, 1.80, 256.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/19/2010"), FormQC.TypeOfQC.Alpha, 11531.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/19/2010"), FormQC.TypeOfQC.Beta, 1, 32405.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/20/2010"), FormQC.TypeOfQC.Background, 1.90, 247.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/20/2010"), FormQC.TypeOfQC.Alpha, 11659.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/20/2010"), FormQC.TypeOfQC.Beta, 1, 31131.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/21/2010"), FormQC.TypeOfQC.Background, 1.10, 256.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/21/2010"), FormQC.TypeOfQC.Alpha, 11752.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/21/2010"), FormQC.TypeOfQC.Beta, 1, 32572.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/22/2010"), FormQC.TypeOfQC.Background, 1.10, 253.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/22/2010"), FormQC.TypeOfQC.Alpha, 11830.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/22/2010"), FormQC.TypeOfQC.Beta, 1, 32436.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/25/2010"), FormQC.TypeOfQC.Background, 1.30, 259.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/25/2010"), FormQC.TypeOfQC.Alpha, 11907.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/25/2010"), FormQC.TypeOfQC.Beta, 1, 32796.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/26/2010"), FormQC.TypeOfQC.Background, 1.30, 258.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/26/2010"), FormQC.TypeOfQC.Alpha, 11444.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/26/2010"), FormQC.TypeOfQC.Beta, 1, 32478.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/27/2010"), FormQC.TypeOfQC.Background, 1.80, 249.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/27/2010"), FormQC.TypeOfQC.Alpha, 11374.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/27/2010"), FormQC.TypeOfQC.Beta, 1, 32578.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/28/2010"), FormQC.TypeOfQC.Background, 1.20, 254.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/28/2010"), FormQC.TypeOfQC.Alpha, 11457.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/28/2010"), FormQC.TypeOfQC.Beta, 1, 32675.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/29/2010"), FormQC.TypeOfQC.Background, 0.50, 250.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/29/2010"), FormQC.TypeOfQC.Alpha, 11350.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("1/29/2010"), FormQC.TypeOfQC.Beta, 1, 32464.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/1/2010"), FormQC.TypeOfQC.Background, 1.50, 244.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/1/2010"), FormQC.TypeOfQC.Alpha, 11509.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/1/2010"), FormQC.TypeOfQC.Beta, 1, 32332.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/2/2010"), FormQC.TypeOfQC.Background, 1.00, 251.20, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/2/2010"), FormQC.TypeOfQC.Alpha, 11692.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/2/2010"), FormQC.TypeOfQC.Beta, 1, 32426.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/3/2010"), FormQC.TypeOfQC.Background, 1.60, 246.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/3/2010"), FormQC.TypeOfQC.Alpha, 11620.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/3/2010"), FormQC.TypeOfQC.Beta, 1, 32228.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/4/2010"), FormQC.TypeOfQC.Background, 1.40, 257.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/4/2010"), FormQC.TypeOfQC.Alpha, 11432.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/4/2010"), FormQC.TypeOfQC.Beta, 1, 32137.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/8/2010"), FormQC.TypeOfQC.Background, 1.80, 253.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/8/2010"), FormQC.TypeOfQC.Alpha, 11510.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/8/2010"), FormQC.TypeOfQC.Beta, 1, 32164.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/9/2010"), FormQC.TypeOfQC.Background, 1.70, 267.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/9/2010"), FormQC.TypeOfQC.Alpha, 11700.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/9/2010"), FormQC.TypeOfQC.Beta, 1, 32147.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/10/2010"), FormQC.TypeOfQC.Background, 2.00, 255.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/10/2010"), FormQC.TypeOfQC.Alpha, 11889.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/10/2010"), FormQC.TypeOfQC.Beta, 1, 32232.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/11/2010"), FormQC.TypeOfQC.Background, 1.20, 264.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/11/2010"), FormQC.TypeOfQC.Alpha, 11459.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/11/2010"), FormQC.TypeOfQC.Beta, 1, 32154.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/12/2010"), FormQC.TypeOfQC.Background, 1.30, 255.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/12/2010"), FormQC.TypeOfQC.Alpha, 11525.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/12/2010"), FormQC.TypeOfQC.Beta, 1, 32188.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/15/2010"), FormQC.TypeOfQC.Background, 1.20, 262.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/15/2010"), FormQC.TypeOfQC.Alpha, 11669.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/15/2010"), FormQC.TypeOfQC.Beta, 1, 32185.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/16/2010"), FormQC.TypeOfQC.Background, 1.00, 254.90, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/16/2010"), FormQC.TypeOfQC.Alpha, 11792.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/16/2010"), FormQC.TypeOfQC.Beta, 1, 32419.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/17/2010"), FormQC.TypeOfQC.Background, 1.30, 251.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/17/2010"), FormQC.TypeOfQC.Alpha, 11550.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/17/2010"), FormQC.TypeOfQC.Beta, 1, 32044.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/18/2010"), FormQC.TypeOfQC.Background, 1.60, 254.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/18/2010"), FormQC.TypeOfQC.Alpha, 11774.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/18/2010"), FormQC.TypeOfQC.Beta, 1, 32442.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/19/2010"), FormQC.TypeOfQC.Background, 1.20, 258.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/19/2010"), FormQC.TypeOfQC.Alpha, 11270.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/19/2010"), FormQC.TypeOfQC.Beta, 1, 32086.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/22/2010"), FormQC.TypeOfQC.Background, 1.20, 252.20, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/22/2010"), FormQC.TypeOfQC.Alpha, 11582.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/22/2010"), FormQC.TypeOfQC.Beta, 1, 32340.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/23/2010"), FormQC.TypeOfQC.Background, 1.20, 264.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/23/2010"), FormQC.TypeOfQC.Alpha, 11785.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/23/2010"), FormQC.TypeOfQC.Beta, 1, 32192.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/24/2010"), FormQC.TypeOfQC.Background, 1.90, 257.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/24/2010"), FormQC.TypeOfQC.Alpha, 11555.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/24/2010"), FormQC.TypeOfQC.Beta, 1, 32318.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/25/2010"), FormQC.TypeOfQC.Background, 1.30, 241.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/25/2010"), FormQC.TypeOfQC.Alpha, 11796.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/25/2010"), FormQC.TypeOfQC.Beta, 1, 32100.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/26/2010"), FormQC.TypeOfQC.Background, 1.30, 252.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/26/2010"), FormQC.TypeOfQC.Alpha, 11756.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/26/2010"), FormQC.TypeOfQC.Beta, 1, 32234.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/27/2010"), FormQC.TypeOfQC.Background, 1.10, 260.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/27/2010"), FormQC.TypeOfQC.Alpha, 11642.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("2/27/2010"), FormQC.TypeOfQC.Beta, 1, 34142.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/1/2010"), FormQC.TypeOfQC.Background, 1.50, 262.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/1/2010"), FormQC.TypeOfQC.Alpha, 11593.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/1/2010"), FormQC.TypeOfQC.Beta, 1, 32450.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/2/2010"), FormQC.TypeOfQC.Background, 1.00, 253.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/2/2010"), FormQC.TypeOfQC.Alpha, 11714.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/2/2010"), FormQC.TypeOfQC.Beta, 1, 32493.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/3/2010"), FormQC.TypeOfQC.Background, 2.00, 259.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/3/2010"), FormQC.TypeOfQC.Alpha, 11635.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/3/2010"), FormQC.TypeOfQC.Beta, 1, 32486.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/4/2010"), FormQC.TypeOfQC.Background, 1.50, 255.20, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/4/2010"), FormQC.TypeOfQC.Alpha, 11310.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/4/2010"), FormQC.TypeOfQC.Beta, 1, 32583.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/5/2010"), FormQC.TypeOfQC.Background, 1.50, 256.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/5/2010"), FormQC.TypeOfQC.Alpha, 11333.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/5/2010"), FormQC.TypeOfQC.Beta, 1, 32077.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/8/2010"), FormQC.TypeOfQC.Background, 1.2, 246.1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/8/2010"), FormQC.TypeOfQC.Alpha, 11743.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/8/2010"), FormQC.TypeOfQC.Beta, 1, 32281.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/9/2010"), FormQC.TypeOfQC.Background, 1.20, 258.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/9/2010"), FormQC.TypeOfQC.Alpha, 11728.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/9/2010"), FormQC.TypeOfQC.Beta, 1, 32433.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/10/2010"), FormQC.TypeOfQC.Background, 0.80, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/10/2010"), FormQC.TypeOfQC.Alpha, 11860.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/10/2010"), FormQC.TypeOfQC.Beta, 1, 32266.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/11/2010"), FormQC.TypeOfQC.Background, 1.70, 254.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/11/2010"), FormQC.TypeOfQC.Alpha, 11814.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/11/2010"), FormQC.TypeOfQC.Beta, 1, 32601.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/12/2010"), FormQC.TypeOfQC.Background, 1.10, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/12/2010"), FormQC.TypeOfQC.Alpha, 11913.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/12/2010"), FormQC.TypeOfQC.Beta, 1, 32495.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/13/2010"), FormQC.TypeOfQC.Background, 1.40, 258.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/13/2010"), FormQC.TypeOfQC.Alpha, 11736.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/13/2010"), FormQC.TypeOfQC.Beta, 1, 32295.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/15/2010"), FormQC.TypeOfQC.Background, 0.90, 247.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/15/2010"), FormQC.TypeOfQC.Alpha, 11536.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/15/2010"), FormQC.TypeOfQC.Beta, 1, 32053.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/16/2010"), FormQC.TypeOfQC.Background, 0.40, 247.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/16/2010"), FormQC.TypeOfQC.Alpha, 11531.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/16/2010"), FormQC.TypeOfQC.Beta, 1, 32350.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/17/2010"), FormQC.TypeOfQC.Background, 1.40, 247.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/17/2010"), FormQC.TypeOfQC.Alpha, 11242.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/17/2010"), FormQC.TypeOfQC.Beta, 1, 32383.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/18/2010"), FormQC.TypeOfQC.Background, 1.50, 248.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/18/2010"), FormQC.TypeOfQC.Alpha, 11592.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/18/2010"), FormQC.TypeOfQC.Beta, 1, 32466.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/19/2010"), FormQC.TypeOfQC.Background, 0.90, 246.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/19/2010"), FormQC.TypeOfQC.Alpha, 11810.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/19/2010"), FormQC.TypeOfQC.Beta, 1, 32211.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/20/2010"), FormQC.TypeOfQC.Background, 0.90, 243.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/20/2010"), FormQC.TypeOfQC.Alpha, 11797.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/20/2010"), FormQC.TypeOfQC.Beta, 1, 32100.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/22/2010"), FormQC.TypeOfQC.Background, 1.50, 247.20, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/22/2010"), FormQC.TypeOfQC.Alpha, 11107.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/22/2010"), FormQC.TypeOfQC.Beta, 1, 32232.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/23/2010"), FormQC.TypeOfQC.Background, 1.00, 255.30, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/23/2010"), FormQC.TypeOfQC.Alpha, 11307.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/23/2010"), FormQC.TypeOfQC.Beta, 1, 32251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/24/2010"), FormQC.TypeOfQC.Background, 1.40, 244.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/24/2010"), FormQC.TypeOfQC.Alpha, 11347.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/24/2010"), FormQC.TypeOfQC.Beta, 1, 31933.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/25/2010"), FormQC.TypeOfQC.Background, 1.70, 248.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/25/2010"), FormQC.TypeOfQC.Alpha, 11983.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/25/2010"), FormQC.TypeOfQC.Beta, 1, 32344.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/26/2010"), FormQC.TypeOfQC.Background, 1.30, 250.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/26/2010"), FormQC.TypeOfQC.Alpha, 11477.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/26/2010"), FormQC.TypeOfQC.Beta, 1, 32142.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/29/2010"), FormQC.TypeOfQC.Background, 1.20, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/29/2010"), FormQC.TypeOfQC.Alpha, 11620.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/29/2010"), FormQC.TypeOfQC.Beta, 1, 32083.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/30/2010"), FormQC.TypeOfQC.Background, 1.20, 247.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/30/2010"), FormQC.TypeOfQC.Alpha, 11313.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/30/2010"), FormQC.TypeOfQC.Beta, 1, 32056.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/31/2010"), FormQC.TypeOfQC.Background, 1.20, 247.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/31/2010"), FormQC.TypeOfQC.Alpha, 11833.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("3/31/2010"), FormQC.TypeOfQC.Beta, 1, 32010.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/1/2010"), FormQC.TypeOfQC.Background, 1.40, 254.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/1/2010"), FormQC.TypeOfQC.Alpha, 11618.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/1/2010"), FormQC.TypeOfQC.Beta, 1, 32050.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/2/2010"), FormQC.TypeOfQC.Background, 1.80, 251.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/2/2010"), FormQC.TypeOfQC.Alpha, 11940.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/2/2010"), FormQC.TypeOfQC.Beta, 1, 32550.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/5/2010"), FormQC.TypeOfQC.Background, 1.10, 249.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/5/2010"), FormQC.TypeOfQC.Alpha, 11579.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/5/2010"), FormQC.TypeOfQC.Beta, 1, 32068.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/6/2010"), FormQC.TypeOfQC.Background, 1.00, 256.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/6/2010"), FormQC.TypeOfQC.Alpha, 11799.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/6/2010"), FormQC.TypeOfQC.Beta, 1, 31926.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/7/2010"), FormQC.TypeOfQC.Background, 1.00, 254.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/7/2010"), FormQC.TypeOfQC.Alpha, 11729.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/7/2010"), FormQC.TypeOfQC.Beta, 1, 32244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/8/2010"), FormQC.TypeOfQC.Background, 1.30, 247.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/8/2010"), FormQC.TypeOfQC.Alpha, 11404.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/8/2010"), FormQC.TypeOfQC.Beta, 1, 32008.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/9/2010"), FormQC.TypeOfQC.Background, 1.10, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/9/2010"), FormQC.TypeOfQC.Alpha, 11532.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/9/2010"), FormQC.TypeOfQC.Beta, 1, 32178.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/12/2010"), FormQC.TypeOfQC.Background, 0.40, 248.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/12/2010"), FormQC.TypeOfQC.Alpha, 11150.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/12/2010"), FormQC.TypeOfQC.Beta, 1, 31969.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/13/2010"), FormQC.TypeOfQC.Background, 1.00, 247.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/13/2010"), FormQC.TypeOfQC.Alpha, 11249.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/13/2010"), FormQC.TypeOfQC.Beta, 1, 32055.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/14/2010"), FormQC.TypeOfQC.Background, 1.70, 234.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/14/2010"), FormQC.TypeOfQC.Alpha, 11548.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/14/2010"), FormQC.TypeOfQC.Beta, 1, 32017.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/15/2010"), FormQC.TypeOfQC.Background, 1.50, 245.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/15/2010"), FormQC.TypeOfQC.Alpha, 11313.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/15/2010"), FormQC.TypeOfQC.Beta, 1, 32388.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/16/2010"), FormQC.TypeOfQC.Background, 1.50, 236.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/16/2010"), FormQC.TypeOfQC.Alpha, 11849.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/16/2010"), FormQC.TypeOfQC.Beta, 1, 32477.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/19/2010"), FormQC.TypeOfQC.Background, 1.60, 249.70, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/19/2010"), FormQC.TypeOfQC.Alpha, 11577.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/19/2010"), FormQC.TypeOfQC.Beta, 1, 32227.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/20/2010"), FormQC.TypeOfQC.Background, 1.50, 242.50, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/20/2010"), FormQC.TypeOfQC.Alpha, 11678.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/20/2010"), FormQC.TypeOfQC.Beta, 1, 32229.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/21/2010"), FormQC.TypeOfQC.Background, 1.90, 248.40, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/21/2010"), FormQC.TypeOfQC.Alpha, 11636.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/21/2010"), FormQC.TypeOfQC.Beta, 1, 31797.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/22/2010"), FormQC.TypeOfQC.Background, 1.20, 246.10, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/22/2010"), FormQC.TypeOfQC.Alpha, 11688.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/22/2010"), FormQC.TypeOfQC.Beta, 1, 32103.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/23/2010"), FormQC.TypeOfQC.Background, 1.60, 251.60, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/23/2010"), FormQC.TypeOfQC.Alpha, 11818.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/23/2010"), FormQC.TypeOfQC.Beta, 1, 32267.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/26/2010"), FormQC.TypeOfQC.Background, 1.50, 256.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/26/2010"), FormQC.TypeOfQC.Alpha, 11680.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/26/2010"), FormQC.TypeOfQC.Beta, 1, 31679.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/27/2010"), FormQC.TypeOfQC.Background, 0.80, 248.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/27/2010"), FormQC.TypeOfQC.Alpha, 11487.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/27/2010"), FormQC.TypeOfQC.Beta, 1, 30194.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/28/2010"), FormQC.TypeOfQC.Background, 1.30, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/28/2010"), FormQC.TypeOfQC.Alpha, 11399.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/28/2010"), FormQC.TypeOfQC.Beta, 1, 32176.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/29/2010"), FormQC.TypeOfQC.Background, 1.40, 244.80, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/29/2010"), FormQC.TypeOfQC.Alpha, 11409.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("4/29/2010"), FormQC.TypeOfQC.Beta, 1, 31847.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/3/2010"), FormQC.TypeOfQC.Background, 1.90, 245.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/3/2010"), FormQC.TypeOfQC.Alpha, 11856.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/3/2010"), FormQC.TypeOfQC.Beta, 1, 32288.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/4/2010"), FormQC.TypeOfQC.Background, 1.90, 243.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/4/2010"), FormQC.TypeOfQC.Alpha, 12006.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/4/2010"), FormQC.TypeOfQC.Beta, 1, 32476.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/5/2010"), FormQC.TypeOfQC.Background, 1.30, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/5/2010"), FormQC.TypeOfQC.Alpha, 12208.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/5/2010"), FormQC.TypeOfQC.Beta, 1, 32369.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/6/2010"), FormQC.TypeOfQC.Background, 1.20, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/6/2010"), FormQC.TypeOfQC.Alpha, 11887.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/6/2010"), FormQC.TypeOfQC.Beta, 1, 32255.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/7/2010"), FormQC.TypeOfQC.Background, 1.10, 264.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/7/2010"), FormQC.TypeOfQC.Alpha, 11910.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/7/2010"), FormQC.TypeOfQC.Beta, 1, 32331.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/10/2010"), FormQC.TypeOfQC.Background, 0.90, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/10/2010"), FormQC.TypeOfQC.Alpha, 11688.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/10/2010"), FormQC.TypeOfQC.Beta, 1, 32048.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/11/2010"), FormQC.TypeOfQC.Background, 0.60, 255.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/11/2010"), FormQC.TypeOfQC.Alpha, 11876.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/11/2010"), FormQC.TypeOfQC.Beta, 1, 32149.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/12/2010"), FormQC.TypeOfQC.Background, 1.50, 257.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/12/2010"), FormQC.TypeOfQC.Alpha, 11719.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/12/2010"), FormQC.TypeOfQC.Beta, 1, 32054.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/13/2010"), FormQC.TypeOfQC.Background, 1.20, 259.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/13/2010"), FormQC.TypeOfQC.Alpha, 11834.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/13/2010"), FormQC.TypeOfQC.Beta, 1, 32531.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/14/2010"), FormQC.TypeOfQC.Background, 1.10, 252.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/14/2010"), FormQC.TypeOfQC.Alpha, 11803.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/14/2010"), FormQC.TypeOfQC.Beta, 1, 31946.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/17/2010"), FormQC.TypeOfQC.Background, 1.40, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/17/2010"), FormQC.TypeOfQC.Alpha, 12167.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/17/2010"), FormQC.TypeOfQC.Beta, 1, 32014.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/18/2010"), FormQC.TypeOfQC.Background, 1.00, 260.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/18/2010"), FormQC.TypeOfQC.Alpha, 11748.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/18/2010"), FormQC.TypeOfQC.Beta, 1, 32290.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/19/2010"), FormQC.TypeOfQC.Background, 1.20, 254.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/19/2010"), FormQC.TypeOfQC.Alpha, 11914.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/19/2010"), FormQC.TypeOfQC.Beta, 1, 32298.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/20/2010"), FormQC.TypeOfQC.Background, 0.70, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/20/2010"), FormQC.TypeOfQC.Alpha, 11983.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/20/2010"), FormQC.TypeOfQC.Beta, 1, 32369.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/21/2010"), FormQC.TypeOfQC.Background, 1.10, 257.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/21/2010"), FormQC.TypeOfQC.Alpha, 11680.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/21/2010"), FormQC.TypeOfQC.Beta, 1, 31948.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/24/2010"), FormQC.TypeOfQC.Background, 1.00, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/24/2010"), FormQC.TypeOfQC.Alpha, 11980.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/24/2010"), FormQC.TypeOfQC.Beta, 1, 31943.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/25/2010"), FormQC.TypeOfQC.Background, 1.50, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/25/2010"), FormQC.TypeOfQC.Alpha, 11797.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/25/2010"), FormQC.TypeOfQC.Beta, 1, 31857.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/26/2010"), FormQC.TypeOfQC.Background, 1.00, 241.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/26/2010"), FormQC.TypeOfQC.Alpha, 11791.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/26/2010"), FormQC.TypeOfQC.Beta, 1, 32095.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/27/2010"), FormQC.TypeOfQC.Background, 1.70, 236.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/27/2010"), FormQC.TypeOfQC.Alpha, 11618.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/27/2010"), FormQC.TypeOfQC.Beta, 1, 31829.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/28/2010"), FormQC.TypeOfQC.Background, 0.70, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/28/2010"), FormQC.TypeOfQC.Alpha, 11789.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("5/28/2010"), FormQC.TypeOfQC.Beta, 1, 32094.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/1/2010"), FormQC.TypeOfQC.Background, 1.50, 237.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/1/2010"), FormQC.TypeOfQC.Alpha, 11934.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/1/2010"), FormQC.TypeOfQC.Beta, 1, 32532.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/2/2010"), FormQC.TypeOfQC.Background, 1.60, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/2/2010"), FormQC.TypeOfQC.Alpha, 11838.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/2/2010"), FormQC.TypeOfQC.Beta, 1, 32267.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/3/2010"), FormQC.TypeOfQC.Background, 0.60, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/3/2010"), FormQC.TypeOfQC.Alpha, 11952.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/3/2010"), FormQC.TypeOfQC.Beta, 1, 31960.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/4/2010"), FormQC.TypeOfQC.Background, 1.30, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/4/2010"), FormQC.TypeOfQC.Alpha, 12029.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/4/2010"), FormQC.TypeOfQC.Beta, 1, 31862.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/7/2010"), FormQC.TypeOfQC.Background, 1.30, 245.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/7/2010"), FormQC.TypeOfQC.Alpha, 11953.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/7/2010"), FormQC.TypeOfQC.Beta, 1, 31809.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/8/2010"), FormQC.TypeOfQC.Background, 1.50, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/8/2010"), FormQC.TypeOfQC.Alpha, 11983.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/8/2010"), FormQC.TypeOfQC.Beta, 1, 31980.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/9/2010"), FormQC.TypeOfQC.Background, 1.20, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/9/2010"), FormQC.TypeOfQC.Alpha, 11863.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/9/2010"), FormQC.TypeOfQC.Beta, 1, 32314.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/10/2010"), FormQC.TypeOfQC.Background, 1.10, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/10/2010"), FormQC.TypeOfQC.Alpha, 11450.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/10/2010"), FormQC.TypeOfQC.Beta, 1, 32238.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/11/2010"), FormQC.TypeOfQC.Background, 0.90, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/11/2010"), FormQC.TypeOfQC.Alpha, 11931.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/11/2010"), FormQC.TypeOfQC.Beta, 1, 32168.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/14/2010"), FormQC.TypeOfQC.Background, 1.10, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/14/2010"), FormQC.TypeOfQC.Alpha, 11797.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/14/2010"), FormQC.TypeOfQC.Beta, 1, 32059.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/15/2010"), FormQC.TypeOfQC.Background, 2.60, 239.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/15/2010"), FormQC.TypeOfQC.Alpha, 11865.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/15/2010"), FormQC.TypeOfQC.Beta, 1, 31984.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/16/2010"), FormQC.TypeOfQC.Background, 1.50, 236.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/16/2010"), FormQC.TypeOfQC.Alpha, 11793.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/16/2010"), FormQC.TypeOfQC.Beta, 1, 32068.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/17/2010"), FormQC.TypeOfQC.Background, 1.60, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/17/2010"), FormQC.TypeOfQC.Alpha, 11762.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/17/2010"), FormQC.TypeOfQC.Beta, 1, 31832.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/18/2010"), FormQC.TypeOfQC.Background, 1.50, 245.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/18/2010"), FormQC.TypeOfQC.Alpha, 11825.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/18/2010"), FormQC.TypeOfQC.Beta, 1, 31986.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/21/2010"), FormQC.TypeOfQC.Background, 1.20, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/21/2010"), FormQC.TypeOfQC.Alpha, 12129.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/21/2010"), FormQC.TypeOfQC.Beta, 1, 32414.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/22/2010"), FormQC.TypeOfQC.Background, 1.00, 252.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/22/2010"), FormQC.TypeOfQC.Alpha, 11777.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/22/2010"), FormQC.TypeOfQC.Beta, 1, 32142.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/23/2010"), FormQC.TypeOfQC.Background, 1.00, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/23/2010"), FormQC.TypeOfQC.Alpha, 11687.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/23/2010"), FormQC.TypeOfQC.Beta, 1, 32232.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/24/2010"), FormQC.TypeOfQC.Background, 2.10, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/24/2010"), FormQC.TypeOfQC.Alpha, 11890.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/24/2010"), FormQC.TypeOfQC.Beta, 1, 32028.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/25/2010"), FormQC.TypeOfQC.Background, 1.60, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/25/2010"), FormQC.TypeOfQC.Alpha, 11919.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/25/2010"), FormQC.TypeOfQC.Beta, 1, 31734.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/28/2010"), FormQC.TypeOfQC.Background, 1.80, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/28/2010"), FormQC.TypeOfQC.Alpha, 11929.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/28/2010"), FormQC.TypeOfQC.Beta, 1, 32260.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/29/2010"), FormQC.TypeOfQC.Background, 1.10, 245.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/29/2010"), FormQC.TypeOfQC.Alpha, 11951.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/29/2010"), FormQC.TypeOfQC.Beta, 1, 32318.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/30/2010"), FormQC.TypeOfQC.Background, 2.20, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/30/2010"), FormQC.TypeOfQC.Alpha, 11742.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("6/30/2010"), FormQC.TypeOfQC.Beta, 1, 32092.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/1/2010"), FormQC.TypeOfQC.Background, 1.10, 232.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/1/2010"), FormQC.TypeOfQC.Alpha, 11670.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/1/2010"), FormQC.TypeOfQC.Beta, 1, 32134.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/2/2010"), FormQC.TypeOfQC.Background, 3.00, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/2/2010"), FormQC.TypeOfQC.Alpha, 11699.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/2/2010"), FormQC.TypeOfQC.Beta, 1, 32192.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/6/2010"), FormQC.TypeOfQC.Background, 2.60, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/6/2010"), FormQC.TypeOfQC.Alpha, 11995.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/6/2010"), FormQC.TypeOfQC.Beta, 1, 32208.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/7/2010"), FormQC.TypeOfQC.Background, 1.90, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/7/2010"), FormQC.TypeOfQC.Alpha, 11839.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/7/2010"), FormQC.TypeOfQC.Beta, 1, 31881.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/8/2010"), FormQC.TypeOfQC.Background, 1.40, 240.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/8/2010"), FormQC.TypeOfQC.Alpha, 12216.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/8/2010"), FormQC.TypeOfQC.Beta, 1, 31890.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/9/2010"), FormQC.TypeOfQC.Background, 1.90, 235.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/9/2010"), FormQC.TypeOfQC.Alpha, 11921.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/9/2010"), FormQC.TypeOfQC.Beta, 1, 32035.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/12/2010"), FormQC.TypeOfQC.Background, 2.00, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/12/2010"), FormQC.TypeOfQC.Alpha, 11964.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/12/2010"), FormQC.TypeOfQC.Beta, 1, 32198.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/13/2010"), FormQC.TypeOfQC.Background, 1.20, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/13/2010"), FormQC.TypeOfQC.Alpha, 11944.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/13/2010"), FormQC.TypeOfQC.Beta, 1, 32002.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/14/2010"), FormQC.TypeOfQC.Background, 1.50, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/14/2010"), FormQC.TypeOfQC.Alpha, 11694.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/14/2010"), FormQC.TypeOfQC.Beta, 1, 31856.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/15/2010"), FormQC.TypeOfQC.Background, 3.10, 240.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/15/2010"), FormQC.TypeOfQC.Alpha, 11821.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/15/2010"), FormQC.TypeOfQC.Beta, 1, 31898.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/16/2010"), FormQC.TypeOfQC.Background, 1.60, 240.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/16/2010"), FormQC.TypeOfQC.Alpha, 11940.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/16/2010"), FormQC.TypeOfQC.Beta, 1, 31607.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/17/2010"), FormQC.TypeOfQC.Background, 1.30, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/17/2010"), FormQC.TypeOfQC.Alpha, 11880.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/17/2010"), FormQC.TypeOfQC.Beta, 1, 31666.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/19/2010"), FormQC.TypeOfQC.Background, 3.20, 241.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/19/2010"), FormQC.TypeOfQC.Alpha, 11818.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/19/2010"), FormQC.TypeOfQC.Beta, 1, 31996.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/20/2010"), FormQC.TypeOfQC.Background, 1.60, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/20/2010"), FormQC.TypeOfQC.Alpha, 11900.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/20/2010"), FormQC.TypeOfQC.Beta, 1, 32071.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/21/2010"), FormQC.TypeOfQC.Background, 1.80, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/21/2010"), FormQC.TypeOfQC.Alpha, 11857.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/21/2010"), FormQC.TypeOfQC.Beta, 1, 31836.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/22/2010"), FormQC.TypeOfQC.Background, 1.60, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/22/2010"), FormQC.TypeOfQC.Alpha, 12121.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/22/2010"), FormQC.TypeOfQC.Beta, 1, 32284.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/23/2010"), FormQC.TypeOfQC.Background, 1.70, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/23/2010"), FormQC.TypeOfQC.Alpha, 12014.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/23/2010"), FormQC.TypeOfQC.Beta, 1, 31905.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/26/2010"), FormQC.TypeOfQC.Background, 1.30, 241.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/26/2010"), FormQC.TypeOfQC.Alpha, 11867.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/26/2010"), FormQC.TypeOfQC.Beta, 1, 32219.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/27/2010"), FormQC.TypeOfQC.Background, 2.00, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/27/2010"), FormQC.TypeOfQC.Alpha, 11739.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/27/2010"), FormQC.TypeOfQC.Beta, 1, 31789.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/28/2010"), FormQC.TypeOfQC.Background, 2.30, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/28/2010"), FormQC.TypeOfQC.Alpha, 12128.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/28/2010"), FormQC.TypeOfQC.Beta, 1, 32091.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/29/2010"), FormQC.TypeOfQC.Background, 2.10, 255.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/29/2010"), FormQC.TypeOfQC.Alpha, 11843.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/29/2010"), FormQC.TypeOfQC.Beta, 1, 31882.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/30/2010"), FormQC.TypeOfQC.Background, 1.40, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/30/2010"), FormQC.TypeOfQC.Alpha, 11939.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/30/2010"), FormQC.TypeOfQC.Beta, 1, 32202.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/2/2010"), FormQC.TypeOfQC.Background, 1.30, 254.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/2/2010"), FormQC.TypeOfQC.Alpha, 11963.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/2/2010"), FormQC.TypeOfQC.Beta, 1, 32032.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/3/2010"), FormQC.TypeOfQC.Background, 0.90, 243.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/3/2010"), FormQC.TypeOfQC.Alpha, 11768.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/3/2010"), FormQC.TypeOfQC.Beta, 1, 31859.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/4/2010"), FormQC.TypeOfQC.Background, 1.50, 238.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/4/2010"), FormQC.TypeOfQC.Alpha, 12099.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/4/2010"), FormQC.TypeOfQC.Beta, 1, 32379.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/5/2010"), FormQC.TypeOfQC.Background, 0.70, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/5/2010"), FormQC.TypeOfQC.Alpha, 11871.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/5/2010"), FormQC.TypeOfQC.Beta, 1, 30830.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/6/2010"), FormQC.TypeOfQC.Background, 1.20, 246.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/6/2010"), FormQC.TypeOfQC.Alpha, 11890.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/6/2010"), FormQC.TypeOfQC.Beta, 1, 32044.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/9/2010"), FormQC.TypeOfQC.Background, 1.10, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/9/2010"), FormQC.TypeOfQC.Alpha, 11820.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/9/2010"), FormQC.TypeOfQC.Beta, 1, 32301.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/10/2010"), FormQC.TypeOfQC.Background, 1.70, 243.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/10/2010"), FormQC.TypeOfQC.Alpha, 11590.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/10/2010"), FormQC.TypeOfQC.Beta, 1, 31860.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/11/2010"), FormQC.TypeOfQC.Background, 1.30, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/11/2010"), FormQC.TypeOfQC.Alpha, 11982.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/11/2010"), FormQC.TypeOfQC.Beta, 1, 31843.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/12/2010"), FormQC.TypeOfQC.Background, 0.90, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/12/2010"), FormQC.TypeOfQC.Alpha, 11960.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/12/2010"), FormQC.TypeOfQC.Beta, 1, 32380.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/13/2010"), FormQC.TypeOfQC.Background, 1.20, 254.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/13/2010"), FormQC.TypeOfQC.Alpha, 11949.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/13/2010"), FormQC.TypeOfQC.Beta, 1, 31653.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/16/2010"), FormQC.TypeOfQC.Background, 1.90, 260.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/16/2010"), FormQC.TypeOfQC.Alpha, 11813.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/16/2010"), FormQC.TypeOfQC.Beta, 1, 32024.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/17/2010"), FormQC.TypeOfQC.Background, 1.70, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/17/2010"), FormQC.TypeOfQC.Alpha, 11732.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/17/2010"), FormQC.TypeOfQC.Beta, 1, 32445.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/18/2010"), FormQC.TypeOfQC.Background, 0.80, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/18/2010"), FormQC.TypeOfQC.Alpha, 11903.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/18/2010"), FormQC.TypeOfQC.Beta, 1, 32239.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/19/2010"), FormQC.TypeOfQC.Background, 1.60, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/19/2010"), FormQC.TypeOfQC.Alpha, 11999.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/19/2010"), FormQC.TypeOfQC.Beta, 1, 31980.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/20/2010"), FormQC.TypeOfQC.Background, 1.20, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/20/2010"), FormQC.TypeOfQC.Alpha, 11781.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/20/2010"), FormQC.TypeOfQC.Beta, 1, 31898.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/23/2010"), FormQC.TypeOfQC.Background, 0.80, 235.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/23/2010"), FormQC.TypeOfQC.Alpha, 11815.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/23/2010"), FormQC.TypeOfQC.Beta, 1, 31644.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/24/2010"), FormQC.TypeOfQC.Background, 1.60, 252.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/24/2010"), FormQC.TypeOfQC.Alpha, 11694.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/24/2010"), FormQC.TypeOfQC.Beta, 1, 32290.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/25/2010"), FormQC.TypeOfQC.Background, 1.80, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/25/2010"), FormQC.TypeOfQC.Alpha, 11825.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/25/2010"), FormQC.TypeOfQC.Beta, 1, 31860.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/26/2010"), FormQC.TypeOfQC.Background, 1.20, 248.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/26/2010"), FormQC.TypeOfQC.Alpha, 11709.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/26/2010"), FormQC.TypeOfQC.Beta, 1, 31887.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/27/2010"), FormQC.TypeOfQC.Background, 1.70, 241.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/27/2010"), FormQC.TypeOfQC.Alpha, 11918.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/27/2010"), FormQC.TypeOfQC.Beta, 1, 31847.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/30/2010"), FormQC.TypeOfQC.Background, 1.60, 240.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/30/2010"), FormQC.TypeOfQC.Alpha, 11862.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("8/30/2010"), FormQC.TypeOfQC.Beta, 1, 32294.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/1/2010"), FormQC.TypeOfQC.Background, 1.90, 240.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/1/2010"), FormQC.TypeOfQC.Alpha, 12109.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/1/2010"), FormQC.TypeOfQC.Beta, 1, 32201.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/2/2010"), FormQC.TypeOfQC.Background, 0.90, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/2/2010"), FormQC.TypeOfQC.Alpha, 11708.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/2/2010"), FormQC.TypeOfQC.Beta, 1, 32067.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/3/2010"), FormQC.TypeOfQC.Background, 1.20, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/3/2010"), FormQC.TypeOfQC.Alpha, 11804.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/3/2010"), FormQC.TypeOfQC.Beta, 1, 32046.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/7/2010"), FormQC.TypeOfQC.Background, 1.40, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/7/2010"), FormQC.TypeOfQC.Alpha, 11862.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/7/2010"), FormQC.TypeOfQC.Beta, 1, 31564.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/8/2010"), FormQC.TypeOfQC.Background, 1.90, 255.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/8/2010"), FormQC.TypeOfQC.Alpha, 11582.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/8/2010"), FormQC.TypeOfQC.Beta, 1, 31803.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/9/2010"), FormQC.TypeOfQC.Background, 0.50, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/9/2010"), FormQC.TypeOfQC.Alpha, 11673.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/9/2010"), FormQC.TypeOfQC.Beta, 1, 32238.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/10/2010"), FormQC.TypeOfQC.Background, 1.60, 248.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/10/2010"), FormQC.TypeOfQC.Alpha, 11778.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/10/2010"), FormQC.TypeOfQC.Beta, 1, 32031.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/13/2010"), FormQC.TypeOfQC.Background, 1.90, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/13/2010"), FormQC.TypeOfQC.Alpha, 11633.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/13/2010"), FormQC.TypeOfQC.Beta, 1, 32071.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/14/2010"), FormQC.TypeOfQC.Background, 1.40, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/14/2010"), FormQC.TypeOfQC.Alpha, 11865.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/14/2010"), FormQC.TypeOfQC.Beta, 1, 32069.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/15/2010"), FormQC.TypeOfQC.Background, 0.90, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/15/2010"), FormQC.TypeOfQC.Alpha, 11752.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/15/2010"), FormQC.TypeOfQC.Beta, 1, 31874.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/16/2010"), FormQC.TypeOfQC.Background, 0.60, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/16/2010"), FormQC.TypeOfQC.Alpha, 12079.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/16/2010"), FormQC.TypeOfQC.Beta, 1, 31736.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2010"), FormQC.TypeOfQC.Background, 1.40, 240.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2010"), FormQC.TypeOfQC.Alpha, 11657.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/17/2010"), FormQC.TypeOfQC.Beta, 1, 32219.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/20/2010"), FormQC.TypeOfQC.Background, 1.40, 247.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/20/2010"), FormQC.TypeOfQC.Alpha, 11798.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/20/2010"), FormQC.TypeOfQC.Beta, 1, 30817.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/21/2010"), FormQC.TypeOfQC.Background, 1.10, 249.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/21/2010"), FormQC.TypeOfQC.Alpha, 11579.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/21/2010"), FormQC.TypeOfQC.Beta, 1, 31903.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/22/2010"), FormQC.TypeOfQC.Background, 0.70, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/22/2010"), FormQC.TypeOfQC.Alpha, 11916.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/22/2010"), FormQC.TypeOfQC.Beta, 1, 32073.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/23/2010"), FormQC.TypeOfQC.Background, 1.60, 243.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/23/2010"), FormQC.TypeOfQC.Alpha, 11997.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/23/2010"), FormQC.TypeOfQC.Beta, 1, 31875.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/24/2010"), FormQC.TypeOfQC.Background, 0.90, 251.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/24/2010"), FormQC.TypeOfQC.Alpha, 11914.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/24/2010"), FormQC.TypeOfQC.Beta, 1, 32337.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/27/2010"), FormQC.TypeOfQC.Background, 1.20, 252.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/27/2010"), FormQC.TypeOfQC.Alpha, 11798.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/27/2010"), FormQC.TypeOfQC.Beta, 1, 32050.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/28/2010"), FormQC.TypeOfQC.Background, 1.20, 253.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/28/2010"), FormQC.TypeOfQC.Alpha, 11960.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/28/2010"), FormQC.TypeOfQC.Beta, 1, 31758.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/29/2010"), FormQC.TypeOfQC.Background, 1.00, 245.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/29/2010"), FormQC.TypeOfQC.Alpha, 11844.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/29/2010"), FormQC.TypeOfQC.Beta, 1, 32013.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/30/2010"), FormQC.TypeOfQC.Background, 1.50, 241.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/30/2010"), FormQC.TypeOfQC.Alpha, 11542.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("9/30/2010"), FormQC.TypeOfQC.Beta, 1, 31422.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/1/2010"), FormQC.TypeOfQC.Background, 1.70, 250.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/1/2010"), FormQC.TypeOfQC.Alpha, 11358.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/1/2010"), FormQC.TypeOfQC.Beta, 1, 31710.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/4/2010"), FormQC.TypeOfQC.Background, 0.60, 244.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/4/2010"), FormQC.TypeOfQC.Alpha, 11509.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/4/2010"), FormQC.TypeOfQC.Beta, 1, 31572.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/5/2010"), FormQC.TypeOfQC.Background, 1.00, 242.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/5/2010"), FormQC.TypeOfQC.Alpha, 11385.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/5/2010"), FormQC.TypeOfQC.Beta, 1, 31799.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/6/2010"), FormQC.TypeOfQC.Background, 0.80, 248.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/6/2010"), FormQC.TypeOfQC.Alpha, 11860.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/6/2010"), FormQC.TypeOfQC.Beta, 1, 31430.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/7/2010"), FormQC.TypeOfQC.Background, 2.80, 259.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/7/2010"), FormQC.TypeOfQC.Alpha, 11984.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/7/2010"), FormQC.TypeOfQC.Beta, 1, 31588.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/08/2010"), FormQC.TypeOfQC.Background, 1.40, 248.00, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/08/2010"), FormQC.TypeOfQC.Alpha, 11409.00, 1, 1, true, "", "Bob"));
            this.Add(new QCCalResultNode(DateTime.Parse("10/08/2010"), FormQC.TypeOfQC.Beta, 1, 32054.00, 1, true, "", "Bob"));

            this.Add(new QCCalResultNode(DateTime.Parse("7/12/2013"), FormQC.TypeOfQC.Background, 44, 10.00, 1, true, "aa", "CBM"));
            this.Add(new QCCalResultNode(DateTime.Parse("7/12/2013"), FormQC.TypeOfQC.Background, 66, 10.00, 1, true, "bb", "DBM"));

        }

        public void ForceClear()
        {
            ListOfCurrentNodes = new List<QCCalResultNode>();
            ListOfDefunctNodes = new List<QCCalResultNode>();
            return;
        }

        #endregion

        #region Add Handler
        public bool Add(QCCalResultNode _N)
        {
            if (!Locked)
            {
                ListOfCurrentNodes.Add(_N);
                this.ListModified = true;
                return true;
            }

            return false;
            
        }
        #endregion

        #region Move Handler
        public void MoveToDefunctList(QCCalResultNode _N)
        {
            int IndexToMove = ListOfCurrentNodes.FindIndex(x => x.Equals(_N));

            if (IndexToMove != -1)
            {
                QCCalResultNode NodeToMove = ListOfCurrentNodes[IndexToMove];
                ListOfDefunctNodes.Add(NodeToMove);
                ListOfCurrentNodes.RemoveAt(IndexToMove);
                    
            }
            return;
        }

        #endregion

        #region Find Handler
        public QCCalResultNode FindByDate(DateTime Time)
        {
            QCCalResultNode Node = ListOfCurrentNodes.Find(x => AreTimesSameDay(x.GetDateTimeCompleted(), Time));
            return Node;
        }
        #endregion

        #region Private Utility Functions
        private bool AreTimesSameDay(DateTime T1, DateTime T2)
        {
            if (T1.Year == T2.Year)
            {
                if (T1.Month == T2.Month)
                {
                    if (T1.Day == T2.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Thread sync functions
        public void Lock()
        {
            this.Locked = true;
            return;
        }

        public void UnLock()
        {
            this.Locked = false;
            return;
        }

        #endregion

        #region Getters
        public bool IsNew()
        {
            return this.ListModified;
        }

        public List<QCCalResultNode> GetCurrentList()
        {
            return this.ListOfCurrentNodes;
        }

        public List<QCCalResultNode> GetFullList()
        {
            List<QCCalResultNode> FullList = new List<QCCalResultNode>();
            
            FullList.AddRange(this.ListOfCurrentNodes);
            FullList.AddRange(this.ListOfDefunctNodes);
            
            return FullList;
        }

        public List<QCCalResultNode> GetDefunctList()
        {
            return this.ListOfDefunctNodes;
        }

        public string[,] GetCurrentCSVRepresentation()
        {
            /*Dynamically get length of array*/
            string[] Header = ListOfCurrentNodes.First().GetCSVHeader();

            string[,] ReturnString = new String[ListOfCurrentNodes.Count + 1, Header.Length];

            /*Write header*/
            for (int i = 0; i < Header.Length; i++)
            {
                ReturnString[0, i] = Header[i];
            }

            for (int i = 0; i < ListOfCurrentNodes.Count; i++)
            {
                string[] CurrentNode = ListOfCurrentNodes[i].GetCSVArray();

                for (int j = 0; j < CurrentNode.Length; j++)
                {
                    ReturnString[i + 1, j] = CurrentNode[j];
                }
            }

            return ReturnString;
        }

        public string[,] GetFullCSVRepresentation()
        {
            /*Dynamically get length of array*/
            string[] Header = ListOfCurrentNodes.First().GetCSVHeader();

            List<QCCalResultNode> FullList = new List<QCCalResultNode>();

            FullList.AddRange(this.ListOfCurrentNodes);
            FullList.AddRange(this.ListOfDefunctNodes);

            string[,] ReturnString = new String[FullList.Count + 1, Header.Length];

            /*Write header*/
            for (int i = 0; i < Header.Length; i++)
            {
                ReturnString[0, i] = Header[i];
            }

            for (int i = 0; i < FullList.Count; i++)
            {
                string[] CurrentNode = FullList[i].GetCSVArray();

                for (int j = 0; j < CurrentNode.Length; j++)
                {
                    ReturnString[i + 1, j] = CurrentNode[j];
                }
            }

            return ReturnString;
        }

        #endregion

        #region Setters
        public void ClearNewFlag()
        {
            this.ListModified = false;
        }
        #endregion
    }
}
