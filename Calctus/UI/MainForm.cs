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
using System.IO;
using Shapoco.Drawings;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.UI.Books;
using Shapoco.Calctus.UI.Sheets;
using Shapoco.Calctus.Model.Functions;
using Shapoco.Calctus.Model.Functions.BuiltIns;

namespace Shapoco.Calctus.UI {
    internal partial class MainForm : Form {
        private static MainForm _instance = null;
        public static MainForm Instance => _instance;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private BookItem _activeBookItem = null;

        private HotKey _hotkey = null;
        private bool _startup = true;
        private Timer _focusTimer = new Timer();
        private Point _startupWindowPos;
        private Size _startupWindowSize;
        private bool _topMost = false;
        private FormWindowState _lastWindowState = FormWindowState.Normal;

        public MainForm() {
            _instance = this;

            // フォントの設定が反映されたときにウィンドウサイズも変わってしまうので
            // 起動時のウィンドウサイズ設定値は先に保持しておいて最後に反映する
            var s = Settings.Instance;

            // 設定保存時の DPI から起動時の DPI へのスケーリングを行う
            var startupScalingFactor = (float)this.DeviceDpi / Settings.Instance.Window_Dpi;
            _startupWindowPos = Point.Round(new PointF(
                s.Window_X * startupScalingFactor,
                s.Window_Y * startupScalingFactor
            ));
            _startupWindowSize = Size.Round(new SizeF(
                s.Window_Width * startupScalingFactor,
                s.Window_Height * startupScalingFactor
            ));

            InitializeComponent();
            if (this.DesignMode) return;

            sidePaneBodyPanel.Visible = false;

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.Load += MainForm_Load;
            this.Activated += MainForm_Activated;
            this.Deactivate += MainForm_Deactivate;
            this.Shown += MainForm_Shown;
            this.VisibleChanged += MainForm_VisibleChanged;
            this.FormClosing += MainForm_FormClosing;
            this.FormClosed += MainForm_FormClosed;
            this.Resize += MainForm_Resize;
            this.LocationChanged += MainForm_LocationChanged;

            if(Program.DebugMode) {
                // デバッグ実行中のウィンドウをタスクバー上で見分けられるようにする
                notifyIcon.Text = Application.ProductName + " (Debug)";
                this.Icon = Properties.Resources.Icon_Debug;
                this.notifyIcon.Icon = Properties.Resources.Icon_Debug;
            }
            else {
                notifyIcon.Text = Application.ProductName;

            }
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            var repFuncs = RepresentaionFuncs.Instance;
            radixAutoButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(null); _activeBookItem.View.Focus(); };
            radixDecButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.dec); _activeBookItem.View.Focus(); };
            radixHexButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.hex); _activeBookItem.View.Focus(); };
            radixBinButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.bin); _activeBookItem.View.Focus(); };
            radixOctButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.oct); _activeBookItem.View.Focus(); };
            radixSiButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.si); _activeBookItem.View.Focus(); };
            radixKibiButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.kibi); _activeBookItem.View.Focus(); };
            radixCharButton.Click += (sender, e) => { _activeBookItem.View.ReplaceFormatterFunction(repFuncs.char_1); _activeBookItem.View.Focus(); };

            toolTip.SetToolTip(radixAutoButton, "Automatic (F8)");
            toolTip.SetToolTip(radixDecButton, "Decimal (F9)");
            toolTip.SetToolTip(radixHexButton, "Hexadecimal (F10)");
            toolTip.SetToolTip(radixBinButton, "Binary (F11)");
            toolTip.SetToolTip(radixOctButton, "Octal");
            toolTip.SetToolTip(radixSiButton, "SI Prefix (F12)");
            toolTip.SetToolTip(radixKibiButton, "Binary Prefix");
            toolTip.SetToolTip(radixCharButton, "Character");

            undoButton.Click += (sender, e) => { _activeBookItem.View.Undo(); };
            redoButton.Click += (sender, e) => { _activeBookItem.View.Redo(); };
            copyButton.Click += (sender, e) => { _activeBookItem.View.Copy(); };
            pasteButton.Click += (sender, e) => { _activeBookItem.View.Paste(); };
            insertButton.Click += (sender, e) => { _activeBookItem.View.ItemInsert(); };
            deleteButton.Click += (sender, e) => { _activeBookItem.View.ItemDelete(); };
            moveUpButton.Click += (sender, e) => { _activeBookItem.View.ItemMoveUp(); };
            moveDownButton.Click += (sender, e) => { _activeBookItem.View.ItemMoveDown(); };

            sidePaneOpenButton.Click += SidePaneOpenButton_Click;
            bookTreeView.AfterSelect += (sender, e) => { onBookItemSelected(); };

            settingsButton.Click += SettingsButton_Click;
            topMostButton.Click += TopMostButton_Click;
            helpButton.Click += (sender, e) => { System.Diagnostics.Process.Start(@"https://github.com/shapoco/calctus"); };

            contextOpen.Click += (sender, e) => { showForeground(); };
            contextExit.Click += (sender, e) => { appExit(); };

            bookTreeView.ScratchPad.View.BeforeCleared += (sender, e) => { saveScratchPadToHistory(); };
            bookTreeView.SelectedNode = bookTreeView.ScratchPad;
            onBookItemSelected();

            _focusTimer.Tick += _focusTimer_Tick;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            reloadSettings();

