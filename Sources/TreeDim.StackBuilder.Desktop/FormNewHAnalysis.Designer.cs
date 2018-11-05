namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewHAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewHAnalysis));
            this.splitContainerHoriz1 = new System.Windows.Forms.SplitContainer();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lbName = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.lbDescription = new System.Windows.Forms.Label();
            this.splitContainerHoriz2 = new System.Windows.Forms.SplitContainer();
            this.splitContainerVert = new System.Windows.Forms.SplitContainer();
            this.gridContent = new SourceGrid.Grid();
            this.splitContainerSolutions = new System.Windows.Forms.SplitContainer();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.bnSolItemIndexUp = new System.Windows.Forms.Button();
            this.lbSolItemIndex = new System.Windows.Forms.Label();
            this.bnSolItemIndexDown = new System.Windows.Forms.Button();
            this.gridSolutions = new SourceGrid.Grid();
            this.bnNext = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz1)).BeginInit();
            this.splitContainerHoriz1.Panel1.SuspendLayout();
            this.splitContainerHoriz1.Panel2.SuspendLayout();
            this.splitContainerHoriz1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz2)).BeginInit();
            this.splitContainerHoriz2.Panel1.SuspendLayout();
            this.splitContainerHoriz2.Panel2.SuspendLayout();
            this.splitContainerHoriz2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.Panel2.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSolutions)).BeginInit();
            this.splitContainerSolutions.Panel1.SuspendLayout();
            this.splitContainerSolutions.Panel2.SuspendLayout();
            this.splitContainerSolutions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz1
            // 
            resources.ApplyResources(this.splitContainerHoriz1, "splitContainerHoriz1");
            this.splitContainerHoriz1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerHoriz1.Name = "splitContainerHoriz1";
            // 
            // splitContainerHoriz1.Panel1
            // 
            this.splitContainerHoriz1.Panel1.Controls.Add(this.tbName);
            this.splitContainerHoriz1.Panel1.Controls.Add(this.lbName);
            this.splitContainerHoriz1.Panel1.Controls.Add(this.tbDescription);
            this.splitContainerHoriz1.Panel1.Controls.Add(this.lbDescription);
            // 
            // splitContainerHoriz1.Panel2
            // 
            this.splitContainerHoriz1.Panel2.Controls.Add(this.splitContainerHoriz2);
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            // 
            // lbDescription
            // 
            resources.ApplyResources(this.lbDescription, "lbDescription");
            this.lbDescription.Name = "lbDescription";
            // 
            // splitContainerHoriz2
            // 
            resources.ApplyResources(this.splitContainerHoriz2, "splitContainerHoriz2");
            this.splitContainerHoriz2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerHoriz2.Name = "splitContainerHoriz2";
            // 
            // splitContainerHoriz2.Panel1
            // 
            this.splitContainerHoriz2.Panel1.Controls.Add(this.splitContainerVert);
            // 
            // splitContainerHoriz2.Panel2
            // 
            this.splitContainerHoriz2.Panel2.Controls.Add(this.bnNext);
            // 
            // splitContainerVert
            // 
            resources.ApplyResources(this.splitContainerVert, "splitContainerVert");
            this.splitContainerVert.Name = "splitContainerVert";
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.gridContent);
            // 
            // splitContainerVert.Panel2
            // 
            this.splitContainerVert.Panel2.Controls.Add(this.splitContainerSolutions);
            // 
            // gridContent
            // 
            this.gridContent.EnableSort = true;
            resources.ApplyResources(this.gridContent, "gridContent");
            this.gridContent.Name = "gridContent";
            this.gridContent.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridContent.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridContent.TabStop = true;
            this.gridContent.ToolTipText = "";
            // 
            // splitContainerSolutions
            // 
            resources.ApplyResources(this.splitContainerSolutions, "splitContainerSolutions");
            this.splitContainerSolutions.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerSolutions.Name = "splitContainerSolutions";
            // 
            // splitContainerSolutions.Panel1
            // 
            this.splitContainerSolutions.Panel1.Controls.Add(this.graphCtrl);
            // 
            // splitContainerSolutions.Panel2
            // 
            this.splitContainerSolutions.Panel2.Controls.Add(this.splitContainer1);
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.Viewer = null;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.bnSolItemIndexUp);
            this.splitContainer1.Panel1.Controls.Add(this.lbSolItemIndex);
            this.splitContainer1.Panel1.Controls.Add(this.bnSolItemIndexDown);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gridSolutions);
            // 
            // bnSolItemIndexUp
            // 
            resources.ApplyResources(this.bnSolItemIndexUp, "bnSolItemIndexUp");
            this.bnSolItemIndexUp.Name = "bnSolItemIndexUp";
            this.bnSolItemIndexUp.UseVisualStyleBackColor = true;
            this.bnSolItemIndexUp.Click += new System.EventHandler(this.OnSolItemIndexUp);
            // 
            // lbSolItemIndex
            // 
            resources.ApplyResources(this.lbSolItemIndex, "lbSolItemIndex");
            this.lbSolItemIndex.Name = "lbSolItemIndex";
            // 
            // bnSolItemIndexDown
            // 
            resources.ApplyResources(this.bnSolItemIndexDown, "bnSolItemIndexDown");
            this.bnSolItemIndexDown.Name = "bnSolItemIndexDown";
            this.bnSolItemIndexDown.UseVisualStyleBackColor = true;
            this.bnSolItemIndexDown.Click += new System.EventHandler(this.OnSolItemIndexDown);
            // 
            // gridSolutions
            // 
            this.gridSolutions.AcceptsInputChar = false;
            this.gridSolutions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridSolutions.ColumnsCount = 5;
            resources.ApplyResources(this.gridSolutions, "gridSolutions");
            this.gridSolutions.EnableSort = false;
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridSolutions.SpecialKeys = SourceGrid.GridSpecialKeys.PageDownUp;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // bnNext
            // 
            resources.ApplyResources(this.bnNext, "bnNext");
            this.bnNext.Name = "bnNext";
            this.bnNext.UseVisualStyleBackColor = true;
            this.bnNext.Click += new System.EventHandler(this.OnNext);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            resources.ApplyResources(this.statusLabel, "statusLabel");
            // 
            // FormNewHAnalysis
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerHoriz1);
            this.Controls.Add(this.statusStrip);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewHAnalysis";
            this.ShowInTaskbar = false;
            this.splitContainerHoriz1.Panel1.ResumeLayout(false);
            this.splitContainerHoriz1.Panel1.PerformLayout();
            this.splitContainerHoriz1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz1)).EndInit();
            this.splitContainerHoriz1.ResumeLayout(false);
            this.splitContainerHoriz2.Panel1.ResumeLayout(false);
            this.splitContainerHoriz2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz2)).EndInit();
            this.splitContainerHoriz2.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.splitContainerSolutions.Panel1.ResumeLayout(false);
            this.splitContainerSolutions.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSolutions)).EndInit();
            this.splitContainerSolutions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        protected System.Windows.Forms.TextBox tbDescription;
        protected System.Windows.Forms.TextBox tbName;
        protected System.Windows.Forms.Label lbDescription;
        protected System.Windows.Forms.Label lbName;
        internal Graphics.Graphics3DControl graphCtrl;
        internal System.Windows.Forms.Button bnNext;
        internal System.Windows.Forms.SplitContainer splitContainerSolutions;
        protected System.Windows.Forms.SplitContainer splitContainerHoriz1;
        protected System.Windows.Forms.SplitContainer splitContainerHoriz2;
        protected System.Windows.Forms.SplitContainer splitContainerVert;
        protected SourceGrid.Grid gridContent;
        protected SourceGrid.Grid gridSolutions;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lbSolItemIndex;
        private System.Windows.Forms.Button bnSolItemIndexDown;
        private System.Windows.Forms.Button bnSolItemIndexUp;
    }
}