using FuzzyString;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuzzyStringConsole
{
    public partial class Form1 : Form
    {
        List<string> resources = new List<string>() { "Natural Gas", "Electricity", "Residual Oil", "Ammonia", "Ammonium Nitrate", "Calcium Carbonate", "Nitrate" , "Gasoline", "Diesel"};
        List<string> technologies = new List<string>() { "Commercial Boiler", "Industrial Boiler", "Utility Boiler", "Turbine", "Tractor", "Train", "Barge" };

        List<FuzzyStringComparisonOptions> options6;

        public Form1()
        {
            InitializeComponent();

            options6 = new List<FuzzyStringComparisonOptions>();
            options6.Add(FuzzyStringComparisonOptions.UseLongestCommonSubsequence);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.listView5.Items.Clear();
            this.listView6.Items.Clear();

            foreach (String str in resources)
                if (str.ApproximatelyEquals(this.textBox1.Text, options6, FuzzyStringComparisonTolerance.Strong))
                    this.listView5.Items.Add(str);

            foreach (String str in technologies)
                if (str.ApproximatelyEquals(this.textBox1.Text, options6, FuzzyStringComparisonTolerance.Strong))
                    this.listView6.Items.Add(str);


    
        }
    }
}
