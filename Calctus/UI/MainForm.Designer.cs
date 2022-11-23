
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.subAnswerLabel = new System.Windows.Forms.Label();
            this.radixBinButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixHexButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixDecButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixAutoButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.exprBox = new System.Windows.Forms.TextBox();
            this.calcButton = new Shapoco.Calctus.UI.FlatButton();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.historyBox = new Shapoco.Calctus.UI.HistoryBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextExit = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.historyMenuCopyText = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuCopyAnswer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.historyMenuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.historyMenuCopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.historyMenuDeleteAll = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.trayMenuStrip.SuspendLayout();
            this.historyMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.exprBox);
            this.panel1.Controls.Add(this.calcButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 194);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(402, 52);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.subAnswerLabel);
            this.panel2.Controls.Add(this.radixBinButton);
            this.panel2.Controls.Add(this.radixHexButton);
            this.panel2.Controls.Add(this.radixDecButton);
            this.panel2.Controls.Add(this.radixAutoButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 37);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(327, 15);
            this.panel2.TabIndex = 3;
            // 
            // subAnswerLabel
            // 
            this.subAnswerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.subAnswerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subAnswerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.subAnswerLabel.Location = new System.Drawing.Point(120, 0);
            this.subAnswerLabel.Name = "subAnswerLabel";
            this.subAnswerLabel.Size = new System.Drawing.Size(207, 15);
            this.subAnswerLabel.TabIndex = 2;
            this.subAnswerLabel.Text = "subanswer";
            this.subAnswerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // radixBinButton
            // 
            this.radixBinButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.radixBinButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixBinButton.ForeColor = System.Drawing.Color.White;
            this.radixBinButton.Location = new System.Drawing.Point(90, 0);
            this.radixBinButton.Name = "radixBinButton";
            this.radixBinButton.Size = new System.Drawing.Size(30, 15);
            this.radixBinButton.TabIndex = 6;
            this.radixBinButton.TabStop = true;
            this.radixBinButton.Text = "Bin";
            this.radixBinButton.UseVisualStyleBackColor = false;
            // 
            // radixHexButton
            // 
            this.radixHexButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.radixHexButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixHexButton.ForeColor = System.Drawing.Color.White;
            this.radixHexButton.Location = new System.Drawing.Point(60, 0);
            this.radixHexButton.Name = "radixHexButton";
            this.radixHexButton.Size = new System.Drawing.Size(30, 15);
            this.radixHexButton.TabIndex = 5;
            this.radixHexButton.TabStop = true;
            this.radixHexButton.Text = "Hex";
            this.radixHexButton.UseVisualStyleBackColor = false;
            // 
            // radixDecButton
            // 
            this.radixDecButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.radixDecButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixDecButton.ForeColor = System.Drawing.Color.White;
            this.radixDecButton.Location = new System.Drawing.Point(30, 0);
            this.radixDecButton.Name = "radixDecButton";
            this.radixDecButton.Size = new System.Drawing.Size(30, 15);
            this.radixDecButton.TabIndex = 4;
            this.radixDecButton.TabStop = true;
            this.radixDecButton.Text = "Dec";
            this.radixDecButton.UseVisualStyleBackColor = false;
            // 
            // radixAutoButton
            // 
            this.radixAutoButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.radixAutoButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixAutoButton.ForeColor = System.Drawing.Color.White;
            this.radixAutoButton.Location = new System.Drawing.Point(0, 0);
            this.radixAutoButton.Name = "radixAutoButton";
            this.radixAutoButton.Size = new System.Drawing.Size(30, 15);
            this.radixAutoButton.TabIndex = 3;
            this.radixAutoButton.TabStop = true;
            this.radixAutoButton.Text = "Auto";
            this.radixAutoButton.UseVisualStyleBackColor = false;
            // 
            // exprBox
            // 
            this.exprBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.exprBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.exprBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exprBox.ForeColor = System.Drawing.Color.White;
            this.exprBox.Location = new System.Drawing.Point(0, 0);
            this.exprBox.Name = "exprBox";
            this.exprBox.Size = new System.Drawing.Size(327, 12);
            this.exprBox.TabIndex = 0;
            // 
            // calcButton
            // 
            this.calcButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.calcButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.calcButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.calcButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.calcButton.Location = new System.Drawing.Point(327, 0);
            this.calcButton.Name = "calcButton";
            this.calcButton.Size = new System.Drawing.Size(75, 52);
            this.calcButton.TabIndex = 1;
            this.calcButton.Text = "=";
            this.calcButton.UseVisualStyleBackColor = false;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpButton,
            this.settingsButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(402, 25);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            // 
            // helpButton
            // 
            this.helpButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Help;
            this.helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(23, 22);
            this.helpButton.Text = "Help";
            // 
            // settingsButton
            // 
            this.settingsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.settingsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingsButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Settings;
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(23, 22);
            this.settingsButton.Text = "Settings";
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
            this.historyBox.Location = new System.Drawing.Point(0, 25);
            this.historyBox.Name = "historyBox";
            this.historyBox.SelectedHistoryItem = null;
            this.historyBox.Size = new System.Drawing.Size(402, 169);
            this.historyBox.TabIndex = 3;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "notifyIcon1";
            this.notifyIcon.Visible = true;
            // 
            // trayMenuStrip
            // 
            this.trayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextOpen,
            this.toolStripMenuItem1,
            this.contextExit});
            this.trayMenuStrip.Name = "contextMenuStrip";
            this.trayMenuStrip.Size = new System.Drawing.Size(124, 54);
            // 
            // contextOpen
            // 
            this.contextOpen.Name = "contextOpen";
            this.contextOpen.Size = new System.Drawing.Size(123, 22);
            this.contextOpen.Text = "Open (&O)";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            // 
            // contextExit
            // 
            this.contextExit.Name = "contextExit";
            this.contextExit.Size = new System.Drawing.Size(123, 22);
            this.contextExit.Text = "Exit (&X)";
            // 
            // historyMenuStrip
            // 
            this.historyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historyMenuCopyText,
            this.historyMenuCopyAnswer,
            this.historyMenuCopyAll,
            this.toolStripMenuItem3,
            this.historyMenuMoveUp,
            this.historyMenuMoveDown,
            this.toolStripMenuItem2,
            this.historyMenuInsert,
            this.historyMenuDelete,
            this.historyMenuDeleteAll});
            this.historyMenuStrip.Name = "historyMenuStrip";
            this.historyMenuStrip.Size = new System.Drawing.Size(144, 192);
            // 
            // historyMenuCopyText
            // 
            this.historyMenuCopyText.Name = "historyMenuCopyText";
            this.historyMenuCopyText.Size = new System.Drawing.Size(143, 22);
            this.historyMenuCopyText.Text = "Copy Text";
            // 
            // historyMenuCopyAnswer
            // 
            this.historyMenuCopyAnswer.Name = "historyMenuCopyAnswer";
            this.historyMenuCopyAnswer.Size = new System.Drawing.Size(143, 22);
            this.historyMenuCopyAnswer.Text = "Copy Answer";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(140, 6);
            // 
            // historyMenuDelete
            // 
            this.historyMenuDelete.Name = "historyMenuDelete";
            this.historyMenuDelete.Size = new System.Drawing.Size(143, 22);
            this.historyMenuDelete.Text = "Delete";
            // 
            // historyMenuInsert
            // 
            this.historyMenuInsert.Name = "historyMenuInsert";
            this.historyMenuInsert.Size = new System.Drawing.Size(143, 22);
            this.historyMenuInsert.Text = "Insert";
            // 
            // historyMenuMoveUp
            // 
            this.historyMenuMoveUp.Name = "historyMenuMoveUp";
            this.historyMenuMoveUp.Size = new System.Drawing.Size(143, 22);
            this.historyMenuMoveUp.Text = "Move Up";
            // 
            // historyMenuMoveDown
            // 
            this.historyMenuMoveDown.Name = "historyMenuMoveDown";
            this.historyMenuMoveDown.Size = new System.Drawing.Size(143, 22);
            this.historyMenuMoveDown.Text = "Move Down";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(140, 6);
            // 
            // historyMenuCopyAll
            // 
            this.historyMenuCopyAll.Name = "historyMenuCopyAll";
            this.historyMenuCopyAll.Size = new System.Drawing.Size(143, 22);
            this.historyMenuCopyAll.Text = "Copy All";
            // 
            // historyMenuDeleteAll
            // 
            this.historyMenuDeleteAll.Name = "historyMenuDeleteAll";
            this.historyMenuDeleteAll.Size = new System.Drawing.Size(143, 22);
            this.historyMenuDeleteAll.Text = "Clear All";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(402, 246);
            this.Controls.Add(this.historyBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.trayMenuStrip.ResumeLayout(false);
            this.historyMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox exprBox;
        private Shapoco.Calctus.UI.FlatButton calcButton;
        private System.Windows.Forms.Label subAnswerLabel;
        private HistoryBox historyBox;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.ToolStripButton helpButton;
        private System.Windows.Forms.Panel panel2;
        private FlatRadioButton radixBinButton;
        private FlatRadioButton radixHexButton;
        private FlatRadioButton radixDecButton;
        private FlatRadioButton radixAutoButton;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem contextOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem contextExit;
        private System.Windows.Forms.ContextMenuStrip historyMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem historyMenuCopyText;
        private System.Windows.Forms.ToolStripMenuItem historyMenuCopyAnswer;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem historyMenuDelete;
        private System.Windows.Forms.ToolStripMenuItem historyMenuCopyAll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem historyMenuMoveUp;
        private System.Windows.Forms.ToolStripMenuItem historyMenuMoveDown;
        private System.Windows.Forms.ToolStripMenuItem historyMenuInsert;
        private System.Windows.Forms.ToolStripMenuItem historyMenuDeleteAll;
    }
}

