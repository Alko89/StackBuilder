#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using log4net;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;

//using treeDiM.PLMPack.DBClient;
//using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewPalletCap : FormNewBase, IDrawingContainer
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewPalletCap));
        #endregion

        #region Constructor
        public FormNewPalletCap(Document document, PalletCapProperties capProperties)
            : base(document, capProperties)
        {
            InitializeComponent();
            // units
            UnitsManager.AdaptUnitLabels(this);

            if (null != capProperties)
            {
                CapLength = capProperties.Length;
                CapWidth = capProperties.Width;
                CapHeight = capProperties.Height;

                CapInnerLength = capProperties.InsideLength;
                CapInnerWidth = capProperties.InsideWidth;
                CapInnerHeight = capProperties.InsideHeight;

                CapWeight = capProperties.Weight;
                CapColor = capProperties.Color;
            }
            else
            {
                CapLength = UnitsManager.ConvertLengthFrom(1200.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                CapWidth = UnitsManager.ConvertLengthFrom(1000.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                CapHeight = UnitsManager.ConvertLengthFrom(50.0, UnitsManager.UnitSystem.UNIT_METRIC1);

                CapWeight = UnitsManager.ConvertSurfaceMassFrom(0.5, UnitsManager.UnitSystem.UNIT_METRIC1);
                CapColor = Color.Khaki;
            }
            UpdateStatus(string.Empty);
        }
        #endregion

        #region FormNewBase overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
        }

        public override string ItemDefaultName
        { get { return Resources.ID_PALLETCAP; } }

        public override void UpdateStatus(string message)
        {
            if (CapInnerLength > CapLength)
                message = string.Format(Resources.ID_INVALIDINSIDELENGTH, CapInnerLength, CapLength);
            else if (CapInnerWidth > CapWidth)
                message = string.Format(Resources.ID_INVALIDINSIDEWIDTH, CapInnerWidth, CapWidth);
            else if (CapInnerHeight > CapHeight)
                message = string.Format(Resources.ID_INVALIDINSIDEHEIGHT, CapInnerHeight, CapHeight);
            base.UpdateStatus(message);
        }
        #endregion

        #region Public properties
        public double CapLength
        {
            get { return uCtrlDimensionsOuter.ValueX; }
            set { uCtrlDimensionsOuter.ValueX = value; }
        }
        public double CapWidth
        {
            get { return uCtrlDimensionsOuter.ValueY; }
            set { uCtrlDimensionsOuter.ValueY = value; }
        }
        public double CapHeight
        {
            get { return uCtrlDimensionsOuter.ValueZ; }
            set { uCtrlDimensionsOuter.ValueZ = value; }
        }
        public double CapInnerLength
        {
            get { return uCtrlDimensionsInner.ValueX; }
            set { uCtrlDimensionsInner.ValueX = value; }
        }
        public double CapInnerWidth
        {
            get { return uCtrlDimensionsInner.ValueY; }
            set { uCtrlDimensionsInner.ValueY = value; }
        }
        public double CapInnerHeight
        {
            get { return uCtrlDimensionsInner.ValueZ; }
            set { uCtrlDimensionsInner.ValueZ = value; }
        }
        public double CapWeight
        {
            get { return uCtrlWeight.Value; }
            set { uCtrlWeight.Value = value; }
        }
        public Color CapColor
        {
            get { return cbColor.Color; }
            set { cbColor.Color = value; }
        }
        #endregion

        #region Handlers
        private void OnColorChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }
        private void UpdateThicknesses(object sender, EventArgs e)
        {
            if (sender is UCtrlTriDouble uCtrlDim)
            {
                double thickness = UnitsManager.ConvertLengthFrom(5.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                if (uCtrlDimensionsOuter == uCtrlDim && CapLength > thickness)
                    CapInnerLength = CapLength - thickness;
                if (uCtrlDimensionsOuter == uCtrlDim && CapWidth > thickness)
                    CapInnerWidth = CapWidth - thickness;
                if (uCtrlDimensionsOuter == uCtrlDim && CapHeight > thickness)
                    CapInnerHeight = CapHeight - thickness;
            }
            // update
            UpdateStatus(string.Empty);
            // draw cap
            graphCtrl.Invalidate();
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (CapLength > 0 && CapWidth > 0 && CapHeight > 0)
            {
                // draw
                PalletCapProperties palletCapProperties = new PalletCapProperties(
                    null, ItemName, ItemDescription, CapLength, CapWidth, CapHeight,
                    CapInnerLength, CapInnerWidth, CapInnerHeight,
                    CapWeight, CapColor);
                PalletCap palletCap = new PalletCap(0, palletCapProperties, BoxPosition.Zero);
                palletCap.Draw(graphics);
                graphics.AddDimensions(new DimensionCube(CapLength, CapWidth, CapHeight));
            }
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
                        wcfClient.Client.CreateNewPalletCap(new DCSBPalletCap()
                        {
                            Name = form.ItemName,
                            Description = ItemDescription,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            DimensionsOuter = new DCSBDim3D() { M0 = CapLength, M1 = CapWidth, M2 = CapHeight },
                            DimensionsInner = new DCSBDim3D() { M0 = CapInnerLength, M1 = CapInnerWidth, M2 = CapInnerHeight },
                            Weight = CapWeight,
                            Color = CapColor.ToArgb(),
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
