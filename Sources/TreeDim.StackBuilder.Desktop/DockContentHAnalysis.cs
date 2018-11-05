#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentHAnalysis : DockContentView, IDrawingContainer
    {
        #region Constructors
        public DockContentHAnalysis()
            : base(null)
        {
            InitializeComponent();
        }
        public DockContentHAnalysis(IDocument document, HAnalysis analysis)
            : base(document)
        {
            Analysis = analysis;
            Analysis.AddListener(this);
            Solution = Analysis.Solution;

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
                Text = Analysis.Name + " - " + Analysis.ParentDocument.Name;

            // initialize drawing container
            graphCtrlSolution.DrawingContainer = this;
            graphCtrlSolution.Invalidate();

            FillGrid();
            UpdateGrid();
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
        }
        public override void Kill(ItemBase item)
        {
            Close();
            if (null != Analysis)
                Analysis.RemoveListener(this);
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            ViewerHSolution sv = new ViewerHSolution(Solution, SolItemIndex);
            sv.Draw(graphics, Transform3D.Identity);
        }
        #endregion

        #region Public properties
        public HAnalysis Analysis { get; set; } = null;
        public HSolution Solution { get; set; }
        #endregion
        #region Protected properties
        protected int GridFontSize { get; set; }
        protected virtual string GridCaption => Resources.ID_LOAD;
        #endregion
        #region Private properties
        private int SolItemIndex { get; set; } = 0;
        #endregion

        #region Grid methods
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
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Data members
        protected HSolution _solution;
        private static ILog _log = LogManager.GetLogger(typeof(DockContentHAnalysis));
        #endregion

        #region Event handlers
        private void OnSolItemIndexUp(object sender, EventArgs e)
        {
            try
            {
                if (null == Solution) return;
                if (SolItemIndex < Solution.SolItemCount - 1)
                    SolItemIndex = SolItemIndex + 1;
                graphCtrlSolution.Invalidate();
                UpdateSolItemIndexButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnSolItemIndexDown(object sender, EventArgs e)
        {
            try
            {
                if (null == Solution) return;
                if (SolItemIndex > 0)
                    SolItemIndex = SolItemIndex - 1;
                graphCtrlSolution.Invalidate();
                UpdateSolItemIndexButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnBack(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnGenerateReport(object sender, EventArgs e)
        {
            FormMain.GenerateReport(Analysis);
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
                FormMain.GetInstance().GenerateExport(Analysis, extension);
        }
        #endregion

        private void UpdateSolItemIndexButtons()
        {
            bnSolItemIndexDown.Enabled = SolItemIndex > 0;
            bnSolItemIndexUp.Enabled = SolItemIndex < Solution.SolItemCount - 1;
        }
    }
}
