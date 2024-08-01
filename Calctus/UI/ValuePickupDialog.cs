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
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Values;

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
        private bool _suppressTextBoxChanged = false;
        private Timer _textUpdateTimer = new Timer();
        private bool _isActive = false;

        public ValuePickupDialog(ExprBoxCore exprBox) {
            InitializeComponent();
            Instance = this;

            _textUpdateTimer.Enabled = false;
            _textUpdateTimer.Interval = 1000;

            _exprBox = exprBox;
            _oldExprText = exprBox.Text;

            joinOperatorComboBox.Items.AddRange(new string[] { "+", "*", "&", "|", "+|", "&&", "||", "," });
            foreach(var item in Enum.GetValues(typeof(ValuePickupFormatting))) {
                formattingComboBox.Items.Add((ValuePickupFormatting)item);
            }

            Windows.DwmApi.SetDarkModeEnable(this, Settings.Instance.GetIsDarkMode());

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            Load += ValueCollectionDialog_Load;
            FormClosed += ValueCollectionDialog_FormClosed;
            Activated += delegate { _isActive = true; };
            Deactivate += delegate { _isActive = false; };
            cancelButton.Click += CancelButton_Click;
            withOperatorRadioButton.CheckedChanged += delegate { _method = ValuePickupJoinMethod.WithOperator; updateExprBox(); };
            asArrayRadioButton.CheckedChanged += delegate { _method = ValuePickupJoinMethod.AsArray; updateExprBox(); };
            joinOperatorComboBox.TextChanged += delegate { updateExprBox(); };
            joinOperatorComboBox.SelectedIndexChanged += delegate { updateExprBox(); };
            removeCommaCheckBox.CheckedChanged += delegate { updateExprBox(); };
            formattingComboBox.TextChanged += delegate { updateExprBox(); };
            formattingComboBox.SelectedIndexChanged += delegate { updateExprBox(); };
            collectedItemsTextBox.TextChanged += CollectedItemsTextBox_TextChanged;
            _textUpdateTimer.Tick += _textUpdateTimer_Tick;
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
            formattingComboBox.SelectedItem = s.ValuePickup_ValueFormatting;
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
            s.ValuePickup_ValueFormatting = (ValuePickupFormatting)formattingComboBox.SelectedItem;
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
            if (_isActive) return;
            if ((DateTime.Now - _formLoadTime).TotalSeconds < 0.5) {
                // クリップボード監視開始後いきなり発生したイベントは無視する
                return;
            }
            try {
                var text = Clipboard.GetText()
                    .Replace("\r", " ")
                    .Replace("\n", " ")
                    .Replace("\t", " ")
                    .Trim();

                _collectedItems.Add(text);
                updateTextBox();
                updateExprBox();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void CollectedItemsTextBox_TextChanged(object sender, EventArgs e) {
            if (_suppressTextBoxChanged) return;
            _textUpdateTimer.Start();
        }

        private void _textUpdateTimer_Tick(object sender, EventArgs e) {
            _textUpdateTimer.Stop();
            _collectedItems.Clear();
            _collectedItems.AddRange(collectedItemsTextBox.Text.Split(
                new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            updateExprBox();
        }

        private void updateTextBox() {
            _suppressTextBoxChanged = true;
            collectedItemsTextBox.Text = String.Join("\r\n", _collectedItems);
            _suppressTextBoxChanged = false;
        }

        private void updateExprBox() {
            string selText = "";
            try {
                var sb = new StringBuilder();
                var delimiter = _method == ValuePickupJoinMethod.AsArray ? ", " : (" " + joinOperatorComboBox.Text + " ");
                foreach (var item in _collectedItems) {
                    var str = item;
                    if (sb.Length > 0) sb.Append(delimiter);
                    if (removeCommaCheckBox.Checked) {
                        str = str.Replace(",", "");
                    }
                    switch ((ValuePickupFormatting)formattingComboBox.SelectedItem) {
                        case ValuePickupFormatting.NoChange:
                            str = str.Trim();
                            break;
                        case ValuePickupFormatting.String:
                            str = Formatter.StringToCStyleLiteral(str, ToStringArgs.ForLiteral());
                            break;
                        case ValuePickupFormatting.DateTime:
                            str = DateTimeFormat.FormatAsStringLiteral(DateTime.Parse(str.Trim()), true);
                            break;
                        case ValuePickupFormatting.TimeSpan:
                            str = TimeSpanFormat.FormatAsStringLiteral((decimal)TimeSpan.Parse(str.Trim()).TotalSeconds, true);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    sb.Append(str);
                }

                if (_method == ValuePickupJoinMethod.AsArray) {
                    selText = "[" + sb.ToString() + "]";
                }
                else {
                    selText = sb.ToString();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(nameof(updateExprBox) + " : " + ex.Message);
            }

            var selStart = _exprBox.SelectionStart;
            _exprBox.SelectedText = selText;
            _exprBox.SelectionStart = selStart;
            _exprBox.SelectionLength = selText.Length;
        }
    }
}
