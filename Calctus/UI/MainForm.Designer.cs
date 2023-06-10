
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.topMostButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.insertButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownButton = new System.Windows.Forms.ToolStripButton();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextExit = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sheetView = new Shapoco.Calctus.UI.Sheets.SheetView();
            this.radixOctButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixBinButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixHexButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixDecButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.radixAutoButton = new Shapoco.Calctus.UI.FlatRadioButton();
            this.toolStrip.SuspendLayout();
            this.trayMenuStrip.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpButton,
            this.topMostButton,
            this.settingsButton,
            this.copyButton,
            this.pasteButton,
            this.insertButton,
            this.deleteButton,
            this.moveUpButton,
            this.moveDownButton});
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
            // topMostButton
            // 
            this.topMostButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.topMostButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.topMostButton.Image = global::Shapoco.Properties.Resources.ToolIcon_TopMostOff;
            this.topMostButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.topMostButton.Name = "topMostButton";
            this.topMostButton.Size = new System.Drawing.Size(23, 22);
            this.topMostButton.Text = "Always on top";
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
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Copy;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(23, 22);
            this.copyButton.Text = "toolStripButton1";
            this.copyButton.ToolTipText = "Copy";
            // 
            // pasteButton
            // 
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Paste;
            this.pasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(23, 22);
            this.pasteButton.Text = "toolStripButton2";
            this.pasteButton.ToolTipText = "Paste";
            // 
            // insertButton
            // 
            this.insertButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.insertButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Insert;
            this.insertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(23, 22);
            this.insertButton.Text = "Insert";
            this.insertButton.ToolTipText = "Insert";
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Delete;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(23, 22);
            this.deleteButton.Text = "toolStripButton4";
            this.deleteButton.ToolTipText = "Delete";
            // 
            // moveUpButton
            // 
            this.moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpButton.Image = global::Shapoco.Properties.Resources.ToolIcon_MoveUp;
            this.moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(23, 22);
            this.moveUpButton.Text = "toolStripButton5";
            this.moveUpButton.ToolTipText = "Move Up";
            // 
            // moveDownButton
            // 
            this.moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownButton.Image = global::Shapoco.Properties.Resources.ToolIcon_MoveDown;
            this.moveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(23, 22);
            this.moveDownButton.Text = "toolStripButton6";
            this.moveDownButton.ToolTipText = "Move Down";
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
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.radixOctButton);
            this.bottomPanel.Controls.Add(this.radixBinButton);
            this.bottomPanel.Controls.Add(this.radixHexButton);
            this.bottomPanel.Controls.Add(this.radixDecButton);
            this.bottomPanel.Controls.Add(this.radixAutoButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 231);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(402, 15);
            this.bottomPanel.TabIndex = 5;
            // 
            // sheetView
            // 
            this.sheetView.ActiveRadixMode = Shapoco.Calctus.Model.Sheets.RadixMode.Auto;
            this.sheetView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.sheetView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sheetView.Location = new System.Drawing.Point(0, 25);
            this.sheetView.Name = "sheetView";
            this.sheetView.Size = new System.Drawing.Size(402, 206);
            this.sheetView.TabIndex = 0;
            this.sheetView.Text = "bookView1";
            // 
            // radixOctButton
            // 
            this.radixOctButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(72)))));
            this.radixOctButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixOctButton.ForeColor = System.Drawing.Color.White;
            this.radixOctButton.Location = new System.Drawing.Point(120, 0);
            this.radixOctButton.Name = "radixOctButton";
            this.radixOctButton.Size = new System.Drawing.Size(30, 15);
            this.radixOctButton.TabIndex = 7;
            this.radixOctButton.TabStop = true;
            this.radixOctButton.Text = "Oct";
            this.radixOctButton.UseVisualStyleBackColor = false;
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
            this.radixAutoButton.Checked = true;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(402, 246);
            this.Controls.Add(this.sheetView);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.toolStrip);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Form1";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.trayMenuStrip.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.ToolStripButton helpButton;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem contextOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem contextExit;
        private System.Windows.Forms.Panel bottomPanel;
        private FlatRadioButton radixBinButton;
        private FlatRadioButton radixHexButton;
        private FlatRadioButton radixDecButton;
        private FlatRadioButton radixAutoButton;
        private System.Windows.Forms.ToolStripButton topMostButton;
        private FlatRadioButton radixOctButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton pasteButton;
        private System.Windows.Forms.ToolStripButton insertButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton moveUpButton;
        private System.Windows.Forms.ToolStripButton moveDownButton;
        private UI.Sheets.SheetView sheetView;
    }
}

