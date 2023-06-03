namespace Shapoco.Calctus.UI {
    partial class PasteOptionForm {
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
            this.sourceText = new System.Windows.Forms.TextBox();
            this.textWillBePasted = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.removeCommaButton = new System.Windows.Forms.Button();
            this.removeRightHandsButton = new System.Windows.Forms.Button();
            this.columnNumberText = new System.Windows.Forms.TextBox();
            this.selectColumnButton = new System.Windows.Forms.Button();
            this.numColumnsLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.columnDelimiterText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // sourceText
            // 
            this.sourceText.AcceptsReturn = true;
            this.sourceText.Location = new System.Drawing.Point(12, 25);
            this.sourceText.Multiline = true;
            this.sourceText.Name = "sourceText";
            this.sourceText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sourceText.Size = new System.Drawing.Size(280, 291);
            this.sourceText.TabIndex = 3;
            // 
            // textWillBePasted
            // 
            this.textWillBePasted.AcceptsReturn = true;
            this.textWillBePasted.Location = new System.Drawing.Point(304, 25);
            this.textWillBePasted.Multiline = true;
            this.textWillBePasted.Name = "textWillBePasted";
            this.textWillBePasted.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textWillBePasted.Size = new System.Drawing.Size(280, 291);
            this.textWillBePasted.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Clipboard Text:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(302, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Text will be pasted:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(504, 379);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(80, 25);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(418, 379);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(80, 25);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // removeCommaButton
            // 
            this.removeCommaButton.Location = new System.Drawing.Point(304, 322);
            this.removeCommaButton.Name = "removeCommaButton";
            this.removeCommaButton.Size = new System.Drawing.Size(125, 23);
            this.removeCommaButton.TabIndex = 12;
            this.removeCommaButton.Text = "Remove Commas";
            this.removeCommaButton.UseVisualStyleBackColor = true;
            // 
            // removeRightHandsButton
            // 
            this.removeRightHandsButton.Location = new System.Drawing.Point(435, 322);
            this.removeRightHandsButton.Name = "removeRightHandsButton";
            this.removeRightHandsButton.Size = new System.Drawing.Size(149, 23);
            this.removeRightHandsButton.TabIndex = 13;
            this.removeRightHandsButton.Text = "Remove Right-hands";
            this.removeRightHandsButton.UseVisualStyleBackColor = true;
            // 
            // columnNumberText
            // 
            this.columnNumberText.Location = new System.Drawing.Point(115, 349);
            this.columnNumberText.Name = "columnNumberText";
            this.columnNumberText.Size = new System.Drawing.Size(33, 19);
            this.columnNumberText.TabIndex = 7;
            this.columnNumberText.Text = "1";
            // 
            // selectColumnButton
            // 
            this.selectColumnButton.Location = new System.Drawing.Point(186, 347);
            this.selectColumnButton.Name = "selectColumnButton";
            this.selectColumnButton.Size = new System.Drawing.Size(106, 23);
            this.selectColumnButton.TabIndex = 9;
            this.selectColumnButton.Text = "Select Column";
            this.selectColumnButton.UseVisualStyleBackColor = true;
            // 
            // numColumnsLabel
            // 
            this.numColumnsLabel.AutoSize = true;
            this.numColumnsLabel.Location = new System.Drawing.Point(154, 352);
            this.numColumnsLabel.Name = "numColumnsLabel";
            this.numColumnsLabel.Size = new System.Drawing.Size(21, 12);
            this.numColumnsLabel.TabIndex = 8;
            this.numColumnsLabel.Text = "/ n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 327);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Column Delimiter:";
            // 
            // columnDelimiterText
            // 
            this.columnDelimiterText.Location = new System.Drawing.Point(115, 324);
            this.columnDelimiterText.Name = "columnDelimiterText";
            this.columnDelimiterText.Size = new System.Drawing.Size(33, 19);
            this.columnDelimiterText.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 352);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Column Index:";
            // 
            // PasteOptionForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(596, 416);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numColumnsLabel);
            this.Controls.Add(this.columnDelimiterText);
            this.Controls.Add(this.columnNumberText);
            this.Controls.Add(this.removeRightHandsButton);
            this.Controls.Add(this.selectColumnButton);
            this.Controls.Add(this.removeCommaButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textWillBePasted);
            this.Controls.Add(this.sourceText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasteOptionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Paste Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sourceText;
        private System.Windows.Forms.TextBox textWillBePasted;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button removeCommaButton;
        private System.Windows.Forms.Button removeRightHandsButton;
        private System.Windows.Forms.TextBox columnNumberText;
        private System.Windows.Forms.Button selectColumnButton;
        private System.Windows.Forms.Label numColumnsLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox columnDelimiterText;
        private System.Windows.Forms.Label label4;
    }
}