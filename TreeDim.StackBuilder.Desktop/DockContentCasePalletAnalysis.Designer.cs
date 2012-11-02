﻿namespace TreeDim.StackBuilder.Desktop
{
    partial class DockContentCasePalletAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockContentCasePalletAnalysis));
            this.splitContainerHoriz = new System.Windows.Forms.SplitContainer();
            this.pictureBoxSolution = new System.Windows.Forms.PictureBox();
            this.trackBarAngleHoriz = new System.Windows.Forms.TrackBar();
            this.btSelectSolution = new System.Windows.Forms.Button();
            this.trackBarAngleVert = new System.Windows.Forms.TrackBar();
            this.gridSolutions = new SourceGrid.Grid();
            this.toolStrip_view = new System.Windows.Forms.ToolStrip();
            this.toolStripCornerView0 = new System.Windows.Forms.ToolStripButton();
            this.toolStripCornerView90 = new System.Windows.Forms.ToolStripButton();
            this.toolStripCornerView180 = new System.Windows.Forms.ToolStripButton();
            this.toolStripCornerView270 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFrontView = new System.Windows.Forms.ToolStripButton();
            this.toolStripRightView = new System.Windows.Forms.ToolStripButton();
            this.toolStripBackView = new System.Windows.Forms.ToolStripButton();
            this.toolStripLeftView = new System.Windows.Forms.ToolStripButton();
            this.toolStripTopView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripShowImages = new System.Windows.Forms.ToolStripButton();
            this.splitContainerHoriz.Panel1.SuspendLayout();
            this.splitContainerHoriz.Panel2.SuspendLayout();
            this.splitContainerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSolution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleHoriz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleVert)).BeginInit();
            this.toolStrip_view.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz
            // 
            resources.ApplyResources(this.splitContainerHoriz, "splitContainerHoriz");
            this.splitContainerHoriz.Name = "splitContainerHoriz";
            // 
            // splitContainerHoriz.Panel1
            // 
            this.splitContainerHoriz.Panel1.Controls.Add(this.pictureBoxSolution);
            this.splitContainerHoriz.Panel1.Controls.Add(this.trackBarAngleHoriz);
            this.splitContainerHoriz.Panel1.Controls.Add(this.btSelectSolution);
            this.splitContainerHoriz.Panel1.Controls.Add(this.trackBarAngleVert);
            // 
            // splitContainerHoriz.Panel2
            // 
            this.splitContainerHoriz.Panel2.Controls.Add(this.gridSolutions);
            // 
            // pictureBoxSolution
            // 
            resources.ApplyResources(this.pictureBoxSolution, "pictureBoxSolution");
            this.pictureBoxSolution.Name = "pictureBoxSolution";
            this.pictureBoxSolution.TabStop = false;
            this.pictureBoxSolution.SizeChanged += new System.EventHandler(this.pictureBoxSolution_SizeChanged);
            // 
            // trackBarAngleHoriz
            // 
            resources.ApplyResources(this.trackBarAngleHoriz, "trackBarAngleHoriz");
            this.trackBarAngleHoriz.LargeChange = 45;
            this.trackBarAngleHoriz.Maximum = 360;
            this.trackBarAngleHoriz.Name = "trackBarAngleHoriz";
            this.trackBarAngleHoriz.SmallChange = 45;
            this.trackBarAngleHoriz.TickFrequency = 90;
            this.trackBarAngleHoriz.Value = 225;
            this.trackBarAngleHoriz.ValueChanged += new System.EventHandler(this.onAngleHorizChanged);
            // 
            // btSelectSolution
            // 
            resources.ApplyResources(this.btSelectSolution, "btSelectSolution");
            this.btSelectSolution.Name = "btSelectSolution";
            this.btSelectSolution.UseVisualStyleBackColor = true;
            this.btSelectSolution.Click += new System.EventHandler(this.btSelectSolution_Click);
            // 
            // trackBarAngleVert
            // 
            resources.ApplyResources(this.trackBarAngleVert, "trackBarAngleVert");
            this.trackBarAngleVert.LargeChange = 15;
            this.trackBarAngleVert.Maximum = 90;
            this.trackBarAngleVert.Name = "trackBarAngleVert";
            this.trackBarAngleVert.TickFrequency = 15;
            this.trackBarAngleVert.Value = 45;
            this.trackBarAngleVert.ValueChanged += new System.EventHandler(this.onAngleVertChanged);
            // 
            // gridSolutions
            // 
            this.gridSolutions.AcceptsInputChar = false;
            resources.ApplyResources(this.gridSolutions, "gridSolutions");
            this.gridSolutions.EnableSort = false;
            this.gridSolutions.Name = "gridSolutions";
            this.gridSolutions.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSolutions.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.gridSolutions.SpecialKeys = SourceGrid.GridSpecialKeys.Arrows;
            this.gridSolutions.TabStop = true;
            this.gridSolutions.ToolTipText = "";
            // 
            // toolStrip_view
            // 
            this.toolStrip_view.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCornerView0,
            this.toolStripCornerView90,
            this.toolStripCornerView180,
            this.toolStripCornerView270,
            this.toolStripSeparator1,
            this.toolStripFrontView,
            this.toolStripRightView,
            this.toolStripBackView,
            this.toolStripLeftView,
            this.toolStripTopView,
            this.toolStripSeparator2,
            this.toolStripShowImages});
            resources.ApplyResources(this.toolStrip_view, "toolStrip_view");
            this.toolStrip_view.Name = "toolStrip_view";
            // 
            // toolStripCornerView0
            // 
            this.toolStripCornerView0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView0.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View0;
            resources.ApplyResources(this.toolStripCornerView0, "toolStripCornerView0");
            this.toolStripCornerView0.Name = "toolStripCornerView0";
            this.toolStripCornerView0.Click += new System.EventHandler(this.onViewCorner_0);
            // 
            // toolStripCornerView90
            // 
            this.toolStripCornerView90.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView90.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View90;
            resources.ApplyResources(this.toolStripCornerView90, "toolStripCornerView90");
            this.toolStripCornerView90.Name = "toolStripCornerView90";
            this.toolStripCornerView90.Click += new System.EventHandler(this.onViewCorner_90);
            // 
            // toolStripCornerView180
            // 
            this.toolStripCornerView180.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView180.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View180;
            resources.ApplyResources(this.toolStripCornerView180, "toolStripCornerView180");
            this.toolStripCornerView180.Name = "toolStripCornerView180";
            this.toolStripCornerView180.Click += new System.EventHandler(this.onViewCorner_180);
            // 
            // toolStripCornerView270
            // 
            this.toolStripCornerView270.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripCornerView270.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View270;
            resources.ApplyResources(this.toolStripCornerView270, "toolStripCornerView270");
            this.toolStripCornerView270.Name = "toolStripCornerView270";
            this.toolStripCornerView270.Click += new System.EventHandler(this.onViewCorner_270);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripFrontView
            // 
            this.toolStripFrontView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripFrontView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_1;
            resources.ApplyResources(this.toolStripFrontView, "toolStripFrontView");
            this.toolStripFrontView.Name = "toolStripFrontView";
            this.toolStripFrontView.Click += new System.EventHandler(this.onViewSideFront);
            // 
            // toolStripRightView
            // 
            this.toolStripRightView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRightView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_2;
            resources.ApplyResources(this.toolStripRightView, "toolStripRightView");
            this.toolStripRightView.Name = "toolStripRightView";
            this.toolStripRightView.Click += new System.EventHandler(this.onViewSideRight);
            // 
            // toolStripBackView
            // 
            this.toolStripBackView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBackView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_3;
            resources.ApplyResources(this.toolStripBackView, "toolStripBackView");
            this.toolStripBackView.Name = "toolStripBackView";
            this.toolStripBackView.Click += new System.EventHandler(this.onViewSideRear);
            // 
            // toolStripLeftView
            // 
            this.toolStripLeftView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLeftView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_4;
            resources.ApplyResources(this.toolStripLeftView, "toolStripLeftView");
            this.toolStripLeftView.Name = "toolStripLeftView";
            this.toolStripLeftView.Click += new System.EventHandler(this.onViewSideLeft);
            // 
            // toolStripTopView
            // 
            this.toolStripTopView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripTopView.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.View_Top;
            resources.ApplyResources(this.toolStripTopView, "toolStripTopView");
            this.toolStripTopView.Name = "toolStripTopView";
            this.toolStripTopView.Click += new System.EventHandler(this.onViewTop);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripShowImages
            // 
            this.toolStripShowImages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripShowImages.Image = global::TreeDim.StackBuilder.Desktop.Properties.Resources.Image;
            this.toolStripShowImages.Name = "toolStripShowImages";
            resources.ApplyResources(this.toolStripShowImages, "toolStripShowImages");
            this.toolStripShowImages.Click += new System.EventHandler(this.toolStripShowImages_Click);
            // 
            // DockContentAnalysis
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerHoriz);
            this.Controls.Add(this.toolStrip_view);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.HideOnClose = true;
            this.Name = "DockContentAnalysis";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.DockContentAnalysis_Load);
            this.splitContainerHoriz.Panel1.ResumeLayout(false);
            this.splitContainerHoriz.Panel1.PerformLayout();
            this.splitContainerHoriz.Panel2.ResumeLayout(false);
            this.splitContainerHoriz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSolution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleHoriz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngleVert)).EndInit();
            this.toolStrip_view.ResumeLayout(false);
            this.toolStrip_view.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxSolution;
        private System.Windows.Forms.TrackBar trackBarAngleHoriz;
        private System.Windows.Forms.TrackBar trackBarAngleVert;
        private SourceGrid.Grid gridSolutions;
        private System.Windows.Forms.ToolStrip toolStrip_view;
        private System.Windows.Forms.ToolStripButton toolStripCornerView0;
        private System.Windows.Forms.ToolStripButton toolStripCornerView90;
        private System.Windows.Forms.ToolStripButton toolStripCornerView180;
        private System.Windows.Forms.ToolStripButton toolStripCornerView270;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripFrontView;
        private System.Windows.Forms.ToolStripButton toolStripRightView;
        private System.Windows.Forms.ToolStripButton toolStripBackView;
        private System.Windows.Forms.ToolStripButton toolStripLeftView;
        private System.Windows.Forms.ToolStripButton toolStripTopView;
        private System.Windows.Forms.Button btSelectSolution;
        private System.Windows.Forms.SplitContainer splitContainerHoriz;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripShowImages;
    }
}