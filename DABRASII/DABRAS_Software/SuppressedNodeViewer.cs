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
    public partial class SuppressedNodeViewer : Form
    {

        #region Data Members
        private QCListKeeper QC_List;
        private List<QCCalResultNode> ListOfNodes;
        private List<QCCalResultNode> SuppressedNodes;
        #endregion

        #region Constructor
        public SuppressedNodeViewer(QCListKeeper _QL)
        {
            InitializeComponent();

            SuppressedNodesDataGridView.Columns.Add("Date", "Date");
            SuppressedNodesDataGridView.Columns.Add("Name", "Name");
            SuppressedNodesDataGridView.Columns.Add("Badge", "Badge #");
            SuppressedNodesDataGridView.Columns.Add("Type", "QC Type");
            SuppressedNodesDataGridView.Columns.Add("NetAlpha", "NetAlpha");
            SuppressedNodesDataGridView.Columns.Add("NetBeta", "NetBeta");
            SuppressedNodesDataGridView.Columns.Add("Comment", "Comment");

            this.QC_List = _QL;
            ListOfNodes = QC_List.GetFullList();

            DrawRows(this.SuppressedNodesDataGridView);
            SuppressedNodesDataGridView.Invalidate();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
            
        }
        #endregion

        #region Row Click Handler
        private void SupressedNodesDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            QCCalResultNode SelectedNode = SuppressedNodes[e.RowIndex];
            QCCalResultNode OriginalCopy = ListOfNodes.Find(x => x == SelectedNode);
            if (MessageBox.Show("Unsupress the node?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OriginalCopy.SetPlottable(true);
                SuppressedNodes.RemoveAt(e.RowIndex);
                MessageBox.Show("Point will now be plotted.");
            }

            DrawRows(this.SuppressedNodesDataGridView);
            SuppressedNodesDataGridView.Invalidate();
        }
        #endregion

        #region Private Utility Functions
        private void DrawRows(DataGridView DG)
        {
            DG.Rows.Clear();
            SuppressedNodes = new List<QCCalResultNode>();
            foreach (QCCalResultNode Q in ListOfNodes)
            {
                if (!Q.IsPlottable())
                {
                    /*If not plottable, add it to the datagridview*/
                    SuppressedNodesDataGridView.Rows.Add(Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetTypeOfQC(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetComment());
                    SuppressedNodes.Add(Q);
                }
            }

        }
        #endregion

        #region Getters
        public QCListKeeper GetListKeeper()
        {
            return this.QC_List;
        }
        #endregion

        #region KeyPress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
        }
        #endregion

    }
}
