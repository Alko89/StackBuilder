﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Sharp3D.Math.Core;
using TreeDim.StackBuilder.Basics;
using TreeDim.StackBuilder.Graphics;
using TreeDim.StackBuilder.Engine;
using TreeDim.StackBuilder.Desktop.Properties;
using log4net;
#endregion

namespace TreeDim.StackBuilder.Desktop
{
    /// <summary>
    /// This forms enables optimizing case dimensions
    /// </summary>
    public partial class FormOptimizeCase : Form, IDrawingContainer
    {
        #region Data members
        private DocumentSB _document;
        private List<CaseOptimSolution> _solutions = new List<CaseOptimSolution>();
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormOptimizeCase));
        #endregion

        #region Combo box item private classes
        private class BoxItem
        {
            private BProperties _boxProperties;
            /// <summary>
            /// Constructor
            /// </summary>
            public BoxItem(BProperties boxProperties)
            {
                _boxProperties = boxProperties;
            }
            /// <summary>
            /// returns the inner item
            /// </summary>
            public BProperties Item
            {
                get { return _boxProperties; }
            }
            /// <summary>
            /// return the box name to be displayed by combo box
            /// </summary>
            public override string ToString()
            {
                return _boxProperties.Name;
            }
        }
        private class PalletItem
        {
            private PalletProperties _palletProperties;
            /// <summary>
            /// constructor
            /// </summary>
            public PalletItem(PalletProperties palletProperties)
            {
                _palletProperties = palletProperties;
            }
            /// <summary>
            /// returns the inner item
            /// </summary>
            public PalletProperties Item
            {
                get { return _palletProperties; }
            }
            /// <summary>
            /// returns the pallet name to be displayed by combo box
            /// </summary>
            public override string ToString()
            {
                return _palletProperties.Name;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor takes document as argument
        /// </summary>
        public FormOptimizeCase(DocumentSB document)
        {
            InitializeComponent();
            // set unit labels
            UnitsManager.AdaptUnitLabels(this);
            // document
            _document = document;

        }
        #endregion

        #region Load / Close methods
        private void FormOptimizeCase_Load(object sender, EventArgs e)
        {
            graphCtrlBoxesLayout.DrawingContainer = this;
            graphCtrlPallet.DrawingContainer = this;
            // intialize box combo
            foreach (BoxProperties bProperties in _document.Boxes)
                cbBoxes.Items.Add(new BoxItem(bProperties));
            if (cbBoxes.Items.Count > 0)
                cbBoxes.SelectedIndex = 0;
            // initialize pallet combo
            foreach (PalletProperties palletProperties in _document.Pallets)
                cbPallet.Items.Add(new PalletItem(palletProperties));
            if (cbPallet.Items.Count > 0)
                cbPallet.SelectedIndex = 0;
            // set default pallet height
            MaximumPalletHeight = Settings.Default.PalletHeight;
            // set default wall numbers and thickness
            nudWallsLengthDir.Value = Settings.Default.NumberWallsLength;
            nudWallsWidthDir.Value = Settings.Default.NumberWallsWidth;
            nudWallsHeightDir.Value = Settings.Default.NumberWallsHeight;
            nudWallThickness.Value = (decimal)Settings.Default.WallThickness;
            nudWallSurfaceMass.Value = (decimal)Settings.Default.WallSurfaceMass;
            nudNumber.Value = Settings.Default.NumberBoxesPerCase;
            // set vertical orientation only
            ForceVerticalBoxOrientation = Settings.Default.ForceVerticalBoxOrientation;
            // set min / max case dimensions
            SetMinCaseDimensions();
            SetMaxCaseDimensions();
            // set event handler for grid selection change event
            gridSolutions.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(Selection_SelectionChanged);
            // fill grid
            FillGrid();

            UpdateButtonOptimizeStatus();
            UpdateButtonAddSolutionStatus();
            // windows settings
            if (null != Settings.Default.FormOptimizeCasePosition)
                Settings.Default.FormOptimizeCasePosition.Restore(this);
        }

        private void FormOptimizeCase_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // save settings
                Settings.Default.NumberWallsLength = (int)nudWallsLengthDir.Value;
                Settings.Default.NumberWallsWidth = (int)nudWallsWidthDir.Value;
                Settings.Default.NumberWallsHeight = (int)nudWallsHeightDir.Value;
                Settings.Default.PalletHeight = MaximumPalletHeight;
                Settings.Default.WallThickness = (double)nudWallThickness.Value;
                Settings.Default.WallSurfaceMass = (double)nudWallSurfaceMass.Value;
                Settings.Default.NumberBoxesPerCase = (int)nudNumber.Value;
                // window position
                if (null == Settings.Default.FormOptimizeCasePosition)
                    Settings.Default.FormOptimizeCasePosition = new WindowSettings();
                Settings.Default.FormOptimizeCasePosition.Record(this);
            }
            catch (Exception ex)
            { _log.Error(ex.ToString()); }
        }
        #endregion

        #region Status
        private void UpdateButtonOptimizeStatus()
        {
            string message = string.Empty;
            // compute maximum volume
            double maxVol = MaxLength * MaxWidth * MaxHeight;
            // compare max vol with volume of Number 
            if (MaxLength <= MinLength)
                message = string.Format(Resources.ID_MAXLOWERTHANMIN, Resources.ID_LENGTH, Resources.ID_LENGTH);
            else if (MaxWidth <= MinWidth)
                message = string.Format(Resources.ID_MAXLOWERTHANMIN, Resources.ID_WIDTH, Resources.ID_WIDTH);
            else if (MaxHeight <= MinHeight)
                message = string.Format(Resources.ID_MAXLOWERTHANMIN, Resources.ID_HEIGHT, Resources.ID_HEIGHT);
            else if (maxVol < BoxPerCase * SelectedBox.Volume)
                message = string.Format(Resources.ID_INSUFFICIENTVOLUME, BoxPerCase, SelectedBox.Name);
            else if (MaximumPalletHeight < MinHeight + SelectedPallet.Height)
                message = string.Format(Resources.ID_INSUFFICIENTPALLETHEIGHT
                    , MaximumPalletHeight, UnitsManager.LengthUnitString
                    , MinHeight + SelectedPallet.Height, UnitsManager.LengthUnitString);
            // btOptimize
            btOptimize.Enabled = string.IsNullOrEmpty(message);
            // status bar
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
        }

        private void UpdateButtonAddSolutionStatus()
        {
            btAddCasePalletAnalysis.Enabled = (null != SelectedSolution);
            btAddPackPalletAnalysis.Enabled = (null != SelectedSolution);
        }
        #endregion

        #region Event handlers
        private void cbBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            BoxItem boxItem = cbBoxes.SelectedItem as BoxItem;
            BProperties bProperties = null != boxItem ? boxItem.Item : null;
            lbBoxDimensions.Text = null != bProperties ?
                string.Format("({0}*{1}*{2})", bProperties.Length, bProperties.Width, bProperties.Height)
                : string.Empty;
            OptimizationParameterChanged(sender, e);
            SetMinCaseDimensions();
        }
        private void cbPallet_SelectedIndexChanged(object sender, EventArgs e)
        {
            PalletItem palletItem = cbPallet.SelectedItem as PalletItem;
            PalletProperties palletProperties = null != palletItem ? palletItem.Item : null;
            lbPalletDimensions.Text = null != palletProperties ?
                string.Format("({0}*{1}*{2})", palletProperties.Length, palletProperties.Width, palletProperties.Height)
                : string.Empty;
            OptimizationParameterChanged(sender, e);
            SetMaxCaseDimensions();
        }
        private void btOptimize_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // build case optimizer and compute solutions
                CaseOptimizer caseOptimizer = new CaseOptimizer(
                    SelectedBox                     // BoxProperties
                    , SelectedPallet                // PalletProperties
                    , BuildPalletConstraintSet()    // ConstraintSet
                    , BuildCaseOptimConstraintSet() // CaseOptimConstraintSet
                    );
                _solutions = caseOptimizer.CaseOptimSolutions(BoxPerCase);

                // fill grid using solutions
                FillGrid();
                UpdateButtonAddSolutionStatus();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void btAddCasePalletAnalysis_Click(object sender, EventArgs e)
        { 
           try
            {
                // get selected box
                BoxProperties boxProperties = SelectedBox;
                // get selected pallet
                PalletProperties palletProperties = SelectedPallet;
                // get selected caseOptimSolution
                CaseOptimSolution sol = SelectedSolution;
                PackArrangement arrangement = sol.CaseDefinition.Arrangement;
                // build new case name
                string name = string.Format("{0}_{1}*{2}*{3}_{4}{5}"
                    , boxProperties.Name
                    , arrangement._iLength
                    , arrangement._iWidth
                    , arrangement._iHeight
                    , sol.CaseDefinition.Dim0
                    , sol.CaseDefinition.Dim1);
                // build new case description
                string description = string.Format("Case generated by case optimization for box {0} and pallet {1}"
                    , boxProperties.Name
                    , palletProperties.Name);
                // analysis name/description
                string analysisName = string.Format("OptimAnalysis{0}_{1}*{2}*{3}_{4}{5}"
                    , boxProperties.Name
                    , boxProperties.Name
                    , arrangement._iLength
                    , arrangement._iWidth
                    , arrangement._iHeight
                    , sol.CaseDefinition.Dim0
                    , sol.CaseDefinition.Dim1);
                string analysisDescription = string.Format("Pallet analysis generated by case optimisation for box {0} and pallet {1}"
                    , boxProperties.Name
                    , palletProperties.Name);
                // create PackProperties
                HalfAxis.HAxis axis = PackProperties.Orientation(sol.CaseDefinition.Dim0, sol.CaseDefinition.Dim1);
                double wrapperWeight =  WrapperCardboard.EstimateWeight(
                            SelectedBox
                            , arrangement
                            , axis
                            , NoWalls
                            , WallThickness
                            , WallSurfaceMass);
                // add new case
                CaseOfBoxesProperties caseProperties = _document.CreateNewCaseOfBoxes(
                    name, description
                    , boxProperties
                    , sol.CaseDefinition
                    , BuildCaseOptimConstraintSet());
                // set color
                caseProperties.SetColor(Color.Chocolate);
                // add new pallet analysis

                List<CasePalletSolution> palletSolutionList = new List<CasePalletSolution>();
                palletSolutionList.Add(sol.PalletSolution);
                CasePalletAnalysis analysis = _document.CreateNewCasePalletAnalysis(
                    analysisName
                    , analysisDescription
                    , caseProperties
                    , palletProperties
                    , null, null
                    , null, null, null
                    , BuildPalletConstraintSet()
                    , palletSolutionList);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }            
        }
        private void btAddPackPalletAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                // get selected box
                BoxProperties boxProperties = SelectedBox;
                // get selected pallet
                PalletProperties palletProperties = SelectedPallet;
                // get selected caseOptimSolution
                CaseOptimSolution sol = SelectedSolution;
                PackArrangement arrangement = sol.CaseDefinition.Arrangement;
                // build new case name
                string name = string.Format("{0}_{1}*{2}*{3}_{4}{5}"
                    , boxProperties.Name
                    , arrangement._iLength
                    , arrangement._iWidth
                    , arrangement._iHeight
                    , sol.CaseDefinition.Dim0
                    , sol.CaseDefinition.Dim1);
                // build new case description
                string description = string.Format("Case generated by case optimization for box {0} and pallet {1}"
                    , boxProperties.Name
                    , palletProperties.Name);
                // analysis name/description
                string analysisName = string.Format("OptimAnalysis{0}_{1}*{2}*{3}_{4}{5}"
                    , boxProperties.Name
                    , boxProperties.Name
                    , arrangement._iLength
                    , arrangement._iWidth
                    , arrangement._iHeight
                    , sol.CaseDefinition.Dim0
                    , sol.CaseDefinition.Dim1);
                string analysisDescription = string.Format("Pallet analysis generated by case optimisation for box {0} and pallet {1}"
                    , boxProperties.Name
                    , palletProperties.Name);
                // create PackProperties
                HalfAxis.HAxis axis = PackProperties.Orientation(sol.CaseDefinition.Dim0, sol.CaseDefinition.Dim1);
                double wrapperWeight =  WrapperCardboard.EstimateWeight(
                            SelectedBox
                            , arrangement
                            , axis
                            , NoWalls
                            , WallThickness
                            , WallSurfaceMass);
                // cardboard wrapper
                WrapperCardboard wrapper = new WrapperCardboard(WallThickness, wrapperWeight, Color.Chocolate);
                wrapper.SetNoWalls(NoWalls[0], NoWalls[1], NoWalls[2]);
                // pack
                PackProperties pack = _document.CreateNewPack(
                    name, description
                    , boxProperties, arrangement, axis
                    , wrapper);
                // constraint set
                PackPalletConstraintSet constraintSet = new PackPalletConstraintSet();
                constraintSet.MaximumPalletHeight = new OptDouble(true, MaximumPalletHeight);
                // create analysis
                _document.CreateNewPackPalletAnalysis(
                    analysisName
                    , analysisDescription
                    , pack
                    , palletProperties
                    , null
                    , constraintSet
                    , new PackPalletSolver());
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void btSetMinimum_Click(object sender, EventArgs e)
        {
            try { SetMinCaseDimensions(); }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void btSetMaximum_Click(object sender, EventArgs e)
        {
            try { SetMaxCaseDimensions(); }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        void Selection_SelectionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            // redraw
            graphCtrlBoxesLayout.Invalidate();
            graphCtrlPallet.Invalidate();
            // update "Add solution" button status
            UpdateButtonAddSolutionStatus();
        }
        private void OptimizationParameterChanged(object sender, EventArgs e)
        {
            // update optimize button status
            UpdateButtonOptimizeStatus();
            // clear grid
            _solutions.Clear();
            FillGrid();
        }
        #endregion

        #region Private properties
        private double OverhangX { get { return uCtrlOverhangLength.Value; } }
        private double OverhangY { get { return uCtrlOverhangWidth.Value; } }
        private double MaxLength { get { return (double)nudMaxCaseLength.Value; } }
        private double MaxWidth { get { return (double)nudMaxCaseWidth.Value; } }
        private double MaxHeight { get { return (double)nudMaxCaseHeight.Value; } }
        private double MinLength
        {
            get { return (double)nudMinCaseLength.Value; }
            set { nudMinCaseLength.Value = (decimal)value; }
        }
        private double MinWidth
        {
            get { return (double)nudMinCaseWidth.Value; }
            set { nudMinCaseWidth.Value = (decimal)value; }
        }
        private double MinHeight
        {
            get { return (double)nudMinCaseHeight.Value; }
            set { nudMinCaseHeight.Value = (decimal)value; }
        }
        private int BoxPerCase { get { return (int)nudNumber.Value; } }
        private int[] NoWalls
        {
            get
            {
                int[] noWalls = new int[3];
                noWalls[0] = (int)nudWallsLengthDir.Value;
                noWalls[1] = (int)nudWallsWidthDir.Value;
                noWalls[2] = (int)nudWallsHeightDir.Value;
                return noWalls;
            }
        }
        private double WallThickness { get { return (double)nudWallThickness.Value; } }
        private double WallSurfaceMass { get { return (double)nudWallSurfaceMass.Value; } }

        private bool ForceVerticalBoxOrientation
        {
            get { return chkVerticalOrientationOnly.Checked; }
            set { chkVerticalOrientationOnly.Checked = value; }
        }
        private double MaximumPalletHeight
        {
            get { return uCtrlPalletHeight.Value; }
            set { uCtrlPalletHeight.Value = value; }
        }
        #endregion

        #region Grid
        private void FillGrid()
        {
            try
            {
                // fill grid solution
                gridSolutions.Rows.Clear();

                // border
                DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.DarkBlue, 1);
                DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);

                // views
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                viewNormal.Border = cellBorder;
                CheckboxBackColorAlternate viewNormalCheck = new CheckboxBackColorAlternate(Color.LightBlue, Color.White);
                viewNormalCheck.Border = cellBorder;

                // column header view
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
                DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
                backHeader.BackColor = Color.LightGray;
                backHeader.Border = DevAge.Drawing.RectangleBorder.NoBorder;
                viewColumnHeader.Background = backHeader;
                viewColumnHeader.ForeColor = Color.White;
                viewColumnHeader.Font = new Font("Arial", 8, FontStyle.Regular);
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;

                // create the grid
                gridSolutions.BorderStyle = BorderStyle.FixedSingle;

                gridSolutions.ColumnsCount = 12;
                gridSolutions.FixedRows = 1;
                gridSolutions.Rows.Insert(0);

                // header
                SourceGrid.Cells.ColumnHeader columnHeader;
                // 0
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_A1);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 0] = columnHeader;
                // 1
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_A2);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 1] = columnHeader;
                // 2
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_A3);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 2] = columnHeader;
                // 3
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_LENGTH);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 3] = columnHeader;
                // 4
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_WIDTH);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 4] = columnHeader;
                // 5
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_HEIGHT);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 5] = columnHeader;
                // 6
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_AREA + "/" + Resources.ID_WEIGHT);
                columnHeader.AutomaticSortEnabled = false;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 6] = columnHeader;
                // 7
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_CASESLAYER);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 7] = columnHeader;
                // 8
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_LAYERS);
                columnHeader.AutomaticSortEnabled = false;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 8] = columnHeader;
                // 9
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_CASESPALLET);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 9] = columnHeader;
                // 10
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_VOLUMEEFFICIENCY);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 10] = columnHeader;
                // 11
                columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_MAXIMUMSPACE);
                columnHeader.AutomaticSortEnabled = true;
                columnHeader.View = viewColumnHeader;
                gridSolutions[0, 11] = columnHeader;

                // column width
                gridSolutions.Columns[0].Width = 30;
                gridSolutions.Columns[1].Width = 30;
                gridSolutions.Columns[2].Width = 30;
                gridSolutions.Columns[3].Width = 50;
                gridSolutions.Columns[4].Width = 50;
                gridSolutions.Columns[5].Width = 50;
                gridSolutions.Columns[6].Width = 80;
                gridSolutions.Columns[7].Width = 80;
                gridSolutions.Columns[8].Width = 50;
                gridSolutions.Columns[9].Width = 80;
                gridSolutions.Columns[10].Width = 100;
                gridSolutions.Columns[11].Width = 80;

                // get BoxProperties
                BoxProperties boxProperties = SelectedBox;
                PalletProperties palletProperties = SelectedPallet;
                CaseOptimConstraintSet caseOptimConstraintSet = BuildCaseOptimConstraintSet();
                PalletConstraintSet palletConstraintSet = new CasePalletConstraintSet();
                palletConstraintSet.MaximumHeight = MaximumPalletHeight;
                // data
                int iIndex = 0;
                foreach (CaseOptimSolution sol in _solutions)
                {
                    // insert new row
                    gridSolutions.Rows.Insert(++iIndex);
                    gridSolutions.Rows[iIndex].Tag = sol;
                    // A1
                    gridSolutions[iIndex, 0] = new SourceGrid.Cells.Cell(sol.CaseDefinition.Arrangement._iLength);
                    // A2
                    gridSolutions[iIndex, 1] = new SourceGrid.Cells.Cell(sol.CaseDefinition.Arrangement._iWidth);
                    // A3
                    gridSolutions[iIndex, 2] = new SourceGrid.Cells.Cell(sol.CaseDefinition.Arrangement._iHeight);
                    // Case inner dimensions
                    Vector3D innerDim = sol.CaseDefinition.InnerDimensions(boxProperties);
                    // LENGTH
                    gridSolutions[iIndex, 3] = new SourceGrid.Cells.Cell(Math.Round(innerDim.X, 1));
                    // WIDTH
                    gridSolutions[iIndex, 4] = new SourceGrid.Cells.Cell(Math.Round(innerDim.Y, 1));
                    // HEIGHT
                    gridSolutions[iIndex, 5] = new SourceGrid.Cells.Cell(Math.Round(innerDim.Z, 1));
                    // AREA
                    gridSolutions[iIndex, 6] = new SourceGrid.Cells.Cell(string.Format("{0:0.00} / {1:0.000}"
                        , sol.CaseDefinition.Area(boxProperties, caseOptimConstraintSet)
                        , sol.CaseDefinition.EmptyWeight(boxProperties, caseOptimConstraintSet) 
                        ));
                    // CASES PER LAYER
                    gridSolutions[iIndex, 7] = new SourceGrid.Cells.Cell(sol.PalletSolution[0].BoxCount);
                    // LAYERS
                    gridSolutions[iIndex, 8] = new SourceGrid.Cells.Cell(sol.PalletSolution.Count);
                    // CASES PER PALLET
                    gridSolutions[iIndex, 9] = new SourceGrid.Cells.Cell(sol.CaseCount);
                    // EFFICIENCY
                    double efficiency = 100.0 * sol.CaseCount * sol.CaseDefinition.InnerVolume(boxProperties) /
                        ((palletProperties.Length - palletConstraintSet.OverhangX)
                        * (palletProperties.Width - palletConstraintSet.OverhangY)
                        * (palletConstraintSet.MaximumHeight - palletProperties.Height)
                        );
                    gridSolutions[iIndex, 10] = new SourceGrid.Cells.Cell(Math.Round(efficiency, 1));
                    // MAXIMUM SPACE
                    gridSolutions[iIndex, 11] = new SourceGrid.Cells.Cell(Math.Round(sol.PalletSolution.MaximumSpace, 1));
                }
                // select first solution
                if (_solutions.Count > 0)
                {
                    gridSolutions.Selection.EnableMultiSelection = false;
                    gridSolutions.Selection.SelectRow(1, true);
                }
            }
            catch (Exception ex)
            {   _log.Error(ex.ToString());  }

            graphCtrlBoxesLayout.Invalidate();
            graphCtrlPallet.Invalidate();

            UpdateButtonAddSolutionStatus();
        }
        #endregion

        #region Drawing
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (ctrl == graphCtrlBoxesLayout)
            {
                // ### draw case definition
                try
                {
                    // get selected solution
                    CaseOptimSolution solution = SelectedSolution;
                    if (null == solution) return;
                    // instantiate case definition viewer
                    CaseDefinitionViewer cdv = new CaseDefinitionViewer(SelectedSolution.CaseDefinition, SelectedBox, BuildCaseOptimConstraintSet());
                    cdv.Orientation = SelectedSolution.PalletSolution.FirstCaseOrientation;
                    cdv.Draw(graphics);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
            if (ctrl == graphCtrlPallet)
            {
                // ### draw associated pallet solution
                try
                {
                    // get selected solution
                    CaseOptimSolution solution = SelectedSolution;
                    // get selected box
                    BoxProperties boxProperties = SelectedBox;
                    // get selected pallet
                    PalletProperties palletProperties = SelectedPallet;
                    if (null != solution && null != boxProperties && null != palletProperties)
                    {
                        Vector3D outerDim = solution.CaseDefinition.OuterDimensions(boxProperties, BuildCaseOptimConstraintSet());
                        BoxProperties caseProperties = new BoxProperties(null, outerDim.X, outerDim.Y, outerDim.Z);
                        caseProperties.SetColor(Color.Chocolate);
                        CasePalletSolutionViewer.Draw(graphics, solution.PalletSolution, caseProperties, null, palletProperties);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
        }
        #endregion

        #region Helpers
        private BoxProperties SelectedBox
        {
            get
            {
                BoxItem boxItem = cbBoxes.SelectedItem as BoxItem;
                return boxItem != null ? boxItem.Item as BoxProperties : null;
            }
        }
        private PalletProperties SelectedPallet
        {
            get
            {
                PalletItem palletItem = cbPallet.SelectedItem as PalletItem;
                return palletItem != null ? palletItem.Item as PalletProperties : null;
            }
        }
        private int SelectedSolutionIndex
        {
            get
            {
                CaseOptimSolution sol = SelectedSolution;
                if (null == sol) return -1;
                // find and return index of sol
                return _solutions.IndexOf(sol, 0);
            }
        }
        private CaseOptimSolution SelectedSolution
        {
            get
            {
                SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
                int[] indexes = region.GetRowsIndex();
                // no selection -> exit
                if (indexes.Length == 0) return null;
                CaseOptimSolution sol = gridSolutions.Rows[indexes[0]].Tag as CaseOptimSolution;
                return sol;
            }
        }
        /// <summary>
        /// Use box dimensions to set min case dimensions
        /// </summary>
        private void SetMinCaseDimensions()
        {
            // get selected box
            BProperties boxProperties = SelectedBox;
            if (null == boxProperties) return;
            // compute min dimension
            double minDim = Math.Min(boxProperties.Length, Math.Min(boxProperties.Width, boxProperties.Height));
            if ((int)nudNumber.Value > 8)
                minDim *= 2;
            // set min dimension
            MinLength = minDim;
            MinWidth = minDim;
            MinHeight = minDim;

            // update message + enable/disable optimise button
            UpdateButtonOptimizeStatus();
        }
        private void SetMaxCaseDimensions()
        {
            PalletProperties palletProperties = SelectedPallet;
            if (null == palletProperties) return;
            // use pallet dimensions to set max case dimensions
            nudMaxCaseLength.Value = (decimal)(palletProperties.Length * 0.5);
            nudMaxCaseWidth.Value = (decimal)(palletProperties.Width * 0.5);
            nudMaxCaseHeight.Value = (decimal)(MaximumPalletHeight * 0.5);
            // update message + enable/disable optimise button
            UpdateButtonOptimizeStatus();
        }
        private CaseOptimConstraintSet BuildCaseOptimConstraintSet()
        {
            return new CaseOptimConstraintSet(
                    OverhangX
                    , OverhangY
                    , NoWalls
                    , WallThickness, WallSurfaceMass
                    , new Vector3D(MinLength, MinWidth, MinHeight)
                    , new Vector3D(MaxLength, MaxWidth, MaxHeight)
                    , ForceVerticalBoxOrientation
                    );
        }
        private PalletConstraintSet BuildPalletConstraintSet()
        {
            // build pallet constraint set
            CasePalletConstraintSet palletConstraintSet = new CasePalletConstraintSet();
            palletConstraintSet.SetAllowedOrthoAxis(HalfAxis.HAxis.AXIS_X_N, !ForceVerticalBoxOrientation);
            palletConstraintSet.SetAllowedOrthoAxis(HalfAxis.HAxis.AXIS_X_P, !ForceVerticalBoxOrientation);
            palletConstraintSet.SetAllowedOrthoAxis(HalfAxis.HAxis.AXIS_Y_N, !ForceVerticalBoxOrientation);
            palletConstraintSet.SetAllowedOrthoAxis(HalfAxis.HAxis.AXIS_Y_P, !ForceVerticalBoxOrientation);
            palletConstraintSet.SetAllowedOrthoAxis(HalfAxis.HAxis.AXIS_Z_N, !ForceVerticalBoxOrientation);
            palletConstraintSet.SetAllowedOrthoAxis(HalfAxis.HAxis.AXIS_Z_P, true);

            // use all existing patterns
            palletConstraintSet.AllowedPatternString = "Column,Diagonale,Interlocked,Trilock,Spirale";

            // allow aligned and alternate layers
            palletConstraintSet.AllowAlignedLayers = true;
            palletConstraintSet.AllowAlternateLayers = true;

            // set maximum pallet height
            palletConstraintSet.MaximumHeight = MaximumPalletHeight;
            palletConstraintSet.UseMaximumHeight = true;

            // do not use other constraints
            palletConstraintSet.UseMaximumPalletWeight = false;
            palletConstraintSet.UseMaximumNumberOfCases = false;
            palletConstraintSet.UseMaximumWeightOnBox = false;

            // overhang
            palletConstraintSet.OverhangX = OverhangX;
            palletConstraintSet.OverhangY = OverhangY;

            return palletConstraintSet;
        }

        #endregion
    }
}
