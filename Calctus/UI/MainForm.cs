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
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.UI.Books;
using Shapoco.Calctus.UI.Sheets;
using Shapoco.Calctus.Model.Functions;

namespace Shapoco.Calctus.UI {
    internal partial class MainForm : Form {
        private static MainForm _instance = null;
        public static MainForm Instance => _instance;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private Font _sheetViewFont = null;
        private SheetView _activeView = null;

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
            _startupWindowPos = new Point(s.Window_X, s.Window_Y);
            _startupWindowSize = new Size(s.Window_Width, s.Window_Height);

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

#if DEBUG
            notifyIcon.Text = Application.ProductName + " (Debug)";
#else
            notifyIcon.Text = Application.ProductName;
#endif
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            radixAutoButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(null); _activeView.Focus(); }; 
            radixDecButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.dec); _activeView.Focus(); };
            radixHexButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.hex); _activeView.Focus(); };
            radixBinButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.bin); _activeView.Focus(); };
            radixOctButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.oct); _activeView.Focus(); };
            radixSiButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.si); _activeView.Focus(); };
            radixKibiButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.kibi); _activeView.Focus(); };
            radixCharButton.Click += (sender, e) => { _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.char_1); _activeView.Focus(); };

            toolTip.SetToolTip(radixAutoButton, "Automatic (F8)");
            toolTip.SetToolTip(radixDecButton, "Decimal (F9)");
            toolTip.SetToolTip(radixHexButton, "Hexadecimal (F10)");
            toolTip.SetToolTip(radixBinButton, "Binary (F11)");
            toolTip.SetToolTip(radixOctButton, "Octal");
            toolTip.SetToolTip(radixSiButton, "SI Prefix (F12)");
            toolTip.SetToolTip(radixKibiButton, "Binary Prefix");
            toolTip.SetToolTip(radixCharButton, "Character");

            undoButton.Click += (sender, e) => { _activeView.Undo(); };
            redoButton.Click += (sender, e) => { _activeView.Redo(); };
            copyButton.Click += (sender, e) => { _activeView.Copy(); };
            pasteButton.Click += (sender, e) => { _activeView.Paste(); };
            insertButton.Click += (sender, e) => { _activeView.ItemInsert(); };
            deleteButton.Click += (sender, e) => { _activeView.ItemDelete(); };
            moveUpButton.Click += (sender, e) => { _activeView.ItemMoveUp(); };
            moveDownButton.Click += (sender, e) => { _activeView.ItemMoveDown(); };

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
            GraphForm.SetTopMostAll(true);
        }

        private void MainForm_Deactivate(object sender, EventArgs e) {
            GraphForm.SetTopMostAll(_topMost);
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
            notifyIcon.Visible = false;
            disableHotkey();
            deleteOldHistories();
        }

        private void MainForm_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                var s = Settings.Instance;
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
                this.Font = font_ui_normal;
                _sheetViewFont = new Font(s.Appearance_Font_Expr_Name, s.Appearance_Font_Size * font_large_coeff, font_style);

                ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());

                foreach(var ctl in Controls) {
                    if (ctl is SheetView view) {
                        setViewAppearance(view);
                    }
                }

                Color panelColor = ColorUtils.Blend(s.Appearance_Color_Background, s.Appearance_Color_Button_Face);

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

                sidePaneBodyPanel.Width = 200;

                bookTreeView.ReloadSettings();
            }
            catch { }
            _activeView.RequestRecalc();
            GraphForm.ReloadSettingsAll();
        }

        private void setViewAppearance(SheetView view) {
            var s = Settings.Instance;
            view.Font = _sheetViewFont;
            view.BackColor = s.Appearance_Color_Background;
            view.ForeColor = s.Appearance_Color_Text;
            view.RelayoutText();
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

        private BookItem _lastSheetNode = null;
        private void onBookItemSelected() {
            if (bookTreeView.SelectedNode == null) return;
            if (!(bookTreeView.SelectedNode is BookItem newNode)) return;
            try {
                if (_lastSheetNode != null && _lastSheetNode.View != null && _lastSheetNode.HasFileName && _lastSheetNode.IsChanged) {
                    _lastSheetNode.Save();
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to save sheet:\r\n\r\n" + ex.Message,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try {
                SheetView newView = newNode.View;
                if (newView == null) {
                    newNode.CreateView();
                    newView = newNode.View;
                }
                if (!this.Controls.Contains(newView)) {
                    newView.Dock = DockStyle.Fill;
                    newView.DialogOpening += delegate { suspendTopMost(); };
                    newView.DialogClosed += delegate { resumeTopMost(); };
                    this.Controls.Add(newView);
                    setViewAppearance(newView);
                }
                newView.BringToFront();
                if (_activeView != newView) {
                    var oldView = _activeView;
                    _activeView = newView;
                    newView.Visible = true;
                    if (oldView != null) {
                        oldView.Visible = false;
                    }
                }
                _lastSheetNode = newNode;
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to load sheet:\r\n\r\n" + ex.Message,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Text =
                Application.ProductName +
                " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ") - " +
                (_lastSheetNode != null ? _lastSheetNode.Name : "(null)");
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
                    foreach(var file in files) {
                        if ((DateTime.Now - new FileInfo(file).LastWriteTime).TotalDays > Settings.Instance.History_KeepPeriod) {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch(Exception ex) {
#if DEBUG
                Console.WriteLine("Failed to delete old history: " + ex.Message);
#endif
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
            if (_topMost) {
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
            GraphForm.SetTopMostAll(value);
            TopMost = value;
        }

        private void suspendTopMost() {
            if (_topMost) {
                GraphForm.SetTopMostAll(false);
                TopMost = false;
            }
        }

        private void resumeTopMost() {
            if (_topMost) {
                GraphForm.SetTopMostAll(true);
                TopMost = true;
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
            _activeView.Focus();
        }

        private void scanFiles(TreeNode parentNode, string folderName) {
            var dirPath = Path.Combine(AppDataManager.ActiveDataPath, folderName);
#if DEBUG
            Console.WriteLine("Scanning directory: '" + dirPath + "'");
#endif
            var existingFiles = Directory.GetFiles(dirPath, "*.txt").ToList();
            var loadedNodes = new List<BookItem>();
            foreach (var node in parentNode.Nodes) {
                loadedNodes.Add((BookItem)node);
            }
            foreach (var existingPath in existingFiles) {
                var relPath = Path.Combine(folderName, Path.GetFileName(existingPath));
                var node = loadedNodes.FirstOrDefault(p => p.FileName == relPath);
                if (node != null) {
                    loadedNodes.Remove(node);
                }
                else {
                    parentNode.Nodes.Add(new BookItem(Path.GetFileNameWithoutExtension(existingPath), relPath, null));
                }
            }
            foreach (var node in loadedNodes) {
                parentNode.Nodes.Remove(node);
                node.View?.Dispose();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.F1) {
                helpButton.PerformClick();
            }
            else if (e.KeyCode == Keys.F5) {
                ExternalFuncDef.ScanScripts();
                _activeView.RequestRecalc();
            }
            else if (e.KeyCode == Keys.F8) {
                _activeView.ReplaceFormatterFunction(null);
            }
            else if (e.KeyCode == Keys.F9) {
                _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.dec);
            }
            else if (e.KeyCode == Keys.F10) {
                _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.hex);
            }
            else if (e.KeyCode == Keys.F11) {
                _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.bin);
            }
            else if (e.KeyCode == Keys.F12) {
                _activeView.ReplaceFormatterFunction(EmbeddedFuncDef.si);
            }
            else {
                e.SuppressKeyPress = false;
            }
        }
    }
}
