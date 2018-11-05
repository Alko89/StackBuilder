#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Engine;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysisCasePalletDM : FormNewAnalysis, IDrawingContainer, IItemBaseFilter
    {
        #region Constructor
        public FormNewAnalysisCasePalletDM(Document doc, Analysis analysis)
            : base(doc, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbCases.Initialize(_document, this, initiallySelectedItem: AnalysisCast?.Content);
            cbPallets.Initialize(_document, this, initiallySelectedItem: AnalysisCast?.PalletProperties);

            if (null == AnalysisCast)
            {
                ItemName = _document.GetValidNewAnalysisName(ItemDefaultName);
                ItemDescription = ItemName;

                uCtrlCaseOrientation.AllowedOrientations = new bool[] { Settings.Default.AllowVerticalX, Settings.Default.AllowVerticalY, Settings.Default.AllowVerticalZ };
                uCtrlMaximumHeight.Value = Settings.Default.MaximumPalletHeight;
                uCtrlOptMaximumWeight.Value = new OptDouble(false, Settings.Default.MaximumPalletWeight);

                uCtrlOverhang.ValueX = Settings.Default.OverhangX;
                uCtrlOverhang.ValueY = Settings.Default.OverhangY;
            }
            else
            {
                ItemName = AnalysisBase.Name;
                ItemDescription = AnalysisBase.Description;

                ConstraintSetCasePallet constraintSet = AnalysisBase.ConstraintSet as ConstraintSetCasePallet;
                uCtrlCaseOrientation.AllowedOrientations = constraintSet.AllowedOrientations;
                uCtrlMaximumHeight.Value = constraintSet.OptMaxHeight.Value;
                uCtrlOptMaximumWeight.Value = constraintSet.OptMaxWeight;
                uCtrlOverhang.ValueX = constraintSet.Overhang.X;
                uCtrlOverhang.ValueY = constraintSet.Overhang.Y;
            }
            checkBoxBestLayersOnly.Checked = Settings.Default.KeepBestSolutions;

            // gridSolutions
            gridSolutions.Selection.SelectionChanged += GridSolutionsSelectionChanged;
            // graphCtrl
            graphCtrl.DrawingContainer = this;
        }

        private void GridSolutionsSelectionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            graphCtrl.Invalidate();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Settings.Default.AllowVerticalX = uCtrlCaseOrientation.AllowedOrientations[0];
            Settings.Default.AllowVerticalY = uCtrlCaseOrientation.AllowedOrientations[1];
            Settings.Default.AllowVerticalZ = uCtrlCaseOrientation.AllowedOrientations[2];
            Settings.Default.MaximumPalletHeight = uCtrlMaximumHeight.Value;
            if (uCtrlOptMaximumWeight.Value.Activated)
                Settings.Default.MaximumPalletWeight = uCtrlOptMaximumWeight.Value.Value;

            Settings.Default.OverhangX = uCtrlOverhang.ValueX;
            Settings.Default.OverhangY = uCtrlOverhang.ValueY;

            Settings.Default.KeepBestSolutions = checkBoxBestLayersOnly.Checked;
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName => Resources.ID_ANALYSIS;
        #endregion

        #region FormNewAnalysis override
        public override void OnNext()
        {
            try
            {
                Layer2D selLayer = SelectedLayer;
                List<LayerDesc> layerDescs = new List<LayerDesc>() { selLayer.LayerDescriptor };
                Solution.SetSolver(new LayerSolver());
                AnalysisCasePallet analysis = AnalysisCast;
                if (null == analysis)
                    _item = _document.CreateNewAnalysisCasePallet(
                        ItemName, ItemDescription
                        , SelectedPackable, SelectedPallet
                        , new List<InterlayerProperties>()
                        , null, null, null
                        , BuildConstraintSet()
                        , layerDescs
                        );
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.Content = SelectedPackable;
                    analysis.PalletProperties = SelectedPallet;
                    analysis.ConstraintSet = BuildConstraintSet();
                    analysis.AddSolution(layerDescs);

                    _document.UpdateAnalysis(analysis);
                }
                Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public override void UpdateStatus(string message)
        {
            base.UpdateStatus(message);
        }
        #endregion

        #region IItemBaseFilter implementation
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            if (ctrl == cbCases)
            {
                Packable packable = itemBase as Packable;
                return null != packable
                    && (
                    (packable is BProperties) ||
                    (packable is PackProperties) ||
                    (packable is LoadedCase)
                    );
            }
            else if (ctrl == cbPallets)
                return itemBase is PalletProperties;
            else
                return false;
        }
        #endregion

        #region IDrawingContainer implementation
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            Layer2D selLayer = SelectedLayer;
            PalletProperties pallet = SelectedPallet;
            Packable packable = SelectedPackable;
            if (null == selLayer || null == packable || null == pallet)
                return;
            var analysis = new AnalysisCasePallet(packable, pallet, BuildConstraintSet(), true /*temporary*/);
            analysis.AddSolution(new List<LayerDesc> { selLayer.LayerDescriptor });

            ViewerSolution sv = new ViewerSolution(analysis.Solution);
            sv.Draw(graphics, Transform3D.Identity);
        }
        #endregion

        #region Timer
        private void OnInputChanged(object sender, EventArgs e)
        {
            // stop timer
            _timer.Stop();
            // clear grid
            ClearGrid();
            // restart timer
            _timer.Start();
        }
        private Image TryGenerateLayerImage(ILayer2D layer)
        {
            double height = 0.0;
            return LayerToImage.DrawEx(
                layer, SelectedPackable
                , height
                , new Size(60, 60)
                , false
                , LayerToImage.EGraphMode.GRAPH_2D
                , false);
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                // get case /pallet
                Packable packable = cbCases.SelectedType as Packable;
                PalletProperties palletProperties = cbPallets.SelectedType as PalletProperties;
                if (null == packable || null == palletProperties)
                    return;
                // compute
                LayerSolver solver = new LayerSolver();
                _layers = solver.BuildLayers(
                    packable.OuterDimensions
                    , new Vector2D(
                        palletProperties.Length + 2.0 * uCtrlOverhang.ValueX
                        , palletProperties.Width + 2.0 * uCtrlOverhang.ValueY)
                    , palletProperties.Height
                    , BuildConstraintSet()
                    , checkBoxBestLayersOnly.Checked);
                FillGrid();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Fill grid
        private void ClearGrid()
        {
            gridSolutions.Rows.Clear();
        }
        private void FillGrid()
        {
            try
            {
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
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
                DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader()
                {
                    BackColor = Color.LightGray,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                };
                viewColumnHeader.Background = backHeader;
                viewColumnHeader.ForeColor = Color.Black;
                viewColumnHeader.Font = new Font("Arial", GridFontSize, FontStyle.Regular);
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridSolutions.BorderStyle = BorderStyle.FixedSingle;
                gridSolutions.ColumnsCount = 7;
                gridSolutions.FixedRows = 1;
                gridSolutions.Rows.Insert(0);
                // header
                int iCol = 0, iRow = -1;
                gridSolutions.Rows.Insert(++iRow);
                // layer pattern
                gridSolutions[0, iCol++] = new SourceGrid.Cells.ColumnHeader(Resources.ID_LAYERPATTERN)
                { AutomaticSortEnabled = false, View = viewColumnHeader };
                // case count
                gridSolutions[0, iCol++] = new SourceGrid.Cells.ColumnHeader(Resources.ID_CASECOUNT)
                { AutomaticSortEnabled = false, View = viewColumnHeader };
                // efficiency
                gridSolutions[0, iCol++] = new SourceGrid.Cells.ColumnHeader(Resources.ID_EFFICIENCYPERCENTAGE)
                { AutomaticSortEnabled = false, View = viewColumnHeader };
                // pallet weight
                gridSolutions[0, iCol++] = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_PALLETWEIGHT, UnitsManager.MassUnitString))
                { AutomaticSortEnabled = false, View = viewColumnHeader };
                // pallet height
                gridSolutions[0, iCol++] = new SourceGrid.Cells.ColumnHeader(string.Format(Resources.ID_PALLETHEIGHT, UnitsManager.LengthUnitString))
                { AutomaticSortEnabled = false, View = viewColumnHeader };

                gridSolutions.AutoStretchRowsToFitHeight = true;
                SourceGrid.Cells.Controllers.ToolTipText toolTipController = new SourceGrid.Cells.Controllers.ToolTipText
                {
                    ToolTipTitle = "",
                    ToolTipIcon = ToolTipIcon.None,
                    IsBalloon = false
                };

                foreach (Layer2D layer in _layers)
                {
                    gridSolutions.Rows.Insert(++iRow);
                    iCol = 0;
                    // layer pattern
                    gridSolutions[iRow, iCol] = new SourceGrid.Cells.Image(TryGenerateLayerImage(layer))
                    { ToolTipText = layer.Name};
                    gridSolutions[iRow, iCol++].AddController(toolTipController);

                    using (FastEvaluatorLayer2Pallet evaluator = new FastEvaluatorLayer2Pallet(layer, SelectedPackable, SelectedPallet, BuildConstraintSet()))
                    {
                        // case count
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(
                            string.Format(Resources.ID_CASECOUNTFORMATSTRING,
                            evaluator.ItemCount,
                            evaluator.NoItemsPerLayer,
                            evaluator.NoLayers));
                        // volume efficiency
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell( string.Format(CultureInfo.InvariantCulture, "{0:0.##}", evaluator.VolumeEfficiency) );
                        // pallet weight
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.#}",  evaluator.PalletWeight) );
                        // pallet height
                        gridSolutions[iRow, iCol++] = new SourceGrid.Cells.Cell(string.Format(CultureInfo.InvariantCulture, "{0:0.##}", evaluator.PalletHeight) );
                    }
                }
                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.AutoSizeCells();
                gridSolutions.Columns.StretchToFit();

                // select first solution
                if (gridSolutions.RowsCount > 1)
                    gridSolutions.Selection.SelectRow(1, true);
                else
                    graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private int GridFontSize => Settings.Default.GridFontSize;
        private Layer2D SelectedLayer
        {
            get
            {
                SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
                int[] indexes = region.GetRowsIndex();
                // no selection -> exit
                if (indexes.Length == 0)
                    return null;
                return _layers[indexes[0]-1];
            }
        }
        #endregion

        #region Event handlers
        protected void OnCaseChanged(object sender, EventArgs e)
        {
            try
            {
                uCtrlCaseOrientation.BProperties = cbCases.SelectedType as PackableBrick;
                OnInputChanged(sender, e);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Helpers
        public AnalysisCasePallet AnalysisCast
        {
            get { return _item as AnalysisCasePallet; }
            set { _item = value; }
        }
        private Packable SelectedPackable => cbCases.SelectedType as Packable;
        private PalletProperties SelectedPallet => cbPallets.SelectedType as PalletProperties;

        private ConstraintSetCasePallet BuildConstraintSet()
        {
            // constraint set
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
            {
                Overhang = new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY)
            };
            // orientations
            constraintSet.SetAllowedOrientations(uCtrlCaseOrientation.AllowedOrientations);
            // conditions
            constraintSet.SetMaxHeight(new OptDouble(true, uCtrlMaximumHeight.Value));
            constraintSet.OptMaxWeight = uCtrlOptMaximumWeight.Value;
            return constraintSet;
        }
        #endregion

        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisCasePalletDM));
        private List<Layer2D> _layers;
        #endregion
    }
}
