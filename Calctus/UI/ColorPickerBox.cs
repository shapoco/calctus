using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Drawings;

namespace Shapoco.Calctus.UI {
    class ColorPickerBox : Panel {
        private static readonly ColorDialog _colorDialog = new ColorDialog();

        public event EventHandler SelectedColorChanged;

        private TextBox _codeTextBox = new TextBox();
        private Button _browseButton = new Button();
        private bool _suppressTextUpdate = false;
        private Color _selectedColor;

        public ColorPickerBox() {
            _codeTextBox.Dock = DockStyle.Fill;
            _codeTextBox.TextAlign = HorizontalAlignment.Center;
            _browseButton.Dock = DockStyle.Right;
            _browseButton.Text = "...";
            _browseButton.TabStop = false;

            _codeTextBox.TextChanged += TextBox_TextChanged;
            _codeTextBox.GotFocus += (sender, e) => { ((TextBox)sender).SelectAll(); };
            _browseButton.Click += _browseButton_Click;

            this.Controls.Add(_codeTextBox);
            this.Controls.Add(_browseButton);
        }

        public Color SelectedColor {
            get => _selectedColor;
            set {
                value = Color.FromArgb(255, value);
                if (value == _selectedColor) return;
                _selectedColor = value;
                selectedColorToBackColor();
                if (!_suppressTextUpdate) {
                    var hexStr = "000000" + Convert.ToString(value.ToArgb(), 16);
                    _codeTextBox.Text = "#" + hexStr.Substring(hexStr.Length - 6).ToUpper();
                }
                SelectedColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnResize(EventArgs eventargs) {
            base.OnResize(eventargs);
            _browseButton.Width = Math.Min(this.Width / 2, this.Height * 4 / 3);
        }

        private void TextBox_TextChanged(object sender, EventArgs e) {
            try {
                var str = _codeTextBox.Text.Trim();
                if (str.StartsWith("#")) str = str.Substring(1);
                if (str.Length == 6) {
                    var color = Color.FromArgb(255, Color.FromArgb( Convert.ToInt32(str, 16)));
                    _suppressTextUpdate = true;
                    SelectedColor = color;
                    _suppressTextUpdate = false;
                    selectedColorToBackColor();
                }
                else if (str.Length == 3) {
                    var r = Convert.ToInt32(str.Substring(0, 1), 16);
                    var g = Convert.ToInt32(str.Substring(1, 1), 16);
                    var b = Convert.ToInt32(str.Substring(2, 1), 16);
                    var color = Color.FromArgb(255, (r << 4) | r, (g << 4) | g, (b << 4) | b);
                    _suppressTextUpdate = true;
                    SelectedColor = color;
                    _suppressTextUpdate = false;
                    selectedColorToBackColor();
                }
                else {
                    _codeTextBox.BackColor = SystemColors.Control;
                    _codeTextBox.ForeColor = Color.Red;
                }
            }
            catch {
                _codeTextBox.BackColor = SystemColors.Control;
                _codeTextBox.ForeColor = Color.Red;
                _suppressTextUpdate = false;
            }
        }

        private void _browseButton_Click(object sender, EventArgs e) {
            _colorDialog.Color = _selectedColor;
            if (_colorDialog.ShowDialog() == DialogResult.OK) {
                SelectedColor = _colorDialog.Color;
            }
        }

        private void selectedColorToBackColor() {
            _codeTextBox.BackColor = _selectedColor;
            var gray = ColorEx.GrayScale(_selectedColor);
            _codeTextBox.ForeColor = gray.R < 128 ? Color.White : Color.Black;
        }
    }
}
