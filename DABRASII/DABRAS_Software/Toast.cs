using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public partial class Toast : Form
    {
        System.Timers.Timer T;

        public delegate void ToastCloser();

        public Toast(string _M)
        {
            InitializeComponent();

            this.Message.Text = _M;

            T = new System.Timers.Timer(2000);
            
            T.Enabled = true;
            T.Elapsed += new System.Timers.ElapsedEventHandler(T_Elapsed);
        }

        private void Toast_Shown(object sender, EventArgs e)
        {
            T.Start();
        }

        private void CloseIt()
        {
            this.Dispose();
            T.Enabled = false;
        }

        void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Invoke(new ToastCloser(CloseIt));
            }
            catch
            {
                ;
            }
        }
    }
}
