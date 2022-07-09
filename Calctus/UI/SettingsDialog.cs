using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapoco.Calctus.UI {
    public partial class SettingsDialog : Form {
        public SettingsDialog() {
            InitializeComponent();

            try {
                this.Font = new Font("Meiryo UI", SystemFonts.DefaultFont.Size);
            }
            catch { }

            var s = Settings.Instance;
            NumberFormat_Exp_Enabled.CheckedChanged += delegate { 
                s.NumberFormat_Exp_Enabled = NumberFormat_Exp_Enabled.Checked;
                eNoteGroup.Enabled = s.NumberFormat_Exp_Enabled;
            };
            NumberFormat_Exp_NegativeMax.ValueChanged += delegate { s.NumberFormat_Exp_NegativeMax = (int)NumberFormat_Exp_NegativeMax.Value; };
            NumberFormat_Exp_PositiveMin.ValueChanged += delegate { s.NumberFormat_Exp_PositiveMin = (int)NumberFormat_Exp_PositiveMin.Value; };
            NumberFormat_Exp_Alignment.CheckedChanged += delegate { s.NumberFormat_Exp_Alignment = NumberFormat_Exp_Alignment.Checked; };

            closeButton.Click += delegate { this.Close(); };
            this.FormClosed += SettingsDialog_FormClosed;
            this.Load += SettingsDialog_Load;
        }

        private void SettingsDialog_Load(object sender, EventArgs e) {
            var s = Settings.Instance;
            try {
                NumberFormat_Exp_Enabled.Checked = s.NumberFormat_Exp_Enabled;
                NumberFormat_Exp_NegativeMax.Value = s.NumberFormat_Exp_NegativeMax;
                NumberFormat_Exp_PositiveMin.Value = s.NumberFormat_Exp_PositiveMin;
                NumberFormat_Exp_Alignment.Checked = s.NumberFormat_Exp_Alignment;
            }
            catch { }

        }

        private void SettingsDialog_FormClosed(object sender, FormClosedEventArgs e) {
            Settings.Instance.Save();
        }
    }
}
