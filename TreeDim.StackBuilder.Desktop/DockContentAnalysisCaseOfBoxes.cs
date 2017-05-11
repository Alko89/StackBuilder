﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using log4net;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisCaseOfBoxes
        : DockContent, IView, IItemListener, IDrawingContainer
    {
        #region Data members
        /// <summary>
        /// document
        /// </summary>
        private IDocument _document;
        /// <summary>
        /// analysis
        /// </summary>
        private CasePalletAnalysis _analysis;
        /// <summary>
        /// view parameters
        /// </summary>
        private const double _cameraDistance = 10000.0;
        /// <summary>
        /// Currently selected solution
        /// </summary>
        private CasePalletSolution _sol;
        /// <summary>
        /// logger
        /// </summary>
        static readonly ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisCaseOfBoxes));

        #endregion

        #region Constructor
        public DockContentAnalysisCaseOfBoxes(IDocument document, CasePalletAnalysis analysis)
        {
            _document = document;
            _analysis = analysis;
            _analysis.AddListener(this);

            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 
            graphCtrlCase.DrawingContainer = this;
            graphCtrlPallet.DrawingContainer = this;
            // set window caption
            this.Text = _analysis.Name + " - " + _analysis.ParentDocument.Name;
            // fill grid
            FillGrid();

            gridSolutions.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(onGridSolutionSelectionChanged);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        #endregion

        #region Fill grid
        private void FillGrid()
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
            viewColumnHeader.Font = new Font("Arial", 10, FontStyle.Bold);
            viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;

            // create the grid
            gridSolutions.BorderStyle = BorderStyle.FixedSingle;

            gridSolutions.ColumnsCount = 7;
            gridSolutions.FixedRows = 1;
            gridSolutions.Rows.Insert(0);

            // header
            SourceGrid.Cells.ColumnHeader columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_INDEX);
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 0] = columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_LAYERPATTERN);
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 1] = columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_CASECOUNT);
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 2] = columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_VOLUMEEFFICIENCY);
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 3] = columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_PALLETWEIGHT, UnitsManager.MassUnitString));
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 4] = columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_PALLETHEIGHT, UnitsManager.LengthUnitString));
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 5] = columnHeader;

            columnHeader = new SourceGrid.Cells.ColumnHeader(Resources.ID_SELECTED);
            columnHeader.AutomaticSortEnabled = false;
            columnHeader.View = viewColumnHeader;
            gridSolutions[0, 6] = columnHeader;

            // handling check box click
            SourceGrid.Cells.Controllers.CustomEvents solCheckboxClickEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            solCheckboxClickEvent.Click += new EventHandler(clickEvent_Click);


            // data rows
            int iIndex = 0;
            foreach (CasePalletSolution sol in _analysis.Solutions)
            {
                ++iIndex;
                gridSolutions.Rows.Insert(iIndex);
                gridSolutions[iIndex, 0] = new SourceGrid.Cells.Cell(string.Format("{0}", iIndex));
                {
                    Graphics2DImage graphics = new Graphics2DImage(new Size(100, 50));
                    CasePalletSolutionViewer sv = new CasePalletSolutionViewer(sol);
                    sv.Draw(graphics);
                    gridSolutions[iIndex, 1] = new SourceGrid.Cells.Image(graphics.Bitmap);
                }
                gridSolutions[iIndex, 2] = new SourceGrid.Cells.Cell(string.Format("{0}", sol.CaseCount));
                gridSolutions[iIndex, 3] = new SourceGrid.Cells.Cell(string.Format("{0:F}", sol.VolumeEfficiencyCases));
                gridSolutions[iIndex, 4] = new SourceGrid.Cells.Cell(string.Format("{0:F}", sol.PalletWeight));
                gridSolutions[iIndex, 5] = new SourceGrid.Cells.Cell(string.Format("{0:F}", sol.PalletHeight));
                gridSolutions[iIndex, 6] = new SourceGrid.Cells.CheckBox(null, _analysis.HasSolutionSelected(iIndex - 1));

                gridSolutions[iIndex, 0].View = viewNormal;
                gridSolutions[iIndex, 1].View = viewNormal;
                gridSolutions[iIndex, 2].View = viewNormal;
                gridSolutions[iIndex, 3].View = viewNormal;
                gridSolutions[iIndex, 4].View = viewNormal;
                gridSolutions[iIndex, 5].View = viewNormal;
                gridSolutions[iIndex, 6].View = viewNormalCheck;

                gridSolutions[iIndex, 6].AddController(solCheckboxClickEvent);
            }
            gridSolutions.AutoStretchColumnsToFitWidth = true;
            gridSolutions.AutoSizeCells();
            gridSolutions.Columns.StretchToFit();

            // select first solution
            gridSolutions.Selection.SelectRow(1, true);
            graphCtrlCase.Invalidate();
            graphCtrlPallet.Invalidate();
        }
        #endregion

        #region Public properties
        public CasePalletAnalysis Analysis
        {
            get { return _analysis; }
        }
        #endregion

        #region Event handlers
        void clickEvent_Click(object sender, EventArgs e)
        {
            SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
            int iSel = context.Position.Row - 1;
            if (!_analysis.HasSolutionSelected(iSel))
                _analysis.SelectSolutionByIndex(iSel);
            else
                _analysis.UnselectSolutionByIndex(iSel);
            UpdateGridCheckBoxes();
        }
        private void onGridSolutionSelectionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
            int[] indexes = region.GetRowsIndex();
            // no selection -> exit
            if (indexes.Length == 0) return;
            // get selected solution
            _sol = _analysis.Solutions[indexes[0] - 1];
            // redraw
            graphCtrlCase.Invalidate();
            graphCtrlPallet.Invalidate();
        }
        private void pictureBox_SizeChanged(object sender, EventArgs e)
        {
        }
        #endregion

        #region Solution selection
        private void UpdateGridCheckBoxes()
        {
            int iRow = 0;
            foreach (CasePalletSolution sol in _analysis.Solutions)
            {
                ++iRow;
                SourceGrid.Cells.CheckBox checkBox = gridSolutions[iRow, 6] as SourceGrid.Cells.CheckBox;
                checkBox.Checked = _analysis.HasSolutionSelected(iRow - 1);
            }
        }
        private int GetCurrentSolutionIndex()
        {
            SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
            int[] indexes = region.GetRowsIndex();
            // no selection -> exit
            if (indexes.Length == 0) return -1;
            // return index
            return indexes[0] - 1;
        }
        private CasePalletSolution GetCurrentSolution()
        {
            int iIndexSol = GetCurrentSolutionIndex();
            if (-1 == iIndexSol) return null;
            else return _analysis.Solutions[iIndexSol];
        }
        #endregion

        #region IItemListener implementation
        /// <summary>
        /// overrides IItemListener.Update
        /// </summary>
        /// <param name="item"></param>
        public void Update(ItemBase item)
        {
            // update grid
            FillGrid();
            // select first solution
            if (gridSolutions.RowsCount > 0)
                gridSolutions.Selection.SelectRow(1, true);
            if (_analysis.Solutions.Count > 0)
                _sol = _analysis.Solutions[0];
            // draw
            graphCtrlCase.Invalidate();
            graphCtrlPallet.Invalidate();
        }
        /// <summary>
        /// overrides IItemListener.Kill
        /// handles analysis removal for any reason (deletion/document closing)
        /// </summary>
        /// <param name="item"></param>
        public void Kill(ItemBase item)
        {
            Close();
            _analysis.RemoveListener(this);
        }
        #endregion

        #region IView implementation
        public IDocument Document
        {
            get { return _document; }
        }
        #endregion

        #region IDrawingContainer implementation
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (ctrl == graphCtrlPallet)
            {
                // instantiate solution viewer
                CasePalletSolutionViewer sv = new CasePalletSolutionViewer(GetCurrentSolution());
                sv.Draw(graphics);
            }
            else if (ctrl == graphCtrlCase)
            {
                
                // get case of boxes
                CaseOfBoxesProperties caseOfBoxes = _analysis.BProperties as CaseOfBoxesProperties;

                // draw
                CaseDefinitionViewer cdv = new CaseDefinitionViewer(caseOfBoxes.CaseDefinition, caseOfBoxes.InsideBoxProperties, caseOfBoxes.CaseOptimConstraintSet);
                cdv.CaseProperties = caseOfBoxes;
                cdv.Orientation = GetCurrentSolution().FirstCaseOrientation;
                cdv.Draw(graphics);
            }
        }
        #endregion
    }
}