#if DEBUG
            var testResultLabel = new Label();
            testResultLabel.Dock = DockStyle.Right;
            if (Test.NumErrors != 0 || Test.NumWarnings != 0 || Test.NumUntested != 0) {
                testResultLabel.Text = Test.NumSuccess + " Success";
                if (Test.NumErrors > 0) {
                    testResultLabel.Text += ", " + Test.NumErrors + " Errors";
                }
                if (Test.NumWarnings > 0) {
                    testResultLabel.Text += ", " + Test.NumWarnings + " Warnings";
                }
                if (Test.NumUntested > 0) {
                    testResultLabel.Text += ", " + Test.NumUntested + " Untested";
                }
            }
            else {
                testResultLabel.Text = "All Test Passed";
            }
            if (Test.NumErrors > 0) {
                testResultLabel.BackColor = Color.Red;
                testResultLabel.ForeColor = Color.White;
            }
            else if (Test.NumWarnings > 0) {
                testResultLabel.BackColor = Color.Yellow;
                testResultLabel.ForeColor = Color.Black;
            }
            else if (Test.NumUntested > 0) {
                testResultLabel.BackColor = Color.Magenta;
                testResultLabel.ForeColor = Color.Black;
            }
            else {
                testResultLabel.BackColor = Color.Lime;
                testResultLabel.ForeColor = Color.Black;
            }
            bottomPanel.Controls.Add(testResultLabel);
            testResultLabel.Width = testResultLabel.PreferredWidth * 11 / 10;
