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
    public partial class FormNewPallet : Form, IDrawingContainer
    {
        #region Data members
        [NonSerialized]private Document _document;
        [NonSerialized]private PalletProperties _palletProperties;
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewPallet));
        #endregion

        #region Constructor
        public FormNewPallet(Document document)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // save document reference
            _document = document;
            // initialize type combo
            cbType.Items.AddRange(PalletData.TypeNames);
            // set selected item
            PalletTypeName = Properties.Settings.Default.PalletTypeName;
            // initialize dimensions
            onPalletTypeChanged(this, null);
        }
        public FormNewPallet(Document document, PalletProperties palletProperties)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // save document reference
            _document = document;
            _palletProperties = palletProperties;
            // initialize type combo
            cbType.Items.AddRange(PalletData.TypeNames);
            // set selected item
            PalletTypeName = _palletProperties.TypeName;
            // set caption text
            Text = string.Format(Resources.ID_PALLETCAPTIONEDIT, _palletProperties.Name);
            // initialize data
            tbName.Text = _palletProperties.Name;
            tbDescription.Text = _palletProperties.Description;
            PalletLength = _palletProperties.Length;
            PalletWidth = _palletProperties.Width;
            PalletHeight = _palletProperties.Height;
            Weight = _palletProperties.Weight;
            PalletColor = _palletProperties.Color;
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;
            // update status
            UpdateButtonOkStatus();
            // windows settings
            if (null != Settings.Default.FormNewPalletPosition)
                Settings.Default.FormNewPalletPosition.Restore(this);
        }
        #endregion

        #region Public properties
        public string PalletName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }

        public string Description
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }

        public double PalletLength
        {
            get { return System.Convert.ToDouble(nudLength.Text); }
            set { nudLength.Text = string.Format("{0:F}", value); }
        }

        public double PalletWidth
        {
            get { return System.Convert.ToDouble(nudWidth.Text); }
            set { nudWidth.Text = string.Format("{0:F}", value); }
        }

        public double PalletHeight
        {
            get { return System.Convert.ToDouble(nudHeight.Text); }
            set { nudHeight.Text = string.Format("{0:F}", value); }
        }

        public double Weight
        {
            get { return System.Convert.ToDouble(nudWeight.Text); }
            set { nudWeight.Text = string.Format("{0:F}", value); }
        }

        public Color PalletColor
        {
            get { return cbColor.Color; }
            set { cbColor.Color = value; }
        }

        public string PalletTypeName
        {
            get { return cbType.Items[cbType.SelectedIndex].ToString(); }
            set
            {
                int index = 0;
                foreach (string item in cbType.Items)
                {
                    if (string.Equals(item, value))
                        break;
                    ++index;
                }
                if (cbType.Items.Count > index)
                    cbType.SelectedIndex = index;
            }
        }
        #endregion

        #region Implement IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (0 == cbType.Items.Count)
                return;
            PalletProperties palletProperties = new PalletProperties(null, PalletTypeName, PalletLength, PalletWidth, PalletHeight);
            palletProperties.Color = PalletColor;
            Pallet pallet = new Pallet(palletProperties);
            pallet.Draw(graphics, Transform3D.Identity);
            graphics.AddDimensions(new DimensionCube(PalletLength, PalletWidth, PalletHeight));
        }
        #endregion

        #region Handlers
        private void onPalletPropertyChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }

        private void UpdateButtonOkStatus()
        {
            string message = string.Empty;
            // name
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;
            // description
            else if (string.IsNullOrEmpty(tbDescription.Text))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;
            // name validity
            else if (!_document.IsValidNewTypeName(tbName.Text, _palletProperties))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            //---
            // button OK
            bnAccept.Enabled = string.IsNullOrEmpty(message);
            // status bar
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;

        }
        private void onNameDescriptionChanged(object sender, EventArgs e)
        {
            UpdateButtonOkStatus();
        }
        private void onPalletTypeChanged(object sender, EventArgs e)
        {
            PalletData palletData = PalletData.GetByName(PalletTypeName);
            if (null == palletData) return;

            // set name / description / length / width / height / weight
            PalletName = palletData.Name;
            Description = palletData.Description;
            PalletLength = UnitsManager.ConvertLengthFrom(palletData.Length, UnitsManager.UnitSystem.UNIT_METRIC1);
            PalletWidth = UnitsManager.ConvertLengthFrom(palletData.Width,  UnitsManager.UnitSystem.UNIT_METRIC1);
            PalletHeight = UnitsManager.ConvertLengthFrom(palletData.Height,  UnitsManager.UnitSystem.UNIT_METRIC1);
            Weight = UnitsManager.ConvertMassFrom(palletData.Weight, UnitsManager.UnitSystem.UNIT_METRIC1);
            PalletColor = palletData.Color;

            graphCtrl.Invalidate();
        }
        #endregion

        #region Load / FormClosing event
        private void FormNewPallet_FormClosing(object sender, FormClosingEventArgs e)
        {
            // window position
            if (null == Settings.Default.FormNewPalletPosition)
                Settings.Default.FormNewPalletPosition = new WindowSettings();
            Settings.Default.FormNewPalletPosition.Record(this);
            // pallet type name
            Settings.Default.PalletTypeName = PalletTypeName;
        }
        #endregion
    }
}