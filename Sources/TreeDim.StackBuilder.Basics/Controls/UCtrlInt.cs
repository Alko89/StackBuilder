#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public partial class UCtrlInt : UserControl
    {
        #region Delegates
        public delegate void ValueChangedDelegate(object sender, EventArgs e);
        #endregion

        #region Events
        public event ValueChangedDelegate ValueChanged;
        #endregion

        #region Constructor
        public UCtrlInt()
        {
            InitializeComponent();
        }
        #endregion

        #region Public properties
        [Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance")]
        public override string Text
        {
            get { return lbName.Text; }
            set { lbName.Text = value; }
        }
        [Browsable(true)]
        public int Value
        {
            get { return (int)nudValue.Value; }
            set { try { nudValue.Value = value; } catch (ArgumentOutOfRangeException) { } }
        }
        [Browsable(true)]
        public decimal Minimum
        {
            get { return nudValue.Minimum; }
            set { nudValue.Minimum = value; }
        }
        #endregion

        #region Event handlers
        private void OnValueChangedLocal(object sender, EventArgs e) => ValueChanged?.Invoke(this, e);
        private void OnSizeChanged(object sender, EventArgs e) => nudValue.Location = new Point(Width - stNudLength, 0);
        #endregion

        #region Data members
        public static int stNudLength = 60;
        #endregion
    }
}
