﻿namespace Shapoco.Calctus.UI {
    partial class GraphForm {
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
            Shapoco.Calctus.Model.Graphs.AxisSettings axisSettings3 = new Shapoco.Calctus.Model.Graphs.AxisSettings();
            Shapoco.Calctus.Model.Graphs.AxisSettings axisSettings4 = new Shapoco.Calctus.Model.Graphs.AxisSettings();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.whiteBackModeButton = new System.Windows.Forms.ToolStripButton();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.axisSettingsY = new Shapoco.Calctus.UI.AxisSettingsPanel();
            this.axisSettingsX = new Shapoco.Calctus.UI.AxisSettingsPanel();
            this.graphPanel = new Shapoco.Calctus.UI.GraphPanel();
            this.toolStrip.SuspendLayout();
            this.sidePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyButton,
            this.whiteBackModeButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(380, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Image = global::Shapoco.Properties.Resources.ToolIcon_Copy;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(23, 22);
            this.copyButton.Text = "Copy";
            this.copyButton.ToolTipText = "Copy Image";
            // 
            // whiteBackModeButton
            // 
            this.whiteBackModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.whiteBackModeButton.Image = global::Shapoco.Properties.Resources.ToolIcon_InvertBrightness;
            this.whiteBackModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.whiteBackModeButton.Name = "whiteBackModeButton";
            this.whiteBackModeButton.Size = new System.Drawing.Size(23, 22);
            this.whiteBackModeButton.Text = "Invert Color";
            // 
            // panel1
            // 
            this.sidePanel.Controls.Add(this.axisSettingsY);
            this.sidePanel.Controls.Add(this.axisSettingsX);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 25);
            this.sidePanel.Name = "panel1";
            this.sidePanel.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.sidePanel.Size = new System.Drawing.Size(100, 211);
            this.sidePanel.TabIndex = 4;
            // 
            // axisSettingsY
            // 
            axisSettings3.PosBottom = new decimal(new int[] {
            105,
            0,
            0,
            -2147418112});
            axisSettings3.PosRange = new decimal(new int[] {
            21,
            0,
            0,
            0});
            axisSettings3.Type = Shapoco.Calctus.Model.Graphs.AxisType.Linear;
            this.axisSettingsY.AxisSettings = axisSettings3;
            this.axisSettingsY.Dock = System.Windows.Forms.DockStyle.Top;
            this.axisSettingsY.Location = new System.Drawing.Point(0, 92);
            this.axisSettingsY.Name = "axisSettingsY";
            this.axisSettingsY.Size = new System.Drawing.Size(95, 94);
            this.axisSettingsY.TabIndex = 1;
            // 
            // axisSettingsX
            // 
            axisSettings4.PosBottom = new decimal(new int[] {
            105,
            0,
            0,
            -2147418112});
            axisSettings4.PosRange = new decimal(new int[] {
            21,
            0,
            0,
            0});
            axisSettings4.Type = Shapoco.Calctus.Model.Graphs.AxisType.Linear;
            this.axisSettingsX.AxisSettings = axisSettings4;
            this.axisSettingsX.Dock = System.Windows.Forms.DockStyle.Top;
            this.axisSettingsX.Location = new System.Drawing.Point(0, 0);
            this.axisSettingsX.Name = "axisSettingsX";
            this.axisSettingsX.Size = new System.Drawing.Size(95, 92);
            this.axisSettingsX.TabIndex = 0;
            // 
            // graphPanel
            // 
            this.graphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphPanel.Location = new System.Drawing.Point(100, 25);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Size = new System.Drawing.Size(280, 211);
            this.graphPanel.TabIndex = 0;
            this.graphPanel.WhiteBackMode = false;
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(380, 236);
            this.Controls.Add(this.graphPanel);
            this.Controls.Add(this.sidePanel);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphForm";
            this.ShowInTaskbar = false;
            this.Text = "GraphForm";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.sidePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GraphPanel graphPanel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton whiteBackModeButton;
        private AxisSettingsPanel axisSettingsX;
        private AxisSettingsPanel axisSettingsY;
        private System.Windows.Forms.Panel sidePanel;
    }
}