#endif

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

        private void MainForm_Activated(object sender, EventArgs e) {
            ExternalFuncDef.ScanScripts();
            //GraphForm.ReshowAll();
            setSubWindowTopMost(true);
            checkActiveFileChange();
        }

        private void MainForm_Deactivate(object sender, EventArgs e) {
            setSubWindowTopMost(_topMost);
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
            if (e.CloseReason == CloseReason.UserClosing && notifyIcon.Visible) {
                e.Cancel = true;
                this.Visible = false;
                GraphForm.HideAll();
                deleteOldHistories();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (!bookTreeView.ScratchPad.View.Sheet.IsEmpty) {
                saveScratchPadToHistory();
            }
            if (_activeBookItem != bookTreeView.ScratchPad && _activeBookItem.HasFileName && _activeBookItem.HasUnsavedChanges) {
                try {
                    _activeBookItem.Save();
                }
                catch (Exception ex) {
                    Console.WriteLine("Save failed: " + ex.Message);
                }
            }
            notifyIcon.Visible = false;
            disableHotkey();
            deleteOldHistories();
        }

        private void MainForm_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                var s = Settings.Instance;
                s.Window_Dpi = this.DeviceDpi;
                s.Window_Width = this.Width;
                s.Window_Height = this.Height;
            }
            if (WindowState != _lastWindowState) {
                if (WindowState == FormWindowState.Minimized) {
                    GraphForm.SetWindowStatusAll(FormWindowState.Minimized);
                }
                else {
                    GraphForm.SetWindowStatusAll(FormWindowState.Normal);
                }
            }
            _lastWindowState = WindowState;
        }

        private void MainForm_LocationChanged(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                var s = Settings.Instance;
                s.Window_Dpi = this.DeviceDpi;
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

                ReloadFontSettings();
                ReloadColorSettings();

                bookTreeView.RequestScanFiles();
            }
            catch { }
            _activeBookItem.View.RequestRecalc();
            GraphForm.ReloadSettingsAll();
        }

        public void ReloadFontSettings() {
            var s = Settings.Instance;

            this.Font = s.Appearance_Font_Button.GetFontObject();

            using (var g = this.CreateGraphics()) {
                var bottomButtonTextSize = Size.Ceiling(g.MeasureString("AAAA", this.Font));
                this.bottomPanel.Height = bottomButtonTextSize.Height;
                foreach (var c in bottomPanel.Controls) {
                    if (c is Button btn) {
                        btn.Width = bottomButtonTextSize.Width;
                    }
                }
                this.sidePaneBodyPanel.Width = bottomButtonTextSize.Width * 4;
                this.sidePaneHeaderPanel.Width = bottomButtonTextSize.Height;
                this.sidePaneOpenButton.Height = bottomButtonTextSize.Height;
            }

            foreach (var ctl in Controls) {
                if (ctl is SheetView view) {
                    view.ReloadFontSettings(s);
                }
            }
        }

        public void ReloadColorSettings() {
            var s = Settings.Instance;

            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());

            Color panelColor = ColorEx.Blend(s.Appearance_Color_Background, s.Appearance_Color_Button_Face);

            bottomPanel.BackColor = s.Appearance_Color_Background;
            bookTreeView.BackColor = s.Appearance_Color_Background;
            bookTreeView.ForeColor = s.Appearance_Color_Text;
            radixAutoButton.BackColor = s.Appearance_Color_Button_Face;
            radixDecButton.BackColor = s.Appearance_Color_Button_Face;
            radixHexButton.BackColor = s.Appearance_Color_Button_Face;
            radixBinButton.BackColor = s.Appearance_Color_Button_Face;
            radixOctButton.BackColor = s.Appearance_Color_Button_Face;
            radixSiButton.BackColor = s.Appearance_Color_Button_Face;
            radixKibiButton.BackColor = s.Appearance_Color_Button_Face;
            radixCharButton.BackColor = s.Appearance_Color_Button_Face;
            bottomPanel.BackColor = panelColor;
            sidePaneHeaderPanel.BackColor = panelColor;
            sidePaneOpenButton.BackColor = s.Appearance_Color_Button_Face;
            radixAutoButton.ForeColor = s.Appearance_Color_Text;
            radixDecButton.ForeColor = s.Appearance_Color_Text;
            radixHexButton.ForeColor = s.Appearance_Color_Text;
            radixBinButton.ForeColor = s.Appearance_Color_Text;
            radixOctButton.ForeColor = s.Appearance_Color_Text;
            radixSiButton.ForeColor = s.Appearance_Color_Text;
            radixKibiButton.ForeColor = s.Appearance_Color_Text;
            radixCharButton.ForeColor = s.Appearance_Color_Text;
            sidePaneOpenButton.ForeColor = s.Appearance_Color_Text;

            foreach (var ctl in Controls) {
                if (ctl is SheetView view) {
                    view.ReloadColorSettings(s);
                }
            }

            var iconSize = getToolIconSize();
            undoButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Undo));
            redoButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Redo));
            copyButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Copy));
            pasteButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Paste));
            insertButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Insert));
            deleteButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Delete));
            moveUpButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_MoveUp));
            moveDownButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_MoveDown));
            settingsButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Settings));
            updateTopMostButtonIcon();
            helpButton.Image = ToolIconGenerator.GenerateToolIcon(iconSize, nameof(Properties.Resources.ToolIcon_Help));

            bookTreeView.ReloadColorSettings();

            Windows.DwmApi.SetDarkModeEnable(this, s.Appearance_IsDarkTheme);
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

        private void SidePaneOpenButton_Click(object sender, EventArgs e) {
            sidePaneBodyPanel.Visible = !sidePaneBodyPanel.Visible;
            if (sidePaneBodyPanel.Visible) {
                sidePaneOpenButton.Text = "<";
                bookTreeView.Focus();
            }
            else {
                sidePaneOpenButton.Text = ">";
                refocus();
            }
        }

        private void onBookItemSelected() {
            if (bookTreeView.SelectedNode == null) return;
            if (!(bookTreeView.SelectedNode is BookItem newBookItem)) return;
            if (newBookItem == _activeBookItem) return;
            var lastBookItem = _activeBookItem;
            
            if (lastBookItem != null && lastBookItem.View != null && lastBookItem.HasFileName && lastBookItem.HasUnsavedChanges) {
                // 以前開いていたシートを保存する
                try {
                    lastBookItem.Save();
                }
                catch (Exception ex) {
                    MessageBox.Show("Failed to save sheet:\r\n\r\n" + ex.Message,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            bool requestCheckFileChange = false;
            try {
                var newView = newBookItem.View;
                var oldView = lastBookItem != null ? lastBookItem.View : null;
                if (newView == null) {
                    // シートが読み込まれてなければ読み込む
                    newBookItem.CreateView();
                    newView = newBookItem.View;
                }
                else {
                    requestCheckFileChange = true;
                }

                // SheetView コントロールをフォームに追加
                addView(newView);

                if (oldView != null) {
                    if (lastBookItem.HasFileName && !lastBookItem.IsTouched) {
                        removeView(oldView, true);
                        lastBookItem.CloseView(true);
                    }
                    else {
                        oldView.Visible = false;
                    }
                }
                _activeBookItem = newBookItem;
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to load sheet:\r\n\r\n" + ex.Message,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            var windowTitle = this.Text =
                (_activeBookItem != null ? _activeBookItem.Name : "(null)") +
                " - " + Application.ProductName +
                " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";

            if (Program.DebugMode) {
                windowTitle += " (Debug)";
            }

            this.Text = windowTitle;

            if (requestCheckFileChange) {
                checkActiveFileChange();
            }
        }

        private void addView(SheetView view) {
            view.Dock = DockStyle.Fill;
            if (!Controls.Contains(view)) {
                view.DialogOpening += SheetView_DialogOpening;
                view.DialogClosed += SheetView_DialogClosed;
                Controls.Add(view);
            }
            view.BringToFront();
            view.Visible = true;
            view.ReloadSettings(Settings.Instance);
        }

        private void removeView(SheetView view, bool saveChanges) {
            if (Controls.Contains(view)) Controls.Remove(view);
            view.DialogOpening -= SheetView_DialogOpening;
            view.DialogClosed -= SheetView_DialogClosed;
        }

        private void SheetView_DialogOpening(object sender, EventArgs e) {
            suspendTopMost();
        }

        private void SheetView_DialogClosed(object sender, EventArgs e) {
            resumeTopMost();
        }

        private void saveScratchPadToHistory() {
            var sheet = bookTreeView.ScratchPad.View.Sheet;
            if (sheet.IsEmpty) return;
            try {
                var dir = Book.HistoryDirectory;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt");
                sheet.Save(path);
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to save scratch pad: " + ex.Message);
            }
        }

        private void deleteOldHistories() {
            try {
                if (Directory.Exists(Book.HistoryDirectory)) {
                    var files = Directory.GetFiles(Book.HistoryDirectory, "*.txt");
                    foreach (var file in files) {
                        if ((DateTime.Now - new FileInfo(file).LastWriteTime).TotalDays > Settings.Instance.History_KeepPeriod) {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to delete old history: " + ex.Message);
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e) {
            suspendTopMost();
            var dlg = new SettingsDialog();
            dlg.ShowDialog();
            dlg.Dispose();
            reloadSettings();
            resumeTopMost();
        }

        private void TopMostButton_Click(object sender, EventArgs e) {
            var btn = (ToolStripButton)sender;
            setTopMost(!_topMost);
            updateTopMostButtonIcon();
        }

        private void updateTopMostButtonIcon() {
            var resName = _topMost ?
                nameof(Properties.Resources.ToolIcon_TopMostOn) :
                nameof(Properties.Resources.ToolIcon_TopMostOff);
            topMostButton.Image = ToolIconGenerator.GenerateToolIcon(getToolIconSize(), resName);
        }

        private Size getToolIconSize() {
            var width = 16 * this.DeviceDpi / 96;
            return new Size(width, width);
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
                if (notifyIcon.Visible) {
                    this.Visible = false;
                }
                else {
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            else {
                showForeground();
            }
        }
        
        private void setTopMost(bool value) {
            _topMost = value;
            setSubWindowTopMost(value);
            TopMost = value;
        }

        private void suspendTopMost() {
            if (_topMost) {
                setSubWindowTopMost(false);
                TopMost = false;
            }
        }

        private void resumeTopMost() {
            if (_topMost) {
                setSubWindowTopMost(true);
                TopMost = true;
            }
        }

        private void setSubWindowTopMost(bool topMost) {
            GraphForm.SetTopMostAll(topMost);
            ValuePickupDialog.SetTopMost(topMost);
        }

        private void showForeground() {
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized) {
                this.WindowState = FormWindowState.Normal;
            }
            Microsoft.VisualBasic.Interaction.AppActivate(this.Text);
        }

        private void checkActiveFileChange() {
            var bookItem = _activeBookItem;
            if (!bookItem.HasFileName) return;
            try {
                var lastSync = bookItem.LastSynchronized;
                var lastMod = File.GetLastWriteTime(bookItem.FilePath);
#if DEBUG
                Console.WriteLine("Last Synchronized: " + lastSync);
                Console.WriteLine("Last Modified    : " + lastMod);
#endif
                if (lastMod > lastSync) {
                    if (!bookItem.HasUnsavedChanges || DialogResult.Yes == MessageBox.Show(
                            "The sheet file '" + Path.GetFileName(bookItem.FilePath) + "' has been modified externally. Ignore local changes and reload ?",
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)) {
                        removeView(bookItem.View, false);
                        bookItem.Reload();
                        addView(bookItem.View);
                    }
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void refocus() {
            // イベントハンドラ内でコントロールにフォーカスしてもなぜか反映されないことがあるため
            // タイマで遅延させてフォーカスを実施する
            _focusTimer.Stop();
            _focusTimer.Interval = 100;
            _focusTimer.Start();
        }

        private void appExit() {
            GraphForm.CloseAll();

            // 通知アイコン使用時、Application.Exit() ではなぜか MainForm が閉じてくれないので明示的に Close する
            // タスクトレイに格納されないように通知アイコンを非表示にする
            notifyIcon.Visible = false; 
            Close();

            Application.Exit();
        }

        private void _focusTimer_Tick(object sender, EventArgs e) {
            _focusTimer.Stop();
            _activeBookItem.View.Focus();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            e.SuppressKeyPress = true;
            var repFuncs = RepresentaionFuncs.Instance;
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S) {
                if (_activeBookItem.HasFileName) {
                    _activeBookItem.Save();
                }
                else {
                    System.Media.SystemSounds.Beep.Play();
                }
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F1) {
                helpButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F5) {
                ExternalFuncDef.ScanScripts();
                _activeBookItem.View.RequestRecalc();
                bookTreeView.RequestScanFiles();
                checkActiveFileChange();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F8) {
                _activeBookItem.View.ReplaceFormatterFunction(null);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F9) {
                _activeBookItem.View.ReplaceFormatterFunction(repFuncs.dec);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F10) {
                _activeBookItem.View.ReplaceFormatterFunction(repFuncs.hex);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F11) {
                _activeBookItem.View.ReplaceFormatterFunction(repFuncs.bin);
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F12) {
                _activeBookItem.View.ReplaceFormatterFunction(repFuncs.si);
            }
            else {
                e.SuppressKeyPress = false;
            }
        }
    }
}
