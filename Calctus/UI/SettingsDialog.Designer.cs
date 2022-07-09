namespace Shapoco.Calctus.UI {
    partial class SettingsDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.closeButton = new System.Windows.Forms.Button();
            this.NumberFormat_Exp_Enabled = new System.Windows.Forms.CheckBox();
            this.eNoteGroup = new System.Windows.Forms.GroupBox();
            this.NumberFormat_Exp_Alignment = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NumberFormat_Exp_NegativeMax = new System.Windows.Forms.NumericUpDown();
            this.NumberFormat_Exp_PositiveMin = new System.Windows.Forms.NumericUpDown();
            this.eNoteGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_NegativeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_PositiveMin)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(123, 125);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(89, 25);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // NumberFormat_Exp_Enabled
            // 
            this.NumberFormat_Exp_Enabled.AutoSize = true;
            this.NumberFormat_Exp_Enabled.Checked = true;
            this.NumberFormat_Exp_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NumberFormat_Exp_Enabled.Location = new System.Drawing.Point(16, 12);
            this.NumberFormat_Exp_Enabled.Name = "NumberFormat_Exp_Enabled";
            this.NumberFormat_Exp_Enabled.Size = new System.Drawing.Size(78, 16);
            this.NumberFormat_Exp_Enabled.TabIndex = 3;
            this.NumberFormat_Exp_Enabled.Text = "E Notation";
            this.NumberFormat_Exp_Enabled.UseVisualStyleBackColor = true;
            // 
            // eNoteGroup
            // 
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_Alignment);
            this.eNoteGroup.Controls.Add(this.label2);
            this.eNoteGroup.Controls.Add(this.label1);
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_NegativeMax);
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_PositiveMin);
            this.eNoteGroup.Location = new System.Drawing.Point(12, 12);
            this.eNoteGroup.Name = "eNoteGroup";
            this.eNoteGroup.Size = new System.Drawing.Size(200, 107);
            this.eNoteGroup.TabIndex = 7;
            this.eNoteGroup.TabStop = false;
            this.eNoteGroup.Text = "groupBox1";
            // 
            // NumberFormat_Exp_Alignment
            // 
            this.NumberFormat_Exp_Alignment.AutoSize = true;
            this.NumberFormat_Exp_Alignment.Location = new System.Drawing.Point(22, 76);
            this.NumberFormat_Exp_Alignment.Name = "NumberFormat_Exp_Alignment";
            this.NumberFormat_Exp_Alignment.Size = new System.Drawing.Size(164, 16);
            this.NumberFormat_Exp_Alignment.TabIndex = 11;
            this.NumberFormat_Exp_Alignment.Text = "Align to multiple of 3 digits";
            this.NumberFormat_Exp_Alignment.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "log10(x) ≧";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "log10(x) ≦";
            // 
            // NumberFormat_Exp_NegativeMax
            // 
            this.NumberFormat_Exp_NegativeMax.Location = new System.Drawing.Point(88, 48);
            this.NumberFormat_Exp_NegativeMax.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.NumberFormat_Exp_NegativeMax.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            -2147483648});
            this.NumberFormat_Exp_NegativeMax.Name = "NumberFormat_Exp_NegativeMax";
            this.NumberFormat_Exp_NegativeMax.Size = new System.Drawing.Size(65, 19);
            this.NumberFormat_Exp_NegativeMax.TabIndex = 8;
            this.NumberFormat_Exp_NegativeMax.Value = new decimal(new int[] {
            6,
            0,
            0,
            -2147483648});
            // 
            // NumberFormat_Exp_PositiveMin
            // 
            this.NumberFormat_Exp_PositiveMin.Location = new System.Drawing.Point(88, 23);
            this.NumberFormat_Exp_PositiveMin.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.NumberFormat_Exp_PositiveMin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumberFormat_Exp_PositiveMin.Name = "NumberFormat_Exp_PositiveMin";
            this.NumberFormat_Exp_PositiveMin.Size = new System.Drawing.Size(65, 19);
            this.NumberFormat_Exp_PositiveMin.TabIndex = 7;
            this.NumberFormat_Exp_PositiveMin.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 163);
            this.Controls.Add(this.NumberFormat_Exp_Enabled);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.eNoteGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SettingsDialog";
            this.eNoteGroup.ResumeLayout(false);
            this.eNoteGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_NegativeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_PositiveMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckBox NumberFormat_Exp_Enabled;
        private System.Windows.Forms.GroupBox eNoteGroup;
        private System.Windows.Forms.CheckBox NumberFormat_Exp_Alignment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NumberFormat_Exp_NegativeMax;
        private System.Windows.Forms.NumericUpDown NumberFormat_Exp_PositiveMin;
    }
}