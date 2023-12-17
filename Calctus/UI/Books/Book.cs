using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI.Books {
    class Book : TreeNode {
#if DEBUG
        public const string HistoryFolderName = "history.Debug";
        public const string NotebookFolderName = "notebook.Debug";
#else
        public const string HistoryFolderName = "history";
        public const string NotebookFolderName = "notebook";
#endif
        public static string HistoryDirectory = Path.Combine(AppDataManager.ActiveDataPath, HistoryFolderName);
        public static string NotebookDirectory = Path.Combine(AppDataManager.ActiveDataPath, NotebookFolderName);

        public readonly string FolderName;
        public readonly SortMode SortMode;

        public Book(string displayName, string folderName, SortMode sortMode) : base(displayName) {
            this.FolderName = folderName;
            this.SortMode = sortMode;
            ImageIndex = 0;
            SelectedImageIndex = 0;
        }

        public new Book Parent => (Book)base.Parent;

        public string DirectoryPath => (Parent == null) ?
            Path.Combine(AppDataManager.ActiveDataPath, FolderName) :
            Path.Combine(Parent.DirectoryPath, FolderName);

        public void ScanFiles() {
            var dirPath = DirectoryPath;
            if (!Directory.Exists(dirPath)) {
#if DEBUG
                Console.WriteLine("Directory not found: '" + dirPath + "'");
#endif
                return;
            }
#if DEBUG
            Console.WriteLine("Scanning directory: '" + dirPath + "'");
#endif
            var existingFiles = Directory.GetFiles(dirPath, "*.txt").ToList();
            var tempNodes = new List<BookItem>();
            var deletedNodes = new List<BookItem>();
            foreach (var node in this.Nodes) {
                deletedNodes.Add((BookItem)node);
                tempNodes.Add((BookItem)node);
            }
            foreach (var filePath in existingFiles) {
                var filename = Path.GetFileName(filePath);
                var node = deletedNodes.FirstOrDefault(p => p.FileName == filename);
                if (node != null) {
                    deletedNodes.Remove(node);
                }
                else {
                    tempNodes.Add(new BookItem(Path.GetFileNameWithoutExtension(filePath), filename, null));
                }
            }
            foreach (var node in deletedNodes) {
                tempNodes.Remove(node);
                node.View?.Dispose();
            }

            TreeNode selectedNode = null;
            if (TreeView != null) selectedNode = TreeView.SelectedNode;
            Nodes.Clear();
            switch (SortMode) {
                case SortMode.ByName: Nodes.AddRange(tempNodes.OrderBy(p => p.Name).ToArray()); break;
                case SortMode.ByLastModified: Nodes.AddRange(tempNodes.OrderByDescending(p => p.LastModified).ToArray()); break;
                default: throw new NotImplementedException();
            }
            if (TreeView != null && tempNodes.Contains(selectedNode)) TreeView.SelectedNode = selectedNode;
        }
    }
}
