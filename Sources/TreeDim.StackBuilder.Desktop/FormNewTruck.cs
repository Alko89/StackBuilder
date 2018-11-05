#region Using directives
using System;
using System.ComponentModel;
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
    public partial class FormNewTruck : FormNewBase, IDrawingContainer
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewTruck));
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor (edit existing properties)
        /// </summary>
        /// <param name="document">Document to which the edited item belongs</param>
        /// <param name="truckProperties">Edited item</param>
        public FormNewTruck(Document document, TruckProperties truckProperties)
            : base(document, truckProperties)
        {
            InitializeComponent();
            // initialize data
            if (null != truckProperties)
            {
                TruckLength = truckProperties.Length;
                TruckWidth = truckProperties.Width;
                TruckHeight = truckProperties.Height;
                TruckAdmissibleLoadWeight = truckProperties.AdmissibleLoadWeight;
                TruckColor = truckProperties.Color;
            }
            else
            { 
                TruckLength = UnitsManager.ConvertLengthFrom(13600, UnitsManager.UnitSystem.UNIT_METRIC1);
                TruckWidth = UnitsManager.ConvertLengthFrom(2450, UnitsManager.UnitSystem.UNIT_METRIC1);
                TruckHeight = UnitsManager.ConvertLengthFrom(2700, UnitsManager.UnitSystem.UNIT_METRIC1);
                TruckAdmissibleLoadWeight = UnitsManager.ConvertMassFrom(38000, UnitsManager.UnitSystem.UNIT_METRIC1);
                TruckColor = Color.LightBlue;            
            }
            UpdateStatus(string.Empty);
            // units
            UnitsManager.AdaptUnitLabels(this);
        }
        #endregion

        #region FormNewBase overrides
        public override string ItemDefaultName
        {   get { return Resources.ID_TRUCK; } }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
            // windows settings
            if (null != Settings.Default.FormNewTruckPosition)
                Settings.Default.FormNewTruckPosition.Restore(this);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // window position
            if (null == Settings.Default.FormNewTruckPosition)
                Settings.Default.FormNewTruckPosition = new WindowSettings();
            Settings.Default.FormNewTruckPosition.Record(this);
        }
        #endregion

        #region Public properties
        public double TruckLength
        {
            get { return uCtrlInnerDimensions.ValueX; }
            set { uCtrlInnerDimensions.ValueX = value; }
        }
        public double TruckWidth
        {
            get { return uCtrlInnerDimensions.ValueY; }
            set { uCtrlInnerDimensions.ValueY = value; }
        }
        public double TruckHeight
        {
            get { return uCtrlInnerDimensions.ValueZ; }
            set { uCtrlInnerDimensions.ValueZ = value; }
        }
        public double TruckAdmissibleLoadWeight
        {
            get { return uCtrlMaxLoadWeight.Value; }
            set { uCtrlMaxLoadWeight.Value = value; }
        }
        public Color TruckColor
        {
            get { return cbColor.Color; }
            set { cbColor.Color = value; }
        }
        #endregion

        #region Draw truck
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (TruckLength == 0 || TruckWidth == 0 || TruckHeight == 0)
                return;
            TruckProperties truckProperties = new TruckProperties(null, TruckLength, TruckWidth, TruckHeight)
            {
                Color = TruckColor
            };
            Truck truck = new Truck(truckProperties);
            truck.DrawBegin(graphics);
            truck.DrawEnd(graphics);
            graphics.AddDimensions(new DimensionCube(TruckLength, TruckWidth, TruckHeight));
        }
        #endregion

        #region Handlers
        public override void UpdateStatus(string message)
        {
            if (!this.DesignMode)
            { 
            }
            base.UpdateStatus(message);
        }

        private void OnTruckPropertyChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
            graphCtrl.Invalidate();
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
                        wcfClient.Client.CreateNewTruck(new DCSBTruck()
                        {
                            Name = form.ItemName,
                            Description = ItemDescription,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            DimensionsInner = new DCSBDim3D() { M0 = TruckLength, M1 = TruckWidth, M2 = TruckHeight },
                            AdmissibleLoad = TruckAdmissibleLoadWeight,
                            Color = TruckColor.ToArgb(),
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
