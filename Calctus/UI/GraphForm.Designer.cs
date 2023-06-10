namespace Shapoco.Calctus.UI {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.invertBrightnessButton = new System.Windows.Forms.ToolStripButton();
            this.graphPanel = new Shapoco.Calctus.UI.GraphPanel();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyButton,
            this.invertBrightnessButton});
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
            // 
            // invertBrightnessButton
            // 
            this.invertBrightnessButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.invertBrightnessButton.Image = global::Shapoco.Properties.Resources.ToolIcon_InvertBrightness;
            this.invertBrightnessButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.invertBrightnessButton.Name = "invertBrightnessButton";
            this.invertBrightnessButton.Size = new System.Drawing.Size(23, 22);
            this.invertBrightnessButton.Text = "Invert Brightness";
            // 
            // graphPanel
            // 
            this.graphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphPanel.InvertBrightness = false;
            this.graphPanel.Location = new System.Drawing.Point(0, 25);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Size = new System.Drawing.Size(380, 211);
            this.graphPanel.TabIndex = 0;
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(380, 236);
            this.Controls.Add(this.graphPanel);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GraphForm";
            this.Text = "GraphForm";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GraphPanel graphPanel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton invertBrightnessButton;
    }
}