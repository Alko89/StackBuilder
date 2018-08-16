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
    public partial class FormNewBundle : FormNewBase, IDrawingContainer
    {
        #region Data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormNewBundle));
        #endregion

        #region Constructor
        public FormNewBundle(Document document)
            : base(document, null)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // initialize value
            BundleLength = UnitsManager.ConvertLengthFrom(400.0, UnitsManager.UnitSystem.UNIT_METRIC1);
            BundleWidth = UnitsManager.ConvertLengthFrom(300.0, UnitsManager.UnitSystem.UNIT_METRIC1);
            UnitThickness = UnitsManager.ConvertLengthFrom(5.0, UnitsManager.UnitSystem.UNIT_METRIC1);
            NoFlats = 10;
            Color = Color.LightGray;
            // disable Ok buttons
            UpdateStatus(string.Empty);
        }
        public FormNewBundle(Document document, BundleProperties bundleProperties)
            : base(document, bundleProperties)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // set caption text
            Text = string.Format(Properties.Resources.ID_EDIT, bundleProperties.Name);
            // initialize value
            tbName.Text = bundleProperties.Name;
            tbDescription.Text = bundleProperties.Description;
            BundleLength = bundleProperties.Length;
            BundleWidth = bundleProperties.Width;
            UnitThickness = bundleProperties.UnitThickness;
            UnitWeight = bundleProperties.UnitWeight;
            NoFlats = bundleProperties.NoFlats;
            // enable/disable Ok buttons
            UpdateStatus(string.Empty);
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
            // windows settings
            if (null != Settings.Default.FormNewBundlePosition)
                Settings.Default.FormNewBundlePosition.Restore(this);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // window position
            if (null == Settings.Default.FormNewBundlePosition)
                Settings.Default.FormNewBundlePosition = new WindowSettings();
            Settings.Default.FormNewBundlePosition.Record(this);
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName => Resources.ID_BUNDLE;
        #endregion

        #region Public properties
        public string BundleName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        public string Description
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        public double BundleLength
        {
            get { return uCtrlLength.Value; }
            set { uCtrlLength.Value = value; }
        }
        public double BundleWidth
        {
            get { return uCtrlWidth.Value; }
            set { uCtrlWidth.Value = value; }
        }
        public double UnitThickness
        {
            get { return uCtrlThickness.Value; }
            set { uCtrlThickness.Value = value; }
        }
        public double UnitWeight
        {
            get { return uCtrlWeight.Value; }
            set { uCtrlWeight.Value = value; }
        }
        public int NoFlats
        {
            get { return (int)nudNoFlats.Value; }
            set { nudNoFlats.Value = (decimal)value; }
        }
        public Color Color
        {
            get { return cbColor.Color; }
            set { cbColor.Color = value;}
        }
        #endregion

        #region Handlers
        private void OnBundlePropertyChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }
        public override void UpdateStatus(string message)
        {
            base.UpdateStatus(message);
        }
        private void OnNameDescriptionChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            BundleProperties bundleProperties = new BundleProperties(
                null, BundleName, Description
                , BundleLength, BundleWidth, UnitThickness, UnitWeight, NoFlats, Color);
            Box box = new Box(0, bundleProperties);
            graphics.AddBox(box);
            graphics.AddDimensions(new DimensionCube(BundleLength, BundleWidth, UnitThickness * NoFlats));
        }
        #endregion

        #region Send to database
        private void OnSendToDatabase(object sender, EventArgs e)
        {
            try
            {
                FormSetItemName form = new FormSetItemName()
                {
                    ItemName = BundleName
                };
                if (DialogResult.OK == form.ShowDialog())
                {
                    /*using (WCFClient wcfClient = new WCFClient())
                    {
                        wcfClient.Client.CreateNewBundle(new DCSBBundle()
                        {
                            Name = form.ItemName,
                            Description = Description,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            DimensionsUnit = new DCSBDim3D() { M0 = BundleLength, M1 = BundleWidth, M2 = UnitThickness },
                            Number = NoFlats,
                            UnitWeight = UnitWeight,
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