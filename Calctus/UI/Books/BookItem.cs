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
    class BookItem : TreeNode {
        private string _fileName = null;
        private SheetView _view = null;
        private bool _hasUnsavedChanges = false;
        private bool _isTouched = false;

        public BookItem(string name, string filePath, SheetView view) : base(name) {
            Name = name;
            _fileName = filePath == null ? null : Path.GetFileName(filePath);
            View = view;
            SelectedImageIndex = ImageIndex = view != null ? 2 : 1;
            try {
                if (HasFileName && File.Exists(filePath)) {
                    LastModified = File.GetLastWriteTime(filePath);
                }
            }
            catch { }
            if (View != null) {
                LastSynchronized = DateTime.Now;
            }
        }

        public bool HasUnsavedChanges => _hasUnsavedChanges;
        public bool IsTouched => _isTouched;
        public DateTime LastModified { get; private set; } = DateTime.Now;
        public DateTime LastSynchronized { get; private set; } = DateTime.MinValue;

        public string FileName {
            get => _fileName;
            set {
                if (value == _fileName) return;
                _fileName = value;
                if (_fileName != null) {
                    Name = Path.GetFileNameWithoutExtension(_fileName);
                }
                this.Text = Name;
            }
        }

        public bool HasFileName => !string.IsNullOrEmpty(FileName);

        public new Book Parent => (Book)base.Parent;

        public string DirectoryPath => (Parent == null) ?
            AppDataManager.ActiveDataPath : Parent.DirectoryPath;

        public string FilePath => HasFileName ?
             Path.Combine(DirectoryPath, FileName) : null;

        public SheetView View {
            get => _view;
            private set {
                if (value == _view) return;
                if (_view != null) {
                    _view.Changed -= _view_Changed;
                }
                _view = value;
                if (_view != null) {
                    _view.Changed += _view_Changed;
                }
            }
        }

        public void CreateView() {
            if (_view != null) return;
            var view = new SheetView();
            view.Sheet = new Sheet(FilePath);
            this.View = view;
            LastSynchronized = DateTime.Now;
            SelectedImageIndex = ImageIndex = 2;
        }

        public void Reload() {
            if (_view == null) return;
            CloseView(false);
            CreateView();
        }

        public void CloseView(bool saveChange) {
            if (_view == null) return;
            if (saveChange && _hasUnsavedChanges) Save();
            View.Dispose();
            this.View = null;
            _isTouched = false;
            SelectedImageIndex = ImageIndex = 1;
        }

        private void _view_Changed(object sender, EventArgs e) {
            _hasUnsavedChanges = true;
            _isTouched = true;
            LastModified = DateTime.Now;
        }

        public void Save() {
            if (_view != null && HasFileName) {
                View.Sheet.Save(FilePath);
                LastSynchronized = DateTime.Now;
                _hasUnsavedChanges = false;
            }
        }
    }
}
