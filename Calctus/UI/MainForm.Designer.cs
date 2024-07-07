
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
            this.undoButton = new System.Windows.Forms.ToolStripButton();
            this.redoButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.insertButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownButton = new System.Windows.Forms.ToolStripButton();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextExit = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.radixCharButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixKibiButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixSiButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixOctButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixBinButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixHexButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixDecButton = new Shapoco.Calctus.UI.FlatButton();
            this.radixAutoButton = new Shapoco.Calctus.UI.FlatButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sidePaneHeaderPanel = new System.Windows.Forms.Panel();
            this.sidePaneOpenButton = new Shapoco.Calctus.UI.FlatButton();
            this.bookTreeView = new Shapoco.Calctus.UI.Books.BookTreeView();
            this.sidePaneBodyPanel = new System.Windows.Forms.Panel();
            this.toolStrip.SuspendLayout();
            this.trayMenuStrip.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.sidePaneHeaderPanel.SuspendLayout();
            this.sidePaneBodyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpButton,
            this.topMostButton,
            this.settingsButton,
            this.undoButton,
            this.redoButton,
            this.toolStripSeparator1,
            this.copyButton,
            this.pasteButton,
            this.toolStripSeparator2,
            this.insertButton,
            this.deleteButton,
            this.toolStripSeparator3,
            this.moveUpButton,
            this.moveDownButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(577, 25);
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
            // undoButton
            // 
            this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Undo;
            this.undoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(23, 22);
            this.undoButton.Text = "toolStripButton1";
            this.undoButton.ToolTipText = "Undo";
            // 
            // redoButton
            // 
            this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Redo;
            this.redoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(23, 22);
            this.redoButton.Text = "toolStripButton2";
            this.redoButton.ToolTipText = "Redo";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
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
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
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
            this.bottomPanel.Controls.Add(this.radixCharButton);
            this.bottomPanel.Controls.Add(this.radixKibiButton);
            this.bottomPanel.Controls.Add(this.radixSiButton);
            this.bottomPanel.Controls.Add(this.radixOctButton);
            this.bottomPanel.Controls.Add(this.radixBinButton);
            this.bottomPanel.Controls.Add(this.radixHexButton);
            this.bottomPanel.Controls.Add(this.radixDecButton);
            this.bottomPanel.Controls.Add(this.radixAutoButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(220, 231);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(357, 15);
            this.bottomPanel.TabIndex = 5;
            // 
            // radixCharButton
            // 
            this.radixCharButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixCharButton.ForeColor = System.Drawing.Color.White;
            this.radixCharButton.Location = new System.Drawing.Point(210, 0);
            this.radixCharButton.Name = "radixCharButton";
            this.radixCharButton.Size = new System.Drawing.Size(30, 15);
            this.radixCharButton.TabIndex = 10;
            this.radixCharButton.Text = "Char";
            this.radixCharButton.UseVisualStyleBackColor = false;
            // 
            // radixKibiButton
            // 
            this.radixKibiButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixKibiButton.ForeColor = System.Drawing.Color.White;
            this.radixKibiButton.Location = new System.Drawing.Point(180, 0);
            this.radixKibiButton.Name = "radixKibiButton";
            this.radixKibiButton.Size = new System.Drawing.Size(30, 15);
            this.radixKibiButton.TabIndex = 9;
            this.radixKibiButton.Text = "Kibi";
            this.radixKibiButton.UseVisualStyleBackColor = false;
            // 
            // radixSiButton
            // 
            this.radixSiButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixSiButton.ForeColor = System.Drawing.Color.White;
            this.radixSiButton.Location = new System.Drawing.Point(150, 0);
            this.radixSiButton.Name = "radixSiButton";
            this.radixSiButton.Size = new System.Drawing.Size(30, 15);
            this.radixSiButton.TabIndex = 8;
            this.radixSiButton.Text = "SI";
            this.radixSiButton.UseVisualStyleBackColor = false;
            // 
            // radixOctButton
            // 
            this.radixOctButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixOctButton.ForeColor = System.Drawing.Color.White;
            this.radixOctButton.Location = new System.Drawing.Point(120, 0);
            this.radixOctButton.Name = "radixOctButton";
            this.radixOctButton.Size = new System.Drawing.Size(30, 15);
            this.radixOctButton.TabIndex = 7;
            this.radixOctButton.Text = "Oct";
            this.radixOctButton.UseVisualStyleBackColor = false;
            // 
            // radixBinButton
            // 
            this.radixBinButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixBinButton.ForeColor = System.Drawing.Color.White;
            this.radixBinButton.Location = new System.Drawing.Point(90, 0);
            this.radixBinButton.Name = "radixBinButton";
            this.radixBinButton.Size = new System.Drawing.Size(30, 15);
            this.radixBinButton.TabIndex = 6;
            this.radixBinButton.Text = "Bin";
            this.radixBinButton.UseVisualStyleBackColor = false;
            // 
            // radixHexButton
            // 
            this.radixHexButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixHexButton.ForeColor = System.Drawing.Color.White;
            this.radixHexButton.Location = new System.Drawing.Point(60, 0);
            this.radixHexButton.Name = "radixHexButton";
            this.radixHexButton.Size = new System.Drawing.Size(30, 15);
            this.radixHexButton.TabIndex = 5;
            this.radixHexButton.Text = "Hex";
            this.radixHexButton.UseVisualStyleBackColor = false;
            // 
            // radixDecButton
            // 
            this.radixDecButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixDecButton.ForeColor = System.Drawing.Color.White;
            this.radixDecButton.Location = new System.Drawing.Point(30, 0);
            this.radixDecButton.Name = "radixDecButton";
            this.radixDecButton.Size = new System.Drawing.Size(30, 15);
            this.radixDecButton.TabIndex = 4;
            this.radixDecButton.Text = "Dec";
            this.radixDecButton.UseVisualStyleBackColor = false;
            // 
            // radixAutoButton
            // 
            this.radixAutoButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.radixAutoButton.ForeColor = System.Drawing.Color.White;
            this.radixAutoButton.Location = new System.Drawing.Point(0, 0);
            this.radixAutoButton.Name = "radixAutoButton";
            this.radixAutoButton.Size = new System.Drawing.Size(30, 15);
            this.radixAutoButton.TabIndex = 3;
            this.radixAutoButton.Text = "Auto";
            this.radixAutoButton.UseVisualStyleBackColor = false;
            // 
            // sidePaneHeaderPanel
            // 
            this.sidePaneHeaderPanel.Controls.Add(this.sidePaneOpenButton);
            this.sidePaneHeaderPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePaneHeaderPanel.Location = new System.Drawing.Point(200, 25);
            this.sidePaneHeaderPanel.Name = "sidePaneHeaderPanel";
            this.sidePaneHeaderPanel.Size = new System.Drawing.Size(20, 221);
            this.sidePaneHeaderPanel.TabIndex = 6;
            // 
            // sidePaneOpenButton
            // 
            this.sidePaneOpenButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.sidePaneOpenButton.Location = new System.Drawing.Point(0, 0);
            this.sidePaneOpenButton.Name = "sidePaneOpenButton";
            this.sidePaneOpenButton.Size = new System.Drawing.Size(20, 20);
            this.sidePaneOpenButton.TabIndex = 1;
            this.sidePaneOpenButton.TabStop = false;
            this.sidePaneOpenButton.Text = ">";
            this.sidePaneOpenButton.UseVisualStyleBackColor = true;
            // 
            // sideTreeView
            // 
            this.bookTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.bookTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bookTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookTreeView.Location = new System.Drawing.Point(0, 0);
            this.bookTreeView.Name = "sideTreeView";
            this.bookTreeView.Size = new System.Drawing.Size(200, 221);
            this.bookTreeView.TabIndex = 0;
            // 
            // sidePaneBodyPanel
            // 
            this.sidePaneBodyPanel.Controls.Add(this.bookTreeView);
            this.sidePaneBodyPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePaneBodyPanel.Location = new System.Drawing.Point(0, 25);
            this.sidePaneBodyPanel.Name = "sidePaneBodyPanel";
            this.sidePaneBodyPanel.Size = new System.Drawing.Size(150, 221);
            this.sidePaneBodyPanel.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(577, 246);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.sidePaneHeaderPanel);
            this.Controls.Add(this.sidePaneBodyPanel);
            this.Controls.Add(this.toolStrip);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Form1";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.trayMenuStrip.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.sidePaneHeaderPanel.ResumeLayout(false);
            this.sidePaneBodyPanel.ResumeLayout(false);
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
        private FlatButton radixBinButton;
        private FlatButton radixHexButton;
        private FlatButton radixDecButton;
        private FlatButton radixAutoButton;
        private System.Windows.Forms.ToolStripButton topMostButton;
        private FlatButton radixOctButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton pasteButton;
        private System.Windows.Forms.ToolStripButton insertButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton moveUpButton;
        private System.Windows.Forms.ToolStripButton moveDownButton;
        private System.Windows.Forms.ToolStripButton undoButton;
        private System.Windows.Forms.ToolStripButton redoButton;
        private FlatButton radixCharButton;
        private FlatButton radixKibiButton;
        private FlatButton radixSiButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Panel sidePaneHeaderPanel;
        private FlatButton sidePaneOpenButton;
        private Shapoco.Calctus.UI.Books.BookTreeView bookTreeView;
        private System.Windows.Forms.Panel sidePaneBodyPanel;
    }
}

