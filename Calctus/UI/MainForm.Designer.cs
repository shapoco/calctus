
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
            this.logBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.exprBox = new Shapoco.Calctus.UI.ExpressionBox();
            this.subAnswerLabel = new System.Windows.Forms.Label();
            this.calcButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(342, 113);
            this.logBox.TabIndex = 0;
            this.logBox.TabStop = false;
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
            this.exprBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exprBox.Location = new System.Drawing.Point(0, 0);
            this.exprBox.Name = "exprBox";
            this.exprBox.Size = new System.Drawing.Size(267, 19);
            this.exprBox.TabIndex = 1;
            // 
            // subAnswerLabel
            // 
            this.subAnswerLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.subAnswerLabel.Location = new System.Drawing.Point(0, 34);
            this.subAnswerLabel.Name = "subAnswerLabel";
            this.subAnswerLabel.Size = new System.Drawing.Size(267, 18);
            this.subAnswerLabel.TabIndex = 2;
            this.subAnswerLabel.Text = "subanswer";
            this.subAnswerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // calcButton
            // 
            this.calcButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.calcButton.Location = new System.Drawing.Point(267, 0);
            this.calcButton.Name = "calcButton";
            this.calcButton.Size = new System.Drawing.Size(75, 52);
            this.calcButton.TabIndex = 1;
            this.calcButton.Text = "=";
            this.calcButton.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 165);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Panel panel1;
        private ExpressionBox exprBox;
        private System.Windows.Forms.Button calcButton;
        private System.Windows.Forms.Label subAnswerLabel;
    }
}

