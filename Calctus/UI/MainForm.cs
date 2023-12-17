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

        private FileSystemWatcher _fsWatcher = new FileSystemWatcher();
        private SheetFileNode _sideTreeNodeScratchPad;
        private TreeNode _sideTreeNodeNotebook = new TreeNode("Notebook");
        private TreeNode _sideTreeNodeHistory = new TreeNode("History");
        private ContextMenuStrip _sidePaneContextMenu = new ContextMenuStrip();
        private ToolStripMenuItem _sidePaneSaveButton = new ToolStripMenuItem("Save to Notebook");
        private ToolStripMenuItem _sidePaneRenameButton = new ToolStripMenuItem("Rename");
        private ToolStripMenuItem _sidePaneRemoveButton = new ToolStripMenuItem("Remove");

        private HotKey _hotkey = null;
        private bool _startup = true;
        private Timer _focusTimer = new Timer();
        private Timer _fileScanTimer = new Timer();
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

            _sidePaneContextMenu.Items.AddRange(new ToolStripItem[] {
                _sidePaneSaveButton, _sidePaneRenameButton, _sidePaneRemoveButton
            });

            _activeView = sheetView;
            _sideTreeNodeScratchPad = new SheetFileNode("Scratch Pad", null, sheetView);
            sidePaneBodyPanel.Visible = false;

            var sheet = new Sheet();
            sheet.Items.Add(new SheetItem());
            sheetView.Sheet = sheet;

            this.Text = Application.ProductName + " (v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
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

            setupView(sheetView);

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
            sideTreeView.Nodes.Add(_sideTreeNodeScratchPad);
            sideTreeView.Nodes.Add(_sideTreeNodeNotebook);
            sideTreeView.Nodes.Add(_sideTreeNodeHistory);
            _sideTreeNodeNotebook.Expand();
            _sideTreeNodeHistory.Expand();
            sideTreeView.LabelEdit = true;
            sideTreeView.AfterSelect += SideTreeView_AfterSelect;
            sideTreeView.BeforeLabelEdit += SideTreeView_BeforeLabelEdit;
            sideTreeView.AfterLabelEdit += SideTreeView_AfterLabelEdit;
            sideTreeView.MouseClick += SideTreeView_MouseClick;
            sideTreeView.KeyDown += SideTreeView_KeyDown;
            _sidePaneSaveButton.Click += _sidePaneSaveButton_Click;
            _sidePaneRenameButton.Click += _sidePaneRenameButton_Click;
            _sidePaneRemoveButton.Click += _sidePaneRemoveButton_Click;
            _sidePaneSaveButton.ShortcutKeys = Keys.Control | Keys.S;
            _sidePaneRenameButton.ShortcutKeys = Keys.F2;
            _sidePaneRemoveButton.ShortcutKeys = Keys.Delete;
            _fsWatcher.Changed += delegate { requestScanFiles(); };
            _fsWatcher.Created += delegate { requestScanFiles(); };
            _fsWatcher.Deleted += delegate { requestScanFiles(); };
            _fsWatcher.Renamed += delegate { requestScanFiles(); };

            settingsButton.Click += SettingsButton_Click;
            topMostButton.Click += TopMostButton_Click;
            helpButton.Click += (sender, e) => { System.Diagnostics.Process.Start(@"https://github.com/shapoco/calctus"); };

            contextOpen.Click += (sender, e) => { showForeground(); };
            contextExit.Click += (sender, e) => { appExit(); };

            sideTreeView.SelectedNode = _sideTreeNodeScratchPad;

            _focusTimer.Tick += _focusTimer_Tick;
            _fileScanTimer.Tick += _fileScanTimer_Tick;
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
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            _activeView.Save();
            notifyIcon.Visible = false;
            disableHotkey();
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

                bottomPanel.BackColor = s.Appearance_Color_Background;
                sideTreeView.BackColor = s.Appearance_Color_Background;
                sideTreeView.ForeColor = s.Appearance_Color_Text;
                radixAutoButton.BackColor = s.Appearance_Color_Button_Face;
                radixDecButton.BackColor = s.Appearance_Color_Button_Face;
                radixHexButton.BackColor = s.Appearance_Color_Button_Face;
                radixBinButton.BackColor = s.Appearance_Color_Button_Face;
                radixOctButton.BackColor = s.Appearance_Color_Button_Face;
                radixSiButton.BackColor = s.Appearance_Color_Button_Face;
                radixKibiButton.BackColor = s.Appearance_Color_Button_Face;
                radixCharButton.BackColor = s.Appearance_Color_Button_Face;
                radixAutoButton.ForeColor = s.Appearance_Color_Text;
                radixDecButton.ForeColor = s.Appearance_Color_Text;
                radixHexButton.ForeColor = s.Appearance_Color_Text;
                radixBinButton.ForeColor = s.Appearance_Color_Text;
                radixOctButton.ForeColor = s.Appearance_Color_Text;
                radixSiButton.ForeColor = s.Appearance_Color_Text;
                radixKibiButton.ForeColor = s.Appearance_Color_Text;
                radixCharButton.ForeColor = s.Appearance_Color_Text;

                sidePaneBodyPanel.Width = 200;
                _fsWatcher.Filter = "*.txt";
                _fsWatcher.Path = AppDataManager.ActiveDataPath;
                _fsWatcher.SynchronizingObject = this;
                _fsWatcher.IncludeSubdirectories = true;
                _fsWatcher.EnableRaisingEvents = true;
                requestScanFiles();

                sheetView.RelayoutText();
            }
            catch { }
            sheetView.RequestRecalc();
            GraphForm.ReloadSettingsAll();
        }

        private void setupView(SheetView view) {
            view.DialogOpening += delegate { suspendTopMost(); };
            view.DialogClosed += delegate { resumeTopMost(); };
            setViewAppearance(view);
        }

        private void setViewAppearance(SheetView view) {
            var s = Settings.Instance;
            view.Font = _sheetViewFont;
            view.BackColor = s.Appearance_Color_Background;
            view.ForeColor = s.Appearance_Color_Text;
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
                sideTreeView.Focus();
            }
            else {
                sidePaneOpenButton.Text = ">";
                refocus();
            }
        }

        private TreeNode _lastSelectedNode = null;
        private void SideTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
            if (_lastSelectedNode != null) {
                _lastSelectedNode.BackColor = Settings.Instance.Appearance_Color_Background;
                _lastSelectedNode.ForeColor = Settings.Instance.Appearance_Color_Text;
            }
            if (sideTreeView.SelectedNode == null) return;
            _lastSelectedNode = sideTreeView.SelectedNode;
            _lastSelectedNode.BackColor = SystemColors.Highlight;
            _lastSelectedNode.ForeColor = SystemColors.HighlightText;
            if (!(sideTreeView.SelectedNode is SheetFileNode node)) return;
            try {
                SheetView newView = node.View;
                if (newView == null) {
                    node.CreateView();
                    newView = node.View;
                    newView.Dock = DockStyle.Fill;
                    setupView(newView);
                    this.Controls.Add(newView);
                    newView.BringToFront();
                }
                if (_activeView != newView) {
                    var oldView = _activeView;
                    _activeView = newView;
                    newView.Visible = true;
                    oldView.Visible = false;
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to load sheet:\r\n" + ex.Message);
            }
        }

        private void SideTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) {
            e.CancelEdit =
                (sideTreeView.SelectedNode == null) ||
                !(sideTreeView.SelectedNode is SheetFileNode) ||
                (sideTreeView.SelectedNode == _sideTreeNodeScratchPad);
        }

        private void SideTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (e.CancelEdit) return;
            if (!(e.Node is SheetFileNode node)) {
                e.CancelEdit = true;
                return;
            }

            if (string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
                return;
            }
            else if (e.Label.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                e.CancelEdit = true;
                MessageBox.Show("Only valid characters as filenames can be used.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var oldPath = node.FilePath;
            var newPath = Path.Combine(Path.GetDirectoryName(oldPath), e.Label + ".txt");
            try {
                File.Move(oldPath, newPath);
                node.FilePath = newPath;
            }
            catch (Exception ex) {
                node.FilePath = oldPath;
                e.CancelEdit = true;
                MessageBox.Show("Failed to rename:\r\n\r\n" + ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SideTreeView_MouseClick(object sender, MouseEventArgs e) {
            var clickedNode = sideTreeView.GetNodeAt(e.X, e.Y);
            if (clickedNode != null) {
                sideTreeView.SelectedNode = clickedNode;
                if (e.Button == MouseButtons.Right) {
                    showSidePaneContextMenu(sideTreeView.PointToScreen(e.Location));
                }
            }
        }

        private void SideTreeView_KeyDown(object sender, KeyEventArgs e) {
            e.SuppressKeyPress = true;
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S) {
                _sidePaneSaveButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F2) {
                _sidePaneRenameButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Delete) {
                _sidePaneRemoveButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Apps) {
                var node = sideTreeView.SelectedNode;
                if (node == null) return;
                showSidePaneContextMenu(sideTreeView.PointToScreen(new Point(node.Bounds.Left, node.Bounds.Bottom)));
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space) {
                var node = sideTreeView.SelectedNode;
                if (node == null || node.Nodes.Count == 0) return;
                if (node.IsExpanded) {
                    node.Collapse();
                }
                else {
                    node.Expand();
                }
            }
            else {
                e.SuppressKeyPress = false;
            }
        }

        private void showSidePaneContextMenu(Point p) {
            if (sideTreeView.SelectedNode == null) return;
            if (!(sideTreeView.SelectedNode is SheetFileNode node)) return;
            _sidePaneRemoveButton.Enabled = (node != _sideTreeNodeScratchPad);
            _sidePaneRenameButton.Enabled = (node != _sideTreeNodeScratchPad);
            _sidePaneContextMenu.Show(p);
        }

        private void _sidePaneSaveButton_Click(object sender, EventArgs e) {
            if (sideTreeView.SelectedNode == null) return;
            if (!(sideTreeView.SelectedNode is SheetFileNode node)) return;

            try {
                var nameBase = node.Name != null ? node.Name : DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"); ;
                var newPath = Path.Combine(SheetView.NotebookDirectory, nameBase + ".txt");
                var suffixNumber = 1;
                if (File.Exists(newPath)) {
                    do {
                        newPath = Path.Combine(SheetView.NotebookDirectory, nameBase + "(" + suffixNumber + ").txt");
                        suffixNumber++;
                    } while (File.Exists(newPath));
                }
                node.View.Sheet.Save(newPath);
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to save file:\r\n\r\n" + ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _sidePaneRenameButton_Click(object sender, EventArgs e) {
            if (sideTreeView.SelectedNode == null) return;
            if (!(sideTreeView.SelectedNode is SheetFileNode node)) return;
            if (node == _sideTreeNodeScratchPad) return;
            node.BeginEdit();
        }

        private void _sidePaneRemoveButton_Click(object sender, EventArgs e) {
            if (sideTreeView.SelectedNode == null) return;
            if (!(sideTreeView.SelectedNode is SheetFileNode node)) return;
            if (node == _sideTreeNodeScratchPad) return;

            if (DialogResult.OK != MessageBox.Show("Are you sure you want to delete this file?:\r\n\r\n" + node.FilePath, Application.ProductName,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)) {
                return;
            }

            try {
                File.Delete(node.FilePath);
                node.Remove();
                sideTreeView.SelectedNode = _sideTreeNodeScratchPad;
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to delete file:\r\n\r\n" + ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void requestScanFiles() {
#if DEBUG
            Console.WriteLine("requestScanFiles()");
#endif
            _fileScanTimer.Stop();
            _fileScanTimer.Interval = 100;
            _fileScanTimer.Start();
        }

        private void _fileScanTimer_Tick(object sender, EventArgs e) {
            _fileScanTimer.Stop();
            try {
                if (Directory.Exists(SheetView.NotebookDirectory)) {
                    scanFiles(_sideTreeNodeNotebook, SheetView.NotebookDirectory);
                }
                if (Directory.Exists(SheetView.HistoryDirectory)) {
                    scanFiles(_sideTreeNodeHistory, SheetView.HistoryDirectory);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("File scan failed: " + ex.Message);
            }
        }

        private void scanFiles(TreeNode parentNode, string dirPath) {
#if DEBUG
            Console.WriteLine("Scanning directory: '" + dirPath + "'");
#endif
            var existingFiles = Directory.GetFiles(dirPath, "*.txt").ToList();
            var loadedNodes = new List<SheetFileNode>();
            foreach (var node in parentNode.Nodes) {
                loadedNodes.Add((SheetFileNode)node);
            }
            foreach (var existingPath in existingFiles) {
                var node = loadedNodes.FirstOrDefault(p => p.FilePath == existingPath);
                if (node != null) {
                    loadedNodes.Remove(node);
                }
                else {
                    parentNode.Nodes.Add(new SheetFileNode(Path.GetFileNameWithoutExtension(existingPath), existingPath, null));
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
