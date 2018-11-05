namespace treeDiM.StackBuilder.Basics.Controls
{
    partial class FormValueArray
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
            this.dataGridArray = new System.Windows.Forms.DataGridView();
            this.ColIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridArray)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridArray
            // 
            this.dataGridArray.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridArray.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColIndex,
            this.ColValue});
            this.dataGridArray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridArray.Location = new System.Drawing.Point(0, 0);
            this.dataGridArray.Name = "dataGridArray";
            this.dataGridArray.Size = new System.Drawing.Size(157, 204);
            this.dataGridArray.TabIndex = 0;
            // 
            // ColIndex
            // 
            this.ColIndex.HeaderText = "#";
            this.ColIndex.Name = "ColIndex";
            this.ColIndex.Width = 30;
            // 
            // ColValue
            // 
            this.ColValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColValue.HeaderText = "Value";
            this.ColValue.Name = "ColValue";
            this.ColValue.Width = 59;
            // 
            // FormDoubleArray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(157, 204);
            this.Controls.Add(this.dataGridArray);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDoubleArray";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Array...";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridArray)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridArray;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColValue;
    }
}