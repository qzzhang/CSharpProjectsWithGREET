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
    public partial class PDFViewer : Form
    {

        public PDFViewer(string Path)
        {
            InitializeComponent();

            PDF_Space.src = Path;
        }

    }
}
