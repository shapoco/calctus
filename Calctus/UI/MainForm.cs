using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Shapoco.Calctus.Model;
using Shapoco.Calctus.Parser;

namespace Shapoco.Calctus.UI {
    internal partial class MainForm : Form {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private RadixMode _radixMode = RadixMode.Auto;
        private HotKey _hotkey = null;
        private bool _startup = true;
        private Timer _focusTimer = new Timer();
        private Point _startupWindowPos;
        private Size _startupWindowSize;

        class CustomProfessionalColors : ProfessionalColorTable {
            public override Color ToolStripGradientBegin { get { return Color.FromArgb(64, 64, 64); } }
            public override Color ToolStripGradientMiddle { get { return Color.FromArgb(56, 56, 56); } }
            public override Color ToolStripGradientEnd { get { return Color.FromArgb(48, 48, 48); } }
            public override Color ToolStripBorder { get { return Color.FromArgb(64, 64, 64); } }
        }

        public MainForm() {
            // フォントの設定が反映されたときにウィンドウサイズも変わってしまうので
            // 起動時のウィンドウサイズ設定値は先に保持しておいて最後に反映する
            var s = Settings.Instance;
            _startupWindowPos = new Point(s.Window_X, s.Window_Y);
            _startupWindowSize = new Size(s.Window_Width, s.Window_Height);

            InitializeComponent();
            if (this.DesignMode) return;

            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());

            this.Text = Application.ProductName + " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
            this.KeyPreview = true; 
            this.KeyDown += MainForm_KeyDown;
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;
            this.VisibleChanged += MainForm_VisibleChanged;
            this.FormClosing += MainForm_FormClosing;
            this.FormClosed += MainForm_FormClosed;
            this.Resize += MainForm_Resize;
            this.LocationChanged += MainForm_LocationChanged;

            notifyIcon.Text = Application.ProductName;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            calcListBox.RadixModeChanged += (sender, e) => { this.RadixMode = ((CalcListBox)sender).RadixMode; };
            
            radixAutoButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Auto); };
            radixDecButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Dec); };
            radixHexButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Hex); };
            radixBinButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Bin); };
            radixOctButton.CheckedChanged += (sender, e) => { RadixCheckedChanged((RadioButton)sender, RadixMode.Oct); };
            radixAutoButton.Checked = true;

            toolTip.SetToolTip(radixAutoButton, "Automatic (F9)");
            toolTip.SetToolTip(radixDecButton, "Decimal (F10)");
            toolTip.SetToolTip(radixHexButton, "Hexadecimal (F11)");
            toolTip.SetToolTip(radixBinButton, "Binary (F12)");
            toolTip.SetToolTip(radixOctButton, "Octal (F8)");

            settingsButton.Click += (sender, e) => { new SettingsDialog().ShowDialog(); reloadSettings(); };
            topMostButton.Click += TopMostButton_Click;
            helpButton.Click += (sender, e) => { System.Diagnostics.Process.Start(@"https://github.com/shapoco/calctus"); };

            contextOpen.Click += (sender, e) => { showForeground(); };
            contextExit.Click += (sender, e) => { Application.Exit(); };

            _focusTimer.Tick += _focusTimer_Tick;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            reloadSettings();

            try {
                if (Settings.Instance.Window_RememberPosition) {
                    // 見えない位置にウィンドウが表示されないよう、
                    // 座標がスクリーン内に含まれていることを確認する
                    foreach (var screen in Screen.AllScreens) {
                        if (screen.Bounds.Contains(_startupWindowPos)) {
                            this.Location = _startupWindowPos;
                            break;
                        }
                    }
                }
                this.Size = _startupWindowSize; 
            }
            catch { }
        }

        private void MainForm_Shown(object sender, EventArgs e) {
            if (_startup) {
                _startup = false;
                if (Settings.Instance.Startup_TrayIcon) {
                    // トレイアイコンが有効になっている場合は非表示状態で起動する
                    // Form_Load で実施しても効かないので、Form_Shown で 1回だけ実施する
                    this.Visible = false;
                }
            }
            refocus();
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e) {
            if (((Form)sender).Visible) {
                refocus();
            }
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

        private void MainForm_LocationChanged(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                var s = Settings.Instance;
                s.Window_X = this.Left;
                s.Window_Y = this.Top;
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
                calcListBox.Font = font_mono_large;
            }
            catch { }
            calcListBox.Recalc();
        }

        private void enableHotkey() {
            var s = Settings.Instance;
            if (s.Hotkey_Enabled && s.HotKey_KeyCode != Keys.None) {
                MOD_KEY mod = MOD_KEY.NONE;
                if (s.HotKey_Win) mod |= MOD_KEY.WIN;
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

        private void TopMostButton_Click(object sender, EventArgs e) {
            var btn = (ToolStripButton)sender;
            this.TopMost = !this.TopMost;
            if (this.TopMost) {
                btn.Image = Properties.Resources.ToolIcon_TopMostOn;
            }
            else {
                btn.Image = Properties.Resources.ToolIcon_TopMostOff;
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
            if (GetForegroundWindow() == this.Handle) {
                this.WindowState = FormWindowState.Minimized;
            }
            else {
                showForeground();
            }
        }
        
        private void showForeground() {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized) {
                this.WindowState = FormWindowState.Normal;
            }
            Microsoft.VisualBasic.Interaction.AppActivate(this.Text);
        }

        private void refocus() {
            // イベントハンドラ内でコントロールにフォーカスしてもなぜか反映されないことがあるため
            // タイマで遅延させてフォーカスを実施する
            _focusTimer.Stop();
            _focusTimer.Interval = 100;
            _focusTimer.Start();
        }

        private void _focusTimer_Tick(object sender, EventArgs e) {
            _focusTimer.Stop();
            calcListBox.Refocus();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.F9) {
                this.RadixMode = RadixMode.Auto;
            }
            else if (e.KeyCode == Keys.F10) {
                this.RadixMode = RadixMode.Dec;
            }
            else if (e.KeyCode == Keys.F11) {
                this.RadixMode = RadixMode.Hex;
            }
            else if (e.KeyCode == Keys.F12) {
                this.RadixMode = RadixMode.Bin;
            }
            else if (e.KeyCode == Keys.F8) {
                this.RadixMode = RadixMode.Oct;
            }
            else {
                e.SuppressKeyPress = false;
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
                    case RadixMode.Oct: radixOctButton.Checked = true; break;
                }
                calcListBox.RadixMode = value;
            }
        }

        private void RadixCheckedChanged(RadioButton btn, RadixMode mode) {
            if (btn.Checked) {
                this.RadixMode = mode;
            }
        }
    }
}
