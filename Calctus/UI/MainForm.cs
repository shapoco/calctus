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
using Shapoco.Calctus.Parser;

namespace Shapoco.Calctus.UI {
    internal partial class MainForm : Form {
        private char[] _selectionCancelChars;
        private RadixMode _radixMode = RadixMode.Auto;
        private bool _loadingExpressionFromHistory = false;
        private HistoryItem _lastItem = null;
        private HotKey _hotkey = null;

        class CustomProfessionalColors : ProfessionalColorTable {
            public override Color ToolStripGradientBegin { get { return Color.FromArgb(64, 64, 64); } }
            public override Color ToolStripGradientMiddle { get { return Color.FromArgb(56, 56, 56); } }
            public override Color ToolStripGradientEnd { get { return Color.FromArgb(48, 48, 48); } }
            public override Color ToolStripBorder { get { return Color.FromArgb(64, 64, 64); } }
        }

        public MainForm() {
            // 全選択された状態で入力されたら選択を解除する文字の一覧
            _selectionCancelChars = OpDef.AllSymbols
                .Select(p => p[0])
                .Distinct()
                .ToArray();

            InitializeComponent();
            if (this.DesignMode) return;

            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());

            historyBox.Items.Add(new HistoryItem());
            historyBox.SelectedIndex = 0;
            historyBox.SelectedIndexChanged += HistoryBox_SelectedIndexChanged;
            historyBox.KeyUp += HistoryBox_KeyUp;
            historyBox.MouseUp += HistoryBox_MouseUp;

            historyMenuCopyText.Click += HistoryMenuCopyText_Click;
            historyMenuCopyAnswer.Click += HistoryMenuCopyAnswer_Click;
            historyMenuCopyAll.Click += HistoryMenuCopyAll_Click;
            historyMenuMoveUp.Click += HistoryMenuMoveUp_Click;
            historyMenuMoveDown.Click += HistoryMenuMoveDown_Click;
            historyMenuInsert.Click += HistoryMenuInsert_Click;
            historyMenuDelete.Click += HistoryMenuDelete_Click;
            historyMenuDeleteAll.Click += HistoryMenuDeleteAll_Click;

            this.Text = Application.ProductName + " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            this.KeyPreview = true; 
            this.KeyDown += MainForm_KeyDown;
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;
            this.FormClosing += MainForm_FormClosing;
            this.FormClosed += MainForm_FormClosed;
            this.Resize += MainForm_Resize;

            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            exprBox.AutoSize = false;
            exprBox.Dock = DockStyle.Fill;
            exprBox.KeyPress += ExprBox_KeyPress;
            exprBox.TextChanged += ExprBox_TextChanged;
            exprBox.KeyDown += ExpressionBox_KeyDown;
            
            calcButton.Click += CalcButton_Click;
            
            radixAutoButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Auto); };
            radixDecButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Dec); };
            radixHexButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Hex); };
            radixBinButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Bin); };
            radixAutoButton.Checked = true;

            settingsButton.Click += (sender, e) => { new SettingsDialog().ShowDialog(); reloadSettings(); };
            helpButton.Click += (sender, e) => { System.Diagnostics.Process.Start(@"https://github.com/shapoco/calctus"); };

            contextOpen.Click += (sender, e) => { showForeground(); };
            contextExit.Click += (sender, e) => { Application.Exit(); };

            subAnswerLabel.Text = "";
        }

        private void MainForm_Load(object sender, EventArgs e) {
            // フォントの設定が反映されたときにウィンドウサイズも変わってしまうので
            // 起動時のウィンドウサイズ設定値は先に保持しておいて最後に反映する
            var s = Settings.Instance;
            Size startupWindowSize = new Size(s.Window_Width, s.Window_Height);
            Console.WriteLine("Load " + s.Window_Width + "x" + s.Window_Height);

            reloadSettings();

            try { this.Size = startupWindowSize; }
            catch { }
        }

        private void MainForm_Shown(object sender, EventArgs e) {
            exprBox.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            var s = Settings.Instance;
            if (e.CloseReason == CloseReason.UserClosing && s.Startup_TrayIcon) {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            notifyIcon.Visible = false;
            disableHotkey();
        }

        private void MainForm_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                var s = Settings.Instance;
                s.Window_Width = this.Width;
                s.Window_Height = this.Height;
            }
        }

        private void reloadSettings() {
            try {
                var s = Settings.Instance;

                notifyIcon.Visible = s.Startup_TrayIcon;

                disableHotkey();
                enableHotkey();

                var font_large_coeff = 1.25f;
                var font_style = s.Appearance_Font_Bold ? FontStyle.Bold : FontStyle.Regular;
                var font_ui_normal = new Font(s.Appearance_Font_Button_Name, s.Appearance_Font_Size, font_style);
                var font_mono_normal = new Font(s.Appearance_Font_Expr_Name, s.Appearance_Font_Size, font_style);
                var font_mono_large = new Font(s.Appearance_Font_Expr_Name, s.Appearance_Font_Size * font_large_coeff, font_style);
                this.Font = font_ui_normal;
                historyBox.Font = font_mono_large;
                exprBox.Font = font_mono_large;
                subAnswerLabel.Font = font_mono_normal;
            }
            catch { }

            Recalc();
        }

        private void enableHotkey() {
            var s = Settings.Instance;
            if (s.Hotkey_Enabled && s.HotKey_KeyCode != Keys.None) {
                MOD_KEY mod = MOD_KEY.NONE;
                if (s.HotKey_Alt) mod |= MOD_KEY.ALT;
                if (s.HotKey_Ctrl) mod |= MOD_KEY.CONTROL;
                if (s.HotKey_Shift) mod |= MOD_KEY.SHIFT;
                _hotkey = new HotKey(mod, s.HotKey_KeyCode);
                _hotkey.HotKeyPush += _hotkey_HotKeyPush;
            }
        }

        private void disableHotkey() {
            if (_hotkey != null) {
                _hotkey.HotKeyPush -= _hotkey_HotKeyPush;
                _hotkey.Dispose();
                _hotkey = null;
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                showForeground();
            }
            else {
                trayMenuStrip.Show(Cursor.Position);
            }
        }

        private void _hotkey_HotKeyPush(object sender, EventArgs e) {
            showForeground();
        }
        
        private void showForeground() {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized) {
                this.WindowState = FormWindowState.Normal;
            }
            Microsoft.VisualBasic.Interaction.AppActivate(this.Text);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            e.SuppressKeyPress = true;
            switch(e.KeyCode) {
                case Keys.F9: this.RadixMode = RadixMode.Auto; break;
                case Keys.F10: this.RadixMode = RadixMode.Dec; break;
                case Keys.F11: this.RadixMode = RadixMode.Hex; break;
                case Keys.F12: this.RadixMode = RadixMode.Bin; break;
                default: e.SuppressKeyPress = false; break;
            }
        }

        public RadixMode RadixMode {
            get => _radixMode;
            set {
                if (value == _radixMode) return;
                _radixMode = value;
                switch(value) {
                    case RadixMode.Auto: radixAutoButton.Checked = true; break;
                    case RadixMode.Dec: radixDecButton.Checked = true; break;
                    case RadixMode.Hex: radixHexButton.Checked = true; break;
                    case RadixMode.Bin: radixBinButton.Checked = true; break;
                }
                Recalc();
            }
        }

        private void HistoryBox_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                historyMenuStrip.Show(Cursor.Position);
            }
        }

        private void HistoryBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Apps) {
                e.Handled = true;

                int index = historyBox.SelectedIndex;
                Point menuPos;
                if (index >= 0) {
                    var itemRect = historyBox.GetItemRectangle(index);
                    menuPos = historyBox.PointToScreen(new Point(itemRect.X, itemRect.Bottom));
                }
                else {
                    var clientRect = historyBox.ClientRectangle;
                    menuPos = historyBox.PointToScreen(new Point(clientRect.X, clientRect.Bottom)); ;
                }
                historyMenuStrip.Show(menuPos);
            }
        }

        private void HistoryBox_SelectedIndexChanged(object sender, EventArgs e) {
            updateHistoryMenu();
            if (historyBox.SelectedIndex < 0) return;
            var item = historyBox.SelectedHistoryItem;
            _lastItem?.Deselected();
            _lastItem = item;
            _loadingExpressionFromHistory = true;
            exprBox.Text = item.Expression;
            this.RadixMode = item.RadixMode;
            _loadingExpressionFromHistory = false;
            exprBox.SelectAll();
        }

        private void updateHistoryMenu() {
            var itemSelected = historyBox.SelectedIndex >= 0;
            var itemsExist = historyBox.Items.Count > 0;
            historyMenuCopyText.Enabled = itemSelected;
            historyMenuCopyAnswer.Enabled = itemSelected;
            historyMenuCopyAll.Enabled = itemsExist;
            historyMenuMoveUp.Enabled = itemSelected;
            historyMenuMoveDown.Enabled = itemSelected;
            historyMenuInsert.Enabled = true;
            historyMenuDelete.Enabled = itemSelected;
            historyMenuDeleteAll.Enabled = itemsExist;
        }

        private void HistoryMenuCopyText_Click(object sender, EventArgs e) {
            var item = historyBox.SelectedHistoryItem;
            if (item != null) {
                Clipboard.Clear();
                Clipboard.SetText(item.Expression + " = " + item.Answer);
            }
        }

        private void HistoryMenuCopyAnswer_Click(object sender, EventArgs e) {
            var item = historyBox.SelectedHistoryItem;
            if (item != null) {
                Clipboard.Clear();
                Clipboard.SetText(item.Answer);
            }
        }

        private void HistoryMenuCopyAll_Click(object sender, EventArgs e) {
            if (historyBox.Items.Count == 0) return;
            var sb = new StringBuilder();
            for(int i = 0; i < historyBox.Items.Count; i++) {
                var item = historyBox[i];
                sb.Append(item.Expression).Append(" = ").AppendLine(item.Answer);
            }
            Clipboard.Clear();
            Clipboard.SetText(sb.ToString());
        }

        private void HistoryMenuMoveUp_Click(object sender, EventArgs e) {
            var index = historyBox.SelectedIndex;
            if (index < 1) return;
            var temp = historyBox[index];
            historyBox.Items.RemoveAt(index);
            historyBox.Items.Insert(index - 1, temp);
            historyBox.SelectedIndex = index - 1;
            Recalc();
        }

        private void HistoryMenuMoveDown_Click(object sender, EventArgs e) {
            var index = historyBox.SelectedIndex;
            if (index > historyBox.Items.Count - 2) return;
            var temp = historyBox[index];
            historyBox.Items.RemoveAt(index);
            historyBox.Items.Insert(index + 1, temp);
            historyBox.SelectedIndex = index + 1;
            Recalc();
        }

        private void HistoryMenuInsert_Click(object sender, EventArgs e) {
            var index = historyBox.SelectedIndex;
            HistoryItem newItem;
            if (index < 0) {
                index = historyBox.Items.Count;
                if (historyBox.Items.Count == 0) {
                    newItem = new HistoryItem();
                }
                else {
                    newItem = new HistoryItem(historyBox[historyBox.Items.Count - 1]);
                }
            }
            else if (index == 0) {
                newItem = new HistoryItem();
            }
            else {
                newItem = new HistoryItem(historyBox[index-1]);
            }
            historyBox.Items.Insert(index, newItem);
            historyBox.SelectedIndex = index;
            Recalc();
        }

        private void HistoryMenuDelete_Click(object sender, EventArgs e) {
            int index = historyBox.SelectedIndex;
            if (index >= 0) {
                historyBox.Items.RemoveAt(index);
                Recalc();
            }
        }

        private void HistoryMenuDeleteAll_Click(object sender, EventArgs e) {
            var ans = MessageBox.Show("Are you sure you want to delete all?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (ans == DialogResult.OK) {
                historyBox.Items.Clear();
                Recalc();
                updateHistoryMenu();
            }
        }

        private void RadixCheckedChanged(RadioButton btn, RadixMode mode) {
            if (_loadingExpressionFromHistory) return;
            if (btn.Checked) {
                this.RadixMode = mode;
            }
            exprBox.Focus();
        }

        private void ExprBox_TextChanged(object sender, EventArgs e) {
            if (_loadingExpressionFromHistory) return;
            var item = historyBox.SelectedHistoryItem;
            if (item != null) {
                item.Expression = exprBox.Text;
            }
            Recalc();
        }

        private void ExprBox_KeyPress(object sender, KeyPressEventArgs e) {
            var item = historyBox.SelectedHistoryItem;
            if (_selectionCancelChars.Contains(e.KeyChar) && item != null) {
                var allSelected = exprBox.SelectionLength > 0 && exprBox.SelectionStart == 0 && exprBox.SelectionLength == exprBox.TextLength;
                if (item.IsFreshAnswer && allSelected) {
                    // 直前の式の評価値が全選択された状態で演算子が入力されたら、
                    // 値を Ans に置換し、選択を解除して末尾にカーソル移動
                    exprBox.SelectedText = "Ans";
                    exprBox.SelectionStart = exprBox.TextLength;
                }
            }
            else if (e.KeyChar == (char)Keys.Return) {
                e.Handled = true;
            }
        }

        private void ExpressionBox_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Return:
                    OnReturnPressed(e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (historyBox.SelectedIndex > 0) {
                        historyBox.SelectedIndex -= 1;
                    }
                    e.Handled = true;
                    break;
                case Keys.Down:
                    if (historyBox.SelectedIndex < historyBox.Items.Count - 1) {
                        historyBox.SelectedIndex += 1;
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void OnReturnPressed(bool insert) {
            if (historyBox.SelectedIndex >= 0) {
                // 数式をヒストリに反映
                historyBox.SelectedHistoryItem.Expression = exprBox.Text;
            }
            else {
                // 選択されていない場合は新規作成
                var newItem = new HistoryItem();
                newItem.Expression = exprBox.Text;
                newItem.RadixMode = this.RadixMode;
                historyBox.Items.Add(newItem);
                historyBox.SelectedIndex = historyBox.Items.Count - 1;
                Recalc();
            }

            // 次のアイテムを選択する
            int nextIndex;
            if (insert) {
                nextIndex = historyBox.SelectedIndex;
            }
            else {
                nextIndex = historyBox.SelectedIndex + 1;
            }

            bool append = nextIndex >= historyBox.Items.Count;
            if (append || insert) {
                // 必要に応じて新しい履歴の要素を追加する
                HistoryItem newItem;
                if (nextIndex > 0) {
                    // 直前に履歴が存在する場合はその評価値と基数を引き継ぐ
                    var prevItem = historyBox[nextIndex - 1];
                    newItem = new HistoryItem(prevItem);
                }
                else {
                    newItem = new HistoryItem();
                }
                historyBox.Items.Insert(nextIndex, newItem);
            }

            historyBox.SelectedIndex = nextIndex;

            if (append) {
                exprBox.SelectAll();
            }
        }

        private void Recalc() {
#if DEBUG
            Console.WriteLine("Recalc()");
#endif
            EvalContext ctx = new EvalContext();
            for (int i = 0; i < historyBox.Items.Count; i++) {
                var item = historyBox[i];
                try {
                    item.Answer = "";
                    if (i == historyBox.SelectedIndex) {
                        item.RadixMode = this.RadixMode;
                    }
                    var expr = Parser.Parser.Parse(item.Expression);
                    var val = expr.Eval(ctx);

                    switch (item.RadixMode) {
                        case RadixMode.Dec: val = val.FormatInt(); break;
                        case RadixMode.Hex: val = val.FormatHex(); break;
                        case RadixMode.Bin: val = val.FormatBin(); break;
                    }

                    var valStr = val.ToString();
                    var hintStr = "";
                    if (val is RealVal realVal) {
                        if (realVal.IsDimless) {
                            var doubleVal = realVal.AsDouble;
                            if (realVal.IsInteger && (doubleVal < 0 || 10 <= doubleVal && doubleVal <= int.MaxValue)) {
                                // 10以上の整数については 10進と 16進を併記する
                                if (realVal.FormatHint.Formatter == Shapoco.Calctus.Model.Syntax.IntFormatter.CStyleInt) {
                                    hintStr = realVal.FormatHex().ToString();
                                }
                                else {
                                    hintStr = realVal.FormatInt().ToString();
                                }
                            }
                        }
                    }

                    item.Answer = valStr;
                    item.Error = false;
                    item.Hint = hintStr;
                    ctx.Ref("Ans", true).Value = val;

                    if (i == historyBox.SelectedIndex) {
                        subAnswerLabel.Text = "= " + valStr;
                    }
                }
                catch (Exception ex) {
                    item.Error = true;
                    if (i == historyBox.SelectedIndex) {
                        subAnswerLabel.Text = ex.Message;
                    }
                    item.Hint = ex.Message;
                }
            }
            historyBox.Invalidate();
        }

        private void CalcButton_Click(object sender, EventArgs e) {
            try {
                OnReturnPressed(false);
                exprBox.Focus();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
