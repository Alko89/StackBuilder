#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

// log4net
using log4net;
// Sharp3D
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisEdit : DockContentView, IDrawingContainer, IItemBaseFilter
    {
        #region Constructor
        public DockContentAnalysisEdit()
            : base(null)
        {
            _analysis = null;

            InitializeComponent();
        }
        public DockContentAnalysisEdit(IDocument document, Analysis analysis)
            : base(document)
        {
            _analysis = analysis;
            _analysis.AddListener(this);
            _solution = analysis.Solution;

            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;

            GridFontSize = Settings.Default.GridFontSize;

            // --- window caption
            if (null != Analysis)
                this.Text = Analysis.Name + " - " + Analysis.ParentDocument.Name;

            // initialize drawing container
            graphCtrlSolution.DrawingContainer = this;
            graphCtrlSolution.Viewer = new ViewerSolution(_solution);
            graphCtrlSolution.Invalidate();

            graphCtrlSolution.VolumeSelected += OnLayerSelected;

            FillGrid();
            UpdateGrid();

            FillLayerControls();
            UpdateControls();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Document.RemoveView(this);
        }
        #endregion

        #region IItemListener implementation
        public override void Update(ItemBase item)
        {
            graphCtrlSolution.Invalidate();
        }
        public override void Kill(ItemBase item)
        {
            Close();
            _analysis.RemoveListener(this);
        }
        #endregion

        #region IItemBaseFilter implementation
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            InterlayerProperties interlayer = itemBase as InterlayerProperties;
            if (ctrl == cbInterlayer && null != interlayer)
                return _analysis.AllowInterlayer(interlayer);
            return false;
        }
        #endregion

        #region Public properties
        public Analysis Analysis
        {
            get { return _analysis; }
        }
        public Solution Solution
        {
            get { return _solution; }
            set { _solution = value; }
        }
        public virtual string GridCaption
        {
            get { return Resources.ID_LOAD; }
        }
        protected int GridFontSize { get; set; }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            ViewerSolution sv = new ViewerSolution(Solution);
            sv.Draw(graphics, Transform3D.Identity);
        }
        #endregion

        #region Virtual functions
        public virtual void FillGrid()
        {
            // clear grid
            gridSolutions.Rows.Clear();
            // border
            gridSolutions.BorderStyle = BorderStyle.FixedSingle;
            gridSolutions.ColumnsCount = 2;
            gridSolutions.FixedColumns = 1;
        }
        public virtual void UpdateGrid()
        {
            try
            {
                // remove all existing rows
                gridSolutions.Rows.Clear();
                // *** IViews
                // caption header
                SourceGrid.Cells.Views.RowHeader captionHeader = new SourceGrid.Cells.Views.RowHeader();
                DevAge.Drawing.VisualElements.RowHeader veHeaderCaption = new DevAge.Drawing.VisualElements.RowHeader()
                {
                    BackColor = Color.SteelBlue,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                captionHeader.Background = veHeaderCaption;
                captionHeader.ForeColor = Color.Black;
                captionHeader.Font = new Font("Arial", GridFontSize, FontStyle.Bold);
                captionHeader.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                // viewRowHeader
                SourceGrid.Cells.Views.RowHeader viewRowHeader = new SourceGrid.Cells.Views.RowHeader();
                DevAge.Drawing.VisualElements.RowHeader backHeader = new DevAge.Drawing.VisualElements.RowHeader()
                {
                    BackColor = Color.LightGray,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                viewRowHeader.Background = backHeader;
                viewRowHeader.ForeColor = Color.Black;
                viewRowHeader.Font = new Font("Arial", GridFontSize, FontStyle.Regular);
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***

                SourceGrid.Cells.RowHeader rowHeader;
                int iRow = -1;

                // loading caption
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(GridCaption)
                {
                    ColumnSpan = 2,
                    View = captionHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                // layer #
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_LAYERCOUNT)
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(_solution.LayerCount);
                // interlayer #
                if (_solution.InterlayerCount > 0)
                {
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_INTERLAYERCOUNT)
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(_solution.InterlayerCount);
                }
                // *** Item # (Recursive count)
                Packable content = _analysis.Content;
                int itemCount = _solution.ItemCount;
                int number = 1;
                do
                {
                    itemCount *= number;
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(string.Format("{0} #", content.DetailedName))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(itemCount);
                }
                while (null != content && content.InnerContent(ref content, ref number));
                // ***

                // load dimensions
                BBox3D bboxLoad = _solution.BBoxLoad;
                // ---
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADDIMENSIONS, UnitsManager.LengthUnitString))
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxLoad.Length, bboxLoad.Width, bboxLoad.Height));
                // net weight
                if (_solution.HasNetWeight)
                {
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(
                        string.Format(Resources.ID_NETWEIGHT_WU, UnitsManager.MassUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.NetWeight));
                }
                // load weight
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_LOADWEIGHT_WU, UnitsManager.MassUnitString))
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LoadWeight));
                // total weight
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_TOTALWEIGHT_WU, UnitsManager.MassUnitString))
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.Weight));
                // volume efficiency
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_VOLUMEEFFICIENCY)
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.VolumeEfficiency));


                int noLayerTypesUsed = 0;
                for (int i = 0; i < _solution.Layers.Count; ++i)
                    noLayerTypesUsed += _solution.Layers[i].BoxCount > 0 ? 1 : 0;

                // ### layers : begin
                for (int i = 0; i < _solution.NoLayerTypesUsed; ++i)
                {
                    // layer caption
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(_solution.LayerCaption(i))
                    {
                        ColumnSpan = 2,
                        View = captionHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;

                    // *** Item # (recursive count)
                    content = _analysis.Content;
                    itemCount = _solution.LayerBoxCount(i);
                    number = 1;
                    do
                    {
                        itemCount *= number;

                        gridSolutions.Rows.Insert(++iRow);
                        rowHeader = new SourceGrid.Cells.RowHeader(
                            string.Format("{0} #", content.DetailedName))
                        {
                            View = viewRowHeader
                        };
                        gridSolutions[iRow, 0] = rowHeader;
                        gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(itemCount);
                    }
                    while (null != content && content.InnerContent(ref content, ref number));
                    // ***

                    // layer weight
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(string.Format(Resources.ID_WEIGHT_WU, UnitsManager.MassUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LayerWeight(i)));
                    // layer space
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(string.Format(Resources.ID_SPACES_WU, UnitsManager.LengthUnitString))
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LayerMaximumSpace(i)));
                }
                // ### layers : end

                gridSolutions.AutoSizeCells();
                gridSolutions.Columns.StretchToFit();
                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Event handlers
        private void OnLayerSelected(int id)
        {
            try
            {
                _solution.SelectLayer(id);
                UpdateControls();
            }
            catch (Exception ex)
            {   _log.Error(ex.ToString()); }
        }
        private void OnLayerTypeChanged(object sender, EventArgs e)
        {
            try
            {
                int iLayerType = cbLayerType.SelectedIndex;
                // get selected layer
                _solution.SetLayerTypeOnSelected(iLayerType);
                // redraw
                graphCtrlSolution.Invalidate();
                UpdateGrid();
            }
            catch (Exception ex)
            {   _log.Error(ex.ToString()); }
        }
        private void OnInterlayerChanged(object sender, EventArgs e)
        {
            try
            {
                // get index of interlayer
                InterlayerProperties interlayer = null;
                if (chkbInterlayer.Checked)
                    interlayer = cbInterlayer.SelectedType as InterlayerProperties;
                _solution.SetInterlayerOnSelected(interlayer);
                graphCtrlSolution.Invalidate();
                UpdateGrid();
            }
            catch (Exception ex)
            {   _log.Error(ex.ToString()); }
        }
        private void OnReflectionX(object sender, EventArgs e)
        {
            _solution.ApplySymetryOnSelected(0);
            graphCtrlSolution.Invalidate();
        }
        private void OnReflectionY(object sender, EventArgs e)
        {
            _solution.ApplySymetryOnSelected(1);
            graphCtrlSolution.Invalidate();
        }
        private void OnChkbInterlayerClicked(object sender, EventArgs e)
        {
            try
            {
                cbInterlayer.Enabled = chkbInterlayer.Checked;
                OnInterlayerChanged(null, null);
                UpdateGrid();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Toolbar event handlers
        private void OnBack(object sender, EventArgs e)
        {
            // close this form
            Close();
            // call edit analysis
            Document.EditAnalysis(_analysis);
        }
        private void OnGenerateReport(object sender, EventArgs e)
        {
            FormMain.GenerateReport(_analysis);
        }
        private void OnGenerateExport(object sender, EventArgs e)
        {
            ToolStripButton tsb = sender as ToolStripButton;
            string extension = string.Empty;
            switch (tsb.Name)
            {
                case "toolStripButtonExportXML": extension = "xml"; break;
                case "toolStripButtonExportCSV": extension = "csv"; break;
                case "toolStripButtonExportDAE": extension = "dae"; break;
                default: break;
            }
            FormMain.GenerateExport(_analysis, extension);
        }
        #endregion

        #region Layer controls
        private void FillLayerControls()
        {
            try
            {
                cbLayerType.Packable = _analysis.Content;
                // build layers and fill CCtrl
                foreach (LayerDesc layerDesc in _solution.LayerDescriptors)
                {
                    LayerSolver solver = new LayerSolver();
                    ILayer2D layer = solver.BuildLayer(_analysis.Content, _analysis.ContainerDimensions, layerDesc, _analysis.ConstraintSet.MinimumSpace.Value);
                    cbLayerType.Items.Add(layer);
                }
                if (cbLayerType.Items.Count > 0)
                    cbLayerType.SelectedIndex = 0;

                // fill combo cbInterlayer
                Document document = _document as Document;
                cbInterlayer.Initialize(document, this, null);
            }
            catch (Exception /*ex*/)
            {
            }
        }
        protected void UpdateControls()
        {
            try
            {
                int index = _solution.SelectedLayerIndex;
                bnSymmetryX.Enabled = (index != -1);
                bnSymetryY.Enabled = (index != -1);
                cbLayerType.Enabled = (index != -1);
                chkbInterlayer.Enabled = (index != -1) && (cbInterlayer.Items.Count > 0);

                gbLayer.Text = index != -1 ? string.Format("Selected layer : {0}", index) : "Double-click a layer";

                if (index != -1)
                {
                    tbClickLayer.Hide();
                    gbLayer.Show();

                    // get selected solution item
                    SolutionItem selItem = _solution.SelectedSolutionItem;
                    if (null != selItem)
                    {
                        // set current layer
                        cbLayerType.SelectedIndex = selItem.LayerIndex;
                        // set interlayer
                        chkbInterlayer.Checked = selItem.HasInterlayer;
                        OnChkbInterlayerClicked(null, null);
                    }
                    // select combo box
                    int selIndex = 0;
                    foreach (object o in cbInterlayer.Items)
                    {
                        InterlayerProperties interlayerProp = o as InterlayerProperties;
                        if (interlayerProp == _solution.SelectedInterlayer)
                            break;
                        ++selIndex;
                    }
                    if (selIndex < cbInterlayer.Items.Count)
                        cbInterlayer.SelectedIndex = selIndex;
                }
                else
                {
                    gbLayer.Hide();
                    tbClickLayer.Show();
                }
            }
            catch (Exception /*ex*/)
            {
            }
        }
        #endregion

        #region Data members
        /// <summary>
        /// analysis
        /// </summary>
        protected Analysis _analysis;
        /// <summary>
        /// solution
        /// </summary>
        protected Solution _solution;
        /// <summary>
        /// logger
        /// </summary>
        protected static readonly ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisEdit));
        #endregion
    }
}
