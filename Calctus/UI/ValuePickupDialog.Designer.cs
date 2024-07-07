namespace Shapoco.Calctus.UI {
    partial class ValuePickupDialog {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValuePickupDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.removeCommaCheckBox = new System.Windows.Forms.CheckBox();
            this.withOperatorRadioButton = new System.Windows.Forms.RadioButton();
            this.joinOperatorComboBox = new System.Windows.Forms.ComboBox();
            this.asArrayRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.valueAsStringCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(139, 166);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // finishButton
            // 
            this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.finishButton.Location = new System.Drawing.Point(58, 166);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 25);
            this.finishButton.TabIndex = 1;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            // 
            // removeCommaCheckBox
            // 
            this.removeCommaCheckBox.AutoSize = true;
            this.removeCommaCheckBox.Location = new System.Drawing.Point(16, 21);
            this.removeCommaCheckBox.Name = "removeCommaCheckBox";
            this.removeCommaCheckBox.Size = new System.Drawing.Size(105, 16);
            this.removeCommaCheckBox.TabIndex = 0;
            this.removeCommaCheckBox.Text = "Remove comma";
            this.removeCommaCheckBox.UseVisualStyleBackColor = true;
            // 
            // joinWithOpRadioButton
            // 
            this.withOperatorRadioButton.AutoSize = true;
            this.withOperatorRadioButton.Checked = true;
            this.withOperatorRadioButton.Location = new System.Drawing.Point(17, 19);
            this.withOperatorRadioButton.Name = "joinWithOpRadioButton";
            this.withOperatorRadioButton.Size = new System.Drawing.Size(118, 16);
            this.withOperatorRadioButton.TabIndex = 3;
            this.withOperatorRadioButton.TabStop = true;
            this.withOperatorRadioButton.Text = "Join with operator:";
            this.withOperatorRadioButton.UseVisualStyleBackColor = true;
            // 
            // joinOperatorComboBox
            // 
            this.joinOperatorComboBox.FormattingEnabled = true;
            this.joinOperatorComboBox.Location = new System.Drawing.Point(141, 18);
            this.joinOperatorComboBox.Name = "joinOperatorComboBox";
            this.joinOperatorComboBox.Size = new System.Drawing.Size(54, 20);
            this.joinOperatorComboBox.TabIndex = 4;
            // 
            // asArrayRadioButton
            // 
            this.asArrayRadioButton.AutoSize = true;
            this.asArrayRadioButton.Location = new System.Drawing.Point(17, 41);
            this.asArrayRadioButton.Name = "asArrayRadioButton";
            this.asArrayRadioButton.Size = new System.Drawing.Size(91, 16);
            this.asArrayRadioButton.TabIndex = 3;
            this.asArrayRadioButton.Text = "Join as array";
            this.asArrayRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.withOperatorRadioButton);
            this.groupBox1.Controls.Add(this.joinOperatorComboBox);
            this.groupBox1.Controls.Add(this.asArrayRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 69);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Join Method";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.valueAsStringCheckBox);
            this.groupBox2.Controls.Add(this.removeCommaCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(201, 72);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // valueAsStringCheckBox
            // 
            this.valueAsStringCheckBox.AutoSize = true;
            this.valueAsStringCheckBox.Location = new System.Drawing.Point(16, 43);
            this.valueAsStringCheckBox.Name = "valueAsStringCheckBox";
            this.valueAsStringCheckBox.Size = new System.Drawing.Size(102, 16);
            this.valueAsStringCheckBox.TabIndex = 0;
            this.valueAsStringCheckBox.Text = "Value as string";
            this.valueAsStringCheckBox.UseVisualStyleBackColor = true;
            // 
            // ValuePickupDialog
            // 
            this.AcceptButton = this.finishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(226, 203);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.cancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ValuePickupDialog";
            this.ShowInTaskbar = false;
            this.Text = "Value Pickup";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.CheckBox removeCommaCheckBox;
        private System.Windows.Forms.RadioButton withOperatorRadioButton;
        private System.Windows.Forms.ComboBox joinOperatorComboBox;
        private System.Windows.Forms.RadioButton asArrayRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox valueAsStringCheckBox;
    }
}