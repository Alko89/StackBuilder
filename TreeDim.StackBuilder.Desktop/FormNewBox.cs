﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using log4net;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewBox : Form, IDrawingContainer
    {
        #region Mode enum
        public enum Mode
        { 
            MODE_BOX
            , MODE_CASE
        }
        #endregion

        #region Data members
        [NonSerialized]private Document _document;
        public Color[] _faceColors = new Color[6];
        public BoxProperties _boxProperties;
        public Mode _mode;
        public List<Pair<HalfAxis.HAxis, Texture>> _textures;
        private double _thicknessLength = 0.0, _thicknessWidth = 0.0, _thicknessHeight = 0.0;
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewBox));
        #endregion

        #region Constructor
        /// <summary>
        /// FormNewBox constructor used when defining a new BoxProperties item
        /// </summary>
        /// <param name="document">Document in which the BoxProperties item is to be created</param>
        /// <param name="mode">Mode is either Mode.MODE_CASE or Mode.MODE_BOX</param>
        public FormNewBox(Document document, Mode mode)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // save document reference
            _document = document;
            // mode
            _mode = mode;

            switch (_mode)
            {
                case Mode.MODE_CASE:
                    tbName.Text = _document.GetValidNewTypeName(Resources.ID_CASE);
                    nudLength.Value = (decimal)UnitsManager.ConvertLengthFrom(400.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudWidth.Value = (decimal)UnitsManager.ConvertLengthFrom(300.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudHeight.Value = (decimal)UnitsManager.ConvertLengthFrom(200.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudInsideLength.Value = nudLength.Value - (decimal)UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudInsideWidth.Value = nudWidth.Value - (decimal)UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudInsideHeight.Value = nudHeight.Value - (decimal)UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudTapeWidth.Value = (decimal)UnitsManager.ConvertLengthFrom(50, UnitsManager.UnitSystem.UNIT_METRIC1);
                    cbTapeColor.Color = Color.Beige;
                    break;
                case Mode.MODE_BOX:
                    tbName.Text = _document.GetValidNewTypeName(Resources.ID_BOX);
                    nudLength.Value = (decimal)UnitsManager.ConvertLengthFrom(120.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudWidth.Value = (decimal)UnitsManager.ConvertLengthFrom(60.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudHeight.Value = (decimal)UnitsManager.ConvertLengthFrom(30.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudInsideLength.Value = nudLength.Value - (decimal)UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudInsideWidth.Value = nudWidth.Value - (decimal)UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    nudInsideHeight.Value = nudHeight.Value - (decimal)UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                    break;
                default:
                    break;
            }
            // description (same as name)
            tbDescription.Text = tbName.Text;
            // color : all faces set together / face by face
            chkAllFaces.Checked = false;
            chkAllFaces_CheckedChanged(this, null);
            // set colors
            for (int i=0; i<6; ++i)
                _faceColors[i] = _mode == Mode.MODE_BOX ? Color.Turquoise : Color.Chocolate;
            // set textures
            _textures = new List<Pair<HalfAxis.HAxis, Texture>>();
            // set default face
            cbFace.SelectedIndex = 0;
            // net weight
            NetWeight = new OptDouble(false, UnitsManager.ConvertMassFrom(0.0, UnitsManager.UnitSystem.UNIT_METRIC1));
            // disable Ok button
            UpdateButtonOkStatus();
        }
        /// <summary>
        /// FormNewBox constructor used to edit existing boxes
        /// </summary>
        /// <param name="document">Document that contains the edited box</param>
        /// <param name="boxProperties">Edited box</param>
        public FormNewBox(Document document, BoxProperties boxProperties)
        { 
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // save document reference
            _document = document;
            _boxProperties = boxProperties;
            _mode = boxProperties.HasInsideDimensions ? Mode.MODE_CASE : Mode.MODE_BOX;
            // set colors
            for (int i=0; i<6; ++i)
                _faceColors[i] = _boxProperties.Colors[i];
            // set textures
            _textures = _boxProperties.TextureListCopy;
            // set caption text
            Text = string.Format(Properties.Resources.ID_EDIT, _boxProperties.Name);
            // initialize value
            tbName.Text = _boxProperties.Name;
            tbDescription.Text = _boxProperties.Description;
            nudLength.Value = (decimal)_boxProperties.Length;
            nudInsideLength.Value = (decimal)_boxProperties.InsideLength;
            nudWidth.Value = (decimal)_boxProperties.Width;
            nudInsideWidth.Value = (decimal)_boxProperties.InsideWidth;
            nudHeight.Value = (decimal)_boxProperties.Height;
            nudInsideHeight.Value = (decimal)_boxProperties.InsideHeight;
            // weight
            vcWeight.Value = _boxProperties.Weight;
            // net weight
            ovcNetWeight.Value = _boxProperties.NetWeight;
            // color : all faces set together / face by face
            chkAllFaces.Checked = _boxProperties.UniqueColor;
            chkAllFaces_CheckedChanged(this, null);
            // tape
            checkBoxTape.Checked = _boxProperties.ShowTape;
            nudTapeWidth.Value = (decimal)_boxProperties.TapeWidth;
            cbTapeColor.Color = _boxProperties.TapeColor;
            // set default face
            cbFace.SelectedIndex = 0;
            // disable Ok button
            UpdateButtonOkStatus();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Name
        /// </summary>
        public string BoxName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        /// <summary>
        /// Length
        /// </summary>
        public double BoxLength
        {
            get { return (double)nudLength.Value; }
            set { nudLength.Value = (decimal)value; }
        }
        /// <summary>
        /// Width
        /// </summary>
        public double BoxWidth
        {
            get { return (double)nudWidth.Value; }
            set { nudWidth.Value = (decimal)value; }
        }
        /// <summary>
        /// Height
        /// </summary>
        public double BoxHeight
        {
            get { return (double)nudHeight.Value; }
            set { nudHeight.Value = (decimal)value; }
        }
        /// <summary>
        /// Inside length
        /// </summary>
        public double InsideLength
        {
            get { return (double)nudInsideLength.Value; }
            set { nudInsideLength.Value = (decimal)value; }
        }
        /// <summary>
        /// Inside width
        /// </summary>
        public double InsideWidth
        {
            get { return (double)nudInsideWidth.Value; }
            set { nudInsideWidth.Value = (decimal)value; }
        }
        /// <summary>
        /// Inside height
        /// </summary>
        public double InsideHeight
        {
            get { return (double)nudInsideHeight.Value; }
            set { nudInsideHeight.Value = (decimal)value; }
        }
        /// <summary>
        /// Weight
        /// </summary>
        public double Weight
        {
            get { return vcWeight.Value; }
            set { vcWeight.Value = value; }
        }
        /// <summary>
        /// Colors
        /// </summary>
        public Color[] Colors
        {
            get { return _faceColors; }
            set { }
        }
        /// <summary>
        /// Textures
        /// </summary>
        public List<Pair<HalfAxis.HAxis, Texture>> TextureList
        {
            get {   return _textures;   }
            set
            {
                _textures.Clear();
                _textures.AddRange(value);
            }
        }
        /// <summary>
        /// Show / Hide tape
        /// </summary>
        public bool ShowTape
        {
            get { return checkBoxTape.Checked; }
            set { checkBoxTape.Checked = value; }
        }
        /// <summary>
        /// Tape width
        /// </summary>
        public double TapeWidth
        {
            get { return (double)nudTapeWidth.Value; }
            set { nudTapeWidth.Value = (decimal)value; }
        }
        /// <summary>
        /// Tape color
        /// </summary>
        public Color TapeColor
        {
            get { return cbTapeColor.Color;}
            set { cbTapeColor.Color = value; }
        }
        #endregion

        #region Load / FormClosing event
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            graphCtrl.DrawingContainer = this;

            // show hide inside dimensions controls
            nudInsideLength.Visible = _mode == Mode.MODE_CASE;
            nudInsideWidth.Visible = _mode == Mode.MODE_CASE;
            nudInsideHeight.Visible = _mode == Mode.MODE_CASE;
            lbInsideLength.Visible = _mode == Mode.MODE_CASE;
            lbInsideWidth.Visible = _mode == Mode.MODE_CASE;
            lbInsideHeight.Visible = _mode == Mode.MODE_CASE;
            uLengthLengthInside.Visible = _mode == Mode.MODE_CASE;
            uLengthWidthInside.Visible = _mode == Mode.MODE_CASE;
            uLengthHeightInside.Visible = _mode == Mode.MODE_CASE;

            gbTape.Visible = _mode == Mode.MODE_CASE;
            checkBoxTape.Visible = _mode == Mode.MODE_CASE;
            lbTapeColor.Visible = _mode == Mode.MODE_CASE;
            cbTapeColor.Visible = _mode == Mode.MODE_CASE;
            lbTapeWidth.Visible = _mode == Mode.MODE_CASE;
            nudTapeWidth.Visible = _mode == Mode.MODE_CASE;
            // caption
            this.Text = Mode.MODE_CASE == _mode ? Resources.ID_ADDNEWCASE : Resources.ID_ADDNEWBOX;
            // update thicknesses
            UpdateThicknesses();
            // update tape definition controls
            checkBoxTape_CheckedChanged(this, null);
            // update box drawing
            graphCtrl.Invalidate();
            // windows settings
            if (null != Settings.Default.FormNewBoxPosition)
                Settings.Default.FormNewBoxPosition.Restore(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // window position
            if (null == Settings.Default.FormNewBoxPosition)
                Settings.Default.FormNewBoxPosition = new WindowSettings();
            Settings.Default.FormNewBoxPosition.Record(this);
        }
        #endregion

        #region Form override
        protected override void OnResize(EventArgs e)
        {
 	         base.OnResize(e);
        }
        #endregion

        #region Handlers
        private void onBoxPropertyChanged(object sender, EventArgs e)
        {
            // maintain inside dimensions
            NumericUpDown nud = sender as NumericUpDown;
            if (null != nud)
            {
                // length
                if (nudLength == nud)
                    InsideLength = BoxLength - _thicknessLength;
                else if (nudInsideLength == nud && BoxLength < InsideLength)
                    BoxLength = InsideLength + _thicknessLength;
                // width
                if (nudWidth == nud)
                    InsideWidth = BoxWidth - _thicknessWidth;
                else if (nudInsideWidth == nud && BoxWidth < InsideWidth)
                    BoxWidth = InsideWidth + _thicknessWidth;
                // height
                if (nudHeight == nud)
                    InsideHeight = BoxHeight - _thicknessHeight;
                else if (nudInsideHeight == nud && BoxHeight <= InsideHeight)
                    BoxHeight = InsideHeight + _thicknessHeight;
            }
            // update thicknesses
            UpdateThicknesses();
            // update ok button status
            UpdateButtonOkStatus();
            // update box drawing
            graphCtrl.Invalidate();
        }

        private void onSelectedFaceChanged(object sender, EventArgs e)
        {
            // get current index
            int iSel = cbFace.SelectedIndex;
            cbColor.Color = _faceColors[iSel];
            graphCtrl.Invalidate();
        }
        private void onFaceColorChanged(object sender, EventArgs e)
        {
            if (!chkAllFaces.Checked)
            {
                int iSel = cbFace.SelectedIndex;
                if (iSel >=0 && iSel < 6)
                    _faceColors[iSel] = cbColor.Color;
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                    _faceColors[i] = cbColor.Color;
            }
            graphCtrl.Invalidate();
        }

        private void UpdateButtonOkStatus()
        {
            // status + message
            string message = string.Empty;
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;
            else if (!_document.IsValidNewTypeName(tbName.Text, _boxProperties))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            // description
            else if (string.IsNullOrEmpty(tbDescription.Text))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;
            // case length consistency
            else if (_mode == Mode.MODE_CASE && InsideLength > BoxLength)
                message = string.Format(Resources.ID_INVALIDINSIDELENGTH, InsideLength, BoxLength);
            // case width consistency
            else if (_mode == Mode.MODE_CASE && InsideWidth > BoxWidth)
                message = string.Format(Resources.ID_INVALIDINSIDEWIDTH, InsideWidth, BoxWidth);
            // case height consistency
            else if (_mode == Mode.MODE_CASE && InsideHeight > BoxHeight)
                message = string.Format(Resources.ID_INVALIDINSIDEHEIGHT, InsideHeight, BoxHeight);
            // box/case net weight consistency
            else if (NetWeight.Activated && NetWeight > Weight)
                message = string.Format(Resources.ID_INVALIDNETWEIGHT, NetWeight.Value, Weight);
            // accept
            bnOK.Enabled = string.IsNullOrEmpty(message);
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
        }

        private void onNameDescriptionChanged(object sender, EventArgs e)
        {
            UpdateButtonOkStatus();
        }

        private void chkAllFaces_CheckedChanged(object sender, EventArgs e)
        {
            lbFace.Enabled = !chkAllFaces.Checked;
            cbFace.Enabled = !chkAllFaces.Checked;
            if (chkAllFaces.Checked)
                cbColor.Color = _faceColors[0];
        }
        private void btBitmaps_Click(object sender, EventArgs e)
        {
            try
            {
                FormEditBitmaps form = null;
                if (null == _boxProperties)
                    form = new FormEditBitmaps(BoxLength, BoxWidth, BoxHeight, _faceColors);
                else
                    form = new FormEditBitmaps(_boxProperties);
                form.Textures = _textures;
                if (DialogResult.OK == form.ShowDialog())
                    _textures = form.Textures;
                graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void checkBoxTape_CheckedChanged(object sender, EventArgs e)
        {
            lbTapeColor.Enabled = checkBoxTape.Checked;
            cbTapeColor.Enabled = checkBoxTape.Checked;
            lbTapeWidth.Enabled = checkBoxTape.Checked;
            nudTapeWidth.Enabled = checkBoxTape.Checked;
            uLengthTapeWidth.Enabled = checkBoxTape.Checked;
            graphCtrl.Invalidate();
        }      
        #endregion

        #region Helpers
        private void UpdateThicknesses()
        {
            _thicknessLength = BoxLength - InsideLength;
            _thicknessWidth = BoxWidth - InsideWidth;
            _thicknessHeight = BoxHeight - InsideHeight;
        }
        #endregion

        #region Net weight
        public OptDouble NetWeight
        {
            get { return ovcNetWeight.Value; }
            set { ovcNetWeight.Value = value; }
        }
        #endregion

        #region Draw box
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            BoxProperties boxProperties = new BoxProperties(null, (double)nudLength.Value, (double)nudWidth.Value, (double)nudHeight.Value);
            boxProperties.SetAllColors(_faceColors);
            boxProperties.TextureList = _textures;
            boxProperties.ShowTape = ShowTape;
            boxProperties.TapeColor = TapeColor;
            boxProperties.TapeWidth = TapeWidth;
            Box box = new Box(0, boxProperties);
            graphics.AddBox(box);
            graphics.AddDimensions(new DimensionCube((double)nudLength.Value, (double)nudWidth.Value, (double)nudHeight.Value));
        }
        #endregion

    }
}