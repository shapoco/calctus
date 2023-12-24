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
        private bool _isChanged = false;
        private bool _isTouched = false;

        public BookItem(string name, string fileName, SheetView view) : base(name) {
            Name = name;
            _fileName = fileName;
            View = view;
            SelectedImageIndex = ImageIndex = view != null ? 2 : 1;
            try {
                if (HasFileName && File.Exists(FullPath)) {
                    LastModified = new FileInfo(FullPath).LastWriteTime;
                }
            }
            catch { }
        }

        public bool IsChanged => _isChanged;
        public bool IsTouched => _isTouched;
        public DateTime LastModified { get; private set; } = DateTime.Now;

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
            SelectedImageIndex = ImageIndex = 2;
        }

        public void CloseView() {
            if (_view == null) return;
            if (_isChanged) Save();
            View.Dispose();
            this.View = null;
            _isTouched = false;
            SelectedImageIndex = ImageIndex = 1;
        }

        private void _view_Changed(object sender, EventArgs e) {
            _isChanged = true;
            _isTouched = true;
            LastModified = DateTime.Now;
        }

        public void Save() {
            if (_view != null && HasFileName) {
                View.Sheet.Save(FilePath);
                _isChanged = false;
            }
        }
    }
}
