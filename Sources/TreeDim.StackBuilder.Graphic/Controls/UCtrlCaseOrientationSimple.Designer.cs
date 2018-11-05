namespace treeDiM.StackBuilder.Graphics.Controls
{
    partial class UCtrlCaseOrientationSimple
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkbX = new System.Windows.Forms.CheckBox();
            this.chkbY = new System.Windows.Forms.CheckBox();
            this.chkbZ = new System.Windows.Forms.CheckBox();
            this.lbAllowedOrientations = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkbX
            // 
            this.chkbX.AutoSize = true;
            this.chkbX.Location = new System.Drawing.Point(183, 2);
            this.chkbX.Name = "chkbX";
            this.chkbX.Size = new System.Drawing.Size(33, 17);
            this.chkbX.TabIndex = 0;
            this.chkbX.Text = "X";
            this.chkbX.UseVisualStyleBackColor = true;
            this.chkbX.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // chkbY
            // 
            this.chkbY.AutoSize = true;
            this.chkbY.Location = new System.Drawing.Point(223, 2);
            this.chkbY.Name = "chkbY";
            this.chkbY.Size = new System.Drawing.Size(33, 17);
            this.chkbY.TabIndex = 1;
            this.chkbY.Text = "Y";
            this.chkbY.UseVisualStyleBackColor = true;
            this.chkbY.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // chkbZ
            // 
            this.chkbZ.AutoSize = true;
            this.chkbZ.Location = new System.Drawing.Point(263, 2);
            this.chkbZ.Name = "chkbZ";
            this.chkbZ.Size = new System.Drawing.Size(33, 17);
            this.chkbZ.TabIndex = 2;
            this.chkbZ.Text = "Z";
            this.chkbZ.UseVisualStyleBackColor = true;
            this.chkbZ.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // lbAllowedOrientations
            // 
            this.lbAllowedOrientations.AutoSize = true;
            this.lbAllowedOrientations.Location = new System.Drawing.Point(0, 2);
            this.lbAllowedOrientations.Name = "lbAllowedOrientations";
            this.lbAllowedOrientations.Size = new System.Drawing.Size(127, 13);
            this.lbAllowedOrientations.TabIndex = 3;
            this.lbAllowedOrientations.Text = "Allowed case orientations";
            // 
            // UCtrlCaseOrientationSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbAllowedOrientations);
            this.Controls.Add(this.chkbZ);
            this.Controls.Add(this.chkbY);
            this.Controls.Add(this.chkbX);
            this.Name = "UCtrlCaseOrientationSimple";
            this.Size = new System.Drawing.Size(300, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkbX;
        private System.Windows.Forms.CheckBox chkbY;
        private System.Windows.Forms.CheckBox chkbZ;
        private System.Windows.Forms.Label lbAllowedOrientations;
    }
}
