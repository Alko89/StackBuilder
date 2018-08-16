#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;

//using treeDiM.PLMPack.DBClient;
//using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewInterlayer : FormNewBase, IDrawingContainer
    {
        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(FormNewInterlayer));
        #endregion

        #region Constructor
        public FormNewInterlayer(Document document, InterlayerProperties interlayerProperties)
            : base(document, interlayerProperties)
        {
            InitializeComponent();
            // initialize value
            if (null != interlayerProperties)
            {
                InterlayerLength = interlayerProperties.Length;
                InterlayerWidth = interlayerProperties.Width;
                Thickness = interlayerProperties.Thickness;
                Weight = interlayerProperties.Weight;
                Color = interlayerProperties.Color;
            }
            else
            { 
                // initialize value
                InterlayerLength = UnitsManager.ConvertLengthFrom(1200.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                InterlayerWidth = UnitsManager.ConvertLengthFrom(1000.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                Thickness = UnitsManager.ConvertLengthFrom(1.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                Weight = UnitsManager.ConvertMassFrom(1.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                Color = Color.Beige;
            }
            // disable Ok button
            UpdateStatus(string.Empty);

            // units
            UnitsManager.AdaptUnitLabels(this);
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName
        {   get { return Resources.ID_INTERLAYER; } }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // windows settings
            if (null != Settings.Default.FormNewInterlayerPosition)
                Settings.Default.FormNewInterlayerPosition.Restore(this);

            graphCtrl.DrawingContainer = this;
            graphCtrl.Invalidate();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // window position
            if (null == Settings.Default.FormNewInterlayerPosition)
                Settings.Default.FormNewInterlayerPosition = new WindowSettings();
            Settings.Default.FormNewInterlayerPosition.Record(this);
        }
        #endregion

        #region Public properties
        public double InterlayerLength
        {
            get { return uCtrlDimensions.ValueX; }
            set { uCtrlDimensions.ValueX = value; }
        }
        public double InterlayerWidth
        {
            get { return uCtrlDimensions.ValueY; }
            set { uCtrlDimensions.ValueY = value; }
        }
        public double Thickness
        {
            get { return uCtrlDimensions.ValueZ; }
            set { uCtrlDimensions.ValueZ = value; }
        }
        public double Weight
        {
            get { return uCtrlWeight.Value; }
            set { uCtrlWeight.Value = value; }
        }
        public Color Color
        {
            get { return cbColor.Color; }
            set { cbColor.Color = value; }
        }
        #endregion

        #region Handlers
        private void OnValueChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
            UpdateStatus(string.Empty);
        }
        public override void UpdateStatus(string message)
        {
            if (!DesignMode)
            { 
            }
 	        base.UpdateStatus(message);
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        { 
            InterlayerProperties interlayerProperties = new InterlayerProperties(
                null, tbName.Text, tbDescription.Text
                , InterlayerLength, InterlayerWidth, Thickness, Weight, Color);
            graphics.AddBox(new Box(0, interlayerProperties));
            graphics.AddDimensions(new DimensionCube(InterlayerLength, InterlayerWidth, Thickness));
        }
        #endregion

        #region Send to database
        private void OnSendToDatabase(object sender, EventArgs e)
        {
            try
            {
                FormSetItemName form = new FormSetItemName()
                {
                    ItemName = ItemName
                };
                if (DialogResult.OK == form.ShowDialog())
                {
                    /*using (WCFClient wcfClient = new WCFClient())
                    {
                        wcfClient.Client.CreateNewInterlayer(new DCSBInterlayer()
                        {
                            Name = form.ItemName,
                            Description = ItemDescription,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            Dimensions = new DCSBDim3D() { M0 = InterlayerLength, M1 = InterlayerWidth, M2 = Thickness },
                            Weight = Weight,
                            Color = Color.ToArgb(),
                            AutoInsert = false
                        }
                            );
                    }*/
                    Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
    }
}