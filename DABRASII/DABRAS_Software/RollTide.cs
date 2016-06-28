using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;

/*Well, you found the easter egg! Congrats!
 * ...the only question that remains: is this the ONLY easter egg? Perhaps if you press alt + f4 at the main screen...
 */


namespace DABRAS_Software
{
    public partial class RollTide : Form
    {
        #region Data Members
        SoundPlayer P;
        #endregion

        #region Constructor
        public RollTide()
        {
            InitializeComponent();

            RTR_Image.Image = DABRAS_Software.Properties.Resources.RTR;

            P = new SoundPlayer(DABRAS_Software.Properties.Resources.NotDone);
            P.Play();
        }
        #endregion

        #region Finalization
        private void RollTide_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (P != null)
            {
                P.Stop();
            }
            P.Dispose();
        }
        #endregion

    }
}
