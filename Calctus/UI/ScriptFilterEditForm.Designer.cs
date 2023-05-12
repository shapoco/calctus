namespace Shapoco.Calctus.UI {
    partial class ScriptFilterEditForm {
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
            this.closeButton = new System.Windows.Forms.Button();
            this.executable = new System.Windows.Forms.TextBox();
            this.filter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.exeBrowseButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.parameter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(378, 129);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(89, 25);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // executable
            // 
            this.executable.Location = new System.Drawing.Point(81, 37);
            this.executable.Name = "executable";
            this.executable.Size = new System.Drawing.Size(310, 19);
            this.executable.TabIndex = 3;
            // 
            // filter
            // 
            this.filter.Location = new System.Drawing.Point(81, 12);
            this.filter.Name = "filter";
            this.filter.Size = new System.Drawing.Size(386, 19);
            this.filter.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Executable";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter";
            // 
            // exeBrowseButton
            // 
            this.exeBrowseButton.Location = new System.Drawing.Point(397, 35);
            this.exeBrowseButton.Name = "exeBrowseButton";
            this.exeBrowseButton.Size = new System.Drawing.Size(70, 23);
            this.exeBrowseButton.TabIndex = 4;
            this.exeBrowseButton.Text = "Browse";
            this.exeBrowseButton.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "If left blank, the script will be passed to the associated application.";
            // 
            // parameter
            // 
            this.parameter.Location = new System.Drawing.Point(81, 79);
            this.parameter.Name = "parameter";
            this.parameter.Size = new System.Drawing.Size(386, 19);
            this.parameter.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Parameter";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(79, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(342, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "%s replaced with script path. %p replaced with function arguments.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(193, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "If left blank, [ \"%s\" %p ] will be used.";
            // 
            // ScriptFilterEditForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 166);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.exeBrowseButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.parameter);
            this.Controls.Add(this.executable);
            this.Controls.Add(this.filter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScriptFilterEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InterpreterEditForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TextBox executable;
        private System.Windows.Forms.TextBox filter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button exeBrowseButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox parameter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}