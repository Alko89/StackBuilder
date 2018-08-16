#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;

//using treeDiM.PLMPack.DBClient;
//using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewCylinder : FormNewBase, IDrawingContainer
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewCylinder));
        #endregion

        #region Constructors
        public FormNewCylinder(Document document, CylinderProperties cylinder)
            : base(document, cylinder)
        {
            InitializeComponent();
            // properties
            if (null != cylinder)
            {
                uCtrlDiameterOuter.Value = 2.0 * cylinder.RadiusOuter;
                uCtrlDiameterInner.Value = 2.0 * cylinder.RadiusInner;
                uCtrlHeight.Value = cylinder.Height;
                uCtrlWeight.Value = cylinder.Weight;
                cbColorWallOuter.Color = cylinder.ColorWallOuter;
                cbColorWallInner.Color = cylinder.ColorWallInner;
                cbColorTop.Color = cylinder.ColorTop;
            }
            else
            { 
                uCtrlDiameterOuter.Value = UnitsManager.ConvertLengthFrom(2.0*75.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                uCtrlDiameterInner.Value = UnitsManager.ConvertLengthFrom(0.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                uCtrlHeight.Value = UnitsManager.ConvertLengthFrom(150.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                uCtrlWeight.Value = UnitsManager.ConvertMassFrom(1.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                cbColorWallOuter.Color = System.Drawing.Color.LightSkyBlue;
                cbColorWallInner.Color = System.Drawing.Color.Chocolate;
                cbColorTop.Color = System.Drawing.Color.Gray;            
            }
            // disable Ok button
            UpdateStatus(string.Empty);        
            // units
            UnitsManager.AdaptUnitLabels(this);
        }
        #endregion

        #region FormNewBase overrides
        public override string ItemDefaultName
        { get { return Resources.ID_CYLINDER; } }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
        }
        #endregion

        #region Public properties
        public double RadiusOuter
        {
            get { return 0.5 * uCtrlDiameterOuter.Value; }
            set { uCtrlDiameterOuter.Value = 2.0 * value; }
        }
        public double RadiusInner
        {
            get { return 0.5 * uCtrlDiameterInner.Value; }
            set { uCtrlDiameterInner.Value = 2.0 * value; }
        }
        public double CylinderHeight
        {
            get { return uCtrlHeight.Value; }
            set { uCtrlHeight.Value = value; }
        }
        public double Weight
        {
            get { return uCtrlWeight.Value; }
            set { uCtrlWeight.Value = value; }
        }
        public OptDouble NetWeight
        {
            get { return uCtrlNetWeight.Value; }
            set { uCtrlNetWeight.Value = value; }
        }
        public Color ColorTop
        {
            get { return cbColorTop.Color; }
            set { cbColorTop.Color = value; }
        }
        public Color ColorWallOuter
        {
            get { return cbColorWallOuter.Color; }
            set { cbColorWallOuter.Color = value; }
        }
        public Color ColorWallInner
        {
            get { return cbColorWallInner.Color; }
            set { cbColorWallInner.Color = value; }
        }
        #endregion

        #region Handlers
        private void OnValueChanged(object sender, EventArgs e)
        {
            // update ok button status
            UpdateStatus(string.Empty);
            // update cylinder drawing
            graphCtrl.Invalidate();
        }

        public override void UpdateStatus(string message)
        {
            if (RadiusInner > RadiusOuter)
                message = Resources.ID_INVALIDDIAMETER;
            else if (Weight < 1.0E-06)
                message = Resources.ID_INVALIDMASS;
            base.UpdateStatus(message);
        }
        #endregion

        #region Draw cylinder
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            CylinderProperties cylProperties = new CylinderProperties(
                null, ItemName, ItemDescription
                , RadiusOuter, RadiusInner, CylinderHeight, Weight
                , ColorTop, ColorWallOuter, ColorWallInner);
            Cylinder cyl = new Cylinder(0, cylProperties);
            graphics.AddCylinder(cyl);
            graphics.AddDimensions(new DimensionCube(
                new Vector3D(-RadiusOuter, -RadiusOuter, 0.0),
                2.0 * RadiusOuter, 2.0 * RadiusOuter, CylinderHeight,
                Color.Black, false)
                );
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
                        wcfClient.Client.CreateNewCylinder(new DCSBCylinder()
                        {
                            Name = form.ItemName,
                            Description = ItemDescription,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            RadiusOuter = RadiusOuter,
                            RadiusInner = RadiusInner,
                            Height = CylinderHeight,
                            Weight = Weight,
                            NetWeight = this.NetWeight.Activated ? this.NetWeight.Value : new Nullable<double>(),
                            ColorOuter = ColorWallOuter.ToArgb(),
                            ColorInner = ColorWallInner.ToArgb(),
                            ColorTop = ColorTop.ToArgb(),
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
