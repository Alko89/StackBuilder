#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public partial class uCtrlCaseOrientation : UserControl
    {
        #region Constructor
        public uCtrlCaseOrientation()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (null != _packable)
            {
                if (PackableOrientationAllowed)
                {
                    BoxToPictureBox.Draw(_packable, HalfAxis.HAxis.AXIS_X_P, pictureBoxX);
                    BoxToPictureBox.Draw(_packable, HalfAxis.HAxis.AXIS_Y_P, pictureBoxY);
                    BoxToPictureBox.Draw(_packable, HalfAxis.HAxis.AXIS_Z_P, pictureBoxZ);
                }
                else
                    BoxToPictureBox.Draw(_packable, HalfAxis.HAxis.AXIS_Z_P, pictureBoxGlobal);
            }
        }
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
        public PackableBrick BProperties
        {
            set
            {
                _packable = value;
                if (null == _packable)
                    return;
                bool allOrientationsAllowed = PackableOrientationAllowed;
                pictureBoxX.Visible = allOrientationsAllowed;
                checkBoxX.Visible = allOrientationsAllowed;
                pictureBoxY.Visible = allOrientationsAllowed;
                checkBoxY.Visible = allOrientationsAllowed;
                pictureBoxZ.Visible = allOrientationsAllowed;
                checkBoxZ.Visible = allOrientationsAllowed;

                pictureBoxGlobal.Visible = !allOrientationsAllowed;

                Invalidate();
            }
        }
        private bool PackableOrientationAllowed
        {
            get
            {
                return (null == _packable) ? false : _packable.OrientationAllowed(HalfAxis.HAxis.AXIS_X_N);
            }
        }

        public bool[] AllowedOrientations
        {
            get
            {
                if (PackableOrientationAllowed)
                    return new bool[] { checkBoxX.Checked, checkBoxY.Checked, checkBoxZ.Checked };
                else
                    return new bool[] { false, false, true };
            }
            set
            {
                checkBoxX.Checked = value[0];
                checkBoxY.Checked = value[1];
                checkBoxZ.Checked = value[2]; 
            }
        }
        public bool IsOrientationAllowed(HalfAxis.HAxis axis)
        {
            if (HalfAxis.HAxis.AXIS_X_N == axis || HalfAxis.HAxis.AXIS_X_P == axis) return PackableOrientationAllowed ? checkBoxX.Checked : false;
            else if (HalfAxis.HAxis.AXIS_Y_N == axis || HalfAxis.HAxis.AXIS_Y_P == axis) return PackableOrientationAllowed ? checkBoxY.Checked : false;
            else if (HalfAxis.HAxis.AXIS_Z_N == axis || HalfAxis.HAxis.AXIS_Z_P == axis) return PackableOrientationAllowed? checkBoxZ.Checked : true;
            else return false;
        }
        public bool HasOrientationSelected
        {
            get { return !PackableOrientationAllowed || (checkBoxX.Checked || checkBoxY.Checked || checkBoxZ.Checked); }
        }
        #endregion

        #region Data members
        private PackableBrick _packable;
        #endregion
    }
}
