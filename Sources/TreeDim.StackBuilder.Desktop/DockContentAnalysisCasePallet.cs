#region Using directives
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
// log4net
using log4net;
// treeDiM
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisCasePallet : DockContentAnalysisEdit
    {
        #region Data members
        /// <summary>
        /// logger
        /// </summary>
        static readonly new ILog _log = LogManager.GetLogger(typeof(DockContentAnalysisCasePallet));

        private bool _initialized = false;
        #endregion

        #region Constructor
        public DockContentAnalysisCasePallet(IDocument document, AnalysisCasePallet analysis)
            : base(document, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // --- initialize drawing container
            graphCtrlSolution.Viewer = new ViewerSolution(_solution);
            graphCtrlSolution.Invalidate();
            graphCtrlSolution.VolumeSelected += OnLayerSelected;
            // ---

            // --- initialize layer controls
            UpdateControls();

            uCtrlMaxPalletHeight.Value = _analysis.ConstraintSet.OptMaxHeight.Value;
            uCtrlOptMaximumWeight.Value = _analysis.ConstraintSet.OptMaxWeight;
            uCtrlOptMaxNumber.Value = _analysis.ConstraintSet.OptMaxNumber;

            uCtrlMaxPalletHeight.ValueChanged += new UCtrlDouble.ValueChangedDelegate(this.OnCriterionChanged);
            uCtrlOptMaximumWeight.ValueChanged += new UCtrlOptDouble.ValueChangedDelegate(this.OnCriterionChanged);
            uCtrlOptMaxNumber.ValueChanged += new UCtrlOptInt.ValueChangedDelegate(this.OnCriterionChanged);

            AnalysisCasePallet analysisCasePallet = _analysis as AnalysisCasePallet;

            ComboBoxHelpers.FillCombo(PalletCorners, cbPalletCorners, analysisCasePallet?.PalletCornerProperties);
            chkbPalletCorners.Enabled = (cbPalletCorners.Items.Count > 0);
            ComboBoxHelpers.FillCombo(PalletCaps, cbPalletCap, analysisCasePallet?.PalletCapProperties);
            chkbPalletCap.Enabled = (cbPalletCap.Items.Count > 0);
            ComboBoxHelpers.FillCombo(PalletFilms, cbPalletFilm, analysisCasePallet?.PalletFilmProperties);
            chkbPalletFilm.Enabled = (cbPalletFilm.Items.Count > 0);

            if (null != analysisCasePallet)
            {
                chkbPalletCorners.Checked = null != analysisCasePallet.PalletCornerProperties;
                chkbPalletCap.Checked = null != analysisCasePallet.PalletCapProperties;
                chkbPalletFilm.Checked = null != analysisCasePallet.PalletFilmProperties;
            }
            // ---
            // --- initialize grid control
            FillGrid();
            UpdateGrid();
            // ---

            _initialized = true;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Document.RemoveView(this);
        }

        private void OnPalletProtectionChanged(object sender, EventArgs e)
        {
            if (!_initialized)  return;

            cbPalletCorners.Enabled = chkbPalletCorners.Checked;
            cbPalletCap.Enabled = chkbPalletCap.Checked;
            cbPalletFilm.Enabled = chkbPalletFilm.Checked;

            if (_solution.Analysis is AnalysisCasePallet casePalletAnalysis)
            {
                casePalletAnalysis.PalletCornerProperties = SelectedPalletCorners;
                casePalletAnalysis.PalletCapProperties = SelectedPalletCap;
                casePalletAnalysis.PalletFilmProperties = SelectedPalletFilm;
            }

            graphCtrlSolution.Invalidate();
        }
        #endregion

        #region Override DockContentAnalysisEdit
        public override string GridCaption
        {   get { return Resources.ID_PALLET; } }
        #endregion

        #region Private properties (Pallet corners, pallet caps, pallet film)
        private PalletCornerProperties SelectedPalletCorners
        {
            get
            {
                if (cbPalletCorners.Items.Count > 0 && chkbPalletCorners.Checked)
                {
                    ItemBaseCB item = cbPalletCorners.SelectedItem as ItemBaseCB;
                    return  item.Item as PalletCornerProperties;
                }
                return null;
            }
        }
        private PalletCapProperties SelectedPalletCap
        {
            get
            {
                if (cbPalletCap.Items.Count > 0 && chkbPalletCap.Checked)
                {
                    ItemBaseCB item = cbPalletCap.SelectedItem as ItemBaseCB;
                    return item.Item as PalletCapProperties;
                }
                return null;
            }
        }
        private PalletFilmProperties SelectedPalletFilm
        {
            get
            {
                if (cbPalletFilm.Items.Count > 0 && chkbPalletFilm.Checked)
                {
                    ItemBaseCB item = cbPalletFilm.SelectedItem as ItemBaseCB;
                    return item.Item as PalletFilmProperties;
                }
                return null;
            }
        }
        #endregion

        #region IItemListener implementation
        public override void Update(ItemBase item)
        {
            base.Update(item);
            graphCtrlSolution.Invalidate();
        }
        public override void Kill(ItemBase item)
        {
            base.Kill(item);
            if (null != _analysis)
                _analysis.RemoveListener(this);
        }
        #endregion

        #region IDrawingContainer
        #endregion

        #region Public properties
        #endregion

        #region Private properties / Helpers
        private ItemBase[] PalletCorners
        {
            get
            {
                Document doc = _document as Document;
                return doc.GetByType(typeof(PalletCornerProperties)).ToArray(); 
            }
        }
        private ItemBase[] PalletCaps
        {
            get
            {
                Document doc = _document as Document;
                return doc.GetByType(typeof(PalletCapProperties)).ToArray(); 
            }
        }
        private ItemBase[] PalletFilms
        {
            get
            {
                Document doc = _document as Document;
                return doc.GetByType(typeof(PalletFilmProperties)).ToArray(); 
            }
        }
        #endregion

        #region Grid filling
        public override void FillGrid()
        { 
            // clear grid
            gridSolutions.Rows.Clear();
            // border
            gridSolutions.BorderStyle = BorderStyle.FixedSingle;
            gridSolutions.ColumnsCount = 2;
            gridSolutions.Columns[0].Width = 100;
            gridSolutions.FixedColumns = 1;
        }
        public override void UpdateGrid()
        {
            try
            {
                // sanity check
                if (gridSolutions.ColumnsCount < 2)
                    return;
                // remove all existing rows
                gridSolutions.Rows.Clear();
                // *** IViews 
                // captionHeader
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
                // pallet caption
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_PALLET)
                {
                    ColumnSpan = 2,
                    View = captionHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                // layer #
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_LAYERNUMBER)
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(_solution.LayerCount);
                // interlayer #
                if (_solution.InterlayerCount > 0)
                {
                    gridSolutions.Rows.Insert(++iRow);
                    rowHeader = new SourceGrid.Cells.RowHeader(Resources.ID_INTERLAYERNUMBER)
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(_solution.InterlayerCount);
                }
                // *** Item # (Recursive count)
                Packable content = _analysis.Content;
                int itemCount = _solution.ItemCount ;
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
                // outer dimensions
                BBox3D bboxGlobal = _solution.BBoxGlobal;
                // ---
                gridSolutions.Rows.Insert(++iRow);
                rowHeader = new SourceGrid.Cells.RowHeader(
                    string.Format(Resources.ID_OUTERDIMENSIONS, UnitsManager.LengthUnitString))
                {
                    View = viewRowHeader
                };
                gridSolutions[iRow, 0] = rowHeader;
                gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                    string.Format(CultureInfo.InvariantCulture, "{0:0.#} x {1:0.#} x {2:0.#}", bboxGlobal.Length, bboxGlobal.Width, bboxGlobal.Height));
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
                    rowHeader = new SourceGrid.Cells.RowHeader("Spaces")
                    {
                        View = viewRowHeader
                    };
                    gridSolutions[iRow, 0] = rowHeader;
                    gridSolutions[iRow, 1] = new SourceGrid.Cells.Cell(
                        string.Format(CultureInfo.InvariantCulture, "{0:0.#}", _solution.LayerMaximumSpace(i)));
                }
                // ### layers : end
                gridSolutions.AutoSizeCells();
                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnLayerSelected(int id)
        {
            try
            {
                _solution.SelectLayer(id);
                UpdateControls();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Event handlers
        private void OnSizeChanged(object sender, EventArgs e)
        {
            int splitDistance = splitContainerHoriz.Size.Height - 120;
            if (splitDistance > 0)
                splitContainerHoriz.SplitterDistance = splitDistance;
        }
        private void OnCriterionChanged(object sender, EventArgs args)
        {
            try
            {
                ConstraintSetCasePallet constraintSet = _solution.Analysis.ConstraintSet as ConstraintSetCasePallet;
                constraintSet.SetMaxHeight(new OptDouble(true, uCtrlMaxPalletHeight.Value));
                constraintSet.OptMaxWeight = uCtrlOptMaximumWeight.Value;
                constraintSet.OptMaxNumber = uCtrlOptMaxNumber.Value;
                _solution.RebuildSolutionItemList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            // update drawing & grid
            graphCtrlSolution.Invalidate();
            UpdateGrid();
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
        #endregion
 
        #region Layer controls
        /*
        private void FillLayerControls()
        {
            try
            {
                cbLayerType.Packable = _analysis.Content;
                // build layers and fill CCtrl
                foreach (LayerDesc layerDesc in _solution.LayerDescriptors)
                {
                    LayerSolver solver = new LayerSolver();
                    Layer2D layer = solver.BuildLayer(_analysis.ContentDimensions, _analysis.ContainerDimensions, layerDesc as LayerDescBox);
                    cbLayerType.Items.Add(layer);
                }
                if (cbLayerType.Items.Count > 0)
                    cbLayerType.SelectedIndex = 0;

                // interlayer combo box
                Document doc = _document as Document;
                if (null == doc) return;
                ItemBase[] interlayers = doc.ListByType(typeof(InterlayerProperties)).ToArray();
                ComboBoxHelpers.FillCombo(interlayers, cbInterlayer, null);
            }
            catch (Exception ex)
            {
                _log.Error( ex.Message );
            }
        }
        */ 
        /*
        private void UpdateControls()
        {
            try
            {
                int index = _solution.SelectedLayerIndex;

                bnSymmetryX.Enabled = (index != -1);
                bnSymetryY.Enabled = (index != -1);
                cbLayerType.Enabled = (index != -1);
                chkbInterlayer.Enabled = (index != -1) && (cbInterlayer.Items.Count > 0);

                gbLayer.Text = index != -1
                    ? string.Format(Resources.ID_SELECTEDLAYER, index)
                    : Resources.ID_DOUBLECLICKALAYER;

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
                        onChkbInterlayerClicked(null, null);
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
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
         * */
        #endregion
    }
}
