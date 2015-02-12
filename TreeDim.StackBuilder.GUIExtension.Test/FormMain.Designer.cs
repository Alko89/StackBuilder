﻿namespace TreeDim.StackBuilder.GUIExtension.Test
{
    partial class FormMain
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
            this.bnCasePalletAnalysis = new System.Windows.Forms.Button();
            this.bnBoxCasePalletOptimization = new System.Windows.Forms.Button();
            this.bnBundlePalletAnalysis = new System.Windows.Forms.Button();
            this.bnBundleCaseAnalysis = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bnCasePalletAnalysis
            // 
            this.bnCasePalletAnalysis.Location = new System.Drawing.Point(16, 13);
            this.bnCasePalletAnalysis.Name = "bnCasePalletAnalysis";
            this.bnCasePalletAnalysis.Size = new System.Drawing.Size(178, 22);
            this.bnCasePalletAnalysis.TabIndex = 0;
            this.bnCasePalletAnalysis.Text = "Case / Pallet analysis";
            this.bnCasePalletAnalysis.UseVisualStyleBackColor = true;
            this.bnCasePalletAnalysis.Click += new System.EventHandler(this.bnCasePalletAnalysis_Click);
            // 
            // bnBoxCasePalletOptimization
            // 
            this.bnBoxCasePalletOptimization.Location = new System.Drawing.Point(16, 39);
            this.bnBoxCasePalletOptimization.Name = "bnBoxCasePalletOptimization";
            this.bnBoxCasePalletOptimization.Size = new System.Drawing.Size(178, 22);
            this.bnBoxCasePalletOptimization.TabIndex = 1;
            this.bnBoxCasePalletOptimization.Text = "Box / Case / Pallet optimization ";
            this.bnBoxCasePalletOptimization.UseVisualStyleBackColor = true;
            this.bnBoxCasePalletOptimization.Click += new System.EventHandler(this.bnBoxCasePalletOptimization_Click);
            // 
            // bnBundlePalletAnalysis
            // 
            this.bnBundlePalletAnalysis.Location = new System.Drawing.Point(16, 66);
            this.bnBundlePalletAnalysis.Name = "bnBundlePalletAnalysis";
            this.bnBundlePalletAnalysis.Size = new System.Drawing.Size(178, 23);
            this.bnBundlePalletAnalysis.TabIndex = 2;
            this.bnBundlePalletAnalysis.Text = "Bundle / Pallet analysis";
            this.bnBundlePalletAnalysis.UseVisualStyleBackColor = true;
            this.bnBundlePalletAnalysis.Click += new System.EventHandler(this.bnBundlePalletAnalysis_Click);
            // 
            // bnBundleCaseAnalysis
            // 
            this.bnBundleCaseAnalysis.Location = new System.Drawing.Point(16, 95);
            this.bnBundleCaseAnalysis.Name = "bnBundleCaseAnalysis";
            this.bnBundleCaseAnalysis.Size = new System.Drawing.Size(178, 23);
            this.bnBundleCaseAnalysis.TabIndex = 3;
            this.bnBundleCaseAnalysis.Text = "Bundle / Case analysis";
            this.bnBundleCaseAnalysis.UseVisualStyleBackColor = true;
            this.bnBundleCaseAnalysis.Click += new System.EventHandler(this.bnBundleCaseAnalysis_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.bnBundleCaseAnalysis);
            this.Controls.Add(this.bnBundlePalletAnalysis);
            this.Controls.Add(this.bnBoxCasePalletOptimization);
            this.Controls.Add(this.bnCasePalletAnalysis);
            this.Name = "FormMain";
            this.Text = "GUIExtension.Test";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnCasePalletAnalysis;
        private System.Windows.Forms.Button bnBoxCasePalletOptimization;
        private System.Windows.Forms.Button bnBundlePalletAnalysis;
        private System.Windows.Forms.Button bnBundleCaseAnalysis;
    }
}

