#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public partial class UCtrlDouble : UserControl
    {
        #region Delegates
        public delegate void ValueChangedDelegate(object sender, EventArgs args);
        #endregion

        #region Events
        public event ValueChangedDelegate ValueChanged;
        #endregion

        #region Constructor
        public UCtrlDouble()
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
        public double Value
        {
            get { return (double)nudValue.Value; }
            set { try { nudValue.Value = (decimal)value; } catch (ArgumentOutOfRangeException) {} }
        }
        [Browsable(true)]
        public decimal Minimum
        {
            get { return nudValue.Minimum; }
            set { nudValue.Minimum = value; }
        }
        [Browsable(true)]
        public UnitsManager.UnitType Unit
        {
            get { return _unitType; }
            set
            {
                _unitType = value;
                lbUnit.Text = UnitsManager.UnitString(_unitType);
                nudValue.DecimalPlaces = UnitsManager.NoDecimals(_unitType);
            }
        }
        #endregion

        #region Event handlers
        private void OnValueChangedLocal(object sender, EventArgs e) => ValueChanged?.Invoke(this, e);
        private void ValueControl_SizeChanged(object sender, EventArgs e)
        {
            // set nud location
            nudValue.Location = new Point(Width - stNudLength - stLbUnitLength, 0);
            // set unit location
            lbUnit.Location = new Point(Width - stLbUnitLength + 1, 4);
        }
        #endregion

        #region Data members
        private UnitsManager.UnitType _unitType;
        public static int stNudLength = 60;
        public static int stLbUnitLength = 38;
        #endregion
    }
}
