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
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Startup_AutoStart = new System.Windows.Forms.CheckBox();
            this.Startup_TrayIcon = new System.Windows.Forms.CheckBox();
            this.Window_RememberPosition = new System.Windows.Forms.CheckBox();
            this.Hotkey_Enabled = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Input_AutoCloseBrackets = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.fontGroup = new System.Windows.Forms.GroupBox();
            this.Appearance_Font_Size = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Appearance_Font_Bold = new System.Windows.Forms.CheckBox();
            this.Appearance_Font_Expr_Name = new System.Windows.Forms.ComboBox();
            this.Appearance_Font_Button_Name = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NumberFormat_Decimal_MaxLen = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.Hotkey_KeyCode = new Shapoco.Calctus.UI.KeyCodeBox();
            this.Input_AutoInputAns = new System.Windows.Forms.CheckBox();
            this.eNoteGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_NegativeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Exp_PositiveMin)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.fontGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Appearance_Font_Size)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Decimal_MaxLen)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(240, 234);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(89, 25);
            this.closeButton.TabIndex = 1;
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
            this.NumberFormat_Exp_Enabled.TabIndex = 0;
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
            this.eNoteGroup.Location = new System.Drawing.Point(6, 60);
            this.eNoteGroup.Name = "eNoteGroup";
            this.eNoteGroup.Size = new System.Drawing.Size(297, 126);
            this.eNoteGroup.TabIndex = 1;
            this.eNoteGroup.TabStop = false;
            this.eNoteGroup.Text = "Exponential Notation";
            // 
            // NumberFormat_Exp_Alignment
            // 
            this.NumberFormat_Exp_Alignment.AutoSize = true;
            this.NumberFormat_Exp_Alignment.Location = new System.Drawing.Point(18, 97);
            this.NumberFormat_Exp_Alignment.Name = "NumberFormat_Exp_Alignment";
            this.NumberFormat_Exp_Alignment.Size = new System.Drawing.Size(138, 16);
            this.NumberFormat_Exp_Alignment.TabIndex = 5;
            this.NumberFormat_Exp_Alignment.Text = "Engineering Alignment";
            this.NumberFormat_Exp_Alignment.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "log10(x) ≧";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 3;
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
            this.NumberFormat_Exp_NegativeMax.TabIndex = 4;
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
            this.NumberFormat_Exp_PositiveMin.TabIndex = 2;
            this.NumberFormat_Exp_PositiveMin.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(317, 216);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(309, 190);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "General";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Startup_AutoStart);
            this.groupBox1.Controls.Add(this.Startup_TrayIcon);
            this.groupBox1.Controls.Add(this.Hotkey_KeyCode);
            this.groupBox1.Controls.Add(this.Window_RememberPosition);
            this.groupBox1.Controls.Add(this.Hotkey_Enabled);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 140);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Start";
            // 
            // Startup_AutoStart
            // 
            this.Startup_AutoStart.AutoSize = true;
            this.Startup_AutoStart.Location = new System.Drawing.Point(10, 20);
            this.Startup_AutoStart.Name = "Startup_AutoStart";
            this.Startup_AutoStart.Size = new System.Drawing.Size(121, 16);
            this.Startup_AutoStart.TabIndex = 0;
            this.Startup_AutoStart.Text = "Start automatically";
            this.Startup_AutoStart.UseVisualStyleBackColor = true;
            // 
            // Startup_TrayIcon
            // 
            this.Startup_TrayIcon.AutoSize = true;
            this.Startup_TrayIcon.Location = new System.Drawing.Point(10, 42);
            this.Startup_TrayIcon.Name = "Startup_TrayIcon";
            this.Startup_TrayIcon.Size = new System.Drawing.Size(122, 16);
            this.Startup_TrayIcon.TabIndex = 1;
            this.Startup_TrayIcon.Text = "Reside in task tray";
            this.Startup_TrayIcon.UseVisualStyleBackColor = true;
            // 
            // Window_RememberPosition
            // 
            this.Window_RememberPosition.AutoSize = true;
            this.Window_RememberPosition.Location = new System.Drawing.Point(10, 108);
            this.Window_RememberPosition.Name = "Window_RememberPosition";
            this.Window_RememberPosition.Size = new System.Drawing.Size(163, 16);
            this.Window_RememberPosition.TabIndex = 4;
            this.Window_RememberPosition.Text = "Remember window position";
            this.Window_RememberPosition.UseVisualStyleBackColor = true;
            // 
            // Hotkey_Enabled
            // 
            this.Hotkey_Enabled.AutoSize = true;
            this.Hotkey_Enabled.Location = new System.Drawing.Point(10, 64);
            this.Hotkey_Enabled.Name = "Hotkey_Enabled";
            this.Hotkey_Enabled.Size = new System.Drawing.Size(60, 16);
            this.Hotkey_Enabled.TabIndex = 2;
            this.Hotkey_Enabled.Text = "Hotkey";
            this.Hotkey_Enabled.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox3);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(309, 190);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Input";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Input_AutoInputAns);
            this.groupBox3.Controls.Add(this.Input_AutoCloseBrackets);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(297, 66);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Auto Input";
            // 
            // Input_AutoCloseBrackets
            // 
            this.Input_AutoCloseBrackets.AutoSize = true;
            this.Input_AutoCloseBrackets.Location = new System.Drawing.Point(6, 18);
            this.Input_AutoCloseBrackets.Name = "Input_AutoCloseBrackets";
            this.Input_AutoCloseBrackets.Size = new System.Drawing.Size(127, 16);
            this.Input_AutoCloseBrackets.TabIndex = 0;
            this.Input_AutoCloseBrackets.Text = "Auto close brackets";
            this.Input_AutoCloseBrackets.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.fontGroup);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(309, 190);
            this.tabPage2.TabIndex = 0;
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
            this.Appearance_Font_Size.TabIndex = 5;
            this.Appearance_Font_Size.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Buttons:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Size:";
            // 
            // Appearance_Font_Bold
            // 
            this.Appearance_Font_Bold.AutoSize = true;
            this.Appearance_Font_Bold.Location = new System.Drawing.Point(244, 71);
            this.Appearance_Font_Bold.Name = "Appearance_Font_Bold";
            this.Appearance_Font_Bold.Size = new System.Drawing.Size(47, 16);
            this.Appearance_Font_Bold.TabIndex = 6;
            this.Appearance_Font_Bold.Text = "Bold";
            this.Appearance_Font_Bold.UseVisualStyleBackColor = true;
            // 
            // Appearance_Font_Expr_Name
            // 
            this.Appearance_Font_Expr_Name.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Appearance_Font_Expr_Name.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Appearance_Font_Expr_Name.FormattingEnabled = true;
            this.Appearance_Font_Expr_Name.Location = new System.Drawing.Point(81, 44);
            this.Appearance_Font_Expr_Name.Name = "Appearance_Font_Expr_Name";
            this.Appearance_Font_Expr_Name.Size = new System.Drawing.Size(210, 20);
            this.Appearance_Font_Expr_Name.TabIndex = 3;
            // 
            // Appearance_Font_Button_Name
            // 
            this.Appearance_Font_Button_Name.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Appearance_Font_Button_Name.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Appearance_Font_Button_Name.FormattingEnabled = true;
            this.Appearance_Font_Button_Name.Location = new System.Drawing.Point(81, 18);
            this.Appearance_Font_Button_Name.Name = "Appearance_Font_Button_Name";
            this.Appearance_Font_Button_Name.Size = new System.Drawing.Size(210, 20);
            this.Appearance_Font_Button_Name.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.eNoteGroup);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(309, 190);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NumberFormat_Decimal_MaxLen);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(297, 48);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Decimal";
            // 
            // NumberFormat_Decimal_MaxLen
            // 
            this.NumberFormat_Decimal_MaxLen.Location = new System.Drawing.Point(222, 18);
            this.NumberFormat_Decimal_MaxLen.Maximum = new decimal(new int[] {
            28,
            0,
            0,
            0});
            this.NumberFormat_Decimal_MaxLen.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumberFormat_Decimal_MaxLen.Name = "NumberFormat_Decimal_MaxLen";
            this.NumberFormat_Decimal_MaxLen.Size = new System.Drawing.Size(65, 19);
            this.NumberFormat_Decimal_MaxLen.TabIndex = 1;
            this.NumberFormat_Decimal_MaxLen.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(205, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Max length of decimal place to display:";
            // 
            // Hotkey_KeyCode
            // 
            this.Hotkey_KeyCode.Alt = false;
            this.Hotkey_KeyCode.Ctrl = false;
            this.Hotkey_KeyCode.Enabled = false;
            this.Hotkey_KeyCode.KeyCode = System.Windows.Forms.Keys.None;
            this.Hotkey_KeyCode.Location = new System.Drawing.Point(41, 83);
            this.Hotkey_KeyCode.Name = "Hotkey_KeyCode";
            this.Hotkey_KeyCode.Shift = false;
            this.Hotkey_KeyCode.Size = new System.Drawing.Size(249, 19);
            this.Hotkey_KeyCode.TabIndex = 3;
            this.Hotkey_KeyCode.Win = false;
            // 
            // Input_AutoInputAns
            // 
            this.Input_AutoInputAns.AutoSize = true;
            this.Input_AutoInputAns.Location = new System.Drawing.Point(6, 40);
            this.Input_AutoInputAns.Name = "Input_AutoInputAns";
            this.Input_AutoInputAns.Size = new System.Drawing.Size(113, 16);
            this.Input_AutoInputAns.TabIndex = 1;
            this.Input_AutoInputAns.Text = "Auto input \"Ans\"";
            this.Input_AutoInputAns.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(341, 272);
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
            this.tabPage3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.fontGroup.ResumeLayout(false);
            this.fontGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Appearance_Font_Size)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberFormat_Decimal_MaxLen)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox Startup_TrayIcon;
        private System.Windows.Forms.CheckBox Startup_AutoStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown NumberFormat_Decimal_MaxLen;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox Window_RememberPosition;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox Input_AutoCloseBrackets;
        private System.Windows.Forms.CheckBox Input_AutoInputAns;
    }
}