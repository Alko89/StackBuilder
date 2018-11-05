namespace treeDiM.StackBuilder.Desktop
{
    partial class FormNewHAnalysisCasePallet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewHAnalysisCasePallet));
            this.lbPallet = new System.Windows.Forms.Label();
            this.uCtrlPalletHeight = new treeDiM.StackBuilder.Basics.UCtrlDouble();
            this.cbPallets = new treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz1)).BeginInit();
            this.splitContainerHoriz1.Panel1.SuspendLayout();
            this.splitContainerHoriz1.Panel2.SuspendLayout();
            this.splitContainerHoriz1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz2)).BeginInit();
            this.splitContainerHoriz2.Panel1.SuspendLayout();
            this.splitContainerHoriz2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).BeginInit();
            this.splitContainerVert.Panel1.SuspendLayout();
            this.splitContainerVert.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHoriz1
            // 
            // 
            // splitContainerHoriz2
            // 
            // 
            // splitContainerVert
            // 
            // 
            // splitContainerVert.Panel1
            // 
            this.splitContainerVert.Panel1.Controls.Add(this.cbPallets);
            this.splitContainerVert.Panel1.Controls.Add(this.uCtrlPalletHeight);
            this.splitContainerVert.Panel1.Controls.Add(this.lbPallet);
            // 
            // gridContent
            // 
            this.gridContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridContent.ColumnsCount = 2;
            this.gridContent.FixedRows = 1;
            this.gridContent.RowsCount = 1;
            // 
            // lbPallet
            // 
            resources.ApplyResources(this.lbPallet, "lbPallet");
            this.lbPallet.Name = "lbPallet";
            // 
            // uCtrlPalletHeight
            // 
            resources.ApplyResources(this.uCtrlPalletHeight, "uCtrlPalletHeight");
            this.uCtrlPalletHeight.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uCtrlPalletHeight.Name = "uCtrlPalletHeight";
            this.uCtrlPalletHeight.Unit = treeDiM.StackBuilder.Basics.UnitsManager.UnitType.UT_LENGTH;
            this.uCtrlPalletHeight.Value = 0D;
            this.uCtrlPalletHeight.ValueChanged += new treeDiM.StackBuilder.Basics.UCtrlDouble.ValueChangedDelegate(this.OnDataModifiedOverride);
            // 
            // cbPallets
            // 
            resources.ApplyResources(this.cbPallets, "cbPallets");
            this.cbPallets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPallets.FormattingEnabled = true;
            this.cbPallets.Name = "cbPallets";
            this.cbPallets.SelectedIndexChanged += new System.EventHandler(this.OnDataModifiedOverride);
            // 
            // FormNewHAnalysisCasePallet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "FormNewHAnalysisCasePallet";
            this.splitContainerHoriz1.Panel1.ResumeLayout(false);
            this.splitContainerHoriz1.Panel1.PerformLayout();
            this.splitContainerHoriz1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz1)).EndInit();
            this.splitContainerHoriz1.ResumeLayout(false);
            this.splitContainerHoriz2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHoriz2)).EndInit();
            this.splitContainerHoriz2.ResumeLayout(false);
            this.splitContainerVert.Panel1.ResumeLayout(false);
            this.splitContainerVert.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerVert)).EndInit();
            this.splitContainerVert.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private treeDiM.StackBuilder.Graphics.Controls.CCtrlComboFiltered cbPallets;
        private Basics.UCtrlDouble uCtrlPalletHeight;
        private System.Windows.Forms.Label lbPallet;
    }
}