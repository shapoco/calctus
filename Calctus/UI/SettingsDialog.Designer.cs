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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.fontGroup = new System.Windows.Forms.GroupBox();
            this.Appearance_Font_Size = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.Appearance_Font_Bold = new System.Windows.Forms.CheckBox();
            this.Appearance_Font_Button_Name = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Appearance_Font_Expr_Name = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.Hotkey_Enabled = new System.Windows.Forms.CheckBox();
            this.Hotkey_KeyCode = new Shapoco.Calctus.UI.KeyCodeBox();
            this.eNoteGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_NegativeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_PositiveMin)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.fontGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Appearance_Font_Size)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(240, 186);
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
            this.NumberFormat_Exp_Enabled.Location = new System.Drawing.Point(8, 18);
            this.NumberFormat_Exp_Enabled.Name = "NumberFormat_Exp_Enabled";
            this.NumberFormat_Exp_Enabled.Size = new System.Drawing.Size(58, 16);
            this.NumberFormat_Exp_Enabled.TabIndex = 3;
            this.NumberFormat_Exp_Enabled.Text = "Enable";
            this.NumberFormat_Exp_Enabled.UseVisualStyleBackColor = true;
            // 
            // eNoteGroup
            // 
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_Enabled);
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_Alignment);
            this.eNoteGroup.Controls.Add(this.label2);
            this.eNoteGroup.Controls.Add(this.label1);
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_NegativeMax);
            this.eNoteGroup.Controls.Add(this.NumberFormat_Exp_PositiveMin);
            this.eNoteGroup.Location = new System.Drawing.Point(6, 6);
            this.eNoteGroup.Name = "eNoteGroup";
            this.eNoteGroup.Size = new System.Drawing.Size(297, 130);
            this.eNoteGroup.TabIndex = 7;
            this.eNoteGroup.TabStop = false;
            this.eNoteGroup.Text = "Exponential Notation";
            // 
            // NumberFormat_Exp_Alignment
            // 
            this.NumberFormat_Exp_Alignment.AutoSize = true;
            this.NumberFormat_Exp_Alignment.Location = new System.Drawing.Point(18, 97);
            this.NumberFormat_Exp_Alignment.Name = "NumberFormat_Exp_Alignment";
            this.NumberFormat_Exp_Alignment.Size = new System.Drawing.Size(164, 16);
            this.NumberFormat_Exp_Alignment.TabIndex = 11;
            this.NumberFormat_Exp_Alignment.Text = "Align to multiple of 3 digits";
            this.NumberFormat_Exp_Alignment.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "log10(x) ≧";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "log10(x) ≦";
            // 
            // NumberFormat_Exp_NegativeMax
            // 
            this.NumberFormat_Exp_NegativeMax.Location = new System.Drawing.Point(84, 69);
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
            this.NumberFormat_Exp_PositiveMin.Location = new System.Drawing.Point(84, 44);
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
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(317, 168);
            this.tabControl.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.eNoteGroup);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(309, 142);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.fontGroup);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(309, 142);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Appearance";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // fontGroup
            // 
            this.fontGroup.Controls.Add(this.Appearance_Font_Size);
            this.fontGroup.Controls.Add(this.label5);
            this.fontGroup.Controls.Add(this.label4);
            this.fontGroup.Controls.Add(this.label3);
            this.fontGroup.Controls.Add(this.Appearance_Font_Bold);
            this.fontGroup.Controls.Add(this.Appearance_Font_Expr_Name);
            this.fontGroup.Controls.Add(this.Appearance_Font_Button_Name);
            this.fontGroup.Location = new System.Drawing.Point(6, 6);
            this.fontGroup.Name = "fontGroup";
            this.fontGroup.Size = new System.Drawing.Size(297, 104);
            this.fontGroup.TabIndex = 0;
            this.fontGroup.TabStop = false;
            this.fontGroup.Text = "Font";
            // 
            // Appearance_Font_Size
            // 
            this.Appearance_Font_Size.Location = new System.Drawing.Point(81, 70);
            this.Appearance_Font_Size.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Appearance_Font_Size.Name = "Appearance_Font_Size";
            this.Appearance_Font_Size.Size = new System.Drawing.Size(65, 19);
            this.Appearance_Font_Size.TabIndex = 8;
            this.Appearance_Font_Size.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Size:";
            // 
            // Appearance_Font_Bold
            // 
            this.Appearance_Font_Bold.AutoSize = true;
            this.Appearance_Font_Bold.Location = new System.Drawing.Point(244, 71);
            this.Appearance_Font_Bold.Name = "Appearance_Font_Bold";
            this.Appearance_Font_Bold.Size = new System.Drawing.Size(47, 16);
            this.Appearance_Font_Bold.TabIndex = 1;
            this.Appearance_Font_Bold.Text = "Bold";
            this.Appearance_Font_Bold.UseVisualStyleBackColor = true;
            // 
            // Appearance_Font_Button_Name
            // 
            this.Appearance_Font_Button_Name.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Appearance_Font_Button_Name.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Appearance_Font_Button_Name.FormattingEnabled = true;
            this.Appearance_Font_Button_Name.Location = new System.Drawing.Point(81, 18);
            this.Appearance_Font_Button_Name.Name = "Appearance_Font_Button_Name";
            this.Appearance_Font_Button_Name.Size = new System.Drawing.Size(210, 20);
            this.Appearance_Font_Button_Name.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Buttons:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Expressions:";
            // 
            // Appearance_Font_Expr_Name
            // 
            this.Appearance_Font_Expr_Name.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Appearance_Font_Expr_Name.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Appearance_Font_Expr_Name.FormattingEnabled = true;
            this.Appearance_Font_Expr_Name.Location = new System.Drawing.Point(81, 44);
            this.Appearance_Font_Expr_Name.Name = "Appearance_Font_Expr_Name";
            this.Appearance_Font_Expr_Name.Size = new System.Drawing.Size(210, 20);
            this.Appearance_Font_Expr_Name.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Hotkey_Enabled);
            this.tabPage3.Controls.Add(this.Hotkey_KeyCode);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(309, 142);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "General";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Hotkey_Enabled
            // 
            this.Hotkey_Enabled.AutoSize = true;
            this.Hotkey_Enabled.Location = new System.Drawing.Point(13, 12);
            this.Hotkey_Enabled.Name = "Hotkey_Enabled";
            this.Hotkey_Enabled.Size = new System.Drawing.Size(98, 16);
            this.Hotkey_Enabled.TabIndex = 4;
            this.Hotkey_Enabled.Text = "Enable Hotkey";
            this.Hotkey_Enabled.UseVisualStyleBackColor = true;
            // 
            // Hotkey_KeyCode
            // 
            this.Hotkey_KeyCode.Alt = false;
            this.Hotkey_KeyCode.Ctrl = false;
            this.Hotkey_KeyCode.KeyCode = System.Windows.Forms.Keys.None;
            this.Hotkey_KeyCode.Location = new System.Drawing.Point(46, 34);
            this.Hotkey_KeyCode.Name = "Hotkey_KeyCode";
            this.Hotkey_KeyCode.Shift = false;
            this.Hotkey_KeyCode.Size = new System.Drawing.Size(260, 19);
            this.Hotkey_KeyCode.TabIndex = 1;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 223);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.eNoteGroup.ResumeLayout(false);
            this.eNoteGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_NegativeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_PositiveMin)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.fontGroup.ResumeLayout(false);
            this.fontGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Appearance_Font_Size)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox fontGroup;
        private System.Windows.Forms.NumericUpDown Appearance_Font_Size;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox Appearance_Font_Bold;
        private System.Windows.Forms.ComboBox Appearance_Font_Button_Name;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox Appearance_Font_Expr_Name;
        private System.Windows.Forms.TabPage tabPage3;
        private KeyCodeBox Hotkey_KeyCode;
        private System.Windows.Forms.CheckBox Hotkey_Enabled;
    }
}