using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapoco.Calctus.UI {
    public class KeyCodeBox : Panel {
        public event EventHandler KeyCodeChanged;

        private CheckBox _altKey = new CheckBox();
        private CheckBox _ctrlKey = new CheckBox();
        private CheckBox _shiftKey = new CheckBox();
        private TextBox _keyBox = new TextBox();

        private Keys _keyCode = Keys.None;

        public KeyCodeBox() {
            if (this.DesignMode) {
                var label = new Label();
                label.Text = nameof(KeyCodeBox);
                this.Controls.Add(label);
                return;
            }

            _altKey.Dock = DockStyle.Left;
            _ctrlKey.Dock = DockStyle.Left;
            _shiftKey.Dock = DockStyle.Left;
            _keyBox.Dock = DockStyle.Fill;
            _altKey.AutoSize = false;
            _ctrlKey.AutoSize = false;
            _shiftKey.AutoSize = false;
            _keyBox.AutoSize = false;
            _altKey.Text = "Alt";
            _ctrlKey.Text = "Ctrl";
            _shiftKey.Text = "Shift";
            _altKey.Size = _altKey.PreferredSize;
            _ctrlKey.Size = _ctrlKey.PreferredSize;
            _shiftKey.Size = _shiftKey.PreferredSize;
            this.Controls.Add(_keyBox);
            this.Controls.Add(_shiftKey);
            this.Controls.Add(_ctrlKey);
            this.Controls.Add(_altKey);

            _altKey.CheckedChanged += (s, e) => { this.Alt = ((CheckBox)s).Checked; };
            _ctrlKey.CheckedChanged += (s, e) => { this.Ctrl = ((CheckBox)s).Checked; };
            _shiftKey.CheckedChanged += (s, e) => { this.Shift = ((CheckBox)s).Checked; };
            _keyBox.KeyDown += _keyBox_KeyDown;
            _keyBox.KeyUp += _keyBox_KeyUp;
            _keyBox.KeyPress += _keyBox_KeyPress;
        }

        private void _keyBox_KeyDown(object sender, KeyEventArgs e) {
            e.Handled = true;
            this.KeyCode = e.KeyCode;
        }

        private void _keyBox_KeyUp(object sender, KeyEventArgs e) {
            e.Handled = true;
        }

        private void _keyBox_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = true;
        }

        public bool Alt {
            get => _altKey.Checked;
            set {
                if (_altKey.Checked == value) return;
                _altKey.Checked = value;
            }
        }

        public bool Ctrl {
            get => _ctrlKey.Checked;
            set {
                if (_ctrlKey.Checked == value) return;
                _ctrlKey.Checked = value;
            }
        }

        public bool Shift {
            get => _shiftKey.Checked;
            set {
                if (_shiftKey.Checked == value) return;
                _shiftKey.Checked = value;
            }
        }

        public Keys KeyCode {
            get => _keyCode;
            set {
                var ignoreKeys = new Keys[] {
                    Keys.Menu, Keys.ControlKey, Keys.ShiftKey, 
                    Keys.Back, Keys.Delete, Keys.Escape
                };
                if (ignoreKeys.Contains(value)) {
                    value = Keys.None;
                }

                if (value == _keyCode) return;

                _keyCode = value;
                _keyBox.Text = value.ToString();
                KeyCodeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
