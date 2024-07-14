using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Windows;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI {
    partial class ValuePickupDialog : Form {
        public static ValuePickupDialog Instance { get; private set; } = null;
        public static void SetTopMost(bool value) {
            if (Instance != null && Instance.IsHandleCreated) {
                var hWndInsertAfter = value ? WinUser.HWND_TOPMOST : WinUser.HWND_NOTOPMOST;
                WinUser.SetWindowPos(Instance.Handle, hWndInsertAfter, 0, 0, 0, 0, WinUser.SWP_NOACTIVATE | WinUser.SWP_NOSIZE | WinUser.SWP_NOMOVE);
            }
        }

        private ExprBoxCore _exprBox;
        private ClipboardListener _clipListener;
        private DateTime _formLoadTime;
        private string _oldExprText;
        private ValuePickupJoinMethod _method = ValuePickupJoinMethod.WithOperator;
        private List<string> _collectedItems = new List<string>();

        public ValuePickupDialog(ExprBoxCore exprBox) {
            InitializeComponent();
            Instance = this;

            _exprBox = exprBox;
            _oldExprText = exprBox.Text;

            joinOperatorComboBox.Items.AddRange(new string[] { "+", "*", "&", "|", "+|", "&&", "||", "," });

            Windows.DwmApi.SetDarkModeEnable(this, Settings.Instance.GetIsDarkMode());

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            Load += ValueCollectionDialog_Load;
            FormClosed += ValueCollectionDialog_FormClosed;
            cancelButton.Click += CancelButton_Click;
            withOperatorRadioButton.CheckedChanged += delegate { _method = ValuePickupJoinMethod.WithOperator; updateExprBox(); };
            asArrayRadioButton.CheckedChanged += delegate { _method = ValuePickupJoinMethod.AsArray; updateExprBox(); };
            joinOperatorComboBox.TextChanged += delegate { updateExprBox(); };
            joinOperatorComboBox.SelectedIndexChanged += delegate { updateExprBox(); };
            removeCommaCheckBox.CheckedChanged += delegate { updateExprBox(); };
            valueAsStringCheckBox.CheckedChanged += delegate { updateExprBox(); };
        }

        private void ValueCollectionDialog_Load(object sender, EventArgs e) {
            _formLoadTime = DateTime.Now;
            var s = Settings.Instance;
            switch(s.ValuePickup_JoinMethod) {
                case ValuePickupJoinMethod.WithOperator: withOperatorRadioButton.Checked = true; break;
                case ValuePickupJoinMethod.AsArray: asArrayRadioButton.Checked = true; break;
            }
            joinOperatorComboBox.Text = s.ValuePickup_JoinOperator;
            removeCommaCheckBox.Checked = s.ValuePickup_RemoveComma;
            valueAsStringCheckBox.Checked = s.ValuePickup_ValueAsString;
            try {
                _clipListener = new ClipboardListener(this);
                _clipListener.ClipboardChanged += _clipListener_ClipboardChanged;
            }
            catch (Exception ex) {
                MessageBox.Show("Clipboard listen failed:\r\n\r\n" + ex.Message, 
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void ValueCollectionDialog_FormClosed(object sender, FormClosedEventArgs e) {
            var s = Settings.Instance;
            s.ValuePickup_JoinMethod = _method;
            s.ValuePickup_JoinOperator = joinOperatorComboBox.Text;
            s.ValuePickup_RemoveComma = removeCommaCheckBox.Checked;
            s.ValuePickup_ValueAsString = valueAsStringCheckBox.Checked;
            try {
                _clipListener.Dispose();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            Instance = null;
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            _exprBox.Text = _oldExprText;
        }

        private void _clipListener_ClipboardChanged(object sender, EventArgs e) {
            if ((DateTime.Now - _formLoadTime).TotalSeconds < 0.5) {
                // クリップボード監視開始後いきなり発生したイベントは無視する
                return;
            }
            try {
                _collectedItems.Add(Clipboard.GetText());
                updateExprBox();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void updateExprBox() {
            var sb = new StringBuilder();
            var delimiter = _method == ValuePickupJoinMethod.AsArray ? ", " : (" " + joinOperatorComboBox.Text +  " ");
            foreach (var item in _collectedItems) {
                var str = item;
                if (sb.Length > 0) sb.Append(delimiter);
                if (removeCommaCheckBox.Checked) {
                    str = str.Replace(",", "");
                }
                if (valueAsStringCheckBox.Checked) {
                    str = Model.Formats.StringFormat.FormatAsStringLiteral(str);
                }
                else {
                    str = str.Trim();
                }
                sb.Append(str);
            }

            string selText;
            if (_method == ValuePickupJoinMethod.AsArray) {
                selText = "[" + sb.ToString()+ "]";
            }
            else {
                selText = sb.ToString();
            }

            var selStart = _exprBox.SelectionStart;
            _exprBox.SelectedText = selText;
            _exprBox.SelectionStart = selStart;
            _exprBox.SelectionLength = selText.Length;
        }
    }
}
