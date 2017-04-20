﻿namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewTruck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewTruck));
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.lbDescription = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.uLengthHeight = new System.Windows.Forms.Label();
            this.uLengthWidth = new System.Windows.Forms.Label();
            this.uLengthLength = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.nudLength = new System.Windows.Forms.NumericUpDown();
            this.lbHeight = new System.Windows.Forms.Label();
            this.lbWidth = new System.Windows.Forms.Label();
            this.lbLength = new System.Windows.Forms.Label();
            this.cbColor = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.lbColor = new System.Windows.Forms.Label();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.label1 = new System.Windows.Forms.Label();
            this.nudAdmissibleLoadWeight = new System.Windows.Forms.NumericUpDown();
            this.uMassAdmissibleLoad = new System.Windows.Forms.Label();
            this.statusStripDef = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDef = new System.Windows.Forms.ToolStripStatusLabel();
            this.bnSendToDB = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdmissibleLoadWeight)).BeginInit();
            this.statusStripDef.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            resources.ApplyResources(this.bnOK, "bnOK");
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Name = "bnOK";
            this.bnOK.UseVisualStyleBackColor = true;
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            this.tbName.TextChanged += new System.EventHandler(this.onNameDescriptionChanged);
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.TextChanged += new System.EventHandler(this.onNameDescriptionChanged);
            // 
            // lbDescription
            // 
            resources.ApplyResources(this.lbDescription, "lbDescription");
            this.lbDescription.Name = "lbDescription";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            // 
            // uLengthHeight
            // 
            resources.ApplyResources(this.uLengthHeight, "uLengthHeight");
            this.uLengthHeight.Name = "uLengthHeight";
            // 
            // uLengthWidth
            // 
            resources.ApplyResources(this.uLengthWidth, "uLengthWidth");
            this.uLengthWidth.Name = "uLengthWidth";
            // 
            // uLengthLength
            // 
            resources.ApplyResources(this.uLengthLength, "uLengthLength");
            this.uLengthLength.Name = "uLengthLength";
            // 
            // nudHeight
            // 
            resources.ApplyResources(this.nudHeight, "nudHeight");
            this.nudHeight.DecimalPlaces = 1;
            this.nudHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.ValueChanged += new System.EventHandler(this.onTruckPropertyChanged);
            // 
            // nudWidth
            // 
            resources.ApplyResources(this.nudWidth, "nudWidth");
            this.nudWidth.DecimalPlaces = 1;
            this.nudWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.ValueChanged += new System.EventHandler(this.onTruckPropertyChanged);
            // 
            // nudLength
            // 
            resources.ApplyResources(this.nudLength, "nudLength");
            this.nudLength.DecimalPlaces = 1;
            this.nudLength.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudLength.Name = "nudLength";
            this.nudLength.ValueChanged += new System.EventHandler(this.onTruckPropertyChanged);
            // 
            // lbHeight
            // 
            resources.ApplyResources(this.lbHeight, "lbHeight");
            this.lbHeight.Name = "lbHeight";
            // 
            // lbWidth
            // 
            resources.ApplyResources(this.lbWidth, "lbWidth");
            this.lbWidth.Name = "lbWidth";
            // 
            // lbLength
            // 
            resources.ApplyResources(this.lbLength, "lbLength");
            this.lbLength.Name = "lbLength";
            // 
            // cbColor
            // 
            resources.ApplyResources(this.cbColor, "cbColor");
            this.cbColor.Color = System.Drawing.Color.Gold;
            this.cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbColor.DropDownHeight = 1;
            this.cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColor.DropDownWidth = 1;
            this.cbColor.FormattingEnabled = true;
            this.cbColor.Items.AddRange(new object[] {
            resources.GetString("cbColor.Items"),
            resources.GetString("cbColor.Items1"),
            resources.GetString("cbColor.Items2"),
            resources.GetString("cbColor.Items3"),
            resources.GetString("cbColor.Items4"),
            resources.GetString("cbColor.Items5"),
            resources.GetString("cbColor.Items6"),
            resources.GetString("cbColor.Items7"),
            resources.GetString("cbColor.Items8"),
            resources.GetString("cbColor.Items9"),
            resources.GetString("cbColor.Items10"),
            resources.GetString("cbColor.Items11"),
            resources.GetString("cbColor.Items12"),
            resources.GetString("cbColor.Items13"),
            resources.GetString("cbColor.Items14"),
            resources.GetString("cbColor.Items15"),
            resources.GetString("cbColor.Items16"),
            resources.GetString("cbColor.Items17"),
            resources.GetString("cbColor.Items18"),
            resources.GetString("cbColor.Items19"),
            resources.GetString("cbColor.Items20"),
            resources.GetString("cbColor.Items21"),
            resources.GetString("cbColor.Items22"),
            resources.GetString("cbColor.Items23"),
            resources.GetString("cbColor.Items24"),
            resources.GetString("cbColor.Items25"),
            resources.GetString("cbColor.Items26"),
            resources.GetString("cbColor.Items27"),
            resources.GetString("cbColor.Items28"),
            resources.GetString("cbColor.Items29"),
            resources.GetString("cbColor.Items30"),
            resources.GetString("cbColor.Items31"),
            resources.GetString("cbColor.Items32"),
            resources.GetString("cbColor.Items33"),
            resources.GetString("cbColor.Items34"),
            resources.GetString("cbColor.Items35"),
            resources.GetString("cbColor.Items36"),
            resources.GetString("cbColor.Items37"),
            resources.GetString("cbColor.Items38"),
            resources.GetString("cbColor.Items39"),
            resources.GetString("cbColor.Items40"),
            resources.GetString("cbColor.Items41"),
            resources.GetString("cbColor.Items42"),
            resources.GetString("cbColor.Items43"),
            resources.GetString("cbColor.Items44")});
            this.cbColor.Name = "cbColor";
            this.cbColor.SelectedColorChanged += new System.EventHandler(this.onTruckPropertyChanged);
            // 
            // lbColor
            // 
            resources.ApplyResources(this.lbColor, "lbColor");
            this.lbColor.Name = "lbColor";
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.TabStop = false;
            this.graphCtrl.Viewer = null;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // nudAdmissibleLoadWeight
            // 
            resources.ApplyResources(this.nudAdmissibleLoadWeight, "nudAdmissibleLoadWeight");
            this.nudAdmissibleLoadWeight.DecimalPlaces = 1;
            this.nudAdmissibleLoadWeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudAdmissibleLoadWeight.Name = "nudAdmissibleLoadWeight";
            this.nudAdmissibleLoadWeight.ValueChanged += new System.EventHandler(this.onTruckPropertyChanged);
            // 
            // uMassAdmissibleLoad
            // 
            resources.ApplyResources(this.uMassAdmissibleLoad, "uMassAdmissibleLoad");
            this.uMassAdmissibleLoad.Name = "uMassAdmissibleLoad";
            // 
            // statusStripDef
            // 
            this.statusStripDef.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDef});
            resources.ApplyResources(this.statusStripDef, "statusStripDef");
            this.statusStripDef.Name = "statusStripDef";
            // 
            // toolStripStatusLabelDef
            // 
            this.toolStripStatusLabelDef.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatusLabelDef.Name = "toolStripStatusLabelDef";
            resources.ApplyResources(this.toolStripStatusLabelDef, "toolStripStatusLabelDef");
            // 
            // bnSendToDB
            // 
            resources.ApplyResources(this.bnSendToDB, "bnSendToDB");
            this.bnSendToDB.Name = "bnSendToDB";
            this.bnSendToDB.UseVisualStyleBackColor = true;
            this.bnSendToDB.Click += new System.EventHandler(this.onSendToDB);
            // 
            // FormNewTruck
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bnSendToDB);
            this.Controls.Add(this.statusStripDef);
            this.Controls.Add(this.uMassAdmissibleLoad);
            this.Controls.Add(this.nudAdmissibleLoadWeight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbColor);
            this.Controls.Add(this.lbColor);
            this.Controls.Add(this.uLengthHeight);
            this.Controls.Add(this.uLengthWidth);
            this.Controls.Add(this.uLengthLength);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.nudLength);
            this.Controls.Add(this.lbHeight);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.lbLength);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.lbDescription);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.graphCtrl);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewTruck";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdmissibleLoadWeight)).EndInit();
            this.statusStripDef.ResumeLayout(false);
            this.statusStripDef.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOK;
        private treeDiM.StackBuilder.Graphics.Graphics3DControl graphCtrl;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label uLengthHeight;
        private System.Windows.Forms.Label uLengthWidth;
        private System.Windows.Forms.Label uLengthLength;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.NumericUpDown nudLength;
        private System.Windows.Forms.Label lbHeight;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbLength;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbColor;
        private System.Windows.Forms.Label lbColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudAdmissibleLoadWeight;
        private System.Windows.Forms.Label uMassAdmissibleLoad;
        private System.Windows.Forms.StatusStrip statusStripDef;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDef;
        private System.Windows.Forms.Button bnSendToDB;
    }
}