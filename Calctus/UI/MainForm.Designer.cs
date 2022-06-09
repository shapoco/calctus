
namespace Shapoco.Calctus.UI
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.exprBox = new System.Windows.Forms.TextBox();
            this.subAnswerLabel = new System.Windows.Forms.Label();
            this.calcButton = new Shapoco.Calctus.UI.FlatButton();
            this.historyBox = new Shapoco.Calctus.UI.HistoryBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exprBox);
            this.panel1.Controls.Add(this.subAnswerLabel);
            this.panel1.Controls.Add(this.calcButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 113);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(342, 52);
            this.panel1.TabIndex = 3;
            // 
            // exprBox
            // 
            this.exprBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.exprBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.exprBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exprBox.ForeColor = System.Drawing.Color.White;
            this.exprBox.Location = new System.Drawing.Point(0, 0);
            this.exprBox.Name = "exprBox";
            this.exprBox.Size = new System.Drawing.Size(267, 12);
            this.exprBox.TabIndex = 0;
            // 
            // subAnswerLabel
            // 
            this.subAnswerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.subAnswerLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.subAnswerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.subAnswerLabel.Location = new System.Drawing.Point(0, 34);
            this.subAnswerLabel.Name = "subAnswerLabel";
            this.subAnswerLabel.Size = new System.Drawing.Size(267, 18);
            this.subAnswerLabel.TabIndex = 2;
            this.subAnswerLabel.Text = "subanswer";
            this.subAnswerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // calcButton
            // 
            this.calcButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.calcButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.calcButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.calcButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.calcButton.Location = new System.Drawing.Point(267, 0);
            this.calcButton.Name = "calcButton";
            this.calcButton.Size = new System.Drawing.Size(75, 52);
            this.calcButton.TabIndex = 1;
            this.calcButton.Text = "=";
            this.calcButton.UseVisualStyleBackColor = false;
            // 
            // historyBox
            // 
            this.historyBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.historyBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.historyBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.historyBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.historyBox.FormattingEnabled = true;
            this.historyBox.IntegralHeight = false;
            this.historyBox.ItemHeight = 12;
            this.historyBox.Location = new System.Drawing.Point(0, 0);
            this.historyBox.Name = "historyBox";
            this.historyBox.Size = new System.Drawing.Size(342, 113);
            this.historyBox.TabIndex = 3;
            this.historyBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(342, 165);
            this.Controls.Add(this.historyBox);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox exprBox;
        private Shapoco.Calctus.UI.FlatButton calcButton;
        private System.Windows.Forms.Label subAnswerLabel;
        private HistoryBox historyBox;
    }
}

