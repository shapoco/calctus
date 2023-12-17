using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Shapoco.Calctus.UI.Sheets;

namespace Shapoco.Calctus.UI {
    class SheetFileNode : TreeNode {
        private string _name;
        private string _path = null;
        private SheetView _view = null;

        public string FilePath {
            get => _path;
            set {
                if (value == _path) return;
                _path = value;
                if (_path != null) {
                    _name = Path.GetFileNameWithoutExtension(_path);
                }
                updateText();
            }
        }

        public SheetView View {
            get => _view;
            private set {
                if (value == _view) return;
                if (_view != null) {
                    _view.IsChangedChanged -= _view_IsChangedChanged;
                }
                _view = value;
                if (_view != null) {
                    _view.IsChangedChanged += _view_IsChangedChanged;
                }
            }
        }

        public SheetFileNode(string name, string path, SheetView view) : base(name) {
            _name = name;
            _path = path;
            View = view;
        }

        public void CreateView() {
            if (_view != null) return;
            var view = new SheetView();
            view.Load(_path);
            this.View = view;
        }

        private void _view_IsChangedChanged(object sender, EventArgs e) {
            updateText();
        }

        private void updateText() {
            if (_view.IsChanged) {
                this.Text = _name + " *";
            }
            else {
                this.Text = _name;
            }
        }

    }
}
