﻿namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewCylinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewCylinder));
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.gbDimensions = new System.Windows.Forms.GroupBox();
            this.gbWeight = new System.Windows.Forms.GroupBox();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.statusStripDef = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDef = new System.Windows.Forms.ToolStripStatusLabel();
            this.gbFaceColor = new System.Windows.Forms.GroupBox();
            this.cbColorWallInner = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cbColorWallOuter = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.lbWallColor = new System.Windows.Forms.Label();
            this.cbColorTop = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.lbTop = new System.Windows.Forms.Label();
            this.uCtrlDiameterOuter = new treeDiM.StackBuilder.Basics.UCtrlDouble();
            this.uCtrlDiameterInner = new treeDiM.StackBuilder.Basics.UCtrlDouble();
            this.uCtrlHeight = new treeDiM.StackBuilder.Basics.UCtrlDouble();
            this.uCtrlWeight = new treeDiM.StackBuilder.Basics.UCtrlDouble();
            this.uCtrlNetWeight = new treeDiM.StackBuilder.Basics.UCtrlOptDouble();
            this.bnSendToDB = new System.Windows.Forms.Button();
            this.gbDimensions.SuspendLayout();
            this.gbWeight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.statusStripDef.SuspendLayout();
            this.gbFaceColor.SuspendLayout();
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
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.TextChanged += new System.EventHandler(this.onCylinderPropertiesChanged);
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            this.tbName.TextChanged += new System.EventHandler(this.onCylinderPropertiesChanged);
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // gbDimensions
            // 
            this.gbDimensions.Controls.Add(this.uCtrlHeight);
            this.gbDimensions.Controls.Add(this.uCtrlDiameterInner);
            this.gbDimensions.Controls.Add(this.uCtrlDiameterOuter);
            resources.ApplyResources(this.gbDimensions, "gbDimensions");
            this.gbDimensions.Name = "gbDimensions";
            this.gbDimensions.TabStop = false;
            // 
            // gbWeight
            // 
            this.gbWeight.Controls.Add(this.uCtrlNetWeight);
            this.gbWeight.Controls.Add(this.uCtrlWeight);
            resources.ApplyResources(this.gbWeight, "gbWeight");
            this.gbWeight.Name = "gbWeight";
            this.gbWeight.TabStop = false;
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.TabStop = false;
            this.graphCtrl.Viewer = null;
            // 
            // statusStripDef
            // 
            this.statusStripDef.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDef});
            resources.ApplyResources(this.statusStripDef, "statusStripDef");
            this.statusStripDef.Name = "statusStripDef";
            this.statusStripDef.SizingGrip = false;
            // 
            // toolStripStatusLabelDef
            // 
            this.toolStripStatusLabelDef.ForeColor = System.Drawing.Color.Red;
            this.toolStripStatusLabelDef.Name = "toolStripStatusLabelDef";
            resources.ApplyResources(this.toolStripStatusLabelDef, "toolStripStatusLabelDef");
            // 
            // gbFaceColor
            // 
            this.gbFaceColor.Controls.Add(this.cbColorWallInner);
            this.gbFaceColor.Controls.Add(this.label1);
            this.gbFaceColor.Controls.Add(this.cbColorWallOuter);
            this.gbFaceColor.Controls.Add(this.lbWallColor);
            this.gbFaceColor.Controls.Add(this.cbColorTop);
            this.gbFaceColor.Controls.Add(this.lbTop);
            resources.ApplyResources(this.gbFaceColor, "gbFaceColor");
            this.gbFaceColor.Name = "gbFaceColor";
            this.gbFaceColor.TabStop = false;
            // 
            // cbColorWallInner
            // 
            this.cbColorWallInner.Color = System.Drawing.Color.LightSkyBlue;
            this.cbColorWallInner.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbColorWallInner.DropDownHeight = 1;
            this.cbColorWallInner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColorWallInner.DropDownWidth = 1;
            resources.ApplyResources(this.cbColorWallInner, "cbColorWallInner");
            this.cbColorWallInner.Items.AddRange(new object[] {
            resources.GetString("cbColorWallInner.Items"),
            resources.GetString("cbColorWallInner.Items1"),
            resources.GetString("cbColorWallInner.Items2"),
            resources.GetString("cbColorWallInner.Items3"),
            resources.GetString("cbColorWallInner.Items4"),
            resources.GetString("cbColorWallInner.Items5"),
            resources.GetString("cbColorWallInner.Items6"),
            resources.GetString("cbColorWallInner.Items7"),
            resources.GetString("cbColorWallInner.Items8"),
            resources.GetString("cbColorWallInner.Items9"),
            resources.GetString("cbColorWallInner.Items10"),
            resources.GetString("cbColorWallInner.Items11"),
            resources.GetString("cbColorWallInner.Items12"),
            resources.GetString("cbColorWallInner.Items13"),
            resources.GetString("cbColorWallInner.Items14"),
            resources.GetString("cbColorWallInner.Items15"),
            resources.GetString("cbColorWallInner.Items16"),
            resources.GetString("cbColorWallInner.Items17"),
            resources.GetString("cbColorWallInner.Items18"),
            resources.GetString("cbColorWallInner.Items19"),
            resources.GetString("cbColorWallInner.Items20"),
            resources.GetString("cbColorWallInner.Items21"),
            resources.GetString("cbColorWallInner.Items22"),
            resources.GetString("cbColorWallInner.Items23"),
            resources.GetString("cbColorWallInner.Items24"),
            resources.GetString("cbColorWallInner.Items25"),
            resources.GetString("cbColorWallInner.Items26"),
            resources.GetString("cbColorWallInner.Items27"),
            resources.GetString("cbColorWallInner.Items28"),
            resources.GetString("cbColorWallInner.Items29"),
            resources.GetString("cbColorWallInner.Items30"),
            resources.GetString("cbColorWallInner.Items31"),
            resources.GetString("cbColorWallInner.Items32"),
            resources.GetString("cbColorWallInner.Items33"),
            resources.GetString("cbColorWallInner.Items34"),
            resources.GetString("cbColorWallInner.Items35"),
            resources.GetString("cbColorWallInner.Items36"),
            resources.GetString("cbColorWallInner.Items37"),
            resources.GetString("cbColorWallInner.Items38"),
            resources.GetString("cbColorWallInner.Items39"),
            resources.GetString("cbColorWallInner.Items40"),
            resources.GetString("cbColorWallInner.Items41"),
            resources.GetString("cbColorWallInner.Items42"),
            resources.GetString("cbColorWallInner.Items43"),
            resources.GetString("cbColorWallInner.Items44"),
            resources.GetString("cbColorWallInner.Items45"),
            resources.GetString("cbColorWallInner.Items46"),
            resources.GetString("cbColorWallInner.Items47"),
            resources.GetString("cbColorWallInner.Items48"),
            resources.GetString("cbColorWallInner.Items49"),
            resources.GetString("cbColorWallInner.Items50"),
            resources.GetString("cbColorWallInner.Items51"),
            resources.GetString("cbColorWallInner.Items52"),
            resources.GetString("cbColorWallInner.Items53"),
            resources.GetString("cbColorWallInner.Items54"),
            resources.GetString("cbColorWallInner.Items55"),
            resources.GetString("cbColorWallInner.Items56"),
            resources.GetString("cbColorWallInner.Items57"),
            resources.GetString("cbColorWallInner.Items58"),
            resources.GetString("cbColorWallInner.Items59"),
            resources.GetString("cbColorWallInner.Items60"),
            resources.GetString("cbColorWallInner.Items61"),
            resources.GetString("cbColorWallInner.Items62"),
            resources.GetString("cbColorWallInner.Items63"),
            resources.GetString("cbColorWallInner.Items64"),
            resources.GetString("cbColorWallInner.Items65"),
            resources.GetString("cbColorWallInner.Items66"),
            resources.GetString("cbColorWallInner.Items67"),
            resources.GetString("cbColorWallInner.Items68"),
            resources.GetString("cbColorWallInner.Items69"),
            resources.GetString("cbColorWallInner.Items70"),
            resources.GetString("cbColorWallInner.Items71"),
            resources.GetString("cbColorWallInner.Items72"),
            resources.GetString("cbColorWallInner.Items73"),
            resources.GetString("cbColorWallInner.Items74"),
            resources.GetString("cbColorWallInner.Items75"),
            resources.GetString("cbColorWallInner.Items76"),
            resources.GetString("cbColorWallInner.Items77"),
            resources.GetString("cbColorWallInner.Items78"),
            resources.GetString("cbColorWallInner.Items79"),
            resources.GetString("cbColorWallInner.Items80"),
            resources.GetString("cbColorWallInner.Items81"),
            resources.GetString("cbColorWallInner.Items82"),
            resources.GetString("cbColorWallInner.Items83"),
            resources.GetString("cbColorWallInner.Items84"),
            resources.GetString("cbColorWallInner.Items85"),
            resources.GetString("cbColorWallInner.Items86")});
            this.cbColorWallInner.Name = "cbColorWallInner";
            this.cbColorWallInner.SelectedColorChanged += new System.EventHandler(this.onCylinderPropertiesChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbColorWallOuter
            // 
            this.cbColorWallOuter.Color = System.Drawing.Color.LightSkyBlue;
            this.cbColorWallOuter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbColorWallOuter.DropDownHeight = 1;
            this.cbColorWallOuter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColorWallOuter.DropDownWidth = 1;
            resources.ApplyResources(this.cbColorWallOuter, "cbColorWallOuter");
            this.cbColorWallOuter.Items.AddRange(new object[] {
            resources.GetString("cbColorWallOuter.Items"),
            resources.GetString("cbColorWallOuter.Items1"),
            resources.GetString("cbColorWallOuter.Items2"),
            resources.GetString("cbColorWallOuter.Items3"),
            resources.GetString("cbColorWallOuter.Items4"),
            resources.GetString("cbColorWallOuter.Items5"),
            resources.GetString("cbColorWallOuter.Items6"),
            resources.GetString("cbColorWallOuter.Items7"),
            resources.GetString("cbColorWallOuter.Items8"),
            resources.GetString("cbColorWallOuter.Items9"),
            resources.GetString("cbColorWallOuter.Items10"),
            resources.GetString("cbColorWallOuter.Items11"),
            resources.GetString("cbColorWallOuter.Items12"),
            resources.GetString("cbColorWallOuter.Items13"),
            resources.GetString("cbColorWallOuter.Items14"),
            resources.GetString("cbColorWallOuter.Items15"),
            resources.GetString("cbColorWallOuter.Items16"),
            resources.GetString("cbColorWallOuter.Items17"),
            resources.GetString("cbColorWallOuter.Items18"),
            resources.GetString("cbColorWallOuter.Items19"),
            resources.GetString("cbColorWallOuter.Items20"),
            resources.GetString("cbColorWallOuter.Items21"),
            resources.GetString("cbColorWallOuter.Items22"),
            resources.GetString("cbColorWallOuter.Items23"),
            resources.GetString("cbColorWallOuter.Items24"),
            resources.GetString("cbColorWallOuter.Items25"),
            resources.GetString("cbColorWallOuter.Items26"),
            resources.GetString("cbColorWallOuter.Items27"),
            resources.GetString("cbColorWallOuter.Items28"),
            resources.GetString("cbColorWallOuter.Items29"),
            resources.GetString("cbColorWallOuter.Items30"),
            resources.GetString("cbColorWallOuter.Items31"),
            resources.GetString("cbColorWallOuter.Items32"),
            resources.GetString("cbColorWallOuter.Items33"),
            resources.GetString("cbColorWallOuter.Items34"),
            resources.GetString("cbColorWallOuter.Items35"),
            resources.GetString("cbColorWallOuter.Items36"),
            resources.GetString("cbColorWallOuter.Items37"),
            resources.GetString("cbColorWallOuter.Items38"),
            resources.GetString("cbColorWallOuter.Items39"),
            resources.GetString("cbColorWallOuter.Items40"),
            resources.GetString("cbColorWallOuter.Items41"),
            resources.GetString("cbColorWallOuter.Items42"),
            resources.GetString("cbColorWallOuter.Items43"),
            resources.GetString("cbColorWallOuter.Items44"),
            resources.GetString("cbColorWallOuter.Items45"),
            resources.GetString("cbColorWallOuter.Items46"),
            resources.GetString("cbColorWallOuter.Items47"),
            resources.GetString("cbColorWallOuter.Items48"),
            resources.GetString("cbColorWallOuter.Items49"),
            resources.GetString("cbColorWallOuter.Items50"),
            resources.GetString("cbColorWallOuter.Items51"),
            resources.GetString("cbColorWallOuter.Items52"),
            resources.GetString("cbColorWallOuter.Items53"),
            resources.GetString("cbColorWallOuter.Items54"),
            resources.GetString("cbColorWallOuter.Items55"),
            resources.GetString("cbColorWallOuter.Items56"),
            resources.GetString("cbColorWallOuter.Items57"),
            resources.GetString("cbColorWallOuter.Items58"),
            resources.GetString("cbColorWallOuter.Items59"),
            resources.GetString("cbColorWallOuter.Items60"),
            resources.GetString("cbColorWallOuter.Items61"),
            resources.GetString("cbColorWallOuter.Items62"),
            resources.GetString("cbColorWallOuter.Items63"),
            resources.GetString("cbColorWallOuter.Items64"),
            resources.GetString("cbColorWallOuter.Items65"),
            resources.GetString("cbColorWallOuter.Items66"),
            resources.GetString("cbColorWallOuter.Items67"),
            resources.GetString("cbColorWallOuter.Items68"),
            resources.GetString("cbColorWallOuter.Items69"),
            resources.GetString("cbColorWallOuter.Items70"),
            resources.GetString("cbColorWallOuter.Items71"),
            resources.GetString("cbColorWallOuter.Items72"),
            resources.GetString("cbColorWallOuter.Items73"),
            resources.GetString("cbColorWallOuter.Items74"),
            resources.GetString("cbColorWallOuter.Items75"),
            resources.GetString("cbColorWallOuter.Items76"),
            resources.GetString("cbColorWallOuter.Items77"),
            resources.GetString("cbColorWallOuter.Items78"),
            resources.GetString("cbColorWallOuter.Items79"),
            resources.GetString("cbColorWallOuter.Items80"),
            resources.GetString("cbColorWallOuter.Items81"),
            resources.GetString("cbColorWallOuter.Items82"),
            resources.GetString("cbColorWallOuter.Items83"),
            resources.GetString("cbColorWallOuter.Items84"),
            resources.GetString("cbColorWallOuter.Items85")});
            this.cbColorWallOuter.Name = "cbColorWallOuter";
            this.cbColorWallOuter.SelectedColorChanged += new System.EventHandler(this.onCylinderPropertiesChanged);
            // 
            // lbWallColor
            // 
            resources.ApplyResources(this.lbWallColor, "lbWallColor");
            this.lbWallColor.Name = "lbWallColor";
            // 
            // cbColorTop
            // 
            this.cbColorTop.Color = System.Drawing.Color.Gray;
            this.cbColorTop.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbColorTop.DropDownHeight = 1;
            this.cbColorTop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColorTop.DropDownWidth = 1;
            resources.ApplyResources(this.cbColorTop, "cbColorTop");
            this.cbColorTop.Items.AddRange(new object[] {
            resources.GetString("cbColorTop.Items"),
            resources.GetString("cbColorTop.Items1"),
            resources.GetString("cbColorTop.Items2"),
            resources.GetString("cbColorTop.Items3"),
            resources.GetString("cbColorTop.Items4"),
            resources.GetString("cbColorTop.Items5"),
            resources.GetString("cbColorTop.Items6"),
            resources.GetString("cbColorTop.Items7"),
            resources.GetString("cbColorTop.Items8"),
            resources.GetString("cbColorTop.Items9"),
            resources.GetString("cbColorTop.Items10"),
            resources.GetString("cbColorTop.Items11"),
            resources.GetString("cbColorTop.Items12"),
            resources.GetString("cbColorTop.Items13"),
            resources.GetString("cbColorTop.Items14"),
            resources.GetString("cbColorTop.Items15"),
            resources.GetString("cbColorTop.Items16"),
            resources.GetString("cbColorTop.Items17"),
            resources.GetString("cbColorTop.Items18"),
            resources.GetString("cbColorTop.Items19"),
            resources.GetString("cbColorTop.Items20"),
            resources.GetString("cbColorTop.Items21"),
            resources.GetString("cbColorTop.Items22"),
            resources.GetString("cbColorTop.Items23"),
            resources.GetString("cbColorTop.Items24"),
            resources.GetString("cbColorTop.Items25"),
            resources.GetString("cbColorTop.Items26"),
            resources.GetString("cbColorTop.Items27"),
            resources.GetString("cbColorTop.Items28"),
            resources.GetString("cbColorTop.Items29"),
            resources.GetString("cbColorTop.Items30"),
            resources.GetString("cbColorTop.Items31"),
            resources.GetString("cbColorTop.Items32"),
            resources.GetString("cbColorTop.Items33"),
            resources.GetString("cbColorTop.Items34"),
            resources.GetString("cbColorTop.Items35"),
            resources.GetString("cbColorTop.Items36"),
            resources.GetString("cbColorTop.Items37"),
            resources.GetString("cbColorTop.Items38"),
            resources.GetString("cbColorTop.Items39"),
            resources.GetString("cbColorTop.Items40"),
            resources.GetString("cbColorTop.Items41"),
            resources.GetString("cbColorTop.Items42"),
            resources.GetString("cbColorTop.Items43"),
            resources.GetString("cbColorTop.Items44"),
            resources.GetString("cbColorTop.Items45"),
            resources.GetString("cbColorTop.Items46"),
            resources.GetString("cbColorTop.Items47"),
            resources.GetString("cbColorTop.Items48"),
            resources.GetString("cbColorTop.Items49"),
            resources.GetString("cbColorTop.Items50"),
            resources.GetString("cbColorTop.Items51"),
            resources.GetString("cbColorTop.Items52"),
            resources.GetString("cbColorTop.Items53"),
            resources.GetString("cbColorTop.Items54"),
            resources.GetString("cbColorTop.Items55"),
            resources.GetString("cbColorTop.Items56"),
            resources.GetString("cbColorTop.Items57"),
            resources.GetString("cbColorTop.Items58"),
            resources.GetString("cbColorTop.Items59"),
            resources.GetString("cbColorTop.Items60"),
            resources.GetString("cbColorTop.Items61"),
            resources.GetString("cbColorTop.Items62"),
            resources.GetString("cbColorTop.Items63"),
            resources.GetString("cbColorTop.Items64"),
            resources.GetString("cbColorTop.Items65"),
            resources.GetString("cbColorTop.Items66"),
            resources.GetString("cbColorTop.Items67"),
            resources.GetString("cbColorTop.Items68"),
            resources.GetString("cbColorTop.Items69"),
            resources.GetString("cbColorTop.Items70"),
            resources.GetString("cbColorTop.Items71"),
            resources.GetString("cbColorTop.Items72"),
            resources.GetString("cbColorTop.Items73"),
            resources.GetString("cbColorTop.Items74"),
            resources.GetString("cbColorTop.Items75"),
            resources.GetString("cbColorTop.Items76"),
            resources.GetString("cbColorTop.Items77"),
            resources.GetString("cbColorTop.Items78"),
            resources.GetString("cbColorTop.Items79"),
            resources.GetString("cbColorTop.Items80"),
            resources.GetString("cbColorTop.Items81"),
            resources.GetString("cbColorTop.Items82"),
            resources.GetString("cbColorTop.Items83"),
            resources.GetString("cbColorTop.Items84")});
            this.cbColorTop.Name = "cbColorTop";
            this.cbColorTop.SelectedColorChanged += new System.EventHandler(this.onCylinderPropertiesChanged);
            // 
            // lbTop
            // 
            resources.ApplyResources(this.lbTop, "lbTop");
            this.lbTop.Name = "lbTop";
            // 
            // uCtrlDiameterOuter
            // 
            resources.ApplyResources(this.uCtrlDiameterOuter, "uCtrlDiameterOuter");
            this.uCtrlDiameterOuter.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlDiameterOuter.Name = "uCtrlDiameterOuter";
            this.uCtrlDiameterOuter.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlDiameterOuter.Value = 0D;
            // 
            // uCtrlDiameterInner
            // 
            resources.ApplyResources(this.uCtrlDiameterInner, "uCtrlDiameterInner");
            this.uCtrlDiameterInner.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlDiameterInner.Name = "uCtrlDiameterInner";
            this.uCtrlDiameterInner.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlDiameterInner.Value = 0D;
            // 
            // uCtrlHeight
            // 
            resources.ApplyResources(this.uCtrlHeight, "uCtrlHeight");
            this.uCtrlHeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlHeight.Name = "uCtrlHeight";
            this.uCtrlHeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlHeight.Value = 0D;
            // 
            // uCtrlWeight
            // 
            resources.ApplyResources(this.uCtrlWeight, "uCtrlWeight");
            this.uCtrlWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlWeight.Name = "uCtrlWeight";
            this.uCtrlWeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlWeight.Value = 0D;
            // 
            // uCtrlNetWeight
            // 
            resources.ApplyResources(this.uCtrlNetWeight, "uCtrlNetWeight");
            this.uCtrlNetWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlNetWeight.Name = "uCtrlNetWeight";
            this.uCtrlNetWeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlNetWeight.Value = ((treeDiM.StackBuilder.Basics.OptDouble)(resources.GetObject("uCtrlNetWeight.Value")));
            // 
            // bnSendToDB
            // 
            resources.ApplyResources(this.bnSendToDB, "bnSendToDB");
            this.bnSendToDB.Name = "bnSendToDB";
            this.bnSendToDB.UseVisualStyleBackColor = true;
            this.bnSendToDB.Click += new System.EventHandler(this.onSendToDatabase);
            // 
            // FormNewCylinder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bnSendToDB);
            this.Controls.Add(this.gbFaceColor);
            this.Controls.Add(this.statusStripDef);
            this.Controls.Add(this.graphCtrl);
            this.Controls.Add(this.gbWeight);
            this.Controls.Add(this.gbDimensions);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewCylinder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.gbDimensions.ResumeLayout(false);
            this.gbWeight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.statusStripDef.ResumeLayout(false);
            this.statusStripDef.PerformLayout();
            this.gbFaceColor.ResumeLayout(false);
            this.gbFaceColor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.GroupBox gbDimensions;
        private System.Windows.Forms.GroupBox gbWeight;
        private treeDiM.StackBuilder.Graphics.Graphics3DControl graphCtrl;
        private System.Windows.Forms.StatusStrip statusStripDef;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDef;
        private System.Windows.Forms.GroupBox gbFaceColor;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbColorWallOuter;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbColorWallInner;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbColorTop;
        private System.Windows.Forms.Label lbWallColor;
        private System.Windows.Forms.Label lbTop;
        private System.Windows.Forms.Label label1;
        private Basics.UCtrlDouble uCtrlHeight;
        private Basics.UCtrlDouble uCtrlDiameterInner;
        private Basics.UCtrlDouble uCtrlDiameterOuter;
        private Basics.UCtrlOptDouble uCtrlNetWeight;
        private Basics.UCtrlDouble uCtrlWeight;
        private System.Windows.Forms.Button bnSendToDB;
    }
}