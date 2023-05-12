using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.UI {
    partial class ScriptFilterEditForm : Form {
        private ScriptFilter _target;
        private OpenFileDialog _fileDialog = new OpenFileDialog();

        public ScriptFilterEditForm() {
            InitializeComponent();

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            _fileDialog.Filter = "Executables (*.exe)|*.exe|All Files (*.*)|*.*";

            filter.TextChanged += TextBox_TextChanged;
            executable.TextChanged += TextBox_TextChanged;
            parameter.TextChanged += TextBox_TextChanged;
            exeBrowseButton.Click += ExeBrowseButton_Click;
            closeButton.Click += CloseButton_Click;
        }

        public ScriptFilter Target {
            get => _target;
            set {
                if (_target == value) return;
                _target = value;
                if (_target != null) {
                    filter.Text = _target.Filter;
                    executable.Text = _target.Command;
                    parameter.Text = _target.Parameter;
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e) {
            executable.Enabled = exeBrowseButton.Enabled = (filter.Text != ScriptFilter.ExeFilter);
            closeButton.Enabled = !string.IsNullOrEmpty(filter.Text.Trim());
        }

        private void ExeBrowseButton_Click(object sender, EventArgs e) {
            _fileDialog.FileName = executable.Text;
            if (_fileDialog.ShowDialog() == DialogResult.OK) {
                executable.Text = _fileDialog.FileName;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) {
            if (!closeButton.Enabled) return;
            if (_target == null) return;
            _target.Filter = filter.Text.Trim();
            _target.Command = executable.Text.Trim();
            _target.Parameter = parameter.Text.Trim();
        }
    }
}
