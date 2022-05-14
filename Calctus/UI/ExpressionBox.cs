using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Shapoco.Calctus.UI
{
    class ExpressionBox : TextBox
    {
        private List<string> _history = new List<string>();
        private int _history_index = 0;
        private bool _history_changed = false;

        public ExpressionBox() {
            if (this.DesignMode) return;
            _history.Add(this.Text);
            this.TextChanged += ExpressionBox_TextChanged;
            this.KeyDown += ExpressionBox_KeyDown;
        }

        private void ExpressionBox_TextChanged(object sender, EventArgs e) {
            _history[_history_index] = this.Text;
            _history_changed = true;
        }

        private void ExpressionBox_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Return:
                    if (!_history_changed) {
                        _history.RemoveAt(_history_index);
                    }
                    _history_index = 0;
                    _history.Insert(0, this.Text);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (_history_index + 1 < _history.Count) {
                        _history_index += 1;
                        _history_changed = false;
                        this.Text = _history[_history_index];
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (_history_index > 0) {
                        _history_index -= 1;
                        _history_changed = false;
                        this.Text = _history[_history_index];
                        e.Handled = true;
                    }
                    break;

            }
        }
    }
}
