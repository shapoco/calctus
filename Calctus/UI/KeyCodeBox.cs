using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapoco.Calctus.UI {
    public class KeyCodeBox : Panel {
        public event EventHandler KeyCodeChanged;

        private CheckBox _winBox = new CheckBox();
        private CheckBox _altBox = new CheckBox();
        private CheckBox _ctrlBox = new CheckBox();
        private CheckBox _shiftBox = new CheckBox();
        private TextBox _keyCodeBox = new TextBox();

        private bool _win = false;
        private bool _alt = false;
        private bool _ctrl = false;
        private bool _shift = false;
        private Keys _keyCode = Keys.None;

        public KeyCodeBox() {
            if (this.DesignMode) {
                var label = new Label();
                label.Text = nameof(KeyCodeBox);
                this.Controls.Add(label);
                return;
            }

            _winBox.TabIndex = 0;
            _altBox.TabIndex = 1;
            _ctrlBox.TabIndex = 2;
            _shiftBox.TabIndex = 3;
            _keyCodeBox.TabIndex = 4;
            _winBox.Dock = DockStyle.Left;
            _altBox.Dock = DockStyle.Left;
            _ctrlBox.Dock = DockStyle.Left;
            _shiftBox.Dock = DockStyle.Left;
            _keyCodeBox.Dock = DockStyle.Fill;
            _winBox.AutoSize = false;
            _altBox.AutoSize = false;
            _ctrlBox.AutoSize = false;
            _shiftBox.AutoSize = false;
            _keyCodeBox.AutoSize = false;
            _winBox.Text = "Win";
            _altBox.Text = "Alt";
            _ctrlBox.Text = "Ctrl";
            _shiftBox.Text = "Shift";
            _winBox.Size = _winBox.PreferredSize;
            _altBox.Size = _altBox.PreferredSize;
            _ctrlBox.Size = _ctrlBox.PreferredSize;
            _shiftBox.Size = _shiftBox.PreferredSize;
            this.Controls.Add(_keyCodeBox);
            this.Controls.Add(_shiftBox);
            this.Controls.Add(_ctrlBox);
            this.Controls.Add(_altBox);
            this.Controls.Add(_winBox);

            _winBox.CheckedChanged += (s, e) => { this.Win = ((CheckBox)s).Checked; };
            _altBox.CheckedChanged += (s, e) => { this.Alt = ((CheckBox)s).Checked; };
            _ctrlBox.CheckedChanged += (s, e) => { this.Ctrl = ((CheckBox)s).Checked; };
            _shiftBox.CheckedChanged += (s, e) => { this.Shift = ((CheckBox)s).Checked; };
            _keyCodeBox.KeyDown += _keyBox_KeyDown;
            _keyCodeBox.KeyUp += _keyBox_KeyUp;
            _keyCodeBox.KeyPress += _keyBox_KeyPress;
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

        public void SetKeyCode(bool win, bool alt, bool ctrl, bool shift, Keys keyCode) {
            bool changed = (win != _win) || (alt != _alt) || (ctrl != _ctrl) || (shift != _shift) || (keyCode != _keyCode);
            if (!changed) return;
            _winBox.Checked = _win = win;
            _altBox.Checked = _alt = alt;
            _ctrlBox.Checked = _ctrl = ctrl;
            _shiftBox.Checked = _shift = shift;
            _keyCode = keyCode;
            _keyCodeBox.Text = keyCode.ToString();
            KeyCodeChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool Win {
            get => _win;
            set => SetKeyCode(value, _alt, _ctrl, _shift, _keyCode);
        }

        public bool Alt {
            get => _alt;
            set => SetKeyCode(_win, value, _ctrl, _shift, _keyCode);
        }

        public bool Ctrl {
            get => _ctrl;
            set => SetKeyCode(_win, _alt, value, _shift, _keyCode);
        }

        public bool Shift {
            get => _shift;
            set => SetKeyCode(_win, _alt, _ctrl, value, _keyCode);
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

                SetKeyCode(_win, _alt, _ctrl, _shift, value);
            }
        }
    }
}
