﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using Sharp3D.Math.Core;
using log4net;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewTruck : Form, IDrawingContainer
    {
        #region Data members
        [NonSerialized]private Document _document;
        [NonSerialized]private TruckProperties _truckProperties;
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewTruck));
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor (new truck properties)
        /// </summary>
        /// <param name="document">Document to which the new item will belong</param>
        public FormNewTruck(Document document)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // save document reference
            _document = document;

            // initialize data
            tbName.Text = _document.GetValidNewTypeName(Resources.ID_TRUCK);
            TruckLength = UnitsManager.ConvertLengthFrom(13600, UnitsManager.UnitSystem.UNIT_METRIC1);
            TruckWidth = UnitsManager.ConvertLengthFrom(2450, UnitsManager.UnitSystem.UNIT_METRIC1);
            TruckHeight = UnitsManager.ConvertLengthFrom(2700, UnitsManager.UnitSystem.UNIT_METRIC1);
            TruckAdmissibleLoadWeight = UnitsManager.ConvertMassFrom(38000, UnitsManager.UnitSystem.UNIT_METRIC1);
            TruckColor = Color.LightBlue;
            // description
            tbDescription.Text = tbName.Text;
            // disable Ok button
            UpdateButtonOkStatus();
        }
        /// <summary>
        /// Constructor (edit existing properties)
        /// </summary>
        /// <param name="document">Document to which the edited item belongs</param>
        /// <param name="truckProperties">Edited item</param>
        public FormNewTruck(Document document, TruckProperties truckProperties)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // save document reference
            _document = document;
            _truckProperties = truckProperties;
            // set caption text
            Text = string.Format(Properties.Resources.ID_EDIT, _truckProperties.Name);
            // initialize data
            tbName.Text = _truckProperties.Name;
            tbDescription.Text = _truckProperties.Description;
            TruckLength = _truckProperties.Length;
            TruckWidth = _truckProperties.Width;
            TruckHeight = _truckProperties.Height;
            TruckAdmissibleLoadWeight = _truckProperties.AdmissibleLoadWeight;
            TruckColor = _truckProperties.Color;
            // disable Ok button
            UpdateButtonOkStatus();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// truck name
        /// </summary>
        public string TruckName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        /// <summary>
        /// truck description
        /// </summary>
        public string Description
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        /// <summary>
        /// truck length
        /// </summary>
        public double TruckLength
        {
            get { return System.Convert.ToDouble(nudLength.Text); }
            set { nudLength.Text = string.Format("{0}", value); }
        }
        /// <summary>
        /// truck width
        /// </summary>
        public double TruckWidth
        {
            get { return System.Convert.ToDouble(nudWidth.Text); }
            set { nudWidth.Text = string.Format("{0}", value); }
        }
        /// <summary>
        /// truck height
        /// </summary>
        public double TruckHeight
        {
            get { return System.Convert.ToDouble(nudHeight.Text); }
            set { nudHeight.Text = string.Format("{0}", value); }
        }
        /// <summary>
        /// truck admissible load weight
        /// </summary>
        public double TruckAdmissibleLoadWeight
        {
            get { return System.Convert.ToDouble(nudAdmissibleLoadWeight.Text); }
            set { nudAdmissibleLoadWeight.Text = string.Format("{0}", value); }
        }
        /// <summary>
        /// truck color
        /// </summary>
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
            TruckProperties truckProperties = new TruckProperties(null, TruckLength, TruckWidth, TruckHeight);
            truckProperties.Color = TruckColor;
            Truck truck = new Truck(truckProperties);
            truck.DrawBegin(graphics);
            truck.DrawEnd(graphics);
            graphics.AddDimensions(new DimensionCube(TruckLength, TruckWidth, TruckHeight));
        }
        #endregion

        #region Handlers
        private void UpdateButtonOkStatus()
        {
            // message ?
            string message = string.Empty;
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;
            else if (!_document.IsValidNewTypeName(tbName.Text, _truckProperties))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            // description ?
            else if (string.IsNullOrEmpty(tbDescription.Text))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;
            // button OK
            bnOK.Enabled = string.IsNullOrEmpty(message);
            // status bar
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
        }

        private void onTruckPropertyChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }

        private void onNameDescriptionChanged(object sender, EventArgs e)
        {
            UpdateButtonOkStatus();
        }
        #endregion

        #region Load / FormClosing event
        private void FormNewTruck_Load(object sender, EventArgs e)
        {
            // windows settings
            if (null != Settings.Default.FormNewTruckPosition)
                Settings.Default.FormNewTruckPosition.Restore(this);
        }

        private void FormNewTruck_FormClosing(object sender, FormClosingEventArgs e)
        {
            // window position
            if (null == Settings.Default.FormNewTruckPosition)
                Settings.Default.FormNewTruckPosition = new WindowSettings();
            Settings.Default.FormNewTruckPosition.Record(this);
        }
        #endregion
    }
}
