using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI.Books {
    class BookTreeView : TreeView {
        private static ImageList IconImageList = null;

        public readonly BookItem ScratchPad;
        public readonly Book Notebook;
        public readonly Book History;
        public readonly Book Samples;

        private FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
        private ContextMenuStrip _sidePaneContextMenu = new ContextMenuStrip();
        private ToolStripMenuItem _copyToNotebookButton = new ToolStripMenuItem("Copy to Notebook");
        private ToolStripMenuItem _renameButton = new ToolStripMenuItem("Rename");
        private ToolStripMenuItem _removeButton = new ToolStripMenuItem("Remove");
        private ToolStripMenuItem _openWithAppButton = new ToolStripMenuItem("Open with External Application");

        private Timer _fileScanTimer = new Timer();
        private bool _suppressSelectedChange = false;

        public BookTreeView() {
            if (DesignMode) return;

            if (IconImageList == null) { 
                var scaleFactor = this.DeviceDpi / 96;
                IconImageList = new ImageList();
                IconImageList.ColorDepth = ColorDepth.Depth32Bit;
                IconImageList.ImageSize = new Size(16 * scaleFactor, 16 * scaleFactor);
                IconImageList.Images.Add(Properties.Resources.ToolIcon_Folder);
                IconImageList.Images.Add(Properties.Resources.ToolIcon_Sheet_Close);
                IconImageList.Images.Add(Properties.Resources.ToolIcon_Sheet_Open);
            }
            this.ImageList = IconImageList;

            LabelEdit = true;
            var scratchPadView = new SheetView();
            scratchPadView.Sheet = new Sheet();
            ScratchPad = new BookItem("Scratch Pad", null, scratchPadView);
            Notebook = new Book("Notebook", Book.NotebookFolderName, SortMode.ByName);
            History = new Book("History", Book.HistoryFolderName, SortMode.ByLastModified);
            this.Nodes.Add(ScratchPad);
            this.Nodes.Add(Notebook);
            this.Nodes.Add(History);
            Notebook.Expand();
            History.Expand();
            if (SampleBook.FindSampleFolder(out _)) {
                Samples = new SampleBook();
                this.Nodes.Add(Samples);
                Samples.Expand();
                Console.WriteLine("Sample book directory: '" + Samples.DirectoryPath + "'");
            }
            _sidePaneContextMenu.Items.AddRange(new ToolStripItem[] {
                _copyToNotebookButton,
                _renameButton,
                _removeButton,
                new ToolStripSeparator(),
                _openWithAppButton
            });

            _copyToNotebookButton.Click += _copyToNotebookButton_Click;
            _renameButton.Click += _renameButton_Click;
            _removeButton.Click += _removeButton_Click;
            _openWithAppButton.Click += _openWithAppButton_Click;
            _copyToNotebookButton.ShortcutKeys = Keys.Control | Keys.S;
            _renameButton.ShortcutKeys = Keys.F2;
            _removeButton.ShortcutKeys = Keys.Delete;
            _fileSystemWatcher.Changed += delegate { RequestScanFiles(); };
            _fileSystemWatcher.Created += delegate { RequestScanFiles(); };
            _fileSystemWatcher.Deleted += delegate { RequestScanFiles(); };
            _fileSystemWatcher.Renamed += delegate { RequestScanFiles(); };

            _fileScanTimer.Tick += _fileScanTimer_Tick;
        }

        public void ReloadSettings() {
            _fileSystemWatcher.Filter = "*.txt";
            _fileSystemWatcher.Path = AppDataManager.ActiveDataPath;
            _fileSystemWatcher.SynchronizingObject = this;
            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.EnableRaisingEvents = true;
            RequestScanFiles();
            foreach(var node in Nodes) {
                updateItemColor((TreeNode)node, true);
            }
        }

        private TreeNode _lastSelectedNode = null;
        protected override void OnAfterSelect(TreeViewEventArgs e) {
            if (_suppressSelectedChange) return;
            base.OnAfterSelect(e);
            updateItemColor(_lastSelectedNode, false);
            _lastSelectedNode = SelectedNode;
            updateItemColor(_lastSelectedNode, false);
        }

        private void updateItemColor(TreeNode node, bool recursive) {
            if (node == null) return;
            if (node.IsSelected) {
                node.BackColor = SystemColors.Highlight;
                node.ForeColor = SystemColors.HighlightText;
            }
            else {
                node.BackColor = Settings.Instance.Appearance_Color_Background;
                node.ForeColor = Settings.Instance.Appearance_Color_Text;
            }
            if (recursive) {
                foreach (var child in node.Nodes) {
                    updateItemColor((TreeNode)child, recursive);
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            var clickedNode = GetNodeAt(e.X, e.Y);
            if (clickedNode != null) {
                SelectedNode = clickedNode;
                if (e.Button == MouseButtons.Right) {
                    showSidePaneContextMenu(PointToScreen(e.Location));
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            e.SuppressKeyPress = true;
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S) {
                _copyToNotebookButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F2) {
                _renameButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Delete) {
                _removeButton.PerformClick();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Apps) {
                var node = SelectedNode;
                if (node == null) return;
                showSidePaneContextMenu(PointToScreen(new Point(node.Bounds.Left, node.Bounds.Bottom)));
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space) {
                var node = SelectedNode;
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

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e) {
            base.OnBeforeLabelEdit(e);
            e.CancelEdit = (SelectedNode == null) || !(SelectedNode is BookItem item) || !item.HasFileName;
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e) {
            base.OnAfterLabelEdit(e);

            if (e.CancelEdit) return;
            if (!(e.Node is BookItem node)) {
                e.CancelEdit = true;
                return;
            }
            if (string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
                return;
            }
            if (e.Label.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                e.CancelEdit = true;
                MessageBox.Show("Only valid characters as filenames can be used.", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                var oldPath = node.FilePath;
                var newPath = Path.Combine(node.DirectoryPath, e.Label + ".txt");
                File.Move(oldPath, newPath);
                node.Name = e.Label;
                node.FileName = e.Label + ".txt";
            }
            catch (Exception ex) {
                e.CancelEdit = true;
                MessageBox.Show("Failed to rename:\r\n\r\n" + ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showSidePaneContextMenu(Point p) {
            var selectedNode = SelectedNode;
            if (selectedNode == null) return;
            if (selectedNode is BookItem bookItem) {
                _copyToNotebookButton.Enabled = true;
                _renameButton.Enabled = _removeButton.Enabled = _openWithAppButton.Enabled = bookItem.HasFileName;
                _sidePaneContextMenu.Show(p);
            }
            else if (selectedNode is Book book) {
                _copyToNotebookButton.Enabled = _renameButton.Enabled = _removeButton.Enabled = false;
                _openWithAppButton.Enabled = true;
                _sidePaneContextMenu.Show(p);
            }
        }

        private void _copyToNotebookButton_Click(object sender, EventArgs e) {
            var selectedNode = SelectedNode;
            if (selectedNode == null || !(selectedNode is BookItem bookItem)) return;

            try {
                var nameBase = bookItem.HasFileName ? bookItem.Name : DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"); ;
                var newPath = Path.Combine(Notebook.DirectoryPath, nameBase + ".txt");
                var suffixNumber = 1;
                if (File.Exists(newPath)) {
                    do {
                        newPath = Path.Combine(Notebook.DirectoryPath, nameBase + "(" + suffixNumber + ").txt");
                        suffixNumber++;
                    } while (File.Exists(newPath));
                }
                bookItem.View.Sheet.Save(newPath);
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to save file:\r\n\r\n" + ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _renameButton_Click(object sender, EventArgs e) {
            var selectedNode = SelectedNode;
            if (selectedNode == null || !(selectedNode is BookItem bookItem) || !bookItem.HasFileName) return;
            bookItem.BeginEdit();
        }

        private void _removeButton_Click(object sender, EventArgs e) {
            var selectedNode = SelectedNode;
            if (selectedNode == null || !(selectedNode is BookItem bookItem) || !bookItem.HasFileName) return;

            if (DialogResult.OK != MessageBox.Show("Are you sure you want to delete this file?:\r\n\r\n" + bookItem.FileName, Application.ProductName,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)) {
                return;
            }

            try {
                File.Delete(bookItem.FilePath);
                bookItem.Remove();
                SelectedNode = ScratchPad;
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to delete file:\r\n\r\n" + ex.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _openWithAppButton_Click(object sender, EventArgs e) {
            var selectedNode = SelectedNode;
            if (selectedNode == null) return;
            if (selectedNode is BookItem node) {
                try {
                    System.Diagnostics.Process.Start(node.FilePath);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (selectedNode is Book book) {
                try {
                    System.Diagnostics.Process.Start(book.DirectoryPath);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void RequestScanFiles() {
#if DEBUG
            Console.WriteLine("File scan requested.");
#endif
            _fileScanTimer.Stop();
            _fileScanTimer.Interval = 500;
            _fileScanTimer.Start();
        }

        private void _fileScanTimer_Tick(object sender, EventArgs e) {
            _fileScanTimer.Stop();
            _suppressSelectedChange = true;
            var selectedNode = SelectedNode;
            try {
                Notebook.ScanFiles();
                History.ScanFiles();
                Samples.ScanFiles();
            }
            catch (Exception ex) {
                Console.WriteLine("File scan failed: " + ex.Message);
            }
            _suppressSelectedChange = false;
            if (selectedNode != null && selectedNode.TreeView == null) {
                // 更新前に選択されていたファイルを見失ったら ScratchPad を選択し直す
                SelectedNode = ScratchPad;
            }
        }
    }
}
