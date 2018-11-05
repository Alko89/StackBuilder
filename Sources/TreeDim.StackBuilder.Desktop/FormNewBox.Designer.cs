namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewBox));
            this.lbFace = new System.Windows.Forms.Label();
            this.gbDimensions = new System.Windows.Forms.GroupBox();
            this.uCtrlDimensionsInner = new treeDiM.StackBuilder.Basics.UCtrlOptTriDouble();
            this.uCtrlDimensionsOuter = new treeDiM.StackBuilder.Basics.UCtrlTriDouble();
            this.cbFace = new System.Windows.Forms.ComboBox();
            this.cbColor = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.gbFaceColor = new System.Windows.Forms.GroupBox();
            this.btBitmaps = new System.Windows.Forms.Button();
            this.chkAllFaces = new System.Windows.Forms.CheckBox();
            this.gbWeight = new System.Windows.Forms.GroupBox();
            this.uCtrlMaxWeight = new treeDiM.StackBuilder.Basics.UCtrlOptDouble();
            this.vcWeight = new treeDiM.StackBuilder.Basics.UCtrlDouble();
            this.uCtrlNetWeight = new treeDiM.StackBuilder.Basics.UCtrlOptDouble();
            this.graphCtrl = new treeDiM.StackBuilder.Graphics.Graphics3DControl();
            this.uCtrlTapeWidth = new treeDiM.StackBuilder.Basics.UCtrlOptDouble();
            this.cbTapeColor = new OfficePickers.ColorPicker.ComboBoxColorPicker();
            this.lbTapeColor = new System.Windows.Forms.Label();
            this.bnSaveToDB = new System.Windows.Forms.Button();
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tabPageTape = new System.Windows.Forms.TabPage();
            this.tabPageStrappers = new System.Windows.Forms.TabPage();
            this.ctrlStrapperSet = new treeDiM.StackBuilder.Basics.Controls.CtrlStrapperSet();
            this.gbDimensions.SuspendLayout();
            this.gbFaceColor.SuspendLayout();
            this.gbWeight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).BeginInit();
            this.tabCtrl.SuspendLayout();
            this.tabPageTape.SuspendLayout();
            this.tabPageStrappers.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnOk
            // 
            resources.ApplyResources(this.bnOk, "bnOk");
            // 
            // bnCancel
            // 
            resources.ApplyResources(this.bnCancel, "bnCancel");
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            // 
            // lbDescription
            // 
            resources.ApplyResources(this.lbDescription, "lbDescription");
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            // 
            // lbFace
            // 
            resources.ApplyResources(this.lbFace, "lbFace");
            this.lbFace.Name = "lbFace";
            // 
            // gbDimensions
            // 
            resources.ApplyResources(this.gbDimensions, "gbDimensions");
            this.gbDimensions.Controls.Add(this.uCtrlDimensionsInner);
            this.gbDimensions.Controls.Add(this.uCtrlDimensionsOuter);
            this.gbDimensions.Name = "gbDimensions";
            this.gbDimensions.TabStop = false;
            // 
            // uCtrlDimensionsInner
            // 
            resources.ApplyResources(this.uCtrlDimensionsInner, "uCtrlDimensionsInner");
            this.uCtrlDimensionsInner.Checked = false;
            this.uCtrlDimensionsInner.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlDimensionsInner.Name = "uCtrlDimensionsInner";
            this.uCtrlDimensionsInner.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlDimensionsInner.Value = ((Sharp3D.Math.Core.Vector3D)(resources.GetObject("uCtrlDimensionsInner.Value")));
            this.uCtrlDimensionsInner.X = 0D;
            this.uCtrlDimensionsInner.Y = 0D;
            this.uCtrlDimensionsInner.Z = 0D;
            this.uCtrlDimensionsInner.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlOptTriDouble.ValueChangedDelegate(this.OnBoxPropertyChanged);
            // 
            // uCtrlDimensionsOuter
            // 
            resources.ApplyResources(this.uCtrlDimensionsOuter, "uCtrlDimensionsOuter");
            this.uCtrlDimensionsOuter.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlDimensionsOuter.Name = "uCtrlDimensionsOuter";
            this.uCtrlDimensionsOuter.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlDimensionsOuter.Value = ((Sharp3D.Math.Core.Vector3D)(resources.GetObject("uCtrlDimensionsOuter.Value")));
            this.uCtrlDimensionsOuter.ValueX = 0D;
            this.uCtrlDimensionsOuter.ValueY = 0D;
            this.uCtrlDimensionsOuter.ValueZ = 0D;
            this.uCtrlDimensionsOuter.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlTriDouble.ValueChangedDelegate(this.OnBoxPropertyChanged);
            // 
            // cbFace
            // 
            resources.ApplyResources(this.cbFace, "cbFace");
            this.cbFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFace.FormattingEnabled = true;
            this.cbFace.Items.AddRange(new object[] {
            resources.GetString("cbFace.Items"),
            resources.GetString("cbFace.Items1"),
            resources.GetString("cbFace.Items2"),
            resources.GetString("cbFace.Items3"),
            resources.GetString("cbFace.Items4"),
            resources.GetString("cbFace.Items5")});
            this.cbFace.Name = "cbFace";
            this.cbFace.SelectedIndexChanged += new System.EventHandler(this.OnSelectedFaceChanged);
            // 
            // cbColor
            // 
            resources.ApplyResources(this.cbColor, "cbColor");
            this.cbColor.Color = System.Drawing.Color.Chocolate;
            this.cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbColor.DropDownHeight = 1;
            this.cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbColor.DropDownWidth = 1;
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
            resources.GetString("cbColor.Items44"),
            resources.GetString("cbColor.Items45"),
            resources.GetString("cbColor.Items46"),
            resources.GetString("cbColor.Items47"),
            resources.GetString("cbColor.Items48"),
            resources.GetString("cbColor.Items49"),
            resources.GetString("cbColor.Items50"),
            resources.GetString("cbColor.Items51"),
            resources.GetString("cbColor.Items52"),
            resources.GetString("cbColor.Items53"),
            resources.GetString("cbColor.Items54"),
            resources.GetString("cbColor.Items55"),
            resources.GetString("cbColor.Items56"),
            resources.GetString("cbColor.Items57"),
            resources.GetString("cbColor.Items58"),
            resources.GetString("cbColor.Items59"),
            resources.GetString("cbColor.Items60"),
            resources.GetString("cbColor.Items61"),
            resources.GetString("cbColor.Items62"),
            resources.GetString("cbColor.Items63"),
            resources.GetString("cbColor.Items64"),
            resources.GetString("cbColor.Items65"),
            resources.GetString("cbColor.Items66"),
            resources.GetString("cbColor.Items67"),
            resources.GetString("cbColor.Items68"),
            resources.GetString("cbColor.Items69"),
            resources.GetString("cbColor.Items70"),
            resources.GetString("cbColor.Items71"),
            resources.GetString("cbColor.Items72"),
            resources.GetString("cbColor.Items73"),
            resources.GetString("cbColor.Items74"),
            resources.GetString("cbColor.Items75"),
            resources.GetString("cbColor.Items76"),
            resources.GetString("cbColor.Items77"),
            resources.GetString("cbColor.Items78"),
            resources.GetString("cbColor.Items79"),
            resources.GetString("cbColor.Items80"),
            resources.GetString("cbColor.Items81"),
            resources.GetString("cbColor.Items82"),
            resources.GetString("cbColor.Items83"),
            resources.GetString("cbColor.Items84"),
            resources.GetString("cbColor.Items85"),
            resources.GetString("cbColor.Items86"),
            resources.GetString("cbColor.Items87"),
            resources.GetString("cbColor.Items88"),
            resources.GetString("cbColor.Items89"),
            resources.GetString("cbColor.Items90"),
            resources.GetString("cbColor.Items91"),
            resources.GetString("cbColor.Items92"),
            resources.GetString("cbColor.Items93"),
            resources.GetString("cbColor.Items94"),
            resources.GetString("cbColor.Items95"),
            resources.GetString("cbColor.Items96"),
            resources.GetString("cbColor.Items97"),
            resources.GetString("cbColor.Items98"),
            resources.GetString("cbColor.Items99"),
            resources.GetString("cbColor.Items100"),
            resources.GetString("cbColor.Items101"),
            resources.GetString("cbColor.Items102"),
            resources.GetString("cbColor.Items103"),
            resources.GetString("cbColor.Items104"),
            resources.GetString("cbColor.Items105"),
            resources.GetString("cbColor.Items106"),
            resources.GetString("cbColor.Items107"),
            resources.GetString("cbColor.Items108"),
            resources.GetString("cbColor.Items109"),
            resources.GetString("cbColor.Items110"),
            resources.GetString("cbColor.Items111"),
            resources.GetString("cbColor.Items112"),
            resources.GetString("cbColor.Items113"),
            resources.GetString("cbColor.Items114"),
            resources.GetString("cbColor.Items115"),
            resources.GetString("cbColor.Items116"),
            resources.GetString("cbColor.Items117"),
            resources.GetString("cbColor.Items118"),
            resources.GetString("cbColor.Items119"),
            resources.GetString("cbColor.Items120"),
            resources.GetString("cbColor.Items121"),
            resources.GetString("cbColor.Items122")});
            this.cbColor.Name = "cbColor";
            this.cbColor.SelectedColorChanged += new System.EventHandler(this.OnFaceColorChanged);
            // 
            // gbFaceColor
            // 
            resources.ApplyResources(this.gbFaceColor, "gbFaceColor");
            this.gbFaceColor.Controls.Add(this.btBitmaps);
            this.gbFaceColor.Controls.Add(this.chkAllFaces);
            this.gbFaceColor.Controls.Add(this.cbColor);
            this.gbFaceColor.Controls.Add(this.cbFace);
            this.gbFaceColor.Controls.Add(this.lbFace);
            this.gbFaceColor.Name = "gbFaceColor";
            this.gbFaceColor.TabStop = false;
            // 
            // btBitmaps
            // 
            resources.ApplyResources(this.btBitmaps, "btBitmaps");
            this.btBitmaps.Name = "btBitmaps";
            this.btBitmaps.UseVisualStyleBackColor = true;
            this.btBitmaps.Click += new System.EventHandler(this.OnEditTextures);
            // 
            // chkAllFaces
            // 
            resources.ApplyResources(this.chkAllFaces, "chkAllFaces");
            this.chkAllFaces.Name = "chkAllFaces";
            this.chkAllFaces.UseVisualStyleBackColor = true;
            this.chkAllFaces.CheckedChanged += new System.EventHandler(this.OnAllFacesColorCheckedChanged);
            // 
            // gbWeight
            // 
            resources.ApplyResources(this.gbWeight, "gbWeight");
            this.gbWeight.Controls.Add(this.uCtrlMaxWeight);
            this.gbWeight.Controls.Add(this.vcWeight);
            this.gbWeight.Controls.Add(this.uCtrlNetWeight);
            this.gbWeight.Name = "gbWeight";
            this.gbWeight.TabStop = false;
            // 
            // uCtrlMaxWeight
            // 
            resources.ApplyResources(this.uCtrlMaxWeight, "uCtrlMaxWeight");
            this.uCtrlMaxWeight.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlMaxWeight.Name = "uCtrlMaxWeight";
            this.uCtrlMaxWeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlMaxWeight.Value = ((treeDiM.StackBuilder.Basics.OptDouble)(resources.GetObject("uCtrlMaxWeight.Value")));
            this.uCtrlMaxWeight.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnBoxPropertyChanged);
            // 
            // vcWeight
            // 
            resources.ApplyResources(this.vcWeight, "vcWeight");
            this.vcWeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.vcWeight.Name = "vcWeight";
            this.vcWeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_MASS;
            this.vcWeight.Value = 0D;
            this.vcWeight.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlDouble.ValueChangedDelegate(this.OnBoxPropertyChanged);
            // 
            // uCtrlNetWeight
            // 
            resources.ApplyResources(this.uCtrlNetWeight, "uCtrlNetWeight");
            this.uCtrlNetWeight.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.uCtrlNetWeight.Name = "uCtrlNetWeight";
            this.uCtrlNetWeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_MASS;
            this.uCtrlNetWeight.Value = ((treeDiM.StackBuilder.Basics.OptDouble)(resources.GetObject("uCtrlNetWeight.Value")));
            this.uCtrlNetWeight.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnBoxPropertyChanged);
            // 
            // graphCtrl
            // 
            resources.ApplyResources(this.graphCtrl, "graphCtrl");
            this.graphCtrl.Name = "graphCtrl";
            this.graphCtrl.TabStop = false;
            this.graphCtrl.Viewer = null;
            // 
            // uCtrlTapeWidth
            // 
            resources.ApplyResources(this.uCtrlTapeWidth, "uCtrlTapeWidth");
            this.uCtrlTapeWidth.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlTapeWidth.Name = "uCtrlTapeWidth";
            this.uCtrlTapeWidth.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlTapeWidth.Value = ((treeDiM.StackBuilder.Basics.OptDouble)(resources.GetObject("uCtrlTapeWidth.Value")));
            this.uCtrlTapeWidth.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlOptDouble.ValueChangedDelegate(this.OnTapeWidthChecked);
            // 
            // cbTapeColor
            // 
            resources.ApplyResources(this.cbTapeColor, "cbTapeColor");
            this.cbTapeColor.Color = System.Drawing.Color.Chocolate;
            this.cbTapeColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbTapeColor.DropDownHeight = 1;
            this.cbTapeColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTapeColor.DropDownWidth = 1;
            this.cbTapeColor.Items.AddRange(new object[] {
            resources.GetString("cbTapeColor.Items"),
            resources.GetString("cbTapeColor.Items1"),
            resources.GetString("cbTapeColor.Items2"),
            resources.GetString("cbTapeColor.Items3"),
            resources.GetString("cbTapeColor.Items4"),
            resources.GetString("cbTapeColor.Items5"),
            resources.GetString("cbTapeColor.Items6"),
            resources.GetString("cbTapeColor.Items7"),
            resources.GetString("cbTapeColor.Items8"),
            resources.GetString("cbTapeColor.Items9"),
            resources.GetString("cbTapeColor.Items10"),
            resources.GetString("cbTapeColor.Items11"),
            resources.GetString("cbTapeColor.Items12"),
            resources.GetString("cbTapeColor.Items13"),
            resources.GetString("cbTapeColor.Items14"),
            resources.GetString("cbTapeColor.Items15"),
            resources.GetString("cbTapeColor.Items16"),
            resources.GetString("cbTapeColor.Items17"),
            resources.GetString("cbTapeColor.Items18"),
            resources.GetString("cbTapeColor.Items19"),
            resources.GetString("cbTapeColor.Items20"),
            resources.GetString("cbTapeColor.Items21"),
            resources.GetString("cbTapeColor.Items22"),
            resources.GetString("cbTapeColor.Items23"),
            resources.GetString("cbTapeColor.Items24"),
            resources.GetString("cbTapeColor.Items25"),
            resources.GetString("cbTapeColor.Items26"),
            resources.GetString("cbTapeColor.Items27"),
            resources.GetString("cbTapeColor.Items28"),
            resources.GetString("cbTapeColor.Items29"),
            resources.GetString("cbTapeColor.Items30"),
            resources.GetString("cbTapeColor.Items31"),
            resources.GetString("cbTapeColor.Items32"),
            resources.GetString("cbTapeColor.Items33"),
            resources.GetString("cbTapeColor.Items34"),
            resources.GetString("cbTapeColor.Items35"),
            resources.GetString("cbTapeColor.Items36"),
            resources.GetString("cbTapeColor.Items37"),
            resources.GetString("cbTapeColor.Items38"),
            resources.GetString("cbTapeColor.Items39"),
            resources.GetString("cbTapeColor.Items40"),
            resources.GetString("cbTapeColor.Items41"),
            resources.GetString("cbTapeColor.Items42"),
            resources.GetString("cbTapeColor.Items43"),
            resources.GetString("cbTapeColor.Items44"),
            resources.GetString("cbTapeColor.Items45"),
            resources.GetString("cbTapeColor.Items46"),
            resources.GetString("cbTapeColor.Items47"),
            resources.GetString("cbTapeColor.Items48"),
            resources.GetString("cbTapeColor.Items49"),
            resources.GetString("cbTapeColor.Items50"),
            resources.GetString("cbTapeColor.Items51"),
            resources.GetString("cbTapeColor.Items52"),
            resources.GetString("cbTapeColor.Items53"),
            resources.GetString("cbTapeColor.Items54"),
            resources.GetString("cbTapeColor.Items55"),
            resources.GetString("cbTapeColor.Items56"),
            resources.GetString("cbTapeColor.Items57"),
            resources.GetString("cbTapeColor.Items58"),
            resources.GetString("cbTapeColor.Items59"),
            resources.GetString("cbTapeColor.Items60"),
            resources.GetString("cbTapeColor.Items61"),
            resources.GetString("cbTapeColor.Items62"),
            resources.GetString("cbTapeColor.Items63"),
            resources.GetString("cbTapeColor.Items64"),
            resources.GetString("cbTapeColor.Items65"),
            resources.GetString("cbTapeColor.Items66"),
            resources.GetString("cbTapeColor.Items67"),
            resources.GetString("cbTapeColor.Items68"),
            resources.GetString("cbTapeColor.Items69"),
            resources.GetString("cbTapeColor.Items70"),
            resources.GetString("cbTapeColor.Items71"),
            resources.GetString("cbTapeColor.Items72"),
            resources.GetString("cbTapeColor.Items73"),
            resources.GetString("cbTapeColor.Items74"),
            resources.GetString("cbTapeColor.Items75"),
            resources.GetString("cbTapeColor.Items76"),
            resources.GetString("cbTapeColor.Items77"),
            resources.GetString("cbTapeColor.Items78"),
            resources.GetString("cbTapeColor.Items79"),
            resources.GetString("cbTapeColor.Items80"),
            resources.GetString("cbTapeColor.Items81"),
            resources.GetString("cbTapeColor.Items82"),
            resources.GetString("cbTapeColor.Items83"),
            resources.GetString("cbTapeColor.Items84"),
            resources.GetString("cbTapeColor.Items85"),
            resources.GetString("cbTapeColor.Items86"),
            resources.GetString("cbTapeColor.Items87"),
            resources.GetString("cbTapeColor.Items88"),
            resources.GetString("cbTapeColor.Items89"),
            resources.GetString("cbTapeColor.Items90"),
            resources.GetString("cbTapeColor.Items91"),
            resources.GetString("cbTapeColor.Items92"),
            resources.GetString("cbTapeColor.Items93"),
            resources.GetString("cbTapeColor.Items94"),
            resources.GetString("cbTapeColor.Items95"),
            resources.GetString("cbTapeColor.Items96"),
            resources.GetString("cbTapeColor.Items97"),
            resources.GetString("cbTapeColor.Items98"),
            resources.GetString("cbTapeColor.Items99"),
            resources.GetString("cbTapeColor.Items100"),
            resources.GetString("cbTapeColor.Items101"),
            resources.GetString("cbTapeColor.Items102"),
            resources.GetString("cbTapeColor.Items103"),
            resources.GetString("cbTapeColor.Items104"),
            resources.GetString("cbTapeColor.Items105"),
            resources.GetString("cbTapeColor.Items106"),
            resources.GetString("cbTapeColor.Items107"),
            resources.GetString("cbTapeColor.Items108"),
            resources.GetString("cbTapeColor.Items109"),
            resources.GetString("cbTapeColor.Items110"),
            resources.GetString("cbTapeColor.Items111"),
            resources.GetString("cbTapeColor.Items112"),
            resources.GetString("cbTapeColor.Items113"),
            resources.GetString("cbTapeColor.Items114"),
            resources.GetString("cbTapeColor.Items115"),
            resources.GetString("cbTapeColor.Items116"),
            resources.GetString("cbTapeColor.Items117"),
            resources.GetString("cbTapeColor.Items118"),
            resources.GetString("cbTapeColor.Items119"),
            resources.GetString("cbTapeColor.Items120"),
            resources.GetString("cbTapeColor.Items121"),
            resources.GetString("cbTapeColor.Items122"),
            resources.GetString("cbTapeColor.Items123")});
            this.cbTapeColor.Name = "cbTapeColor";
            this.cbTapeColor.SelectedColorChanged += new System.EventHandler(this.OnFaceColorChanged);
            // 
            // lbTapeColor
            // 
            resources.ApplyResources(this.lbTapeColor, "lbTapeColor");
            this.lbTapeColor.Name = "lbTapeColor";
            // 
            // bnSaveToDB
            // 
            resources.ApplyResources(this.bnSaveToDB, "bnSaveToDB");
            this.bnSaveToDB.Name = "bnSaveToDB";
            this.bnSaveToDB.UseVisualStyleBackColor = true;
            this.bnSaveToDB.Click += new System.EventHandler(this.OnSaveToDatabase);
            // 
            // tabCtrl
            // 
            resources.ApplyResources(this.tabCtrl, "tabCtrl");
            this.tabCtrl.Controls.Add(this.tabPageTape);
            this.tabCtrl.Controls.Add(this.tabPageStrappers);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            // 
            // tabPageTape
            // 
            resources.ApplyResources(this.tabPageTape, "tabPageTape");
            this.tabPageTape.Controls.Add(this.cbTapeColor);
            this.tabPageTape.Controls.Add(this.uCtrlTapeWidth);
            this.tabPageTape.Controls.Add(this.lbTapeColor);
            this.tabPageTape.Name = "tabPageTape";
            this.tabPageTape.UseVisualStyleBackColor = true;
            // 
            // tabPageStrappers
            // 
            resources.ApplyResources(this.tabPageStrappers, "tabPageStrappers");
            this.tabPageStrappers.Controls.Add(this.ctrlStrapperSet);
            this.tabPageStrappers.Name = "tabPageStrappers";
            this.tabPageStrappers.UseVisualStyleBackColor = true;
            // 
            // ctrlStrapperSet
            // 
            resources.ApplyResources(this.ctrlStrapperSet, "ctrlStrapperSet");
            this.ctrlStrapperSet.Name = "ctrlStrapperSet";
            this.ctrlStrapperSet.Number = 0;
            this.ctrlStrapperSet.StrapperSet = null;
            // 
            // FormNewBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabCtrl);
            this.Controls.Add(this.bnSaveToDB);
            this.Controls.Add(this.graphCtrl);
            this.Controls.Add(this.gbWeight);
            this.Controls.Add(this.gbFaceColor);
            this.Controls.Add(this.gbDimensions);
            this.Name = "FormNewBox";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Controls.SetChildIndex(this.gbDimensions, 0);
            this.Controls.SetChildIndex(this.gbFaceColor, 0);
            this.Controls.SetChildIndex(this.gbWeight, 0);
            this.Controls.SetChildIndex(this.graphCtrl, 0);
            this.Controls.SetChildIndex(this.bnSaveToDB, 0);
            this.Controls.SetChildIndex(this.bnOk, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.lbName, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbDescription, 0);
            this.Controls.SetChildIndex(this.tabCtrl, 0);
            this.gbDimensions.ResumeLayout(false);
            this.gbFaceColor.ResumeLayout(false);
            this.gbFaceColor.PerformLayout();
            this.gbWeight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.graphCtrl)).EndInit();
            this.tabCtrl.ResumeLayout(false);
            this.tabPageTape.ResumeLayout(false);
            this.tabPageTape.PerformLayout();
            this.tabPageStrappers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbFace;
        private System.Windows.Forms.GroupBox gbDimensions;
        private System.Windows.Forms.ComboBox cbFace;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbColor;
        private System.Windows.Forms.GroupBox gbFaceColor;
        private System.Windows.Forms.GroupBox gbWeight;
        private treeDiM.StackBuilder.Graphics.Graphics3DControl graphCtrl;
        private System.Windows.Forms.CheckBox chkAllFaces;
        private System.Windows.Forms.Button btBitmaps;
        private OfficePickers.ColorPicker.ComboBoxColorPicker cbTapeColor;
        private System.Windows.Forms.Label lbTapeColor;
        private Basics.UCtrlOptDouble uCtrlNetWeight;
        private Basics.UCtrlDouble vcWeight;
        private Basics.UCtrlTriDouble uCtrlDimensionsOuter;
        private Basics.UCtrlOptTriDouble uCtrlDimensionsInner;
        private Basics.UCtrlOptDouble uCtrlTapeWidth;
        private System.Windows.Forms.Button bnSaveToDB;
        private Basics.UCtrlOptDouble uCtrlMaxWeight;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tabPageTape;
        private System.Windows.Forms.TabPage tabPageStrappers;
        private Basics.Controls.CtrlStrapperSet ctrlStrapperSet;
    }
}