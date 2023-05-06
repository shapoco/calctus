namespace Shapoco.Calctus.UI {
    partial class CreateTimerForm {
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
            this.timerRadioButton = new System.Windows.Forms.RadioButton();
            this.alarmRadioButton = new System.Windows.Forms.RadioButton();
            this.timerGroup = new System.Windows.Forms.GroupBox();
            this.alarmGroup = new System.Windows.Forms.GroupBox();
            this.timeUpDown = new System.Windows.Forms.NumericUpDown();
            this.secRadioButton = new System.Windows.Forms.RadioButton();
            this.minRadioButton = new System.Windows.Forms.RadioButton();
            this.hourRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimeText = new System.Windows.Forms.TextBox();
            this.timeoutTimeLabel = new System.Windows.Forms.Label();
            this.timerGroup.SuspendLayout();
            this.alarmGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // timerRadioButton
            // 
            this.timerRadioButton.AutoSize = true;
            this.timerRadioButton.Checked = true;
            this.timerRadioButton.Location = new System.Drawing.Point(17, 10);
            this.timerRadioButton.Name = "timerRadioButton";
            this.timerRadioButton.Size = new System.Drawing.Size(52, 16);
            this.timerRadioButton.TabIndex = 0;
            this.timerRadioButton.TabStop = true;
            this.timerRadioButton.Text = "Timer";
            this.timerRadioButton.UseVisualStyleBackColor = true;
            // 
            // alarmRadioButton
            // 
            this.alarmRadioButton.AutoSize = true;
            this.alarmRadioButton.Location = new System.Drawing.Point(17, 81);
            this.alarmRadioButton.Name = "alarmRadioButton";
            this.alarmRadioButton.Size = new System.Drawing.Size(53, 16);
            this.alarmRadioButton.TabIndex = 0;
            this.alarmRadioButton.Text = "Alarm";
            this.alarmRadioButton.UseVisualStyleBackColor = true;
            // 
            // timerGroup
            // 
            this.timerGroup.Controls.Add(this.timeoutTimeLabel);
            this.timerGroup.Controls.Add(this.hourRadioButton);
            this.timerGroup.Controls.Add(this.minRadioButton);
            this.timerGroup.Controls.Add(this.secRadioButton);
            this.timerGroup.Controls.Add(this.timeUpDown);
            this.timerGroup.Location = new System.Drawing.Point(12, 12);
            this.timerGroup.Name = "timerGroup";
            this.timerGroup.Size = new System.Drawing.Size(285, 63);
            this.timerGroup.TabIndex = 1;
            this.timerGroup.TabStop = false;
            // 
            // alarmGroup
            // 
            this.alarmGroup.Controls.Add(this.dateTimeText);
            this.alarmGroup.Controls.Add(this.label1);
            this.alarmGroup.Location = new System.Drawing.Point(12, 84);
            this.alarmGroup.Name = "alarmGroup";
            this.alarmGroup.Size = new System.Drawing.Size(285, 63);
            this.alarmGroup.TabIndex = 1;
            this.alarmGroup.TabStop = false;
            // 
            // timeUpDown
            // 
            this.timeUpDown.Location = new System.Drawing.Point(6, 20);
            this.timeUpDown.Name = "timeUpDown";
            this.timeUpDown.Size = new System.Drawing.Size(120, 19);
            this.timeUpDown.TabIndex = 0;
            this.timeUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.timeUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // secRadioButton
            // 
            this.secRadioButton.AutoSize = true;
            this.secRadioButton.Location = new System.Drawing.Point(132, 20);
            this.secRadioButton.Name = "secRadioButton";
            this.secRadioButton.Size = new System.Drawing.Size(41, 16);
            this.secRadioButton.TabIndex = 1;
            this.secRadioButton.Text = "sec";
            this.secRadioButton.UseVisualStyleBackColor = true;
            // 
            // minRadioButton
            // 
            this.minRadioButton.AutoSize = true;
            this.minRadioButton.Checked = true;
            this.minRadioButton.Location = new System.Drawing.Point(179, 20);
            this.minRadioButton.Name = "minRadioButton";
            this.minRadioButton.Size = new System.Drawing.Size(41, 16);
            this.minRadioButton.TabIndex = 1;
            this.minRadioButton.TabStop = true;
            this.minRadioButton.Text = "min";
            this.minRadioButton.UseVisualStyleBackColor = true;
            // 
            // hourRadioButton
            // 
            this.hourRadioButton.AutoSize = true;
            this.hourRadioButton.Location = new System.Drawing.Point(226, 20);
            this.hourRadioButton.Name = "hourRadioButton";
            this.hourRadioButton.Size = new System.Drawing.Size(45, 16);
            this.hourRadioButton.TabIndex = 1;
            this.hourRadioButton.Text = "hour";
            this.hourRadioButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(223, 153);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 25);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.startButton.Location = new System.Drawing.Point(142, 153);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 25);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Date time in format YYYY/MM/DD hh:mm:ss";
            // 
            // dateTimeText
            // 
            this.dateTimeText.Location = new System.Drawing.Point(8, 32);
            this.dateTimeText.Name = "dateTimeText";
            this.dateTimeText.Size = new System.Drawing.Size(263, 19);
            this.dateTimeText.TabIndex = 1;
            // 
            // timeoutTimeLabel
            // 
            this.timeoutTimeLabel.AutoSize = true;
            this.timeoutTimeLabel.Location = new System.Drawing.Point(6, 42);
            this.timeoutTimeLabel.Name = "timeoutTimeLabel";
            this.timeoutTimeLabel.Size = new System.Drawing.Size(35, 12);
            this.timeoutTimeLabel.TabIndex = 2;
            this.timeoutTimeLabel.Text = "label2";
            // 
            // CreateTimerForm
            // 
            this.AcceptButton = this.startButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(310, 190);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.timerRadioButton);
            this.Controls.Add(this.timerGroup);
            this.Controls.Add(this.alarmRadioButton);
            this.Controls.Add(this.alarmGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTimerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Timer";
            this.timerGroup.ResumeLayout(false);
            this.timerGroup.PerformLayout();
            this.alarmGroup.ResumeLayout(false);
            this.alarmGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton timerRadioButton;
        private System.Windows.Forms.RadioButton alarmRadioButton;
        private System.Windows.Forms.GroupBox timerGroup;
        private System.Windows.Forms.NumericUpDown timeUpDown;
        private System.Windows.Forms.GroupBox alarmGroup;
        private System.Windows.Forms.RadioButton hourRadioButton;
        private System.Windows.Forms.RadioButton minRadioButton;
        private System.Windows.Forms.RadioButton secRadioButton;
        private System.Windows.Forms.TextBox dateTimeText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label timeoutTimeLabel;
    }
}