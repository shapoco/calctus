using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    class CandidateForm : Form {
        private ICandidateProvider _provider;
        private ListBox _list = new ListBox();
        private Label _desc = new Label();

        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x40000000; // WS_CHILD
                cp.ExStyle |= 0x08000080; // WS_EX_NOACTIVATE
                return cp;
            }
        }

        public CandidateForm(ICandidateProvider provicer) {
            _provider = provicer;

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            Size = new Size(200, 200);
            Padding = new Padding(1, 1, 1, 1);
            DoubleBuffered = true;

            _list.Dock = DockStyle.Fill;
            _list.BorderStyle = BorderStyle.None;
            _list.IntegralHeight = false;
            _list.SelectedIndexChanged += _list_SelectedIndexChanged;
            Controls.Add(_list);
            
            _desc.Dock = DockStyle.Bottom;
            _desc.AutoSize = false;
            _desc.Height = 50;
            _desc.BackColor = Color.FromArgb(48, 48, 48);
            Controls.Add(_desc);
        }

        public void SetKey(string value) {
            string lastLabel = null;
            if (_list.SelectedIndex >= 0) {
                lastLabel = _list.Items[_list.SelectedIndex].ToString();
            }

            _list.Items.Clear();
            int selIndex = 0;

            // 先頭一致を探す
            foreach (var c in _provider.GetCandidates()) {
                if (c.Id.StartsWith(value, StringComparison.OrdinalIgnoreCase)) {
                    _list.Items.Add(c);
                    if (c.Id.Equals(value, StringComparison.OrdinalIgnoreCase) || c.Label == lastLabel) {
                        selIndex = _list.Items.Count - 1;
                    }
                }
            }

            // 先頭以外に一致するものを探す
            foreach (var c in _provider.GetCandidates()) {
                if (c.Id.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0 && _list.Items.IndexOf(c) == -1) {
                    _list.Items.Add(c);
                }
            }

            // 説明文に一致するものを探す
            foreach (var c in _provider.GetCandidates()) {
                if (c.Description.IndexOf(value, StringComparison.OrdinalIgnoreCase) >=- 0 && _list.Items.IndexOf(c) == -1) {
                    _list.Items.Add(c);
                }
            }

            if (selIndex < _list.Items.Count) {
                _list.SelectedIndex = selIndex;
            }
            else {
                _desc.Text = "";
            }
        }

        public Candidate SelectedItem {
            get {
                if(_list.SelectedIndex >= 0) {
                    return (Candidate)_list.Items[_list.SelectedIndex];
                }
                else {
                    return null;
                }
            }
        }

        public void SelectUp() {
            if (_list.Items.Count == 0) return;
            if (_list.SelectedIndex >= 0) {
                _list.SelectedIndex = (_list.SelectedIndex + _list.Items.Count - 1) % _list.Items.Count;
            }
            else {
                _list.SelectedIndex = _list.Items.Count - 1;
            }
        }

        public void SelectDown() {
            if (_list.Items.Count == 0) return;
            if (_list.SelectedIndex >= 0) {
                _list.SelectedIndex = (_list.SelectedIndex + 1) % _list.Items.Count;
            }
            else {
                _list.SelectedIndex = 0;
            }
        }

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            int fontSize = (int)Font.Size;
            Size = new Size(fontSize * 15, fontSize * 20);
            _desc.Height = fontSize * 5;
        }

        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            _list.BackColor = this.BackColor;
        }

        protected override void OnForeColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            _list.ForeColor = this.ForeColor;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var g = e.Graphics;
            g.DrawRectangle(Pens.Gray, new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1));
            g.DrawLine(Pens.Gray, 0, _list.Bottom + 10, ClientSize.Width, _list.Bottom - 10);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                _list.Dispose();
                _desc.Dispose();
            }
        }

        private void _list_SelectedIndexChanged(object sender, EventArgs e) {
            if (_list.SelectedIndex >= 0) {
                var c = (Candidate)_list.Items[_list.SelectedIndex];
                _desc.Text = c.Description;
            }
            else {
                _desc.Text = "";
            }
        }

    }
}
