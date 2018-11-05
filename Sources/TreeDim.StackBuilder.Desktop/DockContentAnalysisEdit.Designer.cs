namespace treeDiM.StackBuilder.Desktop
{
    partial class DockContentAnalysisEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockContentAnalysisEdit));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.graphCtrlSolution = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.gridSolutions = new SourceGrid.Grid();
            this.gbLayer = new System.Windows.Forms.GroupBox();
            this.chkbInterlayer = new System.Windows.Forms.CheckBox();
            this.cbInterlayer = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            this.cbLayerType = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboLayer();
            this.bnSymmetryX = new System.Windows.Forms.Button();
            this.bnSymetryY = new System.Windows.Forms.Button();
            this.tbClickLayer = new System.Windows.Forms.TextBox();
            this.toolStripAnalysis = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonBack = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExportXML = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportCSV = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).BeginInit();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlSolution)).BeginInit();
            this.gbLayer.SuspendLayout();
            this.toolStripAnalysis.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.splitContainerVert);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.gbLayer);
            this.splitContainerHoriz.Panel2.Controls.Add(this.tbClickLayer);
            // 
            // splitContainerVert
            // 
            resources.ApplyResources(this.splitContainerVert, "splitContainerVert");
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.graphCtrlSolution);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.gridSolutions);
            // 
            // graphCtrlSolution
            // 
            resources.ApplyResources(this.graphCtrlSolution, "graphCtrlSolution");
            this.graphCtrlSolution.Name = "graphCtrlSolution";
            this.graphCtrlSolution.Viewer = null;
            // 
            // gridSolutions
            // 
            resources.ApplyResources(this.gridSolutions, "gridSolutions");
            this.gridSolutions.EnableSort = false;
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // gbLayer
            // 
            resources.ApplyResources(this.gbLayer, "gbLayer");
            this.gbLayer.Controls.Add(this.chkbInterlayer);
            this.gbLayer.Controls.Add(this.cbInterlayer);
            this.gbLayer.Controls.Add(this.cbLayerType);
            this.gbLayer.Controls.Add(this.bnSymmetryX);
            this.gbLayer.Controls.Add(this.bnSymetryY);
            this.gbLayer.Name = "gbLayer";
            this.gbLayer.TabStop = false;
            // 
            // chkbInterlayer
            // 
            resources.ApplyResources(this.chkbInterlayer, "chkbInterlayer");
            this.chkbInterlayer.Name = "chkbInterlayer";
            this.chkbInterlayer.UseVisualStyleBackColor = true;
            this.chkbInterlayer.CheckedChanged += new System.EventHandler(this.OnChkbInterlayerClicked);
            // 
            // cbInterlayer
            // 
            resources.ApplyResources(this.cbInterlayer, "cbInterlayer");
            this.cbInterlayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInterlayer.FormattingEnabled = true;
            this.cbInterlayer.Name = "cbInterlayer";
            this.cbInterlayer.SelectedIndexChanged += new System.EventHandler(this.OnInterlayerChanged);
            // 
            // cbLayerType
            // 
            resources.ApplyResources(this.cbLayerType, "cbLayerType");
            this.cbLayerType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLayerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayerType.FormattingEnabled = true;
            this.cbLayerType.Name = "cbLayerType";
            this.cbLayerType.SelectedIndexChanged += new System.EventHandler(this.OnLayerTypeChanged);
            // 
            // bnSymmetryX
            // 
            resources.ApplyResources(this.bnSymmetryX, "bnSymmetryX");
            this.bnSymmetryX.Name = "bnSymmetryX";
            this.bnSymmetryX.UseVisualStyleBackColor = true;
            this.bnSymmetryX.Click += new System.EventHandler(this.OnReflectionX);
            // 
            // bnSymetryY
            // 
            resources.ApplyResources(this.bnSymetryY, "bnSymetryY");
            this.bnSymetryY.Name = "bnSymetryY";
            this.bnSymetryY.UseVisualStyleBackColor = true;
            this.bnSymetryY.Click += new System.EventHandler(this.OnReflectionY);
            // 
            // tbClickLayer
            // 
            resources.ApplyResources(this.tbClickLayer, "tbClickLayer");
            this.tbClickLayer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbClickLayer.Name = "tbClickLayer";
            // 
            // toolStripAnalysis
            // 
            this.toolStripAnalysis.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBack,
            this.toolStripSeparator2,
            this.toolStripButtonReport,
            this.toolStripSeparator1,
            this.toolStripButtonExportXML,
            this.toolStripButtonExportCSV});
            resources.ApplyResources(this.toolStripAnalysis, "toolStripAnalysis");
            this.toolStripAnalysis.Name = "toolStripAnalysis";
            // 
            // toolStripButtonBack
            // 
            this.toolStripButtonBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonBack, "toolStripButtonBack");
            this.toolStripButtonBack.Name = "toolStripButtonBack";
            this.toolStripButtonBack.Click += new System.EventHandler(this.OnBack);
            // 
            // toolStripButtonReport
            // 
            this.toolStripButtonReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonReport, "toolStripButtonReport");
            this.toolStripButtonReport.Name = "toolStripButtonReport";
            this.toolStripButtonReport.Click += new System.EventHandler(this.OnGenerateReport);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonExportXML
            // 
            this.toolStripButtonExportXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonExportXML, "toolStripButtonExportXML");
            this.toolStripButtonExportXML.Name = "toolStripButtonExportXML";
            this.toolStripButtonExportXML.Click += new System.EventHandler(this.OnGenerateExport);
            // 
            // toolStripButtonExportCSV
            // 
            this.toolStripButtonExportCSV.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonExportCSV, "toolStripButtonExportCSV");
            this.toolStripButtonExportCSV.Name = "toolStripButtonExportCSV";
            this.toolStripButtonExportCSV.Click += new System.EventHandler(this.OnGenerateExport);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // DockContentAnalysisEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerHoriz);
            this.Controls.Add(this.toolStripAnalysis);
            this.Name = "DockContentAnalysisEdit";
            this.ShowInTaskbar = false;
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            this.splitContainerHoriz.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz)).EndInit();
            this.splitContainerHoriz.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrlSolution)).EndInit();
            this.gbLayer.ResumeLayout(false);
            this.gbLayer.PerformLayout();
            this.toolStripAnalysis.ResumeLayout(false);
            this.toolStripAnalysis.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Graphics.Graphics3DControl graphCtrlSolution;
        protected System.Windows.Forms.SplitContainer splitContainerHoriz;
        protected System.Windows.Forms.SplitContainer splitContainerVert;
        private System.Windows.Forms.ToolStrip toolStripAnalysis;
        private System.Windows.Forms.ToolStripButton toolStripButtonReport;
        private System.Windows.Forms.GroupBox gbLayer;
        private System.Windows.Forms.CheckBox chkbInterlayer;
        protected Graphics.Controls.CCtrlComboFiltered cbInterlayer;
        private Graphics.Controls.CCtrlComboLayer cbLayerType;
        private System.Windows.Forms.Button bnSymmetryX;
        private System.Windows.Forms.Button bnSymetryY;
        private System.Windows.Forms.TextBox tbClickLayer;
        protected SourceGrid.Grid gridSolutions;
        private System.Windows.Forms.ToolStripButton toolStripButtonBack;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportXML;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportCSV;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}