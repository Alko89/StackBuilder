#region Using directives
using System;
using System.Windows.Forms;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class UCtrlCaseOrientationSimple : UserControl
    {
        #region Constructor
        public UCtrlCaseOrientationSimple()
        {
            InitializeComponent();

            chkbX.Checked = true;
            chkbY.Checked = true;
            chkbY.Checked = true;
        }
        #endregion

        #region Event handlers
        private void OnCheckedChanged(object sender, EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }
        #endregion

        #region Delegate and events
        public delegate void CheckChanged(object sender, EventArgs e);
        public event CheckChanged CheckedChanged;
        #endregion

        #region Public properties
        public bool[] AllowedOrientations
        {
            get
            {
                return new bool[] { chkbX.Checked, chkbY.Checked, chkbZ.Checked };
            }
            set
            {
                chkbX.Checked = value[0];
                chkbY.Checked = value[1];
                chkbZ.Checked = value[2];
            }
        }
        public bool HasOrientationSelected => chkbX.Checked || chkbY.Checked || chkbZ.Checked;
        #endregion
    }
}
